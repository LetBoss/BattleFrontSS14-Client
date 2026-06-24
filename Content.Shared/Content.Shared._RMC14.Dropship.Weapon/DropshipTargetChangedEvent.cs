using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropshipTargetChangedEvent : EntityEventArgs
{
	public NetEntity? DropshipTarget;

	public DropshipTargetChangedEvent(NetEntity? dropshipTarget)
	{
		DropshipTarget = dropshipTarget;
	}
}
