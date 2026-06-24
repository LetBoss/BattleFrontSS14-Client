// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.ListingData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Store;

[NetSerializable]
[Virtual]
[DataDefinition]
[Serializable]
public class ListingData : 
  IEquatable<ListingData>,
  ISerializationGenerated<ListingData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string? Name;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<DiscountCategoryPrototype>? DiscountCategory;
  [DataField(null, false, 1, false, false, null)]
  public string? Description;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<StoreCategoryPrototype>> Categories = new HashSet<ProtoId<StoreCategoryPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> OriginalCost = (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();
  [DataField(null, false, 1, false, true, null)]
  [NonSerialized]
  public List<ListingCondition>? Conditions;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon;
  [DataField(null, false, 1, false, false, null)]
  public int Priority;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? ProductEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? ProductAction;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ListingPrototype>? ProductUpgradeId;
  [DataField(null, false, 1, false, false, null)]
  [NonSerialized]
  public EntityUid? ProductActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public object? ProductEvent;
  [DataField(null, false, 1, false, false, null)]
  public bool RaiseProductEventOnUser;
  [DataField(null, false, 1, false, false, null)]
  public int PurchaseAmount;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RestockTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> DiscountDownTo = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();
  [DataField(null, false, 1, false, false, null)]
  public bool DisableRefund;

  public ListingData()
  {
  }

  public ListingData(ListingData other)
    : this(other.Name, other.DiscountCategory, other.Description, other.Conditions, other.Icon, other.Priority, other.ProductEntity, other.ProductAction, other.ProductUpgradeId, other.ProductActionEntity, other.ProductEvent, other.RaiseProductEventOnUser, other.PurchaseAmount, other.ID, other.Categories, other.OriginalCost, other.RestockTime, other.DiscountDownTo, other.DisableRefund)
  {
  }

  public ListingData(
    string? name,
    ProtoId<DiscountCategoryPrototype>? discountCategory,
    string? description,
    List<ListingCondition>? conditions,
    SpriteSpecifier? icon,
    int priority,
    EntProtoId? productEntity,
    EntProtoId? productAction,
    ProtoId<ListingPrototype>? productUpgradeId,
    EntityUid? productActionEntity,
    object? productEvent,
    bool raiseProductEventOnUser,
    int purchaseAmount,
    string id,
    HashSet<ProtoId<StoreCategoryPrototype>> categories,
    IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> originalCost,
    TimeSpan restockTime,
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> dataDiscountDownTo,
    bool disableRefund)
  {
    this.Name = name;
    this.DiscountCategory = discountCategory;
    this.Description = description;
    this.Conditions = conditions != null ? conditions.ToList<ListingCondition>() : (List<ListingCondition>) null;
    this.Icon = icon;
    this.Priority = priority;
    this.ProductEntity = productEntity;
    this.ProductAction = productAction;
    this.ProductUpgradeId = productUpgradeId;
    this.ProductActionEntity = productActionEntity;
    this.ProductEvent = productEvent;
    this.RaiseProductEventOnUser = raiseProductEventOnUser;
    this.PurchaseAmount = purchaseAmount;
    this.ID = id;
    this.Categories = categories.ToHashSet<ProtoId<StoreCategoryPrototype>>();
    this.OriginalCost = originalCost;
    this.RestockTime = restockTime;
    this.DiscountDownTo = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>((IDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) dataDiscountDownTo);
    this.DisableRefund = disableRefund;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  public bool Equals(ListingData? listing)
  {
    if (listing == null || this.Priority != listing.Priority || this.Name != listing.Name || this.Description != listing.Description)
      return false;
    EntProtoId? productEntity = this.ProductEntity;
    EntProtoId? nullable = listing.ProductEntity;
    if ((productEntity.HasValue == nullable.HasValue ? (productEntity.HasValue ? (productEntity.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
    {
      nullable = this.ProductAction;
      EntProtoId? productAction = listing.ProductAction;
      if ((nullable.HasValue == productAction.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != productAction.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && !(this.ProductEvent?.GetType() != listing.ProductEvent?.GetType()) && !(this.RestockTime != listing.RestockTime) && (this.Icon == null || this.Icon.Equals((object) listing.Icon)) && this.Categories.OrderBy<ProtoId<StoreCategoryPrototype>, ProtoId<StoreCategoryPrototype>>((Func<ProtoId<StoreCategoryPrototype>, ProtoId<StoreCategoryPrototype>>) (x => x)).SequenceEqual<ProtoId<StoreCategoryPrototype>>((IEnumerable<ProtoId<StoreCategoryPrototype>>) listing.Categories.OrderBy<ProtoId<StoreCategoryPrototype>, ProtoId<StoreCategoryPrototype>>((Func<ProtoId<StoreCategoryPrototype>, ProtoId<StoreCategoryPrototype>>) (x => x))) && this.OriginalCost.OrderBy<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>((Func<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) (x => x)).SequenceEqual<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>((IEnumerable<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) listing.OriginalCost.OrderBy<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>((Func<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>) (x => x))) && (this.Conditions == null || listing.Conditions == null || this.Conditions.OrderBy<ListingCondition, ListingCondition>((Func<ListingCondition, ListingCondition>) (x => x)).SequenceEqual<ListingCondition>((IEnumerable<ListingCondition>) listing.Conditions.OrderBy<ListingCondition, ListingCondition>((Func<ListingCondition, ListingCondition>) (x => x)))))
        return true;
    }
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref ListingData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ListingData>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Name, ref target2, hookCtx, false, context))
      target2 = this.Name;
    target.Name = target2;
    ProtoId<DiscountCategoryPrototype>? target3 = new ProtoId<DiscountCategoryPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<DiscountCategoryPrototype>?>(this.DiscountCategory, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<DiscountCategoryPrototype>?>(this.DiscountCategory, hookCtx, context);
    target.DiscountCategory = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Description, ref target4, hookCtx, false, context))
      target4 = this.Description;
    target.Description = target4;
    HashSet<ProtoId<StoreCategoryPrototype>> target5 = (HashSet<ProtoId<StoreCategoryPrototype>>) null;
    if (this.Categories == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<StoreCategoryPrototype>>>(this.Categories, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<ProtoId<StoreCategoryPrototype>>>(this.Categories, hookCtx, context);
    target.Categories = target5;
    IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> target6 = (IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) null;
    if (this.OriginalCost == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.OriginalCost, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.OriginalCost, hookCtx, context);
    target.OriginalCost = target6;
    List<ListingCondition> target7 = (List<ListingCondition>) null;
    if (!serialization.TryCustomCopy<List<ListingCondition>>(this.Conditions, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<ListingCondition>>(this.Conditions, hookCtx, context);
    target.Conditions = target7;
    SpriteSpecifier target8 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Priority, ref target9, hookCtx, false, context))
      target9 = this.Priority;
    target.Priority = target9;
    EntProtoId? target10 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ProductEntity, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId?>(this.ProductEntity, hookCtx, context);
    target.ProductEntity = target10;
    EntProtoId? target11 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ProductAction, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId?>(this.ProductAction, hookCtx, context);
    target.ProductAction = target11;
    ProtoId<ListingPrototype>? target12 = new ProtoId<ListingPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ListingPrototype>?>(this.ProductUpgradeId, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<ProtoId<ListingPrototype>?>(this.ProductUpgradeId, hookCtx, context);
    target.ProductUpgradeId = target12;
    EntityUid? target13 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ProductActionEntity, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntityUid?>(this.ProductActionEntity, hookCtx, context);
    target.ProductActionEntity = target13;
    object target14 = (object) null;
    if (!serialization.TryCustomCopy<object>(this.ProductEvent, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy(this.ProductEvent, hookCtx, context);
    target.ProductEvent = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.RaiseProductEventOnUser, ref target15, hookCtx, false, context))
      target15 = this.RaiseProductEventOnUser;
    target.RaiseProductEventOnUser = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.PurchaseAmount, ref target16, hookCtx, false, context))
      target16 = this.PurchaseAmount;
    target.PurchaseAmount = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RestockTime, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.RestockTime, hookCtx, context);
    target.RestockTime = target17;
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> target18 = (Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) null;
    if (this.DiscountDownTo == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.DiscountDownTo, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.DiscountDownTo, hookCtx, context);
    target.DiscountDownTo = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableRefund, ref target19, hookCtx, false, context))
      target19 = this.DisableRefund;
    target.DisableRefund = target19;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref ListingData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ListingData target1 = (ListingData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual ListingData Instantiate() => new ListingData();
}
