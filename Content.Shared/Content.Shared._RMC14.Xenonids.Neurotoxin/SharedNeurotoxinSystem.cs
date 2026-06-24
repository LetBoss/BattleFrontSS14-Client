using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.OrbitalCannon;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stamina;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.ActionBlocker;
using Content.Shared.Chat;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Drugs;
using Content.Shared.Drunk;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Random.Helpers;
using Content.Shared.Rejuvenate;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Neurotoxin;

public abstract class SharedNeurotoxinSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private RMCStaminaSystem _stamina;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private SharedSlurredSystem _slurred;

	[Dependency]
	private SharedStutteringSystem _stutter;

	[Dependency]
	private RMCDazedSystem _daze;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedDeafnessSystem _deafness;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private IPrototypeManager _proto;

	private readonly HashSet<Entity<MarineComponent>> _marines = new HashSet<Entity<MarineComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NeurotoxinComponent, RejuvenateEvent>((EntityEventRefHandler<NeurotoxinComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NeurotoxinInjectorComponent, ProjectileHitEvent>((EntityEventRefHandler<NeurotoxinInjectorComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, (Type[])null);
	}

	private void OnRejuvenate(Entity<NeurotoxinComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<NeurotoxinComponent>(Entity<NeurotoxinComponent>.op_Implicit(ent));
	}

	private void OnProjectileHit(Entity<NeurotoxinInjectorComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<MarineComponent>(args.Target) && (ent.Comp.AffectsDead || !_mobState.IsDead(args.Target)) && (ent.Comp.AffectsInfectedNested || !((EntitySystem)this).HasComp<XenoNestedComponent>(args.Target) || !((EntitySystem)this).HasComp<VictimInfectedComponent>(args.Target)))
		{
			TimeSpan time = _timing.CurTime;
			NeurotoxinComponent neuro = default(NeurotoxinComponent);
			if (!((EntitySystem)this).EnsureComp<NeurotoxinComponent>(args.Target, ref neuro))
			{
				neuro.LastMessage = time;
				neuro.LastAccentTime = time;
				neuro.LastStumbleTime = time;
			}
			_statusEffects.TryAddStatusEffect<RMCBlindedComponent>(args.Target, "Blinded", neuro.BlurTime * 6.0, true, (StatusEffectsComponent?)null, false);
			_daze.TryDaze(Entity<NeurotoxinInjectorComponent>.op_Implicit(ent), ent.Comp.DazeTime, refresh: true, null, stutter: true);
			neuro.NeurotoxinAmount += ent.Comp.NeuroPerSecond;
			neuro.ToxinDamage = ent.Comp.ToxinDamage;
			neuro.OxygenDamage = ent.Comp.OxygenDamage;
			neuro.CoughDamage = ent.Comp.CoughDamage;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<NeurotoxinInjectorComponent> neurotoxinInjectorQuery = ((EntitySystem)this).EntityQueryEnumerator<NeurotoxinInjectorComponent>();
		EntityUid uid = default(EntityUid);
		NeurotoxinInjectorComponent neuroGas = default(NeurotoxinInjectorComponent);
		NeurotoxinComponent builtNeurotoxin = default(NeurotoxinComponent);
		while (neurotoxinInjectorQuery.MoveNext(ref uid, ref neuroGas))
		{
			if (!neuroGas.InjectInContact)
			{
				continue;
			}
			_marines.Clear();
			_entityLookup.GetEntitiesInRange<MarineComponent>(uid.ToCoordinates(), 0.5f, _marines, (LookupFlags)110);
			foreach (Entity<MarineComponent> marine in _marines)
			{
				if ((neuroGas.AffectsDead || !_mobState.IsDead(Entity<MarineComponent>.op_Implicit(marine))) && (neuroGas.AffectsInfectedNested || !((EntitySystem)this).HasComp<XenoNestedComponent>(Entity<MarineComponent>.op_Implicit(marine)) || !((EntitySystem)this).HasComp<VictimInfectedComponent>(Entity<MarineComponent>.op_Implicit(marine))))
				{
					if (!((EntitySystem)this).EnsureComp<NeurotoxinComponent>(Entity<MarineComponent>.op_Implicit(marine), ref builtNeurotoxin))
					{
						builtNeurotoxin.LastMessage = time;
						builtNeurotoxin.LastAccentTime = time;
						builtNeurotoxin.LastStumbleTime = time;
						builtNeurotoxin.NextGasInjectionAt = time;
						builtNeurotoxin.NextNeuroEffectAt = time;
					}
					if (!(time < builtNeurotoxin.NextGasInjectionAt))
					{
						_statusEffects.TryAddStatusEffect<RMCBlindedComponent>(Entity<MarineComponent>.op_Implicit(marine), "Blinded", builtNeurotoxin.BlurTime * 12.0, true, (StatusEffectsComponent?)null, false);
						_daze.TryDaze(Entity<MarineComponent>.op_Implicit(marine), neuroGas.DazeTime, refresh: true, null, stutter: true);
						builtNeurotoxin.NeurotoxinAmount += neuroGas.NeuroPerSecond;
						builtNeurotoxin.ToxinDamage = neuroGas.ToxinDamage;
						builtNeurotoxin.OxygenDamage = neuroGas.OxygenDamage;
						builtNeurotoxin.CoughDamage = neuroGas.CoughDamage;
						builtNeurotoxin.NextGasInjectionAt = time + neuroGas.TimeBetweenGasInjects;
					}
				}
			}
		}
		EntityQueryEnumerator<NeurotoxinComponent> neuroToxinQuery = ((EntitySystem)this).EntityQueryEnumerator<NeurotoxinComponent>();
		EntityUid uid2 = default(EntityUid);
		NeurotoxinComponent neuro = default(NeurotoxinComponent);
		while (neuroToxinQuery.MoveNext(ref uid2, ref neuro))
		{
			if (time < neuro.NextNeuroEffectAt)
			{
				continue;
			}
			neuro.NeurotoxinAmount -= neuro.DepletionPerTick;
			neuro.NextNeuroEffectAt = time + neuro.UpdateEvery;
			if (neuro.NeurotoxinAmount <= 0f || ((EntitySystem)this).HasComp<SynthComponent>(uid2))
			{
				((EntitySystem)this).RemCompDeferred<NeurotoxinComponent>(uid2);
			}
			else
			{
				if (_mobState.IsDead(uid2))
				{
					continue;
				}
				_stamina.DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit(uid2), neuro.StaminaDamagePerTick, visual: false);
				_statusEffects.TryAddStatusEffect<DrunkComponent>(uid2, "Drunk", neuro.DizzyStrength, true, (StatusEffectsComponent?)null, false);
				NeurotoxinNonStackingEffects(uid2, neuro, time, out var coughChance, out var stumbleChance);
				NeurotoxinStackingEffects(uid2, neuro, time);
				if (RandomExtensions.Prob(_random, stumbleChance) && time - neuro.LastStumbleTime >= neuro.MinimumDelayBetweenEvents)
				{
					neuro.LastStumbleTime = time;
					if (_blocker.CanMove(uid2))
					{
						_rmcPulling.TryStopPullsOn(uid2);
						_physics.SetLinearVelocity(uid2, Vector2.Zero, true, true, (FixturesComponent)null, (PhysicsComponent)null);
						_physics.SetAngularVelocity(uid2, 0f, true, (FixturesComponent)null, (PhysicsComponent)null);
						ThrowingSystem throwing = _throwing;
						EntityUid uid3 = uid2;
						Angle val = _random.NextAngle();
						throwing.TryThrow(uid3, Vector2Helpers.Normalized(((Angle)(ref val)).ToVec()) / 10f, 10f, null, 2f, null, compensateFriction: true, recoil: true, animated: false, playSound: false, doSpin: false);
					}
					_popup.PopupEntity(base.Loc.GetString("rmc-stumble-others", (ValueTuple<string, object>)("victim", uid2)), uid2, Filter.PvsExcept(uid2, 2f, (IEntityManager)null), recordReplay: true, PopupType.SmallCaution);
					_popup.PopupEntity(base.Loc.GetString("rmc-stumble"), uid2, uid2, PopupType.MediumCaution);
					_daze.TryDaze(uid2, neuro.DazeLength * 5.0, refresh: true, null, stutter: true);
					_jitter.DoJitter(uid2, neuro.StumbleJitterTime, refresh: true);
					_statusEffects.TryAddStatusEffect<DrunkComponent>(uid2, "Drunk", neuro.DizzyStrengthOnStumble, true, (StatusEffectsComponent?)null, false);
					NeurotoxinEmoteEvent ev = new NeurotoxinEmoteEvent
					{
						Emote = neuro.PainId
					};
					((EntitySystem)this).RaiseLocalEvent<NeurotoxinEmoteEvent>(uid2, ev, false);
				}
				if (RandomExtensions.Prob(_random, coughChance))
				{
					_slow.TrySlowdown(uid2, neuro.BloodCoughDuration);
					_damage.TryChangeDamage(uid2, neuro.CoughDamage);
					_popup.PopupEntity(base.Loc.GetString("rmc-bloodcough"), uid2, uid2, PopupType.MediumCaution);
					NeurotoxinEmoteEvent ev2 = new NeurotoxinEmoteEvent
					{
						Emote = neuro.CoughId
					};
					((EntitySystem)this).RaiseLocalEvent<NeurotoxinEmoteEvent>(uid2, ev2, false);
				}
			}
		}
		EntityQueryEnumerator<NeurotoxinLingeringHallucinationComponent> neuroHallucinationQuery = ((EntitySystem)this).EntityQueryEnumerator<NeurotoxinLingeringHallucinationComponent>();
		EntityUid uid4 = default(EntityUid);
		NeurotoxinLingeringHallucinationComponent hallu = default(NeurotoxinLingeringHallucinationComponent);
		while (neuroHallucinationQuery.MoveNext(ref uid4, ref hallu))
		{
			if (hallu.Hallucinations.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<NeurotoxinLingeringHallucinationComponent>(uid4);
				continue;
			}
			List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> toRemove = new List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>();
			List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> toAdd = new List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>();
			foreach (var entry in hallu.Hallucinations)
			{
				if (!(entry.Item3 > time))
				{
					(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)? newEntry = ProcessHallucination(uid4, hallu, entry);
					toRemove.Add(entry);
					if (newEntry.HasValue)
					{
						toAdd.Add(newEntry.Value);
					}
				}
			}
			hallu.Hallucinations.RemoveAll(((NeuroHallucinations, int, TimeSpan, EntityCoordinates?) a) => toRemove.Contains(a));
			hallu.Hallucinations.AddRange(toAdd);
		}
	}

	private void NeurotoxinNonStackingEffects(EntityUid victim, NeurotoxinComponent neurotoxin, TimeSpan time, out float coughChance, out float stumbleChance)
	{
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		string message = "rmc-neuro-tired";
		PopupType poptype = PopupType.Small;
		coughChance = 0f;
		stumbleChance = 0f;
		if (!(neurotoxin.NeurotoxinAmount <= 9f))
		{
			if (neurotoxin.NeurotoxinAmount <= 14f)
			{
				message = "rmc-neuro-numb";
				poptype = PopupType.SmallCaution;
				coughChance = 0.1f;
			}
			else if (neurotoxin.NeurotoxinAmount <= 19f)
			{
				if (_random.Next(4) == 0)
				{
					message = "rmc-neuro-where";
					poptype = PopupType.Large;
				}
				else
				{
					message = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)new List<string> { "rmc-neuro-very-numb", "rmc-neuro-erratic", "rmc-neuro-panic" });
					poptype = PopupType.MediumCaution;
				}
				coughChance = 0.1f;
				stumbleChance = 0.05f;
			}
			else if (neurotoxin.NeurotoxinAmount <= 24f)
			{
				message = "rmc-neuro-sting";
				poptype = PopupType.MediumCaution;
				coughChance = 0.25f;
				stumbleChance = 0.25f;
			}
			else
			{
				switch (_random.Next(7))
				{
				case 0:
					message = "rmc-neuro-what";
					poptype = PopupType.Large;
					break;
				case 1:
					message = "rmc-neuro-hearing";
					poptype = PopupType.MediumCaution;
					break;
				default:
					message = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)new List<string> { "rmc-neuro-pain", "rmc-neuro-agh", "rmc-neuro-so-numb", "rmc-neuro-limbs", "rmc-neuro-think" });
					poptype = PopupType.LargeCaution;
					break;
				}
				coughChance = 0.25f;
				stumbleChance = 0.25f;
			}
		}
		if (time - neurotoxin.LastMessage >= neurotoxin.TimeBetweenMessages)
		{
			neurotoxin.LastMessage = time;
			_popup.PopupEntity(base.Loc.GetString(message), victim, victim, poptype);
		}
	}

	private void NeurotoxinStackingEffects(EntityUid victim, NeurotoxinComponent neurotoxin, TimeSpan currTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		if (neurotoxin.NeurotoxinAmount >= 10f)
		{
			_statusEffects.TryAddStatusEffect<RMCBlindedComponent>(victim, "Blinded", neurotoxin.BlurTime, true, (StatusEffectsComponent?)null, false);
			if (currTime - neurotoxin.LastAccentTime >= neurotoxin.MinimumDelayBetweenEvents)
			{
				neurotoxin.LastAccentTime = currTime;
				if (RandomExtensions.Prob(_random, 0.5f))
				{
					_slurred.DoSlur(victim, neurotoxin.AccentTime);
				}
				else
				{
					_stutter.DoStutter(victim, neurotoxin.AccentTime, refresh: true);
				}
			}
		}
		if (neurotoxin.NeurotoxinAmount >= 15f)
		{
			_jitter.DoJitter(victim, neurotoxin.JitterTime, refresh: true);
			if (currTime >= neurotoxin.NextHallucination)
			{
				neurotoxin.NextHallucination = currTime + _random.Next(neurotoxin.HallucinationEveryMin, neurotoxin.HallucinationEveryMax);
				DoNeuroHallucination(victim, neurotoxin);
			}
		}
		if (neurotoxin.NeurotoxinAmount >= 20f)
		{
			_statusEffects.TryAddStatusEffect<TemporaryBlindnessComponent>(victim, "TemporaryBlindness", neurotoxin.BlindTime, true, (StatusEffectsComponent?)null, false);
		}
		if (neurotoxin.NeurotoxinAmount >= 27f)
		{
			_daze.TryDaze(victim, neurotoxin.DazeLength, refresh: true, null, stutter: true);
			_damage.TryChangeDamage(victim, neurotoxin.ToxinDamage);
			_deafness.TryDeafen(victim, neurotoxin.DeafenTime, refresh: true, null, ignoreProtection: true);
		}
		if (neurotoxin.NeurotoxinAmount >= 50f)
		{
			_damage.TryChangeDamage(victim, neurotoxin.OxygenDamage);
		}
	}

	private void DoNeuroHallucination(EntityUid victim, NeurotoxinComponent neurotoxin)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		switch (_random.Pick(neurotoxin.Hallucinations))
		{
		case NeuroHallucinations.AlienAttack:
			_audio.PlayStatic(neurotoxin.Pounce, victim, victim.ToCoordinates(), (AudioParams?)null);
			_stun.TryParalyze(victim, neurotoxin.PounceDownTime, refresh: true);
			((EntitySystem)this).EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.AlienAttack, 0, _timing.CurTime + TimeSpan.FromSeconds(1L), null));
			break;
		case NeuroHallucinations.OB:
		{
			ICommonSession session = default(ICommonSession);
			if (_player.TryGetSessionByEntity(victim, ref session))
			{
				string msg = "[font size=16][color=red]Orbital bombardment launch command detected![/color][/font]";
				msg = "[bold][font size=24][color=red]\n" + msg + "\n[/color][/font][/bold]";
				_rmcChat.ChatMessageToOne(ChatChannel.Radio, msg, msg, default(EntityUid), hideChat: false, session.Channel, null, recordReplay: true);
				if (_area.TryGetArea(victim.ToCoordinates(), out Entity<AreaComponent>? _, out EntityPrototype areaProto))
				{
					EntProtoId<OrbitalCannonWarheadComponent> warhead = RandomExtensions.Pick<EntProtoId<OrbitalCannonWarheadComponent>>(_random, (IReadOnlyList<EntProtoId<OrbitalCannonWarheadComponent>>)neurotoxin.WarheadTypes);
					EntityPrototype warHeadProto = default(EntityPrototype);
					if (_proto.TryIndex(EntProtoId<OrbitalCannonWarheadComponent>.op_Implicit(warhead), ref warHeadProto))
					{
						msg = $"[color=red]Launch command informs {warHeadProto.Name}. Estimated impact area: {areaProto.Name}[/color]";
						_rmcChat.ChatMessageToOne(ChatChannel.Radio, msg, msg, default(EntityUid), hideChat: false, session.Channel, null, recordReplay: true);
					}
				}
			}
			_audio.PlayGlobal(neurotoxin.OBAlert, victim, (AudioParams?)null);
			((EntitySystem)this).EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.OB, 0, _timing.CurTime + TimeSpan.FromSeconds(2L), null));
			break;
		}
		case NeuroHallucinations.Screech:
			_audio.PlayStatic(neurotoxin.Screech, victim, HallucinationSoundOffset(victim, 3f), (AudioParams?)null);
			_stun.TryParalyze(victim, neurotoxin.ScreechDownTime, refresh: true);
			break;
		case NeuroHallucinations.CAS:
		{
			EntityCoordinates position = HallucinationSoundOffset(victim, 7f);
			_audio.PlayStatic(neurotoxin.FiremissionStart, victim, position, (AudioParams?)null);
			((EntitySystem)this).EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.CAS, 0, _timing.CurTime + TimeSpan.FromSeconds(3.5), position));
			break;
		}
		case NeuroHallucinations.Giggle:
		{
			NeurotoxinEmoteEvent ev = new NeurotoxinEmoteEvent
			{
				Emote = neurotoxin.GiggleId
			};
			((EntitySystem)this).RaiseLocalEvent<NeurotoxinEmoteEvent>(victim, ev, false);
			_statusEffects.TryAddStatusEffect<SeeingRainbowsStatusEffectComponent>(victim, "StatusEffectSeeingRainbow", neurotoxin.RainbowDuration, true, (StatusEffectsComponent?)null, false);
			break;
		}
		case NeuroHallucinations.Mortar:
		{
			EntityCoordinates position = HallucinationSoundOffset(victim, 7f);
			FakeWarning(position, victim, LocId.op_Implicit("rmc-mortar-shell-impact-warning"), LocId.op_Implicit("rmc-mortar-shell-impact-warning-above"));
			((EntitySystem)this).EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.Mortar, 0, _timing.CurTime + TimeSpan.FromSeconds(1L), position));
			break;
		}
		case NeuroHallucinations.Sounds:
		{
			SoundSpecifier sound = RandomExtensions.Pick<SoundSpecifier>(_random, (IReadOnlyList<SoundSpecifier>)neurotoxin.HallucinationRandomSounds);
			_audio.PlayStatic(sound, victim, HallucinationSoundOffset(victim, 7f), (AudioParams?)null);
			break;
		}
		}
	}

	private (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)? ProcessHallucination(EntityUid victim, NeurotoxinLingeringHallucinationComponent lingering, (NeuroHallucinations, int, TimeSpan, EntityCoordinates?) hallucination)
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		switch (hallucination.Item1)
		{
		case NeuroHallucinations.AlienAttack:
		{
			if (hallucination.Item2 == 0)
			{
				_audio.PlayStatic(lingering.XenoClaw, victim, victim.ToCoordinates(), (AudioParams?)null);
				_audio.PlayStatic(lingering.BoneBreak, victim, victim.ToCoordinates(), (AudioParams?)null);
				hallucination.Item2 = 1;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 < 3)
			{
				_audio.PlayStatic(lingering.XenoClaw, victim, victim.ToCoordinates(), (AudioParams?)null);
				hallucination.Item2++;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			_audio.PlayStatic(lingering.BoneBreak, victim, victim.ToCoordinates(), (AudioParams?)null);
			NeurotoxinEmoteEvent ev = new NeurotoxinEmoteEvent
			{
				Emote = lingering.PainEmote
			};
			((EntitySystem)this).RaiseLocalEvent<NeurotoxinEmoteEvent>(victim, ev, false);
			break;
		}
		case NeuroHallucinations.OB:
			_audio.PlayStatic(lingering.OBTravel, victim, HallucinationSoundOffset(victim, 7f), (AudioParams?)null);
			break;
		case NeuroHallucinations.CAS:
		{
			if (hallucination.Item2 == 0)
			{
				FakeWarning((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), victim, LocId.op_Implicit("rmc-dropship-firemission-warning"), LocId.op_Implicit("rmc-dropship-firemission-warning-above"));
				hallucination.Item2 = 1;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 1)
			{
				_audio.PlayStatic(lingering.RocketFire, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 2;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 2)
			{
				_audio.PlayStatic(lingering.GauFire, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 3;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 3)
			{
				_audio.PlayStatic(lingering.RocketFire, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 4;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(1L);
				return hallucination;
			}
			if (hallucination.Item2 == 4)
			{
				_audio.PlayStatic(lingering.Explosion, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 5;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(1L);
				return hallucination;
			}
			if (hallucination.Item2 == 5)
			{
				_audio.PlayStatic(lingering.RocketFire, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 6;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(1L);
				return hallucination;
			}
			if (hallucination.Item2 == 6)
			{
				_audio.PlayStatic(lingering.Explosion, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 7;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 7)
			{
				_audio.PlayStatic(lingering.BigExplosion, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 8;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 8)
			{
				_audio.PlayStatic(lingering.RocketFire, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 9;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 9)
			{
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				_audio.PlayStatic(lingering.Explosion, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 10;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			if (hallucination.Item2 == 10)
			{
				_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
				hallucination.Item2 = 11;
				hallucination.Item3 = _timing.CurTime + TimeSpan.FromSeconds(0.5);
				return hallucination;
			}
			_audio.PlayStatic(lingering.Explosion, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
			_audio.PlayStatic(lingering.GauHit, victim, HallucinationSoundOffset((EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), 7f), (AudioParams?)null);
			NeurotoxinEmoteEvent ev2 = new NeurotoxinEmoteEvent
			{
				Emote = lingering.PainEmote
			};
			((EntitySystem)this).RaiseLocalEvent<NeurotoxinEmoteEvent>(victim, ev2, false);
			break;
		}
		case NeuroHallucinations.Mortar:
			_audio.PlayStatic(lingering.MortarTravel, victim, (EntityCoordinates)(((_003F?)hallucination.Item4) ?? victim.ToCoordinates()), (AudioParams?)null);
			break;
		}
		return null;
	}

	private EntityCoordinates HallucinationSoundOffset(EntityUid victim, float maxDistance)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 randomOffset = new Vector2(_random.NextFloat(0f - maxDistance, maxDistance + 0.01f), _random.NextFloat(0f - maxDistance, maxDistance + 0.01f));
		EntityCoordinates coordinates = ((EntitySystem)this).Transform(victim).Coordinates;
		return ((EntityCoordinates)(ref coordinates)).Offset(randomOffset);
	}

	private EntityCoordinates HallucinationSoundOffset(EntityCoordinates coords, float maxDistance)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector2 randomOffset = new Vector2(_random.NextFloat(0f - maxDistance, maxDistance + 0.01f), _random.NextFloat(0f - maxDistance, maxDistance + 0.01f));
		return ((EntityCoordinates)(ref coords)).Offset(randomOffset);
	}

	private void FakeWarning(EntityCoordinates coords, EntityUid player, LocId directionWarning, LocId aboveWarning)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Vector2 distanceVec = _transform.GetMapCoordinates(player, (TransformComponent)null).Position - _transform.ToMapCoordinates(coords, true).Position;
		float num = distanceVec.Length();
		string direction = ((object)DirectionExtensions.GetDir(distanceVec)/*cast due to constrained. prefix*/).ToString().ToUpperInvariant();
		string msg = ((num < 1f) ? base.Loc.GetString(LocId.op_Implicit(aboveWarning)) : base.Loc.GetString(LocId.op_Implicit(directionWarning), (ValueTuple<string, object>)("direction", direction)));
		_popup.PopupEntity(msg, player, player, PopupType.LargeCaution);
		ICommonSession session = default(ICommonSession);
		if (_player.TryGetSessionByEntity(player, ref session))
		{
			msg = "[bold][font size=24][color=red]\n" + msg + "\n[/color][/font][/bold]";
			_rmcChat.ChatMessageToOne(ChatChannel.Radio, msg, msg, default(EntityUid), hideChat: false, session.Channel, null, recordReplay: true);
		}
	}
}
