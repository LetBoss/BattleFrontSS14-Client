using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleTopologySystem : EntitySystem
{
	[Dependency]
	private readonly SharedContainerSystem _containers;

	[Dependency]
	private readonly ItemSlotsSystem _itemSlots;

	public bool TryGetVehicle(EntityUid uid, out EntityUid vehicle, bool includeSelf = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryGetContainerAncestor<VehicleComponent>(uid, out vehicle, includeSelf);
	}

	public bool TryGetParentTurret(EntityUid uid, out EntityUid turret, bool includeSelf = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryGetContainerAncestor<VehicleTurretComponent>(uid, out turret, includeSelf);
	}

	public List<VehicleMountedSlot> GetMountedSlots(EntityUid vehicle, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		List<VehicleMountedSlot> result = new List<VehicleMountedSlot>();
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			return result;
		}
		EnumerateMountedSlots(vehicle, vehicle, hardpoints, itemSlots, result, null, null);
		return result;
	}

	public HashSet<string> GetMountedSlotIds(EntityUid vehicle, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> result = new HashSet<string>();
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			return result;
		}
		PopulateMountedSlotIds(vehicle, hardpoints, itemSlots, result, null);
		return result;
	}

	public bool TryGetMountedSlot(EntityUid vehicle, string slotId, out VehicleMountedSlot mountedSlot, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		mountedSlot = default(VehicleMountedSlot);
		if (!VehicleSlotPath.TryParse(slotId, out var path))
		{
			return false;
		}
		return TryGetMountedSlot(vehicle, path, out mountedSlot, hardpoints, itemSlots);
	}

	public bool TryGetMountedSlot(EntityUid vehicle, VehicleSlotPath path, out VehicleMountedSlot mountedSlot, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		mountedSlot = default(VehicleMountedSlot);
		if (!path.IsValid || !((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			return false;
		}
		return TryGetMountedSlotRecursive(vehicle, vehicle, path, hardpoints, itemSlots, null, null, out mountedSlot);
	}

	public bool TryGetMountedSlotByItem(EntityUid vehicle, EntityUid item, out VehicleMountedSlot mountedSlot, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		mountedSlot = default(VehicleMountedSlot);
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			return false;
		}
		return TryGetMountedSlotByItemRecursive(vehicle, vehicle, item, hardpoints, itemSlots, null, null, out mountedSlot);
	}

	public bool TryGetMountedSlotItem(EntityUid vehicle, string slotId, out EntityUid item, ItemSlotsComponent? itemSlots = null, HardpointSlotsComponent? hardpoints = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		item = default(EntityUid);
		if (TryGetMountedSlot(vehicle, slotId, out var mountedSlot, hardpoints, itemSlots))
		{
			EntityUid? item2 = mountedSlot.Item;
			if (item2.HasValue)
			{
				EntityUid mountedItem = item2.GetValueOrDefault();
				item = mountedItem;
				return true;
			}
		}
		return false;
	}

	public bool TryGetMountedSlotHardpointType(EntityUid vehicle, string slotId, out string hardpointType, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		hardpointType = string.Empty;
		if (!TryGetMountedSlot(vehicle, slotId, out var mountedSlot, hardpoints, itemSlots))
		{
			return false;
		}
		hardpointType = mountedSlot.HardpointType;
		return true;
	}

	public List<VehicleMountedAmmoProvider> GetMountedAmmoProviders(EntityUid vehicle, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		List<VehicleMountedAmmoProvider> result = new List<VehicleMountedAmmoProvider>();
		foreach (VehicleMountedSlot slot in GetMountedSlots(vehicle, hardpoints, itemSlots))
		{
			EntityUid? item = slot.Item;
			if (item.HasValue)
			{
				EntityUid item2 = item.GetValueOrDefault();
				if (TryGetAmmoProviderFromItem(slot, item2, out var provider))
				{
					result.Add(provider);
				}
			}
		}
		return result;
	}

	public bool TryGetMountedAmmoProvider(EntityUid vehicle, string? slotId, out VehicleMountedAmmoProvider provider, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		provider = default(VehicleMountedAmmoProvider);
		if (!VehicleSlotPath.TryParse(slotId, out var path))
		{
			return false;
		}
		return TryGetMountedAmmoProvider(vehicle, path, out provider, hardpoints, itemSlots);
	}

	public bool TryGetMountedAmmoProvider(EntityUid vehicle, VehicleSlotPath path, out VehicleMountedAmmoProvider provider, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		provider = default(VehicleMountedAmmoProvider);
		if (path.IsValid && TryGetMountedSlot(vehicle, path, out var mountedSlot, hardpoints, itemSlots))
		{
			EntityUid? item = mountedSlot.Item;
			if (item.HasValue)
			{
				EntityUid item2 = item.GetValueOrDefault();
				return TryGetAmmoProviderFromItem(mountedSlot, item2, out provider);
			}
		}
		return false;
	}

	public bool TryGetPrimaryTurret(EntityUid vehicle, out EntityUid turretUid, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		turretUid = default(EntityUid);
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			return false;
		}
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id) || !_itemSlots.TryGetSlot(vehicle, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				continue;
			}
			EntityUid? item = itemSlot.Item;
			if (item.HasValue)
			{
				EntityUid item2 = item.GetValueOrDefault();
				if (((EntitySystem)this).HasComp<VehicleTurretComponent>(item2) && !((EntitySystem)this).HasComp<VehicleTurretAttachmentComponent>(item2))
				{
					turretUid = item2;
					return true;
				}
			}
		}
		return false;
	}

	private bool TryGetAmmoProviderFromItem(VehicleMountedSlot slot, EntityUid item, out VehicleMountedAmmoProvider provider)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		provider = default(VehicleMountedAmmoProvider);
		BallisticAmmoProviderComponent ammo = default(BallisticAmmoProviderComponent);
		if (!((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(item, ref ammo))
		{
			return false;
		}
		VehicleHardpointAmmoComponent hardpointAmmo = default(VehicleHardpointAmmoComponent);
		if (!((EntitySystem)this).TryComp<VehicleHardpointAmmoComponent>(item, ref hardpointAmmo))
		{
			return false;
		}
		RefillableByBulletBoxComponent refill = default(RefillableByBulletBoxComponent);
		if (!((EntitySystem)this).TryComp<RefillableByBulletBoxComponent>(item, ref refill))
		{
			return false;
		}
		provider = new VehicleMountedAmmoProvider(slot, item, ammo, hardpointAmmo, refill);
		return true;
	}

	private bool TryGetContainerAncestor<TComponent>(EntityUid uid, out EntityUid ancestor, bool includeSelf = false) where TComponent : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		ancestor = default(EntityUid);
		EntityQuery<TComponent> query = ((EntitySystem)this).GetEntityQuery<TComponent>();
		if (includeSelf && query.HasComp(uid))
		{
			ancestor = uid;
			return true;
		}
		EntityUid current = uid;
		BaseContainer container = default(BaseContainer);
		while (_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(current, null)), ref container))
		{
			EntityUid owner = container.Owner;
			if (query.HasComp(owner))
			{
				ancestor = owner;
				return true;
			}
			current = owner;
		}
		return false;
	}

	private void EnumerateMountedSlots(EntityUid vehicle, EntityUid slotOwner, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots, List<VehicleMountedSlot> result, VehicleSlotPath? parentPath, EntityUid? parentItem)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent nestedHardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent nestedItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			EntityUid? item = null;
			if (_itemSlots.TryGetSlot(slotOwner, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem)
			{
				item = itemSlot.Item;
			}
			VehicleSlotPath path = parentPath?.Append(slot.Id) ?? new VehicleSlotPath(slot.Id);
			result.Add(new VehicleMountedSlot(vehicle, slotOwner, slot.Id, path, slot.HardpointType, item, parentItem, parentPath));
			if (item.HasValue)
			{
				EntityUid nestedItem = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(nestedItem, ref nestedHardpoints) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(nestedItem, ref nestedItemSlots))
				{
					EnumerateMountedSlots(vehicle, nestedItem, nestedHardpoints, nestedItemSlots, result, path, nestedItem);
				}
			}
		}
	}

	private void PopulateMountedSlotIds(EntityUid slotOwner, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots, HashSet<string> result, string? parentCompositeId)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent nestedHardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent nestedItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			string compositeId = ((parentCompositeId == null) ? slot.Id : VehicleTurretSlotIds.Compose(parentCompositeId, slot.Id));
			result.Add(compositeId);
			if (!_itemSlots.TryGetSlot(slotOwner, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				continue;
			}
			EntityUid? item = itemSlot.Item;
			if (item.HasValue)
			{
				EntityUid nestedItem = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(nestedItem, ref nestedHardpoints) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(nestedItem, ref nestedItemSlots))
				{
					PopulateMountedSlotIds(nestedItem, nestedHardpoints, nestedItemSlots, result, compositeId);
				}
			}
		}
	}

	private bool TryGetMountedSlotRecursive(EntityUid vehicle, EntityUid slotOwner, VehicleSlotPath targetPath, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots, VehicleSlotPath? parentPath, EntityUid? parentItem, out VehicleMountedSlot mountedSlot)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent nestedHardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent nestedItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			EntityUid? item = null;
			if (_itemSlots.TryGetSlot(slotOwner, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem)
			{
				item = itemSlot.Item;
			}
			VehicleSlotPath path = parentPath?.Append(slot.Id) ?? new VehicleSlotPath(slot.Id);
			VehicleMountedSlot current = new VehicleMountedSlot(vehicle, slotOwner, slot.Id, path, slot.HardpointType, item, parentItem, parentPath);
			if (PathEquals(path, targetPath))
			{
				mountedSlot = current;
				return true;
			}
			if (item.HasValue)
			{
				EntityUid nestedItem = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(nestedItem, ref nestedHardpoints) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(nestedItem, ref nestedItemSlots) && TryGetMountedSlotRecursive(vehicle, nestedItem, targetPath, nestedHardpoints, nestedItemSlots, path, nestedItem, out mountedSlot))
				{
					return true;
				}
			}
		}
		mountedSlot = default(VehicleMountedSlot);
		return false;
	}

	private bool TryGetMountedSlotByItemRecursive(EntityUid vehicle, EntityUid slotOwner, EntityUid targetItem, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots, VehicleSlotPath? parentPath, EntityUid? parentItem, out VehicleMountedSlot mountedSlot)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent nestedHardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent nestedItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			EntityUid? item = null;
			if (_itemSlots.TryGetSlot(slotOwner, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem)
			{
				item = itemSlot.Item;
			}
			VehicleSlotPath path = parentPath?.Append(slot.Id) ?? new VehicleSlotPath(slot.Id);
			VehicleMountedSlot current = new VehicleMountedSlot(vehicle, slotOwner, slot.Id, path, slot.HardpointType, item, parentItem, parentPath);
			EntityUid? val = item;
			if (val.HasValue && val.GetValueOrDefault() == targetItem)
			{
				mountedSlot = current;
				return true;
			}
			if (item.HasValue)
			{
				EntityUid nestedItem = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(nestedItem, ref nestedHardpoints) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(nestedItem, ref nestedItemSlots) && TryGetMountedSlotByItemRecursive(vehicle, nestedItem, targetItem, nestedHardpoints, nestedItemSlots, path, nestedItem, out mountedSlot))
				{
					return true;
				}
			}
		}
		mountedSlot = default(VehicleMountedSlot);
		return false;
	}

	private static bool PathEquals(VehicleSlotPath left, VehicleSlotPath right)
	{
		if (string.Equals(left.Root, right.Root, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(left.Child, right.Child, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}
}
