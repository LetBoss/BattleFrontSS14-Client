// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Tunnel.XenoTunnelSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

public sealed class XenoTunnelSystem : EntitySystem
{
  private const string TunnelPrototypeId = "XenoTunnel";
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedTacticalMapSystem _tacticalMap;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private SharedXenoWeedsSystem _xenoWeeds;
  [Dependency]
  private SharedXenoConstructionSystem _xenoConstruct;
  private readonly List<string> _greekLetters = new List<string>()
  {
    "alpha",
    "beta",
    "gamma",
    "delta",
    "zeta",
    "theta",
    "phi",
    "psi",
    "omega"
  };

  private int NextTempTunnelId { get; set; }

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoComponent, XenoDigTunnelActionEvent>(new EntityEventRefHandler<XenoComponent, XenoDigTunnelActionEvent>(this.OnCreateTunnel));
    this.SubscribeLocalEvent<XenoComponent, XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>(new EntityEventRefHandler<XenoComponent, XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>(this.OnCompleteRemoveWeedSource));
    this.SubscribeLocalEvent<XenoComponent, XenoDigTunnelDoAfter>(new EntityEventRefHandler<XenoComponent, XenoDigTunnelDoAfter>(this.OnFinishCreateTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, InteractHandEvent>(new EntityEventRefHandler<XenoTunnelComponent, InteractHandEvent>(this.OnInteract));
    this.SubscribeLocalEvent<XenoTunnelComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<XenoTunnelComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractVerbs));
    this.SubscribeLocalEvent<XenoTunnelComponent, ContainerRelayMovementEntityEvent>(new EntityEventRefHandler<XenoTunnelComponent, ContainerRelayMovementEntityEvent>(this.OnAttemptMoveInTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, TraverseXenoTunnelMessage>(new EntityEventRefHandler<XenoTunnelComponent, TraverseXenoTunnelMessage>(this.OnMoveThroughTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, EnterXenoTunnelDoAfterEvent>(new EntityEventRefHandler<XenoTunnelComponent, EnterXenoTunnelDoAfterEvent>(this.OnFinishEnterTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, TraverseXenoTunnelDoAfterEvent>(new EntityEventRefHandler<XenoTunnelComponent, TraverseXenoTunnelDoAfterEvent>(this.OnFinishMoveThroughTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, OpenBoundInterfaceMessage>(new EntityEventRefHandler<XenoTunnelComponent, OpenBoundInterfaceMessage>(this.GetAllAvailableTunnels));
    this.SubscribeLocalEvent<XenoTunnelComponent, ExaminedEvent>(new EntityEventRefHandler<XenoTunnelComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<XenoTunnelComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<XenoTunnelComponent, GetVerbsEvent<ActivationVerb>>(this.OnGetRenameVerb));
    this.SubscribeLocalEvent<XenoTunnelComponent, InteractUsingEvent>(new EntityEventRefHandler<XenoTunnelComponent, InteractUsingEvent>(this.OnFillTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, XenoCollapseTunnelDoAfterEvent>(new EntityEventRefHandler<XenoTunnelComponent, XenoCollapseTunnelDoAfterEvent>(this.OnCollapseTunnelFinish));
    this.SubscribeLocalEvent<XenoTunnelComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoTunnelComponent, EntityTerminatingEvent>(this.OnDeleteTunnel));
    this.SubscribeLocalEvent<XenoTunnelComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<XenoTunnelComponent, EntInsertedIntoContainerMessage>(this.OnTunnelEntInserted));
    this.SubscribeLocalEvent<XenoTunnelComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<XenoTunnelComponent, ContainerIsInsertingAttemptEvent>(this.OnInsertEntityIntoTunnel));
    this.SubscribeLocalEvent<InXenoTunnelComponent, RegurgitateEvent>(new EntityEventRefHandler<InXenoTunnelComponent, RegurgitateEvent>(this.OnRegurgitateInTunnel));
    this.SubscribeLocalEvent<InXenoTunnelComponent, ComponentInit>(new EntityEventRefHandler<InXenoTunnelComponent, ComponentInit>(this.OnInTunnel));
    this.SubscribeLocalEvent<InXenoTunnelComponent, ComponentRemove>(new EntityEventRefHandler<InXenoTunnelComponent, ComponentRemove>(this.OnOutTunnel));
    this.SubscribeLocalEvent<InXenoTunnelComponent, DropAttemptEvent>(new EntityEventRefHandler<InXenoTunnelComponent, DropAttemptEvent>(this.OnTryDropInTunnel));
    this.SubscribeLocalEvent<InXenoTunnelComponent, MobStateChangedEvent>(new EntityEventRefHandler<InXenoTunnelComponent, MobStateChangedEvent>(this.OnDeathInTunnel));
    this.Subs.BuiEvents<XenoTunnelComponent>((object) NameTunnelUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoTunnelComponent>) (subs => subs.Event<NameTunnelMessage>(new EntityEventRefHandler<XenoTunnelComponent, NameTunnelMessage>(this.OnNameTunnel))));
    this.Subs.BuiEvents<XenoTunnelComponent>((object) SelectDestinationTunnelUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoTunnelComponent>) (subs =>
    {
      subs.Event<BoundUIOpenedEvent>(new EntityEventRefHandler<XenoTunnelComponent, BoundUIOpenedEvent>(this.OnTunnelUIOpened));
      subs.Event<BoundUIClosedEvent>(new EntityEventRefHandler<XenoTunnelComponent, BoundUIClosedEvent>(this.OnTunnelUIClosed));
    }));
  }

  private void OnTunnelUIOpened(Entity<XenoTunnelComponent> tunnel, ref BoundUIOpenedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this.EnsureComp<TunnelUIUserComponent>(args.Actor);
  }

  private void OnTunnelUIClosed(Entity<XenoTunnelComponent> tunnel, ref BoundUIClosedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this.RemCompDeferred<TunnelUIUserComponent>(args.Actor);
  }

  private void OnExamine(Entity<XenoTunnelComponent> xenoTunnel, ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner) || !this._hive.FromSameHive((Entity<HiveMemberComponent>) args.Examiner, (Entity<HiveMemberComponent>) xenoTunnel.Owner))
    {
      LocId messageId = (LocId) "rmc-xeno-construction-tunnel-examine-not-xeno-empty";
      if (this._container.EnsureContainer<Container>((EntityUid) xenoTunnel, "rmc_xeno_tunnel_mob_container").ContainedEntities.Count > 0)
        messageId = (LocId) "rmc-xeno-construction-tunnel-examine-not-xeno";
      using (args.PushGroup("XenoTunnelComponent"))
        args.PushMarkup(this.Loc.GetString((string) messageId));
    }
    else
    {
      string tunnelName;
      if (!this.TryGetHiveTunnelName(xenoTunnel, out tunnelName))
        return;
      using (args.PushGroup("XenoTunnelComponent"))
        args.PushMarkup(this.Loc.GetString("rmc-xeno-construction-tunnel-examine", ("tunnelName", (object) tunnelName)));
    }
  }

  public bool TryGetHiveTunnelName(Entity<XenoTunnelComponent> xenoTunnel, [NotNullWhen(true)] out string? tunnelName)
  {
    tunnelName = (string) null;
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) xenoTunnel.Owner);
    if (!hive.HasValue)
      return false;
    foreach (KeyValuePair<string, EntityUid> hiveTunnel in hive.GetValueOrDefault().Comp.HiveTunnels)
    {
      if (hiveTunnel.Value == xenoTunnel.Owner)
      {
        tunnelName = hiveTunnel.Key;
        return true;
      }
    }
    return false;
  }

  public bool TryPlaceTunnel(
    EntityUid associatedHiveEnt,
    string? name,
    EntityCoordinates buildLocation,
    [NotNullWhen(true)] out EntityUid? tunnelEnt)
  {
    tunnelEnt = new EntityUid?();
    HiveComponent comp;
    if (!this.TryComp<HiveComponent>(associatedHiveEnt, out comp))
      return false;
    Dictionary<string, EntityUid> hiveTunnels = comp.HiveTunnels;
    if (name == null)
    {
      MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(buildLocation.AlignWithClosestGridTile());
      string name1 = this.Loc.GetString("rmc-xeno-construction-default-area-name");
      string str = RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) this._greekLetters);
      EntityPrototype areaPrototype;
      if (this._area.TryGetArea(buildLocation, out Entity<AreaComponent>? _, out areaPrototype))
        name1 = areaPrototype.Name;
      name = this.Loc.GetString("rmc-xeno-construction-default-tunnel-name", ("areaName", (object) name1), ("coordX", (object) mapCoordinates.X), ("coordY", (object) mapCoordinates.Y), ("greekLetter", (object) str));
    }
    if (hiveTunnels.ContainsKey(name))
      return false;
    EntityUid member = this.Spawn("XenoTunnel", buildLocation);
    tunnelEnt = new EntityUid?(member);
    this._hive.SetHive((Entity<HiveMemberComponent>) member, new EntityUid?(associatedHiveEnt));
    return comp.HiveTunnels.TryAdd(name, member);
  }

  private void OnCreateTunnel(Entity<XenoComponent> xenoBuilder, ref XenoDigTunnelActionEvent args)
  {
    if (args.Handled)
      return;
    EntityCoordinates grid = this._transform.GetMoverCoordinates((EntityUid) xenoBuilder).SnapToGrid((IEntityManager) this.EntityManager);
    if (!this.CanPlaceTunnelPopup(args.Performer, grid))
      return;
    EntityUid? nullable = this._transform.GetGrid(grid);
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault1, out comp) && !this.HasComp<AlmayerComponent>(valueOrDefault1))
      {
        if (!this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xenoBuilder.Owner, (FixedPoint2) args.PlasmaCost, false))
          return;
        Entity<AreaComponent>? area;
        if (!this._area.TryGetArea(grid, out area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-bad-area-tunnel"), (EntityUid) xenoBuilder, new EntityUid?((EntityUid) xenoBuilder));
          return;
        }
        Entity<XenoWeedsComponent>? weedsOnFloor = this._xenoWeeds.GetWeedsOnFloor((Entity<MapGridComponent>) (valueOrDefault1, comp), grid, true);
        if (weedsOnFloor.HasValue)
        {
          Entity<XenoWeedsComponent> valueOrDefault2 = weedsOnFloor.GetValueOrDefault();
          XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent sourceDoAfterEvent = new XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent()
          {
            CreateTunnelDelay = args.CreateTunnelDelay,
            PlasmaCost = args.PlasmaCost,
            Prototype = args.Prototype
          };
          EntityManager entityManager = this.EntityManager;
          EntityUid owner = xenoBuilder.Owner;
          double destroyWeedSourceDelay = (double) args.DestroyWeedSourceDelay;
          XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent @event = sourceDoAfterEvent;
          EntityUid? eventTarget = new EntityUid?(xenoBuilder.Owner);
          EntityUid? target = new EntityUid?((EntityUid) valueOrDefault2);
          nullable = new EntityUid?();
          EntityUid? used = nullable;
          this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, owner, (float) destroyWeedSourceDelay, (DoAfterEvent) @event, eventTarget, target, used)
          {
            BlockDuplicate = true,
            BreakOnMove = true,
            DuplicateCondition = DuplicateConditions.SameTarget
          });
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-resin-tunnel-uproot"), args.Performer, new EntityUid?(args.Performer));
          args.Handled = true;
          return;
        }
        XenoDigTunnelDoAfter digTunnelDoAfter = new XenoDigTunnelDoAfter(args.Prototype, args.PlasmaCost);
        EntityManager entityManager1 = this.EntityManager;
        EntityUid owner1 = xenoBuilder.Owner;
        double createTunnelDelay = (double) args.CreateTunnelDelay;
        XenoDigTunnelDoAfter event1 = digTunnelDoAfter;
        EntityUid? eventTarget1 = new EntityUid?(xenoBuilder.Owner);
        nullable = new EntityUid?();
        EntityUid? target1 = nullable;
        nullable = new EntityUid?();
        EntityUid? used1 = nullable;
        this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager1, owner1, (float) createTunnelDelay, (DoAfterEvent) event1, eventTarget1, target1, used1)
        {
          BlockDuplicate = true,
          BreakOnMove = true,
          DuplicateCondition = DuplicateConditions.SameTarget,
          RootEntity = true
        });
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-resin-tunnel-create-tunnel"), args.Performer, new EntityUid?(args.Performer));
        args.Handled = true;
        return;
      }
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-bad-area-tunnel"), (EntityUid) xenoBuilder, new EntityUid?((EntityUid) xenoBuilder));
  }

  private void OnCompleteRemoveWeedSource(
    Entity<XenoComponent> xenoBuilder,
    ref XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent args)
  {
    if (args.Cancelled)
    {
      foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoDigTunnelActionEvent>(xenoBuilder.Owner))
        this._action.ClearCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)));
    }
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue || !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xenoBuilder.Owner, (FixedPoint2) args.PlasmaCost, false))
      return;
    if (this._net.IsClient)
      this.QueueDel(args.Target);
    XenoDigTunnelDoAfter digTunnelDoAfter = new XenoDigTunnelDoAfter(args.Prototype, args.PlasmaCost);
    EntityManager entityManager = this.EntityManager;
    EntityUid owner = xenoBuilder.Owner;
    double createTunnelDelay = (double) args.CreateTunnelDelay;
    XenoDigTunnelDoAfter @event = digTunnelDoAfter;
    EntityUid? eventTarget = new EntityUid?(xenoBuilder.Owner);
    nullable = new EntityUid?();
    EntityUid? target = nullable;
    nullable = new EntityUid?();
    EntityUid? used = nullable;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, owner, (float) createTunnelDelay, (DoAfterEvent) @event, eventTarget, target, used)
    {
      BlockDuplicate = true,
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameTarget,
      RootEntity = true
    });
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-resin-tunnel-create-tunnel"), xenoBuilder.Owner, new EntityUid?(xenoBuilder.Owner));
    args.Handled = true;
  }

  private void OnFinishCreateTunnel(
    Entity<XenoComponent> xenoBuilder,
    ref XenoDigTunnelDoAfter args)
  {
    if (args.Cancelled)
    {
      using (IEnumerator<Entity<ActionComponent>> enumerator = this._rmcActions.GetActionsWithEvent<XenoDigTunnelActionEvent>(xenoBuilder.Owner).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          Entity<ActionComponent> current = enumerator.Current;
          this._action.ClearCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) current, (ActionComponent) current)));
        }
      }
    }
    if (args.Handled || args.Cancelled || !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xenoBuilder.Owner, (FixedPoint2) args.PlasmaCost))
      return;
    string message = this.Loc.GetString("rmc-xeno-construction-failed-tunnel-rename");
    EntityCoordinates grid = this._transform.GetMoverCoordinates((EntityUid) xenoBuilder).SnapToGrid((IEntityManager) this.EntityManager);
    if (!this.CanPlaceTunnelPopup(xenoBuilder.Owner, grid))
    {
      this._popup.PopupClient(message, xenoBuilder.Owner, new EntityUid?(xenoBuilder.Owner));
    }
    else
    {
      this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) xenoBuilder.Owner, (FixedPoint2) args.PlasmaCost);
      if (this._net.IsClient)
        return;
      EntityUid? tunnelEnt;
      if (!this.TryPlaceTunnel((Entity<HiveMemberComponent>) xenoBuilder.Owner, (string) null, out tunnelEnt))
      {
        this._popup.PopupClient(message, xenoBuilder.Owner, new EntityUid?(xenoBuilder.Owner));
      }
      else
      {
        ++this.NextTempTunnelId;
        this._ui.OpenUi((Entity<UserInterfaceComponent>) tunnelEnt.Value, (Enum) NameTunnelUI.Key, new EntityUid?(xenoBuilder.Owner));
        args.Handled = true;
      }
    }
  }

  private void OnNameTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref NameTunnelMessage args)
  {
    if (this._net.IsClient)
      return;
    string key1 = args.TunnelName;
    if (key1.Length > 50)
      key1 = key1.Substring(0, 50);
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) xenoTunnel.Owner);
    if (!hive.HasValue)
      return;
    Dictionary<string, EntityUid> hiveTunnels = hive.Value.Comp.HiveTunnels;
    string key2 = (string) null;
    foreach (KeyValuePair<string, EntityUid> keyValuePair in hiveTunnels)
    {
      if (keyValuePair.Value == xenoTunnel.Owner)
        key2 = keyValuePair.Key;
    }
    if (!hiveTunnels.TryAdd(key1, xenoTunnel.Owner))
    {
      this._popup.PopupCursor(this.Loc.GetString("rmc-xeno-construction-failed-tunnel-rename"), args.Actor);
    }
    else
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(13, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" renamed ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xenoTunnel)), "ToPrettyString(xenoTunnel)");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted(key1);
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCXenoTunnel, ref local);
      if (key2 != null)
        hiveTunnels.Remove(key2);
      this._ui.CloseUi((Entity<UserInterfaceComponent>) xenoTunnel.Owner, (Enum) NameTunnelUI.Key, new EntityUid?(args.Actor));
    }
  }

  private void OnGetInteractVerbs(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    EntityUid target = args.Target;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Act = (Action) (() => this.RaiseLocalEvent<InteractHandEvent>((EntityUid) xenoTunnel, new InteractHandEvent(user, target)));
    interactionVerb1.Text = this.Loc.GetString("xeno-ui-enter-tunnel-verb");
    InteractionVerb interactionVerb2 = interactionVerb1;
    args.Verbs.Add(interactionVerb2);
  }

  private void OnInteract(Entity<XenoTunnelComponent> xenoTunnel, ref InteractHandEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    if (this._container.ContainsEntity(xenoTunnel.Owner, user))
    {
      this.OpenDestinationUI(xenoTunnel, user);
    }
    else
    {
      Container container = this._container.EnsureContainer<Container>(xenoTunnel.Owner, "rmc_xeno_tunnel_mob_container");
      if (!this.HasComp<XenoComponent>(user))
        this._popup.PopupClient(container.Count == 0 ? this.Loc.GetString("rmc-xeno-construction-tunnel-empty-non-xeno-enter-failure") : this.Loc.GetString("rmc-xeno-construction-tunnel-occupied-non-xeno-enter-failure"), user, new EntityUid?(user));
      else if (container.Count >= xenoTunnel.Comp.MaxMobs)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), user, new EntityUid?(user));
      else if (!this._actionBlocker.CanMove(user) || this.Transform(user).Anchored)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-xeno-immobile-failure"), user, new EntityUid?(user));
      }
      else
      {
        RMCSizeComponent comp;
        if (!this.TryComp<RMCSizeComponent>(user, out comp))
          return;
        TimeSpan delay = xenoTunnel.Comp.StandardXenoEnterDelay;
        string tunnelName;
        this.TryGetHiveTunnelName(xenoTunnel, out tunnelName);
        string messageId = "rmc-xeno-construction-tunnel-default-xeno-enter";
        switch (comp.Size)
        {
          case RMCSizes.Small:
            delay = xenoTunnel.Comp.SmallXenoEnterDelay;
            messageId = "rmc-xeno-construction-tunnel-default-xeno-enter";
            break;
          case RMCSizes.Big:
          case RMCSizes.Immobile:
            delay = xenoTunnel.Comp.LargeXenoEnterDelay;
            messageId = "rmc-xeno-construction-tunnel-large-xeno-enter";
            break;
        }
        if (tunnelName != null)
          this._popup.PopupClient(this.Loc.GetString(messageId, ("tunnelName", (object) tunnelName)), user, new EntityUid?(user));
        EnterXenoTunnelDoAfterEvent @event = new EnterXenoTunnelDoAfterEvent();
        this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?(xenoTunnel.Owner))
        {
          BreakOnMove = true
        });
        args.Handled = true;
      }
    }
  }

  private void OnMoveThroughTunnel(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref TraverseXenoTunnelMessage args)
  {
    EntityUid entity1 = this.GetEntity(args.Entity);
    EntityUid actor = args.Actor;
    if (!this._container.ContainsEntity(entity1, actor))
      return;
    EntityUid entity2 = this.GetEntity(args.DestinationTunnel);
    if (!this.HasComp<XenoTunnelComponent>(entity2))
      return;
    if (this._container.EnsureContainer<Container>(entity2, "rmc_xeno_tunnel_mob_container").Count >= xenoTunnel.Comp.MaxMobs)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), actor, new EntityUid?(actor));
    }
    else
    {
      RMCSizeComponent comp;
      if (!this.TryComp<RMCSizeComponent>(actor, out comp))
        return;
      TimeSpan timeSpan;
      switch (comp.Size)
      {
        case RMCSizes.Small:
          timeSpan = xenoTunnel.Comp.SmallXenoMoveDelay;
          break;
        case RMCSizes.Big:
        case RMCSizes.Immobile:
          timeSpan = xenoTunnel.Comp.LargeXenoMoveDelay;
          break;
        default:
          timeSpan = xenoTunnel.Comp.StandardXenoMoveDelay;
          break;
      }
      TimeSpan delay = timeSpan;
      TraverseXenoTunnelDoAfterEvent @event = new TraverseXenoTunnelDoAfterEvent();
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, actor, delay, (DoAfterEvent) @event, new EntityUid?(entity2), used: new EntityUid?(xenoTunnel.Owner))
      {
        BreakOnMove = true
      });
    }
  }

  private void OnAttemptMoveInTunnel(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref ContainerRelayMovementEntityEvent args)
  {
    this._transform.PlaceNextTo((Entity<TransformComponent>) args.Entity, (Entity<TransformComponent>) xenoTunnel.Owner);
    this.RemCompDeferred<InXenoTunnelComponent>(args.Entity);
  }

  private void OnFinishEnterTunnel(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref EnterXenoTunnelDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid user = args.User;
    Container container = this._container.EnsureContainer<Container>((EntityUid) xenoTunnel, "rmc_xeno_tunnel_mob_container");
    if (container.Count >= xenoTunnel.Comp.MaxMobs)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), user, new EntityUid?(user));
    else if (!this._actionBlocker.CanMove(user) || this.Transform(user).Anchored)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-xeno-immobile-failure"), user, new EntityUid?(user));
    }
    else
    {
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) user, (BaseContainer) container);
      this.OpenDestinationUI(xenoTunnel, user);
      args.Handled = true;
    }
  }

  private void OnFinishMoveThroughTunnel(
    Entity<XenoTunnelComponent> destinationXenoTunnel,
    ref TraverseXenoTunnelDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid user = args.User;
    EntityUid? nullable = args.Used;
    EntityUid uid = nullable.Value;
    if (!this._container.ContainsEntity(uid, user))
      return;
    nullable = this._transform.GetMap((Entity<TransformComponent>) uid);
    EntityUid? map = this._transform.GetMap((Entity<TransformComponent>) destinationXenoTunnel.Owner);
    if ((nullable.HasValue == map.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != map.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      return;
    Container container = this._container.EnsureContainer<Container>((EntityUid) destinationXenoTunnel, "rmc_xeno_tunnel_mob_container");
    if (container.Count >= destinationXenoTunnel.Comp.MaxMobs)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), user, new EntityUid?(user));
    }
    else
    {
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) user, (BaseContainer) container);
      this.OpenDestinationUI(destinationXenoTunnel, args.User);
      args.Handled = true;
    }
  }

  private void OnGetRenameVerb(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !this.HasComp<XenoComponent>(args.User))
      return;
    EntityUid uid = args.User;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Text = this.Loc.GetString("xeno-ui-rename-tunnel-verb");
    activationVerb1.Act = (Action) (() => this._ui.TryOpenUi((Entity<UserInterfaceComponent>) xenoTunnel.Owner, (Enum) NameTunnelUI.Key, uid));
    activationVerb1.Impact = LogImpact.Low;
    ActivationVerb activationVerb2 = activationVerb1;
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) xenoTunnel.Owner, (Entity<HiveMemberComponent>) uid) || !this.HasComp<TunnelRenamerComponent>(uid))
      return;
    args.Verbs.Add(activationVerb2);
  }

  private void GetAllAvailableTunnels(
    Entity<XenoTunnelComponent> destinationXenoTunnel,
    ref OpenBoundInterfaceMessage args)
  {
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) destinationXenoTunnel.Owner);
    HiveComponent comp;
    if (!this.TryComp<HiveComponent>(hive.HasValue ? new EntityUid?((EntityUid) hive.GetValueOrDefault()) : new EntityUid?(), out comp))
      return;
    Dictionary<string, EntityUid> hiveTunnels1 = comp.HiveTunnels;
    Dictionary<string, NetEntity> hiveTunnels2 = new Dictionary<string, NetEntity>();
    foreach ((string key, EntityUid uid) in hiveTunnels1)
      hiveTunnels2.Add(key, this.GetNetEntity(uid));
    SelectDestinationTunnelInterfaceState state = new SelectDestinationTunnelInterfaceState(hiveTunnels2);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) destinationXenoTunnel.Owner, (Enum) SelectDestinationTunnelUI.Key, (BoundUserInterfaceState) state);
  }

  private void OnFillTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid used = args.Used;
    XenoTunnelFillerComponent comp1;
    ItemToggleComponent comp2;
    if (!this.TryComp<XenoTunnelFillerComponent>(used, out comp1) || this.TryComp<ItemToggleComponent>(used, out comp2) && !comp2.Activated)
      return;
    args.Handled = true;
    XenoCollapseTunnelDoAfterEvent @event = new XenoCollapseTunnelDoAfterEvent();
    DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp1.FillDelay, (DoAfterEvent) @event, new EntityUid?(xenoTunnel.Owner), new EntityUid?((EntityUid) xenoTunnel), new EntityUid?(used))
    {
      BreakOnMove = true,
      NeedHand = true,
      BreakOnDropItem = true,
      BreakOnHandChange = true,
      RootEntity = true
    };
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-tunnel-fill"), args.User, new EntityUid?(args.User));
    this._doAfter.TryStartDoAfter(args1);
  }

  private void OnCollapseTunnelFinish(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref XenoCollapseTunnelDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    this.CollapseTunnel(xenoTunnel);
  }

  private void OnDeleteTunnel(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref EntityTerminatingEvent args)
  {
    this.CollapseTunnel(xenoTunnel);
  }

  private void OnInsertEntityIntoTunnel(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref ContainerIsInsertingAttemptEvent args)
  {
    if (args.Cancelled || args.Container.ID != "rmc_xeno_tunnel_mob_container" || this.HasComp<XenoComponent>(args.EntityUid) && this._mobState.IsAlive(args.EntityUid))
      return;
    args.Cancel();
  }

  private void OnTunnelEntInserted(
    Entity<XenoTunnelComponent> xenoTunnel,
    ref EntInsertedIntoContainerMessage args)
  {
    if (this._timing.ApplyingState || !this.HasComp<MobStateComponent>(args.Entity))
      return;
    if (!this._mobState.IsAlive(args.Entity))
      this.RemoveFromTunnel(args.Entity, (EntityUid) xenoTunnel);
    this.EnsureComp<InXenoTunnelComponent>(args.Entity);
  }

  private void CollapseTunnel(Entity<XenoTunnelComponent> xenoTunnel)
  {
    if (this._net.IsClient)
      return;
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) xenoTunnel.Owner);
    if (hive.HasValue)
    {
      Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
      string tunnelName;
      if (this.TryGetHiveTunnelName(xenoTunnel, out tunnelName))
        valueOrDefault.Comp.HiveTunnels.Remove(tunnelName);
    }
    BaseContainer container;
    if (this._container.TryGetContainer(xenoTunnel.Owner, "rmc_xeno_tunnel_mob_container", out container))
    {
      foreach (EntityUid entityUid in container.ContainedEntities.ToArray<EntityUid>())
      {
        this.RemoveFromTunnel(entityUid, container.Owner);
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-tunnel-fill-xeno-drop"), entityUid, entityUid);
      }
    }
    this.QueueDel(new EntityUid?(xenoTunnel.Owner));
  }

  private void OnInTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref ComponentInit args)
  {
    this.DisableAllAbilities(tunneledXeno.Owner);
  }

  private void OnOutTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref ComponentRemove args)
  {
    this.EnableAllAbilities(tunneledXeno.Owner);
  }

  private void OnTryDropInTunnel(
    Entity<InXenoTunnelComponent> tunneledXeno,
    ref DropAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnDeathInTunnel(
    Entity<InXenoTunnelComponent> tunneledXeno,
    ref MobStateChangedEvent args)
  {
    BaseContainer container;
    if (args.NewMobState != MobState.Dead || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) tunneledXeno, (TransformComponent) null, (MetaDataComponent) null), out container))
      return;
    EntityUid owner = container.Owner;
    this.RemoveFromTunnel((EntityUid) tunneledXeno, owner);
  }

  private void OnRegurgitateInTunnel(
    Entity<InXenoTunnelComponent> tunneledXeno,
    ref RegurgitateEvent args)
  {
    EntityUid entity = this.GetEntity(args.NetRegurgitated);
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) tunneledXeno, (TransformComponent) null, (MetaDataComponent) null), out container))
      return;
    EntityUid owner = container.Owner;
    this.RemoveFromTunnel(entity, owner);
  }

  private void RemoveFromTunnel(EntityUid tunneledMob, EntityUid tunnel)
  {
    this.RemCompDeferred<InXenoTunnelComponent>(tunneledMob);
    this._transform.DropNextTo((Entity<TransformComponent>) tunneledMob, (Entity<TransformComponent>) tunnel);
  }

  private bool CanPlaceTunnelPopup(EntityUid user, EntityCoordinates coords)
  {
    string popupType;
    if (!this._xenoConstruct.CanPlaceXenoStructure(user, coords, out popupType, false))
    {
      this._popup.PopupClient(this.Loc.GetString(popupType + "-tunnel"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    EntityUid? gridUid = this.Transform(user).GridUid;
    if (gridUid.HasValue)
    {
      EntityUid valueOrDefault = gridUid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        if (this._turf.GetContentTileDefinition(this._map.GetTileRef(valueOrDefault, comp, coords)).CanPlaceTunnel)
          return true;
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-bad-tile-tunnel"), user, new EntityUid?(user), PopupType.SmallCaution);
        return false;
      }
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-bad-tile-tunnel"), user, new EntityUid?(user), PopupType.SmallCaution);
    return false;
  }

  private void DisableAllAbilities(EntityUid ent) => this.SetEnabledStatusAllAbilities(ent, false);

  private void EnableAllAbilities(EntityUid ent) => this.SetEnabledStatusAllAbilities(ent, true);

  private void SetEnabledStatusAllAbilities(EntityUid ent, bool newStatus)
  {
    foreach (Entity<ActionComponent> action in this._action.GetActions(ent))
      this._action.SetEnabled(new Entity<ActionComponent>?(action.AsNullable()), newStatus);
  }

  private bool TryPlaceTunnel(
    Entity<HiveMemberComponent?> builder,
    string? name,
    [NotNullWhen(true)] out EntityUid? tunnelEnt)
  {
    tunnelEnt = new EntityUid?();
    if (!this.Resolve<HiveMemberComponent>((EntityUid) builder, ref builder.Comp) || !builder.Comp.Hive.HasValue)
      return false;
    int num = this.TryPlaceTunnel(builder.Comp.Hive.Value, name, builder.Owner.ToCoordinates(), out tunnelEnt) ? 1 : 0;
    if (!tunnelEnt.HasValue)
      return num != 0;
    this._hive.SetSameHive((Entity<HiveMemberComponent>) builder.Owner, (Entity<HiveMemberComponent>) tunnelEnt.Value);
    return num != 0;
  }

  private void OpenDestinationUI(Entity<XenoTunnelComponent> tunnel, EntityUid enteringEntity)
  {
    Entity<TacticalMapComponent> map;
    TacticalMapUserComponent comp;
    if (this._tacticalMap.TryGetTacticalMap(out map) && this.TryComp<TacticalMapUserComponent>(enteringEntity, out comp))
      this._tacticalMap.UpdateUserData((Entity<TacticalMapUserComponent>) (enteringEntity, comp), (TacticalMapComponent) map);
    this._ui.OpenUi((Entity<UserInterfaceComponent>) tunnel.Owner, (Enum) SelectDestinationTunnelUI.Key, new EntityUid?(enteringEntity));
  }
}
