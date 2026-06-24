// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.NoiseDunGenLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Noise;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Procedural.DungeonGenerators;

[DataRecord]
public record struct NoiseDunGenLayer
{
  [DataField(null, false, 1, false, false, null)]
  public float Threshold;
  [DataField(null, false, 1, true, false, null)]
  public string Tile;
  [DataField(null, false, 1, true, false, null)]
  public FastNoiseLite Noise;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<float>.Default.GetHashCode(this.Threshold) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Tile)) * -1521134295 + EqualityComparer<FastNoiseLite>.Default.GetHashCode(this.Noise);
  }

  [CompilerGenerated]
  public readonly bool Equals(NoiseDunGenLayer other)
  {
    return EqualityComparer<float>.Default.Equals(this.Threshold, other.Threshold) && EqualityComparer<string>.Default.Equals(this.Tile, other.Tile) && EqualityComparer<FastNoiseLite>.Default.Equals(this.Noise, other.Noise);
  }
}
