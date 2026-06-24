using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
internal sealed class UserInterfaceActorsDeltaState : IComponentDeltaState<UserInterfaceComponentState>, IComponentDeltaState, IComponentState
{
	public Dictionary<Enum, List<NetEntity>> Actors = new Dictionary<Enum, List<NetEntity>>();

	public void ApplyToFullState(UserInterfaceComponentState fullState)
	{
		fullState.Actors.Clear();
		foreach (var (key, value) in Actors)
		{
			fullState.Actors.Add(key, value);
		}
	}

	public UserInterfaceComponentState CreateNewFullState(UserInterfaceComponentState fullState)
	{
		return new UserInterfaceComponentState(new Dictionary<Enum, List<NetEntity>>(Actors), new Dictionary<Enum, BoundUserInterfaceState>(fullState.States), new Dictionary<Enum, InterfaceData>(fullState.Data));
	}
}
