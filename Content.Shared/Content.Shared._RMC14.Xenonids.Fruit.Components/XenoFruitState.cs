using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Fruit.Components;

[Serializable]
[NetSerializable]
public enum XenoFruitState
{
	Item,
	Growing,
	Grown,
	Eaten
}
