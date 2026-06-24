using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Cases;

[DataDefinition]
public sealed class PubgCaseRecipeRarityWeightData : ISerializationGenerated<PubgCaseRecipeRarityWeightData>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Rarity { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public int Weight { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgCaseRecipeRarityWeightData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PubgCaseRecipeRarityWeightData>(this, ref target, hookCtx, false, context))
		{
			string RarityTemp = null;
			if (Rarity == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Rarity, ref RarityTemp, hookCtx, false, context))
			{
				RarityTemp = Rarity;
			}
			target.Rarity = RarityTemp;
			int WeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(Weight, ref WeightTemp, hookCtx, false, context))
			{
				WeightTemp = Weight;
			}
			target.Weight = WeightTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgCaseRecipeRarityWeightData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgCaseRecipeRarityWeightData cast = (PubgCaseRecipeRarityWeightData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PubgCaseRecipeRarityWeightData Instantiate()
	{
		return new PubgCaseRecipeRarityWeightData();
	}
}
