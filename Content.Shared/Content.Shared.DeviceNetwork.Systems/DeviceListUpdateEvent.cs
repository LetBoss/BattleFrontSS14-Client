using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Systems;

public sealed class DeviceListUpdateEvent : EntityEventArgs
{
	public List<EntityUid> OldDevices { get; }

	public List<EntityUid> Devices { get; }

	public DeviceListUpdateEvent(List<EntityUid> oldDevices, List<EntityUid> devices)
	{
		OldDevices = oldDevices;
		Devices = devices;
	}
}
