using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction;

public interface ITargetedInteractEventArgs
{
	EntityUid User { get; }

	EntityUid Target { get; }
}
