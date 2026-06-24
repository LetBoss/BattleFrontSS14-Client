using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class SwitchChargingMachineMessage : BoundUserInterfaceMessage
{
	public bool On;

	public SwitchChargingMachineMessage(bool on)
	{
		On = on;
	}
}
