using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class PowerMonitoringConsoleBoundInterfaceState : BoundUserInterfaceState
{
	public double TotalSources;

	public double TotalBatteryUsage;

	public double TotalLoads;

	public PowerMonitoringConsoleEntry[] AllEntries;

	public PowerMonitoringConsoleEntry[] FocusSources;

	public PowerMonitoringConsoleEntry[] FocusLoads;

	public PowerMonitoringConsoleBoundInterfaceState(double totalSources, double totalBatteryUsage, double totalLoads, PowerMonitoringConsoleEntry[] allEntries, PowerMonitoringConsoleEntry[] focusSources, PowerMonitoringConsoleEntry[] focusLoads)
	{
		TotalSources = totalSources;
		TotalBatteryUsage = totalBatteryUsage;
		TotalLoads = totalLoads;
		AllEntries = allEntries;
		FocusSources = focusSources;
		FocusLoads = focusLoads;
	}
}
