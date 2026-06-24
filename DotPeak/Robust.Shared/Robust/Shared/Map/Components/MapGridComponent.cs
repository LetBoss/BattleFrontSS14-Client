// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Components.MapGridComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Map.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MapGridComponent : 
  Component,
  ISerializationGenerated<MapGridComponent>,
  ISerializationGenerated
{
  [Robust.Shared.IoC.Dependency]
  private readonly IEntityManager _entManager;
  [DataField("index", false, 1, false, false, null)]
  internal int GridIndex;
  [DataField(null, false, 1, false, false, null)]
  internal ushort ChunkSize = 16 /*0x10*/;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal readonly List<(GameTick tick, Vector2i indices)> ChunkDeletionHistory = new List<(GameTick, Vector2i)>();
  internal DynamicTree.Proxy MapProxy = DynamicTree.Proxy.Free;
  [DataField("chunks", false, 1, false, false, null)]
  internal Dictionary<Vector2i, MapChunk> Chunks = new Dictionary<Vector2i, MapChunk>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("canSplit", false, 1, false, false, null)]
  public bool CanSplit = true;

  private SharedMapSystem MapSystem => this._entManager.System<SharedMapSystem>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public int ChunkCount => this.Chunks.Count;

  [DataField("tileSize", false, 1, false, false, null)]
  public ushort TileSize { get; internal set; } = 1;

  public Vector2 TileSizeVector => new Vector2((float) this.TileSize, (float) this.TileSize);

  public Vector2 TileSizeHalfVector
  {
    get => new Vector2((float) this.TileSize / 2f, (float) this.TileSize / 2f);
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick LastTileModifiedTick { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Box2 LocalAABB { get; internal set; }

  [Obsolete("Use the MapSystem method")]
  public TileRef GetTileRef(EntityCoordinates coords)
  {
    return this.MapSystem.GetTileRef(this.Owner, this, coords);
  }

  [Obsolete("Use the MapSystem method")]
  public TileRef GetTileRef(Vector2i tileCoordinates)
  {
    return this.MapSystem.GetTileRef(this.Owner, this, tileCoordinates);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<TileRef> GetAllTiles(bool ignoreEmpty = true)
  {
    return this.MapSystem.GetAllTiles(this.Owner, this, ignoreEmpty);
  }

  [Obsolete("Use the MapSystem method")]
  public GridTileEnumerator GetAllTilesEnumerator(bool ignoreEmpty = true)
  {
    return this.MapSystem.GetAllTilesEnumerator(this.Owner, this, ignoreEmpty);
  }

  [Obsolete("Use the MapSystem method")]
  public void SetTile(EntityCoordinates coords, Tile tile)
  {
    this.MapSystem.SetTile(this.Owner, this, coords, tile);
  }

  [Obsolete("Use the MapSystem method")]
  public void SetTile(Vector2i gridIndices, Tile tile)
  {
    this.MapSystem.SetTile(this.Owner, this, gridIndices, tile);
  }

  [Obsolete("Use the MapSystem method")]
  public void SetTiles(List<(Vector2i GridIndices, Tile Tile)> tiles)
  {
    this.MapSystem.SetTiles(this.Owner, this, tiles);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<TileRef> GetTilesIntersecting(
    Box2 worldArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    return this.MapSystem.GetTilesIntersecting(this.Owner, this, worldArea, ignoreEmpty, predicate);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<TileRef> GetLocalTilesIntersecting(
    Box2 localArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    return this.MapSystem.GetLocalTilesIntersecting(this.Owner, this, localArea, ignoreEmpty, predicate);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<TileRef> GetTilesIntersecting(
    Circle worldArea,
    bool ignoreEmpty = true,
    Predicate<TileRef>? predicate = null)
  {
    return this.MapSystem.GetTilesIntersecting(this.Owner, this, worldArea, ignoreEmpty, predicate);
  }

  [Obsolete("Use the MapSystem method")]
  internal bool TryGetChunk(Vector2i chunkIndices, [NotNullWhen(true)] out MapChunk? chunk)
  {
    return this.MapSystem.TryGetChunk(this.Owner, this, chunkIndices, out chunk);
  }

  [Obsolete("Use the MapSystem method")]
  internal IReadOnlyDictionary<Vector2i, MapChunk> GetMapChunks()
  {
    return this.MapSystem.GetMapChunks(this.Owner, this);
  }

  [Obsolete("Use the MapSystem method")]
  internal ChunkEnumerator GetMapChunks(Box2Rotated worldArea)
  {
    return this.MapSystem.GetMapChunks(this.Owner, this, worldArea);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<EntityUid> GetAnchoredEntities(MapCoordinates coords)
  {
    return this.MapSystem.GetAnchoredEntities(this.Owner, this, coords);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<EntityUid> GetAnchoredEntities(Vector2i pos)
  {
    return this.MapSystem.GetAnchoredEntities(this.Owner, this, pos);
  }

  [Obsolete("Use the MapSystem method")]
  public AnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(Vector2i pos)
  {
    return this.MapSystem.GetAnchoredEntitiesEnumerator(this.Owner, this, pos);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2i TileIndicesFor(EntityCoordinates coords)
  {
    return this.MapSystem.TileIndicesFor(this.Owner, this, coords);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2i TileIndicesFor(MapCoordinates worldPos)
  {
    return this.MapSystem.TileIndicesFor(this.Owner, this, worldPos);
  }

  [Obsolete("Use the MapSystem method")]
  public IEnumerable<EntityUid> GetCellsInSquareArea(EntityCoordinates coords, int n)
  {
    return this.MapSystem.GetCellsInSquareArea(this.Owner, this, coords, n);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2 WorldToLocal(Vector2 posWorld)
  {
    return this.MapSystem.WorldToLocal(this.Owner, this, posWorld);
  }

  [Obsolete("Use the MapSystem method")]
  public EntityCoordinates MapToGrid(MapCoordinates posWorld)
  {
    return this.MapSystem.MapToGrid(this.Owner, posWorld);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2 LocalToWorld(Vector2 posLocal)
  {
    return this.MapSystem.LocalToWorld(this.Owner, this, posLocal);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2i WorldToTile(Vector2 posWorld)
  {
    return this.MapSystem.WorldToTile(this.Owner, this, posWorld);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2i LocalToTile(EntityCoordinates coordinates)
  {
    return this.MapSystem.LocalToTile(this.Owner, this, coordinates);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2i CoordinatesToTile(EntityCoordinates coords)
  {
    return this.MapSystem.CoordinatesToTile(this.Owner, this, coords);
  }

  [Obsolete("Use the MapSystem method")]
  public EntityCoordinates GridTileToLocal(Vector2i gridTile)
  {
    return this.MapSystem.GridTileToLocal(this.Owner, this, gridTile);
  }

  [Obsolete("Use the MapSystem method")]
  public Vector2 GridTileToWorldPos(Vector2i gridTile)
  {
    return this.MapSystem.GridTileToWorldPos(this.Owner, this, gridTile);
  }

  [Obsolete("Use the MapSystem method")]
  public bool TryGetTileRef(Vector2i indices, out TileRef tile)
  {
    return this.MapSystem.TryGetTileRef(this.Owner, this, indices, out tile);
  }

  [Obsolete("Use the MapSystem method")]
  public bool TryGetTileRef(EntityCoordinates coords, out TileRef tile)
  {
    return this.MapSystem.TryGetTileRef(this.Owner, this, coords, out tile);
  }

  public bool HasChunk(Vector2i indices) => this.Chunks.ContainsKey(indices);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MapGridComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MapGridComponent) target1;
    if (serialization.TryCustomCopy<MapGridComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.GridIndex, ref target2, hookCtx, false, context))
      target2 = this.GridIndex;
    target.GridIndex = target2;
    ushort target3 = 0;
    if (!serialization.TryCustomCopy<ushort>(this.ChunkSize, ref target3, hookCtx, false, context))
      target3 = this.ChunkSize;
    target.ChunkSize = target3;
    ushort target4 = 0;
    if (!serialization.TryCustomCopy<ushort>(this.TileSize, ref target4, hookCtx, false, context))
      target4 = this.TileSize;
    target.TileSize = target4;
    Dictionary<Vector2i, MapChunk> target5 = (Dictionary<Vector2i, MapChunk>) null;
    if (this.Chunks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Vector2i, MapChunk>>(this.Chunks, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<Vector2i, MapChunk>>(this.Chunks, hookCtx, context);
    target.Chunks = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanSplit, ref target6, hookCtx, false, context))
      target6 = this.CanSplit;
    target.CanSplit = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MapGridComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MapGridComponent target1 = (MapGridComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MapGridComponent target1 = (MapGridComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MapGridComponent target1 = (MapGridComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MapGridComponent Component.Instantiate() => new MapGridComponent();
}
