// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.Base64Helpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

internal static class Base64Helpers
{
  public static string ConvertToBase64Url(byte[]? data)
  {
    return data != null ? Base64Helpers.ConvertToBase64Url(Convert.ToBase64String(data)) : "";
  }

  public static string ConvertToBase64Url(string b64Str)
  {
    string str1 = b64Str != null ? b64Str : throw new ArgumentNullException(nameof (b64Str));
    int num1;
    if (str1[str1.Length - 1] != '=')
    {
      num1 = 0;
    }
    else
    {
      string str2 = b64Str;
      num1 = str2[str2.Length - 2] == '=' ? 2 : 1;
    }
    int num2 = num1;
    b64Str = new StringBuilder(b64Str).Replace('+', '-').Replace('/', '_').ToString(0, b64Str.Length - num2);
    return b64Str;
  }

  public static byte[] ConvertFromBase64Url(string s)
  {
    int num = s.Length % 3;
    StringBuilder stringBuilder = new StringBuilder(s);
    stringBuilder.Replace('-', '+').Replace('_', '/');
    for (int index = 0; index < num; ++index)
      stringBuilder.Append('=');
    s = stringBuilder.ToString();
    return Convert.FromBase64String(s);
  }

  [return: NotNullIfNotNull("data")]
  public static string? ToBase64Nullable(byte[]? data)
  {
    return data == null ? (string) null : Convert.ToBase64String(data, Base64FormattingOptions.None);
  }
}
