using System;
using System.Collections.Generic;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleHardpointVisualsSystem : EntitySystem
{
	[Dependency]
	private readonly ItemSlotsSystem _itemSlots;

	[Dependency]
	private readonly INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentInit>((EntityEventRefHandler<VehicleHardpointVisualsComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointVisualsComponent, MapInitEvent>((EntityEventRefHandler<VehicleHardpointVisualsComponent, MapInitEvent>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentGetState>((EntityEventRefHandler<VehicleHardpointVisualsComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsChangedEvent>((EntityEventHandler<HardpointSlotsChangedEvent>)OnHardpointSlotsChanged, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<VehicleHardpointVisualsComponent> ent, ref ComponentInit args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			UpdateAppearance(ent.Owner);
		}
	}

	private void OnInit(Entity<VehicleHardpointVisualsComponent> ent, ref MapInitEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			UpdateAppearance(ent.Owner);
		}
	}

	private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && ((EntitySystem)this).HasComp<VehicleHardpointVisualsComponent>(args.Vehicle))
		{
			UpdateAppearance(args.Vehicle);
		}
	}

	private void OnGetState(Entity<VehicleHardpointVisualsComponent> ent, ref ComponentGetState args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			List<VehicleHardpointLayerState> layers = new List<VehicleHardpointLayerState>(ent.Comp.Layers);
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new VehicleHardpointVisualsComponentState(layers);
		}
	}

	private void UpdateAppearance(EntityUid vehicle, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null, VehicleHardpointVisualsComponent? visuals = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent, VehicleHardpointVisualsComponent>(vehicle, ref hardpoints, ref itemSlots, ref visuals, false))
		{
			return;
		}
		List<VehicleHardpointLayerState> newLayers = new List<VehicleHardpointLayerState>(hardpoints.Slots.Count);
		Dictionary<string, int> indexByLayer = new Dictionary<string, int>();
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			string layer = slot.VisualLayer;
			string layerKey = layer.ToLowerInvariant();
			string state = string.Empty;
			bool usesOverlay = false;
			if (!_itemSlots.TryGetSlot(vehicle, slot.Id, out ItemSlot itemSlot, itemSlots))
			{
				continue;
			}
			if (itemSlot.HasItem)
			{
				EntityUid item = itemSlot.Item.Value;
				state = ResolveVisualState(item, out usesOverlay);
			}
			if (string.IsNullOrWhiteSpace(layer))
			{
				continue;
			}
			if (usesOverlay)
			{
				state = string.Empty;
			}
			layerKey = layer.ToLowerInvariant();
			if (indexByLayer.TryGetValue(layerKey, out var existingIndex))
			{
				if (!string.IsNullOrWhiteSpace(state))
				{
					newLayers[existingIndex] = new VehicleHardpointLayerState(layer, state);
				}
			}
			else
			{
				indexByLayer[layerKey] = newLayers.Count;
				newLayers.Add(new VehicleHardpointLayerState(layer, state));
			}
		}
		if (visuals.Layers.Count == newLayers.Count)
		{
			bool unchanged = true;
			for (int i = 0; i < visuals.Layers.Count; i++)
			{
				if (!visuals.Layers[i].Equals(newLayers[i]))
				{
					unchanged = false;
					break;
				}
			}
			if (unchanged)
			{
				return;
			}
		}
		visuals.Layers = newLayers;
		((EntitySystem)this).Dirty(vehicle, (IComponent)(object)visuals, (MetaDataComponent)null);
	}

	private string ResolveVisualState(EntityUid item, out bool usesOverlay, int depth = 0)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		usesOverlay = false;
		if (depth > 2)
		{
			return string.Empty;
		}
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (((EntitySystem)this).TryComp<VehicleTurretComponent>(item, ref turret) && turret.ShowOverlay)
		{
			usesOverlay = true;
		}
		HardpointSlotsComponent attachedSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent attachedItemSlots = default(ItemSlotsComponent);
		if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(item, ref attachedSlots) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(item, ref attachedItemSlots))
		{
			foreach (HardpointSlot slot in attachedSlots.Slots)
			{
				if (!string.IsNullOrWhiteSpace(slot.Id) && _itemSlots.TryGetSlot(item, slot.Id, out ItemSlot itemSlot, attachedItemSlots) && itemSlot.HasItem)
				{
					EntityUid child = itemSlot.Item.Value;
					bool childOverlay;
					string childState = ResolveVisualState(child, out childOverlay, depth + 1);
					usesOverlay |= childOverlay;
					if (!string.IsNullOrWhiteSpace(childState))
					{
						return childState;
					}
				}
			}
		}
		HardpointVisualComponent visual = default(HardpointVisualComponent);
		if (((EntitySystem)this).TryComp<HardpointVisualComponent>(item, ref visual) && !string.IsNullOrWhiteSpace(visual.VehicleState))
		{
			return visual.VehicleState;
		}
		VehicleTurretComponent turretOverlay = default(VehicleTurretComponent);
		if (((EntitySystem)this).TryComp<VehicleTurretComponent>(item, ref turretOverlay) && !string.IsNullOrWhiteSpace(turretOverlay.OverlayState))
		{
			return turretOverlay.OverlayState;
		}
		return string.Empty;
	}
}
