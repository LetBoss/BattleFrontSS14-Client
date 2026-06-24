using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public enum OverwatchLocation
{
	Min = 0,
	Planet = 0,
	Ship = 1,
	Max = 1
}
