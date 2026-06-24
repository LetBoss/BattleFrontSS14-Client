// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Random.Xoroshiro64S
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared._RMC14.Random;

public record struct Xoroshiro64S
{
  private uint _s0;
  private uint _s1;

  public Xoroshiro64S()
    : this(DateTime.UtcNow.Ticks)
  {
  }

  public Xoroshiro64S(long seed)
  {
    SplitMix64 splitMix64 = new SplitMix64(seed);
    this._s0 = (uint) splitMix64.Next();
    this._s1 = (uint) splitMix64.Next();
  }

  public int Next()
  {
    uint s0 = this._s0;
    uint s1 = this._s1;
    int num = (int) s0 * -1640531525;
    uint x = s1 ^ s0;
    this._s0 = (uint) ((int) Xoroshiro64S.RotateLeft(s0, 26) ^ (int) x ^ (int) x << 9);
    this._s1 = Xoroshiro64S.RotateLeft(x, 13);
    return Math.Abs(num);
  }

  public float NextFloat() => (float) this.Next() * 4.656613E-10f;

  public float NextFloat(float min, float max) => this.NextFloat() * (max - min) + min;

  private static uint RotateLeft(uint x, int k) => x << k | x >> 32 /*0x20*/ - k;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<uint>.Default.GetHashCode(this._s0) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this._s1);
  }

  [CompilerGenerated]
  public readonly bool Equals(Xoroshiro64S other)
  {
    return EqualityComparer<uint>.Default.Equals(this._s0, other._s0) && EqualityComparer<uint>.Default.Equals(this._s1, other._s1);
  }
}
