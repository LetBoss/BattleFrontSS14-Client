using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[Serializable]
[NetSerializable]
public sealed class DropshipNavigationLaunchMsg(NetEntity target) : BoundUserInterfaceMessage
{
	public readonly NetEntity Target = target;
}
