using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionRelayEvent<TEvent>(TEvent Event, EntityUid ContainerEnt, string Name)
{
	public readonly EntityUid ContainerEnt;

	public readonly string Name;

	public TEvent Event;

	[CompilerGenerated]
	public readonly void Deconstruct(out TEvent Event, out EntityUid ContainerEnt, out string Name)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Event = this.Event;
		ContainerEnt = this.ContainerEnt;
		Name = this.Name;
	}
}
