using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

[Serializable]
[NetSerializable]
public sealed class NameTunnelMessage(string tunnelName) : BoundUserInterfaceMessage
{
	public string TunnelName = tunnelName;
}
