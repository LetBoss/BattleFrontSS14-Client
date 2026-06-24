using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Input;
using Content.Shared.Vehicle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleSpotlightSystem : EntitySystem
{
	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly VehicleSystem _rmcVehicles;

	[Dependency]
	private readonly SharedPointLightSystem _lights;

	[Dependency]
	private readonly ItemSlotsSystem _itemSlots;

	public override void Initialize()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		((EntitySystem)this).SubscribeLocalEvent<VehicleSpotlightComponent, ComponentStartup>((EntityEventRefHandler<VehicleSpotlightComponent, ComponentStartup>)OnSpotlightStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsChangedEvent>((EntityEventHandler<HardpointSlotsChangedEvent>)OnHardpointSlotsChanged, (Type[])null, (Type[])null);
		if (_net.IsClient)
		{
			CommandBinds.Builder.Bind(ContentKeyFunctions.FlipObject, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
				if (val.HasValue)
				{
					EntityUid valueOrDefault = val.GetValueOrDefault();
					EntityUid? val2 = null;
					VehicleOperatorComponent vehicleOperatorComponent = default(VehicleOperatorComponent);
					EntityUid? vehicle;
					if (((EntitySystem)this).TryComp<VehicleOperatorComponent>(valueOrDefault, ref vehicleOperatorComponent) && vehicleOperatorComponent.Vehicle.HasValue)
					{
						val2 = vehicleOperatorComponent.Vehicle.Value;
					}
					else if (_rmcVehicles.TryGetVehicleFromInterior(valueOrDefault, out vehicle) && vehicle.HasValue)
					{
						val2 = vehicle.Value;
					}
					if (val2.HasValue)
					{
						((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new VehicleSpotlightToggleRequestEvent(((EntitySystem)this).GetNetEntity(val2.Value, (MetaDataComponent)null)));
					}
				}
			}, (StateInputCmdDelegate)null, true, true)).Register<VehicleSpotlightSystem>();
		}
		((EntitySystem)this).SubscribeNetworkEvent<VehicleSpotlightToggleRequestEvent>((EntitySessionEventHandler<VehicleSpotlightToggleRequestEvent>)OnSpotlightToggleRequest, (Type[])null, (Type[])null);
	}

	private void OnSpotlightStartup(Entity<VehicleSpotlightComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EnsureBase(ent.Comp);
		if (_net.IsServer)
		{
			RecalculateFromHardpoints(ent.Owner, ent.Comp);
		}
		ApplySpotlight(ent.Owner, ent.Comp);
	}

	private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		VehicleSpotlightComponent spotlight = default(VehicleSpotlightComponent);
		if (_net.IsServer && ((EntitySystem)this).TryComp<VehicleSpotlightComponent>(args.Vehicle, ref spotlight))
		{
			RecalculateFromHardpoints(args.Vehicle, spotlight);
			ApplySpotlight(args.Vehicle, spotlight);
			((EntitySystem)this).Dirty(args.Vehicle, (IComponent)(object)spotlight, (MetaDataComponent)null);
		}
	}

	private void OnSpotlightToggleRequest(VehicleSpotlightToggleRequestEvent ev, EntitySessionEventArgs args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			return;
		}
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid user = attachedEntity.GetValueOrDefault();
		EntityUid vehicle = ((EntitySystem)this).GetEntity(ev.Vehicle);
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp))
		{
			attachedEntity = vehicleComp.Operator;
			EntityUid val = user;
			VehicleSpotlightComponent spotlight = default(VehicleSpotlightComponent);
			if (attachedEntity.HasValue && !(attachedEntity.GetValueOrDefault() != val) && ((EntitySystem)this).TryComp<VehicleSpotlightComponent>(vehicle, ref spotlight))
			{
				spotlight.Enabled = !spotlight.Enabled;
				ApplySpotlight(vehicle, spotlight);
				((EntitySystem)this).Dirty(vehicle, (IComponent)(object)spotlight, (MetaDataComponent)null);
			}
		}
	}

	private void ApplySpotlight(EntityUid uid, VehicleSpotlightComponent spotlight)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = null;
		if (_lights.ResolveLight(uid, ref light))
		{
			_lights.SetRadius(uid, spotlight.Radius, light, (MetaDataComponent)null);
			_lights.SetEnergy(uid, spotlight.Energy, light);
			_lights.SetSoftness(uid, spotlight.Softness, light);
			_lights.SetEnabled(uid, spotlight.Enabled, light, (MetaDataComponent)null);
		}
	}

	private static void EnsureBase(VehicleSpotlightComponent spotlight)
	{
		if (!spotlight.BaseInitialized)
		{
			spotlight.BaseInitialized = true;
			spotlight.BaseRadius = spotlight.Radius;
			spotlight.BaseEnergy = spotlight.Energy;
			spotlight.BaseSoftness = spotlight.Softness;
		}
	}

	private void RecalculateFromHardpoints(EntityUid vehicle, VehicleSpotlightComponent spotlight, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		EnsureBase(spotlight);
		float radius = spotlight.BaseRadius;
		float energy = spotlight.BaseEnergy;
		float softness = spotlight.BaseSoftness;
		if (((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			VehicleSpotlightModifierComponent modifier = default(VehicleSpotlightModifierComponent);
			foreach (HardpointSlot slot in hardpoints.Slots)
			{
				if (!string.IsNullOrWhiteSpace(slot.Id) && _itemSlots.TryGetSlot(vehicle, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem)
				{
					EntityUid item = itemSlot.Item.Value;
					if (((EntitySystem)this).TryComp<VehicleSpotlightModifierComponent>(item, ref modifier))
					{
						radius = radius * modifier.RadiusMultiplier + modifier.RadiusAdd;
						energy = energy * modifier.EnergyMultiplier + modifier.EnergyAdd;
						softness = softness * modifier.SoftnessMultiplier + modifier.SoftnessAdd;
					}
				}
			}
		}
		spotlight.Radius = radius;
		spotlight.Energy = energy;
		spotlight.Softness = softness;
	}
}
