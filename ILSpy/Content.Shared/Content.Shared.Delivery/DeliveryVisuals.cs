using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Delivery;

[Serializable]
[NetSerializable]
public enum DeliveryVisuals : byte
{
	IsLocked,
	IsTrash,
	IsBroken,
	IsFragile,
	IsBomb,
	PriorityState,
	JobIcon
}
