using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events;

[Serializable]
[NetSerializable]
public sealed class CargoConsoleApproveOrderMessage : BoundUserInterfaceMessage
{
	public int OrderId;

	public CargoConsoleApproveOrderMessage(int orderId)
	{
		OrderId = orderId;
	}
}
