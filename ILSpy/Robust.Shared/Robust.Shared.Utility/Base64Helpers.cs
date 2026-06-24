using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Robust.Shared.Utility;

internal static class Base64Helpers
{
	public static string ConvertToBase64Url(byte[]? data)
	{
		if (data != null)
		{
			return ConvertToBase64Url(Convert.ToBase64String(data));
		}
		return "";
	}

	public static string ConvertToBase64Url(string b64Str)
	{
		if (b64Str == null)
		{
			throw new ArgumentNullException("b64Str");
		}
		string text = b64Str;
		int num;
		if (text[text.Length - 1] != '=')
		{
			num = 0;
		}
		else
		{
			string text2 = b64Str;
			num = ((text2[text2.Length - 2] != '=') ? 1 : 2);
		}
		int num2 = num;
		b64Str = new StringBuilder(b64Str).Replace('+', '-').Replace('/', '_').ToString(0, b64Str.Length - num2);
		return b64Str;
	}

	public static byte[] ConvertFromBase64Url(string s)
	{
		int num = s.Length % 3;
		StringBuilder stringBuilder = new StringBuilder(s);
		stringBuilder.Replace('-', '+').Replace('_', '/');
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append('=');
		}
		s = stringBuilder.ToString();
		return Convert.FromBase64String(s);
	}

	[return: NotNullIfNotNull("data")]
	public static string? ToBase64Nullable(byte[]? data)
	{
		if (data == null)
		{
			return null;
		}
		return Convert.ToBase64String(data, Base64FormattingOptions.None);
	}
}
