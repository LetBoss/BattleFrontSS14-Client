using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Thief;

[Serializable]
[NetSerializable]
public sealed class ThiefBackpackBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly Dictionary<int, ThiefBackpackSetInfo> Sets;

	public int MaxSelectedSets;

	public ThiefBackpackBoundUserInterfaceState(Dictionary<int, ThiefBackpackSetInfo> sets, int max)
	{
		Sets = sets;
		MaxSelectedSets = max;
	}
}
