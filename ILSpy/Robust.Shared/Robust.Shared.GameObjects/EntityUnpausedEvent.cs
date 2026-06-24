using System;
using System.Runtime.CompilerServices;

namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct EntityUnpausedEvent(TimeSpan PausedTime)
{
	public readonly TimeSpan PausedTime = PausedTime;

	[CompilerGenerated]
	public void Deconstruct(out TimeSpan PausedTime)
	{
		PausedTime = this.PausedTime;
	}
}
