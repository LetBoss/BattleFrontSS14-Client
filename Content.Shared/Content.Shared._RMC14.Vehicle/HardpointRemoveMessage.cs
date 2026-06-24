using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class HardpointRemoveMessage : BoundUserInterfaceMessage
{
	public readonly string SlotId;

	public HardpointRemoveMessage(string slotId)
	{
		SlotId = slotId;
	}
}
