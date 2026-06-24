using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class DockingInterfaceState
{
	public Dictionary<NetEntity, List<DockingPortState>> Docks;

	public DockingInterfaceState(Dictionary<NetEntity, List<DockingPortState>> docks)
	{
		Docks = docks;
	}
}
