using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinShopOfferInfo
{
	public string OfferId { get; }

	public string Currency { get; }

	public int Price { get; }

	public int? DurationDays { get; }

	public SkinShopOfferInfo(string offerId, string currency, int price, int? durationDays)
	{
		OfferId = offerId;
		Currency = currency;
		Price = price;
		DurationDays = durationDays;
	}
}
