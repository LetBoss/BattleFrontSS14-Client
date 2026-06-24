using System;
using System.Collections.Generic;
using Content.Shared.Doors.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[Serializable]
[NetSerializable]
public sealed class DropshipNavigationTravellingBuiState(FTLState state, StartEndTime time, string destination, string departureLocation, Dictionary<DoorLocation, bool> doorLockStatus) : BoundUserInterfaceState
{
	public readonly FTLState State = state;

	public readonly StartEndTime Time = time;

	public readonly string Destination = destination;

	public readonly string DepartureLocation = departureLocation;

	public readonly Dictionary<DoorLocation, bool> DoorLockStatus = doorLockStatus;
}
