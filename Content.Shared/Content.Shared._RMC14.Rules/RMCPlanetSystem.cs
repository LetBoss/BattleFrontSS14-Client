using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.TacticalMap;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Rules;

public sealed class RMCPlanetSystem : EntitySystem
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private MapLoaderSystem _mapLoader;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCPowerSystem _rmcPower;

	[Dependency]
	private SharedTransformSystem _transform;

	private int _coordinateVariance;

	private float _hijackSongGain;

	private EntityQuery<RMCPlanetComponent> _rmcPlanetQuery;

	public ImmutableDictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>> PlanetPaths { get; private set; } = ImmutableDictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>>.Empty;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_rmcPlanetQuery = ((EntitySystem)this).GetEntityQuery<RMCPlanetComponent>();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPlanetComponent, MapInitEvent>((EntityEventRefHandler<RMCPlanetComponent, MapInitEvent>)OnPlanetMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHijackSongComponent, ComponentStartup>((EntityEventRefHandler<RMCHijackSongComponent, ComponentStartup>)OnHijackSongStartup, (Type[])null, new Type[1] { typeof(SharedAudioSystem) });
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCPlanetCoordinateVariance, (Action<int>)delegate(int v)
		{
			_coordinateVariance = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.VolumeGainHijackSong, (Action<float>)SetVolumeHijack, true);
		ReloadPlanets();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<EntityPrototype>())
		{
			ReloadPlanets();
		}
	}

	private void OnPlanetMapInit(Entity<RMCPlanetComponent> ent, ref MapInitEvent args)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		int x = _random.Next(-_coordinateVariance, _coordinateVariance + 1);
		int y = _random.Next(-_coordinateVariance, _coordinateVariance + 1);
		ent.Comp.Offset = Vector2i.op_Implicit((x, y));
		RMCPlanetAddedEvent ev = default(RMCPlanetAddedEvent);
		((EntitySystem)this).RaiseLocalEvent<RMCPlanetAddedEvent>(Entity<RMCPlanetComponent>.op_Implicit(ent), ref ev, false);
	}

	private void OnHijackSongStartup(Entity<RMCHijackSongComponent> ent, ref ComponentStartup args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		AudioComponent audio = default(AudioComponent);
		if (!_net.IsServer && ((EntitySystem)this).TryComp<AudioComponent>(Entity<RMCHijackSongComponent>.op_Implicit(ent), ref audio))
		{
			((AudioParams)(ref audio.Params)).Volume = SharedAudioSystem.GainToVolume(_hijackSongGain);
		}
	}

	private void SetVolumeHijack(float gain)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			_hijackSongGain = gain;
			AllEntityQueryEnumerator<RMCHijackSongComponent, AudioComponent> query = ((EntitySystem)this).AllEntityQuery<RMCHijackSongComponent, AudioComponent>();
			EntityUid val = default(EntityUid);
			RMCHijackSongComponent rMCHijackSongComponent = default(RMCHijackSongComponent);
			AudioComponent audio = default(AudioComponent);
			while (query.MoveNext(ref val, ref rMCHijackSongComponent, ref audio))
			{
				AudioComponent obj = audio;
				AudioParams val2 = audio.Params;
				((AudioParams)(ref val2)).Volume = SharedAudioSystem.GainToVolume(gain);
				obj.Params = val2;
			}
		}
	}

	public bool IsOnPlanet(EntityCoordinates coordinates)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcPlanetQuery.HasComp(_transform.GetGrid(coordinates)))
		{
			return true;
		}
		if (_rmcPlanetQuery.HasComp(_transform.GetMap(coordinates)))
		{
			return true;
		}
		return false;
	}

	public bool IsOnPlanet(TransformComponent xform)
	{
		if (_rmcPlanetQuery.HasComp(xform.GridUid))
		{
			return true;
		}
		if (_rmcPlanetQuery.HasComp(xform.MapUid))
		{
			return true;
		}
		return false;
	}

	public bool IsOnPlanet(MapCoordinates coordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return IsOnPlanet(_transform.ToCoordinates(coordinates));
	}

	public bool TryGetOffset(MapCoordinates coordinates, out Vector2i offset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates entCoords = _transform.ToCoordinates(coordinates);
		EntityUid? grid = _transform.GetGrid(entCoords);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			RMCPlanetComponent gridPlanet = default(RMCPlanetComponent);
			if (((EntitySystem)this).TryComp<RMCPlanetComponent>(gridId, ref gridPlanet))
			{
				offset = gridPlanet.Offset;
				return true;
			}
		}
		grid = _transform.GetMap(entCoords);
		if (grid.HasValue)
		{
			EntityUid mapId = grid.GetValueOrDefault();
			RMCPlanetComponent mapPlanet = default(RMCPlanetComponent);
			if (((EntitySystem)this).TryComp<RMCPlanetComponent>(mapId, ref mapPlanet))
			{
				offset = mapPlanet.Offset;
				return true;
			}
		}
		offset = default(Vector2i);
		return false;
	}

	public bool TryPlanetToCoordinates(Vector2i coordinates, out MapCoordinates mapCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		RMCPlanetComponent comp = default(RMCPlanetComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<RMCPlanetComponent>().MoveNext(ref uid, ref comp))
		{
			MapId mapId = _transform.GetMapId(Entity<TransformComponent>.op_Implicit(uid));
			mapCoordinates = new MapCoordinates(Vector2i.op_Implicit(coordinates - comp.Offset), mapId);
			return true;
		}
		mapCoordinates = default(MapCoordinates);
		return false;
	}

	private void ReloadPlanets()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>> planetPaths = new Dictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>>();
		RMCPlanetMapPrototypeComponent planetMapPrototype = default(RMCPlanetMapPrototypeComponent);
		foreach (EntityPrototype entity in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (entity.TryGetComponent<RMCPlanetMapPrototypeComponent>(ref planetMapPrototype, _compFactory))
			{
				planetPaths[((object)Unsafe.As<ResPath, ResPath>(ref planetMapPrototype.Map)/*cast due to constrained. prefix*/).ToString()] = EntProtoId<RMCPlanetMapPrototypeComponent>.op_Implicit(entity.ID);
			}
		}
		PlanetPaths = planetPaths.ToImmutableDictionary();
	}

	public List<RMCPlanet> GetAllPlanets()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		List<RMCPlanet> candidates = new List<RMCPlanet>();
		EntityPrototype planetProto = default(EntityPrototype);
		RMCPlanetMapPrototypeComponent comp = default(RMCPlanetMapPrototypeComponent);
		foreach (EntProtoId<RMCPlanetMapPrototypeComponent> planet in PlanetPaths.Values)
		{
			if (_prototypes.TryIndex(EntProtoId<RMCPlanetMapPrototypeComponent>.op_Implicit(planet), ref planetProto) && planet.TryGet(ref comp, _prototypes, _compFactory))
			{
				candidates.Add(new RMCPlanet(planetProto, comp));
			}
		}
		return candidates;
	}

	public List<RMCPlanet> GetAllPlanetsInRotation()
	{
		return (from p in GetAllPlanets()
			where p.Comp.InRotation
			select p).ToList();
	}

	public List<RMCPlanet> GetCandidatesInRotation()
	{
		List<RMCPlanet> candidates = GetAllPlanetsInRotation();
		int players = _player.PlayerCount;
		if (players == 0)
		{
			return candidates;
		}
		for (int i = candidates.Count - 1; i >= 0; i--)
		{
			RMCPlanetMapPrototypeComponent comp = candidates[i].Comp;
			if ((comp.MinPlayers != 0 && players < comp.MinPlayers) || (comp.MaxPlayers != 0 && players > comp.MaxPlayers))
			{
				candidates.RemoveAt(i);
			}
		}
		return candidates;
	}

	public MapId? Load(ResPath path)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		DeserializationOptions val = default(DeserializationOptions);
		((DeserializationOptions)(ref val))._002Ector();
		val.InitializeMaps = true;
		DeserializationOptions options = val;
		Entity<MapComponent>? map = default(Entity<MapComponent>?);
		HashSet<Entity<MapGridComponent>> hashSet = default(HashSet<Entity<MapGridComponent>>);
		if (!_mapLoader.TryLoadMap(path, ref map, ref hashSet, (DeserializationOptions?)options, default(Vector2), default(Angle)))
		{
			return null;
		}
		Entity<RMCPlanetComponent>[] array = base.EntityManager.AllEntities<RMCPlanetComponent>();
		foreach (Entity<RMCPlanetComponent> entity in array)
		{
			((EntitySystem)this).RemComp<RMCPlanetComponent>(Entity<RMCPlanetComponent>.op_Implicit(entity));
		}
		Entity<TacticalMapComponent>[] array2 = base.EntityManager.AllEntities<TacticalMapComponent>();
		foreach (Entity<TacticalMapComponent> entity2 in array2)
		{
			((EntitySystem)this).RemComp<TacticalMapComponent>(Entity<TacticalMapComponent>.op_Implicit(entity2));
		}
		((EntitySystem)this).EnsureComp<RMCPlanetComponent>(Entity<MapComponent>.op_Implicit(map.Value));
		((EntitySystem)this).EnsureComp<TacticalMapComponent>(Entity<MapComponent>.op_Implicit(map.Value));
		_rmcPower.RecalculatePower();
		return map.Value.Comp.MapId;
	}
}
