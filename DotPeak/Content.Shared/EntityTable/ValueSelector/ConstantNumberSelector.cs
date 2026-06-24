// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.ValueSelector.ConstantNumberSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.ValueSelector;

public sealed class ConstantNumberSelector : 
  NumberSelector,
  ISerializationGenerated<ConstantNumberSelector>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Value = 1;

  public ConstantNumberSelector(int value) => this.Value = value;

  public override int Get(Random rand) => this.Value;

  public ConstantNumberSelector()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConstantNumberSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NumberSelector target1 = (NumberSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ConstantNumberSelector) target1;
    if (serialization.TryCustomCopy<ConstantNumberSelector>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Value, ref target2, hookCtx, false, context))
      target2 = this.Value;
    target.Value = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConstantNumberSelector target,
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
    ConstantNumberSelector target1 = (ConstantNumberSelector) target;
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
    ConstantNumberSelector target1 = (ConstantNumberSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ConstantNumberSelector NumberSelector.Instantiate() => new ConstantNumberSelector();
}
