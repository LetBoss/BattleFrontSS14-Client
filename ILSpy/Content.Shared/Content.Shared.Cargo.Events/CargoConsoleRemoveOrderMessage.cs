using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events;

[Serializable]
[NetSerializable]
public sealed class CargoConsoleRemoveOrderMessage : BoundUserInterfaceMessage
{
	public int OrderId;

	public CargoConsoleRemoveOrderMessage(int orderId)
	{
		OrderId = orderId;
	}
}
