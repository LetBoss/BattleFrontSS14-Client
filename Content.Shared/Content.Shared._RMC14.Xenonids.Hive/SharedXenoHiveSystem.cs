using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Xenonids.Hive;

public abstract class SharedXenoHiveSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedNightVisionSystem _nightVision;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedXenoAnnounceSystem _xenoAnnounce;

	private EntityQuery<HiveComponent> _query;

	private EntityQuery<HiveMemberComponent> _memberQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_query = ((EntitySystem)this).GetEntityQuery<HiveComponent>();
		_memberQuery = ((EntitySystem)this).GetEntityQuery<HiveMemberComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DropshipHijackStartEvent>((EntityEventRefHandler<DropshipHijackStartEvent>)OnDropshipHijackStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveComponent, MapInitEvent>((EntityEventRefHandler<HiveComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEvolutionGranterComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoEvolutionGranterComponent, MobStateChangedEvent>)OnGranterMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AutoAssignHiveComponent, ComponentStartup>((EntityEventRefHandler<AutoAssignHiveComponent, ComponentStartup>)OnAutoAssignHiveAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveGunComponent, AmmoShotEvent>((EntityEventRefHandler<HiveGunComponent, AmmoShotEvent>)OnHiveGunShot, (Type[])null, (Type[])null);
	}

	private void OnDropshipHijackStart(ref DropshipHijackStartEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveComponent> hives = ((EntitySystem)this).EntityQueryEnumerator<HiveComponent>();
		EntityUid uid = default(EntityUid);
		HiveComponent hive = default(HiveComponent);
		while (hives.MoveNext(ref uid, ref hive))
		{
			if (!hive.HijackSurged)
			{
				hive.HijackSurged = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)hive, (MetaDataComponent)null);
				EntityUid boost = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
				EvolutionOverrideComponent evoOverride = ((EntitySystem)this).EnsureComp<EvolutionOverrideComponent>(boost);
				evoOverride.Amount = 10;
				((EntitySystem)this).Dirty(boost, (IComponent)(object)evoOverride, (MetaDataComponent)null);
				((EntitySystem)this).EnsureComp<TimedDespawnComponent>(boost).Lifetime = 180f;
				break;
			}
		}
	}

	private void OnGranterMobStateChanged(Entity<XenoEvolutionGranterComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			Entity<HiveComponent>? hive = GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
			if (hive.HasValue)
			{
				Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
				hive2.Comp.LastQueenDeath = _timing.CurTime;
				hive2.Comp.CurrentQueen = null;
				hive2.Comp.AnnouncedQueenDeathCooldownOver = false;
				hive2.Comp.NewQueenAt = _timing.CurTime + hive2.Comp.NewQueenCooldown;
				((EntitySystem)this).Dirty<HiveComponent>(hive2, (MetaDataComponent)null);
			}
		}
	}

	private void OnMapInit(Entity<HiveComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AnnouncedUnlocks.Clear();
		ent.Comp.Unlocks.Clear();
		ent.Comp.AnnouncementsLeft.Clear();
		XenoComponent xeno = default(XenoComponent);
		foreach (EntityPrototype prototype in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (prototype.TryGetComponent<XenoComponent>(ref xeno, _compFactory) && !(xeno.UnlockAt == TimeSpan.Zero) && !prototype.HasComponent<XenoHiddenComponent>(_compFactory))
			{
				Extensions.GetOrNew<TimeSpan, List<EntProtoId>>(ent.Comp.Unlocks, xeno.UnlockAt).Add(EntProtoId.op_Implicit(prototype.ID));
				if (!ent.Comp.AnnouncementsLeft.Contains(xeno.UnlockAt))
				{
					ent.Comp.AnnouncementsLeft.Add(xeno.UnlockAt);
				}
			}
		}
		foreach (KeyValuePair<TimeSpan, List<EntProtoId>> unlock2 in ent.Comp.Unlocks)
		{
			unlock2.Value.Sort();
		}
		ent.Comp.AnnouncementsLeft.Sort();
	}

	public Entity<HiveComponent>? GetHive(Entity<HiveMemberComponent?> member)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!_memberQuery.Resolve(Entity<HiveMemberComponent>.op_Implicit(member), ref member.Comp, false))
		{
			return null;
		}
		EntityUid? hive = member.Comp.Hive;
		if (hive.HasValue)
		{
			EntityUid uid = hive.GetValueOrDefault();
			if (!((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
			{
				HiveComponent comp = default(HiveComponent);
				if (!_query.TryComp(uid, ref comp))
				{
					return null;
				}
				return Entity<HiveComponent>.op_Implicit((uid, comp));
			}
		}
		return null;
	}

	public Entity<HiveComponent>? GetHiveByName(string hiveName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiveComponent>();
		EntityUid uid = default(EntityUid);
		HiveComponent hive = default(HiveComponent);
		while (query.MoveNext(ref uid, ref hive))
		{
			if (((EntitySystem)this).MetaData(uid).EntityName == hiveName)
			{
				return Entity<HiveComponent>.op_Implicit((uid, hive));
			}
		}
		return null;
	}

	public bool HasHive(Entity<HiveMemberComponent?> member)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetHive(member).HasValue;
	}

	public void SetHive(Entity<HiveMemberComponent?> member, EntityUid? hive)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		HiveMemberComponent comp = member.Comp ?? ((EntitySystem)this).EnsureComp<HiveMemberComponent>(Entity<HiveMemberComponent>.op_Implicit(member));
		EntityUid? old = comp.Hive;
		EntityUid? val = old;
		EntityUid? val2 = hive;
		if (val.HasValue != val2.HasValue || (val.HasValue && !(val.GetValueOrDefault() == val2.GetValueOrDefault())))
		{
			Entity<HiveComponent>? hiveEnt = null;
			HiveComponent hiveComp = default(HiveComponent);
			if (_query.TryComp(hive, ref hiveComp))
			{
				hiveEnt = Entity<HiveComponent>.op_Implicit((hive.Value, hiveComp));
			}
			else if (hive.HasValue)
			{
				((EntitySystem)this).Log.Error($"Tried to set hive of {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HiveMemberComponent>.op_Implicit(member), (MetaDataComponent)null)} to bad hive entity {((EntitySystem)this).ToPrettyString(hive, (MetaDataComponent)null)}");
				return;
			}
			comp.Hive = hive;
			((EntitySystem)this).Dirty(Entity<HiveMemberComponent>.op_Implicit(member), (IComponent)(object)comp, (MetaDataComponent)null);
			if (((EntitySystem)this).HasComp<XenoEvolutionGranterComponent>(Entity<HiveMemberComponent>.op_Implicit(member)) && hiveEnt.HasValue)
			{
				SetHiveQueen(Entity<HiveMemberComponent>.op_Implicit(member), hiveEnt.Value);
			}
			HiveChangedEvent ev = new HiveChangedEvent(hiveEnt, old);
			((EntitySystem)this).RaiseLocalEvent<HiveChangedEvent>(Entity<HiveMemberComponent>.op_Implicit(member), ref ev, false);
		}
	}

	public void SetSameHive(Entity<HiveMemberComponent?> src, Entity<HiveMemberComponent?> dest)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = GetHive(src);
		if (hive.HasValue)
		{
			Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
			SetHive(dest, Entity<HiveComponent>.op_Implicit(hive2));
		}
	}

	public bool FromSameHive(Entity<HiveMemberComponent?> a, Entity<HiveMemberComponent?> b)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = GetHive(a);
		if (hive.HasValue)
		{
			Entity<HiveComponent> aHive = hive.GetValueOrDefault();
			return IsMember(b, Entity<HiveComponent>.op_Implicit(aHive));
		}
		return false;
	}

	public bool IsMember(Entity<HiveMemberComponent?> member, EntityUid? hive)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (hive.HasValue)
		{
			Entity<HiveComponent>? hive2 = GetHive(member);
			if (hive2.HasValue)
			{
				EntityUid owner = hive2.GetValueOrDefault().Owner;
				EntityUid? val = hive;
				if (!val.HasValue)
				{
					return false;
				}
				return owner == val.GetValueOrDefault();
			}
		}
		return false;
	}

	public bool HasHiveQueen(Entity<HiveComponent> hive)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? currentQueen = hive.Comp.CurrentQueen;
		return currentQueen.HasValue;
	}

	public bool SetHiveQueen(EntityUid queen, Entity<HiveComponent> hive)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		hive.Comp.CurrentQueen = queen;
		((EntitySystem)this).Dirty<HiveComponent>(hive, (MetaDataComponent)null);
		return true;
	}

	public bool HasHiveCore(Entity<HiveComponent> hive)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetHiveCore(hive).HasValue;
	}

	public EntityUid? GetHiveCore(Entity<HiveComponent> hive)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveCoreComponent> hiveCoreQuerry = ((EntitySystem)this).EntityQueryEnumerator<HiveCoreComponent>();
		EntityUid hiveCoreEnt = default(EntityUid);
		HiveCoreComponent hiveCoreComponent = default(HiveCoreComponent);
		while (hiveCoreQuerry.MoveNext(ref hiveCoreEnt, ref hiveCoreComponent))
		{
			Entity<HiveComponent>? hive2 = GetHive(Entity<HiveMemberComponent>.op_Implicit(hiveCoreEnt));
			if (hive2.HasValue && hive2.GetValueOrDefault() == hive)
			{
				return hiveCoreEnt;
			}
		}
		return null;
	}

	public void ResetHiveCoreCooldown(Entity<HiveComponent> hive)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		hive.Comp.NewCoreAt = _timing.CurTime;
		((EntitySystem)this).Dirty<HiveComponent>(hive, (MetaDataComponent)null);
	}

	public bool TryGetStructureLimit(Entity<HiveComponent> hive, EntProtoId structureProtoId, out int limit)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return hive.Comp.HiveStructureSlots.TryGetValue(structureProtoId, out limit);
	}

	public void SetSeeThroughContainers(Entity<HiveComponent?> hive, bool see)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(Entity<HiveComponent>.op_Implicit(hive), ref hive.Comp, false))
		{
			return;
		}
		hive.Comp.SeeThroughContainers = see;
		EntityQueryEnumerator<XenoComponent, HiveMemberComponent, NightVisionComponent> xenos = ((EntitySystem)this).EntityQueryEnumerator<XenoComponent, HiveMemberComponent, NightVisionComponent>();
		EntityUid uid = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		HiveMemberComponent member = default(HiveMemberComponent);
		NightVisionComponent nv = default(NightVisionComponent);
		while (xenos.MoveNext(ref uid, ref xenoComponent, ref member, ref nv))
		{
			EntityUid? hive2 = member.Hive;
			EntityUid val = Entity<HiveComponent>.op_Implicit(hive);
			if (hive2.HasValue && !(hive2.GetValueOrDefault() != val))
			{
				_nightVision.SetSeeThroughContainers(Entity<NightVisionComponent>.op_Implicit((uid, nv)), see);
			}
		}
	}

	public void AnnounceNeedsOvipositorToSameHive(Entity<HiveMemberComponent?> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = GetHive(xeno);
		if (!hive.HasValue)
		{
			return;
		}
		Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
		if (hive2.Comp.GotOvipositorPopup)
		{
			return;
		}
		hive2.Comp.GotOvipositorPopup = true;
		((EntitySystem)this).Dirty<HiveComponent>(hive2, (MetaDataComponent)null);
		string msg = "Enough time has passed, we require the Queen in oviposition for evolution.";
		EntityQueryEnumerator<XenoComponent, HiveMemberComponent, ActorComponent> xenos = ((EntitySystem)this).EntityQueryEnumerator<XenoComponent, HiveMemberComponent, ActorComponent>();
		EntityUid uid = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		HiveMemberComponent member = default(HiveMemberComponent);
		ActorComponent val = default(ActorComponent);
		while (xenos.MoveNext(ref uid, ref xenoComponent, ref member, ref val))
		{
			if (!(uid == xeno.Owner))
			{
				EntityUid? hive3 = member.Hive;
				EntityUid val2 = Entity<HiveComponent>.op_Implicit(hive2);
				if (hive3.HasValue && !(hive3.GetValueOrDefault() != val2))
				{
					_popup.PopupEntity(msg, uid, uid, PopupType.LargeCaution);
				}
			}
		}
		_xenoAnnounce.AnnounceToHive(default(EntityUid), Entity<HiveComponent>.op_Implicit(hive2), msg);
	}

	public bool TryGetTierLimit(Entity<HiveComponent?> hive, int tier, out FixedPoint2 value)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		value = default(FixedPoint2);
		if (!_query.Resolve(Entity<HiveComponent>.op_Implicit(hive), ref hive.Comp, false))
		{
			return false;
		}
		return hive.Comp.TierLimits.TryGetValue(tier, out value);
	}

	public bool TryGetFreeSlots(Entity<HiveComponent?> hive, EntProtoId caste, out int value)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		value = 0;
		if (!_query.Resolve(Entity<HiveComponent>.op_Implicit(hive), ref hive.Comp, false))
		{
			return false;
		}
		return hive.Comp.FreeSlots.TryGetValue(caste, out value);
	}

	public void IncreaseBurrowedLarva(int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveComponent> hives = ((EntitySystem)this).EntityQueryEnumerator<HiveComponent>();
		EntityUid uid = default(EntityUid);
		HiveComponent hive = default(HiveComponent);
		while (hives.MoveNext(ref uid, ref hive))
		{
			IncreaseBurrowedLarva(Entity<HiveComponent>.op_Implicit((uid, hive)), amount);
		}
	}

	public void IncreaseBurrowedLarva(Entity<HiveComponent> hive, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SetHiveBurrowedLarva(hive, hive.Comp.BurrowedLarva + amount);
	}

	private void SetHiveBurrowedLarva(Entity<HiveComponent> hive, int larva)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		hive.Comp.BurrowedLarva = larva;
		((EntitySystem)this).Dirty<HiveComponent>(hive, (MetaDataComponent)null);
		BurrowedLarvaChangedEvent ev = new BurrowedLarvaChangedEvent(larva);
		((EntitySystem)this).RaiseLocalEvent<BurrowedLarvaChangedEvent>(Entity<HiveComponent>.op_Implicit(hive), ref ev, true);
	}

	public bool JoinBurrowedLarva(Entity<HiveComponent> hive, ICommonSession session)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return false;
		}
		if (hive.Comp.BurrowedLarva <= 0)
		{
			return false;
		}
		EntityUid? larva = null;
		if (!TrySpawnAt<HiveCoreComponent>() && !TrySpawnAt<XenoEvolutionGranterComponent>() && !TrySpawnAt<XenoComponent>())
		{
			return false;
		}
		if (!larva.HasValue)
		{
			return false;
		}
		IncreaseBurrowedLarva(hive, -1);
		_xeno.MakeXeno(Entity<XenoComponent>.op_Implicit(larva.Value));
		SetHive(Entity<HiveMemberComponent>.op_Implicit(larva.Value), Entity<HiveComponent>.op_Implicit(hive));
		Entity<MindComponent> newMind = _mind.CreateMind(session.UserId, ((EntitySystem)this).Comp<MetaDataComponent>(larva.Value).EntityName);
		_mind.TransferTo(Entity<MindComponent>.op_Implicit(newMind), larva, ghostCheckOverride: true);
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(34, 2);
		handler.AppendFormatted(session.Name, 0, "player");
		handler.AppendLiteral(" took a burrowed larva from hive ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HiveComponent>.op_Implicit(hive), (MetaDataComponent)null), "hive", "ToPrettyString(hive)");
		handler.AppendLiteral(".");
		adminLog.Add(LogType.RMCBurrowedLarva, ref handler);
		return true;
		bool TrySpawnAt<T>() where T : notnull, Component
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			EntityQueryEnumerator<T, HiveMemberComponent> candidates = ((EntitySystem)this).EntityQueryEnumerator<T, HiveMemberComponent>();
			EntityUid uid = default(EntityUid);
			T val = default(T);
			HiveMemberComponent member = default(HiveMemberComponent);
			while (candidates.MoveNext(ref uid, ref val, ref member))
			{
				EntityUid? hive2 = member.Hive;
				EntityUid val2 = Entity<HiveComponent>.op_Implicit(hive);
				if (hive2.HasValue && !(hive2.GetValueOrDefault() != val2) && !_mobState.IsDead(uid))
				{
					EntityCoordinates position = _transform.GetMoverCoordinates(uid);
					larva = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(hive.Comp.BurrowedLarvaId), position);
					_transform.AttachToGridOrMap(larva.Value, (TransformComponent)null);
					return true;
				}
			}
			return false;
		}
	}

	private void OnAutoAssignHiveAdded(Entity<AutoAssignHiveComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = GetHiveByName(ent.Comp.Hive);
		if (!hive.HasValue)
		{
			((EntitySystem)this).Log.Debug("Tried to auto assign hive to " + ent.Comp.Hive + ", but no such hive was found");
			return;
		}
		Entity<HiveMemberComponent> member = Entity<HiveMemberComponent>.op_Implicit(ent.Owner);
		Entity<HiveComponent>? val = hive;
		SetHive(member, val.HasValue ? new EntityUid?(Entity<HiveComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null));
	}

	private void OnHiveGunShot(Entity<HiveGunComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid bullet in args.FiredProjectiles)
		{
			SetSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(bullet));
		}
	}
}
