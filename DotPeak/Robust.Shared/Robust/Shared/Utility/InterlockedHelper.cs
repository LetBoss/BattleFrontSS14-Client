// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.InterlockedHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Threading;

#nullable disable
namespace Robust.Shared.Utility;

internal static class InterlockedHelper
{
  public static void Min(ref uint a, uint b)
  {
    uint num1;
    uint num2;
    do
    {
      num1 = a;
      num2 = Math.Min(num1, b);
    }
    while ((int) Interlocked.CompareExchange(ref a, num2, num1) != (int) num1);
  }
}
