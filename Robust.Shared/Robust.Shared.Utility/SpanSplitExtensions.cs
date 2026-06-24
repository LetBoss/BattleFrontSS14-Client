using System;

namespace Robust.Shared.Utility;

internal static class SpanSplitExtensions
{
	public static bool SplitFindNext<T>(ref ReadOnlySpan<T> source, T splitOn, out ReadOnlySpan<T> splitValue) where T : IEquatable<T>
	{
		if (source.IsEmpty)
		{
			splitValue = ReadOnlySpan<T>.Empty;
			return false;
		}
		int num = source.IndexOf(splitOn);
		if (num == -1)
		{
			splitValue = source;
			source = ReadOnlySpan<T>.Empty;
		}
		else
		{
			splitValue = source.Slice(0, num);
			int num2 = num + 1;
			source = source.Slice(num2, source.Length - num2);
		}
		return true;
	}
}
