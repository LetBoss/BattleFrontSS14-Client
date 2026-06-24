using Robust.Shared.GameObjects;

namespace Robust.Shared.Player;

public sealed class PlayerDetachedEvent : EntityEventArgs
{
	public readonly EntityUid Entity;

	public readonly ICommonSession Player;

	public PlayerDetachedEvent(EntityUid entity, ICommonSession player)
	{
		Entity = entity;
		Player = player;
	}
}
