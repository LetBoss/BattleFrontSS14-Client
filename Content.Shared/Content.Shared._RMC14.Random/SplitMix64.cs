using System;

namespace Content.Shared._RMC14.Random;

public record struct SplitMix64
{
	private ulong x;

	public SplitMix64()
		: this(DateTime.UtcNow.Ticks)
	{
	}

	public SplitMix64(long seed)
	{
		x = (ulong)seed;
	}

	public long Next()
	{
		ulong num = (x += 11400714819323198485uL);
		long num2 = (long)(num ^ (num >> 30)) * -4658895280553007687L;
		long num3 = (num2 ^ (num2 >>> 27)) * -7723592293110705685L;
		return num3 ^ (num3 >>> 31);
	}
}
