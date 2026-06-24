// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderShopEntryState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivCommanderShopEntryState
{
  public string EntryId { get; }

  public int Price { get; }

  public int BasePrice { get; }

  public float PriceCooldownRemainingSeconds { get; }

  public int PurchaseLimitPerTeam { get; }

  public int PurchasedCount { get; }

  public CivCommanderShopEntryState(
    string entryId,
    int price,
    int basePrice,
    float priceCooldownRemainingSeconds,
    int purchaseLimitPerTeam,
    int purchasedCount)
  {
    this.EntryId = entryId;
    this.Price = price;
    this.BasePrice = basePrice;
    this.PriceCooldownRemainingSeconds = priceCooldownRemainingSeconds;
    this.PurchaseLimitPerTeam = purchaseLimitPerTeam;
    this.PurchasedCount = purchasedCount;
  }
}
