using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos;

[Serializable]
[NetSerializable]
public enum Gas : sbyte
{
	Oxygen,
	Nitrogen,
	CarbonDioxide,
	Plasma,
	Tritium,
	WaterVapor,
	Ammonia,
	NitrousOxide,
	Frezon
}
