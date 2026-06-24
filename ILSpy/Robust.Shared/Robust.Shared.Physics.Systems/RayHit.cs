using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Physics.Systems;

public record struct RayHit(EntityUid Entity, Vector2 LocalNormal, float Fraction)
{
	public readonly EntityUid Entity = Entity;

	public readonly Vector2 LocalNormal = LocalNormal;

	public readonly float Fraction = Fraction;

	public Vector2 Point = default(Vector2);

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid Entity, out Vector2 LocalNormal, out float Fraction)
	{
		Entity = this.Entity;
		LocalNormal = this.LocalNormal;
		Fraction = this.Fraction;
	}
}
