using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public record struct ShiverAttemptEvent(EntityUid Uid)
{
	public readonly EntityUid Uid = Uid;

	public bool Cancelled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid Uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Uid = this.Uid;
	}
}
