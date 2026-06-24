using System;
using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Antitoxic : RMCChemicalEffect, ISerializationGenerated<Antitoxic>, ISerializationGenerated
{
	private static readonly ProtoId<DamageGroupPrototype> ToxinGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Toxin");

	private static readonly ProtoId<DamageGroupPrototype> GeneticGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Genetic");

	private static readonly ProtoId<StatusEffectPrototype> Unconscious = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		float healing = base.PotencyPerSecond * 2f;
		return $"Heals [color=green]{healing}[/color] toxin damage and removes [color=green]0.125[/color] units of toxic chemicals from the bloodstream.\nCritical overdoses cause [color=red]5[/color] seconds of unconsciousness with a [color=red]5%[/color] chance";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCDamageableSystem sharedRMCDamageableSystem = args.EntityManager.System<SharedRMCDamageableSystem>();
		DamageSpecifier healing = sharedRMCDamageableSystem.DistributeHealingCached(equal: sharedRMCDamageableSystem.DistributeHealingCached(Entity<DamageableComponent>.op_Implicit(args.TargetEntity), ToxinGroup, potency * 2f), damageable: Entity<DamageableComponent>.op_Implicit(args.TargetEntity), groupId: GeneticGroup, amount: potency * 2f);
		damageable.TryChangeDamage(args.TargetEntity, healing, ignoreResistances: true, interruptsDoAfters: false);
		args.EntityManager.System<SharedRMCBloodstreamSystem>().RemoveBloodstreamToxins(args.TargetEntity, 0.125f);
	}

	protected override void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
	}

	protected override void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (RandomExtensions.Prob(IoCManager.Resolve<IRobustRandom>(), 0.05f))
		{
			args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, ProtoId<StatusEffectPrototype>.op_Implicit(Unconscious), TimeSpan.FromSeconds(5L), false, (StatusEffectsComponent?)null, false);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Antitoxic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Antitoxic)definitionCast;
		serialization.TryCustomCopy<Antitoxic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Antitoxic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Antitoxic cast = (Antitoxic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Antitoxic cast = (Antitoxic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Antitoxic Instantiate()
	{
		return new Antitoxic();
	}
}
