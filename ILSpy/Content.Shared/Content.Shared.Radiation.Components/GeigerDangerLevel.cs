using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Components;

[Serializable]
[NetSerializable]
public enum GeigerDangerLevel : byte
{
	None,
	Low,
	Med,
	High,
	Extreme
}
