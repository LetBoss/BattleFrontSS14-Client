using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components;

[Serializable]
[NetSerializable]
public enum DoorLocation : byte
{
	None,
	Aft,
	Bow,
	Cockpit,
	Port,
	Starboard
}
