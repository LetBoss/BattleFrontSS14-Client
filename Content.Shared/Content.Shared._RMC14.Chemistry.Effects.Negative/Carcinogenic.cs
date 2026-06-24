using System;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Negative;

public sealed class Carcinogenic : RMCChemicalEffect, ISerializationGenerated<Carcinogenic>, ISerializationGenerated
{
	private static readonly ProtoId<DamageTypePrototype> GeneticType = ProtoId<DamageTypePrototype>.op_Implicit("Cellular");

	private static readonly ProtoId<DamageTypePrototype> BluntType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return $"Deals [color=red]{(double)base.PotencyPerSecond * 0.5}[/color] genetic damage.\nOverdoses cause [color=red]{base.PotencyPerSecond * 2f}[/color] genetic damage.\nCritical overdoses cause [color=red]{base.PotencyPerSecond * 2f}[/color] brute damage";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(GeneticType)] = potency * 0.5f;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(GeneticType)] = potency * 2f;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(BluntType)] = potency * 2f;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Carcinogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Carcinogenic)definitionCast;
		serialization.TryCustomCopy<Carcinogenic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Carcinogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Carcinogenic cast = (Carcinogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Carcinogenic cast = (Carcinogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Carcinogenic Instantiate()
	{
		return new Carcinogenic();
	}
}
