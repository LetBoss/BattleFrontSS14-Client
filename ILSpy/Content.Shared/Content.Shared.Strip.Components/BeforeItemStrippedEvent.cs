using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip.Components;

[ByRefEvent]
public sealed class BeforeItemStrippedEvent : BaseBeforeStripEvent
{
	public BeforeItemStrippedEvent(TimeSpan initialTime, bool stealth = false)
		: base(initialTime, stealth)
	{
	}
}
