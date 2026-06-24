using Robust.Shared.GameObjects;

namespace Content.Shared.Fluids;

[ByRefEvent]
public record struct SprayAttemptEvent(EntityUid User, bool Cancelled = false)
{
	public void Cancel()
	{
		Cancelled = true;
	}
}
