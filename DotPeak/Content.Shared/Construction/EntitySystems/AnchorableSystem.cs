// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.EntitySystems.AnchorableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Construction;
using Content.Shared.Administration.Logs;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.EntitySystems;

public sealed class AnchorableSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  private EntityQuery<PhysicsComponent> _physicsQuery;
  public readonly ProtoId<TagPrototype> Unstackable = ProtoId<TagPrototype>.op_Implicit(nameof (Unstackable));

  public virtual void Initialize()
  {
    base.Initialize();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnchorableComponent, InteractUsingEvent>(new ComponentEventHandler<AnchorableComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), new Type[1]
    {
      typeof (ItemSlotsSystem)
    }, new Type[1]{ typeof (SharedConstructionSystem) });
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnchorableComponent, AnchorableSystem.TryAnchorCompletedEvent>(new ComponentEventHandler<AnchorableComponent, AnchorableSystem.TryAnchorCompletedEvent>((object) this, __methodptr(OnAnchorComplete)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnchorableComponent, AnchorableSystem.TryUnanchorCompletedEvent>(new ComponentEventHandler<AnchorableComponent, AnchorableSystem.TryUnanchorCompletedEvent>((object) this, __methodptr(OnUnanchorComplete)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnchorableComponent, ExaminedEvent>(new ComponentEventHandler<AnchorableComponent, ExaminedEvent>((object) this, __methodptr(OnAnchoredExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnchorableComponent, ComponentStartup>(new ComponentEventHandler<AnchorableComponent, ComponentStartup>((object) this, __methodptr(OnAnchorStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnchorableComponent, AnchorStateChangedEvent>(new ComponentEventHandler<AnchorableComponent, AnchorStateChangedEvent>((object) this, __methodptr(OnAnchorStateChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAnchorStartup(EntityUid uid, AnchorableComponent comp, ComponentStartup args)
  {
    this._appearance.SetData(uid, (Enum) AnchorVisuals.Anchored, (object) this.Transform(uid).Anchored, (AppearanceComponent) null);
  }

  private void OnAnchorStateChange(
    EntityUid uid,
    AnchorableComponent comp,
    AnchorStateChangedEvent args)
  {
    this._appearance.SetData(uid, (Enum) AnchorVisuals.Anchored, (object) ((AnchorStateChangedEvent) ref args).Anchored, (AppearanceComponent) null);
  }

  private void TryUnAnchor(
    EntityUid uid,
    EntityUid userUid,
    EntityUid usingUid,
    AnchorableComponent? anchorable = null,
    TransformComponent? transform = null,
    ToolComponent? usingTool = null)
  {
    if (!this.Resolve<AnchorableComponent, TransformComponent>(uid, ref anchorable, ref transform, true) || !this.Resolve<ToolComponent>(usingUid, ref usingTool, true) || !this.Valid(uid, userUid, usingUid, false))
      return;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(29, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userUid)), "user", "ToPrettyString(userUid)");
    logStringHandler.AppendLiteral(" is trying to unanchor ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" from ");
    logStringHandler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "targetlocation", "transform.Coordinates");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Anchor, LogImpact.Low, ref local);
    this._tool.UseTool(usingUid, userUid, new EntityUid?(uid), anchorable.Delay, (IEnumerable<string>) usingTool.Qualities, (DoAfterEvent) new AnchorableSystem.TryUnanchorCompletedEvent());
  }

  private void OnInteractUsing(
    EntityUid uid,
    AnchorableComponent anchorable,
    InteractUsingEvent args)
  {
    ToolComponent toolComponent;
    if (args.Handled || !this.TryComp<ToolComponent>(args.Used, ref toolComponent) || !this._tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(anchorable.Tool), toolComponent))
      return;
    args.Handled = true;
    this.TryToggleAnchor(uid, args.User, args.Used, anchorable, usingTool: toolComponent);
  }

  private void OnAnchoredExamine(EntityUid uid, AnchorableComponent component, ExaminedEvent args)
  {
    if (component.Flags == AnchorableFlags.None)
    {
      args.PushMarkup(this.Loc.GetString("rmc-construction-non-anchorable"));
    }
    else
    {
      string str = this.Comp<TransformComponent>(uid).Anchored ? "examinable-anchored" : "examinable-unanchored";
      args.PushMarkup(this.Loc.GetString(str, ("target", (object) uid)));
    }
  }

  private void OnUnanchorComplete(
    EntityUid uid,
    AnchorableComponent component,
    AnchorableSystem.TryUnanchorCompletedEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    TransformComponent transformComponent = this.Transform(uid);
    this.RaiseLocalEvent<BeforeUnanchoredEvent>(uid, new BeforeUnanchoredEvent(args.User, valueOrDefault), false);
    this._transformSystem.Unanchor(uid, transformComponent, true);
    this.RaiseLocalEvent<UserUnanchoredEvent>(uid, new UserUnanchoredEvent(args.User, valueOrDefault), false);
    this._popup.PopupClient(this.Loc.GetString("anchorable-unanchored"), uid, new EntityUid?(args.User));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(19, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" unanchored ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "anchored", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" using ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault)), "using", "ToPrettyString(used)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Unanchor, LogImpact.Low, ref local);
  }

  private void OnAnchorComplete(
    EntityUid uid,
    AnchorableComponent component,
    AnchorableSystem.TryAnchorCompletedEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    TransformComponent transformComponent = this.Transform(uid);
    PhysicsComponent anchorBody;
    if (this.TryComp<PhysicsComponent>(uid, ref anchorBody) && !this.TileFree(transformComponent.Coordinates, anchorBody, new EntityUid?(uid)))
    {
      this._popup.PopupClient(this.Loc.GetString("anchorable-occupied"), uid, new EntityUid?(args.User));
    }
    else
    {
      Angle localRotation = transformComponent.LocalRotation;
      transformComponent.LocalRotation = Angle.op_Implicit(Math.Round(Angle.op_Implicit(localRotation) / (Math.PI / 2.0)) * (Math.PI / 2.0));
      PullableComponent pullable;
      if (this.TryComp<PullableComponent>(uid, ref pullable) && pullable.Puller.HasValue)
        this._pulling.TryStopPull(uid, pullable);
      if (component.Snap)
      {
        EntityCoordinates grid = transformComponent.Coordinates.SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
        if (this.AnyUnstackable(uid, grid))
        {
          this._popup.PopupClient(this.Loc.GetString("construction-step-condition-no-unstackable-in-tile"), uid, new EntityUid?(args.User));
          return;
        }
        this._transformSystem.SetCoordinates(uid, grid);
      }
      this.RaiseLocalEvent<BeforeAnchoredEvent>(uid, new BeforeAnchoredEvent(args.User, valueOrDefault), false);
      if (!transformComponent.Anchored)
        this._transformSystem.AnchorEntity(uid, transformComponent);
      this.RaiseLocalEvent<UserAnchoredEvent>(uid, new UserAnchoredEvent(args.User, valueOrDefault), false);
      this._popup.PopupClient(this.Loc.GetString("anchorable-anchored"), uid, new EntityUid?(args.User));
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(17, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" anchored ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "anchored", "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" using ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault)), "using", "ToPrettyString(used)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Anchor, LogImpact.Low, ref local);
    }
  }

  public void TryToggleAnchor(
    EntityUid uid,
    EntityUid userUid,
    EntityUid usingUid,
    AnchorableComponent? anchorable = null,
    TransformComponent? transform = null,
    PullableComponent? pullable = null,
    ToolComponent? usingTool = null)
  {
    if (!this.Resolve(uid, ref transform, true))
      return;
    if (transform.Anchored)
      this.TryUnAnchor(uid, userUid, usingUid, anchorable, transform, usingTool);
    else
      this.TryAnchor(uid, userUid, usingUid, anchorable, transform, pullable, usingTool);
  }

  private void TryAnchor(
    EntityUid uid,
    EntityUid userUid,
    EntityUid usingUid,
    AnchorableComponent? anchorable = null,
    TransformComponent? transform = null,
    PullableComponent? pullable = null,
    ToolComponent? usingTool = null)
  {
    if (!this.Resolve<AnchorableComponent, TransformComponent>(uid, ref anchorable, ref transform, true))
      return;
    this.Resolve<PullableComponent>(uid, ref pullable, false);
    if (!this.Resolve<ToolComponent>(usingUid, ref usingTool, true) || !this.Valid(uid, userUid, usingUid, true, anchorable, usingTool))
      return;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(25, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userUid)), "user", "ToPrettyString(userUid)");
    logStringHandler.AppendLiteral(" is trying to anchor ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "targetlocation", "transform.Coordinates");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Anchor, LogImpact.Low, ref local);
    PhysicsComponent anchorBody;
    if (this.TryComp<PhysicsComponent>(uid, ref anchorBody) && !this.TileFree(transform.Coordinates, anchorBody, new EntityUid?(uid)))
      this._popup.PopupClient(this.Loc.GetString("anchorable-occupied"), uid, new EntityUid?(userUid));
    else if (this.AnyUnstackable(uid, transform.Coordinates))
      this._popup.PopupClient(this.Loc.GetString("construction-step-condition-no-unstackable-in-tile"), uid, new EntityUid?(userUid));
    else
      this._tool.UseTool(usingUid, userUid, new EntityUid?(uid), anchorable.Delay, (IEnumerable<string>) usingTool.Qualities, (DoAfterEvent) new AnchorableSystem.TryAnchorCompletedEvent());
  }

  private bool Valid(
    EntityUid uid,
    EntityUid userUid,
    EntityUid usingUid,
    bool anchoring,
    AnchorableComponent? anchorable = null,
    ToolComponent? usingTool = null)
  {
    if (!this.Resolve<AnchorableComponent>(uid, ref anchorable, true) || !this.Resolve<ToolComponent>(usingUid, ref usingTool, true) || anchoring && (anchorable.Flags & AnchorableFlags.Anchorable) == AnchorableFlags.None || !anchoring && (anchorable.Flags & AnchorableFlags.Unanchorable) == AnchorableFlags.None)
      return false;
    BaseAnchoredAttemptEvent anchoredAttemptEvent = anchoring ? (BaseAnchoredAttemptEvent) new AnchorAttemptEvent(userUid, usingUid) : (BaseAnchoredAttemptEvent) new UnanchorAttemptEvent(userUid, usingUid);
    if (anchoring)
      this.RaiseLocalEvent<AnchorAttemptEvent>(uid, (AnchorAttemptEvent) anchoredAttemptEvent, false);
    else
      this.RaiseLocalEvent<UnanchorAttemptEvent>(uid, (UnanchorAttemptEvent) anchoredAttemptEvent, false);
    anchorable.Delay += anchoredAttemptEvent.Delay;
    return !anchoredAttemptEvent.Cancelled;
  }

  public bool TileFree(
    EntityCoordinates coordinates,
    PhysicsComponent anchorBody,
    EntityUid? anchoringEntity = null)
  {
    EntityUid? grid = this._transformSystem.GetGrid(coordinates);
    MapGridComponent mapGridComponent;
    if (!this.TryComp<MapGridComponent>(grid, ref mapGridComponent))
      return false;
    Vector2i gridIndices = this._map.TileIndicesFor(Entity<MapGridComponent>.op_Implicit((grid.Value, mapGridComponent)), coordinates);
    return this.TileFree(Entity<MapGridComponent>.op_Implicit((grid.Value, mapGridComponent)), gridIndices, anchorBody.CollisionLayer, anchorBody.CollisionMask, anchoringEntity);
  }

  public bool TileFree(
    Entity<MapGridComponent> grid,
    Vector2i gridIndices,
    int collisionLayer = 0,
    int collisionMask = 0,
    EntityUid? anchoringEntity = null)
  {
    AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), grid.Comp, gridIndices);
    EntityUid? nullable;
    while (((AnchoredEntitiesEnumerator) ref entitiesEnumerator).MoveNext(ref nullable))
    {
      PhysicsComponent physicsComponent;
      if (this._physicsQuery.TryGetComponent(nullable, ref physicsComponent) && physicsComponent.CanCollide && physicsComponent.Hard && ((physicsComponent.CollisionMask & collisionLayer) != 0 || (physicsComponent.CollisionLayer & collisionMask) != 0))
      {
        if (anchoringEntity.HasValue)
        {
          RMCCheckTileFreeEvent checkTileFreeEvent = new RMCCheckTileFreeEvent(nullable.Value);
          this.RaiseLocalEvent<RMCCheckTileFreeEvent>(anchoringEntity.Value, ref checkTileFreeEvent, false);
          if (checkTileFreeEvent.IsTileFree)
            continue;
        }
        return false;
      }
    }
    return true;
  }

  [Obsolete("Use the Entity<MapGridComponent> version")]
  public bool TileFree(
    MapGridComponent grid,
    Vector2i gridIndices,
    int collisionLayer = 0,
    int collisionMask = 0)
  {
    return this.TileFree(Entity<MapGridComponent>.op_Implicit((((Component) grid).Owner, grid)), gridIndices, collisionLayer, collisionMask);
  }

  public bool AnyUnstackable(EntityUid uid, EntityCoordinates location)
  {
    return this._tagSystem.HasTag(uid, this.Unstackable) && this.AnyUnstackablesAnchoredAt(location);
  }

  public bool AnyUnstackablesAnchoredAt(EntityCoordinates location)
  {
    EntityUid? grid = this._transformSystem.GetGrid(location);
    MapGridComponent mapGridComponent;
    if (!this.TryComp<MapGridComponent>(grid, ref mapGridComponent))
      return false;
    AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(grid.Value, mapGridComponent, this._map.LocalToTile(grid.Value, mapGridComponent, location));
    EntityUid? nullable;
    while (((AnchoredEntitiesEnumerator) ref entitiesEnumerator).MoveNext(ref nullable))
    {
      if (this._tagSystem.HasTag(nullable.Value, this.Unstackable))
        return true;
    }
    return false;
  }

  [NetSerializable]
  [Serializable]
  private sealed class TryUnanchorCompletedEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<AnchorableSystem.TryUnanchorCompletedEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref AnchorableSystem.TryUnanchorCompletedEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (AnchorableSystem.TryUnanchorCompletedEvent) target1;
      serialization.TryCustomCopy<AnchorableSystem.TryUnanchorCompletedEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref AnchorableSystem.TryUnanchorCompletedEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      AnchorableSystem.TryUnanchorCompletedEvent target1 = (AnchorableSystem.TryUnanchorCompletedEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      AnchorableSystem.TryUnanchorCompletedEvent target1 = (AnchorableSystem.TryUnanchorCompletedEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual AnchorableSystem.TryUnanchorCompletedEvent SimpleDoAfterEvent.Instantiate()
    {
      return new AnchorableSystem.TryUnanchorCompletedEvent();
    }
  }

  [NetSerializable]
  [Serializable]
  private sealed class TryAnchorCompletedEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<AnchorableSystem.TryAnchorCompletedEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref AnchorableSystem.TryAnchorCompletedEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (AnchorableSystem.TryAnchorCompletedEvent) target1;
      serialization.TryCustomCopy<AnchorableSystem.TryAnchorCompletedEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref AnchorableSystem.TryAnchorCompletedEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      AnchorableSystem.TryAnchorCompletedEvent target1 = (AnchorableSystem.TryAnchorCompletedEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      AnchorableSystem.TryAnchorCompletedEvent target1 = (AnchorableSystem.TryAnchorCompletedEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual AnchorableSystem.TryAnchorCompletedEvent SimpleDoAfterEvent.Instantiate()
    {
      return new AnchorableSystem.TryAnchorCompletedEvent();
    }
  }
}
