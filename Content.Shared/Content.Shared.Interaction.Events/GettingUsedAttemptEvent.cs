using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class GettingUsedAttemptEvent(EntityUid user) : CancellableEntityEventArgs
{
	public EntityUid User = user;
}
