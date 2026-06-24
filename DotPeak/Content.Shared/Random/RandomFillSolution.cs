// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.RandomFillSolution
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using System;

#nullable enable
namespace Content.Shared.Random;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class RandomFillSolution : 
  ISerializationGenerated<RandomFillSolution>,
  ISerializationGenerated
{
  [DataField("quantity", false, 1, false, false, null)]
  public FixedPoint2 Quantity = (FixedPoint2) 0;
  [DataField("weight", false, 1, false, false, null)]
  public float Weight;
  [DataField("reagents", false, 1, true, false, typeof (PrototypeIdListSerializer<ReagentPrototype>))]
  public System.Collections.Generic.List<string> Reagents = new System.Collections.Generic.List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomFillSolution target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RandomFillSolution>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Quantity, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<FixedPoint2>(this.Quantity, hookCtx, context);
    target.Quantity = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Weight, ref target2, hookCtx, false, context))
      target2 = this.Weight;
    target.Weight = target2;
    System.Collections.Generic.List<string> target3 = (System.Collections.Generic.List<string>) null;
    if (this.Reagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.Reagents, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.Reagents, hookCtx, context);
    target.Reagents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomFillSolution target,
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
    RandomFillSolution target1 = (RandomFillSolution) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RandomFillSolution Instantiate() => new RandomFillSolution();
}
