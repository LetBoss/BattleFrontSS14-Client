using System;

namespace Robust.Shared.Random;

public sealed class RobustRandom : IRobustRandom
{
	private System.Random _random = new System.Random();

	public System.Random GetRandom()
	{
		return _random;
	}

	public void SetSeed(int seed)
	{
		_random = new System.Random(seed);
	}

	public float NextFloat()
	{
		return _random.NextFloat();
	}

	public int Next()
	{
		return _random.Next();
	}

	public int Next(int minValue, int maxValue)
	{
		return _random.Next(minValue, maxValue);
	}

	public TimeSpan Next(TimeSpan minTime, TimeSpan maxTime)
	{
		return minTime + (maxTime - minTime) * _random.NextDouble();
	}

	public TimeSpan Next(TimeSpan maxTime)
	{
		return Next(TimeSpan.Zero, maxTime);
	}

	public int Next(int maxValue)
	{
		return _random.Next(maxValue);
	}

	public double NextDouble()
	{
		return _random.NextDouble();
	}

	public void NextBytes(byte[] buffer)
	{
		_random.NextBytes(buffer);
	}
}
