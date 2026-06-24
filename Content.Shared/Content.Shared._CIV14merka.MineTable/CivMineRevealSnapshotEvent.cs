using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.MineTable;

[Serializable]
[NetSerializable]
public sealed class CivMineRevealSnapshotEvent : EntityEventArgs
{
	public readonly List<CivMineRevealEntry> Entries;

	public CivMineRevealSnapshotEvent(List<CivMineRevealEntry> entries)
	{
		Entries = entries;
	}
}
