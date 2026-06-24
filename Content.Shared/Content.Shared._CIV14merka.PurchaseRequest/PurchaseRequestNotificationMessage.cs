using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public sealed class PurchaseRequestNotificationMessage : BoundUserInterfaceMessage
{
	public string Message { get; init; } = string.Empty;

	public NotificationType Type { get; init; }
}
