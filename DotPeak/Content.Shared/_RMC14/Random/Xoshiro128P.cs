// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Random.Xoshiro128P
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared._RMC14.Random;

public record struct Xoshiro128P
{
  private uint _s0;
  private uint _s1;
  private uint _s2;
  private uint _s3;

  public Xoshiro128P()
    : this(DateTime.UtcNow.Ticks)
  {
  }

  public Xoshiro128P(long seed)
  {
    SplitMix64 splitMix64 = new SplitMix64(seed);
    this._s0 = (uint) splitMix64.Next();
    this._s1 = (uint) splitMix64.Next();
    this._s2 = (uint) splitMix64.Next();
    this._s3 = (uint) splitMix64.Next();
  }

  public Xoshiro128P(long s0, long s1)
  {
    SplitMix64 splitMix64 = new SplitMix64(s0);
    this._s0 = (uint) splitMix64.Next();
    this._s1 = (uint) splitMix64.Next();
    splitMix64 = new SplitMix64(s1);
    this._s2 = (uint) splitMix64.Next();
    this._s3 = (uint) splitMix64.Next();
  }

  public int Next()
  {
    int num1 = (int) this._s0 + (int) this._s3;
    uint num2 = this._s1 << 9;
    this._s2 ^= this._s0;
    this._s3 ^= this._s1;
    this._s1 ^= this._s2;
    this._s0 ^= this._s3;
    this._s2 ^= num2;
    this._s3 = Xoshiro128P.RotateLeft(this._s3, 11);
    return Math.Abs(num1);
  }

  public float NextFloat() => (float) this.Next() * 4.656613E-10f;

  public float NextFloat(float min, float max) => this.NextFloat() * (max - min) + min;

  private static uint RotateLeft(uint x, int k) => x << k | x >> 32 /*0x20*/ - k;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<uint>.Default.GetHashCode(this._s0) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this._s1)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this._s2)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this._s3);
  }

  [CompilerGenerated]
  public readonly bool Equals(Xoshiro128P other)
  {
    return EqualityComparer<uint>.Default.Equals(this._s0, other._s0) && EqualityComparer<uint>.Default.Equals(this._s1, other._s1) && EqualityComparer<uint>.Default.Equals(this._s2, other._s2) && EqualityComparer<uint>.Default.Equals(this._s3, other._s3);
  }
}
