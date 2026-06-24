using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
public enum NotificationType
{
	Submitted,
	Approved,
	Denied,
	InsufficientPoints,
	Error
}
