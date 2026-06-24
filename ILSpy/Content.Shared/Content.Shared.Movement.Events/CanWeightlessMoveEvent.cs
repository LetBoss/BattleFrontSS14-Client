using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public record struct CanWeightlessMoveEvent(EntityUid Uid)
{
	public bool CanMove = false;
}
