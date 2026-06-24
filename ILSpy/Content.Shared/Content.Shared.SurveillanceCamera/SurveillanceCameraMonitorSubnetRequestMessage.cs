using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera;

[Serializable]
[NetSerializable]
public sealed class SurveillanceCameraMonitorSubnetRequestMessage : BoundUserInterfaceMessage
{
	public string Subnet { get; }

	public SurveillanceCameraMonitorSubnetRequestMessage(string subnet)
	{
		Subnet = subnet;
	}
}
