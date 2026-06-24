// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVarCommandUtil
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Configuration;

public static class CVarCommandUtil
{
  public static object ParseObject(Type type, string input)
  {
    if (type == typeof (bool))
    {
      bool result1;
      if (bool.TryParse(input, out result1))
        return (object) result1;
      int result2;
      if (Parse.TryInt32(input.AsSpan(), out result2))
      {
        if (result2 == 0)
          return (object) false;
        if (result2 == 1)
          return (object) true;
      }
      throw new FormatException("Could not parse bool value: " + input);
    }
    if (type == typeof (string))
      return (object) input;
    if (type == typeof (int))
      return (object) Parse.Int32(input.AsSpan());
    if (type == typeof (float))
      return (object) Parse.Float(input.AsSpan());
    if (type == typeof (long))
      return (object) long.Parse(input);
    if (type == typeof (ushort))
      return (object) ushort.Parse(input);
    throw new NotSupportedException();
  }

  internal static IEnumerable<CompletionOption> GetCVarCompletionOptions(IConfigurationManager cfg)
  {
    return cfg.GetRegisteredCVars().Select<string, CompletionOption>((Func<string, CompletionOption>) (c => new CompletionOption(c, CVarCommandUtil.GetCVarValueHint(cfg, c))));
  }

  private static string GetCVarValueHint(IConfigurationManager cfg, string cVar)
  {
    if ((cfg.GetCVarFlags(cVar) & CVar.CONFIDENTIAL) != CVar.NONE)
      return Loc.GetString("cmd-cvar-value-hidden");
    string cvarValueHint = cfg.GetCVar<object>(cVar).ToString() ?? "";
    if (cvarValueHint.Length > 50)
      cvarValueHint = cvarValueHint.Substring(0, 51) + "…";
    return cvarValueHint;
  }
}
