using System;
using Content.Shared._RMC14.Body;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Hemogenic : RMCChemicalEffect, ISerializationGenerated<Hemogenic>, ISerializationGenerated
{
	private static readonly ProtoId<DamageTypePrototype> BluntType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private static readonly ProtoId<DamageTypePrototype> PoisonType = ProtoId<DamageTypePrototype>.op_Implicit("Poison");

	private static readonly ProtoId<DamageTypePrototype> AsphyxiationType = ProtoId<DamageTypePrototype>.op_Implicit("Asphyxiation");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		string baseText = $"Restores [color=green]{base.PotencyPerSecond}[/color]cl of blood while not hungry.\nCauses [color=red]{base.PotencyPerSecond}[/color] nutrient loss per second.\nOverdoses cause [color=red]{base.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{base.PotencyPerSecond * 5f}[/color] additional nutrient loss";
		if (!(base.ActualPotency > 3f))
		{
			return baseText;
		}
		return $"Deals [color=red]{base.PotencyPerSecond}[/color] brute, [color=red]{base.PotencyPerSecond * 2f}[/color] airloss damage, and slows you down.\n{baseText}";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entityManager = args.EntityManager;
		EntityUid target = args.TargetEntity;
		HungerSystem hungerSystem = entityManager.System<HungerSystem>();
		HungerComponent hungerComponent = default(HungerComponent);
		if (entityManager.TryGetComponent<HungerComponent>(target, ref hungerComponent) && !(hungerSystem.GetHunger(hungerComponent) < 200f))
		{
			hungerSystem.ModifyHunger(target, 0f - base.PotencyPerSecond);
			BloodstreamComponent bloodstream = default(BloodstreamComponent);
			if (entityManager.TryGetComponent<BloodstreamComponent>(target, ref bloodstream))
			{
				entityManager.System<SharedBloodstreamSystem>().TryModifyBloodLevel(Entity<BloodstreamComponent>.op_Implicit((target, bloodstream)), potency);
			}
			SharedRMCBloodstreamSystem rmcBloodstreamSystem = entityManager.System<SharedRMCBloodstreamSystem>();
			if (base.ActualPotency > 3f && rmcBloodstreamSystem.TryGetBloodSolution(target, out Solution bloodSolution) && bloodSolution.Volume > 570)
			{
				DamageSpecifier damage = new DamageSpecifier();
				damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(BluntType)] = potency;
				damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(AsphyxiationType)] = potency * 2;
				damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
			}
		}
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entityManager = args.EntityManager;
		EntityUid target = args.TargetEntity;
		entityManager.System<HungerSystem>().ModifyHunger(target, (0f - base.PotencyPerSecond) * 5f);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Hemogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Hemogenic)definitionCast;
		serialization.TryCustomCopy<Hemogenic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Hemogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Hemogenic cast = (Hemogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Hemogenic cast = (Hemogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Hemogenic Instantiate()
	{
		return new Hemogenic();
	}
}
