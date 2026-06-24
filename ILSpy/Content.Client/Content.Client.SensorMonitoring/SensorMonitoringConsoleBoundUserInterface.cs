using System;
using Content.Client.Computer;
using Content.Shared.SensorMonitoring;
using Robust.Shared.GameObjects;

namespace Content.Client.SensorMonitoring;

public sealed class SensorMonitoringConsoleBoundUserInterface : ComputerBoundUserInterface<SensorMonitoringWindow, SensorMonitoringConsoleBoundInterfaceState>
{
	public SensorMonitoringConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)

}
