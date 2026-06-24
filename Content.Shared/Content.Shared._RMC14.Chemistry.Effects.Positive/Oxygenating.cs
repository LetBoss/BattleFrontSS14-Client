using System;
using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Damage;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Oxygenating : RMCChemicalEffect, ISerializationGenerated<Oxygenating>, ISerializationGenerated
{
	private static readonly ProtoId<DamageGroupPrototype> AirlossGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Airloss");

	private static readonly ProtoId<DamageTypePrototype> BluntType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private static readonly ProtoId<DamageTypePrototype> PoisonType = ProtoId<DamageTypePrototype>.op_Implicit("Poison");

	private static readonly ProtoId<ReagentPrototype> Lexorin = ProtoId<ReagentPrototype>.op_Implicit("RMCLexorin");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		string healing = ((base.ActualPotency >= 3f) ? $"Heals [color=green]all[/color] airloss damage and removes [color=green]{base.PotencyPerSecond}[/color] Lexorin from the bloodstream." : $"Heals [color=green]{base.PotencyPerSecond}[/color] airloss damage and removes [color=green]{base.PotencyPerSecond}[/color] Lexorin from the bloodstream.");
		return $"{healing}\nOverdoses cause [color=red]{(double)base.PotencyPerSecond * 0.5}[/color] toxin damage.\nCritical overdoses cause [color=red]{base.PotencyPerSecond}[/color] brute and [color=red]{base.PotencyPerSecond * 2f}[/color] toxin damage";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier healing = args.EntityManager.System<SharedRMCDamageableSystem>().DistributeHealingCached(amount: (base.ActualPotency >= 3f) ? ((FixedPoint2)99999) : potency, damageable: Entity<DamageableComponent>.op_Implicit(args.TargetEntity), groupId: AirlossGroup);
		damageable.TryChangeDamage(args.TargetEntity, healing, ignoreResistances: true, interruptsDoAfters: false);
		args.EntityManager.System<SharedRMCBloodstreamSystem>().RemoveBloodstreamChemical(args.TargetEntity, Lexorin, potency);
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency * 0.5f;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(BluntType)] = potency;
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency * 2f;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Oxygenating target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Oxygenating)definitionCast;
		serialization.TryCustomCopy<Oxygenating>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Oxygenating target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Oxygenating cast = (Oxygenating)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Oxygenating cast = (Oxygenating)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Oxygenating Instantiate()
	{
		return new Oxygenating();
	}
}
