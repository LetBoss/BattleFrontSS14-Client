using Robust.Shared.GameObjects;

namespace Content.Shared.Doors;

public sealed class BeforeDoorOpenedEvent : CancellableEntityEventArgs
{
	public EntityUid? User;
}
