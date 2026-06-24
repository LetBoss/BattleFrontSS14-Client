// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.SharedBorgSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.PowerCell.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.UserInterface;
using Content.Shared.Wires;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Silicons.Borgs;

public abstract class SharedBorgSystem : EntitySystem
{
  [Dependency]
  protected SharedContainerSystem Container;
  [Dependency]
  protected ItemSlotsSystem ItemSlots;
  [Dependency]
  protected ItemToggleSystem Toggle;
  [Dependency]
  protected SharedPopupSystem Popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<BorgChassisComponent, ComponentStartup>(new ComponentEventHandler<BorgChassisComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<BorgChassisComponent, ItemSlotInsertAttemptEvent>(new ComponentEventRefHandler<BorgChassisComponent, ItemSlotInsertAttemptEvent>(this.OnItemSlotInsertAttempt));
    this.SubscribeLocalEvent<BorgChassisComponent, ItemSlotEjectAttemptEvent>(new ComponentEventRefHandler<BorgChassisComponent, ItemSlotEjectAttemptEvent>(this.OnItemSlotEjectAttempt));
    this.SubscribeLocalEvent<BorgChassisComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<BorgChassisComponent, EntInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<BorgChassisComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<BorgChassisComponent, EntRemovedFromContainerMessage>(this.OnRemoved));
    this.SubscribeLocalEvent<BorgChassisComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<BorgChassisComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovementSpeedModifiers));
    this.SubscribeLocalEvent<BorgChassisComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<BorgChassisComponent, ActivatableUIOpenAttemptEvent>(this.OnUIOpenAttempt));
    this.SubscribeLocalEvent<TryGetIdentityShortInfoEvent>(new EntityEventHandler<TryGetIdentityShortInfoEvent>(this.OnTryGetIdentityShortInfo));
    this.InitializeRelay();
  }

  private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent args)
  {
    if (args.Handled || !this.HasComp<BorgChassisComponent>(args.ForActor))
      return;
    args.Title = this.Name(args.ForActor).Trim();
    args.Handled = true;
  }

  private void OnItemSlotInsertAttempt(
    EntityUid uid,
    BorgChassisComponent component,
    ref ItemSlotInsertAttemptEvent args)
  {
    PowerCellSlotComponent comp1;
    WiresPanelComponent comp2;
    ItemSlot itemSlot;
    if (args.Cancelled || !this.TryComp<PowerCellSlotComponent>(uid, out comp1) || !this.TryComp<WiresPanelComponent>(uid, out comp2) || !this.ItemSlots.TryGetSlot(uid, comp1.CellSlotId, out itemSlot) || itemSlot != args.Slot)
      return;
    if (comp2.Open)
    {
      EntityUid? user = args.User;
      EntityUid entityUid = uid;
      if ((user.HasValue ? (user.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0)
        return;
    }
    args.Cancelled = true;
  }

  private void OnItemSlotEjectAttempt(
    EntityUid uid,
    BorgChassisComponent component,
    ref ItemSlotEjectAttemptEvent args)
  {
    PowerCellSlotComponent comp1;
    WiresPanelComponent comp2;
    ItemSlot itemSlot;
    if (args.Cancelled || !this.TryComp<PowerCellSlotComponent>(uid, out comp1) || !this.TryComp<WiresPanelComponent>(uid, out comp2) || !this.ItemSlots.TryGetSlot(uid, comp1.CellSlotId, out itemSlot) || itemSlot != args.Slot)
      return;
    if (comp2.Open)
    {
      EntityUid? user = args.User;
      EntityUid entityUid = uid;
      if ((user.HasValue ? (user.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0)
        return;
    }
    args.Cancelled = true;
  }

  private void OnStartup(EntityUid uid, BorgChassisComponent component, ComponentStartup args)
  {
    ContainerManagerComponent comp;
    if (!this.TryComp<ContainerManagerComponent>(uid, out comp))
      return;
    component.BrainContainer = this.Container.EnsureContainer<ContainerSlot>(uid, component.BrainContainerId, comp);
    component.ModuleContainer = this.Container.EnsureContainer<Robust.Shared.Containers.Container>(uid, component.ModuleContainerId, comp);
  }

  private void OnUIOpenAttempt(
    EntityUid uid,
    BorgChassisComponent component,
    ActivatableUIOpenAttemptEvent args)
  {
    if (!(args.User == uid))
      return;
    args.Cancel();
  }

  protected virtual void OnInserted(
    EntityUid uid,
    BorgChassisComponent component,
    EntInsertedIntoContainerMessage args)
  {
  }

  protected virtual void OnRemoved(
    EntityUid uid,
    BorgChassisComponent component,
    EntRemovedFromContainerMessage args)
  {
  }

  private void OnRefreshMovementSpeedModifiers(
    EntityUid uid,
    BorgChassisComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    MovementSpeedModifierComponent comp;
    if (this.Toggle.IsActivated((Entity<ItemToggleComponent>) uid) || !this.TryComp<MovementSpeedModifierComponent>(uid, out comp))
      return;
    float sprint = comp.BaseWalkSpeed / comp.BaseSprintSpeed;
    args.ModifySpeed(1f, sprint);
  }

  public void SetBorgModuleDefault(Entity<BorgModuleComponent> ent, bool newDefault)
  {
    ent.Comp.DefaultModule = newDefault;
    this.Dirty<BorgModuleComponent>(ent);
  }

  public void InitializeRelay()
  {
    this.SubscribeLocalEvent<BorgChassisComponent, DamageModifyEvent>(new ComponentEventHandler<BorgChassisComponent, DamageModifyEvent>(this.RelayToModule<DamageModifyEvent>));
  }

  protected void RelayToModule<T>(EntityUid uid, BorgChassisComponent component, T args) where T : class
  {
    BorgModuleRelayedEvent<T> args1 = new BorgModuleRelayedEvent<T>(args);
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) component.ModuleContainer.ContainedEntities)
      this.RaiseLocalEvent<BorgModuleRelayedEvent<T>>(containedEntity, ref args1);
  }

  protected void RelayRefToModule<T>(EntityUid uid, BorgChassisComponent component, ref T args) where T : class
  {
    BorgModuleRelayedEvent<T> args1 = new BorgModuleRelayedEvent<T>(args);
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) component.ModuleContainer.ContainedEntities)
      this.RaiseLocalEvent<BorgModuleRelayedEvent<T>>(containedEntity, ref args1);
  }
}
