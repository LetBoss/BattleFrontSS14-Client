// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Errors.ConHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.Errors;

public static class ConHelpers
{
  public static FormattedMessage HighlightSpan(string input, Vector2i span, Color color)
  {
    FormattedMessage formattedMessage1 = FormattedMessage.FromUnformatted(input.Substring(0, span.X));
    formattedMessage1.PushColor(color);
    if (span.Y >= input.Length)
    {
      FormattedMessage formattedMessage2 = formattedMessage1;
      string str = input;
      int x = span.X;
      string text = str.Substring(x, str.Length - x);
      formattedMessage2.AddText(text);
      formattedMessage1.Pop();
      return formattedMessage1;
    }
    FormattedMessage formattedMessage3 = formattedMessage1;
    string str1 = input;
    int x1 = span.X;
    int startIndex = x1;
    int length = span.Y - x1;
    string text1 = str1.Substring(startIndex, length);
    formattedMessage3.AddText(text1);
    formattedMessage1.Pop();
    FormattedMessage formattedMessage4 = formattedMessage1;
    string str2 = input;
    int y = span.Y;
    string text2 = str2.Substring(y, str2.Length - y);
    formattedMessage4.AddText(text2);
    return formattedMessage1;
  }

  public static FormattedMessage ArrowSpan(Vector2i span)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(' ', span.X);
    stringBuilder.Append('^', span.Y - span.X);
    return FormattedMessage.FromUnformatted(stringBuilder.ToString());
  }
}
