using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class NavInterfaceState
{
	public float MaxRange;

	public NetCoordinates? Coordinates;

	public Angle? Angle;

	public Dictionary<NetEntity, List<DockingPortState>> Docks;

	public bool RotateWithEntity = true;

	public NavInterfaceState(float maxRange, NetCoordinates? coordinates, Angle? angle, Dictionary<NetEntity, List<DockingPortState>> docks)
	{
		MaxRange = maxRange;
		Coordinates = coordinates;
		Angle = angle;
		Docks = docks;
	}
}
