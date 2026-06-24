using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition;

public sealed class BeforeFullySlicedEvent : CancellableEntityEventArgs
{
	public EntityUid User;
}
