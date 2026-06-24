// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.CommandParsing
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

public static class CommandParsing
{
  public static void ParseArguments(ReadOnlySpan<char> text, List<string> args)
  {
    ValueList<(int, int)> ranges = new ValueList<(int, int)>();
    CommandParsing.ParseArguments(text, args, ref ranges);
  }

  internal static void ParseArguments(
    ReadOnlySpan<char> text,
    List<string> args,
    ref ValueList<(int start, int end)> ranges)
  {
    StringBuilder stringBuilder = new StringBuilder();
    bool flag = false;
    int num = -1;
    for (int index = 0; index < text.Length; ++index)
    {
      char ch = text[index];
      switch (ch)
      {
        case ' ':
          if (!flag)
          {
            if (stringBuilder.Length != 0)
            {
              args.Add(stringBuilder.ToString());
              stringBuilder.Clear();
              ranges.Add((num, index));
              num = -1;
              break;
            }
            break;
          }
          goto default;
        case '"':
          if (flag)
          {
            args.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            ranges.Add((num, index + 1));
            num = -1;
          }
          else if (num < 0)
            num = index;
          flag = !flag;
          break;
        case '\\':
          ++index;
          num = index;
          if (index == text.Length)
          {
            stringBuilder.Append('\\');
            goto label_18;
          }
          stringBuilder.Append(text[index]);
          break;
        default:
          if (num < 0)
            num = index;
          stringBuilder.Append(ch);
          break;
      }
    }
label_18:
    if (stringBuilder.Length == 0)
      return;
    args.Add(stringBuilder.ToString());
    ranges.Add((num, text.Length));
  }

  public static string Escape(string text) => text.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
