using System;
using System.Collections.Generic;
using Content.Shared.Doors.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[Serializable]
[NetSerializable]
public sealed class DropshipNavigationDestinationsBuiState(NetEntity? flyBy, List<Destination> destinations, Dictionary<DoorLocation, bool> doorLockStatus) : BoundUserInterfaceState
{
	public readonly NetEntity? FlyBy = flyBy;

	public readonly List<Destination> Destinations = destinations;

	public readonly Dictionary<DoorLocation, bool> DoorLockStatus = doorLockStatus;
}
