using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public sealed class AirAlarmCopyDeviceDataMessage : BoundUserInterfaceMessage
{
	public IAtmosDeviceData Data { get; }

	public AirAlarmCopyDeviceDataMessage(IAtmosDeviceData data)
	{
		Data = data;
	}
}
