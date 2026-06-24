// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Announce.SharedMarineAnnounceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Marines.ControlComputer;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared._RMC14.Marines.Announce;

public abstract class SharedMarineAnnounceSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedMarineControlComputerSystem _marineControlComputer;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRankSystem _rankSystem;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SquadSystem _squad;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  public static readonly SoundSpecifier DefaultAnnouncementSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/Marine/notice2.ogg");
  public static readonly SoundSpecifier DefaultSquadSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/tech_notification.ogg");
  public static readonly SoundSpecifier AresAnnouncementSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/AI/announce.ogg");
  public int CharacterLimit = 1000;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MarineCommunicationsComputerComponent, EchoSquadReasonEvent>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, EchoSquadReasonEvent>(this.OnEchoSquadReason));
    this.SubscribeLocalEvent<MarineCommunicationsComputerComponent, EchoSquadConfirmEvent>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, EchoSquadConfirmEvent>(this.OnEchoSquadConfirm));
    this.Subs.BuiEvents<MarineCommunicationsComputerComponent>((object) MarineCommunicationsComputerUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<MarineCommunicationsComputerComponent>) (subs =>
    {
      subs.Event<MarineCommunicationsComputerMsg>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsComputerMsg>(this.OnMarineCommunicationsComputerMsg));
      subs.Event<MarineCommunicationsOpenMapMsg>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsOpenMapMsg>(this.OnMarineCommunicationsOpenMapMsg));
      subs.Event<MarineCommunicationsEchoSquadMsg>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsEchoSquadMsg>(this.OnMarineCommunicationsEchoMsg));
      subs.Event<MarineCommunicationsOverwatchMsg>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineCommunicationsOverwatchMsg>(this.OnMarineCommunicationsOverwatchMsg));
      subs.Event<MarineControlComputerMedalMsg>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineControlComputerMedalMsg>(this.OnMarineCommunicationsMedalMsg));
    }));
    this.Subs.CVar<int>(this._config, CCVars.ChatMaxMessageLength, (Action<int>) (limit => this.CharacterLimit = limit), true);
  }

  private void OnEchoSquadReason(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref EchoSquadReasonEvent args)
  {
    EntityUid? entity;
    if (!ent.Comp.CanCreateEcho || !this.TryGetEntity(args.User, out entity))
      return;
    EchoSquadConfirmEvent ev = new EchoSquadConfirmEvent(args.User, args.Message);
    this._dialog.OpenConfirmation((EntityUid) ent, entity.Value, "Confirm Activation", "Confirm activation of Echo Squad for " + args.Message, (object) ev);
  }

  private void OnEchoSquadConfirm(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref EchoSquadConfirmEvent args)
  {
    EntityUid? entity;
    if (!ent.Comp.CanCreateEcho || !this.TryGetEntity(args.User, out entity))
      return;
    ent.Comp.CanCreateEcho = false;
    this.Dirty<MarineCommunicationsComputerComponent>(ent);
    if (this._squad.HasSquad((EntProtoId) SquadSystem.EchoSquadId))
      return;
    this._squad.TryEnsureSquad((EntProtoId) SquadSystem.EchoSquadId, out Entity<SquadTeamComponent> _);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(39, 2);
    logStringHandler.AppendLiteral("Echo squad was created by ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" with reason ");
    logStringHandler.AppendFormatted(args.Message);
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCSquadCreated, ref local);
  }

  private void OnMarineCommunicationsComputerMsg(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref MarineCommunicationsComputerMsg args)
  {
    if (string.IsNullOrWhiteSpace(args.Text))
      return;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) args.Actor, ent.Comp.AnnounceSkill, ent.Comp.AnnounceSkillLevel))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-skills-no-training", ("target", (object) ent)), new EntityUid?(args.Actor), PopupType.MediumCaution);
    }
    else
    {
      TimeSpan curTime1 = this._timing.CurTime;
      TimeSpan curTime2 = this._timing.CurTime;
      TimeSpan? lastAnnouncement = ent.Comp.LastAnnouncement;
      TimeSpan cooldown = ent.Comp.Cooldown;
      TimeSpan? nullable = lastAnnouncement.HasValue ? new TimeSpan?(lastAnnouncement.GetValueOrDefault() + cooldown) : new TimeSpan?();
      if ((nullable.HasValue ? (curTime2 < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-announcement-cooldown", ("seconds", (object) (int) ent.Comp.Cooldown.TotalSeconds)), new EntityUid?(args.Actor), PopupType.SmallCaution);
      }
      else
      {
        this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) MarineCommunicationsComputerUI.Key);
        string message = args.Text;
        if (message.Length > this.CharacterLimit)
          message = message.Substring(0, this.CharacterLimit).Trim();
        this.AnnounceSigned(args.Actor, message, name: ent.Comp.AnnounceName);
        ent.Comp.LastAnnouncement = new TimeSpan?(curTime1);
        this.Dirty<MarineCommunicationsComputerComponent>(ent);
      }
    }
  }

  private void OnMarineCommunicationsOpenMapMsg(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref MarineCommunicationsOpenMapMsg args)
  {
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) TacticalMapComputerUi.Key, args.Actor);
  }

  private void OnMarineCommunicationsEchoMsg(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref MarineCommunicationsEchoSquadMsg args)
  {
    if (!ent.Comp.CanCreateEcho || this._squad.HasSquad((EntProtoId) SquadSystem.EchoSquadId))
      return;
    EchoSquadReasonEvent ev = new EchoSquadReasonEvent(this.GetNetEntity(args.Actor));
    this._dialog.OpenInput((EntityUid) ent, args.Actor, "What is the purpose of Echo Squad?", (DialogInputEvent) ev);
  }

  private void OnMarineCommunicationsOverwatchMsg(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref MarineCommunicationsOverwatchMsg args)
  {
    if (!this._skills.HasSkill((Entity<SkillsComponent>) args.Actor, ent.Comp.OverwatchSkill, ent.Comp.OverwatchSkillLevel))
      this._popup.PopupClient("You are not trained in overwatch!", new EntityUid?(args.Actor), PopupType.LargeCaution);
    else
      this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) OverwatchConsoleUI.Key, args.Actor);
  }

  private void OnMarineCommunicationsMedalMsg(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref MarineControlComputerMedalMsg args)
  {
    if (!ent.Comp.CanGiveMedals)
      return;
    this._marineControlComputer.GiveMedal((EntityUid) ent, args.Actor);
  }

  public virtual void AnnounceRadio(
    EntityUid sender,
    string message,
    ProtoId<RadioChannelPrototype> channel)
  {
  }

  public virtual void AnnounceARESStaging(
    EntityUid? source,
    string message,
    SoundSpecifier? sound = null,
    LocId? announcement = null)
  {
  }

  public void AnnounceARES(EntityUid? source, string message, SoundSpecifier? sound = null)
  {
    this.AnnounceARESStaging(source, message, sound, (LocId?) "rmc-announcement-ares-command");
  }

  public virtual void AnnounceSquad(
    string message,
    EntProtoId<SquadTeamComponent> squad,
    SoundSpecifier? sound = null)
  {
  }

  public virtual void AnnounceSquad(string message, EntityUid squad, SoundSpecifier? sound = null)
  {
  }

  public virtual void AnnounceSingle(string message, EntityUid receiver, SoundSpecifier? sound = null)
  {
  }

  public virtual void AnnounceToMarines(
    string message,
    SoundSpecifier? sound = null,
    Filter? filter = null,
    bool excludeSurvivors = true)
  {
  }

  public virtual void AnnounceHighCommand(string message, string? author = null, SoundSpecifier? sound = null)
  {
  }

  public void AnnounceSigned(
    EntityUid sender,
    string message,
    string? author = null,
    string? name = null,
    SoundSpecifier? sound = null,
    Filter? filter = null,
    bool excludeSurvivors = true)
  {
    if (this._net.IsClient)
      return;
    if (author == null)
      author = this.Loc.GetString("rmc-announcement-author");
    if (name == null)
      name = this._rankSystem.GetSpeakerFullRankName(sender) ?? this.Name(sender);
    this.AnnounceToMarines(this.Loc.GetString("rmc-announcement-message-signed", (nameof (author), (object) author), (nameof (message), (object) message), (nameof (name), (object) name)), sound, filter, excludeSurvivors);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(27, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) sender), "source", "ToPrettyString(sender)");
    logStringHandler.AppendLiteral(" marine announced message: ");
    logStringHandler.AppendFormatted(message);
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCMarineAnnounce, ref local);
  }

  public string FormatHighCommand(string? author, string message)
  {
    if (author == null)
      author = this.Loc.GetString("rmc-announcement-author-highcommand");
    return this.Loc.GetString("rmc-announcement-message", (nameof (author), (object) author), (nameof (message), (object) message));
  }

  public string FormatARESStaging(LocId? author, string message)
  {
    author.GetValueOrDefault();
    if (!author.HasValue)
      author = new LocId?((LocId) "rmc-announcement-ares-message");
    ILocalizationManager loc = this.Loc;
    LocId? nullable = author;
    string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
    (string, object) valueTuple = (nameof (message), (object) FormattedMessage.EscapeText(message));
    return loc.GetString(valueOrDefault, valueTuple);
  }

  public string FormatARES(string message)
  {
    return this.FormatARESStaging((LocId?) "rmc-announcement-ares-command", message);
  }
}
