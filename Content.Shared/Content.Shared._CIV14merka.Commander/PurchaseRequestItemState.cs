using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestItemState
{
	public string ItemId { get; }

	public string ItemName { get; }

	public int Quantity { get; }

	public PurchaseRequestItemState(string itemId, string itemName, int quantity)
	{
		ItemId = itemId;
		ItemName = itemName;
		Quantity = quantity;
	}
}
