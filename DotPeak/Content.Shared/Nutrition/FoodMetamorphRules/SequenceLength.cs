// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.FoodMetamorphRules.SequenceLength
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Destructible.Thresholds;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.FoodMetamorphRules;

[NetSerializable]
[Serializable]
public sealed class SequenceLength : 
  FoodMetamorphRule,
  ISerializationGenerated<SequenceLength>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public MinMax Range;

  public override bool Check(
    IPrototypeManager protoMan,
    EntityManager entMan,
    EntityUid food,
    List<FoodSequenceVisualLayer> ingredients)
  {
    return ingredients.Count <= this.Range.Max && ingredients.Count >= this.Range.Min;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SequenceLength target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodMetamorphRule target1 = (FoodMetamorphRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SequenceLength) target1;
    if (serialization.TryCustomCopy<SequenceLength>(this, ref target, hookCtx, false, context))
      return;
    MinMax target2 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.Range, ref target2, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.Range, ref target2, hookCtx, context);
    target.Range = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SequenceLength target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref FoodMetamorphRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SequenceLength target1 = (SequenceLength) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (FoodMetamorphRule) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SequenceLength target1 = (SequenceLength) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SequenceLength FoodMetamorphRule.Instantiate() => new SequenceLength();
}
