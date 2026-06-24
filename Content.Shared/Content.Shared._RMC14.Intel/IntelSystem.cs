using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
using Robust.Shared.Audio;
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

	private static readonly ImmutableArray<char> UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToImmutableArray();

	private static readonly EntProtoId<IntelTechTreeComponent> TechTreeProto = EntProtoId<IntelTechTreeComponent>.op_Implicit("RMCIntelTechTree");

	private static readonly EntProtoId PaperScrapProto = EntProtoId.op_Implicit("RMCIntelPaperScrap");

	private static readonly EntProtoId ProgressReportProto = EntProtoId.op_Implicit("RMCIntelProgressReport");

	private static readonly EntProtoId FolderProto = EntProtoId.op_Implicit("RMCIntelFolder");

	private static readonly EntProtoId TechnicalManualProto = EntProtoId.op_Implicit("RMCIntelTechnicalManual");

	private static readonly EntProtoId ExperimentalDevicesProto = EntProtoId.op_Implicit("RMCIntelRetrieveHealthAnalyzer");

	private readonly Dictionary<IntelSpawnerType, float> _paperScrapChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 20f,
		[IntelSpawnerType.Medium] = 5f,
		[IntelSpawnerType.Far] = 2f,
		[IntelSpawnerType.Science] = 10f
	};

	private readonly Dictionary<IntelSpawnerType, float> _progressReportChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 10f,
		[IntelSpawnerType.Medium] = 55f,
		[IntelSpawnerType.Far] = 3f,
		[IntelSpawnerType.Science] = 10f
	};

	private readonly Dictionary<IntelSpawnerType, float> _folderChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 20f,
		[IntelSpawnerType.Medium] = 5f,
		[IntelSpawnerType.Far] = 2f,
		[IntelSpawnerType.Science] = 10f
	};

	private readonly Dictionary<IntelSpawnerType, float> _technicalManualChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 20f,
		[IntelSpawnerType.Medium] = 40f,
		[IntelSpawnerType.Far] = 20f,
		[IntelSpawnerType.Science] = 20f
	};

	private readonly Dictionary<IntelSpawnerType, float> _diskChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 20f,
		[IntelSpawnerType.Medium] = 40f,
		[IntelSpawnerType.Far] = 20f,
		[IntelSpawnerType.Science] = 20f
	};

	private readonly Dictionary<IntelSpawnerType, float> _experimentalDeviceChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 10f,
		[IntelSpawnerType.Medium] = 20f,
		[IntelSpawnerType.Far] = 40f,
		[IntelSpawnerType.Science] = 30f
	};

	private readonly Dictionary<IntelSpawnerType, float> _researchPaperChances = new Dictionary<IntelSpawnerType, float>
	{
		[IntelSpawnerType.Close] = 25f,
		[IntelSpawnerType.Medium] = 20f,
		[IntelSpawnerType.Far] = 5f,
		[IntelSpawnerType.Science] = 50f
	};

	private readonly Dictionary<IntelSpawnerType, float> _vialBoxChances = new Dictionary<IntelSpawnerType, float>
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

	private EntityQuery<IntelReadObjectiveComponent> _readObjectiveQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_readObjectiveQuery = ((EntitySystem)this).GetEntityQuery<IntelReadObjectiveComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipLandedOnPlanetEvent>((EntityEventRefHandler<DropshipLandedOnPlanetEvent>)OnDropshipLandedOnPlanet, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelNumberComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<IntelNumberComponent, RefreshNameModifiersEvent>)OnNumberRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelUnlocksComponent, ComponentRemove>((EntityEventRefHandler<IntelUnlocksComponent, ComponentRemove>)OnUnlocksRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelUnlocksComponent, EntityTerminatingEvent>((EntityEventRefHandler<IntelUnlocksComponent, EntityTerminatingEvent>)OnUnlocksRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelRequiresComponent, ComponentRemove>((EntityEventRefHandler<IntelRequiresComponent, ComponentRemove>)OnRequiresRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelRequiresComponent, EntityTerminatingEvent>((EntityEventRefHandler<IntelRequiresComponent, EntityTerminatingEvent>)OnRequiresRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelReadObjectiveComponent, UseInHandEvent>((EntityEventRefHandler<IntelReadObjectiveComponent, UseInHandEvent>)OnReadUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelReadObjectiveComponent, IntelReadDoAfterEvent>((EntityEventRefHandler<IntelReadObjectiveComponent, IntelReadDoAfterEvent>)OnReadDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelRetrieveItemObjectiveComponent, MapInitEvent>((EntityEventRefHandler<IntelRetrieveItemObjectiveComponent, MapInitEvent>)OnRetrieveMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		((EntitySystem)this).SubscribeLocalEvent<IntelRetrieveItemObjectiveComponent, ContainerGettingInsertedAttemptEvent>((ComponentEventHandler<IntelRetrieveItemObjectiveComponent, ContainerGettingInsertedAttemptEvent>)OnHandPickUp, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelRetrieveItemObjectiveComponent, PullAttemptEvent>((EntityEventRefHandler<IntelRetrieveItemObjectiveComponent, PullAttemptEvent>)OnIntelPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveIntelCorpseComponent, PullAttemptEvent>((EntityEventRefHandler<ActiveIntelCorpseComponent, PullAttemptEvent>)OnIntelCorpsePullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ViewIntelObjectivesComponent, MapInitEvent>((EntityEventRefHandler<ViewIntelObjectivesComponent, MapInitEvent>)OnViewIntelObjectivesMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		((EntitySystem)this).SubscribeLocalEvent<ViewIntelObjectivesComponent, ViewIntelObjectivesActionEvent>((EntityEventRefHandler<ViewIntelObjectivesComponent, ViewIntelObjectivesActionEvent>)OnViewIntelObjectivesAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelHasUnlockedComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<IntelHasUnlockedComponent, RefreshNameModifiersEvent>)OnHasUnlockedRefreshName, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelSerialComponent, MapInitEvent>((EntityEventRefHandler<IntelSerialComponent, MapInitEvent>)OnIntelSerialMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		((EntitySystem)this).SubscribeLocalEvent<IntelSerialComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<IntelSerialComponent, RefreshNameModifiersEvent>)OnIntelSerialRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelSerialComponent, ExaminedEvent>((EntityEventRefHandler<IntelSerialComponent, ExaminedEvent>)OnIntelSerialExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelRecoverCorpseObjectiveOnDeathComponent, MobStateChangedEvent>((EntityEventRefHandler<IntelRecoverCorpseObjectiveOnDeathComponent, MobStateChangedEvent>)OnRescueCorpseObjectiveOnDeathChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelRecoverCorpseObjectiveComponent, MapInitEvent>((EntityEventRefHandler<IntelRecoverCorpseObjectiveComponent, MapInitEvent>)OnRescueCorpseObjectiveMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		((EntitySystem)this).SubscribeLocalEvent<IntelKnowledgeComponent, ComponentRemove>((EntityEventRefHandler<IntelKnowledgeComponent, ComponentRemove>)OnKnowledgeRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelKnowledgeComponent, EntityTerminatingEvent>((EntityEventRefHandler<IntelKnowledgeComponent, EntityTerminatingEvent>)OnKnowledgeRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelReadComponent, ComponentRemove>((EntityEventRefHandler<IntelReadComponent, ComponentRemove>)OnReadRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelReadComponent, EntityTerminatingEvent>((EntityEventRefHandler<IntelReadComponent, EntityTerminatingEvent>)OnReadRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelConsoleComponent, InteractHandEvent>((EntityEventRefHandler<IntelConsoleComponent, InteractHandEvent>)OnConsoleInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelConsoleComponent, IntelSubmitDoAfterEvent>((EntityEventRefHandler<IntelConsoleComponent, IntelSubmitDoAfterEvent>)OnConsoleSubmitDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelCluesComponent, MapInitEvent>((EntityEventRefHandler<IntelCluesComponent, MapInitEvent>)OnIntelCluesMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelPaperScraps, (Action<int>)delegate(int v)
		{
			_paperScraps = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelProgressReports, (Action<int>)delegate(int v)
		{
			_progressReports = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelFolders, (Action<int>)delegate(int v)
		{
			_folders = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelTechnicalManuals, (Action<int>)delegate(int v)
		{
			_technicalManuals = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelDisks, (Action<int>)delegate(int v)
		{
			_disks = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelExperimentalDevices, (Action<int>)delegate(int v)
		{
			_experimentalDevices = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelResearchPapers, (Action<int>)delegate(int v)
		{
			_researchPapers = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelVialBoxes, (Action<int>)delegate(int v)
		{
			_vialBoxes = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelMaxProcessTimeMilliseconds, (Action<float>)delegate(float v)
		{
			_maxProcessTime = TimeSpan.FromMilliseconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelAnnounceEveryMinutes, (Action<float>)delegate(float v)
		{
			_announceEvery = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelPowerObjectiveWattsRequired, (Action<int>)delegate(int v)
		{
			_powerObjectiveWattsRequired = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCIntelHumanoidCorpsesMax, (Action<int>)delegate(int v)
		{
			_intelHumanoidsCorpsesMax = v;
		}, true);
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_spawners.Clear();
		_activePositionIntels.Clear();
		_activeSurvivorIntels.Clear();
	}

	private void OnDropshipLandedOnPlanet(ref DropshipLandedOnPlanetEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Entity<IntelTechTreeComponent> tree = EnsureTechTree();
		tree.Comp.DoAnnouncements = true;
		((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree, (MetaDataComponent)null);
	}

	private void OnNumberRefreshNameModifiers(Entity<IntelNumberComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		args.AddModifier(LocId.op_Implicit("rmc-intel-suffix"), 0, ("number", ent.Comp.Number));
	}

	private void OnUnlocksRemove<T>(Entity<IntelUnlocksComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		IntelRequiresComponent requires = default(IntelRequiresComponent);
		foreach (EntityUid unlocks in ent.Comp.Unlocks)
		{
			if (((EntitySystem)this).TryComp<IntelRequiresComponent>(unlocks, ref requires))
			{
				requires.Requires.Remove(Entity<IntelUnlocksComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnRequiresRemove<T>(Entity<IntelRequiresComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		IntelUnlocksComponent unlocks = default(IntelUnlocksComponent);
		foreach (EntityUid requires in ent.Comp.Requires)
		{
			if (((EntitySystem)this).TryComp<IntelUnlocksComponent>(requires, ref unlocks))
			{
				unlocks.Unlocks.Remove(Entity<IntelRequiresComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnReadUseInHand(Entity<IntelReadObjectiveComponent> ent, ref UseInHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (((EntitySystem)this).HasComp<IntelRescueSurvivorObjectiveComponent>(user))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-intel-survivor-read", (ValueTuple<string, object>)("thing", ((EntitySystem)this).Name(Entity<IntelReadObjectiveComponent>.op_Implicit(ent), (MetaDataComponent)null))), Entity<IntelReadObjectiveComponent>.op_Implicit(ent), user);
			return;
		}
		TimeSpan delay = ent.Comp.Delay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill);
		IntelReadDoAfterEvent ev = new IntelReadDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<IntelReadObjectiveComponent>.op_Implicit(ent))
		{
			BreakOnDropItem = true,
			NeedHand = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupClient("You start reading the " + ((EntitySystem)this).Name(Entity<IntelReadObjectiveComponent>.op_Implicit(ent), (MetaDataComponent)null), Entity<IntelReadObjectiveComponent>.op_Implicit(ent), user);
		}
	}

	private void OnHandPickUp(EntityUid ent, IntelRetrieveItemObjectiveComponent component, ContainerGettingInsertedAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((ContainerAttemptEventBase)args).Container.Owner;
		if (((EntitySystem)this).HasComp<IntelRescueSurvivorObjectiveComponent>(user))
		{
			((CancellableEntityEventArgs)args).Cancel();
			_popup.PopupClient(base.Loc.GetString("rmc-intel-survivor-pickup", (ValueTuple<string, object>)("thing", ((EntitySystem)this).Name(ent, (MetaDataComponent)null))), ent, user);
		}
	}

	private void OnIntelPullAttempt(Entity<IntelRetrieveItemObjectiveComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.PullerUid;
		if (((EntitySystem)this).HasComp<IntelRescueSurvivorObjectiveComponent>(user))
		{
			args.Cancelled = true;
			_popup.PopupClient(base.Loc.GetString("rmc-intel-survivor-pickup", (ValueTuple<string, object>)("thing", ((EntitySystem)this).Name(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(ent), (MetaDataComponent)null))), Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(ent), user);
		}
	}

	private void OnIntelCorpsePullAttempt(Entity<ActiveIntelCorpseComponent> ent, ref PullAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.PullerUid;
		if (((EntitySystem)this).HasComp<IntelRescueSurvivorObjectiveComponent>(user))
		{
			args.Cancelled = true;
			string msg = (((EntitySystem)this).HasComp<XenoComponent>(Entity<ActiveIntelCorpseComponent>.op_Implicit(ent)) ? base.Loc.GetString("rmc-intel-survivor-xeno-pull", (ValueTuple<string, object>)("thing", ((EntitySystem)this).Name(Entity<ActiveIntelCorpseComponent>.op_Implicit(ent), (MetaDataComponent)null))) : base.Loc.GetString("rmc-intel-survivor-corpse-pull", (ValueTuple<string, object>)("thing", ((EntitySystem)this).Name(Entity<ActiveIntelCorpseComponent>.op_Implicit(ent), (MetaDataComponent)null))));
			_popup.PopupClient(msg, Entity<ActiveIntelCorpseComponent>.op_Implicit(ent), user);
		}
	}

	private void OnReadDoAfter(Entity<IntelReadObjectiveComponent> ent, ref IntelReadDoAfterEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid user = args.User;
		((HandledEntityEventArgs)args).Handled = true;
		if (args.Cancelled)
		{
			_popup.PopupClient("You get distracted and lose your train of thought, you'll have to start over reading this.", Entity<IntelReadObjectiveComponent>.op_Implicit(ent), user);
			return;
		}
		if (ent.Comp.State == IntelObjectiveState.Inactive)
		{
			_popup.PopupClient("You don't notice anything useful. You probably need to find its instructions on a paper scrap", Entity<IntelReadObjectiveComponent>.op_Implicit(ent), user);
			return;
		}
		_popup.PopupClient("You finish reading the " + ((EntitySystem)this).Name(Entity<IntelReadObjectiveComponent>.op_Implicit(ent), (MetaDataComponent)null), Entity<IntelReadObjectiveComponent>.op_Implicit(ent), user);
		if (ent.Comp.State == IntelObjectiveState.Complete)
		{
			return;
		}
		ent.Comp.State = IntelObjectiveState.Complete;
		((EntitySystem)this).Dirty<IntelReadObjectiveComponent>(ent, (MetaDataComponent)null);
		if (!_net.IsClient)
		{
			Entity<IntelTechTreeComponent> tree = EnsureTechTree();
			tree.Comp.Tree.Documents.Current++;
			AddPoints(tree, ent.Comp.Value);
			IntelKnowledgeComponent knowledge = ((EntitySystem)this).EnsureComp<IntelKnowledgeComponent>(user);
			knowledge.Read.Add(Entity<IntelReadObjectiveComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty(user, (IComponent)(object)knowledge, (MetaDataComponent)null);
			IntelReadComponent read = ((EntitySystem)this).EnsureComp<IntelReadComponent>(Entity<IntelReadObjectiveComponent>.op_Implicit(ent));
			read.Readers.Add(user);
			((EntitySystem)this).Dirty(Entity<IntelReadObjectiveComponent>.op_Implicit(ent), (IComponent)(object)read, (MetaDataComponent)null);
			IntelRetrieveItemObjectiveComponent retrieve = default(IntelRetrieveItemObjectiveComponent);
			if (((EntitySystem)this).TryComp<IntelRetrieveItemObjectiveComponent>(Entity<IntelReadObjectiveComponent>.op_Implicit(ent), ref retrieve) && retrieve.State == IntelObjectiveState.Inactive)
			{
				retrieve.State = IntelObjectiveState.Active;
				((EntitySystem)this).Dirty(Entity<IntelReadObjectiveComponent>.op_Implicit(ent), (IComponent)(object)retrieve, (MetaDataComponent)null);
			}
			UpdateTree(tree);
		}
	}

	private void OnRetrieveMapInit(Entity<IntelRetrieveItemObjectiveComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == IntelObjectiveState.Active)
		{
			((EntitySystem)this).EnsureComp<ActiveIntelPositionComponent>(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(ent));
		}
	}

	private void OnViewIntelObjectivesMapInit(Entity<ViewIntelObjectivesComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		_actions.AddAction(Entity<ViewIntelObjectivesComponent>.op_Implicit(ent), ref ent.Comp.Action, ent.Comp.ActionId);
	}

	private void OnViewIntelObjectivesAction(Entity<ViewIntelObjectivesComponent> ent, ref ViewIntelObjectivesActionEvent args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			IntelTechTree tree = EnsureTechTree().Comp.Tree;
			ent.Comp.Tree = tree;
			((EntitySystem)this).Dirty<ViewIntelObjectivesComponent>(ent, (MetaDataComponent)null);
		}
		_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ViewIntelObjectivesUI.Key, (EntityUid?)Entity<ViewIntelObjectivesComponent>.op_Implicit(ent), false);
	}

	private void OnHasUnlockedRefreshName(Entity<IntelHasUnlockedComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		args.AddModifier(LocId.op_Implicit("rmc-intel-unlocked"), 0, ("unlocked", string.Join(", ", ent.Comp.Unlocked)));
	}

	private void OnIntelSerialMapInit(Entity<IntelSerialComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Serial = $"{Number()}{Char()}{Number()}{Number()}{Number()}{Number()}{Char()}";
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		char Char()
		{
			return RandomExtensions.Pick<char>(_random, (IReadOnlyList<char>)UppercaseLetters);
		}
		int Number()
		{
			return _random.Next(0, 10);
		}
	}

	private void OnIntelSerialRefreshNameModifiers(Entity<IntelSerialComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		args.AddModifier(LocId.op_Implicit("rmc-intel-serial-name"), 0, ("serial", ent.Comp.Serial));
	}

	private void OnIntelSerialExamined(Entity<IntelSerialComponent> ent, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("IntelSerialComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-intel-serial-examine", (ValueTuple<string, object>)("serial", ent.Comp.Serial)));
		}
	}

	private void OnRescueCorpseObjectiveOnDeathChanged(Entity<IntelRecoverCorpseObjectiveOnDeathComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (args.OldMobState != MobState.Dead && args.NewMobState == MobState.Dead && !((EntitySystem)this).HasComp<IntelRecoverCorpseObjectiveComponent>(args.Target))
		{
			IntelRecoverCorpseObjectiveComponent comp = ((EntitySystem)this).EnsureComp<IntelRecoverCorpseObjectiveComponent>(args.Target);
			comp.Value = ent.Comp.Value;
			((EntitySystem)this).Dirty(args.Target, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnRescueCorpseObjectiveMapInit(Entity<IntelRecoverCorpseObjectiveComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<ActiveIntelCorpseComponent>(Entity<IntelRecoverCorpseObjectiveComponent>.op_Implicit(ent));
	}

	private void OnKnowledgeRemove<T>(Entity<IntelKnowledgeComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		IntelReadComponent readComp = default(IntelReadComponent);
		foreach (EntityUid read in ent.Comp.Read)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(read, (MetaDataComponent)null) && ((EntitySystem)this).TryComp<IntelReadComponent>(read, ref readComp))
			{
				readComp.Readers.Remove(Entity<IntelKnowledgeComponent>.op_Implicit(ent));
				((EntitySystem)this).Dirty(read, (IComponent)(object)readComp, (MetaDataComponent)null);
			}
		}
	}

	private void OnReadRemove<T>(Entity<IntelReadComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		IntelKnowledgeComponent knowledge = default(IntelKnowledgeComponent);
		foreach (EntityUid reader in ent.Comp.Readers)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(reader, (MetaDataComponent)null) && ((EntitySystem)this).TryComp<IntelKnowledgeComponent>(reader, ref knowledge))
			{
				knowledge.Read.Remove(Entity<IntelReadComponent>.op_Implicit(ent));
				((EntitySystem)this).Dirty(reader, (IComponent)(object)knowledge, (MetaDataComponent)null);
			}
		}
	}

	private void OnConsoleInteractHand(Entity<IntelConsoleComponent> ent, ref InteractHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		string msg = "You start typing in intel into the computer...";
		IntelKnowledgeComponent knowledge = default(IntelKnowledgeComponent);
		EntityUid? read = default(EntityUid?);
		if (!((EntitySystem)this).TryComp<IntelKnowledgeComponent>(args.User, ref knowledge) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)knowledge.Read, ref read))
		{
			msg += " and you have nothing new to add...";
			_popup.PopupClient(msg, Entity<IntelConsoleComponent>.op_Implicit(ent), args.User, PopupType.Medium);
			return;
		}
		_popup.PopupClient(msg, Entity<IntelConsoleComponent>.op_Implicit(ent), args.User, PopupType.Medium);
		TimeSpan delay = ent.Comp.Delay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skill);
		IntelSubmitDoAfterEvent ev = new IntelSubmitDoAfterEvent
		{
			Intel = ((EntitySystem)this).GetNetEntity(read.Value, (MetaDataComponent)null)
		};
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, delay, ev, Entity<IntelConsoleComponent>.op_Implicit(ent), Entity<IntelConsoleComponent>.op_Implicit(ent), Entity<IntelConsoleComponent>.op_Implicit(ent))
		{
			BreakOnMove = true
		};
		_doAfter.TryStartDoAfter(doAfter);
	}

	private void OnConsoleSubmitDoAfter(Entity<IntelConsoleComponent> ent, ref IntelSubmitDoAfterEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsClient)
		{
			return;
		}
		if (args.Cancelled)
		{
			_popup.PopupEntity("You get distracted and lose your train of thought, you'll have to start the typing over...", Entity<IntelConsoleComponent>.op_Implicit(ent), args.User, PopupType.MediumCaution);
			args.Repeat = false;
			return;
		}
		IntelKnowledgeComponent knowledge = default(IntelKnowledgeComponent);
		if (!((EntitySystem)this).TryComp<IntelKnowledgeComponent>(args.User, ref knowledge))
		{
			StopPopup(ref args);
			return;
		}
		EntityUid? intel = default(EntityUid?);
		IntelUnlocksComponent unlocks = default(IntelUnlocksComponent);
		EntityUid? unlock = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Intel, ref intel) || !((EntitySystem)this).TryComp<IntelUnlocksComponent>(intel, ref unlocks) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)unlocks.Unlocks, ref unlock))
		{
			if (!Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)knowledge.Read, ref intel))
			{
				StopPopup(ref args);
				return;
			}
			args.Intel = ((EntitySystem)this).GetNetEntity(intel.Value, (MetaDataComponent)null);
			if (!((EntitySystem)this).TryComp<IntelUnlocksComponent>(intel, ref unlocks) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)unlocks.Unlocks, ref unlock))
			{
				knowledge.Read.Remove(intel.Value);
				args.Repeat = true;
				return;
			}
		}
		IntelCluesComponent cluesComp = default(IntelCluesComponent);
		if (((EntitySystem)this).TryComp<IntelCluesComponent>(unlock, ref cluesComp))
		{
			string msg = base.Loc.GetString(LocId.op_Implicit(cluesComp.Clue), (ValueTuple<string, object>)("intel", unlock), (ValueTuple<string, object>)("area", cluesComp.InitialArea));
			_rmcChat.ChatMessageToOne(msg, args.User);
			_popup.PopupEntity(msg, Entity<IntelConsoleComponent>.op_Implicit(ent), args.User, PopupType.Medium);
			IntelRetrieveItemObjectiveComponent retrieve = default(IntelRetrieveItemObjectiveComponent);
			if (((EntitySystem)this).TryComp<IntelRetrieveItemObjectiveComponent>(unlock, ref retrieve) && retrieve.State != IntelObjectiveState.Complete)
			{
				LocId? category = cluesComp.Category;
				if (category.HasValue)
				{
					LocId category2 = category.GetValueOrDefault();
					Extensions.GetOrNew<LocId, Dictionary<NetEntity, string>>(EnsureTechTree().Comp.Tree.Clues, category2)[((EntitySystem)this).GetNetEntity(unlock.Value, (MetaDataComponent)null)] = msg;
				}
			}
		}
		unlocks.Unlocks.Remove(unlock.Value);
		ActivateIntel(intel.Value, unlock.Value);
		args.Amount++;
		_audio.PlayPvs(ent.Comp.TypingSound, Entity<IntelConsoleComponent>.op_Implicit(ent), (AudioParams?)null);
		if (unlocks.Unlocks.Count == 0)
		{
			knowledge.Read.Remove(intel.Value);
		}
		if (knowledge.Read.Count > 0)
		{
			args.Repeat = true;
		}
		else
		{
			StopPopup(ref args);
		}
		void StopPopup(ref IntelSubmitDoAfterEvent reference)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (reference.Amount == 0)
			{
				_popup.PopupEntity("...and you have nothing new to add...", Entity<IntelConsoleComponent>.op_Implicit(ent), reference.User, PopupType.Medium);
			}
			else
			{
				_popup.PopupEntity($"...and done! You uploaded {reference.Amount} entries!", Entity<IntelConsoleComponent>.op_Implicit(ent), reference.User, PopupType.Medium);
			}
		}
	}

	private void OnIntelCluesMapInit(Entity<IntelCluesComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (_area.TryGetArea(Entity<IntelCluesComponent>.op_Implicit(ent), out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			ent.Comp.InitialArea = ((EntitySystem)this).Name(Entity<AreaComponent>.op_Implicit(area.Value), (MetaDataComponent)null);
		}
	}

	private List<EntityUid> SpawnIntel(EntProtoId proto, int count, Dictionary<IntelSpawnerType, float> chances)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> items = new List<EntityUid>();
		for (int i = 0; i < count; i++)
		{
			IntelSpawnerType type = _random.Pick(chances);
			if (!_spawners.TryGetValue(type, out List<Entity<IntelSpawnerComponent>> spawners) || spawners.Count <= 0)
			{
				continue;
			}
			Entity<IntelSpawnerComponent> spawner = RandomExtensions.Pick<Entity<IntelSpawnerComponent>>(_random, (IReadOnlyList<Entity<IntelSpawnerComponent>>)spawners);
			EntityCoordinates coords = _transform.GetMoverCoordinates(Entity<IntelSpawnerComponent>.op_Implicit(spawner));
			EntityUid intel = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(proto), coords);
			items.Add(intel);
			((EntitySystem)this).EnsureComp<ActiveIntelPositionComponent>(intel);
			IntelNumberComponent number = ((EntitySystem)this).EnsureComp<IntelNumberComponent>(intel);
			number.Number = _random.Next(100, 1000);
			((EntitySystem)this).Dirty(intel, (IComponent)(object)number, (MetaDataComponent)null);
			_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(intel));
			_nearby.Clear();
			_entityLookup.GetEntitiesInRange<IntelContainerComponent>(coords, 0.5f, _nearby, (LookupFlags)78);
			foreach (Entity<IntelContainerComponent> nearby in _nearby)
			{
				if ((((EntitySystem)this).HasComp<StorageComponent>(Entity<IntelContainerComponent>.op_Implicit(nearby)) && _storage.Insert(Entity<IntelContainerComponent>.op_Implicit(nearby), intel, out var _)) || _entityStorage.Insert(intel, Entity<IntelContainerComponent>.op_Implicit(nearby)))
				{
					break;
				}
			}
		}
		return items;
	}

	public Entity<IntelTechTreeComponent> EnsureTechTree()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTechTree(out Entity<IntelTechTreeComponent>? tree))
		{
			return tree.Value;
		}
		EntityUid treeId = ((EntitySystem)this).Spawn(EntProtoId<IntelTechTreeComponent>.op_Implicit(TechTreeProto), (ComponentRegistry)null, true);
		IntelTechTreeComponent treeComp = ((EntitySystem)this).EnsureComp<IntelTechTreeComponent>(treeId);
		tree = Entity<IntelTechTreeComponent>.op_Implicit((treeId, treeComp));
		foreach (List<TechOption> tier in treeComp.Tree.Options)
		{
			for (int i = 0; i < tier.Count; i++)
			{
				TechOption option = tier[i];
				if (option.CurrentCost == 0)
				{
					tier[i] = option with
					{
						CurrentCost = option.Cost
					};
				}
			}
		}
		return tree.Value;
	}

	public bool TryGetTechTree([NotNullWhen(true)] out Entity<IntelTechTreeComponent>? tree)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		IntelTechTreeComponent comp = default(IntelTechTreeComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<IntelTechTreeComponent>().MoveNext(ref uid, ref comp))
		{
			tree = Entity<IntelTechTreeComponent>.op_Implicit((uid, comp));
			return true;
		}
		tree = null;
		return false;
	}

	public void RunSpawners()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			EntityQueryEnumerator<IntelSpawnerComponent> spawnerQuery = ((EntitySystem)this).EntityQueryEnumerator<IntelSpawnerComponent>();
			EntityUid uid = default(EntityUid);
			IntelSpawnerComponent comp = default(IntelSpawnerComponent);
			while (spawnerQuery.MoveNext(ref uid, ref comp))
			{
				if (!base.EntityManager.IsQueuedForDeletion(uid))
				{
					Extensions.GetOrNew<IntelSpawnerType, List<Entity<IntelSpawnerComponent>>>(_spawners, comp.IntelType).Add(Entity<IntelSpawnerComponent>.op_Implicit((uid, comp)));
				}
			}
			if (_spawners.Count == 0)
			{
				return;
			}
			foreach (List<Entity<IntelSpawnerComponent>> value in _spawners.Values)
			{
				foreach (Entity<IntelSpawnerComponent> spawner in value)
				{
					((EntitySystem)this).QueueDel((EntityUid?)Entity<IntelSpawnerComponent>.op_Implicit(spawner));
				}
			}
			Entity<IntelTechTreeComponent> tree = EnsureTechTree();
			List<EntityUid> lows = SpawnIntel(PaperScrapProto, _paperScraps, _paperScrapChances);
			List<EntityUid> mediums = SpawnIntel(ProgressReportProto, _progressReports, _progressReportChances);
			mediums.AddRange(SpawnIntel(FolderProto, _folders, _folderChances));
			List<EntityUid> highs = SpawnIntel(TechnicalManualProto, _technicalManuals, _technicalManualChances);
			SpawnIntel(ExperimentalDevicesProto, _experimentalDevices, _experimentalDeviceChances);
			tree.Comp.Tree.Documents.Total = _paperScraps + _progressReports + _folders + _technicalManuals;
			tree.Comp.Tree.UploadData.Total = _disks;
			tree.Comp.Tree.RetrieveItems.Total = tree.Comp.Tree.Documents.Total + tree.Comp.Tree.UploadData.Total - _disks;
			if (mediums.Count > 0)
			{
				foreach (EntityUid low in lows)
				{
					EntityUid medium = RandomExtensions.Pick<EntityUid>(_random, (IReadOnlyList<EntityUid>)mediums);
					ConnectObjectives(low, medium);
				}
			}
			if (highs.Count > 0)
			{
				foreach (EntityUid medium2 in mediums)
				{
					AddRequires(Entity<IntelRequiresComponent>.op_Implicit(medium2), lows);
					EntityUid high = RandomExtensions.Pick<EntityUid>(_random, (IReadOnlyList<EntityUid>)highs);
					ConnectObjectives(medium2, high);
				}
			}
			if (mediums.Count <= 0)
			{
				return;
			}
			foreach (EntityUid high2 in highs)
			{
				AddRequires(Entity<IntelRequiresComponent>.op_Implicit(high2), mediums);
			}
		}
		finally
		{
			_spawners.Clear();
		}
	}

	public void RestoreColonyCommunications()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			Entity<IntelTechTreeComponent> tree = EnsureTechTree();
			if (!tree.Comp.Tree.ColonyCommunications)
			{
				tree.Comp.Tree.ColonyCommunications = true;
				AddPoints(tree, tree.Comp.ColonyCommunicationsPoints);
			}
		}
	}

	private void ConnectObjectives(EntityUid unlocksId, EntityUid requiresId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		IntelUnlocksComponent unlocks = ((EntitySystem)this).EnsureComp<IntelUnlocksComponent>(unlocksId);
		unlocks.Unlocks.Add(requiresId);
		((EntitySystem)this).Dirty(unlocksId, (IComponent)(object)unlocks, (MetaDataComponent)null);
		IntelRequiresComponent requires = ((EntitySystem)this).EnsureComp<IntelRequiresComponent>(requiresId);
		requires.Requires.Add(unlocksId);
		((EntitySystem)this).Dirty(requiresId, (IComponent)(object)requires, (MetaDataComponent)null);
		DeactivateIntel(requiresId);
	}

	private void AddRequires(Entity<IntelRequiresComponent?> requires, List<EntityUid> candidates)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		ref IntelRequiresComponent comp = ref requires.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<IntelRequiresComponent>(Entity<IntelRequiresComponent>.op_Implicit(requires));
		}
		if (requires.Comp.RequiresCount <= requires.Comp.Requires.Count)
		{
			return;
		}
		int left = requires.Comp.RequiresCount - requires.Comp.Requires.Count;
		for (int i = 0; i < left; i++)
		{
			_random.Shuffle<EntityUid>((IList<EntityUid>)candidates);
			foreach (EntityUid candidate in candidates)
			{
				if (!requires.Comp.Requires.Contains(candidate))
				{
					ConnectObjectives(candidate, Entity<IntelRequiresComponent>.op_Implicit(requires));
					if (requires.Comp.RequiresCount <= requires.Comp.Requires.Count)
					{
						break;
					}
				}
			}
			if (requires.Comp.RequiresCount <= requires.Comp.Requires.Count)
			{
				break;
			}
		}
	}

	private void DeactivateIntel(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		IntelReadObjectiveComponent read = default(IntelReadObjectiveComponent);
		if (((EntitySystem)this).TryComp<IntelReadObjectiveComponent>(ent, ref read))
		{
			read.State = IntelObjectiveState.Inactive;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)read, (MetaDataComponent)null);
		}
		IntelRetrieveItemObjectiveComponent retrieve = default(IntelRetrieveItemObjectiveComponent);
		if (((EntitySystem)this).TryComp<IntelRetrieveItemObjectiveComponent>(ent, ref retrieve))
		{
			retrieve.State = IntelObjectiveState.Inactive;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)retrieve, (MetaDataComponent)null);
		}
	}

	private void ActivateIntel(EntityUid activatedBy, EntityUid toActivate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		IntelReadObjectiveComponent read = default(IntelReadObjectiveComponent);
		if (((EntitySystem)this).TryComp<IntelReadObjectiveComponent>(toActivate, ref read) && read.State == IntelObjectiveState.Inactive)
		{
			read.State = IntelObjectiveState.Active;
			((EntitySystem)this).Dirty(toActivate, (IComponent)(object)read, (MetaDataComponent)null);
		}
		IntelRetrieveItemObjectiveComponent retrieve = default(IntelRetrieveItemObjectiveComponent);
		if (((EntitySystem)this).TryComp<IntelRetrieveItemObjectiveComponent>(toActivate, ref retrieve) && retrieve.State == IntelObjectiveState.Inactive)
		{
			retrieve.State = IntelObjectiveState.Active;
			((EntitySystem)this).Dirty(toActivate, (IComponent)(object)retrieve, (MetaDataComponent)null);
		}
		IntelNumberComponent number = default(IntelNumberComponent);
		if (((EntitySystem)this).TryComp<IntelNumberComponent>(toActivate, ref number))
		{
			IntelHasUnlockedComponent unlocked = ((EntitySystem)this).EnsureComp<IntelHasUnlockedComponent>(activatedBy);
			unlocked.Unlocked.Add(number.Number);
			((EntitySystem)this).Dirty(activatedBy, (IComponent)(object)unlocked, (MetaDataComponent)null);
			_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(activatedBy));
		}
	}

	public bool TryUsePoints(FixedPoint2 points)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Entity<IntelTechTreeComponent> tree = EnsureTechTree();
		if (points > tree.Comp.Tree.Points)
		{
			return false;
		}
		tree.Comp.Tree.Points -= points;
		((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree, (MetaDataComponent)null);
		UpdateTree(tree);
		return true;
	}

	public void AddPoints(Entity<IntelTechTreeComponent> tree, FixedPoint2 points)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		tree.Comp.Tree.Points += points;
		tree.Comp.Tree.TotalEarned += points;
		((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree, (MetaDataComponent)null);
		UpdateTree(tree);
	}

	public void AddPoints(FixedPoint2 points)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetTechTree(out Entity<IntelTechTreeComponent>? tree))
		{
			AddPoints(tree.Value, points);
		}
	}

	public void UpdateTree(Entity<IntelTechTreeComponent> tree)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<TechControlConsoleComponent> query = ((EntitySystem)this).EntityQueryEnumerator<TechControlConsoleComponent>();
		EntityUid uid = default(EntityUid);
		TechControlConsoleComponent console = default(TechControlConsoleComponent);
		while (query.MoveNext(ref uid, ref console))
		{
			console.Tree = tree.Comp.Tree;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)console, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (TryGetTechTree(out Entity<IntelTechTreeComponent>? tree) && tree.Value.Comp.DoAnnouncements && time >= tree.Value.Comp.LastAnnounceAt)
		{
			tree.Value.Comp.LastAnnounceAt = time + _announceEvery;
			((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree.Value, (MetaDataComponent)null);
			Entity<ARESComponent> ares = _ares.EnsureARES();
			FixedPoint2 points = tree.Value.Comp.Tree.Points;
			FixedPoint2 last = tree.Value.Comp.LastAnnouncePoints;
			tree.Value.Comp.LastAnnouncePoints = points;
			((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree.Value, (MetaDataComponent)null);
			FixedPoint2 change = points - last;
			foreach (ProtoId<RadioChannelPrototype> channel in tree.Value.Comp.AnnounceIn)
			{
				string announcement = ((change > FixedPoint2.Zero) ? base.Loc.GetString("rmc-intel-announcement-gain", (ValueTuple<string, object>)("points", points), (ValueTuple<string, object>)("change", change)) : base.Loc.GetString("rmc-intel-announcement", (ValueTuple<string, object>)("points", points)));
				_marineAnnounce.AnnounceRadio(Entity<ARESComponent>.op_Implicit(ares), announcement, channel);
			}
		}
		EntityPrototype areaPrototype;
		if (_activePositionIntels.Count > 0)
		{
			Entity<IntelRetrieveItemObjectiveComponent> intel;
			IntelReadObjectiveComponent read = default(IntelReadObjectiveComponent);
			IntelCluesComponent cluesComp = default(IntelCluesComponent);
			while (_activePositionIntels.TryDequeue(out intel))
			{
				if (_timing.CurTime >= time + _maxProcessTime)
				{
					return;
				}
				if (((EntitySystem)this).TerminatingOrDeleted(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(intel), (MetaDataComponent)null) || intel.Comp.State == IntelObjectiveState.Complete || (_readObjectiveQuery.TryComp(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(intel), ref read) && read.State != IntelObjectiveState.Complete) || !_area.TryGetArea(intel.Owner, out Entity<AreaComponent>? area, out areaPrototype) || !area.Value.Comp.RetrieveItemObjective)
				{
					continue;
				}
				intel.Comp.State = IntelObjectiveState.Complete;
				((EntitySystem)this).Dirty<IntelRetrieveItemObjectiveComponent>(intel, (MetaDataComponent)null);
				Entity<IntelTechTreeComponent> valueOrDefault = tree.GetValueOrDefault();
				if (!tree.HasValue)
				{
					valueOrDefault = EnsureTechTree();
					tree = valueOrDefault;
				}
				tree.Value.Comp.Tree.RetrieveItems.Current++;
				if (((EntitySystem)this).TryComp<IntelCluesComponent>(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(intel), ref cluesComp))
				{
					LocId? category = cluesComp.Category;
					if (category.HasValue)
					{
						LocId category2 = category.GetValueOrDefault();
						if (tree.Value.Comp.Tree.Clues.TryGetValue(category2, out Dictionary<NetEntity, string> clues))
						{
							clues.Remove(((EntitySystem)this).GetNetEntity(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(intel), (MetaDataComponent)null));
						}
					}
				}
				AddPoints(tree.Value, intel.Comp.Value);
				((EntitySystem)this).RemComp<ActiveIntelPositionComponent>(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit(intel));
			}
		}
		if (_activeSurvivorIntels.Count > 0)
		{
			Entity<IntelRescueSurvivorObjectiveComponent> intel2;
			while (_activeSurvivorIntels.TryDequeue(out intel2))
			{
				if (_timing.CurTime >= time + _maxProcessTime)
				{
					return;
				}
				if (((EntitySystem)this).TerminatingOrDeleted(Entity<IntelRescueSurvivorObjectiveComponent>.op_Implicit(intel2), (MetaDataComponent)null) || _mobState.IsDead(Entity<IntelRescueSurvivorObjectiveComponent>.op_Implicit(intel2)) || !_area.TryGetArea(intel2.Owner, out Entity<AreaComponent>? area2, out areaPrototype))
				{
					continue;
				}
				Entity<AreaComponent>? val = area2;
				if (((EntitySystem)this).HasComp<IntelRescueSurvivorAreaComponent>(val.HasValue ? new EntityUid?(Entity<AreaComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null)))
				{
					Entity<IntelTechTreeComponent> valueOrDefault = tree.GetValueOrDefault();
					if (!tree.HasValue)
					{
						valueOrDefault = EnsureTechTree();
						tree = valueOrDefault;
					}
					tree.Value.Comp.Tree.RescueSurvivors++;
					AddPoints(tree.Value, intel2.Comp.Value);
					((EntitySystem)this).RemComp<IntelRescueSurvivorObjectiveComponent>(Entity<IntelRescueSurvivorObjectiveComponent>.op_Implicit(intel2));
				}
			}
		}
		if (_activeCorpseIntels.Count > 0)
		{
			Entity<IntelRecoverCorpseObjectiveComponent> intel3;
			while (_activeCorpseIntels.TryDequeue(out intel3))
			{
				if (_timing.CurTime >= time + _maxProcessTime)
				{
					return;
				}
				if (((EntitySystem)this).TerminatingOrDeleted(Entity<IntelRecoverCorpseObjectiveComponent>.op_Implicit(intel3), (MetaDataComponent)null) || !_mobState.IsDead(Entity<IntelRecoverCorpseObjectiveComponent>.op_Implicit(intel3)) || !_area.TryGetArea(intel3.Owner, out Entity<AreaComponent>? area3, out areaPrototype))
				{
					continue;
				}
				Entity<AreaComponent>? val = area3;
				if (!((EntitySystem)this).HasComp<IntelRecoverCorpsesAreaComponent>(val.HasValue ? new EntityUid?(Entity<AreaComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null)))
				{
					continue;
				}
				Entity<IntelTechTreeComponent> valueOrDefault = tree.GetValueOrDefault();
				if (!tree.HasValue)
				{
					valueOrDefault = EnsureTechTree();
					tree = valueOrDefault;
				}
				tree.Value.Comp.Tree.RecoverCorpses++;
				((EntitySystem)this).Dirty<IntelTechTreeComponent>(tree.Value, (MetaDataComponent)null);
				((EntitySystem)this).RemComp<ActiveIntelCorpseComponent>(Entity<IntelRecoverCorpseObjectiveComponent>.op_Implicit(intel3));
				if (!((EntitySystem)this).HasComp<XenoComponent>(Entity<IntelRecoverCorpseObjectiveComponent>.op_Implicit(intel3)))
				{
					if (tree.Value.Comp.HumanoidCorpses >= _intelHumanoidsCorpsesMax)
					{
						continue;
					}
					tree.Value.Comp.HumanoidCorpses++;
				}
				AddPoints(tree.Value, intel3.Comp.Value);
			}
		}
		EntityQueryEnumerator<ActiveIntelPositionComponent, IntelRetrieveItemObjectiveComponent> activeIntelQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveIntelPositionComponent, IntelRetrieveItemObjectiveComponent>();
		EntityUid uid = default(EntityUid);
		ActiveIntelPositionComponent activeIntelPositionComponent = default(ActiveIntelPositionComponent);
		IntelRetrieveItemObjectiveComponent retrieve = default(IntelRetrieveItemObjectiveComponent);
		while (activeIntelQuery.MoveNext(ref uid, ref activeIntelPositionComponent, ref retrieve))
		{
			if (retrieve.State == IntelObjectiveState.Active)
			{
				_activePositionIntels.Enqueue(Entity<IntelRetrieveItemObjectiveComponent>.op_Implicit((uid, retrieve)));
			}
		}
		EntityQueryEnumerator<IntelRescueSurvivorObjectiveComponent> survivorQuery = ((EntitySystem)this).EntityQueryEnumerator<IntelRescueSurvivorObjectiveComponent>();
		EntityUid uid2 = default(EntityUid);
		IntelRescueSurvivorObjectiveComponent comp = default(IntelRescueSurvivorObjectiveComponent);
		while (survivorQuery.MoveNext(ref uid2, ref comp))
		{
			_activeSurvivorIntels.Enqueue(Entity<IntelRescueSurvivorObjectiveComponent>.op_Implicit((uid2, comp)));
		}
		EntityQueryEnumerator<ActiveIntelCorpseComponent, IntelRecoverCorpseObjectiveComponent> corpseQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveIntelCorpseComponent, IntelRecoverCorpseObjectiveComponent>();
		EntityUid uid3 = default(EntityUid);
		ActiveIntelCorpseComponent activeIntelCorpseComponent = default(ActiveIntelCorpseComponent);
		IntelRecoverCorpseObjectiveComponent comp2 = default(IntelRecoverCorpseObjectiveComponent);
		while (corpseQuery.MoveNext(ref uid3, ref activeIntelCorpseComponent, ref comp2))
		{
			_activeCorpseIntels.Enqueue(Entity<IntelRecoverCorpseObjectiveComponent>.op_Implicit((uid3, comp2)));
		}
		if (!tree.HasValue || tree.Value.Comp.Tree.ColonyPower)
		{
			return;
		}
		int watts = 0;
		EntityQueryEnumerator<IntelPowerObjectiveComponent, RMCFusionReactorComponent> generatorQuery = ((EntitySystem)this).EntityQueryEnumerator<IntelPowerObjectiveComponent, RMCFusionReactorComponent>();
		IntelPowerObjectiveComponent intelPowerObjectiveComponent = default(IntelPowerObjectiveComponent);
		RMCFusionReactorComponent generator = default(RMCFusionReactorComponent);
		while (generatorQuery.MoveNext(ref intelPowerObjectiveComponent, ref generator))
		{
			if (generator.State == RMCFusionReactorState.Working)
			{
				watts += generator.Watts;
			}
		}
		if (watts >= _powerObjectiveWattsRequired)
		{
			tree.Value.Comp.Tree.ColonyPower = true;
			AddPoints(tree.Value, tree.Value.Comp.PowerPoints);
		}
	}
}
