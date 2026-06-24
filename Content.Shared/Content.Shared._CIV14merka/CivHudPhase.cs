using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public enum CivHudPhase : byte
{
	None,
	Briefing,
	InRound
}
