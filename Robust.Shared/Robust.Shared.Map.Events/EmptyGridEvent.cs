using Robust.Shared.GameObjects;

namespace Robust.Shared.Map.Events;

public sealed class EmptyGridEvent : EntityEventArgs
{
	public EntityUid GridId { get; init; }
}
