// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Aircraft.AircraftSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Explosion.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Aircraft;

public sealed class AircraftSystem : EntitySystem
{
  [Dependency]
  private readonly SharedActionsSystem _actions;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  [Dependency]
  private readonly DamageableSystem _damageable;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SharedContentEyeSystem _contentEye;
  [Dependency]
  private readonly SharedEyeSystem _eye;
  [Dependency]
  private readonly AlertsSystem _alerts;
  [Dependency]
  private readonly VehicleTopologySystem _topology;
  [Dependency]
  private readonly GridVehicleMoverSystem _mover;
  private const int WallMask = 67108894 /*0x0400001E*/;
  private const string AltitudeAlertId = "AircraftAltitude";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AircraftComponent, VehicleOperatorSetEvent>(new EntityEventRefHandler<AircraftComponent, VehicleOperatorSetEvent>(this.OnOperatorSet));
    this.SubscribeLocalEvent<AircraftPilotActionComponent, AircraftAscendActionEvent>(new EntityEventRefHandler<AircraftPilotActionComponent, AircraftAscendActionEvent>(this.OnAscend));
    this.SubscribeLocalEvent<AircraftPilotActionComponent, AircraftDescendActionEvent>(new EntityEventRefHandler<AircraftPilotActionComponent, AircraftDescendActionEvent>(this.OnDescend));
    this.SubscribeLocalEvent<AircraftPilotActionComponent, AircraftBombActionEvent>(new EntityEventRefHandler<AircraftPilotActionComponent, AircraftBombActionEvent>(this.OnBomb));
    this.SubscribeLocalEvent<AircraftPilotActionComponent, ComponentShutdown>(new EntityEventRefHandler<AircraftPilotActionComponent, ComponentShutdown>(this.OnPilotShutdown));
    this.SubscribeLocalEvent<AircraftCannonComponent, AmmoShotEvent>(new EntityEventRefHandler<AircraftCannonComponent, AmmoShotEvent>(this.OnCannonShot));
    this.SubscribeLocalEvent<AircraftComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<AircraftComponent, BeforeDamageChangedEvent>(this.OnBeforeDamage), new Type[1]
    {
      typeof (Content.Shared.Vehicle.VehicleSystem)
    });
    this.SubscribeLocalEvent<AircraftComponent, ComponentShutdown>(new EntityEventRefHandler<AircraftComponent, ComponentShutdown>(this.OnAircraftShutdown));
  }

  private void OnOperatorSet(Entity<AircraftComponent> ent, ref VehicleOperatorSetEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid? nullable = args.OldOperator;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      EntityUid entityUid = valueOrDefault;
      nullable = args.NewOperator;
      if ((nullable.HasValue ? (entityUid != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        this.ClearOperatorView(valueOrDefault);
        this.RemCompDeferred<AircraftPilotActionComponent>(valueOrDefault);
      }
    }
    nullable = args.NewOperator;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    AircraftPilotActionComponent pilotActionComponent1 = this.EnsureComp<AircraftPilotActionComponent>(valueOrDefault1);
    pilotActionComponent1.Vehicle = new EntityUid?(ent.Owner);
    AircraftPilotActionComponent pilotActionComponent2 = pilotActionComponent1;
    nullable = pilotActionComponent2.AscendAction;
    if (!nullable.HasValue)
      pilotActionComponent2.AscendAction = this._actions.AddAction(valueOrDefault1, (string) ent.Comp.AscendActionId);
    AircraftPilotActionComponent pilotActionComponent3 = pilotActionComponent1;
    nullable = pilotActionComponent3.DescendAction;
    if (!nullable.HasValue)
      pilotActionComponent3.DescendAction = this._actions.AddAction(valueOrDefault1, (string) ent.Comp.DescendActionId);
    AircraftPilotActionComponent pilotActionComponent4 = pilotActionComponent1;
    nullable = pilotActionComponent4.BombAction;
    if (!nullable.HasValue)
      pilotActionComponent4.BombAction = this._actions.AddAction(valueOrDefault1, (string) ent.Comp.BombActionId);
    this.Dirty(valueOrDefault1, (IComponent) pilotActionComponent1);
    this.ApplyOperatorView(ent.Owner, ent.Comp);
    this.RefreshActionStates(ent.Owner);
  }

  private void OnPilotShutdown(Entity<AircraftPilotActionComponent> ent, ref ComponentShutdown args)
  {
    EntityUid? ascendAction = ent.Comp.AscendAction;
    if (ascendAction.HasValue)
    {
      EntityUid valueOrDefault = ascendAction.GetValueOrDefault();
      this._actions.RemoveAction((Entity<ActionsComponent>) ent.Owner, new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault));
    }
    EntityUid? descendAction = ent.Comp.DescendAction;
    if (descendAction.HasValue)
    {
      EntityUid valueOrDefault = descendAction.GetValueOrDefault();
      this._actions.RemoveAction((Entity<ActionsComponent>) ent.Owner, new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault));
    }
    EntityUid? bombAction = ent.Comp.BombAction;
    if (bombAction.HasValue)
    {
      EntityUid valueOrDefault = bombAction.GetValueOrDefault();
      this._actions.RemoveAction((Entity<ActionsComponent>) ent.Owner, new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault));
    }
    this.ClearOperatorView(ent.Owner);
  }

  private void OnAscend(
    Entity<AircraftPilotActionComponent> ent,
    ref AircraftAscendActionEvent args)
  {
    if (this._net.IsClient || args.Handled || args.Performer != ent.Owner)
      return;
    args.Handled = true;
    EntityUid? vehicle = ent.Comp.Vehicle;
    if (!vehicle.HasValue)
      return;
    EntityUid valueOrDefault = vehicle.GetValueOrDefault();
    AircraftComponent comp1;
    if (!this.TryComp<AircraftComponent>(valueOrDefault, out comp1) || comp1.Altitude >= comp1.MaxAltitude)
      return;
    GridVehicleMoverComponent comp2;
    if (comp1.Altitude == 0 && (!this.TryComp<GridVehicleMoverComponent>(valueOrDefault, out comp2) || (double) comp2.CurrentSpeed < (double) comp1.TakeoffSpeed))
      this._popup.PopupClient(this.Loc.GetString("rmc-aircraft-need-speed"), ent.Owner, new EntityUid?(ent.Owner), PopupType.SmallCaution);
    else
      this.SetAltitude(valueOrDefault, comp1, comp1.Altitude + 1);
  }

  private void OnDescend(
    Entity<AircraftPilotActionComponent> ent,
    ref AircraftDescendActionEvent args)
  {
    if (this._net.IsClient || args.Handled || args.Performer != ent.Owner)
      return;
    args.Handled = true;
    EntityUid? vehicle = ent.Comp.Vehicle;
    if (!vehicle.HasValue)
      return;
    EntityUid valueOrDefault = vehicle.GetValueOrDefault();
    AircraftComponent comp;
    if (!this.TryComp<AircraftComponent>(valueOrDefault, out comp) || comp.Altitude <= 0)
      return;
    if (comp.Altitude > 1)
    {
      this.SetAltitude(valueOrDefault, comp, comp.Altitude - 1);
    }
    else
    {
      this._physics.SetCanCollide(valueOrDefault, true, force: true);
      if (this._mover.CanOccupyCurrent(valueOrDefault))
        this.SetAltitude(valueOrDefault, comp, 0);
      else
        this.CrashLand(valueOrDefault, comp);
    }
  }

  private void OnBomb(Entity<AircraftPilotActionComponent> ent, ref AircraftBombActionEvent args)
  {
    if (this._net.IsClient || args.Handled || args.Performer != ent.Owner)
      return;
    args.Handled = true;
    EntityUid? vehicle = ent.Comp.Vehicle;
    if (!vehicle.HasValue)
      return;
    EntityUid valueOrDefault = vehicle.GetValueOrDefault();
    AircraftComponent comp;
    if (!this.TryComp<AircraftComponent>(valueOrDefault, out comp) || comp.Altitude <= 0)
      return;
    this.EnsureComp<ActiveTimerTriggerComponent>(this.Spawn((string) comp.BombProto, this.Transform(valueOrDefault).Coordinates)).TimeRemaining = (float) comp.Altitude * comp.BombFallTimePerLevel;
  }

  private void OnCannonShot(Entity<AircraftCannonComponent> ent, ref AmmoShotEvent args)
  {
    EntityUid vehicle;
    AircraftComponent comp1;
    if (!this._topology.TryGetVehicle(ent.Owner, out vehicle) || !this.TryComp<AircraftComponent>(vehicle, out comp1) || comp1.Altitude < ent.Comp.IgnoreWallsFromAltitude)
      return;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      this.EnsureComp<HighAltitudeProjectileComponent>(firedProjectile);
      FixturesComponent comp2;
      if (this.TryComp<FixturesComponent>(firedProjectile, out comp2))
      {
        foreach ((string str, Fixture fixture) in comp2.Fixtures)
        {
          int mask = fixture.CollisionMask & -67108895;
          if (mask != fixture.CollisionMask)
            this._physics.SetCollisionMask(firedProjectile, str, fixture, mask, comp2);
        }
      }
    }
  }

  private void SetAltitude(EntityUid vehicle, AircraftComponent aircraft, int newAltitude)
  {
    aircraft.Altitude = Math.Clamp(newAltitude, 0, aircraft.MaxAltitude);
    bool flag = aircraft.Altitude > 0;
    this._physics.SetCanCollide(vehicle, !flag, force: true);
    PhysicsComponent comp;
    if (this.TryComp<PhysicsComponent>(vehicle, out comp))
      this._physics.SetBodyStatus(vehicle, comp, flag ? BodyStatus.InAir : BodyStatus.OnGround);
    this.Dirty(vehicle, (IComponent) aircraft);
    this.ApplyOperatorView(vehicle, aircraft);
    this.RefreshActionStates(vehicle);
  }

  private void ApplyOperatorView(EntityUid vehicle, AircraftComponent aircraft)
  {
    VehicleComponent comp;
    if (!this.TryComp<VehicleComponent>(vehicle, out comp))
      return;
    EntityUid? nullable = comp.Operator;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    this._alerts.ShowAlert(valueOrDefault, aircraft.AltitudeAlert, new short?((short) aircraft.Altitude));
    bool flag = aircraft.Altitude > 0;
    this._eye.SetDrawFov(valueOrDefault, !flag);
    if (flag)
    {
      float num = (float) (1.0 + (double) aircraft.Altitude * (double) aircraft.ZoomPerLevel);
      Vector2 zoom = new Vector2(num, num);
      this._contentEye.SetMaxZoom(valueOrDefault, zoom);
      this._contentEye.SetZoom(valueOrDefault, zoom);
    }
    else
      this._contentEye.ResetZoom(valueOrDefault);
  }

  private void ClearOperatorView(EntityUid op)
  {
    this._contentEye.ResetZoom(op);
    this._eye.SetDrawFov(op, true);
    this._alerts.ClearAlert(op, (ProtoId<AlertPrototype>) "AircraftAltitude");
  }

  private void CrashLand(EntityUid vehicle, AircraftComponent aircraft)
  {
    aircraft.Altitude = 0;
    this._physics.SetCanCollide(vehicle, true, force: true);
    PhysicsComponent comp1;
    if (this.TryComp<PhysicsComponent>(vehicle, out comp1))
      this._physics.SetBodyStatus(vehicle, comp1, BodyStatus.OnGround);
    this.Dirty(vehicle, (IComponent) aircraft);
    if (aircraft.CrashSound != null)
      this._audio.PlayPvs(aircraft.CrashSound, vehicle);
    VehicleComponent comp2;
    if (this.TryComp<VehicleComponent>(vehicle, out comp2))
    {
      EntityUid? nullable = comp2.Operator;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        DamageSpecifier damageSpecifier = new DamageSpecifier()
        {
          DamageDict = {
            ["Blunt"] = (FixedPoint2) 500
          }
        };
        DamageableSystem damageable = this._damageable;
        EntityUid? uid = new EntityUid?(valueOrDefault);
        DamageSpecifier damage = damageSpecifier;
        EntityUid? origin = new EntityUid?(vehicle);
        nullable = new EntityUid?();
        EntityUid? tool = nullable;
        damageable.TryChangeDamage(uid, damage, true, origin: origin, tool: tool);
      }
    }
    this.RaiseLocalEvent<RMCVehicleFrameDestroyedEvent>(vehicle, new RMCVehicleFrameDestroyedEvent(vehicle));
    this.ApplyOperatorView(vehicle, aircraft);
    this.RefreshActionStates(vehicle);
  }

  private void OnBeforeDamage(Entity<AircraftComponent> ent, ref BeforeDamageChangedEvent args)
  {
    if (!ent.Comp.Airborne || args.Cancelled)
      return;
    args.Cancelled = true;
  }

  private void OnAircraftShutdown(Entity<AircraftComponent> ent, ref ComponentShutdown args)
  {
    if (!ent.Comp.Airborne)
      return;
    this._physics.SetCanCollide(ent.Owner, true, force: true);
  }

  private void RefreshActionStates(EntityUid vehicle)
  {
    AircraftComponent comp1;
    VehicleComponent comp2;
    if (!this.TryComp<AircraftComponent>(vehicle, out comp1) || !this.TryComp<VehicleComponent>(vehicle, out comp2))
      return;
    EntityUid? nullable = comp2.Operator;
    AircraftPilotActionComponent comp3;
    if (!nullable.HasValue || !this.TryComp<AircraftPilotActionComponent>(nullable.GetValueOrDefault(), out comp3))
      return;
    EntityUid? ascendAction = comp3.AscendAction;
    if (ascendAction.HasValue)
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ascendAction.GetValueOrDefault()), comp1.Altitude > 0);
    EntityUid? descendAction = comp3.DescendAction;
    if (!descendAction.HasValue)
      return;
    this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) descendAction.GetValueOrDefault()), comp1.Altitude > 0);
  }
}
