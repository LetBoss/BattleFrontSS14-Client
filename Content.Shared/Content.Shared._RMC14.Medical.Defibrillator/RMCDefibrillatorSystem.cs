using System;
using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Chemistry.Effects;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Damage;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Medical;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Medical.Defibrillator;

public sealed class RMCDefibrillatorSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedRMCBloodstreamSystem _rmcBloodstream;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private RMCReagentSystem _rmcReagent;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DefibrillatorComponent, RMCDefibrillatorDamageModifyEvent>((EntityEventRefHandler<DefibrillatorComponent, RMCDefibrillatorDamageModifyEvent>)OnDefibrillatorDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDefibrillatorAudioComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCDefibrillatorAudioComponent, EntityTerminatingEvent>)OnDefibrillatorAudioTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDefibrillatorBlockedComponent, ExaminedEvent>((EntityEventRefHandler<RMCDefibrillatorBlockedComponent, ExaminedEvent>)OnNoDefibExamine, (Type[])null, (Type[])null);
	}

	private void OnDefibrillatorDamageModify(Entity<DefibrillatorComponent> ent, ref RMCDefibrillatorDamageModifyEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.RMCZapDamage != null)
		{
			foreach (var item in ent.Comp.RMCZapDamage)
			{
				ProtoId<DamageGroupPrototype> group = item.Group;
				int amount = item.Amount;
				args.Heal = _rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit(args.Target), group, amount, args.Heal);
			}
		}
		if (!_rmcBloodstream.TryGetChemicalSolution(args.Target, out Entity<SolutionComponent> solutionEnt, out Solution _))
		{
			return;
		}
		(Reagent, FixedPoint2, Electrogenetic)? highest = null;
		foreach (ReagentQuantity quantity in solutionEnt.Comp.Solution.Contents)
		{
			if (!_rmcReagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(quantity.Reagent.Prototype), out Reagent reagent) || reagent.Metabolisms == null || !reagent.Metabolisms.TryGetValue(ent.Comp.MetabolismId, out ReagentEffectsEntry effects))
			{
				continue;
			}
			EntityEffect[] effects2 = effects.Effects;
			for (int i = 0; i < effects2.Length; i++)
			{
				if (effects2[i] is Electrogenetic electrogenetic && (!highest.HasValue || electrogenetic.HealAmount > highest.Value.Item2))
				{
					highest = (reagent, electrogenetic.HealAmount, electrogenetic);
				}
			}
		}
		if (highest.HasValue)
		{
			args.Heal += highest.Value.Item3.CalculateHeal(_damageable, args.Target, (IEntityManager)(object)base.EntityManager);
			_solutionContainer.RemoveReagent(solutionEnt, highest.Value.Item1.ID, 1);
		}
	}

	private void OnNoDefibExamine(Entity<RMCDefibrillatorBlockedComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ShowOnExamine)
		{
			args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine), (ValueTuple<string, object>)("victim", ent)));
		}
	}

	private void OnDefibrillatorAudioTerminating(Entity<RMCDefibrillatorAudioComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DefibrillatorComponent defibrillator = default(DefibrillatorComponent);
		if (((EntitySystem)this).TryComp<DefibrillatorComponent>(ent.Comp.Defibrillator, ref defibrillator))
		{
			defibrillator.ChargeSoundEntity = null;
		}
	}

	public void StopChargingAudio(Entity<DefibrillatorComponent> defib)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		_audio.Stop(defib.Comp.ChargeSoundEntity, (AudioComponent)null);
		((EntitySystem)this).QueueDel(defib.Comp.ChargeSoundEntity);
		defib.Comp.ChargeSoundEntity = null;
	}
}
