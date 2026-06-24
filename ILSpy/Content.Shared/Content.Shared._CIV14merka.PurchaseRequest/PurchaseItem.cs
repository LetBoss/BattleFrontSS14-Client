using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseItem
{
	public string ItemId { get; init; } = string.Empty;

	public string ItemName { get; init; } = string.Empty;

	public int Quantity { get; init; }

	public int UnitPrice { get; init; }

	public int TotalPrice => Quantity * UnitPrice;
}
