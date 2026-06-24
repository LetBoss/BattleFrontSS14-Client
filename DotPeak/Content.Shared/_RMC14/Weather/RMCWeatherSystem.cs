// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weather.RMCWeatherSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Light;
using Content.Shared.Dataset;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Weather;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weather;

public sealed class RMCWeatherSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedRoofSystem _roof;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedWeatherSystem _weather;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private RMCAmbientLightSystem _rmcLight;
  [Dependency]
  private SharedAudioSystem _audio;
  private Robust.Shared.GameObjects.EntityQuery<BlockWeatherComponent> _blockQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._blockQuery = this.GetEntityQuery<BlockWeatherComponent>();
    this.SubscribeLocalEvent<RMCWeatherCycleComponent, MapInitEvent>(new EntityEventRefHandler<RMCWeatherCycleComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnMapInit(Entity<RMCWeatherCycleComponent> ent, ref MapInitEvent args)
  {
    if (ent.Comp.WeatherEvents.Count <= 0)
      return;
    this.EnsureComp<RMCAmbientLightComponent>((EntityUid) ent);
    this.EnsureComp<RMCAmbientLightEffectsComponent>((EntityUid) ent);
    ent.Comp.LastEventCooldown = this._random.Next(ent.Comp.MinTimeBetweenEvents);
  }

  public bool CanWeatherAffectArea(
    EntityUid uid,
    MapGridComponent grid,
    TileRef tileRef,
    RoofComponent? roofComp = null)
  {
    if (this.Resolve<RoofComponent>(uid, ref roofComp, false) && this._roof.IsRooved((Entity<MapGridComponent, RoofComponent>) (uid, grid, roofComp), tileRef.GridIndices) || !this._area.IsWeatherEnabled((Entity<MapGridComponent>) (uid, grid), tileRef.GridIndices))
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

  public void HandleWeatherEffects(Entity<RMCWeatherCycleComponent> ent)
  {
    if (ent.Comp.CurrentEvent == null || (double) ent.Comp.CurrentEvent.LightningChance <= 0.0 || ent.Comp.CurrentEvent.LightningEffects.Count <= 0 || !this._random.Prob(ent.Comp.CurrentEvent.LightningChance))
      return;
    RMCAmbientLightComponent comp1;
    this.EnsureComp<RMCAmbientLightComponent>((EntityUid) ent, out comp1);
    MapLightComponent comp2;
    this.EnsureComp<MapLightComponent>((EntityUid) ent, out comp2);
    if (comp1.IsAnimating || Color.op_Inequality(comp2.AmbientLightColor, Color.Black))
      return;
    List<Color> colorList = this._rmcLight.ProcessPrototype((ProtoId<DatasetPrototype>) RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) ent.Comp.CurrentEvent.LightningEffects));
    TimeSpan lightningDuration = ent.Comp.CurrentEvent.LightningDuration;
    this._rmcLight.SetColor((Entity<RMCAmbientLightComponent>) ((EntityUid) ent, comp1), colorList, lightningDuration);
    this._audio.PlayGlobal(this._audio.ResolveSound(ent.Comp.CurrentEvent.LightningSound), Filter.BroadcastMap(this.Transform((EntityUid) ent).MapID), true);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCWeatherCycleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCWeatherCycleComponent>();
    EntityUid uid;
    RMCWeatherCycleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.LastEventCooldown -= TimeSpan.FromSeconds((double) frameTime);
      if (comp1.LastEventCooldown <= TimeSpan.Zero)
      {
        RMCWeatherEvent rmcWeatherEvent = RandomExtensions.Pick<RMCWeatherEvent>(this._random, (IReadOnlyList<RMCWeatherEvent>) comp1.WeatherEvents);
        WeatherPrototype prototype;
        this._proto.TryIndex<WeatherPrototype>(rmcWeatherEvent.WeatherType, out prototype);
        TimeSpan timeSpan1 = this._timing.CurTime + rmcWeatherEvent.Duration;
        comp1.CurrentEvent = rmcWeatherEvent;
        comp1.CurrentEvent.DurationRemaining = rmcWeatherEvent.Duration;
        this._weather.SetWeather(this.Transform(uid).MapID, prototype, new TimeSpan?(timeSpan1));
        TimeSpan timeSpan2 = -comp1.MinTimeVariance * 0.5 + this._random.Next(comp1.MinTimeVariance);
        comp1.LastEventCooldown = rmcWeatherEvent.Duration + comp1.MinTimeBetweenEvents + timeSpan2;
      }
      if (comp1.CurrentEvent != null)
      {
        comp1.CurrentEvent.DurationRemaining -= TimeSpan.FromSeconds((double) frameTime);
        if (comp1.CurrentEvent.DurationRemaining <= TimeSpan.Zero)
        {
          comp1.CurrentEvent = (RMCWeatherEvent) null;
        }
        else
        {
          comp1.CurrentEvent.LightningCooldown -= TimeSpan.FromSeconds((double) frameTime);
          if (comp1.CurrentEvent.LightningCooldown <= TimeSpan.Zero)
          {
            this.HandleWeatherEffects((Entity<RMCWeatherCycleComponent>) (uid, comp1));
            comp1.CurrentEvent.LightningCooldown = comp1.CurrentEvent.LightningCooldownDuration;
          }
        }
      }
    }
  }
}
