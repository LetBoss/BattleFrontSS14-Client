using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleWeaponsSystem : EntitySystem
{
	private readonly record struct HardpointActionSlot(EntityUid MountedWeapon, EntityUid IconEntity, string DisplayName);

	private const string HardpointSelectActionId = "ActionVehicleSelectHardpoint";

	[Dependency]
	private readonly SharedActionsSystem _actions;

	[Dependency]
	private readonly SharedUserInterfaceSystem _ui;

	[Dependency]
	private readonly SharedEyeSystem _eye;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SkillsSystem _skills;

	[Dependency]
	private readonly VehicleTopologySystem _topology;

	[Dependency]
	private readonly VehicleHardpointAmmoSystem _hardpointAmmo;

	[Dependency]
	private readonly VehicleSystem _vehicleSystem;

	[Dependency]
	private readonly VehicleTurretSystem _turretSystem;

	[Dependency]
	private readonly VehicleViewToggleSystem _viewToggle;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedContentEyeSystem _eyeSystem;

	[Dependency]
	private readonly SharedAudioSystem _audio;

	[Dependency]
	private readonly MetaDataSystem _metaData;

	[Dependency]
	private readonly IGameTiming _timing;

	private void OnHardpointActionSelect(Entity<VehicleWeaponsOperatorComponent> ent, ref VehicleHardpointSelectActionEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		if (_net.IsClient || ((HandledEntityEventArgs)args).Handled || args.Performer == default(EntityUid) || !((EntitySystem)this).Exists(args.Performer) || args.Performer != ent.Owner || !CanUseHardpointActions(args.Performer) || !((EntitySystem)this).TryComp<BuckleComponent>(args.Performer, ref buckle))
		{
			return;
		}
		EntityUid? buckledTo = buckle.BuckledTo;
		if (buckledTo.HasValue)
		{
			EntityUid seat = buckledTo.GetValueOrDefault();
			VehicleHardpointActionComponent hardpointAction = default(VehicleHardpointActionComponent);
			if (((EntitySystem)this).HasComp<VehicleWeaponsSeatComponent>(seat) && ((EntitySystem)this).TryComp<VehicleHardpointActionComponent>(Entity<ActionComponent>.op_Implicit(args.Action), ref hardpointAction) && TrySelectHardpoint(seat, args.Performer, hardpointAction.MountedWeapon, fromUi: false))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnViewToggled(Entity<VehicleWeaponsOperatorComponent> ent, ref VehicleViewToggledEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid? vehicle = ent.Comp.Vehicle;
		if (!vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicle2 = vehicle.GetValueOrDefault();
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicle2, ref weapons))
		{
			RefreshHardpointActions(ent.Owner, vehicle2, weapons, ent.Comp);
			if (TryGetUserWeaponsSeat(ent.Owner, out EntityUid seat, out VehicleWeaponsSeatComponent _))
			{
				UpdateWeaponsUi(seat, vehicle2, weapons, null, null, ent.Owner);
			}
		}
	}

	private void RefreshHardpointActions(EntityUid user, EntityUid vehicle, VehicleWeaponsComponent weapons, VehicleWeaponsOperatorComponent? operatorComp = null, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !((EntitySystem)this).Resolve<VehicleWeaponsOperatorComponent>(user, ref operatorComp, false))
		{
			return;
		}
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
		{
			ClearHardpointActions(user, operatorComp);
			return;
		}
		List<HardpointActionSlot> desired = (CanUseHardpointActions(user) ? GetSelectableHardpointActionSlots(vehicle, user, weapons, hardpoints, itemSlots) : new List<HardpointActionSlot>());
		HashSet<EntityUid> desiredSlots = new HashSet<EntityUid>(desired.Select((HardpointActionSlot slot) => slot.MountedWeapon));
		KeyValuePair<EntityUid, EntityUid>[] array = operatorComp.HardpointActions.ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			KeyValuePair<EntityUid, EntityUid> pair = array[num];
			if (!desiredSlots.Contains(pair.Key) || !((EntitySystem)this).Exists(pair.Value))
			{
				RemoveAndDeleteHardpointAction(user, pair.Value);
				operatorComp.HardpointActions.Remove(pair.Key);
			}
		}
		ActionComponent existingActionComp = default(ActionComponent);
		VehicleHardpointActionComponent existingHardpointAction = default(VehicleHardpointActionComponent);
		for (int i = 0; i < desired.Count; i++)
		{
			HardpointActionSlot desiredSlot = desired[i];
			if (operatorComp.HardpointActions.TryGetValue(desiredSlot.MountedWeapon, out var existingAction) && ((EntitySystem)this).Exists(existingAction) && ((EntitySystem)this).TryComp<ActionComponent>(existingAction, ref existingActionComp))
			{
				EntityUid? container = existingActionComp.Container;
				EntityUid iconEntity = desiredSlot.IconEntity;
				if (container.HasValue && container.GetValueOrDefault() == iconEntity)
				{
					if (((EntitySystem)this).TryComp<VehicleHardpointActionComponent>(existingAction, ref existingHardpointAction))
					{
						existingHardpointAction.MountedWeapon = desiredSlot.MountedWeapon;
						existingHardpointAction.SortOrder = i;
						((EntitySystem)this).Dirty(existingAction, (IComponent)(object)existingHardpointAction, (MetaDataComponent)null);
					}
					_actions.SetTemporary(Entity<ActionComponent>.op_Implicit((existingAction, existingActionComp)), temporary: false);
					_metaData.SetEntityName(existingAction, desiredSlot.DisplayName, (MetaDataComponent)null, true);
					continue;
				}
			}
			if (operatorComp.HardpointActions.TryGetValue(desiredSlot.MountedWeapon, out var staleAction) && ((EntitySystem)this).Exists(staleAction))
			{
				RemoveAndDeleteHardpointAction(user, staleAction);
				operatorComp.HardpointActions.Remove(desiredSlot.MountedWeapon);
			}
			EntityUid? action = null;
			if (_actions.AddAction(user, ref action, "ActionVehicleSelectHardpoint", desiredSlot.IconEntity) && action.HasValue)
			{
				VehicleHardpointActionComponent hardpointAction = ((EntitySystem)this).EnsureComp<VehicleHardpointActionComponent>(action.Value);
				hardpointAction.MountedWeapon = desiredSlot.MountedWeapon;
				hardpointAction.SortOrder = i;
				((EntitySystem)this).Dirty(action.Value, (IComponent)(object)hardpointAction, (MetaDataComponent)null);
				_actions.SetTemporary(Entity<ActionComponent>.op_Implicit((action.Value, ((EntitySystem)this).Comp<ActionComponent>(action.Value))), temporary: false);
				_metaData.SetEntityName(action.Value, desiredSlot.DisplayName, (MetaDataComponent)null, true);
				operatorComp.HardpointActions[desiredSlot.MountedWeapon] = action.Value;
			}
		}
		UpdateHardpointActionStates(user, weapons, operatorComp);
	}

	private List<HardpointActionSlot> GetSelectableHardpointActionSlots(EntityUid vehicle, EntityUid user, VehicleWeaponsComponent weapons, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		List<HardpointActionSlot> slots = new List<HardpointActionSlot>();
		if (!TryGetUserWeaponsSeat(user, out EntityUid _, out VehicleWeaponsSeatComponent seatComp))
		{
			return slots;
		}
		foreach (VehicleMountedSlot mountedSlot in _topology.GetMountedSlots(vehicle, hardpoints, itemSlots))
		{
			if (!IsHardpointTypeAllowed(seatComp, mountedSlot.HardpointType))
			{
				continue;
			}
			EntityUid? item = mountedSlot.Item;
			if (item.HasValue)
			{
				EntityUid installed = item.GetValueOrDefault();
				if (((EntitySystem)this).HasComp<VehicleTurretComponent>(installed) && ((EntitySystem)this).HasComp<GunComponent>(installed) && (IsSharedHardpointType(mountedSlot.HardpointType) || !weapons.HardpointOperators.TryGetValue(installed, out var slotOperator) || slotOperator == user))
				{
					slots.Add(new HardpointActionSlot(installed, installed, ((EntitySystem)this).Name(installed, (MetaDataComponent)null)));
				}
			}
		}
		return slots;
	}

	private void UpdateHardpointActionStates(EntityUid user, VehicleWeaponsComponent weapons, VehicleWeaponsOperatorComponent? operatorComp = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !((EntitySystem)this).Resolve<VehicleWeaponsOperatorComponent>(user, ref operatorComp, false))
		{
			return;
		}
		bool canUseHardpointActions = CanUseHardpointActions(user);
		EntityUid weapon;
		EntityUid? selectedWeapon = (weapons.OperatorSelections.TryGetValue(user, out weapon) ? new EntityUid?(weapon) : ((EntityUid?)null));
		foreach (KeyValuePair<EntityUid, EntityUid> pair in operatorComp.HardpointActions)
		{
			_actions.SetEnabled(Entity<ActionComponent>.op_Implicit(pair.Value), canUseHardpointActions);
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit(pair.Value), canUseHardpointActions && selectedWeapon.HasValue && pair.Key == selectedWeapon.Value);
		}
	}

	private void ClearHardpointActions(EntityUid user, VehicleWeaponsOperatorComponent? operatorComp = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !((EntitySystem)this).Resolve<VehicleWeaponsOperatorComponent>(user, ref operatorComp, false))
		{
			return;
		}
		EntityUid[] array = operatorComp.HardpointActions.Values.ToArray();
		foreach (EntityUid action in array)
		{
			if (((EntitySystem)this).Exists(action))
			{
				RemoveAndDeleteHardpointAction(user, action);
			}
		}
		operatorComp.HardpointActions.Clear();
	}

	private void RemoveAndDeleteHardpointAction(EntityUid user, EntityUid action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Exists(action))
		{
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionComponent>.op_Implicit(action));
			if (((EntitySystem)this).Exists(action))
			{
				((EntitySystem)this).QueueDel((EntityUid?)action);
			}
		}
	}

	private bool CanUseHardpointActions(EntityUid user, bool forUi = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetUserWeaponsSeat(user, out EntityUid _, out VehicleWeaponsSeatComponent seatComp))
		{
			return false;
		}
		if (forUi && !seatComp.AllowUiSelection)
		{
			return false;
		}
		if (!forUi && !seatComp.AllowHotbarSelection)
		{
			return false;
		}
		VehicleViewToggleComponent viewToggle = default(VehicleViewToggleComponent);
		if (((EntitySystem)this).TryComp<VehicleViewToggleComponent>(user, ref viewToggle) && !viewToggle.IsOutside)
		{
			return false;
		}
		return true;
	}

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, StrapAttemptEvent>((EntityEventRefHandler<VehicleWeaponsSeatComponent, StrapAttemptEvent>)OnWeaponSeatStrapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, StrappedEvent>((EntityEventRefHandler<VehicleWeaponsSeatComponent, StrappedEvent>)OnWeaponSeatStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, UnstrappedEvent>((EntityEventRefHandler<VehicleWeaponsSeatComponent, UnstrappedEvent>)OnWeaponSeatUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, BoundUIOpenedEvent>((EntityEventRefHandler<VehicleWeaponsSeatComponent, BoundUIOpenedEvent>)OnWeaponsUiOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, BoundUIClosedEvent>((EntityEventRefHandler<VehicleWeaponsSeatComponent, BoundUIClosedEvent>)OnWeaponsUiClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, VehicleWeaponsSelectMessage>((EntityEventRefHandler<VehicleWeaponsSeatComponent, VehicleWeaponsSelectMessage>)OnWeaponsSelect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, VehicleWeaponsStabilizationMessage>((EntityEventRefHandler<VehicleWeaponsSeatComponent, VehicleWeaponsStabilizationMessage>)OnWeaponsStabilization, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsSeatComponent, VehicleWeaponsAutoModeMessage>((EntityEventRefHandler<VehicleWeaponsSeatComponent, VehicleWeaponsAutoModeMessage>)OnWeaponsAutoMode, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsOperatorComponent, ComponentShutdown>((EntityEventRefHandler<VehicleWeaponsOperatorComponent, ComponentShutdown>)OnOperatorShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsOperatorComponent, ShotAttemptedEvent>((EntityEventRefHandler<VehicleWeaponsOperatorComponent, ShotAttemptedEvent>)OnOperatorShotAttempted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsOperatorComponent, VehicleHardpointSelectActionEvent>((EntityEventRefHandler<VehicleWeaponsOperatorComponent, VehicleHardpointSelectActionEvent>)OnHardpointActionSelect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsOperatorComponent, VehicleViewToggledEvent>((EntityEventRefHandler<VehicleWeaponsOperatorComponent, VehicleViewToggledEvent>)OnViewToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsChangedEvent>((EntityEventHandler<HardpointSlotsChangedEvent>)OnHardpointSlotsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretComponent, GunShotEvent>((EntityEventRefHandler<VehicleTurretComponent, GunShotEvent>)OnTurretGunShot, (Type[])null, (Type[])null);
	}

	private void OnWeaponSeatStrapAttempt(Entity<VehicleWeaponsSeatComponent> ent, ref StrapAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(args.Buckle.Owner), ent.Comp.Skills) && args.Popup)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-skills-cant-operate", (ValueTuple<string, object>)("target", ent)), Entity<BuckleComponent>.op_Implicit(args.Buckle), args.User);
		}
	}

	private void OnWeaponSeatStrapped(Entity<VehicleWeaponsSeatComponent> ent, ref StrappedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && _vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) && vehicle.HasValue)
		{
			EntityUid vehicleUid = vehicle.Value;
			VehicleWeaponsComponent weapons = ((EntitySystem)this).EnsureComp<VehicleWeaponsComponent>(vehicleUid);
			ClearOperatorSelections(weapons, args.Buckle.Owner);
			if (ent.Comp.IsPrimaryOperatorSeat)
			{
				weapons.Operator = args.Buckle.Owner;
			}
			RecalculateSelectedWeapon(vehicleUid, weapons);
			((EntitySystem)this).Dirty(vehicleUid, (IComponent)(object)weapons, (MetaDataComponent)null);
			VehicleWeaponsOperatorComponent operatorComp = ((EntitySystem)this).EnsureComp<VehicleWeaponsOperatorComponent>(args.Buckle.Owner);
			operatorComp.Vehicle = vehicle;
			operatorComp.SelectedWeapon = null;
			operatorComp.HardpointActions.Clear();
			((EntitySystem)this).Dirty(args.Buckle.Owner, (IComponent)(object)operatorComp, (MetaDataComponent)null);
			RefreshOperatorSelectedWeapons(vehicleUid, weapons);
			RefreshHardpointActions(args.Buckle.Owner, vehicleUid, weapons, operatorComp);
			if (((EntitySystem)this).HasComp<VehicleEnterComponent>(vehicleUid))
			{
				_eye.SetTarget(args.Buckle.Owner, (EntityUid?)vehicleUid, (EyeComponent)null);
				_viewToggle.EnableViewToggle(args.Buckle.Owner, vehicleUid, ent.Owner, null, isOutside: true);
			}
			UpdateGunnerView(args.Buckle.Owner, vehicleUid, ent.Comp);
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)VehicleWeaponsUiKey.Key, (EntityUid?)args.Buckle.Owner, false);
			UpdateWeaponsUiForAllOperators(vehicleUid, weapons);
		}
	}

	private void OnWeaponSeatUnstrapped(Entity<VehicleWeaponsSeatComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(args.Buckle.Owner, ref operatorComp))
		{
			ClearHardpointActions(args.Buckle.Owner, operatorComp);
		}
		((EntitySystem)this).RemCompDeferred<VehicleWeaponsOperatorComponent>(args.Buckle.Owner);
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)VehicleWeaponsUiKey.Key, (EntityUid?)args.Buckle.Owner, false);
		UpdateGunnerView(args.Buckle.Owner, ent.Owner, ent.Comp, removeOnly: true);
		_viewToggle.DisableViewToggle(args.Buckle.Owner, ent.Owner);
		if (!_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) || !vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicleUid = vehicle.Value;
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref weapons) && ent.Comp.IsPrimaryOperatorSeat)
		{
			EntityUid? val = weapons.Operator;
			EntityUid owner = args.Buckle.Owner;
			if (val.HasValue && val.GetValueOrDefault() == owner)
			{
				weapons.Operator = null;
				ClearOperatorSelections(weapons, args.Buckle.Owner);
				RecalculateSelectedWeapon(vehicleUid, weapons);
				((EntitySystem)this).Dirty(vehicleUid, (IComponent)(object)weapons, (MetaDataComponent)null);
				goto IL_0182;
			}
		}
		VehicleWeaponsComponent otherWeapons = default(VehicleWeaponsComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref otherWeapons))
		{
			ClearOperatorSelections(otherWeapons, args.Buckle.Owner);
			RecalculateSelectedWeapon(vehicleUid, otherWeapons);
			((EntitySystem)this).Dirty(vehicleUid, (IComponent)(object)otherWeapons, (MetaDataComponent)null);
		}
		goto IL_0182;
		IL_0182:
		VehicleWeaponsComponent selectionWeapons = default(VehicleWeaponsComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref selectionWeapons))
		{
			RefreshOperatorSelectedWeapons(vehicleUid, selectionWeapons);
		}
		VehicleWeaponsComponent refreshedWeapons = default(VehicleWeaponsComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref refreshedWeapons))
		{
			UpdateWeaponsUiForAllOperators(vehicleUid, refreshedWeapons);
		}
		EyeComponent eye = default(EyeComponent);
		if (((EntitySystem)this).TryComp<EyeComponent>(args.Buckle.Owner, ref eye))
		{
			EntityUid? val = eye.Target;
			EntityUid owner = vehicleUid;
			if (val.HasValue && val.GetValueOrDefault() == owner)
			{
				_eye.SetTarget(args.Buckle.Owner, (EntityUid?)null, eye);
			}
		}
	}

	private void OnOperatorShutdown(Entity<VehicleWeaponsOperatorComponent> ent, ref ComponentShutdown args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			ClearHardpointActions(ent.Owner, ent.Comp);
		}
	}

	private void OnOperatorShotAttempted(Entity<VehicleWeaponsOperatorComponent> ent, ref ShotAttemptedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || args.User != ent.Owner)
		{
			return;
		}
		EntityUid? vehicle = ent.Comp.Vehicle;
		if (!vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicle2 = vehicle.GetValueOrDefault();
		HardpointIntegrityComponent frameIntegrity = default(HardpointIntegrityComponent);
		if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(vehicle2, ref frameIntegrity) && frameIntegrity.Integrity <= 0f)
		{
			args.Cancel();
			_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-hull-destroyed"), ent.Owner, ent.Owner, PopupType.SmallCaution);
		}
		else
		{
			VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
			ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
			if (!((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicle2, ref weapons) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicle2, ref itemSlots) || !CanUseHardpointActions(ent.Owner) || !weapons.OperatorSelections.TryGetValue(ent.Owner, out var selectedWeapon) || selectedWeapon != args.Used.Owner)
			{
				return;
			}
			TimeSpan remaining = args.Used.Comp.NextFire - _timing.CurTime;
			if (remaining <= TimeSpan.Zero || _timing.CurTime < ent.Comp.NextCooldownFeedbackAt)
			{
				return;
			}
			ent.Comp.NextCooldownFeedbackAt = _timing.CurTime + TimeSpan.FromSeconds(0.25);
			BuckleComponent buckle = default(BuckleComponent);
			if (!((EntitySystem)this).TryComp<BuckleComponent>(ent.Owner, ref buckle))
			{
				return;
			}
			vehicle = buckle.BuckledTo;
			if (vehicle.HasValue)
			{
				EntityUid seat = vehicle.GetValueOrDefault();
				if (((EntitySystem)this).HasComp<VehicleWeaponsSeatComponent>(seat))
				{
					_ui.ServerSendUiMessage(Entity<UserInterfaceComponent>.op_Implicit(seat), (Enum)VehicleWeaponsUiKey.Key, (BoundUserInterfaceMessage)(object)new VehicleWeaponsCooldownFeedbackMessage((float)remaining.TotalSeconds), ent.Owner);
					_audio.PlayPredicted(args.Used.Comp.SoundEmpty, args.Used.Owner, (EntityUid?)ent.Owner, (AudioParams?)null);
				}
			}
		}
	}

	private bool TrySelectHardpoint(EntityUid seat, EntityUid actor, EntityUid? mountedWeapon, bool fromUi)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return false;
		}
		if (!_vehicleSystem.TryGetVehicleFromInterior(seat, out var vehicle) || !vehicle.HasValue)
		{
			return false;
		}
		EntityUid vehicleUid = vehicle.Value;
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (!((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref weapons))
		{
			return false;
		}
		BuckleComponent buckle = default(BuckleComponent);
		if (((EntitySystem)this).TryComp<BuckleComponent>(actor, ref buckle))
		{
			EntityUid? buckledTo = buckle.BuckledTo;
			VehicleWeaponsSeatComponent seatComp = default(VehicleWeaponsSeatComponent);
			if (buckledTo.HasValue && !(buckledTo.GetValueOrDefault() != seat) && ((EntitySystem)this).TryComp<VehicleWeaponsSeatComponent>(seat, ref seatComp))
			{
				if (fromUi && !seatComp.AllowUiSelection)
				{
					return false;
				}
				if (!fromUi && !seatComp.AllowHotbarSelection)
				{
					return false;
				}
				VehiclePortGunOperatorComponent portGunOperator = default(VehiclePortGunOperatorComponent);
				if (((EntitySystem)this).TryComp<VehiclePortGunOperatorComponent>(actor, ref portGunOperator) && portGunOperator.Gun.HasValue)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-vehicle-portgun-active"), seat, actor);
					return true;
				}
				HardpointSlotsComponent hardpoints = default(HardpointSlotsComponent);
				ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
				if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(vehicleUid, ref hardpoints) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicleUid, ref itemSlots))
				{
					return false;
				}
				VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
				if (!((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(actor, ref operatorComp))
				{
					return false;
				}
				if (!mountedWeapon.HasValue)
				{
					ClearOperatorSelections(weapons, actor);
					RecalculateSelectedWeapon(vehicleUid, weapons, itemSlots);
					RefreshOperatorSelectedWeapons(vehicleUid, weapons, itemSlots);
					((EntitySystem)this).Dirty(vehicleUid, (IComponent)(object)weapons, (MetaDataComponent)null);
					UpdateHardpointActionStates(actor, weapons, operatorComp);
					UpdateWeaponsUiForAllOperators(vehicleUid, weapons, hardpoints, itemSlots);
					return true;
				}
				if (!((EntitySystem)this).Exists(mountedWeapon.Value) || !_topology.TryGetMountedSlotByItem(vehicleUid, mountedWeapon.Value, out var _, hardpoints, itemSlots) || !((EntitySystem)this).HasComp<GunComponent>(mountedWeapon.Value) || !((EntitySystem)this).HasComp<VehicleTurretComponent>(mountedWeapon.Value) || !TryGetMountedWeaponHardpointType(vehicleUid, mountedWeapon.Value, out string hardpointType, hardpoints, itemSlots) || !IsHardpointTypeAllowed(seatComp, hardpointType))
				{
					return false;
				}
				bool sharedSelection = IsSharedHardpointType(hardpointType);
				if (!sharedSelection && weapons.HardpointOperators.TryGetValue(mountedWeapon.Value, out var currentOperator) && currentOperator != actor)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-vehicle-weapons-ui-hardpoint-in-use", (ValueTuple<string, object>)("operator", currentOperator)), seat, actor);
					UpdateWeaponsUiForAllOperators(vehicleUid, weapons, hardpoints, itemSlots);
					return true;
				}
				EntityUid priorWeapon;
				bool playSelectSound = !weapons.OperatorSelections.TryGetValue(actor, out priorWeapon) || priorWeapon != mountedWeapon.Value;
				if (weapons.OperatorSelections.TryGetValue(actor, out var existingWeapon) && existingWeapon == mountedWeapon.Value)
				{
					weapons.OperatorSelections.Remove(actor);
					if (!sharedSelection && weapons.HardpointOperators.TryGetValue(mountedWeapon.Value, out var existingOperator) && existingOperator == actor)
					{
						weapons.HardpointOperators.Remove(mountedWeapon.Value);
					}
				}
				else
				{
					if (weapons.OperatorSelections.TryGetValue(actor, out var previousWeapon) && weapons.HardpointOperators.TryGetValue(previousWeapon, out var existingOperator2) && existingOperator2 == actor)
					{
						weapons.HardpointOperators.Remove(previousWeapon);
					}
					weapons.OperatorSelections[actor] = mountedWeapon.Value;
					if (!sharedSelection)
					{
						weapons.HardpointOperators[mountedWeapon.Value] = actor;
					}
					GunSpinupComponent spinup = default(GunSpinupComponent);
					if (playSelectSound && ((EntitySystem)this).TryComp<GunSpinupComponent>(mountedWeapon.Value, ref spinup) && spinup.SelectSound != null)
					{
						_audio.PlayPredicted(spinup.SelectSound, mountedWeapon.Value, (EntityUid?)actor, (AudioParams?)null);
					}
				}
				RecalculateSelectedWeapon(vehicleUid, weapons, itemSlots);
				RefreshOperatorSelectedWeapons(vehicleUid, weapons, itemSlots);
				((EntitySystem)this).Dirty(vehicleUid, (IComponent)(object)weapons, (MetaDataComponent)null);
				UpdateHardpointActionStates(actor, weapons, operatorComp);
				UpdateWeaponsUiForAllOperators(vehicleUid, weapons, hardpoints, itemSlots);
				return true;
			}
		}
		return false;
	}

	private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<VehicleWeaponsComponent>(args.Vehicle, ref weapons))
		{
			return;
		}
		HardpointSlotsComponent hardpoints = null;
		ItemSlotsComponent itemSlots = null;
		EntityUid? selectedWeapon = weapons.SelectedWeapon;
		if (selectedWeapon.HasValue)
		{
			EntityUid selected = selectedWeapon.GetValueOrDefault();
			if (((EntitySystem)this).Resolve<HardpointSlotsComponent>(args.Vehicle, ref hardpoints, false) && ((EntitySystem)this).Resolve<ItemSlotsComponent>(args.Vehicle, ref itemSlots, false) && !IsSelectedWeaponInstalled(args.Vehicle, selected, hardpoints, itemSlots))
			{
				weapons.SelectedWeapon = null;
				((EntitySystem)this).Dirty(args.Vehicle, (IComponent)(object)weapons, (MetaDataComponent)null);
			}
		}
		PruneHardpointOperators(args.Vehicle, weapons, hardpoints, itemSlots);
		RecalculateSelectedWeapon(args.Vehicle, weapons, itemSlots);
		RefreshOperatorSelectedWeapons(args.Vehicle, weapons, itemSlots);
		RefreshSeatGunnerViews(args.Vehicle);
		((EntitySystem)this).Dirty(args.Vehicle, (IComponent)(object)weapons, (MetaDataComponent)null);
		UpdateWeaponsUiForAllOperators(args.Vehicle, weapons, hardpoints, itemSlots, refreshActions: true);
	}

	private void RefreshSeatGunnerViews(EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<VehicleWeaponsOperatorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
		EntityUid user = default(EntityUid);
		VehicleWeaponsOperatorComponent op = default(VehicleWeaponsOperatorComponent);
		while (query.MoveNext(ref user, ref op))
		{
			EntityUid? vehicle2 = op.Vehicle;
			EntityUid seat = vehicle;
			if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != seat) && TryGetUserWeaponsSeat(user, out seat, out VehicleWeaponsSeatComponent seatComp))
			{
				UpdateGunnerView(user, vehicle, seatComp);
			}
		}
	}

	private void UpdateGunnerView(EntityUid user, EntityUid vehicle, VehicleWeaponsSeatComponent? seatComp = null, bool removeOnly = false)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		if (seatComp == null)
		{
			seatComp = ((EntitySystem)this).CompOrNull<VehicleWeaponsSeatComponent>(((EntitySystem)this).Transform(user).ParentUid);
		}
		if (removeOnly)
		{
			if (((EntitySystem)this).RemCompDeferred<VehicleGunnerViewUserComponent>(user))
			{
				_eyeSystem.UpdatePvsScale(user);
			}
			return;
		}
		bool hasView = false;
		float pvsScale = 0f;
		float cursorMaxOffset = 0f;
		float cursorOffsetSpeed = 0.5f;
		float cursorPvsIncrease = 0f;
		if (seatComp != null && HasBaseGunnerView(seatComp))
		{
			pvsScale = Math.Max(pvsScale, seatComp.BaseViewPvsScale);
			cursorMaxOffset = Math.Max(cursorMaxOffset, seatComp.BaseViewCursorMaxOffset);
			cursorOffsetSpeed = MathF.Max(cursorOffsetSpeed, seatComp.BaseViewCursorOffsetSpeed);
			cursorPvsIncrease = Math.Max(cursorPvsIncrease, seatComp.BaseViewCursorPvsIncrease);
			hasView = true;
		}
		VehicleGunnerViewComponent gunnerView = default(VehicleGunnerViewComponent);
		if (seatComp != null && (seatComp.IsPrimaryOperatorSeat || HasBaseGunnerView(seatComp)) && ((EntitySystem)this).TryComp<VehicleGunnerViewComponent>(vehicle, ref gunnerView) && gunnerView.PvsScale > 0f)
		{
			pvsScale = Math.Max(pvsScale, gunnerView.PvsScale);
			cursorMaxOffset = Math.Max(cursorMaxOffset, gunnerView.CursorMaxOffset);
			cursorOffsetSpeed = MathF.Max(cursorOffsetSpeed, gunnerView.CursorOffsetSpeed);
			cursorPvsIncrease = Math.Max(cursorPvsIncrease, gunnerView.CursorPvsIncrease);
			hasView = true;
		}
		if (hasView && pvsScale > 0f)
		{
			VehicleGunnerViewUserComponent view = ((EntitySystem)this).EnsureComp<VehicleGunnerViewUserComponent>(user);
			view.PvsScale = pvsScale;
			view.CursorMaxOffset = cursorMaxOffset;
			view.CursorOffsetSpeed = cursorOffsetSpeed;
			view.CursorPvsIncrease = cursorPvsIncrease;
			((EntitySystem)this).Dirty(user, (IComponent)(object)view, (MetaDataComponent)null);
			_eyeSystem.UpdatePvsScale(user);
		}
		else if (((EntitySystem)this).RemCompDeferred<VehicleGunnerViewUserComponent>(user))
		{
			_eyeSystem.UpdatePvsScale(user);
		}
	}

	private static bool HasBaseGunnerView(VehicleWeaponsSeatComponent seatComp)
	{
		if (!(seatComp.BaseViewPvsScale > 0f) && !(seatComp.BaseViewCursorMaxOffset > 0f))
		{
			return seatComp.BaseViewCursorPvsIncrease > 0f;
		}
		return true;
	}

	private bool IsSelectedWeaponInstalled(EntityUid vehicle, EntityUid selected, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		foreach (VehicleMountedSlot mountedSlot2 in _topology.GetMountedSlots(vehicle, hardpoints, itemSlots))
		{
			EntityUid? item = mountedSlot2.Item;
			if (item.HasValue && item.GetValueOrDefault() == selected)
			{
				return true;
			}
		}
		return false;
	}

	private void OnTurretGunShot(Entity<VehicleTurretComponent> ent, ref GunShotEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (!_net.IsClient && TryGetContainingVehicle(ent.Owner, out var vehicle) && ((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicle, ref weapons))
		{
			UpdateWeaponsUiForAllOperators(vehicle, weapons);
		}
	}

	private bool TryGetContainingVehicle(EntityUid owner, out EntityUid vehicle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _topology.TryGetVehicle(owner, out vehicle);
	}

	private void ClearOperatorSelections(VehicleWeaponsComponent weapons, EntityUid operatorUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		weapons.OperatorSelections.Remove(operatorUid);
		KeyValuePair<EntityUid, EntityUid>[] array = weapons.HardpointOperators.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<EntityUid, EntityUid> pair = array[i];
			if (pair.Value == operatorUid)
			{
				weapons.HardpointOperators.Remove(pair.Key);
			}
		}
	}

	private void PruneHardpointOperators(EntityUid vehicle, VehicleWeaponsComponent weapons, HardpointSlotsComponent? hardpoints, ItemSlotsComponent? itemSlots)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HardpointSlotsComponent>(vehicle, ref hardpoints, false))
		{
			return;
		}
		KeyValuePair<EntityUid, EntityUid>[] array = weapons.HardpointOperators.ToArray();
		VehicleMountedSlot mountedSlot;
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<EntityUid, EntityUid> entry = array[i];
			if (!((EntitySystem)this).Exists(entry.Key) || !((EntitySystem)this).Exists(entry.Value) || !_topology.TryGetMountedSlotByItem(vehicle, entry.Key, out mountedSlot, hardpoints, itemSlots))
			{
				weapons.HardpointOperators.Remove(entry.Key);
			}
		}
		array = weapons.OperatorSelections.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<EntityUid, EntityUid> entry2 = array[i];
			if (!((EntitySystem)this).Exists(entry2.Key) || !((EntitySystem)this).Exists(entry2.Value) || !_topology.TryGetMountedSlotByItem(vehicle, entry2.Value, out mountedSlot, hardpoints, itemSlots))
			{
				weapons.OperatorSelections.Remove(entry2.Key);
			}
		}
	}

	private bool TryGetUserWeaponsSeat(EntityUid user, out EntityUid seat, out VehicleWeaponsSeatComponent seatComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		seat = default(EntityUid);
		seatComp = null;
		BuckleComponent buckle = default(BuckleComponent);
		if (((EntitySystem)this).TryComp<BuckleComponent>(user, ref buckle))
		{
			EntityUid? buckledTo = buckle.BuckledTo;
			if (buckledTo.HasValue)
			{
				EntityUid buckledSeat = buckledTo.GetValueOrDefault();
				VehicleWeaponsSeatComponent resolvedSeatComp = default(VehicleWeaponsSeatComponent);
				if (((EntitySystem)this).TryComp<VehicleWeaponsSeatComponent>(buckledSeat, ref resolvedSeatComp))
				{
					seatComp = resolvedSeatComp;
					seat = buckledSeat;
					return true;
				}
			}
		}
		return false;
	}

	private bool TryGetMountedWeaponHardpointType(EntityUid vehicle, EntityUid mountedWeapon, out string hardpointType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TryGetMountedWeaponHardpointType(vehicle, mountedWeapon, out hardpointType, null, null);
	}

	private bool TryGetMountedWeaponHardpointType(EntityUid vehicle, EntityUid mountedWeapon, out string hardpointType, HardpointSlotsComponent? hardpoints, ItemSlotsComponent? itemSlots)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		hardpointType = string.Empty;
		if (!_topology.TryGetMountedSlotByItem(vehicle, mountedWeapon, out var mountedSlot, hardpoints, itemSlots))
		{
			return false;
		}
		hardpointType = mountedSlot.HardpointType;
		return true;
	}

	private bool IsHardpointTypeAllowed(VehicleWeaponsSeatComponent seatComp, string hardpointType)
	{
		if (seatComp.AllowedHardpointTypes.Count == 0)
		{
			return true;
		}
		foreach (string allowedHardpointType in seatComp.AllowedHardpointTypes)
		{
			if (string.Equals(allowedHardpointType, hardpointType, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsSharedHardpointType(string hardpointType)
	{
		return string.Equals(hardpointType, "Support", StringComparison.OrdinalIgnoreCase);
	}

	private void RefreshOperatorSelectedWeapons(EntityUid vehicle, VehicleWeaponsComponent weapons, ItemSlotsComponent? itemSlots = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<VehicleWeaponsOperatorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
		EntityUid operatorUid = default(EntityUid);
		VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
		while (query.MoveNext(ref operatorUid, ref operatorComp))
		{
			EntityUid? vehicle2 = operatorComp.Vehicle;
			if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != vehicle))
			{
				EntityUid? selectedWeapon = null;
				if (weapons.OperatorSelections.TryGetValue(operatorUid, out var operatorSelectedWeapon) && IsSelectableMountedWeapon(vehicle, operatorSelectedWeapon, null, itemSlots))
				{
					selectedWeapon = operatorSelectedWeapon;
				}
				vehicle2 = operatorComp.SelectedWeapon;
				EntityUid? val = selectedWeapon;
				if (vehicle2.HasValue != val.HasValue || (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() == val.GetValueOrDefault())))
				{
					operatorComp.SelectedWeapon = selectedWeapon;
					((EntitySystem)this).Dirty(operatorUid, (IComponent)(object)operatorComp, (MetaDataComponent)null);
				}
			}
		}
	}

	public bool TryGetSelectedWeaponForOperator(EntityUid vehicle, EntityUid operatorUid, out EntityUid weapon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		weapon = default(EntityUid);
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (!((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicle, ref weapons))
		{
			return false;
		}
		if (weapons.OperatorSelections.TryGetValue(operatorUid, out var selectedWeapon) && IsSelectableMountedWeapon(vehicle, selectedWeapon))
		{
			weapon = selectedWeapon;
			return true;
		}
		VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
		EntityUid val;
		EntityUid? vehicle2;
		if (((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(operatorUid, ref operatorComp))
		{
			vehicle2 = operatorComp.Vehicle;
			val = vehicle;
			if (vehicle2.HasValue && vehicle2.GetValueOrDefault() == val)
			{
				vehicle2 = operatorComp.SelectedWeapon;
				if (vehicle2.HasValue)
				{
					EntityUid operatorWeapon = vehicle2.GetValueOrDefault();
					if (((EntitySystem)this).Exists(operatorWeapon) && ((EntitySystem)this).HasComp<GunComponent>(operatorWeapon))
					{
						weapon = operatorWeapon;
						return true;
					}
				}
			}
		}
		vehicle2 = weapons.Operator;
		val = operatorUid;
		if (vehicle2.HasValue && vehicle2.GetValueOrDefault() == val)
		{
			vehicle2 = weapons.SelectedWeapon;
			if (vehicle2.HasValue)
			{
				EntityUid primaryWeapon = vehicle2.GetValueOrDefault();
				if (((EntitySystem)this).Exists(primaryWeapon) && ((EntitySystem)this).HasComp<GunComponent>(primaryWeapon))
				{
					weapon = primaryWeapon;
					return true;
				}
			}
		}
		return false;
	}

	public bool TryGetOperatorForSelectedWeapon(EntityUid vehicle, EntityUid weapon, out EntityUid operatorUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		operatorUid = default(EntityUid);
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (!((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicle, ref weapons))
		{
			return false;
		}
		foreach (KeyValuePair<EntityUid, EntityUid> entry in weapons.OperatorSelections)
		{
			if (((EntitySystem)this).Exists(entry.Key) && !(entry.Value != weapon) && IsSelectableMountedWeapon(vehicle, entry.Value))
			{
				operatorUid = entry.Key;
				return true;
			}
		}
		EntityQueryEnumerator<VehicleWeaponsOperatorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
		EntityUid candidateUid = default(EntityUid);
		VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
		while (query.MoveNext(ref candidateUid, ref operatorComp))
		{
			EntityUid? vehicle2 = operatorComp.Vehicle;
			EntityUid val = vehicle;
			if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != val))
			{
				vehicle2 = operatorComp.SelectedWeapon;
				val = weapon;
				if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != val))
				{
					operatorUid = candidateUid;
					return true;
				}
			}
		}
		return false;
	}

	private void RecalculateSelectedWeapon(EntityUid vehicle, VehicleWeaponsComponent weapons, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = weapons.Operator;
		if (val.HasValue)
		{
			EntityUid primaryOperator = val.GetValueOrDefault();
			if (weapons.OperatorSelections.TryGetValue(primaryOperator, out var selectedWeapon))
			{
				if (!IsSelectableMountedWeapon(vehicle, selectedWeapon, null, itemSlots))
				{
					weapons.SelectedWeapon = null;
				}
				else
				{
					weapons.SelectedWeapon = selectedWeapon;
				}
				return;
			}
		}
		weapons.SelectedWeapon = null;
	}

	private bool IsSelectableMountedWeapon(EntityUid vehicle, EntityUid mountedWeapon, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		VehicleMountedSlot mountedSlot;
		if (((EntitySystem)this).Exists(mountedWeapon) && ((EntitySystem)this).HasComp<VehicleTurretComponent>(mountedWeapon) && ((EntitySystem)this).HasComp<GunComponent>(mountedWeapon))
		{
			return _topology.TryGetMountedSlotByItem(vehicle, mountedWeapon, out mountedSlot, hardpoints, itemSlots);
		}
		return false;
	}

	private void OnWeaponsUiOpened(Entity<VehicleWeaponsSeatComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, VehicleWeaponsUiKey.Key) || !_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) || !vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicleUid = vehicle.Value;
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref weapons))
		{
			EntityUid viewer = ((BaseBoundUserInterfaceEvent)args).Actor;
			if (!(viewer == default(EntityUid)) && ((EntitySystem)this).Exists(viewer))
			{
				UpdateWeaponsUi(ent.Owner, vehicleUid, weapons, null, null, viewer);
			}
		}
	}

	private void OnWeaponsUiClosed(Entity<VehicleWeaponsSeatComponent> ent, ref BoundUIClosedEvent args)
	{
		object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, VehicleWeaponsUiKey.Key);
	}

	private void OnWeaponsSelect(Entity<VehicleWeaponsSeatComponent> ent, ref VehicleWeaponsSelectMessage args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, VehicleWeaponsUiKey.Key) && !(((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid)) && ((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor) && CanUseHardpointActions(((BaseBoundUserInterfaceEvent)args).Actor, forUi: true))
		{
			EntityUid? mountedWeapon = (args.MountedEntity.HasValue ? new EntityUid?(((EntitySystem)this).GetEntity(args.MountedEntity.Value)) : ((EntityUid?)null));
			TrySelectHardpoint(ent.Owner, ((BaseBoundUserInterfaceEvent)args).Actor, mountedWeapon, fromUi: true);
		}
	}

	private void OnWeaponsStabilization(Entity<VehicleWeaponsSeatComponent> ent, ref VehicleWeaponsStabilizationMessage args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		if (!object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, VehicleWeaponsUiKey.Key) || ((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid) || !((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor) || !_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) || !vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicleUid = vehicle.Value;
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (!((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref weapons))
		{
			return;
		}
		EntityUid? val = weapons.Operator;
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		BuckleComponent buckle = default(BuckleComponent);
		if (!val.HasValue || val.GetValueOrDefault() != actor || !((EntitySystem)this).TryComp<BuckleComponent>(((BaseBoundUserInterfaceEvent)args).Actor, ref buckle))
		{
			return;
		}
		val = buckle.BuckledTo;
		actor = ent.Owner;
		if (!val.HasValue || val.GetValueOrDefault() != actor)
		{
			return;
		}
		HardpointSlotsComponent hardpoints = null;
		ItemSlotsComponent itemSlots = null;
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (((EntitySystem)this).Resolve<HardpointSlotsComponent>(vehicleUid, ref hardpoints, false) && ((EntitySystem)this).Resolve<ItemSlotsComponent>(vehicleUid, ref itemSlots, false) && weapons.OperatorSelections.TryGetValue(((BaseBoundUserInterfaceEvent)args).Actor, out var selectedWeapon) && ((EntitySystem)this).Exists(selectedWeapon) && ((EntitySystem)this).TryComp<VehicleTurretComponent>(selectedWeapon, ref turret) && _turretSystem.TryResolveRotationTarget(selectedWeapon, out EntityUid targetUid, out VehicleTurretComponent targetTurret) && targetTurret.RotateToCursor)
		{
			targetTurret.StabilizedRotation = args.Enabled;
			Angle vehicleRot = _transform.GetWorldRotation(vehicleUid);
			Angle val2 = targetTurret.WorldRotation + vehicleRot;
			Angle currentWorld = ((Angle)(ref val2)).Reduced();
			if (args.Enabled)
			{
				targetTurret.TargetRotation = currentWorld;
			}
			else
			{
				targetTurret.TargetRotation = targetTurret.WorldRotation;
			}
			((EntitySystem)this).Dirty(targetUid, (IComponent)(object)targetTurret, (MetaDataComponent)null);
			UpdateWeaponsUiForAllOperators(vehicleUid, weapons, hardpoints, itemSlots);
		}
	}

	private void OnWeaponsAutoMode(Entity<VehicleWeaponsSeatComponent> ent, ref VehicleWeaponsAutoModeMessage args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (!object.Equals(((BaseBoundUserInterfaceEvent)args).UiKey, VehicleWeaponsUiKey.Key) || ((BaseBoundUserInterfaceEvent)args).Actor == default(EntityUid) || !((EntitySystem)this).Exists(((BaseBoundUserInterfaceEvent)args).Actor) || !_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) || !vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicleUid = vehicle.Value;
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		if (!((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicleUid, ref weapons))
		{
			return;
		}
		EntityUid? val = weapons.Operator;
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		BuckleComponent buckle = default(BuckleComponent);
		if (val.HasValue && !(val.GetValueOrDefault() != actor) && ((EntitySystem)this).TryComp<BuckleComponent>(((BaseBoundUserInterfaceEvent)args).Actor, ref buckle))
		{
			val = buckle.BuckledTo;
			actor = ent.Owner;
			VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
			if (val.HasValue && !(val.GetValueOrDefault() != actor) && ((EntitySystem)this).TryComp<VehicleDeployableComponent>(vehicleUid, ref deployable))
			{
				deployable.AutoTurretEnabled = args.Enabled;
				((EntitySystem)this).Dirty(vehicleUid, (IComponent)(object)deployable, (MetaDataComponent)null);
				UpdateWeaponsUiForAllOperators(vehicleUid, weapons);
			}
		}
	}

	private void UpdateWeaponsUi(EntityUid seat, EntityUid vehicle, VehicleWeaponsComponent? weapons = null, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null, EntityUid? operatorUid = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !((EntitySystem)this).Resolve<VehicleWeaponsComponent>(vehicle, ref weapons, false) || !((EntitySystem)this).Resolve<HardpointSlotsComponent>(vehicle, ref hardpoints, false) || !((EntitySystem)this).Resolve<ItemSlotsComponent>(vehicle, ref itemSlots, false))
		{
			return;
		}
		if (!operatorUid.HasValue)
		{
			operatorUid = weapons.Operator;
		}
		VehicleWeaponsSeatComponent operatorSeatComp = null;
		EntityUid seat2;
		if (operatorUid.HasValue)
		{
			TryGetUserWeaponsSeat(operatorUid.Value, out seat2, out operatorSeatComp);
		}
		EntityUid? operatorSelection = null;
		if (operatorUid.HasValue && weapons.OperatorSelections.TryGetValue(operatorUid.Value, out var selectedWeapon))
		{
			operatorSelection = selectedWeapon;
		}
		if (!operatorSelection.HasValue && operatorUid.HasValue && operatorSeatComp != null && !operatorSeatComp.AllowUiSelection)
		{
			EntityUid? val = weapons.Operator;
			if (val.HasValue)
			{
				EntityUid primaryOperator = val.GetValueOrDefault();
				if (weapons.OperatorSelections.TryGetValue(primaryOperator, out var primarySelection))
				{
					operatorSelection = primarySelection;
				}
			}
		}
		List<VehicleMountedSlot> mountedSlots = _topology.GetMountedSlots(vehicle, hardpoints, itemSlots);
		List<VehicleWeaponsUiEntry> entries = new List<VehicleWeaponsUiEntry>(mountedSlots.Count);
		bool canUseHardpointActions = !operatorUid.HasValue || CanUseHardpointActions(operatorUid.Value, forUi: true);
		foreach (VehicleMountedSlot mountedSlot in mountedSlots)
		{
			entries.Add(CreateMountedSlotUiEntry(mountedSlot, weapons, operatorUid, operatorSelection, canUseHardpointActions, operatorSeatComp));
		}
		bool canToggleStabilization = false;
		bool stabilizationEnabled = false;
		bool canToggleAuto = false;
		bool autoEnabled = false;
		int num;
		if (operatorUid.HasValue)
		{
			EntityUid? val = weapons.Operator;
			EntityUid? val2 = operatorUid;
			if (val.HasValue != val2.HasValue)
			{
				num = 0;
			}
			else
			{
				if (!val.HasValue)
				{
					num = 1;
				}
				else
				{
					num = ((val.GetValueOrDefault() == val2.GetValueOrDefault()) ? 1 : 0);
					if (num == 0)
					{
						goto IL_0207;
					}
				}
				VehicleTurretComponent selectedTurret = default(VehicleTurretComponent);
				if (operatorSelection.HasValue && ((EntitySystem)this).Exists(operatorSelection.Value) && ((EntitySystem)this).TryComp<VehicleTurretComponent>(operatorSelection.Value, ref selectedTurret) && _turretSystem.TryResolveRotationTarget(operatorSelection.Value, out seat2, out VehicleTurretComponent targetTurret))
				{
					stabilizationEnabled = targetTurret.StabilizedRotation;
					canToggleStabilization = targetTurret.RotateToCursor;
				}
			}
		}
		else
		{
			num = 0;
		}
		goto IL_0207;
		IL_0207:
		VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
		if (num != 0 && ((EntitySystem)this).TryComp<VehicleDeployableComponent>(vehicle, ref deployable))
		{
			canToggleAuto = true;
			autoEnabled = deployable.AutoTurretEnabled;
		}
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(seat), (Enum)VehicleWeaponsUiKey.Key, (BoundUserInterfaceState)(object)new VehicleWeaponsUiState(((EntitySystem)this).GetNetEntity(vehicle, (MetaDataComponent)null), entries, canToggleStabilization, stabilizationEnabled, canToggleAuto, autoEnabled));
	}

	private VehicleWeaponsUiEntry CreateMountedSlotUiEntry(VehicleMountedSlot mountedSlot, VehicleWeaponsComponent weapons, EntityUid? operatorUid, EntityUid? operatorSelection, bool canUseHardpointActions, VehicleWeaponsSeatComponent? operatorSeatComp)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		bool slotAllowed = operatorSeatComp == null || IsHardpointTypeAllowed(operatorSeatComp, mountedSlot.HardpointType);
		bool sharedSelection = IsSharedHardpointType(mountedSlot.HardpointType);
		bool hasItem = mountedSlot.Item.HasValue;
		EntityUid? item = mountedSlot.Item;
		string installedName = null;
		NetEntity? installedEntity = null;
		if (item.HasValue)
		{
			installedName = ((EntitySystem)this).Name(item.Value, (MetaDataComponent)null);
			installedEntity = ((EntitySystem)this).GetNetEntity(item.Value, (MetaDataComponent)null);
		}
		string operatorName = null;
		bool operatorIsSelf = false;
		EntityUid slotOperator = default(EntityUid);
		bool hasOperator = item.HasValue && weapons.HardpointOperators.TryGetValue(item.Value, out slotOperator);
		if (hasOperator)
		{
			operatorName = ((EntitySystem)this).Name(slotOperator, (MetaDataComponent)null);
			operatorIsSelf = operatorUid.HasValue && slotOperator == operatorUid.Value;
		}
		bool selectable = canUseHardpointActions && slotAllowed && item.HasValue && ((EntitySystem)this).HasComp<VehicleTurretComponent>(item.Value);
		if (selectable && hasOperator && !operatorIsSelf && !sharedSelection)
		{
			selectable = false;
		}
		bool selected = item.HasValue && operatorSelection.HasValue && operatorSelection.Value == item.Value;
		int ammoCount = 0;
		int ammoCapacity = 0;
		bool hasAmmo = false;
		float cooldownRemaining = 0f;
		float cooldownTotal = 0f;
		bool isOnCooldown = false;
		GunComponent gun = default(GunComponent);
		if (item.HasValue && ((EntitySystem)this).TryComp<GunComponent>(item.Value, ref gun))
		{
			GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(item.Value, ref ammoEv, false);
			ammoCount = ammoEv.Count;
			ammoCapacity = ammoEv.Capacity;
			hasAmmo = ammoEv.Capacity > 0;
			if (gun.FireRateModified > 0f)
			{
				cooldownTotal = 1f / gun.FireRateModified;
			}
			TimeSpan remaining = gun.NextFire - _timing.CurTime;
			if (remaining > TimeSpan.Zero)
			{
				cooldownRemaining = (float)remaining.TotalSeconds;
				isOnCooldown = cooldownRemaining > 0.001f;
			}
		}
		int magazineSize = 0;
		int storedMagazines = 0;
		int maxStoredMagazines = 0;
		bool hasMagazineData = false;
		float integrity = 0f;
		float maxIntegrity = 0f;
		bool hasIntegrity = false;
		VehicleHardpointAmmoComponent hardpointAmmo = default(VehicleHardpointAmmoComponent);
		RMCVehicleHardpointAmmoComponent rmcAmmo = default(RMCVehicleHardpointAmmoComponent);
		if (item.HasValue && ((EntitySystem)this).TryComp<VehicleHardpointAmmoComponent>(item.Value, ref hardpointAmmo))
		{
			magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
			BallisticAmmoProviderComponent ammoProvider = default(BallisticAmmoProviderComponent);
			if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(item.Value, ref ammoProvider))
			{
				magazineSize = _hardpointAmmo.GetMagazineSize(hardpointAmmo, ammoProvider);
			}
			storedMagazines = _hardpointAmmo.GetStoredRounds(hardpointAmmo, magazineSize);
			maxStoredMagazines = _hardpointAmmo.GetMaxStoredRounds(hardpointAmmo, magazineSize);
			hasMagazineData = hardpointAmmo.MagazineSize > 0 || hardpointAmmo.MaxStoredMagazines > 0;
		}
		else if (item.HasValue && ((EntitySystem)this).TryComp<RMCVehicleHardpointAmmoComponent>(item.Value, ref rmcAmmo))
		{
			magazineSize = Math.Max(1, rmcAmmo.MagazineSize);
			BallisticAmmoProviderComponent rmcProvider = default(BallisticAmmoProviderComponent);
			if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(item.Value, ref rmcProvider))
			{
				magazineSize = Math.Min(magazineSize, Math.Max(1, rmcProvider.Capacity));
			}
			storedMagazines = rmcAmmo.StoredMagazines * magazineSize;
			maxStoredMagazines = rmcAmmo.MaxStoredMagazines * magazineSize;
			hasMagazineData = rmcAmmo.MagazineSize > 0 || rmcAmmo.MaxStoredMagazines > 0;
		}
		HardpointIntegrityComponent hardpointIntegrity = default(HardpointIntegrityComponent);
		if (item.HasValue && ((EntitySystem)this).TryComp<HardpointIntegrityComponent>(item.Value, ref hardpointIntegrity))
		{
			integrity = hardpointIntegrity.Integrity;
			maxIntegrity = hardpointIntegrity.MaxIntegrity;
			hasIntegrity = true;
		}
		return new VehicleWeaponsUiEntry(mountedSlot.CompositeId, mountedSlot.HardpointType, installedEntity, installedName, installedEntity, hasItem, selectable, selected, ammoCount, ammoCapacity, hasAmmo, magazineSize, storedMagazines, maxStoredMagazines, hasMagazineData, operatorName, operatorIsSelf, integrity, maxIntegrity, hasIntegrity, cooldownRemaining, cooldownTotal, isOnCooldown);
	}

	private void UpdateWeaponsUiForAllOperators(EntityUid vehicle, VehicleWeaponsComponent weapons, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null, bool refreshActions = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<VehicleWeaponsOperatorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleWeaponsOperatorComponent>();
		EntityUid operatorUid = default(EntityUid);
		VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
		while (query.MoveNext(ref operatorUid, ref operatorComp))
		{
			EntityUid? vehicle2 = operatorComp.Vehicle;
			if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != vehicle) && TryGetUserWeaponsSeat(operatorUid, out EntityUid seat, out VehicleWeaponsSeatComponent _))
			{
				if (refreshActions)
				{
					RefreshHardpointActions(operatorUid, vehicle, weapons, operatorComp, hardpoints, itemSlots);
				}
				UpdateWeaponsUi(seat, vehicle, weapons, hardpoints, itemSlots, operatorUid);
			}
		}
	}
}
