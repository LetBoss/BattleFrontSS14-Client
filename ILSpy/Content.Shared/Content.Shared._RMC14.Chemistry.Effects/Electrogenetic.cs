using System;
using Content.Shared._RMC14.Damage;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class Electrogenetic : RMCChemicalEffect, ISerializationGenerated<Electrogenetic>, ISerializationGenerated
{
	public static readonly ProtoId<DamageGroupPrototype> BruteGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	public static readonly ProtoId<DamageGroupPrototype> BurnGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Burn");

	public static readonly ProtoId<DamageGroupPrototype> ToxinGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Toxin");

	private readonly FixedPoint2 _healPerLevel = 10;

	public FixedPoint2 HealAmount => _healPerLevel * Potency;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return $"Heals [color=green]{HealAmount}[/color] brute, burn, and toxin damage when defibrillated.\nRemoves 1u of this chemical from the solution when defibrillated";
	}

	public DamageSpecifier CalculateHeal(DamageableSystem damageable, EntityUid target, IEntityManager entityManager)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCDamageableSystem sharedRMCDamageableSystem = entityManager.System<SharedRMCDamageableSystem>();
		return sharedRMCDamageableSystem.DistributeHealingCached(equal: sharedRMCDamageableSystem.DistributeHealingCached(equal: sharedRMCDamageableSystem.DistributeHealingCached(equal: new DamageSpecifier(), damageable: Entity<DamageableComponent>.op_Implicit(target), groupId: BruteGroup, amount: HealAmount), damageable: Entity<DamageableComponent>.op_Implicit(target), groupId: BurnGroup, amount: HealAmount), damageable: Entity<DamageableComponent>.op_Implicit(target), groupId: ToxinGroup, amount: HealAmount);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Electrogenetic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Electrogenetic)definitionCast;
		serialization.TryCustomCopy<Electrogenetic>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Electrogenetic target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Electrogenetic cast = (Electrogenetic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Electrogenetic cast = (Electrogenetic)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Electrogenetic Instantiate()
	{
		return new Electrogenetic();
	}
}
