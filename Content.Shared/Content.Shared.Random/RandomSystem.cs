using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared.Random;

public sealed class RandomSystem : EntitySystem
{
	public IBudgetEntry? GetBudgetEntry(ref float budget, ref float probSum, IList<IBudgetEntry> entries, System.Random random)
	{
		if (entries.Count == 0)
		{
			return null;
		}
		int n = entries.Count;
		while (n > 1)
		{
			n--;
			int k = random.Next(n + 1);
			int index = k;
			int index2 = n;
			IBudgetEntry value = entries[n];
			IBudgetEntry value2 = entries[k];
			entries[index] = value;
			entries[index2] = value2;
		}
		IBudgetEntry budgetEntry = (IBudgetEntry)GetProbEntry(entries, probSum, random);
		budget -= budgetEntry.Cost;
		for (int i = 0; i < entries.Count; i++)
		{
			IBudgetEntry entry = entries[i];
			if (!(entry.Cost < budget))
			{
				Extensions.RemoveSwap<IBudgetEntry>(entries, i);
				i--;
				probSum -= entry.Prob;
			}
		}
		return budgetEntry;
	}

	public IProbEntry GetProbEntry(IEnumerable<IProbEntry> entries, float probSum, System.Random random)
	{
		float value = random.NextSingle() * probSum;
		foreach (IProbEntry entry in entries)
		{
			value -= entry.Prob;
			if (value < 0f)
			{
				return entry;
			}
		}
		throw new InvalidOperationException();
	}
}
