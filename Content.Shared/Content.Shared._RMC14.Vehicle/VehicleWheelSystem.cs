using System;
using Content.Shared._RMC14.Repairable;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Popups;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

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
		((EntitySystem)this).SubscribeLocalEvent<VehicleWheelSlotsComponent, ComponentInit>((EntityEventRefHandler<VehicleWheelSlotsComponent, ComponentInit>)OnWheelInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWheelSlotsComponent, MapInitEvent>((EntityEventRefHandler<VehicleWheelSlotsComponent, MapInitEvent>)OnWheelMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWheelSlotsComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<VehicleWheelSlotsComponent, EntInsertedIntoContainerMessage>)OnWheelInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWheelSlotsComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<VehicleWheelSlotsComponent, EntRemovedFromContainerMessage>)OnWheelRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWheelSlotsComponent, VehicleCanRunEvent>((EntityEventRefHandler<VehicleWheelSlotsComponent, VehicleCanRunEvent>)OnVehicleCanRun, (Type[])null, (Type[])null);
	}

	private void OnWheelInit(Entity<VehicleWheelSlotsComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EnsureSlots(ent.Owner, ent.Comp);
		UpdateAppearance(ent.Owner, ent.Comp);
	}

	private void OnWheelMapInit(Entity<VehicleWheelSlotsComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EnsureSlots(ent.Owner, ent.Comp);
		UpdateAppearance(ent.Owner, ent.Comp);
	}

	private void OnWheelInserted(Entity<VehicleWheelSlotsComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (IsWheelSlot(ent.Comp, ((ContainerModifiedMessage)args).Container.ID))
		{
			UpdateAppearance(ent.Owner, ent.Comp);
			RefreshCanRun(ent.Owner);
		}
	}

	private void OnWheelRemoved(Entity<VehicleWheelSlotsComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (IsWheelSlot(ent.Comp, ((ContainerModifiedMessage)args).Container.ID))
		{
			UpdateAppearance(ent.Owner, ent.Comp);
			RefreshCanRun(ent.Owner);
		}
	}

	private void OnVehicleCanRun(Entity<VehicleWheelSlotsComponent> ent, ref VehicleCanRunEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanRun && !HasAllWheels(ent.Owner, ent.Comp))
		{
			args.CanRun = false;
		}
	}

	private void EnsureSlots(EntityUid uid, VehicleWheelSlotsComponent component, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (itemSlots == null)
		{
			itemSlots = ((EntitySystem)this).EnsureComp<ItemSlotsComponent>(uid);
		}
		HardpointSlotsComponent hardpoints = default(HardpointSlotsComponent);
		if (component.Slots.Count == 0 && ((EntitySystem)this).TryComp<HardpointSlotsComponent>(uid, ref hardpoints))
		{
			foreach (HardpointSlot slot in hardpoints.Slots)
			{
				if (string.Equals(slot.HardpointType, "Wheel", StringComparison.OrdinalIgnoreCase))
				{
					component.Slots.Add(slot.Id);
				}
			}
		}
		if (component.Slots.Count == 0)
		{
			for (int i = 0; i < component.SlotCount; i++)
			{
				component.Slots.Add($"{component.SlotPrefix}-{i + 1}");
			}
		}
		if (component.WheelWhitelist.Components == null || component.WheelWhitelist.Components.Length == 0)
		{
			component.WheelWhitelist.Components = new string[1] { "VehicleWheelItem" };
		}
		foreach (string slotId in component.Slots)
		{
			if (!_itemSlots.TryGetSlot(uid, slotId, out ItemSlot _, itemSlots))
			{
				ItemSlot slot2 = new ItemSlot
				{
					Whitelist = component.WheelWhitelist
				};
				_itemSlots.AddItemSlot(uid, slotId, slot2, itemSlots);
				_itemSlots.SetEjectFlags(uid, slot2, disableEject: true, ejectOnInteract: false, ejectOnUse: false, itemSlots);
			}
		}
	}

	private bool IsWheelSlot(VehicleWheelSlotsComponent component, string? id)
	{
		if (id != null)
		{
			return component.Slots.Contains(id);
		}
		return false;
	}

	private bool HasAllWheels(EntityUid uid, VehicleWheelSlotsComponent? component = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VehicleWheelSlotsComponent>(uid, ref component, false) || !((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return false;
		}
		if (component.Slots.Count == 0)
		{
			return false;
		}
		foreach (string slotId in component.Slots)
		{
			if (!_itemSlots.TryGetSlot(uid, slotId, out ItemSlot slot, itemSlots) || !slot.HasItem)
			{
				return false;
			}
			EntityUid? item = slot.Item;
			if (item.HasValue)
			{
				EntityUid wheel = item.GetValueOrDefault();
				if (IsWheelFunctional(wheel))
				{
					continue;
				}
			}
			return false;
		}
		return true;
	}

	private int GetWheelCount(EntityUid uid, VehicleWheelSlotsComponent component, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return count;
		}
		foreach (string slotId in component.Slots)
		{
			if (_itemSlots.TryGetSlot(uid, slotId, out ItemSlot slot, itemSlots) && slot.HasItem)
			{
				count++;
			}
		}
		return count;
	}

	private int GetFunctionalWheelCount(EntityUid uid, VehicleWheelSlotsComponent component, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return count;
		}
		foreach (string slotId in component.Slots)
		{
			if (!_itemSlots.TryGetSlot(uid, slotId, out ItemSlot slot, itemSlots) || !slot.HasItem)
			{
				continue;
			}
			EntityUid? item = slot.Item;
			if (item.HasValue)
			{
				EntityUid wheel = item.GetValueOrDefault();
				if (IsWheelFunctional(wheel))
				{
					count++;
				}
			}
		}
		return count;
	}

	private float GetAverageWheelIntegrityFraction(EntityUid uid, VehicleWheelSlotsComponent component, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return 1f;
		}
		float total = 0f;
		int installed = 0;
		HardpointIntegrityComponent integrity = default(HardpointIntegrityComponent);
		foreach (string slotId in component.Slots)
		{
			if (!_itemSlots.TryGetSlot(uid, slotId, out ItemSlot slot, itemSlots) || !slot.HasItem)
			{
				continue;
			}
			installed++;
			float fraction = 1f;
			EntityUid? item = slot.Item;
			if (item.HasValue)
			{
				EntityUid wheel = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(wheel, ref integrity))
				{
					float max = ((integrity.MaxIntegrity > 0f) ? integrity.MaxIntegrity : 1f);
					fraction = Math.Clamp(integrity.Integrity / max, 0f, 1f);
				}
			}
			total += fraction;
		}
		if (installed == 0)
		{
			return 1f;
		}
		return Math.Clamp(total / (float)installed, 0f, 1f);
	}

	private void UpdateAppearance(EntityUid uid, VehicleWheelSlotsComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			bool hasAll = HasAllWheels(uid, component);
			_appearance.SetData(uid, (Enum)VehicleWheelVisuals.HasAllWheels, (object)hasAll, appearance);
			int count = GetWheelCount(uid, component);
			_appearance.SetData(uid, (Enum)VehicleWheelVisuals.WheelCount, (object)count, appearance);
			int functional = GetFunctionalWheelCount(uid, component);
			_appearance.SetData(uid, (Enum)VehicleWheelVisuals.WheelFunctionalCount, (object)functional, appearance);
			float averageIntegrity = GetAverageWheelIntegrityFraction(uid, component);
			_appearance.SetData(uid, (Enum)VehicleWheelVisuals.WheelIntegrityFraction, (object)averageIntegrity, appearance);
		}
	}

	private bool IsWheelFunctional(EntityUid wheel, HardpointIntegrityComponent? integrity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HardpointIntegrityComponent>(wheel, ref integrity, false))
		{
			return true;
		}
		return integrity.Integrity > 0f;
	}

	public void DamageWheels(EntityUid vehicle, float amount)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		VehicleWheelSlotsComponent wheels = default(VehicleWheelSlotsComponent);
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (amount <= 0f || !((EntitySystem)this).TryComp<VehicleWheelSlotsComponent>(vehicle, ref wheels) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicle, ref itemSlots))
		{
			return;
		}
		bool changed = false;
		foreach (string slotId in wheels.Slots)
		{
			if (!_itemSlots.TryGetSlot(vehicle, slotId, out ItemSlot slot, itemSlots))
			{
				continue;
			}
			EntityUid? item = slot.Item;
			if (item.HasValue)
			{
				EntityUid wheel = item.GetValueOrDefault();
				if (_hardpoints.DamageHardpoint(vehicle, wheel, amount))
				{
					changed = true;
				}
			}
		}
		if (changed)
		{
			UpdateAppearance(vehicle, wheels);
			RefreshCanRun(vehicle);
		}
	}

	public void OnWheelDamaged(EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		VehicleWheelSlotsComponent wheels = default(VehicleWheelSlotsComponent);
		if (((EntitySystem)this).TryComp<VehicleWheelSlotsComponent>(vehicle, ref wheels))
		{
			UpdateAppearance(vehicle, wheels);
			RefreshCanRun(vehicle);
		}
	}

	private void RefreshCanRun(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicle = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(uid, ref vehicle))
		{
			_vehicles.RefreshCanRun(Entity<VehicleComponent>.op_Implicit((uid, vehicle)));
		}
	}
}
