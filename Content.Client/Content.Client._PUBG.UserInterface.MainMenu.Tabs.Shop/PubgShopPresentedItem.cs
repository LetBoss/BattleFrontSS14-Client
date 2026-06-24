using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Skin;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopPresentedItem
{
	public string ItemId => ShopItem.ItemId;

	public SkinShopItemInfo ShopItem { get; }

	public EntityPrototype Prototype { get; }

	public PubgSkinItemComponent SkinComponent { get; }

	public IReadOnlyList<SkinShopOfferInfo> Offers { get; }

	public SkinShopOfferInfo LowestOffer { get; }

	public DateTime? ExpiresAt { get; }

	public bool IsTimedOwned { get; }

	public bool IsLimited { get; }

	public bool IsCollectible => ShopItem.IsCollectible;

	public int CollectibleLimit => ShopItem.CollectibleLimit;

	public int SoldCount => ShopItem.SoldCount;

	public bool IsOwnedPermanent => ShopItem.IsOwnedPermanent;

	public int Remaining => ShopItem.Remaining;

	public PubgShopPresentedItem(SkinShopItemInfo shopItem, EntityPrototype prototype, PubgSkinItemComponent skinComponent, IReadOnlyList<SkinShopOfferInfo> offers, SkinShopOfferInfo lowestOffer, DateTime? expiresAt, bool isTimedOwned, bool isLimited)
	{
		ShopItem = shopItem;
		Prototype = prototype;
		SkinComponent = skinComponent;
		Offers = offers;
		LowestOffer = lowestOffer;
		ExpiresAt = expiresAt;
		IsTimedOwned = isTimedOwned;
		IsLimited = isLimited;
	}
}
