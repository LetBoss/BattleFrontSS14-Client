// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWheelSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Repairable;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleWheelSystem : EntitySystem
{
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;
  [Dependency]
  private readonly SharedAppearanceSystem _appearance;
  [Dependency]
  private readonly Content.Shared.Vehicle.VehicleSystem _vehicles;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly RMCRepairableSystem _repairable;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly SharedContainerSystem _containers;
  [Dependency]
  private readonly HardpointSystem _hardpoints;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleWheelSlotsComponent, ComponentInit>(new EntityEventRefHandler<VehicleWheelSlotsComponent, ComponentInit>(this.OnWheelInit));
    this.SubscribeLocalEvent<VehicleWheelSlotsComponent, MapInitEvent>(new EntityEventRefHandler<VehicleWheelSlotsComponent, MapInitEvent>(this.OnWheelMapInit));
    this.SubscribeLocalEvent<VehicleWheelSlotsComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<VehicleWheelSlotsComponent, EntInsertedIntoContainerMessage>(this.OnWheelInserted));
    this.SubscribeLocalEvent<VehicleWheelSlotsComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<VehicleWheelSlotsComponent, EntRemovedFromContainerMessage>(this.OnWheelRemoved));
    this.SubscribeLocalEvent<VehicleWheelSlotsComponent, VehicleCanRunEvent>(new EntityEventRefHandler<VehicleWheelSlotsComponent, VehicleCanRunEvent>(this.OnVehicleCanRun));
  }

  private void OnWheelInit(Entity<VehicleWheelSlotsComponent> ent, ref ComponentInit args)
  {
    this.EnsureSlots(ent.Owner, ent.Comp);
    this.UpdateAppearance(ent.Owner, ent.Comp);
  }

  private void OnWheelMapInit(Entity<VehicleWheelSlotsComponent> ent, ref MapInitEvent args)
  {
    this.EnsureSlots(ent.Owner, ent.Comp);
    this.UpdateAppearance(ent.Owner, ent.Comp);
  }

  private void OnWheelInserted(
    Entity<VehicleWheelSlotsComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    if (!this.IsWheelSlot(ent.Comp, args.Container.ID))
      return;
    this.UpdateAppearance(ent.Owner, ent.Comp);
    this.RefreshCanRun(ent.Owner);
  }

  private void OnWheelRemoved(
    Entity<VehicleWheelSlotsComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!this.IsWheelSlot(ent.Comp, args.Container.ID))
      return;
    this.UpdateAppearance(ent.Owner, ent.Comp);
    this.RefreshCanRun(ent.Owner);
  }

  private void OnVehicleCanRun(Entity<VehicleWheelSlotsComponent> ent, ref VehicleCanRunEvent args)
  {
    if (!args.CanRun || this.HasAllWheels(ent.Owner, ent.Comp))
      return;
    args.CanRun = false;
  }

  private void EnsureSlots(
    EntityUid uid,
    VehicleWheelSlotsComponent component,
    ItemSlotsComponent? itemSlots = null)
  {
    if (itemSlots == null)
      itemSlots = this.EnsureComp<ItemSlotsComponent>(uid);
    HardpointSlotsComponent comp;
    if (component.Slots.Count == 0 && this.TryComp<HardpointSlotsComponent>(uid, out comp))
    {
      foreach (HardpointSlot slot in comp.Slots)
      {
        if (string.Equals(slot.HardpointType, "Wheel", StringComparison.OrdinalIgnoreCase))
          component.Slots.Add(slot.Id);
      }
    }
    if (component.Slots.Count == 0)
    {
      for (int index = 0; index < component.SlotCount; ++index)
        component.Slots.Add($"{component.SlotPrefix}-{index + 1}");
    }
    if (component.WheelWhitelist.Components == null || component.WheelWhitelist.Components.Length == 0)
      component.WheelWhitelist.Components = new string[1]
      {
        "VehicleWheelItem"
      };
    foreach (string slot1 in component.Slots)
    {
      if (!this._itemSlots.TryGetSlot(uid, slot1, out ItemSlot _, itemSlots))
      {
        ItemSlot slot2 = new ItemSlot()
        {
          Whitelist = component.WheelWhitelist
        };
        this._itemSlots.AddItemSlot(uid, slot1, slot2, itemSlots);
        this._itemSlots.SetEjectFlags(uid, slot2, true, itemSlots: itemSlots);
      }
    }
  }

  private bool IsWheelSlot(VehicleWheelSlotsComponent component, string? id)
  {
    return id != null && component.Slots.Contains(id);
  }

  private bool HasAllWheels(
    EntityUid uid,
    VehicleWheelSlotsComponent? component = null,
    ItemSlotsComponent? itemSlots = null)
  {
    if (!this.Resolve<VehicleWheelSlotsComponent>(uid, ref component, false) || !this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false) || component.Slots.Count == 0)
      return false;
    foreach (string slot in component.Slots)
    {
      ItemSlot itemSlot;
      if (!this._itemSlots.TryGetSlot(uid, slot, out itemSlot, itemSlots) || !itemSlot.HasItem)
        return false;
      EntityUid? nullable = itemSlot.Item;
      if (!nullable.HasValue || !this.IsWheelFunctional(nullable.GetValueOrDefault()))
        return false;
    }
    return true;
  }

  private int GetWheelCount(
    EntityUid uid,
    VehicleWheelSlotsComponent component,
    ItemSlotsComponent? itemSlots = null)
  {
    int wheelCount = 0;
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return wheelCount;
    foreach (string slot in component.Slots)
    {
      ItemSlot itemSlot;
      if (this._itemSlots.TryGetSlot(uid, slot, out itemSlot, itemSlots) && itemSlot.HasItem)
        ++wheelCount;
    }
    return wheelCount;
  }

  private int GetFunctionalWheelCount(
    EntityUid uid,
    VehicleWheelSlotsComponent component,
    ItemSlotsComponent? itemSlots = null)
  {
    int functionalWheelCount = 0;
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return functionalWheelCount;
    foreach (string slot in component.Slots)
    {
      ItemSlot itemSlot;
      if (this._itemSlots.TryGetSlot(uid, slot, out itemSlot, itemSlots) && itemSlot.HasItem)
      {
        EntityUid? nullable = itemSlot.Item;
        if (nullable.HasValue && this.IsWheelFunctional(nullable.GetValueOrDefault()))
          ++functionalWheelCount;
      }
    }
    return functionalWheelCount;
  }

  private float GetAverageWheelIntegrityFraction(
    EntityUid uid,
    VehicleWheelSlotsComponent component,
    ItemSlotsComponent? itemSlots = null)
  {
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return 1f;
    float num1 = 0.0f;
    int num2 = 0;
    foreach (string slot in component.Slots)
    {
      ItemSlot itemSlot;
      if (this._itemSlots.TryGetSlot(uid, slot, out itemSlot, itemSlots) && itemSlot.HasItem)
      {
        ++num2;
        float num3 = 1f;
        EntityUid? nullable = itemSlot.Item;
        HardpointIntegrityComponent comp;
        if (nullable.HasValue && this.TryComp<HardpointIntegrityComponent>(nullable.GetValueOrDefault(), out comp))
        {
          float num4 = (double) comp.MaxIntegrity > 0.0 ? comp.MaxIntegrity : 1f;
          num3 = Math.Clamp(comp.Integrity / num4, 0.0f, 1f);
        }
        num1 += num3;
      }
    }
    return num2 == 0 ? 1f : Math.Clamp(num1 / (float) num2, 0.0f, 1f);
  }

  private void UpdateAppearance(EntityUid uid, VehicleWheelSlotsComponent component)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    bool flag = this.HasAllWheels(uid, component);
    this._appearance.SetData(uid, (Enum) VehicleWheelVisuals.HasAllWheels, (object) flag, comp);
    int wheelCount = this.GetWheelCount(uid, component);
    this._appearance.SetData(uid, (Enum) VehicleWheelVisuals.WheelCount, (object) wheelCount, comp);
    int functionalWheelCount = this.GetFunctionalWheelCount(uid, component);
    this._appearance.SetData(uid, (Enum) VehicleWheelVisuals.WheelFunctionalCount, (object) functionalWheelCount, comp);
    float integrityFraction = this.GetAverageWheelIntegrityFraction(uid, component);
    this._appearance.SetData(uid, (Enum) VehicleWheelVisuals.WheelIntegrityFraction, (object) integrityFraction, comp);
  }

  private bool IsWheelFunctional(EntityUid wheel, HardpointIntegrityComponent? integrity = null)
  {
    return !this.Resolve<HardpointIntegrityComponent>(wheel, ref integrity, false) || (double) integrity.Integrity > 0.0;
  }

  public void DamageWheels(EntityUid vehicle, float amount)
  {
    VehicleWheelSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if ((double) amount <= 0.0 || !this.TryComp<VehicleWheelSlotsComponent>(vehicle, out comp1) || !this.TryComp<ItemSlotsComponent>(vehicle, out comp2))
      return;
    bool flag = false;
    foreach (string slot in comp1.Slots)
    {
      ItemSlot itemSlot;
      if (this._itemSlots.TryGetSlot(vehicle, slot, out itemSlot, comp2))
      {
        EntityUid? nullable = itemSlot.Item;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          if (this._hardpoints.DamageHardpoint(vehicle, valueOrDefault, amount))
            flag = true;
        }
      }
    }
    if (!flag)
      return;
    this.UpdateAppearance(vehicle, comp1);
    this.RefreshCanRun(vehicle);
  }

  public void OnWheelDamaged(EntityUid vehicle)
  {
    VehicleWheelSlotsComponent comp;
    if (!this.TryComp<VehicleWheelSlotsComponent>(vehicle, out comp))
      return;
    this.UpdateAppearance(vehicle, comp);
    this.RefreshCanRun(vehicle);
  }

  private void RefreshCanRun(EntityUid uid)
  {
    VehicleComponent comp;
    if (!this.TryComp<VehicleComponent>(uid, out comp))
      return;
    this._vehicles.RefreshCanRun((Entity<VehicleComponent>) (uid, comp));
  }
}
