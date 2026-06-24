using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionOverflowEvent(Entity<SolutionComponent> Solution, FixedPoint2 Overflow)
{
	public readonly Entity<SolutionComponent> Solution = Solution;

	public readonly FixedPoint2 Overflow = Overflow;

	public bool Handled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out Entity<SolutionComponent> Solution, out FixedPoint2 Overflow)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Solution = this.Solution;
		Overflow = this.Overflow;
	}
}
