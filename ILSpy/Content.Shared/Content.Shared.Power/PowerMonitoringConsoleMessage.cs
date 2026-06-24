using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class PowerMonitoringConsoleMessage : BoundUserInterfaceMessage
{
	public NetEntity? FocusDevice;

	public PowerMonitoringConsoleGroup FocusGroup;

	public PowerMonitoringConsoleMessage(NetEntity? focusDevice, PowerMonitoringConsoleGroup focusGroup)
	{
		FocusDevice = focusDevice;
		FocusGroup = focusGroup;
	}
}
