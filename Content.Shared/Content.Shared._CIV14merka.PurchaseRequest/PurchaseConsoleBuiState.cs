using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseConsoleBuiState : BoundUserInterfaceState
{
	public Dictionary<string, List<CatalogItem>> Catalog { get; init; } = new Dictionary<string, List<CatalogItem>>();

	public List<PurchaseRequest> PendingRequests { get; init; } = new List<PurchaseRequest>();

	public bool IsLeader { get; init; }

	public int AvailablePoints { get; init; }
}
