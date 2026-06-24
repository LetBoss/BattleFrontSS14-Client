// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weather.SharedWeatherSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Weather;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Maps;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weather;

public abstract class SharedWeatherSystem : EntitySystem
{
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
  private Robust.Shared.GameObjects.EntityQuery<BlockWeatherComponent> _blockQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._blockQuery = this.GetEntityQuery<BlockWeatherComponent>();
    this.SubscribeLocalEvent<WeatherComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<WeatherComponent, EntityUnpausedEvent>(this.OnWeatherUnpaused));
  }

  private void OnWeatherUnpaused(
    EntityUid uid,
    WeatherComponent component,
    ref EntityUnpausedEvent args)
  {
    foreach (WeatherData weatherData in component.Weather.Values)
    {
      weatherData.StartTime += args.PausedTime;
      if (weatherData.EndTime.HasValue)
        weatherData.EndTime = new TimeSpan?(weatherData.EndTime.Value + args.PausedTime);
    }
  }

  public bool CanWeatherAffect(
    EntityUid uid,
    MapGridComponent grid,
    TileRef tileRef,
    RoofComponent? roofComp = null)
  {
    if (this.TryComp<AreaGridComponent>(uid, out AreaGridComponent _))
      return this._rmcWeather.CanWeatherAffectArea(uid, grid, tileRef, roofComp);
    if (tileRef.Tile.IsEmpty)
      return true;
    if (this.Resolve<RoofComponent>(uid, ref roofComp, false) && this._roof.IsRooved((Entity<MapGridComponent, RoofComponent>) (uid, grid, roofComp), tileRef.GridIndices) || !((ContentTileDefinition) this._tileDefManager[tileRef.Tile.TypeId]).Weather)
      return false;
    AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(uid, grid, tileRef.GridIndices);
    EntityUid? uid1;
    while (entitiesEnumerator.MoveNext(out uid1))
    {
      BlockWeatherComponent component;
      if (this._blockQuery.TryGetComponent(uid1.Value, out component) && component.Enabled)
        return false;
    }
    return true;
  }

  public float GetPercent(WeatherData component, EntityUid mapUid)
  {
    TimeSpan pauseTime = this._metadata.GetPauseTime(mapUid);
    TimeSpan timeSpan1 = this.Timing.CurTime - (component.StartTime + pauseTime);
    TimeSpan timeSpan2 = component.Duration - timeSpan1;
    return !(timeSpan2 < WeatherComponent.ShutdownTime) ? (!(timeSpan1 < WeatherComponent.StartupTime) ? 1f : (float) (timeSpan1 / WeatherComponent.StartupTime)) : (float) (timeSpan2 / WeatherComponent.ShutdownTime);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this.Timing.IsFirstTimePredicted)
      return;
    TimeSpan curTime = this.Timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<WeatherComponent> entityQueryEnumerator = this.EntityQueryEnumerator<WeatherComponent>();
    EntityUid uid;
    WeatherComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Weather.Count != 0)
      {
        foreach ((ProtoId<WeatherPrototype> protoId, WeatherData weather) in comp1.Weather)
        {
          TimeSpan? endTime = weather.EndTime;
          if (endTime.HasValue)
          {
            TimeSpan? nullable = endTime;
            TimeSpan timeSpan = curTime;
            if ((nullable.HasValue ? (nullable.GetValueOrDefault() < timeSpan ? 1 : 0) : 0) != 0)
            {
              this.EndWeather(uid, comp1, (string) protoId);
              continue;
            }
          }
          TimeSpan? nullable1 = endTime;
          TimeSpan timeSpan1 = curTime;
          TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - timeSpan1) : new TimeSpan?();
          WeatherPrototype prototype;
          if (!this.ProtoMan.TryIndex<WeatherPrototype>(protoId, out prototype))
          {
            this.Log.Error($"Unable to find weather prototype for {comp1.Weather}, ending!");
            this.EndWeather(uid, comp1, (string) protoId);
          }
          else
          {
            if (endTime.HasValue)
            {
              TimeSpan? nullable3 = nullable2;
              TimeSpan shutdownTime = WeatherComponent.ShutdownTime;
              if ((nullable3.HasValue ? (nullable3.GetValueOrDefault() < shutdownTime ? 1 : 0) : 0) != 0)
              {
                this.SetState(uid, WeatherState.Ending, comp1, weather, prototype);
                goto label_17;
              }
            }
            if (this.Timing.CurTime - weather.StartTime < WeatherComponent.StartupTime)
              this.SetState(uid, WeatherState.Starting, comp1, weather, prototype);
label_17:
            this.Run(uid, weather, prototype, frameTime);
          }
        }
      }
    }
  }

  public void SetWeather(MapId mapId, WeatherPrototype? proto, TimeSpan? endTime)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return;
    WeatherComponent component = this.EnsureComp<WeatherComponent>(uid.Value);
    foreach ((ProtoId<WeatherPrototype> key, WeatherData weatherData) in component.Weather)
    {
      if (proto == null)
      {
        endTime.GetValueOrDefault();
        if (!endTime.HasValue)
          endTime = new TimeSpan?(this.Timing.CurTime + WeatherComponent.ShutdownTime);
      }
      if (proto != null && key == (ProtoId<WeatherPrototype>) proto.ID)
      {
        weatherData.EndTime = endTime;
        if (weatherData.State == WeatherState.Ending)
          weatherData.State = WeatherState.Running;
        this.Dirty(uid.Value, (IComponent) component);
      }
      else
      {
        TimeSpan timeSpan1 = this.Timing.CurTime + WeatherComponent.ShutdownTime;
        if (weatherData.EndTime.HasValue)
        {
          TimeSpan? endTime1 = weatherData.EndTime;
          TimeSpan timeSpan2 = timeSpan1;
          if ((endTime1.HasValue ? (endTime1.GetValueOrDefault() > timeSpan2 ? 1 : 0) : 0) == 0)
            continue;
        }
        weatherData.EndTime = new TimeSpan?(timeSpan1);
        this.Dirty(uid.Value, (IComponent) component);
      }
    }
    if (proto == null)
      return;
    this.StartWeather(uid.Value, component, proto, endTime);
  }

  protected virtual void Run(
    EntityUid uid,
    WeatherData weather,
    WeatherPrototype weatherProto,
    float frameTime)
  {
  }

  protected void StartWeather(
    EntityUid uid,
    WeatherComponent component,
    WeatherPrototype weather,
    TimeSpan? endTime)
  {
    if (component.Weather.ContainsKey((ProtoId<WeatherPrototype>) weather.ID))
      return;
    WeatherData weatherData = new WeatherData()
    {
      StartTime = this.Timing.CurTime,
      EndTime = endTime
    };
    component.Weather.Add((ProtoId<WeatherPrototype>) weather.ID, weatherData);
    this.Dirty(uid, (IComponent) component);
  }

  protected virtual void EndWeather(EntityUid uid, WeatherComponent component, string proto)
  {
    WeatherData weatherData;
    if (!component.Weather.TryGetValue((ProtoId<WeatherPrototype>) proto, out weatherData))
      return;
    this._audio.Stop(weatherData.Stream);
    weatherData.Stream = new EntityUid?();
    component.Weather.Remove((ProtoId<WeatherPrototype>) proto);
    this.Dirty(uid, (IComponent) component);
  }

  protected virtual bool SetState(
    EntityUid uid,
    WeatherState state,
    WeatherComponent component,
    WeatherData weather,
    WeatherPrototype weatherProto)
  {
    if (weather.State.Equals((object) state))
      return false;
    weather.State = state;
    this.Dirty(uid, (IComponent) component);
    return true;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class WeatherComponentState : ComponentState
  {
    public Dictionary<ProtoId<WeatherPrototype>, WeatherData> Weather;

    public WeatherComponentState(
      Dictionary<ProtoId<WeatherPrototype>, WeatherData> weather)
    {
      this.Weather = weather;
    }
  }
}
