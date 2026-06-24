// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Overwatch.SharedOverwatchConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills.Pamphlets;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.OrbitalCannon;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.SupplyDrop;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Vendors;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Overwatch;

public abstract class SharedOverwatchConsoleSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private OrbitalCannonSystem _orbitalCannon;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private SquadSystem _squad;
  [Dependency]
  private SharedSupplyDropSystem _supplyDrop;
  [Dependency]
  private SharedTacticalMapSystem _tacticalMap;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private Robust.Shared.GameObjects.EntityQuery<ActorComponent> _actor;
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;
  private Robust.Shared.GameObjects.EntityQuery<OriginalRoleComponent> _originalRoleQuery;
  private Robust.Shared.GameObjects.EntityQuery<RankComponent> _rankQuery;
  private Robust.Shared.GameObjects.EntityQuery<OverwatchDataComponent> _overwatchDataQuery;
  private Robust.Shared.GameObjects.EntityQuery<RMCPlanetComponent> _planetQuery;
  private readonly ProtoId<DamageGroupPrototype> _bruteGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  private readonly ProtoId<DamageGroupPrototype> _burnGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  private readonly ProtoId<DamageGroupPrototype> _toxinGroup = (ProtoId<DamageGroupPrototype>) "Toxin";
  private TimeSpan _maxProcessTime;
  private TimeSpan _nextUpdateTime;
  private TimeSpan _updateEvery;
  private readonly Dictionary<Entity<SquadTeamComponent>, Queue<EntityUid>> _toProcess = new Dictionary<Entity<SquadTeamComponent>, Queue<EntityUid>>();
  private readonly HashSet<Entity<SquadTeamComponent>> _toRemove = new HashSet<Entity<SquadTeamComponent>>();

  public override void Initialize()
  {
    this._actor = this.GetEntityQuery<ActorComponent>();
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    this._originalRoleQuery = this.GetEntityQuery<OriginalRoleComponent>();
    this._rankQuery = this.GetEntityQuery<RankComponent>();
    this._overwatchDataQuery = this.GetEntityQuery<OverwatchDataComponent>();
    this._planetQuery = this.GetEntityQuery<RMCPlanetComponent>();
    this.SubscribeLocalEvent<OrbitalCannonChangedEvent>(new EntityEventRefHandler<OrbitalCannonChangedEvent>(this.OnOrbitalCannonChanged));
    this.SubscribeLocalEvent<OrbitalCannonLaunchEvent>(new EntityEventRefHandler<OrbitalCannonLaunchEvent>(this.OnOrbitalCannonLaunch));
    this.SubscribeLocalEvent<OverwatchConsoleComponent, BoundUIOpenedEvent>(new EntityEventRefHandler<OverwatchConsoleComponent, BoundUIOpenedEvent>(this.OnBUIOpened));
    this.SubscribeLocalEvent<OverwatchConsoleComponent, OverwatchTransferMarineSelectedEvent>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchTransferMarineSelectedEvent>(this.OnTransferMarineSelected));
    this.SubscribeLocalEvent<OverwatchConsoleComponent, OverwatchTransferMarineSquadEvent>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchTransferMarineSquadEvent>(this.OnTransferMarineSquad));
    this.SubscribeLocalEvent<OverwatchWatchingComponent, MoveInputEvent>(new EntityEventRefHandler<OverwatchWatchingComponent, MoveInputEvent>(this.OnWatchingMoveInput));
    this.SubscribeLocalEvent<OverwatchWatchingComponent, DamageChangedEvent>(new EntityEventRefHandler<OverwatchWatchingComponent, DamageChangedEvent>(this.OnWatchingDamageChanged));
    this.Subs.BuiEvents<OverwatchConsoleComponent>((object) OverwatchConsoleUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<OverwatchConsoleComponent>) (subs =>
    {
      subs.Event<OverwatchConsoleSelectSquadBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSelectSquadBuiMsg>(this.OnOverwatchSelectSquadBui));
      subs.Event<OverwatchViewTacticalMapBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchViewTacticalMapBuiMsg>(this.OnOverwatchViewTacticalMapBui));
      subs.Event<OverwatchConsoleTakeOperatorBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleTakeOperatorBuiMsg>(this.OnOverwatchTakeOperatorBui));
      subs.Event<OverwatchConsoleStopOverwatchBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleStopOverwatchBuiMsg>(this.OnOverwatchStopBui));
      subs.Event<OverwatchConsoleSetLocationBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSetLocationBuiMsg>(this.OnOverwatchSetLocationBui));
      subs.Event<OverwatchConsoleShowDeadBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleShowDeadBuiMsg>(this.OnOverwatchShowDeadBui));
      subs.Event<OverwatchConsoleShowHiddenBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleShowHiddenBuiMsg>(this.OnOverwatchShowHiddenBui));
      subs.Event<OverwatchConsoleTransferMarineBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleTransferMarineBuiMsg>(this.OnOverwatchTransferMarineBui));
      subs.Event<OverwatchConsoleWatchBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleWatchBuiMsg>(this.OnOverwatchWatchBui));
      subs.Event<OverwatchConsoleHideBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleHideBuiMsg>(this.OnOverwatchHideBui));
      subs.Event<OverwatchConsolePromoteLeaderBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsolePromoteLeaderBuiMsg>(this.OnOverwatchPromoteLeaderBui));
      subs.Event<OverwatchConsoleSupplyDropLongitudeBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropLongitudeBuiMsg>(this.OnOverwatchSupplyDropLongitudeBui));
      subs.Event<OverwatchConsoleSupplyDropLatitudeBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropLatitudeBuiMsg>(this.OnOverwatchSupplyDropLatitudeBui));
      subs.Event<OverwatchConsoleSupplyDropLaunchBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropLaunchBuiMsg>(this.OnOverwatchSupplyDropLaunchBui));
      subs.Event<OverwatchConsoleSupplyDropSaveBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropSaveBuiMsg>(this.OnOverwatchSupplyDropSaveBui));
      subs.Event<OverwatchConsoleLocationCommentBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleLocationCommentBuiMsg>(this.OnOverwatchSupplyDropCommentBui));
      subs.Event<OverwatchConsoleOrbitalLongitudeBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleOrbitalLongitudeBuiMsg>(this.OnOverwatchOrbitalCoordinatesBui));
      subs.Event<OverwatchConsoleOrbitalLatitudeBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleOrbitalLatitudeBuiMsg>(this.OnOverwatchOrbitalCoordinatesBui));
      subs.Event<OverwatchConsoleOrbitalLaunchBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleOrbitalLaunchBuiMsg>(this.OnOverwatchOrbitalLaunchBui));
      subs.Event<OverwatchConsoleSendMessageBuiMsg>(new EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSendMessageBuiMsg>(this.OnOverwatchSendMessageBui));
    }));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCOverwatchMaxProcessTimeMilliseconds, (Action<float>) (v => this._maxProcessTime = TimeSpan.FromMilliseconds((double) v)), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCOverwatchConsoleUpdateEverySeconds, (Action<float>) (v => this._updateEvery = TimeSpan.FromSeconds((double) v)), true);
  }

  private void OnOrbitalCannonChanged(ref OrbitalCannonChangedEvent ev)
  {
    bool flag = ev.Cannon.Comp.Status == OrbitalCannonStatus.Chambered;
    Robust.Shared.GameObjects.EntityQueryEnumerator<OverwatchConsoleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OverwatchConsoleComponent>();
    EntityUid uid;
    OverwatchConsoleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.HasOrbital = flag;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private void OnOrbitalCannonLaunch(ref OrbitalCannonLaunchEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<OverwatchConsoleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OverwatchConsoleComponent>();
    EntityUid uid;
    OverwatchConsoleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.NextOrbitalLaunch = this._timing.CurTime + ev.Cooldown;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private void OnBUIOpened(Entity<OverwatchConsoleComponent> ent, ref BoundUIOpenedEvent args)
  {
    if (this._net.IsClient)
      return;
    OverwatchConsoleBuiState overwatchBuiState = this.GetOverwatchBuiState(ent);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) ent.Owner, (Enum) OverwatchConsoleUI.Key, (BoundUserInterfaceState) overwatchBuiState);
  }

  private void OnTransferMarineSelected(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchTransferMarineSelectedEvent args)
  {
    EntityUid? entity1;
    if (this._net.IsClient || !this.TryGetEntity(args.Actor, out entity1))
      return;
    EntityUid? nullable1 = new EntityUid?();
    EntityUid? entity2;
    Entity<SquadTeamComponent> squad1;
    if (this.TryGetEntity(args.Marine, out entity2) && this._squad.TryGetMemberSquad((Entity<SquadMemberComponent>) entity2.Value, out squad1))
      nullable1 = new EntityUid?((EntityUid) squad1);
    OverwatchConsoleBuiState overwatchBuiState = this.GetOverwatchBuiState(ent);
    List<DialogOption> options = new List<DialogOption>();
    foreach (OverwatchSquad squad2 in overwatchBuiState.Squads)
    {
      EntityUid? nullable2 = nullable1;
      EntityUid entity3 = this.GetEntity(squad2.Id);
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() == entity3 ? 1 : 0) : 0) == 0)
        options.Add(new DialogOption(squad2.Name, (object) new OverwatchTransferMarineSquadEvent(args.Actor, args.Marine, squad2.Id)));
    }
    this._dialog.OpenOptions((EntityUid) ent, entity1.Value, "Squad Selection", options, "Choose the marine's new squad");
  }

  private void OnTransferMarineSquad(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchTransferMarineSquadEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid entity1 = this.GetEntity(args.Actor);
    if (!entity1.Valid)
      return;
    NetEntity squadId = args.Squad;
    OverwatchSquad? element;
    if (!this.GetOverwatchBuiState(ent).Squads.TryFirstOrNull<OverwatchSquad>((Func<OverwatchSquad, bool>) (s => s.Id == squadId), out element))
    {
      this._popup.PopupCursor("You can't transfer marines to that squad!", entity1, PopupType.LargeCaution);
    }
    else
    {
      EntityUid? entity2;
      if (!this.TryGetEntity(args.Marine, out entity2))
        this._popup.PopupCursor("That marine is KIA.", entity1, PopupType.LargeCaution);
      else if (this._mobState.IsDead(entity2.Value))
        this._popup.PopupCursor(this.Name(entity2.Value) + " is KIA.", entity1, PopupType.LargeCaution);
      else if (element.Value.Leader.HasValue && this.HasComp<SquadLeaderComponent>(entity2))
      {
        this._popup.PopupCursor($"Transfer aborted. {element.Value.Name} can't have another Squad Leader.", entity1, PopupType.LargeCaution);
      }
      else
      {
        EntityUid? entity3;
        if (!this.TryGetEntity(element.Value.Id, out entity3))
        {
          this._popup.PopupCursor("You can't transfer marines to that squad!", entity1, PopupType.LargeCaution);
        }
        else
        {
          Entity<SquadTeamComponent> squad;
          if (this._squad.TryGetMemberSquad((Entity<SquadMemberComponent>) entity2.Value, out squad) && squad.Owner == this.GetEntity(args.Squad))
          {
            this._popup.PopupCursor($"{this.Name(entity2.Value)} is already in {this.Name(entity3.Value)}!", entity1, PopupType.LargeCaution);
          }
          else
          {
            SquadTeamComponent comp;
            OriginalRoleComponent component;
            if (this.TryComp<SquadTeamComponent>(entity3, out comp) && this._originalRoleQuery.TryComp(entity2, out component))
            {
              ProtoId<JobPrototype>? job = component.Job;
              if (job.HasValue)
              {
                ProtoId<JobPrototype> valueOrDefault = job.GetValueOrDefault();
                if (!this._squad.HasSpaceForRole((Entity<SquadTeamComponent>) (entity3.Value, comp), valueOrDefault))
                {
                  string id = valueOrDefault.Id;
                  JobPrototype prototype;
                  if (this._prototypes.TryIndex<JobPrototype>(valueOrDefault, out prototype))
                    id = this.Loc.GetString(prototype.Name);
                  this._popup.PopupCursor($"Transfer aborted. {this.Name(entity3.Value)} can't have another {id}.", entity1, PopupType.LargeCaution);
                  return;
                }
              }
            }
            this._squad.AssignSquad(entity2.Value, (Entity<SquadTeamComponent>) entity3.Value, new ProtoId<JobPrototype>?());
            string message1 = $"{this.Name(entity2.Value)} has been transfered from squad '{this.Name((EntityUid) squad)}' to squad '{this.Name(entity3.Value)}'. Logging to enlistment file.";
            this._marineAnnounce.AnnounceSingle(message1, entity1);
            this._popup.PopupCursor(message1, entity1, PopupType.Large);
            string message2 = $"You've been transfered to {this.Name(entity3.Value)}!";
            this._marineAnnounce.AnnounceSingle(message2, entity2.Value);
            this._popup.PopupEntity(message2, entity2.Value, entity2.Value, PopupType.Large);
          }
        }
      }
    }
  }

  private void OnWatchingMoveInput(Entity<OverwatchWatchingComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    this.TryLocalUnwatch(ent);
  }

  private void OnWatchingDamageChanged(
    Entity<OverwatchWatchingComponent> ent,
    ref DamageChangedEvent args)
  {
    if (!args.DamageIncreased)
      return;
    DamageSpecifier damageDelta = args.DamageDelta;
    if (damageDelta == null)
      return;
    Dictionary<string, FixedPoint2> damagePerGroup = damageDelta.GetDamagePerGroup(this._prototypes);
    FixedPoint2 valueOrDefault1 = damagePerGroup.GetValueOrDefault<string, FixedPoint2>((string) this._bruteGroup);
    FixedPoint2 valueOrDefault2 = damagePerGroup.GetValueOrDefault<string, FixedPoint2>((string) this._burnGroup);
    FixedPoint2 valueOrDefault3 = damagePerGroup.GetValueOrDefault<string, FixedPoint2>((string) this._toxinGroup);
    if (valueOrDefault1 + valueOrDefault2 <= FixedPoint2.Zero && valueOrDefault3 <= 10)
      return;
    this.TryLocalUnwatch(ent);
    foreach ((EntityUid Entity, Enum @enum) in this._ui.GetActorUis((Entity<UserInterfaceUserComponent>) ent.Owner).ToArray<(EntityUid, Enum)>())
    {
      if (@enum is OverwatchConsoleUI.Key)
        this._ui.CloseUi((Entity<UserInterfaceComponent>) Entity, @enum, new EntityUid?((EntityUid) ent));
    }
    if (!this._net.IsServer)
      return;
    this._popup.PopupEntity("The pain kicked you out of the console!", (EntityUid) ent, (EntityUid) ent, PopupType.MediumCaution);
  }

  private void OnOverwatchSelectSquadBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSelectSquadBuiMsg args)
  {
    if (this._net.IsServer)
    {
      EntityUid? entity;
      if (!this.TryGetEntity(args.Squad, out entity) || !this.HasComp<SquadTeamComponent>(entity))
      {
        this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.Actor)} tried to select invalid squad id {this.ToPrettyString(entity)}");
        return;
      }
      SupplyDropComputerComponent comp;
      if (this.TryComp<SupplyDropComputerComponent>((EntityUid) ent, out comp))
        this._supplyDrop.SetSquad((Entity<SupplyDropComputerComponent>) ((EntityUid) ent, comp), (EntProtoId<SquadTeamComponent>?) this.Prototype(entity.Value)?.ID);
    }
    ent.Comp.Squad = new NetEntity?(args.Squad);
    ent.Comp.Operator = (string) Identity.Name(args.Actor, (IEntityManager) this.EntityManager);
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchViewTacticalMapBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchViewTacticalMapBuiMsg args)
  {
    this._tacticalMap.OpenComputerMap((Entity<TacticalMapComputerComponent>) ent.Owner, args.Actor);
  }

  private void OnOverwatchTakeOperatorBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleTakeOperatorBuiMsg args)
  {
    ent.Comp.Operator = (string) Identity.Name(args.Actor, (IEntityManager) this.EntityManager);
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchStopBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleStopOverwatchBuiMsg args)
  {
    ent.Comp.Squad = new NetEntity?();
    ent.Comp.Operator = (string) null;
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchSetLocationBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSetLocationBuiMsg args)
  {
    OverwatchLocation? location1 = args.Location;
    OverwatchLocation overwatchLocation1 = OverwatchLocation.Min;
    if (location1.GetValueOrDefault() < overwatchLocation1 & location1.HasValue)
      return;
    OverwatchLocation? location2 = args.Location;
    OverwatchLocation overwatchLocation2 = OverwatchLocation.Ship;
    if (location2.GetValueOrDefault() > overwatchLocation2 & location2.HasValue)
      return;
    ent.Comp.Location = args.Location;
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchShowDeadBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleShowDeadBuiMsg args)
  {
    ent.Comp.ShowDead = args.Show;
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchShowHiddenBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleShowHiddenBuiMsg args)
  {
    ent.Comp.ShowHidden = args.Show;
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchTransferMarineBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleTransferMarineBuiMsg args)
  {
    if (this._net.IsClient)
      return;
    NetEntity? squad = ent.Comp.Squad;
    if (!squad.HasValue)
      return;
    NetEntity valueOrDefault = squad.GetValueOrDefault();
    OverwatchConsoleBuiState overwatchBuiState = this.GetOverwatchBuiState(ent);
    List<DialogOption> options = new List<DialogOption>();
    List<OverwatchMarine> overwatchMarineList;
    if (overwatchBuiState.Marines.TryGetValue(valueOrDefault, out overwatchMarineList))
    {
      foreach (OverwatchMarine overwatchMarine in overwatchMarineList)
      {
        DialogOption dialogOption = new DialogOption()
        {
          Text = overwatchMarine.Name ?? "",
          Event = (object) new OverwatchTransferMarineSelectedEvent(this.GetNetEntity(args.Actor), overwatchMarine.Id)
        };
        options.Add(dialogOption);
      }
    }
    this._dialog.OpenOptions((EntityUid) ent, args.Actor, "Transfer Marine", options, "Choose marine to transfer");
  }

  private void OnOverwatchWatchBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleWatchBuiMsg args)
  {
    EntityUid? entity;
    Entity<OverwatchCameraComponent> target;
    if (args.Target == new NetEntity() || !this.TryGetEntity(args.Target, out entity) || !this._inventory.TryGetInventoryEntity<OverwatchCameraComponent>((Entity<InventoryComponent>) entity.Value, out target))
      return;
    this.Watch((Entity<ActorComponent, EyeComponent>) args.Actor, target);
  }

  private void OnOverwatchHideBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleHideBuiMsg args)
  {
    if (this._net.IsClient)
    {
      if (args.Hide)
        ent.Comp.Hidden.Add(args.Target);
      else
        ent.Comp.Hidden.Remove(args.Target);
      this.Dirty<OverwatchConsoleComponent>(ent);
    }
    else
    {
      EntityUid? entity;
      if (args.Target == new NetEntity() || !this.TryGetEntity(args.Target, out entity) || !this.HasComp<SquadMemberComponent>(entity))
        return;
      if (args.Hide)
        ent.Comp.Hidden.Add(args.Target);
      else
        ent.Comp.Hidden.Remove(args.Target);
      this.Dirty<OverwatchConsoleComponent>(ent);
      OverwatchConsoleBuiState overwatchBuiState = this.GetOverwatchBuiState(ent);
      this._ui.SetUiState((Entity<UserInterfaceComponent>) ent.Owner, (Enum) OverwatchConsoleUI.Key, (BoundUserInterfaceState) overwatchBuiState);
    }
  }

  private void OnOverwatchPromoteLeaderBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsolePromoteLeaderBuiMsg args)
  {
    EntityUid? entity;
    SquadMemberComponent comp;
    if (this._net.IsClient || !this.TryGetEntity(args.Target, out entity) || !this.TryComp<SquadMemberComponent>(entity, out comp))
      return;
    this._squad.PromoteSquadLeader((Entity<SquadMemberComponent>) (entity.Value, comp), args.Actor, args.Icon);
    OverwatchConsoleBuiState overwatchBuiState = this.GetOverwatchBuiState(ent);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) ent.Owner, (Enum) OverwatchConsoleUI.Key, (BoundUserInterfaceState) overwatchBuiState);
  }

  private void OnOverwatchSupplyDropLongitudeBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSupplyDropLongitudeBuiMsg args)
  {
    this._supplyDrop.SetLongitude((Entity<SupplyDropComputerComponent>) ent.Owner, args.Longitude);
  }

  private void OnOverwatchSupplyDropLatitudeBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSupplyDropLatitudeBuiMsg args)
  {
    this._supplyDrop.SetLatitude((Entity<SupplyDropComputerComponent>) ent.Owner, args.Latitude);
  }

  private void OnOverwatchSupplyDropLaunchBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSupplyDropLaunchBuiMsg args)
  {
    SupplyDropComputerComponent comp;
    if (this._net.IsClient || !this.TryComp<SupplyDropComputerComponent>((EntityUid) ent, out comp))
      return;
    this._supplyDrop.TryLaunchSupplyDropPopup((Entity<SupplyDropComputerComponent>) ((EntityUid) ent, comp), args.Actor);
    OverwatchConsoleBuiState overwatchBuiState = this.GetOverwatchBuiState(ent);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) ent.Owner, (Enum) OverwatchConsoleUI.Key, (BoundUserInterfaceState) overwatchBuiState);
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchSupplyDropSaveBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSupplyDropSaveBuiMsg args)
  {
    OverwatchSavedLocation?[] savedLocations = ent.Comp.SavedLocations;
    if (savedLocations.Length == 0)
      return;
    ref int local = ref ent.Comp.LastLocation;
    if (local >= savedLocations.Length)
      local = 0;
    savedLocations[local] = new OverwatchSavedLocation?(new OverwatchSavedLocation(args.Longitude, args.Latitude, string.Empty));
    ++local;
    this.Dirty<OverwatchConsoleComponent>(ent);
  }

  private void OnOverwatchSupplyDropCommentBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleLocationCommentBuiMsg args)
  {
    OverwatchSavedLocation?[] savedLocations = ent.Comp.SavedLocations;
    if (args.Index < 0 || args.Index >= savedLocations.Length)
      return;
    OverwatchSavedLocation? nullable = savedLocations[args.Index];
    if (!nullable.HasValue)
      return;
    OverwatchSavedLocation valueOrDefault = nullable.GetValueOrDefault();
    string str = args.Comment;
    if (str.Length > 50)
      str = str.Substring(0, 50);
    savedLocations[args.Index] = new OverwatchSavedLocation?(valueOrDefault with
    {
      Comment = str
    });
  }

  private void OnOverwatchOrbitalCoordinatesBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleOrbitalLongitudeBuiMsg args)
  {
    ent.Comp.OrbitalCoordinates = new Vector2i(args.Longitude, ent.Comp.OrbitalCoordinates.Y);
  }

  private void OnOverwatchOrbitalCoordinatesBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleOrbitalLatitudeBuiMsg args)
  {
    ent.Comp.OrbitalCoordinates = new Vector2i(ent.Comp.OrbitalCoordinates.X, args.Latitude);
  }

  private void OnOverwatchOrbitalLaunchBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleOrbitalLaunchBuiMsg args)
  {
    Entity<OrbitalCannonComponent> cannon;
    if (!ent.Comp.CanOrbitalBombardment || !this._orbitalCannon.TryGetClosestCannon((EntityUid) ent, out cannon))
      return;
    EntityUid squad = new EntityUid();
    EntityUid? entity;
    if (this.TryGetEntity(ent.Comp.Squad, out entity))
      squad = entity.Value;
    this._orbitalCannon.Fire(cannon, ent.Comp.OrbitalCoordinates, args.Actor, squad);
  }

  private void OnOverwatchSendMessageBui(
    Entity<OverwatchConsoleComponent> ent,
    ref OverwatchConsoleSendMessageBuiMsg args)
  {
    if (!ent.Comp.CanMessageSquad)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < ent.Comp.LastMessage + ent.Comp.MessageCooldown)
      return;
    string str1 = args.Message;
    if (str1.Length > 200)
      str1 = str1.Substring(0, 200);
    EntityUid? entity;
    if (string.IsNullOrWhiteSpace(str1) || !this.TryGetEntity(ent.Comp.Squad, out entity))
      return;
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = this.Prototype(entity.Value);
    if (entityPrototype == null)
      return;
    ent.Comp.LastMessage = curTime;
    this.Dirty<OverwatchConsoleComponent>(ent);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(22, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" sent ");
    logStringHandler.AppendFormatted(entityPrototype.Name);
    logStringHandler.AppendLiteral(" squad message: ");
    logStringHandler.AppendFormatted(args.Message);
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCMarineAnnounce, ref local);
    this._marineAnnounce.AnnounceSquad($"[color=#3C70FF][bold]Overwatch:[/bold] {this.Name(args.Actor)} transmits: [font size=16][bold]{str1}[/bold][/font][/color]", (EntProtoId<SquadTeamComponent>) entityPrototype.ID);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) ent);
    Filter filter1 = Filter.Empty().AddInRange(mapCoordinates, 12f, this._player, (IEntityManager) this.EntityManager);
    filter1.RemoveWhereAttachedEntity(new Predicate<EntityUid>(((EntitySystem) this).HasComp<XenoComponent>));
    string str2 = $"[bold][color=#6685F5]'{this.Name(entity.Value)}' squad message sent: '{str1}'.[/color][/bold]";
    NetUserId? userId = this.CompOrNull<ActorComponent>(args.Actor)?.PlayerSession.UserId;
    SharedCMChatSystem rmcChat = this._rmcChat;
    string message = str2;
    string wrappedMessage = str2;
    Filter filter2 = filter1;
    NetUserId? nullable = userId;
    EntityUid source = new EntityUid();
    Color? colorOverride = new Color?();
    NetUserId? author = nullable;
    rmcChat.ChatMessageToMany(message, wrappedMessage, filter2, ChatChannel.Local, source, colorOverride: colorOverride, author: author);
  }

  protected virtual void Watch(
    Entity<ActorComponent?, EyeComponent?> watcher,
    Entity<OverwatchCameraComponent?> toWatch)
  {
  }

  protected virtual void Unwatch(Entity<EyeComponent?> watcher, ICommonSession player)
  {
    if (!this.Resolve<EyeComponent>((EntityUid) watcher, ref watcher.Comp))
      return;
    this._eye.SetTarget((EntityUid) watcher, new EntityUid?());
  }

  private OverwatchConsoleBuiState GetOverwatchBuiState(Entity<OverwatchConsoleComponent> console)
  {
    return this.GetOverwatchBuiState(console.Comp);
  }

  private OverwatchConsoleBuiState GetOverwatchBuiState(OverwatchConsoleComponent console)
  {
    List<OverwatchSquad> squads = new List<OverwatchSquad>();
    Dictionary<NetEntity, List<OverwatchMarine>> dictionary = new Dictionary<NetEntity, List<OverwatchMarine>>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadTeamComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadTeamComponent>();
    EntityUid uid;
    SquadTeamComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(console.Group != "ADMINISTRATOR") || !(comp1.Group != console.Group))
      {
        NetEntity netEntity = this.GetNetEntity(uid);
        OverwatchSquad overwatchSquad = new OverwatchSquad(netEntity, this.Name(uid), comp1.Color, new NetEntity?(), comp1.CanSupplyDrop, comp1.LeaderIcon);
        List<OverwatchMarine> orNew = dictionary.GetOrNew<NetEntity, List<OverwatchMarine>>(netEntity);
        foreach (EntityUid member in comp1.Members)
        {
          OverwatchMarine? marine = (OverwatchMarine?) this._overwatchDataQuery.CompOrNull(member)?.Marine;
          if (marine.HasValue)
          {
            OverwatchMarine valueOrDefault = marine.GetValueOrDefault();
            orNew.Add(valueOrDefault);
          }
        }
        squads.Add(overwatchSquad);
      }
    }
    return new OverwatchConsoleBuiState(squads, dictionary);
  }

  public bool IsHidden(Entity<OverwatchConsoleComponent> console, NetEntity marine)
  {
    return console.Comp.Hidden.Contains(marine);
  }

  private void TryLocalUnwatch(Entity<OverwatchWatchingComponent> ent)
  {
    if (this._net.IsClient)
    {
      EntityUid? localEntity = this._player.LocalEntity;
      EntityUid owner = ent.Owner;
      if ((localEntity.HasValue ? (localEntity.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0 && this._player.LocalSession != null)
      {
        this.Unwatch((Entity<EyeComponent>) ent.Owner, this._player.LocalSession);
        return;
      }
    }
    ActorComponent comp;
    if (!this.TryComp<ActorComponent>((EntityUid) ent, out comp))
      return;
    this.Unwatch((Entity<EyeComponent>) ent.Owner, comp.PlayerSession);
  }

  private void ProcessData()
  {
    if (this._net.IsClient)
    {
      this._toProcess.Clear();
    }
    else
    {
      try
      {
        TimeSpan curTime = this._timing.CurTime;
        if (this._toProcess.Count > 0)
        {
          foreach ((Entity<SquadTeamComponent> entity, Queue<EntityUid> entityUidQueue) in this._toProcess)
          {
            if (this.TerminatingOrDeleted((EntityUid) entity))
            {
              this._toRemove.Add(entity);
            }
            else
            {
              MapCoordinates? nullable = new MapCoordinates?();
              Entity<SquadLeaderComponent> leader;
              if (this._squad.TryGetSquadLeader(entity, out leader))
                nullable = new MapCoordinates?(this._transform.GetMapCoordinates((EntityUid) leader));
              EntityUid result;
              while (entityUidQueue.TryDequeue(out result) && !(this._timing.CurTime > curTime + this._maxProcessTime))
              {
                EntityUid? uid;
                if (!this.TerminatingOrDeleted(result) && this._map.TryGetMap(new MapId?(this.Transform(result).MapID), out uid) && !this._map.IsPaused((Entity<MapComponent>) uid.Value))
                {
                  MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(result);
                  IdentityEntity Name = Identity.Name(result, (IEntityManager) this.EntityManager);
                  MobStateComponent mobStateComponent = this._mobStateQuery.CompOrNull(result);
                  MobState State = mobStateComponent != null ? mobStateComponent.CurrentState : MobState.Alive;
                  bool SSD = !this._actor.HasComp(result);
                  ProtoId<JobPrototype>? job = (ProtoId<JobPrototype>?) this._originalRoleQuery.CompOrNull(result)?.Job;
                  ProtoId<RankPrototype>? rank = (ProtoId<RankPrototype>?) this._rankQuery.CompOrNull(result)?.Rank;
                  OverwatchLocation Location = this._planetQuery.HasComp(uid) ? OverwatchLocation.Min : OverwatchLocation.Ship;
                  Robust.Shared.Prototypes.EntityPrototype areaPrototype;
                  string AreaName = this._area.TryGetArea(mapCoordinates, out Entity<AreaComponent>? _, out areaPrototype) ? areaPrototype.Name : string.Empty;
                  NetEntity netEntity = this.GetNetEntity(result);
                  LocId? RoleOverride = (LocId?) ((LocId?) this.CompOrNull<RMCVendorRoleOverrideComponent>(result)?.GiveSquadRoleName ?? this.CompOrNull<UsedSkillPamphletComponent>(result)?.JobTitle);
                  Vector2? LeaderDistance = new Vector2?();
                  if (result != leader.Owner && nullable.HasValue && nullable.Value.MapId == mapCoordinates.MapId)
                    LeaderDistance = new Vector2?(nullable.Value.Position - mapCoordinates.Position);
                  Entity<OverwatchCameraComponent> target;
                  this._inventory.TryGetInventoryEntity<OverwatchCameraComponent>((Entity<InventoryComponent>) result, out target);
                  this.EnsureComp<OverwatchDataComponent>(result).Marine = new OverwatchMarine?(new OverwatchMarine(netEntity, this.GetNetEntity((EntityUid) target), (string) Name, State, SSD, job, Location == OverwatchLocation.Min, Location, AreaName, LeaderDistance, rank, RoleOverride));
                }
              }
              if (entityUidQueue.Count == 0)
                this._toRemove.Add(entity);
            }
          }
          foreach (Entity<SquadTeamComponent> key in this._toRemove)
            this._toProcess.Remove(key);
        }
        else
        {
          Robust.Shared.GameObjects.EntityQueryEnumerator<SquadTeamComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SquadTeamComponent>();
          EntityUid uid;
          SquadTeamComponent comp1;
          while (entityQueryEnumerator.MoveNext(out uid, out comp1))
          {
            Queue<EntityUid> orNew = this._toProcess.GetOrNew<Entity<SquadTeamComponent>, Queue<EntityUid>>((Entity<SquadTeamComponent>) (uid, comp1));
            foreach (EntityUid member in comp1.Members)
              orNew.Enqueue(member);
          }
        }
      }
      catch
      {
        this._toProcess.Clear();
        throw;
      }
    }
  }

  private void UpdateConsoles()
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < this._nextUpdateTime)
      return;
    this._nextUpdateTime = curTime + this._updateEvery;
    OverwatchConsoleBuiState state = (OverwatchConsoleBuiState) null;
    Robust.Shared.GameObjects.EntityQueryEnumerator<OverwatchConsoleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OverwatchConsoleComponent>();
    EntityUid uid;
    OverwatchConsoleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (this._ui.IsUiOpen((Entity<UserInterfaceComponent>) uid, (Enum) OverwatchConsoleUI.Key))
      {
        if (state == null)
          state = this.GetOverwatchBuiState(comp1);
        this._ui.SetUiState((Entity<UserInterfaceComponent>) uid, (Enum) OverwatchConsoleUI.Key, (BoundUserInterfaceState) state);
      }
    }
  }

  public override void Update(float frameTime)
  {
    this.ProcessData();
    this.UpdateConsoles();
  }
}
