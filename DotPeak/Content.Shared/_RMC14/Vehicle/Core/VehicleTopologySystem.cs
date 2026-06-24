// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTopologySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleTopologySystem : EntitySystem
{
  [Dependency]
  private readonly SharedContainerSystem _containers;
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;

  public bool TryGetVehicle(EntityUid uid, out EntityUid vehicle, bool includeSelf = true)
  {
    return this.TryGetContainerAncestor<VehicleComponent>(uid, out vehicle, includeSelf);
  }

  public bool TryGetParentTurret(EntityUid uid, out EntityUid turret, bool includeSelf = false)
  {
    return this.TryGetContainerAncestor<VehicleTurretComponent>(uid, out turret, includeSelf);
  }

  public List<VehicleMountedSlot> GetMountedSlots(
    EntityUid vehicle,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    List<VehicleMountedSlot> result = new List<VehicleMountedSlot>();
    if (!this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
      return result;
    this.EnumerateMountedSlots(vehicle, vehicle, hardpoints, itemSlots, result, new VehicleSlotPath?(), new EntityUid?());
    return result;
  }

  public HashSet<string> GetMountedSlotIds(
    EntityUid vehicle,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    HashSet<string> result = new HashSet<string>();
    if (!this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
      return result;
    this.PopulateMountedSlotIds(vehicle, hardpoints, itemSlots, result, (string) null);
    return result;
  }

  public bool TryGetMountedSlot(
    EntityUid vehicle,
    string slotId,
    out VehicleMountedSlot mountedSlot,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    mountedSlot = new VehicleMountedSlot();
    VehicleSlotPath path;
    return VehicleSlotPath.TryParse(slotId, out path) && this.TryGetMountedSlot(vehicle, path, out mountedSlot, hardpoints, itemSlots);
  }

  public bool TryGetMountedSlot(
    EntityUid vehicle,
    VehicleSlotPath path,
    out VehicleMountedSlot mountedSlot,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    mountedSlot = new VehicleMountedSlot();
    return path.IsValid && this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false) && this.TryGetMountedSlotRecursive(vehicle, vehicle, path, hardpoints, itemSlots, new VehicleSlotPath?(), new EntityUid?(), out mountedSlot);
  }

  public bool TryGetMountedSlotByItem(
    EntityUid vehicle,
    EntityUid item,
    out VehicleMountedSlot mountedSlot,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    mountedSlot = new VehicleMountedSlot();
    return this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false) && this.TryGetMountedSlotByItemRecursive(vehicle, vehicle, item, hardpoints, itemSlots, new VehicleSlotPath?(), new EntityUid?(), out mountedSlot);
  }

  public bool TryGetMountedSlotItem(
    EntityUid vehicle,
    string slotId,
    out EntityUid item,
    ItemSlotsComponent? itemSlots = null,
    HardpointSlotsComponent? hardpoints = null)
  {
    item = new EntityUid();
    VehicleMountedSlot mountedSlot;
    if (this.TryGetMountedSlot(vehicle, slotId, out mountedSlot, hardpoints, itemSlots))
    {
      EntityUid? nullable = mountedSlot.Item;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        item = valueOrDefault;
        return true;
      }
    }
    return false;
  }

  public bool TryGetMountedSlotHardpointType(
    EntityUid vehicle,
    string slotId,
    out string hardpointType,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    hardpointType = string.Empty;
    VehicleMountedSlot mountedSlot;
    if (!this.TryGetMountedSlot(vehicle, slotId, out mountedSlot, hardpoints, itemSlots))
      return false;
    hardpointType = mountedSlot.HardpointType;
    return true;
  }

  public List<VehicleMountedAmmoProvider> GetMountedAmmoProviders(
    EntityUid vehicle,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    List<VehicleMountedAmmoProvider> mountedAmmoProviders = new List<VehicleMountedAmmoProvider>();
    foreach (VehicleMountedSlot mountedSlot in this.GetMountedSlots(vehicle, hardpoints, itemSlots))
    {
      EntityUid? nullable = mountedSlot.Item;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        VehicleMountedAmmoProvider provider;
        if (this.TryGetAmmoProviderFromItem(mountedSlot, valueOrDefault, out provider))
          mountedAmmoProviders.Add(provider);
      }
    }
    return mountedAmmoProviders;
  }

  public bool TryGetMountedAmmoProvider(
    EntityUid vehicle,
    string? slotId,
    out VehicleMountedAmmoProvider provider,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    provider = new VehicleMountedAmmoProvider();
    VehicleSlotPath path;
    return VehicleSlotPath.TryParse(slotId, out path) && this.TryGetMountedAmmoProvider(vehicle, path, out provider, hardpoints, itemSlots);
  }

  public bool TryGetMountedAmmoProvider(
    EntityUid vehicle,
    VehicleSlotPath path,
    out VehicleMountedAmmoProvider provider,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    provider = new VehicleMountedAmmoProvider();
    VehicleMountedSlot mountedSlot;
    if (path.IsValid && this.TryGetMountedSlot(vehicle, path, out mountedSlot, hardpoints, itemSlots))
    {
      EntityUid? nullable = mountedSlot.Item;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        return this.TryGetAmmoProviderFromItem(mountedSlot, valueOrDefault, out provider);
      }
    }
    return false;
  }

  public bool TryGetPrimaryTurret(
    EntityUid vehicle,
    out EntityUid turretUid,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    turretUid = new EntityUid();
    if (!this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
      return false;
    foreach (HardpointSlot slot in hardpoints.Slots)
    {
      ItemSlot itemSlot;
      if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(vehicle, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
      {
        EntityUid? nullable = itemSlot.Item;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          if (this.HasComp<VehicleTurretComponent>(valueOrDefault) && !this.HasComp<VehicleTurretAttachmentComponent>(valueOrDefault))
          {
            turretUid = valueOrDefault;
            return true;
          }
        }
      }
    }
    return false;
  }

  private bool TryGetAmmoProviderFromItem(
    VehicleMountedSlot slot,
    EntityUid item,
    out VehicleMountedAmmoProvider provider)
  {
    provider = new VehicleMountedAmmoProvider();
    BallisticAmmoProviderComponent comp1;
    VehicleHardpointAmmoComponent comp2;
    RefillableByBulletBoxComponent comp3;
    if (!this.TryComp<BallisticAmmoProviderComponent>(item, out comp1) || !this.TryComp<VehicleHardpointAmmoComponent>(item, out comp2) || !this.TryComp<RefillableByBulletBoxComponent>(item, out comp3))
      return false;
    provider = new VehicleMountedAmmoProvider(slot, item, comp1, comp2, comp3);
    return true;
  }

  private bool TryGetContainerAncestor<TComponent>(
    EntityUid uid,
    out EntityUid ancestor,
    bool includeSelf = false)
    where TComponent : IComponent
  {
    ancestor = new EntityUid();
    Robust.Shared.GameObjects.EntityQuery<TComponent> entityQuery = this.GetEntityQuery<TComponent>();
    if (includeSelf && entityQuery.HasComp(uid))
    {
      ancestor = uid;
      return true;
    }
    BaseContainer container;
    EntityUid owner;
    for (EntityUid entityUid = uid; this._containers.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entityUid, (TransformComponent) null), out container); entityUid = owner)
    {
      owner = container.Owner;
      if (entityQuery.HasComp(owner))
      {
        ancestor = owner;
        return true;
      }
    }
    return false;
  }

  private void EnumerateMountedSlots(
    EntityUid vehicle,
    EntityUid slotOwner,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots,
    List<VehicleMountedSlot> result,
    VehicleSlotPath? parentPath,
    EntityUid? parentItem)
  {
    foreach (HardpointSlot slot in hardpoints.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        EntityUid? nullable = new EntityUid?();
        ItemSlot itemSlot;
        if (this._itemSlots.TryGetSlot(slotOwner, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
          nullable = itemSlot.Item;
        VehicleSlotPath Path = parentPath.HasValue ? parentPath.GetValueOrDefault().Append(slot.Id) : new VehicleSlotPath(slot.Id);
        result.Add(new VehicleMountedSlot(vehicle, slotOwner, slot.Id, Path, slot.HardpointType, nullable, parentItem, parentPath));
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          HardpointSlotsComponent comp1;
          ItemSlotsComponent comp2;
          if (this.TryComp<HardpointSlotsComponent>(valueOrDefault, out comp1) && this.TryComp<ItemSlotsComponent>(valueOrDefault, out comp2))
            this.EnumerateMountedSlots(vehicle, valueOrDefault, comp1, comp2, result, new VehicleSlotPath?(Path), new EntityUid?(valueOrDefault));
        }
      }
    }
  }

  private void PopulateMountedSlotIds(
    EntityUid slotOwner,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots,
    HashSet<string> result,
    string? parentCompositeId)
  {
    foreach (HardpointSlot slot in hardpoints.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        string parentCompositeId1 = parentCompositeId == null ? slot.Id : VehicleTurretSlotIds.Compose(parentCompositeId, slot.Id);
        result.Add(parentCompositeId1);
        ItemSlot itemSlot;
        if (this._itemSlots.TryGetSlot(slotOwner, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
        {
          EntityUid? nullable = itemSlot.Item;
          if (nullable.HasValue)
          {
            EntityUid valueOrDefault = nullable.GetValueOrDefault();
            HardpointSlotsComponent comp1;
            ItemSlotsComponent comp2;
            if (this.TryComp<HardpointSlotsComponent>(valueOrDefault, out comp1) && this.TryComp<ItemSlotsComponent>(valueOrDefault, out comp2))
              this.PopulateMountedSlotIds(valueOrDefault, comp1, comp2, result, parentCompositeId1);
          }
        }
      }
    }
  }

  private bool TryGetMountedSlotRecursive(
    EntityUid vehicle,
    EntityUid slotOwner,
    VehicleSlotPath targetPath,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots,
    VehicleSlotPath? parentPath,
    EntityUid? parentItem,
    out VehicleMountedSlot mountedSlot)
  {
    foreach (HardpointSlot slot in hardpoints.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        EntityUid? nullable = new EntityUid?();
        ItemSlot itemSlot;
        if (this._itemSlots.TryGetSlot(slotOwner, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
          nullable = itemSlot.Item;
        VehicleSlotPath vehicleSlotPath = parentPath.HasValue ? parentPath.GetValueOrDefault().Append(slot.Id) : new VehicleSlotPath(slot.Id);
        VehicleMountedSlot vehicleMountedSlot = new VehicleMountedSlot(vehicle, slotOwner, slot.Id, vehicleSlotPath, slot.HardpointType, nullable, parentItem, parentPath);
        if (VehicleTopologySystem.PathEquals(vehicleSlotPath, targetPath))
        {
          mountedSlot = vehicleMountedSlot;
          return true;
        }
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          HardpointSlotsComponent comp1;
          ItemSlotsComponent comp2;
          if (this.TryComp<HardpointSlotsComponent>(valueOrDefault, out comp1) && this.TryComp<ItemSlotsComponent>(valueOrDefault, out comp2) && this.TryGetMountedSlotRecursive(vehicle, valueOrDefault, targetPath, comp1, comp2, new VehicleSlotPath?(vehicleSlotPath), new EntityUid?(valueOrDefault), out mountedSlot))
            return true;
        }
      }
    }
    mountedSlot = new VehicleMountedSlot();
    return false;
  }

  private bool TryGetMountedSlotByItemRecursive(
    EntityUid vehicle,
    EntityUid slotOwner,
    EntityUid targetItem,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots,
    VehicleSlotPath? parentPath,
    EntityUid? parentItem,
    out VehicleMountedSlot mountedSlot)
  {
    foreach (HardpointSlot slot in hardpoints.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        EntityUid? nullable1 = new EntityUid?();
        ItemSlot itemSlot;
        if (this._itemSlots.TryGetSlot(slotOwner, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
          nullable1 = itemSlot.Item;
        VehicleSlotPath Path = parentPath.HasValue ? parentPath.GetValueOrDefault().Append(slot.Id) : new VehicleSlotPath(slot.Id);
        VehicleMountedSlot vehicleMountedSlot = new VehicleMountedSlot(vehicle, slotOwner, slot.Id, Path, slot.HardpointType, nullable1, parentItem, parentPath);
        EntityUid? nullable2 = nullable1;
        EntityUid entityUid = targetItem;
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        {
          mountedSlot = vehicleMountedSlot;
          return true;
        }
        if (nullable1.HasValue)
        {
          EntityUid valueOrDefault = nullable1.GetValueOrDefault();
          HardpointSlotsComponent comp1;
          ItemSlotsComponent comp2;
          if (this.TryComp<HardpointSlotsComponent>(valueOrDefault, out comp1) && this.TryComp<ItemSlotsComponent>(valueOrDefault, out comp2) && this.TryGetMountedSlotByItemRecursive(vehicle, valueOrDefault, targetItem, comp1, comp2, new VehicleSlotPath?(Path), new EntityUid?(valueOrDefault), out mountedSlot))
            return true;
        }
      }
    }
    mountedSlot = new VehicleMountedSlot();
    return false;
  }

  private static bool PathEquals(VehicleSlotPath left, VehicleSlotPath right)
  {
    return string.Equals(left.Root, right.Root, StringComparison.OrdinalIgnoreCase) && string.Equals(left.Child, right.Child, StringComparison.OrdinalIgnoreCase);
  }
}
