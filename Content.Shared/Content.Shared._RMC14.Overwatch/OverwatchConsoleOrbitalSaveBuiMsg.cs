using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleOrbitalSaveBuiMsg(int longitude, int latitude) : BoundUserInterfaceMessage
{
	public readonly int Longitude = longitude;

	public readonly int Latitude = latitude;
}
