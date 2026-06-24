// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.Xeno.HiveTrackerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Alert;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Tracker.Xeno;

public sealed class HiveTrackerSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private IComponentFactory _factory;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private TrackerSystem _tracker;
  [Dependency]
  private SquadLeaderTrackerSystem _squadLeaderTrackerSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedXenoWatchSystem _xenoWatch;
  private const string HiveTrackerCategory = "HiveTracker";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<HiveTrackerComponent, ComponentRemove>(new EntityEventRefHandler<HiveTrackerComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<HiveTrackerComponent, HiveTrackerClickedAlertEvent>(new EntityEventRefHandler<HiveTrackerComponent, HiveTrackerClickedAlertEvent>(this.OnClickedAlert));
    this.SubscribeLocalEvent<HiveTrackerComponent, HiveTrackerAltClickedAlertEvent>(new EntityEventRefHandler<HiveTrackerComponent, HiveTrackerAltClickedAlertEvent>(this.OnAltClickedAlert));
    this.SubscribeLocalEvent<HiveTrackerComponent, HiveTrackerChangeModeEvent>(new EntityEventRefHandler<HiveTrackerComponent, HiveTrackerChangeModeEvent>(this.OnHiveTrackerChangeMode));
    this.SubscribeLocalEvent<HiveTrackerComponent, LeaderTrackerSelectTargetEvent>(new EntityEventRefHandler<HiveTrackerComponent, LeaderTrackerSelectTargetEvent>(this.OnHiveTrackerSelectTarget));
    this.SubscribeLocalEvent<RMCTrackableComponent, RequestTrackableNameEvent>(new EntityEventRefHandler<RMCTrackableComponent, RequestTrackableNameEvent>(this.OnRequestTrackableName));
    this.SubscribeLocalEvent<RMCTrackableComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCTrackableComponent, MobStateChangedEvent>(this.OnMobStateChanged));
  }

  private void OnRemove(Entity<HiveTrackerComponent> ent, ref ComponentRemove args)
  {
    TrackerModePrototype prototype;
    this._prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, out prototype);
    if (prototype == null)
      return;
    this._alerts.ClearAlert((EntityUid) ent, prototype.Alert);
  }

  private void OnClickedAlert(
    Entity<HiveTrackerComponent> ent,
    ref HiveTrackerClickedAlertEvent args)
  {
    Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (!hive1.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive1.GetValueOrDefault();
    EntityUid? uid = new EntityUid?();
    HiveMemberComponent comp;
    if (this.TryComp<HiveMemberComponent>(ent.Comp.Target, out comp))
    {
      EntityUid? hive2 = comp.Hive;
      EntityUid owner = valueOrDefault.Owner;
      if ((hive2.HasValue ? (hive2.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
        uid = new EntityUid?(ent.Comp.Target.Value);
    }
    if (!this.HasComp<XenoComponent>(uid))
      uid = valueOrDefault.Comp.CurrentQueen;
    if (!uid.HasValue)
      return;
    args.Handled = true;
    this._xenoWatch.Watch((Entity<HiveMemberComponent, ActorComponent, EyeComponent>) ent.Owner, (Entity<HiveMemberComponent>) uid.Value);
  }

  private void OnAltClickedAlert(
    Entity<HiveTrackerComponent> ent,
    ref HiveTrackerAltClickedAlertEvent args)
  {
    List<DialogOption> options = new List<DialogOption>();
    foreach (ProtoId<TrackerModePrototype> trackerMode in ent.Comp.TrackerModes)
      options.Add(new DialogOption(this.Loc.GetString("rmc-xeno-tracker-target-" + (string) trackerMode), (object) new HiveTrackerChangeModeEvent(trackerMode)));
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-squad-info-tracking-selection"), options, this.Loc.GetString("rmc-squad-info-tracking-choose"));
  }

  private void OnHiveTrackerChangeMode(
    Entity<HiveTrackerComponent> ent,
    ref HiveTrackerChangeModeEvent args)
  {
    HiveMemberComponent comp1;
    if (!this._timing.IsFirstTimePredicted || !this.TryComp<HiveMemberComponent>(ent.Owner, out comp1))
      return;
    List<DialogOption> options;
    List<EntityUid> trackingOptions;
    this._squadLeaderTrackerSystem.TryFindTargets(args.Mode, out options, out trackingOptions);
    int index = 0;
    while (index < trackingOptions.Count)
    {
      HiveMemberComponent comp2;
      if (this.TryComp<HiveMemberComponent>(trackingOptions[index], out comp2))
      {
        EntityUid? hive1 = comp2.Hive;
        EntityUid? hive2 = comp1.Hive;
        if ((hive1.HasValue == hive2.HasValue ? (hive1.HasValue ? (hive1.GetValueOrDefault() != hive2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
        {
          ++index;
          continue;
        }
      }
      options.RemoveAt(index);
      trackingOptions.RemoveAt(index);
    }
    this._dialog.OpenOptions((EntityUid) ent, this.Loc.GetString("rmc-squad-info-tracking-selection"), options, this.Loc.GetString("rmc-squad-info-tracking-choose"));
  }

  private void OnHiveTrackerSelectTarget(
    Entity<HiveTrackerComponent> ent,
    ref LeaderTrackerSelectTargetEvent args)
  {
    this.SetTarget(ent, new EntityUid?(this.GetEntity(args.Target)));
    this.SetMode(ent, args.Mode);
    this.Dirty<HiveTrackerComponent>(ent);
  }

  private void SetTarget(Entity<HiveTrackerComponent> ent, EntityUid? target)
  {
    ent.Comp.Target = target;
    this.Dirty<HiveTrackerComponent>(ent);
  }

  private void SetMode(Entity<HiveTrackerComponent> ent, ProtoId<TrackerModePrototype> mode)
  {
    ent.Comp.Mode = new ProtoId<TrackerModePrototype>?(mode);
    this.Dirty<HiveTrackerComponent>(ent);
  }

  private void UpdateDirection(Entity<HiveTrackerComponent> ent, MapCoordinates? coordinates = null)
  {
    this._alerts.ClearAlertCategory((EntityUid) ent, (ProtoId<AlertCategoryPrototype>) "HiveTracker");
    TrackerModePrototype prototype;
    this._prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, out prototype);
    if (prototype == null)
      return;
    ProtoId<AlertPrototype> alert = prototype.Alert;
    short num = TrackerSystem.CenterSeverity;
    if (coordinates.HasValue)
      num = this._tracker.GetAlertSeverity(ent.Owner, coordinates.Value);
    this._alerts.ShowAlert(ent.Owner, alert, new short?(num));
  }

  private void OnRequestTrackableName(
    Entity<RMCTrackableComponent> ent,
    ref RequestTrackableNameEvent args)
  {
    if (args.Handled)
      return;
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (!hive.HasValue)
      return;
    foreach (KeyValuePair<string, EntityUid> hiveTunnel in hive.Value.Comp.HiveTunnels)
    {
      if (!(hiveTunnel.Value != ent.Owner))
      {
        args.Name = hiveTunnel.Key;
        break;
      }
    }
    args.Handled = true;
  }

  private void OnMobStateChanged(Entity<RMCTrackableComponent> ent, ref MobStateChangedEvent args)
  {
    if (!this.HasComp<XenoComponent>((EntityUid) ent) || args.NewMobState != MobState.Dead)
      return;
    this.RemCompDeferred<RMCTrackableComponent>((EntityUid) ent);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveTrackerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<HiveTrackerComponent>();
    EntityUid uid1;
    HiveTrackerComponent comp1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1))
    {
      if (!(curTime < comp1.UpdateAt))
      {
        comp1.UpdateAt = curTime + comp1.UpdateEvery;
        EntityUid? nullable;
        if (comp1.Target.HasValue)
        {
          if (!this.HasComp<RMCTrackableComponent>(comp1.Target.Value))
          {
            Entity<HiveTrackerComponent> ent = (Entity<HiveTrackerComponent>) (uid1, comp1);
            nullable = new EntityUid?();
            EntityUid? target = nullable;
            this.SetTarget(ent, target);
          }
          else
            this.UpdateDirection((Entity<HiveTrackerComponent>) (uid1, comp1), new MapCoordinates?(this._transform.GetMapCoordinates(comp1.Target.Value)));
        }
        else
        {
          Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTrackableComponent, HiveMemberComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCTrackableComponent, HiveMemberComponent>();
          EntityUid uid2;
          HiveMemberComponent comp2;
          HiveMemberComponent comp;
          while (entityQueryEnumerator2.MoveNext(out uid2, out RMCTrackableComponent _, out comp2) && this.TryComp<HiveMemberComponent>(uid1, out comp))
          {
            TrackerModePrototype prototype;
            this._prototypeManager.TryIndex<TrackerModePrototype>(comp1.Mode, out prototype);
            if (prototype != null && prototype.Component != null)
            {
              nullable = comp.Hive;
              EntityUid? hive = comp2.Hive;
              if ((nullable.HasValue == hive.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != hive.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
              {
                Type type = this._factory.GetComponent(prototype.Component).GetType();
                if (this.EntityManager.TryGetComponent(uid2, type, out IComponent _))
                {
                  this.SetTarget((Entity<HiveTrackerComponent>) (uid1, comp1), new EntityUid?(uid2));
                  if (comp1.Target.HasValue)
                  {
                    this.UpdateDirection((Entity<HiveTrackerComponent>) (uid1, comp1), new MapCoordinates?(this._transform.GetMapCoordinates(comp1.Target.Value)));
                    break;
                  }
                  break;
                }
                break;
              }
            }
            else
              break;
          }
          this.UpdateDirection((Entity<HiveTrackerComponent>) (uid1, comp1));
        }
      }
    }
  }
}
