using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public enum PowerWireActionKey : byte
{
	Key,
	Status,
	Pulsed,
	Electrified,
	PulseCancel,
	ElectrifiedCancel,
	MainWire,
	WireCount,
	CutWires
}
