using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionContainerChangedEvent
{
	public readonly Solution Solution;

	public readonly string SolutionId;

	public SolutionContainerChangedEvent(Solution solution, string solutionId)
	{
		SolutionId = solutionId;
		Solution = solution;
	}
}
