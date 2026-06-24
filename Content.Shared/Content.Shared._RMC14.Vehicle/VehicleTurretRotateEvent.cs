using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleTurretRotateEvent : EntityEventArgs
{
	public NetEntity Turret;

	public NetCoordinates Coordinates;
}
