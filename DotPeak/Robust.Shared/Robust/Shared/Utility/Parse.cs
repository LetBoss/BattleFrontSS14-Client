// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.Parse
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Globalization;

#nullable disable
namespace Robust.Shared.Utility;

public static class Parse
{
  public static bool TryInt16(ReadOnlySpan<char> text, out short result)
  {
    return Parse.TryInt16(text, NumberStyles.Integer, out result);
  }

  public static bool TryInt16(ReadOnlySpan<char> text, NumberStyles style, out short result)
  {
    return short.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static short Int16(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Integer)
  {
    return short.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryUInt16(ReadOnlySpan<char> text, out ushort result)
  {
    return Parse.TryUInt16(text, NumberStyles.Integer, out result);
  }

  public static bool TryUInt16(ReadOnlySpan<char> text, NumberStyles style, out ushort result)
  {
    return ushort.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static ushort UInt16(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Integer)
  {
    return ushort.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryInt32(ReadOnlySpan<char> text, out int result)
  {
    return Parse.TryInt32(text, NumberStyles.Integer, out result);
  }

  public static bool TryInt32(ReadOnlySpan<char> text, NumberStyles style, out int result)
  {
    return int.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static int Int32(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Integer)
  {
    return int.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryUInt32(ReadOnlySpan<char> text, out uint result)
  {
    return Parse.TryUInt32(text, NumberStyles.Integer, out result);
  }

  public static bool TryUInt32(ReadOnlySpan<char> text, NumberStyles style, out uint result)
  {
    return uint.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static uint UInt32(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Integer)
  {
    return uint.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryInt64(ReadOnlySpan<char> text, out long result)
  {
    return Parse.TryInt64(text, NumberStyles.Integer, out result);
  }

  public static bool TryInt64(ReadOnlySpan<char> text, NumberStyles style, out long result)
  {
    return long.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static long Int64(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Integer)
  {
    return long.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryUInt64(ReadOnlySpan<char> text, out ulong result)
  {
    return Parse.TryUInt64(text, NumberStyles.Integer, out result);
  }

  public static bool TryUInt64(ReadOnlySpan<char> text, NumberStyles style, out ulong result)
  {
    return ulong.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static ulong UInt64(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Integer)
  {
    return ulong.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryFloat(ReadOnlySpan<char> text, out float result)
  {
    return Parse.TryFloat(text, NumberStyles.Float, out result);
  }

  public static bool TryFloat(ReadOnlySpan<char> text, NumberStyles style, out float result)
  {
    return float.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static float Float(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Float)
  {
    return float.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryDouble(ReadOnlySpan<char> text, out double result)
  {
    return Parse.TryDouble(text, NumberStyles.Float, out result);
  }

  public static bool TryDouble(ReadOnlySpan<char> text, NumberStyles style, out double result)
  {
    return double.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static double Double(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Float)
  {
    return double.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool TryDecimal(ReadOnlySpan<char> text, out System.Decimal result)
  {
    return Parse.TryDecimal(text, NumberStyles.Float, out result);
  }

  public static bool TryDecimal(ReadOnlySpan<char> text, NumberStyles style, out System.Decimal result)
  {
    return System.Decimal.TryParse(text, style, (IFormatProvider) CultureInfo.InvariantCulture, out result);
  }

  public static System.Decimal Decimal(ReadOnlySpan<char> text, NumberStyles style = NumberStyles.Float)
  {
    return System.Decimal.Parse(text, style, (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
