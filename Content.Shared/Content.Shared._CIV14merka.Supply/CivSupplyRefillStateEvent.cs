using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Supply;

[Serializable]
[NetSerializable]
public sealed class CivSupplyRefillStateEvent : EntityEventArgs
{
	public List<CivSupplyRefillEntry> Entries;

	public int RefillThreshold;

	public CivSupplyRefillStateEvent(List<CivSupplyRefillEntry> entries, int refillThreshold)
	{
		Entries = entries;
		RefillThreshold = refillThreshold;
	}
}
