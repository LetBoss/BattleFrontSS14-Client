// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.WeedKiller.WeedKillerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared.Coordinates;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.WeedKiller;

public sealed class WeedKillerSystem : EntitySystem
{
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private RMCCameraShakeSystem _rmcCameraShake;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<DeletedByWeedKillerComponent> _deletedByWeedKillerQuery;
  private static readonly EntProtoId WeedKiller = (EntProtoId) "RMCGasWeedKiller";
  private TimeSpan _dropshipDelay;
  private TimeSpan _disableDuration;

  public override void Initialize()
  {
    this._deletedByWeedKillerQuery = this.GetEntityQuery<DeletedByWeedKillerComponent>();
    this.SubscribeLocalEvent<DropshipLaunchedFromWarshipEvent>(new EntityEventRefHandler<DropshipLaunchedFromWarshipEvent>(this.OnDropshipLaunchedFromWarship));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCWeedKillerDropshipDelaySeconds, (Action<int>) (v => this._dropshipDelay = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCWeedKillerDisableDurationMinutes, (Action<int>) (v => this._disableDuration = TimeSpan.FromMinutes((long) v)), true);
  }

  private void OnDropshipLaunchedFromWarship(ref DropshipLaunchedFromWarshipEvent ev)
  {
    if (this._net.IsClient || this.Count<WeedKillerComponent>() > 0)
      return;
    EntityUid? destination = ev.Dropship.Comp.Destination;
    if (!destination.HasValue)
      return;
    EntityCoordinates coordinates = destination.GetValueOrDefault().ToCoordinates();
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea(coordinates, out area, out EntityPrototype _) || string.IsNullOrWhiteSpace(area.Value.Comp.LinkedLz))
      return;
    this.CreateWeedKiller((EntityUid) ev.Dropship, coordinates);
  }

  public void CreateWeedKiller(EntityUid dropship, EntityCoordinates coordinates)
  {
    EntityUid uid1 = this.Spawn();
    WeedKillerComponent weedKillerComponent = this.EnsureComp<WeedKillerComponent>(uid1);
    weedKillerComponent.DeployAt = this._timing.CurTime + this._dropshipDelay;
    weedKillerComponent.DisableAt = this._timing.CurTime + this._dropshipDelay + this._disableDuration;
    weedKillerComponent.Dropship = new EntityUid?(dropship);
    this.Dirty(uid1, (IComponent) weedKillerComponent);
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea(coordinates, out area, out EntityPrototype _))
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<AreaComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AreaComponent>();
    EntityUid uid2;
    AreaComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid2, out comp1))
    {
      string linkedLz = comp1.LinkedLz;
      if ((linkedLz != null ? (linkedLz.Contains(',') ? 1 : 0) : 0) != 0)
      {
        if (!((IEnumerable<string>) comp1.LinkedLz.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).Contains<string>(area.Value.Comp.LinkedLz))
          continue;
      }
      else if (comp1.LinkedLz != area.Value.Comp.LinkedLz)
        continue;
      string id = this.Prototype(uid2)?.ID;
      if (id != null)
        weedKillerComponent.AreaPrototypes.Add((EntProtoId) id);
      weedKillerComponent.LinkedAreas.Add(uid2);
    }
    EntityUid? grid = this._transform.GetGrid(coordinates);
    MapGridComponent comp2;
    AreaGridComponent comp3;
    if (!this.TryComp<MapGridComponent>(grid, out comp2) || !this.TryComp<AreaGridComponent>(grid, out comp3))
      return;
    foreach ((Vector2i key, EntProtoId<AreaComponent> entProtoId) in comp3.Areas)
    {
      if (weedKillerComponent.AreaPrototypes.Contains((EntProtoId) entProtoId))
        weedKillerComponent.Positions.Add(((Entity<MapGridComponent>) (grid.Value, comp2), key));
    }
  }

  private void KillWeeds(Entity<WeedKillerComponent> killer)
  {
    foreach (EntityUid linkedArea in killer.Comp.LinkedAreas)
    {
      AreaComponent comp;
      if (this.TryComp<AreaComponent>(linkedArea, out comp))
      {
        comp.WeedKilling = true;
        this.Dirty(linkedArea, (IComponent) comp);
      }
    }
    EntityUid? dropship = killer.Comp.Dropship;
    if (dropship.HasValue)
    {
      Filter filter = Filter.BroadcastMap(this._transform.GetMapId((Entity<TransformComponent>) dropship.Value));
      this._audio.PlayGlobal(killer.Comp.Sound, filter, false);
      this._rmcCameraShake.ShakeCamera(filter, 3, 1);
      this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), this.Loc.GetString("rmc-weed-killer-deploying", ("dropship", (object) this.Name(dropship.Value))));
    }
    using (List<(Entity<MapGridComponent> Grid, Vector2i Indices)>.Enumerator enumerator = killer.Comp.Positions.GetEnumerator())
    {
label_14:
      while (enumerator.MoveNext())
      {
        (Entity<MapGridComponent> Grid, Vector2i Indices) current = enumerator.Current;
        this.Spawn((string) WeedKillerSystem.WeedKiller, this._map.ToCoordinates((EntityUid) current.Grid, current.Indices, (MapGridComponent) current.Grid));
        RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(current.Grid, current.Indices, facing: (DirectionFlag) 0);
        while (true)
        {
          EntityUid uid;
          do
          {
            if (!entitiesEnumerator.MoveNext(out uid))
              goto label_14;
          }
          while (!this._deletedByWeedKillerQuery.HasComp(uid));
          this.QueueDel(new EntityUid?(uid));
        }
      }
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<WeedKillerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<WeedKillerComponent>();
    EntityUid uid;
    WeedKillerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!comp1.Deployed)
      {
        if (!(curTime < comp1.DeployAt))
        {
          comp1.Deployed = true;
          this.Dirty(uid, (IComponent) comp1);
          this.KillWeeds((Entity<WeedKillerComponent>) (uid, comp1));
        }
        else
          continue;
      }
      if (!comp1.Disabled && !(curTime < comp1.DisableAt))
      {
        comp1.Disabled = true;
        this.Dirty(uid, (IComponent) comp1);
        foreach (EntityUid linkedArea in comp1.LinkedAreas)
        {
          AreaComponent comp;
          if (this.TryComp<AreaComponent>(linkedArea, out comp))
          {
            comp.WeedKilling = false;
            this.Dirty(linkedArea, (IComponent) comp);
          }
        }
      }
    }
  }
}
