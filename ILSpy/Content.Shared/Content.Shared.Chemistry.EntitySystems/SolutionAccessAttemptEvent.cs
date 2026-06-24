using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionAccessAttemptEvent(string SolutionName)
{
	public bool Cancelled = false;
}
