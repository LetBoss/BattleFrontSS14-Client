using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera;

[Serializable]
[NetSerializable]
public sealed class SurveillanceCameraMonitorUiState : BoundUserInterfaceState
{
	public string ActiveAddress;

	public NetEntity? ActiveCamera { get; }

	public HashSet<string> Subnets { get; }

	public string ActiveSubnet { get; }

	public Dictionary<string, string> Cameras { get; }

	public SurveillanceCameraMonitorUiState(NetEntity? activeCamera, HashSet<string> subnets, string activeAddress, string activeSubnet, Dictionary<string, string> cameras)
	{
		ActiveCamera = activeCamera;
		Subnets = subnets;
		ActiveAddress = activeAddress;
		ActiveSubnet = activeSubnet;
		Cameras = cameras;
	}
}
