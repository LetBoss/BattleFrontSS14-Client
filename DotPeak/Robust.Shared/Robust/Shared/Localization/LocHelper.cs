// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Linguini.Bundle;
using Linguini.Bundle.Errors;
using Linguini.Syntax.Ast;
using Linguini.Syntax.Parser.Error;
using Robust.Shared.Collections;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Localization;

internal static class LocHelper
{
  public static string FormatCompileErrors(
    this ParseError self,
    ReadOnlyMemory<char> resource,
    string? newLine = null)
  {
    int row = self.Row;
    Index index = self.Slice.Value.Start;
    int num1 = index.Value;
    index = self.Slice.Value.End;
    int num2 = index.Value;
    index = self.Position.Start;
    int num3 = index.Value;
    index = self.Position.End;
    int num4 = index.Value;
    ErrorSpan span = new ErrorSpan(row, num1, num2, num3, num4);
    return LocHelper.FormatErrors(self.Message, span, resource, newLine);
  }

  public static bool InsertResourcesAndReport(
    this FluentBundle bundle,
    Resource resource,
    ResPath path,
    [NotNullWhen(false)] out List<LocError>? errors)
  {
    List<FluentError> fluentErrorList;
    if (!bundle.AddResource(resource, ref fluentErrorList))
    {
      errors = new List<LocError>();
      foreach (FluentError fluentError in fluentErrorList)
        errors.Add(new LocError(path, fluentError));
      return false;
    }
    errors = (List<LocError>) null;
    return true;
  }

  private static string FormatErrors(
    string message,
    ErrorSpan span,
    ReadOnlyMemory<char> resource,
    string? newLine)
  {
    if (newLine == null)
      newLine = Environment.NewLine;
    ValueList<(int, int)> valueList = new ValueList<(int, int)>();
    int num1 = -1;
    int num2 = 0;
    int num3 = 0;
    LineEnumerator lineEnumerator = new LineEnumerator(resource);
    int start1;
    int end;
    while (lineEnumerator.MoveNext(out start1, out end))
    {
      ++num3;
      if (span.StartSpan < end)
      {
        if (span.EndSpan > start1)
        {
          valueList.Add((start1, end));
          if (num1 == -1)
            num1 = num3;
          if (span.StartMark < end && span.StartMark >= start1)
            num2 = span.StartMark - start1;
        }
        else
          break;
      }
    }
    int length1 = $"{valueList.Count + num1 - 1}".Length;
    StringBuilder stringBuilder1 = new StringBuilder();
    int num4 = num1;
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
    foreach ((int num5, int num6) in valueList)
    {
      string str = $"{num4}".PadLeft(length1);
      ReadOnlySpan<char> span1 = resource.Span;
      ref ReadOnlySpan<char> local1 = ref span1;
      int num7 = num5;
      int start2 = num7;
      int length2 = num6 - num7;
      ReadOnlySpan<char> span2 = local1.Slice(start2, length2);
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder2);
      interpolatedStringHandler.AppendLiteral(" ");
      interpolatedStringHandler.AppendFormatted(str);
      interpolatedStringHandler.AppendLiteral(" |");
      interpolatedStringHandler.AppendFormatted(span2.TrimEnd());
      ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
      stringBuilder3.Append(ref local2);
      stringBuilder1.Append(newLine);
      ++num4;
    }
    stringBuilder1.Append(' ', num2 + length1 + 3);
    stringBuilder1.Append('^', span.EndMark - span.StartMark);
    StringBuilder stringBuilder4 = stringBuilder1;
    StringBuilder stringBuilder5 = stringBuilder4;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder4);
    interpolatedStringHandler.AppendLiteral(" ");
    interpolatedStringHandler.AppendFormatted(message);
    ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
    stringBuilder5.Append(ref local);
    return stringBuilder1.ToString();
  }
}
