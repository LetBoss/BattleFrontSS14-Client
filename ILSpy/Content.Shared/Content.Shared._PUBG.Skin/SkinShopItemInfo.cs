using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
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

	public SkinShopItemInfo(string itemId, bool isCollectible, int collectibleLimit, int soldCount, int remaining, bool isOwnedPermanent, DateTime? activeUntil, List<SkinShopOfferInfo> offers)
	{
		ItemId = itemId;
		IsCollectible = isCollectible;
		CollectibleLimit = collectibleLimit;
		SoldCount = soldCount;
		Remaining = remaining;
		IsOwnedPermanent = isOwnedPermanent;
		ActiveUntil = activeUntil;
		Offers = offers;
	}
}
