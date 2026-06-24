using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Systems;

public abstract class SharedDeviceNetworkSystem : EntitySystem
{
	public virtual bool QueuePacket(EntityUid uid, string? address, NetworkPayload data, uint? frequency = null, int? network = null, DeviceNetworkComponent? device = null)
	{
		return false;
	}
}
