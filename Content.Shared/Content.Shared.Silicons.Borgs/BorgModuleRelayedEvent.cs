using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Silicons.Borgs;

[ByRefEvent]
public record struct BorgModuleRelayedEvent<TEvent>(TEvent Args)
{
	public readonly TEvent Args;

	[CompilerGenerated]
	public readonly void Deconstruct(out TEvent Args)
	{
		Args = this.Args;
	}
}
