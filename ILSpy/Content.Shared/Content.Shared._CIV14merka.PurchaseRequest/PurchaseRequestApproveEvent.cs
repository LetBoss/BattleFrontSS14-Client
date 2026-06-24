using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestApproveEvent : EntityEventArgs
{
	public Guid RequestId { get; }

	public PurchaseRequestApproveEvent(Guid requestId)
	{
		RequestId = requestId;
	}
}
