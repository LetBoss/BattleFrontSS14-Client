using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo;

[Serializable]
[NetSerializable]
public enum CargoTelepadState : byte
{
	Unpowered,
	Idle,
	Teleporting
}
