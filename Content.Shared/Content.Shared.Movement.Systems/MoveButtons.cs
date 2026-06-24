using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Systems;

[Serializable]
[Flags]
[NetSerializable]
public enum MoveButtons : byte
{
	None = 0,
	Up = 1,
	Down = 2,
	Left = 4,
	Right = 8,
	Walk = 0x10,
	AnyDirection = 0xF
}
