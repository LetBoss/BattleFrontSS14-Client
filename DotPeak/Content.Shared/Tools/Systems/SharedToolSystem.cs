// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Systems.SharedToolSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Tools;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Systems;

public abstract class SharedToolSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private IMapManager _mapManager;
  [Robust.Shared.IoC.Dependency]
  private IPrototypeManager _protoMan;
  [Robust.Shared.IoC.Dependency]
  protected ISharedAdminLogManager AdminLogger;
  [Robust.Shared.IoC.Dependency]
  private ITileDefinitionManager _tileDefManager;
  [Robust.Shared.IoC.Dependency]
  private SharedAudioSystem _audioSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Robust.Shared.IoC.Dependency]
  protected SharedInteractionSystem InteractionSystem;
  [Robust.Shared.IoC.Dependency]
  protected ItemToggleSystem ItemToggle;
  [Robust.Shared.IoC.Dependency]
  private SharedMapSystem _maps;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popup;
  [Robust.Shared.IoC.Dependency]
  protected SharedSolutionContainerSystem SolutionContainerSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedTransformSystem _transformSystem;
  [Robust.Shared.IoC.Dependency]
  private TileSystem _tiles;
  [Robust.Shared.IoC.Dependency]
  private TurfSystem _turfs;
  public const string CutQuality = "Cutting";
  public const string PulseQuality = "Pulsing";
  [Robust.Shared.IoC.Dependency]
  private INetManager _net;
  [Robust.Shared.IoC.Dependency]
  private ActionBlockerSystem _actionBlocker;

  public override void Initialize()
  {
    this.InitializeMultipleTool();
    this.InitializeTile();
    this.InitializeWelder();
    this.SubscribeLocalEvent<ToolComponent, SharedToolSystem.ToolDoAfterEvent>(new ComponentEventHandler<ToolComponent, SharedToolSystem.ToolDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<ToolComponent, ExaminedEvent>(new EntityEventRefHandler<ToolComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnDoAfter(EntityUid uid, ToolComponent tool, SharedToolSystem.ToolDoAfterEvent args)
  {
    if (!args.Cancelled)
      this.PlayToolSound(uid, tool, new EntityUid?(args.User), args.Predicted);
    DoAfterEvent wrappedEvent = args.WrappedEvent;
    wrappedEvent.DoAfter = args.DoAfter;
    if (args.OriginalTarget.HasValue)
      this.RaiseLocalEvent(this.GetEntity(args.OriginalTarget.Value), (object) wrappedEvent);
    else
      this.RaiseLocalEvent((object) wrappedEvent);
  }

  private void OnExamine(Entity<ToolComponent> ent, ref ExaminedEvent args)
  {
    if (ent.Comp.Qualities.Count == 0)
      return;
    FormattedMessage message = new FormattedMessage();
    List<string> values = new List<string>();
    foreach (string quality in ent.Comp.Qualities)
    {
      ToolQualityPrototype prototype;
      if (this._protoMan.TryIndex<ToolQualityPrototype>(quality ?? string.Empty, out prototype))
        values.Add(this.Loc.GetString(prototype.Name));
    }
    string str = string.Join(", ", (IEnumerable<string>) values);
    message.AddMarkupPermissive(this.Loc.GetString("tool-component-qualities", ("qualities", (object) str)));
    args.PushMessage(message);
  }

  public void PlayToolSound(EntityUid uid, ToolComponent tool, EntityUid? user, bool predicted = true)
  {
    if (tool.UseSound == null)
      return;
    if (predicted)
    {
      this._audioSystem.PlayPredicted(tool.UseSound, uid, user);
    }
    else
    {
      if (!this._net.IsServer)
        return;
      this._audioSystem.PlayPvs(tool.UseSound, uid);
    }
  }

  public bool UseTool(
    EntityUid tool,
    EntityUid user,
    EntityUid? target,
    float doAfterDelay,
    [ForbidLiteral] IEnumerable<string> toolQualitiesNeeded,
    DoAfterEvent doAfterEv,
    float fuel = 0.0f,
    ToolComponent? toolComponent = null)
  {
    return this.UseTool(tool, user, target, TimeSpan.FromSeconds((double) doAfterDelay), toolQualitiesNeeded, doAfterEv, out DoAfterId? _, fuel, toolComponent);
  }

  public bool UseTool(
    EntityUid tool,
    EntityUid user,
    EntityUid? target,
    TimeSpan delay,
    [ForbidLiteral] IEnumerable<string> toolQualitiesNeeded,
    DoAfterEvent doAfterEv,
    out DoAfterId? id,
    float fuel = 0.0f,
    ToolComponent? toolComponent = null,
    DuplicateConditions duplicateCondition = DuplicateConditions.None,
    bool predicted = true)
  {
    id = new DoAfterId?();
    if (!this.Resolve<ToolComponent>(tool, ref toolComponent, false) || !this.CanStartToolUse(tool, user, target, fuel, toolQualitiesNeeded, toolComponent))
      return false;
    RMCToolUseEvent args = new RMCToolUseEvent(user, delay);
    this.RaiseLocalEvent<RMCToolUseEvent>(tool, ref args);
    if (args.Handled)
      delay = args.Delay;
    SharedToolSystem.ToolDoAfterEvent @event = new SharedToolSystem.ToolDoAfterEvent(fuel, doAfterEv, this.GetNetEntity(target))
    {
      Predicted = predicted
    };
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay / (double) toolComponent.SpeedModifier, (DoAfterEvent) @event, new EntityUid?(tool), target, new EntityUid?(tool))
    {
      BreakOnDamage = true,
      BreakOnMove = true,
      BreakOnWeightlessMove = false,
      NeedHand = tool != user,
      AttemptFrequency = (double) fuel > 0.0 ? AttemptFrequency.EveryTick : AttemptFrequency.Never,
      DuplicateCondition = duplicateCondition
    }, out id);
    return true;
  }

  public bool UseTool(
    EntityUid tool,
    EntityUid user,
    EntityUid? target,
    float doAfterDelay,
    [ForbidLiteral] string toolQualityNeeded,
    DoAfterEvent doAfterEv,
    float fuel = 0.0f,
    ToolComponent? toolComponent = null,
    DuplicateConditions duplicateCondition = DuplicateConditions.None)
  {
    return this.UseTool(tool, user, target, TimeSpan.FromSeconds((double) doAfterDelay), (IEnumerable<string>) new string[1]
    {
      toolQualityNeeded
    }, doAfterEv, out DoAfterId? _, fuel, toolComponent);
  }

  public bool HasQuality(EntityUid uid, [ForbidLiteral] string quality, ToolComponent? tool = null)
  {
    return this.Resolve<ToolComponent>(uid, ref tool, false) && tool.Qualities.Contains(quality);
  }

  public bool HasAllQualities(EntityUid uid, [ForbidLiteral] IEnumerable<string> qualities, ToolComponent? tool = null)
  {
    return this.Resolve<ToolComponent>(uid, ref tool, false) && tool.Qualities.ContainsAll(qualities);
  }

  private bool CanStartToolUse(
    EntityUid tool,
    EntityUid user,
    EntityUid? target,
    float fuel,
    IEnumerable<string> toolQualitiesNeeded,
    ToolComponent? toolComponent = null)
  {
    if (!this.Resolve<ToolComponent>(tool, ref toolComponent) || !toolComponent.Qualities.ContainsAll(toolQualitiesNeeded))
      return false;
    ToolUserAttemptUseEvent args1 = new ToolUserAttemptUseEvent(target);
    this.RaiseLocalEvent<ToolUserAttemptUseEvent>(user, ref args1);
    if (args1.Cancelled)
      return false;
    ToolUseAttemptEvent args2 = new ToolUseAttemptEvent(user, fuel);
    this.RaiseLocalEvent<ToolUseAttemptEvent>(tool, args2);
    if (args2.Cancelled)
      return false;
    if (target.HasValue)
    {
      EntityUid? nullable = target;
      EntityUid entityUid = tool;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
        this.RaiseLocalEvent<ToolUseAttemptEvent>(target.Value, args2);
    }
    return !args2.Cancelled;
  }

  public void InitializeMultipleTool()
  {
    this.SubscribeLocalEvent<MultipleToolComponent, ComponentStartup>(new ComponentEventHandler<MultipleToolComponent, ComponentStartup>(this.OnMultipleToolStartup));
    this.SubscribeLocalEvent<MultipleToolComponent, ActivateInWorldEvent>(new ComponentEventHandler<MultipleToolComponent, ActivateInWorldEvent>(this.OnMultipleToolActivated));
    this.SubscribeLocalEvent<MultipleToolComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<MultipleToolComponent, AfterAutoHandleStateEvent>(this.OnMultipleToolHandleState));
  }

  private void OnMultipleToolHandleState(
    EntityUid uid,
    MultipleToolComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.SetMultipleTool(uid, component);
  }

  private void OnMultipleToolStartup(
    EntityUid uid,
    MultipleToolComponent multiple,
    ComponentStartup args)
  {
    ToolComponent comp;
    if (!this.TryComp<ToolComponent>(uid, out comp))
      return;
    this.SetMultipleTool(uid, multiple, comp);
  }

  private void OnMultipleToolActivated(
    EntityUid uid,
    MultipleToolComponent multiple,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    args.Handled = this.CycleMultipleTool(uid, multiple, new EntityUid?(args.User));
  }

  public bool CycleMultipleTool(EntityUid uid, MultipleToolComponent? multiple = null, EntityUid? user = null)
  {
    if (!this.Resolve<MultipleToolComponent>(uid, ref multiple) || multiple.Entries.Length == 0)
      return false;
    multiple.CurrentEntry = (uint) ((ulong) (multiple.CurrentEntry + 1U) % (ulong) multiple.Entries.Length);
    this.SetMultipleTool(uid, multiple, playSound: true, user: user);
    return true;
  }

  public virtual void SetMultipleTool(
    EntityUid uid,
    MultipleToolComponent? multiple = null,
    ToolComponent? tool = null,
    bool playSound = false,
    EntityUid? user = null)
  {
    if (!this.Resolve<MultipleToolComponent, ToolComponent>(uid, ref multiple, ref tool))
      return;
    this.Dirty(uid, (IComponent) multiple);
    if ((long) multiple.Entries.Length <= (long) multiple.CurrentEntry)
    {
      multiple.CurrentQualityName = this.Loc.GetString("multiple-tool-component-no-behavior");
    }
    else
    {
      MultipleToolComponent.ToolEntry entry = multiple.Entries[(int) multiple.CurrentEntry];
      tool.UseSound = entry.UseSound;
      tool.Qualities = entry.Behavior;
      PryingComponent comp;
      if (this.TryComp<PryingComponent>(uid, out comp))
        comp.Enabled = entry.Behavior.Contains("Prying");
      if (playSound && entry.ChangeSound != null)
        this._audioSystem.PlayPredicted(entry.ChangeSound, uid, user);
      ToolQualityPrototype prototype;
      if (!this._protoMan.TryIndex<ToolQualityPrototype>(entry.Behavior.First<string>(), out prototype))
        return;
      multiple.CurrentQualityName = this.Loc.GetString(prototype.Name);
    }
  }

  public void InitializeTile()
  {
    this.SubscribeLocalEvent<ToolTileCompatibleComponent, AfterInteractEvent>(new EntityEventRefHandler<ToolTileCompatibleComponent, AfterInteractEvent>(this.OnToolTileAfterInteract));
    this.SubscribeLocalEvent<ToolTileCompatibleComponent, TileToolDoAfterEvent>(new EntityEventRefHandler<ToolTileCompatibleComponent, TileToolDoAfterEvent>(this.OnToolTileComplete));
  }

  private void OnToolTileAfterInteract(
    Entity<ToolTileCompatibleComponent> ent,
    ref AfterInteractEvent args)
  {
    if (args.Handled || args.Target.HasValue && !this.HasComp<PuddleComponent>(args.Target))
      return;
    args.Handled = this.UseToolOnTile((Entity<ToolTileCompatibleComponent, ToolComponent>) ((EntityUid) ent, (ToolTileCompatibleComponent) ent, (ToolComponent) null), args.User, args.ClickLocation);
  }

  private void OnToolTileComplete(
    Entity<ToolTileCompatibleComponent> ent,
    ref TileToolDoAfterEvent args)
  {
    ToolTileCompatibleComponent comp1 = ent.Comp;
    ToolComponent comp2;
    if (args.Handled || args.Cancelled || !this.TryComp<ToolComponent>((EntityUid) ent, out comp2))
      return;
    EntityUid entity = this.GetEntity(args.Grid);
    MapGridComponent comp3;
    if (!this.TryComp<MapGridComponent>(entity, out comp3))
    {
      this.Log.Error("Attempted use tool on a non-existent grid?");
    }
    else
    {
      TileRef tileRef = this._maps.GetTileRef(entity, comp3, args.GridTile);
      EntityCoordinates coordinates = this._maps.ToCoordinates(tileRef, comp3);
      if (comp1.RequiresUnobstructed && this._turfs.IsTileBlocked(entity, tileRef.GridIndices, CollisionGroup.MobMask) || !this.TryDeconstructWithToolQualities(tileRef, comp2.Qualities))
        return;
      ISharedAdminLogManager adminLogger = this.AdminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(27, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "player", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" used ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "ToPrettyString(ent)");
      logStringHandler.AppendLiteral(" to edit the tile at ");
      logStringHandler.AppendFormatted<EntityCoordinates>(coordinates, "coords");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.LatticeCut, LogImpact.Medium, ref local);
      args.Handled = true;
    }
  }

  private bool UseToolOnTile(
    Entity<ToolTileCompatibleComponent?, ToolComponent?> ent,
    EntityUid user,
    EntityCoordinates clickLocation)
  {
    if (!this.Resolve<ToolTileCompatibleComponent, ToolComponent>((EntityUid) ent, ref ent.Comp1, ref ent.Comp2, false))
      return false;
    ToolTileCompatibleComponent comp1 = ent.Comp1;
    ToolComponent comp2 = ent.Comp2;
    EntityUid uid;
    MapGridComponent grid;
    if (!this._mapManager.TryFindGridAt(this._transformSystem.ToMapCoordinates(clickLocation), out uid, out grid))
      return false;
    TileRef tileRef = this._maps.GetTileRef(uid, grid, clickLocation);
    ContentTileDefinition contentTileDefinition = (ContentTileDefinition) this._tileDefManager[tileRef.Tile.TypeId];
    if (!comp2.Qualities.ContainsAny((IEnumerable<string>) contentTileDefinition.DeconstructTools) || string.IsNullOrWhiteSpace(contentTileDefinition.BaseTurf) || comp1.RequiresUnobstructed && this._turfs.IsTileBlocked(uid, tileRef.GridIndices, CollisionGroup.MobMask))
      return false;
    EntityCoordinates local = this._maps.GridTileToLocal(uid, grid, tileRef.GridIndices);
    if (!this.InteractionSystem.InRangeUnobstructed(user, local))
      return false;
    TileToolDoAfterEvent doAfterEv = new TileToolDoAfterEvent(this.GetNetEntity(uid), tileRef.GridIndices);
    this.UseTool((EntityUid) ent, user, new EntityUid?((EntityUid) ent), comp1.Delay, (IEnumerable<string>) comp2.Qualities, (DoAfterEvent) doAfterEv, out DoAfterId? _, toolComponent: comp2);
    return true;
  }

  public bool TryDeconstructWithToolQualities(
    TileRef tileRef,
    PrototypeFlags<ToolQualityPrototype> withToolQualities)
  {
    ContentTileDefinition contentTileDefinition = (ContentTileDefinition) this._tileDefManager[tileRef.Tile.TypeId];
    if (!withToolQualities.ContainsAny((IEnumerable<string>) contentTileDefinition.DeconstructTools))
      return false;
    return this._net.IsClient || this._tiles.DeconstructTile(tileRef);
  }

  public void InitializeWelder()
  {
    this.SubscribeLocalEvent<WelderComponent, ExaminedEvent>(new EntityEventRefHandler<WelderComponent, ExaminedEvent>(this.OnWelderExamine));
    this.SubscribeLocalEvent<WelderComponent, ToolUseAttemptEvent>((ComponentEventHandler<WelderComponent, ToolUseAttemptEvent>) ((uid, comp, ev) => this.CanCancelWelderUse((Entity<WelderComponent>) (uid, comp), ev.User, ev.Fuel, (CancellableEntityEventArgs) ev)));
    this.SubscribeLocalEvent<WelderComponent, DoAfterAttemptEvent<SharedToolSystem.ToolDoAfterEvent>>((ComponentEventHandler<WelderComponent, DoAfterAttemptEvent<SharedToolSystem.ToolDoAfterEvent>>) ((uid, comp, ev) => this.CanCancelWelderUse((Entity<WelderComponent>) (uid, comp), ev.Event.User, ev.Event.Fuel, (CancellableEntityEventArgs) ev)));
    this.SubscribeLocalEvent<WelderComponent, SharedToolSystem.ToolDoAfterEvent>(new EntityEventRefHandler<WelderComponent, SharedToolSystem.ToolDoAfterEvent>(this.OnWelderDoAfter));
    this.SubscribeLocalEvent<WelderComponent, ItemToggledEvent>(new EntityEventRefHandler<WelderComponent, ItemToggledEvent>(this.OnToggle));
    this.SubscribeLocalEvent<WelderComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<WelderComponent, ItemToggleActivateAttemptEvent>(this.OnActivateAttempt));
    this.SubscribeLocalEvent<WelderComponent, ItemToggleDeactivateAttemptEvent>(new EntityEventRefHandler<WelderComponent, ItemToggleDeactivateAttemptEvent>(this.OnDeactivateAttempt));
  }

  public void TurnOn(Entity<WelderComponent> entity, EntityUid? user)
  {
    Entity<SolutionComponent>? entity1;
    if (!this.SolutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.FuelSolutionName, out entity1, out Solution _))
      return;
    this.SolutionContainerSystem.RemoveReagent(entity1.Value, (string) entity.Comp.FuelReagent, entity.Comp.FuelLitCost);
    ISharedAdminLogManager adminLogger = this.AdminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(12, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user), nameof (user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" toggled ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity.Owner), "welder", "ToPrettyString(entity.Owner)");
    logStringHandler.AppendLiteral(" on");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.InteractActivate, LogImpact.Low, ref local);
    entity.Comp.Enabled = true;
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
  }

  public void TurnOff(Entity<WelderComponent> entity, EntityUid? user)
  {
    ISharedAdminLogManager adminLogger = this.AdminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(13, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user), nameof (user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" toggled ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity.Owner), "welder", "ToPrettyString(entity.Owner)");
    logStringHandler.AppendLiteral(" off");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.InteractActivate, LogImpact.Low, ref local);
    entity.Comp.Enabled = false;
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
  }

  public (FixedPoint2 fuel, FixedPoint2 capacity) GetWelderFuelAndCapacity(
    EntityUid uid,
    WelderComponent? welder = null,
    SolutionContainerManagerComponent? solutionContainer = null)
  {
    if (!this.Resolve<WelderComponent, SolutionContainerManagerComponent>(uid, ref welder, ref solutionContainer))
      return ();
    Solution solution;
    return !this.SolutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) (uid, solutionContainer), welder.FuelSolutionName, out Entity<SolutionComponent>? _, out solution) ? () : (solution.GetTotalPrototypeQuantity((string) welder.FuelReagent), solution.MaxVolume);
  }

  private void OnWelderExamine(Entity<WelderComponent> entity, ref ExaminedEvent args)
  {
    using (args.PushGroup("WelderComponent"))
    {
      if (this.ItemToggle.IsActivated((Entity<ItemToggleComponent>) entity.Owner))
        args.PushMarkup(this.Loc.GetString("welder-component-on-examine-welder-lit-message"));
      else
        args.PushMarkup(this.Loc.GetString("welder-component-on-examine-welder-not-lit-message"));
      if (!args.IsInDetailsRange)
        return;
      (FixedPoint2 fuel, FixedPoint2 capacity) = this.GetWelderFuelAndCapacity(entity.Owner, entity.Comp);
      args.PushMarkup(this.Loc.GetString("welder-component-on-examine-detailed-message", ("colorName", fuel < capacity / FixedPoint2.New(4f) ? (object) "darkorange" : (object) "orange"), ("fuelLeft", (object) fuel), ("fuelCapacity", (object) capacity), ("status", (object) string.Empty)));
    }
  }

  private void CanCancelWelderUse(
    Entity<WelderComponent> entity,
    EntityUid user,
    float requiredFuel,
    CancellableEntityEventArgs ev)
  {
    if (!this.ItemToggle.IsActivated((Entity<ItemToggleComponent>) entity.Owner))
    {
      this._popup.PopupClient(this.Loc.GetString("welder-component-welder-not-lit-message"), (EntityUid) entity, new EntityUid?(user));
      ev.Cancel();
    }
    FixedPoint2 fuel = this.GetWelderFuelAndCapacity((EntityUid) entity).fuel;
    if (!((FixedPoint2) requiredFuel > fuel))
      return;
    this._popup.PopupClient(this.Loc.GetString("welder-component-cannot-weld-message"), (EntityUid) entity, new EntityUid?(user));
    ev.Cancel();
  }

  private void OnWelderDoAfter(
    Entity<WelderComponent> ent,
    ref SharedToolSystem.ToolDoAfterEvent args)
  {
    Entity<SolutionComponent>? entity;
    if (args.Cancelled || !this.SolutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.FuelSolutionName, out entity))
      return;
    this.SolutionContainerSystem.RemoveReagent(entity.Value, (string) ent.Comp.FuelReagent, FixedPoint2.New(args.Fuel));
  }

  private void OnToggle(Entity<WelderComponent> entity, ref ItemToggledEvent args)
  {
    if (args.Activated)
      this.TurnOn(entity, args.User);
    else
      this.TurnOff(entity, args.User);
  }

  private void OnActivateAttempt(
    Entity<WelderComponent> entity,
    ref ItemToggleActivateAttemptEvent args)
  {
    if (args.User.HasValue && !this._actionBlocker.CanComplexInteract(args.User.Value))
    {
      args.Cancelled = true;
    }
    else
    {
      Solution solution;
      if (!this.SolutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.FuelSolutionName, out Entity<SolutionComponent>? _, out solution))
      {
        args.Cancelled = true;
        args.Popup = this.Loc.GetString("welder-component-no-fuel-message");
      }
      else
      {
        FixedPoint2 prototypeQuantity = solution.GetTotalPrototypeQuantity((string) entity.Comp.FuelReagent);
        if (!(prototypeQuantity == FixedPoint2.Zero) && !(prototypeQuantity < entity.Comp.FuelLitCost))
          return;
        args.Popup = this.Loc.GetString("welder-component-no-fuel-message");
        args.Cancelled = true;
      }
    }
  }

  private void OnDeactivateAttempt(
    Entity<WelderComponent> entity,
    ref ItemToggleDeactivateAttemptEvent args)
  {
    if (!args.User.HasValue || this._actionBlocker.CanComplexInteract(args.User.Value))
      return;
    args.Cancelled = true;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class ToolDoAfterEvent : 
    DoAfterEvent,
    ISerializationGenerated<SharedToolSystem.ToolDoAfterEvent>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, false, false, null)]
    public float Fuel;
    [DataField("target", false, 1, false, false, null)]
    public NetEntity? OriginalTarget;
    [DataField("wrappedEvent", false, 1, false, false, null)]
    public DoAfterEvent WrappedEvent;
    [DataField(null, false, 1, false, false, null)]
    public bool Predicted = true;

    private ToolDoAfterEvent()
    {
    }

    public ToolDoAfterEvent(float fuel, DoAfterEvent wrappedEvent, NetEntity? originalTarget)
    {
      this.Fuel = fuel;
      this.WrappedEvent = wrappedEvent;
      this.OriginalTarget = originalTarget;
    }

    public override DoAfterEvent Clone()
    {
      DoAfterEvent wrappedEvent = this.WrappedEvent.Clone();
      return wrappedEvent == this.WrappedEvent ? (DoAfterEvent) this : (DoAfterEvent) new SharedToolSystem.ToolDoAfterEvent(this.Fuel, wrappedEvent, this.OriginalTarget);
    }

    public override bool IsDuplicate(DoAfterEvent other)
    {
      return other is SharedToolSystem.ToolDoAfterEvent toolDoAfterEvent && this.WrappedEvent.IsDuplicate(toolDoAfterEvent.WrappedEvent);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedToolSystem.ToolDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DoAfterEvent target1 = (DoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedToolSystem.ToolDoAfterEvent) target1;
      if (serialization.TryCustomCopy<SharedToolSystem.ToolDoAfterEvent>(this, ref target, hookCtx, false, context))
        return;
      float target2 = 0.0f;
      if (!serialization.TryCustomCopy<float>(this.Fuel, ref target2, hookCtx, false, context))
        target2 = this.Fuel;
      target.Fuel = target2;
      NetEntity? target3 = new NetEntity?();
      if (!serialization.TryCustomCopy<NetEntity?>(this.OriginalTarget, ref target3, hookCtx, false, context))
        target3 = serialization.CreateCopy<NetEntity?>(this.OriginalTarget, hookCtx, context);
      target.OriginalTarget = target3;
      DoAfterEvent target4 = (DoAfterEvent) null;
      if (this.WrappedEvent == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<DoAfterEvent>(this.WrappedEvent, ref target4, hookCtx, true, context))
        target4 = serialization.CreateCopy<DoAfterEvent>(this.WrappedEvent, hookCtx, context);
      target.WrappedEvent = target4;
      bool target5 = false;
      if (!serialization.TryCustomCopy<bool>(this.Predicted, ref target5, hookCtx, false, context))
        target5 = this.Predicted;
      target.Predicted = target5;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedToolSystem.ToolDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref DoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedToolSystem.ToolDoAfterEvent target1 = (SharedToolSystem.ToolDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (DoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedToolSystem.ToolDoAfterEvent target1 = (SharedToolSystem.ToolDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedToolSystem.ToolDoAfterEvent DoAfterEvent.Instantiate()
    {
      return new SharedToolSystem.ToolDoAfterEvent();
    }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class LatticeCuttingCompleteEvent : 
    DoAfterEvent,
    ISerializationGenerated<SharedToolSystem.LatticeCuttingCompleteEvent>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, true, false, null)]
    public NetCoordinates Coordinates;

    private LatticeCuttingCompleteEvent()
    {
    }

    public LatticeCuttingCompleteEvent(NetCoordinates coordinates)
    {
      this.Coordinates = coordinates;
    }

    public override DoAfterEvent Clone() => (DoAfterEvent) this;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedToolSystem.LatticeCuttingCompleteEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DoAfterEvent target1 = (DoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedToolSystem.LatticeCuttingCompleteEvent) target1;
      if (serialization.TryCustomCopy<SharedToolSystem.LatticeCuttingCompleteEvent>(this, ref target, hookCtx, false, context))
        return;
      NetCoordinates target2 = new NetCoordinates();
      if (!serialization.TryCustomCopy<NetCoordinates>(this.Coordinates, ref target2, hookCtx, false, context))
        target2 = serialization.CreateCopy<NetCoordinates>(this.Coordinates, hookCtx, context);
      target.Coordinates = target2;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedToolSystem.LatticeCuttingCompleteEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref DoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedToolSystem.LatticeCuttingCompleteEvent target1 = (SharedToolSystem.LatticeCuttingCompleteEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (DoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedToolSystem.LatticeCuttingCompleteEvent target1 = (SharedToolSystem.LatticeCuttingCompleteEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedToolSystem.LatticeCuttingCompleteEvent DoAfterEvent.Instantiate()
    {
      return new SharedToolSystem.LatticeCuttingCompleteEvent();
    }
  }
}
