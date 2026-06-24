// Decompiled with JetBrains decompiler
// Type: Robust.Shared.SharedWarmup
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared;

internal static class SharedWarmup
{
  [MethodImpl(MethodImplOptions.NoOptimization)]
  public static void WarmupCore()
  {
    RuntimeHelpers.RunClassConstructor(typeof (Color).TypeHandle);
    RuntimeHelpers.RunClassConstructor(typeof (EntitySystemManager).TypeHandle);
  }
}
