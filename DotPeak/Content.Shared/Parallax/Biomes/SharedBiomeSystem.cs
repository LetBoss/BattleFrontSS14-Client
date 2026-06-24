// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.SharedBiomeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Content.Shared.Parallax.Biomes.Layers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Parallax.Biomes;

public abstract class SharedBiomeSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager ProtoManager;
  [Dependency]
  private ISerializationManager _serManager;
  [Dependency]
  protected ITileDefinitionManager TileDefManager;
  [Dependency]
  private TileSystem _tile;
  [Dependency]
  private SharedMapSystem _map;
  protected const byte ChunkSize = 8;

  private T Pick<T>(List<T> collection, float value)
  {
    value %= 1f;
    value = Math.Clamp(value, 0.0f, 1f);
    if (collection.Count == 1)
      return collection[0];
    float num = value * (float) collection.Count;
    foreach (T obj in collection)
    {
      --num;
      if ((double) num <= 0.0)
        return obj;
    }
    throw new ArgumentOutOfRangeException();
  }

  private int Pick(int count, float value)
  {
    value %= 1f;
    value = Math.Clamp(value, 0.0f, 1f);
    if (count == 1)
      return 0;
    value *= (float) count;
    for (int index = 0; index < count; ++index)
    {
      --value;
      if ((double) value <= 0.0)
        return index;
    }
    throw new ArgumentOutOfRangeException();
  }

  public bool TryGetBiomeTile(
    EntityUid uid,
    MapGridComponent grid,
    Vector2i indices,
    [NotNullWhen(true)] out Tile? tile)
  {
    TileRef tile1;
    if (this._map.TryGetTileRef(uid, grid, indices, out tile1) && !tile1.Tile.IsEmpty)
    {
      tile = new Tile?(tile1.Tile);
      return true;
    }
    BiomeComponent comp;
    if (this.TryComp<BiomeComponent>(uid, out comp))
      return this.TryGetBiomeTile(indices, comp.Layers, comp.Seed, new Entity<MapGridComponent>?((Entity<MapGridComponent>) (uid, grid)), out tile);
    tile = new Tile?();
    return false;
  }

  public bool TryGetBiomeTile(
    Vector2i indices,
    List<IBiomeLayer> layers,
    int seed,
    Entity<MapGridComponent>? grid,
    [NotNullWhen(true)] out Tile? tile)
  {
    if (grid.HasValue)
    {
      Entity<MapGridComponent> valueOrDefault = grid.GetValueOrDefault();
      TileRef tile1;
      if (this._map.TryGetTileRef((EntityUid) valueOrDefault, valueOrDefault.Comp, indices, out tile1) && !tile1.Tile.IsEmpty)
      {
        tile = new Tile?(tile1.Tile);
        return true;
      }
    }
    return this.TryGetTile(indices, layers, seed, grid, out tile);
  }

  [Obsolete("Use the Entity<MapGridComponent>? overload")]
  public bool TryGetBiomeTile(
    Vector2i indices,
    List<IBiomeLayer> layers,
    int seed,
    MapGridComponent? grid,
    [NotNullWhen(true)] out Tile? tile)
  {
    return this.TryGetBiomeTile(indices, layers, seed, grid == null ? new Entity<MapGridComponent>?() : new Entity<MapGridComponent>?((Entity<MapGridComponent>) (grid.Owner, grid)), out tile);
  }

  public bool TryGetTile(
    Vector2i indices,
    List<IBiomeLayer> layers,
    int seed,
    Entity<MapGridComponent>? grid,
    [NotNullWhen(true)] out Tile? tile)
  {
    for (int index = layers.Count - 1; index >= 0; --index)
    {
      IBiomeLayer layer = layers[index];
      FastNoiseLite noise1 = this.GetNoise(layer.Noise, seed);
      int num = layer.Invert ? 1 : 0;
      float noise2 = noise1.GetNoise((float) indices.X, (float) indices.Y);
      if ((num != 0 ? (double) noise2 * -1.0 : (double) noise2) >= (double) layer.Threshold)
      {
        switch (layer)
        {
          case BiomeMetaLayer biomeMetaLayer:
            if (this.TryGetBiomeTile(indices, this.ProtoManager.Index<BiomeTemplatePrototype>(biomeMetaLayer.Template).Layers, seed, grid, out tile))
              return true;
            continue;
          case BiomeTileLayer biomeTileLayer:
            if (this.TryGetTile(indices, noise1, biomeTileLayer.Invert, biomeTileLayer.Threshold, this.ProtoManager.Index<ContentTileDefinition>(biomeTileLayer.Tile), biomeTileLayer.Variants, out tile))
              return true;
            continue;
          default:
            continue;
        }
      }
    }
    tile = new Tile?();
    return false;
  }

  [Obsolete("Use the Entity<MapGridComponent>? overload")]
  public bool TryGetTile(
    Vector2i indices,
    List<IBiomeLayer> layers,
    int seed,
    MapGridComponent? grid,
    [NotNullWhen(true)] out Tile? tile)
  {
    return this.TryGetTile(indices, layers, seed, grid == null ? new Entity<MapGridComponent>?() : new Entity<MapGridComponent>?((Entity<MapGridComponent>) (grid.Owner, grid)), out tile);
  }

  private bool TryGetTile(
    Vector2i indices,
    FastNoiseLite noise,
    bool invert,
    float threshold,
    ContentTileDefinition tileDef,
    List<byte>? variants,
    [NotNullWhen(true)] out Tile? tile)
  {
    float noise1 = noise.GetNoise((float) indices.X, (float) indices.Y);
    if ((invert ? (double) noise1 * -1.0 : (double) noise1) < (double) threshold)
    {
      tile = new Tile?();
      return false;
    }
    byte flags = 0;
    // ISSUE: explicit non-virtual call
    int z = variants != null ? __nonvirtual (variants.Count) : (int) tileDef.Variants;
    if (z > 1)
    {
      float seed = (float) (((double) noise.GetNoise((float) (indices.X * 8), (float) (indices.Y * 8), (float) z) + 1.0) * 100.0);
      flags = this._tile.PickVariant(tileDef, (int) seed);
    }
    tile = new Tile?(new Tile((int) tileDef.TileId, flags));
    return true;
  }

  public bool TryGetEntity(
    Vector2i indices,
    BiomeComponent component,
    Entity<MapGridComponent>? grid,
    [NotNullWhen(true)] out string? entity)
  {
    Tile? tile;
    if (this.TryGetBiomeTile(indices, component.Layers, component.Seed, grid, out tile))
      return this.TryGetEntity(indices, component.Layers, tile.Value, component.Seed, grid, out entity);
    entity = (string) null;
    return false;
  }

  [Obsolete("Use the Entity<MapGridComponent>? overload")]
  public bool TryGetEntity(
    Vector2i indices,
    BiomeComponent component,
    MapGridComponent grid,
    [NotNullWhen(true)] out string? entity)
  {
    return this.TryGetEntity(indices, component, grid == null ? new Entity<MapGridComponent>?() : new Entity<MapGridComponent>?((Entity<MapGridComponent>) (grid.Owner, grid)), out entity);
  }

  public bool TryGetEntity(
    Vector2i indices,
    List<IBiomeLayer> layers,
    Tile tileRef,
    int seed,
    Entity<MapGridComponent>? grid,
    [NotNullWhen(true)] out string? entity)
  {
    string id = this.TileDefManager[tileRef.TypeId].ID;
    for (int index = layers.Count - 1; index >= 0; --index)
    {
      IBiomeLayer layer = layers[index];
      switch (layer)
      {
        case IBiomeWorldLayer biomeWorldLayer:
          if (!biomeWorldLayer.AllowedTiles.Contains(id))
            break;
          goto label_3;
        case BiomeMetaLayer _:
label_3:
          FastNoiseLite noise1 = this.GetNoise(layer.Noise, seed);
          bool invert = layer.Invert;
          float noise2 = noise1.GetNoise((float) indices.X, (float) indices.Y);
          if ((invert ? (double) noise2 * -1.0 : (double) noise2) >= (double) layer.Threshold)
          {
            switch (layer)
            {
              case BiomeMetaLayer biomeMetaLayer:
                if (this.TryGetEntity(indices, this.ProtoManager.Index<BiomeTemplatePrototype>(biomeMetaLayer.Template).Layers, tileRef, seed, grid, out entity))
                  return true;
                continue;
              case BiomeEntityLayer biomeEntityLayer:
                float noise3 = noise1.GetNoise((float) indices.X, (float) indices.Y, (float) index);
                entity = this.Pick<string>(biomeEntityLayer.Entities, (float) (((double) noise3 + 1.0) / 2.0));
                return true;
              default:
                entity = (string) null;
                return false;
            }
          }
          else
            break;
      }
    }
    entity = (string) null;
    return false;
  }

  [Obsolete("Use the Entity<MapGridComponent>? overload")]
  public bool TryGetEntity(
    Vector2i indices,
    List<IBiomeLayer> layers,
    Tile tileRef,
    int seed,
    MapGridComponent grid,
    [NotNullWhen(true)] out string? entity)
  {
    return this.TryGetEntity(indices, layers, tileRef, seed, grid == null ? new Entity<MapGridComponent>?() : new Entity<MapGridComponent>?((Entity<MapGridComponent>) (grid.Owner, grid)), out entity);
  }

  public bool TryGetDecals(
    Vector2i indices,
    List<IBiomeLayer> layers,
    int seed,
    Entity<MapGridComponent>? grid,
    [NotNullWhen(true)] out List<(string ID, Vector2 Position)>? decals)
  {
    Tile? tile;
    if (!this.TryGetBiomeTile(indices, layers, seed, grid, out tile))
    {
      decals = (List<(string, Vector2)>) null;
      return false;
    }
    string id = this.TileDefManager[tile.Value.TypeId].ID;
    for (int index1 = layers.Count - 1; index1 >= 0; --index1)
    {
      IBiomeLayer layer = layers[index1];
      switch (layer)
      {
        case IBiomeWorldLayer biomeWorldLayer:
          if (!biomeWorldLayer.AllowedTiles.Contains(id))
            break;
          goto label_5;
        case BiomeMetaLayer _:
label_5:
          bool invert = layer.Invert;
          FastNoiseLite noise1 = this.GetNoise(layer.Noise, seed);
          float noise2 = noise1.GetNoise((float) indices.X, (float) indices.Y);
          if ((invert ? (double) noise2 * -1.0 : (double) noise2) >= (double) layer.Threshold)
          {
            switch (layer)
            {
              case BiomeMetaLayer biomeMetaLayer:
                if (this.TryGetDecals(indices, this.ProtoManager.Index<BiomeTemplatePrototype>(biomeMetaLayer.Template).Layers, seed, grid, out decals))
                  return true;
                continue;
              case BiomeDecalLayer biomeDecalLayer:
                decals = new List<(string, Vector2)>();
                for (int index2 = 0; (double) index2 < (double) biomeDecalLayer.Divisions; ++index2)
                {
                  for (int index3 = 0; (double) index3 < (double) biomeDecalLayer.Divisions; ++index3)
                  {
                    Vector2 vector2 = new Vector2((float) indices.X + (float) index2 * 1f / biomeDecalLayer.Divisions, (float) indices.Y + (float) index3 * 1f / biomeDecalLayer.Divisions);
                    float noise3 = noise1.GetNoise(vector2.X, vector2.Y);
                    if ((invert ? (double) noise3 * -1.0 : (double) noise3) >= (double) biomeDecalLayer.Threshold)
                      decals.Add((this.Pick<string>(biomeDecalLayer.Decals, (float) (((double) noise1.GetNoise((float) indices.X, (float) indices.Y, (float) index2 + (float) index3 * biomeDecalLayer.Divisions) + 1.0) / 2.0)), vector2));
                  }
                }
                if (decals.Count != 0)
                  return true;
                continue;
              default:
                decals = (List<(string, Vector2)>) null;
                return false;
            }
          }
          else
            break;
      }
    }
    decals = (List<(string, Vector2)>) null;
    return false;
  }

  [Obsolete("Use the Entity<MapGridComponent>? overload")]
  public bool TryGetDecals(
    Vector2i indices,
    List<IBiomeLayer> layers,
    int seed,
    MapGridComponent grid,
    [NotNullWhen(true)] out List<(string ID, Vector2 Position)>? decals)
  {
    return this.TryGetDecals(indices, layers, seed, grid == null ? new Entity<MapGridComponent>?() : new Entity<MapGridComponent>?((Entity<MapGridComponent>) (grid.Owner, grid)), out decals);
  }

  private FastNoiseLite GetNoise(FastNoiseLite seedNoise, int seed)
  {
    FastNoiseLite target = new FastNoiseLite();
    this._serManager.CopyTo<FastNoiseLite>(seedNoise, ref target, notNullableOverride: true);
    target.SetSeed(target.GetSeed() + seed);
    target.SetFractalOctaves(target.GetFractalOctaves());
    return target;
  }
}
