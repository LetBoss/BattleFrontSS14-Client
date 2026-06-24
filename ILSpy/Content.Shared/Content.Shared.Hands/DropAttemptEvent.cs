using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public sealed class DropAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid Uid;
}
