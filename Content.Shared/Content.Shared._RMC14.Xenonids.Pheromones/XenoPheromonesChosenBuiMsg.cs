using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Pheromones;

[Serializable]
[NetSerializable]
public sealed class XenoPheromonesChosenBuiMsg(XenoPheromones pheromones) : BoundUserInterfaceMessage
{
	public readonly XenoPheromones Pheromones = pheromones;
}
