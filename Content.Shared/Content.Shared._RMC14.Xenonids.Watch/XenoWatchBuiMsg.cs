using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Watch;

[Serializable]
[NetSerializable]
public sealed class XenoWatchBuiMsg(NetEntity target) : BoundUserInterfaceMessage
{
	public readonly NetEntity Target = target;
}
