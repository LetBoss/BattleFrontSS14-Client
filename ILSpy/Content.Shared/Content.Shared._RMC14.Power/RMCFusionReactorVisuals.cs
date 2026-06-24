using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Power;

[Serializable]
[NetSerializable]
public enum RMCFusionReactorVisuals
{
	Off,
	Weld,
	Wire,
	Wrench,
	Ten,
	TwentyFive,
	Fifty,
	SeventyFive,
	Hundred,
	Overloaded,
	Empty
}
