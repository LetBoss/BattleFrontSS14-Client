using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics.Systems;

internal record struct RayCastInput
{
	public Vector2 Origin;

	public Vector2 Translation;

	public float MaxFraction;

	public bool IsValidRay()
	{
		if (Vector2Helpers.IsValid(Origin) && Vector2Helpers.IsValid(Translation) && MathHelper.IsValid(MaxFraction) && 0f <= MaxFraction)
		{
			return MaxFraction < float.MaxValue;
		}
		return false;
	}
}
