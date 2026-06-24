using System;

namespace Content.Shared.Movement.Systems;

[Flags]
public enum ShuttleButtons : byte
{
	None = 0,
	StrafeUp = 1,
	StrafeDown = 2,
	StrafeLeft = 4,
	StrafeRight = 8,
	RotateLeft = 0x10,
	RotateRight = 0x20,
	Brake = 0x40
}
