// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleLockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleLockSystem : EntitySystem
{
  [Dependency]
  private readonly SharedActionsSystem _actions;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleEnterComponent, MapInitEvent>(new EntityEventRefHandler<VehicleEnterComponent, MapInitEvent>(this.OnVehicleMapInit));
    this.SubscribeLocalEvent<VehicleLockActionComponent, VehicleLockActionEvent>(new EntityEventRefHandler<VehicleLockActionComponent, VehicleLockActionEvent>(this.OnLockAction));
    this.SubscribeLocalEvent<VehicleLockActionComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleLockActionComponent, ComponentShutdown>(this.OnLockActionShutdown));
  }

  private void OnVehicleMapInit(Entity<VehicleEnterComponent> ent, ref MapInitEvent args)
  {
    if (this._net.IsClient)
      return;
    this.EnsureComp<VehicleLockComponent>(ent.Owner);
  }

  public void EnableLockAction(EntityUid user, EntityUid vehicle)
  {
    VehicleLockActionComponent lockActionComponent = this.EnsureComp<VehicleLockActionComponent>(user);
    lockActionComponent.Sources.Add(vehicle);
    lockActionComponent.Vehicle = new EntityUid?(vehicle);
    VehicleLockComponent vehicleLockComponent = this.EnsureComp<VehicleLockComponent>(vehicle);
    if (!lockActionComponent.Action.HasValue || this.TerminatingOrDeleted(lockActionComponent.Action.Value))
      lockActionComponent.Action = this._actions.AddAction(user, (string) lockActionComponent.ActionId);
    EntityUid? action = lockActionComponent.Action;
    if (action.HasValue)
    {
      EntityUid valueOrDefault = action.GetValueOrDefault();
      this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault), true);
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault), vehicleLockComponent.Locked);
    }
    this.Dirty(user, (IComponent) lockActionComponent);
  }

  public void DisableLockAction(EntityUid user, EntityUid vehicle)
  {
    VehicleLockActionComponent comp1;
    if (!this.TryComp<VehicleLockActionComponent>(user, out comp1))
      return;
    comp1.Sources.Remove(vehicle);
    if (comp1.Sources.Count > 0)
    {
      using (HashSet<EntityUid>.Enumerator enumerator = comp1.Sources.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          EntityUid current = enumerator.Current;
          comp1.Vehicle = new EntityUid?(current);
        }
      }
      EntityUid? action = comp1.Action;
      if (action.HasValue)
      {
        EntityUid valueOrDefault = action.GetValueOrDefault();
        EntityUid? vehicle1 = comp1.Vehicle;
        VehicleLockComponent comp2;
        if (vehicle1.HasValue && this.TryComp<VehicleLockComponent>(vehicle1.GetValueOrDefault(), out comp2))
          this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) valueOrDefault), comp2.Locked);
      }
      this.Dirty(user, (IComponent) comp1);
    }
    else
    {
      if (comp1.Action.HasValue)
      {
        this._actions.RemoveAction((Entity<ActionsComponent>) user, new Entity<ActionComponent>?((Entity<ActionComponent>) comp1.Action.Value));
        comp1.Action = new EntityUid?();
      }
      this.RemCompDeferred<VehicleLockActionComponent>(user);
    }
  }

  private void OnLockActionShutdown(
    Entity<VehicleLockActionComponent> ent,
    ref ComponentShutdown args)
  {
    EntityUid? action = ent.Comp.Action;
    if (!action.HasValue)
      return;
    this._actions.RemoveAction(new Entity<ActionComponent>?((Entity<ActionComponent>) action.GetValueOrDefault()));
  }

  private void OnLockAction(Entity<VehicleLockActionComponent> ent, ref VehicleLockActionEvent args)
  {
    if (this._net.IsClient || args.Handled || args.Performer != ent.Owner)
      return;
    args.Handled = true;
    EntityUid? nullable = ent.Comp.Vehicle;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    if (this.Deleted(valueOrDefault))
      return;
    VehicleComponent comp;
    if (this.TryComp<VehicleComponent>(valueOrDefault, out comp))
    {
      nullable = comp.Operator;
      EntityUid owner = ent.Owner;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) == 0)
      {
        VehicleLockComponent vehicleLockComponent = this.EnsureComp<VehicleLockComponent>(valueOrDefault);
        vehicleLockComponent.Locked = !vehicleLockComponent.Locked;
        nullable = ent.Comp.Action;
        if (nullable.HasValue)
          this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()), vehicleLockComponent.Locked);
        this._popup.PopupEntity(this.Loc.GetString(vehicleLockComponent.Locked ? "rmc-vehicle-lock-set-locked" : "rmc-vehicle-lock-set-unlocked"), ent.Owner, ent.Owner);
        return;
      }
    }
    this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-lock-not-driver"), ent.Owner, ent.Owner, PopupType.SmallCaution);
  }
}
