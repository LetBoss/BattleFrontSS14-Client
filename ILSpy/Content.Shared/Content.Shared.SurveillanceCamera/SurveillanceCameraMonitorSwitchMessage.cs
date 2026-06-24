using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera;

[Serializable]
[NetSerializable]
public sealed class SurveillanceCameraMonitorSwitchMessage : BoundUserInterfaceMessage
{
	public string Address { get; }

	public SurveillanceCameraMonitorSwitchMessage(string address)
	{
		Address = address;
	}
}
