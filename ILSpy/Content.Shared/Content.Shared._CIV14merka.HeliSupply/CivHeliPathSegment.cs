using System.Numerics;

namespace Content.Shared._CIV14merka.HeliSupply;

public sealed class CivHeliPathSegment
{
	public Vector2 A;

	public Vector2 B;

	public float Length;

	public float SpeedFactor = 1f;

	public float Cost
	{
		get
		{
			if (!(SpeedFactor > 0f))
			{
				return Length;
			}
			return Length / SpeedFactor;
		}
	}
}
