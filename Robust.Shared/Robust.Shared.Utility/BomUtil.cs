using System;

namespace Robust.Shared.Utility;

internal static class BomUtil
{
	public static Span<byte> SkipBom(Span<byte> span)
	{
		if (!HasBom(span))
		{
			return span;
		}
		return span.Slice(3, span.Length - 3);
	}

	public static bool HasBom(Span<byte> span)
	{
		if (span[2] == 191 && span[1] == 187)
		{
			return span[0] == 239;
		}
		return false;
	}
}
