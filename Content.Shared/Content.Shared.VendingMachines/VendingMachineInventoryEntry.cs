using System;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.VendingMachines;

[Serializable]
[NetSerializable]
public sealed class VendingMachineInventoryEntry
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public InventoryType Type;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ID;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public uint Amount;

	public VendingMachineInventoryEntry(InventoryType type, string id, uint amount)
	{
		Type = type;
		ID = id;
		Amount = amount;
	}

	public VendingMachineInventoryEntry(VendingMachineInventoryEntry entry)
	{
		Type = entry.Type;
		ID = entry.ID;
		Amount = entry.Amount;
	}
}
