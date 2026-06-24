using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Thunderdome;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Bioscan;

public sealed class BioscanSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedXenoAnnounceSystem _xenoAnnounce;

	private const string None = "none";

	private TimeSpan _bioscanInitialDelay;

	private TimeSpan _bioscanCheckDelay;

	private TimeSpan _bioscanMinimumCooldown;

	private TimeSpan _bioscanBaseCooldown;

	private int _bioscanVariance;

	private readonly List<MapId> _planetMaps = new List<MapId>();

	private readonly List<MapId> _warshipMaps = new List<MapId>();

	private readonly List<string> _planetAreas = new List<string>();

	private readonly List<string> _warshipAreas = new List<string>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BioscanComponent, MapInitEvent>((EntityEventRefHandler<BioscanComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBioscanInitialDelaySeconds, (Action<int>)delegate(int v)
		{
			_bioscanInitialDelay = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBioscanCheckDelaySeconds, (Action<int>)delegate(int v)
		{
			_bioscanCheckDelay = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBioscanMinimumCooldownSeconds, (Action<int>)delegate(int v)
		{
			_bioscanMinimumCooldown = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBioscanBaseCooldownSeconds, (Action<int>)delegate(int v)
		{
			_bioscanBaseCooldown = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBioscanVariance, (Action<int>)delegate(int v)
		{
			_bioscanVariance = v;
		}, true);
	}

	private void OnMapInit(Entity<BioscanComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.LastMarine = _timing.CurTime + _bioscanInitialDelay;
		ent.Comp.LastXeno = _timing.CurTime + _bioscanInitialDelay;
		((EntitySystem)this).Dirty<BioscanComponent>(ent, (MetaDataComponent)null);
	}

	private bool TryBioscan<T>(TimeSpan last, bool force, ref int max, out int alive, out int aliveShip, out int alivePlanet, out string warshipArea, out string planetArea) where T : IComponent
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		alive = 0;
		aliveShip = 0;
		alivePlanet = 0;
		warshipArea = "none";
		planetArea = "none";
		TimeSpan time = _timing.CurTime;
		if (!force && last + _bioscanMinimumCooldown > time)
		{
			return false;
		}
		_planetMaps.Clear();
		_warshipMaps.Clear();
		_planetAreas.Clear();
		_warshipAreas.Clear();
		EntityQueryEnumerator<RMCPlanetComponent, TransformComponent> planetQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCPlanetComponent, TransformComponent>();
		RMCPlanetComponent rMCPlanetComponent = default(RMCPlanetComponent);
		TransformComponent xform = default(TransformComponent);
		while (planetQuery.MoveNext(ref rMCPlanetComponent, ref xform))
		{
			_planetMaps.Add(xform.MapID);
		}
		EntityQueryEnumerator<AlmayerComponent, TransformComponent> warshipQuery = ((EntitySystem)this).EntityQueryEnumerator<AlmayerComponent, TransformComponent>();
		AlmayerComponent almayerComponent = default(AlmayerComponent);
		TransformComponent xform2 = default(TransformComponent);
		while (warshipQuery.MoveNext(ref almayerComponent, ref xform2))
		{
			_warshipMaps.Add(xform2.MapID);
		}
		alive = 0;
		aliveShip = 0;
		alivePlanet = 0;
		EntityQueryEnumerator<T, ActorComponent, MobStateComponent, TransformComponent> playersQuery = ((EntitySystem)this).EntityQueryEnumerator<T, ActorComponent, MobStateComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		T val = default(T);
		ActorComponent val2 = default(ActorComponent);
		MobStateComponent mobState = default(MobStateComponent);
		TransformComponent xform3 = default(TransformComponent);
		while (playersQuery.MoveNext(ref uid, ref val, ref val2, ref mobState, ref xform3))
		{
			if (!_mobState.IsAlive(uid, mobState) || ((EntitySystem)this).HasComp<ThunderdomeMapComponent>(xform3.MapUid))
			{
				continue;
			}
			alive++;
			string name;
			bool bioscanBlocked = _area.BioscanBlocked(uid, out name);
			MapId mapId = xform3.MapID;
			if (_warshipMaps.Contains(mapId))
			{
				if (!bioscanBlocked)
				{
					aliveShip++;
					if (name != null)
					{
						_warshipAreas.Add(name);
					}
				}
			}
			else if (_planetMaps.Contains(mapId))
			{
				alivePlanet++;
				if (!bioscanBlocked && name != null)
				{
					_planetAreas.Add(name);
				}
			}
		}
		if (alive > max)
		{
			max = alive;
		}
		if (max != 0)
		{
			TimeSpan next = _bioscanBaseCooldown * alive / max;
			if (next < _bioscanMinimumCooldown)
			{
				next = _bioscanMinimumCooldown;
			}
			next += last;
			if (!force && time < next)
			{
				return false;
			}
		}
		else if (!force)
		{
			return false;
		}
		if (_warshipAreas.Count > 0)
		{
			warshipArea = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)_warshipAreas);
		}
		if (_planetAreas.Count > 0)
		{
			planetArea = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)_planetAreas);
		}
		return true;
	}

	public void TryBioscanARES(Entity<BioscanComponent> bioscan, bool force)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (TryBioscan<XenoComponent>(bioscan.Comp.LastMarine, force, ref bioscan.Comp.MaxXenoAlive, out int _, out int aliveShip, out int alivePlanet, out string warshipArea, out string planetArea))
		{
			int variance = _bioscanVariance;
			alivePlanet = Math.Max(0, alivePlanet + _random.Next(-variance, variance + 1));
			if (alivePlanet == 0)
			{
				planetArea = "none";
			}
			bioscan.Comp.LastMarine = time;
			string message = base.Loc.GetString("rmc-bioscan-ares", new(string, object)[4]
			{
				("shipUncontained", aliveShip),
				("shipLocation", warshipArea),
				("planetLocation", planetArea),
				("onPlanet", alivePlanet)
			});
			_marineAnnounce.AnnounceARESStaging(null, message, bioscan.Comp.MarineSound, LocId.op_Implicit("rmc-bioscan-ares-announcement"));
			((EntitySystem)this).Dirty<BioscanComponent>(bioscan, (MetaDataComponent)null);
		}
	}

	public void TryBioscanQueenMother(Entity<BioscanComponent> bioscan, bool force)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (TryBioscan<MarineComponent>(bioscan.Comp.LastXeno, force, ref bioscan.Comp.MaxMarinesAlive, out int _, out int aliveShip, out int alivePlanet, out string warshipArea, out string planetArea))
		{
			int variance = _bioscanVariance;
			aliveShip = Math.Max(0, aliveShip + _random.Next(-variance, variance + 1));
			if (aliveShip == 0)
			{
				planetArea = "none";
			}
			bioscan.Comp.LastXeno = time;
			string message = base.Loc.GetString("rmc-bioscan-xeno", new(string, object)[4]
			{
				("shipLocation", warshipArea),
				("planetLocation", planetArea),
				("onShip", aliveShip),
				("onPlanet", alivePlanet)
			});
			message = base.Loc.GetString("rmc-bioscan-xeno-announcement", (ValueTuple<string, object>)("message", message));
			_xenoAnnounce.AnnounceAll(default(EntityUid), message, bioscan.Comp.XenoSound);
			((EntitySystem)this).Dirty<BioscanComponent>(bioscan, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<BioscanComponent> query = ((EntitySystem)this).EntityQueryEnumerator<BioscanComponent>();
		EntityUid uid = default(EntityUid);
		BioscanComponent bioscan = default(BioscanComponent);
		while (query.MoveNext(ref uid, ref bioscan))
		{
			if (!(bioscan.NextCheck > time))
			{
				bioscan.NextCheck = time + _bioscanCheckDelay;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)bioscan, (MetaDataComponent)null);
				TryBioscanARES(Entity<BioscanComponent>.op_Implicit((uid, bioscan)), force: false);
				TryBioscanQueenMother(Entity<BioscanComponent>.op_Implicit((uid, bioscan)), force: false);
			}
		}
	}
}
