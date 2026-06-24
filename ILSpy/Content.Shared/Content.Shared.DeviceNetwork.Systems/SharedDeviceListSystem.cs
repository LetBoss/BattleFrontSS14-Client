using System.Collections.Generic;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedDeviceListSystem : EntitySystem
{
	public IEnumerable<EntityUid> GetAllDevices(EntityUid uid, DeviceListComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceListComponent>(uid, ref component, true))
		{
			return (IEnumerable<EntityUid>)(object)new EntityUid[0];
		}
		return component.Devices;
	}
}
