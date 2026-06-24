using Robust.Shared.GameObjects;

namespace Content.Shared.Buckle.Components;

[ByRefEvent]
public record struct BuckleAttemptEvent(Entity<StrapComponent> Strap, Entity<BuckleComponent> Buckle, EntityUid? User, bool Popup)
{
	public bool Cancelled = false;
}
