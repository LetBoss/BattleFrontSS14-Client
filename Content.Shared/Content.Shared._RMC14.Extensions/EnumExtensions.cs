using System;
using System.Collections.Generic;

namespace Content.Shared._RMC14.Extensions;

public static class EnumExtensions
{
	private static class EnumInformation<T> where T : struct, Enum
	{
		internal static readonly T[] Values;

		static EnumInformation()
		{
			Values = Enum.GetValues<T>();
		}
	}

	public static T NextWrap<T>(this T en) where T : struct, Enum
	{
		Span<T> values = EnumInformation<T>.Values.AsSpan();
		if (values.Length == 0)
		{
			return default(T);
		}
		for (int i = 0; i < values.Length; i++)
		{
			T value = values[i];
			if (EqualityComparer<T>.Default.Equals(en, value))
			{
				if (values.Length <= i + 1)
				{
					return values[0];
				}
				return values[i + 1];
			}
		}
		return values[0];
	}
}
