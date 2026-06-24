using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Random;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class RandomFillSolution : ISerializationGenerated<RandomFillSolution>, ISerializationGenerated
{
	[DataField("quantity", false, 1, false, false, null)]
	public FixedPoint2 Quantity = 0;

	[DataField("weight", false, 1, false, false, null)]
	public float Weight;

	[DataField("reagents", false, 1, true, false, typeof(PrototypeIdListSerializer<ReagentPrototype>))]
	public List<string> Reagents = new List<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomFillSolution target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RandomFillSolution>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 QuantityTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Quantity, ref QuantityTemp, hookCtx, false, context))
			{
				QuantityTemp = serialization.CreateCopy<FixedPoint2>(Quantity, hookCtx, context, false);
			}
			target.Quantity = QuantityTemp;
			float WeightTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Weight, ref WeightTemp, hookCtx, false, context))
			{
				WeightTemp = Weight;
			}
			target.Weight = WeightTemp;
			List<string> ReagentsTemp = null;
			if (Reagents == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Reagents, ref ReagentsTemp, hookCtx, true, context))
			{
				ReagentsTemp = serialization.CreateCopy<List<string>>(Reagents, hookCtx, context, false);
			}
			target.Reagents = ReagentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomFillSolution target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomFillSolution cast = (RandomFillSolution)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RandomFillSolution Instantiate()
	{
		return new RandomFillSolution();
	}
}
