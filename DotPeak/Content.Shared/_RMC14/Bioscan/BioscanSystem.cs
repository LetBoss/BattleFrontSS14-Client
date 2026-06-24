// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Bioscan.BioscanSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;

#nullable enable
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
    this.SubscribeLocalEvent<BioscanComponent, MapInitEvent>(new EntityEventRefHandler<BioscanComponent, MapInitEvent>(this.OnMapInit));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBioscanInitialDelaySeconds, (Action<int>) (v => this._bioscanInitialDelay = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBioscanCheckDelaySeconds, (Action<int>) (v => this._bioscanCheckDelay = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBioscanMinimumCooldownSeconds, (Action<int>) (v => this._bioscanMinimumCooldown = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBioscanBaseCooldownSeconds, (Action<int>) (v => this._bioscanBaseCooldown = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCBioscanVariance, (Action<int>) (v => this._bioscanVariance = v), true);
  }

  private void OnMapInit(Entity<BioscanComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.LastMarine = this._timing.CurTime + this._bioscanInitialDelay;
    ent.Comp.LastXeno = this._timing.CurTime + this._bioscanInitialDelay;
    this.Dirty<BioscanComponent>(ent);
  }

  private bool TryBioscan<T>(
    TimeSpan last,
    bool force,
    ref int max,
    out int alive,
    out int aliveShip,
    out int alivePlanet,
    out string warshipArea,
    out string planetArea)
    where T : IComponent
  {
    alive = 0;
    aliveShip = 0;
    alivePlanet = 0;
    warshipArea = "none";
    planetArea = "none";
    TimeSpan curTime = this._timing.CurTime;
    if (!force && last + this._bioscanMinimumCooldown > curTime)
      return false;
    this._planetMaps.Clear();
    this._warshipMaps.Clear();
    this._planetAreas.Clear();
    this._warshipAreas.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCPlanetComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCPlanetComponent, TransformComponent>();
    TransformComponent comp2_1;
    while (entityQueryEnumerator1.MoveNext(out RMCPlanetComponent _, out comp2_1))
      this._planetMaps.Add(comp2_1.MapID);
    Robust.Shared.GameObjects.EntityQueryEnumerator<AlmayerComponent, TransformComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<AlmayerComponent, TransformComponent>();
    TransformComponent comp2_2;
    while (entityQueryEnumerator2.MoveNext(out AlmayerComponent _, out comp2_2))
      this._warshipMaps.Add(comp2_2.MapID);
    alive = 0;
    aliveShip = 0;
    alivePlanet = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<T, ActorComponent, MobStateComponent, TransformComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<T, ActorComponent, MobStateComponent, TransformComponent>();
    EntityUid uid;
    MobStateComponent comp3;
    TransformComponent comp4;
    while (entityQueryEnumerator3.MoveNext(out uid, out T _, out ActorComponent _, out comp3, out comp4))
    {
      if (this._mobState.IsAlive(uid, comp3) && !this.HasComp<ThunderdomeMapComponent>(comp4.MapUid))
      {
        ++alive;
        string name;
        bool flag = this._area.BioscanBlocked(uid, out name);
        MapId mapId = comp4.MapID;
        if (this._warshipMaps.Contains(mapId))
        {
          if (!flag)
          {
            ++aliveShip;
            if (name != null)
              this._warshipAreas.Add(name);
          }
        }
        else if (this._planetMaps.Contains(mapId))
        {
          ++alivePlanet;
          if (!flag && name != null)
            this._planetAreas.Add(name);
        }
      }
    }
    if (alive > max)
      max = alive;
    if (max != 0)
    {
      TimeSpan timeSpan1 = this._bioscanBaseCooldown * (double) alive / (double) max;
      if (timeSpan1 < this._bioscanMinimumCooldown)
        timeSpan1 = this._bioscanMinimumCooldown;
      TimeSpan timeSpan2 = timeSpan1 + last;
      if (!force && curTime < timeSpan2)
        return false;
    }
    else if (!force)
      return false;
    if (this._warshipAreas.Count > 0)
      warshipArea = RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) this._warshipAreas);
    if (this._planetAreas.Count > 0)
      planetArea = RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) this._planetAreas);
    return true;
  }

  public void TryBioscanARES(Entity<BioscanComponent> bioscan, bool force)
  {
    TimeSpan curTime = this._timing.CurTime;
    int aliveShip;
    int alivePlanet;
    string warshipArea;
    string planetArea;
    if (!this.TryBioscan<XenoComponent>(bioscan.Comp.LastMarine, force, ref bioscan.Comp.MaxXenoAlive, out int _, out aliveShip, out alivePlanet, out warshipArea, out planetArea))
      return;
    int bioscanVariance = this._bioscanVariance;
    int num = Math.Max(0, alivePlanet + this._random.Next(-bioscanVariance, bioscanVariance + 1));
    if (num == 0)
      planetArea = "none";
    bioscan.Comp.LastMarine = curTime;
    this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), this.Loc.GetString("rmc-bioscan-ares", ("shipUncontained", (object) aliveShip), ("shipLocation", (object) warshipArea), ("planetLocation", (object) planetArea), ("onPlanet", (object) num)), bioscan.Comp.MarineSound, (LocId?) "rmc-bioscan-ares-announcement");
    this.Dirty<BioscanComponent>(bioscan);
  }

  public void TryBioscanQueenMother(Entity<BioscanComponent> bioscan, bool force)
  {
    TimeSpan curTime = this._timing.CurTime;
    int aliveShip;
    int alivePlanet;
    string warshipArea;
    string planetArea;
    if (!this.TryBioscan<MarineComponent>(bioscan.Comp.LastXeno, force, ref bioscan.Comp.MaxMarinesAlive, out int _, out aliveShip, out alivePlanet, out warshipArea, out planetArea))
      return;
    int bioscanVariance = this._bioscanVariance;
    int num = Math.Max(0, aliveShip + this._random.Next(-bioscanVariance, bioscanVariance + 1));
    if (num == 0)
      planetArea = "none";
    bioscan.Comp.LastXeno = curTime;
    this._xenoAnnounce.AnnounceAll(new EntityUid(), this.Loc.GetString("rmc-bioscan-xeno-announcement", ("message", (object) this.Loc.GetString("rmc-bioscan-xeno", ("shipLocation", (object) warshipArea), ("planetLocation", (object) planetArea), ("onShip", (object) num), ("onPlanet", (object) alivePlanet)))), bioscan.Comp.XenoSound);
    this.Dirty<BioscanComponent>(bioscan);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<BioscanComponent> entityQueryEnumerator = this.EntityQueryEnumerator<BioscanComponent>();
    EntityUid uid;
    BioscanComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.NextCheck > curTime))
      {
        comp1.NextCheck = curTime + this._bioscanCheckDelay;
        this.Dirty(uid, (IComponent) comp1);
        this.TryBioscanARES((Entity<BioscanComponent>) (uid, comp1), false);
        this.TryBioscanQueenMother((Entity<BioscanComponent>) (uid, comp1), false);
      }
    }
  }
}
