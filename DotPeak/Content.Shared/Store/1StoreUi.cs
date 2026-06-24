// Decompiled with JetBrains decompiler
// Type: Content.Shared.Store.StoreUpdateState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Store;

[NetSerializable]
[Serializable]
public sealed class StoreUpdateState : BoundUserInterfaceState
{
  public readonly HashSet<ListingDataWithCostModifiers> Listings;
  public readonly Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Balance;
  public readonly bool ShowFooter;
  public readonly bool AllowRefund;

  public StoreUpdateState(
    HashSet<ListingDataWithCostModifiers> listings,
    Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> balance,
    bool showFooter,
    bool allowRefund)
  {
    this.Listings = listings;
    this.Balance = balance;
    this.ShowFooter = showFooter;
    this.AllowRefund = allowRefund;
  }
}
