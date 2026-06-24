// Decompiled with JetBrains decompiler
// Type: Content.Shared.Localizations.UserInputParser
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Globalization;

#nullable enable
namespace Content.Shared.Localizations;

public static class UserInputParser
{
  private static readonly NumberFormatInfo[] StandardDecimalNumberFormats = new NumberFormatInfo[2]
  {
    new NumberFormatInfo() { NumberDecimalSeparator = "." },
    new NumberFormatInfo() { NumberDecimalSeparator = "," }
  };

  public static bool TryFloat(ReadOnlySpan<char> text, out float result)
  {
    foreach (NumberFormatInfo decimalNumberFormat in UserInputParser.StandardDecimalNumberFormats)
    {
      if (float.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, (IFormatProvider) decimalNumberFormat, out result))
        return true;
    }
    result = 0.0f;
    return false;
  }

  public static bool TryDouble(ReadOnlySpan<char> text, out double result)
  {
    foreach (NumberFormatInfo decimalNumberFormat in UserInputParser.StandardDecimalNumberFormats)
    {
      if (double.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, (IFormatProvider) decimalNumberFormat, out result))
        return true;
    }
    result = 0.0;
    return false;
  }
}
