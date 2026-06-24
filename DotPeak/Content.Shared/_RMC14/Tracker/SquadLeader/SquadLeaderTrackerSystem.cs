// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.SquadLeader.SquadLeaderTrackerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills.Pamphlets;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Vendors;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Tracker.SquadLeader;

public sealed class SquadLeaderTrackerSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private IComponentFactory _factory;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedRankSystem _rank;
  [Dependency]
  private SquadSystem _squad;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private TrackerSystem _tracker;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private readonly Dictionary<EntityUid, MapCoordinates> _squadLeaders = new Dictionary<EntityUid, MapCoordinates>();
  private readonly Dictionary<EntityUid, MapCoordinates>?[] _fireteamLeaders = new Dictionary<EntityUid, MapCoordinates>[3];
  private Robust.Shared.GameObjects.EntityQuery<FireteamLeaderComponent> _fireteamLeaderQuery;
  private Robust.Shared.GameObjects.EntityQuery<FireteamMemberComponent> _fireteamMemberQuery;
  private Robust.Shared.GameObjects.EntityQuery<OriginalRoleComponent> _originalRoleQuery;
  private Robust.Shared.GameObjects.EntityQuery<SquadLeaderTrackerComponent> _squadLeaderTrackerQuery;
  private Robust.Shared.GameObjects.EntityQuery<SquadMemberComponent> _squadMemberQuery;
  private const string SquadTrackerCategory = "SquadTracker";
  private const string SquadLeaderMode = "SquadLeader";
  private const string FireteamLeader = "FireteamLeader";

  public override void Initialize()
  {
    this._fireteamLeaderQuery = this.GetEntityQuery<FireteamLeaderComponent>();
    this._fireteamMemberQuery = this.GetEntityQuery<FireteamMemberComponent>();
    this._originalRoleQuery = this.GetEntityQuery<OriginalRoleComponent>();
    this._squadLeaderTrackerQuery = this.GetEntityQuery<SquadLeaderTrackerComponent>();
    this._squadMemberQuery = this.GetEntityQuery<SquadMemberComponent>();
    this.SubscribeLocalEvent<SquadMemberAddedEvent>(new EntityEventRefHandler<SquadMemberAddedEvent>(this.OnSquadMemberAdded));
    this.SubscribeLocalEvent<SquadMemberRemovedEvent>(new EntityEventRefHandler<SquadMemberRemovedEvent>(this.OnSquadMemberRemoved));
    this.SubscribeLocalEvent<GrantSquadLeaderTrackerComponent, GotEquippedEvent>(new EntityEventRefHandler<GrantSquadLeaderTrackerComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<GrantSquadLeaderTrackerComponent, GotUnequippedEvent>(new EntityEventRefHandler<GrantSquadLeaderTrackerComponent, GotUnequippedEvent>(this.OnGotUnequipped));
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, MapInitEvent>(new EntityEventRefHandler<SquadLeaderTrackerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, ComponentRemove>(new EntityEventRefHandler<SquadLeaderTrackerComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, SquadLeaderTrackerClickedEvent>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerClickedEvent>(this.OnSquadLeaderTrackerClicked));
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, SquadLeaderTrackerChangeModeEvent>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerChangeModeEvent>(this.OnSquadLeaderTrackerChangeMode));
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, LeaderTrackerSelectTargetEvent>(new EntityEventRefHandler<SquadLeaderTrackerComponent, LeaderTrackerSelectTargetEvent>(this.OnLeaderTrackerSelectTargetEvent));
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, GetMarineSquadNameEvent>(new EntityEventRefHandler<SquadLeaderTrackerComponent, GetMarineSquadNameEvent>(this.OnRoleChange), after: new Type[2]
    {
      typeof (SkillPamphletSystem),
      typeof (VendorRoleOverrideSystem)
    });
    this.Subs.BuiEvents<SquadLeaderTrackerComponent>((object) SquadLeaderTrackerUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<SquadLeaderTrackerComponent>) (subs =>
    {
      subs.Event<SquadLeaderTrackerAssignFireteamMsg>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerAssignFireteamMsg>(this.OnAssignFireteamMsg));
      subs.Event<SquadLeaderTrackerUnassignFireteamMsg>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerUnassignFireteamMsg>(this.OnUnassignFireteamMsg));
      subs.Event<SquadLeaderTrackerPromoteFireteamLeaderMsg>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerPromoteFireteamLeaderMsg>(this.OnPromoteFireteamLeaderMsg));
      subs.Event<SquadLeaderTrackerDemoteFireteamLeaderMsg>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerDemoteFireteamLeaderMsg>(this.OnDemoteFireteamLeaderMsg));
      subs.Event<SquadLeaderTrackerChangeTrackedMsg>(new EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerChangeTrackedMsg>(this.OnChangeTrackedMsg));
    }));
  }

  private void OnSquadMemberAdded(ref SquadMemberAddedEvent ev)
  {
    this.AddFireteamMember(ev.Squad.Comp.Fireteams, ev.Member);
    Entity<SquadLeaderComponent> leader;
    if (this._squad.TryGetSquadLeader(ev.Squad, out leader))
    {
      ev.Squad.Comp.Fireteams.SquadLeader = this.Name((EntityUid) leader);
      ev.Squad.Comp.Fireteams.SquadLeaderId = new NetEntity?(this.GetNetEntity((EntityUid) leader));
    }
    else
    {
      ev.Squad.Comp.Fireteams.SquadLeader = (string) null;
      ev.Squad.Comp.Fireteams.SquadLeaderId = new NetEntity?();
    }
    this.SyncMemberFireteams((Entity<SquadMemberComponent>) ev.Member);
  }

  private void OnSquadMemberRemoved(ref SquadMemberRemovedEvent ev)
  {
    NetEntity netEntity = this.GetNetEntity(ev.Member);
    this.RemoveFireteamMember(ev.Squad.Comp.Fireteams, netEntity);
    this.SyncFireteams(ev.Squad.AsNullable());
  }

  private void OnGotEquipped(
    Entity<GrantSquadLeaderTrackerComponent> ent,
    ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    SquadLeaderTrackerComponent trackerComponent = this.EnsureComp<SquadLeaderTrackerComponent>(args.Equipee);
    trackerComponent.TrackerModes = ent.Comp.TrackerModes;
    this.SetMode((Entity<SquadLeaderTrackerComponent>) (args.Equipee, trackerComponent), ent.Comp.DefaultMode);
    this.Dirty(args.Equipee, (IComponent) trackerComponent);
  }

  private void OnGotUnequipped(
    Entity<GrantSquadLeaderTrackerComponent> ent,
    ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE || this._inventory.TryGetInventoryEntity<GrantSquadLeaderTrackerComponent>((Entity<InventoryComponent>) args.Equipee, out Entity<GrantSquadLeaderTrackerComponent> _))
      return;
    this.RemCompDeferred<SquadLeaderTrackerComponent>(args.Equipee);
  }

  private void OnMapInit(Entity<SquadLeaderTrackerComponent> ent, ref MapInitEvent args)
  {
    this.UpdateDirection(ent);
    Entity<SquadTeamComponent> squad1;
    if (!this._squad.TryGetMemberSquad((Entity<SquadMemberComponent>) ent.Owner, out squad1))
      return;
    Entity<SquadLeaderTrackerComponent> ent1 = ent;
    string str = this.Name((EntityUid) squad1);
    MapCoordinates? coordinates = new MapCoordinates?();
    string squad2 = str;
    this.UpdateDirection(ent1, coordinates, squad2);
    ent.Comp.Fireteams = squad1.Comp.Fireteams;
    this.Dirty<SquadLeaderTrackerComponent>(ent);
  }

  private void OnRemove(Entity<SquadLeaderTrackerComponent> ent, ref ComponentRemove args)
  {
    TrackerModePrototype prototype;
    this._prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, out prototype);
    if (prototype == null)
      return;
    this._alerts.ClearAlert((EntityUid) ent, prototype.Alert);
  }

  private void OnSquadLeaderTrackerClicked(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerClickedEvent args)
  {
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) SquadLeaderTrackerUI.Key, (EntityUid) ent);
  }

  private void OnSquadLeaderTrackerChangeMode(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerChangeModeEvent args)
  {
    List<DialogOption> options;
    List<EntityUid> trackingOptions;
    if (!this._timing.IsFirstTimePredicted || !this.TryFindTargets(args.Mode, out options, out trackingOptions))
      return;
    SquadMemberComponent component1;
    EntityUid? nullable;
    if (this._squadMemberQuery.TryComp((EntityUid) ent, out component1))
    {
      int index = 0;
      while (index < trackingOptions.Count)
      {
        SquadMemberComponent component2;
        if (this._squadMemberQuery.TryComp(trackingOptions[index], out component2))
        {
          EntityUid? squad = component1.Squad;
          nullable = component2.Squad;
          if ((squad.HasValue == nullable.HasValue ? (squad.HasValue ? (squad.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0 && !this.HasComp<SquadLeaderComponent>((EntityUid) ent))
          {
            options.RemoveAt(index);
            trackingOptions.RemoveAt(index);
            continue;
          }
        }
        ++index;
      }
    }
    if (options.Count <= 1)
    {
      EntityUid entityUid;
      if (trackingOptions.TryGetValue<EntityUid>(0, out entityUid))
      {
        this.SetTarget(ent, new EntityUid?(entityUid));
      }
      else
      {
        Entity<SquadLeaderTrackerComponent> ent1 = ent;
        nullable = new EntityUid?();
        EntityUid? target = nullable;
        this.SetTarget(ent1, target);
      }
      this.SetMode(ent, args.Mode);
      if (this._net.IsClient)
        return;
      MapCoordinates? coordinates = new MapCoordinates?();
      string squad = "";
      if (ent.Comp.Target.HasValue)
      {
        coordinates = new MapCoordinates?(this._transform.GetMapCoordinates(ent.Comp.Target.Value));
        SquadMemberComponent component3;
        if (this._squadMemberQuery.TryComp(ent.Comp.Target.Value, out component3))
        {
          nullable = component3.Squad;
          if (nullable.HasValue)
            squad = this.Name(nullable.GetValueOrDefault());
        }
      }
      this.UpdateDirection(ent, coordinates, squad);
    }
    else
      this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-squad-info-tracking-selection"), options, this.Loc.GetString("rmc-squad-info-tracking-choose"));
  }

  private void OnLeaderTrackerSelectTargetEvent(
    Entity<SquadLeaderTrackerComponent> ent,
    ref LeaderTrackerSelectTargetEvent args)
  {
    this.SetTarget(ent, new EntityUid?(this.GetEntity(args.Target)));
    this.SetMode(ent, args.Mode);
    this.Dirty<SquadLeaderTrackerComponent>(ent);
  }

  private void OnAssignFireteamMsg(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerAssignFireteamMsg args)
  {
    EntityUid? entity;
    if (this._net.IsClient || args.Fireteam < 0 || args.Fireteam >= ent.Comp.Fireteams.Fireteams.Length || !this.TryGetEntity(args.Marine, out entity) || !this.CanChangeFireteamMember(args.Actor, entity.Value, true))
      return;
    this.RemoveFireteamMember(ent.Comp.Fireteams, args.Marine);
    FireteamMemberComponent fireteamMemberComponent = this.EnsureComp<FireteamMemberComponent>(entity.Value);
    fireteamMemberComponent.Fireteam = args.Fireteam;
    this.Dirty(entity.Value, (IComponent) fireteamMemberComponent);
    this.AddFireteamMember(ent.Comp.Fireteams, entity.Value);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(23, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" assigned ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "ToPrettyString(marine)");
    logStringHandler.AppendLiteral(" to fireteam ");
    logStringHandler.AppendFormatted<int>(args.Fireteam, "args.Fireteam");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCFireteam, ref local);
    this.Dirty<SquadLeaderTrackerComponent>(ent);
    this.SyncMemberFireteams((Entity<SquadMemberComponent>) ent.Owner);
  }

  private void OnUnassignFireteamMsg(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerUnassignFireteamMsg args)
  {
    EntityUid? entity;
    FireteamMemberComponent comp;
    if (this._net.IsClient || !this.TryGetEntity(args.Marine, out entity) || !this.CanChangeFireteamMember(args.Actor, entity.Value, false) || !this.TryComp<FireteamMemberComponent>(entity.Value, out comp))
      return;
    this.RemoveFireteamMember(ent.Comp.Fireteams, args.Marine);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(27, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" unassigned ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "ToPrettyString(marine)");
    logStringHandler.AppendLiteral(" from fireteam ");
    logStringHandler.AppendFormatted<int>(comp.Fireteam, "member.Fireteam");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCFireteam, ref local);
    this.Dirty<SquadLeaderTrackerComponent>(ent);
    this.SyncMemberFireteams((Entity<SquadMemberComponent>) ent.Owner);
  }

  private void OnPromoteFireteamLeaderMsg(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerPromoteFireteamLeaderMsg args)
  {
    EntityUid? entity;
    FireteamMemberComponent comp;
    if (this._net.IsClient || !this.TryGetEntity(args.Marine, out entity) || !this.CanChangeFireteamMember(args.Actor, entity.Value, true) || !this.TryComp<FireteamMemberComponent>(entity, out comp) || comp.Fireteam < 0 || comp.Fireteam >= ent.Comp.Fireteams.Fireteams.Length)
      return;
    NetEntity netEntity = this.GetNetEntity(entity.Value);
    ProtoId<JobPrototype>? job = (ProtoId<JobPrototype>?) this._originalRoleQuery.CompOrNull(entity.Value)?.Job;
    SpriteSpecifier.Rsi IconOverride = this.CompOrNull<RMCVendorRoleOverrideComponent>(entity)?.GiveIcon ?? this.CompOrNull<UsedSkillPamphletComponent>(entity)?.Icon;
    SquadLeaderTrackerMarine leaderTrackerMarine = new SquadLeaderTrackerMarine(netEntity, job, this._rank.GetSpeakerRankName(entity.Value) ?? this.Name(entity.Value), IconOverride);
    ref SquadLeaderTrackerFireteam local1 = ref ent.Comp.Fireteams.Fireteams[comp.Fireteam];
    if ((object) local1 == null)
      local1 = new SquadLeaderTrackerFireteam();
    this.DemoteFireteamLeader(local1, args.Actor);
    local1.Leader = new SquadLeaderTrackerMarine?(leaderTrackerMarine);
    this.EnsureComp<FireteamLeaderComponent>(entity.Value);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(29, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" promoted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "ToPrettyString(marineId)");
    logStringHandler.AppendLiteral(" to fireteam leader");
    ref LogStringHandler local2 = ref logStringHandler;
    adminLog.Add(LogType.RMCFireteam, ref local2);
    this.Dirty<SquadLeaderTrackerComponent>(ent);
    this.SyncMemberFireteams((Entity<SquadMemberComponent>) ent.Owner);
  }

  private void OnDemoteFireteamLeaderMsg(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerDemoteFireteamLeaderMsg args)
  {
    if (this._net.IsClient || args.Fireteam < 0 || args.Fireteam >= ent.Comp.Fireteams.Fireteams.Length)
      return;
    ref SquadLeaderTrackerFireteam local1 = ref ent.Comp.Fireteams.Fireteams[args.Fireteam];
    SquadLeaderTrackerFireteam leaderTrackerFireteam = local1;
    NetEntity? nEntity;
    if ((object) leaderTrackerFireteam == null)
    {
      nEntity = new NetEntity?();
    }
    else
    {
      ref SquadLeaderTrackerMarine? local2 = ref leaderTrackerFireteam.Leader;
      nEntity = local2.HasValue ? new NetEntity?(local2.GetValueOrDefault().Id) : new NetEntity?();
    }
    EntityUid? nullable;
    ref EntityUid? local3 = ref nullable;
    if (!this.TryGetEntity(nEntity, out local3) || !this.CanChangeFireteamMember(args.Actor, nullable.Value, false))
      return;
    this.DemoteFireteamLeader(local1, args.Actor);
    this.Dirty<SquadLeaderTrackerComponent>(ent);
    this.SyncMemberFireteams((Entity<SquadMemberComponent>) ent.Owner);
  }

  private void OnChangeTrackedMsg(
    Entity<SquadLeaderTrackerComponent> ent,
    ref SquadLeaderTrackerChangeTrackedMsg args)
  {
    List<DialogOption> options = new List<DialogOption>();
    foreach (ProtoId<TrackerModePrototype> trackerMode in ent.Comp.TrackerModes)
      options.Add(new DialogOption(this.Loc.GetString("rmc-squad-info-" + (string) trackerMode), (object) new SquadLeaderTrackerChangeModeEvent(trackerMode)));
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-squad-info-tracking-selection"), options, this.Loc.GetString("rmc-squad-info-tracking-choose"));
  }

  private bool CanChangeFireteamMember(EntityUid user, EntityUid target, bool add)
  {
    return this.HasComp<SquadLeaderComponent>(user) && this._squad.AreInSameSquad((Entity<SquadMemberComponent>) user, (Entity<SquadMemberComponent>) target) && (!add || !this.HasComp<SquadLeaderComponent>(target));
  }

  private void SyncMemberFireteams(Entity<SquadMemberComponent?> member)
  {
    if (!this.Resolve<SquadMemberComponent>((EntityUid) member, ref member.Comp, false) || !member.Comp.Squad.HasValue)
      return;
    this.SyncFireteams((Entity<SquadTeamComponent>) member.Comp.Squad.Value);
  }

  private void SyncFireteams(Entity<SquadTeamComponent?> squad)
  {
    if (!this.Resolve<SquadTeamComponent>((EntityUid) squad, ref squad.Comp, false))
      return;
    Array.Clear((Array) squad.Comp.Fireteams.Fireteams);
    squad.Comp.Fireteams.Unassigned.Clear();
    Entity<SquadLeaderComponent> leader;
    if (this._squad.TryGetSquadLeader((Entity<SquadTeamComponent>) ((EntityUid) squad, squad.Comp), out leader))
    {
      squad.Comp.Fireteams.SquadLeader = this.Name((EntityUid) leader);
      squad.Comp.Fireteams.SquadLeaderId = new NetEntity?(this.GetNetEntity((EntityUid) leader));
    }
    else
    {
      squad.Comp.Fireteams.SquadLeader = (string) null;
      squad.Comp.Fireteams.SquadLeaderId = new NetEntity?();
    }
    foreach (EntityUid member in squad.Comp.Members)
      this.AddFireteamMember(squad.Comp.Fireteams, member);
  }

  private void AddFireteamMember(FireteamData fireteamData, EntityUid member)
  {
    NetEntity netEntity = this.GetNetEntity(member);
    ProtoId<JobPrototype>? job = (ProtoId<JobPrototype>?) this._originalRoleQuery.CompOrNull(member)?.Job;
    SpriteSpecifier.Rsi IconOverride = this.CompOrNull<RMCVendorRoleOverrideComponent>(member)?.GiveIcon ?? this.CompOrNull<UsedSkillPamphletComponent>(member)?.Icon;
    SquadLeaderTrackerMarine leaderTrackerMarine = new SquadLeaderTrackerMarine(netEntity, job, this._rank.GetSpeakerRankName(member) ?? this.Name(member), IconOverride);
    FireteamMemberComponent component1;
    if (this._fireteamMemberQuery.TryComp(member, out component1) && component1.Fireteam >= 0 && component1.Fireteam < fireteamData.Fireteams.Length)
    {
      ref SquadLeaderTrackerFireteam local1 = ref fireteamData.Fireteams[component1.Fireteam];
      if ((object) local1 == null)
        local1 = new SquadLeaderTrackerFireteam();
      SquadLeaderTrackerFireteam leaderTrackerFireteam1 = local1;
      if (leaderTrackerFireteam1.Members == null)
        leaderTrackerFireteam1.Members = new Dictionary<NetEntity, SquadLeaderTrackerMarine>();
      local1.Members[netEntity] = leaderTrackerMarine;
      if (this._fireteamLeaderQuery.HasComp(member))
        local1.Leader = new SquadLeaderTrackerMarine?(leaderTrackerMarine);
      SquadLeaderTrackerComponent component2;
      if (this._squadLeaderTrackerQuery.TryComp(member, out component2) && local1.Leader.HasValue)
      {
        SquadLeaderTrackerFireteam leaderTrackerFireteam2 = local1;
        NetEntity? nEntity;
        if ((object) leaderTrackerFireteam2 == null)
        {
          nEntity = new NetEntity?();
        }
        else
        {
          ref SquadLeaderTrackerMarine? local2 = ref leaderTrackerFireteam2.Leader;
          nEntity = local2.HasValue ? new NetEntity?(local2.GetValueOrDefault().Id) : new NetEntity?();
        }
        EntityUid? target;
        ref EntityUid? local3 = ref target;
        if (this.TryGetEntity(nEntity, out local3))
        {
          EntityUid? nullable = target;
          EntityUid entityUid = member;
          if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
          {
            ProtoId<TrackerModePrototype> mode = (ProtoId<TrackerModePrototype>) "FireteamLeader";
            this.SetTarget((Entity<SquadLeaderTrackerComponent>) (member, component2), target);
            this.SetMode((Entity<SquadLeaderTrackerComponent>) (member, component2), mode);
          }
        }
      }
    }
    else
      fireteamData.Unassigned[netEntity] = leaderTrackerMarine;
    SquadLeaderTrackerComponent component3;
    if (!this._squadLeaderTrackerQuery.TryComp(member, out component3))
      return;
    component3.Fireteams = fireteamData;
    this.Dirty(member, (IComponent) component3);
  }

  private void RemoveFireteamMember(FireteamData fireteamData, NetEntity member)
  {
    foreach (SquadLeaderTrackerFireteam fireteam in fireteamData.Fireteams)
    {
      if ((object) fireteam != null)
      {
        ref SquadLeaderTrackerMarine? local = ref fireteam.Leader;
        NetEntity? nullable = local.HasValue ? new NetEntity?(local.GetValueOrDefault().Id) : new NetEntity?();
        NetEntity netEntity = member;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() == netEntity ? 1 : 0) : 0) != 0)
          fireteam.Leader = new SquadLeaderTrackerMarine?();
      }
      fireteam?.Members?.Remove(member);
    }
    fireteamData.Unassigned.Remove(member);
    EntityUid? entity;
    if (!this.TryGetEntity(member, out entity))
      return;
    this.RemComp<FireteamMemberComponent>(entity.Value);
    SquadLeaderTrackerComponent component;
    if (!this._squadLeaderTrackerQuery.TryComp(entity, out component))
      return;
    component.Fireteams = new FireteamData();
    this.Dirty(entity.Value, (IComponent) component);
  }

  private void DemoteFireteamLeader(SquadLeaderTrackerFireteam? fireteam, EntityUid user)
  {
    if (!(fireteam != (SquadLeaderTrackerFireteam) null))
      return;
    ref SquadLeaderTrackerMarine? local1 = ref fireteam.Leader;
    NetEntity? nullable = local1.HasValue ? new NetEntity?(local1.GetValueOrDefault().Id) : new NetEntity?();
    EntityUid? entity;
    if (!nullable.HasValue || !this.TryGetEntity(nullable.GetValueOrDefault(), out entity) || this.TerminatingOrDeleted(entity))
      return;
    this.RemComp<FireteamLeaderComponent>(entity.Value);
    fireteam.Leader = new SquadLeaderTrackerMarine?();
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(30, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" demoted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "ToPrettyString(oldLeader)");
    logStringHandler.AppendLiteral(" from fireteam leader");
    ref LogStringHandler local2 = ref logStringHandler;
    adminLog.Add(LogType.RMCFireteam, ref local2);
  }

  private void UpdateDirection(
    Entity<SquadLeaderTrackerComponent> ent,
    MapCoordinates? coordinates = null,
    string squad = "")
  {
    this._alerts.ClearAlertCategory((EntityUid) ent, (ProtoId<AlertCategoryPrototype>) "SquadTracker");
    TrackerModePrototype prototype;
    this._prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, out prototype);
    if (prototype == null)
      return;
    ProtoId<AlertPrototype> alertType = prototype.Alert;
    short num = TrackerSystem.CenterSeverity;
    ProtoId<TrackerModePrototype>? mode = ent.Comp.Mode;
    ProtoId<TrackerModePrototype>? nullable = (ProtoId<TrackerModePrototype>?) "SquadLeader";
    if ((mode.HasValue == nullable.HasValue ? (mode.HasValue ? (mode.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      alertType = (ProtoId<AlertPrototype>) ((string) alertType + squad);
    if (coordinates.HasValue)
      num = this._tracker.GetAlertSeverity(ent.Owner, coordinates.Value);
    this._alerts.ShowAlert(ent.Owner, alertType, new short?(num));
  }

  private void SetTarget(Entity<SquadLeaderTrackerComponent> ent, EntityUid? target)
  {
    ent.Comp.Target = target;
    this.Dirty<SquadLeaderTrackerComponent>(ent);
  }

  private void SetMode(Entity<SquadLeaderTrackerComponent> ent, ProtoId<TrackerModePrototype> mode)
  {
    ent.Comp.Mode = new ProtoId<TrackerModePrototype>?(mode);
    this.Dirty<SquadLeaderTrackerComponent>(ent);
  }

  private void OnRoleChange(Entity<SquadLeaderTrackerComponent> ent, ref GetMarineSquadNameEvent _)
  {
    this.SyncMemberFireteams((Entity<SquadMemberComponent>) ent.Owner);
  }

  public bool TryFindTargets(
    ProtoId<TrackerModePrototype> mode,
    out List<DialogOption> options,
    out List<EntityUid> trackingOptions)
  {
    options = new List<DialogOption>();
    trackingOptions = new List<EntityUid>();
    TrackerModePrototype prototype;
    this._prototypeManager.TryIndex<TrackerModePrototype>(mode, out prototype);
    if (prototype == null)
      return false;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTrackableComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCTrackableComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out RMCTrackableComponent _))
    {
      if (prototype.Component != null)
      {
        Type type = this._factory.GetComponent(prototype.Component).GetType();
        string text = "";
        if (this.EntityManager.TryGetComponent(uid, type, out IComponent _))
        {
          if (!this._net.IsClient)
            text = this.Name(uid);
          RequestTrackableNameEvent args = new RequestTrackableNameEvent();
          this.RaiseLocalEvent<RequestTrackableNameEvent>(uid, ref args);
          if (args.Name != null)
            text = args.Name;
          options.Add(new DialogOption(text, (object) new LeaderTrackerSelectTargetEvent(this.GetNetEntity(uid), mode)));
          trackingOptions.Add(uid);
        }
      }
      else
      {
        string str1 = "NoOriginalRole";
        string str2 = "";
        OriginalRoleComponent component1;
        ProtoId<JobPrototype>? nullable;
        if (this._originalRoleQuery.TryComp(uid, out component1))
        {
          nullable = component1.Job;
          str1 = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
        }
        nullable = (ProtoId<JobPrototype>?) str1;
        ProtoId<JobPrototype>? job = prototype.Job;
        if ((nullable.HasValue == job.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != job.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 || !(mode != (ProtoId<TrackerModePrototype>) "SquadLeader") && this.HasComp<SquadLeaderComponent>(uid))
        {
          SquadMemberComponent component2;
          if (!this._net.IsClient && this._squadMemberQuery.TryComp(uid, out component2))
          {
            EntityUid? squad = component2.Squad;
            if (squad.HasValue)
              str2 = this.Name(squad.GetValueOrDefault());
          }
          options.Add(new DialogOption($"({str2}) {this._rank.GetSpeakerFullRankName(uid)}", (object) new LeaderTrackerSelectTargetEvent(this.GetNetEntity(uid), mode)));
          trackingOptions.Add(uid);
        }
      }
    }
    return true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    this._squadLeaders.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent, RMCTrackableComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent, RMCTrackableComponent>();
    EntityUid uid1;
    SquadMemberComponent comp2_1;
    RMCTrackableComponent trackableComponent;
    EntityUid? squad1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out SquadLeaderComponent _, out comp2_1, out trackableComponent))
    {
      squad1 = comp2_1.Squad;
      if (squad1.HasValue)
        this._squadLeaders.TryAdd(squad1.GetValueOrDefault(), this._transform.GetMapCoordinates(uid1));
    }
    Array.Clear((Array) this._fireteamLeaders);
    Robust.Shared.GameObjects.EntityQueryEnumerator<FireteamLeaderComponent, FireteamMemberComponent, SquadMemberComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<FireteamLeaderComponent, FireteamMemberComponent, SquadMemberComponent>();
    EntityUid uid2;
    FireteamMemberComponent comp2_2;
    SquadMemberComponent comp3;
    while (entityQueryEnumerator2.MoveNext(out uid2, out FireteamLeaderComponent _, out comp2_2, out comp3))
    {
      squad1 = comp3.Squad;
      if (squad1.HasValue)
      {
        EntityUid valueOrDefault = squad1.GetValueOrDefault();
        if (comp2_2.Fireteam >= 0 && comp2_2.Fireteam < this._fireteamLeaders.Length)
        {
          ref Dictionary<EntityUid, MapCoordinates> local = ref this._fireteamLeaders[comp2_2.Fireteam];
          if (local == null)
            local = new Dictionary<EntityUid, MapCoordinates>();
          local[valueOrDefault] = this._transform.GetMapCoordinates(uid2);
        }
      }
    }
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<SquadLeaderTrackerComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<SquadLeaderTrackerComponent>();
    EntityUid uid3;
    SquadLeaderTrackerComponent comp1;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1))
    {
      if (!(curTime < comp1.UpdateAt))
      {
        comp1.UpdateAt = curTime + comp1.UpdateEvery;
        string squad2 = "";
        ProtoId<TrackerModePrototype>? nullable1;
        ProtoId<TrackerModePrototype>? nullable2;
        if (comp1.Target.HasValue)
        {
          nullable1 = comp1.Mode;
          nullable2 = (ProtoId<TrackerModePrototype>?) "SquadLeader";
          SquadMemberComponent component;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && !this.HasComp<SquadLeaderComponent>(comp1.Target) && this._squadMemberQuery.TryComp(comp1.Target.Value, out component))
          {
            squad1 = component.Squad;
            if (squad1.HasValue)
            {
              EntityUid valueOrDefault = squad1.GetValueOrDefault();
              SquadTeamComponent comp;
              Entity<SquadLeaderComponent> leader;
              if (this.TryComp<SquadTeamComponent>(valueOrDefault, out comp) && this._squad.TryGetSquadLeader((Entity<SquadTeamComponent>) (valueOrDefault, comp), out leader))
              {
                this.SetTarget((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new EntityUid?((EntityUid) leader));
                string squad3 = this.Name(valueOrDefault);
                MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(comp1.Target.Value);
                this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new MapCoordinates?(mapCoordinates), squad3);
                continue;
              }
            }
          }
        }
        SquadMemberComponent component1;
        if (this._squadMemberQuery.TryComp(uid3, out component1))
        {
          squad1 = component1.Squad;
          if (squad1.HasValue)
          {
            EntityUid valueOrDefault = squad1.GetValueOrDefault();
            FireteamMemberComponent component2;
            if (this._fireteamMemberQuery.TryComp(uid3, out component2))
            {
              nullable2 = comp1.Mode;
              nullable1 = (ProtoId<TrackerModePrototype>?) "FireteamLeader";
              if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
              {
                int fireteam = component2.Fireteam;
                if (fireteam >= 0 && fireteam < this._fireteamLeaders.Length)
                {
                  Dictionary<EntityUid, MapCoordinates> fireteamLeader = this._fireteamLeaders[fireteam];
                  MapCoordinates mapCoordinates;
                  if (fireteamLeader != null && fireteamLeader.TryGetValue(valueOrDefault, out mapCoordinates))
                  {
                    this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new MapCoordinates?(mapCoordinates), this.Name(valueOrDefault));
                    continue;
                  }
                  goto label_35;
                }
                goto label_35;
              }
            }
            nullable1 = comp1.Mode;
            nullable2 = (ProtoId<TrackerModePrototype>?) "SquadLeader";
            MapCoordinates mapCoordinates1;
            if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this._squadLeaders.TryGetValue(valueOrDefault, out mapCoordinates1))
            {
              string squad4 = this.Name(valueOrDefault);
              if (this.HasComp<SquadLeaderComponent>(uid3) && comp1.Target.HasValue)
              {
                SquadMemberComponent component3;
                if (this._squadMemberQuery.TryComp(comp1.Target.Value, out component3))
                {
                  squad1 = component3.Squad;
                  if (squad1.HasValue)
                    squad4 = this.Name(squad1.GetValueOrDefault());
                }
                mapCoordinates1 = this._transform.GetMapCoordinates(comp1.Target.Value);
              }
              this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new MapCoordinates?(mapCoordinates1), squad4);
              continue;
            }
          }
        }
label_35:
        if (comp1.Target.HasValue)
        {
          SquadMemberComponent component4;
          if (this._squadMemberQuery.TryComp(comp1.Target, out component4) && component4.Squad.HasValue)
            squad2 = this.Name(component4.Squad.Value);
          this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new MapCoordinates?(this._transform.GetMapCoordinates(comp1.Target.Value)), squad2);
        }
        else
        {
          Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTrackableComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<RMCTrackableComponent>();
          EntityUid uid4;
          while (entityQueryEnumerator4.MoveNext(out uid4, out trackableComponent))
          {
            TrackerModePrototype prototype;
            this._prototypeManager.TryIndex<TrackerModePrototype>(comp1.Mode, out prototype);
            if (prototype != null)
            {
              if (prototype.Component != null)
              {
                Type type = this._factory.GetComponent(prototype.Component).GetType();
                if (this.EntityManager.TryGetComponent(uid4, type, out IComponent _))
                {
                  this.SetTarget((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new EntityUid?(uid4));
                  if (comp1.Target.HasValue)
                  {
                    this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new MapCoordinates?(this._transform.GetMapCoordinates(comp1.Target.Value)), squad2);
                    break;
                  }
                  break;
                }
                break;
              }
              string str = "NoOriginalRole";
              OriginalRoleComponent component5;
              ProtoId<JobPrototype>? nullable3;
              if (this._originalRoleQuery.TryComp(uid4, out component5))
              {
                nullable3 = component5.Job;
                str = nullable3.HasValue ? (string) nullable3.GetValueOrDefault() : (string) null;
              }
              nullable3 = (ProtoId<JobPrototype>?) str;
              ProtoId<JobPrototype>? job = prototype.Job;
              if ((nullable3.HasValue == job.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() != job.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
              {
                SquadMemberComponent component6;
                if (this._squadMemberQuery.TryComp(comp1.Target, out component6) && component6.Squad.HasValue)
                  squad2 = this.Name(component6.Squad.Value);
                comp1.Target = new EntityUid?(uid4);
                this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1), new MapCoordinates?(this._transform.GetMapCoordinates(comp1.Target.Value)), squad2);
                break;
              }
            }
          }
          this.UpdateDirection((Entity<SquadLeaderTrackerComponent>) (uid3, comp1));
        }
      }
    }
  }
}
