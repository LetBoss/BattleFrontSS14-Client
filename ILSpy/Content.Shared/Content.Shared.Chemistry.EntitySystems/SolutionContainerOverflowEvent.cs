using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionContainerOverflowEvent(EntityUid SolutionEnt, Solution SolutionHolder, Solution Overflow)
{
	public readonly EntityUid SolutionEnt = SolutionEnt;

	public readonly Solution SolutionHolder = SolutionHolder;

	public readonly Solution Overflow = Overflow;

	public readonly FixedPoint2 OverflowVol = Overflow.Volume;

	public bool Handled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid SolutionEnt, out Solution SolutionHolder, out Solution Overflow)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SolutionEnt = this.SolutionEnt;
		SolutionHolder = this.SolutionHolder;
		Overflow = this.Overflow;
	}
}
