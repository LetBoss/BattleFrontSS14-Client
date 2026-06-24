// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Cases.PubgCaseRecipeRarityWeightData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._PUBG.Cases;

[DataDefinition]
public sealed class PubgCaseRecipeRarityWeightData : 
  ISerializationGenerated<PubgCaseRecipeRarityWeightData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Rarity { get; private set; } = string.Empty;

  [DataField(null, false, 1, true, false, null)]
  public int Weight { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgCaseRecipeRarityWeightData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PubgCaseRecipeRarityWeightData>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Rarity == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Rarity, ref target1, hookCtx, false, context))
      target1 = this.Rarity;
    target.Rarity = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Weight, ref target2, hookCtx, false, context))
      target2 = this.Weight;
    target.Weight = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgCaseRecipeRarityWeightData target,
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
    PubgCaseRecipeRarityWeightData target1 = (PubgCaseRecipeRarityWeightData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PubgCaseRecipeRarityWeightData Instantiate() => new PubgCaseRecipeRarityWeightData();
}
