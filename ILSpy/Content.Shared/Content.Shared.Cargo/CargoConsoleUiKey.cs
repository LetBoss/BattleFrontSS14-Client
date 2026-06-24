using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo;

[Serializable]
[NetSerializable]
public enum CargoConsoleUiKey : byte
{
	Orders,
	Bounty,
	Shuttle,
	Telepad
}
