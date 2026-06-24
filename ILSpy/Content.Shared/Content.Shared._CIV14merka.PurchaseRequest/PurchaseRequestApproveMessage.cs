using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestApproveMessage : BoundUserInterfaceMessage
{
	public Guid RequestId { get; init; }
}
