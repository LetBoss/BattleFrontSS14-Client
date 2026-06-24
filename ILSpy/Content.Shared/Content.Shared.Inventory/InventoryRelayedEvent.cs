using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory;

public sealed class InventoryRelayedEvent<TEvent> : EntityEventArgs
{
	public TEvent Args;

	public InventoryRelayedEvent(TEvent args)
	{
		Args = args;
	}
}
