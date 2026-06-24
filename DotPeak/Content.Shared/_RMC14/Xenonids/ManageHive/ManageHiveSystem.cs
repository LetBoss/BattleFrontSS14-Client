// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.ManageHiveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Commendations;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.ManageHive.Boons;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Systems;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Popups;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive;

public sealed class ManageHiveSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedCommendationSystem _commendation;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private HiveBoonSystem _hiveBoon;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedPlaytimeManager _playtime;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private SharedXenoWatchSystem _xenoWatch;
  [Dependency]
  private XenoEvolutionSystem _xenoEvolution;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  private static readonly ProtoId<LocalizedDatasetPrototype> JelliesDatasetId = (ProtoId<LocalizedDatasetPrototype>) "RMCXenoJellies";
  private LocalizedDatasetPrototype _jelliesDataset;
  private int _jelliesPerQueen;
  private TimeSpan _burrowedLarvaSacrificeTime;
  private int _burrowedLarvaEvolutionPointsPer;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveActionEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveActionEvent>(this.OnManageHiveAction));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveDevolveEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveDevolveEvent>(this.OnManageHiveDevolve));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyEvent>(this.OnManageHiveJelly));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveSacrificeBurrowedEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveSacrificeBurrowedEvent>(this.OnSacrificeBurrowed));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveSacrificeBurrowedTargetEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveSacrificeBurrowedTargetEvent>(this.OnSacrificeBurrowedTarget));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveActivateBoonsEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveActivateBoonsEvent>(this.OnPurchaseBoons));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveActivateBoonsChosenEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveActivateBoonsChosenEvent>(this.OnPurchaseBoonsChosen));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyXenoEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyXenoEvent>(this.OnManageHiveJellyXeno));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyNameEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyNameEvent>(this.OnManageHiveJellyType));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyMessageEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyMessageEvent>(this.OnManageHiveJellyMessage));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveDevolveConfirmEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveDevolveConfirmEvent>(this.OnManageHiveDevolveConfirm));
    this.SubscribeLocalEvent<ManageHiveComponent, ManageHiveDevolveMessageEvent>(new EntityEventRefHandler<ManageHiveComponent, ManageHiveDevolveMessageEvent>(this.OnManageHiveDevolveMessage));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCJelliesPerQueen, (Action<int>) (v => this._jelliesPerQueen = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBurrowedLarvaSacrificeTimeMinutes, (Action<int>) (v => this._burrowedLarvaSacrificeTime = TimeSpan.FromMinutes((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBurrowedLarvaEvolutionPointsPer, (Action<int>) (v => this._burrowedLarvaEvolutionPointsPer = v), true);
    this._jelliesDataset = this._prototype.Index<LocalizedDatasetPrototype>(ManageHiveSystem.JelliesDatasetId);
  }

  private void OnManageHiveAction(
    Entity<ManageHiveComponent> manage,
    ref ManageHiveActionEvent args)
  {
    List<DialogOption> options = new List<DialogOption>()
    {
      new DialogOption(this.Loc.GetString("rmc-hivemanagement-deevolve"), (object) new ManageHiveDevolveEvent())
    };
    CommendationGiverComponent comp;
    if (this.TryComp<CommendationGiverComponent>((EntityUid) manage, out comp) && comp.Given < this._jelliesPerQueen)
      options.Add(new DialogOption(this.Loc.GetString("rmc-hivemanagement-reward"), (object) new ManageHiveJellyEvent()));
    options.Add(new DialogOption(this.Loc.GetString("rmc-hivemanagement-exchange-larva"), (object) new ManageHiveSacrificeBurrowedEvent()));
    options.Add(new DialogOption(this.Loc.GetString("rmc-boon-activate"), (object) new ManageHiveActivateBoonsEvent()));
    this._dialog.OpenOptions((EntityUid) manage, this.Loc.GetString("rmc-hivemanagement-hive-management"), options, this.Loc.GetString("rmc-hivemanagement-manage-the-hive"));
  }

  private void OnManageHiveDevolve(
    Entity<ManageHiveComponent> manage,
    ref ManageHiveDevolveEvent args)
  {
    Entity<XenoDevolveComponent> watched;
    if (this._net.IsClient || !this.CanDevolveTargetPopup(manage, out watched))
      return;
    EntProtoId[] devolvesTo = watched.Comp.DevolvesTo;
    if (devolvesTo.Length == 1)
    {
      string str1 = this.Name((EntityUid) watched);
      string str2 = (string) null;
      string name = this.Prototype((EntityUid) watched)?.Name;
      if (name != null)
        str2 = name;
      bool flag1 = str2 != null;
      EntityPrototype prototype;
      bool flag2 = this._prototype.TryIndex(devolvesTo[0], out prototype);
      string message;
      if (flag1 & flag2)
        message = this.Loc.GetString("rmc-hivemanagement-are-you-sure-deevolve-from-to", ("name", (object) str1), ("from", (object) (str2 ?? "")), ("to", (object) (prototype?.Name ?? "")));
      else
        message = !flag1 ? this.Loc.GetString("rmc-hivemanagement-are-you-sure-deevolve", ("name", (object) str1)) : this.Loc.GetString("rmc-hivemanagement-are-you-sure-deevolve-from", ("name", (object) str1), ("from", (object) (str2 ?? "")));
      this._dialog.OpenConfirmation((EntityUid) manage, this.Loc.GetString("rmc-hivemanagement-deevolution"), message, (object) new ManageHiveDevolveConfirmEvent(devolvesTo[0]));
    }
    else
    {
      List<DialogOption> options = new List<DialogOption>();
      foreach (EntProtoId entProtoId in devolvesTo)
      {
        string text = entProtoId.Id;
        EntityPrototype prototype;
        if (this._prototype.TryIndex(entProtoId, out prototype))
          text = prototype.Name;
        options.Add(new DialogOption(text, (object) new ManageHiveDevolveConfirmEvent(entProtoId)));
      }
      this._dialog.OpenOptions((EntityUid) manage, this.Loc.GetString("rmc-hivemanagement-choose-caste"), options);
    }
  }

  private void OnManageHiveJelly(Entity<ManageHiveComponent> ent, ref ManageHiveJellyEvent args)
  {
    CommendationGiverComponent comp1;
    if (this._net.IsClient || !this.TryComp<CommendationGiverComponent>((EntityUid) ent, out comp1))
      return;
    ActorComponent comp2;
    if (!this.TryComp<ActorComponent>((EntityUid) ent, out comp2))
      return;
    try
    {
      TimeSpan timeSpan;
      if (this._playtime.GetPlayTimes(comp2.PlayerSession).TryGetValue((string) ent.Comp.PlayTime, out timeSpan))
      {
        if (!(timeSpan < ent.Comp.JellyRequiredTime))
          goto label_8;
      }
      this._popup.PopupCursor(this.Loc.GetString("rmc-jelly-error-not-enough-playtime", ("requiredHours", (object) (int) ent.Comp.JellyRequiredTime.TotalHours)), (EntityUid) ent, PopupType.LargeCaution);
      return;
    }
    catch
    {
    }
label_8:
    if (!this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) ent.Owner, ent.Comp.JellyPlasmaCost, false))
      return;
    List<DialogOption> options = new List<DialogOption>();
    HiveMemberComponent comp3 = this.CompOrNull<HiveMemberComponent>((EntityUid) ent);
    Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent> manage = new Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>((EntityUid) ent, (ManageHiveComponent) ent, comp1, comp3, comp2);
    Robust.Shared.GameObjects.EntityQueryEnumerator<CommendationReceiverComponent, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CommendationReceiverComponent, HiveMemberComponent>();
    EntityUid uid;
    HiveMemberComponent comp2_1;
    while (entityQueryEnumerator.MoveNext(out uid, out CommendationReceiverComponent _, out comp2_1))
    {
      if (this.CanAwardJellyPopup(manage, (Entity<HiveMemberComponent>) (uid, comp2_1), false))
        options.Add(new DialogOption(this.Name(uid), (object) new ManageHiveJellyXenoEvent(this.GetNetEntity(uid))));
    }
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-jelly-recipient"), options, this.Loc.GetString("rmc-jelly-recipient-prompt"));
  }

  private void OnSacrificeBurrowed(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveSacrificeBurrowedEvent args)
  {
    if (this._net.IsClient || !this.CanSacrificeBurrowedPopup(ent, out Entity<HiveComponent> _))
      return;
    List<DialogOption> options = new List<DialogOption>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActorComponent, XenoComponent, XenoEvolutionComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActorComponent, XenoComponent, XenoEvolutionComponent>();
    EntityUid uid;
    XenoEvolutionComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out ActorComponent _, out XenoComponent _, out comp3))
    {
      if (!(uid == ent.Owner) && !this._mobState.IsIncapacitated(uid))
      {
        FixedPoint2 points = comp3.Points;
        FixedPoint2 max = comp3.Max;
        if (!(comp3.Points >= comp3.Max) && this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) uid))
        {
          string text = $"{this.Name(uid)} ({points.Int()}/{max.Int()})";
          ManageHiveSacrificeBurrowedTargetEvent ev = new ManageHiveSacrificeBurrowedTargetEvent(this.GetNetEntity(uid));
          options.Add(new DialogOption(text, (object) ev));
        }
      }
    }
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-hivemanagement-exchange-larva-title"), options, this.Loc.GetString("rmc-hivemanagement-exchange-larva-description", ("points", (object) this._burrowedLarvaEvolutionPointsPer)));
  }

  private void OnSacrificeBurrowedTarget(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveSacrificeBurrowedTargetEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid entity = this.GetEntity(args.Target);
    Entity<HiveComponent> hive;
    if (!entity.Valid || ent.Owner == entity || !this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) entity) || this._mobState.IsIncapacitated(entity) || !this.CanSacrificeBurrowedPopup(ent, out hive))
      return;
    this._hive.IncreaseBurrowedLarva(hive, -1);
    FixedPoint2 fixedPoint2 = this._xenoEvolution.AddPointsCapped((Entity<XenoEvolutionComponent>) entity, (FixedPoint2) this._burrowedLarvaEvolutionPointsPer);
    this._popup.PopupCursor(this.Loc.GetString("rmc-hivemanagement-exchange-larva-given-user", ("target", (object) ent), ("points", (object) fixedPoint2)), (EntityUid) ent);
    this._popup.PopupCursor(this.Loc.GetString("rmc-hivemanagement-exchange-larva-given-target", ("user", (object) ent), ("points", (object) fixedPoint2)), (EntityUid) ent);
  }

  private void OnPurchaseBoons(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveActivateBoonsEvent args)
  {
    if (this._net.IsClient)
      return;
    List<DialogOption> options = new List<DialogOption>();
    foreach ((EntityPrototype Prototype, HiveBoonDefinitionComponent Component) boon in this._hiveBoon.Boons)
    {
      string text = this.Loc.GetString("rmc-boon-name-cost", ("boon", (object) boon.Prototype.Name), ("cost", (object) boon.Component.Cost), ("pylons", (object) boon.Component.Pylons));
      ManageHiveActivateBoonsChosenEvent ev = new ManageHiveActivateBoonsChosenEvent((EntProtoId<HiveBoonDefinitionComponent>) boon.Prototype.ID);
      options.Add(new DialogOption(text, (object) ev));
    }
    int num = 0;
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (hive.HasValue)
      num = this._hiveBoon.EnsureBoons(hive.GetValueOrDefault()).Comp.RoyalResin;
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-boon-activate"), options, this.Loc.GetString("rmc-boon-message", ("current", (object) num)));
  }

  private void OnPurchaseBoonsChosen(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveActivateBoonsChosenEvent args)
  {
    if (this._net.IsClient)
      return;
    this._hiveBoon.TryActivateBoon(ent, args.Boon);
  }

  private void OnManageHiveJellyXeno(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveJellyXenoEvent args)
  {
    if (this._net.IsClient)
      return;
    List<DialogOption> options = new List<DialogOption>();
    foreach (string messageId in this._jelliesDataset.Values)
      options.Add(new DialogOption(this.Loc.GetString(messageId), (object) new ManageHiveJellyNameEvent(args.Xeno, this.Loc.GetString(messageId))));
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-jelly-type"), options, this.Loc.GetString("rmc-jelly-type-prompt"));
  }

  private void OnManageHiveJellyType(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveJellyNameEvent args)
  {
    if (this._net.IsClient)
      return;
    ManageHiveJellyMessageEvent ev = new ManageHiveJellyMessageEvent(args.Xeno, args.Name);
    this._dialog.OpenInput((EntityUid) ent, this.Loc.GetString("rmc-jelly-citation-prompt"), (DialogInputEvent) ev, true, this._commendation.CharacterLimit);
  }

  private void OnManageHiveJellyMessage(
    Entity<ManageHiveComponent> ent,
    ref ManageHiveJellyMessageEvent args)
  {
    EntityUid? entity;
    if (this._net.IsClient || !this.TryGetEntity(args.Xeno, out entity) || !this.CanAwardJellyPopup((Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>) ent.Owner, (Entity<HiveMemberComponent>) entity.Value) || !this._commendation.ValidCommendation((Entity<CommendationGiverComponent, ActorComponent>) ent.Owner, (Entity<CommendationReceiverComponent>) entity.Value, args.Message) || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) ent.Owner, ent.Comp.JellyPlasmaCost))
      return;
    this._commendation.GiveCommendation((Entity<CommendationGiverComponent, ActorComponent>) ent.Owner, (Entity<CommendationReceiverComponent>) entity.Value, this.Loc.GetString(args.Name), args.Message, CommendationType.Jelly);
    this._popup.PopupCursor(this.Loc.GetString("rmc-jelly-awarded"), (EntityUid) ent, PopupType.Large);
  }

  private void OnManageHiveDevolveConfirm(
    Entity<ManageHiveComponent> manage,
    ref ManageHiveDevolveConfirmEvent args)
  {
    Entity<XenoDevolveComponent> watched;
    if (this._net.IsClient || !this.CanDevolveTargetPopup(manage, out watched) || !((IEnumerable<EntProtoId>) watched.Comp.DevolvesTo).Contains<EntProtoId>((EntProtoId) args.Choice.Id))
      return;
    this._dialog.OpenInput((EntityUid) manage, this.Loc.GetString("rmc-hivemanagement-provide-reason", ("name", (object) this.Name((EntityUid) watched))), (DialogInputEvent) new ManageHiveDevolveMessageEvent(args.Choice));
  }

  private void OnManageHiveDevolveMessage(
    Entity<ManageHiveComponent> manage,
    ref ManageHiveDevolveMessageEvent args)
  {
    Entity<XenoDevolveComponent> watched;
    if (this._net.IsClient || !this.CanDevolveTargetPopup(manage, out watched) || !((IEnumerable<EntProtoId>) watched.Comp.DevolvesTo).Contains<EntProtoId>(args.Choice) || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) manage.Owner, manage.Comp.DevolvePlasmaCost))
      return;
    EntityStringRepresentation? prettyString = this.ToPrettyString(new EntityUid?((EntityUid) watched));
    EntityUid? nullable = this._xenoEvolution.Devolve(watched, args.Choice);
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    ActorComponent comp;
    if (this.TryComp<ActorComponent>(valueOrDefault, out comp))
    {
      string str = this.Loc.GetString("rmc-hivemanagement-queen-deevolving", ("reason", (object) args.Message));
      this._rmcChat.ChatMessageToOne(ChatChannel.Local, str, str, new EntityUid(), false, comp.PlayerSession.Channel);
      this._popup.PopupEntity(str, valueOrDefault, PopupType.LargeCaution);
    }
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(14, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) manage)), "ToPrettyString(manage)");
    logStringHandler.AppendLiteral(" devolved ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(prettyString, "oldString");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault), "ToPrettyString(devolution)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCDevolve, ref local);
  }

  private bool CanDevolveTargetPopup(
    Entity<ManageHiveComponent> manage,
    out Entity<XenoDevolveComponent> watched)
  {
    watched = new Entity<XenoDevolveComponent>();
    EntityUid watched1;
    if (!this._xenoWatch.TryGetWatched((Entity<XenoWatchingComponent>) manage.Owner, out watched1) || watched1 == manage.Owner)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-hivemanagement-must-overwatch"), (EntityUid) manage, (EntityUid) manage, PopupType.MediumCaution);
      return false;
    }
    XenoDevolveComponent comp;
    if (!this.TryComp<XenoDevolveComponent>(watched1, out comp) || comp.DevolvesTo.Length == 0)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-hivemanagement-cant-be-devolved", ("name", (object) this.Name(watched1))), watched1, (EntityUid) manage, PopupType.MediumCaution);
      return false;
    }
    if (!comp.CanBeDevolvedByOther)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-hivemanagement-cant-deevolve-larva"), watched1, (EntityUid) manage, PopupType.MediumCaution);
      return false;
    }
    if (!this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) manage.Owner, manage.Comp.DevolvePlasmaCost, false))
      return false;
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) manage.Owner, (Entity<HiveMemberComponent>) watched1))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-hivemanagement-cant-deevolve-other-hive"), watched1, (EntityUid) manage, PopupType.MediumCaution);
      return false;
    }
    watched = (Entity<XenoDevolveComponent>) (watched1, comp);
    return true;
  }

  private bool CanAwardJellyPopup(
    Entity<ManageHiveComponent?, CommendationGiverComponent?, HiveMemberComponent?, ActorComponent?> manage,
    Entity<HiveMemberComponent?> target,
    bool popup = true)
  {
    if (!this.Resolve<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>((EntityUid) manage, ref manage.Comp1, ref manage.Comp2, ref manage.Comp3, ref manage.Comp4, false))
      return false;
    CommendationReceiverComponent comp;
    if (!this.Resolve<HiveMemberComponent>((EntityUid) target, ref target.Comp, false) || !this._hive.FromSameHive((Entity<HiveMemberComponent>) manage.Owner, target) || !this.TryComp<CommendationReceiverComponent>((EntityUid) target, out comp) || comp.LastPlayerId == null || manage.Owner == target.Owner || Guid.Parse(comp.LastPlayerId) == (Guid) manage.Comp4.PlayerSession.UserId)
    {
      if (popup)
        this._popup.PopupCursor(this.Loc.GetString("rmc-jelly-error-cant-give"), (EntityUid) manage, PopupType.MediumCaution);
      return false;
    }
    if (manage.Comp2.Given < this._jelliesPerQueen)
      return true;
    if (popup)
      this._popup.PopupCursor(this.Loc.GetString("rmc-jelly-error-limit-reached", ("given", (object) manage.Comp2.Given), ("limit", (object) this._jelliesPerQueen)), (EntityUid) manage, PopupType.MediumCaution);
    return false;
  }

  private bool CanSacrificeBurrowedPopup(
    Entity<ManageHiveComponent> user,
    out Entity<HiveComponent> hive)
  {
    hive = new Entity<HiveComponent>();
    Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) user.Owner);
    if (!hive1.HasValue)
      return false;
    Entity<HiveComponent> valueOrDefault = hive1.GetValueOrDefault();
    hive = valueOrDefault;
    if (hive.Comp.BurrowedLarva <= 0)
    {
      this._popup.PopupCursor(this.Loc.GetString("rmc-hivemanagement-exchange-larva-not-enough"), (EntityUid) user, PopupType.MediumCaution);
      return false;
    }
    TimeSpan timeSpan = this._burrowedLarvaSacrificeTime - this._gameTicker.RoundDuration();
    if (timeSpan > TimeSpan.Zero)
    {
      this._popup.PopupCursor(this.Loc.GetString("rmc-hivemanagement-exchange-larva-need-minutes", ("minutes", (object) (int) timeSpan.TotalMinutes)), (EntityUid) user, PopupType.MediumCaution);
      return false;
    }
    return this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) user.Owner, user.Comp.SacrificeBurrowedLarvaForEvolutionCost, false);
  }
}
