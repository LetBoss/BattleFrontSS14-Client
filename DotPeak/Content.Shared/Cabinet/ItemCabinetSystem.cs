// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cabinet.ItemCabinetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Cabinet;

public sealed class ItemCabinetSystem : EntitySystem
{
  [Dependency]
  private ItemSlotsSystem _slots;
  [Dependency]
  private OpenableSystem _openable;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCabinetComponent, ComponentStartup>(new EntityEventRefHandler<ItemCabinetComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCabinetComponent, MapInitEvent>(new EntityEventRefHandler<ItemCabinetComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCabinetComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ItemCabinetComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnContainerModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCabinetComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ItemCabinetComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnContainerModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCabinetComponent, OpenableOpenedEvent>(new EntityEventRefHandler<ItemCabinetComponent, OpenableOpenedEvent>((object) this, __methodptr(OnOpened)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCabinetComponent, OpenableClosedEvent>(new EntityEventRefHandler<ItemCabinetComponent, OpenableClosedEvent>((object) this, __methodptr(OnClosed)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(Entity<ItemCabinetComponent> ent, ref ComponentStartup args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnMapInit(Entity<ItemCabinetComponent> ent, ref MapInitEvent args)
  {
    this.SetSlotLock(ent, !this._openable.IsOpen(Entity<ItemCabinetComponent>.op_Implicit(ent)));
  }

  private void UpdateAppearance(Entity<ItemCabinetComponent> ent)
  {
    this._appearance.SetData(Entity<ItemCabinetComponent>.op_Implicit(ent), (Enum) ItemCabinetVisuals.ContainsItem, (object) this.HasItem(ent), (AppearanceComponent) null);
  }

  private void OnContainerModified(
    EntityUid uid,
    ItemCabinetComponent component,
    ContainerModifiedMessage args)
  {
    if (!(args.Container.ID == component.Slot))
      return;
    this.UpdateAppearance(Entity<ItemCabinetComponent>.op_Implicit((uid, component)));
  }

  private void OnOpened(Entity<ItemCabinetComponent> ent, ref OpenableOpenedEvent args)
  {
    this.SetSlotLock(ent, false);
  }

  private void OnClosed(Entity<ItemCabinetComponent> ent, ref OpenableClosedEvent args)
  {
    this.SetSlotLock(ent, true);
  }

  public bool TryGetSlot(Entity<ItemCabinetComponent> ent, [NotNullWhen(true)] out ItemSlot? slot)
  {
    slot = (ItemSlot) null;
    ItemSlotsComponent component;
    return this.TryComp<ItemSlotsComponent>(Entity<ItemCabinetComponent>.op_Implicit(ent), ref component) && this._slots.TryGetSlot(Entity<ItemCabinetComponent>.op_Implicit(ent), ent.Comp.Slot, out slot, component);
  }

  public bool HasItem(Entity<ItemCabinetComponent> ent)
  {
    ItemSlot slot;
    return this.TryGetSlot(ent, out slot) && slot.HasItem;
  }

  public void SetSlotLock(Entity<ItemCabinetComponent> ent, bool closed)
  {
    ItemSlotsComponent itemSlotsComponent;
    ItemSlot itemSlot;
    if (!this.TryComp<ItemSlotsComponent>(Entity<ItemCabinetComponent>.op_Implicit(ent), ref itemSlotsComponent) || !this._slots.TryGetSlot(Entity<ItemCabinetComponent>.op_Implicit(ent), ent.Comp.Slot, out itemSlot, itemSlotsComponent))
      return;
    this._slots.SetLock(Entity<ItemCabinetComponent>.op_Implicit(ent), itemSlot, closed, itemSlotsComponent);
  }
}
