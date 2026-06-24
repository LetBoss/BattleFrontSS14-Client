using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._PUBG.UserInterface.Controls;
using Content.Shared._PUBG.Skin;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopPresenter
{
	private static readonly string[] ClothesCategories = new string[5] { "jumpsuit", "outerClothing", "head", "neck", "shoes" };

	private readonly IPrototypeManager _prototypeManager;

	public PubgShopPresenter(IPrototypeManager prototypeManager)
	{
		_prototypeManager = prototypeManager;
	}

	public List<PubgShopPresentedItem> BuildPresentedItems(IReadOnlyList<SkinShopItemInfo> shopItems, IReadOnlyDictionary<string, DateTime?> itemExpiresAt, PubgShopFilter filter)
	{
		List<PubgShopPresentedItem> list = new List<PubgShopPresentedItem>();
		EntityPrototype val = default(EntityPrototype);
		PubgSkinItemComponent pubgSkinItemComponent = default(PubgSkinItemComponent);
		foreach (SkinShopItemInfo shopItem in shopItems)
		{
			if (!_prototypeManager.TryIndex<EntityPrototype>(shopItem.ItemId, ref val) || !val.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref pubgSkinItemComponent))
			{
				continue;
			}
			List<SkinShopOfferInfo> list2 = SkinContextMenuBuilder.SortShopOffers(shopItem.Offers).ToList();
			if (list2.Count != 0)
			{
				DateTime? expiresAt = shopItem.ActiveUntil;
				if (!expiresAt.HasValue && itemExpiresAt.TryGetValue(shopItem.ItemId, out var value))
				{
					expiresAt = value;
				}
				bool isTimedOwned = !shopItem.IsOwnedPermanent && expiresAt.HasValue && expiresAt.Value > DateTime.UtcNow;
				bool isLimited = shopItem.IsCollectible || shopItem.CollectibleLimit > 0 || shopItem.SoldCount > 0 || shopItem.Remaining > 0;
				if (IsAllowedByFilter(filter, pubgSkinItemComponent.Category, isLimited))
				{
					list.Add(new PubgShopPresentedItem(shopItem, val, pubgSkinItemComponent, list2, list2[0], expiresAt, isTimedOwned, isLimited));
				}
			}
		}
		return (from i in list
			orderby i.IsLimited descending, i.LowestOffer.Price, i.ItemId
			select i).ToList();
	}

	private static bool IsAllowedByFilter(PubgShopFilter filter, string category, bool isLimited)
	{
		return filter switch
		{
			PubgShopFilter.All => true, 
			PubgShopFilter.Clothes => IsClothesCategory(category), 
			PubgShopFilter.Ghosts => string.Equals(category, "ghost", StringComparison.OrdinalIgnoreCase), 
			PubgShopFilter.Emotes => string.Equals(category, "emote", StringComparison.OrdinalIgnoreCase), 
			PubgShopFilter.Limited => isLimited, 
			_ => true, 
		};
	}

	private static bool IsClothesCategory(string category)
	{
		string[] clothesCategories = ClothesCategories;
		for (int i = 0; i < clothesCategories.Length; i++)
		{
			if (string.Equals(clothesCategories[i], category, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
