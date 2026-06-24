// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.HyperSleep.SharedHyperSleepChamberSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Movement.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.HyperSleep;

public abstract class SharedHyperSleepChamberSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<HyperSleepChamberComponent> _hyperSleepQuery;
  private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this._hyperSleepQuery = this.GetEntityQuery<HyperSleepChamberComponent>();
    this.SubscribeLocalEvent<HyperSleepChamberComponent, MapInitEvent>(new EntityEventRefHandler<HyperSleepChamberComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<HyperSleepChamberComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<HyperSleepChamberComponent, ContainerIsInsertingAttemptEvent>(this.OnInsertAttempt));
    this.SubscribeLocalEvent<HyperSleepChamberComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<HyperSleepChamberComponent, EntInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<InsideHyperSleepChamberComponent, MoveInputEvent>(new EntityEventRefHandler<InsideHyperSleepChamberComponent, MoveInputEvent>(this.OnMoveInput));
    this.SubscribeLocalEvent<OutsideHyperSleepChamberComponent, PreventCollideEvent>(new EntityEventRefHandler<OutsideHyperSleepChamberComponent, PreventCollideEvent>(this.OnPreventCollide));
  }

  private void OnMapInit(Entity<HyperSleepChamberComponent> ent, ref MapInitEvent args)
  {
    this._containers.EnsureContainer<Container>((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnInsertAttempt(
    Entity<HyperSleepChamberComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.EntityUid))
      return;
    args.Cancel();
  }

  private void OnInserted(
    Entity<HyperSleepChamberComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    if (this._timing.ApplyingState)
      return;
    this.EnsureComp<InsideHyperSleepChamberComponent>(args.Entity).Chamber = new EntityUid?((EntityUid) ent);
  }

  private void OnMoveInput(Entity<InsideHyperSleepChamberComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement || this._timing.ApplyingState)
      return;
    EntityUid? chamber = ent.Comp.Chamber;
    if (!chamber.HasValue)
      return;
    EntityUid valueOrDefault = chamber.GetValueOrDefault();
    this.RemCompDeferred<InsideHyperSleepChamberComponent>((EntityUid) ent);
    OutsideHyperSleepChamberComponent chamberComponent = this.EnsureComp<OutsideHyperSleepChamberComponent>((EntityUid) ent);
    chamberComponent.Chamber = new EntityUid?(valueOrDefault);
    this.Dirty((EntityUid) ent, (IComponent) chamberComponent);
  }

  private void OnPreventCollide(
    Entity<OutsideHyperSleepChamberComponent> ent,
    ref PreventCollideEvent args)
  {
    EntityUid? chamber = ent.Comp.Chamber;
    EntityUid otherEntity = args.OtherEntity;
    if ((chamber.HasValue ? (chamber.GetValueOrDefault() == otherEntity ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  public void EjectChamber(Entity<HyperSleepChamberComponent?> ent)
  {
    BaseContainer container;
    if (!this._hyperSleepQuery.Resolve((EntityUid) ent, ref ent.Comp, false) || !this._containers.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container))
      return;
    this._containers.EmptyContainer(container);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<OutsideHyperSleepChamberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OutsideHyperSleepChamberComponent>();
    EntityUid uid;
    OutsideHyperSleepChamberComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? chamber = comp1.Chamber;
      if (chamber.HasValue)
      {
        EntityUid valueOrDefault = chamber.GetValueOrDefault();
        this._intersecting.Clear();
        this._entityLookup.GetEntitiesIntersecting(uid, this._intersecting);
        if (!this._intersecting.Contains(valueOrDefault))
          this.RemCompDeferred<OutsideHyperSleepChamberComponent>(uid);
      }
      else
        this.RemCompDeferred<OutsideHyperSleepChamberComponent>(uid);
    }
  }
}
