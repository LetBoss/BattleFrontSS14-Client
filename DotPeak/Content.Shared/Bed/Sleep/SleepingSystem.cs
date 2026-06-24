// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Sleep.SleepingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.ForceSay;
using Content.Shared.Emoting;
using Content.Shared.Examine;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Pointing;
using Content.Shared.Popups;
using Content.Shared.Slippery;
using Content.Shared.Sound;
using Content.Shared.Sound.Components;
using Content.Shared.Speech;
using Content.Shared.StatusEffect;
using Content.Shared.StatusEffectNew;
using Content.Shared.Stunnable;
using Content.Shared.Traits.Assorted;
using Content.Shared.Verbs;
using Content.Shared.Zombies;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Bed.Sleep;

public sealed class SleepingSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private BlindableSystem _blindableSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedEmitSoundSystem _emitSound;
  [Dependency]
  private StatusEffectsSystem _statusEffectOld;
  [Dependency]
  private SharedStatusEffectsSystem _statusEffectNew;
  public static readonly EntProtoId SleepActionId = EntProtoId.op_Implicit("ActionSleep");
  public static readonly EntProtoId WakeActionId = EntProtoId.op_Implicit("ActionWake");
  public static readonly EntProtoId StatusEffectForcedSleeping = EntProtoId.op_Implicit(nameof (StatusEffectForcedSleeping));

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, SleepActionEvent>(new EntityEventRefHandler<ActionsContainerComponent, SleepActionEvent>((object) this, __methodptr(OnBedSleepAction)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MobStateComponent, SleepStateChangedEvent>(new EntityEventRefHandler<MobStateComponent, SleepStateChangedEvent>((object) this, __methodptr(OnSleepStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MobStateComponent, WakeActionEvent>(new EntityEventRefHandler<MobStateComponent, WakeActionEvent>((object) this, __methodptr(OnWakeAction)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MobStateComponent, SleepActionEvent>(new EntityEventRefHandler<MobStateComponent, SleepActionEvent>((object) this, __methodptr(OnSleepAction)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, DamageChangedEvent>(new EntityEventRefHandler<SleepingComponent, DamageChangedEvent>((object) this, __methodptr(OnDamageChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, EntityZombifiedEvent>(new EntityEventRefHandler<SleepingComponent, EntityZombifiedEvent>((object) this, __methodptr(OnZombified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, MobStateChangedEvent>(new EntityEventRefHandler<SleepingComponent, MobStateChangedEvent>((object) this, __methodptr(OnMobStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, MapInitEvent>(new EntityEventRefHandler<SleepingComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, SpeakAttemptEvent>(new EntityEventRefHandler<SleepingComponent, SpeakAttemptEvent>((object) this, __methodptr(OnSpeakAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, CanSeeAttemptEvent>(new EntityEventRefHandler<SleepingComponent, CanSeeAttemptEvent>((object) this, __methodptr(OnSeeAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, PointAttemptEvent>(new EntityEventRefHandler<SleepingComponent, PointAttemptEvent>((object) this, __methodptr(OnPointAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, SlipAttemptEvent>(new EntityEventRefHandler<SleepingComponent, SlipAttemptEvent>((object) this, __methodptr(OnSlip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, ConsciousAttemptEvent>(new EntityEventRefHandler<SleepingComponent, ConsciousAttemptEvent>((object) this, __methodptr(OnConsciousAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, ExaminedEvent>(new EntityEventRefHandler<SleepingComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<SleepingComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddWakeVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, InteractHandEvent>(new EntityEventRefHandler<SleepingComponent, InteractHandEvent>((object) this, __methodptr(OnInteractHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ForcedSleepingStatusEffectComponent, StatusEffectAppliedEvent>(new EntityEventRefHandler<ForcedSleepingStatusEffectComponent, StatusEffectAppliedEvent>((object) this, __methodptr(OnStatusEffectApplied)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, UnbuckleAttemptEvent>(new EntityEventRefHandler<SleepingComponent, UnbuckleAttemptEvent>((object) this, __methodptr(OnUnbuckleAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, EmoteAttemptEvent>(new EntityEventRefHandler<SleepingComponent, EmoteAttemptEvent>((object) this, __methodptr(OnEmoteAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SleepingComponent, BeforeForceSayEvent>(new EntityEventRefHandler<SleepingComponent, BeforeForceSayEvent>((object) this, __methodptr(OnChangeForceSay)), (Type[]) null, new Type[1]
    {
      typeof (PainNumbnessSystem)
    });
  }

  private void OnUnbuckleAttempt(Entity<SleepingComponent> ent, ref UnbuckleAttemptEvent args)
  {
    EntityUid owner = ent.Owner;
    EntityUid? user = args.User;
    if ((user.HasValue ? (EntityUid.op_Equality(owner, user.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  private void OnBedSleepAction(Entity<ActionsContainerComponent> ent, ref SleepActionEvent args)
  {
    this.TrySleeping(Entity<MobStateComponent>.op_Implicit(args.Performer));
  }

  private void OnWakeAction(Entity<MobStateComponent> ent, ref WakeActionEvent args)
  {
    if (!this.TryWakeWithCooldown(Entity<SleepingComponent>.op_Implicit(ent.Owner)))
      return;
    args.Handled = true;
  }

  private void OnSleepAction(Entity<MobStateComponent> ent, ref SleepActionEvent args)
  {
    this.TrySleeping(Entity<MobStateComponent>.op_Implicit((Entity<MobStateComponent>.op_Implicit(ent), ent.Comp)));
  }

  private void OnSleepStateChanged(Entity<MobStateComponent> ent, ref SleepStateChangedEvent args)
  {
    if (args.FellAsleep)
    {
      this._statusEffectOld.TryRemoveStatusEffect(ent.Owner, "Stun");
      this._statusEffectOld.TryRemoveStatusEffect(ent.Owner, "KnockedDown");
      this.EnsureComp<StunnedComponent>(Entity<MobStateComponent>.op_Implicit(ent));
      this.EnsureComp<KnockedDownComponent>(Entity<MobStateComponent>.op_Implicit(ent));
      SleepEmitSoundComponent emitSoundComponent1;
      if (!this.TryComp<SleepEmitSoundComponent>(Entity<MobStateComponent>.op_Implicit(ent), ref emitSoundComponent1))
        return;
      SpamEmitSoundComponent emitSoundComponent2 = this.EnsureComp<SpamEmitSoundComponent>(Entity<MobStateComponent>.op_Implicit(ent));
      if (this.HasComp<SnoringComponent>(Entity<MobStateComponent>.op_Implicit(ent)))
        emitSoundComponent2.Sound = emitSoundComponent1.Snore;
      emitSoundComponent2.MinInterval = emitSoundComponent1.Interval;
      emitSoundComponent2.MaxInterval = emitSoundComponent1.MaxInterval;
      emitSoundComponent2.PopUp = new LocId?(emitSoundComponent1.PopUp);
      this.Dirty(ent.Owner, (IComponent) emitSoundComponent2, (MetaDataComponent) null);
    }
    else
    {
      this.RemComp<StunnedComponent>(Entity<MobStateComponent>.op_Implicit(ent));
      this.RemComp<KnockedDownComponent>(Entity<MobStateComponent>.op_Implicit(ent));
      this.RemComp<SpamEmitSoundComponent>(Entity<MobStateComponent>.op_Implicit(ent));
    }
  }

  private void OnMapInit(Entity<SleepingComponent> ent, ref MapInitEvent args)
  {
    SleepStateChangedEvent stateChangedEvent = new SleepStateChangedEvent(true);
    this.RaiseLocalEvent<SleepStateChangedEvent>(Entity<SleepingComponent>.op_Implicit(ent), ref stateChangedEvent, false);
    this._blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(ent.Owner));
    this._actionsSystem.AddAction(Entity<SleepingComponent>.op_Implicit(ent), ref ent.Comp.WakeAction, EntProtoId.op_Implicit(SleepingSystem.WakeActionId), Entity<SleepingComponent>.op_Implicit(ent));
  }

  private void OnSpeakAttempt(Entity<SleepingComponent> ent, ref SpeakAttemptEvent args)
  {
    if (this.HasComp<AllowNextCritSpeechComponent>(Entity<SleepingComponent>.op_Implicit(ent)))
      this.RemCompDeferred<AllowNextCritSpeechComponent>(Entity<SleepingComponent>.op_Implicit(ent));
    else
      args.Cancel();
  }

  private void OnSeeAttempt(Entity<SleepingComponent> ent, ref CanSeeAttemptEvent args)
  {
    if (ent.Comp.LifeStage > 6)
      return;
    args.Cancel();
  }

  private void OnPointAttempt(Entity<SleepingComponent> ent, ref PointAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnSlip(Entity<SleepingComponent> ent, ref SlipAttemptEvent args)
  {
    args.NoSlip = true;
  }

  private void OnConsciousAttempt(Entity<SleepingComponent> ent, ref ConsciousAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnExamined(Entity<SleepingComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("sleep-examined", ("target", (object) Identity.Entity(Entity<SleepingComponent>.op_Implicit(ent), (IEntityManager) this.EntityManager))));
  }

  private void AddWakeVerb(Entity<SleepingComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    EntityUid target = args.Target;
    EntityUid user = args.User;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryWakeWithCooldown(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)), new EntityUid?(user)));
    alternativeVerb1.Text = this.Loc.GetString("action-name-wake");
    alternativeVerb1.Priority = 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void OnInteractHand(Entity<SleepingComponent> ent, ref InteractHandEvent args)
  {
    args.Handled = true;
    this.TryWakeWithCooldown(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)), new EntityUid?(args.User));
  }

  private void OnDamageChanged(Entity<SleepingComponent> ent, ref DamageChangedEvent args)
  {
    if (!args.DamageIncreased || args.DamageDelta == null || !(args.DamageDelta.GetTotal() >= ent.Comp.WakeThreshold))
      return;
    this.TryWaking(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)));
  }

  private void OnZombified(Entity<SleepingComponent> ent, ref EntityZombifiedEvent args)
  {
    this.TryWaking(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)), true);
  }

  private void OnMobStateChanged(Entity<SleepingComponent> ent, ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Dead)
    {
      this.RemComp<SpamEmitSoundComponent>(Entity<SleepingComponent>.op_Implicit(ent));
      this.RemComp<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent));
    }
    else
    {
      SpamEmitSoundComponent emitSoundComponent;
      if (!this.TryComp<SpamEmitSoundComponent>(Entity<SleepingComponent>.op_Implicit(ent), ref emitSoundComponent))
        return;
      this._emitSound.SetEnabled(Entity<SpamEmitSoundComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), emitSoundComponent)), args.NewMobState == MobState.Alive);
    }
  }

  private void OnStatusEffectApplied(
    Entity<ForcedSleepingStatusEffectComponent> ent,
    ref StatusEffectAppliedEvent args)
  {
    this.TrySleeping(Entity<MobStateComponent>.op_Implicit(args.Target));
  }

  private void Wake(Entity<SleepingComponent> ent)
  {
    this.RemComp<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent));
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ent.Owner);
    EntityUid? wakeAction = ent.Comp.WakeAction;
    Entity<ActionComponent>? action = wakeAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(wakeAction.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
    SleepStateChangedEvent stateChangedEvent = new SleepStateChangedEvent(false);
    this.RaiseLocalEvent<SleepStateChangedEvent>(Entity<SleepingComponent>.op_Implicit(ent), ref stateChangedEvent, false);
    this._blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(ent.Owner));
  }

  public bool TrySleeping(Entity<MobStateComponent?> ent)
  {
    if (!this.Resolve<MobStateComponent>(Entity<MobStateComponent>.op_Implicit(ent), ref ent.Comp, false))
      return false;
    TryingToSleepEvent tryingToSleepEvent = new TryingToSleepEvent(Entity<MobStateComponent>.op_Implicit(ent));
    this.RaiseLocalEvent<TryingToSleepEvent>(Entity<MobStateComponent>.op_Implicit(ent), ref tryingToSleepEvent, false);
    if (tryingToSleepEvent.Cancelled)
      return false;
    this.EnsureComp<SleepingComponent>(Entity<MobStateComponent>.op_Implicit(ent));
    return true;
  }

  public bool TryWakeWithCooldown(Entity<SleepingComponent?> ent, EntityUid? user = null)
  {
    if (!this.Resolve<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent), ref ent.Comp, false))
      return false;
    TimeSpan curTime = this._gameTiming.CurTime;
    if (curTime < ent.Comp.CooldownEnd)
      return false;
    ent.Comp.CooldownEnd = curTime + ent.Comp.Cooldown;
    this.Dirty(Entity<SleepingComponent>.op_Implicit(ent), (IComponent) ent.Comp, (MetaDataComponent) null);
    return this.TryWaking(ent, user: user);
  }

  public bool TryWaking(Entity<SleepingComponent?> ent, bool force = false, EntityUid? user = null)
  {
    if (!this.Resolve<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent), ref ent.Comp, false))
      return false;
    if (!force && this._statusEffectNew.HasEffectComp<ForcedSleepingStatusEffectComponent>(new EntityUid?(Entity<SleepingComponent>.op_Implicit(ent))))
    {
      if (user.HasValue)
      {
        this._audio.PlayPredicted(ent.Comp.WakeAttemptSound, Entity<SleepingComponent>.op_Implicit(ent), user, new AudioParams?());
        this._popupSystem.PopupClient(this.Loc.GetString("wake-other-failure", ("target", (object) Identity.Entity(Entity<SleepingComponent>.op_Implicit(ent), (IEntityManager) this.EntityManager))), Entity<SleepingComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
      }
      return false;
    }
    if (user.HasValue)
    {
      this._audio.PlayPredicted(ent.Comp.WakeAttemptSound, Entity<SleepingComponent>.op_Implicit(ent), user, new AudioParams?());
      this._popupSystem.PopupClient(this.Loc.GetString("wake-other-success", ("target", (object) Identity.Entity(Entity<SleepingComponent>.op_Implicit(ent), (IEntityManager) this.EntityManager))), Entity<SleepingComponent>.op_Implicit(ent), user);
    }
    this.Wake(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)));
    return true;
  }

  public void OnEmoteAttempt(Entity<SleepingComponent> ent, ref EmoteAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnChangeForceSay(Entity<SleepingComponent> ent, ref BeforeForceSayEvent args)
  {
    args.Prefix = ent.Comp.ForceSaySleepDataset;
  }
}
