using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Teams;

[Serializable]
[NetSerializable]
public enum CivSquadType : byte
{
	Rifle,
	Engineer,
	Medic,
	Support
}
