// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.Boons.HiveBoonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Bioscan;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Communications;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.GameTicking;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

public sealed class HiveBoonSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private SharedMindSystem _mind;
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
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCGameTickerSystem _rmcGameTicker;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private ISerializationManager _serialization;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedXenoAnnounceSystem _xenoAnnounce;
  private static readonly EntProtoId<HiveBoonDefinitionComponent> KingBoonId = (EntProtoId<HiveBoonDefinitionComponent>) "RMCHiveBoonKing";
  private static readonly EntProtoId<HiveKingCocoonComponent> KingCocoonId = (EntProtoId<HiveKingCocoonComponent>) "RMCHiveCocoonKing";
  private int _aliveMarineRequirement;
  private TimeSpan _royalResinEvery;
  private TimeSpan _kingVoteCandidateTimeRequired;
  private TimeSpan _kingFirstWarningTime;
  private TimeSpan _kingVoteStartTime;
  private TimeSpan _kingVoteAskCandidatesTime;
  private TimeSpan _kingVoteStartHatchingTime;
  private Robust.Shared.GameObjects.EntityQuery<ExcludedFromKingVoteComponent> _excludedFromKingVoteQuery;
  private readonly HashSet<ProtoId<PlayTimeTrackerPrototype>> _xenoJobs = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();

  public ImmutableArray<(EntityPrototype Prototype, HiveBoonDefinitionComponent Component)> Boons { get; private set; } = ImmutableArray<(EntityPrototype, HiveBoonDefinitionComponent)>.Empty;

  public TimeSpan CommunicationTowerXenoTakeoverTime { get; private set; }

  public override void Initialize()
  {
    this._excludedFromKingVoteQuery = this.GetEntityQuery<ExcludedFromKingVoteComponent>();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.SubscribeLocalEvent<HiveBoonActivateFireResistanceEvent>(new EntityEventHandler<HiveBoonActivateFireResistanceEvent>(this.OnActivateFireResistance));
    this.SubscribeLocalEvent<HiveBoonActivateLarvaSurgeEvent>(new EntityEventHandler<HiveBoonActivateLarvaSurgeEvent>(this.OnActivateLarvaSurge));
    this.SubscribeLocalEvent<HiveBoonActivateKingEvent>(new EntityEventHandler<HiveBoonActivateKingEvent>(this.OnActivateKing));
    this.SubscribeLocalEvent<XenoComponent, RMCGetFireImmunityEvent>(new EntityEventRefHandler<XenoComponent, RMCGetFireImmunityEvent>(this.OnGetTileFireImmunity));
    this.SubscribeLocalEvent<XenoComponent, GetIgnitionImmunityEvent>(new EntityEventRefHandler<XenoComponent, GetIgnitionImmunityEvent>(this.OnGetTileFireIgnitionImmunity));
    this.SubscribeLocalEvent<XenoComponent, HiveKingVoteDialogEvent>(new EntityEventRefHandler<XenoComponent, HiveKingVoteDialogEvent>(this.OnKingVote));
    this.SubscribeLocalEvent<HiveClusterComponent, ExaminedEvent>(new EntityEventRefHandler<HiveClusterComponent, ExaminedEvent>(this.OnClusterExamined));
    this.SubscribeLocalEvent<HiveClusterComponent, AfterEntityWeedingEvent>(new EntityEventRefHandler<HiveClusterComponent, AfterEntityWeedingEvent>(this.OnClusterAfterWeeding));
    this.SubscribeLocalEvent<HivePylonComponent, ExaminedEvent>(new EntityEventRefHandler<HivePylonComponent, ExaminedEvent>(this.OnPylonExamined));
    this.SubscribeLocalEvent<HivePylonComponent, EntityTerminatingEvent>(new EntityEventRefHandler<HivePylonComponent, EntityTerminatingEvent>(this.OnPylonTerminating));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, CommunicationsTowerStateChangedEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, CommunicationsTowerStateChangedEvent>(this.OnTowerBreak));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, RMCRepairableTargetAttemptEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, RMCRepairableTargetAttemptEvent>(this.OnTowerRepairAttempt));
    this.SubscribeLocalEvent<HiveKingCocoonComponent, EntityTerminatingEvent>(new EntityEventRefHandler<HiveKingCocoonComponent, EntityTerminatingEvent>(this.OnCocoonTerminating));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBoonsLiveMarineRequirement, (Action<int>) (v => this._aliveMarineRequirement = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCRoyalResinEveryMinutes, (Action<int>) (v => this._royalResinEvery = TimeSpan.FromMinutes((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCCommunicationTowerXenoTakeoverMinutes, (Action<int>) (v => this.CommunicationTowerXenoTakeoverTime = TimeSpan.FromMinutes((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCKingVoteCandidateTimeRequirementHours, (Action<int>) (v => this._kingVoteCandidateTimeRequired = TimeSpan.FromHours(v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCKingHatchingFirstWarningMinutes, (Action<int>) (v => this._kingFirstWarningTime = TimeSpan.FromMinutes((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCKingVoteStartTimeSeconds, (Action<int>) (v => this._kingVoteStartTime = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCKingVoteAskCandidatesTimeSeconds, (Action<int>) (v => this._kingVoteAskCandidatesTime = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCKingVoteStartHatchingTimeSeconds, (Action<int>) (v => this._kingVoteStartHatchingTime = TimeSpan.FromSeconds((long) v)), true);
    this.ReloadPrototypes();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<EntityPrototype>())
      return;
    this.ReloadPrototypes();
  }

  private void OnActivateFireResistance(HiveBoonActivateFireResistanceEvent ev)
  {
    this.EnsureComp<HiveBoonFireImmunityComponent>(ev.Boon);
    this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) ev.Boon, "The Queen has imbued us with flame-resistant chitin for 5 minutes.");
  }

  private void OnActivateLarvaSurge(HiveBoonActivateLarvaSurgeEvent ev)
  {
    this._hive.IncreaseBurrowedLarva(ev.Hive, 5);
    this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) ev.Boon, "The Queen has awakened 5 extra burrowed larva to join the hive!");
  }

  private void OnActivateKing(HiveBoonActivateKingEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HivePylonComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HivePylonComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HivePylonComponent _))
      this._area.TrySetCanOrbitalBombardRoofing((Entity<RoofingEntityComponent>) uid, false);
    EntityUid? core = ev.Core;
    if (!core.HasValue)
      return;
    EntityUid valueOrDefault = core.GetValueOrDefault();
    this._hive.SetHive((Entity<HiveMemberComponent>) this.SpawnAtPosition((string) HiveBoonSystem.KingCocoonId, valueOrDefault.ToCoordinates()), new EntityUid?((EntityUid) ev.Hive));
    string areaName = this._area.GetAreaName(valueOrDefault);
    this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-marine", ("area", (object) areaName)));
    this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) valueOrDefault, this.Loc.GetString("rmc-boon-king-announcement-xenos", ("area", (object) areaName)));
  }

  private void OnGetTileFireImmunity(Entity<XenoComponent> xeno, ref RMCGetFireImmunityEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveBoonFireImmunityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveBoonFireImmunityComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HiveBoonFireImmunityComponent _))
    {
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) uid, (Entity<HiveMemberComponent>) xeno.Owner))
      {
        ev.Ignite = false;
        ev.Immune = true;
        break;
      }
    }
  }

  private void OnGetTileFireIgnitionImmunity(
    Entity<XenoComponent> xeno,
    ref GetIgnitionImmunityEvent args)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveBoonFireImmunityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveBoonFireImmunityComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HiveBoonFireImmunityComponent _))
    {
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) uid, (Entity<HiveMemberComponent>) xeno.Owner))
        args.Ignite = false;
    }
  }

  private void OnKingVote(Entity<XenoComponent> ent, ref HiveKingVoteDialogEvent args)
  {
    EntityUid entity1 = this.GetEntity(args.Cocoon);
    if (!entity1.Valid)
      return;
    EntityUid entity2 = this.GetEntity(args.Voted);
    if (!entity2.Valid)
      return;
    bool flag;
    bool canVote;
    this.GetKingVotingData((Entity<ActorComponent>) ent.Owner, entity1, out flag, out canVote);
    if (!canVote)
      return;
    bool canBeKing;
    this.GetKingVotingData((Entity<ActorComponent>) entity2, entity1, out canBeKing, out flag);
    ActorComponent comp;
    if (!canBeKing || !this.TryComp<ActorComponent>(entity2, out comp))
      return;
    Entity<HiveKingVoteComponent> ent1 = this.EnsureVote(entity1);
    NetUserId userId = comp.PlayerSession.UserId;
    ent1.Comp.Votes[userId] = ent1.Comp.Votes.GetValueOrDefault<NetUserId, int>(userId) + 1;
    this.Dirty<HiveKingVoteComponent>(ent1);
  }

  private void OnPylonExamined(Entity<HivePylonComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("HivePylonComponent"))
    {
      string markup = $"[color=cyan]This will grant the hive 1 royal resin every {(int) this._royalResinEvery.TotalMinutes} minutes, allowing the Queen to obtain buffs![/color]";
      args.PushMarkup(markup);
    }
  }

  private void OnPylonTerminating(Entity<HivePylonComponent> ent, ref EntityTerminatingEvent args)
  {
    TransformComponent comp1;
    if (!this.TryComp((EntityUid) ent, out comp1) || this.TerminatingOrDeleted(comp1.MapUid) || this.HasComp<HiveConstructionSuppressAnnouncementsComponent>((EntityUid) ent))
      return;
    string areaName = this._area.GetAreaName((EntityUid) ent);
    this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-pylon-destroyed-announcement-marine", ("area", (object) areaName)));
    this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) ent.Owner, $"We have lost our control of the tall's communication relay at {areaName}.");
    EntityUid? tower = ent.Comp.Tower;
    if (!tower.HasValue)
      return;
    EntityUid valueOrDefault = tower.GetValueOrDefault();
    this._appearance.SetData(valueOrDefault, (Enum) WeededEntityLayers.Layer, (object) false);
    CommunicationsTowerComponent comp2;
    if (!this.TryComp<CommunicationsTowerComponent>(valueOrDefault, out comp2))
      return;
    comp2.XenoControlled = false;
    this.Dirty(valueOrDefault, (IComponent) comp2);
  }

  private void OnClusterExamined(Entity<HiveClusterComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("HivePylonComponent"))
    {
      string markup = $"[color=cyan]If placed {(int) this.CommunicationTowerXenoTakeoverTime.TotalMinutes} minutes into the round, this can turn into a hive pylon when its weeds take over a telecommunications tower![/color]";
      args.PushMarkup(markup);
    }
  }

  private void OnClusterAfterWeeding(
    Entity<HiveClusterComponent> ent,
    ref AfterEntityWeedingEvent args)
  {
    CommunicationsTowerComponent comp;
    if (this.TerminatingOrDeleted((EntityUid) ent) || !this.TryComp<CommunicationsTowerComponent>(args.CoveredEntity, out comp))
      return;
    this.ReplaceCluster(ent, (Entity<CommunicationsTowerComponent>) (args.CoveredEntity, comp));
  }

  private void OnTowerBreak(
    Entity<CommunicationsTowerComponent> ent,
    ref CommunicationsTowerStateChangedEvent args)
  {
    if (ent.Comp.State != CommunicationsTowerState.Broken)
      return;
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator((EntityUid) ent, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      XenoWeedsComponent comp1;
      HiveClusterComponent comp2;
      if (this.TryComp<XenoWeedsComponent>(uid, out comp1) && this.TryComp<HiveClusterComponent>(comp1.Source, out comp2))
        this.ReplaceCluster((Entity<HiveClusterComponent>) (comp1.Source.Value, comp2), ent);
    }
  }

  private void OnTowerRepairAttempt(
    Entity<CommunicationsTowerComponent> ent,
    ref RMCRepairableTargetAttemptEvent args)
  {
    if (!ent.Comp.XenoControlled)
      return;
    args.Cancelled = true;
    args.Popup = $"The {this.Name((EntityUid) ent)} is entangled in resin. Impossible to interact with.";
  }

  private void OnCocoonTerminating(
    Entity<HiveKingCocoonComponent> ent,
    ref EntityTerminatingEvent args)
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp) || this.TerminatingOrDeleted(comp.MapUid) || this.HasComp<HiveConstructionSuppressAnnouncementsComponent>((EntityUid) ent))
      return;
    this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-stopped-marine", ("area", (object) this._area.GetAreaName((EntityUid) ent))));
    this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) ent.Owner, this.Loc.GetString("rmc-boon-king-announcement-stopped-xeno"));
  }

  public bool HasEnoughAliveMarines()
  {
    if (this._aliveMarineRequirement <= 0)
      return true;
    int num = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActorComponent, MarineComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActorComponent, MarineComponent, TransformComponent>();
    EntityUid uid;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out ActorComponent _, out MarineComponent _, out comp3))
    {
      if (this._rmcPlanet.IsOnPlanet(comp3) && !this._mobState.IsIncapacitated(uid))
      {
        ++num;
        if (num >= this._aliveMarineRequirement)
          return true;
      }
    }
    return false;
  }

  private void StartKingVote(Entity<HiveKingCocoonComponent> cocoon)
  {
    this._xenoJobs.Clear();
    foreach (PlayTimeTrackerPrototype enumeratePrototype in this._prototype.EnumeratePrototypes<PlayTimeTrackerPrototype>())
    {
      if (enumeratePrototype.IsXeno)
        this._xenoJobs.Add((ProtoId<PlayTimeTrackerPrototype>) enumeratePrototype.ID);
    }
    List<DialogOption> options = new List<DialogOption>();
    List<EntityUid> entityUidList = new List<EntityUid>();
    NetEntity netEntity = this.GetNetEntity((EntityUid) cocoon);
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActorComponent, XenoComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActorComponent, XenoComponent>();
    EntityUid uid;
    ActorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out XenoComponent _))
    {
      bool canBeKing;
      bool canVote;
      this.GetKingVotingData((Entity<ActorComponent>) (uid, comp1), (EntityUid) cocoon, out canBeKing, out canVote);
      if (canBeKing)
        options.Add(new DialogOption(this.Name(uid), (object) new HiveKingVoteDialogEvent(netEntity, this.GetNetEntity(uid))));
      if (canVote)
        entityUidList.Add(uid);
    }
    foreach (EntityUid actor in entityUidList)
      this._dialog.OpenOptions(actor, "Choose a sister", options, "Vote for a sister you wish to become the King.");
    this.EnsureVote((EntityUid) cocoon);
  }

  private void GetKingVotingData(
    Entity<ActorComponent?> xeno,
    EntityUid cocoon,
    out bool canBeKing,
    out bool canVote)
  {
    canBeKing = false;
    canVote = false;
    if (!this.Resolve<ActorComponent>((EntityUid) xeno, ref xeno.Comp, false) || this._mobState.IsDead((EntityUid) xeno) || !this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) cocoon))
      return;
    ExcludedFromKingVoteComponent component;
    if (this._excludedFromKingVoteQuery.TryComp((EntityUid) xeno, out component))
    {
      canBeKing = component.CanBeKing;
      canVote = component.CanVote;
    }
    else
    {
      canVote = true;
      IReadOnlyDictionary<string, TimeSpan> playTimes;
      try
      {
        playTimes = this._playtime.GetPlayTimes(xeno.Comp.PlayerSession);
      }
      catch
      {
        return;
      }
      TimeSpan zero = TimeSpan.Zero;
      foreach ((string key, TimeSpan timeSpan) in (IEnumerable<KeyValuePair<string, TimeSpan>>) playTimes)
      {
        if (this._xenoJobs.Contains((ProtoId<PlayTimeTrackerPrototype>) key))
          zero += timeSpan;
      }
      if (zero < this._kingVoteCandidateTimeRequired)
        return;
      canBeKing = true;
    }
  }

  public Entity<HiveKingVoteComponent> EnsureVote(EntityUid xeno)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveKingVoteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveKingVoteComponent>();
    EntityUid uid;
    HiveKingVoteComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) uid, (Entity<HiveMemberComponent>) xeno))
        return (Entity<HiveKingVoteComponent>) (uid, comp1);
    }
    EntityUid entityUid = this.Spawn();
    HiveKingVoteComponent kingVoteComponent = this.EnsureComp<HiveKingVoteComponent>(entityUid);
    this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno, (Entity<HiveMemberComponent>) entityUid);
    return (Entity<HiveKingVoteComponent>) (entityUid, kingVoteComponent);
  }

  private void ReplaceCluster(
    Entity<HiveClusterComponent> cluster,
    Entity<CommunicationsTowerComponent> tower)
  {
    if (tower.Comp.XenoControlled || tower.Comp.State != CommunicationsTowerState.Broken || this._gameTicker.RoundDuration() < this.CommunicationTowerXenoTakeoverTime)
      return;
    EntityUid entityUid = this.SpawnAtPosition((string) cluster.Comp.TowerReplaceWith, cluster.Owner.ToCoordinates());
    this._hive.SetSameHive((Entity<HiveMemberComponent>) cluster.Owner, (Entity<HiveMemberComponent>) entityUid);
    this._appearance.SetData((EntityUid) tower, (Enum) WeededEntityLayers.Layer, (object) true);
    tower.Comp.XenoControlled = true;
    this.Dirty<CommunicationsTowerComponent>(tower);
    HivePylonComponent comp1;
    if (this.TryComp<HivePylonComponent>(entityUid, out comp1))
    {
      comp1.Tower = new EntityUid?((EntityUid) tower);
      comp1.NextRoyalResin = this._timing.CurTime + this._royalResinEvery;
      this.Dirty(entityUid, (IComponent) comp1);
      string areaName = this._area.GetAreaName((EntityUid) tower);
      this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-pylon-announcement-marine", ("area", (object) areaName)));
      this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) entityUid, $"We have harnessed the tall's communication relay at {areaName}.\n\nWe will now grow royal resin from this pylon. Hold it!");
    }
    XenoWeedsComponent comp2;
    XenoWeedsComponent comp3;
    if (!this.TryComp<XenoWeedsComponent>(entityUid, out comp2) || !this.TryComp<XenoWeedsComponent>((EntityUid) cluster, out comp3))
      return;
    foreach (EntityUid uid in comp3.Spread)
    {
      XenoWeedsComponent xenoWeedsComponent = this.EnsureComp<XenoWeedsComponent>(uid);
      xenoWeedsComponent.Range = comp2.Range;
      xenoWeedsComponent.Source = new EntityUid?(entityUid);
      comp2.Spread.Add(uid);
    }
    comp3.Spread.Clear();
    this.Dirty((EntityUid) cluster, (IComponent) comp3);
    this.RemComp<XenoWeedsSpreadingComponent>(entityUid);
    this.QueueDel(new EntityUid?((EntityUid) cluster));
  }

  public void TryActivateBoon(
    Entity<ManageHiveComponent> manage,
    EntProtoId<HiveBoonDefinitionComponent> boon)
  {
    EntityPrototype prototype;
    HiveBoonDefinitionComponent component;
    if (!this._prototype.TryIndex((EntProtoId) boon, out prototype) || !prototype.TryGetComponent<HiveBoonDefinitionComponent>(out component, this._compFactory))
      return;
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) manage.Owner);
    if (!hive.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault1 = hive.GetValueOrDefault();
    Entity<HiveBoonsComponent> entity = this.EnsureBoons(valueOrDefault1);
    if (entity.Comp.RoyalResin < component.Cost)
    {
      this._popup.PopupCursor(this.Loc.GetString("rmc-boon-not-enough-royal-resin", ("cost", (object) component.Cost), ("current", (object) entity.Comp.RoyalResin)), (EntityUid) manage, PopupType.MediumCaution);
    }
    else
    {
      int num = 0;
      Robust.Shared.GameObjects.EntityQueryEnumerator<HivePylonComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<HivePylonComponent>();
      EntityUid uid1;
      while (entityQueryEnumerator1.MoveNext(out uid1, out HivePylonComponent _))
      {
        if (this._hive.FromSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) manage.Owner))
          ++num;
      }
      if (num < component.Pylons)
      {
        this._popup.PopupCursor(this.Loc.GetString("rmc-boon-not-enough-pylons", ("cost", (object) component.Pylons), ("current", (object) num)), (EntityUid) manage, PopupType.MediumCaution);
      }
      else
      {
        TimeSpan unlockAt;
        if (!this.TryGetUnlockAt(entity, (EntProtoId<HiveBoonDefinitionComponent>) prototype.ID, out unlockAt))
          return;
        if (this._gameTicker.RoundDuration() < unlockAt)
          this._popup.PopupCursor(this.Loc.GetString("rmc-boon-not-enough-time"), (EntityUid) manage, PopupType.MediumCaution);
        else if (!this.HasEnoughAliveMarines())
        {
          this._popup.PopupCursor(this.Loc.GetString("rmc-boon-not-enough-marines"), (EntityUid) manage, PopupType.MediumCaution);
        }
        else
        {
          if (component.NoLivingKing)
          {
            Robust.Shared.GameObjects.EntityQueryEnumerator<HiveBoonKingComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<HiveBoonKingComponent>();
            EntityUid uid2;
            while (entityQueryEnumerator2.MoveNext(out uid2, out HiveBoonKingComponent _))
            {
              if (!this._mobState.IsDead(uid2) && this._hive.FromSameHive((Entity<HiveMemberComponent>) manage.Owner, (Entity<HiveMemberComponent>) uid2))
              {
                this._popup.PopupCursor(this.Loc.GetString("rmc-boon-only-one-king"), (EntityUid) manage, PopupType.MediumCaution);
                return;
              }
            }
          }
          if (component.RequiresCore && !this._hive.GetHiveCore(valueOrDefault1).HasValue)
          {
            this._popup.PopupCursor(this.Loc.GetString("rmc-boon-requires-core"), (EntityUid) manage, PopupType.MediumCaution);
          }
          else
          {
            TimeSpan curTime = this._timing.CurTime;
            TimeSpan timeSpan1;
            if (entity.Comp.UsedAt.TryGetValue((EntProtoId<HiveBoonDefinitionComponent>) prototype.ID, out timeSpan1))
            {
              TimeSpan timeSpan2 = timeSpan1 + component.Cooldown - curTime;
              if (timeSpan2 > TimeSpan.Zero)
              {
                this._popup.PopupCursor(this.Loc.GetString("rmc-boon-on-cooldown", (nameof (boon), (object) prototype.Name), ("minutes", (object) (int) timeSpan2.TotalMinutes)), (EntityUid) manage, PopupType.MediumCaution);
                return;
              }
            }
            EntProtoId<HiveBoonDefinitionComponent>? duplicateId = component.DuplicateId;
            if (duplicateId.HasValue)
            {
              EntProtoId<HiveBoonDefinitionComponent> valueOrDefault2 = duplicateId.GetValueOrDefault();
              EntityUid uid3;
              if (entity.Comp.Active.TryGetValue(valueOrDefault2, out uid3) && !this.TerminatingOrDeleted(uid3))
              {
                this._popup.PopupCursor(this.Loc.GetString("rmc-boon-duplicate-active", (nameof (boon), (object) this.Name(uid3))), (EntityUid) manage, PopupType.MediumCaution);
                return;
              }
            }
            if (!component.Reusable && entity.Comp.UsedAt.ContainsKey((EntProtoId<HiveBoonDefinitionComponent>) prototype.ID))
            {
              this._popup.PopupCursor(this.Loc.GetString("rmc-boon-not-reusable", (nameof (boon), (object) prototype.Name)), (EntityUid) manage, PopupType.MediumCaution);
            }
            else
            {
              if (component.Event == null)
                return;
              HiveBoonEvent copy = (HiveBoonEvent) this._serialization.CreateCopy((object) component.Event, notNullableOverride: true);
              if (copy == null)
                return;
              entity.Comp.UsedAt[(EntProtoId<HiveBoonDefinitionComponent>) prototype.ID] = curTime;
              entity.Comp.RoyalResin = Math.Max(0, entity.Comp.RoyalResin - component.Cost);
              EntityUid entityUid = this.Spawn(prototype.ID, MapCoordinates.Nullspace, rotation: new Angle());
              this._hive.SetSameHive((Entity<HiveMemberComponent>) manage.Owner, (Entity<HiveMemberComponent>) entityUid);
              this.EnsureComp<TimedDespawnComponent>(entityUid).Lifetime = (float) component.Duration.TotalSeconds;
              entity.Comp.Active[(EntProtoId<HiveBoonDefinitionComponent>) prototype.ID] = entityUid;
              this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
              copy.Boon = entityUid;
              copy.Hive = valueOrDefault1;
              copy.Core = this._hive.GetHiveCore(valueOrDefault1);
              this.RaiseLocalEvent((object) copy);
            }
          }
        }
      }
    }
  }

  private bool TryGetUnlockAt(
    Entity<HiveBoonsComponent> boons,
    EntProtoId<HiveBoonDefinitionComponent> boonId,
    out TimeSpan unlockAt)
  {
    EntityPrototype prototype;
    HiveBoonDefinitionComponent component;
    if (!this._prototype.TryIndex((EntProtoId) boonId, out prototype) || !prototype.TryGetComponent<HiveBoonDefinitionComponent>(out component, this._compFactory))
    {
      unlockAt = TimeSpan.Zero;
      return false;
    }
    if (boons.Comp.UnlockAt.TryGetValue(boonId, out unlockAt))
      return true;
    unlockAt = component.UnlockAt;
    if (component.UnlockAtRandomAdd != TimeSpan.Zero)
      unlockAt += this._random.Next(TimeSpan.Zero, component.UnlockAtRandomAdd);
    boons.Comp.UnlockAt[boonId] = unlockAt;
    this.Dirty<HiveBoonsComponent>(boons);
    return true;
  }

  public Entity<HiveBoonsComponent> EnsureBoons(Entity<HiveComponent> hive)
  {
    HiveBoonsComponent hiveBoonsComponent = this.EnsureComp<HiveBoonsComponent>((EntityUid) hive);
    return (Entity<HiveBoonsComponent>) ((EntityUid) hive, hiveBoonsComponent);
  }

  private void ReloadPrototypes()
  {
    ImmutableArray<(EntityPrototype, HiveBoonDefinitionComponent)>.Builder builder = ImmutableArray.CreateBuilder<(EntityPrototype, HiveBoonDefinitionComponent)>();
    foreach (EntityPrototype enumeratePrototype in this._prototype.EnumeratePrototypes<EntityPrototype>())
    {
      HiveBoonDefinitionComponent component;
      if (enumeratePrototype.TryGetComponent<HiveBoonDefinitionComponent>(out component, this._compFactory))
        builder.Add((enumeratePrototype, component));
    }
    builder.Sort((Comparison<(EntityPrototype, HiveBoonDefinitionComponent)>) ((a, b) => string.Compare(a.Prototype.Name, b.Prototype.Name, StringComparison.InvariantCultureIgnoreCase)));
    this.Boons = builder.ToImmutable();
  }

  private void GainResin()
  {
    try
    {
      TimeSpan curTime = this._timing.CurTime;
      Robust.Shared.GameObjects.EntityQueryEnumerator<HivePylonComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HivePylonComponent>();
      EntityUid uid;
      HivePylonComponent comp1;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      {
        if (!(comp1.NextRoyalResin >= curTime))
        {
          comp1.NextRoyalResin = curTime + this._royalResinEvery;
          this.Dirty(uid, (IComponent) comp1);
          Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) uid);
          if (hive.HasValue)
          {
            Entity<HiveBoonsComponent> entity = this.EnsureBoons(hive.GetValueOrDefault());
            entity.Comp.RoyalResin = Math.Clamp(entity.Comp.RoyalResin + 1, 0, entity.Comp.RoyalResinMax);
          }
        }
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error gaining royal resin:\n{ex}");
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    this.AnnounceKingUnlock();
    this.GainResin();
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveKingCocoonComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<HiveKingCocoonComponent>();
    EntityUid uid1;
    HiveKingCocoonComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      int num1 = 0;
      Robust.Shared.GameObjects.EntityQueryEnumerator<HivePylonComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<HivePylonComponent>();
      EntityUid uid2;
      HivePylonComponent comp1_2;
      while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
      {
        if (this._hive.FromSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) uid2))
          ++num1;
      }
      if (num1 >= comp1_1.RequiredPylons)
      {
        if (comp1_1.LastPylons < comp1_1.RequiredPylons)
        {
          this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-resumed-marine", ("area", (object) this._area.GetAreaName(uid1))));
          this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) uid1, this.Loc.GetString("rmc-boon-king-announcement-resumed-xeno"));
        }
        comp1_1.LastPylons = num1;
        Robust.Shared.GameObjects.EntityQueryEnumerator<HivePylonComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<HivePylonComponent>();
        EntityUid uid3;
        while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_2))
        {
          if (this._hive.FromSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) uid3))
            this._area.TrySetCanOrbitalBombardRoofing((Entity<RoofingEntityComponent>) uid3, false);
        }
        comp1_1.TimeLeft -= TimeSpan.FromSeconds((double) frameTime);
        if (comp1_1.TimeLeft > this._kingVoteStartTime && !this.HasEnoughAliveMarines())
        {
          comp1_1.TimeLeft = this._kingVoteStartTime;
          comp1_1.FirstWarning = true;
          this.Dirty(uid1, (IComponent) comp1_1);
        }
        if (comp1_1.TimeLeft <= this._kingFirstWarningTime && !comp1_1.FirstWarning)
        {
          comp1_1.FirstWarning = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-minutes-marine", ("area", (object) this._area.GetAreaName(uid1)), ("minutes", (object) ((int) comp1_1.TimeLeft.TotalMinutes + 1))));
          this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) uid1, this.Loc.GetString("rmc-boon-king-announcement-minutes-xeno", ("minutes", (object) ((int) comp1_1.TimeLeft.TotalMinutes + 1))));
        }
        if (!(comp1_1.TimeLeft > this._kingVoteStartTime))
        {
          if (!comp1_1.VoteStarted)
          {
            comp1_1.VoteStarted = true;
            this.Dirty(uid1, (IComponent) comp1_1);
            this.StartKingVote((Entity<HiveKingCocoonComponent>) (uid1, comp1_1));
          }
          if (!(comp1_1.TimeLeft > this._kingVoteStartHatchingTime))
          {
            if (!comp1_1.FinalWarning)
            {
              comp1_1.FinalWarning = true;
              this.Dirty(uid1, (IComponent) comp1_1);
              this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-seconds-marine", ("area", (object) this._area.GetAreaName(uid1)), ("seconds", (object) ((int) comp1_1.TimeLeft.TotalSeconds + 1))));
              this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) uid1, this.Loc.GetString("rmc-boon-king-announcement-seconds-xeno", ("seconds", (object) ((int) comp1_1.TimeLeft.TotalSeconds + 1))));
            }
            if (!(comp1_1.TimeLeft > TimeSpan.Zero))
            {
              Entity<HiveKingVoteComponent> entity = this.EnsureVote(uid1);
              List<(NetUserId, int)> source = new List<(NetUserId, int)>();
              foreach ((NetUserId key, int num2) in entity.Comp.Votes)
                source.Add((key, num2));
              List<(NetUserId, int)> list = source.OrderByDescending<(NetUserId, int), int>((Func<(NetUserId, int), int>) (a => a.Votes)).ToList<(NetUserId, int)>();
              EntityUid entityUid = this.SpawnAtPosition((string) comp1_1.Spawn, uid1.ToCoordinates());
              this._hive.SetSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) entityUid);
              this.EnsureComp<HiveConstructionSuppressAnnouncementsComponent>(uid1);
              this.QueueDel(new EntityUid?(uid1));
              foreach ((NetUserId user, int _) in list)
              {
                this._mind.ControlMob(user, entityUid);
                ActorComponent comp;
                if (this.TryComp<ActorComponent>(entityUid, out comp) && comp.PlayerSession.UserId == user)
                {
                  this.QueueDel(new EntityUid?((EntityUid) entity));
                  this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-hatch-marine", ("area", (object) this._area.GetAreaName(uid1))));
                  this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) uid1, this.Loc.GetString("rmc-boon-king-announcement-hatch-xeno"));
                  return;
                }
              }
            }
          }
        }
      }
      else
      {
        if (comp1_1.LastPylons >= comp1_1.RequiredPylons)
        {
          this._marineAnnounce.AnnounceToMarines(this.Loc.GetString("rmc-boon-king-announcement-paused-marine", ("area", (object) this._area.GetAreaName(uid1))));
          this._xenoAnnounce.AnnounceSameHiveDefaultSound((Entity<HiveMemberComponent>) uid1, this.Loc.GetString("rmc-boon-king-announcement-paused-xeno"));
        }
        comp1_1.LastPylons = num1;
        break;
      }
    }
  }

  private void AnnounceKingUnlock()
  {
    try
    {
      if (this._net.IsClient || !this._rmcGameTicker.ServerOnlyIsInRound())
        return;
      Robust.Shared.GameObjects.EntityQueryEnumerator<HiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveComponent>();
      EntityUid uid;
      HiveComponent comp1;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      {
        Entity<HiveBoonsComponent> entity = this.EnsureBoons((Entity<HiveComponent>) (uid, comp1));
        TimeSpan unlockAt;
        if (!entity.Comp.KingAnnounced && this.TryGetUnlockAt(entity, HiveBoonSystem.KingBoonId, out unlockAt) && !(this._gameTicker.RoundDuration() < unlockAt))
        {
          entity.Comp.KingAnnounced = true;
          this.Dirty<HiveBoonsComponent>(entity);
          SoundSpecifier xenoSound = new BioscanComponent().XenoSound;
          this._xenoAnnounce.AnnounceToHive(new EntityUid(), uid, "The hive is now ready to begin hatching His Grace, the King, if we gain control of both tall hivemind towers.", xenoSound);
        }
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error announcing king unlock:\n{ex}");
    }
  }
}
