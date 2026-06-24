using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Robotics;

[Serializable]
[NetSerializable]
public sealed class RoboticsConsoleDisableMessage : BoundUserInterfaceMessage
{
	public readonly string Address;

	public RoboticsConsoleDisableMessage(string address)
	{
		Address = address;
	}
}
