using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public sealed class NetworkConfiguratorRemoveDeviceMessage : BoundUserInterfaceMessage
{
	public readonly string Address;

	public NetworkConfiguratorRemoveDeviceMessage(string address)
	{
		Address = address;
	}
}
