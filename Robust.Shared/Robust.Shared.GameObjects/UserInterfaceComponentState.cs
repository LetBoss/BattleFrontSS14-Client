using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
internal sealed class UserInterfaceComponentState(Dictionary<Enum, List<NetEntity>> actors, Dictionary<Enum, BoundUserInterfaceState> states, Dictionary<Enum, InterfaceData> data) : IComponentState
{
	public Dictionary<Enum, List<NetEntity>> Actors = actors;

	public Dictionary<Enum, BoundUserInterfaceState> States = states;

	public Dictionary<Enum, InterfaceData> Data = data;
}
