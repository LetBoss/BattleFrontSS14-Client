using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.UniformAccessories;

[Serializable]
[NetSerializable]
public sealed class UniformAccessoriesBuiMsg(NetEntity toRemove) : BoundUserInterfaceMessage
{
	public readonly NetEntity ToRemove = toRemove;
}
