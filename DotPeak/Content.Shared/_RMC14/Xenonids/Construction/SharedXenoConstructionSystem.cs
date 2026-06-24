// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.SharedXenoConstructionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Sentry;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Buckle.Components;
using Content.Shared.Climbing.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

public sealed class SharedXenoConstructionSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedXenoAnnounceSystem _announce;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ISharedAdminLogManager _adminLogs;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private IMapManager _map;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private QueenEyeSystem _queenEye;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private XenoNestSystem _xenoNest;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private SharedXenoWeedsSystem _xenoWeeds;
  [Dependency]
  private ITileDefinitionManager _tile;
  private static readonly ProtoId<TagPrototype> AirlockTag = (ProtoId<TagPrototype>) "Airlock";
  private static readonly ProtoId<TagPrototype> StructureTag = (ProtoId<TagPrototype>) "Structure";
  private static readonly ProtoId<TagPrototype> PlatformTag = (ProtoId<TagPrototype>) "Platform";
  private static readonly ImmutableArray<Direction> Directions = ((IEnumerable<Direction>) Enum.GetValues<Direction>()).Where<Direction>((Func<Direction, bool>) (d => d != -1)).ToImmutableArray<Direction>();
  private Robust.Shared.GameObjects.EntityQuery<BlockXenoConstructionComponent> _blockXenoConstructionQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoConstructionSupportComponent> _constructionSupportQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoConstructionRequiresSupportComponent> _constructionRequiresSupportQuery;
  private Robust.Shared.GameObjects.EntityQuery<HiveConstructionNodeComponent> _hiveConstructionNodeQuery;
  private Robust.Shared.GameObjects.EntityQuery<SentryComponent> _sentryQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _transformQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoConstructComponent> _xenoConstructQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoEggComponent> _xenoEggQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoTunnelComponent> _xenoTunnelQuery;
  private Robust.Shared.GameObjects.EntityQuery<QueenBuildingBoostComponent> _queenBoostQuery;
  private const string XenoStructuresAnimation = "RMCEffect";
  private const string XenoHiveCoreNodeId = "HiveCoreXenoConstructionNode";
  private float _densityThreshold;
  private TimeSpan _newResinPreventCollideTime;
  private readonly HashSet<EntityUid> _intersectingResin = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this._blockXenoConstructionQuery = this.GetEntityQuery<BlockXenoConstructionComponent>();
    this._constructionSupportQuery = this.GetEntityQuery<XenoConstructionSupportComponent>();
    this._constructionRequiresSupportQuery = this.GetEntityQuery<XenoConstructionRequiresSupportComponent>();
    this._hiveConstructionNodeQuery = this.GetEntityQuery<HiveConstructionNodeComponent>();
    this._sentryQuery = this.GetEntityQuery<SentryComponent>();
    this._transformQuery = this.GetEntityQuery<TransformComponent>();
    this._xenoConstructQuery = this.GetEntityQuery<XenoConstructComponent>();
    this._xenoEggQuery = this.GetEntityQuery<XenoEggComponent>();
    this._xenoTunnelQuery = this.GetEntityQuery<XenoTunnelComponent>();
    this._queenBoostQuery = this.GetEntityQuery<QueenBuildingBoostComponent>();
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoPlantWeedsActionEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoPlantWeedsActionEvent>(this.OnXenoPlantWeedsAction));
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoExpandWeedsActionEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoExpandWeedsActionEvent>(this.OnXenoExpandWeedsAction));
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoChooseStructureActionEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoChooseStructureActionEvent>(this.OnXenoChooseStructureAction));
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoSecreteStructureActionEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoSecreteStructureActionEvent>(this.OnXenoSecreteStructureAction));
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoSecreteStructureDoAfterEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoSecreteStructureDoAfterEvent>(this.OnXenoSecreteStructureDoAfter));
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoOrderConstructionActionEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoOrderConstructionActionEvent>(this.OnXenoOrderConstructionAction));
    this.SubscribeLocalEvent<XenoConstructionComponent, XenoOrderConstructionDoAfterEvent>(new EntityEventRefHandler<XenoConstructionComponent, XenoOrderConstructionDoAfterEvent>(this.OnXenoOrderConstructionDoAfter));
    this.SubscribeLocalEvent<XenoCanAddPlasmaToConstructComponent, XenoConstructionAddPlasmaDoAfterEvent>(new EntityEventRefHandler<XenoCanAddPlasmaToConstructComponent, XenoConstructionAddPlasmaDoAfterEvent>(this.OnHiveConstructionNodeAddPlasmaDoAfter));
    this.SubscribeLocalEvent<XenoChooseConstructionActionComponent, XenoConstructionChosenEvent>(new EntityEventRefHandler<XenoChooseConstructionActionComponent, XenoConstructionChosenEvent>(this.OnActionConstructionChosen));
    this.SubscribeLocalEvent<XenoConstructionActionComponent, ActionValidateEvent>(new EntityEventRefHandler<XenoConstructionActionComponent, ActionValidateEvent>(this.OnSecreteActionValidateTarget));
    this.SubscribeLocalEvent<HiveConstructionNodeComponent, ExaminedEvent>(new EntityEventRefHandler<HiveConstructionNodeComponent, ExaminedEvent>(this.OnHiveConstructionNodeExamined));
    this.SubscribeLocalEvent<HiveConstructionNodeComponent, ActivateInWorldEvent>(new EntityEventRefHandler<HiveConstructionNodeComponent, ActivateInWorldEvent>(this.OnHiveConstructionNodeActivated));
    this.SubscribeLocalEvent<RepairableXenoStructureComponent, ActivateInWorldEvent>(new EntityEventRefHandler<RepairableXenoStructureComponent, ActivateInWorldEvent>(this.OnHiveConstructionRepair));
    this.SubscribeLocalEvent<RepairableXenoStructureComponent, XenoRepairStructureDoAfterEvent>(new EntityEventRefHandler<RepairableXenoStructureComponent, XenoRepairStructureDoAfterEvent>(this.OnHiveConstructionRepairDoAfter));
    this.SubscribeLocalEvent<XenoWeedsComponent, XenoStructureRepairedEvent>(new EntityEventRefHandler<XenoWeedsComponent, XenoStructureRepairedEvent>(this.OnWeedStructureRepair));
    this.SubscribeLocalEvent<XenoConstructionSupportComponent, ComponentRemove>(new EntityEventRefHandler<XenoConstructionSupportComponent, ComponentRemove>(this.OnCheckAdjacentCollapse<ComponentRemove>));
    this.SubscribeLocalEvent<XenoConstructionSupportComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoConstructionSupportComponent, EntityTerminatingEvent>(this.OnCheckAdjacentCollapse<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<XenoAnnounceStructureDestructionComponent, DestructionEventArgs>(new EntityEventRefHandler<XenoAnnounceStructureDestructionComponent, DestructionEventArgs>(this.OnXenoStructureDestruction));
    this.SubscribeLocalEvent<DeleteXenoResinOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<DeleteXenoResinOnHitComponent, ProjectileHitEvent>(this.OnDeleteXenoResinHit));
    this.SubscribeNetworkEvent<XenoOrderConstructionClickEvent>(new EntitySessionEventHandler<XenoOrderConstructionClickEvent>(this.OnXenoOrderConstructionClick));
    this.SubscribeNetworkEvent<XenoOrderConstructionCancelEvent>(new EntitySessionEventHandler<XenoOrderConstructionCancelEvent>(this.OnXenoOrderConstructionCancel));
    this.SubscribeLocalEvent<XenoConstructComponent, MapInitEvent>(new EntityEventRefHandler<XenoConstructComponent, MapInitEvent>(this.OnXenoConstructMapInit));
    this.SubscribeLocalEvent<XenoConstructComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoConstructComponent, EntityTerminatingEvent>(this.OnXenoConstructRemoved));
    this.SubscribeLocalEvent<XenoRecentlyConstructedComponent, PreventCollideEvent>(new EntityEventRefHandler<XenoRecentlyConstructedComponent, PreventCollideEvent>(this.OnRecentlyPreventCollide));
    this.Subs.BuiEvents<XenoConstructionComponent>((object) XenoChooseStructureUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoConstructionComponent>) (subs => subs.Event<XenoChooseStructureBuiMsg>(new EntityEventRefHandler<XenoConstructionComponent, XenoChooseStructureBuiMsg>(this.OnXenoChooseStructureBui))));
    this.Subs.BuiEvents<XenoConstructionComponent>((object) XenoOrderConstructionUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoConstructionComponent>) (subs => subs.Event<XenoOrderConstructionBuiMsg>(new EntityEventRefHandler<XenoConstructionComponent, XenoOrderConstructionBuiMsg>(this.OnXenoOrderConstructionBui))));
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCResinConstructionDensityCostIncreaseThreshold, (Action<float>) (v => this._densityThreshold = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCNewResinPreventCollideTimeSeconds, (Action<int>) (v => this._newResinPreventCollideTime = TimeSpan.FromSeconds((long) v)), true);
  }

  private void OnXenoOrderConstructionClick(
    XenoOrderConstructionClickEvent ev,
    EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    XenoConstructionComponent comp;
    if (!this.TryComp<XenoConstructionComponent>(valueOrDefault, out comp) || !comp.OrderConstructionTargeting)
      return;
    EntProtoId? constructionChoice = comp.OrderConstructionChoice;
    EntProtoId structureId = ev.StructureId;
    if ((constructionChoice.HasValue ? (constructionChoice.GetValueOrDefault() != structureId ? 1 : 0) : 1) != 0)
      return;
    EntityCoordinates coordinates = this.GetCoordinates(ev.Target);
    if (!this.CanOrderConstructionPopup((Entity<XenoConstructionComponent>) (valueOrDefault, comp), coordinates, new EntProtoId?(ev.StructureId)))
      return;
    XenoOrderConstructionDoAfterEvent @event = new XenoOrderConstructionDoAfterEvent(ev.StructureId, ev.Target);
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, valueOrDefault, comp.OrderConstructionDelay, (DoAfterEvent) @event, new EntityUid?(valueOrDefault))
    {
      BreakOnMove = true
    }))
      return;
    comp.OrderConstructionTargeting = false;
    comp.OrderConstructionChoice = new EntProtoId?();
    if (comp.ConfirmOrderConstructionAction.HasValue)
    {
      SharedActionsSystem actions = this._actions;
      EntityUid? constructionAction = comp.ConfirmOrderConstructionAction;
      Entity<ActionComponent>? action = constructionAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) constructionAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
      actions.SetToggled(action, false);
    }
    this.Dirty(valueOrDefault, (IComponent) comp);
  }

  private void OnXenoOrderConstructionCancel(
    XenoOrderConstructionCancelEvent ev,
    EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    XenoConstructionComponent comp;
    if (!this.TryComp<XenoConstructionComponent>(valueOrDefault, out comp))
      return;
    this.CancelOrderConstructionTargeting((Entity<XenoConstructionComponent>) (valueOrDefault, comp));
  }

  private void OnXenoStructureDestruction(
    Entity<XenoAnnounceStructureDestructionComponent> ent,
    ref DestructionEventArgs args)
  {
    if (this.HasComp<HiveConstructionSuppressAnnouncementsComponent>((EntityUid) ent))
      return;
    Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (!hive1.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive1.GetValueOrDefault();
    string str1 = "Unknown";
    string str2 = "Unknown";
    Robust.Shared.Prototypes.EntityPrototype areaPrototype;
    if (this._area.TryGetArea(ent.Owner, out Entity<AreaComponent>? _, out areaPrototype))
      str1 = areaPrototype.Name;
    if (ent.Comp.StructureName == null)
    {
      Robust.Shared.Prototypes.EntityPrototype entityPrototype = this.Prototype(ent.Owner);
      if (entityPrototype != null)
        str2 = entityPrototype.Name;
    }
    else
      str2 = ent.Comp.StructureName;
    string str3 = this.Loc.GetString((string) ent.Comp.MessageID, ("location", (object) str1), ("structureName", (object) str2), ("destructionVerb", (object) ent.Comp.DestructionVerb));
    SharedXenoAnnounceSystem announce = this._announce;
    EntityUid owner = ent.Owner;
    EntityUid hive2 = (EntityUid) valueOrDefault;
    string message = str3;
    Color? nullable = new Color?(ent.Comp.MessageColor);
    PopupType? popup = new PopupType?();
    Color? color = nullable;
    announce.AnnounceToHive(owner, hive2, message, popup: popup, color: color);
  }

  private void OnXenoPlantWeedsAction(
    Entity<XenoConstructionComponent> xeno,
    ref XenoPlantWeedsActionEvent args)
  {
    EntityCoordinates grid1 = this._transform.GetMoverCoordinates((EntityUid) xeno).SnapToGrid((IEntityManager) this.EntityManager, this._map);
    EntityUid? grid2 = this._transform.GetGrid(grid1);
    if (!grid2.HasValue)
      return;
    EntityUid valueOrDefault = grid2.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      return;
    Entity<MapGridComponent> grid3 = new Entity<MapGridComponent>(valueOrDefault, comp);
    if (this._xenoWeeds.IsOnWeeds(grid3, grid1, true))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-weeds-source-already-here"), xeno.Owner, new EntityUid?(xeno.Owner));
    }
    else
    {
      Vector2i tile = this._mapSystem.CoordinatesToTile(valueOrDefault, comp, grid1);
      if (!this._xenoWeeds.CanSpreadWeedsPopup(grid3, tile, new EntityUid?((EntityUid) xeno), args.UseOnSemiWeedable, true) || !this._xenoWeeds.CanPlaceWeedsPopup((EntityUid) xeno, grid3, grid1, args.LimitDistance) || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, args.PlasmaCost))
        return;
      args.Handled = true;
      if (this._net.IsServer)
      {
        EntityUid dest = this.Spawn((string) args.Prototype, grid1);
        ISharedAdminLogManager adminLogs = this._adminLogs;
        LogStringHandler logStringHandler = new LogStringHandler(24, 3);
        logStringHandler.AppendLiteral("Xeno ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), nameof (xeno), "ToPrettyString(xeno)");
        logStringHandler.AppendLiteral(" planted weeds ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) dest), "weeds", "ToPrettyString(weeds)");
        logStringHandler.AppendLiteral(" at ");
        logStringHandler.AppendFormatted<EntityCoordinates>(grid1, "coordinates");
        ref LogStringHandler local = ref logStringHandler;
        adminLogs.Add(LogType.RMCXenoPlantWeeds, ref local);
        this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) dest);
      }
      this._audio.PlayPredicted(xeno.Comp.BuildSound, grid1, new EntityUid?((EntityUid) xeno));
    }
  }

  private void OnXenoExpandWeedsAction(
    Entity<XenoConstructionComponent> xeno,
    ref XenoExpandWeedsActionEvent args)
  {
    EntityCoordinates target = args.Target;
    EntityUid? grid1 = this._transform.GetGrid(target);
    if (!grid1.HasValue)
      return;
    EntityUid valueOrDefault = grid1.GetValueOrDefault();
    MapGridComponent comp1;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp1) || this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) xeno.Owner) && !this._queenEye.CanSeeTarget((Entity<QueenEyeActionComponent>) xeno.Owner, target))
      return;
    Entity<MapGridComponent> grid2 = new Entity<MapGridComponent>(valueOrDefault, comp1);
    Entity<XenoWeedsComponent>? weedsOnFloor = this._xenoWeeds.GetWeedsOnFloor(grid2, target);
    if (weedsOnFloor.HasValue)
    {
      XenoWeedsComponent comp2 = weedsOnFloor.GetValueOrDefault().Comp;
      if (comp2 != null && comp2.IsSource)
      {
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-weeds-source-already-here"), xeno.Owner, new EntityUid?(xeno.Owner));
        return;
      }
    }
    if (!weedsOnFloor.HasValue)
    {
      bool flag = false;
      foreach (Direction cardinalDirection in this._rmcMap.CardinalDirections)
      {
        if (this._rmcMap.HasAnchoredEntityEnumerator<XenoWeedsComponent>(target, facing: (DirectionFlag) 0))
        {
          flag = true;
          break;
        }
      }
      int num = flag ? 1 : 0;
    }
    EntProtoId prototype = !weedsOnFloor.HasValue ? args.Expand : args.Source;
    Vector2i tile = this._mapSystem.CoordinatesToTile(valueOrDefault, comp1, target);
    if (!this._xenoWeeds.CanSpreadWeedsPopup(grid2, tile, new EntityUid?((EntityUid) xeno), source: true) || !this._xenoWeeds.CanPlaceWeedsPopup((EntityUid) xeno, grid2, target, false) || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, args.PlasmaCost))
      return;
    args.Handled = true;
    if (this._net.IsServer)
    {
      EntityUid dest = this.Spawn((string) prototype, target);
      ISharedAdminLogManager adminLogs = this._adminLogs;
      LogStringHandler logStringHandler = new LogStringHandler(24, 3);
      logStringHandler.AppendLiteral("Xeno ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), nameof (xeno), "ToPrettyString(xeno)");
      logStringHandler.AppendLiteral(" planted weeds ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) dest), "weeds", "ToPrettyString(weeds)");
      logStringHandler.AppendLiteral(" at ");
      logStringHandler.AppendFormatted<EntityCoordinates>(target, "coordinates");
      ref LogStringHandler local = ref logStringHandler;
      adminLogs.Add(LogType.RMCXenoPlantWeeds, ref local);
      this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) dest);
    }
    this._audio.PlayPredicted(xeno.Comp.BuildSound, target, new EntityUid?((EntityUid) xeno));
  }

  private void OnXenoChooseStructureAction(
    Entity<XenoConstructionComponent> xeno,
    ref XenoChooseStructureActionEvent args)
  {
    args.Handled = true;
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoChooseStructureUI.Key, (EntityUid) xeno);
  }

  private void OnXenoChooseStructureBui(
    Entity<XenoConstructionComponent> xeno,
    ref XenoChooseStructureBuiMsg args)
  {
    if (!xeno.Comp.CanBuild.Contains(args.StructureId))
      return;
    xeno.Comp.BuildChoice = new EntProtoId?(args.StructureId);
    if (xeno.Comp.OrderConstructionTargeting)
    {
      xeno.Comp.OrderConstructionTargeting = false;
      if (xeno.Comp.ConfirmOrderConstructionAction.HasValue)
      {
        SharedActionsSystem actions = this._actions;
        EntityUid? constructionAction = xeno.Comp.ConfirmOrderConstructionAction;
        Entity<ActionComponent>? action = constructionAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) constructionAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
        actions.SetToggled(action, false);
      }
    }
    this.Dirty<XenoConstructionComponent>(xeno);
    XenoConstructionChosenEvent args1 = new XenoConstructionChosenEvent(args.StructureId, xeno.Owner);
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) xeno))
      this.RaiseLocalEvent<XenoConstructionChosenEvent>(entityUid, ref args1);
  }

  private void OnXenoSecreteStructureAction(
    Entity<XenoConstructionComponent> xeno,
    ref XenoSecreteStructureActionEvent args)
  {
    if (xeno.Comp.OrderConstructionTargeting)
      return;
    this.HandleSecreteResinPlacement(xeno, ref args);
  }

  private EntProtoId GetQueenVariant(EntProtoId originalId)
  {
    EntProtoId queenVariant;
    switch (originalId.Id)
    {
      case "WallXenoResin":
        queenVariant = (EntProtoId) "WallXenoResinQueen";
        break;
      case "WallXenoMembrane":
        queenVariant = (EntProtoId) "WallXenoMembraneQueen";
        break;
      case "DoorXenoResin":
        queenVariant = (EntProtoId) "DoorXenoResinQueen";
        break;
      default:
        queenVariant = originalId;
        break;
    }
    return queenVariant;
  }

  private EntProtoId GetQueenAnimationVariant(EntProtoId originalId)
  {
    EntProtoId animationVariant;
    switch (originalId.Id)
    {
      case "WallXenoResin":
        animationVariant = (EntProtoId) "WallXenoResinThick";
        break;
      case "WallXenoMembrane":
        animationVariant = (EntProtoId) "WallXenoMembraneThick";
        break;
      case "DoorXenoResin":
        animationVariant = (EntProtoId) "DoorXenoResinThick";
        break;
      default:
        animationVariant = originalId;
        break;
    }
    return animationVariant;
  }

  private void HandleSecreteResinPlacement(
    Entity<XenoConstructionComponent> xeno,
    ref XenoSecreteStructureActionEvent args)
  {
    EntityCoordinates grid = args.Target.SnapToGrid((IEntityManager) this.EntityManager, this._map);
    bool flag = this._queenBoostQuery.HasComp(xeno.Owner);
    Entity<XenoStructureUpgradeableComponent> ent;
    EntProtoId? nullable;
    if (xeno.Comp.CanUpgrade | flag && this._rmcMap.HasAnchoredEntityEnumerator<XenoStructureUpgradeableComponent>(grid, out ent, facing: (DirectionFlag) 0))
    {
      nullable = ent.Comp.To;
      if (nullable.HasValue)
      {
        EntProtoId valueOrDefault = nullable.GetValueOrDefault();
        if (this._prototype.HasIndex(valueOrDefault))
        {
          QueenBuildingBoostComponent component;
          if ((this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) xeno.Owner) ? 1 : (!flag || !this._queenBoostQuery.TryComp(xeno.Owner, out component) ? (this._transform.InRange(this._transform.GetMoverCoordinates(xeno.Owner), args.Target, xeno.Comp.BuildRange.Float()) ? 1 : 0) : (this._transform.InRange(this._transform.GetMoverCoordinates(xeno.Owner), args.Target, component.RemoteUpgradeRange) ? 1 : 0))) == 0)
            return;
          FixedPoint2 fixedPoint2 = ent.Comp.Cost;
          Entity<AreaComponent>? area;
          if (this._area.TryGetArea(grid, out area, out Robust.Shared.Prototypes.EntityPrototype _))
            fixedPoint2 = this.GetDensityCost(area.Value, xeno, fixedPoint2);
          if (!flag && !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, fixedPoint2))
            return;
          string message;
          if (!flag)
            message = $"We regurgitate some resin and thicken the {this.Name((EntityUid) ent)}, using {fixedPoint2} plasma.";
          else
            message = $"We regurgitate some resin and thicken the {this.Name((EntityUid) ent)} effortlessly.";
          this._popup.PopupClient(message, (EntityUid) ent, new EntityUid?((EntityUid) xeno));
          if (this._net.IsClient)
            return;
          this.QueueDel(new EntityUid?((EntityUid) ent));
          EntityUid dest = this.Spawn((string) valueOrDefault, grid);
          this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) dest);
          args.Handled = true;
          return;
        }
      }
    }
    nullable = xeno.Comp.BuildChoice;
    if (!nullable.HasValue)
      return;
    EntProtoId valueOrDefault1 = nullable.GetValueOrDefault();
    if (!this.CanSecreteOnTilePopup(xeno, new EntProtoId?(valueOrDefault1), args.Target, true, true))
      return;
    XenoSecreteStructureAttemptEvent args1 = new XenoSecreteStructureAttemptEvent(args.Target);
    this.RaiseLocalEvent<XenoSecreteStructureAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    string str = "RMCEffect" + (string) (flag ? this.GetQueenAnimationVariant(valueOrDefault1) : valueOrDefault1);
    NetCoordinates netCoordinates = this.GetNetCoordinates(args.Target);
    EntityCoordinates coordinates = this.GetCoordinates(netCoordinates);
    EntityUid? uid = new EntityUid?();
    float num = this.GetBuildSpeed(valueOrDefault1) ?? 1f;
    QueenBuildingBoostComponent component1;
    if (flag && this._queenBoostQuery.TryComp(xeno.Owner, out component1))
      num *= component1.BuildSpeedMultiplier;
    TimeSpan timeSpan = xeno.Comp.BuildDelay * (double) num;
    if (this._net.IsServer && this._prototype.HasIndex((EntProtoId) str))
    {
      uid = new EntityUid?(this.Spawn(str, coordinates));
      this.RaiseNetworkEvent((EntityEventArgs) new XenoConstructionAnimationStartEvent(this.GetNetEntity(uid.Value), this.GetNetEntity((EntityUid) xeno), timeSpan), Filter.PvsExcept(uid.Value));
    }
    XenoSecreteStructureDoAfterEvent @event = new XenoSecreteStructureDoAfterEvent(netCoordinates, valueOrDefault1, this.GetNetEntity(uid));
    args.Handled = true;
    if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, timeSpan, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true,
      RootEntity = true,
      CancelDuplicate = false
    }) || !uid.HasValue || !this._net.IsServer)
      return;
    this.QueueDel(uid);
  }

  public void CancelOrderConstructionTargeting(Entity<XenoConstructionComponent> xeno)
  {
    if (!xeno.Comp.OrderConstructionTargeting)
      return;
    xeno.Comp.OrderConstructionTargeting = false;
    xeno.Comp.OrderConstructionChoice = new EntProtoId?();
    if (xeno.Comp.ConfirmOrderConstructionAction.HasValue)
    {
      SharedActionsSystem actions = this._actions;
      EntityUid? constructionAction = xeno.Comp.ConfirmOrderConstructionAction;
      Entity<ActionComponent>? action = constructionAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) constructionAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
      actions.SetToggled(action, false);
    }
    this.Dirty<XenoConstructionComponent>(xeno);
  }

  private void OnXenoSecreteStructureDoAfter(
    Entity<XenoConstructionComponent> xeno,
    ref XenoSecreteStructureDoAfterEvent args)
  {
    if (this._net.IsServer && args.Effect.HasValue)
      this.QueueDel(this.GetEntity(args.Effect));
    if (args.Handled || args.Cancelled)
      return;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    if (!coordinates.IsValid((IEntityManager) this.EntityManager) || !xeno.Comp.CanBuild.Contains(args.StructureId) || !this.CanSecreteOnTilePopup(xeno, new EntProtoId?(args.StructureId), this.GetCoordinates(args.Coordinates), true, true))
      return;
    bool flag = this._queenBoostQuery.HasComp(xeno.Owner);
    Entity<AreaComponent>? area;
    if (this._area.TryGetArea(this.GetCoordinates(args.Coordinates), out area, out Robust.Shared.Prototypes.EntityPrototype _))
    {
      FixedPoint2? structurePlasmaCost = this.GetStructurePlasmaCost(args.StructureId);
      if (structurePlasmaCost.HasValue)
      {
        FixedPoint2 fixedPoint2 = structurePlasmaCost.GetValueOrDefault();
        Robust.Shared.Prototypes.EntityPrototype prototype;
        XenoConstructionPlasmaCostComponent component;
        if (area.Value.Comp.ResinConstructCount != 0 && !area.Value.Comp.Unweedable && this._prototype.TryIndex(args.StructureId, out prototype) && prototype.TryGetComponent<XenoConstructionPlasmaCostComponent>(out component, this._compFactory) && component.ScalingCost)
          fixedPoint2 = this.GetDensityCost(area.Value, xeno, fixedPoint2);
        if (!flag && !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, fixedPoint2))
          return;
      }
    }
    args.Handled = true;
    if (this._net.IsServer)
    {
      EntProtoId prototype = args.StructureId;
      if (flag)
      {
        EntProtoId queenVariant = this.GetQueenVariant(args.StructureId);
        if (this._prototype.HasIndex(queenVariant))
          prototype = queenVariant;
      }
      EntityUid entityUid = this.Spawn((string) prototype, coordinates);
      this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) entityUid);
      if (this.HasComp<XenoRecentlyBuiltPreventCollisionsComponent>(entityUid))
      {
        this._intersectingResin.Clear();
        this._entityLookup.GetEntitiesIntersecting(entityUid, this._intersectingResin);
        XenoRecentlyConstructedComponent constructedComponent = (XenoRecentlyConstructedComponent) null;
        foreach (EntityUid uid in this._intersectingResin)
        {
          if ((this.HasComp<MarineComponent>(uid) || this.HasComp<XenoComponent>(uid)) && !(uid == xeno.Owner))
          {
            if (constructedComponent == null)
              constructedComponent = this.EnsureComp<XenoRecentlyConstructedComponent>(entityUid);
            constructedComponent.StopCollide.Add(uid);
          }
        }
        if (constructedComponent != null)
        {
          constructedComponent.ExpireAt = this._timing.CurTime + this._newResinPreventCollideTime;
          this.Dirty(entityUid, (IComponent) constructedComponent);
        }
      }
      ISharedAdminLogManager adminLogs = this._adminLogs;
      LogStringHandler logStringHandler = new LogStringHandler(22, 3);
      logStringHandler.AppendLiteral("Xeno ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), nameof (xeno), "ToPrettyString(xeno)");
      logStringHandler.AppendLiteral(" constructed ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "structure", "ToPrettyString(structure)");
      logStringHandler.AppendLiteral(" at ");
      logStringHandler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
      ref LogStringHandler local = ref logStringHandler;
      adminLogs.Add(LogType.RMCXenoConstruct, ref local);
    }
    this._audio.PlayPredicted(xeno.Comp.BuildSound, coordinates, new EntityUid?((EntityUid) xeno));
  }

  private void OnXenoOrderConstructionAction(
    Entity<XenoConstructionComponent> xeno,
    ref XenoOrderConstructionActionEvent args)
  {
    args.Handled = true;
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoOrderConstructionUI.Key, (EntityUid) xeno);
  }

  private void OnXenoOrderConstructionBui(
    Entity<XenoConstructionComponent> xeno,
    ref XenoOrderConstructionBuiMsg args)
  {
    if (!xeno.Comp.CanOrderConstruction.Contains(args.StructureId))
      return;
    xeno.Comp.OrderConstructionChoice = new EntProtoId?(args.StructureId);
    xeno.Comp.OrderConstructionTargeting = true;
    if (xeno.Comp.ConfirmOrderConstructionAction.HasValue)
    {
      SharedActionsSystem actions = this._actions;
      EntityUid? constructionAction = xeno.Comp.ConfirmOrderConstructionAction;
      Entity<ActionComponent>? action = constructionAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) constructionAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
      actions.SetToggled(action, true);
    }
    this.Dirty<XenoConstructionComponent>(xeno);
    XenoConstructionChosenEvent args1 = new XenoConstructionChosenEvent(args.StructureId, xeno.Owner);
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) xeno))
      this.RaiseLocalEvent<XenoConstructionChosenEvent>(entityUid, ref args1);
    this._ui.CloseUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoOrderConstructionUI.Key);
  }

  private void OnXenoOrderConstructionDoAfter(
    Entity<XenoConstructionComponent> xeno,
    ref XenoOrderConstructionDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    XenoPlasmaComponent comp1;
    Robust.Shared.Prototypes.EntityPrototype prototype1;
    if (!xeno.Comp.CanOrderConstruction.Contains(args.StructureId) || !this.CanOrderConstructionPopup(xeno, coordinates, new EntProtoId?(args.StructureId)) || !this.TryComp<XenoPlasmaComponent>((EntityUid) xeno, out comp1) || !this._prototype.TryIndex(args.StructureId, out prototype1))
      return;
    bool flag = this._queenBoostQuery.HasComp(xeno.Owner);
    HiveConstructionNodeComponent component;
    if (prototype1.TryGetComponent<HiveConstructionNodeComponent>(out component, this._compFactory) && !flag && !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, comp1), component.InitialPlasmaCost) || this._net.IsClient)
      return;
    EntityCoordinates grid = coordinates.SnapToGrid((IEntityManager) this.EntityManager, this._map);
    EntityUid entityUid = this.Spawn((string) args.StructureId, grid);
    this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) entityUid);
    ISharedAdminLogManager adminLogs = this._adminLogs;
    LogStringHandler logStringHandler = new LogStringHandler(34, 3);
    logStringHandler.AppendLiteral("Xeno ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), nameof (xeno), "ToPrettyString(xeno)");
    logStringHandler.AppendLiteral(" ordered construction of ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "structure", "ToPrettyString(structure)");
    logStringHandler.AppendLiteral(" at ");
    logStringHandler.AppendFormatted<EntityCoordinates>(grid, "coordinates");
    ref LogStringHandler local = ref logStringHandler;
    adminLogs.Add(LogType.RMCXenoOrderConstruction, ref local);
    Robust.Shared.Prototypes.EntityPrototype prototype2;
    if (!this._prototype.TryIndex(args.StructureId, out prototype2))
      return;
    HiveConstructionLimitedComponent comp2;
    int? limit;
    int? curCount;
    if (this.TryComp<HiveConstructionLimitedComponent>(entityUid, out comp2) && this.CanPlaceLimitedHiveStructure(xeno.Owner, comp2, out limit, out curCount))
    {
      int? nullable1 = limit;
      int? nullable2 = curCount;
      int? nullable3 = nullable1.HasValue & nullable2.HasValue ? new int?(nullable1.GetValueOrDefault() - nullable2.GetValueOrDefault()) : new int?();
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-order-construction-limited-structure-designated", ("construct", (object) prototype2.Name), ("remainCount", (object) nullable3), ("maxCount", (object) limit)), xeno.Owner, xeno.Owner);
    }
    string str = "Unknown";
    Robust.Shared.Prototypes.EntityPrototype areaPrototype;
    if (this._area.TryGetArea(coordinates, out Entity<AreaComponent>? _, out areaPrototype))
      str = areaPrototype.Name;
    string message = this.Loc.GetString("rmc-xeno-order-construction-structure-designated", ("construct", (object) prototype2.Name), ("area", (object) str));
    this._announce.AnnounceSameHive((Entity<HiveMemberComponent>) xeno.Owner, message, needsQueen: true);
  }

  private void OnHiveConstructionNodeAddPlasmaDoAfter(
    Entity<XenoCanAddPlasmaToConstructComponent> xeno,
    ref XenoConstructionAddPlasmaDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? nullable1 = args.Target;
    if (!nullable1.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable1.GetValueOrDefault();
    HiveConstructionNodeComponent comp1;
    TransformComponent comp2;
    XenoPlasmaComponent comp3;
    if (!this.TryComp<HiveConstructionNodeComponent>(valueOrDefault1, out comp1) || !this.TryComp(valueOrDefault1, out comp2) || !this.TryComp<XenoPlasmaComponent>((EntityUid) xeno, out comp3) || !this.InRangePopup(args.User, comp2.Coordinates, xeno.Comp.Range.Float()))
      return;
    int num = this._queenBoostQuery.HasComp(xeno.Owner) ? 1 : 0;
    FixedPoint2 b = comp1.PlasmaCost - comp1.PlasmaStored;
    FixedPoint2 fixedPoint2;
    if (num != 0)
    {
      comp1.PlasmaStored = comp1.PlasmaCost;
      fixedPoint2 = (FixedPoint2) 0;
    }
    else
    {
      FixedPoint2 plasma = FixedPoint2.Min(comp3.Plasma, b);
      if (b < FixedPoint2.Zero || comp3.Plasma < 1 || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) (args.User, comp3), plasma))
        return;
      comp1.PlasmaStored += plasma;
      fixedPoint2 = comp1.PlasmaCost - comp1.PlasmaStored;
    }
    args.Handled = true;
    ISharedAdminLogManager adminLogs1 = this._adminLogs;
    LogStringHandler logStringHandler1 = new LogStringHandler(26, 3);
    logStringHandler1.AppendLiteral("Xeno ");
    logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), nameof (xeno), "ToPrettyString(xeno)");
    logStringHandler1.AppendLiteral(" added plasma to ");
    logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault1), "target", "ToPrettyString(target)");
    logStringHandler1.AppendLiteral(" at ");
    logStringHandler1.AppendFormatted<EntityCoordinates>(comp2.Coordinates, "transform.Coordinates");
    ref LogStringHandler local1 = ref logStringHandler1;
    adminLogs1.Add(LogType.RMCXenoOrderConstructionPlasma, ref local1);
    if (comp1.PlasmaStored < comp1.PlasmaCost)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-requires-more-plasma", ("construction", (object) valueOrDefault1), ("plasma", (object) fixedPoint2)), valueOrDefault1, new EntityUid?(args.User));
    }
    else
    {
      TransformComponent component;
      if (!this._transformQuery.TryComp(xeno.Owner, out component))
        return;
      nullable1 = this._transform.GetGrid((Entity<TransformComponent>) (xeno.Owner, component));
      if (!nullable1.HasValue)
        return;
      EntityUid valueOrDefault2 = nullable1.GetValueOrDefault();
      MapGridComponent comp4;
      if (!valueOrDefault2.Valid || !this.TryComp<MapGridComponent>(valueOrDefault2, out comp4))
        return;
      if (this.HasComp<HiveConstructionRequiresHiveWeedsComponent>(valueOrDefault1) && !this._xenoWeeds.IsOnHiveWeeds((Entity<MapGridComponent>) (valueOrDefault2, comp4), valueOrDefault1.ToCoordinates()))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-requires-hive-weeds", ("choice", (object) valueOrDefault1)), valueOrDefault1, new EntityUid?(args.User));
      }
      else
      {
        if (this.HasComp<HiveConstructionRequiresSpaceComponent>(valueOrDefault1) && !this.CanPlaceSpaceRequiringStructurePopup(this._transform.GetMapCoordinates(valueOrDefault1), (Entity<MapGridComponent>) (valueOrDefault2, comp4), xeno.Owner, this.MetaData(valueOrDefault1).EntityName) || this._net.IsClient)
          return;
        EntityUid? uid = new EntityUid?();
        Robust.Shared.Prototypes.EntityPrototype prototype;
        if (this._prototype.TryIndex(comp1.Spawn, out prototype) && prototype.HasComponent<XenoWeedsComponent>())
          uid = this._xenoWeeds.GetWeedsOnFloor(comp2.Coordinates);
        EntityUid entityUid = this.Spawn((string) comp1.Spawn, comp2.Coordinates);
        Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) valueOrDefault1);
        SharedXenoHiveSystem hive2 = this._hive;
        Entity<HiveMemberComponent> member = (Entity<HiveMemberComponent>) entityUid;
        Entity<HiveComponent>? nullable2 = hive1;
        EntityUid? hive3;
        if (!nullable2.HasValue)
        {
          nullable1 = new EntityUid?();
          hive3 = nullable1;
        }
        else
          hive3 = new EntityUid?((EntityUid) nullable2.GetValueOrDefault());
        hive2.SetHive(member, hive3);
        this.QueueDel(new EntityUid?(valueOrDefault1));
        this.QueueDel(uid);
        ISharedAdminLogManager adminLogs2 = this._adminLogs;
        LogStringHandler logStringHandler2 = new LogStringHandler(55, 4);
        logStringHandler2.AppendLiteral("Xeno ");
        logStringHandler2.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), nameof (xeno), "ToPrettyString(xeno)");
        logStringHandler2.AppendLiteral(" completed construction of ");
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault1), nameof (xeno), "ToPrettyString(target)");
        logStringHandler2.AppendLiteral(" which turned into ");
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "spawn", "ToPrettyString(spawn)");
        logStringHandler2.AppendLiteral(" at ");
        logStringHandler2.AppendFormatted<EntityCoordinates>(comp2.Coordinates, "transform.Coordinates");
        ref LogStringHandler local2 = ref logStringHandler2;
        adminLogs2.Add(LogType.RMCXenoOrderConstructionComplete, ref local2);
      }
    }
  }

  private void OnActionConstructionChosen(
    Entity<XenoChooseConstructionActionComponent> xeno,
    ref XenoConstructionChosenEvent args)
  {
    XenoConstructionComponent comp;
    if (!this.TryComp<XenoConstructionComponent>(args.User, out comp) || comp.OrderConstructionTargeting)
      return;
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?((Entity<ActionComponent>) xeno.Owner));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
    if (!this._prototype.HasIndex(args.Choice))
      return;
    EntProtoId entityPrototypeId = this._queenBoostQuery.HasComp(args.User) ? this.GetQueenVariant(args.Choice) : args.Choice;
    this._actions.SetIcon(valueOrDefault.AsNullable(), (SpriteSpecifier) new SpriteSpecifier.EntityPrototype((string) entityPrototypeId));
  }

  private void OnSecreteActionValidateTarget(
    Entity<XenoConstructionActionComponent> ent,
    ref ActionValidateEvent args)
  {
    XenoConstructionComponent comp;
    if (args.Invalid || !this.TryComp<XenoConstructionComponent>(args.User, out comp))
      return;
    EntityCoordinates? coordinates = this.GetCoordinates(args.Input.EntityCoordinatesTarget);
    if (!coordinates.HasValue)
      return;
    EntityCoordinates valueOrDefault = coordinates.GetValueOrDefault();
    EntityCoordinates grid = valueOrDefault.SnapToGrid((IEntityManager) this.EntityManager, this._map);
    XenoSecreteStructureAdjustFields args1 = new XenoSecreteStructureAdjustFields(grid);
    this.RaiseLocalEvent<XenoSecreteStructureAdjustFields>(args.User, ref args1);
    bool flag = this._queenBoostQuery.HasComp(args.User);
    Entity<XenoStructureUpgradeableComponent> ent1;
    QueenBuildingBoostComponent component;
    if (ent.Comp.CanUpgrade && comp.CanUpgrade | flag && this._rmcMap.HasAnchoredEntityEnumerator<XenoStructureUpgradeableComponent>(grid, out ent1, facing: (DirectionFlag) 0) && ent1.Comp.To.HasValue && (this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) args.User) || flag && this._queenBoostQuery.TryComp(args.User, out component) && this._transform.InRange(this._transform.GetMoverCoordinates(args.User), valueOrDefault, component.RemoteUpgradeRange) || this._transform.InRange(this._transform.GetMoverCoordinates(args.User), ent1.Owner.ToCoordinates(), comp.BuildRange.Float())))
      return;
    if (comp.OrderConstructionTargeting && comp.OrderConstructionChoice.HasValue)
    {
      if (this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) args.User) && !this._queenEye.CanSeeTarget((Entity<QueenEyeActionComponent>) args.User, valueOrDefault))
      {
        args.Invalid = true;
      }
      else
      {
        if (this.CanOrderConstructionPopup((Entity<XenoConstructionComponent>) (args.User, comp), valueOrDefault, comp.OrderConstructionChoice))
          return;
        args.Invalid = true;
      }
    }
    else
    {
      if (this.CanSecreteOnTilePopup((Entity<XenoConstructionComponent>) (args.User, comp), comp.BuildChoice, valueOrDefault, ent.Comp.CheckStructureSelected, ent.Comp.CheckWeeds))
        return;
      args.Invalid = true;
    }
  }

  private void OnHiveConstructionNodeExamined(
    Entity<HiveConstructionNodeComponent> node,
    ref ExaminedEvent args)
  {
    FixedPoint2 fixedPoint2 = node.Comp.PlasmaCost - node.Comp.PlasmaStored;
    args.PushMarkup(this.Loc.GetString("cm-xeno-construction-plasma-left", ("construction", (object) node.Owner), ("plasma", (object) fixedPoint2)));
  }

  private void OnHiveConstructionNodeActivated(
    Entity<HiveConstructionNodeComponent> node,
    ref ActivateInWorldEvent args)
  {
    EntityUid user = args.User;
    FixedPoint2 b = node.Comp.PlasmaCost - node.Comp.PlasmaStored;
    XenoCanAddPlasmaToConstructComponent comp1;
    TransformComponent comp2;
    XenoPlasmaComponent comp3;
    if (!this.TryComp<XenoCanAddPlasmaToConstructComponent>(user, out comp1) || b < FixedPoint2.Zero || !this.TryComp((EntityUid) node, out comp2) || !this.TryComp<XenoPlasmaComponent>(user, out comp3) || !this.InRangePopup(user, comp2.Coordinates, comp1.Range.Float()))
      return;
    FixedPoint2 plasma = FixedPoint2.Min(comp3.Plasma, b);
    if (comp3.Plasma < 1 || !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) (user, comp3), plasma))
      return;
    XenoConstructionAddPlasmaDoAfterEvent @event = new XenoConstructionAddPlasmaDoAfterEvent();
    TimeSpan addPlasmaDelay = comp1.AddPlasmaDelay;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, addPlasmaDelay, (DoAfterEvent) @event, new EntityUid?(user), new EntityUid?((EntityUid) node))
    {
      BreakOnMove = true
    });
  }

  private void OnHiveConstructionRepair(
    Entity<RepairableXenoStructureComponent> xenoStructure,
    ref ActivateInWorldEvent args)
  {
    EntityUid user = args.User;
    FixedPoint2 fixedPoint2 = xenoStructure.Comp.PlasmaCost - xenoStructure.Comp.StoredPlasma;
    XenoConstructionComponent comp1;
    TransformComponent comp2;
    XenoPlasmaComponent comp3;
    DamageableComponent comp4;
    if (!this.TryComp<XenoConstructionComponent>(user, out comp1) || fixedPoint2 < FixedPoint2.Zero || !this.TryComp((EntityUid) xenoStructure, out comp2) || !this.TryComp<XenoPlasmaComponent>(user, out comp3) || !this.TryComp<DamageableComponent>((EntityUid) xenoStructure, out comp4))
      return;
    if (comp4.TotalDamage <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-repair-structure-no-damage-failure", ("struct", (object) xenoStructure.Owner)), xenoStructure.Owner.ToCoordinates(), new EntityUid?(user));
    }
    else
    {
      if (!this.InRangePopup(user, comp2.Coordinates, comp1.OrderConstructionRange.Float()) || comp3.Plasma < 1)
        return;
      XenoRepairStructureDoAfterEvent @event = new XenoRepairStructureDoAfterEvent();
      TimeSpan repairLength = xenoStructure.Comp.RepairLength;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, repairLength, (DoAfterEvent) @event, new EntityUid?((EntityUid) xenoStructure), new EntityUid?((EntityUid) xenoStructure))
      {
        BreakOnMove = true,
        RootEntity = true
      });
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-repair-structure-start-attempt", ("struct", (object) xenoStructure.Owner)), comp2.Coordinates, new EntityUid?(user));
    }
  }

  private void OnHiveConstructionRepairDoAfter(
    Entity<RepairableXenoStructureComponent> xenoStructure,
    ref XenoRepairStructureDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid user = args.User;
    FixedPoint2 b = xenoStructure.Comp.PlasmaCost - xenoStructure.Comp.StoredPlasma;
    XenoConstructionComponent comp1;
    TransformComponent comp2;
    XenoPlasmaComponent comp3;
    DamageableComponent comp4;
    if (!this.TryComp<XenoConstructionComponent>(user, out comp1) || b < FixedPoint2.Zero || !this.TryComp((EntityUid) xenoStructure, out comp2) || !this.TryComp<XenoPlasmaComponent>(user, out comp3) || !this.TryComp<DamageableComponent>((EntityUid) xenoStructure, out comp4) || comp4.TotalDamage <= 0)
      return;
    args.Handled = true;
    if (!this.InRangePopup(user, comp2.Coordinates, comp1.OrderConstructionRange.Float()))
      return;
    FixedPoint2 plasma = FixedPoint2.Min(comp3.Plasma, b);
    if (comp3.Plasma < 1 || !this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) (user, comp3), plasma))
      return;
    xenoStructure.Comp.StoredPlasma += plasma;
    if (xenoStructure.Comp.StoredPlasma >= xenoStructure.Comp.PlasmaCost)
    {
      xenoStructure.Comp.StoredPlasma = (FixedPoint2) 0;
      this._damageable.SetAllDamage(xenoStructure.Owner, comp4, (FixedPoint2) 0);
      XenoStructureRepairedEvent args1 = new XenoStructureRepairedEvent();
      this.RaiseLocalEvent<XenoStructureRepairedEvent>((EntityUid) xenoStructure, args1);
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-repair-structure-success", ("struct", (object) xenoStructure.Owner)), comp2.Coordinates, new EntityUid?(user));
    }
    else
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-repair-structure-insufficient-plasma-warn", ("struct", (object) xenoStructure.Owner), ("remainingPlasma", (object) (xenoStructure.Comp.PlasmaCost - xenoStructure.Comp.StoredPlasma))), xenoStructure.Owner.ToCoordinates(), new EntityUid?(user));
  }

  private void OnWeedStructureRepair(
    Entity<XenoWeedsComponent> weedsStructure,
    ref XenoStructureRepairedEvent args)
  {
    (EntityUid entityUid, XenoWeedsComponent comp) = weedsStructure;
    XenoWeedsSpreadingComponent spreadingComponent1 = this.EnsureComp<XenoWeedsSpreadingComponent>(entityUid);
    TimeSpan timeSpan = this._timing.CurTime + spreadingComponent1.RepairedSpreadDelay;
    spreadingComponent1.SpreadAt = timeSpan;
    this.Dirty(entityUid, (IComponent) spreadingComponent1);
    foreach (EntityUid uid in comp.Spread)
    {
      XenoWeedsSpreadingComponent spreadingComponent2 = this.EnsureComp<XenoWeedsSpreadingComponent>(uid);
      spreadingComponent2.SpreadAt = timeSpan;
      this.Dirty(uid, (IComponent) spreadingComponent2);
    }
  }

  private void OnCheckAdjacentCollapse<T>(Entity<XenoConstructionSupportComponent> ent, ref T args)
  {
    TransformComponent component;
    if (!this._transformQuery.TryComp((EntityUid) ent, out component))
      return;
    EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) ((EntityUid) ent, component));
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent comp;
    if (!valueOrDefault.Valid || !this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((Entity<TransformComponent>) ((EntityUid) ent, component));
    Vector2i pos = this._mapSystem.TileIndicesFor(valueOrDefault, comp, mapCoordinates);
    for (int index = 0; index < 4; ++index)
    {
      AtmosDirection dir = (AtmosDirection) (1 << index);
      Vector2i vector2i = pos.Offset(dir);
      AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, comp, vector2i);
      EntityUid? uid;
      while (entitiesEnumerator.MoveNext(out uid))
      {
        if (!this.TerminatingOrDeleted(uid.Value) && !this.EntityManager.IsQueuedForDeletion(uid.Value) && this._constructionRequiresSupportQuery.HasComp(uid) && !this.IsSupported((Entity<MapGridComponent>) (valueOrDefault, comp), vector2i))
          this.QueueDel(uid);
      }
    }
  }

  private void OnDeleteXenoResinHit(
    Entity<DeleteXenoResinOnHitComponent> ent,
    ref ProjectileHitEvent args)
  {
    if (!this._net.IsServer || !this._xenoConstructQuery.HasComp(args.Target))
      return;
    this.QueueDel(new EntityUid?(args.Target));
  }

  private void OnXenoConstructMapInit(Entity<XenoConstructComponent> ent, ref MapInitEvent args)
  {
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea((EntityUid) ent, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return;
    ++area.Value.Comp.ResinConstructCount;
    this.Dirty<AreaComponent>(area.Value);
  }

  private void OnXenoConstructRemoved(
    Entity<XenoConstructComponent> ent,
    ref EntityTerminatingEvent args)
  {
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea((EntityUid) ent, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return;
    --area.Value.Comp.ResinConstructCount;
    this.Dirty<AreaComponent>(area.Value);
  }

  private void OnRecentlyPreventCollide(
    Entity<XenoRecentlyConstructedComponent> ent,
    ref PreventCollideEvent args)
  {
    if (!ent.Comp.StopCollide.Contains(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  public FixedPoint2? GetStructurePlasmaCost(EntProtoId prototype)
  {
    Robust.Shared.Prototypes.EntityPrototype prototype1;
    XenoConstructionPlasmaCostComponent component;
    return this._prototype.TryIndex(prototype, out prototype1) && prototype1.TryGetComponent<XenoConstructionPlasmaCostComponent>(out component, this._compFactory) ? new FixedPoint2?(component.Plasma) : new FixedPoint2?();
  }

  public FixedPoint2 GetStructureMinRange(EntProtoId prototype)
  {
    XenoConstructionMinRangeComponent component = (XenoConstructionMinRangeComponent) null;
    Robust.Shared.Prototypes.EntityPrototype prototype1;
    if (this._prototype.TryIndex(prototype, out prototype1))
      prototype1.TryGetComponent<XenoConstructionMinRangeComponent>(out component, this._compFactory);
    return component != null ? (FixedPoint2) component.MinRange.Float() : (FixedPoint2) 0;
  }

  private float? GetBuildSpeed(EntProtoId prototype)
  {
    Robust.Shared.Prototypes.EntityPrototype prototype1;
    XenoConstructionBuildSpeedComponent component;
    return this._prototype.TryIndex(prototype, out prototype1) && prototype1.TryGetComponent<XenoConstructionBuildSpeedComponent>(out component, this._compFactory) ? new float?(component.BuildTimeMult) : new float?();
  }

  private FixedPoint2? GetStructurePlasmaCost(EntProtoId? building)
  {
    if (building.HasValue)
    {
      FixedPoint2? structurePlasmaCost = this.GetStructurePlasmaCost(building.GetValueOrDefault());
      if (structurePlasmaCost.HasValue)
        return new FixedPoint2?(structurePlasmaCost.GetValueOrDefault());
    }
    return new FixedPoint2?();
  }

  public FixedPoint2 GetStructureMinRange(EntProtoId? building)
  {
    return building.HasValue ? this.GetStructureMinRange(building.GetValueOrDefault()) : (FixedPoint2) 0;
  }

  private bool TileSolidAndNotBlocked(EntityCoordinates target)
  {
    TileRef? tileRef = this._turf.GetTileRef(target);
    if (tileRef.HasValue)
    {
      TileRef valueOrDefault = tileRef.GetValueOrDefault();
      if (!this._turf.IsSpace(valueOrDefault) && this._turf.GetContentTileDefinition(valueOrDefault).Sturdy && !this._turf.IsTileBlocked(valueOrDefault, CollisionGroup.Impassable))
        return !this._xenoNest.HasAdjacentNestFacing(target);
    }
    return false;
  }

  private bool InRangePopup(EntityUid xeno, EntityCoordinates target, float range, float minRange = 0.0f)
  {
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(xeno);
    target = target.SnapToGrid((IEntityManager) this.EntityManager, this._map);
    if (!this._transform.InRange(moverCoordinates, target, range))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-cant-reach-there"), target, new EntityUid?(xeno));
      return false;
    }
    if ((double) minRange == 0.0 || !this._transform.InRange(moverCoordinates, target, minRange))
      return true;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-cant-build-in-self"), target, new EntityUid?(xeno));
    return false;
  }

  private bool CanSecreteOnTilePopup(
    Entity<XenoConstructionComponent> xeno,
    EntProtoId? buildChoice,
    EntityCoordinates target,
    bool checkStructureSelected,
    bool checkWeeds)
  {
    if (checkStructureSelected && !buildChoice.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-select-structure"), target, new EntityUid?((EntityUid) xeno));
      return false;
    }
    EntityUid? grid = this._transform.GetGrid(target);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault1 = grid.GetValueOrDefault();
      MapGridComponent comp1;
      if (this.TryComp<MapGridComponent>(valueOrDefault1, out comp1))
      {
        target = target.SnapToGrid((IEntityManager) this.EntityManager, this._map);
        bool flag = this._queenBoostQuery.HasComp(xeno.Owner);
        if (checkWeeds && !this._xenoWeeds.IsOnWeeds((Entity<MapGridComponent>) (valueOrDefault1, comp1), target))
        {
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-need-weeds"), target, new EntityUid?((EntityUid) xeno));
          return false;
        }
        XenoConstructionRangeEvent args = new XenoConstructionRangeEvent(xeno.Comp.BuildRange);
        this.RaiseLocalEvent<XenoConstructionRangeEvent>((EntityUid) xeno, ref args);
        if (args.Range > 0 && !this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) xeno.Owner))
        {
          EntityUid xeno1 = (EntityUid) xeno;
          EntityCoordinates target1 = target;
          FixedPoint2 fixedPoint2 = args.Range;
          double range = (double) fixedPoint2.Float();
          fixedPoint2 = this.GetStructureMinRange(buildChoice);
          double minRange = (double) fixedPoint2.Float();
          if (!this.InRangePopup(xeno1, target1, (float) range, (float) minRange))
            return false;
        }
        if (!this.TileSolidAndNotBlocked(target))
        {
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-cant-build"), target, new EntityUid?((EntityUid) xeno));
          return false;
        }
        Vector2i tile = this._mapSystem.CoordinatesToTile(valueOrDefault1, comp1, target);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault1, comp1, tile);
        EntityUid? uid;
        while (entitiesEnumerator.MoveNext(out uid))
        {
          if (this._xenoConstructQuery.HasComp(uid) || this._xenoEggQuery.HasComp(uid) || this._xenoTunnelQuery.HasComp(uid) || this._sentryQuery.HasComp(uid) || this._blockXenoConstructionQuery.HasComp(uid))
          {
            this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-cant-build"), target, new EntityUid?((EntityUid) xeno));
            return false;
          }
          DoorComponent comp2;
          if (!this.HasComp<BarricadeComponent>(uid) && ((this._tags.HasAnyTag(uid.Value, SharedXenoConstructionSystem.StructureTag) || this.HasComp<StrapComponent>(uid) || this.HasComp<ClimbableComponent>(uid)) && !this._tags.HasTag(uid.Value, SharedXenoConstructionSystem.PlatformTag) && !this.HasComp<DoorComponent>(uid) || this.TryComp<DoorComponent>(uid, out comp2) && comp2.State != DoorState.Open))
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-blocked-structure"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
            return false;
          }
        }
        if (checkStructureSelected)
        {
          FixedPoint2? structurePlasmaCost = this.GetStructurePlasmaCost(buildChoice);
          if (structurePlasmaCost.HasValue)
          {
            FixedPoint2 valueOrDefault2 = structurePlasmaCost.GetValueOrDefault();
            if (!flag && !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, valueOrDefault2))
              return false;
          }
        }
        Robust.Shared.Prototypes.EntityPrototype prototype;
        if (checkStructureSelected && buildChoice.HasValue && this._prototype.TryIndex(buildChoice.GetValueOrDefault(), out prototype) && prototype.HasComponent<XenoConstructionRequiresSupportComponent>(this._compFactory) && !this.IsSupported((Entity<MapGridComponent>) (valueOrDefault1, comp1), target))
        {
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-requires-support", ("choice", (object) prototype.Name)), target, new EntityUid?((EntityUid) xeno));
          return false;
        }
        return this._area.CanResinPopup((Entity<MapGridComponent, AreaGridComponent>) (valueOrDefault1, comp1, (AreaGridComponent) null), tile, new EntityUid?((EntityUid) xeno));
      }
    }
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-cant-build"), target, new EntityUid?((EntityUid) xeno));
    return false;
  }

  private bool CanOrderConstructionPopup(
    Entity<XenoConstructionComponent> xeno,
    EntityCoordinates target,
    EntProtoId? choice)
  {
    if (this._queenEye.IsInQueenEye((Entity<QueenEyeActionComponent>) xeno.Owner) && !this._queenEye.CanSeeTarget((Entity<QueenEyeActionComponent>) xeno.Owner, target) || !this.CanSecreteOnTilePopup(xeno, choice, target, false, false))
      return false;
    EntityUid? grid = this._transform.GetGrid(target);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Vector2i vector2i = this._mapSystem.TileIndicesFor(valueOrDefault, comp, target);
        ImmutableArray<Direction>.Enumerator enumerator = SharedXenoConstructionSystem.Directions.GetEnumerator();
label_10:
        while (enumerator.MoveNext())
        {
          Direction current = enumerator.Current;
          Vector2i direction = SharedMapSystem.GetDirection(vector2i, current);
          AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, comp, direction);
          EntityUid? uid;
          HiveConstructionNodeComponent component;
          do
          {
            if (!entitiesEnumerator.MoveNext(out uid))
              goto label_10;
          }
          while (!this._hiveConstructionNodeQuery.TryGetComponent(uid, out component) || !component.BlockOtherNodes);
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-too-close-to-other-node", (nameof (target), (object) uid.Value)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
          return false;
        }
        Robust.Shared.Prototypes.EntityPrototype prototype;
        if (choice.HasValue && this._prototype.TryIndex(choice, out prototype))
        {
          TileRef tile;
          ITileDefinition definition;
          if (prototype.HasComponent<HiveConstructionRequiresWeedableSurfaceComponent>(this._compFactory) && (!this._mapSystem.TryGetTileRef(valueOrDefault, comp, vector2i, out tile) || !this._tile.TryGetDefinition(tile.Tile.TypeId, out definition) || definition.ID == "Space" || definition is ContentTileDefinition contentTileDefinition && !contentTileDefinition.WeedsSpreadable))
          {
            this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-cant-build"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
            return false;
          }
          Entity<HiveComponent>? hive;
          if (prototype.HasComponent<HiveConstructionRequiresHiveCoreComponent>(this._compFactory))
          {
            hive = this._hive.GetHive((Entity<HiveMemberComponent>) xeno.Owner);
            if (hive.HasValue)
            {
              if (!this._hive.HasHiveCore(hive.GetValueOrDefault()))
              {
                if (this._net.IsServer)
                  this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-requires-hive-core", (nameof (choice), (object) prototype.Name)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
                return false;
              }
            }
            else
            {
              if (this._net.IsServer)
                this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-requires-hive-core", (nameof (choice), (object) prototype.Name)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
              return false;
            }
          }
          if (prototype.HasComponent<HiveConstructionRequiresHiveWeedsComponent>(this._compFactory) && !this._xenoWeeds.IsOnHiveWeeds((Entity<MapGridComponent>) (valueOrDefault, comp), target))
          {
            if (this._net.IsServer)
              this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-requires-hive-weeds", (nameof (choice), (object) prototype.Name)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
            return false;
          }
          if (prototype.HasComponent<HiveConstructionRequiresSpaceComponent>(this._compFactory) && !this.CanPlaceSpaceRequiringStructurePopup(this._transform.ToMapCoordinates(target), (Entity<MapGridComponent>) (valueOrDefault, comp), xeno.Owner, prototype.Name))
            return false;
          HiveConstructionLimitedComponent component;
          int? limit;
          if (prototype.TryGetComponent<HiveConstructionLimitedComponent>(out component, this._compFactory) && !this.CanPlaceLimitedHiveStructure(xeno.Owner, component, out limit, out int? _))
          {
            if (this._net.IsServer)
              this._popup.PopupEntity(limit.GetValueOrDefault() == 1 ? this.Loc.GetString("rmc-xeno-construction-unique-exists", (nameof (choice), (object) prototype.Name)) : this.Loc.GetString("rmc-xeno-construction-hive-limit-met", (nameof (choice), (object) prototype.Name)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
            return false;
          }
          if (prototype.ID == "HiveCoreXenoConstructionNode")
          {
            hive = this._hive.GetHive((Entity<HiveMemberComponent>) xeno.Owner);
            if (hive.HasValue)
            {
              TimeSpan? newCoreAt = hive.GetValueOrDefault().Comp.NewCoreAt;
              TimeSpan curTime = this._timing.CurTime;
              if ((newCoreAt.HasValue ? (newCoreAt.GetValueOrDefault() > curTime ? 1 : 0) : 0) != 0)
              {
                if (this._net.IsServer)
                  this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-cant-build-new-yet", (nameof (choice), (object) prototype.Name)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
                return false;
              }
            }
          }
        }
        return true;
      }
    }
    return false;
  }

  private bool CanPlaceLimitedHiveStructure(
    EntityUid hiveMember,
    HiveConstructionLimitedComponent comp,
    [NotNullWhen(true)] out int? limit,
    [NotNullWhen(true)] out int? curCount)
  {
    limit = new int?();
    curCount = new int?();
    EntProtoId id = comp.Id;
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) hiveMember);
    int limit1;
    if (!hive.HasValue || !this._hive.TryGetStructureLimit(hive.GetValueOrDefault(), id, out limit1))
      return false;
    limit = new int?(limit1);
    curCount = new int?(0);
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent>();
    HiveConstructionLimitedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out comp1, out HiveMemberComponent _))
    {
      if (comp1.Id == id)
      {
        ref int? local = ref curCount;
        int? nullable1 = curCount;
        int? nullable2 = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + 1) : new int?();
        local = nullable2;
      }
    }
    int? nullable3 = limit;
    int? nullable4 = curCount;
    return nullable3.GetValueOrDefault() > nullable4.GetValueOrDefault() & nullable3.HasValue & nullable4.HasValue;
  }

  private bool IsSupported(Entity<MapGridComponent> grid, EntityCoordinates coordinates)
  {
    Vector2i tile = this._mapSystem.TileIndicesFor((EntityUid) grid, (MapGridComponent) grid, coordinates);
    return this.IsSupported(grid, tile);
  }

  private bool IsSupported(Entity<MapGridComponent> grid, Vector2i tile)
  {
    bool flag = false;
    for (int index = 0; index < 4; ++index)
    {
      AtmosDirection dir = (AtmosDirection) (1 << index);
      Vector2i pos = tile.Offset(dir);
      AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator((EntityUid) grid, (MapGridComponent) grid, pos);
      EntityUid? uid;
      while (entitiesEnumerator.MoveNext(out uid))
      {
        if (!this.TerminatingOrDeleted(uid.Value) && !this.EntityManager.IsQueuedForDeletion(uid.Value) && this._constructionSupportQuery.HasComp(uid))
        {
          flag = true;
          break;
        }
      }
      if (flag)
        break;
    }
    return flag;
  }

  private bool CanPlaceSpaceRequiringStructurePopup(
    MapCoordinates mapCoords,
    Entity<MapGridComponent> map,
    EntityUid user,
    string structName)
  {
    MapId mapId = mapCoords.MapId;
    Box2 worldAABB;
    // ISSUE: explicit constructor call
    ((Box2) ref worldAABB).\u002Ector(mapCoords.X - 1.5f, mapCoords.Y + 1.5f, mapCoords.X + 1.5f, mapCoords.Y - 1.5f);
    int num = this._entityLookup.AnyComponentsIntersecting(typeof (HiveConstructionLimitedComponent), mapId, worldAABB) ? 1 : 0;
    TileRef tileRef = this._mapSystem.GetTileRef(map, mapCoords);
    EntityCoordinates coordinates = this._transform.ToCoordinates((Entity<TransformComponent>) user, mapCoords);
    if (num != 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-requires-space", ("choice", (object) structName)), coordinates, new EntityUid?(user));
      return false;
    }
    for (int index1 = tileRef.X - 1; index1 <= tileRef.X + 1; ++index1)
    {
      for (int index2 = tileRef.Y - 1; index2 <= tileRef.Y + 1; ++index2)
      {
        if (index1 != index2 || index1 != 0)
        {
          Vector2i indices;
          // ISSUE: explicit constructor call
          ((Vector2i) ref indices).\u002Ector(index1, index2);
          if (this._turf.IsTileBlocked((EntityUid) map, indices, CollisionGroup.MobMask, map.Comp))
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-requires-space", ("choice", (object) structName)), coordinates, new EntityUid?(user));
            return false;
          }
        }
      }
    }
    return true;
  }

  public bool CanPlaceXenoStructure(
    EntityUid user,
    EntityCoordinates coords,
    [NotNullWhen(false)] out string? popupType,
    bool needsWeeds = true)
  {
    popupType = (string) null;
    EntityUid? grid = this._transform.GetGrid(coords);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Vector2i vector2i = this._mapSystem.TileIndicesFor(valueOrDefault, comp, coords);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, comp, vector2i);
        bool flag = false;
        EntityUid? uid;
        while (entitiesEnumerator.MoveNext(out uid))
        {
          if (this.HasComp<XenoEggComponent>(uid))
          {
            popupType = "rmc-xeno-construction-blocked";
            return false;
          }
          if (!this.HasComp<XenoConstructComponent>(uid))
          {
            if (!this._tags.HasAnyTag(uid.Value, SharedXenoConstructionSystem.StructureTag, SharedXenoConstructionSystem.AirlockTag) && !this.HasComp<StrapComponent>(uid) && !this._xenoTunnelQuery.HasComp(uid) && !this._sentryQuery.HasComp(uid) && !this._blockXenoConstructionQuery.HasComp(uid))
            {
              if (this.HasComp<XenoWeedsComponent>(uid))
              {
                flag = true;
                continue;
              }
              continue;
            }
          }
          popupType = "rmc-xeno-construction-blocked";
          return false;
        }
        if (this._turf.IsTileBlocked(valueOrDefault, vector2i, CollisionGroup.FlyingMobMask | CollisionGroup.MidImpassable, comp))
        {
          popupType = "rmc-xeno-construction-blocked";
          return false;
        }
        if (!(!flag & needsWeeds))
          return true;
        popupType = "rmc-xeno-construction-must-have-weeds";
        return false;
      }
    }
    popupType = "rmc-xeno-construction-no-map";
    return false;
  }

  public void GiveQueenBoost(EntityUid queen, float speedMultiplier, float remoteRange)
  {
    QueenBuildingBoostComponent buildingBoostComponent = this.EnsureComp<QueenBuildingBoostComponent>(queen);
    buildingBoostComponent.BuildSpeedMultiplier = speedMultiplier;
    buildingBoostComponent.RemoteUpgradeRange = remoteRange;
    this.Dirty(queen, (IComponent) buildingBoostComponent);
  }

  public void RemoveQueenBoost(EntityUid queen)
  {
    this.RemCompDeferred<QueenBuildingBoostComponent>(queen);
  }

  private FixedPoint2 GetDensityCost(
    Entity<AreaComponent> area,
    Entity<XenoConstructionComponent> xeno,
    FixedPoint2 cost)
  {
    float num = (float) area.Comp.ResinConstructCount / (float) area.Comp.BuildableTiles;
    if ((double) num >= (double) this._densityThreshold)
      cost = (FixedPoint2) Math.Ceiling((double) cost.Float() * ((double) num + (double) xeno.Comp.DensityConstructionCostModifier) * (double) xeno.Comp.DensityConstructionCostMultiplier);
    XenoPlasmaComponent comp;
    if (this.TryComp<XenoPlasmaComponent>((EntityUid) xeno, out comp) && cost > comp.MaxPlasma)
      cost = (FixedPoint2) comp.MaxPlasma;
    return cost;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoRecentlyConstructedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoRecentlyConstructedComponent>();
    EntityUid uid;
    XenoRecentlyConstructedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (curTime >= comp1.ExpireAt)
      {
        this.RemCompDeferred<XenoRecentlyConstructedComponent>(uid);
      }
      else
      {
        this._intersectingResin.Clear();
        this._entityLookup.GetEntitiesIntersecting(uid, this._intersectingResin);
        for (int index = comp1.StopCollide.Count - 1; index >= 0; --index)
        {
          if (!this._intersectingResin.Contains(comp1.StopCollide[index]))
            comp1.StopCollide.RemoveAt(index);
        }
        if (comp1.StopCollide.Count == 0)
          this.RemCompDeferred<XenoRecentlyConstructedComponent>(uid);
      }
    }
  }
}
