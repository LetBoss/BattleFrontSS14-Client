using System;
using System.Globalization;

namespace Content.Shared.Localizations;

public static class UserInputParser
{
	private static readonly NumberFormatInfo[] StandardDecimalNumberFormats = new NumberFormatInfo[2]
	{
		new NumberFormatInfo
		{
			NumberDecimalSeparator = "."
		},
		new NumberFormatInfo
		{
			NumberDecimalSeparator = ","
		}
	};

	public static bool TryFloat(ReadOnlySpan<char> text, out float result)
	{
		NumberFormatInfo[] standardDecimalNumberFormats = StandardDecimalNumberFormats;
		foreach (NumberFormatInfo format in standardDecimalNumberFormats)
		{
			if (float.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, format, out result))
			{
				return true;
			}
		}
		result = 0f;
		return false;
	}

	public static bool TryDouble(ReadOnlySpan<char> text, out double result)
	{
		NumberFormatInfo[] standardDecimalNumberFormats = StandardDecimalNumberFormats;
		foreach (NumberFormatInfo format in standardDecimalNumberFormats)
		{
			if (double.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, format, out result))
			{
				return true;
			}
		}
		result = 0.0;
		return false;
	}
}
