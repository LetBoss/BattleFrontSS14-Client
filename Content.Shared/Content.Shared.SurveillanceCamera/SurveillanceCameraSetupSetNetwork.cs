using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera;

[Serializable]
[NetSerializable]
public sealed class SurveillanceCameraSetupSetNetwork : BoundUserInterfaceMessage
{
	public int Network { get; }

	public SurveillanceCameraSetupSetNetwork(int network)
	{
		Network = network;
	}
}
