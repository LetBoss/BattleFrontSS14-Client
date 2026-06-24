using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
internal sealed class UserInterfaceStatesDeltaState : IComponentDeltaState<UserInterfaceComponentState>, IComponentDeltaState, IComponentState
{
	public Dictionary<Enum, BoundUserInterfaceState> States = new Dictionary<Enum, BoundUserInterfaceState>();

	public void ApplyToFullState(UserInterfaceComponentState fullState)
	{
		fullState.States.Clear();
		foreach (var (key, value) in States)
		{
			fullState.States.Add(key, value);
		}
	}

	public UserInterfaceComponentState CreateNewFullState(UserInterfaceComponentState fullState)
	{
		return new UserInterfaceComponentState(new Dictionary<Enum, List<NetEntity>>(fullState.Actors), new Dictionary<Enum, BoundUserInterfaceState>(States), new Dictionary<Enum, InterfaceData>(fullState.Data));
	}
}
