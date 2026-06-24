using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Cases;

[DataDefinition]
public sealed class PubgCaseDropEntryData : ISerializationGenerated<PubgCaseDropEntryData>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Kind { get; private set; } = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public int Weight { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int? MinAmount { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int? MaxAmount { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public List<string> RecipeRarities { get; private set; } = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public List<PubgCaseRecipeRarityWeightData> RecipeRarityWeights { get; private set; } = new List<PubgCaseRecipeRarityWeightData>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgCaseDropEntryData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PubgCaseDropEntryData>(this, ref target, hookCtx, false, context))
		{
			string KindTemp = null;
			if (Kind == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Kind, ref KindTemp, hookCtx, false, context))
			{
				KindTemp = Kind;
			}
			target.Kind = KindTemp;
			int WeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(Weight, ref WeightTemp, hookCtx, false, context))
			{
				WeightTemp = Weight;
			}
			target.Weight = WeightTemp;
			int? MinAmountTemp = null;
			if (!serialization.TryCustomCopy<int?>(MinAmount, ref MinAmountTemp, hookCtx, false, context))
			{
				MinAmountTemp = MinAmount;
			}
			target.MinAmount = MinAmountTemp;
			int? MaxAmountTemp = null;
			if (!serialization.TryCustomCopy<int?>(MaxAmount, ref MaxAmountTemp, hookCtx, false, context))
			{
				MaxAmountTemp = MaxAmount;
			}
			target.MaxAmount = MaxAmountTemp;
			List<string> RecipeRaritiesTemp = null;
			if (RecipeRarities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(RecipeRarities, ref RecipeRaritiesTemp, hookCtx, true, context))
			{
				RecipeRaritiesTemp = serialization.CreateCopy<List<string>>(RecipeRarities, hookCtx, context, false);
			}
			target.RecipeRarities = RecipeRaritiesTemp;
			List<PubgCaseRecipeRarityWeightData> RecipeRarityWeightsTemp = null;
			if (RecipeRarityWeights == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<PubgCaseRecipeRarityWeightData>>(RecipeRarityWeights, ref RecipeRarityWeightsTemp, hookCtx, true, context))
			{
				RecipeRarityWeightsTemp = serialization.CreateCopy<List<PubgCaseRecipeRarityWeightData>>(RecipeRarityWeights, hookCtx, context, false);
			}
			target.RecipeRarityWeights = RecipeRarityWeightsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgCaseDropEntryData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgCaseDropEntryData cast = (PubgCaseDropEntryData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PubgCaseDropEntryData Instantiate()
	{
		return new PubgCaseDropEntryData();
	}
}
