using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Teleportation;

[Serializable]
[NetSerializable]
public sealed class TeleportLocationDestinationMessage(NetEntity netEnt, string pointName) : BoundUserInterfaceMessage
{
	public NetEntity NetEnt = netEnt;

	public string PointName = pointName;
}
