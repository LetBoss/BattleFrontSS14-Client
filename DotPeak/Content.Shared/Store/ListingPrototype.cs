// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.ListingPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Store;

[Prototype(null, 1)]
[DataDefinition]
public sealed class ListingPrototype : 
  ListingData,
  IPrototype,
  ISerializationGenerated<ListingPrototype>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Cost
  {
    get => this.OriginalCost;
    set => this.OriginalCost = value;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ListingPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ListingData target1 = (ListingData) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ListingPrototype) target1;
    if (serialization.TryCustomCopy<ListingPrototype>(this, ref target, hookCtx, false, context))
      return;
    IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> target2 = (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) null;
    if (this.Cost == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.Cost, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.Cost, hookCtx, context);
    target.Cost = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ListingPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ListingData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ListingPrototype target1 = (ListingPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ListingData) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ListingPrototype target1 = (ListingPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ListingPrototype ListingData.Instantiate() => new ListingPrototype();
}
