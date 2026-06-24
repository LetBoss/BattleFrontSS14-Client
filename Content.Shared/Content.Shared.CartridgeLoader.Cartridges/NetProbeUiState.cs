using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public sealed class NetProbeUiState : BoundUserInterfaceState
{
	public List<ProbedNetworkDevice> ProbedDevices;

	public NetProbeUiState(List<ProbedNetworkDevice> probedDevices)
	{
		ProbedDevices = probedDevices;
	}
}
