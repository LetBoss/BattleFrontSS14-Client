// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivParticleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Weather;
using Content.Shared._CIV14merka.Particles;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Weather;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

public sealed class CivParticleSystem : EntitySystem
{
  [Dependency]
  private readonly IOverlayManager _overlay;
  [Dependency]
  private readonly IPrototypeManager _proto;
  [Dependency]
  private readonly IPlayerManager _player;
  [Dependency]
  private readonly IResourceCache _resource;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly IMapManager _mapMan;
  [Dependency]
  private readonly WeatherSystem _weather;
  private CivParticleOverlay? _overlayInstance;
  private readonly Dictionary<string, CivParticlePresetPrototype> _byWeather = new Dictionary<string, CivParticlePresetPrototype>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.BuildLookup();
    this._proto.PrototypesReloaded += (Action<PrototypesReloadedEventArgs>) (_ => this.BuildLookup());
    this._overlayInstance = new CivParticleOverlay((IEntityManager) this.EntityManager, this._mapMan, this.EntityManager.System<SharedTransformSystem>(), this.EntityManager.System<SharedRoofSystem>(), this._resource);
    this._overlay.AddOverlay((Overlay) this._overlayInstance);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlayInstance != null)
      this._overlay.RemoveOverlay((Overlay) this._overlayInstance);
    this._overlayInstance = (CivParticleOverlay) null;
  }

  private void BuildLookup()
  {
    this._byWeather.Clear();
    foreach (CivParticlePresetPrototype enumeratePrototype in this._proto.EnumeratePrototypes<CivParticlePresetPrototype>())
    {
      foreach (ProtoId<WeatherPrototype> weatherType in enumeratePrototype.WeatherTypes)
        this._byWeather[weatherType.Id] = enumeratePrototype;
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    if (this._overlayInstance == null)
      return;
    this._overlayInstance.Preset = (CivParticlePresetPrototype) null;
    this._overlayInstance.Intensity = 0.0f;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid? mapUid = this.Transform(localEntity.Value).MapUid;
    WeatherComponent weatherComponent;
    if (!mapUid.HasValue || !this.TryComp<WeatherComponent>(mapUid.Value, ref weatherComponent) || weatherComponent.Weather.Count == 0)
      return;
    CivParticlePresetPrototype particlePresetPrototype1 = (CivParticlePresetPrototype) null;
    float num = 0.0f;
    foreach ((ProtoId<WeatherPrototype> key, WeatherData component) in weatherComponent.Weather)
    {
      CivParticlePresetPrototype particlePresetPrototype2;
      if (this._byWeather.TryGetValue(key.Id, out particlePresetPrototype2))
      {
        float percent = this._weather.GetPercent(component, mapUid.Value);
        if ((double) percent > (double) num)
        {
          num = percent;
          particlePresetPrototype1 = particlePresetPrototype2;
        }
      }
    }
    if (particlePresetPrototype1 == null)
      return;
    this._overlayInstance.Preset = particlePresetPrototype1;
    this._overlayInstance.Intensity = num;
    this._overlayInstance.Wind = this.GetWind();
  }

  private Vector2 GetWind()
  {
    float totalSeconds = (float) this._timing.RealTime.TotalSeconds;
    float num = (float) (1.3999999761581421 + (double) MathF.Sin(totalSeconds * 0.13f) * 0.89999997615814209 + (double) MathF.Sin(totalSeconds * 0.41f) * 0.40000000596046448);
    float x = MathF.Sin(totalSeconds * 0.05f) * 0.6f;
    return new Vector2(MathF.Cos(x), MathF.Sin(x)) * num;
  }
}
