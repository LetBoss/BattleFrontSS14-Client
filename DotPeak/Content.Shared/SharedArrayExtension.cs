// Decompiled with JetBrains decompiler
// Type: Content.Shared.SharedArrayExtension
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Shared;

public static class SharedArrayExtension
{
  public static void Shuffle<T>(this Span<T> array, IRobustRandom? random = null)
  {
    int length = array.Length;
    if (length <= 1)
      return;
    IoCManager.Resolve<IRobustRandom>(ref random);
    while (length > 1)
    {
      --length;
      int index = random.Next(length + 1);
      ref T local1 = ref array[index];
      ref T local2 = ref array[length];
      T obj1 = array[length];
      T obj2 = array[index];
      local1 = obj1;
      T obj3 = obj2;
      local2 = obj3;
    }
  }
}
