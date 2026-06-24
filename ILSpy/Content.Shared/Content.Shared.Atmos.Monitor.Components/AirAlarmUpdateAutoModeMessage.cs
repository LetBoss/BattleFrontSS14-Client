using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable]
[NetSerializable]
public sealed class AirAlarmUpdateAutoModeMessage : BoundUserInterfaceMessage
{
	public bool Enabled { get; }

	public AirAlarmUpdateAutoModeMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
