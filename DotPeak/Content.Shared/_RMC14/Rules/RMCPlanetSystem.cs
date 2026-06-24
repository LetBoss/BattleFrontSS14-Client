// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rules.RMCPlanetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.TacticalMap;
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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
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
  private Robust.Shared.GameObjects.EntityQuery<RMCPlanetComponent> _rmcPlanetQuery;

  public ImmutableDictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>> PlanetPaths { get; private set; } = ImmutableDictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>>.Empty;

  public override void Initialize()
  {
    this._rmcPlanetQuery = this.GetEntityQuery<RMCPlanetComponent>();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.SubscribeLocalEvent<RMCPlanetComponent, MapInitEvent>(new EntityEventRefHandler<RMCPlanetComponent, MapInitEvent>(this.OnPlanetMapInit));
    this.SubscribeLocalEvent<RMCHijackSongComponent, ComponentStartup>(new EntityEventRefHandler<RMCHijackSongComponent, ComponentStartup>(this.OnHijackSongStartup), after: new Type[1]
    {
      typeof (SharedAudioSystem)
    });
    this.Subs.CVar<int>(this._config, RMCCVars.RMCPlanetCoordinateVariance, (Action<int>) (v => this._coordinateVariance = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.VolumeGainHijackSong, new Action<float>(this.SetVolumeHijack), true);
    this.ReloadPlanets();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<Robust.Shared.Prototypes.EntityPrototype>())
      return;
    this.ReloadPlanets();
  }

  private void OnPlanetMapInit(Entity<RMCPlanetComponent> ent, ref MapInitEvent args)
  {
    int num1 = this._random.Next(-this._coordinateVariance, this._coordinateVariance + 1);
    int num2 = this._random.Next(-this._coordinateVariance, this._coordinateVariance + 1);
    ent.Comp.Offset = Vector2i.op_Implicit((num1, num2));
    RMCPlanetAddedEvent args1 = new RMCPlanetAddedEvent();
    this.RaiseLocalEvent<RMCPlanetAddedEvent>((EntityUid) ent, ref args1);
  }

  private void OnHijackSongStartup(Entity<RMCHijackSongComponent> ent, ref ComponentStartup args)
  {
    AudioComponent comp;
    if (this._net.IsServer || !this.TryComp<AudioComponent>((EntityUid) ent, out comp))
      return;
    comp.Params.Volume = SharedAudioSystem.GainToVolume(this._hijackSongGain);
  }

  private void SetVolumeHijack(float gain)
  {
    if (this._net.IsServer)
      return;
    this._hijackSongGain = gain;
    AllEntityQueryEnumerator<RMCHijackSongComponent, AudioComponent> entityQueryEnumerator = this.AllEntityQuery<RMCHijackSongComponent, AudioComponent>();
    AudioComponent comp2;
    while (entityQueryEnumerator.MoveNext(out EntityUid _, out RMCHijackSongComponent _, out comp2))
      comp2.Params = comp2.Params with
      {
        Volume = SharedAudioSystem.GainToVolume(gain)
      };
  }

  public bool IsOnPlanet(EntityCoordinates coordinates)
  {
    return this._rmcPlanetQuery.HasComp(this._transform.GetGrid(coordinates)) || this._rmcPlanetQuery.HasComp(this._transform.GetMap(coordinates));
  }

  public bool IsOnPlanet(TransformComponent xform)
  {
    return this._rmcPlanetQuery.HasComp(xform.GridUid) || this._rmcPlanetQuery.HasComp(xform.MapUid);
  }

  public bool IsOnPlanet(MapCoordinates coordinates)
  {
    return this.IsOnPlanet(this._transform.ToCoordinates(coordinates));
  }

  public bool TryGetOffset(MapCoordinates coordinates, out Vector2i offset)
  {
    EntityCoordinates coordinates1 = this._transform.ToCoordinates(coordinates);
    EntityUid? grid = this._transform.GetGrid(coordinates1);
    RMCPlanetComponent comp1;
    if (grid.HasValue && this.TryComp<RMCPlanetComponent>(grid.GetValueOrDefault(), out comp1))
    {
      offset = comp1.Offset;
      return true;
    }
    EntityUid? map = this._transform.GetMap(coordinates1);
    RMCPlanetComponent comp2;
    if (map.HasValue && this.TryComp<RMCPlanetComponent>(map.GetValueOrDefault(), out comp2))
    {
      offset = comp2.Offset;
      return true;
    }
    offset = new Vector2i();
    return false;
  }

  public bool TryPlanetToCoordinates(Vector2i coordinates, out MapCoordinates mapCoordinates)
  {
    EntityUid uid;
    RMCPlanetComponent comp1;
    if (this.EntityQueryEnumerator<RMCPlanetComponent>().MoveNext(out uid, out comp1))
    {
      MapId mapId = this._transform.GetMapId((Entity<TransformComponent>) uid);
      mapCoordinates = new MapCoordinates(Vector2i.op_Implicit(Vector2i.op_Subtraction(coordinates, comp1.Offset)), mapId);
      return true;
    }
    mapCoordinates = new MapCoordinates();
    return false;
  }

  private void ReloadPlanets()
  {
    Dictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>> source = new Dictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>>();
    foreach (Robust.Shared.Prototypes.EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<Robust.Shared.Prototypes.EntityPrototype>())
    {
      RMCPlanetMapPrototypeComponent component;
      if (enumeratePrototype.TryGetComponent<RMCPlanetMapPrototypeComponent>(out component, this._compFactory))
        source[component.Map.ToString()] = (EntProtoId<RMCPlanetMapPrototypeComponent>) enumeratePrototype.ID;
    }
    this.PlanetPaths = source.ToImmutableDictionary<string, EntProtoId<RMCPlanetMapPrototypeComponent>>();
  }

  public List<RMCPlanet> GetAllPlanets()
  {
    List<RMCPlanet> allPlanets = new List<RMCPlanet>();
    foreach (EntProtoId<RMCPlanetMapPrototypeComponent> id in this.PlanetPaths.Values)
    {
      Robust.Shared.Prototypes.EntityPrototype prototype;
      RMCPlanetMapPrototypeComponent comp;
      if (this._prototypes.TryIndex((EntProtoId) id, out prototype) && id.TryGet(out comp, this._prototypes, this._compFactory))
        allPlanets.Add(new RMCPlanet(prototype, comp));
    }
    return allPlanets;
  }

  public List<RMCPlanet> GetAllPlanetsInRotation()
  {
    return this.GetAllPlanets().Where<RMCPlanet>((Func<RMCPlanet, bool>) (p => p.Comp.InRotation)).ToList<RMCPlanet>();
  }

  public List<RMCPlanet> GetCandidatesInRotation()
  {
    List<RMCPlanet> planetsInRotation = this.GetAllPlanetsInRotation();
    int playerCount = this._player.PlayerCount;
    if (playerCount == 0)
      return planetsInRotation;
    for (int index = planetsInRotation.Count - 1; index >= 0; --index)
    {
      RMCPlanetMapPrototypeComponent comp = planetsInRotation[index].Comp;
      if (comp.MinPlayers != 0 && playerCount < comp.MinPlayers || comp.MaxPlayers != 0 && playerCount > comp.MaxPlayers)
        planetsInRotation.RemoveAt(index);
    }
    return planetsInRotation;
  }

  public MapId? Load(ResPath path)
  {
    DeserializationOptions deserializationOptions = new DeserializationOptions()
    {
      InitializeMaps = true
    };
    Entity<MapComponent>? map;
    if (!this._mapLoader.TryLoadMap(path, out map, out HashSet<Entity<MapGridComponent>> _, new DeserializationOptions?(deserializationOptions), rot: new Angle()))
      return new MapId?();
    foreach (Entity<RMCPlanetComponent> allEntity in this.EntityManager.AllEntities<RMCPlanetComponent>())
      this.RemComp<RMCPlanetComponent>((EntityUid) allEntity);
    foreach (Entity<TacticalMapComponent> allEntity in this.EntityManager.AllEntities<TacticalMapComponent>())
      this.RemComp<TacticalMapComponent>((EntityUid) allEntity);
    this.EnsureComp<RMCPlanetComponent>((EntityUid) map.Value);
    this.EnsureComp<TacticalMapComponent>((EntityUid) map.Value);
    this._rmcPower.RecalculatePower();
    return new MapId?(map.Value.Comp.MapId);
  }
}
