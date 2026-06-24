using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Delivery;

[Serializable]
[NetSerializable]
public enum DeliveryPriorityState : byte
{
	Off,
	Active,
	Inactive
}
