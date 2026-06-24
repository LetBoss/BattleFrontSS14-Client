// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.GasChunkEnumerator
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.EntitySystems;

#nullable enable
namespace Content.Shared.Atmos;

public struct GasChunkEnumerator
{
  private readonly SharedGasTileOverlaySystem.GasOverlayData[] _tileData;
  private int _index;
  public int X;
  public int Y;

  public GasChunkEnumerator(GasOverlayChunk chunk)
  {
    this._index = -1;
    this.X = 7;
    this.Y = -1;
    this._tileData = chunk.TileData;
  }

  public bool MoveNext(out SharedGasTileOverlaySystem.GasOverlayData gas)
  {
    while (++this._index < this._tileData.Length)
    {
      ++this.X;
      if (this.X >= 8)
      {
        this.X = 0;
        ++this.Y;
      }
      gas = this._tileData[this._index];
      if (!gas.Equals(new SharedGasTileOverlaySystem.GasOverlayData()))
        return true;
    }
    gas = new SharedGasTileOverlaySystem.GasOverlayData();
    return false;
  }
}
