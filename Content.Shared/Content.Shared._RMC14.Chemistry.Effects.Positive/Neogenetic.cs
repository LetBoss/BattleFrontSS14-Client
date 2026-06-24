using System;
using Content.Shared._RMC14.Damage;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Neogenetic : RMCChemicalEffect, ISerializationGenerated<Neogenetic>, ISerializationGenerated
{
	private static readonly ProtoId<DamageGroupPrototype> BruteGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	private static readonly ProtoId<DamageTypePrototype> HeatType = ProtoId<DamageTypePrototype>.op_Implicit("Heat");

	private static readonly ProtoId<DamageTypePrototype> PoisonType = ProtoId<DamageTypePrototype>.op_Implicit("Poison");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		float healing = base.PotencyPerSecond;
		if (base.ActualPotency > 2f)
		{
			healing += base.PotencyPerSecond * 0.5f;
		}
		return $"Heals [color=green]{healing}[/color] brute damage.\nOverdoses cause [color=red]{base.PotencyPerSecond}[/color] burn damage.\nCritical overdoses cause [color=red]{base.PotencyPerSecond * 5f}[/color] burn and [color=red]{base.PotencyPerSecond * 2f}[/color] toxin damage";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCDamageableSystem rmcDamageable = args.EntityManager.System<SharedRMCDamageableSystem>();
		DamageSpecifier healing = rmcDamageable.DistributeHealingCached(Entity<DamageableComponent>.op_Implicit(args.TargetEntity), BruteGroup, potency);
		damageable.TryChangeDamage(args.TargetEntity, healing, ignoreResistances: true, interruptsDoAfters: false);
		if (base.ActualPotency > 2f)
		{
			healing = rmcDamageable.DistributeHealingCached(Entity<DamageableComponent>.op_Implicit(args.TargetEntity), BruteGroup, potency * 0.5f);
			damageable.TryChangeDamage(args.TargetEntity, healing, ignoreResistances: true, interruptsDoAfters: false);
		}
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(HeatType)] = potency;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(HeatType)] = potency * 5;
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency * 2;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Neogenetic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Neogenetic)definitionCast;
		serialization.TryCustomCopy<Neogenetic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Neogenetic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Neogenetic cast = (Neogenetic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Neogenetic cast = (Neogenetic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Neogenetic Instantiate()
	{
		return new Neogenetic();
	}
}
