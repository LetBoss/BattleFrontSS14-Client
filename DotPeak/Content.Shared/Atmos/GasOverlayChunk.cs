// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.GasOverlayChunk
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Atmos;

[NetSerializable]
[Access(new Type[] {typeof (SharedGasTileOverlaySystem)})]
[Serializable]
public sealed class GasOverlayChunk
{
  public readonly Vector2i Index;
  public readonly Vector2i Origin;
  public SharedGasTileOverlaySystem.GasOverlayData[] TileData = new SharedGasTileOverlaySystem.GasOverlayData[64 /*0x40*/];
  [NonSerialized]
  public GameTick LastUpdate;

  public GasOverlayChunk(Vector2i index)
  {
    this.Index = index;
    this.Origin = Vector2i.op_Multiply(this.Index, 8);
  }

  public GasOverlayChunk(GasOverlayChunk data)
  {
    this.Index = data.Index;
    this.Origin = data.Origin;
    Array.Copy((Array) data.TileData, (Array) this.TileData, data.TileData.Length);
  }

  public int GetDataIndex(Vector2i gridIndices)
  {
    return gridIndices.X - this.Origin.X + (gridIndices.Y - this.Origin.Y) * 8;
  }

  private bool InBounds(Vector2i gridIndices)
  {
    return gridIndices.X >= this.Origin.X && gridIndices.Y >= this.Origin.Y && gridIndices.X < this.Origin.X + 8 && gridIndices.Y < this.Origin.Y + 8;
  }
}
