using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Teams;

[Serializable]
[NetSerializable]
public enum CivTeamSlot : byte
{
	Team1 = 1,
	Team2,
	Team3
}
