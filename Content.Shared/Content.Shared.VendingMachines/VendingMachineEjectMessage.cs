using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines;

[Serializable]
[NetSerializable]
public sealed class VendingMachineEjectMessage : BoundUserInterfaceMessage
{
	public readonly InventoryType Type;

	public readonly string ID;

	public VendingMachineEjectMessage(InventoryType type, string id)
	{
		Type = type;
		ID = id;
	}
}
