using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionTransferAttemptEvent(EntityUid From, Entity<SolutionComponent> FromSolution, EntityUid To, Entity<SolutionComponent> ToSolution, string? CancelReason = null)
{
	public void Cancel(string reason)
	{
		CancelReason = reason;
	}
}
