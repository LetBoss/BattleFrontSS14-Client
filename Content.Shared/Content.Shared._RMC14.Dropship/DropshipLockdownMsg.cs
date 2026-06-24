using System;
using Content.Shared.Doors.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[Serializable]
[NetSerializable]
public sealed class DropshipLockdownMsg(DoorLocation doorLocation) : BoundUserInterfaceMessage
{
	public readonly DoorLocation DoorLocation = doorLocation;
}
