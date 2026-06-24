using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Light.Components;
using Content.Shared.Weather;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Weather;

public sealed class WeatherSystem : SharedWeatherSystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private AudioSystem _audio;

	[Dependency]
	private MapSystem _mapSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WeatherComponent, ComponentHandleState>((ComponentEventRefHandler<WeatherComponent, ComponentHandleState>)OnWeatherHandleState, (Type[])null, (Type[])null);
	}

	protected override void Run(EntityUid uid, WeatherData weather, WeatherPrototype weatherProto, float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		base.Run(uid, weather, weatherProto, frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid? mapUid = ((EntitySystem)this).Transform(uid).MapUid;
		TransformComponent val = ((EntitySystem)this).Transform(localEntity.Value);
		if (mapUid.HasValue)
		{
			EntityUid? mapUid2 = val.MapUid;
			EntityUid? val2 = mapUid;
			if (mapUid2.HasValue == val2.HasValue && (!mapUid2.HasValue || !(mapUid2.GetValueOrDefault() != val2.GetValueOrDefault())))
			{
				if (!Timing.IsFirstTimePredicted || weatherProto.Sound == null)
				{
					return;
				}
				val2 = weather.Stream;
				if (!val2.HasValue)
				{
					weather.Stream = ((SharedAudioSystem)_audio).PlayGlobal(weatherProto.Sound, Filter.Local(), true, (AudioParams?)null)?.Item1;
				}
				AudioComponent val3 = default(AudioComponent);
				if (!((EntitySystem)this).TryComp<AudioComponent>(weather.Stream, ref val3))
				{
					return;
				}
				float occlusion = 0f;
				MapGridComponent val4 = default(MapGridComponent);
				if (((EntitySystem)this).TryComp<MapGridComponent>(val.GridUid, ref val4))
				{
					RoofComponent roofComp = default(RoofComponent);
					((EntitySystem)this).TryComp<RoofComponent>(val.GridUid, ref roofComp);
					EntityUid value = val.GridUid.Value;
					TileRef tileRef = ((SharedMapSystem)_mapSystem).GetTileRef(value, val4, val.Coordinates);
					Queue<TileRef> queue = new Queue<TileRef>();
					queue.Enqueue(tileRef);
					EntityCoordinates? val5 = null;
					HashSet<Vector2i> hashSet = new HashSet<Vector2i>();
					TileRef result;
					while (queue.TryDequeue(out result))
					{
						if (!hashSet.Add(result.GridIndices))
						{
							continue;
						}
						if (!CanWeatherAffect(val.GridUid.Value, val4, result, roofComp))
						{
							for (int i = -1; i <= 1; i++)
							{
								for (int j = -1; j <= 1; j++)
								{
									if ((Math.Abs(i) != 1 || Math.Abs(j) != 1) && (i != 0 || j != 0) && !((new Vector2(i, j) + Vector2i.op_Implicit(result.GridIndices) - Vector2i.op_Implicit(tileRef.GridIndices)).Length() > 3f))
									{
										queue.Enqueue(((SharedMapSystem)_mapSystem).GetTileRef(value, val4, new Vector2i(i, j) + result.GridIndices));
									}
								}
							}
							continue;
						}
						val5 = new EntityCoordinates(val.GridUid.Value, Vector2i.op_Implicit(result.GridIndices) + val4.TileSizeHalfVector);
						break;
					}
					if (val5.HasValue)
					{
						MapCoordinates mapCoordinates = _transform.GetMapCoordinates(val);
						Vector2 vector = _transform.ToMapCoordinates(val5.Value, true).Position - mapCoordinates.Position;
						float num = vector.Length();
						occlusion = _audio.GetOcclusion(mapCoordinates, vector, num, (EntityUid?)null);
					}
					else
					{
						occlusion = 3f;
					}
				}
				float percent = GetPercent(weather, uid);
				float num2 = percent;
				AudioParams val6 = weatherProto.Sound.Params;
				percent = num2 * SharedAudioSystem.VolumeToGain(((AudioParams)(ref val6)).Volume);
				((SharedAudioSystem)_audio).SetGain(weather.Stream, percent, val3);
				val3.Occlusion = occlusion;
				return;
			}
		}
		weather.Stream = ((SharedAudioSystem)_audio).Stop(weather.Stream, (AudioComponent)null);
	}

	protected override bool SetState(EntityUid uid, WeatherState state, WeatherComponent comp, WeatherData weather, WeatherPrototype weatherProto)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!base.SetState(uid, state, comp, weather, weatherProto))
		{
			return false;
		}
		if (!Timing.IsFirstTimePredicted)
		{
			return true;
		}
		weather.Stream = ((SharedAudioSystem)_audio).Stop(weather.Stream, (AudioComponent)null);
		weather.Stream = ((SharedAudioSystem)_audio).PlayGlobal(weatherProto.Sound, Filter.Local(), true, (AudioParams?)null)?.Item1;
		return true;
	}

	private void OnWeatherHandleState(EntityUid uid, WeatherComponent component, ref ComponentHandleState args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is WeatherComponentState weatherComponentState))
		{
			return;
		}
		ProtoId<WeatherPrototype> key;
		WeatherData value;
		foreach (KeyValuePair<ProtoId<WeatherPrototype>, WeatherData> item in component.Weather)
		{
			item.Deconstruct(out key, out value);
			ProtoId<WeatherPrototype> val = key;
			WeatherData weatherData = value;
			if (!weatherComponentState.Weather.TryGetValue(val, out WeatherData value2))
			{
				EndWeather(uid, component, ProtoId<WeatherPrototype>.op_Implicit(val));
				continue;
			}
			weatherData.StartTime = value2.StartTime;
			weatherData.EndTime = value2.EndTime;
			weatherData.State = value2.State;
		}
		foreach (KeyValuePair<ProtoId<WeatherPrototype>, WeatherData> item2 in weatherComponentState.Weather)
		{
			item2.Deconstruct(out key, out value);
			ProtoId<WeatherPrototype> val2 = key;
			WeatherData weatherData2 = value;
			if (!component.Weather.ContainsKey(val2))
			{
				StartWeather(uid, component, ProtoMan.Index<WeatherPrototype>(val2), weatherData2.EndTime);
			}
		}
	}
}
