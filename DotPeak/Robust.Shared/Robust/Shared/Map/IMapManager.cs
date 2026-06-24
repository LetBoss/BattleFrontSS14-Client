// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.IMapManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Map;

[NotContentImplementable]
public interface IMapManager
{
  const bool Approximate = false;
  const bool IncludeMap = true;

  bool SuppressOnTileChanged { get; set; }

  void Initialize();

  void Shutdown();

  void Startup();

  void Restart();

  [Obsolete("Use MapSystem")]
  MapId CreateMap(MapId? mapId = null);

  [Obsolete("Use MapSystem")]
  bool MapExists([NotNullWhen(true)] MapId? mapId);

  [Obsolete("Use MapSystem")]
  EntityUid GetMapEntityId(MapId mapId);

  [Obsolete("Use MapSystem")]
  EntityUid GetMapEntityIdOrThrow(MapId mapId);

  [Obsolete("Use MapSystem")]
  IEnumerable<MapId> GetAllMapIds();

  [Obsolete("Use MapSystem")]
  void DeleteMap(MapId mapId);

  MapGridComponent CreateGrid(MapId currentMapId, ushort chunkSize = 16 /*0x10*/);

  MapGridComponent CreateGrid(MapId currentMapId, in GridCreateOptions options);

  MapGridComponent CreateGrid(MapId currentMapId);

  Entity<MapGridComponent> CreateGridEntity(MapId currentMapId, GridCreateOptions? options = null);

  Entity<MapGridComponent> CreateGridEntity(EntityUid map, GridCreateOptions? options = null);

  IEnumerable<MapGridComponent> GetAllMapGrids(MapId mapId);

  IEnumerable<Entity<MapGridComponent>> GetAllGrids(MapId mapId);

  void FindGridsIntersecting<T>(
    MapId mapId,
    T shape,
    Transform transform,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape;

  void FindGridsIntersecting<T>(
    MapId mapId,
    T shape,
    Transform transform,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape;

  void FindGridsIntersecting(
    MapId mapId,
    Box2 worldAABB,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting<TState>(
    MapId mapId,
    Box2 worldAABB,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting(
    MapId mapId,
    Box2 worldAABB,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting(
    MapId mapId,
    Box2Rotated worldBounds,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting<TState>(
    MapId mapId,
    Box2Rotated worldBounds,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting(
    MapId mapId,
    Box2Rotated worldBounds,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting<T>(
    EntityUid mapEnt,
    T shape,
    Transform transform,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape;

  void FindGridsIntersecting<T, TState>(
    EntityUid mapEnt,
    T shape,
    Transform transform,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape;

  void FindGridsIntersecting(
    EntityUid mapEnt,
    List<IPhysShape> shapes,
    Transform transform,
    ref List<Entity<MapGridComponent>> entities,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting<T>(
    EntityUid mapEnt,
    T shape,
    Transform transform,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true)
    where T : IPhysShape;

  void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2 worldAABB,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting<TState>(
    EntityUid mapEnt,
    Box2 worldAABB,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2 worldAABB,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2Rotated worldBounds,
    GridCallback callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting<TState>(
    EntityUid mapEnt,
    Box2Rotated worldBounds,
    ref TState state,
    GridCallback<TState> callback,
    bool approx = false,
    bool includeMap = true);

  void FindGridsIntersecting(
    EntityUid mapEnt,
    Box2Rotated worldBounds,
    ref List<Entity<MapGridComponent>> grids,
    bool approx = false,
    bool includeMap = true);

  bool TryFindGridAt(
    EntityUid mapEnt,
    Vector2 worldPos,
    out EntityUid uid,
    [NotNullWhen(true)] out MapGridComponent? grid);

  bool TryFindGridAt(MapId mapId, Vector2 worldPos, out EntityUid uid, [NotNullWhen(true)] out MapGridComponent? grid);

  bool TryFindGridAt(MapCoordinates mapCoordinates, out EntityUid uid, [NotNullWhen(true)] out MapGridComponent? grid);

  [Obsolete]
  bool TryFindGridAt(
    MapId mapId,
    Vector2 worldPos,
    EntityQuery<TransformComponent> query,
    out EntityUid uid,
    [NotNullWhen(true)] out MapGridComponent? grid)
  {
    return this.TryFindGridAt(mapId, worldPos, out uid, out grid);
  }

  [Obsolete]
  IEnumerable<MapGridComponent> FindGridsIntersecting(
    MapId mapId,
    Box2 worldAabb,
    bool approx = false,
    bool includeMap = true)
  {
    List<Entity<MapGridComponent>> grids = new List<Entity<MapGridComponent>>();
    this.FindGridsIntersecting(mapId, worldAabb, ref grids, approx, includeMap);
    foreach (Entity<MapGridComponent> entity in grids)
      yield return entity.Comp;
  }

  [Obsolete]
  IEnumerable<MapGridComponent> FindGridsIntersecting(
    MapId mapId,
    Box2Rotated worldArea,
    bool approx = false,
    bool includeMap = true)
  {
    List<Entity<MapGridComponent>> grids = new List<Entity<MapGridComponent>>();
    this.FindGridsIntersecting(mapId, worldArea, ref grids, approx, includeMap);
    foreach (Entity<MapGridComponent> entity in grids)
      yield return entity.Comp;
  }

  [Obsolete("Just delete the grid entity")]
  void DeleteGrid(EntityUid euid);

  [Obsolete("Use HasComp")]
  bool IsGrid(EntityUid uid);

  [Obsolete("Use HasComp")]
  bool IsMap(EntityUid uid);

  [Obsolete("Use MapSystem")]
  void SetMapPaused(MapId mapId, bool paused);

  [Obsolete("Use MapSystem")]
  void DoMapInitialize(MapId mapId);

  [Obsolete("Use MapSystem")]
  bool IsMapPaused(MapId mapId);

  [Obsolete("Use MapSystem")]
  bool IsMapInitialized(MapId mapId);
}
