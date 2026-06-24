// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.ListingDataWithCostModifiers
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
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Store;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class ListingDataWithCostModifiers : 
  ListingData,
  ISerializationGenerated<ListingDataWithCostModifiers>,
  ISerializationGenerated
{
  private IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>? _costModified;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>> CostModifiersBySourceId = new Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>();

  public ListingDataWithCostModifiers(ListingData listingData)
    : base(listingData.Name, listingData.DiscountCategory, listingData.Description, listingData.Conditions, listingData.Icon, listingData.Priority, listingData.ProductEntity, listingData.ProductAction, listingData.ProductUpgradeId, listingData.ProductActionEntity, listingData.ProductEvent, listingData.RaiseProductEventOnUser, listingData.PurchaseAmount, listingData.ID, listingData.Categories, listingData.OriginalCost, listingData.RestockTime, listingData.DiscountDownTo, listingData.DisableRefund)
  {
  }

  public bool IsCostModified => this.CostModifiersBySourceId.Count > 0;

  public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Cost
  {
    get
    {
      return this._costModified ?? (this._costModified = this.CostModifiersBySourceId.Count == 0 ? (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>((IEnumerable<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) this.OriginalCost) : (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) this.ApplyAllModifiers());
    }
  }

  public void AddCostModifier(
    string modifierSourceId,
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> modifiers)
  {
    this.CostModifiersBySourceId.Add(modifierSourceId, modifiers);
    if (this._costModified == null)
      return;
    this._costModified = (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) this.ApplyAllModifiers();
  }

  public void RemoveCostModifier(string modifierSourceId)
  {
    this.CostModifiersBySourceId.Remove(modifierSourceId);
    if (this._costModified == null)
      return;
    this._costModified = (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) this.ApplyAllModifiers();
  }

  public bool CanBuyWith(
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> balance)
  {
    foreach ((ProtoId<CurrencyPrototype> key, FixedPoint2 fixedPoint2) in (IEnumerable<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) this.Cost)
    {
      if (!balance.ContainsKey(key) || balance[key] < fixedPoint2)
        return false;
    }
    return true;
  }

  public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, float> GetModifiersSummaryRelative()
  {
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> dictionary = this.CostModifiersBySourceId.Aggregate<KeyValuePair<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>(), (Func<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>) ((accumulator, x) =>
    {
      foreach ((ProtoId<CurrencyPrototype> key, FixedPoint2 fixedPoint2_1) in x.Value)
      {
        FixedPoint2 fixedPoint2_2;
        accumulator.TryGetValue(key, out fixedPoint2_2);
        accumulator[key] = fixedPoint2_2 + fixedPoint2_1;
      }
      return accumulator;
    }));
    Dictionary<ProtoId<CurrencyPrototype>, float> modifiersSummaryRelative = new Dictionary<ProtoId<CurrencyPrototype>, float>();
    foreach ((ProtoId<CurrencyPrototype> key, FixedPoint2 fixedPoint2_3) in dictionary)
    {
      FixedPoint2 fixedPoint2_4;
      if (this.OriginalCost.TryGetValue(key, out fixedPoint2_4))
      {
        float num = (float) fixedPoint2_3.Value / (float) fixedPoint2_4.Value;
        modifiersSummaryRelative.Add(key, num);
      }
    }
    return (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, float>) modifiersSummaryRelative;
  }

  private Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> ApplyAllModifiers()
  {
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> applyTo = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>((IEnumerable<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) this.OriginalCost);
    foreach ((string _, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> modifier) in this.CostModifiersBySourceId)
      this.ApplyModifier(applyTo, (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) modifier);
    return applyTo;
  }

  private void ApplyModifier(
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> applyTo,
    IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> modifier)
  {
    foreach ((ProtoId<CurrencyPrototype> key, FixedPoint2 fixedPoint2_1) in (IEnumerable<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) modifier)
    {
      FixedPoint2 fixedPoint2_2;
      if (applyTo.TryGetValue(key, out fixedPoint2_2))
      {
        FixedPoint2 fixedPoint2_3 = fixedPoint2_2 + fixedPoint2_1;
        if (fixedPoint2_3 < 0)
          fixedPoint2_3 = (FixedPoint2) 0;
        applyTo[key] = fixedPoint2_3;
      }
    }
  }

  public ListingDataWithCostModifiers()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ListingDataWithCostModifiers target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ListingData target1 = (ListingData) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ListingDataWithCostModifiers) target1;
    if (serialization.TryCustomCopy<ListingDataWithCostModifiers>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>> target2 = (Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>) null;
    if (this.CostModifiersBySourceId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>>(this.CostModifiersBySourceId, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>>(this.CostModifiersBySourceId, hookCtx, context);
    target.CostModifiersBySourceId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ListingDataWithCostModifiers target,
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
    ListingDataWithCostModifiers target1 = (ListingDataWithCostModifiers) target;
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
    ListingDataWithCostModifiers target1 = (ListingDataWithCostModifiers) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ListingDataWithCostModifiers ListingData.Instantiate()
  {
    return new ListingDataWithCostModifiers();
  }
}
