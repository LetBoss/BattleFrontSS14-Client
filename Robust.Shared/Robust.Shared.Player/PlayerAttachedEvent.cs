using Robust.Shared.GameObjects;

namespace Robust.Shared.Player;

public sealed class PlayerAttachedEvent : EntityEventArgs
{
	public readonly EntityUid Entity;

	public readonly ICommonSession Player;

	public PlayerAttachedEvent(EntityUid entity, ICommonSession player)
	{
		Entity = entity;
		Player = player;
	}
}
