// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.ValueSelector.RangeNumberSelector
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityTable.ValueSelector;

public sealed class RangeNumberSelector : 
  NumberSelector,
  ISerializationGenerated<RangeNumberSelector>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Vector2i Range = new Vector2i(1, 1);

  public RangeNumberSelector(Vector2i range) => this.Range = range;

  public override int Get(Random rand) => rand.Next(this.Range.X, this.Range.Y + 1);

  public RangeNumberSelector()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RangeNumberSelector target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NumberSelector target1 = (NumberSelector) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RangeNumberSelector) target1;
    if (serialization.TryCustomCopy<RangeNumberSelector>(this, ref target, hookCtx, false, context))
      return;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Range, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Range, hookCtx, context);
    target.Range = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RangeNumberSelector target,
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
    RangeNumberSelector target1 = (RangeNumberSelector) target;
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
    RangeNumberSelector target1 = (RangeNumberSelector) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RangeNumberSelector NumberSelector.Instantiate() => new RangeNumberSelector();
}
