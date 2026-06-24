using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class BatterySetOutputBreakerMessage(bool on) : BoundUserInterfaceMessage
{
	public bool On = on;
}
