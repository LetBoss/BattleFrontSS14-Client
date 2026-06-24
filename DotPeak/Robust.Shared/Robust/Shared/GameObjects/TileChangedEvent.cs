// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TileChangedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct TileChangedEvent
{
  public readonly Robust.Shared.GameObjects.Entity<MapGridComponent> Entity;
  public readonly TileChangedEntry[] Changes;

  public TileChangedEvent(
    Robust.Shared.GameObjects.Entity<MapGridComponent> entity,
    TileRef newTile,
    Tile oldTile,
    Vector2i chunkIndex)
    : this(entity, newTile.Tile, oldTile, chunkIndex, newTile.GridIndices)
  {
  }

  public TileChangedEvent(
    Robust.Shared.GameObjects.Entity<MapGridComponent> entity,
    Tile newTile,
    Tile oldTile,
    Vector2i chunkIndex,
    Vector2i gridIndices)
  {
    this.Entity = entity;
    this.Changes = new TileChangedEntry[1]
    {
      new TileChangedEntry(newTile, oldTile, chunkIndex, gridIndices)
    };
  }

  public TileChangedEvent(Robust.Shared.GameObjects.Entity<MapGridComponent> entity, TileChangedEntry[] changes)
  {
    this.Entity = entity;
    this.Changes = changes;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return EqualityComparer<Robust.Shared.GameObjects.Entity<MapGridComponent>>.Default.GetHashCode(this.Entity) * -1521134295 + EqualityComparer<TileChangedEntry[]>.Default.GetHashCode(this.Changes);
  }

  [CompilerGenerated]
  public bool Equals(TileChangedEvent other)
  {
    return EqualityComparer<Robust.Shared.GameObjects.Entity<MapGridComponent>>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<TileChangedEntry[]>.Default.Equals(this.Changes, other.Changes);
  }
}
