using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Linguini.Bundle;
using Linguini.Bundle.Errors;
using Linguini.Syntax.Ast;
using Linguini.Syntax.Parser.Error;
using Robust.Shared.Collections;
using Robust.Shared.Utility;

namespace Robust.Shared.Localization;

internal static class LocHelper
{
	public static string FormatCompileErrors(this ParseError self, ReadOnlyMemory<char> resource, string? newLine = null)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		ErrorSpan span = new ErrorSpan(self.Row, self.Slice.Value.Start.Value, self.Slice.Value.End.Value, self.Position.Start.Value, self.Position.End.Value);
		return FormatErrors(self.Message, span, resource, newLine);
	}

	public static bool InsertResourcesAndReport(this FluentBundle bundle, Resource resource, ResPath path, [NotNullWhen(false)] out List<LocError>? errors)
	{
		Unsafe.SkipInit(out List<FluentError> list);
		if (!bundle.AddResource(resource, ref list))
		{
			errors = new List<LocError>();
			foreach (FluentError item in list)
			{
				errors.Add(new LocError(path, item));
			}
			return false;
		}
		errors = null;
		return true;
	}

	private static string FormatErrors(string message, ErrorSpan span, ReadOnlyMemory<char> resource, string? newLine)
	{
		if (newLine == null)
		{
			newLine = Environment.NewLine;
		}
		ValueList<(int, int)> valueList = default(ValueList<(int, int)>);
		int num = -1;
		int num2 = 0;
		int num3 = 0;
		LineEnumerator lineEnumerator = new LineEnumerator(resource);
		int start;
		int end;
		while (lineEnumerator.MoveNext(out start, out end))
		{
			num3++;
			if (span.StartSpan < end)
			{
				if (span.EndSpan <= start)
				{
					break;
				}
				valueList.Add((start, end));
				if (num == -1)
				{
					num = num3;
				}
				if (span.StartMark < end && span.StartMark >= start)
				{
					num2 = span.StartMark - start;
				}
			}
		}
		int value = valueList.Count + num - 1;
		int length = $"{value}".Length;
		StringBuilder stringBuilder = new StringBuilder();
		num3 = num;
		StringBuilder stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler;
		foreach (var item3 in valueList)
		{
			int item = item3.Item1;
			int item2 = item3.Item2;
			string value2 = $"{num3}".PadLeft(length);
			ReadOnlySpan<char> span2 = resource.Span;
			int num4 = item;
			ReadOnlySpan<char> span3 = span2.Slice(num4, item2 - num4);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder2);
			handler.AppendLiteral(" ");
			handler.AppendFormatted(value2);
			handler.AppendLiteral(" |");
			handler.AppendFormatted(span3.TrimEnd());
			stringBuilder3.Append(ref handler);
			stringBuilder.Append(newLine);
			num3++;
		}
		stringBuilder.Append(' ', num2 + length + 3);
		stringBuilder.Append('^', span.EndMark - span.StartMark);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
		handler.AppendLiteral(" ");
		handler.AppendFormatted(message);
		stringBuilder4.Append(ref handler);
		return stringBuilder.ToString();
	}
}
