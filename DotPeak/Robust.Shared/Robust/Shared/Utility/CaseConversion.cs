// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.CaseConversion
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Text.RegularExpressions;

#nullable enable
namespace Robust.Shared.Utility;

public static class CaseConversion
{
  private static readonly Regex PascalToKebabRegex = new Regex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", RegexOptions.Compiled);

  public static string PascalToKebab(string str)
  {
    return string.IsNullOrWhiteSpace(str) ? str : CaseConversion.PascalToKebabRegex.Replace(str, "-$1").Trim().ToLower();
  }
}
