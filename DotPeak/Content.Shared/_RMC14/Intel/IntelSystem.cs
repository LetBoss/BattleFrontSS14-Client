// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.IntelSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.ARES;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Intel.Tech;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Random.Helpers;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Intel;

public sealed class IntelSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private ARESSystem _ares;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private NameModifierSystem _nameModifier;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private static readonly ImmutableArray<char> UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToImmutableArray<char>();
  private static readonly EntProtoId<IntelTechTreeComponent> TechTreeProto = (EntProtoId<IntelTechTreeComponent>) "RMCIntelTechTree";
  private static readonly EntProtoId PaperScrapProto = (EntProtoId) "RMCIntelPaperScrap";
  private static readonly EntProtoId ProgressReportProto = (EntProtoId) "RMCIntelProgressReport";
  private static readonly EntProtoId FolderProto = (EntProtoId) "RMCIntelFolder";
  private static readonly EntProtoId TechnicalManualProto = (EntProtoId) "RMCIntelTechnicalManual";
  private static readonly EntProtoId ExperimentalDevicesProto = (EntProtoId) "RMCIntelRetrieveHealthAnalyzer";
  private readonly Dictionary<IntelSpawnerType, float> _paperScrapChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 20f,
    [IntelSpawnerType.Medium] = 5f,
    [IntelSpawnerType.Far] = 2f,
    [IntelSpawnerType.Science] = 10f
  };
  private readonly Dictionary<IntelSpawnerType, float> _progressReportChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 10f,
    [IntelSpawnerType.Medium] = 55f,
    [IntelSpawnerType.Far] = 3f,
    [IntelSpawnerType.Science] = 10f
  };
  private readonly Dictionary<IntelSpawnerType, float> _folderChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 20f,
    [IntelSpawnerType.Medium] = 5f,
    [IntelSpawnerType.Far] = 2f,
    [IntelSpawnerType.Science] = 10f
  };
  private readonly Dictionary<IntelSpawnerType, float> _technicalManualChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 20f,
    [IntelSpawnerType.Medium] = 40f,
    [IntelSpawnerType.Far] = 20f,
    [IntelSpawnerType.Science] = 20f
  };
  private readonly Dictionary<IntelSpawnerType, float> _diskChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 20f,
    [IntelSpawnerType.Medium] = 40f,
    [IntelSpawnerType.Far] = 20f,
    [IntelSpawnerType.Science] = 20f
  };
  private readonly Dictionary<IntelSpawnerType, float> _experimentalDeviceChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 10f,
    [IntelSpawnerType.Medium] = 20f,
    [IntelSpawnerType.Far] = 40f,
    [IntelSpawnerType.Science] = 30f
  };
  private readonly Dictionary<IntelSpawnerType, float> _researchPaperChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 25f,
    [IntelSpawnerType.Medium] = 20f,
    [IntelSpawnerType.Far] = 5f,
    [IntelSpawnerType.Science] = 50f
  };
  private readonly Dictionary<IntelSpawnerType, float> _vialBoxChances = new Dictionary<IntelSpawnerType, float>()
  {
    [IntelSpawnerType.Close] = 15f,
    [IntelSpawnerType.Medium] = 30f,
    [IntelSpawnerType.Far] = 5f,
    [IntelSpawnerType.Science] = 50f
  };
  private int _paperScraps;
  private int _progressReports;
  private int _folders;
  private int _technicalManuals;
  private int _disks;
  private int _experimentalDevices;
  private int _researchPapers;
  private int _vialBoxes;
  private TimeSpan _maxProcessTime;
  private TimeSpan _announceEvery;
  private int _powerObjectiveWattsRequired;
  private int _intelHumanoidsCorpsesMax;
  private readonly Dictionary<IntelSpawnerType, List<Entity<IntelSpawnerComponent>>> _spawners = new Dictionary<IntelSpawnerType, List<Entity<IntelSpawnerComponent>>>();
  private readonly Queue<Entity<IntelRetrieveItemObjectiveComponent>> _activePositionIntels = new Queue<Entity<IntelRetrieveItemObjectiveComponent>>();
  private readonly Queue<Entity<IntelRescueSurvivorObjectiveComponent>> _activeSurvivorIntels = new Queue<Entity<IntelRescueSurvivorObjectiveComponent>>();
  private readonly Queue<Entity<IntelRecoverCorpseObjectiveComponent>> _activeCorpseIntels = new Queue<Entity<IntelRecoverCorpseObjectiveComponent>>();
  private readonly HashSet<Entity<IntelContainerComponent>> _nearby = new HashSet<Entity<IntelContainerComponent>>();
  private Robust.Shared.GameObjects.EntityQuery<IntelReadObjectiveComponent> _readObjectiveQuery;

  public override void Initialize()
  {
    this._readObjectiveQuery = this.GetEntityQuery<IntelReadObjectiveComponent>();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<DropshipLandedOnPlanetEvent>(new EntityEventRefHandler<DropshipLandedOnPlanetEvent>(this.OnDropshipLandedOnPlanet));
    this.SubscribeLocalEvent<IntelNumberComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<IntelNumberComponent, RefreshNameModifiersEvent>(this.OnNumberRefreshNameModifiers));
    this.SubscribeLocalEvent<IntelUnlocksComponent, ComponentRemove>(new EntityEventRefHandler<IntelUnlocksComponent, ComponentRemove>(this.OnUnlocksRemove<ComponentRemove>));
    this.SubscribeLocalEvent<IntelUnlocksComponent, EntityTerminatingEvent>(new EntityEventRefHandler<IntelUnlocksComponent, EntityTerminatingEvent>(this.OnUnlocksRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<IntelRequiresComponent, ComponentRemove>(new EntityEventRefHandler<IntelRequiresComponent, ComponentRemove>(this.OnRequiresRemove<ComponentRemove>));
    this.SubscribeLocalEvent<IntelRequiresComponent, EntityTerminatingEvent>(new EntityEventRefHandler<IntelRequiresComponent, EntityTerminatingEvent>(this.OnRequiresRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<IntelReadObjectiveComponent, UseInHandEvent>(new EntityEventRefHandler<IntelReadObjectiveComponent, UseInHandEvent>(this.OnReadUseInHand));
    this.SubscribeLocalEvent<IntelReadObjectiveComponent, IntelReadDoAfterEvent>(new EntityEventRefHandler<IntelReadObjectiveComponent, IntelReadDoAfterEvent>(this.OnReadDoAfter));
    this.SubscribeLocalEvent<IntelRetrieveItemObjectiveComponent, MapInitEvent>(new EntityEventRefHandler<IntelRetrieveItemObjectiveComponent, MapInitEvent>(this.OnRetrieveMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.SubscribeLocalEvent<IntelRetrieveItemObjectiveComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<IntelRetrieveItemObjectiveComponent, ContainerGettingInsertedAttemptEvent>(this.OnHandPickUp));
    this.SubscribeLocalEvent<IntelRetrieveItemObjectiveComponent, PullAttemptEvent>(new EntityEventRefHandler<IntelRetrieveItemObjectiveComponent, PullAttemptEvent>(this.OnIntelPullAttempt));
    this.SubscribeLocalEvent<ActiveIntelCorpseComponent, PullAttemptEvent>(new EntityEventRefHandler<ActiveIntelCorpseComponent, PullAttemptEvent>(this.OnIntelCorpsePullAttempt));
    this.SubscribeLocalEvent<ViewIntelObjectivesComponent, MapInitEvent>(new EntityEventRefHandler<ViewIntelObjectivesComponent, MapInitEvent>(this.OnViewIntelObjectivesMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.SubscribeLocalEvent<ViewIntelObjectivesComponent, ViewIntelObjectivesActionEvent>(new EntityEventRefHandler<ViewIntelObjectivesComponent, ViewIntelObjectivesActionEvent>(this.OnViewIntelObjectivesAction));
    this.SubscribeLocalEvent<IntelHasUnlockedComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<IntelHasUnlockedComponent, RefreshNameModifiersEvent>(this.OnHasUnlockedRefreshName));
    this.SubscribeLocalEvent<IntelSerialComponent, MapInitEvent>(new EntityEventRefHandler<IntelSerialComponent, MapInitEvent>(this.OnIntelSerialMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.SubscribeLocalEvent<IntelSerialComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<IntelSerialComponent, RefreshNameModifiersEvent>(this.OnIntelSerialRefreshNameModifiers));
    this.SubscribeLocalEvent<IntelSerialComponent, ExaminedEvent>(new EntityEventRefHandler<IntelSerialComponent, ExaminedEvent>(this.OnIntelSerialExamined));
    this.SubscribeLocalEvent<IntelRecoverCorpseObjectiveOnDeathComponent, MobStateChangedEvent>(new EntityEventRefHandler<IntelRecoverCorpseObjectiveOnDeathComponent, MobStateChangedEvent>(this.OnRescueCorpseObjectiveOnDeathChanged));
    this.SubscribeLocalEvent<IntelRecoverCorpseObjectiveComponent, MapInitEvent>(new EntityEventRefHandler<IntelRecoverCorpseObjectiveComponent, MapInitEvent>(this.OnRescueCorpseObjectiveMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.SubscribeLocalEvent<IntelKnowledgeComponent, ComponentRemove>(new EntityEventRefHandler<IntelKnowledgeComponent, ComponentRemove>(this.OnKnowledgeRemove<ComponentRemove>));
    this.SubscribeLocalEvent<IntelKnowledgeComponent, EntityTerminatingEvent>(new EntityEventRefHandler<IntelKnowledgeComponent, EntityTerminatingEvent>(this.OnKnowledgeRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<IntelReadComponent, ComponentRemove>(new EntityEventRefHandler<IntelReadComponent, ComponentRemove>(this.OnReadRemove<ComponentRemove>));
    this.SubscribeLocalEvent<IntelReadComponent, EntityTerminatingEvent>(new EntityEventRefHandler<IntelReadComponent, EntityTerminatingEvent>(this.OnReadRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<IntelConsoleComponent, InteractHandEvent>(new EntityEventRefHandler<IntelConsoleComponent, InteractHandEvent>(this.OnConsoleInteractHand));
    this.SubscribeLocalEvent<IntelConsoleComponent, IntelSubmitDoAfterEvent>(new EntityEventRefHandler<IntelConsoleComponent, IntelSubmitDoAfterEvent>(this.OnConsoleSubmitDoAfter));
    this.SubscribeLocalEvent<IntelCluesComponent, MapInitEvent>(new EntityEventRefHandler<IntelCluesComponent, MapInitEvent>(this.OnIntelCluesMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelPaperScraps, (Action<int>) (v => this._paperScraps = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelProgressReports, (Action<int>) (v => this._progressReports = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelFolders, (Action<int>) (v => this._folders = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelTechnicalManuals, (Action<int>) (v => this._technicalManuals = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelDisks, (Action<int>) (v => this._disks = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelExperimentalDevices, (Action<int>) (v => this._experimentalDevices = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelResearchPapers, (Action<int>) (v => this._researchPapers = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelVialBoxes, (Action<int>) (v => this._vialBoxes = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCIntelMaxProcessTimeMilliseconds, (Action<float>) (v => this._maxProcessTime = TimeSpan.FromMilliseconds((double) v)), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCIntelAnnounceEveryMinutes, (Action<float>) (v => this._announceEvery = TimeSpan.FromMinutes((double) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelPowerObjectiveWattsRequired, (Action<int>) (v => this._powerObjectiveWattsRequired = v), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCIntelHumanoidCorpsesMax, (Action<int>) (v => this._intelHumanoidsCorpsesMax = v), true);
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._spawners.Clear();
    this._activePositionIntels.Clear();
    this._activeSurvivorIntels.Clear();
  }

  private void OnDropshipLandedOnPlanet(ref DropshipLandedOnPlanetEvent ev)
  {
    Entity<IntelTechTreeComponent> ent = this.EnsureTechTree();
    ent.Comp.DoAnnouncements = true;
    this.Dirty<IntelTechTreeComponent>(ent);
  }

  private void OnNumberRefreshNameModifiers(
    Entity<IntelNumberComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    args.AddModifier((LocId) "rmc-intel-suffix", 0, ("number", (object) ent.Comp.Number));
  }

  private void OnUnlocksRemove<T>(Entity<IntelUnlocksComponent> ent, ref T args)
  {
    foreach (EntityUid unlock in ent.Comp.Unlocks)
    {
      IntelRequiresComponent comp;
      if (this.TryComp<IntelRequiresComponent>(unlock, out comp))
        comp.Requires.Remove((EntityUid) ent);
    }
  }

  private void OnRequiresRemove<T>(Entity<IntelRequiresComponent> ent, ref T args)
  {
    foreach (EntityUid require in ent.Comp.Requires)
    {
      IntelUnlocksComponent comp;
      if (this.TryComp<IntelUnlocksComponent>(require, out comp))
        comp.Unlocks.Remove((EntityUid) ent);
    }
  }

  private void OnReadUseInHand(Entity<IntelReadObjectiveComponent> ent, ref UseInHandEvent args)
  {
    EntityUid user = args.User;
    if (this.HasComp<IntelRescueSurvivorObjectiveComponent>(user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-intel-survivor-read", ("thing", (object) this.Name((EntityUid) ent))), (EntityUid) ent, new EntityUid?(user));
    }
    else
    {
      TimeSpan delay = ent.Comp.Delay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user, ent.Comp.Skill);
      IntelReadDoAfterEvent @event = new IntelReadDoAfterEvent();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent))
      {
        BreakOnDropItem = true,
        NeedHand = true
      }))
        return;
      this._popup.PopupClient("You start reading the " + this.Name((EntityUid) ent), (EntityUid) ent, new EntityUid?(user));
    }
  }

  private void OnHandPickUp(
    EntityUid ent,
    IntelRetrieveItemObjectiveComponent component,
    ContainerGettingInsertedAttemptEvent args)
  {
    EntityUid owner = args.Container.Owner;
    if (!this.HasComp<IntelRescueSurvivorObjectiveComponent>(owner))
      return;
    args.Cancel();
    this._popup.PopupClient(this.Loc.GetString("rmc-intel-survivor-pickup", ("thing", (object) this.Name(ent))), ent, new EntityUid?(owner));
  }

  private void OnIntelPullAttempt(
    Entity<IntelRetrieveItemObjectiveComponent> ent,
    ref PullAttemptEvent args)
  {
    EntityUid pullerUid = args.PullerUid;
    if (!this.HasComp<IntelRescueSurvivorObjectiveComponent>(pullerUid))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("rmc-intel-survivor-pickup", ("thing", (object) this.Name((EntityUid) ent))), (EntityUid) ent, new EntityUid?(pullerUid));
  }

  private void OnIntelCorpsePullAttempt(
    Entity<ActiveIntelCorpseComponent> ent,
    ref PullAttemptEvent args)
  {
    EntityUid pullerUid = args.PullerUid;
    if (!this.HasComp<IntelRescueSurvivorObjectiveComponent>(pullerUid))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.HasComp<XenoComponent>((EntityUid) ent) ? this.Loc.GetString("rmc-intel-survivor-xeno-pull", ("thing", (object) this.Name((EntityUid) ent))) : this.Loc.GetString("rmc-intel-survivor-corpse-pull", ("thing", (object) this.Name((EntityUid) ent))), (EntityUid) ent, new EntityUid?(pullerUid));
  }

  private void OnReadDoAfter(
    Entity<IntelReadObjectiveComponent> ent,
    ref IntelReadDoAfterEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    args.Handled = true;
    if (args.Cancelled)
      this._popup.PopupClient("You get distracted and lose your train of thought, you'll have to start over reading this.", (EntityUid) ent, new EntityUid?(user));
    else if (ent.Comp.State == IntelObjectiveState.Inactive)
    {
      this._popup.PopupClient("You don't notice anything useful. You probably need to find its instructions on a paper scrap", (EntityUid) ent, new EntityUid?(user));
    }
    else
    {
      this._popup.PopupClient("You finish reading the " + this.Name((EntityUid) ent), (EntityUid) ent, new EntityUid?(user));
      if (ent.Comp.State == IntelObjectiveState.Complete)
        return;
      ent.Comp.State = IntelObjectiveState.Complete;
      this.Dirty<IntelReadObjectiveComponent>(ent);
      if (this._net.IsClient)
        return;
      Entity<IntelTechTreeComponent> tree = this.EnsureTechTree();
      ++tree.Comp.Tree.Documents.Current;
      this.AddPoints(tree, ent.Comp.Value);
      IntelKnowledgeComponent knowledgeComponent = this.EnsureComp<IntelKnowledgeComponent>(user);
      knowledgeComponent.Read.Add((EntityUid) ent);
      this.Dirty(user, (IComponent) knowledgeComponent);
      IntelReadComponent intelReadComponent = this.EnsureComp<IntelReadComponent>((EntityUid) ent);
      intelReadComponent.Readers.Add(user);
      this.Dirty((EntityUid) ent, (IComponent) intelReadComponent);
      IntelRetrieveItemObjectiveComponent comp;
      if (this.TryComp<IntelRetrieveItemObjectiveComponent>((EntityUid) ent, out comp) && comp.State == IntelObjectiveState.Inactive)
      {
        comp.State = IntelObjectiveState.Active;
        this.Dirty((EntityUid) ent, (IComponent) comp);
      }
      this.UpdateTree(tree);
    }
  }

  private void OnRetrieveMapInit(
    Entity<IntelRetrieveItemObjectiveComponent> ent,
    ref MapInitEvent args)
  {
    if (ent.Comp.State != IntelObjectiveState.Active)
      return;
    this.EnsureComp<ActiveIntelPositionComponent>((EntityUid) ent);
  }

  private void OnViewIntelObjectivesMapInit(
    Entity<ViewIntelObjectivesComponent> ent,
    ref MapInitEvent args)
  {
    this._actions.AddAction((EntityUid) ent, ref ent.Comp.Action, ent.Comp.ActionId);
  }

  private void OnViewIntelObjectivesAction(
    Entity<ViewIntelObjectivesComponent> ent,
    ref ViewIntelObjectivesActionEvent args)
  {
    if (this._net.IsServer)
    {
      IntelTechTree tree = this.EnsureTechTree().Comp.Tree;
      ent.Comp.Tree = tree;
      this.Dirty<ViewIntelObjectivesComponent>(ent);
    }
    this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ViewIntelObjectivesUI.Key, new EntityUid?((EntityUid) ent));
  }

  private void OnHasUnlockedRefreshName(
    Entity<IntelHasUnlockedComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    args.AddModifier((LocId) "rmc-intel-unlocked", 0, ("unlocked", (object) string.Join<int>(", ", (IEnumerable<int>) ent.Comp.Unlocked)));
  }

  private void OnIntelSerialMapInit(Entity<IntelSerialComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.Serial = $"{Number()}{Char()}{Number()}{Number()}{Number()}{Number()}{Char()}";
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);

    int Number() => this._random.Next(0, 10);

    char Char()
    {
      return RandomExtensions.Pick<char>(this._random, (IReadOnlyList<char>) IntelSystem.UppercaseLetters);
    }
  }

  private void OnIntelSerialRefreshNameModifiers(
    Entity<IntelSerialComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    args.AddModifier((LocId) "rmc-intel-serial-name", 0, ("serial", (object) ent.Comp.Serial));
  }

  private void OnIntelSerialExamined(Entity<IntelSerialComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("IntelSerialComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-intel-serial-examine", ("serial", (object) ent.Comp.Serial)));
  }

  private void OnRescueCorpseObjectiveOnDeathChanged(
    Entity<IntelRecoverCorpseObjectiveOnDeathComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.OldMobState == MobState.Dead || args.NewMobState != MobState.Dead || this.HasComp<IntelRecoverCorpseObjectiveComponent>(args.Target))
      return;
    IntelRecoverCorpseObjectiveComponent objectiveComponent = this.EnsureComp<IntelRecoverCorpseObjectiveComponent>(args.Target);
    objectiveComponent.Value = ent.Comp.Value;
    this.Dirty(args.Target, (IComponent) objectiveComponent);
  }

  private void OnRescueCorpseObjectiveMapInit(
    Entity<IntelRecoverCorpseObjectiveComponent> ent,
    ref MapInitEvent args)
  {
    this.EnsureComp<ActiveIntelCorpseComponent>((EntityUid) ent);
  }

  private void OnKnowledgeRemove<T>(Entity<IntelKnowledgeComponent> ent, ref T args)
  {
    foreach (EntityUid uid in ent.Comp.Read)
    {
      IntelReadComponent comp;
      if (!this.TerminatingOrDeleted(uid) && this.TryComp<IntelReadComponent>(uid, out comp))
      {
        comp.Readers.Remove((EntityUid) ent);
        this.Dirty(uid, (IComponent) comp);
      }
    }
  }

  private void OnReadRemove<T>(Entity<IntelReadComponent> ent, ref T args)
  {
    foreach (EntityUid reader in ent.Comp.Readers)
    {
      IntelKnowledgeComponent comp;
      if (!this.TerminatingOrDeleted(reader) && this.TryComp<IntelKnowledgeComponent>(reader, out comp))
      {
        comp.Read.Remove((EntityUid) ent);
        this.Dirty(reader, (IComponent) comp);
      }
    }
  }

  private void OnConsoleInteractHand(Entity<IntelConsoleComponent> ent, ref InteractHandEvent args)
  {
    string message = "You start typing in intel into the computer...";
    IntelKnowledgeComponent comp;
    EntityUid? element;
    if (!this.TryComp<IntelKnowledgeComponent>(args.User, out comp) || !comp.Read.TryFirstOrNull<EntityUid>(out element))
    {
      this._popup.PopupClient(message + " and you have nothing new to add...", (EntityUid) ent, new EntityUid?(args.User), PopupType.Medium);
    }
    else
    {
      this._popup.PopupClient(message, (EntityUid) ent, new EntityUid?(args.User), PopupType.Medium);
      TimeSpan delay = ent.Comp.Delay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) args.User, ent.Comp.Skill);
      IntelSubmitDoAfterEvent @event = new IntelSubmitDoAfterEvent()
      {
        Intel = this.GetNetEntity(element.Value)
      };
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
      {
        BreakOnMove = true
      });
    }
  }

  private void OnConsoleSubmitDoAfter(
    Entity<IntelConsoleComponent> ent,
    ref IntelSubmitDoAfterEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (this._net.IsClient)
      return;
    if (args.Cancelled)
    {
      this._popup.PopupEntity("You get distracted and lose your train of thought, you'll have to start the typing over...", (EntityUid) ent, args.User, PopupType.MediumCaution);
      args.Repeat = false;
    }
    else
    {
      IntelKnowledgeComponent comp1;
      if (!this.TryComp<IntelKnowledgeComponent>(args.User, out comp1))
      {
        StopPopup(ref args);
      }
      else
      {
        EntityUid? uid;
        IntelUnlocksComponent comp2;
        EntityUid? element;
        if (!this.TryGetEntity(args.Intel, out uid) || !this.TryComp<IntelUnlocksComponent>(uid, out comp2) || !comp2.Unlocks.TryFirstOrNull<EntityUid>(out element))
        {
          if (!comp1.Read.TryFirstOrNull<EntityUid>(out uid))
          {
            StopPopup(ref args);
            return;
          }
          args.Intel = this.GetNetEntity(uid.Value);
          if (!this.TryComp<IntelUnlocksComponent>(uid, out comp2) || !comp2.Unlocks.TryFirstOrNull<EntityUid>(out element))
          {
            comp1.Read.Remove(uid.Value);
            args.Repeat = true;
            return;
          }
        }
        IntelCluesComponent comp3;
        if (this.TryComp<IntelCluesComponent>(element, out comp3))
        {
          string message = this.Loc.GetString((string) comp3.Clue, ("intel", (object) element), ("area", (object) comp3.InitialArea));
          this._rmcChat.ChatMessageToOne(message, args.User);
          this._popup.PopupEntity(message, (EntityUid) ent, args.User, PopupType.Medium);
          IntelRetrieveItemObjectiveComponent comp4;
          if (this.TryComp<IntelRetrieveItemObjectiveComponent>(element, out comp4) && comp4.State != IntelObjectiveState.Complete)
          {
            LocId? category = comp3.Category;
            if (category.HasValue)
            {
              LocId valueOrDefault = category.GetValueOrDefault();
              this.EnsureTechTree().Comp.Tree.Clues.GetOrNew<LocId, Dictionary<NetEntity, string>>(valueOrDefault)[this.GetNetEntity(element.Value)] = message;
            }
          }
        }
        comp2.Unlocks.Remove(element.Value);
        this.ActivateIntel(uid.Value, element.Value);
        ++args.Amount;
        this._audio.PlayPvs(ent.Comp.TypingSound, (EntityUid) ent);
        if (comp2.Unlocks.Count == 0)
          comp1.Read.Remove(uid.Value);
        if (comp1.Read.Count > 0)
          args.Repeat = true;
        else
          StopPopup(ref args);
      }
    }

    void StopPopup(ref IntelSubmitDoAfterEvent args)
    {
      if (args.Amount == 0)
        this._popup.PopupEntity("...and you have nothing new to add...", (EntityUid) ent, args.User, PopupType.Medium);
      else
        this._popup.PopupEntity($"...and done! You uploaded {args.Amount} entries!", (EntityUid) ent, args.User, PopupType.Medium);
    }
  }

  private void OnIntelCluesMapInit(Entity<IntelCluesComponent> ent, ref MapInitEvent args)
  {
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea((EntityUid) ent, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return;
    ent.Comp.InitialArea = this.Name((EntityUid) area.Value);
  }

  private List<EntityUid> SpawnIntel(
    EntProtoId proto,
    int count,
    Dictionary<IntelSpawnerType, float> chances)
  {
    List<EntityUid> entityUidList = new List<EntityUid>();
    for (int index = 0; index < count; ++index)
    {
      List<Entity<IntelSpawnerComponent>> list;
      if (this._spawners.TryGetValue(this._random.Pick<IntelSpawnerType>(chances), out list) && list.Count > 0)
      {
        EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) RandomExtensions.Pick<Entity<IntelSpawnerComponent>>(this._random, (IReadOnlyList<Entity<IntelSpawnerComponent>>) list));
        EntityUid entityUid = this.Spawn((string) proto, moverCoordinates);
        entityUidList.Add(entityUid);
        this.EnsureComp<ActiveIntelPositionComponent>(entityUid);
        IntelNumberComponent intelNumberComponent = this.EnsureComp<IntelNumberComponent>(entityUid);
        intelNumberComponent.Number = this._random.Next(100, 1000);
        this.Dirty(entityUid, (IComponent) intelNumberComponent);
        this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) entityUid);
        this._nearby.Clear();
        this._entityLookup.GetEntitiesInRange<IntelContainerComponent>(moverCoordinates, 0.5f, this._nearby, LookupFlags.Uncontained);
        foreach (Entity<IntelContainerComponent> entity in this._nearby)
        {
          if (this.HasComp<StorageComponent>((EntityUid) entity))
          {
            if (this._storage.Insert((EntityUid) entity, entityUid, out EntityUid? _))
              break;
          }
          if (this._entityStorage.Insert(entityUid, (EntityUid) entity))
            break;
        }
      }
    }
    return entityUidList;
  }

  public Entity<IntelTechTreeComponent> EnsureTechTree()
  {
    Entity<IntelTechTreeComponent>? tree;
    if (this.TryGetTechTree(out tree))
      return tree.Value;
    EntityUid uid = this.Spawn((string) IntelSystem.TechTreeProto);
    IntelTechTreeComponent techTreeComponent = this.EnsureComp<IntelTechTreeComponent>(uid);
    tree = new Entity<IntelTechTreeComponent>?((Entity<IntelTechTreeComponent>) (uid, techTreeComponent));
    foreach (List<TechOption> option in techTreeComponent.Tree.Options)
    {
      for (int index = 0; index < option.Count; ++index)
      {
        TechOption techOption = option[index];
        if (techOption.CurrentCost == 0)
          option[index] = techOption with
          {
            CurrentCost = techOption.Cost
          };
      }
    }
    return tree.Value;
  }

  public bool TryGetTechTree([NotNullWhen(true)] out Entity<IntelTechTreeComponent>? tree)
  {
    EntityUid uid;
    IntelTechTreeComponent comp1;
    if (this.EntityQueryEnumerator<IntelTechTreeComponent>().MoveNext(out uid, out comp1))
    {
      tree = new Entity<IntelTechTreeComponent>?((Entity<IntelTechTreeComponent>) (uid, comp1));
      return true;
    }
    tree = new Entity<IntelTechTreeComponent>?();
    return false;
  }

  public void RunSpawners()
  {
    try
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<IntelSpawnerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<IntelSpawnerComponent>();
      EntityUid uid;
      IntelSpawnerComponent comp1;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      {
        if (!this.EntityManager.IsQueuedForDeletion(uid))
          this._spawners.GetOrNew<IntelSpawnerType, List<Entity<IntelSpawnerComponent>>>(comp1.IntelType).Add((Entity<IntelSpawnerComponent>) (uid, comp1));
      }
      if (this._spawners.Count == 0)
        return;
      foreach (List<Entity<IntelSpawnerComponent>> entityList in this._spawners.Values)
      {
        foreach (Entity<IntelSpawnerComponent> entity in entityList)
          this.QueueDel(new EntityUid?((EntityUid) entity));
      }
      Entity<IntelTechTreeComponent> entity1 = this.EnsureTechTree();
      List<EntityUid> candidates = this.SpawnIntel(IntelSystem.PaperScrapProto, this._paperScraps, this._paperScrapChances);
      List<EntityUid> entityUidList = this.SpawnIntel(IntelSystem.ProgressReportProto, this._progressReports, this._progressReportChances);
      entityUidList.AddRange((IEnumerable<EntityUid>) this.SpawnIntel(IntelSystem.FolderProto, this._folders, this._folderChances));
      List<EntityUid> list = this.SpawnIntel(IntelSystem.TechnicalManualProto, this._technicalManuals, this._technicalManualChances);
      this.SpawnIntel(IntelSystem.ExperimentalDevicesProto, this._experimentalDevices, this._experimentalDeviceChances);
      entity1.Comp.Tree.Documents.Total = this._paperScraps + this._progressReports + this._folders + this._technicalManuals;
      entity1.Comp.Tree.UploadData.Total = this._disks;
      entity1.Comp.Tree.RetrieveItems.Total = entity1.Comp.Tree.Documents.Total + entity1.Comp.Tree.UploadData.Total - this._disks;
      if (entityUidList.Count > 0)
      {
        foreach (EntityUid unlocksId in candidates)
          this.ConnectObjectives(unlocksId, RandomExtensions.Pick<EntityUid>(this._random, (IReadOnlyList<EntityUid>) entityUidList));
      }
      if (list.Count > 0)
      {
        foreach (EntityUid entityUid in entityUidList)
        {
          this.AddRequires((Entity<IntelRequiresComponent>) entityUid, candidates);
          EntityUid requiresId = RandomExtensions.Pick<EntityUid>(this._random, (IReadOnlyList<EntityUid>) list);
          this.ConnectObjectives(entityUid, requiresId);
        }
      }
      if (entityUidList.Count <= 0)
        return;
      foreach (EntityUid requires in list)
        this.AddRequires((Entity<IntelRequiresComponent>) requires, entityUidList);
    }
    finally
    {
      this._spawners.Clear();
    }
  }

  public void RestoreColonyCommunications()
  {
    if (this._net.IsClient)
      return;
    Entity<IntelTechTreeComponent> tree = this.EnsureTechTree();
    if (tree.Comp.Tree.ColonyCommunications)
      return;
    tree.Comp.Tree.ColonyCommunications = true;
    this.AddPoints(tree, tree.Comp.ColonyCommunicationsPoints);
  }

  private void ConnectObjectives(EntityUid unlocksId, EntityUid requiresId)
  {
    IntelUnlocksComponent unlocksComponent = this.EnsureComp<IntelUnlocksComponent>(unlocksId);
    unlocksComponent.Unlocks.Add(requiresId);
    this.Dirty(unlocksId, (IComponent) unlocksComponent);
    IntelRequiresComponent requiresComponent = this.EnsureComp<IntelRequiresComponent>(requiresId);
    requiresComponent.Requires.Add(unlocksId);
    this.Dirty(requiresId, (IComponent) requiresComponent);
    this.DeactivateIntel(requiresId);
  }

  private void AddRequires(Entity<IntelRequiresComponent?> requires, List<EntityUid> candidates)
  {
    ref IntelRequiresComponent local = ref requires.Comp;
    if (local == null)
      local = this.EnsureComp<IntelRequiresComponent>((EntityUid) requires);
    if (requires.Comp.RequiresCount <= requires.Comp.Requires.Count)
      return;
    int num = requires.Comp.RequiresCount - requires.Comp.Requires.Count;
    for (int index = 0; index < num; ++index)
    {
      this._random.Shuffle<EntityUid>((IList<EntityUid>) candidates);
      foreach (EntityUid candidate in candidates)
      {
        if (!requires.Comp.Requires.Contains(candidate))
        {
          this.ConnectObjectives(candidate, (EntityUid) requires);
          if (requires.Comp.RequiresCount <= requires.Comp.Requires.Count)
            break;
        }
      }
      if (requires.Comp.RequiresCount <= requires.Comp.Requires.Count)
        break;
    }
  }

  private void DeactivateIntel(EntityUid ent)
  {
    IntelReadObjectiveComponent comp1;
    if (this.TryComp<IntelReadObjectiveComponent>(ent, out comp1))
    {
      comp1.State = IntelObjectiveState.Inactive;
      this.Dirty(ent, (IComponent) comp1);
    }
    IntelRetrieveItemObjectiveComponent comp2;
    if (!this.TryComp<IntelRetrieveItemObjectiveComponent>(ent, out comp2))
      return;
    comp2.State = IntelObjectiveState.Inactive;
    this.Dirty(ent, (IComponent) comp2);
  }

  private void ActivateIntel(EntityUid activatedBy, EntityUid toActivate)
  {
    IntelReadObjectiveComponent comp1;
    if (this.TryComp<IntelReadObjectiveComponent>(toActivate, out comp1) && comp1.State == IntelObjectiveState.Inactive)
    {
      comp1.State = IntelObjectiveState.Active;
      this.Dirty(toActivate, (IComponent) comp1);
    }
    IntelRetrieveItemObjectiveComponent comp2;
    if (this.TryComp<IntelRetrieveItemObjectiveComponent>(toActivate, out comp2) && comp2.State == IntelObjectiveState.Inactive)
    {
      comp2.State = IntelObjectiveState.Active;
      this.Dirty(toActivate, (IComponent) comp2);
    }
    IntelNumberComponent comp3;
    if (!this.TryComp<IntelNumberComponent>(toActivate, out comp3))
      return;
    IntelHasUnlockedComponent unlockedComponent = this.EnsureComp<IntelHasUnlockedComponent>(activatedBy);
    unlockedComponent.Unlocked.Add(comp3.Number);
    this.Dirty(activatedBy, (IComponent) unlockedComponent);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) activatedBy);
  }

  public bool TryUsePoints(FixedPoint2 points)
  {
    Entity<IntelTechTreeComponent> entity = this.EnsureTechTree();
    if (points > entity.Comp.Tree.Points)
      return false;
    entity.Comp.Tree.Points -= points;
    this.Dirty<IntelTechTreeComponent>(entity);
    this.UpdateTree(entity);
    return true;
  }

  public void AddPoints(Entity<IntelTechTreeComponent> tree, FixedPoint2 points)
  {
    tree.Comp.Tree.Points += points;
    tree.Comp.Tree.TotalEarned += points;
    this.Dirty<IntelTechTreeComponent>(tree);
    this.UpdateTree(tree);
  }

  public void AddPoints(FixedPoint2 points)
  {
    Entity<IntelTechTreeComponent>? tree;
    if (!this.TryGetTechTree(out tree))
      return;
    this.AddPoints(tree.Value, points);
  }

  public void UpdateTree(Entity<IntelTechTreeComponent> tree)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<TechControlConsoleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TechControlConsoleComponent>();
    EntityUid uid;
    TechControlConsoleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Tree = tree.Comp.Tree;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Entity<IntelTechTreeComponent>? tree;
    if (this.TryGetTechTree(out tree) && tree.Value.Comp.DoAnnouncements && curTime >= tree.Value.Comp.LastAnnounceAt)
    {
      tree.Value.Comp.LastAnnounceAt = curTime + this._announceEvery;
      this.Dirty<IntelTechTreeComponent>(tree.Value);
      Entity<ARESComponent> sender = this._ares.EnsureARES();
      FixedPoint2 points = tree.Value.Comp.Tree.Points;
      FixedPoint2 lastAnnouncePoints = tree.Value.Comp.LastAnnouncePoints;
      tree.Value.Comp.LastAnnouncePoints = points;
      this.Dirty<IntelTechTreeComponent>(tree.Value);
      FixedPoint2 fixedPoint2 = points - lastAnnouncePoints;
      foreach (ProtoId<RadioChannelPrototype> channel in tree.Value.Comp.AnnounceIn)
      {
        string message = fixedPoint2 > FixedPoint2.Zero ? this.Loc.GetString("rmc-intel-announcement-gain", ("points", (object) points), ("change", (object) fixedPoint2)) : this.Loc.GetString("rmc-intel-announcement", ("points", (object) points));
        this._marineAnnounce.AnnounceRadio((EntityUid) sender, message, channel);
      }
    }
    Robust.Shared.Prototypes.EntityPrototype areaPrototype;
    Entity<IntelTechTreeComponent> valueOrDefault1;
    if (this._activePositionIntels.Count > 0)
    {
      Entity<IntelRetrieveItemObjectiveComponent> result;
      while (this._activePositionIntels.TryDequeue(out result))
      {
        if (this._timing.CurTime >= curTime + this._maxProcessTime)
          return;
        IntelReadObjectiveComponent component;
        Entity<AreaComponent>? area;
        if (!this.TerminatingOrDeleted((EntityUid) result) && result.Comp.State != IntelObjectiveState.Complete && (!this._readObjectiveQuery.TryComp((EntityUid) result, out component) || component.State == IntelObjectiveState.Complete) && this._area.TryGetArea(result.Owner, out area, out areaPrototype) && area.Value.Comp.RetrieveItemObjective)
        {
          result.Comp.State = IntelObjectiveState.Complete;
          this.Dirty<IntelRetrieveItemObjectiveComponent>(result);
          valueOrDefault1 = tree.GetValueOrDefault();
          if (!tree.HasValue)
            tree = new Entity<IntelTechTreeComponent>?(this.EnsureTechTree());
          ++tree.Value.Comp.Tree.RetrieveItems.Current;
          IntelCluesComponent comp;
          if (this.TryComp<IntelCluesComponent>((EntityUid) result, out comp))
          {
            LocId? category = comp.Category;
            if (category.HasValue)
            {
              LocId valueOrDefault2 = category.GetValueOrDefault();
              Dictionary<NetEntity, string> dictionary;
              if (tree.Value.Comp.Tree.Clues.TryGetValue(valueOrDefault2, out dictionary))
                dictionary.Remove(this.GetNetEntity((EntityUid) result));
            }
          }
          this.AddPoints(tree.Value, result.Comp.Value);
          this.RemComp<ActiveIntelPositionComponent>((EntityUid) result);
        }
      }
    }
    Entity<AreaComponent>? nullable;
    if (this._activeSurvivorIntels.Count > 0)
    {
      Entity<IntelRescueSurvivorObjectiveComponent> result;
      while (this._activeSurvivorIntels.TryDequeue(out result))
      {
        if (this._timing.CurTime >= curTime + this._maxProcessTime)
          return;
        Entity<AreaComponent>? area;
        if (!this.TerminatingOrDeleted((EntityUid) result) && !this._mobState.IsDead((EntityUid) result) && this._area.TryGetArea(result.Owner, out area, out areaPrototype))
        {
          nullable = area;
          if (this.HasComp<IntelRescueSurvivorAreaComponent>(nullable.HasValue ? new EntityUid?((EntityUid) nullable.GetValueOrDefault()) : new EntityUid?()))
          {
            valueOrDefault1 = tree.GetValueOrDefault();
            if (!tree.HasValue)
              tree = new Entity<IntelTechTreeComponent>?(this.EnsureTechTree());
            ++tree.Value.Comp.Tree.RescueSurvivors;
            this.AddPoints(tree.Value, result.Comp.Value);
            this.RemComp<IntelRescueSurvivorObjectiveComponent>((EntityUid) result);
          }
        }
      }
    }
    if (this._activeCorpseIntels.Count > 0)
    {
      Entity<IntelRecoverCorpseObjectiveComponent> result;
      while (this._activeCorpseIntels.TryDequeue(out result))
      {
        if (this._timing.CurTime >= curTime + this._maxProcessTime)
          return;
        Entity<AreaComponent>? area;
        if (!this.TerminatingOrDeleted((EntityUid) result) && this._mobState.IsDead((EntityUid) result) && this._area.TryGetArea(result.Owner, out area, out areaPrototype))
        {
          nullable = area;
          if (this.HasComp<IntelRecoverCorpsesAreaComponent>(nullable.HasValue ? new EntityUid?((EntityUid) nullable.GetValueOrDefault()) : new EntityUid?()))
          {
            valueOrDefault1 = tree.GetValueOrDefault();
            if (!tree.HasValue)
              tree = new Entity<IntelTechTreeComponent>?(this.EnsureTechTree());
            ++tree.Value.Comp.Tree.RecoverCorpses;
            this.Dirty<IntelTechTreeComponent>(tree.Value);
            this.RemComp<ActiveIntelCorpseComponent>((EntityUid) result);
            if (!this.HasComp<XenoComponent>((EntityUid) result))
            {
              if (tree.Value.Comp.HumanoidCorpses < this._intelHumanoidsCorpsesMax)
                ++tree.Value.Comp.HumanoidCorpses;
              else
                continue;
            }
            this.AddPoints(tree.Value, result.Comp.Value);
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveIntelPositionComponent, IntelRetrieveItemObjectiveComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<ActiveIntelPositionComponent, IntelRetrieveItemObjectiveComponent>();
    EntityUid uid1;
    IntelRetrieveItemObjectiveComponent comp2_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out ActiveIntelPositionComponent _, out comp2_1))
    {
      if (comp2_1.State == IntelObjectiveState.Active)
        this._activePositionIntels.Enqueue((Entity<IntelRetrieveItemObjectiveComponent>) (uid1, comp2_1));
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<IntelRescueSurvivorObjectiveComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<IntelRescueSurvivorObjectiveComponent>();
    EntityUid uid2;
    IntelRescueSurvivorObjectiveComponent comp1;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1))
      this._activeSurvivorIntels.Enqueue((Entity<IntelRescueSurvivorObjectiveComponent>) (uid2, comp1));
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveIntelCorpseComponent, IntelRecoverCorpseObjectiveComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<ActiveIntelCorpseComponent, IntelRecoverCorpseObjectiveComponent>();
    EntityUid uid3;
    IntelRecoverCorpseObjectiveComponent comp2_2;
    while (entityQueryEnumerator3.MoveNext(out uid3, out ActiveIntelCorpseComponent _, out comp2_2))
      this._activeCorpseIntels.Enqueue((Entity<IntelRecoverCorpseObjectiveComponent>) (uid3, comp2_2));
    if (!tree.HasValue || tree.Value.Comp.Tree.ColonyPower)
      return;
    int num = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<IntelPowerObjectiveComponent, RMCFusionReactorComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<IntelPowerObjectiveComponent, RMCFusionReactorComponent>();
    RMCFusionReactorComponent comp2_3;
    while (entityQueryEnumerator4.MoveNext(out IntelPowerObjectiveComponent _, out comp2_3))
    {
      if (comp2_3.State == RMCFusionReactorState.Working)
        num += comp2_3.Watts;
    }
    if (num < this._powerObjectiveWattsRequired)
      return;
    tree.Value.Comp.Tree.ColonyPower = true;
    this.AddPoints(tree.Value, tree.Value.Comp.PowerPoints);
  }
}
