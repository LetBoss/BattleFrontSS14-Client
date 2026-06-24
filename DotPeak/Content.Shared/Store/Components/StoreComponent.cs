// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.Components.StoreComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Store.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StoreComponent : 
  Component,
  ISerializationGenerated<StoreComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId Name = (LocId) "store-ui-default-title";
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<StoreCategoryPrototype>> Categories = new HashSet<ProtoId<StoreCategoryPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Balance = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<CurrencyPrototype>> CurrencyWhitelist = new HashSet<ProtoId<CurrencyPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? AccountOwner;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ListingDataWithCostModifiers> FullListingsCatalog = new HashSet<ListingDataWithCostModifiers>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<ListingDataWithCostModifiers> LastAvailableListings = new HashSet<ListingDataWithCostModifiers>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> BoughtEntities = new List<EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> BalanceSpent = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public bool RefundAllowed;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool OwnerOnly;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? StartingMap;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BuySuccessSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/kaching.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StoreComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StoreComponent) target1;
    if (serialization.TryCustomCopy<StoreComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Name, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.Name, hookCtx, context);
    target.Name = target2;
    HashSet<ProtoId<StoreCategoryPrototype>> target3 = (HashSet<ProtoId<StoreCategoryPrototype>>) null;
    if (this.Categories == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<StoreCategoryPrototype>>>(this.Categories, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<StoreCategoryPrototype>>>(this.Categories, hookCtx, context);
    target.Categories = target3;
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> target4 = (Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) null;
    if (this.Balance == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.Balance, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.Balance, hookCtx, context);
    target.Balance = target4;
    HashSet<ProtoId<CurrencyPrototype>> target5 = (HashSet<ProtoId<CurrencyPrototype>>) null;
    if (this.CurrencyWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<CurrencyPrototype>>>(this.CurrencyWhitelist, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<ProtoId<CurrencyPrototype>>>(this.CurrencyWhitelist, hookCtx, context);
    target.CurrencyWhitelist = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AccountOwner, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.AccountOwner, hookCtx, context);
    target.AccountOwner = target6;
    HashSet<ListingDataWithCostModifiers> target7 = (HashSet<ListingDataWithCostModifiers>) null;
    if (this.FullListingsCatalog == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ListingDataWithCostModifiers>>(this.FullListingsCatalog, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<HashSet<ListingDataWithCostModifiers>>(this.FullListingsCatalog, hookCtx, context);
    target.FullListingsCatalog = target7;
    List<EntityUid> target8 = (List<EntityUid>) null;
    if (this.BoughtEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.BoughtEntities, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<EntityUid>>(this.BoughtEntities, hookCtx, context);
    target.BoughtEntities = target8;
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> target9 = (Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>) null;
    if (this.BalanceSpent == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.BalanceSpent, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(this.BalanceSpent, hookCtx, context);
    target.BalanceSpent = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.RefundAllowed, ref target10, hookCtx, false, context))
      target10 = this.RefundAllowed;
    target.RefundAllowed = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.OwnerOnly, ref target11, hookCtx, false, context))
      target11 = this.OwnerOnly;
    target.OwnerOnly = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.StartingMap, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.StartingMap, hookCtx, context);
    target.StartingMap = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (this.BuySuccessSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BuySuccessSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.BuySuccessSound, hookCtx, context);
    target.BuySuccessSound = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StoreComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StoreComponent target1 = (StoreComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StoreComponent target1 = (StoreComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StoreComponent target1 = (StoreComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StoreComponent Component.Instantiate() => new StoreComponent();
}
