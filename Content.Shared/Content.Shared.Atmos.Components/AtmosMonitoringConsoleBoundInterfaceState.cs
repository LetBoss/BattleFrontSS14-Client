using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class AtmosMonitoringConsoleBoundInterfaceState : BoundUserInterfaceState
{
	public AtmosMonitoringConsoleEntry[] AtmosNetworks;

	public AtmosMonitoringConsoleBoundInterfaceState(AtmosMonitoringConsoleEntry[] atmosNetworks)
	{
		AtmosNetworks = atmosNetworks;
	}
}
