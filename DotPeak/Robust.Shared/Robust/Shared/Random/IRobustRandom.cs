// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Random.IRobustRandom
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Random;

public interface IRobustRandom
{
  [Obsolete("Do not access the underlying implementation")]
  System.Random GetRandom();

  void SetSeed(int seed);

  float NextFloat();

  float NextFloat(float minValue, float maxValue)
  {
    return this.NextFloat() * (maxValue - minValue) + minValue;
  }

  float NextFloat(float maxValue) => this.NextFloat() * maxValue;

  int Next();

  int Next(int maxValue);

  int Next(int minValue, int maxValue);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  byte NextByte() => this.NextByte(byte.MaxValue);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  byte NextByte(byte maxValue) => this.NextByte((byte) 0, maxValue);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  byte NextByte(byte minValue, byte maxValue) => (byte) this.Next((int) minValue, (int) maxValue);

  double NextDouble();

  double Next(double maxValue) => this.NextDouble() * maxValue;

  double NextDouble(double minValue, double maxValue)
  {
    return this.NextDouble() * (maxValue - minValue) + minValue;
  }

  TimeSpan Next(TimeSpan maxTime);

  TimeSpan Next(TimeSpan minTime, TimeSpan maxTime);

  void NextBytes(byte[] buffer);

  Angle NextAngle() => Angle.op_Implicit(this.NextFloat() * 6.28318548f);

  Angle NextAngle(Angle maxValue)
  {
    return Angle.op_Implicit((double) this.NextFloat() * Angle.op_Implicit(maxValue));
  }

  Angle NextAngle(Angle minValue, Angle maxValue)
  {
    return Angle.op_Addition(Angle.op_Implicit((double) this.NextFloat() * Angle.op_Implicit(Angle.op_Subtraction(maxValue, minValue))), minValue);
  }

  Vector2 NextVector2(float maxMagnitude = 1f) => this.NextVector2(0.0f, maxMagnitude);

  Vector2 NextVector2(float minMagnitude, float maxMagnitude)
  {
    Angle angle = this.NextAngle();
    ref Angle local1 = ref angle;
    Vector2 vector2 = new Vector2(this.NextFloat(minMagnitude, maxMagnitude), 0.0f);
    ref Vector2 local2 = ref vector2;
    return ((Angle) ref local1).RotateVec(ref local2);
  }

  Vector2 NextVector2Box(float minX, float minY, float maxX, float maxY)
  {
    return new Vector2(this.NextFloat(minX, maxX), this.NextFloat(minY, maxY));
  }

  Vector2 NextVector2Box(float maxAbsX = 1f, float maxAbsY = 1f)
  {
    return this.NextVector2Box(-maxAbsX, -maxAbsY, maxAbsX, maxAbsY);
  }

  void Shuffle<T>(IList<T> list)
  {
    if (list is T[] list1)
    {
      this.Shuffle<T>((Span<T>) list1);
    }
    else
    {
      int count = list.Count;
      while (count > 1)
      {
        --count;
        int index1 = this.Next(count + 1);
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
  }

  void Shuffle<T>(Span<T> list)
  {
    int length = list.Length;
    while (length > 1)
    {
      --length;
      int index = this.Next(length + 1);
      ref T local1 = ref list[index];
      ref T local2 = ref list[length];
      T obj1 = list[length];
      T obj2 = list[index];
      local1 = obj1;
      T obj3 = obj2;
      local2 = obj3;
    }
  }

  void Shuffle<T>(ValueList<T> list) => this.Shuffle<T>(list.Span);
}
