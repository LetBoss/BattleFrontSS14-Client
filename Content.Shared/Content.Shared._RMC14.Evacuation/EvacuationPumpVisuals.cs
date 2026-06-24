using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Evacuation;

[Serializable]
[NetSerializable]
public enum EvacuationPumpVisuals
{
	Empty,
	TwentyFive,
	Fifty,
	SeventyFive,
	Full
}
