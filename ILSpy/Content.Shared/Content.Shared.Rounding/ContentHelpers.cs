using System;

namespace Content.Shared.Rounding;

public static class ContentHelpers
{
	public static int RoundToLevels(double actual, double max, int levels)
	{
		if (levels <= 0)
		{
			throw new ArgumentException("Levels must be greater than 0.", "levels");
		}
		if (actual >= max)
		{
			return levels - 1;
		}
		if (actual <= 0.0)
		{
			return 0;
		}
		return (int)Math.Ceiling(actual / max * (double)(levels - 2));
	}

	public static int RoundToNearestLevels(double actual, double max, int levels)
	{
		if (levels <= 1)
		{
			throw new ArgumentException("Levels must be greater than 1.", "levels");
		}
		if (actual >= max)
		{
			return levels;
		}
		if (actual <= 0.0)
		{
			return 0;
		}
		return (int)Math.Round(actual / max * (double)levels, MidpointRounding.AwayFromZero);
	}

	public static int RoundToEqualLevels(double actual, double max, int levels)
	{
		if (levels <= 1)
		{
			throw new ArgumentException("Levels must be greater than 1.", "levels");
		}
		if (actual >= max)
		{
			return levels - 1;
		}
		if (actual <= 0.0)
		{
			return 0;
		}
		return (int)Math.Round(actual / max * (double)levels, MidpointRounding.ToZero);
	}
}
