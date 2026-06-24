using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.Announce;

[Serializable]
[NetSerializable]
public sealed class MarineCommunicationsComputerBuiState(string planet, string operation, List<LandingZone> landingZones) : BoundUserInterfaceState
{
	public readonly string Planet = planet;

	public readonly string Operation = operation;

	public readonly List<LandingZone> LandingZones = landingZones;
}
