using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[Serializable]
[NetSerializable]
public sealed class TraverseXenoTunnelMessage(NetEntity destinationTunnel) : BoundUserInterfaceMessage
{
	public NetEntity DestinationTunnel = destinationTunnel;
}
