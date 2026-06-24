using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Robotics;

[Serializable]
[NetSerializable]
public sealed class RoboticsConsoleDestroyMessage : BoundUserInterfaceMessage
{
	public readonly string Address;

	public RoboticsConsoleDestroyMessage(string address)
	{
		Address = address;
	}
}
