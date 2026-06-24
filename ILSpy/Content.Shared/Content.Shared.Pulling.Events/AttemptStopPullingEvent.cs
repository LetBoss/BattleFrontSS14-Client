using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Pulling.Events;

public record struct AttemptStopPullingEvent(EntityUid? User = null)
{
	public readonly EntityUid? User = User;

	public bool Cancelled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid? User)
	{
		User = this.User;
	}
}
