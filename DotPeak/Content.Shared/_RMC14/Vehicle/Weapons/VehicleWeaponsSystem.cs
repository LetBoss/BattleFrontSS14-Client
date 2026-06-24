// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWeaponsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleWeaponsSystem : EntitySystem
{
  private const string HardpointSelectActionId = "ActionVehicleSelectHardpoint";
  [Dependency]
  private readonly SharedActionsSystem _actions;
  [Dependency]
  private readonly SharedUserInterfaceSystem _ui;
  [Dependency]
  private readonly SharedEyeSystem _eye;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SkillsSystem _skills;
  [Dependency]
  private readonly VehicleTopologySystem _topology;
  [Dependency]
  private readonly VehicleHardpointAmmoSystem _hardpointAmmo;
  [Dependency]
  private readonly VehicleSystem _vehicleSystem;
  [Dependency]
  private readonly VehicleTurretSystem _turretSystem;
  [Dependency]
  private readonly VehicleViewToggleSystem _viewToggle;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedContentEyeSystem _eyeSystem;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly MetaDataSystem _metaData;
  [Dependency]
  private readonly IGameTiming _timing;

  private void OnHardpointActionSelect(
    Entity<VehicleWeaponsOperatorComponent> ent,
    ref VehicleHardpointSelectActionEvent args)
  {
    BuckleComponent comp1;
    if (this._net.IsClient || args.Handled || args.Performer == new EntityUid() || !this.Exists(args.Performer) || args.Performer != ent.Owner || !this.CanUseHardpointActions(args.Performer) || !this.TryComp<BuckleComponent>(args.Performer, out comp1))
      return;
    EntityUid? buckledTo = comp1.BuckledTo;
    if (!buckledTo.HasValue)
      return;
    EntityUid valueOrDefault = buckledTo.GetValueOrDefault();
    VehicleHardpointActionComponent comp2;
    if (!this.HasComp<VehicleWeaponsSeatComponent>(valueOrDefault) || !this.TryComp<VehicleHardpointActionComponent>((EntityUid) args.Action, out comp2) || !this.TrySelectHardpoint(valueOrDefault, args.Performer, comp2.MountedWeapon, false))
      return;
    args.Handled = true;
  }

  private void OnViewToggled(
    Entity<VehicleWeaponsOperatorComponent> ent,
    ref VehicleViewToggledEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid? vehicle = ent.Comp.Vehicle;
    if (!vehicle.HasValue)
      return;
    EntityUid valueOrDefault = vehicle.GetValueOrDefault();
    VehicleWeaponsComponent comp;
    if (!this.TryComp<VehicleWeaponsComponent>(valueOrDefault, out comp))
      return;
    this.RefreshHardpointActions(ent.Owner, valueOrDefault, comp, ent.Comp);
    EntityUid seat;
    if (!this.TryGetUserWeaponsSeat(ent.Owner, out seat, out VehicleWeaponsSeatComponent _))
      return;
    this.UpdateWeaponsUi(seat, valueOrDefault, comp, operatorUid: new EntityUid?(ent.Owner));
  }

  private void RefreshHardpointActions(
    EntityUid user,
    EntityUid vehicle,
    VehicleWeaponsComponent weapons,
    VehicleWeaponsOperatorComponent? operatorComp = null,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    if (this._net.IsClient || !this.Resolve<VehicleWeaponsOperatorComponent>(user, ref operatorComp, false))
      return;
    if (!this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
    {
      this.ClearHardpointActions(user, operatorComp);
    }
    else
    {
      List<VehicleWeaponsSystem.HardpointActionSlot> source = this.CanUseHardpointActions(user) ? this.GetSelectableHardpointActionSlots(vehicle, user, weapons, hardpoints, itemSlots) : new List<VehicleWeaponsSystem.HardpointActionSlot>();
      HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>(source.Select<VehicleWeaponsSystem.HardpointActionSlot, EntityUid>((Func<VehicleWeaponsSystem.HardpointActionSlot, EntityUid>) (slot => slot.MountedWeapon)));
      foreach (KeyValuePair<EntityUid, EntityUid> keyValuePair in operatorComp.HardpointActions.ToArray<KeyValuePair<EntityUid, EntityUid>>())
      {
        if (!entityUidSet.Contains(keyValuePair.Key) || !this.Exists(keyValuePair.Value))
        {
          this.RemoveAndDeleteHardpointAction(user, keyValuePair.Value);
          operatorComp.HardpointActions.Remove(keyValuePair.Key);
        }
      }
      for (int index = 0; index < source.Count; ++index)
      {
        VehicleWeaponsSystem.HardpointActionSlot hardpointActionSlot = source[index];
        EntityUid uid;
        ActionComponent comp1;
        if (operatorComp.HardpointActions.TryGetValue(hardpointActionSlot.MountedWeapon, out uid) && this.Exists(uid) && this.TryComp<ActionComponent>(uid, out comp1))
        {
          EntityUid? container = comp1.Container;
          EntityUid iconEntity = hardpointActionSlot.IconEntity;
          if ((container.HasValue ? (container.GetValueOrDefault() == iconEntity ? 1 : 0) : 0) != 0)
          {
            VehicleHardpointActionComponent comp2;
            if (this.TryComp<VehicleHardpointActionComponent>(uid, out comp2))
            {
              comp2.MountedWeapon = new EntityUid?(hardpointActionSlot.MountedWeapon);
              comp2.SortOrder = index;
              this.Dirty(uid, (IComponent) comp2);
            }
            this._actions.SetTemporary((Entity<ActionComponent>) (uid, comp1), false);
            this._metaData.SetEntityName(uid, hardpointActionSlot.DisplayName);
            continue;
          }
        }
        EntityUid entityUid;
        if (operatorComp.HardpointActions.TryGetValue(hardpointActionSlot.MountedWeapon, out entityUid) && this.Exists(entityUid))
        {
          this.RemoveAndDeleteHardpointAction(user, entityUid);
          operatorComp.HardpointActions.Remove(hardpointActionSlot.MountedWeapon);
        }
        EntityUid? actionId = new EntityUid?();
        if (this._actions.AddAction(user, ref actionId, "ActionVehicleSelectHardpoint", hardpointActionSlot.IconEntity) && actionId.HasValue)
        {
          VehicleHardpointActionComponent hardpointActionComponent = this.EnsureComp<VehicleHardpointActionComponent>(actionId.Value);
          hardpointActionComponent.MountedWeapon = new EntityUid?(hardpointActionSlot.MountedWeapon);
          hardpointActionComponent.SortOrder = index;
          this.Dirty(actionId.Value, (IComponent) hardpointActionComponent);
          this._actions.SetTemporary((Entity<ActionComponent>) (actionId.Value, this.Comp<ActionComponent>(actionId.Value)), false);
          this._metaData.SetEntityName(actionId.Value, hardpointActionSlot.DisplayName);
          operatorComp.HardpointActions[hardpointActionSlot.MountedWeapon] = actionId.Value;
        }
      }
      this.UpdateHardpointActionStates(user, weapons, operatorComp);
    }
  }

  private List<VehicleWeaponsSystem.HardpointActionSlot> GetSelectableHardpointActionSlots(
    EntityUid vehicle,
    EntityUid user,
    VehicleWeaponsComponent weapons,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots)
  {
    List<VehicleWeaponsSystem.HardpointActionSlot> hardpointActionSlots = new List<VehicleWeaponsSystem.HardpointActionSlot>();
    VehicleWeaponsSeatComponent seatComp;
    if (!this.TryGetUserWeaponsSeat(user, out EntityUid _, out seatComp))
      return hardpointActionSlots;
    foreach (VehicleMountedSlot mountedSlot in this._topology.GetMountedSlots(vehicle, hardpoints, itemSlots))
    {
      if (this.IsHardpointTypeAllowed(seatComp, mountedSlot.HardpointType))
      {
        EntityUid? nullable = mountedSlot.Item;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          EntityUid entityUid;
          if (this.HasComp<VehicleTurretComponent>(valueOrDefault) && this.HasComp<GunComponent>(valueOrDefault) && (VehicleWeaponsSystem.IsSharedHardpointType(mountedSlot.HardpointType) || !weapons.HardpointOperators.TryGetValue(valueOrDefault, out entityUid) || entityUid == user))
            hardpointActionSlots.Add(new VehicleWeaponsSystem.HardpointActionSlot(valueOrDefault, valueOrDefault, this.Name(valueOrDefault)));
        }
      }
    }
    return hardpointActionSlots;
  }

  private void UpdateHardpointActionStates(
    EntityUid user,
    VehicleWeaponsComponent weapons,
    VehicleWeaponsOperatorComponent? operatorComp = null)
  {
    if (this._net.IsClient || !this.Resolve<VehicleWeaponsOperatorComponent>(user, ref operatorComp, false))
      return;
    bool enabled = this.CanUseHardpointActions(user);
    EntityUid entityUid;
    EntityUid? nullable = weapons.OperatorSelections.TryGetValue(user, out entityUid) ? new EntityUid?(entityUid) : new EntityUid?();
    foreach (KeyValuePair<EntityUid, EntityUid> hardpointAction in operatorComp.HardpointActions)
    {
      this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) hardpointAction.Value), enabled);
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) hardpointAction.Value), enabled && nullable.HasValue && hardpointAction.Key == nullable.Value);
    }
  }

  private void ClearHardpointActions(EntityUid user, VehicleWeaponsOperatorComponent? operatorComp = null)
  {
    if (this._net.IsClient || !this.Resolve<VehicleWeaponsOperatorComponent>(user, ref operatorComp, false))
      return;
    foreach (EntityUid entityUid in operatorComp.HardpointActions.Values.ToArray<EntityUid>())
    {
      if (this.Exists(entityUid))
        this.RemoveAndDeleteHardpointAction(user, entityUid);
    }
    operatorComp.HardpointActions.Clear();
  }

  private void RemoveAndDeleteHardpointAction(EntityUid user, EntityUid action)
  {
    if (!this.Exists(action))
      return;
    this._actions.RemoveAction((Entity<ActionsComponent>) user, new Entity<ActionComponent>?((Entity<ActionComponent>) action));
    if (!this.Exists(action))
      return;
    this.QueueDel(new EntityUid?(action));
  }

  private bool CanUseHardpointActions(EntityUid user, bool forUi = false)
  {
    VehicleWeaponsSeatComponent seatComp;
    VehicleViewToggleComponent comp;
    return this.TryGetUserWeaponsSeat(user, out EntityUid _, out seatComp) && (!forUi || seatComp.AllowUiSelection) && (forUi || seatComp.AllowHotbarSelection) && (!this.TryComp<VehicleViewToggleComponent>(user, out comp) || comp.IsOutside);
  }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, StrapAttemptEvent>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, StrapAttemptEvent>(this.OnWeaponSeatStrapAttempt));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, StrappedEvent>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, StrappedEvent>(this.OnWeaponSeatStrapped));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, UnstrappedEvent>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, UnstrappedEvent>(this.OnWeaponSeatUnstrapped));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, BoundUIOpenedEvent>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, BoundUIOpenedEvent>(this.OnWeaponsUiOpened));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, BoundUIClosedEvent>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, BoundUIClosedEvent>(this.OnWeaponsUiClosed));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, VehicleWeaponsSelectMessage>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, VehicleWeaponsSelectMessage>(this.OnWeaponsSelect));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, VehicleWeaponsStabilizationMessage>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, VehicleWeaponsStabilizationMessage>(this.OnWeaponsStabilization));
    this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, VehicleWeaponsAutoModeMessage>(new EntityEventRefHandler<VehicleWeaponsSeatComponent, VehicleWeaponsAutoModeMessage>(this.OnWeaponsAutoMode));
    this.SubscribeLocalEvent<VehicleWeaponsOperatorComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleWeaponsOperatorComponent, ComponentShutdown>(this.OnOperatorShutdown));
    this.SubscribeLocalEvent<VehicleWeaponsOperatorComponent, ShotAttemptedEvent>(new EntityEventRefHandler<VehicleWeaponsOperatorComponent, ShotAttemptedEvent>(this.OnOperatorShotAttempted));
    this.SubscribeLocalEvent<VehicleWeaponsOperatorComponent, VehicleHardpointSelectActionEvent>(new EntityEventRefHandler<VehicleWeaponsOperatorComponent, VehicleHardpointSelectActionEvent>(this.OnHardpointActionSelect));
    this.SubscribeLocalEvent<VehicleWeaponsOperatorComponent, VehicleViewToggledEvent>(new EntityEventRefHandler<VehicleWeaponsOperatorComponent, VehicleViewToggledEvent>(this.OnViewToggled));
    this.SubscribeLocalEvent<HardpointSlotsChangedEvent>(new EntityEventHandler<HardpointSlotsChangedEvent>(this.OnHardpointSlotsChanged));
    this.SubscribeLocalEvent<VehicleTurretComponent, GunShotEvent>(new EntityEventRefHandler<VehicleTurretComponent, GunShotEvent>(this.OnTurretGunShot));
  }

  private void OnWeaponSeatStrapAttempt(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref StrapAttemptEvent args)
  {
    if (args.Cancelled || this._skills.HasSkills((Entity<SkillsComponent>) args.Buckle.Owner, ent.Comp.Skills) || !args.Popup)
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-operate", ("target", (object) ent)), (EntityUid) args.Buckle, args.User);
  }

  private void OnWeaponSeatStrapped(Entity<VehicleWeaponsSeatComponent> ent, ref StrappedEvent args)
  {
    EntityUid? vehicle;
    if (this._net.IsClient || !this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return;
    EntityUid entityUid = vehicle.Value;
    VehicleWeaponsComponent weapons = this.EnsureComp<VehicleWeaponsComponent>(entityUid);
    this.ClearOperatorSelections(weapons, args.Buckle.Owner);
    if (ent.Comp.IsPrimaryOperatorSeat)
      weapons.Operator = new EntityUid?(args.Buckle.Owner);
    this.RecalculateSelectedWeapon(entityUid, weapons);
    this.Dirty(entityUid, (IComponent) weapons);
    VehicleWeaponsOperatorComponent operatorComp = this.EnsureComp<VehicleWeaponsOperatorComponent>(args.Buckle.Owner);
    operatorComp.Vehicle = vehicle;
    operatorComp.SelectedWeapon = new EntityUid?();
    operatorComp.HardpointActions.Clear();
    this.Dirty(args.Buckle.Owner, (IComponent) operatorComp);
    this.RefreshOperatorSelectedWeapons(entityUid, weapons);
    this.RefreshHardpointActions(args.Buckle.Owner, entityUid, weapons, operatorComp);
    if (this.HasComp<VehicleEnterComponent>(entityUid))
    {
      this._eye.SetTarget(args.Buckle.Owner, new EntityUid?(entityUid));
      this._viewToggle.EnableViewToggle(args.Buckle.Owner, entityUid, ent.Owner, new EntityUid?(), true);
    }
    this.UpdateGunnerView(args.Buckle.Owner, entityUid, ent.Comp);
    this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) VehicleWeaponsUiKey.Key, new EntityUid?(args.Buckle.Owner));
    this.UpdateWeaponsUiForAllOperators(entityUid, weapons);
  }

  private void OnWeaponSeatUnstrapped(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref UnstrappedEvent args)
  {
    if (this._net.IsClient)
      return;
    VehicleWeaponsOperatorComponent comp1;
    if (this.TryComp<VehicleWeaponsOperatorComponent>(args.Buckle.Owner, out comp1))
      this.ClearHardpointActions(args.Buckle.Owner, comp1);
    this.RemCompDeferred<VehicleWeaponsOperatorComponent>(args.Buckle.Owner);
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) VehicleWeaponsUiKey.Key, new EntityUid?(args.Buckle.Owner));
    this.UpdateGunnerView(args.Buckle.Owner, ent.Owner, ent.Comp, true);
    this._viewToggle.DisableViewToggle(args.Buckle.Owner, ent.Owner);
    EntityUid? vehicle;
    if (!this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return;
    EntityUid entityUid1 = vehicle.Value;
    VehicleWeaponsComponent comp2;
    EntityUid? nullable1;
    if (this.TryComp<VehicleWeaponsComponent>(entityUid1, out comp2) && ent.Comp.IsPrimaryOperatorSeat)
    {
      nullable1 = comp2.Operator;
      EntityUid owner = args.Buckle.Owner;
      if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
      {
        comp2.Operator = new EntityUid?();
        this.ClearOperatorSelections(comp2, args.Buckle.Owner);
        this.RecalculateSelectedWeapon(entityUid1, comp2);
        this.Dirty(entityUid1, (IComponent) comp2);
        goto label_11;
      }
    }
    VehicleWeaponsComponent comp3;
    if (this.TryComp<VehicleWeaponsComponent>(entityUid1, out comp3))
    {
      this.ClearOperatorSelections(comp3, args.Buckle.Owner);
      this.RecalculateSelectedWeapon(entityUid1, comp3);
      this.Dirty(entityUid1, (IComponent) comp3);
    }
label_11:
    VehicleWeaponsComponent comp4;
    if (this.TryComp<VehicleWeaponsComponent>(entityUid1, out comp4))
      this.RefreshOperatorSelectedWeapons(entityUid1, comp4);
    VehicleWeaponsComponent comp5;
    if (this.TryComp<VehicleWeaponsComponent>(entityUid1, out comp5))
      this.UpdateWeaponsUiForAllOperators(entityUid1, comp5);
    EyeComponent comp6;
    if (!this.TryComp<EyeComponent>(args.Buckle.Owner, out comp6))
      return;
    nullable1 = comp6.Target;
    EntityUid entityUid2 = entityUid1;
    if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) == 0)
      return;
    SharedEyeSystem eye = this._eye;
    EntityUid owner1 = args.Buckle.Owner;
    nullable1 = new EntityUid?();
    EntityUid? nullable2 = nullable1;
    EyeComponent eyeComponent = comp6;
    eye.SetTarget(owner1, nullable2, eyeComponent);
  }

  private void OnOperatorShutdown(
    Entity<VehicleWeaponsOperatorComponent> ent,
    ref ComponentShutdown args)
  {
    if (this._net.IsClient)
      return;
    this.ClearHardpointActions(ent.Owner, ent.Comp);
  }

  private void OnOperatorShotAttempted(
    Entity<VehicleWeaponsOperatorComponent> ent,
    ref ShotAttemptedEvent args)
  {
    if (this._net.IsClient || args.User != ent.Owner)
      return;
    EntityUid? nullable = ent.Comp.Vehicle;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    HardpointIntegrityComponent comp1;
    if (this.TryComp<HardpointIntegrityComponent>(valueOrDefault1, out comp1) && (double) comp1.Integrity <= 0.0)
    {
      args.Cancel();
      this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-hull-destroyed"), ent.Owner, ent.Owner, PopupType.SmallCaution);
    }
    else
    {
      VehicleWeaponsComponent comp2;
      EntityUid entityUid;
      if (!this.TryComp<VehicleWeaponsComponent>(valueOrDefault1, out comp2) || !this.TryComp<ItemSlotsComponent>(valueOrDefault1, out ItemSlotsComponent _) || !this.CanUseHardpointActions(ent.Owner) || !comp2.OperatorSelections.TryGetValue(ent.Owner, out entityUid) || entityUid != args.Used.Owner)
        return;
      TimeSpan timeSpan = args.Used.Comp.NextFire - this._timing.CurTime;
      if (timeSpan <= TimeSpan.Zero || this._timing.CurTime < ent.Comp.NextCooldownFeedbackAt)
        return;
      ent.Comp.NextCooldownFeedbackAt = this._timing.CurTime + TimeSpan.FromSeconds(0.25);
      BuckleComponent comp3;
      if (!this.TryComp<BuckleComponent>(ent.Owner, out comp3))
        return;
      nullable = comp3.BuckledTo;
      if (!nullable.HasValue)
        return;
      EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
      if (!this.HasComp<VehicleWeaponsSeatComponent>(valueOrDefault2))
        return;
      this._ui.ServerSendUiMessage((Entity<UserInterfaceComponent>) valueOrDefault2, (Enum) VehicleWeaponsUiKey.Key, (BoundUserInterfaceMessage) new VehicleWeaponsCooldownFeedbackMessage((float) timeSpan.TotalSeconds), ent.Owner);
      this._audio.PlayPredicted(args.Used.Comp.SoundEmpty, args.Used.Owner, new EntityUid?(ent.Owner));
    }
  }

  private bool TrySelectHardpoint(
    EntityUid seat,
    EntityUid actor,
    EntityUid? mountedWeapon,
    bool fromUi)
  {
    EntityUid? vehicle;
    if (this._net.IsClient || !this._vehicleSystem.TryGetVehicleFromInterior(seat, out vehicle) || !vehicle.HasValue)
      return false;
    EntityUid entityUid1 = vehicle.Value;
    VehicleWeaponsComponent comp1;
    BuckleComponent comp2;
    if (!this.TryComp<VehicleWeaponsComponent>(entityUid1, out comp1) || !this.TryComp<BuckleComponent>(actor, out comp2))
      return false;
    EntityUid? buckledTo = comp2.BuckledTo;
    EntityUid entityUid2 = seat;
    VehicleWeaponsSeatComponent comp3;
    if ((buckledTo.HasValue ? (buckledTo.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0 || !this.TryComp<VehicleWeaponsSeatComponent>(seat, out comp3) || fromUi && !comp3.AllowUiSelection || !fromUi && !comp3.AllowHotbarSelection)
      return false;
    VehiclePortGunOperatorComponent comp4;
    if (this.TryComp<VehiclePortGunOperatorComponent>(actor, out comp4) && comp4.Gun.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-portgun-active"), seat, new EntityUid?(actor));
      return true;
    }
    HardpointSlotsComponent comp5;
    ItemSlotsComponent comp6;
    VehicleWeaponsOperatorComponent comp7;
    if (!this.TryComp<HardpointSlotsComponent>(entityUid1, out comp5) || !this.TryComp<ItemSlotsComponent>(entityUid1, out comp6) || !this.TryComp<VehicleWeaponsOperatorComponent>(actor, out comp7))
      return false;
    if (!mountedWeapon.HasValue)
    {
      this.ClearOperatorSelections(comp1, actor);
      this.RecalculateSelectedWeapon(entityUid1, comp1, comp6);
      this.RefreshOperatorSelectedWeapons(entityUid1, comp1, comp6);
      this.Dirty(entityUid1, (IComponent) comp1);
      this.UpdateHardpointActionStates(actor, comp1, comp7);
      this.UpdateWeaponsUiForAllOperators(entityUid1, comp1, comp5, comp6);
      return true;
    }
    string hardpointType;
    if (!this.Exists(mountedWeapon.Value) || !this._topology.TryGetMountedSlotByItem(entityUid1, mountedWeapon.Value, out VehicleMountedSlot _, comp5, comp6) || !this.HasComp<GunComponent>(mountedWeapon.Value) || !this.HasComp<VehicleTurretComponent>(mountedWeapon.Value) || !this.TryGetMountedWeaponHardpointType(entityUid1, mountedWeapon.Value, out hardpointType, comp5, comp6) || !this.IsHardpointTypeAllowed(comp3, hardpointType))
      return false;
    bool flag1 = VehicleWeaponsSystem.IsSharedHardpointType(hardpointType);
    EntityUid entityUid3;
    if (!flag1 && comp1.HardpointOperators.TryGetValue(mountedWeapon.Value, out entityUid3) && entityUid3 != actor)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-weapons-ui-hardpoint-in-use", ("operator", (object) entityUid3)), seat, new EntityUid?(actor));
      this.UpdateWeaponsUiForAllOperators(entityUid1, comp1, comp5, comp6);
      return true;
    }
    EntityUid entityUid4;
    bool flag2 = !comp1.OperatorSelections.TryGetValue(actor, out entityUid4) || entityUid4 != mountedWeapon.Value;
    EntityUid entityUid5;
    if (comp1.OperatorSelections.TryGetValue(actor, out entityUid5) && entityUid5 == mountedWeapon.Value)
    {
      comp1.OperatorSelections.Remove(actor);
      EntityUid entityUid6;
      if (!flag1 && comp1.HardpointOperators.TryGetValue(mountedWeapon.Value, out entityUid6) && entityUid6 == actor)
        comp1.HardpointOperators.Remove(mountedWeapon.Value);
    }
    else
    {
      EntityUid key;
      EntityUid entityUid7;
      if (comp1.OperatorSelections.TryGetValue(actor, out key) && comp1.HardpointOperators.TryGetValue(key, out entityUid7) && entityUid7 == actor)
        comp1.HardpointOperators.Remove(key);
      comp1.OperatorSelections[actor] = mountedWeapon.Value;
      if (!flag1)
        comp1.HardpointOperators[mountedWeapon.Value] = actor;
      GunSpinupComponent comp8;
      if (flag2 && this.TryComp<GunSpinupComponent>(mountedWeapon.Value, out comp8) && comp8.SelectSound != null)
        this._audio.PlayPredicted(comp8.SelectSound, mountedWeapon.Value, new EntityUid?(actor));
    }
    this.RecalculateSelectedWeapon(entityUid1, comp1, comp6);
    this.RefreshOperatorSelectedWeapons(entityUid1, comp1, comp6);
    this.Dirty(entityUid1, (IComponent) comp1);
    this.UpdateHardpointActionStates(actor, comp1, comp7);
    this.UpdateWeaponsUiForAllOperators(entityUid1, comp1, comp5, comp6);
    return true;
  }

  private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
  {
    VehicleWeaponsComponent comp;
    if (this._net.IsClient || !this.TryComp<VehicleWeaponsComponent>(args.Vehicle, out comp))
      return;
    HardpointSlotsComponent component1 = (HardpointSlotsComponent) null;
    ItemSlotsComponent component2 = (ItemSlotsComponent) null;
    EntityUid? selectedWeapon = comp.SelectedWeapon;
    if (selectedWeapon.HasValue)
    {
      EntityUid valueOrDefault = selectedWeapon.GetValueOrDefault();
      if (this.Resolve<HardpointSlotsComponent>(args.Vehicle, ref component1, false) && this.Resolve<ItemSlotsComponent>(args.Vehicle, ref component2, false) && !this.IsSelectedWeaponInstalled(args.Vehicle, valueOrDefault, component1, component2))
      {
        comp.SelectedWeapon = new EntityUid?();
        this.Dirty(args.Vehicle, (IComponent) comp);
      }
    }
    this.PruneHardpointOperators(args.Vehicle, comp, component1, component2);
    this.RecalculateSelectedWeapon(args.Vehicle, comp, component2);
    this.RefreshOperatorSelectedWeapons(args.Vehicle, comp, component2);
    this.RefreshSeatGunnerViews(args.Vehicle);
    this.Dirty(args.Vehicle, (IComponent) comp);
    this.UpdateWeaponsUiForAllOperators(args.Vehicle, comp, component1, component2, true);
  }

  private void RefreshSeatGunnerViews(EntityUid vehicle)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleWeaponsOperatorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
    EntityUid uid;
    VehicleWeaponsOperatorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? vehicle1 = comp1.Vehicle;
      EntityUid seat = vehicle;
      VehicleWeaponsSeatComponent seatComp;
      if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() != seat ? 1 : 0) : 1) == 0 && this.TryGetUserWeaponsSeat(uid, out seat, out seatComp))
        this.UpdateGunnerView(uid, vehicle, seatComp);
    }
  }

  private void UpdateGunnerView(
    EntityUid user,
    EntityUid vehicle,
    VehicleWeaponsSeatComponent? seatComp = null,
    bool removeOnly = false)
  {
    if (seatComp == null)
      seatComp = this.CompOrNull<VehicleWeaponsSeatComponent>(this.Transform(user).ParentUid);
    if (removeOnly)
    {
      if (!this.RemCompDeferred<VehicleGunnerViewUserComponent>(user))
        return;
      this._eyeSystem.UpdatePvsScale(user);
    }
    else
    {
      bool flag = false;
      float val1_1 = 0.0f;
      float val1_2 = 0.0f;
      float x = 0.5f;
      float val1_3 = 0.0f;
      if (seatComp != null && VehicleWeaponsSystem.HasBaseGunnerView(seatComp))
      {
        val1_1 = Math.Max(val1_1, seatComp.BaseViewPvsScale);
        val1_2 = Math.Max(val1_2, seatComp.BaseViewCursorMaxOffset);
        x = MathF.Max(x, seatComp.BaseViewCursorOffsetSpeed);
        val1_3 = Math.Max(val1_3, seatComp.BaseViewCursorPvsIncrease);
        flag = true;
      }
      VehicleGunnerViewComponent comp;
      if (seatComp != null && (seatComp.IsPrimaryOperatorSeat || VehicleWeaponsSystem.HasBaseGunnerView(seatComp)) && this.TryComp<VehicleGunnerViewComponent>(vehicle, out comp) && (double) comp.PvsScale > 0.0)
      {
        val1_1 = Math.Max(val1_1, comp.PvsScale);
        val1_2 = Math.Max(val1_2, comp.CursorMaxOffset);
        x = MathF.Max(x, comp.CursorOffsetSpeed);
        val1_3 = Math.Max(val1_3, comp.CursorPvsIncrease);
        flag = true;
      }
      if (flag && (double) val1_1 > 0.0)
      {
        VehicleGunnerViewUserComponent viewUserComponent = this.EnsureComp<VehicleGunnerViewUserComponent>(user);
        viewUserComponent.PvsScale = val1_1;
        viewUserComponent.CursorMaxOffset = val1_2;
        viewUserComponent.CursorOffsetSpeed = x;
        viewUserComponent.CursorPvsIncrease = val1_3;
        this.Dirty(user, (IComponent) viewUserComponent);
        this._eyeSystem.UpdatePvsScale(user);
      }
      else
      {
        if (!this.RemCompDeferred<VehicleGunnerViewUserComponent>(user))
          return;
        this._eyeSystem.UpdatePvsScale(user);
      }
    }
  }

  private static bool HasBaseGunnerView(VehicleWeaponsSeatComponent seatComp)
  {
    return (double) seatComp.BaseViewPvsScale > 0.0 || (double) seatComp.BaseViewCursorMaxOffset > 0.0 || (double) seatComp.BaseViewCursorPvsIncrease > 0.0;
  }

  private bool IsSelectedWeaponInstalled(
    EntityUid vehicle,
    EntityUid selected,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots)
  {
    foreach (VehicleMountedSlot mountedSlot in this._topology.GetMountedSlots(vehicle, hardpoints, itemSlots))
    {
      EntityUid? nullable = mountedSlot.Item;
      EntityUid entityUid = selected;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        return true;
    }
    return false;
  }

  private void OnTurretGunShot(Entity<VehicleTurretComponent> ent, ref GunShotEvent args)
  {
    EntityUid vehicle;
    VehicleWeaponsComponent comp;
    if (this._net.IsClient || !this.TryGetContainingVehicle(ent.Owner, out vehicle) || !this.TryComp<VehicleWeaponsComponent>(vehicle, out comp))
      return;
    this.UpdateWeaponsUiForAllOperators(vehicle, comp);
  }

  private bool TryGetContainingVehicle(EntityUid owner, out EntityUid vehicle)
  {
    return this._topology.TryGetVehicle(owner, out vehicle);
  }

  private void ClearOperatorSelections(VehicleWeaponsComponent weapons, EntityUid operatorUid)
  {
    weapons.OperatorSelections.Remove(operatorUid);
    foreach (KeyValuePair<EntityUid, EntityUid> keyValuePair in weapons.HardpointOperators.ToArray<KeyValuePair<EntityUid, EntityUid>>())
    {
      if (keyValuePair.Value == operatorUid)
        weapons.HardpointOperators.Remove(keyValuePair.Key);
    }
  }

  private void PruneHardpointOperators(
    EntityUid vehicle,
    VehicleWeaponsComponent weapons,
    HardpointSlotsComponent? hardpoints,
    ItemSlotsComponent? itemSlots)
  {
    if (!this.Resolve<HardpointSlotsComponent>(vehicle, ref hardpoints, false))
      return;
    VehicleMountedSlot mountedSlot;
    foreach (KeyValuePair<EntityUid, EntityUid> keyValuePair in weapons.HardpointOperators.ToArray<KeyValuePair<EntityUid, EntityUid>>())
    {
      if (!this.Exists(keyValuePair.Key) || !this.Exists(keyValuePair.Value) || !this._topology.TryGetMountedSlotByItem(vehicle, keyValuePair.Key, out mountedSlot, hardpoints, itemSlots))
        weapons.HardpointOperators.Remove(keyValuePair.Key);
    }
    foreach (KeyValuePair<EntityUid, EntityUid> keyValuePair in weapons.OperatorSelections.ToArray<KeyValuePair<EntityUid, EntityUid>>())
    {
      if (!this.Exists(keyValuePair.Key) || !this.Exists(keyValuePair.Value) || !this._topology.TryGetMountedSlotByItem(vehicle, keyValuePair.Value, out mountedSlot, hardpoints, itemSlots))
        weapons.OperatorSelections.Remove(keyValuePair.Key);
    }
  }

  private bool TryGetUserWeaponsSeat(
    EntityUid user,
    out EntityUid seat,
    out VehicleWeaponsSeatComponent seatComp)
  {
    seat = new EntityUid();
    seatComp = (VehicleWeaponsSeatComponent) null;
    BuckleComponent comp1;
    if (this.TryComp<BuckleComponent>(user, out comp1))
    {
      EntityUid? buckledTo = comp1.BuckledTo;
      if (buckledTo.HasValue)
      {
        EntityUid valueOrDefault = buckledTo.GetValueOrDefault();
        VehicleWeaponsSeatComponent comp2;
        if (this.TryComp<VehicleWeaponsSeatComponent>(valueOrDefault, out comp2))
        {
          seatComp = comp2;
          seat = valueOrDefault;
          return true;
        }
      }
    }
    return false;
  }

  private bool TryGetMountedWeaponHardpointType(
    EntityUid vehicle,
    EntityUid mountedWeapon,
    out string hardpointType)
  {
    return this.TryGetMountedWeaponHardpointType(vehicle, mountedWeapon, out hardpointType, (HardpointSlotsComponent) null, (ItemSlotsComponent) null);
  }

  private bool TryGetMountedWeaponHardpointType(
    EntityUid vehicle,
    EntityUid mountedWeapon,
    out string hardpointType,
    HardpointSlotsComponent? hardpoints,
    ItemSlotsComponent? itemSlots)
  {
    hardpointType = string.Empty;
    VehicleMountedSlot mountedSlot;
    if (!this._topology.TryGetMountedSlotByItem(vehicle, mountedWeapon, out mountedSlot, hardpoints, itemSlots))
      return false;
    hardpointType = mountedSlot.HardpointType;
    return true;
  }

  private bool IsHardpointTypeAllowed(VehicleWeaponsSeatComponent seatComp, string hardpointType)
  {
    if (seatComp.AllowedHardpointTypes.Count == 0)
      return true;
    foreach (string allowedHardpointType in seatComp.AllowedHardpointTypes)
    {
      if (string.Equals(allowedHardpointType, hardpointType, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private static bool IsSharedHardpointType(string hardpointType)
  {
    return string.Equals(hardpointType, "Support", StringComparison.OrdinalIgnoreCase);
  }

  private void RefreshOperatorSelectedWeapons(
    EntityUid vehicle,
    VehicleWeaponsComponent weapons,
    ItemSlotsComponent? itemSlots = null)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleWeaponsOperatorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
    EntityUid uid;
    VehicleWeaponsOperatorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? nullable1 = comp1.Vehicle;
      EntityUid entityUid = vehicle;
      if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
      {
        EntityUid? nullable2 = new EntityUid?();
        EntityUid mountedWeapon;
        if (weapons.OperatorSelections.TryGetValue(uid, out mountedWeapon) && this.IsSelectableMountedWeapon(vehicle, mountedWeapon, itemSlots: itemSlots))
          nullable2 = new EntityUid?(mountedWeapon);
        nullable1 = comp1.SelectedWeapon;
        EntityUid? nullable3 = nullable2;
        if ((nullable1.HasValue == nullable3.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable3.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
        {
          comp1.SelectedWeapon = nullable2;
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }

  public bool TryGetSelectedWeaponForOperator(
    EntityUid vehicle,
    EntityUid operatorUid,
    out EntityUid weapon)
  {
    weapon = new EntityUid();
    VehicleWeaponsComponent comp1;
    if (!this.TryComp<VehicleWeaponsComponent>(vehicle, out comp1))
      return false;
    EntityUid mountedWeapon;
    if (comp1.OperatorSelections.TryGetValue(operatorUid, out mountedWeapon) && this.IsSelectableMountedWeapon(vehicle, mountedWeapon))
    {
      weapon = mountedWeapon;
      return true;
    }
    VehicleWeaponsOperatorComponent comp2;
    if (this.TryComp<VehicleWeaponsOperatorComponent>(operatorUid, out comp2))
    {
      EntityUid? vehicle1 = comp2.Vehicle;
      EntityUid entityUid = vehicle;
      if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
      {
        EntityUid? selectedWeapon = comp2.SelectedWeapon;
        if (selectedWeapon.HasValue)
        {
          EntityUid valueOrDefault = selectedWeapon.GetValueOrDefault();
          if (this.Exists(valueOrDefault) && this.HasComp<GunComponent>(valueOrDefault))
          {
            weapon = valueOrDefault;
            return true;
          }
        }
      }
    }
    EntityUid? nullable = comp1.Operator;
    EntityUid entityUid1 = operatorUid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid1 ? 1 : 0) : 0) != 0)
    {
      EntityUid? selectedWeapon = comp1.SelectedWeapon;
      if (selectedWeapon.HasValue)
      {
        EntityUid valueOrDefault = selectedWeapon.GetValueOrDefault();
        if (this.Exists(valueOrDefault) && this.HasComp<GunComponent>(valueOrDefault))
        {
          weapon = valueOrDefault;
          return true;
        }
      }
    }
    return false;
  }

  public bool TryGetOperatorForSelectedWeapon(
    EntityUid vehicle,
    EntityUid weapon,
    out EntityUid operatorUid)
  {
    operatorUid = new EntityUid();
    VehicleWeaponsComponent comp;
    if (!this.TryComp<VehicleWeaponsComponent>(vehicle, out comp))
      return false;
    foreach (KeyValuePair<EntityUid, EntityUid> operatorSelection in comp.OperatorSelections)
    {
      if (this.Exists(operatorSelection.Key) && !(operatorSelection.Value != weapon) && this.IsSelectableMountedWeapon(vehicle, operatorSelection.Value))
      {
        operatorUid = operatorSelection.Key;
        return true;
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleWeaponsOperatorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
    EntityUid uid;
    VehicleWeaponsOperatorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? vehicle1 = comp1.Vehicle;
      EntityUid entityUid1 = vehicle;
      if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() != entityUid1 ? 1 : 0) : 1) == 0)
      {
        EntityUid? selectedWeapon = comp1.SelectedWeapon;
        EntityUid entityUid2 = weapon;
        if ((selectedWeapon.HasValue ? (selectedWeapon.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) == 0)
        {
          operatorUid = uid;
          return true;
        }
      }
    }
    return false;
  }

  private void RecalculateSelectedWeapon(
    EntityUid vehicle,
    VehicleWeaponsComponent weapons,
    ItemSlotsComponent? itemSlots = null)
  {
    EntityUid? nullable = weapons.Operator;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      EntityUid mountedWeapon;
      if (weapons.OperatorSelections.TryGetValue(valueOrDefault, out mountedWeapon))
      {
        if (!this.IsSelectableMountedWeapon(vehicle, mountedWeapon, itemSlots: itemSlots))
        {
          weapons.SelectedWeapon = new EntityUid?();
          return;
        }
        weapons.SelectedWeapon = new EntityUid?(mountedWeapon);
        return;
      }
    }
    weapons.SelectedWeapon = new EntityUid?();
  }

  private bool IsSelectableMountedWeapon(
    EntityUid vehicle,
    EntityUid mountedWeapon,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    return this.Exists(mountedWeapon) && this.HasComp<VehicleTurretComponent>(mountedWeapon) && this.HasComp<GunComponent>(mountedWeapon) && this._topology.TryGetMountedSlotByItem(vehicle, mountedWeapon, out VehicleMountedSlot _, hardpoints, itemSlots);
  }

  private void OnWeaponsUiOpened(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref BoundUIOpenedEvent args)
  {
    EntityUid? vehicle;
    if (!object.Equals((object) args.UiKey, (object) VehicleWeaponsUiKey.Key) || !this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return;
    EntityUid entityUid = vehicle.Value;
    VehicleWeaponsComponent comp;
    if (!this.TryComp<VehicleWeaponsComponent>(entityUid, out comp))
      return;
    EntityUid actor = args.Actor;
    if (actor == new EntityUid() || !this.Exists(actor))
      return;
    this.UpdateWeaponsUi(ent.Owner, entityUid, comp, operatorUid: new EntityUid?(actor));
  }

  private void OnWeaponsUiClosed(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref BoundUIClosedEvent args)
  {
    object.Equals((object) args.UiKey, (object) VehicleWeaponsUiKey.Key);
  }

  private void OnWeaponsSelect(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref VehicleWeaponsSelectMessage args)
  {
    if (!object.Equals((object) args.UiKey, (object) VehicleWeaponsUiKey.Key) || args.Actor == new EntityUid() || !this.Exists(args.Actor) || !this.CanUseHardpointActions(args.Actor, true))
      return;
    EntityUid? mountedWeapon = args.MountedEntity.HasValue ? new EntityUid?(this.GetEntity(args.MountedEntity.Value)) : new EntityUid?();
    this.TrySelectHardpoint(ent.Owner, args.Actor, mountedWeapon, true);
  }

  private void OnWeaponsStabilization(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref VehicleWeaponsStabilizationMessage args)
  {
    EntityUid? vehicle;
    if (!object.Equals((object) args.UiKey, (object) VehicleWeaponsUiKey.Key) || args.Actor == new EntityUid() || !this.Exists(args.Actor) || !this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return;
    EntityUid entityUid1 = vehicle.Value;
    VehicleWeaponsComponent comp1;
    if (!this.TryComp<VehicleWeaponsComponent>(entityUid1, out comp1))
      return;
    EntityUid? buckledTo = comp1.Operator;
    EntityUid actor = args.Actor;
    BuckleComponent comp2;
    if ((buckledTo.HasValue ? (buckledTo.GetValueOrDefault() != actor ? 1 : 0) : 1) != 0 || !this.TryComp<BuckleComponent>(args.Actor, out comp2))
      return;
    buckledTo = comp2.BuckledTo;
    EntityUid owner = ent.Owner;
    if ((buckledTo.HasValue ? (buckledTo.GetValueOrDefault() != owner ? 1 : 0) : 1) != 0)
      return;
    HardpointSlotsComponent component1 = (HardpointSlotsComponent) null;
    ItemSlotsComponent component2 = (ItemSlotsComponent) null;
    EntityUid entityUid2;
    EntityUid targetUid;
    VehicleTurretComponent targetTurret;
    if (!this.Resolve<HardpointSlotsComponent>(entityUid1, ref component1, false) || !this.Resolve<ItemSlotsComponent>(entityUid1, ref component2, false) || !comp1.OperatorSelections.TryGetValue(args.Actor, out entityUid2) || !this.Exists(entityUid2) || !this.TryComp<VehicleTurretComponent>(entityUid2, out VehicleTurretComponent _) || !this._turretSystem.TryResolveRotationTarget(entityUid2, out targetUid, out targetTurret) || !targetTurret.RotateToCursor)
      return;
    targetTurret.StabilizedRotation = args.Enabled;
    Angle worldRotation = this._transform.GetWorldRotation(entityUid1);
    Angle angle1 = Angle.op_Addition(targetTurret.WorldRotation, worldRotation);
    Angle angle2 = ((Angle) ref angle1).Reduced();
    targetTurret.TargetRotation = !args.Enabled ? targetTurret.WorldRotation : angle2;
    this.Dirty(targetUid, (IComponent) targetTurret);
    this.UpdateWeaponsUiForAllOperators(entityUid1, comp1, component1, component2);
  }

  private void OnWeaponsAutoMode(
    Entity<VehicleWeaponsSeatComponent> ent,
    ref VehicleWeaponsAutoModeMessage args)
  {
    EntityUid? vehicle;
    if (!object.Equals((object) args.UiKey, (object) VehicleWeaponsUiKey.Key) || args.Actor == new EntityUid() || !this.Exists(args.Actor) || !this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return;
    EntityUid entityUid = vehicle.Value;
    VehicleWeaponsComponent comp1;
    if (!this.TryComp<VehicleWeaponsComponent>(entityUid, out comp1))
      return;
    EntityUid? buckledTo = comp1.Operator;
    EntityUid actor = args.Actor;
    BuckleComponent comp2;
    if ((buckledTo.HasValue ? (buckledTo.GetValueOrDefault() != actor ? 1 : 0) : 1) != 0 || !this.TryComp<BuckleComponent>(args.Actor, out comp2))
      return;
    buckledTo = comp2.BuckledTo;
    EntityUid owner = ent.Owner;
    VehicleDeployableComponent comp3;
    if ((buckledTo.HasValue ? (buckledTo.GetValueOrDefault() != owner ? 1 : 0) : 1) != 0 || !this.TryComp<VehicleDeployableComponent>(entityUid, out comp3))
      return;
    comp3.AutoTurretEnabled = args.Enabled;
    this.Dirty(entityUid, (IComponent) comp3);
    this.UpdateWeaponsUiForAllOperators(entityUid, comp1);
  }

  private void UpdateWeaponsUi(
    EntityUid seat,
    EntityUid vehicle,
    VehicleWeaponsComponent? weapons = null,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null,
    EntityUid? operatorUid = null)
  {
    if (this._net.IsClient || !this.Resolve<VehicleWeaponsComponent>(vehicle, ref weapons, false) || !this.Resolve<HardpointSlotsComponent>(vehicle, ref hardpoints, false) || !this.Resolve<ItemSlotsComponent>(vehicle, ref itemSlots, false))
      return;
    if (!operatorUid.HasValue)
      operatorUid = weapons.Operator;
    VehicleWeaponsSeatComponent seatComp = (VehicleWeaponsSeatComponent) null;
    EntityUid entityUid1;
    if (operatorUid.HasValue)
      this.TryGetUserWeaponsSeat(operatorUid.Value, out entityUid1, out seatComp);
    EntityUid? operatorSelection = new EntityUid?();
    EntityUid entityUid2;
    if (operatorUid.HasValue && weapons.OperatorSelections.TryGetValue(operatorUid.Value, out entityUid2))
      operatorSelection = new EntityUid?(entityUid2);
    if (!operatorSelection.HasValue && operatorUid.HasValue && seatComp != null && !seatComp.AllowUiSelection)
    {
      EntityUid? nullable = weapons.Operator;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        EntityUid entityUid3;
        if (weapons.OperatorSelections.TryGetValue(valueOrDefault, out entityUid3))
          operatorSelection = new EntityUid?(entityUid3);
      }
    }
    List<VehicleMountedSlot> mountedSlots = this._topology.GetMountedSlots(vehicle, hardpoints, itemSlots);
    List<VehicleWeaponsUiEntry> hardpoints1 = new List<VehicleWeaponsUiEntry>(mountedSlots.Count);
    bool canUseHardpointActions = !operatorUid.HasValue || this.CanUseHardpointActions(operatorUid.Value, true);
    foreach (VehicleMountedSlot mountedSlot in mountedSlots)
      hardpoints1.Add(this.CreateMountedSlotUiEntry(mountedSlot, weapons, operatorUid, operatorSelection, canUseHardpointActions, seatComp));
    bool canToggleStabilization = false;
    bool stabilizationEnabled = false;
    bool canToggleAuto = false;
    bool autoEnabled = false;
    int num;
    if (operatorUid.HasValue)
    {
      EntityUid? nullable1 = weapons.Operator;
      EntityUid? nullable2 = operatorUid;
      num = nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0;
    }
    else
      num = 0;
    VehicleTurretComponent targetTurret;
    if (num != 0 && operatorSelection.HasValue && this.Exists(operatorSelection.Value) && this.TryComp<VehicleTurretComponent>(operatorSelection.Value, out VehicleTurretComponent _) && this._turretSystem.TryResolveRotationTarget(operatorSelection.Value, out entityUid1, out targetTurret))
    {
      stabilizationEnabled = targetTurret.StabilizedRotation;
      canToggleStabilization = targetTurret.RotateToCursor;
    }
    VehicleDeployableComponent comp;
    if (num != 0 && this.TryComp<VehicleDeployableComponent>(vehicle, out comp))
    {
      canToggleAuto = true;
      autoEnabled = comp.AutoTurretEnabled;
    }
    this._ui.SetUiState((Entity<UserInterfaceComponent>) seat, (Enum) VehicleWeaponsUiKey.Key, (BoundUserInterfaceState) new VehicleWeaponsUiState(this.GetNetEntity(vehicle), hardpoints1, canToggleStabilization, stabilizationEnabled, canToggleAuto, autoEnabled));
  }

  private VehicleWeaponsUiEntry CreateMountedSlotUiEntry(
    VehicleMountedSlot mountedSlot,
    VehicleWeaponsComponent weapons,
    EntityUid? operatorUid,
    EntityUid? operatorSelection,
    bool canUseHardpointActions,
    VehicleWeaponsSeatComponent? operatorSeatComp)
  {
    bool flag1 = operatorSeatComp == null || this.IsHardpointTypeAllowed(operatorSeatComp, mountedSlot.HardpointType);
    bool flag2 = VehicleWeaponsSystem.IsSharedHardpointType(mountedSlot.HardpointType);
    bool hasValue = mountedSlot.Item.HasValue;
    EntityUid? nullable1 = mountedSlot.Item;
    string installedName = (string) null;
    NetEntity? nullable2 = new NetEntity?();
    if (nullable1.HasValue)
    {
      installedName = this.Name(nullable1.Value);
      nullable2 = new NetEntity?(this.GetNetEntity(nullable1.Value));
    }
    string operatorName = (string) null;
    bool operatorIsSelf = false;
    EntityUid uid = new EntityUid();
    bool flag3 = nullable1.HasValue && weapons.HardpointOperators.TryGetValue(nullable1.Value, out uid);
    if (flag3)
    {
      operatorName = this.Name(uid);
      operatorIsSelf = operatorUid.HasValue && uid == operatorUid.Value;
    }
    bool selectable = canUseHardpointActions & flag1 && nullable1.HasValue && this.HasComp<VehicleTurretComponent>(nullable1.Value);
    if (selectable & flag3 && !operatorIsSelf && !flag2)
      selectable = false;
    bool selected = nullable1.HasValue && operatorSelection.HasValue && operatorSelection.Value == nullable1.Value;
    int ammoCount = 0;
    int ammoCapacity = 0;
    bool hasAmmo = false;
    float cooldownRemaining = 0.0f;
    float cooldownTotal = 0.0f;
    bool isOnCooldown = false;
    GunComponent comp1;
    if (nullable1.HasValue && this.TryComp<GunComponent>(nullable1.Value, out comp1))
    {
      GetAmmoCountEvent args = new GetAmmoCountEvent();
      this.RaiseLocalEvent<GetAmmoCountEvent>(nullable1.Value, ref args);
      ammoCount = args.Count;
      ammoCapacity = args.Capacity;
      hasAmmo = args.Capacity > 0;
      if ((double) comp1.FireRateModified > 0.0)
        cooldownTotal = 1f / comp1.FireRateModified;
      TimeSpan timeSpan = comp1.NextFire - this._timing.CurTime;
      if (timeSpan > TimeSpan.Zero)
      {
        cooldownRemaining = (float) timeSpan.TotalSeconds;
        isOnCooldown = (double) cooldownRemaining > 1.0 / 1000.0;
      }
    }
    int num = 0;
    int storedMagazines = 0;
    int maxStoredMagazines = 0;
    bool hasMagazineData = false;
    float integrity = 0.0f;
    float maxIntegrity = 0.0f;
    bool hasIntegrity = false;
    VehicleHardpointAmmoComponent comp2;
    if (nullable1.HasValue && this.TryComp<VehicleHardpointAmmoComponent>(nullable1.Value, out comp2))
    {
      num = Math.Max(1, comp2.MagazineSize);
      BallisticAmmoProviderComponent comp3;
      if (this.TryComp<BallisticAmmoProviderComponent>(nullable1.Value, out comp3))
        num = this._hardpointAmmo.GetMagazineSize(comp2, comp3);
      storedMagazines = this._hardpointAmmo.GetStoredRounds(comp2, num);
      maxStoredMagazines = this._hardpointAmmo.GetMaxStoredRounds(comp2, num);
      hasMagazineData = comp2.MagazineSize > 0 || comp2.MaxStoredMagazines > 0;
    }
    else
    {
      RMCVehicleHardpointAmmoComponent comp4;
      if (nullable1.HasValue && this.TryComp<RMCVehicleHardpointAmmoComponent>(nullable1.Value, out comp4))
      {
        num = Math.Max(1, comp4.MagazineSize);
        BallisticAmmoProviderComponent comp5;
        if (this.TryComp<BallisticAmmoProviderComponent>(nullable1.Value, out comp5))
          num = Math.Min(num, Math.Max(1, comp5.Capacity));
        storedMagazines = comp4.StoredMagazines * num;
        maxStoredMagazines = comp4.MaxStoredMagazines * num;
        hasMagazineData = comp4.MagazineSize > 0 || comp4.MaxStoredMagazines > 0;
      }
    }
    HardpointIntegrityComponent comp6;
    if (nullable1.HasValue && this.TryComp<HardpointIntegrityComponent>(nullable1.Value, out comp6))
    {
      integrity = comp6.Integrity;
      maxIntegrity = comp6.MaxIntegrity;
      hasIntegrity = true;
    }
    return new VehicleWeaponsUiEntry(mountedSlot.CompositeId, mountedSlot.HardpointType, nullable2, installedName, nullable2, hasValue, selectable, selected, ammoCount, ammoCapacity, hasAmmo, num, storedMagazines, maxStoredMagazines, hasMagazineData, operatorName, operatorIsSelf, integrity, maxIntegrity, hasIntegrity, cooldownRemaining, cooldownTotal, isOnCooldown);
  }

  private void UpdateWeaponsUiForAllOperators(
    EntityUid vehicle,
    VehicleWeaponsComponent weapons,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null,
    bool refreshActions = false)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleWeaponsOperatorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
    EntityUid uid;
    VehicleWeaponsOperatorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? vehicle1 = comp1.Vehicle;
      EntityUid entityUid = vehicle;
      EntityUid seat;
      if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0 && this.TryGetUserWeaponsSeat(uid, out seat, out VehicleWeaponsSeatComponent _))
      {
        if (refreshActions)
          this.RefreshHardpointActions(uid, vehicle, weapons, comp1, hardpoints, itemSlots);
        this.UpdateWeaponsUi(seat, vehicle, weapons, hardpoints, itemSlots, new EntityUid?(uid));
      }
    }
  }

  private readonly record struct HardpointActionSlot(
    EntityUid MountedWeapon,
    EntityUid IconEntity,
    string DisplayName)
  ;
}
