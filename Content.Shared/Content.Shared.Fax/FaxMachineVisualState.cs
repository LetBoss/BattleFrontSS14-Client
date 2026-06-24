using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public enum FaxMachineVisualState : byte
{
	Normal,
	Inserting,
	Printing
}
