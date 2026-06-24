using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinBuyMessage : EntityEventArgs
{
	public string ItemId { get; }

	public string OfferId { get; }

	public SkinBuyMessage(string itemId, string offerId)
	{
		ItemId = itemId;
		OfferId = offerId;
	}
}
