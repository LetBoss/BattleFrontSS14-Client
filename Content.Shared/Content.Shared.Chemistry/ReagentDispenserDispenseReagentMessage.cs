using System;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentDispenserDispenseReagentMessage : BoundUserInterfaceMessage
{
	public readonly ItemStorageLocation StorageLocation;

	public ReagentDispenserDispenseReagentMessage(ItemStorageLocation storageLocation)
	{
		StorageLocation = storageLocation;
	}
}
