using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition.EntitySystems;

[ByRefEvent]
public record struct AttemptAddFizzinessEvent(Entity<PressurizedSolutionComponent> Entity, float Amount)
{
	public bool Cancelled = false;
}
