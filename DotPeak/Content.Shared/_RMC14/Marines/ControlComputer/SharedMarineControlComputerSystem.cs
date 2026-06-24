// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.ControlComputer.SharedMarineControlComputerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.AlertLevel;
using Content.Shared._RMC14.Commendations;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Evacuation;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Survivor;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.Ghost;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.ControlComputer;

public abstract class SharedMarineControlComputerSystem : EntitySystem
{
  [Dependency]
  private RMCAlertLevelSystem _alertLevel;
  [Dependency]
  private SharedCommendationSystem _commendation;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedEvacuationSystem _evacuation;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private WarshipSystem _warship;
  private static readonly ProtoId<LocalizedDatasetPrototype> MedalsDatasetId = (ProtoId<LocalizedDatasetPrototype>) "RMCMarineMedals";
  private LocalizedDatasetPrototype _medalsDataset;
  private int _characterLimit = 1000;

  public override void Initialize()
  {
    base.Initialize();
    this._medalsDataset = this._prototype.Index<LocalizedDatasetPrototype>(SharedMarineControlComputerSystem.MedalsDatasetId);
    this.SubscribeLocalEvent<EvacuationEnabledEvent>(new EntityEventRefHandler<EvacuationEnabledEvent>(this.OnRefreshComputers<EvacuationEnabledEvent>));
    this.SubscribeLocalEvent<EvacuationDisabledEvent>(new EntityEventRefHandler<EvacuationDisabledEvent>(this.OnRefreshComputers<EvacuationDisabledEvent>));
    this.SubscribeLocalEvent<EvacuationProgressEvent>(new EntityEventRefHandler<EvacuationProgressEvent>(this.OnRefreshComputers<EvacuationProgressEvent>));
    this.SubscribeLocalEvent<DropshipHijackStartEvent>(new EntityEventRefHandler<DropshipHijackStartEvent>(this.OnRefreshComputers<DropshipHijackStartEvent>));
    this.SubscribeLocalEvent<RMCAlertLevelChangedEvent>(new EntityEventRefHandler<RMCAlertLevelChangedEvent>(this.OnRefreshComputers<RMCAlertLevelChangedEvent>));
    this.SubscribeLocalEvent<MarineControlComputerComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<MarineControlComputerComponent, BeforeActivatableUIOpenEvent>(this.OnComputerBeforeUIOpen));
    this.SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerMedalMarineEvent>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalMarineEvent>(this.OnComputerMedalMarine));
    this.SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerMedalNameEvent>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalNameEvent>(this.OnComputerMedalName));
    this.SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerMedalMessageEvent>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalMessageEvent>(this.OnComputerMedalMessage));
    this.SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerAlertEvent>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerAlertEvent>(this.OnComputerAlert));
    this.SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerShipAnnouncementDialogEvent>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerShipAnnouncementDialogEvent>(this.OnShipAnnouncementDialog));
    this.Subs.BuiEvents<MarineControlComputerComponent>((object) MarineControlComputerUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<MarineControlComputerComponent>) (subs =>
    {
      subs.Event<MarineControlComputerAlertLevelMsg>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerAlertLevelMsg>(this.OnAlertLevel));
      subs.Event<MarineControlComputerShipAnnouncementMsg>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerShipAnnouncementMsg>(this.OnShipAnnouncement));
      subs.Event<MarineControlComputerMedalMsg>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalMsg>(this.OnMedal));
      subs.Event<MarineControlComputerToggleEvacuationMsg>(new EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerToggleEvacuationMsg>(this.OnToggleEvacuationMsg));
    }));
    this.Subs.BuiEvents<MarineCommunicationsComputerComponent>((object) MarineCommunicationsComputerUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<MarineCommunicationsComputerComponent>) (subs => subs.Event<MarineControlComputerToggleEvacuationMsg>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineControlComputerToggleEvacuationMsg>(this.OnMarineCommunicationsToggleEvacuation))));
    this.Subs.CVar<int>(this._config, CCVars.ChatMaxMessageLength, (Action<int>) (limit => this._characterLimit = limit), true);
  }

  private void OnRefreshComputers<T>(ref T ev) => this.RefreshComputers();

  private void OnComputerBeforeUIOpen(
    Entity<MarineControlComputerComponent> ent,
    ref BeforeActivatableUIOpenEvent args)
  {
    this.RefreshComputers();
  }

  private void OnComputerMedalMarine(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerMedalMarineEvent args)
  {
    EntityUid? entity1;
    if (!this.TryGetEntity(args.Actor, out entity1))
      return;
    if (args.Marine.HasValue)
    {
      EntityUid? entity2;
      if (!this.TryGetEntity(args.Marine, out entity2))
        return;
      EntityUid? nullable1 = entity2;
      EntityUid? nullable2 = entity1;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-medal-error-self-award"), entity1, PopupType.MediumCaution);
        return;
      }
    }
    else if (args.LastPlayerId == null)
      return;
    if (this._net.IsClient)
      return;
    List<DialogOption> options = new List<DialogOption>();
    foreach (string messageId in this._medalsDataset.Values)
      options.Add(new DialogOption(this.Loc.GetString(messageId), (object) new MarineControlComputerMedalNameEvent(args.Actor, args.Marine, this.Loc.GetString(messageId), args.LastPlayerId)));
    this._dialog.OpenOptions((EntityUid) ent, entity1.Value, this.Loc.GetString("rmc-medal-type"), options, this.Loc.GetString("rmc-medal-type-prompt"));
  }

  private void OnComputerMedalName(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerMedalNameEvent args)
  {
    EntityUid? entity;
    if (this._net.IsClient || !this.TryGetEntity(args.Actor, out entity))
      return;
    MarineControlComputerMedalMessageEvent ev = new MarineControlComputerMedalMessageEvent(args.Actor, args.Marine, args.Name, LastPlayerId: args.LastPlayerId);
    this._dialog.OpenInput((EntityUid) ent, entity.Value, this.Loc.GetString("rmc-medal-citation-prompt"), (DialogInputEvent) ev, true, this._commendation.CharacterLimit);
  }

  private void OnComputerMedalMessage(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerMedalMessageEvent args)
  {
    EntityUid? entity1;
    if (!this.TryGetEntity(args.Actor, out entity1) || !this.HasComp<CommendationGiverComponent>(entity1) || string.IsNullOrWhiteSpace(args.Message.Trim()))
      return;
    if (args.Marine.HasValue)
    {
      EntityUid? entity2;
      CommendationReceiverComponent comp;
      if (!this.TryGetEntity(args.Marine, out entity2) || !this.TryComp<CommendationReceiverComponent>(entity2, out comp) || comp.LastPlayerId == null)
        return;
      this._commendation.GiveCommendation((Entity<CommendationGiverComponent, ActorComponent>) entity1.Value, (Entity<CommendationReceiverComponent>) entity2.Value, args.Name, args.Message, CommendationType.Medal);
    }
    else
    {
      if (args.LastPlayerId == null)
        return;
      MarineControlComputerComponent comp;
      GibbedMarineInfo gibbedMarineInfo;
      if (this.TryComp<MarineControlComputerComponent>((EntityUid) ent, out comp) && comp.GibbedMarines.TryGetValue(args.LastPlayerId, out gibbedMarineInfo))
        this._commendation.GiveCommendationByLastPlayerId((Entity<CommendationGiverComponent, ActorComponent>) entity1.Value, args.LastPlayerId, gibbedMarineInfo.Name, args.Name, args.Message, CommendationType.Medal);
    }
    if (this._net.IsClient)
      return;
    this._popup.PopupCursor(this.Loc.GetString("rmc-medal-awarded"), entity1.Value, PopupType.Large);
  }

  private void OnComputerAlert(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerAlertEvent args)
  {
    this._alertLevel.Set(args.Level, new EntityUid?(this.GetEntity(args.User)));
  }

  private void OnAlertLevel(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerAlertLevelMsg args)
  {
    RMCAlertLevels? nullable1 = this._alertLevel.Get();
    List<DialogOption> options = new List<DialogOption>();
    foreach (RMCAlertLevels Level in Enum.GetValues<RMCAlertLevels>())
    {
      int num = (int) Level;
      RMCAlertLevels? nullable2 = nullable1;
      int valueOrDefault = (int) nullable2.GetValueOrDefault();
      if (!(num == valueOrDefault & nullable2.HasValue) && Level < RMCAlertLevels.Red)
      {
        string text = this.Loc.GetString("rmc-alert-" + Level.ToString().ToLowerInvariant());
        options.Add(new DialogOption(text, (object) new MarineControlComputerAlertEvent(this.GetNetEntity(args.Actor), Level)));
      }
    }
    this._dialog.OpenOptions((EntityUid) ent, args.Actor, this.Loc.GetString("rmc-alert-level"), options, this.Loc.GetString("rmc-alert-level-which"));
  }

  private void OnShipAnnouncement(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerShipAnnouncementMsg args)
  {
    if (!this.CanUseShipAnnouncementPopup(ent, args.Actor))
      return;
    MarineControlComputerShipAnnouncementDialogEvent ev = new MarineControlComputerShipAnnouncementDialogEvent(this.GetNetEntity(args.Actor));
    this._dialog.OpenInput((EntityUid) ent, args.Actor, this.Loc.GetString("rmc-announcement-shipside-header"), (DialogInputEvent) ev, true, this._characterLimit);
  }

  private void OnShipAnnouncementDialog(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerShipAnnouncementDialogEvent args)
  {
    EntityUid entity = this.GetEntity(args.User);
    if (!entity.Valid || !this.CanUseShipAnnouncementPopup(ent, entity))
      return;
    ent.Comp.LastShipAnnouncement = new TimeSpan?(this._timing.CurTime);
    MapId mapId;
    MapId map = this._warship.TryGetWarshipMap((EntityUid) ent, out mapId) ? mapId : this._transform.GetMapId((Entity<TransformComponent>) ent.Owner);
    this._marineAnnounce.AnnounceSigned(entity, args.Message, this.Loc.GetString("rmc-announcement-author-shipside"), sound: SharedMarineAnnounceSystem.AresAnnouncementSound, filter: Filter.BroadcastMap(map).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => !this.HasComp<MarineComponent>(e) && !this.HasComp<GhostComponent>(e))), excludeSurvivors: false);
  }

  private void OnMedal(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerMedalMsg args)
  {
    this.GiveMedal((EntityUid) ent, args.Actor);
  }

  public void GiveMedal(EntityUid computer, EntityUid actor)
  {
    ActorComponent comp;
    if (!this.TryComp<ActorComponent>(actor, out comp))
      return;
    if (!this.HasComp<CommendationGiverComponent>(actor))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-medal-error-officer-only"), new EntityUid?(actor), PopupType.MediumCaution);
    }
    else
    {
      if (this._net.IsClient)
        return;
      NetEntity netEntity = this.GetNetEntity(actor);
      List<DialogOption> options = new List<DialogOption>();
      Robust.Shared.GameObjects.EntityQueryEnumerator<CommendationReceiverComponent, MarineComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<CommendationReceiverComponent, MarineComponent>();
      EntityUid uid;
      CommendationReceiverComponent comp1_1;
      while (entityQueryEnumerator1.MoveNext(out uid, out comp1_1, out MarineComponent _))
      {
        if (comp1_1.LastPlayerId != null && !(Guid.Parse(comp1_1.LastPlayerId) == (Guid) comp.PlayerSession.UserId) && !this.HasComp<RMCSurvivorComponent>(uid) && !(uid == actor))
          options.Add(new DialogOption(this.Name(uid), (object) new MarineControlComputerMedalMarineEvent(netEntity, new NetEntity?(this.GetNetEntity(uid)))));
      }
      Dictionary<string, GibbedMarineInfo> dictionary = new Dictionary<string, GibbedMarineInfo>();
      Robust.Shared.GameObjects.EntityQueryEnumerator<MarineControlComputerComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<MarineControlComputerComponent>();
      MarineControlComputerComponent comp1_2;
      string key4;
      GibbedMarineInfo gibbedMarineInfo4;
      while (entityQueryEnumerator2.MoveNext(out EntityUid _, out comp1_2))
      {
        foreach ((key4, gibbedMarineInfo4) in comp1_2.GibbedMarines)
        {
          string key3 = key4;
          GibbedMarineInfo gibbedMarineInfo3 = gibbedMarineInfo4;
          if (gibbedMarineInfo3.LastPlayerId != null)
            dictionary[key3] = gibbedMarineInfo3;
        }
      }
      foreach ((key4, gibbedMarineInfo4) in dictionary)
      {
        string LastPlayerId = key4;
        GibbedMarineInfo gibbedMarineInfo5 = gibbedMarineInfo4;
        options.Add(new DialogOption(gibbedMarineInfo5.Name, (object) new MarineControlComputerMedalMarineEvent(netEntity, new NetEntity?(), LastPlayerId)));
      }
      this._dialog.OpenOptions(computer, actor, this.Loc.GetString("rmc-medal-recipient"), options, this.Loc.GetString("rmc-medal-recipient-prompt"));
    }
  }

  private void OnToggleEvacuationMsg(
    Entity<MarineControlComputerComponent> ent,
    ref MarineControlComputerToggleEvacuationMsg args)
  {
    if (this._ui.HasUi(ent.Owner, (Enum) MarineControlComputerUi.Key))
      this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) MarineControlComputerUi.Key, new EntityUid?(args.Actor));
    if (this._ui.HasUi(ent.Owner, (Enum) MarineCommunicationsComputerUI.Key))
      this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) MarineCommunicationsComputerUI.Key, new EntityUid?(args.Actor));
    if (!ent.Comp.CanEvacuate)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < ent.Comp.LastToggle + ent.Comp.ToggleCooldown)
      return;
    ent.Comp.LastToggle = curTime;
    this._evacuation.ToggleEvacuation((SoundSpecifier) null, ent.Comp.EvacuationCancelledSound, this._transform.GetMap((Entity<TransformComponent>) ent.Owner));
    this.RefreshComputers();
  }

  private void OnMarineCommunicationsToggleEvacuation(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref MarineControlComputerToggleEvacuationMsg args)
  {
    MarineControlComputerComponent comp;
    if (!this.TryComp<MarineControlComputerComponent>(ent.Owner, out comp))
      return;
    this.OnToggleEvacuationMsg(new Entity<MarineControlComputerComponent>(ent.Owner, comp), ref args);
  }

  private void RefreshComputers()
  {
    if (this._net.IsClient)
      return;
    bool flag1 = this._alertLevel.IsRedOrDeltaAlert() || this._evacuation.IsEvacuationEnabled();
    bool flag2 = this._evacuation.IsEvacuationEnabled();
    Robust.Shared.GameObjects.EntityQueryEnumerator<MarineControlComputerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MarineControlComputerComponent>();
    EntityUid uid;
    MarineControlComputerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Evacuating = flag2;
      comp1.CanEvacuate = flag1;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private bool CanUseShipAnnouncementPopup(
    Entity<MarineControlComputerComponent> ent,
    EntityUid user)
  {
    TimeSpan announcementCooldown = ent.Comp.ShipAnnouncementCooldown;
    if (ent.Comp.LastShipAnnouncement.HasValue)
    {
      TimeSpan curTime = this._timing.CurTime;
      TimeSpan? shipAnnouncement = ent.Comp.LastShipAnnouncement;
      TimeSpan timeSpan = announcementCooldown;
      TimeSpan? nullable = shipAnnouncement.HasValue ? new TimeSpan?(shipAnnouncement.GetValueOrDefault() + timeSpan) : new TimeSpan?();
      if ((nullable.HasValue ? (curTime < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-announcement-cooldown", ("seconds", (object) (int) announcementCooldown.TotalSeconds)), new EntityUid?(user));
        return false;
      }
    }
    return true;
  }
}
