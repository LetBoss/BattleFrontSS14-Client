using System;

namespace Content.Shared._RMC14.Random;

public record struct Xoshiro128P
{
	private uint _s0;

	private uint _s1;

	private uint _s2;

	private uint _s3;

	public Xoshiro128P()
		: this(DateTime.UtcNow.Ticks)
	{
	}

	public Xoshiro128P(long seed)
	{
		SplitMix64 sm64 = new SplitMix64(seed);
		_s0 = (uint)sm64.Next();
		_s1 = (uint)sm64.Next();
		_s2 = (uint)sm64.Next();
		_s3 = (uint)sm64.Next();
	}

	public Xoshiro128P(long s0, long s1)
	{
		SplitMix64 sm64 = new SplitMix64(s0);
		_s0 = (uint)sm64.Next();
		_s1 = (uint)sm64.Next();
		sm64 = new SplitMix64(s1);
		_s2 = (uint)sm64.Next();
		_s3 = (uint)sm64.Next();
	}

	public int Next()
	{
		uint value = _s0 + _s3;
		uint t = _s1 << 9;
		_s2 ^= _s0;
		_s3 ^= _s1;
		_s1 ^= _s2;
		_s0 ^= _s3;
		_s2 ^= t;
		_s3 = RotateLeft(_s3, 11);
		return Math.Abs((int)value);
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
