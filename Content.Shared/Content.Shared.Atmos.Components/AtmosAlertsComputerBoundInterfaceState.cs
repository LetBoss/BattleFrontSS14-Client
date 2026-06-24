using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class AtmosAlertsComputerBoundInterfaceState : BoundUserInterfaceState
{
	public AtmosAlertsComputerEntry[] AirAlarms;

	public AtmosAlertsComputerEntry[] FireAlarms;

	public AtmosAlertsFocusDeviceData? FocusData;

	public AtmosAlertsComputerBoundInterfaceState(AtmosAlertsComputerEntry[] airAlarms, AtmosAlertsComputerEntry[] fireAlarms, AtmosAlertsFocusDeviceData? focusData)
	{
		AirAlarms = airAlarms;
		FireAlarms = fireAlarms;
		FocusData = focusData;
	}
}
