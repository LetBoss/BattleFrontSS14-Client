// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tiles.ProtectedGridSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.Tiles;

public sealed class ProtectedGridSystem : EntitySystem
{
  [Dependency]
  private SharedMapSystem _map;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ProtectedGridComponent, MapInitEvent>(new EntityEventRefHandler<ProtectedGridComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ProtectedGridComponent, FloorTileAttemptEvent>(new EntityEventRefHandler<ProtectedGridComponent, FloorTileAttemptEvent>(this.OnFloorTileAttempt));
  }

  private void OnMapInit(Entity<ProtectedGridComponent> ent, ref MapInitEvent args)
  {
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>((EntityUid) ent, out comp))
      return;
    ChunkIndicesEnumerator indicesEnumerator = new ChunkIndicesEnumerator(comp.LocalAABB, 8);
    Vector2i? indices;
    while (indicesEnumerator.MoveNext(out indices))
    {
      ulong num = 0;
      for (int index1 = 0; index1 < 8; ++index1)
      {
        for (int index2 = 0; index2 < 8; ++index2)
        {
          Vector2i tileCoordinates;
          // ISSUE: explicit constructor call
          ((Vector2i) ref tileCoordinates).\u002Ector(index1 + indices.Value.X * 8, index2 + indices.Value.Y * 8);
          if (!this._map.GetTileRef(ent.Owner, comp, tileCoordinates).Tile.IsEmpty)
          {
            ulong bitmask = SharedMapSystem.ToBitmask(new Vector2i(index1, index2));
            num |= bitmask;
          }
        }
      }
      if (num != 0UL)
        ent.Comp.BaseIndices[indices.Value] = num;
    }
    this.Dirty<ProtectedGridComponent>(ent);
  }

  private void OnFloorTileAttempt(
    Entity<ProtectedGridComponent> ent,
    ref FloorTileAttemptEvent args)
  {
    Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(args.GridIndices, 8);
    ulong bitmask;
    if (!ent.Comp.BaseIndices.TryGetValue(chunkIndices, out bitmask))
    {
      args.Cancelled = true;
    }
    else
    {
      if (!SharedMapSystem.FromBitmask(args.GridIndices, bitmask))
        return;
      args.Cancelled = true;
    }
  }
}
