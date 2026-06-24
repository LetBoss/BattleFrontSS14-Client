using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLinesSnapshotEvent : EntityEventArgs
{
	public List<CivCommanderLineState> Lines { get; }

	public CivCommanderLinesSnapshotEvent(IEnumerable<CivCommanderLineState> lines)
	{
		Lines = lines.ToList();
	}
}
