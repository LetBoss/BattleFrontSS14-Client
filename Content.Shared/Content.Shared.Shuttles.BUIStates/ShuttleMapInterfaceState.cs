using System;
using System.Collections.Generic;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Shuttles.UI.MapObjects;
using Content.Shared.Timing;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class ShuttleMapInterfaceState
{
	public readonly FTLState FTLState;

	public StartEndTime FTLTime;

	public List<ShuttleBeaconObject> Destinations;

	public List<ShuttleExclusionObject> Exclusions;

	public ShuttleMapInterfaceState(FTLState ftlState, StartEndTime ftlTime, List<ShuttleBeaconObject> destinations, List<ShuttleExclusionObject> exclusions)
	{
		FTLState = ftlState;
		FTLTime = ftlTime;
		Destinations = destinations;
		Exclusions = exclusions;
	}
}
