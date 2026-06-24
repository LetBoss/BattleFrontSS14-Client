using System;
using Content.Shared._RMC14.Body;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.neutral;

public sealed class Antihallucinogenic : RMCChemicalEffect, ISerializationGenerated<Antihallucinogenic>, ISerializationGenerated
{
	private static readonly ProtoId<DamageTypePrototype> BluntType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private static readonly ProtoId<DamageTypePrototype> HeatType = ProtoId<DamageTypePrototype>.op_Implicit("Heat");

	private static readonly ProtoId<DamageTypePrototype> PoisonType = ProtoId<DamageTypePrototype>.op_Implicit("Poison");

	private static readonly EntProtoId<StatusEffectComponent> SeeingRainbows = EntProtoId<StatusEffectComponent>.op_Implicit("StatusEffectSeeingRainbow");

	private static readonly ProtoId<ReagentPrototype> MindbreakerToxin = ProtoId<ReagentPrototype>.op_Implicit("RMCMindbreakerToxin");

	private static readonly ProtoId<ReagentPrototype> SpaceDrugs = ProtoId<ReagentPrototype>.op_Implicit("RMCSpaceDrugs");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return $"Removes [color=green]2.5[/color] units of Mindbreaker Toxin and Space Drugs from the bloodstream. It also stabilizes perceptive abnormalities such as hallucinations\nOverdoses cause [color=red]{base.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{base.PotencyPerSecond}[/color] brute, [color=red]{base.PotencyPerSecond}[/color] burn, and [color=red]{base.PotencyPerSecond * 3f}[/color] toxin damage";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCBloodstreamSystem sharedRMCBloodstreamSystem = args.EntityManager.System<SharedRMCBloodstreamSystem>();
		sharedRMCBloodstreamSystem.RemoveBloodstreamChemical(args.TargetEntity, MindbreakerToxin, 2.5f);
		sharedRMCBloodstreamSystem.RemoveBloodstreamChemical(args.TargetEntity, SpaceDrugs, 2.5f);
		args.EntityManager.System<StatusEffectsSystem>().TryRemoveTime(args.TargetEntity, EntProtoId<StatusEffectComponent>.op_Implicit(SeeingRainbows), TimeSpan.FromSeconds(base.PotencyPerSecond * 10f));
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(BluntType)] = potency;
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(HeatType)] = potency;
		damage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(PoisonType)] = potency * 3;
		damageable.TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Antihallucinogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Antihallucinogenic)definitionCast;
		serialization.TryCustomCopy<Antihallucinogenic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Antihallucinogenic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Antihallucinogenic cast = (Antihallucinogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Antihallucinogenic cast = (Antihallucinogenic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Antihallucinogenic Instantiate()
	{
		return new Antihallucinogenic();
	}
}
