using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class EmergencyConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
	public TimeSpan? EarlyLaunchTime;

	public List<string> Authorizations = new List<string>();

	public int AuthorizationsRequired;

	public TimeSpan? TimeToLaunch;
}
