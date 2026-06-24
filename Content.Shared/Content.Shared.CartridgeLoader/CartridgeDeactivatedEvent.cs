using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader;

public sealed class CartridgeDeactivatedEvent : EntityEventArgs
{
	public readonly EntityUid Loader;

	public CartridgeDeactivatedEvent(EntityUid loader)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Loader = loader;
	}
}
