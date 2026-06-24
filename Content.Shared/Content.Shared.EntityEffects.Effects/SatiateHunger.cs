using System;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class SatiateHunger : EntityEffect, ISerializationGenerated<SatiateHunger>, ISerializationGenerated
{
	private const float DefaultNutritionFactor = 3f;

	[DataField("factor", false, 1, false, false, null)]
	public float NutritionFactor { get; set; } = 3f;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager entman = args.EntityManager;
		HungerComponent hunger = default(HungerComponent);
		if (entman.TryGetComponent<HungerComponent>(args.TargetEntity, ref hunger))
		{
			if (args is EntityEffectReagentArgs reagentArgs)
			{
				entman.System<HungerSystem>().ModifyHunger(reagentArgs.TargetEntity, NutritionFactor * (float)reagentArgs.Quantity, hunger);
			}
			else
			{
				entman.System<HungerSystem>().ModifyHunger(args.TargetEntity, NutritionFactor, hunger);
			}
		}
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-satiate-hunger", new(string, object)[2]
		{
			("chance", Probability),
			("relative", NutritionFactor / 3f)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SatiateHunger target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SatiateHunger)definitionCast;
		if (!serialization.TryCustomCopy<SatiateHunger>(this, ref target, hookCtx, false, context))
		{
			float NutritionFactorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NutritionFactor, ref NutritionFactorTemp, hookCtx, false, context))
			{
				NutritionFactorTemp = NutritionFactor;
			}
			target.NutritionFactor = NutritionFactorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SatiateHunger target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SatiateHunger cast = (SatiateHunger)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SatiateHunger cast = (SatiateHunger)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SatiateHunger Instantiate()
	{
		return new SatiateHunger();
	}
}
