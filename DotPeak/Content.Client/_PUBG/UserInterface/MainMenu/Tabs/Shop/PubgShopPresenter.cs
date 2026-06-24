// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop.PubgShopPresenter
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.Controls;
using Content.Shared._PUBG.Skin;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopPresenter
{
  private static readonly string[] ClothesCategories = new string[5]
  {
    "jumpsuit",
    "outerClothing",
    "head",
    "neck",
    "shoes"
  };
  private readonly IPrototypeManager _prototypeManager;

  public PubgShopPresenter(IPrototypeManager prototypeManager)
  {
    this._prototypeManager = prototypeManager;
  }

  public List<PubgShopPresentedItem> BuildPresentedItems(
    IReadOnlyList<SkinShopItemInfo> shopItems,
    IReadOnlyDictionary<string, DateTime?> itemExpiresAt,
    PubgShopFilter filter)
  {
    List<PubgShopPresentedItem> source = new List<PubgShopPresentedItem>();
    foreach (SkinShopItemInfo shopItem in (IEnumerable<SkinShopItemInfo>) shopItems)
    {
      EntityPrototype prototype;
      PubgSkinItemComponent skinComponent;
      if (this._prototypeManager.TryIndex<EntityPrototype>(shopItem.ItemId, ref prototype) && prototype.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref skinComponent))
      {
        List<SkinShopOfferInfo> list = SkinContextMenuBuilder.SortShopOffers((IEnumerable<SkinShopOfferInfo>) shopItem.Offers).ToList<SkinShopOfferInfo>();
        if (list.Count != 0)
        {
          DateTime? expiresAt = shopItem.ActiveUntil;
          DateTime? nullable;
          if (!expiresAt.HasValue && itemExpiresAt.TryGetValue(shopItem.ItemId, out nullable))
            expiresAt = nullable;
          bool isTimedOwned = !shopItem.IsOwnedPermanent && expiresAt.HasValue && expiresAt.Value > DateTime.UtcNow;
          bool isLimited = shopItem.IsCollectible || shopItem.CollectibleLimit > 0 || shopItem.SoldCount > 0 || shopItem.Remaining > 0;
          if (PubgShopPresenter.IsAllowedByFilter(filter, skinComponent.Category, isLimited))
            source.Add(new PubgShopPresentedItem(shopItem, prototype, skinComponent, (IReadOnlyList<SkinShopOfferInfo>) list, list[0], expiresAt, isTimedOwned, isLimited));
        }
      }
    }
    return source.OrderByDescending<PubgShopPresentedItem, bool>((Func<PubgShopPresentedItem, bool>) (i => i.IsLimited)).ThenBy<PubgShopPresentedItem, int>((Func<PubgShopPresentedItem, int>) (i => i.LowestOffer.Price)).ThenBy<PubgShopPresentedItem, string>((Func<PubgShopPresentedItem, string>) (i => i.ItemId)).ToList<PubgShopPresentedItem>();
  }

  private static bool IsAllowedByFilter(PubgShopFilter filter, string category, bool isLimited)
  {
    bool flag;
    switch (filter)
    {
      case PubgShopFilter.All:
        flag = true;
        break;
      case PubgShopFilter.Clothes:
        flag = PubgShopPresenter.IsClothesCategory(category);
        break;
      case PubgShopFilter.Ghosts:
        flag = string.Equals(category, "ghost", StringComparison.OrdinalIgnoreCase);
        break;
      case PubgShopFilter.Emotes:
        flag = string.Equals(category, "emote", StringComparison.OrdinalIgnoreCase);
        break;
      case PubgShopFilter.Limited:
        flag = isLimited;
        break;
      default:
        flag = true;
        break;
    }
    return flag;
  }

  private static bool IsClothesCategory(string category)
  {
    foreach (string clothesCategory in PubgShopPresenter.ClothesCategories)
    {
      if (string.Equals(clothesCategory, category, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }
}
