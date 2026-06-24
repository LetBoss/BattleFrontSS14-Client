// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameStates.ChunkDatum
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameStates;

[NetSerializable]
[Serializable]
public readonly struct ChunkDatum
{
  public static readonly ChunkDatum Empty;
  public readonly HashSet<string>? Fixtures;
  public readonly Tile[]? TileData;
  public readonly Box2i? CachedBounds;

  [MemberNotNullWhen(false, new string[] {"TileData", "Fixtures"})]
  public bool IsDeleted() => this.TileData == null;

  private ChunkDatum(Tile[] tileData, HashSet<string> fixtures, Box2i cachedBounds)
  {
    this.TileData = tileData;
    this.Fixtures = fixtures;
    this.CachedBounds = new Box2i?(cachedBounds);
  }

  public static ChunkDatum CreateModified(
    Tile[] tileData,
    HashSet<string> fixtures,
    Box2i cachedBounds)
  {
    return new ChunkDatum(tileData, fixtures, cachedBounds);
  }
}
