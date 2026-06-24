// Decompiled with JetBrains decompiler
// Type: Content.Shared.Payload.EntitySystems.ChemicalPayloadSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Payload.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Payload.EntitySystems;

public sealed class ChemicalPayloadSystem : EntitySystem
{
  [Dependency]
  private ItemSlotsSystem _itemSlotsSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ChemicalPayloadComponent, ComponentInit>(new ComponentEventHandler<ChemicalPayloadComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<ChemicalPayloadComponent, ComponentRemove>(new ComponentEventHandler<ChemicalPayloadComponent, ComponentRemove>(this.OnComponentRemove));
    this.SubscribeLocalEvent<ChemicalPayloadComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ChemicalPayloadComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified));
    this.SubscribeLocalEvent<ChemicalPayloadComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ChemicalPayloadComponent, EntRemovedFromContainerMessage>(this.OnContainerModified));
  }

  private void OnContainerModified(
    EntityUid uid,
    ChemicalPayloadComponent component,
    ContainerModifiedMessage args)
  {
    this.UpdateAppearance(uid, component);
  }

  private void UpdateAppearance(
    EntityUid uid,
    ChemicalPayloadComponent? component = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<ChemicalPayloadComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
      return;
    ChemicalPayloadFilledSlots payloadFilledSlots = ChemicalPayloadFilledSlots.None;
    if (component.BeakerSlotA.HasItem)
      payloadFilledSlots |= ChemicalPayloadFilledSlots.Left;
    if (component.BeakerSlotB.HasItem)
      payloadFilledSlots |= ChemicalPayloadFilledSlots.Right;
    this._appearance.SetData(uid, (Enum) ChemicalPayloadVisuals.Slots, (object) payloadFilledSlots, appearance);
  }

  private void OnComponentInit(EntityUid uid, ChemicalPayloadComponent payload, ComponentInit args)
  {
    this._itemSlotsSystem.AddItemSlot(uid, "BeakerSlotA", payload.BeakerSlotA);
    this._itemSlotsSystem.AddItemSlot(uid, "BeakerSlotB", payload.BeakerSlotB);
  }

  private void OnComponentRemove(
    EntityUid uid,
    ChemicalPayloadComponent payload,
    ComponentRemove args)
  {
    this._itemSlotsSystem.RemoveItemSlot(uid, payload.BeakerSlotA);
    this._itemSlotsSystem.RemoveItemSlot(uid, payload.BeakerSlotB);
  }
}
