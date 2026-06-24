// Decompiled with JetBrains decompiler
// Type: Content.Client.IconSmoothing.IconSmoothSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.IconSmoothing;
using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.IconSmoothing;

public sealed class IconSmoothSystem : EntitySystem
{
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SpriteSystem _sprite;
  private readonly Queue<EntityUid> _dirtyEntities = new Queue<EntityUid>();
  private readonly Queue<EntityUid> _anchorChangedEntities = new Queue<EntityUid>();
  private int _generation;
  private EntityQuery<CMIconSmoothComponent> _cmIconSmoothQuery;

  public void SetEnabled(EntityUid uid, bool value, IconSmoothComponent? component = null)
  {
    if (!this.Resolve<IconSmoothComponent>(uid, ref component, false) || value == component.Enabled)
      return;
    component.Enabled = value;
    this.DirtyNeighbours(uid, component);
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeEdge();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IconSmoothComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<IconSmoothComponent, AnchorStateChangedEvent>((object) this, __methodptr(OnAnchorChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IconSmoothComponent, ComponentShutdown>(new ComponentEventHandler<IconSmoothComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IconSmoothComponent, ComponentStartup>(new ComponentEventHandler<IconSmoothComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, IconSmoothComponent component, ComponentStartup args)
  {
    TransformComponent transformComponent = this.Transform(uid);
    if (transformComponent.Anchored)
    {
      IconSmoothComponent iconSmoothComponent = component;
      MapGridComponent mapGridComponent1;
      (EntityUid?, Vector2i)? nullable1;
      if (!this.TryComp<MapGridComponent>(transformComponent.GridUid, ref mapGridComponent1))
      {
        nullable1 = new (EntityUid?, Vector2i)?((new EntityUid?(), new Vector2i(0, 0)));
      }
      else
      {
        EntityUid? gridUid = transformComponent.GridUid;
        EntityUid? nullable2 = new EntityUid?(gridUid.Value);
        SharedMapSystem mapSystem = this._mapSystem;
        gridUid = transformComponent.GridUid;
        EntityUid entityUid = gridUid.Value;
        MapGridComponent mapGridComponent2 = mapGridComponent1;
        EntityCoordinates coordinates = transformComponent.Coordinates;
        Vector2i vector2i = mapSystem.TileIndicesFor(entityUid, mapGridComponent2, coordinates);
        nullable1 = new (EntityUid?, Vector2i)?((nullable2, vector2i));
      }
      iconSmoothComponent.LastPosition = nullable1;
      this.DirtyNeighbours(uid, component);
    }
    SpriteComponent spriteComponent;
    if (component.Mode != IconSmoothingMode.Corners || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this.SetCornerLayers(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component);
    if (component.Shader == null)
      return;
    spriteComponent.LayerSetShader((object) IconSmoothSystem.CornerLayers.SE, component.Shader);
    spriteComponent.LayerSetShader((object) IconSmoothSystem.CornerLayers.NE, component.Shader);
    spriteComponent.LayerSetShader((object) IconSmoothSystem.CornerLayers.NW, component.Shader);
    spriteComponent.LayerSetShader((object) IconSmoothSystem.CornerLayers.SW, component.Shader);
  }

  public void SetStateBase(EntityUid uid, IconSmoothComponent component, string newState)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    component.StateBase = newState;
    this.SetCornerLayers(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component);
  }

  private void SetCornerLayers(Entity<SpriteComponent?> sprite, IconSmoothComponent component)
  {
    this._sprite.LayerMapRemove(sprite, (Enum) IconSmoothSystem.CornerLayers.SE);
    this._sprite.LayerMapRemove(sprite, (Enum) IconSmoothSystem.CornerLayers.NE);
    this._sprite.LayerMapRemove(sprite, (Enum) IconSmoothSystem.CornerLayers.NW);
    this._sprite.LayerMapRemove(sprite, (Enum) IconSmoothSystem.CornerLayers.SW);
    string str = component.StateBase + "0";
    this._sprite.LayerMapSet(sprite, (Enum) IconSmoothSystem.CornerLayers.SE, this._sprite.AddRsiLayer(sprite, RSI.StateId.op_Implicit(str), (RSI) null, new int?()));
    this._sprite.LayerSetDirOffset(sprite, (Enum) IconSmoothSystem.CornerLayers.SE, (SpriteComponent.DirectionOffset) 0);
    this._sprite.LayerMapSet(sprite, (Enum) IconSmoothSystem.CornerLayers.NE, this._sprite.AddRsiLayer(sprite, RSI.StateId.op_Implicit(str), (RSI) null, new int?()));
    this._sprite.LayerSetDirOffset(sprite, (Enum) IconSmoothSystem.CornerLayers.NE, (SpriteComponent.DirectionOffset) 2);
    this._sprite.LayerMapSet(sprite, (Enum) IconSmoothSystem.CornerLayers.NW, this._sprite.AddRsiLayer(sprite, RSI.StateId.op_Implicit(str), (RSI) null, new int?()));
    this._sprite.LayerSetDirOffset(sprite, (Enum) IconSmoothSystem.CornerLayers.NW, (SpriteComponent.DirectionOffset) 3);
    this._sprite.LayerMapSet(sprite, (Enum) IconSmoothSystem.CornerLayers.SW, this._sprite.AddRsiLayer(sprite, RSI.StateId.op_Implicit(str), (RSI) null, new int?()));
    this._sprite.LayerSetDirOffset(sprite, (Enum) IconSmoothSystem.CornerLayers.SW, (SpriteComponent.DirectionOffset) 1);
  }

  private void OnShutdown(EntityUid uid, IconSmoothComponent component, ComponentShutdown args)
  {
    this._dirtyEntities.Enqueue(uid);
    this.DirtyNeighbours(uid, component);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    EntityQuery<IconSmoothComponent> entityQuery2 = this.GetEntityQuery<IconSmoothComponent>();
    EntityUid result1;
    while (this._anchorChangedEntities.TryDequeue(out result1))
    {
      TransformComponent transform;
      if (entityQuery1.TryGetComponent(result1, ref transform) && !MapId.op_Equality(transform.MapID, MapId.Nullspace))
        this.DirtyNeighbours(result1, transform: transform, smoothQuery: new EntityQuery<IconSmoothComponent>?(entityQuery2));
    }
    if (this._dirtyEntities.Count == 0)
      return;
    ++this._generation;
    EntityQuery<SpriteComponent> entityQuery3 = this.GetEntityQuery<SpriteComponent>();
    EntityUid result2;
    while (this._dirtyEntities.TryDequeue(out result2))
      this.CalculateNewSprite(result2, entityQuery3, entityQuery2, entityQuery1);
  }

  public void DirtyNeighbours(
    EntityUid uid,
    IconSmoothComponent? comp = null,
    TransformComponent? transform = null,
    EntityQuery<IconSmoothComponent>? smoothQuery = null)
  {
    smoothQuery.GetValueOrDefault();
    if (!smoothQuery.HasValue)
      smoothQuery = new EntityQuery<IconSmoothComponent>?(this.GetEntityQuery<IconSmoothComponent>());
    if (!smoothQuery.Value.Resolve(uid, ref comp, true) || !comp.Running)
      return;
    this._dirtyEntities.Enqueue(uid);
    if (!this.Resolve(uid, ref transform, true))
      return;
    MapGridComponent mapGridComponent;
    EntityUid entityUid;
    Vector2i vector2i1;
    if (transform.Anchored && this.TryComp<MapGridComponent>(transform.GridUid, ref mapGridComponent))
    {
      entityUid = transform.GridUid.Value;
      vector2i1 = this._mapSystem.CoordinatesToTile(transform.GridUid.Value, mapGridComponent, transform.Coordinates);
    }
    else
    {
      (EntityUid?, Vector2i)? lastPosition = comp.LastPosition;
      if (!lastPosition.HasValue)
        return;
      (EntityUid?, Vector2i) valueOrDefault1 = lastPosition.GetValueOrDefault();
      EntityUid? nullable = valueOrDefault1.Item1;
      if (!nullable.HasValue)
        return;
      EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
      Vector2i vector2i2 = valueOrDefault1.Item2;
      if (!this.TryComp<MapGridComponent>(valueOrDefault2, ref mapGridComponent))
        return;
      entityUid = valueOrDefault2;
      vector2i1 = vector2i2;
    }
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(1, 0))));
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(-1, 0))));
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(0, 1))));
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(0, -1))));
    bool flag;
    switch (comp.Mode)
    {
      case IconSmoothingMode.Corners:
      case IconSmoothingMode.Diagonal:
      case IconSmoothingMode.NoSprite:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (!flag)
      return;
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(1, 1))));
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(-1, -1))));
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(-1, 1))));
    this.DirtyEntities(this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, Vector2i.op_Addition(vector2i1, new Vector2i(1, -1))));
  }

  private void DirtyEntities(AnchoredEntitiesEnumerator entities)
  {
    EntityUid? nullable;
    while (((AnchoredEntitiesEnumerator) ref entities).MoveNext(ref nullable))
      this._dirtyEntities.Enqueue(nullable.Value);
  }

  private void OnAnchorChanged(
    EntityUid uid,
    IconSmoothComponent component,
    ref AnchorStateChangedEvent args)
  {
    if (args.Detaching)
      return;
    this._anchorChangedEntities.Enqueue(uid);
  }

  private void CalculateNewSprite(
    EntityUid uid,
    EntityQuery<SpriteComponent> spriteQuery,
    EntityQuery<IconSmoothComponent> smoothQuery,
    EntityQuery<TransformComponent> xformQuery,
    IconSmoothComponent? smooth = null)
  {
    Entity<MapGridComponent>? gridEntity = new Entity<MapGridComponent>?();
    if (!smoothQuery.Resolve(uid, ref smooth, false) || smooth.Mode == IconSmoothingMode.NoSprite || smooth.UpdateGeneration == this._generation || !smooth.Enabled || !smooth.Running)
    {
      SmoothEdgeComponent component;
      TransformComponent transformComponent;
      if (smooth == null || !smooth.Enabled || !this.TryComp<SmoothEdgeComponent>(uid, ref component) || !xformQuery.TryGetComponent(uid, ref transformComponent))
        return;
      DirectionFlag directions = (DirectionFlag) 0;
      MapGridComponent mapGridComponent;
      if (this.TryComp<MapGridComponent>(transformComponent.GridUid, ref mapGridComponent))
      {
        EntityUid entityUid = transformComponent.GridUid.Value;
        Vector2i vector2i = this._mapSystem.TileIndicesFor(entityUid, mapGridComponent, transformComponent.Coordinates);
        gridEntity = new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((entityUid, mapGridComponent)));
        if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, DirectionExtensions.Offset(vector2i, (Direction) 4)), smoothQuery))
          directions = (DirectionFlag) (directions | 4);
        if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, DirectionExtensions.Offset(vector2i, (Direction) 0)), smoothQuery))
          directions = (DirectionFlag) (directions | 1);
        if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, DirectionExtensions.Offset(vector2i, (Direction) 2)), smoothQuery))
          directions = (DirectionFlag) (directions | 2);
        if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(entityUid, mapGridComponent, DirectionExtensions.Offset(vector2i, (Direction) 6)), smoothQuery))
          directions = (DirectionFlag) (directions | 8);
      }
      this.CalculateEdge(uid, directions, component: component);
    }
    else
    {
      TransformComponent component = xformQuery.GetComponent(uid);
      smooth.UpdateGeneration = this._generation;
      SpriteComponent spriteComponent;
      if (!spriteQuery.TryGetComponent(uid, ref spriteComponent))
      {
        this.Log.Error($"Encountered a icon-smoothing entity without a sprite: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
        this.RemCompDeferred(uid, (IComponent) smooth);
      }
      else
      {
        (EntityUid, SpriteComponent) valueTuple = (uid, spriteComponent);
        if (component.Anchored)
        {
          MapGridComponent mapGridComponent;
          if (this.TryComp<MapGridComponent>(component.GridUid, ref mapGridComponent))
          {
            gridEntity = new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((component.GridUid.Value, mapGridComponent)));
          }
          else
          {
            this.Log.Error($"Failed to calculate IconSmoothComponent sprite in {uid} because grid {component.GridUid} was missing.");
            return;
          }
        }
        switch (smooth.Mode)
        {
          case IconSmoothingMode.Corners:
            this.CalculateNewSpriteCorners(uid, gridEntity, smooth, Entity<SpriteComponent>.op_Implicit(valueTuple), component, smoothQuery);
            break;
          case IconSmoothingMode.CardinalFlags:
            this.CalculateNewSpriteCardinal(uid, gridEntity, smooth, Entity<SpriteComponent>.op_Implicit(valueTuple), component, smoothQuery);
            break;
          case IconSmoothingMode.Diagonal:
            this.CalculateNewSpriteDiagonal(uid, gridEntity, smooth, Entity<SpriteComponent>.op_Implicit(valueTuple), component, smoothQuery);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        IconSmoothingUpdatedEvent smoothingUpdatedEvent = new IconSmoothingUpdatedEvent();
        this.RaiseLocalEvent<IconSmoothingUpdatedEvent>(uid, ref smoothingUpdatedEvent, false);
      }
    }
  }

  private void CalculateNewSpriteDiagonal(
    EntityUid uid,
    Entity<MapGridComponent>? gridEntity,
    IconSmoothComponent smooth,
    Entity<SpriteComponent> sprite,
    TransformComponent xform,
    EntityQuery<IconSmoothComponent> smoothQuery)
  {
    if (!gridEntity.HasValue)
    {
      this._sprite.LayerSetRsiState(sprite.AsNullable(), 0, RSI.StateId.op_Implicit(smooth.StateBase + "0"));
    }
    else
    {
      EntityUid owner = gridEntity.Value.Owner;
      MapGridComponent comp = gridEntity.Value.Comp;
      Vector2[] vector2Array = new Vector2[3]
      {
        new Vector2(1f, 0.0f),
        new Vector2(1f, -1f),
        new Vector2(0.0f, -1f)
      };
      Vector2i vector2i1 = this._mapSystem.TileIndicesFor(owner, comp, xform.Coordinates);
      Angle localRotation = xform.LocalRotation;
      bool flag = true;
      for (int index = 0; index < vector2Array.Length; ++index)
      {
        Vector2i vector2i2 = Vector2i.op_Explicit(((Angle) ref localRotation).RotateVec(ref vector2Array[index]));
        flag = flag && this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, Vector2i.op_Addition(vector2i1, vector2i2)), smoothQuery);
      }
      if (flag)
        this._sprite.LayerSetRsiState(sprite.AsNullable(), 0, RSI.StateId.op_Implicit(smooth.StateBase + "1"));
      else
        this._sprite.LayerSetRsiState(sprite.AsNullable(), 0, RSI.StateId.op_Implicit(smooth.StateBase + "0"));
    }
  }

  private void CalculateNewSpriteCardinal(
    EntityUid uid,
    Entity<MapGridComponent>? gridEntity,
    IconSmoothComponent smooth,
    Entity<SpriteComponent> sprite,
    TransformComponent xform,
    EntityQuery<IconSmoothComponent> smoothQuery)
  {
    IconSmoothSystem.CardinalConnectDirs cardinalConnectDirs = IconSmoothSystem.CardinalConnectDirs.None;
    if (!gridEntity.HasValue)
    {
      this._sprite.LayerSetRsiState(sprite.AsNullable(), 0, RSI.StateId.op_Implicit($"{smooth.StateBase}{(int) cardinalConnectDirs}"));
    }
    else
    {
      EntityUid owner = gridEntity.Value.Owner;
      MapGridComponent comp = gridEntity.Value.Comp;
      Vector2i vector2i = this._mapSystem.TileIndicesFor(owner, comp, xform.Coordinates);
      if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 4)), smoothQuery))
        cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.North;
      if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 0)), smoothQuery))
        cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.South;
      if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 2)), smoothQuery))
        cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.East;
      if (this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 6)), smoothQuery))
        cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.West;
      this._sprite.LayerSetRsiState(sprite.AsNullable(), 0, RSI.StateId.op_Implicit($"{smooth.StateBase}{(int) cardinalConnectDirs}"));
      DirectionFlag directions = (DirectionFlag) 0;
      if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.South) != IconSmoothSystem.CardinalConnectDirs.None)
        directions = (DirectionFlag) (directions | 1);
      if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.East) != IconSmoothSystem.CardinalConnectDirs.None)
        directions = (DirectionFlag) (directions | 2);
      if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.North) != IconSmoothSystem.CardinalConnectDirs.None)
        directions = (DirectionFlag) (directions | 4);
      if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.West) != IconSmoothSystem.CardinalConnectDirs.None)
        directions = (DirectionFlag) (directions | 8);
      this.CalculateEdge(Entity<SpriteComponent>.op_Implicit(sprite), directions, Entity<SpriteComponent>.op_Implicit(sprite));
    }
  }

  private bool MatchingEntity(
    EntityUid uid,
    IconSmoothComponent smooth,
    AnchoredEntitiesEnumerator candidates,
    EntityQuery<IconSmoothComponent> smoothQuery)
  {
    EntityUid? nullable;
    while (((AnchoredEntitiesEnumerator) ref candidates).MoveNext(ref nullable))
    {
      IconSmoothComponent iconSmoothComponent1;
      CMIconSmoothComponent iconSmoothComponent2;
      if (smoothQuery.TryGetComponent(nullable, ref iconSmoothComponent1) && iconSmoothComponent1.Enabled && (iconSmoothComponent1.SmoothKey != null && (iconSmoothComponent1.SmoothKey == smooth.SmoothKey || smooth.AdditionalKeys.Contains(iconSmoothComponent1.SmoothKey)) || this._cmIconSmoothQuery.TryComp(uid, ref iconSmoothComponent2) && iconSmoothComponent2.Smooth && this._cmIconSmoothQuery.HasComp(nullable)))
        return true;
    }
    return false;
  }

  private void CalculateNewSpriteCorners(
    EntityUid uid,
    Entity<MapGridComponent>? gridEntity,
    IconSmoothComponent smooth,
    Entity<SpriteComponent> spriteEnt,
    TransformComponent xform,
    EntityQuery<IconSmoothComponent> smoothQuery)
  {
    IconSmoothSystem.CornerFill ne;
    IconSmoothSystem.CornerFill nw;
    IconSmoothSystem.CornerFill sw;
    IconSmoothSystem.CornerFill se;
    if (gridEntity.HasValue)
    {
      (ne, nw, sw, se) = this.CalculateCornerFill(uid, gridEntity.Value, smooth, xform, smoothQuery);
    }
    else
    {
      ne = IconSmoothSystem.CornerFill.None;
      nw = IconSmoothSystem.CornerFill.None;
      sw = IconSmoothSystem.CornerFill.None;
      se = IconSmoothSystem.CornerFill.None;
    }
    SpriteComponent comp = spriteEnt.Comp;
    this._sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum) IconSmoothSystem.CornerLayers.NE, RSI.StateId.op_Implicit($"{smooth.StateBase}{(int) ne}"));
    this._sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum) IconSmoothSystem.CornerLayers.SE, RSI.StateId.op_Implicit($"{smooth.StateBase}{(int) se}"));
    this._sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum) IconSmoothSystem.CornerLayers.SW, RSI.StateId.op_Implicit($"{smooth.StateBase}{(int) sw}"));
    this._sprite.LayerSetRsiState(spriteEnt.AsNullable(), (Enum) IconSmoothSystem.CornerLayers.NW, RSI.StateId.op_Implicit($"{smooth.StateBase}{(int) nw}"));
    DirectionFlag directions = (DirectionFlag) 0;
    if ((se & sw) != IconSmoothSystem.CornerFill.None)
      directions = (DirectionFlag) (directions | 1);
    if ((se & ne) != IconSmoothSystem.CornerFill.None)
      directions = (DirectionFlag) (directions | 2);
    if ((ne & nw) != IconSmoothSystem.CornerFill.None)
      directions = (DirectionFlag) (directions | 4);
    if ((nw & sw) != IconSmoothSystem.CornerFill.None)
      directions = (DirectionFlag) (directions | 8);
    this.CalculateEdge(Entity<SpriteComponent>.op_Implicit(spriteEnt), directions, comp);
  }

  private (IconSmoothSystem.CornerFill ne, IconSmoothSystem.CornerFill nw, IconSmoothSystem.CornerFill sw, IconSmoothSystem.CornerFill se) CalculateCornerFill(
    EntityUid uid,
    Entity<MapGridComponent> gridEntity,
    IconSmoothComponent smooth,
    TransformComponent xform,
    EntityQuery<IconSmoothComponent> smoothQuery)
  {
    EntityUid owner = gridEntity.Owner;
    MapGridComponent comp = gridEntity.Comp;
    Vector2i vector2i = this._mapSystem.TileIndicesFor(owner, comp, xform.Coordinates);
    int num = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 4)), smoothQuery) ? 1 : 0;
    bool flag1 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 3)), smoothQuery);
    bool flag2 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 2)), smoothQuery);
    bool flag3 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 1)), smoothQuery);
    bool flag4 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 0)), smoothQuery);
    bool flag5 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 7)), smoothQuery);
    bool flag6 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 6)), smoothQuery);
    bool flag7 = this.MatchingEntity(uid, smooth, this._mapSystem.GetAnchoredEntitiesEnumerator(owner, comp, DirectionExtensions.Offset(vector2i, (Direction) 5)), smoothQuery);
    IconSmoothSystem.CornerFill cornerFill1 = IconSmoothSystem.CornerFill.None;
    IconSmoothSystem.CornerFill cornerFill2 = IconSmoothSystem.CornerFill.None;
    IconSmoothSystem.CornerFill cornerFill3 = IconSmoothSystem.CornerFill.None;
    IconSmoothSystem.CornerFill cornerFill4 = IconSmoothSystem.CornerFill.None;
    if (num != 0)
    {
      cornerFill1 |= IconSmoothSystem.CornerFill.CounterClockwise;
      cornerFill4 |= IconSmoothSystem.CornerFill.Clockwise;
    }
    if (flag1)
      cornerFill1 |= IconSmoothSystem.CornerFill.Diagonal;
    if (flag2)
    {
      cornerFill1 |= IconSmoothSystem.CornerFill.Clockwise;
      cornerFill2 |= IconSmoothSystem.CornerFill.CounterClockwise;
    }
    if (flag3)
      cornerFill2 |= IconSmoothSystem.CornerFill.Diagonal;
    if (flag4)
    {
      cornerFill2 |= IconSmoothSystem.CornerFill.Clockwise;
      cornerFill3 |= IconSmoothSystem.CornerFill.CounterClockwise;
    }
    if (flag5)
      cornerFill3 |= IconSmoothSystem.CornerFill.Diagonal;
    if (flag6)
    {
      cornerFill3 |= IconSmoothSystem.CornerFill.Clockwise;
      cornerFill4 |= IconSmoothSystem.CornerFill.CounterClockwise;
    }
    if (flag7)
      cornerFill4 |= IconSmoothSystem.CornerFill.Diagonal;
    Angle localRotation = xform.LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    if (cardinalDir == null)
      return (cornerFill1, cornerFill4, cornerFill3, cornerFill2);
    if (cardinalDir == 4)
      return (cornerFill3, cornerFill2, cornerFill1, cornerFill4);
    return cardinalDir == 6 ? (cornerFill2, cornerFill1, cornerFill4, cornerFill3) : (cornerFill4, cornerFill3, cornerFill2, cornerFill1);
  }

  private void InitializeEdge()
  {
    this._cmIconSmoothQuery = this.GetEntityQuery<CMIconSmoothComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SmoothEdgeComponent, ComponentStartup>(new ComponentEventHandler<SmoothEdgeComponent, ComponentStartup>((object) this, __methodptr(OnEdgeStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SmoothEdgeComponent, ComponentShutdown>(new ComponentEventHandler<SmoothEdgeComponent, ComponentShutdown>((object) this, __methodptr(OnEdgeShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnEdgeStartup(EntityUid uid, SmoothEdgeComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.South, new Vector2(0.0f, -1f));
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.East, new Vector2(1f, 0.0f));
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.North, new Vector2(0.0f, 1f));
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.West, new Vector2(-1f, 0.0f));
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.South, false);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.East, false);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.North, false);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.West, false);
  }

  private void OnEdgeShutdown(EntityUid uid, SmoothEdgeComponent component, ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.South);
    this._sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.East);
    this._sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.North);
    this._sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) IconSmoothSystem.EdgeLayer.West);
  }

  private void CalculateEdge(
    EntityUid uid,
    DirectionFlag directions,
    SpriteComponent? sprite = null,
    SmoothEdgeComponent? component = null)
  {
    if (!this.Resolve<SpriteComponent, SmoothEdgeComponent>(uid, ref sprite, ref component, false))
      return;
    for (int y = 0; y < 4; ++y)
    {
      DirectionFlag direction = (DirectionFlag) (int) (sbyte) Math.Pow(2.0, (double) y);
      IconSmoothSystem.EdgeLayer edge = this.GetEdge(direction);
      if ((direction & directions) != null)
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) edge, false);
      else
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) edge, true);
    }
  }

  private IconSmoothSystem.EdgeLayer GetEdge(DirectionFlag direction)
  {
    switch (direction - 1)
    {
      case 0:
        return IconSmoothSystem.EdgeLayer.South;
      case 1:
        return IconSmoothSystem.EdgeLayer.East;
      case 2:
        throw new ArgumentOutOfRangeException();
      case 3:
        return IconSmoothSystem.EdgeLayer.North;
      default:
        if (direction == 8)
          return IconSmoothSystem.EdgeLayer.West;
        goto case 2;
    }
  }

  [Flags]
  private enum CardinalConnectDirs : byte
  {
    None = 0,
    North = 1,
    South = 2,
    East = 4,
    West = 8,
  }

  [Flags]
  private enum CornerFill : byte
  {
    None = 0,
    CounterClockwise = 1,
    Diagonal = 2,
    Clockwise = 4,
  }

  private enum CornerLayers : byte
  {
    SE,
    NE,
    NW,
    SW,
  }

  private enum EdgeLayer : byte
  {
    South,
    East,
    North,
    West,
  }
}
