using System;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Hunger : EntityEffectCondition, ISerializationGenerated<Hunger>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Max = float.PositiveInfinity;

	[DataField(null, false, 1, false, false, null)]
	public float Min;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		HungerComponent hunger = default(HungerComponent);
		if (args.EntityManager.TryGetComponent<HungerComponent>(args.TargetEntity, ref hunger))
		{
			float total = args.EntityManager.System<HungerSystem>().GetHunger(hunger);
			if (total > Min && total < Max)
			{
				return true;
			}
		}
		return false;
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-total-hunger", new(string, object)[2]
		{
			("max", float.IsPositiveInfinity(Max) ? 2.1474836E+09f : Max),
			("min", Min)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Hunger target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Hunger)definitionCast;
		if (!serialization.TryCustomCopy<Hunger>(this, ref target, hookCtx, false, context))
		{
			float MaxTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = Max;
			}
			target.Max = MaxTemp;
			float MinTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Min, ref MinTemp, hookCtx, false, context))
			{
				MinTemp = Min;
			}
			target.Min = MinTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Hunger target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Hunger cast = (Hunger)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Hunger cast = (Hunger)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Hunger Instantiate()
	{
		return new Hunger();
	}
}
