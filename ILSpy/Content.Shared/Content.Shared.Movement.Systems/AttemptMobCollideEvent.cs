using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct AttemptMobCollideEvent
{
	public bool Cancelled;
}
