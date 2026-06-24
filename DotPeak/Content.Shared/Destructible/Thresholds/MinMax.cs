// Decompiled with JetBrains decompiler
// Type: Content.Shared.Destructible.Thresholds.MinMax
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Destructible.Thresholds;

[DataDefinition]
[Serializable]
public struct MinMax : ISerializationGenerated<MinMax>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Min;
  [DataField(null, false, 1, false, false, null)]
  public int Max;

  public MinMax(int min, int max)
  {
    this.Min = min;
    this.Max = max;
  }

  public readonly int Next(IRobustRandom random) => random.Next(this.Min, this.Max + 1);

  public readonly int Next(System.Random random) => random.Next(this.Min, this.Max + 1);

  public MinMax()
  {
    this.Min = 0;
    this.Max = 0;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MinMax target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MinMax>(this, ref target, hookCtx, false, context))
      return;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Min, ref num1, hookCtx, false, context))
      num1 = this.Min;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Max, ref num2, hookCtx, false, context))
      num2 = this.Max;
    target = target with { Min = num1, Max = num2 };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MinMax target,
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
    MinMax target1 = (MinMax) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MinMax Instantiate() => new MinMax();
}
