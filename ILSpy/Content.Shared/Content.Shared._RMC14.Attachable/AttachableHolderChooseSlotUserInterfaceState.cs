using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[NetSerializable]
public sealed class AttachableHolderChooseSlotUserInterfaceState(List<string> attachableSlots) : BoundUserInterfaceState
{
	public List<string> AttachableSlots = attachableSlots;
}
