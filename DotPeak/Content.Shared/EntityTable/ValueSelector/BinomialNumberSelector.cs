// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.ValueSelector.BinomialNumberSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.ValueSelector;

public sealed class BinomialNumberSelector : 
  NumberSelector,
  ISerializationGenerated<BinomialNumberSelector>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Trials = 1;
  [DataField(null, false, 1, false, false, null)]
  public float Chance = 0.5f;

  public override int Get(System.Random rand)
  {
    IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
    int num = 0;
    for (int index = 0; index < this.Trials; ++index)
    {
      if (random.Prob(this.Chance))
        ++num;
    }
    return num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BinomialNumberSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NumberSelector target1 = (NumberSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BinomialNumberSelector) target1;
    if (serialization.TryCustomCopy<BinomialNumberSelector>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Trials, ref target2, hookCtx, false, context))
      target2 = this.Trials;
    target.Trials = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Chance, ref target3, hookCtx, false, context))
      target3 = this.Chance;
    target.Chance = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BinomialNumberSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref NumberSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BinomialNumberSelector target1 = (BinomialNumberSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (NumberSelector) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BinomialNumberSelector target1 = (BinomialNumberSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BinomialNumberSelector NumberSelector.Instantiate() => new BinomialNumberSelector();
}
