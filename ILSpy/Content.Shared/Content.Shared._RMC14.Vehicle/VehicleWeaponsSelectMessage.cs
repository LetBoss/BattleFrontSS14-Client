using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleWeaponsSelectMessage : BoundUserInterfaceMessage
{
	public readonly NetEntity? MountedEntity;

	public VehicleWeaponsSelectMessage(NetEntity? mountedEntity)
	{
		MountedEntity = mountedEntity;
	}
}
