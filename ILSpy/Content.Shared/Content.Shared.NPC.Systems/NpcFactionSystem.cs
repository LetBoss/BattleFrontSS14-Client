using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.NPC.Systems;

public sealed class NpcFactionSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedTransformSystem _xform;

	private FrozenDictionary<string, FactionData> _factions = FrozenDictionary<string, FactionData>.Empty;

	private EntityQuery<FactionExceptionComponent> _exceptionQuery;

	private EntityQuery<FactionExceptionTrackerComponent> _trackerQuery;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NpcFactionMemberComponent, ComponentStartup>((EntityEventRefHandler<NpcFactionMemberComponent, ComponentStartup>)OnFactionStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnProtoReload, (Type[])null, (Type[])null);
		InitializeException();
		RefreshFactions();
	}

	private void OnProtoReload(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<NpcFactionPrototype>())
		{
			RefreshFactions();
		}
	}

	private void OnFactionStartup(Entity<NpcFactionMemberComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshFactions(ent);
	}

	private void RefreshFactions(Entity<NpcFactionMemberComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.FriendlyFactions.Clear();
		ent.Comp.HostileFactions.Clear();
		foreach (ProtoId<NpcFactionPrototype> faction in ent.Comp.Factions)
		{
			if (_factions.TryGetValue(ProtoId<NpcFactionPrototype>.op_Implicit(faction), out var factionData))
			{
				ent.Comp.FriendlyFactions.UnionWith(factionData.Friendly);
				ent.Comp.HostileFactions.UnionWith(factionData.Hostile);
			}
		}
		if (ent.Comp.AddFriendlyFactions != null)
		{
			ent.Comp.FriendlyFactions.UnionWith(ent.Comp.AddFriendlyFactions);
		}
		if (ent.Comp.AddHostileFactions != null)
		{
			ent.Comp.HostileFactions.UnionWith(ent.Comp.AddHostileFactions);
		}
	}

	public bool IsMember(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] string faction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		return ent.Comp.Factions.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(faction));
	}

	public bool IsMemberOfAny(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] IEnumerable<ProtoId<NpcFactionPrototype>> factions)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		foreach (ProtoId<NpcFactionPrototype> faction in factions)
		{
			if (ent.Comp.Factions.Contains(faction))
			{
				return true;
			}
		}
		return false;
	}

	public void AddFaction(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] string faction, bool dirty = true)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!_proto.HasIndex<NpcFactionPrototype>(faction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + faction);
			return;
		}
		ref NpcFactionMemberComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent));
		}
		if (ent.Comp.Factions.Add(ProtoId<NpcFactionPrototype>.op_Implicit(faction)) && dirty)
		{
			RefreshFactions(Entity<NpcFactionMemberComponent>.op_Implicit((Entity<NpcFactionMemberComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	public void AddFactions(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] HashSet<ProtoId<NpcFactionPrototype>> factions, bool dirty = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		ref NpcFactionMemberComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent));
		}
		foreach (ProtoId<NpcFactionPrototype> faction in factions)
		{
			if (!_proto.HasIndex<NpcFactionPrototype>(faction))
			{
				((EntitySystem)this).Log.Error($"Unable to find faction {faction}");
			}
			else
			{
				ent.Comp.Factions.Add(faction);
			}
		}
		if (dirty)
		{
			RefreshFactions(Entity<NpcFactionMemberComponent>.op_Implicit((Entity<NpcFactionMemberComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	public void RemoveFaction(Entity<NpcFactionMemberComponent?> ent, [ForbidLiteral] string faction, bool dirty = true)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!_proto.HasIndex<NpcFactionPrototype>(faction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + faction);
		}
		else if (((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent), ref ent.Comp, false) && ent.Comp.Factions.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(faction)) && dirty)
		{
			RefreshFactions(Entity<NpcFactionMemberComponent>.op_Implicit((Entity<NpcFactionMemberComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	public void ClearFactions(Entity<NpcFactionMemberComponent?> ent, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			ent.Comp.Factions.Clear();
			if (dirty)
			{
				RefreshFactions(Entity<NpcFactionMemberComponent>.op_Implicit((Entity<NpcFactionMemberComponent>.op_Implicit(ent), ent.Comp)));
			}
		}
	}

	public IEnumerable<EntityUid> GetNearbyHostiles(Entity<NpcFactionMemberComponent?, FactionExceptionComponent?> ent, float range)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent, FactionExceptionComponent>.op_Implicit(ent), ref ent.Comp1, false))
		{
			return Array.Empty<EntityUid>();
		}
		IEnumerable<EntityUid> hostiles = from target in GetNearbyFactions(Entity<NpcFactionMemberComponent, FactionExceptionComponent>.op_Implicit(ent), range, ent.Comp1.HostileFactions)
			where !IsEntityFriendly(Entity<NpcFactionMemberComponent>.op_Implicit((Entity<NpcFactionMemberComponent, FactionExceptionComponent>.op_Implicit(ent), ent.Comp1)), Entity<NpcFactionMemberComponent>.op_Implicit(target))
			select target;
		if (!((EntitySystem)this).Resolve<FactionExceptionComponent>(Entity<NpcFactionMemberComponent, FactionExceptionComponent>.op_Implicit(ent), ref ent.Comp2, false))
		{
			return hostiles;
		}
		(EntityUid Owner, FactionExceptionComponent Comp2) faction = (Owner: ent.Owner, Comp2: ent.Comp2);
		return from target in hostiles.Union(GetHostiles(Entity<FactionExceptionComponent>.op_Implicit(faction)))
			where !IsIgnored(Entity<FactionExceptionComponent>.op_Implicit(faction), target)
			select target;
	}

	public IEnumerable<EntityUid> GetNearbyFriendlies(Entity<NpcFactionMemberComponent?> ent, float range)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return Array.Empty<EntityUid>();
		}
		return GetNearbyFactions(Entity<NpcFactionMemberComponent>.op_Implicit(ent), range, ent.Comp.FriendlyFactions);
	}

	private IEnumerable<EntityUid> GetNearbyFactions(EntityUid entity, float range, [ForbidLiteral] HashSet<ProtoId<NpcFactionPrototype>> factions)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(entity);
		foreach (Entity<NpcFactionMemberComponent> ent in _lookup.GetEntitiesInRange<NpcFactionMemberComponent>(_xform.GetMapCoordinates(Entity<TransformComponent>.op_Implicit((entity, xform))), range, (LookupFlags)110))
		{
			if (!(ent.Owner == entity) && factions.Overlaps(ent.Comp.Factions))
			{
				yield return ent.Owner;
			}
		}
	}

	public bool IsEntityFriendly(Entity<NpcFactionMemberComponent?> ent, Entity<NpcFactionMemberComponent?> other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(ent), ref ent.Comp, false) || !((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(other), ref other.Comp, false))
		{
			return false;
		}
		if (!ent.Comp.Factions.Overlaps(other.Comp.Factions))
		{
			return ent.Comp.FriendlyFactions.Overlaps(other.Comp.Factions);
		}
		return true;
	}

	public bool IsFactionFriendly([ForbidLiteral] string target, [ForbidLiteral] string with)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (_factions[target].Friendly.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(with)))
		{
			return _factions[with].Friendly.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		}
		return false;
	}

	public bool IsFactionFriendly([ForbidLiteral] string target, Entity<NpcFactionMemberComponent?> with)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(with), ref with.Comp, false))
		{
			return false;
		}
		if (!with.Comp.Factions.All<ProtoId<NpcFactionPrototype>>((ProtoId<NpcFactionPrototype> x) => IsFactionFriendly(target, ProtoId<NpcFactionPrototype>.op_Implicit(x))))
		{
			return with.Comp.FriendlyFactions.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		}
		return true;
	}

	public bool IsFactionHostile([ForbidLiteral] string target, [ForbidLiteral] string with)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (_factions[target].Hostile.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(with)))
		{
			return _factions[with].Hostile.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		}
		return false;
	}

	public bool IsFactionHostile([ForbidLiteral] string target, Entity<NpcFactionMemberComponent?> with)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NpcFactionMemberComponent>(Entity<NpcFactionMemberComponent>.op_Implicit(with), ref with.Comp, false))
		{
			return false;
		}
		if (!with.Comp.Factions.All<ProtoId<NpcFactionPrototype>>((ProtoId<NpcFactionPrototype> x) => IsFactionHostile(target, ProtoId<NpcFactionPrototype>.op_Implicit(x))))
		{
			return with.Comp.HostileFactions.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		}
		return true;
	}

	public bool IsFactionNeutral([ForbidLiteral] string target, [ForbidLiteral] string with)
	{
		if (!IsFactionFriendly(target, with))
		{
			return !IsFactionHostile(target, with);
		}
		return false;
	}

	public void MakeFriendly([ForbidLiteral] string source, [ForbidLiteral] string target)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!_factions.TryGetValue(source, out var sourceFaction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + source);
			return;
		}
		if (!_factions.ContainsKey(target))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + target);
			return;
		}
		sourceFaction.Friendly.Add(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		sourceFaction.Hostile.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		RefreshFactions();
	}

	public void MakeHostile([ForbidLiteral] string source, [ForbidLiteral] string target)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!_factions.TryGetValue(source, out var sourceFaction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + source);
			return;
		}
		if (!_factions.ContainsKey(target))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + target);
			return;
		}
		sourceFaction.Friendly.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		sourceFaction.Hostile.Add(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		RefreshFactions();
	}

	private void RefreshFactions()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		_factions = _proto.EnumeratePrototypes<NpcFactionPrototype>().ToFrozenDictionary((NpcFactionPrototype faction) => faction.ID, (NpcFactionPrototype faction) => new FactionData
		{
			Friendly = faction.Friendly.ToHashSet(),
			Hostile = faction.Hostile.ToHashSet()
		});
		AllEntityQueryEnumerator<NpcFactionMemberComponent> query = ((EntitySystem)this).AllEntityQuery<NpcFactionMemberComponent>();
		EntityUid uid = default(EntityUid);
		NpcFactionMemberComponent comp = default(NpcFactionMemberComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			comp.FriendlyFactions.Clear();
			comp.HostileFactions.Clear();
			RefreshFactions(Entity<NpcFactionMemberComponent>.op_Implicit((uid, comp)));
		}
	}

	public void InitializeException()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_exceptionQuery = ((EntitySystem)this).GetEntityQuery<FactionExceptionComponent>();
		_trackerQuery = ((EntitySystem)this).GetEntityQuery<FactionExceptionTrackerComponent>();
		((EntitySystem)this).SubscribeLocalEvent<FactionExceptionComponent, ComponentShutdown>((EntityEventRefHandler<FactionExceptionComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FactionExceptionTrackerComponent, ComponentShutdown>((EntityEventRefHandler<FactionExceptionTrackerComponent, ComponentShutdown>)OnTrackerShutdown, (Type[])null, (Type[])null);
	}

	private void OnShutdown(Entity<FactionExceptionComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		FactionExceptionTrackerComponent tracker = default(FactionExceptionTrackerComponent);
		foreach (EntityUid uid in ent.Comp.Hostiles)
		{
			if (_trackerQuery.TryGetComponent(uid, ref tracker))
			{
				tracker.Entities.Remove(Entity<FactionExceptionComponent>.op_Implicit(ent));
			}
		}
		FactionExceptionTrackerComponent tracker2 = default(FactionExceptionTrackerComponent);
		foreach (EntityUid uid2 in ent.Comp.Ignored)
		{
			if (_trackerQuery.TryGetComponent(uid2, ref tracker2))
			{
				tracker2.Entities.Remove(Entity<FactionExceptionComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnTrackerShutdown(Entity<FactionExceptionTrackerComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		FactionExceptionComponent exception = default(FactionExceptionComponent);
		foreach (EntityUid uid in ent.Comp.Entities)
		{
			if (_exceptionQuery.TryGetComponent(uid, ref exception))
			{
				exception.Ignored.Remove(Entity<FactionExceptionTrackerComponent>.op_Implicit(ent));
				exception.Hostiles.Remove(Entity<FactionExceptionTrackerComponent>.op_Implicit(ent));
			}
		}
	}

	public bool IsIgnored(Entity<FactionExceptionComponent?> ent, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		return ent.Comp.Ignored.Contains(target);
	}

	public IEnumerable<EntityUid> GetHostiles(Entity<FactionExceptionComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return Array.Empty<EntityUid>();
		}
		return ent.Comp.Hostiles;
	}

	public void IgnoreEntity(Entity<FactionExceptionComponent?> ent, Entity<FactionExceptionTrackerComponent?> target)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ref FactionExceptionComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent));
		}
		ent.Comp.Ignored.Add(Entity<FactionExceptionTrackerComponent>.op_Implicit(target));
		ref FactionExceptionTrackerComponent comp2 = ref target.Comp;
		if (comp2 == null)
		{
			comp2 = ((EntitySystem)this).EnsureComp<FactionExceptionTrackerComponent>(Entity<FactionExceptionTrackerComponent>.op_Implicit(target));
		}
		target.Comp.Entities.Add(Entity<FactionExceptionComponent>.op_Implicit(ent));
	}

	public void IgnoreEntities(Entity<FactionExceptionComponent?> ent, IEnumerable<EntityUid> ignored)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ref FactionExceptionComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent));
		}
		foreach (EntityUid ignore in ignored)
		{
			IgnoreEntity(ent, Entity<FactionExceptionTrackerComponent>.op_Implicit(ignore));
		}
	}

	public void AggroEntity(Entity<FactionExceptionComponent?> ent, Entity<FactionExceptionTrackerComponent?> target)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ref FactionExceptionComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent));
		}
		ent.Comp.Hostiles.Add(Entity<FactionExceptionTrackerComponent>.op_Implicit(target));
		ref FactionExceptionTrackerComponent comp2 = ref target.Comp;
		if (comp2 == null)
		{
			comp2 = ((EntitySystem)this).EnsureComp<FactionExceptionTrackerComponent>(Entity<FactionExceptionTrackerComponent>.op_Implicit(target));
		}
		target.Comp.Entities.Add(Entity<FactionExceptionComponent>.op_Implicit(ent));
	}

	public void DeAggroEntity(Entity<FactionExceptionComponent?> ent, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		FactionExceptionTrackerComponent tracker = default(FactionExceptionTrackerComponent);
		if (((EntitySystem)this).Resolve<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent), ref ent.Comp, false) && ent.Comp.Hostiles.Remove(target) && _trackerQuery.TryGetComponent(target, ref tracker))
		{
			tracker.Entities.Remove(Entity<FactionExceptionComponent>.op_Implicit(ent));
		}
	}

	public void AggroEntities(Entity<FactionExceptionComponent?> ent, IEnumerable<EntityUid> entities)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ref FactionExceptionComponent comp = ref ent.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<FactionExceptionComponent>(Entity<FactionExceptionComponent>.op_Implicit(ent));
		}
		foreach (EntityUid uid in entities)
		{
			AggroEntity(ent, Entity<FactionExceptionTrackerComponent>.op_Implicit(uid));
		}
	}

	public FrozenDictionary<string, FactionData> GetFactions()
	{
		return _factions;
	}

	public void RealMakeNeutral(string source, string target)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!_factions.TryGetValue(source, out var sourceFaction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + source);
			return;
		}
		if (!_factions.ContainsKey(target))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + target);
			return;
		}
		sourceFaction.Friendly.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		sourceFaction.Hostile.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		RealRefreshFactions();
	}

	public void RealMakeFriendly(string source, string target)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!_factions.TryGetValue(source, out var sourceFaction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + source);
			return;
		}
		if (!_factions.ContainsKey(target))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + target);
			return;
		}
		sourceFaction.Friendly.Add(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		sourceFaction.Hostile.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		RealRefreshFactions();
	}

	public void RealMakeHostile(string source, string target)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!_factions.TryGetValue(source, out var sourceFaction))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + source);
			return;
		}
		if (!_factions.ContainsKey(target))
		{
			((EntitySystem)this).Log.Error("Unable to find faction " + target);
			return;
		}
		sourceFaction.Friendly.Remove(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		sourceFaction.Hostile.Add(ProtoId<NpcFactionPrototype>.op_Implicit(target));
		RealRefreshFactions();
	}

	private void RealRefreshFactions()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<NpcFactionMemberComponent> query = ((EntitySystem)this).AllEntityQuery<NpcFactionMemberComponent>();
		EntityUid uid = default(EntityUid);
		NpcFactionMemberComponent comp = default(NpcFactionMemberComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			comp.FriendlyFactions.Clear();
			comp.HostileFactions.Clear();
			RefreshFactions(Entity<NpcFactionMemberComponent>.op_Implicit((uid, comp)));
		}
	}
}
