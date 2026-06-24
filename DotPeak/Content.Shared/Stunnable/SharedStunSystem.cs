// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stunnable.SharedStunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using System;

#nullable enable
namespace Content.Shared.Stunnable;

public abstract class SharedStunSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private StandingStateSystem _standingState;
  [Dependency]
  private StatusEffectsSystem _statusEffect;
  public const float KnockDownModifier = 0.2f;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<KnockedDownComponent, ComponentInit>(new ComponentEventHandler<KnockedDownComponent, ComponentInit>(this.OnKnockInit));
    this.SubscribeLocalEvent<KnockedDownComponent, ComponentShutdown>(new ComponentEventHandler<KnockedDownComponent, ComponentShutdown>(this.OnKnockShutdown));
    this.SubscribeLocalEvent<KnockedDownComponent, StandAttemptEvent>(new ComponentEventHandler<KnockedDownComponent, StandAttemptEvent>(this.OnStandAttempt));
    this.SubscribeLocalEvent<SlowedDownComponent, ComponentInit>(new ComponentEventHandler<SlowedDownComponent, ComponentInit>(this.OnSlowInit));
    this.SubscribeLocalEvent<SlowedDownComponent, ComponentShutdown>(new ComponentEventHandler<SlowedDownComponent, ComponentShutdown>(this.OnSlowRemove));
    this.SubscribeLocalEvent<StunnedComponent, ComponentStartup>(new ComponentEventHandler<StunnedComponent, ComponentStartup>(this.UpdateCanMove));
    this.SubscribeLocalEvent<StunnedComponent, ComponentShutdown>(new ComponentEventHandler<StunnedComponent, ComponentShutdown>(this.UpdateCanMove));
    this.SubscribeLocalEvent<StunOnContactComponent, StartCollideEvent>(new EntityEventRefHandler<StunOnContactComponent, StartCollideEvent>(this.OnStunOnContactCollide));
    this.SubscribeLocalEvent<KnockedDownComponent, InteractHandEvent>(new ComponentEventHandler<KnockedDownComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<SlowedDownComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SlowedDownComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed));
    this.SubscribeLocalEvent<KnockedDownComponent, TileFrictionEvent>(new ComponentEventRefHandler<KnockedDownComponent, TileFrictionEvent>(this.OnKnockedTileFriction));
    this.SubscribeLocalEvent<StunnedComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<StunnedComponent, ChangeDirectionAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<StunnedComponent, UpdateCanMoveEvent>(new ComponentEventHandler<StunnedComponent, UpdateCanMoveEvent>(this.OnMoveAttempt));
    this.SubscribeLocalEvent<StunnedComponent, InteractionAttemptEvent>(new EntityEventRefHandler<StunnedComponent, InteractionAttemptEvent>(this.OnAttemptInteract));
    this.SubscribeLocalEvent<StunnedComponent, UseAttemptEvent>(new ComponentEventHandler<StunnedComponent, UseAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<StunnedComponent, ThrowAttemptEvent>(new ComponentEventHandler<StunnedComponent, ThrowAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<StunnedComponent, DropAttemptEvent>(new ComponentEventHandler<StunnedComponent, DropAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<StunnedComponent, AttackAttemptEvent>(new ComponentEventHandler<StunnedComponent, AttackAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<StunnedComponent, PickupAttemptEvent>(new ComponentEventHandler<StunnedComponent, PickupAttemptEvent>(this.OnAttempt));
    this.SubscribeLocalEvent<StunnedComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<StunnedComponent, IsEquippingAttemptEvent>(this.OnEquipAttempt));
    this.SubscribeLocalEvent<StunnedComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<StunnedComponent, IsUnequippingAttemptEvent>(this.OnUnequipAttempt));
    this.SubscribeLocalEvent<MobStateComponent, MobStateChangedEvent>(new ComponentEventHandler<MobStateComponent, MobStateChangedEvent>(this.OnMobStateChanged));
  }

  private void OnAttemptInteract(Entity<StunnedComponent> ent, ref InteractionAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnMobStateChanged(
    EntityUid uid,
    MobStateComponent component,
    MobStateChangedEvent args)
  {
    if (!this.TryComp<StatusEffectsComponent>(uid, out StatusEffectsComponent _))
      return;
    switch (args.NewMobState)
    {
      case MobState.Critical:
        this._statusEffect.TryRemoveStatusEffect(uid, "Stun");
        break;
      case MobState.Dead:
        this._statusEffect.TryRemoveStatusEffect(uid, "Stun");
        break;
    }
  }

  private void UpdateCanMove(EntityUid uid, StunnedComponent component, EntityEventArgs args)
  {
    this._blocker.UpdateCanMove(uid);
  }

  private void OnStunOnContactCollide(
    Entity<StunOnContactComponent> ent,
    ref StartCollideEvent args)
  {
    StatusEffectsComponent comp;
    if (args.OurFixtureId != ent.Comp.FixtureId || this._entityWhitelist.IsBlacklistPass(ent.Comp.Blacklist, args.OtherEntity) || !this.TryComp<StatusEffectsComponent>(args.OtherEntity, out comp))
      return;
    this.TryStun(args.OtherEntity, ent.Comp.Duration, true, comp);
    this.TryKnockdown(args.OtherEntity, ent.Comp.Duration, true, comp);
  }

  private void OnKnockInit(EntityUid uid, KnockedDownComponent component, ComponentInit args)
  {
    this._standingState.Down(uid);
  }

  private void OnKnockShutdown(
    EntityUid uid,
    KnockedDownComponent component,
    ComponentShutdown args)
  {
    this._standingState.Stand(uid);
  }

  private void OnStandAttempt(
    EntityUid uid,
    KnockedDownComponent component,
    StandAttemptEvent args)
  {
    if (component.LifeStage > ComponentLifeStage.Running)
      return;
    args.Cancel();
  }

  private void OnSlowInit(EntityUid uid, SlowedDownComponent component, ComponentInit args)
  {
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
  }

  private void OnSlowRemove(EntityUid uid, SlowedDownComponent component, ComponentShutdown args)
  {
    component.SprintSpeedModifier = 1f;
    component.WalkSpeedModifier = 1f;
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
  }

  private void OnRefreshMovespeed(
    EntityUid uid,
    SlowedDownComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
  }

  public bool TryStun(
    EntityUid uid,
    TimeSpan time,
    bool refresh,
    StatusEffectsComponent? status = null,
    bool force = false)
  {
    if (time <= TimeSpan.Zero || !this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this._statusEffect.TryAddStatusEffect<StunnedComponent>(uid, "Stun", time, refresh, force: force))
      return false;
    StunnedEvent args = new StunnedEvent();
    this.RaiseLocalEvent<StunnedEvent>(uid, ref args);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(21, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "user", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" stunned for ");
    logStringHandler.AppendFormatted<int>(time.Seconds, "time.Seconds");
    logStringHandler.AppendLiteral(" seconds");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stamina, LogImpact.Medium, ref local);
    return true;
  }

  public bool TryKnockdown(
    EntityUid uid,
    TimeSpan time,
    bool refresh,
    StatusEffectsComponent? status = null,
    bool force = false)
  {
    if (time <= TimeSpan.Zero || !this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this._statusEffect.TryAddStatusEffect<KnockedDownComponent>(uid, "KnockedDown", time, refresh, force: force))
      return false;
    KnockedDownEvent args = new KnockedDownEvent();
    this.RaiseLocalEvent<KnockedDownEvent>(uid, ref args);
    return true;
  }

  public bool TryParalyze(
    EntityUid uid,
    TimeSpan time,
    bool refresh,
    StatusEffectsComponent? status = null,
    bool force = false)
  {
    return this.Resolve<StatusEffectsComponent>(uid, ref status, false) && this.TryKnockdown(uid, time, refresh, status, force) && this.TryStun(uid, time, refresh, status, force);
  }

  public bool TrySlowdown(
    EntityUid uid,
    TimeSpan time,
    bool refresh,
    float walkSpeedMultiplier = 1f,
    float runSpeedMultiplier = 1f,
    StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || time <= TimeSpan.Zero || !this._statusEffect.TryAddStatusEffect<SlowedDownComponent>(uid, "SlowedDown", time, refresh, status))
      return false;
    SlowedDownComponent slowedDownComponent = this.Comp<SlowedDownComponent>(uid);
    walkSpeedMultiplier = Math.Clamp(walkSpeedMultiplier, 0.0f, 1f);
    runSpeedMultiplier = Math.Clamp(runSpeedMultiplier, 0.0f, 1f);
    slowedDownComponent.WalkSpeedModifier *= walkSpeedMultiplier;
    slowedDownComponent.SprintSpeedModifier *= runSpeedMultiplier;
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
    return true;
  }

  public void UpdateStunModifiers(
    Entity<StaminaComponent?> ent,
    float walkSpeedModifier = 1f,
    float runSpeedModifier = 1f)
  {
    if (!this.Resolve<StaminaComponent>((EntityUid) ent, ref ent.Comp))
      return;
    if (MathHelper.CloseTo(walkSpeedModifier, 1f, 1E-07f) && MathHelper.CloseTo(runSpeedModifier, 1f, 1E-07f) && (double) ent.Comp.StaminaDamage == 0.0 || (double) walkSpeedModifier == 0.0 && (double) runSpeedModifier == 0.0)
    {
      this.RemComp<SlowedDownComponent>((EntityUid) ent);
    }
    else
    {
      SlowedDownComponent comp;
      this.EnsureComp<SlowedDownComponent>((EntityUid) ent, out comp);
      comp.WalkSpeedModifier = walkSpeedModifier;
      comp.SprintSpeedModifier = runSpeedModifier;
      this._movementSpeedModifier.RefreshMovementSpeedModifiers((EntityUid) ent);
      this.Dirty<StaminaComponent>(ent);
    }
  }

  public void UpdateStunModifiers(Entity<StaminaComponent?> ent, float speedModifier = 1f)
  {
    this.UpdateStunModifiers(ent, speedModifier, speedModifier);
  }

  private void OnInteractHand(EntityUid uid, KnockedDownComponent knocked, InteractHandEvent args)
  {
    if (args.Handled || (double) knocked.HelpTimer > 0.0 || this.HasComp<SleepingComponent>(uid))
      return;
    knocked.HelpTimer = knocked.HelpInterval / 2f;
    this._statusEffect.TryRemoveTime(uid, "KnockedDown", TimeSpan.FromSeconds((double) knocked.HelpInterval));
    this._audio.PlayPredicted(knocked.StunAttemptSound, uid, new EntityUid?(args.User));
    this.Dirty(uid, (IComponent) knocked);
    args.Handled = true;
  }

  private void OnKnockedTileFriction(
    EntityUid uid,
    KnockedDownComponent component,
    ref TileFrictionEvent args)
  {
  }

  private void OnMoveAttempt(EntityUid uid, StunnedComponent stunned, UpdateCanMoveEvent args)
  {
    if (stunned.LifeStage > ComponentLifeStage.Running)
      return;
    args.Cancel();
  }

  private void OnAttempt(EntityUid uid, StunnedComponent stunned, CancellableEntityEventArgs args)
  {
    args.Cancel();
  }

  private void OnEquipAttempt(
    EntityUid uid,
    StunnedComponent stunned,
    IsEquippingAttemptEvent args)
  {
    if (!(args.Equipee == uid))
      return;
    args.Cancel();
  }

  private void OnUnequipAttempt(
    EntityUid uid,
    StunnedComponent stunned,
    IsUnequippingAttemptEvent args)
  {
    if (!(args.Unequipee == uid))
      return;
    args.Cancel();
  }
}
