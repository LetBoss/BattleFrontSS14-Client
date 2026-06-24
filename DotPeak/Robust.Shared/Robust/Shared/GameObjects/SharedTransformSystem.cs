// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedTransformSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedTransformSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private readonly IGameTiming _gameTiming;
  [Robust.Shared.IoC.Dependency]
  private readonly IMapManager _mapManager;
  [Robust.Shared.IoC.Dependency]
  private readonly EntityLookupSystem _lookup;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedMapSystem _map;
  [Robust.Shared.IoC.Dependency]
  private readonly MetaDataSystem _metaData;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedPhysicsSystem _physics;
  [Robust.Shared.IoC.Dependency]
  private readonly INetManager _netMan;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedContainerSystem _container;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedGridTraversalSystem _traversal;
  private Robust.Shared.GameObjects.EntityQuery<MapComponent> _mapQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> _metaQuery;
  protected Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;

  internal void ReAnchor(
    EntityUid uid,
    TransformComponent xform,
    MapGridComponent oldGrid,
    MapGridComponent newGrid,
    Vector2i oldTilePos,
    Vector2i tilePos,
    EntityUid oldGridUid,
    EntityUid newGridUid,
    TransformComponent oldGridXform,
    TransformComponent newGridXform,
    Angle rotation)
  {
    this._map.RemoveFromSnapGridCell(oldGridUid, oldGrid, oldTilePos, uid);
    this._map.AddToSnapGridCell(newGridUid, newGrid, tilePos, uid);
    xform._anchored = false;
    oldGridXform._children.Remove(uid);
    newGridXform._children.Add(uid);
    xform._parent = newGridUid;
    xform._anchored = true;
    Vector2 localPosition = xform._localPosition;
    Angle localRotation = xform._localRotation;
    EntityUid? mapUid = xform.MapUid;
    xform._localPosition = Vector2i.op_Implicit(tilePos) + newGrid.TileSizeHalfVector;
    TransformComponent transformComponent = xform;
    transformComponent._localRotation = Angle.op_Addition(transformComponent._localRotation, rotation);
    MetaDataComponent meta = this.MetaData(uid);
    this.SetGridId((Entity<TransformComponent, MetaDataComponent>) (uid, xform, meta), new EntityUid?(newGridUid));
    this.RaiseMoveEvent((Entity<TransformComponent, MetaDataComponent>) (uid, xform, meta), oldGridUid, localPosition, localRotation, mapUid);
    this.Dirty(uid, (IComponent) xform, meta);
    ReAnchorEvent args = new ReAnchorEvent(uid, oldGridUid, newGridUid, tilePos, xform);
    this.RaiseLocalEvent<ReAnchorEvent>(uid, ref args);
  }

  [Obsolete("Use Entity<T> variant")]
  public bool AnchorEntity(
    EntityUid uid,
    TransformComponent xform,
    EntityUid gridUid,
    MapGridComponent grid,
    Vector2i tileIndices)
  {
    return this.AnchorEntity((Entity<TransformComponent>) (uid, xform), (Entity<MapGridComponent>) (gridUid, grid), tileIndices);
  }

  public bool AnchorEntity(
    Entity<TransformComponent> entity,
    Entity<MapGridComponent> grid,
    Vector2i tileIndices)
  {
    (EntityUid entityUid, TransformComponent transformComponent) = entity;
    if (!this._map.AddToSnapGridCell((EntityUid) grid, (MapGridComponent) grid, tileIndices, entityUid))
      return false;
    int num = entity.Comp._anchored ? 1 : 0;
    transformComponent._anchored = true;
    MetaDataComponent meta = this.MetaData(entityUid);
    this.Dirty<TransformComponent>(entity, meta);
    this._physics.TrySetBodyType(entityUid, BodyType.Static, xform: transformComponent);
    if (num == 0 && transformComponent.Running)
    {
      AnchorStateChangedEvent args = new AnchorStateChangedEvent(entityUid, transformComponent);
      this.RaiseLocalEvent<AnchorStateChangedEvent>(entityUid, ref args, true);
    }
    EntityCoordinates entityCoordinates = new EntityCoordinates((EntityUid) grid, this._map.GridTileToLocal((EntityUid) grid, (MapGridComponent) grid, tileIndices).Position);
    this.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (entityUid, transformComponent, meta), entityCoordinates, unanchor: false);
    return true;
  }

  [Obsolete("Use Entity<T> variants")]
  public bool AnchorEntity(EntityUid uid, TransformComponent xform, MapGridComponent grid)
  {
    Vector2i tileIndices = this._map.TileIndicesFor(grid.Owner, grid, xform.Coordinates);
    return this.AnchorEntity(uid, xform, grid.Owner, grid, tileIndices);
  }

  public bool AnchorEntity(EntityUid uid)
  {
    return this.AnchorEntity(uid, this.XformQuery.GetComponent(uid));
  }

  public bool AnchorEntity(EntityUid uid, TransformComponent xform)
  {
    return this.AnchorEntity((Entity<TransformComponent>) (uid, xform));
  }

  public bool AnchorEntity(Entity<TransformComponent> entity, Entity<MapGridComponent>? grid = null)
  {
    if (grid.HasValue)
    {
      EntityUid owner = grid.Value.Owner;
      EntityUid? gridUid = entity.Comp.GridUid;
      if ((gridUid.HasValue ? (owner != gridUid.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        this.Log.Error($"Tried to anchor entity {this.Name((EntityUid) entity)} to a grid ({grid.Value.Owner}) different from its GridUid ({entity.Comp.GridUid})");
        return false;
      }
    }
    if (!grid.HasValue)
    {
      MapGridComponent comp;
      if (!this.TryComp<MapGridComponent>(entity.Comp.GridUid, out comp))
        return false;
      grid = new Entity<MapGridComponent>?((Entity<MapGridComponent>) (entity.Comp.GridUid.Value, comp));
    }
    Vector2i tileIndices = this._map.TileIndicesFor((EntityUid) grid.Value, (MapGridComponent) grid.Value, entity.Comp.Coordinates);
    return this.AnchorEntity(entity, grid.Value, tileIndices);
  }

  public void Unanchor(EntityUid uid) => this.Unanchor(uid, this.XformQuery.GetComponent(uid));

  public void Unanchor(EntityUid uid, TransformComponent xform, bool setPhysics = true)
  {
    if (!xform._anchored)
      return;
    this.Dirty(uid, (IComponent) xform);
    xform._anchored = false;
    if (setPhysics)
      this._physics.TrySetBodyType(uid, BodyType.Dynamic, xform: xform);
    if (xform.LifeStage < ComponentLifeStage.Initialized)
      return;
    MapGridComponent component;
    if (this._gridQuery.TryGetComponent(xform.GridUid, out component))
    {
      Vector2i pos = this._map.TileIndicesFor(xform.GridUid.Value, component, xform.Coordinates);
      this._map.RemoveFromSnapGridCell(xform.GridUid.Value, component, pos, uid);
    }
    if (!xform.Running)
      return;
    AnchorStateChangedEvent args = new AnchorStateChangedEvent(uid, xform);
    this.RaiseLocalEvent<AnchorStateChangedEvent>(uid, ref args, true);
  }

  public bool ContainsEntity(EntityUid parent, Entity<TransformComponent?> child)
  {
    if (!this.Resolve(child.Owner, ref child.Comp) || !child.Comp.ParentUid.IsValid())
      return false;
    if (parent == child.Comp.ParentUid)
      return true;
    TransformComponent component;
    return this.XformQuery.TryGetComponent(child.Comp.ParentUid, out component) && this.ContainsEntity(parent, (Entity<TransformComponent>) (child.Comp.ParentUid, component));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool IsParentOf(TransformComponent parent, EntityUid child)
  {
    return parent._children.Contains(child);
  }

  internal (EntityUid?, MapId) InitializeMapUid(EntityUid uid, TransformComponent xform)
  {
    if (xform._mapIdInitialized)
      return (xform.MapUid, xform.MapID);
    if (xform.ParentUid.IsValid())
    {
      TransformComponent transformComponent1 = xform;
      TransformComponent transformComponent2 = xform;
      (EntityUid?, MapId) tuple = this.InitializeMapUid(xform.ParentUid, this.Transform(xform.ParentUid));
      EntityUid? nullable = tuple.Item1;
      transformComponent1.MapUid = nullable;
      transformComponent2.MapID = tuple.Item2;
    }
    else
    {
      MapComponent component;
      if (this._mapQuery.TryComp(uid, out component))
      {
        if (component.MapId == MapId.Nullspace)
        {
          this.Log.Error("Transform is initialising before map ids have been assigned?");
          this._map.AssignMapId((Entity<MapComponent>) (uid, component));
        }
        xform.MapUid = new EntityUid?(uid);
        xform.MapID = component.MapId;
      }
      else
      {
        xform.MapUid = new EntityUid?();
        xform.MapID = MapId.Nullspace;
      }
    }
    xform._mapIdInitialized = true;
    return (xform.MapUid, xform.MapID);
  }

  private void OnCompInit(EntityUid uid, TransformComponent component, ComponentInit args)
  {
    this.InitializeMapUid(uid, component);
    if (component.ParentUid.IsValid())
    {
      TransformComponent component1 = this.XformQuery.GetComponent(component.ParentUid);
      if (component1.LifeStage > ComponentLifeStage.Running || this.LifeStage(component.ParentUid) > EntityLifeStage.MapInitialized)
      {
        this.Log.Error($"Attempted to re-parent to a terminating object. Entity: {this.ToPrettyString((Entity<MetaDataComponent>) component.ParentUid)}, new parent: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
        this.Del(new EntityUid?(uid));
      }
      component1._children.Add(uid);
    }
    this.InitializeGridUid(uid, component);
    component.MatricesDirty = true;
    if (!component._anchored)
      return;
    Entity<MapGridComponent>? grid = new Entity<MapGridComponent>?();
    EntityUid? gridUid = component.GridUid;
    EntityUid parentUid = component.ParentUid;
    MapGridComponent mapGridComponent;
    if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == parentUid ? 1 : 0) : 0) != 0 && this.TryComp<MapGridComponent>(component.ParentUid, out mapGridComponent))
    {
      grid = new Entity<MapGridComponent>?((Entity<MapGridComponent>) (component.ParentUid, mapGridComponent));
    }
    else
    {
      EntityUid uid1;
      if (this._mapManager.TryFindGridAt(new MapCoordinates(this.GetWorldPosition(component), component.MapID), out uid1, out mapGridComponent))
        grid = new Entity<MapGridComponent>?((Entity<MapGridComponent>) (uid1, mapGridComponent));
    }
    if (!grid.HasValue)
    {
      this.Unanchor(uid, component);
    }
    else
    {
      if (this.AnchorEntity((Entity<TransformComponent>) (uid, component), grid))
        return;
      component._anchored = false;
    }
  }

  internal void InitializeGridUid(EntityUid uid, TransformComponent xform)
  {
    if (xform._gridInitialized)
      return;
    if (this._gridQuery.HasComponent(uid))
    {
      xform._gridUid = new EntityUid?(uid);
      xform._gridInitialized = true;
    }
    else
    {
      if (xform.LifeStage >= ComponentLifeStage.Initializing)
        xform._gridInitialized = true;
      if (!xform._parent.IsValid())
        return;
      TransformComponent component = this.XformQuery.GetComponent(xform._parent);
      this.InitializeGridUid(xform._parent, component);
      xform._gridUid = component._gridUid;
    }
  }

  private void OnCompStartup(EntityUid uid, TransformComponent xform, ComponentStartup args)
  {
    if (xform.Anchored)
    {
      AnchorStateChangedEvent args1 = new AnchorStateChangedEvent(uid, xform);
      this.RaiseLocalEvent<AnchorStateChangedEvent>(uid, ref args1, true);
    }
    EntParentChangedMessage args2 = new EntParentChangedMessage(uid, new EntityUid?(), new EntityUid?(), xform);
    this.RaiseLocalEvent<EntParentChangedMessage>(uid, ref args2, true);
    TransformStartupEvent args3 = new TransformStartupEvent((Entity<TransformComponent>) (uid, xform));
    this.RaiseLocalEvent<TransformStartupEvent>(uid, ref args3, true);
  }

  public void SetGridId(
    EntityUid uid,
    TransformComponent xform,
    EntityUid? gridId,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent>? xformQuery = null)
  {
    this.SetGridId((Entity<TransformComponent, MetaDataComponent>) (uid, xform, this.MetaData(uid)), gridId);
  }

  public void SetGridId(Entity<TransformComponent, MetaDataComponent?> ent, EntityUid? gridId)
  {
    if (!ent.Comp1._gridInitialized)
      return;
    EntityUid? gridUid1 = ent.Comp1._gridUid;
    EntityUid? nullable = gridId;
    if ((gridUid1.HasValue == nullable.HasValue ? (gridUid1.HasValue ? (gridUid1.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    EntityUid? gridUid2 = ent.Comp1.GridUid;
    EntityUid owner = ent.Owner;
    if ((gridUid2.HasValue ? (gridUid2.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
      return;
    this._metaQuery.ResolveInternal(ent.Owner, ref ent.Comp2);
    if ((ent.Comp2.Flags & MetaDataFlags.ExtraTransformEvents) != MetaDataFlags.None)
    {
      GridUidChangedEvent args = new GridUidChangedEvent((Entity<TransformComponent, MetaDataComponent>) (ent.Owner, ent.Comp1, ent.Comp2), ent.Comp1._gridUid);
      ent.Comp1._gridUid = gridId;
      this.RaiseLocalEvent<GridUidChangedEvent>((EntityUid) ent, ref args);
    }
    ent.Comp1._gridUid = gridId;
    foreach (EntityUid child in ent.Comp1._children)
      this.SetGridId((Entity<TransformComponent, MetaDataComponent>) (child, this.XformQuery.GetComponent(child), (MetaDataComponent) null), gridId);
  }

  [Obsolete("use override with EntityUid")]
  public void SetLocalPosition(TransformComponent xform, Vector2 value)
  {
    this.SetLocalPosition(xform.Owner, value, xform);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public virtual void SetLocalPosition(EntityUid uid, Vector2 value, TransformComponent? xform = null)
  {
    this.SetLocalPositionNoLerp(uid, value, xform);
  }

  [Obsolete("use override with EntityUid")]
  public void SetLocalPositionNoLerp(TransformComponent xform, Vector2 value)
  {
    this.SetLocalPositionNoLerp(xform.Owner, value, xform);
  }

  public void SetLocalPositionNoLerp(EntityUid uid, Vector2 value, TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return;
    xform.LocalPosition = value;
  }

  public void SetLocalRotationNoLerp(EntityUid uid, Angle value, TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return;
    xform.LocalRotation = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public virtual void SetLocalRotation(EntityUid uid, Angle value, TransformComponent? xform = null)
  {
    this.SetLocalRotationNoLerp(uid, value, xform);
  }

  [Obsolete("use override with EntityUid")]
  public void SetLocalRotation(TransformComponent xform, Angle value)
  {
    this.SetLocalRotation(xform.Owner, value, xform);
  }

  public void SetCoordinates(EntityUid uid, EntityCoordinates value)
  {
    this.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (uid, this.Transform(uid), this.MetaData(uid)), value);
  }

  public void SetCoordinates(
    Entity<TransformComponent, MetaDataComponent> entity,
    EntityCoordinates value,
    Angle? rotation = null,
    bool unanchor = true,
    TransformComponent? newParent = null,
    TransformComponent? oldParent = null)
  {
    (EntityUid entityUid, TransformComponent transformComponent1, MetaDataComponent metaDataComponent) = entity;
    if (transformComponent1.ParentUid == value.EntityId && Vector2Helpers.EqualsApprox(transformComponent1._localPosition, value.Position) && (!rotation.HasValue || MathHelper.CloseTo(rotation.Value.Theta, transformComponent1._localRotation.Theta, 1E-07)))
      return;
    if (transformComponent1.Anchored & unanchor)
      this.Unanchor(entityUid, transformComponent1);
    if (value.EntityId != transformComponent1.ParentUid && value.EntityId.IsValid())
    {
      if (metaDataComponent.EntityLifeStage >= EntityLifeStage.Terminating)
      {
        this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) entityUid)} is attempting to move while terminating. New parent: {this.ToPrettyString((Entity<MetaDataComponent>) value.EntityId)}. Trace: {Environment.StackTrace}");
        return;
      }
      if (this.TerminatingOrDeleted(value.EntityId))
      {
        this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) entityUid)} is attempting to attach itself to a terminating entity {this.ToPrettyString((Entity<MetaDataComponent>) value.EntityId)}. Trace: {Environment.StackTrace}");
        return;
      }
    }
    EntityUid parent = transformComponent1._parent;
    Vector2 localPosition = transformComponent1._localPosition;
    Angle localRotation = transformComponent1._localRotation;
    EntityUid? mapUid1 = transformComponent1.MapUid;
    this.Dirty(entityUid, (IComponent) transformComponent1, metaDataComponent);
    transformComponent1.MatricesDirty = true;
    transformComponent1._localPosition = value.Position;
    if (rotation.HasValue && !transformComponent1.NoLocalRotation)
      transformComponent1._localRotation = rotation.Value;
    if (value.EntityId != transformComponent1._parent)
    {
      if (value.EntityId == entityUid)
      {
        this.DetachEntity(entityUid, transformComponent1);
        if (this._netMan.IsServer || this.IsClientSide(entityUid))
          this.QueueDel(new EntityUid?(entityUid));
        throw new InvalidOperationException($"Attempted to parent an entity to itself: {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)}");
      }
      EntityUid? nullable;
      if (value.EntityId.IsValid())
      {
        if (!this.XformQuery.Resolve(value.EntityId, ref newParent, false))
        {
          this.DetachEntity(entityUid, transformComponent1);
          if (this._netMan.IsServer || this.IsClientSide(entityUid))
            this.QueueDel(new EntityUid?(entityUid));
          throw new InvalidOperationException($"Attempted to parent entity {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)} to non-existent entity {value.EntityId}");
        }
        if (newParent.LifeStage >= ComponentLifeStage.Stopping || this.LifeStage(value.EntityId) >= EntityLifeStage.Terminating)
        {
          this.DetachEntity(entityUid, transformComponent1);
          if (this._netMan.IsServer || this.IsClientSide(entityUid))
            this.QueueDel(new EntityUid?(entityUid));
          throw new InvalidOperationException($"Attempted to re-parent to a terminating object. Entity: {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)}, new parent: {this.ToPrettyString((Entity<MetaDataComponent>) value.EntityId)}");
        }
        this.InitializeMapUid(value.EntityId, newParent);
        EntityUid? mapUid2 = transformComponent1.MapUid;
        nullable = newParent.MapUid;
        if ((mapUid2.HasValue == nullable.HasValue ? (mapUid2.HasValue ? (mapUid2.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          EntityUid uid = value.EntityId;
          for (TransformComponent xform = newParent; xform.ParentUid.IsValid(); xform = this.XformQuery.GetComponent(uid))
          {
            if (xform.ParentUid == entityUid)
            {
              if (!this._gameTiming.ApplyingState)
                throw new InvalidOperationException($"Attempted to parent an entity to one of its descendants! {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)}. new parent: {this.ToPrettyString((Entity<MetaDataComponent>) value.EntityId)}");
              this.Log.Warning($"Encountered circular transform hierarchy while applying state for entity: {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)}. Detaching child to null: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
              this.DetachEntity(uid, xform);
              break;
            }
            uid = xform.ParentUid;
          }
        }
      }
      if (transformComponent1._parent.IsValid())
        this.XformQuery.Resolve(transformComponent1._parent, ref oldParent);
      oldParent?._children.Remove(entityUid);
      newParent?._children.Add(entityUid);
      transformComponent1._parent = value.EntityId;
      if (newParent != null)
      {
        this.ChangeMapId(entity, newParent.MapID);
        if (!transformComponent1._gridInitialized)
        {
          this.InitializeGridUid(entityUid, transformComponent1);
        }
        else
        {
          if (!newParent._gridInitialized)
            this.InitializeGridUid(value.EntityId, newParent);
          this.SetGridId(entity, newParent.GridUid);
        }
      }
      else
      {
        this.ChangeMapId(entity, MapId.Nullspace);
        if (!transformComponent1._gridInitialized)
        {
          this.InitializeGridUid(entityUid, transformComponent1);
        }
        else
        {
          Entity<TransformComponent, MetaDataComponent> ent = entity;
          nullable = new EntityUid?();
          EntityUid? gridId = nullable;
          this.SetGridId(ent, gridId);
        }
      }
      if (transformComponent1.Initialized && !rotation.HasValue && oldParent != null && newParent != null && !transformComponent1.NoLocalRotation)
      {
        TransformComponent transformComponent2 = transformComponent1;
        transformComponent2._localRotation = Angle.op_Addition(transformComponent2._localRotation, Angle.op_Subtraction(this.GetWorldRotation(oldParent), this.GetWorldRotation(newParent)));
      }
    }
    if (!transformComponent1.Initialized)
      return;
    this.RaiseMoveEvent(entity, parent, localPosition, localRotation, mapUid1);
  }

  public void SetCoordinates(
    EntityUid uid,
    TransformComponent xform,
    EntityCoordinates value,
    Angle? rotation = null,
    bool unanchor = true,
    TransformComponent? newParent = null,
    TransformComponent? oldParent = null)
  {
    this.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (uid, xform, this._metaQuery.GetComponent(uid)), value, rotation, unanchor, newParent, oldParent);
  }

  private void ChangeMapId(Entity<TransformComponent, MetaDataComponent> ent, MapId newMapId)
  {
    if (newMapId == ent.Comp1.MapID)
      return;
    EntityUid? newMap = newMapId == MapId.Nullspace ? new EntityUid?() : new EntityUid?(this._map.GetMap(newMapId));
    bool? paused = new bool?();
    if (!this._gameTiming.ApplyingState)
    {
      paused = new bool?(this._map.IsPaused(newMapId));
      this._metaData.SetEntityPaused(ent.Owner, paused.Value, ent.Comp2);
    }
    this.ChangeMapIdRecursive(ent, newMap, newMapId, paused);
  }

  private void ChangeMapIdRecursive(
    Entity<TransformComponent, MetaDataComponent> ent,
    EntityUid? newMap,
    MapId newMapId,
    bool? paused)
  {
    if (paused.HasValue)
    {
      bool valueOrDefault = paused.GetValueOrDefault();
      this._metaData.SetEntityPaused(ent.Owner, valueOrDefault, ent.Comp2);
    }
    if ((ent.Comp2.Flags & MetaDataFlags.ExtraTransformEvents) != MetaDataFlags.None)
    {
      MapUidChangedEvent args = new MapUidChangedEvent(ent, ent.Comp1.MapUid, ent.Comp1.MapID);
      ent.Comp1.MapUid = newMap;
      ent.Comp1.MapID = newMapId;
      this.RaiseLocalEvent<MapUidChangedEvent>(ent.Owner, ref args);
    }
    ent.Comp1.MapUid = newMap;
    ent.Comp1.MapID = newMapId;
    foreach (EntityUid child in ent.Comp1._children)
      this.ChangeMapIdRecursive(new Entity<TransformComponent, MetaDataComponent>(child, this.Transform(child), this.MetaData(child)), newMap, newMapId, paused);
  }

  public void ReparentChildren(EntityUid oldUid, EntityUid uid)
  {
    this.ReparentChildren(oldUid, uid, this.XformQuery);
  }

  public void ReparentChildren(
    EntityUid oldUid,
    EntityUid uid,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    if (oldUid == uid)
    {
      this.Log.Error($"Tried to reparent entities from the same entity, {this.ToPrettyString((Entity<MetaDataComponent>) oldUid)}");
    }
    else
    {
      TransformComponent component1 = xformQuery.GetComponent(oldUid);
      TransformComponent component2 = xformQuery.GetComponent(uid);
      foreach (EntityUid uid1 in component1._children.ToArray<EntityUid>())
        this.SetParent(uid1, xformQuery.GetComponent(uid1), uid, xformQuery, component2);
    }
  }

  public TransformComponent? GetParent(EntityUid uid)
  {
    return this.GetParent(this.XformQuery.GetComponent(uid));
  }

  public TransformComponent? GetParent(TransformComponent xform)
  {
    return !xform.ParentUid.IsValid() ? (TransformComponent) null : this.XformQuery.GetComponent(xform.ParentUid);
  }

  public EntityUid GetParentUid(EntityUid uid) => this.XformQuery.GetComponent(uid).ParentUid;

  public void SetParent(EntityUid uid, EntityUid parent)
  {
    this.SetParent(uid, this.XformQuery.GetComponent(uid), parent, this.XformQuery);
  }

  public void SetParent(
    EntityUid uid,
    TransformComponent xform,
    EntityUid parent,
    TransformComponent? parentXform = null)
  {
    this.SetParent(uid, xform, parent, this.XformQuery, parentXform);
  }

  public void SetParent(
    EntityUid uid,
    TransformComponent xform,
    EntityUid parent,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery,
    TransformComponent? parentXform = null)
  {
    if (xform.ParentUid == parent)
      return;
    if (!parent.IsValid())
    {
      this.DetachEntity(uid, xform);
    }
    else
    {
      if (!xformQuery.Resolve(parent, ref parentXform))
        return;
      (Vector2 _, Angle WorldRotation1, Matrix3x2 matrix3x2) = this.GetWorldPositionRotationInvMatrix(parentXform, xformQuery);
      (Vector2 vector2, Angle WorldRotation2) = this.GetWorldPositionRotation(xform, xformQuery);
      Vector2 position = Vector2.Transform(vector2, matrix3x2);
      Angle angle1 = WorldRotation1;
      Angle angle2 = Angle.op_Subtraction(WorldRotation2, angle1);
      this.SetCoordinates(uid, xform, new EntityCoordinates(parent, position), new Angle?(angle2), newParent: parentXform);
    }
  }

  public virtual void ActivateLerp(EntityUid uid, TransformComponent xform)
  {
  }

  internal void OnGetState(EntityUid uid, TransformComponent component, ref ComponentGetState args)
  {
    NetEntity netEntity = this.GetNetEntity(component.ParentUid);
    args.State = (IComponentState) new TransformComponentState(component.LocalPosition, component.LocalRotation, netEntity, component.NoLocalRotation, component.Anchored);
  }

  internal void OnHandleState(
    EntityUid uid,
    TransformComponent xform,
    ref ComponentHandleState args)
  {
    if (args.Current is TransformComponentState current)
    {
      EntityUid entityId = this.EnsureEntity<TransformComponent>(current.ParentID, uid);
      int num1 = xform.Anchored ? 1 : 0;
      if (Vector2Helpers.EqualsApprox(xform.LocalPosition, current.LocalPosition))
      {
        Angle localRotation = xform.LocalRotation;
        if (((Angle) ref localRotation).EqualsApprox(current.Rotation) && !(xform.ParentUid != entityId))
        {
          xform.Anchored = current.Anchored;
          goto label_10;
        }
      }
      MapGridComponent comp1;
      if (xform.Anchored && this.TryComp<MapGridComponent>(xform.ParentUid, out comp1))
      {
        Vector2i pos = this._map.TileIndicesFor(xform.ParentUid, comp1, xform.Coordinates);
        this._map.RemoveFromSnapGridCell(xform.ParentUid, comp1, pos, uid);
      }
      xform._anchored |= current.Anchored;
      this.SetCoordinates(uid, xform, new EntityCoordinates(entityId, current.LocalPosition), new Angle?(current.Rotation), false);
      xform._anchored = current.Anchored;
      if (xform._anchored && xform.Initialized)
      {
        EntityUid parentUid = xform.ParentUid;
        EntityUid? gridUid1 = xform.GridUid;
        MapGridComponent comp2;
        if ((gridUid1.HasValue ? (parentUid == gridUid1.GetValueOrDefault() ? 1 : 0) : 0) != 0 && this.TryComp<MapGridComponent>(xform.GridUid, out comp2))
        {
          SharedMapSystem map1 = this._map;
          EntityUid? gridUid2 = xform.GridUid;
          EntityUid uid1 = gridUid2.Value;
          MapGridComponent grid1 = comp2;
          EntityCoordinates coordinates = xform.Coordinates;
          Vector2i vector2i = map1.TileIndicesFor(uid1, grid1, coordinates);
          SharedMapSystem map2 = this._map;
          gridUid2 = xform.GridUid;
          EntityUid gridUid3 = gridUid2.Value;
          MapGridComponent grid2 = comp2;
          Vector2i pos = vector2i;
          EntityUid euid = uid;
          map2.AddToSnapGridCell(gridUid3, grid2, pos, euid);
        }
        else
          xform._anchored = false;
      }
label_10:
      int num2 = current.Anchored ? 1 : 0;
      if (num1 != num2 && xform.Initialized)
      {
        AnchorStateChangedEvent args1 = new AnchorStateChangedEvent(uid, xform);
        this.RaiseLocalEvent<AnchorStateChangedEvent>(uid, ref args1, true);
      }
      xform._noLocalRotation = current.NoLocalRotation;
    }
    if (!(args.Next is TransformComponentState next) || !(next.ParentID == this.GetNetEntity(xform.ParentUid)))
      return;
    xform.NextPosition = new Vector2?(next.LocalPosition);
    xform.NextRotation = new Angle?(next.Rotation);
    this.ActivateLerp(uid, xform);
  }

  public Matrix3x2 GetWorldMatrix(EntityUid uid)
  {
    return this.GetWorldMatrix(this.XformQuery.GetComponent(uid), this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Matrix3x2 GetWorldMatrix(TransformComponent component)
  {
    return this.GetWorldMatrix(component, this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Matrix3x2 GetWorldMatrix(EntityUid uid, Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetWorldMatrix(xformQuery.GetComponent(uid), xformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Matrix3x2 GetWorldMatrix(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    (Vector2 WorldPosition, Angle WorldRotation) = this.GetWorldPositionRotation(component, xformQuery);
    return Matrix3Helpers.CreateTransform(ref WorldPosition, ref WorldRotation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 GetWorldPosition(EntityUid uid)
  {
    return this.GetWorldPosition(this.XformQuery.GetComponent(uid));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 GetWorldPosition(TransformComponent component)
  {
    Vector2 worldPosition = component._localPosition;
    while (true)
    {
      EntityUid parentUid = component.ParentUid;
      EntityUid? mapUid = component.MapUid;
      if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        parentUid = component.ParentUid;
        if (parentUid.IsValid())
        {
          component = this.XformQuery.GetComponent(component.ParentUid);
          worldPosition = ((Angle) ref component._localRotation).RotateVec(ref worldPosition) + component._localPosition;
        }
        else
          break;
      }
      else
        break;
    }
    return worldPosition;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 GetWorldPosition(EntityUid uid, Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetWorldPosition(xformQuery.GetComponent(uid));
  }

  public Vector2 GetWorldPosition(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetWorldPosition(component);
  }

  public MapCoordinates GetMapCoordinates(EntityUid entity, TransformComponent? xform = null)
  {
    return !this.XformQuery.Resolve(entity, ref xform) ? MapCoordinates.Nullspace : this.GetMapCoordinates(xform);
  }

  public MapCoordinates GetMapCoordinates(TransformComponent xform)
  {
    return new MapCoordinates(this.GetWorldPosition(xform), xform.MapID);
  }

  public MapCoordinates GetMapCoordinates(Entity<TransformComponent> entity)
  {
    return this.GetMapCoordinates(entity.Comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetMapCoordinates(EntityUid entity, MapCoordinates coordinates)
  {
    TransformComponent component = this.XformQuery.GetComponent(entity);
    this.SetMapCoordinates((Entity<TransformComponent>) (entity, component), coordinates);
  }

  public void SetMapCoordinates(Entity<TransformComponent> entity, MapCoordinates coordinates)
  {
    EntityUid map = this._map.GetMap(coordinates.MapId);
    EntityUid uid;
    if (!this._gridQuery.HasComponent((EntityUid) entity) && this._mapManager.TryFindGridAt(map, coordinates.Position, out uid, out MapGridComponent _))
    {
      Matrix3x2 invWorldMatrix = this.GetInvWorldMatrix(uid);
      this.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (entity.Owner, entity.Comp, this.MetaData(entity.Owner)), new EntityCoordinates(uid, Vector2.Transform(coordinates.Position, invWorldMatrix)));
    }
    else
      this.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (entity.Owner, entity.Comp, this.MetaData(entity.Owner)), new EntityCoordinates(map, coordinates.Position));
  }

  public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation(EntityUid uid)
  {
    return this.GetWorldPositionRotation(this.XformQuery.GetComponent(uid));
  }

  public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation(
    TransformComponent component)
  {
    Vector2 vector2 = component._localPosition;
    Angle angle = component._localRotation;
    while (true)
    {
      EntityUid parentUid = component.ParentUid;
      EntityUid? mapUid = component.MapUid;
      if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && component.ParentUid.IsValid())
      {
        component = this.XformQuery.GetComponent(component.ParentUid);
        vector2 = ((Angle) ref component._localRotation).RotateVec(ref vector2) + component._localPosition;
        angle = Angle.op_Addition(angle, component._localRotation);
      }
      else
        break;
    }
    return (vector2, angle);
  }

  public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetWorldPositionRotation(component);
  }

  [Obsolete("Use variant without entity query")]
  public (Vector2 Position, Angle Rotation) GetRelativePositionRotation(
    TransformComponent component,
    EntityUid relative,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> query)
  {
    return this.GetRelativePositionRotation(component, relative);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 Position, Angle Rotation) GetRelativePositionRotation(
    TransformComponent component,
    EntityUid relative)
  {
    Angle angle = component._localRotation;
    Vector2 position = component._localPosition;
    TransformComponent comp = component;
    while (comp.ParentUid != relative)
    {
      if (comp.ParentUid.IsValid() && this.TryComp(comp.ParentUid, out comp))
      {
        angle = Angle.op_Addition(angle, comp._localRotation);
        position = ((Angle) ref comp._localRotation).RotateVec(ref position) + comp._localPosition;
      }
      else
      {
        this.Log.Warning($"Target entity ({this.ToPrettyString((Entity<MetaDataComponent>) relative)}) not in transform hierarchy while calling {nameof (GetRelativePositionRotation)}.");
        TransformComponent component1 = this.Transform(relative);
        position = Vector2.Transform(position, this.GetInvWorldMatrix(component1));
        angle = Angle.op_Subtraction(angle, this.GetWorldRotation(component1));
        break;
      }
    }
    return (position, angle);
  }

  [Obsolete("Use variant without entity query")]
  public Vector2 GetRelativePosition(
    TransformComponent component,
    EntityUid relative,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> query)
  {
    return this.GetRelativePosition(component, relative);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 GetRelativePosition(TransformComponent component, EntityUid relative)
  {
    Vector2 position = component._localPosition;
    TransformComponent comp = component;
    while (comp.ParentUid != relative)
    {
      if (comp.ParentUid.IsValid() && this.TryComp(comp.ParentUid, out comp))
      {
        position = ((Angle) ref comp._localRotation).RotateVec(ref position) + comp._localPosition;
      }
      else
      {
        this.Log.Warning($"Target entity ({this.ToPrettyString((Entity<MetaDataComponent>) relative)}) not in transform hierarchy while calling {"GetRelativePositionRotation"}.");
        TransformComponent component1 = this.Transform(relative);
        position = Vector2.Transform(position, this.GetInvWorldMatrix(component1));
        break;
      }
    }
    return position;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldPosition(EntityUid uid, Vector2 worldPos)
  {
    TransformComponent component = this.XformQuery.GetComponent(uid);
    this.SetWorldPosition((Entity<TransformComponent>) (uid, component), worldPos);
  }

  [Obsolete("Use overload that takes Entity<T> instead")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldPosition(TransformComponent component, Vector2 worldPos)
  {
    this.SetWorldPosition((Entity<TransformComponent>) (component.Owner, component), worldPos);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldPosition(Entity<TransformComponent> entity, Vector2 worldPos)
  {
    this.SetWorldPositionRotationInternal(entity.Owner, worldPos, component: entity.Comp);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Angle GetWorldRotation(EntityUid uid)
  {
    return this.GetWorldRotation(this.XformQuery.GetComponent(uid), this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Angle GetWorldRotation(TransformComponent component)
  {
    return this.GetWorldRotation(component, this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Angle GetWorldRotation(EntityUid uid, Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetWorldRotation(xformQuery.GetComponent(uid), xformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Angle GetWorldRotation(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    Angle worldRotation = component._localRotation;
    while (true)
    {
      EntityUid parentUid = component.ParentUid;
      EntityUid? mapUid = component.MapUid;
      if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        parentUid = component.ParentUid;
        if (parentUid.IsValid())
        {
          component = xformQuery.GetComponent(component.ParentUid);
          worldRotation = Angle.op_Addition(worldRotation, component._localRotation);
        }
        else
          break;
      }
      else
        break;
    }
    return worldRotation;
  }

  public void SetWorldRotationNoLerp(Entity<TransformComponent?> entity, Angle angle)
  {
    if (!this.XformQuery.Resolve(entity.Owner, ref entity.Comp))
      return;
    Angle worldRotation = this.GetWorldRotation(entity.Comp);
    Angle angle1 = Angle.op_Subtraction(angle, worldRotation);
    this.SetLocalRotationNoLerp((EntityUid) entity, Angle.op_Addition(entity.Comp.LocalRotation, angle1));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldRotation(EntityUid uid, Angle angle)
  {
    this.SetWorldRotation(this.Transform(uid), angle);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldRotation(TransformComponent component, Angle angle)
  {
    Angle worldRotation = this.GetWorldRotation(component);
    Angle angle1 = Angle.op_Subtraction(angle, worldRotation);
    this.SetLocalRotation(component, Angle.op_Addition(component.LocalRotation, angle1));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldRotation(
    EntityUid uid,
    Angle angle,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    this.SetWorldRotation(xformQuery.GetComponent(uid), angle, xformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldRotation(
    TransformComponent component,
    Angle angle,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    Angle worldRotation = this.GetWorldRotation(component, xformQuery);
    Angle angle1 = Angle.op_Subtraction(angle, worldRotation);
    this.SetLocalRotation(component, Angle.op_Addition(component.LocalRotation, angle1));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetWorldPositionRotation(
    EntityUid uid,
    Vector2 worldPos,
    Angle worldRot,
    TransformComponent? component = null)
  {
    this.SetWorldPositionRotationInternal(uid, worldPos, new Angle?(worldRot), component);
  }

  private void SetWorldPositionRotationInternal(
    EntityUid uid,
    Vector2 worldPos,
    Angle? worldRot = null,
    TransformComponent? component = null)
  {
    if (!this.XformQuery.Resolve(uid, ref component) || !component._parent.IsValid() || !component.MapUid.HasValue)
      return;
    EntityUid? nullable1 = component.GridUid;
    EntityUid entityUid1 = uid;
    if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() != entityUid1 ? 1 : 0) : 1) != 0)
    {
      IMapManager mapManager = this._mapManager;
      nullable1 = component.MapUid;
      EntityUid mapEnt = nullable1.Value;
      Vector2 worldPos1 = worldPos;
      EntityUid entityUid2;
      ref EntityUid local1 = ref entityUid2;
      MapGridComponent mapGridComponent;
      ref MapGridComponent local2 = ref mapGridComponent;
      if (mapManager.TryFindGridAt(mapEnt, worldPos1, out local1, out local2))
      {
        TransformComponent component1 = this.XformQuery.GetComponent(entityUid2);
        Matrix3x2 invLocalMatrix = component1.InvLocalMatrix;
        Angle localRotation = component1.LocalRotation;
        Angle? nullable2 = worldRot;
        Angle angle = localRotation;
        Angle? rotation = nullable2.HasValue ? new Angle?(Angle.op_Subtraction(nullable2.GetValueOrDefault(), angle)) : new Angle?();
        this.SetCoordinates(uid, component, new EntityCoordinates(entityUid2, Vector2.Transform(worldPos, invLocalMatrix)), rotation);
        return;
      }
    }
    EntityUid uid1 = uid;
    TransformComponent xform = component;
    nullable1 = component.MapUid;
    EntityCoordinates entityCoordinates = new EntityCoordinates(nullable1.Value, worldPos);
    Angle? rotation1 = worldRot;
    this.SetCoordinates(uid1, xform, entityCoordinates, rotation1);
  }

  [Obsolete("Use override with EntityUid")]
  public void SetLocalPositionRotation(TransformComponent xform, Vector2 pos, Angle rot)
  {
    this.SetLocalPositionRotation(xform.Owner, pos, rot, xform);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public virtual void SetLocalPositionRotation(
    EntityUid uid,
    Vector2 pos,
    Angle rot,
    TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform) || !xform._parent.IsValid())
      return;
    if (Vector2Helpers.EqualsApprox(xform._localPosition, pos))
    {
      Angle localRotation = xform.LocalRotation;
      if (((Angle) ref localRotation).EqualsApprox(rot))
        return;
    }
    EntityUid parent = xform._parent;
    Vector2 localPosition = xform._localPosition;
    Angle localRotation1 = xform.LocalRotation;
    if (!xform.Anchored)
      xform._localPosition = pos;
    if (!xform.NoLocalRotation)
      xform._localRotation = rot;
    MetaDataComponent meta = this.MetaData(uid);
    this.Dirty(uid, (IComponent) xform, meta);
    xform.MatricesDirty = true;
    if (!xform.Initialized)
      return;
    this.RaiseMoveEvent((Entity<TransformComponent, MetaDataComponent>) (uid, xform, meta), parent, localPosition, localRotation1, xform.MapUid, !localPosition.Equals(pos));
  }

  public Matrix3x2 GetInvWorldMatrix(EntityUid uid)
  {
    return this.GetInvWorldMatrix(this.XformQuery.GetComponent(uid), this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Matrix3x2 GetInvWorldMatrix(TransformComponent component)
  {
    return this.GetInvWorldMatrix(component, this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Matrix3x2 GetInvWorldMatrix(EntityUid uid, Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetInvWorldMatrix(xformQuery.GetComponent(uid), xformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Matrix3x2 GetInvWorldMatrix(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    (Vector2 WorldPosition, Angle WorldRotation) = this.GetWorldPositionRotation(component, xformQuery);
    return Matrix3Helpers.CreateInverseTransform(ref WorldPosition, ref WorldRotation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(
    EntityUid uid)
  {
    return this.GetWorldPositionRotationMatrix(this.XformQuery.GetComponent(uid), this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(
    TransformComponent xform)
  {
    return this.GetWorldPositionRotationMatrix(xform, this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(
    EntityUid uid,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xforms)
  {
    return this.GetWorldPositionRotationMatrix(xforms.GetComponent(uid), xforms);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xforms)
  {
    (Vector2 WorldPosition, Angle WorldRotation) = this.GetWorldPositionRotation(component, xforms);
    return (WorldPosition, WorldRotation, Matrix3Helpers.CreateTransform(ref WorldPosition, ref WorldRotation));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(
    EntityUid uid)
  {
    return this.GetWorldPositionRotationInvMatrix(this.XformQuery.GetComponent(uid));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(
    TransformComponent xform)
  {
    return this.GetWorldPositionRotationInvMatrix(xform, this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(
    EntityUid uid,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xforms)
  {
    return this.GetWorldPositionRotationInvMatrix(xforms.GetComponent(uid), xforms);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xforms)
  {
    (Vector2 WorldPosition, Angle WorldRotation) = this.GetWorldPositionRotation(component, xforms);
    return (WorldPosition, WorldRotation, Matrix3Helpers.CreateInverseTransform(ref WorldPosition, ref WorldRotation));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(
    EntityUid uid)
  {
    return this.GetWorldPositionRotationMatrixWithInv(this.XformQuery.GetComponent(uid), this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(
    TransformComponent xform)
  {
    return this.GetWorldPositionRotationMatrixWithInv(xform, this.XformQuery);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(
    EntityUid uid,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xforms)
  {
    return this.GetWorldPositionRotationMatrixWithInv(xforms.GetComponent(uid), xforms);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(
    TransformComponent component,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xforms)
  {
    (Vector2 WorldPosition, Angle WorldRotation) = this.GetWorldPositionRotation(component, xforms);
    return (WorldPosition, WorldRotation, Matrix3Helpers.CreateTransform(ref WorldPosition, ref WorldRotation), Matrix3Helpers.CreateInverseTransform(ref WorldPosition, ref WorldRotation));
  }

  public void AttachToGridOrMap(EntityUid uid, TransformComponent? xform = null)
  {
    if (this.TerminatingOrDeleted(uid) || !this.XformQuery.Resolve(uid, ref xform, false) || !xform.ParentUid.IsValid())
      return;
    EntityUid parentUid = xform.ParentUid;
    EntityUid? gridUid = xform.GridUid;
    if ((gridUid.HasValue ? (parentUid == gridUid.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    EntityCoordinates? coordinates;
    if (!this.TryGetMapOrGridCoordinates(uid, out coordinates, xform))
    {
      if (!this._mapQuery.HasComp(uid))
        this.Log.Warning($"Failed to attach entity to map or grid. Entity: ({this.ToPrettyString((Entity<MetaDataComponent>) uid)}). Trace: {Environment.StackTrace}");
      this.DetachEntity(uid, xform);
    }
    else
    {
      if (coordinates.Value.EntityId == xform.ParentUid || coordinates.Value.EntityId == uid)
        return;
      this.SetCoordinates(uid, xform, coordinates.Value);
    }
  }

  public bool TryGetMapOrGridCoordinates(
    EntityUid uid,
    [NotNullWhen(true)] out EntityCoordinates? coordinates,
    TransformComponent? xform = null)
  {
    coordinates = new EntityCoordinates?();
    if (!this.XformQuery.Resolve(uid, ref xform, false) || !xform.ParentUid.IsValid())
      return false;
    EntityUid? mapUid = xform.MapUid;
    if (mapUid.HasValue)
    {
      EntityUid valueOrDefault = mapUid.GetValueOrDefault();
      if (!this.TerminatingOrDeleted(valueOrDefault))
      {
        Vector2 worldPosition = this.GetWorldPosition(xform);
        EntityUid uid1;
        coordinates = !this._mapManager.TryFindGridAt(valueOrDefault, worldPosition, out uid1, out MapGridComponent _) || this.TerminatingOrDeleted(uid1) ? new EntityCoordinates?(new EntityCoordinates(valueOrDefault, worldPosition)) : new EntityCoordinates?(uid1 == xform.ParentUid ? new EntityCoordinates(uid1, xform.LocalPosition) : new EntityCoordinates(uid1, Vector2.Transform(worldPosition, this.GetInvWorldMatrix(uid1))));
        return true;
      }
    }
    return false;
  }

  [Obsolete("Use DetachEntity")]
  public void DetachParentToNull(EntityUid uid, TransformComponent xform)
  {
    this.DetachEntity(uid, xform);
  }

  public void DetachEntity(EntityUid uid, TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return;
    TransformComponent component;
    this.XformQuery.TryGetComponent(xform.ParentUid, out component);
    this.DetachEntity(uid, xform, this.MetaData(uid), component);
  }

  public void DetachEntity(Entity<TransformComponent?> ent)
  {
    if (!this.XformQuery.Resolve(ent.Owner, ref ent.Comp))
      return;
    TransformComponent component;
    this.XformQuery.TryGetComponent(ent.Comp.ParentUid, out component);
    this.DetachEntity(ent.Owner, ent.Comp, this.MetaData(ent.Owner), component);
  }

  public void DetachEntity(
    EntityUid uid,
    TransformComponent xform,
    MetaDataComponent meta,
    TransformComponent? oldXform,
    bool terminating = false)
  {
    try
    {
      this.DetachEntityInternal(uid, xform, meta, oldXform, terminating);
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while attempting to detach an entity to nullspace. Entity: {this.ToPrettyString(uid, meta)}. Exception: {ex}");
    }
  }

  internal void DetachEntityInternal(
    EntityUid uid,
    TransformComponent xform,
    MetaDataComponent meta,
    TransformComponent? oldXform,
    bool terminating = false)
  {
    if (!terminating && meta.EntityLifeStage >= EntityLifeStage.Terminating)
    {
      this.Log.Error($"Attempting to detach a terminating entity: {this.ToPrettyString(uid, meta)}. Trace: {Environment.StackTrace}");
    }
    else
    {
      if (!xform._parent.IsValid())
        return;
      this._lookup.RemoveFromEntityTree(uid, xform);
      xform.NextPosition = new Vector2?();
      xform.NextRotation = new Angle?();
      xform.LerpParent = EntityUid.Invalid;
      MetaDataComponent component;
      if (xform.Anchored && this._metaQuery.TryGetComponent(xform.GridUid, out component) && component.EntityLifeStage <= EntityLifeStage.MapInitialized)
      {
        MapGridComponent grid = this.Comp<MapGridComponent>(xform.GridUid.Value);
        Vector2i pos = this._map.TileIndicesFor(xform.GridUid.Value, grid, xform.Coordinates);
        this._map.RemoveFromSnapGridCell(xform.GridUid.Value, grid, pos, uid);
        xform._anchored = false;
        AnchorStateChangedEvent args = new AnchorStateChangedEvent(uid, xform, true);
        this.RaiseLocalEvent<AnchorStateChangedEvent>(uid, ref args, true);
      }
      this.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (uid, xform, meta), new EntityCoordinates(), new Angle?(Angle.Zero), oldParent: oldXform);
    }
  }

  private void OnGridAdd(EntityUid uid, TransformComponent component, GridAddEvent args)
  {
    MetaDataComponent metaDataComponent = this.MetaData(uid);
    if (metaDataComponent.EntityLifeStage > EntityLifeStage.Initialized)
    {
      this.SetGridId((Entity<TransformComponent, MetaDataComponent>) (uid, component, metaDataComponent), new EntityUid?(uid));
    }
    else
    {
      component._gridInitialized = true;
      component._gridUid = new EntityUid?(uid);
    }
  }

  public void DropNextTo(Entity<TransformComponent?> entity, Entity<TransformComponent?> target)
  {
    TransformComponent comp = entity.Comp;
    if (!this.XformQuery.Resolve((EntityUid) entity, ref comp))
      return;
    TransformComponent component = target.Comp;
    if (this.XformQuery.Resolve((EntityUid) target, ref component))
    {
      EntityUid parentUid = component.ParentUid;
      if (parentUid.IsValid())
      {
        EntityCoordinates coordinates = component.Coordinates;
        EntityUid entityUid = target.Owner;
        while (true)
        {
          parentUid = component.ParentUid;
          if (parentUid.IsValid())
          {
            BaseContainer container;
            if (!this._container.IsEntityInContainer(entityUid) || !this._container.TryGetContainingContainer(component.ParentUid, entityUid, out container) || !this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) ((EntityUid) entity, comp, (MetaDataComponent) null, (PhysicsComponent) null), container))
            {
              entityUid = component.ParentUid;
              component = this.XformQuery.GetComponent(entityUid);
            }
            else
              break;
          }
          else
            goto label_9;
        }
        return;
label_9:
        this.SetCoordinates((EntityUid) entity, comp, coordinates);
        this.AttachToGridOrMap((EntityUid) entity, comp);
        return;
      }
    }
    this.DetachEntity((EntityUid) entity, comp);
  }

  public void PlaceNextTo(Entity<TransformComponent?> entity, Entity<TransformComponent?> target)
  {
    TransformComponent comp1 = entity.Comp;
    if (!this.XformQuery.Resolve((EntityUid) entity, ref comp1))
      return;
    TransformComponent comp2 = target.Comp;
    if (!this.XformQuery.Resolve((EntityUid) target, ref comp2) || !comp2.ParentUid.IsValid())
      this.DetachEntity((EntityUid) entity, comp1);
    else if (!this._container.IsEntityInContainer((EntityUid) target))
    {
      this.SetCoordinates((EntityUid) entity, comp1, comp2.Coordinates);
    }
    else
    {
      foreach (BaseContainer container in this.Comp<ContainerManagerComponent>(comp2.ParentUid).Containers.Values)
      {
        if (container.Contains((EntityUid) target) && !this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) ((EntityUid) entity, comp1, (MetaDataComponent) null, (PhysicsComponent) null), container))
          this.PlaceNextTo((Entity<TransformComponent>) ((EntityUid) entity, comp1), (Entity<TransformComponent>) comp2.ParentUid);
      }
    }
  }

  public bool SwapPositions(Entity<TransformComponent?> entity1, Entity<TransformComponent?> entity2)
  {
    if (!this.XformQuery.Resolve((EntityUid) entity1, ref entity1.Comp) || !this.XformQuery.Resolve((EntityUid) entity2, ref entity2.Comp))
      return false;
    if (entity1 == entity2)
      return true;
    if (this.IsParentOf(entity1.Comp, (EntityUid) entity2) || this.IsParentOf(entity2.Comp, (EntityUid) entity1))
      return false;
    MapCoordinates? nullable1 = new MapCoordinates?();
    MapCoordinates? nullable2 = new MapCoordinates?();
    BaseContainer container1;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) entity1, out container1))
      this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entity1, container1, force: true);
    else
      nullable1 = new MapCoordinates?(this.GetMapCoordinates(entity1.Comp));
    BaseContainer container2;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) entity2, out container2))
      this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entity2, container2, force: true);
    else
      nullable2 = new MapCoordinates?(this.GetMapCoordinates(entity2.Comp));
    if (container1 != null && container1.Owner == entity2.Owner || container2 != null && container2.Owner == entity1.Owner)
      return false;
    MapGridComponent grid;
    if (container2 != null)
    {
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entity1, container2);
    }
    else
    {
      if (!nullable2.HasValue)
        throw new InvalidOperationException();
      EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(nullable2.Value.MapId));
      EntityUid uid;
      if (!this._gridQuery.HasComponent((EntityUid) entity1) && this._mapManager.TryFindGridAt(mapOrInvalid, nullable2.Value.Position, out uid, out grid))
      {
        Matrix3x2 invWorldMatrix = this.GetInvWorldMatrix(uid);
        this.SetCoordinates((EntityUid) entity1, new EntityCoordinates(uid, Vector2.Transform(nullable2.Value.Position, invWorldMatrix)));
      }
      else
        this.SetCoordinates((EntityUid) entity1, new EntityCoordinates(mapOrInvalid, nullable2.Value.Position));
    }
    if (container1 != null)
    {
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entity2, container1);
    }
    else
    {
      if (!nullable1.HasValue)
        throw new InvalidOperationException();
      EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(nullable1.Value.MapId));
      EntityUid uid;
      if (!this._gridQuery.HasComponent((EntityUid) entity1) && this._mapManager.TryFindGridAt(mapOrInvalid, nullable1.Value.Position, out uid, out grid))
      {
        Matrix3x2 invWorldMatrix = this.GetInvWorldMatrix(uid);
        this.SetCoordinates((EntityUid) entity2, new EntityCoordinates(uid, Vector2.Transform(nullable1.Value.Position, invWorldMatrix)));
      }
      else
        this.SetCoordinates((EntityUid) entity2, new EntityCoordinates(mapOrInvalid, nullable1.Value.Position));
    }
    return true;
  }

  public bool IsValid(EntityCoordinates coordinates)
  {
    EntityUid entityId = coordinates.EntityId;
    return entityId.IsValid() && this.Exists(entityId) && float.IsFinite(coordinates.Position.X) && float.IsFinite(coordinates.Position.Y);
  }

  public EntityCoordinates WithEntityId(EntityCoordinates coordinates, EntityUid entity)
  {
    return !(entity == coordinates.EntityId) ? this.ToCoordinates((Entity<TransformComponent>) entity, this.ToMapCoordinates(coordinates)) : coordinates;
  }

  public MapCoordinates ToMapCoordinates(EntityCoordinates coordinates, bool logError = true)
  {
    TransformComponent comp;
    if (!this.TryComp(coordinates.EntityId, out comp))
    {
      if (logError)
        this.Log.Error($"Attempted to convert coordinates with invalid entity: {coordinates}. Trace: {Environment.StackTrace}");
      return MapCoordinates.Nullspace;
    }
    Vector2 position = ((Angle) ref comp._localRotation).RotateVec(ref coordinates.Position) + comp._localPosition;
    while (true)
    {
      EntityUid parentUid = comp.ParentUid;
      EntityUid? mapUid = comp.MapUid;
      if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && comp.ParentUid.IsValid())
      {
        comp = this.XformQuery.GetComponent(comp.ParentUid);
        position = ((Angle) ref comp._localRotation).RotateVec(ref position) + comp._localPosition;
      }
      else
        break;
    }
    return new MapCoordinates(position, comp.MapID);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public MapCoordinates ToMapCoordinates(NetCoordinates coordinates)
  {
    return this.ToMapCoordinates(this.GetCoordinates(coordinates));
  }

  public Vector2 ToWorldPosition(EntityCoordinates coordinates, bool logError = true)
  {
    TransformComponent comp;
    if (!this.TryComp(coordinates.EntityId, out comp))
    {
      if (logError)
        this.Log.Error($"Attempted to convert coordinates with invalid entity: {coordinates}. Trace: {Environment.StackTrace}");
      return Vector2.Zero;
    }
    Vector2 worldPosition = ((Angle) ref comp._localRotation).RotateVec(ref coordinates.Position) + comp._localPosition;
    while (true)
    {
      EntityUid parentUid = comp.ParentUid;
      EntityUid? mapUid = comp.MapUid;
      if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && comp.ParentUid.IsValid())
      {
        comp = this.XformQuery.GetComponent(comp.ParentUid);
        worldPosition = ((Angle) ref comp._localRotation).RotateVec(ref worldPosition) + comp._localPosition;
      }
      else
        break;
    }
    return worldPosition;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 ToWorldPosition(NetCoordinates coordinates)
  {
    return this.ToWorldPosition(this.GetCoordinates(coordinates));
  }

  public EntityCoordinates ToCoordinates(
    Entity<TransformComponent?> entity,
    MapCoordinates coordinates)
  {
    if (!this.Resolve((EntityUid) entity, ref entity.Comp, false))
    {
      this.Log.Error($"Attempted to convert coordinates with invalid entity: {coordinates}. Trace: {Environment.StackTrace}");
      return new EntityCoordinates();
    }
    if (entity.Comp.MapID != coordinates.MapId)
    {
      this.Log.Error($"Attempted to convert map coordinates {coordinates} to entity coordinates on a different map. Entity: {this.ToPrettyString(new EntityUid?((EntityUid) entity))}. Trace: {Environment.StackTrace}");
      return new EntityCoordinates();
    }
    Vector2 position = Vector2.Transform(coordinates.Position, this.GetInvWorldMatrix(entity.Comp));
    return new EntityCoordinates((EntityUid) entity, position);
  }

  public EntityCoordinates ToCoordinates(MapCoordinates coordinates)
  {
    EntityUid? uid;
    if (this._map.TryGetMap(new MapId?(coordinates.MapId), out uid))
      return this.ToCoordinates((Entity<TransformComponent>) uid.Value, coordinates);
    this.Log.Error($"Attempted to convert map coordinates with unknown map id: {coordinates}. Trace: {Environment.StackTrace}");
    return new EntityCoordinates();
  }

  public EntityUid? GetGrid(EntityCoordinates coordinates)
  {
    return this.GetGrid((Entity<TransformComponent>) coordinates.EntityId);
  }

  public EntityUid? GetGrid(Entity<TransformComponent?> entity)
  {
    return this.Resolve((EntityUid) entity, ref entity.Comp, false) ? entity.Comp.GridUid : new EntityUid?();
  }

  public MapId GetMapId(EntityCoordinates coordinates)
  {
    return this.GetMapId((Entity<TransformComponent>) coordinates.EntityId);
  }

  public MapId GetMapId(Entity<TransformComponent?> entity)
  {
    return this.Resolve((EntityUid) entity, ref entity.Comp, false) ? entity.Comp.MapID : MapId.Nullspace;
  }

  public EntityUid? GetMap(EntityCoordinates coordinates)
  {
    return this.GetMap((Entity<TransformComponent>) coordinates.EntityId);
  }

  public EntityUid? GetMap(Entity<TransformComponent?> entity)
  {
    return this.Resolve((EntityUid) entity, ref entity.Comp, false) ? entity.Comp.MapUid : new EntityUid?();
  }

  public bool InRange(EntityCoordinates coordA, EntityCoordinates coordB, float range)
  {
    if (!coordA.EntityId.IsValid() || !coordB.EntityId.IsValid())
      return false;
    if (coordA.EntityId == coordB.EntityId)
      return (double) (coordA.Position - coordB.Position).LengthSquared() < (double) range * (double) range;
    MapCoordinates mapCoordinates1 = this.ToMapCoordinates(coordA, false);
    MapCoordinates mapCoordinates2 = this.ToMapCoordinates(coordB, false);
    return !(mapCoordinates1.MapId != mapCoordinates2.MapId) && !(mapCoordinates1.MapId == MapId.Nullspace) && mapCoordinates1.InRange(mapCoordinates2, range);
  }

  public bool InRange(
    Entity<TransformComponent?> entA,
    Entity<TransformComponent?> entB,
    float range)
  {
    if (!this.Resolve((EntityUid) entA, ref entA.Comp, false) || !this.Resolve((EntityUid) entB, ref entB.Comp, false) || !entA.Comp.ParentUid.IsValid() || !entB.Comp.ParentUid.IsValid())
      return false;
    if (entA.Comp.ParentUid == entB.Comp.ParentUid)
      return (double) (entA.Comp.LocalPosition - entB.Comp.LocalPosition).LengthSquared() < (double) range * (double) range;
    if (entA.Comp.ParentUid == entB.Owner)
      return (double) entA.Comp.LocalPosition.LengthSquared() < (double) range * (double) range;
    if (entB.Comp.ParentUid == entA.Owner)
      return (double) entB.Comp.LocalPosition.LengthSquared() < (double) range * (double) range;
    MapCoordinates mapCoordinates1 = this.GetMapCoordinates(entA);
    MapCoordinates mapCoordinates2 = this.GetMapCoordinates(entB);
    return !(mapCoordinates1.MapId != mapCoordinates2.MapId) && !(mapCoordinates1.MapId == MapId.Nullspace) && mapCoordinates1.InRange(mapCoordinates2, range);
  }

  public event SharedTransformSystem.MoveEventHandler? OnGlobalMoveEvent;

  internal event SharedTransformSystem.MoveEventHandler? OnBeforeMoveEvent;

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this._mapQuery = this.GetEntityQuery<MapComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._metaQuery = this.GetEntityQuery<MetaDataComponent>();
    this.XformQuery = this.GetEntityQuery<TransformComponent>();
    this.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.MapManagerOnTileChanged));
    this.SubscribeLocalEvent<TransformComponent, ComponentInit>(new ComponentEventHandler<TransformComponent, ComponentInit>(this.OnCompInit));
    this.SubscribeLocalEvent<TransformComponent, ComponentStartup>(new ComponentEventHandler<TransformComponent, ComponentStartup>(this.OnCompStartup));
    this.SubscribeLocalEvent<TransformComponent, ComponentGetState>(new ComponentEventRefHandler<TransformComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<TransformComponent, ComponentHandleState>(new ComponentEventRefHandler<TransformComponent, ComponentHandleState>(this.OnHandleState));
    this.SubscribeLocalEvent<TransformComponent, GridAddEvent>(new ComponentEventHandler<TransformComponent, GridAddEvent>(this.OnGridAdd));
  }

  private void MapManagerOnTileChanged(ref TileChangedEvent e)
  {
    foreach (TileChangedEntry change in e.Changes)
    {
      if (!(change.NewTile != Tile.Empty))
        this.DeparentAllEntsOnTile((EntityUid) e.Entity, change.GridIndices);
    }
  }

  private void DeparentAllEntsOnTile(EntityUid gridId, Vector2i tileIndices)
  {
    BroadphaseComponent comp1;
    MapGridComponent comp2;
    TransformComponent component1;
    TransformComponent component2;
    if (!this.TryComp<BroadphaseComponent>(gridId, out comp1) || !this.TryComp<MapGridComponent>(gridId, out comp2) || !this.XformQuery.TryGetComponent(gridId, out component1) || !this.XformQuery.TryGetComponent(component1.MapUid, out component2))
      return;
    Box2 localBounds = this._lookup.GetLocalBounds(tileIndices, comp2.TileSize);
    foreach (EntityUid uid in this._lookup.GetLocalEntitiesIntersecting(comp1, localBounds, LookupFlags.Uncontained | LookupFlags.Approximate))
    {
      TransformComponent component3;
      if (this.XformQuery.TryGetComponent(uid, out component3) && !(component3.ParentUid != gridId) && ((Box2) ref localBounds).Contains(component3.LocalPosition, true))
      {
        if (this.EntityManager.IsQueuedForDeletion(uid))
          this.DetachEntity(uid, component3, this.MetaData(uid), component1);
        else
          this.SetParent(uid, component3, component1.MapUid.Value, component2);
      }
    }
  }

  public EntityCoordinates GetMoverCoordinates(EntityUid uid)
  {
    return this.GetMoverCoordinates(uid, this.XformQuery.GetComponent(uid));
  }

  public EntityCoordinates GetMoverCoordinates(EntityUid uid, TransformComponent xform)
  {
    if (!xform.ParentUid.IsValid())
      return xform.Coordinates;
    if (!xform._gridInitialized)
      this.InitializeGridUid(uid, xform);
    EntityUid? gridUid = xform.GridUid;
    EntityUid parentUid = xform.ParentUid;
    if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == parentUid ? 1 : 0) : 0) != 0)
      return xform.Coordinates;
    Vector2 worldPosition = this.GetWorldPosition(xform, this.XformQuery);
    return xform.GridUid.HasValue ? new EntityCoordinates(xform.GridUid.Value, Vector2.Transform(worldPosition, this.XformQuery.GetComponent(xform.GridUid.Value).InvLocalMatrix)) : new EntityCoordinates(xform.MapUid ?? xform.ParentUid, worldPosition);
  }

  public EntityCoordinates GetMoverCoordinates(
    EntityCoordinates coordinates,
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> xformQuery)
  {
    return this.GetMoverCoordinates(coordinates);
  }

  public EntityCoordinates GetMoverCoordinates(EntityCoordinates coordinates)
  {
    EntityUid entityId1 = coordinates.EntityId;
    if (!entityId1.IsValid())
      return coordinates;
    TransformComponent component = this.XformQuery.GetComponent(entityId1);
    if (!component._gridInitialized)
      this.InitializeGridUid(entityId1, component);
    EntityUid? nullable = component.GridUid;
    EntityUid entityUid1 = entityId1;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid1 ? 1 : 0) : 0) != 0)
      return coordinates;
    EntityUid? mapUid = component.MapUid;
    nullable = mapUid;
    EntityUid entityUid2 = entityId1;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) != 0)
      return coordinates;
    Vector2 position1 = Vector2.Transform(coordinates.Position, this.GetWorldMatrix(component, this.XformQuery));
    nullable = component.GridUid;
    if (!nullable.HasValue)
      return new EntityCoordinates(mapUid ?? entityId1, position1);
    nullable = component.GridUid;
    EntityUid entityId2 = nullable.Value;
    Vector2 position2 = position1;
    ref Robust.Shared.GameObjects.EntityQuery<TransformComponent> local = ref this.XformQuery;
    nullable = component.GridUid;
    EntityUid uid = nullable.Value;
    Matrix3x2 invLocalMatrix = local.GetComponent(uid).InvLocalMatrix;
    Vector2 position3 = Vector2.Transform(position2, invLocalMatrix);
    return new EntityCoordinates(entityId2, position3);
  }

  public (EntityCoordinates Coords, Angle worldRot) GetMoverCoordinateRotation(
    EntityUid uid,
    TransformComponent xform)
  {
    if (!xform.ParentUid.IsValid())
      return (xform.Coordinates, xform.LocalRotation);
    if (!xform._gridInitialized)
      this.InitializeGridUid(uid, xform);
    EntityUid? nullable = xform.GridUid;
    EntityUid parentUid = xform.ParentUid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == parentUid ? 1 : 0) : 0) != 0)
      return (xform.Coordinates, this.GetWorldRotation(xform, this.XformQuery));
    (Vector2 vector2, Angle WorldRotation) = this.GetWorldPositionRotation(xform, this.XformQuery);
    nullable = xform.GridUid;
    EntityCoordinates entityCoordinates;
    if (nullable.HasValue)
    {
      nullable = xform.GridUid;
      EntityUid entityId = nullable.Value;
      Vector2 position1 = vector2;
      ref Robust.Shared.GameObjects.EntityQuery<TransformComponent> local = ref this.XformQuery;
      nullable = xform.GridUid;
      EntityUid uid1 = nullable.Value;
      Matrix3x2 invLocalMatrix = local.GetComponent(uid1).InvLocalMatrix;
      Vector2 position2 = Vector2.Transform(position1, invLocalMatrix);
      entityCoordinates = new EntityCoordinates(entityId, position2);
    }
    else
    {
      nullable = xform.MapUid;
      entityCoordinates = new EntityCoordinates(nullable ?? xform.ParentUid, vector2);
    }
    Angle angle = WorldRotation;
    return (entityCoordinates, angle);
  }

  public Vector2i GetGridOrMapTilePosition(EntityUid uid, TransformComponent? xform = null)
  {
    if (!this.Resolve(uid, ref xform, false))
      return Vector2i.Zero;
    if (!xform.GridUid.HasValue)
      return Vector2Helpers.Floored(this.GetWorldPosition(xform));
    SharedMapSystem map = this._map;
    EntityUid? gridUid = xform.GridUid;
    EntityUid uid1 = gridUid.Value;
    gridUid = xform.GridUid;
    MapGridComponent grid = this.Comp<MapGridComponent>(gridUid.Value);
    EntityCoordinates coordinates = xform.Coordinates;
    return map.CoordinatesToTile(uid1, grid, coordinates);
  }

  public Vector2i GetGridTilePositionOrDefault(
    Entity<TransformComponent?> entity,
    MapGridComponent? grid = null)
  {
    TransformComponent comp = entity.Comp;
    if (this.Resolve(entity.Owner, ref comp))
    {
      EntityUid? gridUid = comp.GridUid;
      if (gridUid.HasValue)
      {
        gridUid = comp.GridUid;
        if (!this.Resolve<MapGridComponent>(gridUid.Value, ref grid))
          return Vector2i.Zero;
        SharedMapSystem map = this._map;
        gridUid = comp.GridUid;
        EntityUid uid = gridUid.Value;
        MapGridComponent grid1 = grid;
        EntityCoordinates coordinates = comp.Coordinates;
        return map.CoordinatesToTile(uid, grid1, coordinates);
      }
    }
    return Vector2i.Zero;
  }

  public bool TryGetGridTilePosition(
    Entity<TransformComponent?> entity,
    out Vector2i indices,
    MapGridComponent? grid = null)
  {
    indices = new Vector2i();
    TransformComponent comp = entity.Comp;
    if (!this.Resolve(entity.Owner, ref comp) || !comp.GridUid.HasValue)
      return false;
    EntityUid? gridUid = comp.GridUid;
    if (!this.Resolve<MapGridComponent>(gridUid.Value, ref grid))
      return false;
    ref Vector2i local = ref indices;
    SharedMapSystem map = this._map;
    gridUid = comp.GridUid;
    EntityUid uid = gridUid.Value;
    MapGridComponent grid1 = grid;
    EntityCoordinates coordinates = comp.Coordinates;
    Vector2i tile = map.CoordinatesToTile(uid, grid1, coordinates);
    local = tile;
    return true;
  }

  internal void RaiseMoveEvent(
    Entity<TransformComponent, MetaDataComponent> ent,
    EntityUid oldParent,
    Vector2 oldPosition,
    Angle oldRotation,
    EntityUid? oldMap,
    bool checkTraversal = true)
  {
    EntityCoordinates newPos = ent.Comp1._parent == EntityUid.Invalid ? new EntityCoordinates() : new EntityCoordinates(ent.Comp1._parent, ent.Comp1._localPosition);
    EntityCoordinates oldPos = oldParent == EntityUid.Invalid ? new EntityCoordinates() : new EntityCoordinates(oldParent, oldPosition);
    MoveEvent moveEvent = new MoveEvent(ent, oldPos, newPos, oldRotation, ent.Comp1._localRotation);
    if (oldParent != ent.Comp1._parent)
    {
      this._physics.OnParentChange(ent, oldParent, oldMap);
      SharedTransformSystem.MoveEventHandler onBeforeMoveEvent = this.OnBeforeMoveEvent;
      if (onBeforeMoveEvent != null)
        onBeforeMoveEvent(ref moveEvent);
      EntParentChangedMessage args = new EntParentChangedMessage(moveEvent.Sender, new EntityUid?(oldParent), oldMap, moveEvent.Component);
      this.RaiseLocalEvent<EntParentChangedMessage>(moveEvent.Sender, ref args, true);
    }
    else
    {
      SharedTransformSystem.MoveEventHandler onBeforeMoveEvent = this.OnBeforeMoveEvent;
      if (onBeforeMoveEvent != null)
        onBeforeMoveEvent(ref moveEvent);
    }
    this.RaiseLocalEvent<MoveEvent>(moveEvent.Sender, ref moveEvent);
    SharedTransformSystem.MoveEventHandler onGlobalMoveEvent = this.OnGlobalMoveEvent;
    if (onGlobalMoveEvent != null)
      onGlobalMoveEvent(ref moveEvent);
    if (!checkTraversal)
      return;
    this._traversal.CheckTraverse((Entity<TransformComponent>) ent);
  }

  public delegate void MoveEventHandler(ref MoveEvent ev);
}
