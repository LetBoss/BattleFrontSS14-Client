// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.AreaInfoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Alert;
using Content.Shared.Coordinates;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

public sealed class AreaInfoSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private InventorySystem _inv;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly Queue<Entity<AreaInfoComponent>> _marineAlertCopyQueue = new Queue<Entity<AreaInfoComponent>>();
  private TimeSpan _maxProcessTime;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GrantAreaInfoComponent, GotEquippedEvent>(new EntityEventRefHandler<GrantAreaInfoComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<GrantAreaInfoComponent, GotUnequippedEvent>(new EntityEventRefHandler<GrantAreaInfoComponent, GotUnequippedEvent>(this.OnGotUnequipped));
    this.SubscribeLocalEvent<AreaInfoComponent, MapInitEvent>(new EntityEventRefHandler<AreaInfoComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<AreaInfoComponent, ComponentRemove>(new EntityEventRefHandler<AreaInfoComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<AreaInfoComponent, MoveEvent>(new EntityEventRefHandler<AreaInfoComponent, MoveEvent>(this.OnMoveEvent));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCMaxTacmapAlertProcessTimeMilliseconds, (Action<float>) (v => this._maxProcessTime = TimeSpan.FromMilliseconds((double) v)), true);
  }

  private void OnGotEquipped(Entity<GrantAreaInfoComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    this.EnsureComp<AreaInfoComponent>(args.Equipee);
  }

  private void OnGotUnequipped(Entity<GrantAreaInfoComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE || this._inv.TryGetInventoryEntity<GrantAreaInfoComponent>((Entity<InventoryComponent>) args.Equipee, out Entity<GrantAreaInfoComponent> _))
      return;
    this.RemCompDeferred<AreaInfoComponent>(args.Equipee);
  }

  private void OnMapInit(Entity<AreaInfoComponent> ent, ref MapInitEvent args)
  {
    (string areaName, short ceilingLevel, string restrictions) = this.GetAreaInfo((EntityUid) ent);
    AlertsSystem alerts = this._alerts;
    EntityUid euid = (EntityUid) ent;
    ProtoId<AlertPrototype> alert = ent.Comp.Alert;
    short? severity = new short?(ceilingLevel);
    string str = this.Loc.GetString("rmc-area-info", ("area", (object) areaName), ("ceilingLevel", (object) ceilingLevel), ("restrictions", (object) restrictions));
    (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
    string dynamicMessage = str;
    alerts.ShowAlert(euid, alert, severity, cooldown, dynamicMessage: dynamicMessage);
  }

  private void OnRemove(Entity<AreaInfoComponent> ent, ref ComponentRemove args)
  {
    this._alerts.ClearAlert((EntityUid) ent, ent.Comp.Alert);
  }

  private void OnMoveEvent(Entity<AreaInfoComponent> ent, ref MoveEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    (string areaName, short ceilingLevel, string restrictions) = this.GetAreaInfo((EntityUid) ent);
    AlertsSystem alerts = this._alerts;
    EntityUid euid = (EntityUid) ent;
    ProtoId<AlertPrototype> alert = ent.Comp.Alert;
    short? severity = new short?(ceilingLevel);
    string str = this.Loc.GetString("rmc-area-info", ("area", (object) areaName), ("ceilingLevel", (object) ceilingLevel), ("restrictions", (object) restrictions));
    (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
    string dynamicMessage = str;
    alerts.ShowAlert(euid, alert, severity, cooldown, dynamicMessage: dynamicMessage);
  }

  private (string areaName, short ceilingLevel, string restrictions) GetAreaInfo(EntityUid ent)
  {
    EntityCoordinates coordinates = ent.ToCoordinates();
    Entity<AreaComponent>? area;
    EntityPrototype areaPrototype;
    if (!this._area.TryGetArea(coordinates, out area, out areaPrototype))
      return (this.Loc.GetString("rmc-tacmap-alert-no-area"), (short) 0, string.Empty);
    bool flag1 = this.IsProtectedByRoofing(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanOrbitalBombard && (double) r.Comp.Range > 10.0));
    bool flag2 = this.IsProtectedByRoofing(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => r.Comp.CanOrbitalBombard && !r.Comp.CanCAS && (double) r.Comp.Range < 10.0));
    short num1;
    short num2;
    if (!this._area.CanOrbitalBombard(coordinates, out bool _))
    {
      num1 = (short) 4;
      num2 = flag1 ? (short) 7 : (short) 5;
    }
    else if (!this._area.CanCAS(coordinates))
    {
      num1 = (short) 3;
      num2 = flag2 ? (short) 6 : (short) 4;
    }
    else if (!this._area.CanSupplyDrop(this._transform.ToMapCoordinates(coordinates)) || !this._area.CanMortarFire(coordinates))
    {
      num1 = (short) 2;
      num2 = (short) 3;
    }
    else if (!this._area.CanMortarPlacement(coordinates) || !this._area.CanLase(coordinates) || !this._area.CanMedevac(coordinates) || !this._area.CanParadrop(coordinates))
    {
      num1 = (short) 1;
      num2 = (short) 2;
    }
    else
    {
      num1 = (short) 0;
      num2 = (short) 1;
    }
    List<string> values1 = new List<string>();
    List<string> values2 = new List<string>();
    if (this._area.CanOrbitalBombard(coordinates, out bool _))
      values1.Add("Orbital Strike");
    else
      values2.Add("Orbital Strike");
    if (this._area.CanCAS(coordinates))
      values1.Add("Close Air Support");
    else
      values2.Add("Close Air Support");
    if (this._area.CanSupplyDrop(this._transform.ToMapCoordinates(coordinates)))
      values1.Add("Supply Drops");
    else
      values2.Add("Supply Drops");
    if (this._area.CanMortarFire(coordinates))
      values1.Add("Mortar Fire");
    else
      values2.Add("Mortar Fire");
    if (this._area.CanMortarPlacement(coordinates))
      values1.Add("Mortar Placement");
    else
      values2.Add("Mortar Placement");
    if (this._area.CanLase(coordinates))
      values1.Add("Laser Designation");
    else
      values2.Add("Laser Designation");
    if (area.Value.Comp.Medevac)
      values1.Add("Casualty Evacuation");
    else
      values2.Add("Casualty Evacuation");
    if (area.Value.Comp.Paradropping)
      values1.Add("Paradropping");
    else
      values2.Add("Paradropping");
    if (area.Value.Comp.NoTunnel)
      values2.Add("Tunneling");
    if (area.Value.Comp.Unweedable)
      values2.Add("Weed Placement");
    else if (!area.Value.Comp.ResinAllowed)
      values2.Add("Resin Structures");
    string str1 = "";
    if (flag1)
      str1 = "\nProtection: Hive Core";
    else if (flag2)
      str1 = "\nProtection: Hive Pylon";
    string str2 = $"\nCeiling level: {num1}{str1}";
    if (values1.Count > 0)
      str2 = $"{str2 + "\n\nAllowed:"}\n• {string.Join("\n• ", (IEnumerable<string>) values1)}";
    if (values2.Count > 0)
      str2 = $"{str2 + "\n\nBlocked:"}\n• {string.Join("\n• ", (IEnumerable<string>) values2)}";
    return (areaPrototype.Name, num2, str2);
  }

  private bool IsProtectedByRoofing(
    EntityCoordinates coordinates,
    Predicate<Entity<RoofingEntityComponent>> predicate)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RoofingEntityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RoofingEntityComponent>();
    EntityUid uid;
    RoofingEntityComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      float distance;
      if (predicate((Entity<RoofingEntityComponent>) (uid, comp1)) && coordinates.TryDistance(this._entityManager, uid.ToCoordinates(), out distance) && (double) distance <= (double) comp1.Range)
        return true;
    }
    return false;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (this._marineAlertCopyQueue.Count > 0)
    {
      Entity<AreaInfoComponent> result;
      while (this._marineAlertCopyQueue.TryDequeue(out result))
      {
        if (this._timing.CurTime >= curTime + this._maxProcessTime)
          return;
        if (!this.TerminatingOrDeleted((EntityUid) result))
        {
          (string areaName, short ceilingLevel, string restrictions) = this.GetAreaInfo((EntityUid) result);
          AlertsSystem alerts = this._alerts;
          EntityUid euid = (EntityUid) result;
          ProtoId<AlertPrototype> alert = result.Comp.Alert;
          short? severity = new short?(ceilingLevel);
          string str = this.Loc.GetString("rmc-area-info", ("area", (object) areaName), ("ceilingLevel", (object) ceilingLevel), ("restrictions", (object) restrictions));
          (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
          string dynamicMessage = str;
          alerts.ShowAlert(euid, alert, severity, cooldown, dynamicMessage: dynamicMessage);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<AreaInfoComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AreaInfoComponent>();
    EntityUid uid;
    AreaInfoComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.NextUpdateTime))
      {
        this._marineAlertCopyQueue.Enqueue((Entity<AreaInfoComponent>) (uid, comp1));
        comp1.NextUpdateTime = curTime + comp1.UpdateInterval;
      }
    }
  }
}
