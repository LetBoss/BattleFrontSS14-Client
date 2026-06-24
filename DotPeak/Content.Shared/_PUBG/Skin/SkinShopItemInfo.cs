// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.SkinShopItemInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
public sealed class SkinShopItemInfo
{
  public string ItemId { get; }

  public bool IsCollectible { get; }

  public int CollectibleLimit { get; }

  public int SoldCount { get; }

  public int Remaining { get; }

  public bool IsOwnedPermanent { get; }

  public DateTime? ActiveUntil { get; }

  public List<SkinShopOfferInfo> Offers { get; }

  public SkinShopItemInfo(
    string itemId,
    bool isCollectible,
    int collectibleLimit,
    int soldCount,
    int remaining,
    bool isOwnedPermanent,
    DateTime? activeUntil,
    List<SkinShopOfferInfo> offers)
  {
    this.ItemId = itemId;
    this.IsCollectible = isCollectible;
    this.CollectibleLimit = collectibleLimit;
    this.SoldCount = soldCount;
    this.Remaining = remaining;
    this.IsOwnedPermanent = isOwnedPermanent;
    this.ActiveUntil = activeUntil;
    this.Offers = offers;
  }
}
