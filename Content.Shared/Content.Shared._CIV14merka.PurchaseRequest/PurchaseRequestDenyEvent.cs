using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestDenyEvent : EntityEventArgs
{
	public Guid RequestId { get; }

	public PurchaseRequestDenyEvent(Guid requestId)
	{
		RequestId = requestId;
	}
}
