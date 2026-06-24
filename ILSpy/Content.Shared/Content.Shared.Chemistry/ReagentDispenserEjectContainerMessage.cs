using System;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentDispenserEjectContainerMessage : BoundUserInterfaceMessage
{
	public readonly ItemStorageLocation StorageLocation;

	public ReagentDispenserEjectContainerMessage(ItemStorageLocation storageLocation)
	{
		StorageLocation = storageLocation;
	}
}
