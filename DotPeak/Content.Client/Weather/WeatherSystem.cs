// Decompiled with JetBrains decompiler
// Type: Content.Client.Weather.WeatherSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
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
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WeatherComponent, ComponentHandleState>(new ComponentEventRefHandler<WeatherComponent, ComponentHandleState>((object) this, __methodptr(OnWeatherHandleState)), (Type[]) null, (Type[]) null);
  }

  protected override void Run(
    EntityUid uid,
    WeatherData weather,
    WeatherPrototype weatherProto,
    float frameTime)
  {
    base.Run(uid, weather, weatherProto, frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid? mapUid = this.Transform(uid).MapUid;
    TransformComponent transformComponent = this.Transform(localEntity.Value);
    if (mapUid.HasValue)
    {
      EntityUid? nullable1 = transformComponent.MapUid;
      EntityUid? nullable2 = mapUid;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (EntityUid.op_Inequality(nullable1.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 0) : 1) == 0)
      {
        if (!this.Timing.IsFirstTimePredicted || weatherProto.Sound == null)
          return;
        WeatherData weatherData1 = weather;
        nullable2 = weatherData1.Stream;
        if (!nullable2.HasValue)
        {
          WeatherData weatherData2 = weatherData1;
          (EntityUid, AudioComponent)? nullable3 = ((SharedAudioSystem) this._audio).PlayGlobal(weatherProto.Sound, Filter.Local(), true, new AudioParams?());
          ref (EntityUid, AudioComponent)? local = ref nullable3;
          EntityUid? nullable4;
          if (!local.HasValue)
          {
            nullable1 = new EntityUid?();
            nullable4 = nullable1;
          }
          else
            nullable4 = new EntityUid?(local.GetValueOrDefault().Item1);
          weatherData2.Stream = nullable4;
        }
        AudioComponent audioComponent;
        if (!this.TryComp<AudioComponent>(weather.Stream, ref audioComponent))
          return;
        float num1 = 0.0f;
        MapGridComponent grid;
        if (this.TryComp<MapGridComponent>(transformComponent.GridUid, ref grid))
        {
          RoofComponent roofComp;
          this.TryComp<RoofComponent>(transformComponent.GridUid, ref roofComp);
          nullable2 = transformComponent.GridUid;
          EntityUid entityUid = nullable2.Value;
          TileRef tileRef = ((SharedMapSystem) this._mapSystem).GetTileRef(entityUid, grid, transformComponent.Coordinates);
          Queue<TileRef> tileRefQueue = new Queue<TileRef>();
          tileRefQueue.Enqueue(tileRef);
          EntityCoordinates? nullable5 = new EntityCoordinates?();
          HashSet<Vector2i> vector2iSet = new HashSet<Vector2i>();
          TileRef result;
          while (tileRefQueue.TryDequeue(out result))
          {
            if (vector2iSet.Add(result.GridIndices))
            {
              nullable2 = transformComponent.GridUid;
              if (!this.CanWeatherAffect(nullable2.Value, grid, result, roofComp))
              {
                for (int x = -1; x <= 1; ++x)
                {
                  for (int y = -1; y <= 1; ++y)
                  {
                    if ((Math.Abs(x) != 1 || Math.Abs(y) != 1) && (x != 0 || y != 0) && (double) (new Vector2((float) x, (float) y) + Vector2i.op_Implicit(result.GridIndices) - Vector2i.op_Implicit(tileRef.GridIndices)).Length() <= 3.0)
                      tileRefQueue.Enqueue(((SharedMapSystem) this._mapSystem).GetTileRef(entityUid, grid, Vector2i.op_Addition(new Vector2i(x, y), result.GridIndices)));
                  }
                }
              }
              else
              {
                ref EntityCoordinates? local = ref nullable5;
                nullable2 = transformComponent.GridUid;
                EntityCoordinates entityCoordinates = new EntityCoordinates(nullable2.Value, Vector2i.op_Implicit(result.GridIndices) + grid.TileSizeHalfVector);
                local = new EntityCoordinates?(entityCoordinates);
                break;
              }
            }
          }
          if (nullable5.HasValue)
          {
            MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(transformComponent);
            Vector2 vector2_1 = this._transform.ToMapCoordinates(nullable5.Value, true).Position - mapCoordinates1.Position;
            float num2 = vector2_1.Length();
            AudioSystem audio = this._audio;
            MapCoordinates mapCoordinates2 = mapCoordinates1;
            Vector2 vector2_2 = vector2_1;
            double num3 = (double) num2;
            nullable2 = new EntityUid?();
            EntityUid? nullable6 = nullable2;
            num1 = audio.GetOcclusion(mapCoordinates2, vector2_2, (float) num3, nullable6);
          }
          else
            num1 = 3f;
        }
        double percent = (double) this.GetPercent(weather, uid);
        AudioParams audioParams = weatherProto.Sound.Params;
        double gain = (double) SharedAudioSystem.VolumeToGain(((AudioParams) ref audioParams).Volume);
        float num4 = (float) (percent * gain);
        ((SharedAudioSystem) this._audio).SetGain(weather.Stream, num4, audioComponent);
        audioComponent.Occlusion = num1;
        return;
      }
    }
    weather.Stream = ((SharedAudioSystem) this._audio).Stop(weather.Stream, (AudioComponent) null);
  }

  protected override bool SetState(
    EntityUid uid,
    WeatherState state,
    WeatherComponent comp,
    WeatherData weather,
    WeatherPrototype weatherProto)
  {
    if (!base.SetState(uid, state, comp, weather, weatherProto))
      return false;
    if (!this.Timing.IsFirstTimePredicted)
      return true;
    weather.Stream = ((SharedAudioSystem) this._audio).Stop(weather.Stream, (AudioComponent) null);
    WeatherData weatherData = weather;
    (EntityUid, AudioComponent)? nullable1 = ((SharedAudioSystem) this._audio).PlayGlobal(weatherProto.Sound, Filter.Local(), true, new AudioParams?());
    ref (EntityUid, AudioComponent)? local = ref nullable1;
    EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
    weatherData.Stream = nullable2;
    return true;
  }

  private void OnWeatherHandleState(
    EntityUid uid,
    WeatherComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is SharedWeatherSystem.WeatherComponentState current))
      return;
    foreach ((ProtoId<WeatherPrototype> key3, WeatherData weatherData4) in component.Weather)
    {
      ProtoId<WeatherPrototype> key2 = key3;
      WeatherData weatherData2 = weatherData4;
      WeatherData weatherData3;
      if (!current.Weather.TryGetValue(key2, out weatherData3))
      {
        this.EndWeather(uid, component, ProtoId<WeatherPrototype>.op_Implicit(key2));
      }
      else
      {
        weatherData2.StartTime = weatherData3.StartTime;
        weatherData2.EndTime = weatherData3.EndTime;
        weatherData2.State = weatherData3.State;
      }
    }
    foreach ((key3, weatherData4) in current.Weather)
    {
      ProtoId<WeatherPrototype> key4 = key3;
      WeatherData weatherData5 = weatherData4;
      if (!component.Weather.ContainsKey(key4))
        this.StartWeather(uid, component, this.ProtoMan.Index<WeatherPrototype>(key4), weatherData5.EndTime);
    }
  }
}
