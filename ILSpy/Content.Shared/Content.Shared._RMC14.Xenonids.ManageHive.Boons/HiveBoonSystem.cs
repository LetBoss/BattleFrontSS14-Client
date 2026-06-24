using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

	private static readonly EntProtoId<HiveBoonDefinitionComponent> KingBoonId = EntProtoId<HiveBoonDefinitionComponent>.op_Implicit("RMCHiveBoonKing");

	private static readonly EntProtoId<HiveKingCocoonComponent> KingCocoonId = EntProtoId<HiveKingCocoonComponent>.op_Implicit("RMCHiveCocoonKing");

	private int _aliveMarineRequirement;

	private TimeSpan _royalResinEvery;

	private TimeSpan _kingVoteCandidateTimeRequired;

	private TimeSpan _kingFirstWarningTime;

	private TimeSpan _kingVoteStartTime;

	private TimeSpan _kingVoteAskCandidatesTime;

	private TimeSpan _kingVoteStartHatchingTime;

	private EntityQuery<ExcludedFromKingVoteComponent> _excludedFromKingVoteQuery;

	private readonly HashSet<ProtoId<PlayTimeTrackerPrototype>> _xenoJobs = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();

	public ImmutableArray<(EntityPrototype Prototype, HiveBoonDefinitionComponent Component)> Boons { get; private set; } = ImmutableArray<(EntityPrototype, HiveBoonDefinitionComponent)>.Empty;

	public TimeSpan CommunicationTowerXenoTakeoverTime { get; private set; }

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_excludedFromKingVoteQuery = ((EntitySystem)this).GetEntityQuery<ExcludedFromKingVoteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveBoonActivateFireResistanceEvent>((EntityEventHandler<HiveBoonActivateFireResistanceEvent>)OnActivateFireResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveBoonActivateLarvaSurgeEvent>((EntityEventHandler<HiveBoonActivateLarvaSurgeEvent>)OnActivateLarvaSurge, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveBoonActivateKingEvent>((EntityEventHandler<HiveBoonActivateKingEvent>)OnActivateKing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, RMCGetFireImmunityEvent>((EntityEventRefHandler<XenoComponent, RMCGetFireImmunityEvent>)OnGetTileFireImmunity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetIgnitionImmunityEvent>((EntityEventRefHandler<XenoComponent, GetIgnitionImmunityEvent>)OnGetTileFireIgnitionImmunity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, HiveKingVoteDialogEvent>((EntityEventRefHandler<XenoComponent, HiveKingVoteDialogEvent>)OnKingVote, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveClusterComponent, ExaminedEvent>((EntityEventRefHandler<HiveClusterComponent, ExaminedEvent>)OnClusterExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveClusterComponent, AfterEntityWeedingEvent>((EntityEventRefHandler<HiveClusterComponent, AfterEntityWeedingEvent>)OnClusterAfterWeeding, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HivePylonComponent, ExaminedEvent>((EntityEventRefHandler<HivePylonComponent, ExaminedEvent>)OnPylonExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HivePylonComponent, EntityTerminatingEvent>((EntityEventRefHandler<HivePylonComponent, EntityTerminatingEvent>)OnPylonTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, CommunicationsTowerStateChangedEvent>((EntityEventRefHandler<CommunicationsTowerComponent, CommunicationsTowerStateChangedEvent>)OnTowerBreak, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, RMCRepairableTargetAttemptEvent>((EntityEventRefHandler<CommunicationsTowerComponent, RMCRepairableTargetAttemptEvent>)OnTowerRepairAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveKingCocoonComponent, EntityTerminatingEvent>((EntityEventRefHandler<HiveKingCocoonComponent, EntityTerminatingEvent>)OnCocoonTerminating, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBoonsLiveMarineRequirement, (Action<int>)delegate(int v)
		{
			_aliveMarineRequirement = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCRoyalResinEveryMinutes, (Action<int>)delegate(int v)
		{
			_royalResinEvery = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCCommunicationTowerXenoTakeoverMinutes, (Action<int>)delegate(int v)
		{
			CommunicationTowerXenoTakeoverTime = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCKingVoteCandidateTimeRequirementHours, (Action<int>)delegate(int v)
		{
			_kingVoteCandidateTimeRequired = TimeSpan.FromHours(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCKingHatchingFirstWarningMinutes, (Action<int>)delegate(int v)
		{
			_kingFirstWarningTime = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCKingVoteStartTimeSeconds, (Action<int>)delegate(int v)
		{
			_kingVoteStartTime = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCKingVoteAskCandidatesTimeSeconds, (Action<int>)delegate(int v)
		{
			_kingVoteAskCandidatesTime = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCKingVoteStartHatchingTimeSeconds, (Action<int>)delegate(int v)
		{
			_kingVoteStartHatchingTime = TimeSpan.FromSeconds(v);
		}, true);
		ReloadPrototypes();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<EntityPrototype>())
		{
			ReloadPrototypes();
		}
	}

	private void OnActivateFireResistance(HiveBoonActivateFireResistanceEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<HiveBoonFireImmunityComponent>(ev.Boon);
		_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(ev.Boon), "The Queen has imbued us with flame-resistant chitin for 5 minutes.");
	}

	private void OnActivateLarvaSurge(HiveBoonActivateLarvaSurgeEvent ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_hive.IncreaseBurrowedLarva(ev.Hive, 5);
		_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(ev.Boon), "The Queen has awakened 5 extra burrowed larva to join the hive!");
	}

	private void OnActivateKing(HiveBoonActivateKingEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HivePylonComponent> pylonQuery = ((EntitySystem)this).EntityQueryEnumerator<HivePylonComponent>();
		EntityUid uid = default(EntityUid);
		HivePylonComponent hivePylonComponent = default(HivePylonComponent);
		while (pylonQuery.MoveNext(ref uid, ref hivePylonComponent))
		{
			_area.TrySetCanOrbitalBombardRoofing(Entity<RoofingEntityComponent>.op_Implicit(uid), ob: false);
		}
		EntityUid? core = ev.Core;
		if (core.HasValue)
		{
			EntityUid core2 = core.GetValueOrDefault();
			EntityUid cocoon = ((EntitySystem)this).SpawnAtPosition(EntProtoId<HiveKingCocoonComponent>.op_Implicit(KingCocoonId), core2.ToCoordinates(), (ComponentRegistry)null);
			_hive.SetHive(Entity<HiveMemberComponent>.op_Implicit(cocoon), Entity<HiveComponent>.op_Implicit(ev.Hive));
			string areaName = _area.GetAreaName(core2);
			_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-marine", (ValueTuple<string, object>)("area", areaName)));
			_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(core2), base.Loc.GetString("rmc-boon-king-announcement-xenos", (ValueTuple<string, object>)("area", areaName)));
		}
	}

	private void OnGetTileFireImmunity(Entity<XenoComponent> xeno, ref RMCGetFireImmunityEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveBoonFireImmunityComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiveBoonFireImmunityComponent>();
		EntityUid uid = default(EntityUid);
		HiveBoonFireImmunityComponent hiveBoonFireImmunityComponent = default(HiveBoonFireImmunityComponent);
		while (query.MoveNext(ref uid, ref hiveBoonFireImmunityComponent))
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(xeno.Owner)))
			{
				ev.Ignite = false;
				ev.Immune = true;
				break;
			}
		}
	}

	private void OnGetTileFireIgnitionImmunity(Entity<XenoComponent> xeno, ref GetIgnitionImmunityEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveBoonFireImmunityComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiveBoonFireImmunityComponent>();
		EntityUid uid = default(EntityUid);
		HiveBoonFireImmunityComponent hiveBoonFireImmunityComponent = default(HiveBoonFireImmunityComponent);
		while (query.MoveNext(ref uid, ref hiveBoonFireImmunityComponent))
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(xeno.Owner)))
			{
				args.Ignite = false;
			}
		}
	}

	private void OnKingVote(Entity<XenoComponent> ent, ref HiveKingVoteDialogEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		EntityUid cocoon = ((EntitySystem)this).GetEntity(args.Cocoon);
		if (!((EntityUid)(ref cocoon)).Valid)
		{
			return;
		}
		EntityUid votedId = ((EntitySystem)this).GetEntity(args.Voted);
		if (!((EntityUid)(ref votedId)).Valid)
		{
			return;
		}
		GetKingVotingData(Entity<ActorComponent>.op_Implicit(ent.Owner), cocoon, out var canBeKing, out var canVote);
		if (canVote)
		{
			GetKingVotingData(Entity<ActorComponent>.op_Implicit(votedId), cocoon, out var canBeKing2, out canBeKing);
			ActorComponent actor = default(ActorComponent);
			if (canBeKing2 && ((EntitySystem)this).TryComp<ActorComponent>(votedId, ref actor))
			{
				Entity<HiveKingVoteComponent> vote = EnsureVote(cocoon);
				NetUserId votedUserId = actor.PlayerSession.UserId;
				vote.Comp.Votes[votedUserId] = vote.Comp.Votes.GetValueOrDefault(votedUserId) + 1;
				((EntitySystem)this).Dirty<HiveKingVoteComponent>(vote, (MetaDataComponent)null);
			}
		}
	}

	private void OnPylonExamined(Entity<HivePylonComponent> ent, ref ExaminedEvent args)
	{
		using (args.PushGroup("HivePylonComponent"))
		{
			string msg = $"[color=cyan]This will grant the hive 1 royal resin every {(int)_royalResinEvery.TotalMinutes} minutes, allowing the Queen to obtain buffs![/color]";
			args.PushMarkup(msg);
		}
	}

	private void OnPylonTerminating(Entity<HivePylonComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<HivePylonComponent>.op_Implicit(ent), ref xform) || ((EntitySystem)this).TerminatingOrDeleted(xform.MapUid, (MetaDataComponent)null) || ((EntitySystem)this).HasComp<HiveConstructionSuppressAnnouncementsComponent>(Entity<HivePylonComponent>.op_Implicit(ent)))
		{
			return;
		}
		string area = _area.GetAreaName(Entity<HivePylonComponent>.op_Implicit(ent));
		_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-pylon-destroyed-announcement-marine", (ValueTuple<string, object>)("area", area)));
		_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), "We have lost our control of the tall's communication relay at " + area + ".");
		EntityUid? tower = ent.Comp.Tower;
		if (tower.HasValue)
		{
			EntityUid tower2 = tower.GetValueOrDefault();
			_appearance.SetData(tower2, (Enum)WeededEntityLayers.Layer, (object)false, (AppearanceComponent)null);
			CommunicationsTowerComponent towerComp = default(CommunicationsTowerComponent);
			if (((EntitySystem)this).TryComp<CommunicationsTowerComponent>(tower2, ref towerComp))
			{
				towerComp.XenoControlled = false;
				((EntitySystem)this).Dirty(tower2, (IComponent)(object)towerComp, (MetaDataComponent)null);
			}
		}
	}

	private void OnClusterExamined(Entity<HiveClusterComponent> ent, ref ExaminedEvent args)
	{
		using (args.PushGroup("HivePylonComponent"))
		{
			string msg = $"[color=cyan]If placed {(int)CommunicationTowerXenoTakeoverTime.TotalMinutes} minutes into the round, this can turn into a hive pylon when its weeds take over a telecommunications tower![/color]";
			args.PushMarkup(msg);
		}
	}

	private void OnClusterAfterWeeding(Entity<HiveClusterComponent> ent, ref AfterEntityWeedingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		CommunicationsTowerComponent tower = default(CommunicationsTowerComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<HiveClusterComponent>.op_Implicit(ent), (MetaDataComponent)null) && ((EntitySystem)this).TryComp<CommunicationsTowerComponent>(args.CoveredEntity, ref tower))
		{
			ReplaceCluster(ent, Entity<CommunicationsTowerComponent>.op_Implicit((args.CoveredEntity, tower)));
		}
	}

	private void OnTowerBreak(Entity<CommunicationsTowerComponent> ent, ref CommunicationsTowerStateChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State != CommunicationsTowerState.Broken)
		{
			return;
		}
		RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(Entity<CommunicationsTowerComponent>.op_Implicit(ent), null, (DirectionFlag)0);
		EntityUid uid;
		XenoWeedsComponent weeds = default(XenoWeedsComponent);
		HiveClusterComponent cluster = default(HiveClusterComponent);
		while (anchored.MoveNext(out uid))
		{
			if (((EntitySystem)this).TryComp<XenoWeedsComponent>(uid, ref weeds) && ((EntitySystem)this).TryComp<HiveClusterComponent>(weeds.Source, ref cluster))
			{
				ReplaceCluster(Entity<HiveClusterComponent>.op_Implicit((weeds.Source.Value, cluster)), ent);
			}
		}
	}

	private void OnTowerRepairAttempt(Entity<CommunicationsTowerComponent> ent, ref RMCRepairableTargetAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.XenoControlled)
		{
			args.Cancelled = true;
			args.Popup = "The " + ((EntitySystem)this).Name(Entity<CommunicationsTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + " is entangled in resin. Impossible to interact with.";
		}
	}

	private void OnCocoonTerminating(Entity<HiveKingCocoonComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<HiveKingCocoonComponent>.op_Implicit(ent), ref xform) && !((EntitySystem)this).TerminatingOrDeleted(xform.MapUid, (MetaDataComponent)null) && !((EntitySystem)this).HasComp<HiveConstructionSuppressAnnouncementsComponent>(Entity<HiveKingCocoonComponent>.op_Implicit(ent)))
		{
			string areaName = _area.GetAreaName(Entity<HiveKingCocoonComponent>.op_Implicit(ent));
			_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-stopped-marine", (ValueTuple<string, object>)("area", areaName)));
			_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), base.Loc.GetString("rmc-boon-king-announcement-stopped-xeno"));
		}
	}

	public bool HasEnoughAliveMarines()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (_aliveMarineRequirement <= 0)
		{
			return true;
		}
		int marines = 0;
		EntityQueryEnumerator<ActorComponent, MarineComponent, TransformComponent> marineQuery = ((EntitySystem)this).EntityQueryEnumerator<ActorComponent, MarineComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		ActorComponent val = default(ActorComponent);
		MarineComponent marineComponent = default(MarineComponent);
		TransformComponent xform = default(TransformComponent);
		while (marineQuery.MoveNext(ref uid, ref val, ref marineComponent, ref xform))
		{
			if (_rmcPlanet.IsOnPlanet(xform) && !_mobState.IsIncapacitated(uid))
			{
				marines++;
				if (marines >= _aliveMarineRequirement)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void StartKingVote(Entity<HiveKingCocoonComponent> cocoon)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		_xenoJobs.Clear();
		foreach (PlayTimeTrackerPrototype prototype in _prototype.EnumeratePrototypes<PlayTimeTrackerPrototype>())
		{
			if (prototype.IsXeno)
			{
				_xenoJobs.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(prototype.ID));
			}
		}
		List<DialogOption> options = new List<DialogOption>();
		List<EntityUid> canVoteList = new List<EntityUid>();
		NetEntity netCocoon = ((EntitySystem)this).GetNetEntity(Entity<HiveKingCocoonComponent>.op_Implicit(cocoon), (MetaDataComponent)null);
		EntityQueryEnumerator<ActorComponent, XenoComponent> xenosQuery = ((EntitySystem)this).EntityQueryEnumerator<ActorComponent, XenoComponent>();
		EntityUid uid = default(EntityUid);
		ActorComponent actor = default(ActorComponent);
		XenoComponent xenoComponent = default(XenoComponent);
		while (xenosQuery.MoveNext(ref uid, ref actor, ref xenoComponent))
		{
			GetKingVotingData(Entity<ActorComponent>.op_Implicit((uid, actor)), Entity<HiveKingCocoonComponent>.op_Implicit(cocoon), out var canBeKing, out var canVote);
			if (canBeKing)
			{
				options.Add(new DialogOption(((EntitySystem)this).Name(uid, (MetaDataComponent)null), new HiveKingVoteDialogEvent(netCocoon, ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null))));
			}
			if (canVote)
			{
				canVoteList.Add(uid);
			}
		}
		foreach (EntityUid uid2 in canVoteList)
		{
			_dialog.OpenOptions(uid2, "Choose a sister", options, "Vote for a sister you wish to become the King.");
		}
		EnsureVote(Entity<HiveKingCocoonComponent>.op_Implicit(cocoon));
	}

	private void GetKingVotingData(Entity<ActorComponent?> xeno, EntityUid cocoon, out bool canBeKing, out bool canVote)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		canBeKing = false;
		canVote = false;
		if (!((EntitySystem)this).Resolve<ActorComponent>(Entity<ActorComponent>.op_Implicit(xeno), ref xeno.Comp, false) || _mobState.IsDead(Entity<ActorComponent>.op_Implicit(xeno)) || !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(cocoon)))
		{
			return;
		}
		ExcludedFromKingVoteComponent excluded = default(ExcludedFromKingVoteComponent);
		if (_excludedFromKingVoteQuery.TryComp(Entity<ActorComponent>.op_Implicit(xeno), ref excluded))
		{
			canBeKing = excluded.CanBeKing;
			canVote = excluded.CanVote;
			return;
		}
		canVote = true;
		IReadOnlyDictionary<string, TimeSpan> playTimes;
		try
		{
			playTimes = _playtime.GetPlayTimes(xeno.Comp.PlayerSession);
		}
		catch
		{
			return;
		}
		TimeSpan totalTime = TimeSpan.Zero;
		foreach (var (jobId, jobTime) in playTimes)
		{
			if (_xenoJobs.Contains(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobId)))
			{
				totalTime += jobTime;
			}
		}
		if (!(totalTime < _kingVoteCandidateTimeRequired))
		{
			canBeKing = true;
		}
	}

	public Entity<HiveKingVoteComponent> EnsureVote(EntityUid xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveKingVoteComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiveKingVoteComponent>();
		EntityUid voteId = default(EntityUid);
		HiveKingVoteComponent voteComp = default(HiveKingVoteComponent);
		while (query.MoveNext(ref voteId, ref voteComp))
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(voteId), Entity<HiveMemberComponent>.op_Implicit(xeno)))
			{
				return Entity<HiveKingVoteComponent>.op_Implicit((voteId, voteComp));
			}
		}
		EntityUid vote = ((EntitySystem)this).Spawn((string)null, (ComponentRegistry)null, true);
		HiveKingVoteComponent comp = ((EntitySystem)this).EnsureComp<HiveKingVoteComponent>(vote);
		_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveMemberComponent>.op_Implicit(vote));
		return Entity<HiveKingVoteComponent>.op_Implicit((vote, comp));
	}

	private void ReplaceCluster(Entity<HiveClusterComponent> cluster, Entity<CommunicationsTowerComponent> tower)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		if (tower.Comp.XenoControlled || tower.Comp.State != CommunicationsTowerState.Broken || _gameTicker.RoundDuration() < CommunicationTowerXenoTakeoverTime)
		{
			return;
		}
		EntityUid newWeedSource = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(cluster.Comp.TowerReplaceWith), cluster.Owner.ToCoordinates(), (ComponentRegistry)null);
		_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(cluster.Owner), Entity<HiveMemberComponent>.op_Implicit(newWeedSource));
		_appearance.SetData(Entity<CommunicationsTowerComponent>.op_Implicit(tower), (Enum)WeededEntityLayers.Layer, (object)true, (AppearanceComponent)null);
		tower.Comp.XenoControlled = true;
		((EntitySystem)this).Dirty<CommunicationsTowerComponent>(tower, (MetaDataComponent)null);
		HivePylonComponent pylon = default(HivePylonComponent);
		if (((EntitySystem)this).TryComp<HivePylonComponent>(newWeedSource, ref pylon))
		{
			pylon.Tower = Entity<CommunicationsTowerComponent>.op_Implicit(tower);
			pylon.NextRoyalResin = _timing.CurTime + _royalResinEvery;
			((EntitySystem)this).Dirty(newWeedSource, (IComponent)(object)pylon, (MetaDataComponent)null);
			string areaName = _area.GetAreaName(Entity<CommunicationsTowerComponent>.op_Implicit(tower));
			_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-pylon-announcement-marine", (ValueTuple<string, object>)("area", areaName)));
			_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(newWeedSource), "We have harnessed the tall's communication relay at " + areaName + ".\n\nWe will now grow royal resin from this pylon. Hold it!");
		}
		XenoWeedsComponent newWeedSourceComp = default(XenoWeedsComponent);
		XenoWeedsComponent oldWeeds = default(XenoWeedsComponent);
		if (!((EntitySystem)this).TryComp<XenoWeedsComponent>(newWeedSource, ref newWeedSourceComp) || !((EntitySystem)this).TryComp<XenoWeedsComponent>(Entity<HiveClusterComponent>.op_Implicit(cluster), ref oldWeeds))
		{
			return;
		}
		foreach (EntityUid curWeed in oldWeeds.Spread)
		{
			XenoWeedsComponent xenoWeedsComponent = ((EntitySystem)this).EnsureComp<XenoWeedsComponent>(curWeed);
			xenoWeedsComponent.Range = newWeedSourceComp.Range;
			xenoWeedsComponent.Source = newWeedSource;
			newWeedSourceComp.Spread.Add(curWeed);
		}
		oldWeeds.Spread.Clear();
		((EntitySystem)this).Dirty(Entity<HiveClusterComponent>.op_Implicit(cluster), (IComponent)(object)oldWeeds, (MetaDataComponent)null);
		((EntitySystem)this).RemComp<XenoWeedsSpreadingComponent>(newWeedSource);
		((EntitySystem)this).QueueDel((EntityUid?)Entity<HiveClusterComponent>.op_Implicit(cluster));
	}

	public void TryActivateBoon(Entity<ManageHiveComponent> manage, EntProtoId<HiveBoonDefinitionComponent> boon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype boonProto = default(EntityPrototype);
		HiveBoonDefinitionComponent boonComp = default(HiveBoonDefinitionComponent);
		if (!_prototype.TryIndex(EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boon), ref boonProto) || !boonProto.TryGetComponent<HiveBoonDefinitionComponent>(ref boonComp, _compFactory))
		{
			return;
		}
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(manage.Owner));
		if (!hive.HasValue)
		{
			return;
		}
		Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
		Entity<HiveBoonsComponent> boons = EnsureBoons(hive2);
		if (boons.Comp.RoyalResin < boonComp.Cost)
		{
			string msg = base.Loc.GetString("rmc-boon-not-enough-royal-resin", (ValueTuple<string, object>)("cost", boonComp.Cost), (ValueTuple<string, object>)("current", boons.Comp.RoyalResin));
			_popup.PopupCursor(msg, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
			return;
		}
		int pylons = 0;
		EntityQueryEnumerator<HivePylonComponent> pylonQuery = ((EntitySystem)this).EntityQueryEnumerator<HivePylonComponent>();
		EntityUid uid = default(EntityUid);
		HivePylonComponent hivePylonComponent = default(HivePylonComponent);
		while (pylonQuery.MoveNext(ref uid, ref hivePylonComponent))
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(manage.Owner)))
			{
				pylons++;
			}
		}
		if (pylons < boonComp.Pylons)
		{
			string msg2 = base.Loc.GetString("rmc-boon-not-enough-pylons", (ValueTuple<string, object>)("cost", boonComp.Pylons), (ValueTuple<string, object>)("current", pylons));
			_popup.PopupCursor(msg2, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
		}
		else
		{
			if (!TryGetUnlockAt(boons, EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boonProto.ID), out var unlockAt))
			{
				return;
			}
			if (_gameTicker.RoundDuration() < unlockAt)
			{
				string msg3 = base.Loc.GetString("rmc-boon-not-enough-time");
				_popup.PopupCursor(msg3, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
				return;
			}
			if (!HasEnoughAliveMarines())
			{
				string msg4 = base.Loc.GetString("rmc-boon-not-enough-marines");
				_popup.PopupCursor(msg4, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
				return;
			}
			if (boonComp.NoLivingKing)
			{
				EntityQueryEnumerator<HiveBoonKingComponent> kingQuery = ((EntitySystem)this).EntityQueryEnumerator<HiveBoonKingComponent>();
				EntityUid uid2 = default(EntityUid);
				HiveBoonKingComponent hiveBoonKingComponent = default(HiveBoonKingComponent);
				while (kingQuery.MoveNext(ref uid2, ref hiveBoonKingComponent))
				{
					if (!_mobState.IsDead(uid2) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(manage.Owner), Entity<HiveMemberComponent>.op_Implicit(uid2)))
					{
						string msg5 = base.Loc.GetString("rmc-boon-only-one-king");
						_popup.PopupCursor(msg5, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
						return;
					}
				}
			}
			if (boonComp.RequiresCore && !_hive.GetHiveCore(hive2).HasValue)
			{
				string msg6 = base.Loc.GetString("rmc-boon-requires-core");
				_popup.PopupCursor(msg6, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
				return;
			}
			TimeSpan time = _timing.CurTime;
			if (boons.Comp.UsedAt.TryGetValue(EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boonProto.ID), out var usedAt))
			{
				TimeSpan cooldownLeft = usedAt + boonComp.Cooldown - time;
				if (cooldownLeft > TimeSpan.Zero)
				{
					string msg7 = base.Loc.GetString("rmc-boon-on-cooldown", (ValueTuple<string, object>)("boon", boonProto.Name), (ValueTuple<string, object>)("minutes", (int)cooldownLeft.TotalMinutes));
					_popup.PopupCursor(msg7, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
					return;
				}
			}
			EntProtoId<HiveBoonDefinitionComponent>? duplicateId = boonComp.DuplicateId;
			if (duplicateId.HasValue)
			{
				EntProtoId<HiveBoonDefinitionComponent> duplicateId2 = duplicateId.GetValueOrDefault();
				if (boons.Comp.Active.TryGetValue(duplicateId2, out var activeBoonId) && !((EntitySystem)this).TerminatingOrDeleted(activeBoonId, (MetaDataComponent)null))
				{
					string msg8 = base.Loc.GetString("rmc-boon-duplicate-active", (ValueTuple<string, object>)("boon", ((EntitySystem)this).Name(activeBoonId, (MetaDataComponent)null)));
					_popup.PopupCursor(msg8, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
					return;
				}
			}
			if (!boonComp.Reusable && boons.Comp.UsedAt.ContainsKey(EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boonProto.ID)))
			{
				string msg9 = base.Loc.GetString("rmc-boon-not-reusable", (ValueTuple<string, object>)("boon", boonProto.Name));
				_popup.PopupCursor(msg9, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
			}
			else if (boonComp.Event != null)
			{
				HiveBoonEvent ev = (HiveBoonEvent)_serialization.CreateCopy((object)boonComp.Event, (ISerializationContext)null, false, true);
				if (ev != null)
				{
					boons.Comp.UsedAt[EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boonProto.ID)] = time;
					boons.Comp.RoyalResin = Math.Max(0, boons.Comp.RoyalResin - boonComp.Cost);
					EntityUid boonEnt = ((EntitySystem)this).Spawn(boonProto.ID, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
					_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(manage.Owner), Entity<HiveMemberComponent>.op_Implicit(boonEnt));
					((EntitySystem)this).EnsureComp<TimedDespawnComponent>(boonEnt).Lifetime = (float)boonComp.Duration.TotalSeconds;
					boons.Comp.Active[EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boonProto.ID)] = boonEnt;
					((EntitySystem)this).Dirty(Entity<HiveBoonsComponent>.op_Implicit(boons), (IComponent)(object)boons.Comp, (MetaDataComponent)null);
					ev.Boon = boonEnt;
					ev.Hive = hive2;
					ev.Core = _hive.GetHiveCore(hive2);
					((EntitySystem)this).RaiseLocalEvent((object)ev);
				}
			}
		}
	}

	private bool TryGetUnlockAt(Entity<HiveBoonsComponent> boons, EntProtoId<HiveBoonDefinitionComponent> boonId, out TimeSpan unlockAt)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype boon = default(EntityPrototype);
		HiveBoonDefinitionComponent boonComp = default(HiveBoonDefinitionComponent);
		if (!_prototype.TryIndex(EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boonId), ref boon) || !boon.TryGetComponent<HiveBoonDefinitionComponent>(ref boonComp, _compFactory))
		{
			unlockAt = TimeSpan.Zero;
			return false;
		}
		if (boons.Comp.UnlockAt.TryGetValue(boonId, out unlockAt))
		{
			return true;
		}
		unlockAt = boonComp.UnlockAt;
		if (boonComp.UnlockAtRandomAdd != TimeSpan.Zero)
		{
			unlockAt += _random.Next(TimeSpan.Zero, boonComp.UnlockAtRandomAdd);
		}
		boons.Comp.UnlockAt[boonId] = unlockAt;
		((EntitySystem)this).Dirty<HiveBoonsComponent>(boons, (MetaDataComponent)null);
		return true;
	}

	public Entity<HiveBoonsComponent> EnsureBoons(Entity<HiveComponent> hive)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		HiveBoonsComponent boons = ((EntitySystem)this).EnsureComp<HiveBoonsComponent>(Entity<HiveComponent>.op_Implicit(hive));
		return Entity<HiveBoonsComponent>.op_Implicit((Entity<HiveComponent>.op_Implicit(hive), boons));
	}

	private void ReloadPrototypes()
	{
		ImmutableArray<(EntityPrototype, HiveBoonDefinitionComponent)>.Builder boons = ImmutableArray.CreateBuilder<(EntityPrototype, HiveBoonDefinitionComponent)>();
		HiveBoonDefinitionComponent comp = default(HiveBoonDefinitionComponent);
		foreach (EntityPrototype prototype in _prototype.EnumeratePrototypes<EntityPrototype>())
		{
			if (prototype.TryGetComponent<HiveBoonDefinitionComponent>(ref comp, _compFactory))
			{
				boons.Add((prototype, comp));
			}
		}
		boons.Sort(((EntityPrototype Prototype, HiveBoonDefinitionComponent Component) a, (EntityPrototype Prototype, HiveBoonDefinitionComponent Component) b) => string.Compare(a.Prototype.Name, b.Prototype.Name, StringComparison.InvariantCultureIgnoreCase));
		Boons = boons.ToImmutable();
	}

	private void GainResin()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			TimeSpan time = _timing.CurTime;
			EntityQueryEnumerator<HivePylonComponent> pylons = ((EntitySystem)this).EntityQueryEnumerator<HivePylonComponent>();
			EntityUid uid = default(EntityUid);
			HivePylonComponent pylon = default(HivePylonComponent);
			while (pylons.MoveNext(ref uid, ref pylon))
			{
				if (!(pylon.NextRoyalResin >= time))
				{
					pylon.NextRoyalResin = time + _royalResinEvery;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)pylon, (MetaDataComponent)null);
					Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(uid));
					if (hive.HasValue)
					{
						Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
						Entity<HiveBoonsComponent> boons = EnsureBoons(hive2);
						boons.Comp.RoyalResin = Math.Clamp(boons.Comp.RoyalResin + 1, 0, boons.Comp.RoyalResinMax);
					}
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error gaining royal resin:\n{value}");
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		AnnounceKingUnlock();
		GainResin();
		EntityQueryEnumerator<HiveKingCocoonComponent> cocoonQuery = ((EntitySystem)this).EntityQueryEnumerator<HiveKingCocoonComponent>();
		EntityUid cocoonId = default(EntityUid);
		HiveKingCocoonComponent cocoonComp = default(HiveKingCocoonComponent);
		EntityUid pylonId = default(EntityUid);
		HivePylonComponent hivePylonComponent = default(HivePylonComponent);
		EntityUid pylonId2 = default(EntityUid);
		ActorComponent actor = default(ActorComponent);
		while (cocoonQuery.MoveNext(ref cocoonId, ref cocoonComp))
		{
			int pylons = 0;
			EntityQueryEnumerator<HivePylonComponent> pylonQuery = ((EntitySystem)this).EntityQueryEnumerator<HivePylonComponent>();
			while (pylonQuery.MoveNext(ref pylonId, ref hivePylonComponent))
			{
				if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(cocoonId), Entity<HiveMemberComponent>.op_Implicit(pylonId)))
				{
					pylons++;
				}
			}
			if (pylons >= cocoonComp.RequiredPylons)
			{
				if (cocoonComp.LastPylons < cocoonComp.RequiredPylons)
				{
					string areaName = _area.GetAreaName(cocoonId);
					_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-resumed-marine", (ValueTuple<string, object>)("area", areaName)));
					_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(cocoonId), base.Loc.GetString("rmc-boon-king-announcement-resumed-xeno"));
				}
				cocoonComp.LastPylons = pylons;
				pylonQuery = ((EntitySystem)this).EntityQueryEnumerator<HivePylonComponent>();
				while (pylonQuery.MoveNext(ref pylonId2, ref hivePylonComponent))
				{
					if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(cocoonId), Entity<HiveMemberComponent>.op_Implicit(pylonId2)))
					{
						_area.TrySetCanOrbitalBombardRoofing(Entity<RoofingEntityComponent>.op_Implicit(pylonId2), ob: false);
					}
				}
				cocoonComp.TimeLeft -= TimeSpan.FromSeconds(frameTime);
				if (cocoonComp.TimeLeft > _kingVoteStartTime && !HasEnoughAliveMarines())
				{
					cocoonComp.TimeLeft = _kingVoteStartTime;
					cocoonComp.FirstWarning = true;
					((EntitySystem)this).Dirty(cocoonId, (IComponent)(object)cocoonComp, (MetaDataComponent)null);
				}
				if (cocoonComp.TimeLeft <= _kingFirstWarningTime && !cocoonComp.FirstWarning)
				{
					cocoonComp.FirstWarning = true;
					((EntitySystem)this).Dirty(cocoonId, (IComponent)(object)cocoonComp, (MetaDataComponent)null);
					string areaName2 = _area.GetAreaName(cocoonId);
					_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-minutes-marine", (ValueTuple<string, object>)("area", areaName2), (ValueTuple<string, object>)("minutes", (int)cocoonComp.TimeLeft.TotalMinutes + 1)));
					_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(cocoonId), base.Loc.GetString("rmc-boon-king-announcement-minutes-xeno", (ValueTuple<string, object>)("minutes", (int)cocoonComp.TimeLeft.TotalMinutes + 1)));
				}
				if (cocoonComp.TimeLeft > _kingVoteStartTime)
				{
					continue;
				}
				if (!cocoonComp.VoteStarted)
				{
					cocoonComp.VoteStarted = true;
					((EntitySystem)this).Dirty(cocoonId, (IComponent)(object)cocoonComp, (MetaDataComponent)null);
					StartKingVote(Entity<HiveKingCocoonComponent>.op_Implicit((cocoonId, cocoonComp)));
				}
				if (cocoonComp.TimeLeft > _kingVoteStartHatchingTime)
				{
					continue;
				}
				if (!cocoonComp.FinalWarning)
				{
					cocoonComp.FinalWarning = true;
					((EntitySystem)this).Dirty(cocoonId, (IComponent)(object)cocoonComp, (MetaDataComponent)null);
					string areaName3 = _area.GetAreaName(cocoonId);
					_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-seconds-marine", (ValueTuple<string, object>)("area", areaName3), (ValueTuple<string, object>)("seconds", (int)cocoonComp.TimeLeft.TotalSeconds + 1)));
					_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(cocoonId), base.Loc.GetString("rmc-boon-king-announcement-seconds-xeno", (ValueTuple<string, object>)("seconds", (int)cocoonComp.TimeLeft.TotalSeconds + 1)));
				}
				if (cocoonComp.TimeLeft > TimeSpan.Zero)
				{
					continue;
				}
				Entity<HiveKingVoteComponent> vote = EnsureVote(cocoonId);
				List<(NetUserId, int)> votes = new List<(NetUserId, int)>();
				foreach (var (user, userVotes) in vote.Comp.Votes)
				{
					votes.Add((user, userVotes));
				}
				votes = votes.OrderByDescending<(NetUserId, int), int>(((NetUserId Id, int Votes) a) => a.Votes).ToList();
				EntityUid king = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(cocoonComp.Spawn), cocoonId.ToCoordinates(), (ComponentRegistry)null);
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(cocoonId), Entity<HiveMemberComponent>.op_Implicit(king));
				((EntitySystem)this).EnsureComp<HiveConstructionSuppressAnnouncementsComponent>(cocoonId);
				((EntitySystem)this).QueueDel((EntityUid?)cocoonId);
				foreach (var item in votes)
				{
					NetUserId user2 = item.Item1;
					_mind.ControlMob(user2, king);
					if (((EntitySystem)this).TryComp<ActorComponent>(king, ref actor) && actor.PlayerSession.UserId == user2)
					{
						((EntitySystem)this).QueueDel((EntityUid?)Entity<HiveKingVoteComponent>.op_Implicit(vote));
						string areaName4 = _area.GetAreaName(cocoonId);
						_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-hatch-marine", (ValueTuple<string, object>)("area", areaName4)));
						_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(cocoonId), base.Loc.GetString("rmc-boon-king-announcement-hatch-xeno"));
						return;
					}
				}
				continue;
			}
			if (cocoonComp.LastPylons >= cocoonComp.RequiredPylons)
			{
				string areaName5 = _area.GetAreaName(cocoonId);
				_marineAnnounce.AnnounceToMarines(base.Loc.GetString("rmc-boon-king-announcement-paused-marine", (ValueTuple<string, object>)("area", areaName5)));
				_xenoAnnounce.AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent>.op_Implicit(cocoonId), base.Loc.GetString("rmc-boon-king-announcement-paused-xeno"));
			}
			cocoonComp.LastPylons = pylons;
			break;
		}
	}

	private void AnnounceKingUnlock()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (_net.IsClient || !_rmcGameTicker.ServerOnlyIsInRound())
			{
				return;
			}
			EntityQueryEnumerator<HiveComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiveComponent>();
			EntityUid uid = default(EntityUid);
			HiveComponent hive = default(HiveComponent);
			while (query.MoveNext(ref uid, ref hive))
			{
				Entity<HiveBoonsComponent> boons = EnsureBoons(Entity<HiveComponent>.op_Implicit((uid, hive)));
				if (!boons.Comp.KingAnnounced && TryGetUnlockAt(boons, KingBoonId, out var unlockAt) && !(_gameTicker.RoundDuration() < unlockAt))
				{
					boons.Comp.KingAnnounced = true;
					((EntitySystem)this).Dirty<HiveBoonsComponent>(boons, (MetaDataComponent)null);
					SoundSpecifier sound = new BioscanComponent().XenoSound;
					_xenoAnnounce.AnnounceToHive(default(EntityUid), uid, "The hive is now ready to begin hatching His Grace, the King, if we gain control of both tall hivemind towers.", sound);
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error announcing king unlock:\n{value}");
		}
	}
}
