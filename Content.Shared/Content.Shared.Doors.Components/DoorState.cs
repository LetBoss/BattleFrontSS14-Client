using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components;

[Serializable]
[NetSerializable]
public enum DoorState : byte
{
	Closed,
	Closing,
	Open,
	Opening,
	Welded,
	Denying,
	Emagging
}
