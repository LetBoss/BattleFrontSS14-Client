// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Rangefinder.SharedCivRangefinderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Timing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Rangefinder;

public abstract class SharedCivRangefinderSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CivRangefinderComponent, MapInitEvent>(new EntityEventRefHandler<CivRangefinderComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<CivRangefinderComponent, AfterInteractEvent>(new EntityEventRefHandler<CivRangefinderComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<CivRangefinderComponent, CivRangefinderDoAfterEvent>(new EntityEventRefHandler<CivRangefinderComponent, CivRangefinderDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<CivRangefinderComponent, ExaminedEvent>(new EntityEventRefHandler<CivRangefinderComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<CivRangefinderComponent, ComponentRemove>(new EntityEventRefHandler<CivRangefinderComponent, ComponentRemove>(this.OnRangefinderRemove<ComponentRemove>));
    this.SubscribeLocalEvent<CivRangefinderComponent, EntityTerminatingEvent>(new EntityEventRefHandler<CivRangefinderComponent, EntityTerminatingEvent>(this.OnRangefinderRemove<EntityTerminatingEvent>));
  }

  private void OnRangefinderRemove<T>(Entity<CivRangefinderComponent> rangefinder, ref T args)
  {
    if (this._net.IsClient)
      return;
    EntityUid? markerEntity = rangefinder.Comp.MarkerEntity;
    if (!markerEntity.HasValue)
      return;
    this.QueueDel(new EntityUid?(markerEntity.GetValueOrDefault()));
  }

  private void OnMapInit(Entity<CivRangefinderComponent> rangefinder, ref MapInitEvent args)
  {
    if (rangefinder.Comp.TargetDelay > TimeSpan.Zero)
      this._useDelay.SetLength((Entity<UseDelayComponent>) rangefinder.Owner, rangefinder.Comp.TargetDelay, rangefinder.Comp.TargetUseDelay);
    this.Dirty<CivRangefinderComponent>(rangefinder);
  }

  private void OnAfterInteract(
    Entity<CivRangefinderComponent> rangefinder,
    ref AfterInteractEvent args)
  {
    EntityCoordinates grid = args.ClickLocation.SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
    if (!grid.IsValid((IEntityManager) this.EntityManager))
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(grid);
    if (this._transform.GetMapId((Entity<TransformComponent>) args.User) != mapCoordinates.MapId)
      return;
    args.Handled = true;
    this.TryTarget(rangefinder, args.User, grid);
  }

  private void OnDoAfter(
    Entity<CivRangefinderComponent> rangefinder,
    ref CivRangefinderDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    if (!coordinates.IsValid((IEntityManager) this.EntityManager))
      return;
    this._audio.PlayPredicted(rangefinder.Comp.AcquireSound, (EntityUid) rangefinder, new EntityUid?(args.User));
    if (this._net.IsClient)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(coordinates);
    rangefinder.Comp.LastTarget = new Vector2i?(Vector2Helpers.Floored(mapCoordinates.Position));
    rangefinder.Comp.LastCoords = new MapCoordinates?(mapCoordinates);
    this.Dirty<CivRangefinderComponent>(rangefinder);
    EntityUid? markerEntity = rangefinder.Comp.MarkerEntity;
    if (markerEntity.HasValue)
      this.QueueDel(new EntityUid?(markerEntity.GetValueOrDefault()));
    rangefinder.Comp.MarkerEntity = new EntityUid?(this.Spawn((string) rangefinder.Comp.MarkerSpawn, this._transform.GetMoverCoordinates(coordinates)));
    this._ui.OpenUi((Entity<UserInterfaceComponent>) rangefinder.Owner, (Enum) CivRangefinderUiKey.Key, new EntityUid?(args.User));
  }

  private void OnExamined(Entity<CivRangefinderComponent> rangefinder, ref ExaminedEvent args)
  {
    Vector2i? lastTarget = rangefinder.Comp.LastTarget;
    if (!lastTarget.HasValue)
      return;
    Vector2i valueOrDefault = lastTarget.GetValueOrDefault();
    using (args.PushGroup("CivRangefinderComponent"))
      args.PushText(this.Loc.GetString("civ-eq-rangefinder-examine-coords", ("x", (object) valueOrDefault.X), ("y", (object) valueOrDefault.Y)));
  }

  private void TryTarget(
    Entity<CivRangefinderComponent> rangefinder,
    EntityUid user,
    EntityCoordinates coordinates)
  {
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) rangefinder, out comp))
    {
      if (this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) rangefinder, comp), rangefinder.Comp.TargetUseDelay))
        return;
      this._useDelay.TryResetDelay((EntityUid) rangefinder, component: comp, id: rangefinder.Comp.TargetUseDelay);
    }
    CivRangefinderDoAfterEvent @event = new CivRangefinderDoAfterEvent(this.GetNetCoordinates(coordinates));
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, rangefinder.Comp.TargetDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) rangefinder))
    {
      BreakOnMove = true,
      NeedHand = true,
      BreakOnHandChange = false,
      MovementThreshold = 0.5f
    }))
      return;
    this._audio.PlayPredicted(rangefinder.Comp.TargetSound, (EntityUid) rangefinder, new EntityUid?(user));
    rangefinder.Comp.DoAfter = @event.DoAfter;
  }
}
