using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestSubmitMessage : BoundUserInterfaceMessage
{
	public List<PurchaseItem> Items { get; init; } = new List<PurchaseItem>();
}
