using System;

namespace Content.Shared._RMC14.Random;

public record struct Xoroshiro64S
{
	private uint _s0;

	private uint _s1;

	public Xoroshiro64S()
		: this(DateTime.UtcNow.Ticks)
	{
	}

	public Xoroshiro64S(long seed)
	{
		SplitMix64 sm64 = new SplitMix64(seed);
		_s0 = (uint)sm64.Next();
		_s1 = (uint)sm64.Next();
	}

	public int Next()
	{
		uint s0 = _s0;
		uint s1 = _s1;
		int value = (int)s0 * -1640531525;
		s1 ^= s0;
		_s0 = RotateLeft(s0, 26) ^ s1 ^ (s1 << 9);
		_s1 = RotateLeft(s1, 13);
		return Math.Abs(value);
	}

	public float NextFloat()
	{
		return (float)Next() * 4.656613E-10f;
	}

	public float NextFloat(float min, float max)
	{
		return NextFloat() * (max - min) + min;
	}

	private static uint RotateLeft(uint x, int k)
	{
		return (x << k) | (x >> 32 - k);
	}
}
