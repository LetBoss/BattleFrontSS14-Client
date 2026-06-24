using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class NavBoundUserInterfaceState : BoundUserInterfaceState
{
	public NavInterfaceState State;

	public NavBoundUserInterfaceState(NavInterfaceState state)
	{
		State = state;
	}
}
