using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Specialist;

[Serializable]
[NetSerializable]
public sealed class CivSpecialistBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly Dictionary<int, CivSpecialistSetInfo> Sets;

	public int MaxSelectedSets;

	public CivSpecialistBoundUserInterfaceState(Dictionary<int, CivSpecialistSetInfo> sets, int max)
	{
		Sets = sets;
		MaxSelectedSets = max;
	}
}
