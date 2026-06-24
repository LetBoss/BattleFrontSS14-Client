using System;
using System.Collections.Generic;
using System.Text;
using Robust.Shared.Collections;

namespace Robust.Shared.Utility;

public static class CommandParsing
{
	public static void ParseArguments(ReadOnlySpan<char> text, List<string> args)
	{
		ValueList<(int, int)> ranges = default(ValueList<(int, int)>);
		ParseArguments(text, args, ref ranges);
	}

	internal static void ParseArguments(ReadOnlySpan<char> text, List<string> args, ref ValueList<(int start, int end)> ranges)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		int num = -1;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '\\')
			{
				i++;
				num = i;
				if (i == text.Length)
				{
					stringBuilder.Append('\\');
					break;
				}
				stringBuilder.Append(text[i]);
				continue;
			}
			switch (c)
			{
			case '"':
				if (flag)
				{
					args.Add(stringBuilder.ToString());
					stringBuilder.Clear();
					ranges.Add((num, i + 1));
					num = -1;
				}
				else if (num < 0)
				{
					num = i;
				}
				flag = !flag;
				continue;
			case ' ':
				if (!flag)
				{
					if (stringBuilder.Length != 0)
					{
						args.Add(stringBuilder.ToString());
						stringBuilder.Clear();
						ranges.Add((num, i));
						num = -1;
					}
					continue;
				}
				break;
			}
			if (num < 0)
			{
				num = i;
			}
			stringBuilder.Append(c);
		}
		if (stringBuilder.Length != 0)
		{
			args.Add(stringBuilder.ToString());
			ranges.Add((num, text.Length));
		}
	}

	public static string Escape(string text)
	{
		return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
	}
}
