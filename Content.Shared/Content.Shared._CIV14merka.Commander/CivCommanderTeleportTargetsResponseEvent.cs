using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderTeleportTargetsResponseEvent : EntityEventArgs
{
	public List<CivCommanderTeleportTargetState> Targets { get; }

	public CivCommanderTeleportTargetsResponseEvent(IEnumerable<CivCommanderTeleportTargetState> targets)
	{
		Targets = targets.ToList();
	}
}
