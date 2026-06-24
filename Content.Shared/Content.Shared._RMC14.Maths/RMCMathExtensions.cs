using System;

namespace Content.Shared._RMC14.Maths;

public static class RMCMathExtensions
{
	public static float CircleAreaFromSquareSide(float squareSide)
	{
		return (float)((double)squareSide / Math.Sqrt(Math.PI));
	}

	public static float CircleAreaFromSquareAbilityRange(float squareRadius)
	{
		return (float)((double)(squareRadius * 2f + 1f) / Math.Sqrt(Math.PI));
	}
}
