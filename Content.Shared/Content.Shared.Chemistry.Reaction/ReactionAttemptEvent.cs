using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction;

[ByRefEvent]
public record struct ReactionAttemptEvent(ReactionPrototype Reaction, Entity<SolutionComponent> Solution)
{
	public readonly ReactionPrototype Reaction = Reaction;

	public readonly Entity<SolutionComponent> Solution = Solution;

	public bool Cancelled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out ReactionPrototype Reaction, out Entity<SolutionComponent> Solution)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Reaction = this.Reaction;
		Solution = this.Solution;
	}
}
