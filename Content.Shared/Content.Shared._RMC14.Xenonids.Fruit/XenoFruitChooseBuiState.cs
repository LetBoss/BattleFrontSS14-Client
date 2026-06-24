using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Fruit;

[Serializable]
[NetSerializable]
public sealed class XenoFruitChooseBuiState(int count, int max) : BoundUserInterfaceState
{
	public readonly int Count = count;

	public readonly int Max = max;
}
