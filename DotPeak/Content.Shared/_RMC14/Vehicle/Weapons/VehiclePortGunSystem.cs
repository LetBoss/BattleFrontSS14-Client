// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehiclePortGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Buckle.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehiclePortGunSystem : EntitySystem
{
  [Dependency]
  private readonly SharedEyeSystem _eye;
  [Dependency]
  private readonly SharedHandsSystem _hands;
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SkillsSystem _skills;
  [Dependency]
  private readonly VehicleSystem _vehicleSystem;
  [Dependency]
  private readonly VehicleViewToggleSystem _viewToggle;
  [Dependency]
  private readonly INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehiclePortGunSeatComponent, StrapAttemptEvent>(new EntityEventRefHandler<VehiclePortGunSeatComponent, StrapAttemptEvent>(this.OnPortGunSeatStrapAttempt));
    this.SubscribeLocalEvent<VehiclePortGunSeatComponent, UnstrappedEvent>(new EntityEventRefHandler<VehiclePortGunSeatComponent, UnstrappedEvent>(this.OnPortGunSeatUnstrapped));
    this.SubscribeLocalEvent<VehiclePortGunControllerComponent, InteractHandEvent>(new EntityEventRefHandler<VehiclePortGunControllerComponent, InteractHandEvent>(this.OnPortGunInteractHand));
    this.SubscribeLocalEvent<VehiclePortGunControllerComponent, InteractUsingEvent>(new EntityEventRefHandler<VehiclePortGunControllerComponent, InteractUsingEvent>(this.OnPortGunInteractUsing));
    this.SubscribeLocalEvent<VehiclePortGunControllerComponent, ExaminedEvent>(new EntityEventRefHandler<VehiclePortGunControllerComponent, ExaminedEvent>(this.OnPortGunExamined));
    this.SubscribeLocalEvent<VehiclePortGunControllerComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<VehiclePortGunControllerComponent, GetVerbsEvent<AlternativeVerb>>(this.OnPortGunVerbs));
    this.SubscribeLocalEvent<VehiclePortGunComponent, ComponentShutdown>(new EntityEventRefHandler<VehiclePortGunComponent, ComponentShutdown>(this.OnPortGunShutdown));
    this.SubscribeLocalEvent<VehiclePortGunComponent, GunShotEvent>(new EntityEventRefHandler<VehiclePortGunComponent, GunShotEvent>(this.OnPortGunShot));
    this.SubscribeLocalEvent<VehiclePortGunComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<VehiclePortGunComponent, EntInsertedIntoContainerMessage>(this.OnPortGunContainerInserted));
    this.SubscribeLocalEvent<VehiclePortGunComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<VehiclePortGunComponent, EntRemovedFromContainerMessage>(this.OnPortGunContainerRemoved));
    this.SubscribeLocalEvent<VehiclePortGunOperatorComponent, ComponentShutdown>(new EntityEventRefHandler<VehiclePortGunOperatorComponent, ComponentShutdown>(this.OnPortGunOperatorShutdown));
  }

  private void OnPortGunSeatStrapAttempt(
    Entity<VehiclePortGunSeatComponent> ent,
    ref StrapAttemptEvent args)
  {
    if (args.Cancelled || this._skills.HasSkills((Entity<SkillsComponent>) args.Buckle.Owner, ent.Comp.Skills) || !args.Popup)
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-operate", ("target", (object) ent)), (EntityUid) args.Buckle, args.User);
  }

  private void OnPortGunSeatUnstrapped(
    Entity<VehiclePortGunSeatComponent> ent,
    ref UnstrappedEvent args)
  {
    if (this._net.IsClient)
      return;
    this.ClearOperator(args.Buckle.Owner);
  }

  private void OnPortGunInteractHand(
    Entity<VehiclePortGunControllerComponent> ent,
    ref InteractHandEvent args)
  {
    EntityUid vehicle;
    EntityUid gunUid;
    VehiclePortGunComponent portGun;
    if (args.Handled || this._net.IsClient || !this.TryGetPortGun(ent, args.User, out vehicle, out gunUid, out portGun))
      return;
    EntityUid? nullable;
    if (portGun.Operator.HasValue)
    {
      nullable = portGun.Operator;
      EntityUid user = args.User;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != user ? 1 : 0) : 1) != 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-portgun-in-use", ("operator", (object) this.Name(portGun.Operator.Value))), (EntityUid) ent, new EntityUid?(args.User));
        return;
      }
    }
    VehiclePortGunOperatorComponent comp;
    if (this.TryComp<VehiclePortGunOperatorComponent>(args.User, out comp) && comp.Gun.HasValue)
    {
      nullable = comp.Gun;
      EntityUid entityUid = gunUid;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
        this.ClearOperator(args.User, comp);
    }
    nullable = portGun.Operator;
    EntityUid user1 = args.User;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == user1 ? 1 : 0) : 0) != 0)
    {
      this.ClearOperator(args.User);
      args.Handled = true;
    }
    else
    {
      portGun.Operator = new EntityUid?(args.User);
      this.Dirty(gunUid, (IComponent) portGun);
      VehiclePortGunOperatorComponent operatorComponent = this.EnsureComp<VehiclePortGunOperatorComponent>(args.User);
      operatorComponent.Gun = new EntityUid?(gunUid);
      operatorComponent.Vehicle = new EntityUid?(vehicle);
      operatorComponent.Controller = new EntityUid?(ent.Owner);
      this.Dirty(args.User, (IComponent) operatorComponent);
      if (this.HasComp<VehicleEnterComponent>(vehicle))
      {
        this._eye.SetTarget(args.User, new EntityUid?(vehicle));
        VehicleViewToggleSystem viewToggle = this._viewToggle;
        EntityUid user2 = args.User;
        EntityUid outsideTarget = vehicle;
        EntityUid owner = ent.Owner;
        nullable = new EntityUid?();
        EntityUid? insideTarget = nullable;
        viewToggle.EnableViewToggle(user2, outsideTarget, owner, insideTarget, true);
      }
      args.Handled = true;
    }
  }

  private void OnPortGunInteractUsing(
    Entity<VehiclePortGunControllerComponent> ent,
    ref InteractUsingEvent args)
  {
    EntityUid gunUid;
    ItemSlotsComponent comp;
    ItemSlot itemSlot;
    if (args.Handled || this._net.IsClient || !this.TryGetGunFromController(ent, out gunUid) || !this.TryComp<ItemSlotsComponent>(gunUid, out comp) || !this._itemSlots.TryGetSlot(gunUid, "gun_magazine", out itemSlot, comp) || itemSlot.HasItem && !this._itemSlots.TryEjectToHands(gunUid, itemSlot, new EntityUid?(args.User)) || !this._itemSlots.CanInsert(gunUid, args.Used, new EntityUid?(args.User), itemSlot) || !this._hands.TryDrop((Entity<HandsComponent>) args.User, args.Used) || !this._itemSlots.TryInsert(gunUid, itemSlot, args.Used, new EntityUid?(args.User)))
      return;
    args.Handled = true;
  }

  private void OnPortGunShutdown(Entity<VehiclePortGunComponent> ent, ref ComponentShutdown args)
  {
    if (!ent.Comp.Operator.HasValue)
      return;
    this.ClearOperator(ent.Comp.Operator.Value);
  }

  private void OnPortGunShot(Entity<VehiclePortGunComponent> ent, ref GunShotEvent args)
  {
    int num = this._net.IsClient ? 1 : 0;
  }

  private void OnPortGunContainerInserted(
    Entity<VehiclePortGunComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    int num = this._net.IsClient ? 1 : 0;
  }

  private void OnPortGunContainerRemoved(
    Entity<VehiclePortGunComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    int num = this._net.IsClient ? 1 : 0;
  }

  private void OnPortGunOperatorShutdown(
    Entity<VehiclePortGunOperatorComponent> ent,
    ref ComponentShutdown args)
  {
    EntityUid? gun = ent.Comp.Gun;
    if (!gun.HasValue)
      return;
    EntityUid valueOrDefault = gun.GetValueOrDefault();
    VehiclePortGunComponent comp;
    if (!this.TryComp<VehiclePortGunComponent>(valueOrDefault, out comp))
      return;
    EntityUid? nullable = comp.Operator;
    EntityUid owner = ent.Owner;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) != 0)
      return;
    comp.Operator = new EntityUid?();
    this.Dirty(valueOrDefault, (IComponent) comp);
  }

  private void OnPortGunExamined(
    Entity<VehiclePortGunControllerComponent> ent,
    ref ExaminedEvent args)
  {
    EntityUid gunUid;
    if (!args.IsInDetailsRange || !this.TryGetGunFromController(ent, out gunUid))
      return;
    GetAmmoCountEvent args1 = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>(gunUid, ref args1);
    if (args1.Capacity <= 0)
      return;
    args.PushMarkup(this.Loc.GetString("rmc-vehicle-portgun-examine-ammo", ("current", (object) args1.Count), ("max", (object) args1.Capacity)));
  }

  private void OnPortGunVerbs(
    Entity<VehiclePortGunControllerComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    EntityUid gunUid;
    ItemSlotsComponent comp;
    ItemSlot itemSlot;
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !this.TryGetGunFromController(ent, out gunUid) || !this.TryComp<ItemSlotsComponent>(gunUid, out comp) || !this._itemSlots.TryGetSlot(gunUid, "gun_magazine", out itemSlot, comp) || !itemSlot.HasItem)
      return;
    EntityUid user = args.User;
    EntityUid gun = gunUid;
    ItemSlot slot = itemSlot;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = this.Loc.GetString("rmc-vehicle-portgun-eject");
    alternativeVerb1.Act = (Action) (() => this._itemSlots.TryEjectToHands(gun, slot, new EntityUid?(user), true));
    alternativeVerb1.Priority = 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private bool TryGetPortGun(
    Entity<VehiclePortGunControllerComponent> ent,
    EntityUid user,
    out EntityUid vehicle,
    out EntityUid gunUid,
    out VehiclePortGunComponent portGun)
  {
    vehicle = new EntityUid();
    gunUid = new EntityUid();
    portGun = (VehiclePortGunComponent) null;
    if (!this.TryGetPortGunSeat(user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-portgun-need-seat"), (EntityUid) ent, new EntityUid?(user));
      return false;
    }
    EntityUid? vehicle1;
    if (!this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle1) || !vehicle1.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-portgun-no-vehicle"), (EntityUid) ent, new EntityUid?(user));
      return false;
    }
    vehicle = vehicle1.Value;
    ItemSlotsComponent comp1;
    ItemSlot itemSlot;
    if (this.TryComp<ItemSlotsComponent>(vehicle, out comp1) && this._itemSlots.TryGetSlot(vehicle, ent.Comp.GunSlotId, out itemSlot, comp1) && itemSlot.HasItem)
    {
      EntityUid? nullable = itemSlot.Item;
      if (nullable.HasValue)
      {
        ref EntityUid local = ref gunUid;
        nullable = itemSlot.Item;
        EntityUid entityUid = nullable.Value;
        local = entityUid;
        VehiclePortGunComponent comp2;
        if (!this.TryComp<VehiclePortGunComponent>(gunUid, out comp2) || !this.TryComp<GunComponent>(gunUid, out GunComponent _))
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-portgun-no-gun"), (EntityUid) ent, new EntityUid?(user));
          return false;
        }
        portGun = comp2;
        return true;
      }
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-portgun-no-gun"), (EntityUid) ent, new EntityUid?(user));
    return false;
  }

  private bool TryGetPortGunSeat(EntityUid user)
  {
    BuckleComponent comp;
    return this.TryComp<BuckleComponent>(user, out comp) && comp.BuckledTo.HasValue && this.HasComp<VehiclePortGunSeatComponent>(comp.BuckledTo.Value);
  }

  private bool TryGetGunFromController(
    Entity<VehiclePortGunControllerComponent> ent,
    out EntityUid gunUid)
  {
    gunUid = new EntityUid();
    EntityUid? vehicle;
    if (!this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return false;
    EntityUid uid = vehicle.Value;
    ItemSlotsComponent comp;
    ItemSlot itemSlot;
    if (this.TryComp<ItemSlotsComponent>(uid, out comp) && this._itemSlots.TryGetSlot(uid, ent.Comp.GunSlotId, out itemSlot, comp) && itemSlot.HasItem)
    {
      EntityUid? nullable = itemSlot.Item;
      if (nullable.HasValue)
      {
        ref EntityUid local = ref gunUid;
        nullable = itemSlot.Item;
        EntityUid entityUid = nullable.Value;
        local = entityUid;
        return true;
      }
    }
    return false;
  }

  private void ClearOperator(EntityUid user, VehiclePortGunOperatorComponent? operatorComp = null)
  {
    if (!this.Resolve<VehiclePortGunOperatorComponent>(user, ref operatorComp, false))
      return;
    EntityUid? gun = operatorComp.Gun;
    EntityUid? target;
    if (gun.HasValue)
    {
      EntityUid valueOrDefault = gun.GetValueOrDefault();
      VehiclePortGunComponent comp;
      if (this.TryComp<VehiclePortGunComponent>(valueOrDefault, out comp))
      {
        target = comp.Operator;
        EntityUid entityUid = user;
        if ((target.HasValue ? (target.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        {
          comp.Operator = new EntityUid?();
          this.Dirty(valueOrDefault, (IComponent) comp);
        }
      }
    }
    EntityUid? vehicle = operatorComp.Vehicle;
    this.RemCompDeferred<VehiclePortGunOperatorComponent>(user);
    if (operatorComp.Controller.HasValue)
      this._viewToggle.DisableViewToggle(user, operatorComp.Controller.Value);
    EyeComponent comp1;
    if (this._net.IsClient || !vehicle.HasValue || !this.TryComp<EyeComponent>(user, out comp1))
      return;
    target = comp1.Target;
    EntityUid? nullable = vehicle;
    if ((target.HasValue == nullable.HasValue ? (target.HasValue ? (target.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
      return;
    this._eye.SetTarget(user, new EntityUid?(), comp1);
  }
}
