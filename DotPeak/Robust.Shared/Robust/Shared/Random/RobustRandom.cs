// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Random.RobustRandom
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Random;

public sealed class RobustRandom : IRobustRandom
{
  private System.Random _random = new System.Random();

  public System.Random GetRandom() => this._random;

  public void SetSeed(int seed) => this._random = new System.Random(seed);

  public float NextFloat() => this._random.NextFloat();

  public int Next() => this._random.Next();

  public int Next(int minValue, int maxValue) => this._random.Next(minValue, maxValue);

  public TimeSpan Next(TimeSpan minTime, TimeSpan maxTime)
  {
    return minTime + (maxTime - minTime) * this._random.NextDouble();
  }

  public TimeSpan Next(TimeSpan maxTime) => this.Next(TimeSpan.Zero, maxTime);

  public int Next(int maxValue) => this._random.Next(maxValue);

  public double NextDouble() => this._random.NextDouble();

  public void NextBytes(byte[] buffer) => this._random.NextBytes(buffer);
}
