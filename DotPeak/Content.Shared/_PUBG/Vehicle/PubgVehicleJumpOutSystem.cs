// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Vehicle.PubgVehicleJumpOutSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Vehicle;

public sealed class PubgVehicleJumpOutSystem : EntitySystem
{
  private const string ActionId = "ActionPubgVehicleJumpOut";
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private VehicleSystem _vehicles;
  private static readonly DamageSpecifier JumpDamage = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      ["Blunt"] = (FixedPoint2) 20
    }
  };

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCVehicleInteriorOccupantComponent, ComponentStartup>(new EntityEventRefHandler<RMCVehicleInteriorOccupantComponent, ComponentStartup>(this.OnOccupantAdded));
    this.SubscribeLocalEvent<RMCVehicleInteriorOccupantComponent, ComponentShutdown>(new EntityEventRefHandler<RMCVehicleInteriorOccupantComponent, ComponentShutdown>(this.OnOccupantRemoved));
    this.SubscribeLocalEvent<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutActionEvent>(new EntityEventRefHandler<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutActionEvent>(this.OnJumpOutAction));
    this.SubscribeLocalEvent<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutDoAfterEvent>(new EntityEventRefHandler<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutDoAfterEvent>(this.OnJumpOutDoAfter));
  }

  private void OnOccupantAdded(
    Entity<RMCVehicleInteriorOccupantComponent> ent,
    ref ComponentStartup args)
  {
    if (this._net.IsClient)
      return;
    PubgVehicleJumpOutOccupantComponent occupantComponent = this.EnsureComp<PubgVehicleJumpOutOccupantComponent>(ent.Owner);
    EntityUid? actionId = new EntityUid?();
    this._actions.AddAction(ent.Owner, ref actionId, "ActionPubgVehicleJumpOut");
    EntityUid? nullable = actionId;
    occupantComponent.ActionEntity = nullable;
  }

  private void OnOccupantRemoved(
    Entity<RMCVehicleInteriorOccupantComponent> ent,
    ref ComponentShutdown args)
  {
    PubgVehicleJumpOutOccupantComponent comp;
    if (this._net.IsClient || !this.TryComp<PubgVehicleJumpOutOccupantComponent>(ent.Owner, out comp))
      return;
    EntityUid? actionEntity = comp.ActionEntity;
    if (actionEntity.HasValue)
    {
      EntityUid valueOrDefault = actionEntity.GetValueOrDefault();
      this._actions.RemoveAction((Entity<ActionsComponent>) ent.Owner, new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault));
    }
    this.RemCompDeferred<PubgVehicleJumpOutOccupantComponent>(ent.Owner);
  }

  private void OnJumpOutAction(
    Entity<PubgVehicleJumpOutOccupantComponent> ent,
    ref PubgVehicleJumpOutActionEvent args)
  {
    RMCVehicleInteriorOccupantComponent comp1;
    VehicleEnterComponent comp2;
    if (!this.TryComp<RMCVehicleInteriorOccupantComponent>(ent.Owner, out comp1) || !comp1.Vehicle.HasValue || !this.TryComp<VehicleEnterComponent>(comp1.Vehicle.Value, out comp2))
      return;
    float seconds = Math.Max(0.0f, comp2.ExitDoAfter / 2f);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, ent.Owner, seconds, (DoAfterEvent) new PubgVehicleJumpOutDoAfterEvent(), new EntityUid?(ent.Owner))
    {
      BreakOnMove = false
    });
    args.Handled = true;
  }

  private void OnJumpOutDoAfter(
    Entity<PubgVehicleJumpOutOccupantComponent> ent,
    ref PubgVehicleJumpOutDoAfterEvent args)
  {
    RMCVehicleInteriorOccupantComponent comp;
    if (args.Cancelled || args.Handled || !this.TryComp<RMCVehicleInteriorOccupantComponent>(ent.Owner, out comp) || !comp.Vehicle.HasValue)
      return;
    args.Handled = true;
    if (!this._vehicles.TryExitFromInterior(ent.Owner, comp.Vehicle.Value))
      return;
    this._damageable.TryChangeDamage(new EntityUid?(ent.Owner), PubgVehicleJumpOutSystem.JumpDamage);
  }
}
