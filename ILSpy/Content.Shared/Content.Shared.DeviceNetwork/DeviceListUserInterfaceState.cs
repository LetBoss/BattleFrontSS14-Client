using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public sealed class DeviceListUserInterfaceState : BoundUserInterfaceState
{
	public readonly HashSet<(string address, string name)> DeviceList;

	public DeviceListUserInterfaceState(HashSet<(string address, string name)> deviceList)
	{
		DeviceList = deviceList;
	}
}
