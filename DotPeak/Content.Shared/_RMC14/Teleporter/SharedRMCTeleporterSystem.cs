// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Teleporter.SharedRMCTeleporterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Marines;
using Content.Shared.Damage;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Teleporter;

public abstract class SharedRMCTeleporterSystem : EntitySystem
{
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private DamageableSystem _damageableSystem;
  private Robust.Shared.GameObjects.EntityQuery<ActorComponent> _actorQuery;
  private Robust.Shared.GameObjects.EntityQuery<AlmayerComponent> _almayerQuery;
  private Robust.Shared.GameObjects.EntityQuery<DropshipComponent> _dropshipQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _mapGridQuery;

  public override void Initialize()
  {
    this._actorQuery = this.GetEntityQuery<ActorComponent>();
    this._almayerQuery = this.GetEntityQuery<AlmayerComponent>();
    this._dropshipQuery = this.GetEntityQuery<DropshipComponent>();
    this._mapGridQuery = this.GetEntityQuery<MapGridComponent>();
    this.SubscribeLocalEvent<RMCTeleporterComponent, StartCollideEvent>(new EntityEventRefHandler<RMCTeleporterComponent, StartCollideEvent>(this.OnTeleportStartCollide));
    this.SubscribeLocalEvent<RMCTeleporterViewerComponent, StartCollideEvent>(new EntityEventRefHandler<RMCTeleporterViewerComponent, StartCollideEvent>(this.OnViewerStartCollide));
    this.SubscribeLocalEvent<RMCTeleporterViewerComponent, EndCollideEvent>(new EntityEventRefHandler<RMCTeleporterViewerComponent, EndCollideEvent>(this.OnViewerEndCollide));
  }

  private void OnTeleportStartCollide(
    Entity<RMCTeleporterComponent> ent,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (this._almayerQuery.HasComp(otherEntity) || this._dropshipQuery.HasComp(otherEntity) || this._mapGridQuery.HasComp(otherEntity))
      return;
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(otherEntity);
    MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates((EntityUid) ent);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return;
    Vector2 offset = mapCoordinates1.Position - mapCoordinates2.Position;
    if ((double) offset.Length() > 10.0)
      return;
    MapCoordinates teleport = mapCoordinates2.Offset(offset).Offset(ent.Comp.Adjust);
    this.HandlePulling(otherEntity, teleport);
    if (ent.Comp.TeleportDamage == null)
      return;
    this._damageableSystem.TryChangeDamage(new EntityUid?(args.OtherEntity), ent.Comp.TeleportDamage, origin: new EntityUid?((EntityUid) ent));
  }

  private void OnViewerStartCollide(
    Entity<RMCTeleporterViewerComponent> ent,
    ref StartCollideEvent args)
  {
    ActorComponent component;
    if (!this._actorQuery.TryComp(args.OtherEntity, out component))
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTeleporterViewerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCTeleporterViewerComponent>();
    EntityUid uid;
    RMCTeleporterViewerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(uid == ent.Owner) && !(comp1.Id != ent.Comp.Id))
        this.AddViewer((Entity<RMCTeleporterViewerComponent>) (uid, comp1), component.PlayerSession);
    }
  }

  private void OnViewerEndCollide(
    Entity<RMCTeleporterViewerComponent> ent,
    ref EndCollideEvent args)
  {
    ActorComponent component;
    if (!this._actorQuery.TryComp(args.OtherEntity, out component))
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTeleporterViewerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCTeleporterViewerComponent>();
    EntityUid uid;
    RMCTeleporterViewerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(uid == ent.Owner) && !(comp1.Id != ent.Comp.Id))
        this.RemoveViewer((Entity<RMCTeleporterViewerComponent>) (uid, comp1), component.PlayerSession);
    }
  }

  protected virtual void AddViewer(
    Entity<RMCTeleporterViewerComponent> viewer,
    ICommonSession player)
  {
  }

  protected virtual void RemoveViewer(
    Entity<RMCTeleporterViewerComponent> viewer,
    ICommonSession player)
  {
  }

  public IEnumerable<Entity<RMCTeleporterViewerComponent>> GetMatchingTeleporterViewers(
    Entity<RMCTeleporterViewerComponent> viewer)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTeleporterViewerComponent> viewers = this.EntityQueryEnumerator<RMCTeleporterViewerComponent>();
    EntityUid uid;
    RMCTeleporterViewerComponent comp1;
    while (viewers.MoveNext(out uid, out comp1))
    {
      if (viewer.Owner != uid && viewer.Comp.Id == comp1.Id)
        yield return (Entity<RMCTeleporterViewerComponent>) (uid, comp1);
    }
  }

  public void HandlePulling(EntityUid user, MapCoordinates teleport)
  {
    PullableComponent comp1;
    if (this.TryComp<PullableComponent>(user, out comp1) && comp1.Puller.HasValue)
      this._pulling.TryStopPull(user, comp1, new EntityUid?(comp1.Puller.Value));
    PullerComponent comp2;
    PullableComponent comp3;
    if (this.TryComp<PullerComponent>(user, out comp2) && this.TryComp<PullableComponent>(comp2.Pulling, out comp3))
    {
      PullerComponent comp4;
      PullableComponent comp5;
      if (this.TryComp<PullerComponent>(comp2.Pulling, out comp4) && this.TryComp<PullableComponent>(comp4.Pulling, out comp5))
        this._pulling.TryStopPull(comp4.Pulling.Value, comp5, comp2.Pulling);
      EntityUid entityUid = comp2.Pulling.Value;
      this._pulling.TryStopPull(entityUid, comp3, new EntityUid?(user));
      this._transform.SetMapCoordinates(user, teleport);
      this._transform.SetMapCoordinates(entityUid, teleport);
      this._pulling.TryStartPull(user, entityUid);
    }
    else
      this._transform.SetMapCoordinates(user, teleport);
  }
}
