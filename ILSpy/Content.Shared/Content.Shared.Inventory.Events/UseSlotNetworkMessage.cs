using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

[Serializable]
[NetSerializable]
public sealed class UseSlotNetworkMessage : EntityEventArgs
{
	public readonly string Slot;

	public UseSlotNetworkMessage(string slot)
	{
		Slot = slot;
	}
}
