// Decompiled with JetBrains decompiler
// Type: Content.Shared.Maps.TileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Coordinates.Helpers;
using Content.Shared.Decals;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Maps;

public sealed class TileSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IRobustRandom _robustRandom;
  [Dependency]
  private ITileDefinitionManager _tileDefinitionManager;
  [Dependency]
  private SharedDecalSystem _decal;
  [Dependency]
  private SharedMapSystem _maps;
  [Dependency]
  private TurfSystem _turf;

  public byte PickVariant(ContentTileDefinition tile)
  {
    return this.PickVariant(tile, this._robustRandom.Next());
  }

  public byte PickVariant(ContentTileDefinition tile, int seed)
  {
    System.Random random = new System.Random(seed);
    return this.PickVariant(tile, random);
  }

  public byte PickVariant(ContentTileDefinition tile, System.Random random)
  {
    float[] placementVariants = tile.PlacementVariants;
    float num1 = ((IEnumerable<float>) placementVariants).Sum();
    float num2 = 0.0f;
    float num3 = random.NextSingle() * num1;
    for (byte index = 0; (int) index < placementVariants.Length; ++index)
    {
      num2 += placementVariants[(int) index];
      if ((double) num2 >= (double) num3)
        return index;
    }
    throw new InvalidOperationException($"Invalid weighted variantize tile pick for {tile.ID}!");
  }

  public Tile GetVariantTile(ContentTileDefinition tile, System.Random random)
  {
    return new Tile((int) tile.TileId, variant: this.PickVariant(tile, random));
  }

  public Tile GetVariantTile(ContentTileDefinition tile, int seed)
  {
    System.Random random = new System.Random(seed);
    return new Tile((int) tile.TileId, variant: this.PickVariant(tile, random));
  }

  public bool PryTile(Vector2i indices, EntityUid gridId)
  {
    MapGridComponent grid = this.Comp<MapGridComponent>(gridId);
    return this.PryTile(this._maps.GetTileRef(gridId, grid, indices));
  }

  public bool PryTile(TileRef tileRef) => this.PryTile(tileRef, false);

  public bool PryTile(TileRef tileRef, bool pryPlating)
  {
    Tile tile = tileRef.Tile;
    return !tile.IsEmpty && ((ContentTileDefinition) this._tileDefinitionManager[tile.TypeId]).CanCrowbar && this.DeconstructTile(tileRef);
  }

  public bool ReplaceTile(TileRef tileref, ContentTileDefinition replacementTile)
  {
    MapGridComponent comp;
    return this.TryComp<MapGridComponent>(tileref.GridUid, out comp) && this.ReplaceTile(tileref, replacementTile, tileref.GridUid, comp);
  }

  public bool ReplaceTile(
    TileRef tileref,
    ContentTileDefinition replacementTile,
    EntityUid grid,
    MapGridComponent? component = null)
  {
    if (!this.Resolve<MapGridComponent>(grid, ref component))
      return false;
    byte variant = this.PickVariant(replacementTile);
    foreach ((uint num, Decal _) in this._decal.GetDecalsInRange(tileref.GridUid, this._turf.GetTileCenter(tileref).Position, 0.5f))
      this._decal.RemoveDecal(tileref.GridUid, num);
    this._maps.SetTile(grid, component, tileref.GridIndices, new Tile((int) replacementTile.TileId, variant: variant));
    return true;
  }

  public bool DeconstructTile(TileRef tileRef)
  {
    if (tileRef.Tile.IsEmpty)
      return false;
    ContentTileDefinition contentTileDefinition = (ContentTileDefinition) this._tileDefinitionManager[tileRef.Tile.TypeId];
    if (string.IsNullOrEmpty(contentTileDefinition.BaseTurf))
      return false;
    EntityUid gridUid = tileRef.GridUid;
    MapGridComponent grid = this.Comp<MapGridComponent>(gridUid);
    float num1 = (float) grid.TileSize - 0.2f;
    Vector2i gridIndices = tileRef.GridIndices;
    EntityCoordinates coordinates = this._maps.GridTileToLocal(gridUid, grid, gridIndices).Offset(new Vector2((this._robustRandom.NextFloat() - 0.5f) * num1, (this._robustRandom.NextFloat() - 0.5f) * num1));
    this.Transform(this.Spawn(contentTileDefinition.ItemDropPrototypeName, coordinates)).LocalRotation = Angle.op_Implicit(this._robustRandom.NextDouble() * (2.0 * Math.PI));
    foreach ((uint num2, Decal _) in this._decal.GetDecalsInRange(gridUid, coordinates.SnapToGrid((IEntityManager) this.EntityManager, this._mapManager).Position, 0.5f))
      this._decal.RemoveDecal(tileRef.GridUid, num2);
    ITileDefinition tileDefinition = this._tileDefinitionManager[contentTileDefinition.BaseTurf];
    this._maps.SetTile(gridUid, grid, tileRef.GridIndices, new Tile((int) tileDefinition.TileId));
    return true;
  }
}
