using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[Serializable]
[NetSerializable]
public sealed class SelectDestinationTunnelInterfaceState(Dictionary<string, NetEntity> hiveTunnels) : BoundUserInterfaceState
{
	public Dictionary<string, NetEntity> HiveTunnels = hiveTunnels;
}
