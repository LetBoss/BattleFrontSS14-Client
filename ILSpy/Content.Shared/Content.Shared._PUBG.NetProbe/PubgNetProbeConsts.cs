using System;

namespace Content.Shared._PUBG.NetProbe;

public static class PubgNetProbeConsts
{
	public const int BytesInKb = 1024;

	public const int MaxKb = 3072;

	public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5L);

	public static int ToBytes(int kb)
	{
		return kb * 1024;
	}

	public static bool IsValidKb(int kb)
	{
		if (kb > 0)
		{
			return kb <= 3072;
		}
		return false;
	}
}
