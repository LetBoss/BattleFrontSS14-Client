using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Light.EntitySystems;

[ByRefEvent]
public record struct LightCycleOffsetEvent(TimeSpan Offset)
{
	public readonly TimeSpan Offset = Offset;

	[CompilerGenerated]
	public readonly void Deconstruct(out TimeSpan Offset)
	{
		Offset = this.Offset;
	}
}
