using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech.EntitySystems;

[ByRefEvent]
public record struct MechPilotRelayedEvent<TEvent>(TEvent Args)
{
	public TEvent Args;

	[CompilerGenerated]
	public readonly void Deconstruct(out TEvent Args)
	{
		Args = this.Args;
	}
}
