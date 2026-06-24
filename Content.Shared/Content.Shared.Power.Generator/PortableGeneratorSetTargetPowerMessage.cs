using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power.Generator;

[Serializable]
[NetSerializable]
public sealed class PortableGeneratorSetTargetPowerMessage : BoundUserInterfaceMessage
{
	public int TargetPower;

	public PortableGeneratorSetTargetPowerMessage(int targetPower)
	{
		TargetPower = targetPower;
	}
}
