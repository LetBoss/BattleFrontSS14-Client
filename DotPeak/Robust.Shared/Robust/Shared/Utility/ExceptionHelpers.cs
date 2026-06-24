// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ExceptionHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Reflection;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

public static class ExceptionHelpers
{
  public static string ToStringBetter(this Exception exception)
  {
    if (!(exception is ReflectionTypeLoadException typeLoadException))
      return exception.ToString();
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.AppendLine(typeLoadException.ToString());
    if (typeLoadException.LoaderExceptions != null)
    {
      int num = 0;
      foreach (Exception loaderException in typeLoadException.LoaderExceptions)
      {
        if (loaderException != null)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          StringBuilder stringBuilder3 = stringBuilder2;
          StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(30, 2, stringBuilder2);
          interpolatedStringHandler.AppendLiteral("---> (Loader Exception #");
          interpolatedStringHandler.AppendFormatted<int>(num);
          interpolatedStringHandler.AppendLiteral(" ");
          interpolatedStringHandler.AppendFormatted(loaderException.ToStringBetter());
          interpolatedStringHandler.AppendLiteral("\n<---");
          ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
          stringBuilder3.Append(ref local);
          ++num;
        }
      }
    }
    return stringBuilder1.ToString();
  }
}
