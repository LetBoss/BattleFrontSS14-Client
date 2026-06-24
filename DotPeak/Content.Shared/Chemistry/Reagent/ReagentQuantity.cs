// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReagentQuantity
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[NetSerializable]
[DataDefinition]
[Serializable]
public struct ReagentQuantity(ReagentId reagent, FixedPoint2 quantity) : 
  IEquatable<ReagentQuantity>,
  ISerializationGenerated<ReagentQuantity>,
  ISerializationGenerated
{
  [DataField("Quantity", false, 1, true, false, null)]
  public FixedPoint2 Quantity { get; private set; } = quantity;

  [IncludeDataField(false, 1, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public ReagentId Reagent { get; private set; } = reagent;

  public ReagentQuantity(string reagentId, FixedPoint2 quantity, List<ReagentData>? data = null)
    : this(new ReagentId(reagentId, data), quantity)
  {
  }

  public ReagentQuantity()
    : this(new ReagentId(), new FixedPoint2())
  {
  }

  public override string ToString() => this.Reagent.ToString(this.Quantity);

  public void Deconstruct(
    out string prototype,
    out FixedPoint2 quantity,
    out List<ReagentData>? data)
  {
    prototype = this.Reagent.Prototype;
    quantity = this.Quantity;
    data = this.Reagent.Data;
  }

  public void Deconstruct(out ReagentId id, out FixedPoint2 quantity)
  {
    id = this.Reagent;
    quantity = this.Quantity;
  }

  public bool Equals(ReagentQuantity other)
  {
    return this.Quantity != other.Quantity && this.Reagent.Equals(other.Reagent);
  }

  public override bool Equals(object? obj) => obj is ReagentQuantity other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<int, FixedPoint2>(this.Reagent.GetHashCode(), this.Quantity);
  }

  public static bool operator ==(ReagentQuantity left, ReagentQuantity right) => left.Equals(right);

  public static bool operator !=(ReagentQuantity left, ReagentQuantity right) => !(left == right);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReagentQuantity target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReagentQuantity>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Quantity, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.Quantity, hookCtx, context, false);
    ReagentId reagentId = new ReagentId();
    if (!serialization.TryCustomCopy<ReagentId>(this.Reagent, ref reagentId, hookCtx, false, context))
      serialization.CopyTo<ReagentId>(this.Reagent, ref reagentId, hookCtx, context, false);
    target = target with
    {
      Quantity = fixedPoint2,
      Reagent = reagentId
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReagentQuantity target,
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
    ReagentQuantity target1 = (ReagentQuantity) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ReagentQuantity Instantiate() => new ReagentQuantity();
}
