using System;
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Nutritious : RMCChemicalEffect, ISerializationGenerated<Nutritious>, ISerializationGenerated
{
	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		float updatedFactor = base.NutrimentFactor + base.ActualPotency;
		return $"Restores [color=green]{updatedFactor * base.ActualPotency}[/color] nutrients to the body and satiates hunger";
	}

	protected override void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entityManager = args.EntityManager;
		EntityUid target = args.TargetEntity;
		MobStateSystem mobStateSystem = entityManager.System<MobStateSystem>();
		HungerSystem hungerSystem = entityManager.System<HungerSystem>();
		if (!mobStateSystem.IsDead(target))
		{
			float updatedFactor = base.NutrimentFactor + base.ActualPotency;
			hungerSystem.ModifyHunger(target, updatedFactor * base.ActualPotency);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Nutritious target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Nutritious)definitionCast;
		serialization.TryCustomCopy<Nutritious>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Nutritious target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Nutritious cast = (Nutritious)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Nutritious cast = (Nutritious)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Nutritious Instantiate()
	{
		return new Nutritious();
	}
}
