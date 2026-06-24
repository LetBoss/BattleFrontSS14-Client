// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParaDrop.SharedParaDropSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.ParaDrop;

public abstract class SharedParaDropSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  protected ActionBlockerSystem Blocker;
  [Dependency]
  private SharedCrashLandSystem _crashLand;
  [Dependency]
  private SharedDropshipSystem _dropship;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private const int CrashScatter = 7;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CrashLandOnTouchComponent, AttemptCrashLandEvent>(new EntityEventRefHandler<CrashLandOnTouchComponent, AttemptCrashLandEvent>(this.OnAttemptCrashLand));
    this.SubscribeLocalEvent<MapGridComponent, AttemptCrashLandEvent>(new EntityEventRefHandler<MapGridComponent, AttemptCrashLandEvent>(this.OnAttemptCrashLand));
    this.SubscribeLocalEvent<GrantParaDroppableComponent, GotEquippedEvent>(new EntityEventRefHandler<GrantParaDroppableComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<GrantParaDroppableComponent, GotUnequippedEvent>(new EntityEventRefHandler<GrantParaDroppableComponent, GotUnequippedEvent>(this.OnGotUnEquipped));
    this.SubscribeLocalEvent<ParaDroppingComponent, MapInitEvent>(new EntityEventRefHandler<ParaDroppingComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ParaDroppingComponent, ComponentShutdown>(new EntityEventRefHandler<ParaDroppingComponent, ComponentShutdown>(this.OnComponentShutdown));
    this.SubscribeLocalEvent<ParaDroppingComponent, RMCIgniteAttemptEvent>(new EntityEventRefHandler<ParaDroppingComponent, RMCIgniteAttemptEvent>(this.OnIgniteAttempt));
    this.SubscribeLocalEvent<ParaDroppingComponent, GettingAttackedAttemptEvent>(new EntityEventRefHandler<ParaDroppingComponent, GettingAttackedAttemptEvent>(this.OnGettingAttacked));
    this.SubscribeLocalEvent<ParaDroppingComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<ParaDroppingComponent, AttemptMobCollideEvent>(this.OnAttemptMobCollide));
    this.SubscribeLocalEvent<ParaDroppingComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<ParaDroppingComponent, AttemptMobTargetCollideEvent>(this.OnAttemptMobTargetCollide));
    this.SubscribeLocalEvent<ParaDroppingComponent, ThrowPushbackAttemptEvent>(new EntityEventRefHandler<ParaDroppingComponent, ThrowPushbackAttemptEvent>(this.OnThrowPushbackAttempt));
    this.SubscribeLocalEvent<ParaDroppingComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<ParaDroppingComponent, BeforeDamageChangedEvent>(this.OnBeforeDamageChanged));
    this.SubscribeLocalEvent<ParaDroppingComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<ParaDroppingComponent, UpdateCanMoveEvent>(this.OnUpdateCanMove));
    this.SubscribeLocalEvent<SkyFallingComponent, ComponentShutdown>(new EntityEventRefHandler<SkyFallingComponent, ComponentShutdown>(this.OnComponentShutdown));
  }

  private void OnGotEquipped(Entity<GrantParaDroppableComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    this.EnsureComp<ParaDroppableComponent>(args.Equipee);
  }

  private void OnGotUnEquipped(Entity<GrantParaDroppableComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    this.RemComp<ParaDroppableComponent>(args.Equipee);
  }

  private void OnAttemptCrashLand(
    Entity<CrashLandOnTouchComponent> ent,
    ref AttemptCrashLandEvent args)
  {
    Entity<DropshipComponent> dropship;
    ActiveParaDropComponent comp;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || !this.TryComp<ActiveParaDropComponent>((EntityUid) dropship, out comp) && !this.HasComp<ParaDroppableComponent>(args.Crashing))
      return;
    args.Cancelled = true;
    this.AttemptParaDrop((Entity<ActiveParaDropComponent>) ((EntityUid) dropship, comp), args.Crashing);
  }

  private void OnAttemptCrashLand(Entity<MapGridComponent> ent, ref AttemptCrashLandEvent args)
  {
    Entity<DropshipComponent> dropship;
    ActiveParaDropComponent comp;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || !this.TryComp<ActiveParaDropComponent>((EntityUid) dropship, out comp) && !this.HasComp<ParaDroppableComponent>(args.Crashing))
      return;
    args.Cancelled = true;
    this.AttemptParaDrop((Entity<ActiveParaDropComponent>) ((EntityUid) dropship, comp), args.Crashing);
  }

  private void OnMapInit(Entity<ParaDroppingComponent> ent, ref MapInitEvent args)
  {
    FixturesComponent comp;
    if (!this.TryComp<PhysicsComponent>((EntityUid) ent, out PhysicsComponent _) || !this.TryComp<FixturesComponent>((EntityUid) ent, out comp))
      return;
    foreach (KeyValuePair<string, Fixture> fixture in comp.Fixtures)
    {
      ent.Comp.OriginalLayers.TryAdd(fixture.Key, fixture.Value.CollisionLayer);
      ent.Comp.OriginalMasks.TryAdd(fixture.Key, fixture.Value.CollisionMask);
      this._physics.SetCollisionLayer((EntityUid) ent, fixture.Key, fixture.Value, 0);
      this._physics.SetCollisionMask((EntityUid) ent, fixture.Key, fixture.Value, 0);
    }
    this.Dirty<ParaDroppingComponent>(ent);
  }

  private void OnComponentShutdown(Entity<ParaDroppingComponent> ent, ref ComponentShutdown args)
  {
    FixturesComponent comp;
    if (!this.TryComp<PhysicsComponent>((EntityUid) ent, out PhysicsComponent _) || !this.TryComp<FixturesComponent>((EntityUid) ent, out comp))
      return;
    foreach (KeyValuePair<string, Fixture> fixture in comp.Fixtures)
    {
      int layer;
      int mask;
      if (ent.Comp.OriginalLayers.TryGetValue(fixture.Key, out layer) && ent.Comp.OriginalMasks.TryGetValue(fixture.Key, out mask))
      {
        this._physics.SetCollisionLayer((EntityUid) ent, fixture.Key, fixture.Value, layer);
        this._physics.SetCollisionMask((EntityUid) ent, fixture.Key, fixture.Value, mask);
      }
    }
  }

  private void OnComponentShutdown(Entity<SkyFallingComponent> ent, ref ComponentShutdown args)
  {
    if (!ent.Comp.TargetCoordinates.HasValue)
      return;
    this._transform.SetMapCoordinates((EntityUid) ent, this._transform.ToMapCoordinates(ent.Comp.TargetCoordinates.Value));
    ParaDroppableComponent comp;
    if (!this.TryComp<ParaDroppableComponent>((EntityUid) ent, out comp))
      return;
    this._audio.PlayPvs(comp.DropSound, (EntityUid) ent);
  }

  private void OnIgniteAttempt(Entity<ParaDroppingComponent> ent, ref RMCIgniteAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnAttemptMobCollide(
    Entity<ParaDroppingComponent> ent,
    ref AttemptMobCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnAttemptMobTargetCollide(
    Entity<ParaDroppingComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnGettingAttacked(
    Entity<ParaDroppingComponent> ent,
    ref GettingAttackedAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnThrowPushbackAttempt(
    Entity<ParaDroppingComponent> ent,
    ref ThrowPushbackAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnBeforeDamageChanged(
    Entity<ParaDroppingComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    args.Cancelled = true;
  }

  private void OnUpdateCanMove(Entity<ParaDroppingComponent> ent, ref UpdateCanMoveEvent args)
  {
    args.Cancel();
  }

  private void AttemptParaDrop(Entity<ActiveParaDropComponent?> dropShip, EntityUid dropping)
  {
    if (this._net.IsClient || this.HasComp<ParaDroppingComponent>(dropping))
      return;
    EntityUid? nullable1 = new EntityUid?();
    ActiveParaDropComponent comp = dropShip.Comp;
    if ((comp != null ? (comp.DropTarget.HasValue ? 1 : 0) : 0) != 0)
      nullable1 = dropShip.Comp.DropTarget;
    if (!nullable1.HasValue)
    {
      EntityCoordinates? nullable2 = new EntityCoordinates?();
      EntityCoordinates location;
      if (this._crashLand.TryGetCrashLandLocation(out location))
        nullable2 = new EntityCoordinates?(location);
      if (!nullable2.HasValue)
        this._popup.PopupClient("Your harness got stuck and is preventing your from jumping", new EntityUid?(dropping), PopupType.SmallCaution);
      else
        this.TryDrop(dropping, nullable2.Value);
    }
    else
      this.TryDrop(dropping, this._transform.GetMoverCoordinates(nullable1.Value));
  }

  private bool TryDrop(EntityUid dropping, EntityCoordinates dropCoordinates)
  {
    ParaDroppableComponent comp1;
    if (!this.TryComp<ParaDroppableComponent>(dropping, out comp1))
    {
      EntityCoordinates adjustedLocation;
      if (this.TryGetParaDropLocation(dropCoordinates, 7, out adjustedLocation))
        dropCoordinates = adjustedLocation;
      this._crashLand.TryCrashLand((Entity<CrashLandableComponent>) dropping, true, dropCoordinates);
      return false;
    }
    comp1.LastParaDrop = new TimeSpan?(this._timing.CurTime);
    this.Dirty(dropping, (IComponent) comp1);
    this._rmcPulling.TryStopAllPullsFromAndOn(dropping);
    PhysicsComponent comp2;
    if (this.TryComp<PhysicsComponent>(dropping, out comp2))
      this._physics.SetLinearVelocity(dropping, Vector2.Zero, body: comp2);
    EntityCoordinates adjustedLocation1;
    if (this.TryGetParaDropLocation(dropCoordinates, comp1.DropScatter, out adjustedLocation1))
      dropCoordinates = adjustedLocation1;
    SkyFallingComponent fallingComponent = this.EnsureComp<SkyFallingComponent>(dropping);
    fallingComponent.TargetCoordinates = new EntityCoordinates?(dropCoordinates);
    this.Dirty(dropping, (IComponent) fallingComponent);
    ParaDroppingComponent droppingComponent = this.EnsureComp<ParaDroppingComponent>(dropping);
    droppingComponent.RemainingTime = comp1.DropDuration;
    this.Dirty(dropping, (IComponent) droppingComponent);
    this.Blocker.UpdateCanMove(dropping);
    return true;
  }

  private bool TryGetParaDropLocation(
    EntityCoordinates targetLocation,
    int dropScatter,
    out EntityCoordinates adjustedLocation)
  {
    adjustedLocation = new EntityCoordinates();
    EntityUid uid;
    MapGridComponent comp;
    if (!this.EntityQueryEnumerator<RMCPlanetComponent>().MoveNext(out uid, out RMCPlanetComponent _) || !this.TryComp<MapGridComponent>(uid, out comp))
      return false;
    Vector2i tile1 = this._mapSystem.LocalToTile(uid, comp, targetLocation);
    Box2 aabb;
    // ISSUE: explicit constructor call
    ((Box2) ref aabb).\u002Ector((float) (tile1.X - dropScatter), (float) (tile1.Y - dropScatter), (float) (tile1.X + dropScatter), (float) (tile1.Y + dropScatter));
    SharedMapSystem.TilesEnumerator tilesEnumerator = this._mapSystem.GetTilesEnumerator(uid, comp, aabb);
    List<TileRef> tileRefList = new List<TileRef>();
    TileRef tile2;
    while (tilesEnumerator.MoveNext(out tile2))
    {
      if (this._crashLand.IsLandableTile((Entity<MapGridComponent>) (uid, comp), tile2))
        tileRefList.Add(tile2);
    }
    if (tileRefList.Count == 0)
      return false;
    int index = this._random.Next(0, tileRefList.Count);
    adjustedLocation = this._mapSystem.GridTileToLocal(uid, comp, tileRefList[index].GridIndices);
    return true;
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveParaDropComponent, DropshipComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<ActiveParaDropComponent, DropshipComponent>();
    EntityUid uid1;
    ActiveParaDropComponent comp1_1;
    DropshipComponent comp2;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2))
    {
      if (comp2.State == FTLState.Arriving || !this.HasComp<DropshipTargetComponent>(comp1_1.DropTarget))
        this.RemComp<ActiveParaDropComponent>(uid1);
    }
    if (!this._timing.IsFirstTimePredicted)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ParaDroppingComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<ParaDroppingComponent>();
    EntityUid uid2;
    ParaDroppingComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!this.HasComp<SkyFallingComponent>(uid2))
      {
        comp1_2.RemainingTime -= frameTime;
        if ((double) comp1_2.RemainingTime <= 0.0)
          this.RemComp<ParaDroppingComponent>(uid2);
        this.Blocker.UpdateCanMove(uid2);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<SkyFallingComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<SkyFallingComponent>();
    EntityUid uid3;
    SkyFallingComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      comp1_3.RemainingTime -= frameTime;
      if ((double) comp1_3.RemainingTime <= 0.0)
        this.RemComp<SkyFallingComponent>(uid3);
    }
  }
}
