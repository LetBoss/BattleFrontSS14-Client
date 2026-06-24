// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tiles.ReplaceFloorOnSpawnSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Tiles;

public sealed class ReplaceFloorOnSpawnSystem : EntitySystem
{
  [Dependency]
  private ITileDefinitionManager _tile;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedMapSystem _map;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ReplaceFloorOnSpawnComponent, MapInitEvent>(new EntityEventRefHandler<ReplaceFloorOnSpawnComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnMapInit(Entity<ReplaceFloorOnSpawnComponent> ent, ref MapInitEvent args)
  {
    TransformComponent transformComponent = this.Transform((EntityUid) ent);
    EntityUid? gridUid = transformComponent.GridUid;
    if (!gridUid.HasValue)
      return;
    EntityUid valueOrDefault = gridUid.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp) || ent.Comp.ReplaceableTiles != null && ent.Comp.ReplaceableTiles.Count == 0)
      return;
    Vector2i tile1 = this._map.LocalToTile(valueOrDefault, comp, transformComponent.Coordinates);
    foreach (Vector2i offset in ent.Comp.Offsets)
    {
      Vector2i indices = Vector2i.op_Addition(tile1, offset);
      TileRef tile2;
      if (this._map.TryGetTileRef(valueOrDefault, comp, indices, out tile2) && (ent.Comp.ReplaceableTiles == null || tile2.Tile.IsEmpty || ent.Comp.ReplaceableTiles.Contains((ProtoId<ContentTileDefinition>) this._tile[tile2.Tile.TypeId].ID)))
      {
        ProtoId<ContentTileDefinition> id = RandomExtensions.Pick<ProtoId<ContentTileDefinition>>(this._random, (IReadOnlyList<ProtoId<ContentTileDefinition>>) ent.Comp.ReplacementTiles);
        this._map.SetTile(valueOrDefault, comp, tile2.GridIndices, new Tile((int) this._prototype.Index<ContentTileDefinition>(id).TileId));
      }
    }
  }
}
