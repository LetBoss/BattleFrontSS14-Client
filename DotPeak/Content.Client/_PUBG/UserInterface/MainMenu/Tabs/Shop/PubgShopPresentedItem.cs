// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop.PubgShopPresentedItem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Skin;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopPresentedItem
{
  public string ItemId => this.ShopItem.ItemId;

  public SkinShopItemInfo ShopItem { get; }

  public EntityPrototype Prototype { get; }

  public PubgSkinItemComponent SkinComponent { get; }

  public IReadOnlyList<SkinShopOfferInfo> Offers { get; }

  public SkinShopOfferInfo LowestOffer { get; }

  public DateTime? ExpiresAt { get; }

  public bool IsTimedOwned { get; }

  public bool IsLimited { get; }

  public bool IsCollectible => this.ShopItem.IsCollectible;

  public int CollectibleLimit => this.ShopItem.CollectibleLimit;

  public int SoldCount => this.ShopItem.SoldCount;

  public bool IsOwnedPermanent => this.ShopItem.IsOwnedPermanent;

  public int Remaining => this.ShopItem.Remaining;

  public PubgShopPresentedItem(
    SkinShopItemInfo shopItem,
    EntityPrototype prototype,
    PubgSkinItemComponent skinComponent,
    IReadOnlyList<SkinShopOfferInfo> offers,
    SkinShopOfferInfo lowestOffer,
    DateTime? expiresAt,
    bool isTimedOwned,
    bool isLimited)
  {
    this.ShopItem = shopItem;
    this.Prototype = prototype;
    this.SkinComponent = skinComponent;
    this.Offers = offers;
    this.LowestOffer = lowestOffer;
    this.ExpiresAt = expiresAt;
    this.IsTimedOwned = isTimedOwned;
    this.IsLimited = isLimited;
  }
}
