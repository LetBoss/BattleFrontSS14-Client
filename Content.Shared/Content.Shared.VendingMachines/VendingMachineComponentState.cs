using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines;

[Serializable]
[NetSerializable]
public sealed class VendingMachineComponentState : ComponentState
{
	public Dictionary<string, VendingMachineInventoryEntry> Inventory = new Dictionary<string, VendingMachineInventoryEntry>();

	public Dictionary<string, VendingMachineInventoryEntry> EmaggedInventory = new Dictionary<string, VendingMachineInventoryEntry>();

	public Dictionary<string, VendingMachineInventoryEntry> ContrabandInventory = new Dictionary<string, VendingMachineInventoryEntry>();

	public bool Contraband;

	public TimeSpan? EjectEnd;

	public TimeSpan? DenyEnd;

	public TimeSpan? DispenseOnHitEnd;
}
