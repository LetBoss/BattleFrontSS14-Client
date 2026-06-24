using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public sealed class BatterySetChargeRateMessage(float rate) : BoundUserInterfaceMessage
{
	public float Rate = rate;
}
