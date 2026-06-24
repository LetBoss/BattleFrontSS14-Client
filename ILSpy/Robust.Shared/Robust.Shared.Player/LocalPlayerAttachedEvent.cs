using Robust.Shared.GameObjects;

namespace Robust.Shared.Player;

public sealed class LocalPlayerAttachedEvent : EntityEventArgs
{
	public readonly EntityUid Entity;

	public LocalPlayerAttachedEvent(EntityUid entity)
	{
		Entity = entity;
	}
}
