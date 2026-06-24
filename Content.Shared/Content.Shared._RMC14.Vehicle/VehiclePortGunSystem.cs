using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Buckle.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehiclePortGunSystem : EntitySystem
{
	[Dependency]
	private readonly SharedEyeSystem _eye;

	[Dependency]
	private readonly SharedHandsSystem _hands;

	[Dependency]
	private readonly ItemSlotsSystem _itemSlots;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SkillsSystem _skills;

	[Dependency]
	private readonly VehicleSystem _vehicleSystem;

	[Dependency]
	private readonly VehicleViewToggleSystem _viewToggle;

	[Dependency]
	private readonly INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunSeatComponent, StrapAttemptEvent>((EntityEventRefHandler<VehiclePortGunSeatComponent, StrapAttemptEvent>)OnPortGunSeatStrapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunSeatComponent, UnstrappedEvent>((EntityEventRefHandler<VehiclePortGunSeatComponent, UnstrappedEvent>)OnPortGunSeatUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunControllerComponent, InteractHandEvent>((EntityEventRefHandler<VehiclePortGunControllerComponent, InteractHandEvent>)OnPortGunInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunControllerComponent, InteractUsingEvent>((EntityEventRefHandler<VehiclePortGunControllerComponent, InteractUsingEvent>)OnPortGunInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunControllerComponent, ExaminedEvent>((EntityEventRefHandler<VehiclePortGunControllerComponent, ExaminedEvent>)OnPortGunExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunControllerComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<VehiclePortGunControllerComponent, GetVerbsEvent<AlternativeVerb>>)OnPortGunVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunComponent, ComponentShutdown>((EntityEventRefHandler<VehiclePortGunComponent, ComponentShutdown>)OnPortGunShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunComponent, GunShotEvent>((EntityEventRefHandler<VehiclePortGunComponent, GunShotEvent>)OnPortGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<VehiclePortGunComponent, EntInsertedIntoContainerMessage>)OnPortGunContainerInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<VehiclePortGunComponent, EntRemovedFromContainerMessage>)OnPortGunContainerRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehiclePortGunOperatorComponent, ComponentShutdown>((EntityEventRefHandler<VehiclePortGunOperatorComponent, ComponentShutdown>)OnPortGunOperatorShutdown, (Type[])null, (Type[])null);
	}

	private void OnPortGunSeatStrapAttempt(Entity<VehiclePortGunSeatComponent> ent, ref StrapAttemptEvent args)
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

	private void OnPortGunSeatUnstrapped(Entity<VehiclePortGunSeatComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			ClearOperator(args.Buckle.Owner);
		}
	}

	private void OnPortGunInteractHand(Entity<VehiclePortGunControllerComponent> ent, ref InteractHandEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || _net.IsClient || !TryGetPortGun(ent, args.User, out EntityUid vehicle, out EntityUid gunUid, out VehiclePortGunComponent portGun))
		{
			return;
		}
		EntityUid? val;
		EntityUid user;
		if (portGun.Operator.HasValue)
		{
			val = portGun.Operator;
			user = args.User;
			if (!val.HasValue || val.GetValueOrDefault() != user)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-vehicle-portgun-in-use", (ValueTuple<string, object>)("operator", ((EntitySystem)this).Name(portGun.Operator.Value, (MetaDataComponent)null))), Entity<VehiclePortGunControllerComponent>.op_Implicit(ent), args.User);
				return;
			}
		}
		VehiclePortGunOperatorComponent existing = default(VehiclePortGunOperatorComponent);
		if (((EntitySystem)this).TryComp<VehiclePortGunOperatorComponent>(args.User, ref existing) && existing.Gun.HasValue)
		{
			val = existing.Gun;
			user = gunUid;
			if (!val.HasValue || val.GetValueOrDefault() != user)
			{
				ClearOperator(args.User, existing);
			}
		}
		val = portGun.Operator;
		user = args.User;
		if (val.HasValue && val.GetValueOrDefault() == user)
		{
			ClearOperator(args.User);
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		portGun.Operator = args.User;
		((EntitySystem)this).Dirty(gunUid, (IComponent)(object)portGun, (MetaDataComponent)null);
		VehiclePortGunOperatorComponent operatorComp = ((EntitySystem)this).EnsureComp<VehiclePortGunOperatorComponent>(args.User);
		operatorComp.Gun = gunUid;
		operatorComp.Vehicle = vehicle;
		operatorComp.Controller = ent.Owner;
		((EntitySystem)this).Dirty(args.User, (IComponent)(object)operatorComp, (MetaDataComponent)null);
		if (((EntitySystem)this).HasComp<VehicleEnterComponent>(vehicle))
		{
			_eye.SetTarget(args.User, (EntityUid?)vehicle, (EyeComponent)null);
			_viewToggle.EnableViewToggle(args.User, vehicle, ent.Owner, null, isOutside: true);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnPortGunInteractUsing(Entity<VehiclePortGunControllerComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent gunSlots = default(ItemSlotsComponent);
		if (!((HandledEntityEventArgs)args).Handled && !_net.IsClient && TryGetGunFromController(ent, out var gunUid) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(gunUid, ref gunSlots) && _itemSlots.TryGetSlot(gunUid, "gun_magazine", out ItemSlot magSlot, gunSlots) && (!magSlot.HasItem || _itemSlots.TryEjectToHands(gunUid, magSlot, args.User)) && _itemSlots.CanInsert(gunUid, args.Used, args.User, magSlot) && _hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), args.Used) && _itemSlots.TryInsert(gunUid, magSlot, args.Used, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnPortGunShutdown(Entity<VehiclePortGunComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Operator.HasValue)
		{
			ClearOperator(ent.Comp.Operator.Value);
		}
	}

	private void OnPortGunShot(Entity<VehiclePortGunComponent> ent, ref GunShotEvent args)
	{
		_ = _net.IsClient;
	}

	private void OnPortGunContainerInserted(Entity<VehiclePortGunComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		_ = _net.IsClient;
	}

	private void OnPortGunContainerRemoved(Entity<VehiclePortGunComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		_ = _net.IsClient;
	}

	private void OnPortGunOperatorShutdown(Entity<VehiclePortGunOperatorComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gun = ent.Comp.Gun;
		if (!gun.HasValue)
		{
			return;
		}
		EntityUid gun2 = gun.GetValueOrDefault();
		VehiclePortGunComponent portGun = default(VehiclePortGunComponent);
		if (((EntitySystem)this).TryComp<VehiclePortGunComponent>(gun2, ref portGun))
		{
			gun = portGun.Operator;
			EntityUid owner = ent.Owner;
			if (gun.HasValue && !(gun.GetValueOrDefault() != owner))
			{
				portGun.Operator = null;
				((EntitySystem)this).Dirty(gun2, (IComponent)(object)portGun, (MetaDataComponent)null);
			}
		}
	}

	private void OnPortGunExamined(Entity<VehiclePortGunControllerComponent> ent, ref ExaminedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange && TryGetGunFromController(ent, out var gunUid))
		{
			GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(gunUid, ref ammoEv, false);
			if (ammoEv.Capacity > 0)
			{
				args.PushMarkup(base.Loc.GetString("rmc-vehicle-portgun-examine-ammo", (ValueTuple<string, object>)("current", ammoEv.Count), (ValueTuple<string, object>)("max", ammoEv.Capacity)));
			}
		}
	}

	private void OnPortGunVerbs(Entity<VehiclePortGunControllerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent gunSlots = default(ItemSlotsComponent);
		if (args.CanAccess && args.CanInteract && args.Hands != null && TryGetGunFromController(ent, out var gunUid) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(gunUid, ref gunSlots) && _itemSlots.TryGetSlot(gunUid, "gun_magazine", out ItemSlot magSlot, gunSlots) && magSlot.HasItem)
		{
			EntityUid user = args.User;
			EntityUid gun = gunUid;
			ItemSlot slot = magSlot;
			AlternativeVerb ejectVerb = new AlternativeVerb
			{
				Text = base.Loc.GetString("rmc-vehicle-portgun-eject"),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					_itemSlots.TryEjectToHands(gun, slot, user, excludeUserAudio: true);
				},
				Priority = 2
			};
			args.Verbs.Add(ejectVerb);
		}
	}

	private bool TryGetPortGun(Entity<VehiclePortGunControllerComponent> ent, EntityUid user, out EntityUid vehicle, out EntityUid gunUid, out VehiclePortGunComponent portGun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		vehicle = default(EntityUid);
		gunUid = default(EntityUid);
		portGun = null;
		if (!TryGetPortGunSeat(user))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-portgun-need-seat"), Entity<VehiclePortGunControllerComponent>.op_Implicit(ent), user);
			return false;
		}
		if (!_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicleUid) || !vehicleUid.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-portgun-no-vehicle"), Entity<VehiclePortGunControllerComponent>.op_Implicit(ent), user);
			return false;
		}
		vehicle = vehicleUid.Value;
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicle, ref itemSlots) || !_itemSlots.TryGetSlot(vehicle, ent.Comp.GunSlotId, out ItemSlot slot, itemSlots) || !slot.HasItem || !slot.Item.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-portgun-no-gun"), Entity<VehiclePortGunControllerComponent>.op_Implicit(ent), user);
			return false;
		}
		gunUid = slot.Item.Value;
		VehiclePortGunComponent portGunComp = default(VehiclePortGunComponent);
		GunComponent gunComponent = default(GunComponent);
		if (!((EntitySystem)this).TryComp<VehiclePortGunComponent>(gunUid, ref portGunComp) || !((EntitySystem)this).TryComp<GunComponent>(gunUid, ref gunComponent))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vehicle-portgun-no-gun"), Entity<VehiclePortGunControllerComponent>.op_Implicit(ent), user);
			return false;
		}
		portGun = portGunComp;
		return true;
	}

	private bool TryGetPortGunSeat(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		if (!((EntitySystem)this).TryComp<BuckleComponent>(user, ref buckle) || !buckle.BuckledTo.HasValue)
		{
			return false;
		}
		return ((EntitySystem)this).HasComp<VehiclePortGunSeatComponent>(buckle.BuckledTo.Value);
	}

	private bool TryGetGunFromController(Entity<VehiclePortGunControllerComponent> ent, out EntityUid gunUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		gunUid = default(EntityUid);
		if (!_vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicleUid) || !vehicleUid.HasValue)
		{
			return false;
		}
		EntityUid vehicle = vehicleUid.Value;
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicle, ref itemSlots) || !_itemSlots.TryGetSlot(vehicle, ent.Comp.GunSlotId, out ItemSlot slot, itemSlots) || !slot.HasItem || !slot.Item.HasValue)
		{
			return false;
		}
		gunUid = slot.Item.Value;
		return true;
	}

	private void ClearOperator(EntityUid user, VehiclePortGunOperatorComponent? operatorComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VehiclePortGunOperatorComponent>(user, ref operatorComp, false))
		{
			return;
		}
		EntityUid? gun = operatorComp.Gun;
		if (gun.HasValue)
		{
			EntityUid gun2 = gun.GetValueOrDefault();
			VehiclePortGunComponent portGun = default(VehiclePortGunComponent);
			if (((EntitySystem)this).TryComp<VehiclePortGunComponent>(gun2, ref portGun))
			{
				gun = portGun.Operator;
				if (gun.HasValue && gun.GetValueOrDefault() == user)
				{
					portGun.Operator = null;
					((EntitySystem)this).Dirty(gun2, (IComponent)(object)portGun, (MetaDataComponent)null);
				}
			}
		}
		EntityUid? vehicle = operatorComp.Vehicle;
		((EntitySystem)this).RemCompDeferred<VehiclePortGunOperatorComponent>(user);
		if (operatorComp.Controller.HasValue)
		{
			_viewToggle.DisableViewToggle(user, operatorComp.Controller.Value);
		}
		EyeComponent eye = default(EyeComponent);
		if (!_net.IsClient && vehicle.HasValue && ((EntitySystem)this).TryComp<EyeComponent>(user, ref eye))
		{
			gun = eye.Target;
			EntityUid? val = vehicle;
			if (gun.HasValue == val.HasValue && (!gun.HasValue || gun.GetValueOrDefault() == val.GetValueOrDefault()))
			{
				_eye.SetTarget(user, (EntityUid?)null, eye);
			}
		}
	}
}
