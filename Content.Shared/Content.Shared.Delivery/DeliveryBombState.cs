using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Delivery;

[Serializable]
[NetSerializable]
public enum DeliveryBombState : byte
{
	Off,
	Inactive,
	Primed
}
