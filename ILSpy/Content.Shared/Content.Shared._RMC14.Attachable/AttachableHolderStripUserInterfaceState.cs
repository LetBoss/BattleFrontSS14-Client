using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[NetSerializable]
public sealed class AttachableHolderStripUserInterfaceState(Dictionary<string, (string?, bool)> attachableSlots) : BoundUserInterfaceState
{
	public Dictionary<string, (string?, bool)> AttachableSlots = attachableSlots;
}
