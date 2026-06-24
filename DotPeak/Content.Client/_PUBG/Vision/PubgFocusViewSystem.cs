// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Vision.PubgFocusViewSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Input;
using Content.Shared._PUBG.Loadout;
using Content.Shared._PUBG.Vision;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Camera;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Vision;

public sealed class PubgFocusViewSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private PubgWeaponModulesSystem _modules;
  [Dependency]
  private VehicleWeaponsSystem _vehicleWeapons;
  private EntityUid? _activeWeapon;
  private readonly Dictionary<EntityUid, Vector2> _currentOffsets = new Dictionary<EntityUid, Vector2>();
  private const float SmoothSpeed = 12f;
  private const float EdgeOffset = 0.9f;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(PubgKeyFunctions.PubgFocusView, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__11_0)), new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__11_1)), true, true)).Register<PubgFocusViewSystem>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PubgFocusViewComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>(new EntityEventRefHandler<PubgFocusViewComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>((object) this, __methodptr(OnGetEyeOffset)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleWeaponsOperatorComponent, GetEyeOffsetEvent>(new EntityEventRefHandler<VehicleWeaponsOperatorComponent, GetEyeOffsetEvent>((object) this, __methodptr(OnOperatorGetEyeOffset)), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<PubgFocusViewSystem>();
  }

  public MapCoordinates AdjustMapCoordinates(EntityUid weapon, MapCoordinates coordinates)
  {
    PubgFocusViewComponent focusViewComponent;
    Vector2 offset;
    return MapId.op_Equality(coordinates.MapId, MapId.Nullspace) || this.TryComp<PubgFocusViewComponent>(weapon, ref focusViewComponent) && !focusViewComponent.AdjustShotCoordinates || !this.TryGetActiveOffset(weapon, out offset) || offset == Vector2.Zero ? coordinates : new MapCoordinates(coordinates.Position + offset, coordinates.MapId);
  }

  private void SetActive(bool active)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid? nullable = new EntityUid?();
    PubgFocusViewComponent focus = (PubgFocusViewComponent) null;
    if (active)
    {
      EntityUid weapon;
      if (!this.TryGetFocusWeapon(localEntity.Value, out weapon, out focus))
        return;
      Vector2? offsetFromCursor = this.ComputeOffsetFromCursor(focus.OffsetTiles + this._modules.GetFocusBonusTiles(weapon), focus.FixedOffset);
      if (!offsetFromCursor.HasValue)
        return;
      nullable = new EntityUid?(weapon);
      this._activeWeapon = new EntityUid?(weapon);
      this._currentOffsets[weapon] = offsetFromCursor.Value;
    }
    else
    {
      if (!this._activeWeapon.HasValue)
        return;
      if (!this.Exists(this._activeWeapon))
      {
        this._currentOffsets.Remove(this._activeWeapon.Value);
        this._activeWeapon = new EntityUid?();
        return;
      }
      nullable = this._activeWeapon;
      if (!this.TryComp<PubgFocusViewComponent>(nullable.Value, ref focus))
        return;
    }
    if (focus.Active == active)
      return;
    focus.Active = active;
    this.RaisePredictiveEvent<PubgFocusViewStateEvent>(new PubgFocusViewStateEvent(this.GetNetEntity(nullable.Value, (MetaDataComponent) null), active));
    if (!active)
      this._activeWeapon = new EntityUid?();
    if (active)
      return;
    this._currentOffsets.Remove(nullable.Value);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    if (!this._activeWeapon.HasValue)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid weapon;
    PubgFocusViewComponent focus;
    if (this.TryGetFocusWeapon(localEntity.Value, out weapon, out focus))
    {
      EntityUid entityUid = weapon;
      EntityUid? activeWeapon = this._activeWeapon;
      if ((activeWeapon.HasValue ? (EntityUid.op_Inequality(entityUid, activeWeapon.GetValueOrDefault()) ? 1 : 0) : 1) == 0)
      {
        Vector2? offsetFromCursor = this.ComputeOffsetFromCursor(focus.OffsetTiles + this._modules.GetFocusBonusTiles(this._activeWeapon.Value), focus.FixedOffset);
        Vector2 vector2;
        if (!offsetFromCursor.HasValue || !this._currentOffsets.TryGetValue(this._activeWeapon.Value, out vector2))
          return;
        float amount = Math.Clamp(12f * frameTime, 0.0f, 1f);
        this._currentOffsets[this._activeWeapon.Value] = Vector2.Lerp(vector2, offsetFromCursor.Value, amount);
        return;
      }
    }
    this.SetActive(false);
  }

  private void OnGetEyeOffset(
    Entity<PubgFocusViewComponent> ent,
    ref HeldRelayedEvent<GetEyeOffsetRelayedEvent> args)
  {
    Vector2 offset;
    if (!this.TryGetActiveOffset(ent.Owner, out offset))
      return;
    args.Args.Offset += offset;
  }

  private void OnOperatorGetEyeOffset(
    Entity<VehicleWeaponsOperatorComponent> ent,
    ref GetEyeOffsetEvent args)
  {
    EntityUid? vehicle = ent.Comp.Vehicle;
    EntityUid weapon;
    Vector2 offset;
    if (!vehicle.HasValue || !this._vehicleWeapons.TryGetSelectedWeaponForOperator(vehicle.GetValueOrDefault(), ent.Owner, out weapon) || !this.TryGetActiveOffset(weapon, out offset))
      return;
    args.Offset += offset;
  }

  private bool TryGetFocusWeapon(
    EntityUid player,
    out EntityUid weapon,
    [NotNullWhen(true)] out PubgFocusViewComponent? focus)
  {
    weapon = new EntityUid();
    focus = (PubgFocusViewComponent) null;
    VehicleWeaponsOperatorComponent operatorComponent;
    if (this.TryComp<VehicleWeaponsOperatorComponent>(player, ref operatorComponent))
    {
      EntityUid? vehicle = operatorComponent.Vehicle;
      EntityUid weapon1;
      if (vehicle.HasValue && this._vehicleWeapons.TryGetSelectedWeaponForOperator(vehicle.GetValueOrDefault(), player, out weapon1) && this.TryComp<PubgFocusViewComponent>(weapon1, ref focus))
      {
        weapon = weapon1;
        return true;
      }
    }
    EntityUid? nullable;
    if (!this._hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((player, (HandsComponent) null)), out nullable) || !this.TryComp<PubgFocusViewComponent>(nullable.Value, ref focus))
      return false;
    weapon = nullable.Value;
    return true;
  }

  private Vector2? ComputeOffsetFromCursor(float maxOffset, bool fixedOffset)
  {
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    if (WindowId.op_Equality(mouseScreenPosition.Window, WindowId.Invalid))
      return new Vector2?();
    if (fixedOffset)
    {
      IEye currentEye = this._eyeManager.CurrentEye;
      MapCoordinates map = this._eyeManager.PixelToMap(mouseScreenPosition);
      if (MapId.op_Equality(map.MapId, MapId.Nullspace) || MapId.op_Inequality(map.MapId, currentEye.Position.MapId))
        return new Vector2?();
      Vector2 vector2 = map.Position - currentEye.Position.Position;
      return (double) vector2.Length() < 0.5 ? new Vector2?(Vector2.Zero) : new Vector2?(Vector2Helpers.Normalized(vector2) * maxOffset);
    }
    Vector2i size = this._clyde.MainWindow.Size;
    float num = MathF.Min((float) size.X / 2f, (float) size.Y / 2f) * 0.9f;
    if ((double) num <= 0.0)
      return new Vector2?();
    Vector2 vector2_1 = new Vector2((float) -((double) ((ScreenCoordinates) ref mouseScreenPosition).X - (double) size.X / 2.0) / num, (((ScreenCoordinates) ref mouseScreenPosition).Y - (float) size.Y / 2f) / num);
    Angle rotation = this._eyeManager.CurrentEye.Rotation;
    Quaternion fromAxisAngle = Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float) ((Angle) ref rotation).Opposite().Theta);
    Vector2 vector2_2 = Vector2.Transform(vector2_1, fromAxisAngle) * maxOffset;
    if ((double) vector2_2.Length() > (double) maxOffset)
      vector2_2 = Vector2Helpers.Normalized(vector2_2) * maxOffset;
    return new Vector2?(vector2_2);
  }

  public bool TryGetActiveOffset(EntityUid weapon, out Vector2 offset)
  {
    offset = Vector2.Zero;
    PubgFocusViewComponent focusViewComponent;
    Vector2 vector2;
    if (!this.TryComp<PubgFocusViewComponent>(weapon, ref focusViewComponent) || !focusViewComponent.Active || !this._currentOffsets.TryGetValue(weapon, out vector2) || vector2 == Vector2.Zero)
      return false;
    offset = vector2;
    return true;
  }
}
