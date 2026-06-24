using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Teams;

[Serializable]
[NetSerializable]
public enum CivTdmClass : byte
{
	Rifleman,
	MachineGunner,
	Specialist,
	Medic,
	SquadLeader,
	Engineer,
	EngineerLeader,
	MedicLeader,
	Logist,
	Scout
}
