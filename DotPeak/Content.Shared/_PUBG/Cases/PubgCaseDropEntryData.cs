// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Cases.PubgCaseDropEntryData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Cases;

[DataDefinition]
public sealed class PubgCaseDropEntryData : 
  ISerializationGenerated<PubgCaseDropEntryData>,
  ISerializationGenerated
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
  public void InternalCopy(
    ref PubgCaseDropEntryData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PubgCaseDropEntryData>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Kind == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Kind, ref target1, hookCtx, false, context))
      target1 = this.Kind;
    target.Kind = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Weight, ref target2, hookCtx, false, context))
      target2 = this.Weight;
    target.Weight = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MinAmount, ref target3, hookCtx, false, context))
      target3 = this.MinAmount;
    target.MinAmount = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxAmount, ref target4, hookCtx, false, context))
      target4 = this.MaxAmount;
    target.MaxAmount = target4;
    List<string> target5 = (List<string>) null;
    if (this.RecipeRarities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.RecipeRarities, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<string>>(this.RecipeRarities, hookCtx, context);
    target.RecipeRarities = target5;
    List<PubgCaseRecipeRarityWeightData> target6 = (List<PubgCaseRecipeRarityWeightData>) null;
    if (this.RecipeRarityWeights == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<PubgCaseRecipeRarityWeightData>>(this.RecipeRarityWeights, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<PubgCaseRecipeRarityWeightData>>(this.RecipeRarityWeights, hookCtx, context);
    target.RecipeRarityWeights = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgCaseDropEntryData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgCaseDropEntryData target1 = (PubgCaseDropEntryData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PubgCaseDropEntryData Instantiate() => new PubgCaseDropEntryData();
}
