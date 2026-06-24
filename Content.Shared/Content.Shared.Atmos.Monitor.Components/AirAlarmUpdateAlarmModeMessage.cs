using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public sealed class AirAlarmUpdateAlarmModeMessage : BoundUserInterfaceMessage
{
	public AirAlarmMode Mode { get; }

	public AirAlarmUpdateAlarmModeMessage(AirAlarmMode mode)
	{
		Mode = mode;
	}
}
