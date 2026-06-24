using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Rangefinder;

[Serializable]
[NetSerializable]
public enum RangefinderMode
{
	Rangefinder,
	Designator,
	Spotter
}
