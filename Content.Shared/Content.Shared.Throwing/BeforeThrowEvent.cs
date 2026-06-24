using System.Numerics;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

[ByRefEvent]
public struct BeforeThrowEvent(EntityUid itemUid, Vector2 direction, float throwSpeed, EntityUid playerUid)
{
	public bool Cancelled = false;

	public EntityUid ItemUid { get; set; } = itemUid;

	public Vector2 Direction { get; } = direction;

	public float ThrowSpeed { get; set; } = throwSpeed;

	public EntityUid PlayerUid { get; } = playerUid;
}
