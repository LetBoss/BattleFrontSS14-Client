using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Robotics;

[Serializable]
[NetSerializable]
public sealed class RoboticsConsoleState : BoundUserInterfaceState
{
	public Dictionary<string, CyborgControlData> Cyborgs;

	public RoboticsConsoleState(Dictionary<string, CyborgControlData> cyborgs)
	{
		Cyborgs = cyborgs;
	}
}
