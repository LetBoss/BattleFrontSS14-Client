using System;
using System.Collections.Generic;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Nutrition.FoodMetamorphRules;

[Serializable]
[NetSerializable]
public sealed class SequenceLength : FoodMetamorphRule, ISerializationGenerated<SequenceLength>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public MinMax Range;

	public override bool Check(IPrototypeManager protoMan, EntityManager entMan, EntityUid food, List<FoodSequenceVisualLayer> ingredients)
	{
		if (ingredients.Count <= Range.Max)
		{
			return ingredients.Count >= Range.Min;
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SequenceLength target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodMetamorphRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SequenceLength)definitionCast;
		if (!serialization.TryCustomCopy<SequenceLength>(this, ref target, hookCtx, false, context))
		{
			MinMax RangeTemp = default(MinMax);
			if (!serialization.TryCustomCopy<MinMax>(Range, ref RangeTemp, hookCtx, false, context))
			{
				serialization.CopyTo<MinMax>(Range, ref RangeTemp, hookCtx, context, false);
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SequenceLength target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SequenceLength cast = (SequenceLength)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SequenceLength cast = (SequenceLength)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SequenceLength Instantiate()
	{
		return new SequenceLength();
	}
}
