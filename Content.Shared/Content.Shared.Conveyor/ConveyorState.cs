using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Conveyor;

[Serializable]
[NetSerializable]
public enum ConveyorState : byte
{
	Off,
	Forward,
	Reverse
}
