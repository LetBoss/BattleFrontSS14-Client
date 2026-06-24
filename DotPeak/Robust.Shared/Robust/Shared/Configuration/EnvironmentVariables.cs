// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.EnvironmentVariables
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Configuration;

internal static class EnvironmentVariables
{
  public const string ConfigVarEnvironmentVariable = "ROBUST_CVARS";
  public const string SingleVarPrefix = "ROBUST_CVAR_";

  internal static IEnumerable<(string, string)> GetEnvironmentCVars()
  {
    string[] strArray = (Environment.GetEnvironmentVariable("ROBUST_CVARS") ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries);
    for (int index = 0; index < strArray.Length; ++index)
    {
      string[] strArray1 = strArray[index].Split('=', 2);
      yield return (strArray1[0], strArray1[1]);
    }
    strArray = (string[]) null;
    foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
    {
      string key = (string) environmentVariable.Key;
      string str1 = (string) environmentVariable.Value;
      if (str1 != null && key.StartsWith("ROBUST_CVAR_"))
      {
        string str2 = key;
        int length = "ROBUST_CVAR_".Length;
        yield return (str2.Substring(length, str2.Length - length).Replace("__", "."), str1);
      }
    }
  }
}
