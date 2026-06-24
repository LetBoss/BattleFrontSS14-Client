using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public readonly record struct VehicleSlotPath(string Root, string? Child = null)
{
	public bool IsValid => !string.IsNullOrWhiteSpace(Root);

	public bool IsNested => !string.IsNullOrWhiteSpace(Child);

	public string ToCompositeId()
	{
		if (!IsNested)
		{
			return Root;
		}
		return VehicleTurretSlotIds.Compose(Root, Child);
	}

	public VehicleSlotPath Append(string child)
	{
		if (!IsNested)
		{
			return new VehicleSlotPath(Root, child);
		}
		return new VehicleSlotPath(Root, VehicleTurretSlotIds.Compose(Child, child));
	}

	public static bool TryParse(string? value, out VehicleSlotPath path)
	{
		path = default(VehicleSlotPath);
		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}
		if (VehicleTurretSlotIds.TryParse(value, out string parent, out string child))
		{
			path = new VehicleSlotPath(parent, child);
			return true;
		}
		path = new VehicleSlotPath(value);
		return true;
	}
}
