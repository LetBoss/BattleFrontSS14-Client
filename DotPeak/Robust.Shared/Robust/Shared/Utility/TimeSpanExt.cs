// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.TimeSpanExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Value;
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Utility;

public static class TimeSpanExt
{
  public static TimeSpan Mul(this TimeSpan time, long factor)
  {
    return TimeSpan.FromTicks(time.Ticks * factor);
  }

  public static bool TryTimeSpan(ValueDataNode node, out TimeSpan timeSpan)
  {
    return TimeSpanExt.TryTimeSpan(node.Value, out timeSpan);
  }

  public static bool TryTimeSpan(string str, out TimeSpan timeSpan)
  {
    timeSpan = TimeSpan.Zero;
    if (str.Contains(',') || str.Contains(' ') || str.Contains(':'))
      return false;
    double result1;
    if (double.TryParse(str, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
    {
      timeSpan = TimeSpan.FromSeconds(result1);
      return true;
    }
    if (str.Length <= 1)
      return false;
    ReadOnlySpan<char> readOnlySpan = str.AsSpan();
    ref ReadOnlySpan<char> local = ref readOnlySpan;
    double result2;
    if (!double.TryParse(local.Slice(0, local.Length - 1), (IFormatProvider) CultureInfo.InvariantCulture, out result2))
      return false;
    string str1 = str;
    switch (str1[str1.Length - 1])
    {
      case 'H':
      case 'h':
        timeSpan = TimeSpan.FromHours(result2);
        return true;
      case 'M':
      case 'm':
        timeSpan = TimeSpan.FromMinutes(result2);
        return true;
      case 'S':
      case 's':
        timeSpan = TimeSpan.FromSeconds(result2);
        return true;
      default:
        return false;
    }
  }
}
