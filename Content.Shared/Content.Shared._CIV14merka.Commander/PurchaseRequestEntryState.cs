using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestEntryState
{
	public string RequestId { get; }

	public string RequesterName { get; }

	public string Faction { get; }

	public List<PurchaseRequestItemState> Items { get; }

	public int TotalPrice { get; }

	public double RequestTime { get; }

	public PurchaseRequestEntryState(string requestId, string requesterName, string faction, IEnumerable<PurchaseRequestItemState> items, int totalPrice, double requestTime)
	{
		RequestId = requestId;
		RequesterName = requesterName;
		Faction = faction;
		Items = items.ToList();
		TotalPrice = totalPrice;
		RequestTime = requestTime;
	}
}
