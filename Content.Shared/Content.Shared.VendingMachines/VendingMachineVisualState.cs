using System;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines;

[Serializable]
[NetSerializable]
public enum VendingMachineVisualState : byte
{
	Normal,
	Off,
	Broken,
	Eject,
	Deny
}
