using System;
using System.Collections.Generic;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequest
{
	public NetUserId RequesterId { get; init; }

	public string RequesterName { get; init; } = string.Empty;

	public List<PurchaseItem> Items { get; init; } = new List<PurchaseItem>();

	public int TotalPrice { get; init; }

	public TimeSpan RequestTime { get; init; }

	public Guid RequestId { get; init; }
}
