using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Weather;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Maps;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Weather;

public abstract class SharedWeatherSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class WeatherComponentState : ComponentState
	{
		public Dictionary<ProtoId<WeatherPrototype>, WeatherData> Weather;

		public WeatherComponentState(Dictionary<ProtoId<WeatherPrototype>, WeatherData> weather)
		{
			Weather = weather;
		}
	}

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	protected IPrototypeManager ProtoMan;

	[Dependency]
	private ITileDefinitionManager _tileDefManager;

	[Dependency]
	private MetaDataSystem _metadata;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedRoofSystem _roof;

	[Dependency]
	private RMCWeatherSystem _rmcWeather;

	private EntityQuery<BlockWeatherComponent> _blockQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_blockQuery = ((EntitySystem)this).GetEntityQuery<BlockWeatherComponent>();
		((EntitySystem)this).SubscribeLocalEvent<WeatherComponent, EntityUnpausedEvent>((ComponentEventRefHandler<WeatherComponent, EntityUnpausedEvent>)OnWeatherUnpaused, (Type[])null, (Type[])null);
	}

	private void OnWeatherUnpaused(EntityUid uid, WeatherComponent component, ref EntityUnpausedEvent args)
	{
		foreach (WeatherData weather in component.Weather.Values)
		{
			weather.StartTime += args.PausedTime;
			if (weather.EndTime.HasValue)
			{
				weather.EndTime = weather.EndTime.Value + args.PausedTime;
			}
		}
	}

	public bool CanWeatherAffect(EntityUid uid, MapGridComponent grid, TileRef tileRef, RoofComponent? roofComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		AreaGridComponent areaGridComponent = default(AreaGridComponent);
		if (((EntitySystem)this).TryComp<AreaGridComponent>(uid, ref areaGridComponent))
		{
			return _rmcWeather.CanWeatherAffectArea(uid, grid, tileRef, roofComp);
		}
		if (((Tile)(ref tileRef.Tile)).IsEmpty)
		{
			return true;
		}
		if (((EntitySystem)this).Resolve<RoofComponent>(uid, ref roofComp, false) && _roof.IsRooved(Entity<MapGridComponent, RoofComponent>.op_Implicit((uid, grid, roofComp)), tileRef.GridIndices))
		{
			return false;
		}
		if (!((ContentTileDefinition)(object)_tileDefManager[tileRef.Tile.TypeId]).Weather)
		{
			return false;
		}
		AnchoredEntitiesEnumerator anchoredEntities = _mapSystem.GetAnchoredEntitiesEnumerator(uid, grid, tileRef.GridIndices);
		EntityUid? ent = default(EntityUid?);
		BlockWeatherComponent block = default(BlockWeatherComponent);
		while (((AnchoredEntitiesEnumerator)(ref anchoredEntities)).MoveNext(ref ent))
		{
			if (_blockQuery.TryGetComponent(ent.Value, ref block) && block.Enabled)
			{
				return false;
			}
		}
		return true;
	}

	public float GetPercent(WeatherData component, EntityUid mapUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan pauseTime = _metadata.GetPauseTime(mapUid, (MetaDataComponent)null);
		TimeSpan elapsed = Timing.CurTime - (component.StartTime + pauseTime);
		TimeSpan remaining = component.Duration - elapsed;
		if (remaining < WeatherComponent.ShutdownTime)
		{
			return (float)(remaining / WeatherComponent.ShutdownTime);
		}
		if (elapsed < WeatherComponent.StartupTime)
		{
			return (float)(elapsed / WeatherComponent.StartupTime);
		}
		return 1f;
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!Timing.IsFirstTimePredicted)
		{
			return;
		}
		TimeSpan curTime = Timing.CurTime;
		EntityQueryEnumerator<WeatherComponent> query = ((EntitySystem)this).EntityQueryEnumerator<WeatherComponent>();
		EntityUid uid = default(EntityUid);
		WeatherComponent comp = default(WeatherComponent);
		WeatherPrototype weatherProto = default(WeatherPrototype);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.Weather.Count == 0)
			{
				continue;
			}
			foreach (KeyValuePair<ProtoId<WeatherPrototype>, WeatherData> item in comp.Weather)
			{
				item.Deconstruct(out var key, out var value);
				ProtoId<WeatherPrototype> proto = key;
				WeatherData weather = value;
				TimeSpan? endTime = weather.EndTime;
				if (endTime.HasValue && endTime < curTime)
				{
					EndWeather(uid, comp, ProtoId<WeatherPrototype>.op_Implicit(proto));
					continue;
				}
				TimeSpan? remainingTime = endTime - curTime;
				if (!ProtoMan.TryIndex<WeatherPrototype>(proto, ref weatherProto))
				{
					((EntitySystem)this).Log.Error($"Unable to find weather prototype for {comp.Weather}, ending!");
					EndWeather(uid, comp, ProtoId<WeatherPrototype>.op_Implicit(proto));
					continue;
				}
				if (endTime.HasValue && remainingTime < WeatherComponent.ShutdownTime)
				{
					SetState(uid, WeatherState.Ending, comp, weather, weatherProto);
				}
				else
				{
					TimeSpan startTime = weather.StartTime;
					if (Timing.CurTime - startTime < WeatherComponent.StartupTime)
					{
						SetState(uid, WeatherState.Starting, comp, weather, weatherProto);
					}
				}
				Run(uid, weather, weatherProto, frameTime);
			}
		}
	}

	public void SetWeather(MapId mapId, WeatherPrototype? proto, TimeSpan? endTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? mapUid = default(EntityUid?);
		if (!_mapSystem.TryGetMap((MapId?)mapId, ref mapUid))
		{
			return;
		}
		WeatherComponent weatherComp = ((EntitySystem)this).EnsureComp<WeatherComponent>(mapUid.Value);
		foreach (var (eProto, weather) in weatherComp.Weather)
		{
			if (proto == null)
			{
				TimeSpan valueOrDefault = endTime.GetValueOrDefault();
				if (!endTime.HasValue)
				{
					valueOrDefault = Timing.CurTime + WeatherComponent.ShutdownTime;
					endTime = valueOrDefault;
				}
			}
			if (proto != null && eProto == ProtoId<WeatherPrototype>.op_Implicit(proto.ID))
			{
				weather.EndTime = endTime;
				if (weather.State == WeatherState.Ending)
				{
					weather.State = WeatherState.Running;
				}
				((EntitySystem)this).Dirty(mapUid.Value, (IComponent)(object)weatherComp, (MetaDataComponent)null);
			}
			else
			{
				TimeSpan end = Timing.CurTime + WeatherComponent.ShutdownTime;
				if (!weather.EndTime.HasValue || weather.EndTime > end)
				{
					weather.EndTime = end;
					((EntitySystem)this).Dirty(mapUid.Value, (IComponent)(object)weatherComp, (MetaDataComponent)null);
				}
			}
		}
		if (proto != null)
		{
			StartWeather(mapUid.Value, weatherComp, proto, endTime);
		}
	}

	protected virtual void Run(EntityUid uid, WeatherData weather, WeatherPrototype weatherProto, float frameTime)
	{
	}

	protected void StartWeather(EntityUid uid, WeatherComponent component, WeatherPrototype weather, TimeSpan? endTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Weather.ContainsKey(ProtoId<WeatherPrototype>.op_Implicit(weather.ID)))
		{
			WeatherData data = new WeatherData
			{
				StartTime = Timing.CurTime,
				EndTime = endTime
			};
			component.Weather.Add(ProtoId<WeatherPrototype>.op_Implicit(weather.ID), data);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	protected virtual void EndWeather(EntityUid uid, WeatherComponent component, string proto)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (component.Weather.TryGetValue(ProtoId<WeatherPrototype>.op_Implicit(proto), out WeatherData data))
		{
			_audio.Stop(data.Stream, (AudioComponent)null);
			data.Stream = null;
			component.Weather.Remove(ProtoId<WeatherPrototype>.op_Implicit(proto));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	protected virtual bool SetState(EntityUid uid, WeatherState state, WeatherComponent component, WeatherData weather, WeatherPrototype weatherProto)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (weather.State.Equals(state))
		{
			return false;
		}
		weather.State = state;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		return true;
	}
}
