// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Events.RegenerateGridBoundsEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Map.Events;

[ByRefEvent]
internal readonly record struct RegenerateGridBoundsEvent(
  EntityUid Entity,
  Dictionary<MapChunk, List<Box2i>> ChunkRectangles,
  List<MapChunk> RemovedChunks)
{
  public readonly EntityUid Entity = Entity;
  public readonly Dictionary<MapChunk, List<Box2i>> ChunkRectangles = ChunkRectangles;
  public readonly List<MapChunk> RemovedChunks = RemovedChunks;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<EntityUid>.Default.GetHashCode(this.Entity) * -1521134295 + EqualityComparer<Dictionary<MapChunk, List<Box2i>>>.Default.GetHashCode(this.ChunkRectangles)) * -1521134295 + EqualityComparer<List<MapChunk>>.Default.GetHashCode(this.RemovedChunks);
  }

  [CompilerGenerated]
  public bool Equals(RegenerateGridBoundsEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<Dictionary<MapChunk, List<Box2i>>>.Default.Equals(this.ChunkRectangles, other.ChunkRectangles) && EqualityComparer<List<MapChunk>>.Default.Equals(this.RemovedChunks, other.RemovedChunks);
  }

  [CompilerGenerated]
  public void Deconstruct(
    out EntityUid Entity,
    out Dictionary<MapChunk, List<Box2i>> ChunkRectangles,
    out List<MapChunk> RemovedChunks)
  {
    Entity = this.Entity;
    ChunkRectangles = this.ChunkRectangles;
    RemovedChunks = this.RemovedChunks;
  }
}
