using System;
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
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

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

	public static readonly EntProtoId StatusEffectForcedSleeping = EntProtoId.op_Implicit("StatusEffectForcedSleeping");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActionsContainerComponent, SleepActionEvent>((EntityEventRefHandler<ActionsContainerComponent, SleepActionEvent>)OnBedSleepAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, SleepStateChangedEvent>((EntityEventRefHandler<MobStateComponent, SleepStateChangedEvent>)OnSleepStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, WakeActionEvent>((EntityEventRefHandler<MobStateComponent, WakeActionEvent>)OnWakeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, SleepActionEvent>((EntityEventRefHandler<MobStateComponent, SleepActionEvent>)OnSleepAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, DamageChangedEvent>((EntityEventRefHandler<SleepingComponent, DamageChangedEvent>)OnDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, EntityZombifiedEvent>((EntityEventRefHandler<SleepingComponent, EntityZombifiedEvent>)OnZombified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, MobStateChangedEvent>((EntityEventRefHandler<SleepingComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, MapInitEvent>((EntityEventRefHandler<SleepingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, SpeakAttemptEvent>((EntityEventRefHandler<SleepingComponent, SpeakAttemptEvent>)OnSpeakAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, CanSeeAttemptEvent>((EntityEventRefHandler<SleepingComponent, CanSeeAttemptEvent>)OnSeeAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, PointAttemptEvent>((EntityEventRefHandler<SleepingComponent, PointAttemptEvent>)OnPointAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, SlipAttemptEvent>((EntityEventRefHandler<SleepingComponent, SlipAttemptEvent>)OnSlip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, ConsciousAttemptEvent>((EntityEventRefHandler<SleepingComponent, ConsciousAttemptEvent>)OnConsciousAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, ExaminedEvent>((EntityEventRefHandler<SleepingComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<SleepingComponent, GetVerbsEvent<AlternativeVerb>>)AddWakeVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, InteractHandEvent>((EntityEventRefHandler<SleepingComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ForcedSleepingStatusEffectComponent, StatusEffectAppliedEvent>((EntityEventRefHandler<ForcedSleepingStatusEffectComponent, StatusEffectAppliedEvent>)OnStatusEffectApplied, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, UnbuckleAttemptEvent>((EntityEventRefHandler<SleepingComponent, UnbuckleAttemptEvent>)OnUnbuckleAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, EmoteAttemptEvent>((EntityEventRefHandler<SleepingComponent, EmoteAttemptEvent>)OnEmoteAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SleepingComponent, BeforeForceSayEvent>((EntityEventRefHandler<SleepingComponent, BeforeForceSayEvent>)OnChangeForceSay, (Type[])null, new Type[1] { typeof(PainNumbnessSystem) });
	}

	private void OnUnbuckleAttempt(Entity<SleepingComponent> ent, ref UnbuckleAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = ent.Owner;
		EntityUid? user = args.User;
		if (user.HasValue && owner == user.GetValueOrDefault())
		{
			args.Cancelled = true;
		}
	}

	private void OnBedSleepAction(Entity<ActionsContainerComponent> ent, ref SleepActionEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TrySleeping(Entity<MobStateComponent>.op_Implicit(args.Performer));
	}

	private void OnWakeAction(Entity<MobStateComponent> ent, ref WakeActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (TryWakeWithCooldown(Entity<SleepingComponent>.op_Implicit(ent.Owner)))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnSleepAction(Entity<MobStateComponent> ent, ref SleepActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TrySleeping(Entity<MobStateComponent>.op_Implicit((Entity<MobStateComponent>.op_Implicit(ent), ent.Comp)));
	}

	private void OnSleepStateChanged(Entity<MobStateComponent> ent, ref SleepStateChangedEvent args)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (args.FellAsleep)
		{
			_statusEffectOld.TryRemoveStatusEffect(ent.Owner, "Stun");
			_statusEffectOld.TryRemoveStatusEffect(ent.Owner, "KnockedDown");
			((EntitySystem)this).EnsureComp<StunnedComponent>(Entity<MobStateComponent>.op_Implicit(ent));
			((EntitySystem)this).EnsureComp<KnockedDownComponent>(Entity<MobStateComponent>.op_Implicit(ent));
			SleepEmitSoundComponent sleepSound = default(SleepEmitSoundComponent);
			if (((EntitySystem)this).TryComp<SleepEmitSoundComponent>(Entity<MobStateComponent>.op_Implicit(ent), ref sleepSound))
			{
				SpamEmitSoundComponent emitSound = ((EntitySystem)this).EnsureComp<SpamEmitSoundComponent>(Entity<MobStateComponent>.op_Implicit(ent));
				if (((EntitySystem)this).HasComp<SnoringComponent>(Entity<MobStateComponent>.op_Implicit(ent)))
				{
					emitSound.Sound = sleepSound.Snore;
				}
				emitSound.MinInterval = sleepSound.Interval;
				emitSound.MaxInterval = sleepSound.MaxInterval;
				emitSound.PopUp = sleepSound.PopUp;
				((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)emitSound, (MetaDataComponent)null);
			}
		}
		else
		{
			((EntitySystem)this).RemComp<StunnedComponent>(Entity<MobStateComponent>.op_Implicit(ent));
			((EntitySystem)this).RemComp<KnockedDownComponent>(Entity<MobStateComponent>.op_Implicit(ent));
			((EntitySystem)this).RemComp<SpamEmitSoundComponent>(Entity<MobStateComponent>.op_Implicit(ent));
		}
	}

	private void OnMapInit(Entity<SleepingComponent> ent, ref MapInitEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		SleepStateChangedEvent ev = new SleepStateChangedEvent(FellAsleep: true);
		((EntitySystem)this).RaiseLocalEvent<SleepStateChangedEvent>(Entity<SleepingComponent>.op_Implicit(ent), ref ev, false);
		_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(ent.Owner));
		_actionsSystem.AddAction(Entity<SleepingComponent>.op_Implicit(ent), ref ent.Comp.WakeAction, EntProtoId.op_Implicit(WakeActionId), Entity<SleepingComponent>.op_Implicit(ent));
	}

	private void OnSpeakAttempt(Entity<SleepingComponent> ent, ref SpeakAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<AllowNextCritSpeechComponent>(Entity<SleepingComponent>.op_Implicit(ent)))
		{
			((EntitySystem)this).RemCompDeferred<AllowNextCritSpeechComponent>(Entity<SleepingComponent>.op_Implicit(ent));
		}
		else
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnSeeAttempt(Entity<SleepingComponent> ent, ref CanSeeAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((Component)ent.Comp).LifeStage <= 6)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnPointAttempt(Entity<SleepingComponent> ent, ref PointAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("sleep-examined", (ValueTuple<string, object>)("target", Identity.Entity(Entity<SleepingComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager))));
		}
	}

	private void AddWakeVerb(Entity<SleepingComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanInteract && args.CanAccess)
		{
			_ = args.Target;
			EntityUid user = args.User;
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					TryWakeWithCooldown(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)), user);
				},
				Text = base.Loc.GetString("action-name-wake"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnInteractHand(Entity<SleepingComponent> ent, ref InteractHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		TryWakeWithCooldown(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)), args.User);
	}

	private void OnDamageChanged(Entity<SleepingComponent> ent, ref DamageChangedEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (args.DamageIncreased && args.DamageDelta != null && args.DamageDelta.GetTotal() >= ent.Comp.WakeThreshold)
		{
			TryWaking(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	private void OnZombified(Entity<SleepingComponent> ent, ref EntityZombifiedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TryWaking(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)), force: true);
	}

	private void OnMobStateChanged(Entity<SleepingComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		SpamEmitSoundComponent spam = default(SpamEmitSoundComponent);
		if (args.NewMobState == MobState.Dead)
		{
			((EntitySystem)this).RemComp<SpamEmitSoundComponent>(Entity<SleepingComponent>.op_Implicit(ent));
			((EntitySystem)this).RemComp<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent));
		}
		else if (((EntitySystem)this).TryComp<SpamEmitSoundComponent>(Entity<SleepingComponent>.op_Implicit(ent), ref spam))
		{
			_emitSound.SetEnabled(Entity<SpamEmitSoundComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), spam)), args.NewMobState == MobState.Alive);
		}
	}

	private void OnStatusEffectApplied(Entity<ForcedSleepingStatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TrySleeping(Entity<MobStateComponent>.op_Implicit(args.Target));
	}

	private void Wake(Entity<SleepingComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent));
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ent.Owner);
		EntityUid? wakeAction = ent.Comp.WakeAction;
		actionsSystem.RemoveAction(performer, wakeAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(wakeAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SleepStateChangedEvent ev = new SleepStateChangedEvent(FellAsleep: false);
		((EntitySystem)this).RaiseLocalEvent<SleepStateChangedEvent>(Entity<SleepingComponent>.op_Implicit(ent), ref ev, false);
		_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(ent.Owner));
	}

	public bool TrySleeping(Entity<MobStateComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MobStateComponent>(Entity<MobStateComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		TryingToSleepEvent tryingToSleepEvent = new TryingToSleepEvent(Entity<MobStateComponent>.op_Implicit(ent));
		((EntitySystem)this).RaiseLocalEvent<TryingToSleepEvent>(Entity<MobStateComponent>.op_Implicit(ent), ref tryingToSleepEvent, false);
		if (tryingToSleepEvent.Cancelled)
		{
			return false;
		}
		((EntitySystem)this).EnsureComp<SleepingComponent>(Entity<MobStateComponent>.op_Implicit(ent));
		return true;
	}

	public bool TryWakeWithCooldown(Entity<SleepingComponent?> ent, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		TimeSpan curTime = _gameTiming.CurTime;
		if (curTime < ent.Comp.CooldownEnd)
		{
			return false;
		}
		ent.Comp.CooldownEnd = curTime + ent.Comp.Cooldown;
		((EntitySystem)this).Dirty(Entity<SleepingComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		return TryWaking(ent, force: false, user);
	}

	public bool TryWaking(Entity<SleepingComponent?> ent, bool force = false, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SleepingComponent>(Entity<SleepingComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (!force && _statusEffectNew.HasEffectComp<ForcedSleepingStatusEffectComponent>((EntityUid?)Entity<SleepingComponent>.op_Implicit(ent)))
		{
			if (user.HasValue)
			{
				_audio.PlayPredicted(ent.Comp.WakeAttemptSound, Entity<SleepingComponent>.op_Implicit(ent), user, (AudioParams?)null);
				_popupSystem.PopupClient(base.Loc.GetString("wake-other-failure", (ValueTuple<string, object>)("target", Identity.Entity(Entity<SleepingComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager))), Entity<SleepingComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			}
			return false;
		}
		if (user.HasValue)
		{
			_audio.PlayPredicted(ent.Comp.WakeAttemptSound, Entity<SleepingComponent>.op_Implicit(ent), user, (AudioParams?)null);
			_popupSystem.PopupClient(base.Loc.GetString("wake-other-success", (ValueTuple<string, object>)("target", Identity.Entity(Entity<SleepingComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager))), Entity<SleepingComponent>.op_Implicit(ent), user);
		}
		Wake(Entity<SleepingComponent>.op_Implicit((Entity<SleepingComponent>.op_Implicit(ent), ent.Comp)));
		return true;
	}

	public void OnEmoteAttempt(Entity<SleepingComponent> ent, ref EmoteAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnChangeForceSay(Entity<SleepingComponent> ent, ref BeforeForceSayEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.Prefix = ent.Comp.ForceSaySleepDataset;
	}
}
