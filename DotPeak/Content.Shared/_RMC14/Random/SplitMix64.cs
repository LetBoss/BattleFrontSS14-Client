// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Random.SplitMix64
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared._RMC14.Random;

public record struct SplitMix64(long seed)
{
  private ulong x = (ulong) seed;

  public SplitMix64()
    : this(DateTime.UtcNow.Ticks)
  {
  }

  public long Next()
  {
    long num1 = (long) (this.x += 11400714819323198485UL);
    long num2 = (num1 ^ num1 >>> 30) * -4658895280553007687L;
    long num3 = (num2 ^ num2 >>> 27) * -7723592293110705685L;
    return num3 ^ num3 >>> 31 /*0x1F*/;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode() => EqualityComparer<ulong>.Default.GetHashCode(this.x);

  [CompilerGenerated]
  public readonly bool Equals(SplitMix64 other)
  {
    return EqualityComparer<ulong>.Default.Equals(this.x, other.x);
  }
}
