// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Systems.MobStateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle.Components;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Damage;
using Content.Shared.Damage.ForceSay;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Pointing;
using Content.Shared.Pulling.Events;
using Content.Shared.Speech;
using Content.Shared.Standing;
using Content.Shared.Strip.Components;
using Content.Shared.Throwing;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Mobs.Systems;

[Virtual]
public class MobStateSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ILogManager _logManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private DamageableSystem _damageable;
  private ISawmill _sawmill;
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;

  public override void Initialize()
  {
    this._sawmill = this._logManager.GetSawmill("MobState");
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    base.Initialize();
    this.SubscribeEvents();
  }

  public bool IsAlive(EntityUid target, MobStateComponent? component = null)
  {
    return this._mobStateQuery.Resolve(target, ref component, false) && component.CurrentState == MobState.Alive;
  }

  public bool IsCritical(EntityUid target, MobStateComponent? component = null)
  {
    return this._mobStateQuery.Resolve(target, ref component, false) && component.CurrentState == MobState.Critical;
  }

  public bool IsDead(EntityUid target, MobStateComponent? component = null)
  {
    return this._mobStateQuery.Resolve(target, ref component, false) && component.CurrentState == MobState.Dead;
  }

  public bool IsIncapacitated(EntityUid target, MobStateComponent? component = null)
  {
    if (!this._mobStateQuery.Resolve(target, ref component, false))
      return false;
    bool flag;
    switch (component.CurrentState)
    {
      case MobState.Critical:
      case MobState.Dead:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  public bool IsInvalidState(EntityUid target, MobStateComponent? component = null)
  {
    return this._mobStateQuery.Resolve(target, ref component, false) && component.CurrentState == MobState.Invalid;
  }

  public bool HasState(EntityUid entity, MobState mobState, MobStateComponent? component = null)
  {
    return this._mobStateQuery.Resolve(entity, ref component, false) && component.AllowedStates.Contains(mobState);
  }

  public void UpdateMobState(EntityUid entity, MobStateComponent? component = null, EntityUid? origin = null)
  {
    if (!this._mobStateQuery.Resolve(entity, ref component))
      return;
    UpdateMobStateEvent args = new UpdateMobStateEvent()
    {
      Target = entity,
      Component = component,
      Origin = origin
    };
    this.RaiseLocalEvent<UpdateMobStateEvent>(entity, ref args);
    this.ChangeState(entity, component, args.State, origin);
  }

  public void ChangeMobState(
    EntityUid entity,
    MobState mobState,
    MobStateComponent? component = null,
    EntityUid? origin = null)
  {
    if (!this._mobStateQuery.Resolve(entity, ref component))
      return;
    this.ChangeState(entity, component, mobState, origin);
  }

  protected virtual void OnEnterState(
    EntityUid entity,
    MobStateComponent component,
    MobState state)
  {
    this.OnStateEnteredSubscribers(entity, component, state);
  }

  protected virtual void OnStateChanged(
    EntityUid entity,
    MobStateComponent component,
    MobState oldState,
    MobState newState)
  {
  }

  protected virtual void OnExitState(EntityUid entity, MobStateComponent component, MobState state)
  {
    this.OnStateExitSubscribers(entity, component, state);
  }

  private void ChangeState(
    EntityUid target,
    MobStateComponent component,
    MobState newState,
    EntityUid? origin = null)
  {
    MobState currentState = component.CurrentState;
    if (currentState == newState || !component.AllowedStates.Contains(newState))
      return;
    this.OnExitState(target, component, currentState);
    component.CurrentState = newState;
    this.OnEnterState(target, component, newState);
    MobStateChangedEvent args = new MobStateChangedEvent(target, component, currentState, newState, origin);
    this.OnStateChanged(target, component, currentState, newState);
    this.RaiseLocalEvent<MobStateChangedEvent>(target, args, true);
    this.Dirty(target, (IComponent) component);
  }

  private void SubscribeEvents()
  {
    this.SubscribeLocalEvent<MobStateComponent, BeforeGettingStrippedEvent>(new ComponentEventHandler<MobStateComponent, BeforeGettingStrippedEvent>(this.OnGettingStripped));
    this.SubscribeLocalEvent<MobStateComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<MobStateComponent, ChangeDirectionAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, UseAttemptEvent>(new ComponentEventHandler<MobStateComponent, UseAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, AttackAttemptEvent>(new ComponentEventHandler<MobStateComponent, AttackAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, ConsciousAttemptEvent>(new EntityEventRefHandler<MobStateComponent, ConsciousAttemptEvent>(this.CheckConcious));
    this.SubscribeLocalEvent<MobStateComponent, ThrowAttemptEvent>(new ComponentEventHandler<MobStateComponent, ThrowAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, SpeakAttemptEvent>(new ComponentEventHandler<MobStateComponent, SpeakAttemptEvent>(this.OnSpeakAttempt));
    this.SubscribeLocalEvent<MobStateComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<MobStateComponent, IsEquippingAttemptEvent>(this.OnEquipAttempt));
    this.SubscribeLocalEvent<MobStateComponent, EmoteAttemptEvent>(new ComponentEventHandler<MobStateComponent, EmoteAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<MobStateComponent, IsUnequippingAttemptEvent>(this.OnUnequipAttempt));
    this.SubscribeLocalEvent<MobStateComponent, DropAttemptEvent>(new ComponentEventHandler<MobStateComponent, DropAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, PickupAttemptEvent>(new ComponentEventHandler<MobStateComponent, PickupAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, StartPullAttemptEvent>(new ComponentEventHandler<MobStateComponent, StartPullAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, UpdateCanMoveEvent>(new ComponentEventHandler<MobStateComponent, UpdateCanMoveEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, StandAttemptEvent>(new ComponentEventHandler<MobStateComponent, StandAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, PointAttemptEvent>(new ComponentEventHandler<MobStateComponent, PointAttemptEvent>(this.CheckAct));
    this.SubscribeLocalEvent<MobStateComponent, TryingToSleepEvent>(new ComponentEventRefHandler<MobStateComponent, TryingToSleepEvent>(this.OnSleepAttempt));
    this.SubscribeLocalEvent<MobStateComponent, CombatModeShouldHandInteractEvent>(new ComponentEventRefHandler<MobStateComponent, CombatModeShouldHandInteractEvent>(this.OnCombatModeShouldHandInteract));
    this.SubscribeLocalEvent<MobStateComponent, AttemptPacifiedAttackEvent>(new EntityEventRefHandler<MobStateComponent, AttemptPacifiedAttackEvent>(this.OnAttemptPacifiedAttack));
    this.SubscribeLocalEvent<MobStateComponent, DamageModifyEvent>(new EntityEventRefHandler<MobStateComponent, DamageModifyEvent>(this.OnDamageModify));
    this.SubscribeLocalEvent<MobStateComponent, UnbuckleAttemptEvent>(new EntityEventRefHandler<MobStateComponent, UnbuckleAttemptEvent>(this.OnUnbuckleAttempt));
  }

  private void OnUnbuckleAttempt(Entity<MobStateComponent> ent, ref UnbuckleAttemptEvent args)
  {
    EntityUid? user = args.User;
    EntityUid owner = ent.Owner;
    if ((user.HasValue ? (user.GetValueOrDefault() == owner ? 1 : 0) : 0) == 0 || !this.IsIncapacitated((EntityUid) ent))
      return;
    args.Cancelled = true;
  }

  private void CheckConcious(Entity<MobStateComponent> ent, ref ConsciousAttemptEvent args)
  {
    switch (ent.Comp.CurrentState)
    {
      case MobState.Critical:
      case MobState.Dead:
        args.Cancelled = true;
        break;
    }
  }

  private void OnStateExitSubscribers(
    EntityUid target,
    MobStateComponent component,
    MobState state)
  {
    switch (state)
    {
      case MobState.Invalid:
        break;
      case MobState.Alive:
        break;
      case MobState.Critical:
        this._standing.Stand(target);
        break;
      case MobState.Dead:
        this.RemComp<CollisionWakeComponent>(target);
        this._standing.Stand(target);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void OnStateEnteredSubscribers(
    EntityUid target,
    MobStateComponent component,
    MobState state)
  {
    if (this._timing.ApplyingState)
      return;
    this._blocker.UpdateCanMove(target);
    switch (state)
    {
      case MobState.Invalid:
        break;
      case MobState.Alive:
        this._standing.Stand(target);
        this._appearance.SetData(target, (Enum) MobStateVisuals.State, (object) MobState.Alive);
        break;
      case MobState.Critical:
        this._standing.Down(target);
        this._appearance.SetData(target, (Enum) MobStateVisuals.State, (object) MobState.Critical);
        break;
      case MobState.Dead:
        this.EnsureComp<CollisionWakeComponent>(target);
        this._standing.Down(target);
        this._appearance.SetData(target, (Enum) MobStateVisuals.State, (object) MobState.Dead);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void OnSleepAttempt(
    EntityUid target,
    MobStateComponent component,
    ref TryingToSleepEvent args)
  {
    if (!this.IsDead(target, component))
      return;
    args.Cancelled = true;
  }

  private void OnGettingStripped(
    EntityUid target,
    MobStateComponent component,
    BeforeGettingStrippedEvent args)
  {
    if (this.IsDead(target, component))
    {
      args.Multiplier /= 3f;
    }
    else
    {
      if (!this.IsCritical(target, component))
        return;
      args.Multiplier /= 2f;
    }
  }

  private void OnSpeakAttempt(EntityUid uid, MobStateComponent component, SpeakAttemptEvent args)
  {
    if (this.HasComp<AllowNextCritSpeechComponent>(uid))
      this.RemCompDeferred<AllowNextCritSpeechComponent>(uid);
    else
      this.CheckAct(uid, component, (CancellableEntityEventArgs) args);
  }

  private void CheckAct(
    EntityUid target,
    MobStateComponent component,
    CancellableEntityEventArgs args)
  {
    switch (component.CurrentState)
    {
      case MobState.Critical:
      case MobState.Dead:
        args.Cancel();
        break;
    }
  }

  private void OnEquipAttempt(
    EntityUid target,
    MobStateComponent component,
    IsEquippingAttemptEvent args)
  {
    if (!(args.Equipee == target))
      return;
    this.CheckAct(target, component, (CancellableEntityEventArgs) args);
  }

  private void OnUnequipAttempt(
    EntityUid target,
    MobStateComponent component,
    IsUnequippingAttemptEvent args)
  {
    if (!(args.Unequipee == target))
      return;
    this.CheckAct(target, component, (CancellableEntityEventArgs) args);
  }

  private void OnCombatModeShouldHandInteract(
    EntityUid uid,
    MobStateComponent component,
    ref CombatModeShouldHandInteractEvent args)
  {
    if (this.IsDead(uid, component))
      return;
    args.Cancelled = true;
  }

  private void OnAttemptPacifiedAttack(
    Entity<MobStateComponent> ent,
    ref AttemptPacifiedAttackEvent args)
  {
    args.Cancelled = true;
  }

  private void OnDamageModify(Entity<MobStateComponent> ent, ref DamageModifyEvent args)
  {
    args.Damage *= this._damageable.UniversalMobDamageModifier;
  }
}
