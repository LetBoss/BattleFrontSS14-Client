using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition;

public sealed class IngestionAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid? Blocker;
}
