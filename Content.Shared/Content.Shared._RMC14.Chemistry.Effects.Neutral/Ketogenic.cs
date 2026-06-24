using System;
using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Ketogenic : RMCChemicalEffect, ISerializationGenerated<Ketogenic>, ISerializationGenerated
{
	private static readonly ProtoId<DamageTypePrototype> PoisonType = ProtoId<DamageTypePrototype>.op_Implicit("Poison");

	private static readonly ProtoId<StatusEffectPrototype> Unconscious = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return $"Removes [color=red]{base.PotencyPerSecond * 5f}[/color] nutrients, causing hunger over time.\nIncreases alcohol metabolism rate by [color=green]{base.PotencyPerSecond}[/color] units.\nOverdoses cause [color=red]{base.PotencyPerSecond * 5f}[/color] nutrition loss, [color=red]{base.PotencyPerSecond}[/color] toxin damage, and a [color=red]{(double)base.ActualPotency * 2.5}%[/color] chance of vomiting.\nCritical overdoses will knock you unconscious for [color=red]10[/color] seconds";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entityManager = args.EntityManager;
		EntityUid target = args.TargetEntity;
		entityManager.System<HungerSystem>().ModifyHunger(target, (0f - base.PotencyPerSecond) * 5f);
		if (args.EntityManager.System<SharedRMCBloodstreamSystem>().RemoveBloodstreamAlcohols(args.TargetEntity, potency))
		{
			args.EntityManager.System<SharedDrunkSystem>().TryApplyDrunkenness(args.TargetEntity, base.PotencyPerSecond * 5f);
		}
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entityManager = args.EntityManager;
		EntityUid target = args.TargetEntity;
		entityManager.System<HungerSystem>().ModifyHunger(target, (0f - base.PotencyPerSecond) * 5f);
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency;
		damageable.TryChangeDamage(target, damage, ignoreResistances: true, interruptsDoAfters: false);
		if (RandomExtensions.Prob(IoCManager.Resolve<IRobustRandom>(), 0.025f * base.ActualPotency))
		{
			RMCVomitEvent vomitEvent = new RMCVomitEvent(target);
			((IBroadcastEventBus)entityManager.EventBus).RaiseEvent<RMCVomitEvent>((EventSource)1, ref vomitEvent);
		}
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, ProtoId<StatusEffectPrototype>.op_Implicit(Unconscious), TimeSpan.FromSeconds(10L), false, (StatusEffectsComponent?)null, false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Ketogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Ketogenic)definitionCast;
		serialization.TryCustomCopy<Ketogenic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Ketogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Ketogenic cast = (Ketogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Ketogenic cast = (Ketogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Ketogenic Instantiate()
	{
		return new Ketogenic();
	}
}
