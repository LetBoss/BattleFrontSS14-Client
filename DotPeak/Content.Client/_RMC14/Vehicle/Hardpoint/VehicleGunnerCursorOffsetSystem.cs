// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Hardpoint.VehicleGunnerCursorOffsetSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Movement.Systems;
using Content.Client.Weapons.Ranged.Systems;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Camera;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Hardpoint;

public sealed class VehicleGunnerCursorOffsetSystem : EntitySystem
{
  [Dependency]
  private readonly ContentEyeSystem _contentEye;
  [Dependency]
  private readonly IEyeManager _eyeManager;
  [Dependency]
  private readonly IInputManager _inputManager;
  [Dependency]
  private readonly IClyde _clyde;
  [Dependency]
  private readonly IPlayerManager _player;
  private readonly Dictionary<EntityUid, Vector2> _currentPositions = new Dictionary<EntityUid, Vector2>();
  private static readonly float EdgeOffset = 0.9f;

  public virtual void Initialize()
  {
    this.UpdatesBefore.Add(typeof (VehicleTurretInputSystem));
    this.UpdatesBefore.Add(typeof (GunSystem));
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, GetEyeOffsetEvent>(new EntityEventRefHandler<VehicleGunnerViewUserComponent, GetEyeOffsetEvent>((object) this, __methodptr(OnGetEyeOffset)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    VehicleGunnerViewUserComponent viewUserComponent;
    EyeComponent eyeComponent;
    if (!this.TryComp<VehicleGunnerViewUserComponent>(valueOrDefault, ref viewUserComponent) || (double) viewUserComponent.CursorMaxOffset <= 0.0 || !this.TryComp<EyeComponent>(valueOrDefault, ref eyeComponent))
      return;
    this._contentEye.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((valueOrDefault, eyeComponent)));
  }

  private void OnGetEyeOffset(
    Entity<VehicleGunnerViewUserComponent> ent,
    ref GetEyeOffsetEvent args)
  {
    if ((double) ent.Comp.CursorMaxOffset <= 0.0)
    {
      this._currentPositions.Remove(ent.Owner);
    }
    else
    {
      Vector2? nullable = this.OffsetAfterMouse(ent.Owner, ent.Comp);
      if (!nullable.HasValue)
        return;
      args.Offset += nullable.Value;
    }
  }

  private Vector2? OffsetAfterMouse(EntityUid uid, VehicleGunnerViewUserComponent component)
  {
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    if (WindowId.op_Equality(mouseScreenPosition.Window, WindowId.Invalid))
      return new Vector2?(this._currentPositions.GetValueOrDefault<EntityUid, Vector2>(uid, Vector2.Zero));
    Vector2i size = this._clyde.MainWindow.Size;
    float num = MathF.Min((float) size.X / 2f, (float) size.Y / 2f) * VehicleGunnerCursorOffsetSystem.EdgeOffset;
    if ((double) num <= 0.0)
      return new Vector2?(this._currentPositions.GetValueOrDefault<EntityUid, Vector2>(uid, Vector2.Zero));
    Vector2 vector2_1 = new Vector2((float) -((double) ((ScreenCoordinates) ref mouseScreenPosition).X - (double) size.X / 2.0) / num, (((ScreenCoordinates) ref mouseScreenPosition).Y - (float) size.Y / 2f) / num);
    Angle rotation = this._eyeManager.CurrentEye.Rotation;
    Quaternion fromAxisAngle = Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float) ((Angle) ref rotation).Opposite().Theta);
    Vector2 vector2_2 = Vector2.Transform(vector2_1, fromAxisAngle) * component.CursorMaxOffset;
    if ((double) vector2_2.Length() > (double) component.CursorMaxOffset)
      vector2_2 = Vector2Helpers.Normalized(vector2_2) * component.CursorMaxOffset;
    Vector2 valueOrDefault = this._currentPositions.GetValueOrDefault<EntityUid, Vector2>(uid, Vector2.Zero);
    if (valueOrDefault != vector2_2)
    {
      Vector2 vector2_3 = vector2_2 - valueOrDefault;
      if ((double) vector2_3.Length() > (double) component.CursorOffsetSpeed)
        vector2_3 = Vector2Helpers.Normalized(vector2_3) * component.CursorOffsetSpeed;
      valueOrDefault += vector2_3;
      this._currentPositions[uid] = valueOrDefault;
    }
    return new Vector2?(valueOrDefault);
  }
}
