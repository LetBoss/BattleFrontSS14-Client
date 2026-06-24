using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleOrbitalLongitudeBuiMsg(int longitude) : BoundUserInterfaceMessage
{
	public readonly int Longitude = longitude;
}
