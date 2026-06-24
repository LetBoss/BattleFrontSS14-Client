using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[NetSerializable]
public sealed class VehicleSupplyEntryState
{
	public string Id;

	public string Name;

	public int Count;

	public VehicleSupplyEntryState(string id, string name, int count)
	{
		Id = id;
		Name = name;
		Count = count;
	}
}
