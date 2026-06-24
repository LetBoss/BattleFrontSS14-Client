// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Random.RandomHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Random;

public static class RandomHelpers
{
  public static void Shuffle<T>(this System.Random random, IList<T> list)
  {
    int count = list.Count;
    while (count > 1)
    {
      --count;
      int index1 = random.Next(count + 1);
      IList<T> objList1 = list;
      int index2 = index1;
      IList<T> objList2 = list;
      int num = count;
      T obj1 = list[count];
      T obj2 = list[index1];
      objList1[index2] = obj1;
      int index3 = num;
      T obj3 = obj2;
      objList2[index3] = obj3;
    }
  }

  public static bool Prob(this System.Random random, double chance) => random.NextDouble() < chance;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte NextByte(this System.Random random, byte maxValue)
  {
    return random.NextByte((byte) 0, maxValue);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte NextByte(this System.Random random) => random.NextByte(byte.MaxValue);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte NextByte(this System.Random random, byte minValue, byte maxValue)
  {
    return (byte) random.Next((int) minValue, (int) maxValue);
  }
}
