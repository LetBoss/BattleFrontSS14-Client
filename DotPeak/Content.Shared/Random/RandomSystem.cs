// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.RandomSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Random;

public sealed class RandomSystem : EntitySystem
{
  public IBudgetEntry? GetBudgetEntry(
    ref float budget,
    ref float probSum,
    IList<IBudgetEntry> entries,
    System.Random random)
  {
    if (entries.Count == 0)
      return (IBudgetEntry) null;
    int count = entries.Count;
    while (count > 1)
    {
      --count;
      int index1 = random.Next(count + 1);
      IList<IBudgetEntry> budgetEntryList1 = entries;
      int index2 = index1;
      IList<IBudgetEntry> budgetEntryList2 = entries;
      int num = count;
      IBudgetEntry entry1 = entries[count];
      IBudgetEntry entry2 = entries[index1];
      budgetEntryList1[index2] = entry1;
      int index3 = num;
      IBudgetEntry budgetEntry = entry2;
      budgetEntryList2[index3] = budgetEntry;
    }
    IBudgetEntry probEntry = (IBudgetEntry) this.GetProbEntry((IEnumerable<IProbEntry>) entries, probSum, random);
    budget -= probEntry.Cost;
    for (int index = 0; index < entries.Count; ++index)
    {
      IBudgetEntry entry = entries[index];
      if ((double) entry.Cost >= (double) budget)
      {
        entries.RemoveSwap<IBudgetEntry>(index);
        --index;
        probSum -= entry.Prob;
      }
    }
    return probEntry;
  }

  public IProbEntry GetProbEntry(IEnumerable<IProbEntry> entries, float probSum, System.Random random)
  {
    float num = random.NextSingle() * probSum;
    foreach (IProbEntry entry in entries)
    {
      num -= entry.Prob;
      if ((double) num < 0.0)
        return entry;
    }
    throw new InvalidOperationException();
  }
}
