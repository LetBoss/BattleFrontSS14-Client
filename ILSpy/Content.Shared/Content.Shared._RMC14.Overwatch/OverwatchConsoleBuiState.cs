using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleBuiState(List<OverwatchSquad> squads, Dictionary<NetEntity, List<OverwatchMarine>> marines) : BoundUserInterfaceState
{
	public readonly List<OverwatchSquad> Squads = squads;

	public readonly Dictionary<NetEntity, List<OverwatchMarine>> Marines = marines;
}
