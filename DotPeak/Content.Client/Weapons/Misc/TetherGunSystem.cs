// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Misc.TetherGunSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Weapons.Misc;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Weapons.Misc;

public sealed class TetherGunSystem : SharedTetherGunSystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private MapSystem _mapSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TetheredComponent, ComponentStartup>(new ComponentEventHandler<TetheredComponent, ComponentStartup>((object) this, __methodptr(OnTetheredStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TetheredComponent, ComponentShutdown>(new ComponentEventHandler<TetheredComponent, ComponentShutdown>((object) this, __methodptr(OnTetheredShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TetherGunComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<TetherGunComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ForceGunComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<ForceGunComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterState)), (Type[]) null, (Type[]) null);
    this._overlay.AddOverlay((Overlay) new TetherGunOverlay((IEntityManager) this.EntityManager));
  }

  private void OnAfterState(
    EntityUid uid,
    BaseForceGunComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(component.Tethered, ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((component.Tethered.Value, spriteComponent)), component.LineColor);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<TetherGunOverlay>();
  }

  protected override bool CanTether(
    EntityUid uid,
    BaseForceGunComponent component,
    EntityUid target,
    EntityUid? user)
  {
    return false;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TetherGunComponent gun;
    if (!localEntity.HasValue || !this.TryGetTetherGun(localEntity.Value, out EntityUid? _, out gun) || !gun.TetherEntity.HasValue)
      return;
    MapCoordinates map = this._eyeManager.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return;
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    EntityCoordinates entityCoordinates = !this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent) ? this.TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(((SharedMapSystem) this._mapSystem).GetMap(map.MapId)), map) : this.TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(entityUid), map);
    TransformComponent transformComponent;
    if (this.TryComp(gun.TetherEntity, ref transformComponent))
    {
      EntityCoordinates coordinates = transformComponent.Coordinates;
      float num;
      if (((EntityCoordinates) ref coordinates).TryDistance((IEntityManager) this.EntityManager, this.TransformSystem, entityCoordinates, ref num) && (double) num < 0.10000000149011612)
        return;
    }
    this.RaisePredictiveEvent<SharedTetherGunSystem.RequestTetherMoveEvent>(new SharedTetherGunSystem.RequestTetherMoveEvent()
    {
      Coordinates = this.GetNetCoordinates(entityCoordinates, (MetaDataComponent) null)
    });
  }

  private void OnTetheredStartup(EntityUid uid, TetheredComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    ForceGunComponent forceGunComponent;
    if (this.TryComp<ForceGunComponent>(component.Tetherer, ref forceGunComponent))
    {
      this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), forceGunComponent.LineColor);
    }
    else
    {
      TetherGunComponent tetherGunComponent;
      if (!this.TryComp<TetherGunComponent>(component.Tetherer, ref tetherGunComponent))
        return;
      this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), tetherGunComponent.LineColor);
    }
  }

  private void OnTetheredShutdown(
    EntityUid uid,
    TetheredComponent component,
    ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), Color.White);
  }
}
