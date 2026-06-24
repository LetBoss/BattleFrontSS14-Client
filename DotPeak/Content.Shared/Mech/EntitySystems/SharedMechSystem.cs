// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mech.EntitySystems.SharedMechSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Mech.EntitySystems;

public abstract class SharedMechSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MechComponent, MechToggleEquipmentEvent>(new ComponentEventHandler<MechComponent, MechToggleEquipmentEvent>(this.OnToggleEquipmentAction));
    this.SubscribeLocalEvent<MechComponent, MechEjectPilotEvent>(new ComponentEventHandler<MechComponent, MechEjectPilotEvent>(this.OnEjectPilotEvent));
    this.SubscribeLocalEvent<MechComponent, UserActivateInWorldEvent>(new ComponentEventHandler<MechComponent, UserActivateInWorldEvent>(this.RelayInteractionEvent));
    this.SubscribeLocalEvent<MechComponent, ComponentStartup>(new ComponentEventHandler<MechComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<MechComponent, DestructionEventArgs>(new ComponentEventHandler<MechComponent, DestructionEventArgs>(this.OnDestruction));
    this.SubscribeLocalEvent<MechComponent, GetAdditionalAccessEvent>(new ComponentEventRefHandler<MechComponent, GetAdditionalAccessEvent>(this.OnGetAdditionalAccess));
    this.SubscribeLocalEvent<MechComponent, DragDropTargetEvent>(new ComponentEventRefHandler<MechComponent, DragDropTargetEvent>(this.OnDragDrop));
    this.SubscribeLocalEvent<MechComponent, CanDropTargetEvent>(new ComponentEventRefHandler<MechComponent, CanDropTargetEvent>(this.OnCanDragDrop));
    this.SubscribeLocalEvent<MechPilotComponent, GetMeleeWeaponEvent>(new ComponentEventHandler<MechPilotComponent, GetMeleeWeaponEvent>(this.OnGetMeleeWeapon));
    this.SubscribeLocalEvent<MechPilotComponent, CanAttackFromContainerEvent>(new ComponentEventHandler<MechPilotComponent, CanAttackFromContainerEvent>(this.OnCanAttackFromContainer));
    this.SubscribeLocalEvent<MechPilotComponent, AttackAttemptEvent>(new ComponentEventHandler<MechPilotComponent, AttackAttemptEvent>(this.OnAttackAttempt));
    this.InitializeRelay();
  }

  private void OnToggleEquipmentAction(
    EntityUid uid,
    MechComponent component,
    MechToggleEquipmentEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.CycleEquipment(uid);
  }

  private void OnEjectPilotEvent(EntityUid uid, MechComponent component, MechEjectPilotEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.TryEject(uid, component);
  }

  private void RelayInteractionEvent(
    EntityUid uid,
    MechComponent component,
    UserActivateInWorldEvent args)
  {
    if (!component.PilotSlot.ContainedEntity.HasValue || !this._timing.IsFirstTimePredicted || !component.CurrentSelectedEquipment.HasValue)
      return;
    this.RaiseLocalEvent<UserActivateInWorldEvent>(component.CurrentSelectedEquipment.Value, args);
  }

  private void OnStartup(EntityUid uid, MechComponent component, ComponentStartup args)
  {
    component.PilotSlot = this._container.EnsureContainer<ContainerSlot>(uid, component.PilotSlotId);
    component.EquipmentContainer = this._container.EnsureContainer<Container>(uid, component.EquipmentContainerId);
    component.BatterySlot = this._container.EnsureContainer<ContainerSlot>(uid, component.BatterySlotId);
    this.UpdateAppearance(uid, component);
  }

  private void OnDestruction(EntityUid uid, MechComponent component, DestructionEventArgs args)
  {
    this.BreakMech(uid, component);
  }

  private void OnGetAdditionalAccess(
    EntityUid uid,
    MechComponent component,
    ref GetAdditionalAccessEvent args)
  {
    EntityUid? containedEntity = component.PilotSlot.ContainedEntity;
    if (!containedEntity.HasValue)
      return;
    args.Entities.Add(containedEntity.Value);
  }

  private void SetupUser(EntityUid mech, EntityUid pilot, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(mech, ref component))
      return;
    MechPilotComponent mechPilotComponent = this.EnsureComp<MechPilotComponent>(pilot);
    InteractionRelayComponent component1 = this.EnsureComp<InteractionRelayComponent>(pilot);
    this._mover.SetRelay(pilot, mech);
    this._interaction.SetRelay(pilot, new EntityUid?(mech), component1);
    mechPilotComponent.Mech = mech;
    this.Dirty(pilot, (IComponent) mechPilotComponent);
    if (this._net.IsClient)
      return;
    this._actions.AddAction(pilot, ref component.MechCycleActionEntity, (string) component.MechCycleAction, mech);
    this._actions.AddAction(pilot, ref component.MechUiActionEntity, (string) component.MechUiAction, mech);
    this._actions.AddAction(pilot, ref component.MechEjectActionEntity, (string) component.MechEjectAction, mech);
  }

  private void RemoveUser(EntityUid mech, EntityUid pilot)
  {
    if (!this.RemComp<MechPilotComponent>(pilot))
      return;
    this.RemComp<RelayInputMoverComponent>(pilot);
    this.RemComp<InteractionRelayComponent>(pilot);
    this._actions.RemoveProvidedActions(pilot, mech);
  }

  public virtual void BreakMech(EntityUid uid, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component))
      return;
    this.TryEject(uid, component);
    foreach (EntityUid toRemove in new List<EntityUid>((IEnumerable<EntityUid>) component.EquipmentContainer.ContainedEntities))
      this.RemoveEquipment(uid, toRemove, component, forced: true);
    component.Broken = true;
    this.UpdateAppearance(uid, component);
  }

  public void CycleEquipment(EntityUid uid, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component))
      return;
    List<EntityUid> list = component.EquipmentContainer.ContainedEntities.ToList<EntityUid>();
    int num = -1;
    if (component.CurrentSelectedEquipment.HasValue)
      num = list.FindIndex(new Predicate<EntityUid>(StartIndex));
    int index = num + 1;
    component.CurrentSelectedEquipment = index >= list.Count ? new EntityUid?() : new EntityUid?(list[index]);
    string message = component.CurrentSelectedEquipment.HasValue ? this.Loc.GetString("mech-equipment-select-popup", ("item", (object) component.CurrentSelectedEquipment)) : this.Loc.GetString("mech-equipment-select-none-popup");
    if (this._net.IsServer)
      this._popup.PopupEntity(message, uid);
    this.Dirty(uid, (IComponent) component);

    bool StartIndex(EntityUid u)
    {
      EntityUid entityUid = u;
      EntityUid? selectedEquipment = component.CurrentSelectedEquipment;
      return selectedEquipment.HasValue && entityUid == selectedEquipment.GetValueOrDefault();
    }
  }

  public void InsertEquipment(
    EntityUid uid,
    EntityUid toInsert,
    MechComponent? component = null,
    MechEquipmentComponent? equipmentComponent = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component) || !this.Resolve<MechEquipmentComponent>(toInsert, ref equipmentComponent) || component.EquipmentContainer.ContainedEntities.Count >= component.MaxEquipmentAmount || this._whitelistSystem.IsWhitelistFail(component.EquipmentWhitelist, toInsert))
      return;
    equipmentComponent.EquipmentOwner = new EntityUid?(uid);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) toInsert, (BaseContainer) component.EquipmentContainer);
    MechEquipmentInsertedEvent args = new MechEquipmentInsertedEvent(uid);
    this.RaiseLocalEvent<MechEquipmentInsertedEvent>(toInsert, ref args);
    this.UpdateUserInterface(uid, component);
  }

  public void RemoveEquipment(
    EntityUid uid,
    EntityUid toRemove,
    MechComponent? component = null,
    MechEquipmentComponent? equipmentComponent = null,
    bool forced = false)
  {
    if (!this.Resolve<MechComponent>(uid, ref component) || !this.Resolve<MechEquipmentComponent>(toRemove, ref equipmentComponent))
      return;
    if (!forced)
    {
      AttemptRemoveMechEquipmentEvent args = new AttemptRemoveMechEquipmentEvent();
      this.RaiseLocalEvent<AttemptRemoveMechEquipmentEvent>(toRemove, ref args);
      if (args.Cancelled)
        return;
    }
    MechEquipmentRemovedEvent args1 = new MechEquipmentRemovedEvent(uid);
    this.RaiseLocalEvent<MechEquipmentRemovedEvent>(toRemove, ref args1);
    EntityUid? selectedEquipment = component.CurrentSelectedEquipment;
    EntityUid entityUid = toRemove;
    if ((selectedEquipment.HasValue ? (selectedEquipment.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
      this.CycleEquipment(uid, component);
    equipmentComponent.EquipmentOwner = new EntityUid?();
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) toRemove, (BaseContainer) component.EquipmentContainer);
    this.UpdateUserInterface(uid, component);
  }

  public virtual bool TryChangeEnergy(EntityUid uid, FixedPoint2 delta, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component) || component.Energy + delta < 0)
      return false;
    component.Energy = FixedPoint2.Clamp(component.Energy + delta, (FixedPoint2) 0, component.MaxEnergy);
    this.Dirty(uid, (IComponent) component);
    this.UpdateUserInterface(uid, component);
    return true;
  }

  public void SetIntegrity(EntityUid uid, FixedPoint2 value, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component))
      return;
    component.Integrity = FixedPoint2.Clamp(value, (FixedPoint2) 0, component.MaxIntegrity);
    if (component.Integrity <= 0)
      this.BreakMech(uid, component);
    else if (component.Broken)
    {
      component.Broken = false;
      this.UpdateAppearance(uid, component);
    }
    this.Dirty(uid, (IComponent) component);
    this.UpdateUserInterface(uid, component);
  }

  public bool IsEmpty(MechComponent component) => !component.PilotSlot.ContainedEntity.HasValue;

  public bool CanInsert(EntityUid uid, EntityUid toInsert, MechComponent? component = null)
  {
    return this.Resolve<MechComponent>(uid, ref component) && this.IsEmpty(component) && this._actionBlocker.CanMove(toInsert);
  }

  public virtual void UpdateUserInterface(EntityUid uid, MechComponent? component = null)
  {
  }

  public bool TryInsert(EntityUid uid, EntityUid? toInsert, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component) || !toInsert.HasValue)
      return false;
    EntityUid? containedEntity = component.PilotSlot.ContainedEntity;
    EntityUid? nullable = toInsert;
    if ((containedEntity.HasValue == nullable.HasValue ? (containedEntity.HasValue ? (containedEntity.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 || !this.CanInsert(uid, toInsert.Value, component))
      return false;
    this.SetupUser(uid, toInsert.Value);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) toInsert.Value, (BaseContainer) component.PilotSlot);
    this.UpdateAppearance(uid, component);
    return true;
  }

  public bool TryEject(EntityUid uid, MechComponent? component = null)
  {
    if (!this.Resolve<MechComponent>(uid, ref component) || !component.PilotSlot.ContainedEntity.HasValue)
      return false;
    EntityUid entityUid = component.PilotSlot.ContainedEntity.Value;
    this.RemoveUser(uid, entityUid);
    this._container.RemoveEntity(uid, entityUid);
    this.UpdateAppearance(uid, component);
    return true;
  }

  private void OnGetMeleeWeapon(
    EntityUid uid,
    MechPilotComponent component,
    GetMeleeWeaponEvent args)
  {
    MechComponent comp;
    if (args.Handled || !this.TryComp<MechComponent>(component.Mech, out comp))
      return;
    EntityUid entityUid = comp.CurrentSelectedEquipment ?? component.Mech;
    args.Weapon = new EntityUid?(entityUid);
    args.Handled = true;
  }

  private void OnCanAttackFromContainer(
    EntityUid uid,
    MechPilotComponent component,
    CanAttackFromContainerEvent args)
  {
    args.CanAttack = true;
  }

  private void OnAttackAttempt(
    EntityUid uid,
    MechPilotComponent component,
    AttackAttemptEvent args)
  {
    EntityUid? target = args.Target;
    EntityUid mech = component.Mech;
    if ((target.HasValue ? (target.GetValueOrDefault() == mech ? 1 : 0) : 0) == 0)
      return;
    args.Cancel();
  }

  private void UpdateAppearance(
    EntityUid uid,
    MechComponent? component = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<MechComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
      return;
    this._appearance.SetData(uid, (Enum) MechVisuals.Open, (object) this.IsEmpty(component), appearance);
    this._appearance.SetData(uid, (Enum) MechVisuals.Broken, (object) component.Broken, appearance);
  }

  private void OnDragDrop(EntityUid uid, MechComponent component, ref DragDropTargetEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.Dragged, component.EntryDelay, (DoAfterEvent) new MechEntryEvent(), new EntityUid?(uid), new EntityUid?(uid))
    {
      BreakOnMove = true
    });
  }

  private void OnCanDragDrop(EntityUid uid, MechComponent component, ref CanDropTargetEvent args)
  {
    args.Handled = true;
    ref bool local = ref args.CanDrop;
    local = ((local ? 1 : 0) | (component.Broken ? 0 : (this.CanInsert(uid, args.Dragged, component) ? 1 : 0))) != 0;
  }

  private void InitializeRelay()
  {
    this.SubscribeLocalEvent<MechComponent, GettingAttackedAttemptEvent>(new EntityEventRefHandler<MechComponent, GettingAttackedAttemptEvent>(this.RelayRefToPilot<GettingAttackedAttemptEvent>));
  }

  private void RelayToPilot<T>(Entity<MechComponent> uid, T args) where T : class
  {
    EntityUid? containedEntity = uid.Comp.PilotSlot.ContainedEntity;
    if (!containedEntity.HasValue)
      return;
    EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
    MechPilotRelayedEvent<T> args1 = new MechPilotRelayedEvent<T>(args);
    this.RaiseLocalEvent<MechPilotRelayedEvent<T>>(valueOrDefault, ref args1);
  }

  private void RelayRefToPilot<T>(Entity<MechComponent> uid, ref T args) where T : struct
  {
    EntityUid? containedEntity = uid.Comp.PilotSlot.ContainedEntity;
    if (!containedEntity.HasValue)
      return;
    EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
    MechPilotRelayedEvent<T> args1 = new MechPilotRelayedEvent<T>(args);
    this.RaiseLocalEvent<MechPilotRelayedEvent<T>>(valueOrDefault, ref args1);
    args = args1.Args;
  }
}
