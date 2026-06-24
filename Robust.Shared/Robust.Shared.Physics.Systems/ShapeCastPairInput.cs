using System.Numerics;
using Robust.Shared.Physics.Collision;

namespace Robust.Shared.Physics.Systems;

internal ref struct ShapeCastPairInput
{
	public DistanceProxy ProxyA;

	public DistanceProxy ProxyB;

	public Transform TransformA;

	public Transform TransformB;

	public Vector2 TranslationB;

	public float MaxFraction;
}
