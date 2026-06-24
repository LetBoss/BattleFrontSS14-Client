using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderShopEntryState
{
	public string EntryId { get; }

	public int Price { get; }

	public int BasePrice { get; }

	public float PriceCooldownRemainingSeconds { get; }

	public int PurchaseLimitPerTeam { get; }

	public int PurchasedCount { get; }

	public CivCommanderShopEntryState(string entryId, int price, int basePrice, float priceCooldownRemainingSeconds, int purchaseLimitPerTeam, int purchasedCount)
	{
		EntryId = entryId;
		Price = price;
		BasePrice = basePrice;
		PriceCooldownRemainingSeconds = priceCooldownRemainingSeconds;
		PurchaseLimitPerTeam = purchaseLimitPerTeam;
		PurchasedCount = purchasedCount;
	}
}
