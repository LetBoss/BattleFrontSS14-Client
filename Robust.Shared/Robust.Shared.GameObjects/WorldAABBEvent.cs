using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

[ByRefEvent]
public record struct WorldAABBEvent
{
	public Box2 AABB;
}
