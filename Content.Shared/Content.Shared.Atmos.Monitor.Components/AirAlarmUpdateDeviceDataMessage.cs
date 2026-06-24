using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public sealed class AirAlarmUpdateDeviceDataMessage : BoundUserInterfaceMessage
{
	public string Address { get; }

	public IAtmosDeviceData Data { get; }

	public AirAlarmUpdateDeviceDataMessage(string addr, IAtmosDeviceData data)
	{
		Address = addr;
		Data = data;
	}
}
