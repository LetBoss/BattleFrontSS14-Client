using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentDispenserBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly ContainerInfo? OutputContainer;

	public readonly NetEntity? OutputContainerEntity;

	public readonly List<ReagentInventoryItem> Inventory;

	public readonly ReagentDispenserDispenseAmount SelectedDispenseAmount;

	public ReagentDispenserBoundUserInterfaceState(ContainerInfo? outputContainer, NetEntity? outputContainerEntity, List<ReagentInventoryItem> inventory, ReagentDispenserDispenseAmount selectedDispenseAmount)
	{
		OutputContainer = outputContainer;
		OutputContainerEntity = outputContainerEntity;
		Inventory = inventory;
		SelectedDispenseAmount = selectedDispenseAmount;
	}
}
