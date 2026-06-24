using System.Numerics;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Physics;

public readonly struct RayCastResults
{
	public EntityUid HitEntity { get; }

	public Vector2 HitPos { get; }

	public float Distance { get; }

	public RayCastResults(float distance, Vector2 hitPos, EntityUid hitEntity)
	{
		Distance = distance;
		HitPos = hitPos;
		HitEntity = hitEntity;
	}
}
