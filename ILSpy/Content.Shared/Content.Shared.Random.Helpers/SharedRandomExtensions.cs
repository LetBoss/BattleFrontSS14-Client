using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Random.Helpers;

public static class SharedRandomExtensions
{
	public static string Pick(this IRobustRandom random, DatasetPrototype prototype)
	{
		return RandomExtensions.Pick<string>(random, prototype.Values);
	}

	public static string Pick(this IRobustRandom random, LocalizedDatasetPrototype prototype)
	{
		int index = random.Next(prototype.Values.Count);
		return Loc.GetString(prototype.Values[index]);
	}

	public static string Pick(this IWeightedRandomPrototype prototype, System.Random random)
	{
		Dictionary<string, float> weights = prototype.Weights;
		float sum = weights.Values.Sum();
		float accumulated = 0f;
		float rand = random.NextSingle() * sum;
		foreach (KeyValuePair<string, float> item in weights)
		{
			item.Deconstruct(out var key, out var value);
			string key2 = key;
			float weight = value;
			accumulated += weight;
			if (accumulated >= rand)
			{
				return key2;
			}
		}
		throw new InvalidOperationException("Invalid weighted pick for " + ((IPrototype)prototype).ID + "!");
	}

	public static string Pick(this IWeightedRandomPrototype prototype, IRobustRandom? random = null)
	{
		IoCManager.Resolve<IRobustRandom>(ref random);
		Dictionary<string, float> weights = prototype.Weights;
		float sum = weights.Values.Sum();
		float accumulated = 0f;
		float rand = random.NextFloat() * sum;
		foreach (KeyValuePair<string, float> item in weights)
		{
			item.Deconstruct(out var key, out var value);
			string key2 = key;
			float weight = value;
			accumulated += weight;
			if (accumulated >= rand)
			{
				return key2;
			}
		}
		throw new InvalidOperationException("Invalid weighted pick for " + ((IPrototype)prototype).ID + "!");
	}

	public static T Pick<T>(this IRobustRandom random, Dictionary<T, float> weights) where T : notnull
	{
		float sum = weights.Values.Sum();
		float accumulated = 0f;
		float rand = random.NextFloat() * sum;
		foreach (KeyValuePair<T, float> weight2 in weights)
		{
			weight2.Deconstruct(out var key, out var value);
			T key2 = key;
			float weight = value;
			accumulated += weight;
			if (accumulated >= rand)
			{
				return key2;
			}
		}
		throw new InvalidOperationException("Invalid weighted pick");
	}

	public static T PickAndTake<T>(this IRobustRandom random, Dictionary<T, float> weights) where T : notnull
	{
		T pick = random.Pick(weights);
		weights.Remove(pick);
		return pick;
	}

	public static bool TryPickAndTake<T>(this IRobustRandom random, Dictionary<T, float> weights, [NotNullWhen(true)] out T? pick) where T : notnull
	{
		if (weights.Count == 0)
		{
			pick = default(T);
			return false;
		}
		pick = random.PickAndTake(weights);
		return true;
	}

	public static T Pick<T>(Dictionary<T, float> weights, System.Random random) where T : notnull
	{
		float sum = weights.Values.Sum();
		float accumulated = 0f;
		float rand = random.NextSingle() * sum;
		foreach (KeyValuePair<T, float> weight2 in weights)
		{
			weight2.Deconstruct(out var key, out var value);
			T key2 = key;
			float weight = value;
			accumulated += weight;
			if (accumulated >= rand)
			{
				return key2;
			}
		}
		throw new InvalidOperationException("Invalid weighted pick");
	}

	public static (string reagent, FixedPoint2 quantity) Pick(this WeightedRandomFillSolutionPrototype prototype, IRobustRandom? random = null)
	{
		RandomFillSolution randomFill = prototype.PickRandomFill(random);
		IoCManager.Resolve<IRobustRandom>(ref random);
		int sum = randomFill.Reagents.Count;
		float accumulated = 0f;
		float rand = random.NextFloat() * (float)sum;
		foreach (string reagent in randomFill.Reagents)
		{
			accumulated += 1f;
			if (accumulated >= rand)
			{
				return (reagent: reagent, quantity: randomFill.Quantity);
			}
		}
		throw new InvalidOperationException("Invalid weighted pick for " + prototype.ID + "!");
	}

	public static RandomFillSolution PickRandomFill(this WeightedRandomFillSolutionPrototype prototype, IRobustRandom? random = null)
	{
		IoCManager.Resolve<IRobustRandom>(ref random);
		List<RandomFillSolution> fills = prototype.Fills;
		Dictionary<RandomFillSolution, float> picks = new Dictionary<RandomFillSolution, float>();
		foreach (RandomFillSolution fill in fills)
		{
			picks[fill] = fill.Weight;
		}
		float sum = picks.Values.Sum();
		float accumulated = 0f;
		float rand = random.NextFloat() * sum;
		foreach (KeyValuePair<RandomFillSolution, float> item in picks)
		{
			item.Deconstruct(out var key, out var value);
			RandomFillSolution randSolution = key;
			float weight = value;
			accumulated += weight;
			if (accumulated >= rand)
			{
				return randSolution;
			}
		}
		throw new InvalidOperationException("Invalid weighted pick for " + prototype.ID + "!");
	}

	public static int HashCodeCombine(List<int> values)
	{
		int hash = 5381;
		foreach (int value in values)
		{
			hash = (hash << 5) + hash + value;
		}
		return hash;
	}
}
