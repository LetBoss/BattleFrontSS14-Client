using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Medical.Stasis;
using Content.Shared._RMC14.Medical.Wounds;
using Content.Shared.Alert;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Content.Shared.Forensics.Components;
using Content.Shared.HealthExaminable;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Rejuvenate;
using Content.Shared.Speech.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Body.Systems;

public abstract class SharedBloodstreamSystem : EntitySystem
{
	private static readonly bool DisableBleedingExamineText = true;

	[Dependency]
	protected SharedSolutionContainerSystem SolutionContainer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPuddleSystem _puddle;

	[Dependency]
	private AlertsSystem _alertsSystem;

	[Dependency]
	private MobStateSystem _mobStateSystem;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private SharedDrunkSystem _drunkSystem;

	[Dependency]
	private SharedStutteringSystem _stutteringSystem;

	[Dependency]
	private CMStasisBagSystem _cmStasisBag;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, MapInitEvent>((EntityEventRefHandler<BloodstreamComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<BloodstreamComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, ReactionAttemptEvent>((EntityEventRefHandler<BloodstreamComponent, ReactionAttemptEvent>)OnReactionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, SolutionRelayEvent<ReactionAttemptEvent>>((EntityEventRefHandler<BloodstreamComponent, SolutionRelayEvent<ReactionAttemptEvent>>)OnReactionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, DamageChangedEvent>((EntityEventRefHandler<BloodstreamComponent, DamageChangedEvent>)OnDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, HealthBeingExaminedEvent>((EntityEventRefHandler<BloodstreamComponent, HealthBeingExaminedEvent>)OnHealthBeingExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, BeingGibbedEvent>((EntityEventRefHandler<BloodstreamComponent, BeingGibbedEvent>)OnBeingGibbed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, ApplyMetabolicMultiplierEvent>((EntityEventRefHandler<BloodstreamComponent, ApplyMetabolicMultiplierEvent>)OnApplyMetabolicMultiplier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BloodstreamComponent, RejuvenateEvent>((EntityEventRefHandler<BloodstreamComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan curTime = _timing.CurTime;
		EntityQueryEnumerator<BloodstreamComponent> query = ((EntitySystem)this).EntityQueryEnumerator<BloodstreamComponent>();
		EntityUid uid = default(EntityUid);
		BloodstreamComponent bloodstream = default(BloodstreamComponent);
		while (query.MoveNext(ref uid, ref bloodstream))
		{
			if (curTime < bloodstream.NextUpdate)
			{
				continue;
			}
			bloodstream.NextUpdate += bloodstream.AdjustedUpdateInterval;
			((EntitySystem)this).DirtyField<BloodstreamComponent>(uid, bloodstream, "NextUpdate", (MetaDataComponent)null);
			if (!_cmStasisBag.CanBodyMetabolize(uid) || !SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), bloodstream.BloodSolutionName, ref bloodstream.BloodSolution, out Solution bloodSolution))
			{
				continue;
			}
			if (bloodSolution.Volume < bloodSolution.MaxVolume && !_mobStateSystem.IsDead(uid))
			{
				TryModifyBloodLevel(Entity<BloodstreamComponent>.op_Implicit((uid, bloodstream)), bloodstream.BloodRefreshAmount);
			}
			if (bloodstream.BleedAmount > 0f)
			{
				TryModifyBloodLevel(Entity<BloodstreamComponent>.op_Implicit((uid, bloodstream)), 0f - bloodstream.BleedAmount);
				TryModifyBleedAmount(Entity<BloodstreamComponent>.op_Implicit((uid, bloodstream)), 0f - bloodstream.BleedReductionAmount);
			}
			float bloodPercentage = GetBloodLevelPercentage(Entity<BloodstreamComponent>.op_Implicit((uid, bloodstream)));
			if (bloodPercentage < bloodstream.BloodlossThreshold && !_mobStateSystem.IsDead(uid))
			{
				DamageSpecifier amt = bloodstream.BloodlossDamage / (0.1f + bloodPercentage);
				_damageableSystem.TryChangeDamage(uid, amt, ignoreResistances: false, interruptsDoAfters: false);
				_drunkSystem.TryApplyDrunkenness(uid, (float)bloodstream.AdjustedUpdateInterval.TotalSeconds * 2f, applySlur: false);
				_stutteringSystem.DoStutter(uid, bloodstream.AdjustedUpdateInterval * 2.0, refresh: false);
				bloodstream.StatusTime += bloodstream.AdjustedUpdateInterval * 2.0;
				((EntitySystem)this).DirtyField<BloodstreamComponent>(uid, bloodstream, "StatusTime", (MetaDataComponent)null);
			}
			else if (!_mobStateSystem.IsDead(uid))
			{
				if (_rmcDamageable.HasAnyDamage(Entity<DamageableComponent>.op_Implicit(uid), bloodstream.BloodlossHealDamage))
				{
					_damageableSystem.TryChangeDamage(uid, bloodstream.BloodlossHealDamage * bloodPercentage, ignoreResistances: true, interruptsDoAfters: false);
				}
				_drunkSystem.TryRemoveDrunkenessTime(uid, bloodstream.StatusTime.TotalSeconds);
				_stutteringSystem.DoRemoveStutterTime(uid, bloodstream.StatusTime.TotalSeconds);
				bloodstream.StatusTime = TimeSpan.Zero;
				((EntitySystem)this).DirtyField<BloodstreamComponent>(uid, bloodstream, "StatusTime", (MetaDataComponent)null);
			}
		}
	}

	private void OnMapInit(Entity<BloodstreamComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextUpdate = _timing.CurTime + ent.Comp.AdjustedUpdateInterval;
		((EntitySystem)this).DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "NextUpdate", (MetaDataComponent)null);
	}

	private void OnEntRemoved(Entity<BloodstreamComponent> entity, ref EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity2 = ((ContainerModifiedMessage)args).Entity;
		EntityUid? val = entity.Comp.BloodSolution?.Owner;
		if (val.HasValue && entity2 == val.GetValueOrDefault())
		{
			entity.Comp.BloodSolution = null;
		}
		entity2 = ((ContainerModifiedMessage)args).Entity;
		val = entity.Comp.ChemicalSolution?.Owner;
		if (val.HasValue && entity2 == val.GetValueOrDefault())
		{
			entity.Comp.ChemicalSolution = null;
		}
		entity2 = ((ContainerModifiedMessage)args).Entity;
		val = entity.Comp.TemporarySolution?.Owner;
		if (val.HasValue && entity2 == val.GetValueOrDefault())
		{
			entity.Comp.TemporarySolution = null;
		}
	}

	private void OnReactionAttempt(Entity<BloodstreamComponent> ent, ref ReactionAttemptEvent args)
	{
		if (args.Cancelled)
		{
			return;
		}
		foreach (EntityEffect effect in args.Reaction.Effects)
		{
			if (effect is CreateEntityReactionEffect || effect is AreaReactionEffect)
			{
				args.Cancelled = true;
				break;
			}
		}
	}

	private void OnReactionAttempt(Entity<BloodstreamComponent> ent, ref SolutionRelayEvent<ReactionAttemptEvent> args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Name != ent.Comp.BloodSolutionName) || !(args.Name != ent.Comp.ChemicalSolutionName) || !(args.Name != ent.Comp.BloodTemporarySolutionName))
		{
			OnReactionAttempt(ent, ref args.Event);
		}
	}

	private void OnDamageChanged(Entity<BloodstreamComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState)
		{
			return;
		}
		CMBleedEvent ev = new CMBleedEvent(args);
		((EntitySystem)this).RaiseLocalEvent<CMBleedEvent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ev, false);
		DamageModifierSetPrototype modifiers = default(DamageModifierSetPrototype);
		if (ev.Handled || args.DamageDelta == null || !args.DamageIncreased || !_prototypeManager.TryIndex<DamageModifierSetPrototype>(ent.Comp.DamageBleedModifiers, ref modifiers))
		{
			return;
		}
		DamageSpecifier bloodloss = DamageSpecifier.ApplyModifierSet(DamageSpecifier.GetPositive(args.DamageDelta), modifiers);
		if (!bloodloss.Empty)
		{
			float oldBleedAmount = ent.Comp.BleedAmount;
			FixedPoint2 total = bloodloss.GetTotal();
			float totalFloat = total.Float();
			TryModifyBleedAmount(ent.AsNullable(), totalFloat);
			System.Random rand = new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>
			{
				(int)_timing.CurTick.Value,
				((EntitySystem)this).GetNetEntity(Entity<BloodstreamComponent>.op_Implicit(ent), (MetaDataComponent)null).Id,
				((EntitySystem)this).GetNetEntity(args.Origin, (MetaDataComponent)null)?.Id ?? 0
			}));
			float prob = Math.Clamp(totalFloat / 25f, 0f, 1f);
			if (totalFloat > 0f && rand.NextDouble() < (double)prob)
			{
				TryModifyBloodLevel(ent.AsNullable(), -total / 5f);
				_audio.PlayPredicted(ent.Comp.InstantBloodSound, Entity<BloodstreamComponent>.op_Implicit(ent), args.Origin, (AudioParams?)null);
			}
			else if (totalFloat <= ent.Comp.BloodHealedSoundThreshold && oldBleedAmount > 0f)
			{
				_popup.PopupEntity(base.Loc.GetString("bloodstream-component-wounds-cauterized"), Entity<BloodstreamComponent>.op_Implicit(ent), Entity<BloodstreamComponent>.op_Implicit(ent), PopupType.Medium);
				_audio.PlayPredicted(ent.Comp.BloodHealedSound, Entity<BloodstreamComponent>.op_Implicit(ent), args.Origin, (AudioParams?)null);
			}
		}
	}

	private void OnHealthBeingExamined(Entity<BloodstreamComponent> ent, ref HealthBeingExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (!DisableBleedingExamineText)
		{
			if (ent.Comp.BleedAmount > ent.Comp.MaxBleedAmount * 0.75f)
			{
				args.Message.PushNewline();
				args.Message.AddMarkupOrThrow(base.Loc.GetString("bloodstream-component-massive-bleeding", (ValueTuple<string, object>)("target", ent.Owner)));
			}
			else if (ent.Comp.BleedAmount > ent.Comp.MaxBleedAmount * 0.5f)
			{
				args.Message.PushNewline();
				args.Message.AddMarkupOrThrow(base.Loc.GetString("bloodstream-component-strong-bleeding", (ValueTuple<string, object>)("target", ent.Owner)));
			}
			else if (ent.Comp.BleedAmount > ent.Comp.MaxBleedAmount * 0.25f)
			{
				args.Message.PushNewline();
				args.Message.AddMarkupOrThrow(base.Loc.GetString("bloodstream-component-bleeding", (ValueTuple<string, object>)("target", ent.Owner)));
			}
			else if (ent.Comp.BleedAmount > 0f)
			{
				args.Message.PushNewline();
				args.Message.AddMarkupOrThrow(base.Loc.GetString("bloodstream-component-slight-bleeding", (ValueTuple<string, object>)("target", ent.Owner)));
			}
			if (GetBloodLevelPercentage(ent.AsNullable()) < ent.Comp.BloodlossThreshold)
			{
				args.Message.PushNewline();
				args.Message.AddMarkupOrThrow(base.Loc.GetString("bloodstream-component-looks-pale", (ValueTuple<string, object>)("target", ent.Owner)));
			}
		}
	}

	private void OnBeingGibbed(Entity<BloodstreamComponent> ent, ref BeingGibbedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		SpillAllSolutions(ent.AsNullable());
	}

	private void OnApplyMetabolicMultiplier(Entity<BloodstreamComponent> ent, ref ApplyMetabolicMultiplierEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.UpdateIntervalMultiplier = args.Multiplier;
		((EntitySystem)this).DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "UpdateIntervalMultiplier", (MetaDataComponent)null);
	}

	private void OnRejuvenate(Entity<BloodstreamComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		TryModifyBleedAmount(ent.AsNullable(), 0f - ent.Comp.BleedAmount);
		if (SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out Solution bloodSolution))
		{
			TryModifyBloodLevel(ent.AsNullable(), bloodSolution.AvailableVolume);
		}
		if (SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution))
		{
			SolutionContainer.RemoveAllSolution(ent.Comp.ChemicalSolution.Value);
		}
	}

	public float GetBloodLevelPercentage(Entity<BloodstreamComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, true) || !SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out Solution bloodSolution))
		{
			return 0f;
		}
		return bloodSolution.FillFraction;
	}

	public void SetBloodLossThreshold(Entity<BloodstreamComponent?> ent, float threshold)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ent.Comp.BloodlossThreshold = threshold;
			((EntitySystem)this).DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "BloodlossThreshold", (MetaDataComponent)null);
		}
	}

	public bool TryAddToChemicals(Entity<BloodstreamComponent?> ent, Solution solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || !SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution))
		{
			return false;
		}
		if (SolutionContainer.TryAddSolution(ent.Comp.ChemicalSolution.Value, solution))
		{
			return true;
		}
		return false;
	}

	public bool FlushChemicals(Entity<BloodstreamComponent?> ent, ProtoId<ReagentPrototype>? excludedReagentID, FixedPoint2 quantity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || !SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution, out Solution chemSolution))
		{
			return false;
		}
		for (int i = chemSolution.Contents.Count - 1; i >= 0; i--)
		{
			chemSolution.Contents[i].Deconstruct(out var id, out var _);
			ReagentId reagentId = id;
			ProtoId<ReagentPrototype>? val = ProtoId<ReagentPrototype>.op_Implicit(reagentId.Prototype);
			ProtoId<ReagentPrototype>? val2 = excludedReagentID;
			if (val.HasValue != val2.HasValue || (val.HasValue && val.GetValueOrDefault() != val2.GetValueOrDefault()))
			{
				SolutionContainer.RemoveReagent(ent.Comp.ChemicalSolution.Value, reagentId, quantity);
			}
		}
		return true;
	}

	public bool TryModifyBloodLevel(Entity<BloodstreamComponent?> ent, FixedPoint2 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || !SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution))
		{
			return false;
		}
		if (amount >= 0)
		{
			return SolutionContainer.TryAddReagent(ent.Comp.BloodSolution.Value, ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.BloodReagent), amount, null, GetEntityBloodData(Entity<BloodstreamComponent>.op_Implicit(ent)));
		}
		Solution newSol = SolutionContainer.SplitSolution(ent.Comp.BloodSolution.Value, -amount);
		if (!SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodTemporarySolutionName, ref ent.Comp.TemporarySolution, out Solution tempSolution))
		{
			return true;
		}
		tempSolution.AddSolution(newSol, _prototypeManager);
		if (tempSolution.Volume > ent.Comp.BleedPuddleThreshold)
		{
			if (ent.Comp.SpillChemicals && SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution))
			{
				Solution temp = SolutionContainer.SplitSolution(ent.Comp.ChemicalSolution.Value, tempSolution.Volume / 10f);
				tempSolution.AddSolution(temp, _prototypeManager);
			}
			_puddle.TrySpillAt(ent.Owner, tempSolution, out var _, sound: false);
			tempSolution.RemoveAllSolution();
		}
		SolutionContainer.UpdateChemicals(ent.Comp.TemporarySolution.Value);
		return true;
	}

	public bool TryModifyBleedAmount(Entity<BloodstreamComponent?> ent, float amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		ent.Comp.BleedAmount += amount;
		ent.Comp.BleedAmount = Math.Clamp(ent.Comp.BleedAmount, 0f, ent.Comp.MaxBleedAmount);
		((EntitySystem)this).DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "BleedAmount", (MetaDataComponent)null);
		if (ent.Comp.BleedAmount == 0f)
		{
			_alertsSystem.ClearAlert(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp.BleedingAlert);
		}
		else
		{
			short severity = (short)Math.Clamp(Math.Round(ent.Comp.BleedAmount, MidpointRounding.ToZero), 0.0, 10.0);
			_alertsSystem.ShowAlert(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp.BleedingAlert, severity);
		}
		return true;
	}

	public void SpillAllSolutions(Entity<BloodstreamComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			Solution tempSol = new Solution();
			if (SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out Solution bloodSolution))
			{
				tempSol.MaxVolume += bloodSolution.MaxVolume;
				tempSol.AddSolution(bloodSolution, _prototypeManager);
				SolutionContainer.RemoveAllSolution(ent.Comp.BloodSolution.Value);
			}
			if (SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.ChemicalSolutionName, ref ent.Comp.ChemicalSolution, out Solution chemSolution))
			{
				tempSol.MaxVolume += chemSolution.MaxVolume;
				tempSol.AddSolution(chemSolution, _prototypeManager);
				SolutionContainer.RemoveAllSolution(ent.Comp.ChemicalSolution.Value);
			}
			if (SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodTemporarySolutionName, ref ent.Comp.TemporarySolution, out Solution tempSolution))
			{
				tempSol.MaxVolume += tempSolution.MaxVolume;
				tempSol.AddSolution(tempSolution, _prototypeManager);
				SolutionContainer.RemoveAllSolution(ent.Comp.TemporarySolution.Value);
			}
			_puddle.TrySpillAt(Entity<BloodstreamComponent>.op_Implicit(ent), tempSol, out var _);
		}
	}

	public void ChangeBloodReagent(Entity<BloodstreamComponent?> ent, ProtoId<ReagentPrototype> reagent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ref ent.Comp, false) || reagent == ent.Comp.BloodReagent)
		{
			return;
		}
		if (!SolutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BloodSolutionName, ref ent.Comp.BloodSolution, out Solution bloodSolution))
		{
			ent.Comp.BloodReagent = reagent;
			return;
		}
		FixedPoint2 currentVolume = bloodSolution.RemoveReagent(ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.BloodReagent), bloodSolution.Volume, null, ignoreReagentData: true);
		ent.Comp.BloodReagent = reagent;
		((EntitySystem)this).DirtyField<BloodstreamComponent>(Entity<BloodstreamComponent>.op_Implicit(ent), ent.Comp, "BloodReagent", (MetaDataComponent)null);
		if (currentVolume > 0)
		{
			SolutionContainer.TryAddReagent(ent.Comp.BloodSolution.Value, ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.BloodReagent), currentVolume, null, GetEntityBloodData(Entity<BloodstreamComponent>.op_Implicit(ent)));
		}
	}

	public List<ReagentData> GetEntityBloodData(EntityUid uid)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<ReagentData> list = new List<ReagentData>();
		DnaData dnaData = new DnaData();
		DnaComponent donorComp = default(DnaComponent);
		if (((EntitySystem)this).TryComp<DnaComponent>(uid, ref donorComp) && donorComp.DNA != null)
		{
			dnaData.DNA = donorComp.DNA;
		}
		else
		{
			dnaData.DNA = base.Loc.GetString("forensics-dna-unknown");
		}
		list.Add(dnaData);
		return list;
	}
}
