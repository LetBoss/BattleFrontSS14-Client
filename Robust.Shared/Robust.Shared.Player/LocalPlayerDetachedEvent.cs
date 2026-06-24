using Robust.Shared.GameObjects;

namespace Robust.Shared.Player;

public sealed class LocalPlayerDetachedEvent : EntityEventArgs
{
	public readonly EntityUid Entity;

	public LocalPlayerDetachedEvent(EntityUid entity)
	{
		Entity = entity;
	}
}
