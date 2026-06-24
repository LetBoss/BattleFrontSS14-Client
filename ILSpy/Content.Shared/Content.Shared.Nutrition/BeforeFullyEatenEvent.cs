using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition;

public sealed class BeforeFullyEatenEvent : CancellableEntityEventArgs
{
	public EntityUid User;
}
