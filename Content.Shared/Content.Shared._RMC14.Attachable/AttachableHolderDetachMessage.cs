using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[NetSerializable]
public sealed class AttachableHolderDetachMessage(string slot) : BoundUserInterfaceMessage
{
	public readonly string Slot = slot;
}
