using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

[ByRefEvent]
public sealed class HeldRelayedEvent<TEvent> : EntityEventArgs
{
	public TEvent Args;

	public HeldRelayedEvent(TEvent args)
	{
		Args = args;
	}
}
