using System;
using System.Globalization;
using Robust.Shared.Serialization.Markdown.Value;

namespace Robust.Shared.Utility;

public static class TimeSpanExt
{
	public static TimeSpan Mul(this TimeSpan time, long factor)
	{
		return TimeSpan.FromTicks(time.Ticks * factor);
	}

	public static bool TryTimeSpan(ValueDataNode node, out TimeSpan timeSpan)
	{
		return TryTimeSpan(node.Value, out timeSpan);
	}

	public static bool TryTimeSpan(string str, out TimeSpan timeSpan)
	{
		timeSpan = TimeSpan.Zero;
		if (str.Contains(',') || str.Contains(' ') || str.Contains(':'))
		{
			return false;
		}
		if (double.TryParse(str, CultureInfo.InvariantCulture, out var result))
		{
			timeSpan = TimeSpan.FromSeconds(result);
			return true;
		}
		if (str.Length <= 1)
		{
			return false;
		}
		ReadOnlySpan<char> readOnlySpan = str.AsSpan();
		if (!double.TryParse(readOnlySpan.Slice(0, readOnlySpan.Length - 1), CultureInfo.InvariantCulture, out var result2))
		{
			return false;
		}
		switch (str[str.Length - 1])
		{
		case 'S':
		case 's':
			timeSpan = TimeSpan.FromSeconds(result2);
			return true;
		case 'M':
		case 'm':
			timeSpan = TimeSpan.FromMinutes(result2);
			return true;
		case 'H':
		case 'h':
			timeSpan = TimeSpan.FromHours(result2);
			return true;
		default:
			return false;
		}
	}
}
