using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionContainerRelayEvent<TEvent>(TEvent Event, Entity<SolutionComponent> SolutionEnt, string Name)
{
	public readonly Entity<SolutionComponent> SolutionEnt;

	public readonly string Name;

	public TEvent Event;

	[CompilerGenerated]
	public readonly void Deconstruct(out TEvent Event, out Entity<SolutionComponent> SolutionEnt, out string Name)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Event = this.Event;
		SolutionEnt = this.SolutionEnt;
		Name = this.Name;
	}
}
