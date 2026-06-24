using System;

namespace Robust.Shared.Utility;

public static class ByteHelpers
{
	private static readonly string[] ByteSuffixes = new string[9] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

	public static string FormatKibibytes(long bytes)
	{
		return $"{bytes / 1024} KiB";
	}

	public static string FormatBytes(long bytes)
	{
		double num = bytes;
		int i;
		for (i = 0; i < ByteSuffixes.Length; i++)
		{
			if (!(num >= 1024.0))
			{
				break;
			}
			num /= 1024.0;
		}
		return $"{Math.Round(num, 2)} {ByteSuffixes[i]}";
	}
}
