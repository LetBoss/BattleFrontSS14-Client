// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Fireman.FiremanCarrySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Sprite;
using Content.Shared.ActionBlocker;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs;
using Content.Shared.MouseRotator;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Strip;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Fireman;

public sealed class FiremanCarrySystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedRMCSpriteSystem _rmcSprite;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly List<(EntityUid Target, EntityUid Carrier)> _toReparent = new List<(EntityUid, EntityUid)>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<FiremanCarriableComponent, CanDragEvent>(new EntityEventRefHandler<FiremanCarriableComponent, CanDragEvent>(this.OnCarriableCanDrag));
    this.SubscribeLocalEvent<FiremanCarriableComponent, DragDropDraggedEvent>(new EntityEventRefHandler<FiremanCarriableComponent, DragDropDraggedEvent>(this.OnCarriableDragDropDragged), new Type[1]
    {
      typeof (SharedStrippableSystem)
    });
    this.SubscribeLocalEvent<FiremanCarriableComponent, DoAfterAttemptEvent<FiremanCarryDoAfterEvent>>(new EntityEventRefHandler<FiremanCarriableComponent, DoAfterAttemptEvent<FiremanCarryDoAfterEvent>>(this.OnCarriableFiremanCarryDoAfterAttempt));
    this.SubscribeLocalEvent<FiremanCarriableComponent, FiremanCarryDoAfterEvent>(new EntityEventRefHandler<FiremanCarriableComponent, FiremanCarryDoAfterEvent>(this.OnCarriableFiremanCarryDoAfter));
    this.SubscribeLocalEvent<FiremanCarriableComponent, StandAttemptEvent>(new EntityEventRefHandler<FiremanCarriableComponent, StandAttemptEvent>(this.OnCarriableStandAttempt));
    this.SubscribeLocalEvent<FiremanCarriableComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<FiremanCarriableComponent, UpdateCanMoveEvent>(this.OnCarriableCanMove));
    this.SubscribeLocalEvent<FiremanCarriableComponent, MoveInputEvent>(new EntityEventRefHandler<FiremanCarriableComponent, MoveInputEvent>(this.OnCarriableMoveInput));
    this.SubscribeLocalEvent<FiremanCarriableComponent, BreakFiremanCarryDoAfterEvent>(new EntityEventRefHandler<FiremanCarriableComponent, BreakFiremanCarryDoAfterEvent>(this.OnCarriableBreakCarryDoAfter));
    this.SubscribeLocalEvent<FiremanCarriableComponent, PullStartedMessage>(new EntityEventRefHandler<FiremanCarriableComponent, PullStartedMessage>(this.OnCarriablePullStarted));
    this.SubscribeLocalEvent<FiremanCarriableComponent, PullStoppedMessage>(new EntityEventRefHandler<FiremanCarriableComponent, PullStoppedMessage>(this.OnCarriablePullStopped));
    this.SubscribeLocalEvent<FiremanCarriableComponent, PullAttemptEvent>(new EntityEventRefHandler<FiremanCarriableComponent, PullAttemptEvent>(this.OnCarriablePullAttempt));
    this.SubscribeLocalEvent<CanFiremanCarryComponent, PullStartedMessage>(new EntityEventRefHandler<CanFiremanCarryComponent, PullStartedMessage>(this.OnCarrierPullStarted));
    this.SubscribeLocalEvent<CanFiremanCarryComponent, PullStoppedMessage>(new EntityEventRefHandler<CanFiremanCarryComponent, PullStoppedMessage>(this.OnCarrierPullStopped));
    this.SubscribeLocalEvent<CanFiremanCarryComponent, PullSlowdownAttemptEvent>(new EntityEventRefHandler<CanFiremanCarryComponent, PullSlowdownAttemptEvent>(this.OnCarrierPullSlowdownAttempt));
    this.SubscribeLocalEvent<CanFiremanCarryComponent, MobStateChangedEvent>(new EntityEventRefHandler<CanFiremanCarryComponent, MobStateChangedEvent>(this.OnCarrierMobStateChanged));
    this.SubscribeLocalEvent<CanFiremanCarryComponent, RMCPullToggleEvent>(new EntityEventRefHandler<CanFiremanCarryComponent, RMCPullToggleEvent>(this.OnCarrierPullToggle));
    this.SubscribeLocalEvent<BeingFiremanCarriedComponent, PreventCollideEvent>(new EntityEventRefHandler<BeingFiremanCarriedComponent, PreventCollideEvent>(this.OnBeingCarriedPreventCollide));
  }

  private void OnCarriableCanDrag(Entity<FiremanCarriableComponent> ent, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnCarriableDragDropDragged(
    Entity<FiremanCarriableComponent> ent,
    ref DragDropDraggedEvent args)
  {
    EntityUid user = args.User;
    CanFiremanCarryComponent comp;
    if (!this.TryComp<CanFiremanCarryComponent>(user, out comp) || args.Target != user || !this._rmcPulling.IsPulling((Entity<PullerComponent>) user, (Entity<PullableComponent>) ent.Owner))
      return;
    args.Handled = true;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) user, ent.Comp.Skill, 1))
      this._popup.PopupClient(this.Loc.GetString("You aren't trained to carry people!"), (EntityUid) ent, new EntityUid?(user), PopupType.MediumCaution);
    else if (!comp.AggressiveGrab)
    {
      this._popup.PopupClient(this.Loc.GetString("You need to grab them aggressively first!"), (EntityUid) ent, new EntityUid?(user), PopupType.MediumCaution);
    }
    else
    {
      TimeSpan delay = ent.Comp.Delay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user, ent.Comp.Skill);
      FiremanCarryDoAfterEvent @event = new FiremanCarryDoAfterEvent();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(user))
      {
        BreakOnMove = true,
        AttemptFrequency = AttemptFrequency.EveryTick,
        ForceVisible = true
      }))
        return;
      this._popup.PopupClient(this.Loc.GetString($"You start loading {Identity.Name((EntityUid) ent, (IEntityManager) this.EntityManager, new EntityUid?(args.User))} onto your back."), (EntityUid) ent, new EntityUid?(user), PopupType.Medium);
    }
  }

  private void OnCarriableFiremanCarryDoAfterAttempt(
    Entity<FiremanCarriableComponent> ent,
    ref DoAfterAttemptEvent<FiremanCarryDoAfterEvent> args)
  {
    if (this._rmcPulling.IsPulling((Entity<PullerComponent>) args.DoAfter.Args.User, (Entity<PullableComponent>) ent.Owner))
      return;
    args.Cancel();
  }

  private void OnCarriableFiremanCarryDoAfter(
    Entity<FiremanCarriableComponent> ent,
    ref FiremanCarryDoAfterEvent args)
  {
    EntityUid user = args.User;
    CanFiremanCarryComponent comp;
    if (args.Cancelled || args.Handled || !this.TryComp<CanFiremanCarryComponent>(user, out comp) || this._transform.IsParentOf(this.Transform(ent.Owner), user))
      return;
    ent.Comp.BeingCarried = true;
    this.Dirty<FiremanCarriableComponent>(ent);
    this.EnsureComp<BeingFiremanCarriedComponent>((EntityUid) ent);
    comp.Carrying = new EntityUid?((EntityUid) ent);
    this.Dirty(user, (IComponent) comp);
    if (!this._timing.ApplyingState && !this.HasComp<MouseRotatorComponent>(user))
      this.RemCompDeferred<NoRotateOnMoveComponent>(user);
    args.Handled = true;
    this._transform.SetParent((EntityUid) ent, user);
    this._transform.SetLocalPosition((EntityUid) ent, Vector2.Zero);
    this._standing.Down((EntityUid) ent, changeCollision: true);
    this._movementSpeed.RefreshMovementSpeedModifiers(user);
    this._rmcSprite.SetRenderOrder(user, 1);
  }

  private void OnCarriableStandAttempt(
    Entity<FiremanCarriableComponent> ent,
    ref StandAttemptEvent args)
  {
    if (!ent.Comp.BeingCarried && !this.IsBeingAggressivelyGrabbed((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnCarriableCanMove(
    Entity<FiremanCarriableComponent> ent,
    ref UpdateCanMoveEvent args)
  {
    if (!this.IsBeingAggressivelyGrabbed((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnCarriableMoveInput(Entity<FiremanCarriableComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement || ent.Comp.BreakingFree || !ent.Comp.BeingCarried && !this.IsBeingAggressivelyGrabbed((EntityUid) ent))
      return;
    BreakFiremanCarryDoAfterEvent @event = new BreakFiremanCarryDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) ent, ent.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent))))
      return;
    ent.Comp.BreakingFree = true;
    EntityUid user;
    if (!this._rmcPulling.IsBeingPulled((Entity<PullableComponent>) ent.Owner, out user))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-pull-break-start-self", ("puller", (object) user)), (EntityUid) ent, new EntityUid?((EntityUid) ent), PopupType.MediumCaution);
    foreach (ICommonSession recipient in Filter.PvsExcept((EntityUid) ent, entityManager: (IEntityManager) this.EntityManager).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("rmc-pull-break-start-others", ("puller", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), ("pulled", (object) Identity.Name((EntityUid) ent, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), (EntityUid) ent, valueOrDefault, PopupType.MediumCaution);
      }
    }
  }

  private void OnCarriableBreakCarryDoAfter(
    Entity<FiremanCarriableComponent> ent,
    ref BreakFiremanCarryDoAfterEvent args)
  {
    ent.Comp.BreakingFree = false;
    this.Dirty<FiremanCarriableComponent>(ent);
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    ent.Comp.BeingCarried = false;
    this.RemCompDeferred<BeingFiremanCarriedComponent>((EntityUid) ent);
    EntityUid user;
    if (!this._rmcPulling.IsBeingPulled((Entity<PullableComponent>) ent.Owner, out user))
      return;
    this.StopCarry((Entity<CanFiremanCarryComponent>) user, new Entity<FiremanCarriableComponent>?((Entity<FiremanCarriableComponent>) ((EntityUid) ent, (FiremanCarriableComponent) ent)));
    this._popup.PopupClient(this.Loc.GetString("rmc-pull-break-finish-self", ("puller", (object) user)), (EntityUid) ent, new EntityUid?((EntityUid) ent), PopupType.MediumCaution);
    foreach (ICommonSession recipient in Filter.PvsExcept((EntityUid) ent, entityManager: (IEntityManager) this.EntityManager).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("rmc-pull-break-finish-others", ("puller", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), ("pulled", (object) Identity.Name((EntityUid) ent, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), (EntityUid) ent, valueOrDefault, PopupType.MediumCaution);
      }
    }
  }

  private void OnCarriablePullStarted(
    Entity<FiremanCarriableComponent> ent,
    ref PullStartedMessage args)
  {
    EntityUid user;
    CanFiremanCarryComponent comp;
    if (args.PulledUid != ent.Owner || !this._rmcPulling.IsBeingPulled((Entity<PullableComponent>) ent.Owner, out user) || !this.TryComp<CanFiremanCarryComponent>(user, out comp))
      return;
    this.StopPull((Entity<CanFiremanCarryComponent>) (user, comp), (EntityUid) ent);
  }

  private void OnCarriablePullStopped(
    Entity<FiremanCarriableComponent> ent,
    ref PullStoppedMessage args)
  {
    if (ent.Owner != args.PulledUid)
      return;
    this._standing.Stand((EntityUid) ent);
  }

  private void OnCarriablePullAttempt(
    Entity<FiremanCarriableComponent> ent,
    ref PullAttemptEvent args)
  {
    if (args.Cancelled || !ent.Comp.BeingCarried && !this.IsBeingAggressivelyGrabbed((EntityUid) ent))
      return;
    args.Cancelled = true;
  }

  private void OnCarrierPullStarted(
    Entity<CanFiremanCarryComponent> ent,
    ref PullStartedMessage args)
  {
    if (!(ent.Owner == args.PullerUid))
      return;
    this.StopPull(ent, args.PulledUid);
  }

  private void OnCarrierPullStopped(
    Entity<CanFiremanCarryComponent> ent,
    ref PullStoppedMessage args)
  {
    if (!(ent.Owner == args.PullerUid))
      return;
    this.StopPull(ent, args.PulledUid);
  }

  private void OnCarrierPullSlowdownAttempt(
    Entity<CanFiremanCarryComponent> ent,
    ref PullSlowdownAttemptEvent args)
  {
    EntityUid? carrying = ent.Comp.Carrying;
    EntityUid target = args.Target;
    if ((carrying.HasValue ? (carrying.GetValueOrDefault() == target ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  private void OnCarrierMobStateChanged(
    Entity<CanFiremanCarryComponent> ent,
    ref MobStateChangedEvent args)
  {
    EntityUid? carrying = ent.Comp.Carrying;
    if (!carrying.HasValue)
      return;
    EntityUid valueOrDefault = carrying.GetValueOrDefault();
    if (args.NewMobState != MobState.Dead)
      return;
    this.StopCarry((Entity<CanFiremanCarryComponent>) ((EntityUid) ent, (CanFiremanCarryComponent) ent), new Entity<FiremanCarriableComponent>?((Entity<FiremanCarriableComponent>) valueOrDefault));
  }

  private void OnCarrierPullToggle(
    Entity<CanFiremanCarryComponent> ent,
    ref RMCPullToggleEvent args)
  {
    args.Handled = true;
    TimeSpan timeSpan = ent.Comp.AggressiveGrabDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) ent.Owner, ent.Comp.Skill);
    if (ent.Comp.AggressiveGrab || this._timing.CurTime < ent.Comp.PullTime + timeSpan)
      return;
    ent.Comp.AggressiveGrab = true;
    this.Dirty<CanFiremanCarryComponent>(ent);
    PullerComponent comp;
    if (!this.TryComp<PullerComponent>((EntityUid) ent, out comp))
      return;
    EntityUid? nullable = comp.Pulling;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    this._actionBlocker.UpdateCanMove(valueOrDefault1);
    this._standing.Down(valueOrDefault1, changeCollision: true);
    this._rmcPulling.PlayPullEffect((EntityUid) ent, valueOrDefault1);
    this._popup.PopupClient(this.Loc.GetString("rmc-pull-aggressive-self", ("pulled", (object) valueOrDefault1)), valueOrDefault1, new EntityUid?((EntityUid) ent), PopupType.SmallCaution);
    foreach (ICommonSession recipient in Filter.PvsExcept((EntityUid) ent, entityManager: (IEntityManager) this.EntityManager).Recipients)
    {
      nullable = recipient.AttachedEntity;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("rmc-pull-aggressive-others", ("puller", (object) Identity.Name((EntityUid) ent, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault2))), ("pulled", (object) Identity.Name(valueOrDefault1, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault2)))), (EntityUid) ent, valueOrDefault2, PopupType.SmallCaution);
      }
    }
  }

  private void OnBeingCarriedPreventCollide(
    Entity<BeingFiremanCarriedComponent> ent,
    ref PreventCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void StopPull(Entity<CanFiremanCarryComponent> ent, EntityUid target)
  {
    if (this._timing.ApplyingState)
      return;
    this.StopCarry((Entity<CanFiremanCarryComponent>) ((EntityUid) ent, (CanFiremanCarryComponent) ent), new Entity<FiremanCarriableComponent>?((Entity<FiremanCarriableComponent>) target));
    this._actionBlocker.UpdateCanMove(target);
    ent.Comp.PullTime = this._timing.CurTime;
    this.Dirty<CanFiremanCarryComponent>(ent);
  }

  private void StopCarry(
    Entity<CanFiremanCarryComponent?> user,
    Entity<FiremanCarriableComponent?>? targetNullable)
  {
    EntityUid? nullable1 = new EntityUid?();
    if (this.Resolve<CanFiremanCarryComponent>((EntityUid) user, ref user.Comp, false))
    {
      nullable1 = user.Comp.Carrying;
      user.Comp.Carrying = new EntityUid?();
      user.Comp.AggressiveGrab = false;
      this.Dirty<CanFiremanCarryComponent>(user);
      this._rmcSprite.SetRenderOrder((EntityUid) user, 0);
    }
    if (!targetNullable.HasValue)
      return;
    Entity<FiremanCarriableComponent> valueOrDefault = targetNullable.GetValueOrDefault();
    if (this.Resolve<FiremanCarriableComponent>((EntityUid) valueOrDefault, ref valueOrDefault.Comp, false))
    {
      valueOrDefault.Comp.BeingCarried = false;
      this.Dirty<FiremanCarriableComponent>(valueOrDefault);
      this.RemCompDeferred<BeingFiremanCarriedComponent>((EntityUid) valueOrDefault);
      EntityUid? nullable2 = nullable1;
      EntityUid entityUid = (EntityUid) valueOrDefault;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        this._toReparent.Add(((EntityUid) valueOrDefault, (EntityUid) user));
    }
    this._standing.Stand((EntityUid) valueOrDefault);
    this._actionBlocker.UpdateCanMove((EntityUid) valueOrDefault);
  }

  private bool IsBeingAggressivelyGrabbed(EntityUid target)
  {
    EntityUid user;
    CanFiremanCarryComponent comp;
    return this._rmcPulling.IsBeingPulled((Entity<PullableComponent>) target, out user) && this.TryComp<CanFiremanCarryComponent>(user, out comp) && comp.AggressiveGrab;
  }

  public override void Update(float frameTime)
  {
    try
    {
      foreach ((EntityUid entityUid1, EntityUid entityUid2) in this._toReparent)
      {
        if (!this.TerminatingOrDeleted(entityUid1))
        {
          if (this.TerminatingOrDeleted(entityUid2))
          {
            EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(entityUid1);
            if (!this.TerminatingOrDeleted(moverCoordinates.EntityId))
              this._transform.SetCoordinates(entityUid1, moverCoordinates);
          }
          else
          {
            EntityUid entityId = this._transform.GetMoverCoordinates(entityUid1).EntityId;
            if (!(entityUid1 == entityId))
              this._transform.SetParent(entityUid1, entityId);
          }
        }
      }
    }
    finally
    {
      this._toReparent.Clear();
    }
  }
}
