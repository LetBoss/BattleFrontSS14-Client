// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Helpers.SharedRandomExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
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
    float num1 = weights.Values.Sum();
    float num2 = 0.0f;
    float num3 = random.NextSingle() * num1;
    foreach ((string key, float num4) in weights)
    {
      num2 += num4;
      if ((double) num2 >= (double) num3)
        return key;
    }
    throw new InvalidOperationException($"Invalid weighted pick for {prototype.ID}!");
  }

  public static string Pick(this IWeightedRandomPrototype prototype, IRobustRandom? random = null)
  {
    IoCManager.Resolve<IRobustRandom>(ref random);
    Dictionary<string, float> weights = prototype.Weights;
    float num1 = weights.Values.Sum();
    float num2 = 0.0f;
    float num3 = random.NextFloat() * num1;
    foreach ((string key, float num4) in weights)
    {
      num2 += num4;
      if ((double) num2 >= (double) num3)
        return key;
    }
    throw new InvalidOperationException($"Invalid weighted pick for {prototype.ID}!");
  }

  public static T Pick<T>(this IRobustRandom random, Dictionary<T, float> weights) where T : notnull
  {
    float num1 = weights.Values.Sum();
    float num2 = 0.0f;
    float num3 = random.NextFloat() * num1;
    foreach ((T key, float num4) in weights)
    {
      num2 += num4;
      if ((double) num2 >= (double) num3)
        return key;
    }
    throw new InvalidOperationException("Invalid weighted pick");
  }

  public static T PickAndTake<T>(this IRobustRandom random, Dictionary<T, float> weights) where T : notnull
  {
    T key = random.Pick<T>(weights);
    weights.Remove(key);
    return key;
  }

  public static bool TryPickAndTake<T>(
    this IRobustRandom random,
    Dictionary<T, float> weights,
    [NotNullWhen(true)] out T? pick)
    where T : notnull
  {
    if (weights.Count == 0)
    {
      pick = default (T);
      return false;
    }
    pick = random.PickAndTake<T>(weights);
    return true;
  }

  public static T Pick<T>(Dictionary<T, float> weights, System.Random random) where T : notnull
  {
    float num1 = weights.Values.Sum();
    float num2 = 0.0f;
    float num3 = random.NextSingle() * num1;
    foreach ((T key, float num4) in weights)
    {
      num2 += num4;
      if ((double) num2 >= (double) num3)
        return key;
    }
    throw new InvalidOperationException("Invalid weighted pick");
  }

  public static (string reagent, FixedPoint2 quantity) Pick(
    this WeightedRandomFillSolutionPrototype prototype,
    IRobustRandom? random = null)
  {
    RandomFillSolution randomFillSolution = prototype.PickRandomFill(random);
    IoCManager.Resolve<IRobustRandom>(ref random);
    int count = randomFillSolution.Reagents.Count;
    float num1 = 0.0f;
    float num2 = random.NextFloat() * (float) count;
    foreach (string reagent in randomFillSolution.Reagents)
    {
      ++num1;
      if ((double) num1 >= (double) num2)
        return (reagent, randomFillSolution.Quantity);
    }
    throw new InvalidOperationException($"Invalid weighted pick for {prototype.ID}!");
  }

  public static RandomFillSolution PickRandomFill(
    this WeightedRandomFillSolutionPrototype prototype,
    IRobustRandom? random = null)
  {
    IoCManager.Resolve<IRobustRandom>(ref random);
    List<RandomFillSolution> fills = prototype.Fills;
    Dictionary<RandomFillSolution, float> dictionary = new Dictionary<RandomFillSolution, float>();
    foreach (RandomFillSolution key in fills)
      dictionary[key] = key.Weight;
    float num1 = dictionary.Values.Sum();
    float num2 = 0.0f;
    float num3 = random.NextFloat() * num1;
    foreach ((RandomFillSolution key, float num4) in dictionary)
    {
      num2 += num4;
      if ((double) num2 >= (double) num3)
        return key;
    }
    throw new InvalidOperationException($"Invalid weighted pick for {prototype.ID}!");
  }

  public static int HashCodeCombine(List<int> values)
  {
    int num1 = 5381;
    foreach (int num2 in values)
      num1 = (num1 << 5) + num1 + num2;
    return num1;
  }
}
