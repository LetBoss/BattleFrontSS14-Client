// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.ReplaceTileLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Procedural.DungeonGenerators;

[DataRecord]
public record struct ReplaceTileLayer
{
  public ProtoId<ContentTileDefinition> Tile;
  public float Threshold;
  public FastNoiseLite Noise;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<ProtoId<ContentTileDefinition>>.Default.GetHashCode(this.Tile) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Threshold)) * -1521134295 + EqualityComparer<FastNoiseLite>.Default.GetHashCode(this.Noise);
  }

  [CompilerGenerated]
  public readonly bool Equals(ReplaceTileLayer other)
  {
    return EqualityComparer<ProtoId<ContentTileDefinition>>.Default.Equals(this.Tile, other.Tile) && EqualityComparer<float>.Default.Equals(this.Threshold, other.Threshold) && EqualityComparer<FastNoiseLite>.Default.Equals(this.Noise, other.Noise);
  }
}
