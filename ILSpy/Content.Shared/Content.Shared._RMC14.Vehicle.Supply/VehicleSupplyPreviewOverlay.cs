using System;
using System.Numerics;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleSupplyPreviewOverlay
{
	public string Rsi;

	public string State;

	public int Order;

	public Vector2 BaseOffset;

	public bool UseDirectional;

	public Vector2 North;

	public Vector2 East;

	public Vector2 South;

	public Vector2 West;

	public VehicleSupplyPreviewOverlay(string rsi, string state, int order, Vector2 baseOffset, bool useDirectional, Vector2 north, Vector2 east, Vector2 south, Vector2 west)
	{
		Rsi = rsi;
		State = state;
		Order = order;
		BaseOffset = baseOffset;
		UseDirectional = useDirectional;
		North = north;
		East = east;
		South = south;
		West = west;
	}
}
