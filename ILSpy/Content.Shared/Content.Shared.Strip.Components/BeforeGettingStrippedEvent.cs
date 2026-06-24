using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip.Components;

[ByRefEvent]
public sealed class BeforeGettingStrippedEvent : BaseBeforeStripEvent
{
	public BeforeGettingStrippedEvent(TimeSpan initialTime, bool stealth = false)
		: base(initialTime, stealth)
	{
	}
}
