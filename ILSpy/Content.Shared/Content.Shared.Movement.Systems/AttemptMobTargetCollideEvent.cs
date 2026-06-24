using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct AttemptMobTargetCollideEvent(EntityUid Entity)
{
	public bool Cancelled = false;
}
