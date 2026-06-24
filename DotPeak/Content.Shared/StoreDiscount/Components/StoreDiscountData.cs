// Decompiled with JetBrains decompiler
// Type: Content.Shared.StoreDiscount.Components.StoreDiscountData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.StoreDiscount.Components;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class StoreDiscountData : 
  ISerializationGenerated<StoreDiscountData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ListingPrototype> ListingId;
  [DataField(null, false, 1, false, false, null)]
  public int Count;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<DiscountCategoryPrototype> DiscountCategory;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> DiscountAmountByCurrency = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StoreDiscountData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<StoreDiscountData>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ListingPrototype> target1 = new ProtoId<ListingPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ListingPrototype>>(this.ListingId, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<ListingPrototype>>(this.ListingId, hookCtx, context);
    target.ListingId = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target2, hookCtx, false, context))
      target2 = this.Count;
    target.Count = target2;
    ProtoId<DiscountCategoryPrototype> target3 = new ProtoId<DiscountCategoryPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DiscountCategoryPrototype>>(this.DiscountCategory, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<DiscountCategoryPrototype>>(this.DiscountCategory, hookCtx, context);
    target.DiscountCategory = target3;
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> target4 = (Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) null;
    if (this.DiscountAmountByCurrency == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.DiscountAmountByCurrency, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.DiscountAmountByCurrency, hookCtx, context);
    target.DiscountAmountByCurrency = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StoreDiscountData target,
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
    StoreDiscountData target1 = (StoreDiscountData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public StoreDiscountData Instantiate() => new StoreDiscountData();
}
