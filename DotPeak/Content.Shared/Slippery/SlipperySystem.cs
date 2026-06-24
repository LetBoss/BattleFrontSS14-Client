// Decompiled with JetBrains decompiler
// Type: Content.Shared.Slippery.SlipperySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Content.Shared.Slippery;

public sealed class SlipperySystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SpeedModifierContactsSystem _speedModifier;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SlipperyComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<SlipperyComponent, StepTriggerAttemptEvent>(this.HandleAttemptCollide));
    this.SubscribeLocalEvent<SlipperyComponent, StepTriggeredOffEvent>(new ComponentEventRefHandler<SlipperyComponent, StepTriggeredOffEvent>(this.HandleStepTrigger));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.SubscribeLocalEvent<NoSlipComponent, SlipAttemptEvent>(SlipperySystem.\u003C\u003EO.\u003C0\u003E__OnNoSlipAttempt ?? (SlipperySystem.\u003C\u003EO.\u003C0\u003E__OnNoSlipAttempt = new ComponentEventHandler<NoSlipComponent, SlipAttemptEvent>(SlipperySystem.OnNoSlipAttempt)));
    this.SubscribeLocalEvent<SlowedOverSlipperyComponent, SlipAttemptEvent>(new ComponentEventHandler<SlowedOverSlipperyComponent, SlipAttemptEvent>(this.OnSlowedOverSlipAttempt));
    this.SubscribeLocalEvent<ThrownItemComponent, SlipCausingAttemptEvent>(new ComponentEventRefHandler<ThrownItemComponent, SlipCausingAttemptEvent>(this.OnThrownSlipAttempt));
    this.SubscribeLocalEvent<NoSlipComponent, InventoryRelayedEvent<SlipAttemptEvent>>((ComponentEventHandler<NoSlipComponent, InventoryRelayedEvent<SlipAttemptEvent>>) ((e, c, ev) => SlipperySystem.OnNoSlipAttempt(e, c, ev.Args)));
    this.SubscribeLocalEvent<SlowedOverSlipperyComponent, InventoryRelayedEvent<SlipAttemptEvent>>((ComponentEventHandler<SlowedOverSlipperyComponent, InventoryRelayedEvent<SlipAttemptEvent>>) ((e, c, ev) => this.OnSlowedOverSlipAttempt(e, c, ev.Args)));
    this.SubscribeLocalEvent<SlowedOverSlipperyComponent, InventoryRelayedEvent<GetSlowedOverSlipperyModifierEvent>>(new ComponentEventRefHandler<SlowedOverSlipperyComponent, InventoryRelayedEvent<GetSlowedOverSlipperyModifierEvent>>(this.OnGetSlowedOverSlipperyModifier));
    this.SubscribeLocalEvent<SlipperyComponent, EndCollideEvent>(new ComponentEventRefHandler<SlipperyComponent, EndCollideEvent>(this.OnEntityExit));
  }

  private void HandleStepTrigger(
    EntityUid uid,
    SlipperyComponent component,
    ref StepTriggeredOffEvent args)
  {
    this.TrySlip(uid, component, args.Tripper);
  }

  private void HandleAttemptCollide(
    EntityUid uid,
    SlipperyComponent component,
    ref StepTriggerAttemptEvent args)
  {
    args.Continue |= this.CanSlip(uid, args.Tripper);
  }

  private static void OnNoSlipAttempt(
    EntityUid uid,
    NoSlipComponent component,
    SlipAttemptEvent args)
  {
    args.NoSlip = true;
  }

  private void OnSlowedOverSlipAttempt(
    EntityUid uid,
    SlowedOverSlipperyComponent component,
    SlipAttemptEvent args)
  {
    args.SlowOverSlippery = true;
  }

  private void OnThrownSlipAttempt(
    EntityUid uid,
    ThrownItemComponent comp,
    ref SlipCausingAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnGetSlowedOverSlipperyModifier(
    EntityUid uid,
    SlowedOverSlipperyComponent comp,
    ref InventoryRelayedEvent<GetSlowedOverSlipperyModifierEvent> args)
  {
    args.Args.SlowdownModifier *= comp.SlowdownModifier;
  }

  private void OnEntityExit(EntityUid uid, SlipperyComponent component, ref EndCollideEvent args)
  {
    if (!this.HasComp<SpeedModifiedByContactComponent>(args.OtherEntity))
      return;
    this._speedModifier.AddModifiedEntity(args.OtherEntity);
  }

  private bool CanSlip(EntityUid uid, EntityUid toSlip)
  {
    return !this._container.IsEntityInContainer(uid) && this._statusEffects.CanApplyEffect(toSlip, "Stun");
  }

  public void TrySlip(
    EntityUid uid,
    SlipperyComponent component,
    EntityUid other,
    bool requiresContact = true)
  {
    if (this.HasComp<KnockedDownComponent>(other) && !component.SlipData.SuperSlippery)
      return;
    SlipAttemptEvent args1 = new SlipAttemptEvent(new EntityUid?(uid));
    this.RaiseLocalEvent<SlipAttemptEvent>(other, args1);
    if (args1.SlowOverSlippery)
      this._speedModifier.AddModifiedEntity(other);
    if (args1.NoSlip)
      return;
    SlipCausingAttemptEvent args2 = new SlipCausingAttemptEvent();
    this.RaiseLocalEvent<SlipCausingAttemptEvent>(uid, ref args2);
    if (args2.Cancelled)
      return;
    SlipEvent args3 = new SlipEvent(other);
    this.RaiseLocalEvent<SlipEvent>(uid, ref args3);
    PhysicsComponent comp;
    if (this.TryComp<PhysicsComponent>(other, out comp) && !this.HasComp<SlidingComponent>(other))
    {
      this._physics.SetLinearVelocity(other, comp.LinearVelocity * component.SlipData.LaunchForwardsMultiplier, body: comp);
      if (component.SlipData.SuperSlippery & requiresContact)
        this.EnsureComp<SlidingComponent>(other).CollidingEntities.Add(uid);
    }
    int num = !this._statusEffects.HasStatusEffect(other, "KnockedDown") ? 1 : 0;
    this._stun.TryParalyze(other, component.SlipData.ParalyzeTime, true);
    if (num != 0)
      this._audio.PlayPredicted(component.SlipSound, other, new EntityUid?(other));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(27, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) other), "mob", "ToPrettyString(other)");
    logStringHandler.AppendLiteral(" slipped on collision with ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "entity", "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Slip, LogImpact.Low, ref local);
  }
}
