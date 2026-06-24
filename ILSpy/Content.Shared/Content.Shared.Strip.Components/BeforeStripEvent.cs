using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip.Components;

[ByRefEvent]
public sealed class BeforeStripEvent : BaseBeforeStripEvent
{
	public BeforeStripEvent(TimeSpan initialTime, bool stealth = false)
		: base(initialTime, stealth)
	{
	}
}
