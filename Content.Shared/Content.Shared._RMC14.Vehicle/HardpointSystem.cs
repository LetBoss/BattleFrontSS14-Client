using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Explosion.Components;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class HardpointSystem : EntitySystem
{
	private static readonly EntProtoId<SkillDefinitionComponent> EngineerSkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillEngineer");

	[Dependency]
	private readonly ItemSlotsSystem _itemSlots;

	[Dependency]
	private readonly Content.Shared.Vehicle.VehicleSystem _vehicles;

	[Dependency]
	private readonly SharedDoAfterSystem _doAfter;

	[Dependency]
	private readonly SharedToolSystem _tool;

	[Dependency]
	private readonly VehicleWheelSystem _wheels;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly RMCRepairableSystem _repairable;

	[Dependency]
	private readonly SharedAudioSystem _audio;

	[Dependency]
	private readonly SharedContainerSystem _containers;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SharedUserInterfaceSystem _ui;

	[Dependency]
	private readonly SharedHandsSystem _hands;

	[Dependency]
	private readonly SharedAppearanceSystem _appearance;

	[Dependency]
	private readonly EntityWhitelistSystem _whitelist;

	[Dependency]
	private readonly SharedGunSystem _guns;

	[Dependency]
	private readonly IPrototypeManager _prototypeManager;

	[Dependency]
	private readonly SharedExplosionSystem _explosion;

	[Dependency]
	private readonly VehicleTopologySystem _topology;

	[Dependency]
	private readonly SkillsSystem _skills;

	[Dependency]
	private readonly HardpointSlotSystem _hardpointSlots;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, ComponentInit>((EntityEventRefHandler<HardpointSlotsComponent, ComponentInit>)OnSlotsInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, MapInitEvent>((EntityEventRefHandler<HardpointSlotsComponent, MapInitEvent>)OnSlotsMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<HardpointSlotsComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<HardpointSlotsComponent, EntRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, VehicleCanRunEvent>((EntityEventRefHandler<HardpointSlotsComponent, VehicleCanRunEvent>)OnVehicleCanRun, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsComponent, DamageModifyEvent>((EntityEventRefHandler<HardpointSlotsComponent, DamageModifyEvent>)OnVehicleDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointIntegrityComponent, ComponentInit>((EntityEventRefHandler<HardpointIntegrityComponent, ComponentInit>)OnHardpointIntegrityInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointIntegrityComponent, InteractUsingEvent>((EntityEventRefHandler<HardpointIntegrityComponent, InteractUsingEvent>)OnHardpointRepair, new Type[1] { typeof(ItemSlotsSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointIntegrityComponent, ExaminedEvent>((EntityEventRefHandler<HardpointIntegrityComponent, ExaminedEvent>)OnHardpointExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointIntegrityComponent, HardpointRepairDoAfterEvent>((EntityEventRefHandler<HardpointIntegrityComponent, HardpointRepairDoAfterEvent>)OnHardpointRepairDoAfter, (Type[])null, (Type[])null);
	}

	private void OnSlotsInit(Entity<HardpointSlotsComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EnsureSlots(ent.Owner, ent.Comp);
	}

	private void OnSlotsMapInit(Entity<HardpointSlotsComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EnsureSlots(ent.Owner, ent.Comp);
		_hardpointSlots.DisableEjectForAllSlots(ent);
	}

	private void OnInserted(Entity<HardpointSlotsComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetSlot(ent.Comp, ((ContainerModifiedMessage)args).Container.ID, out HardpointSlot slot))
		{
			return;
		}
		ent.Comp.PendingRemovals.Clear();
		if (!IsValidHardpoint(((ContainerModifiedMessage)args).Entity, ent.Comp, slot))
		{
			ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
			if (((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots))
			{
				_itemSlots.TryEject(ent.Owner, ((ContainerModifiedMessage)args).Container.ID, null, out var _, itemSlots, excludeUserAudio: true);
			}
			return;
		}
		ent.Comp.LastUiError = null;
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(((ContainerModifiedMessage)args).Entity, ref gun))
		{
			_guns.RefreshModifiers(Entity<GunComponent>.op_Implicit((((ContainerModifiedMessage)args).Entity, gun)));
		}
		ApplyArmorHardpointModifiers(ent.Owner, ((ContainerModifiedMessage)args).Entity, adding: true);
		RefreshSupportModifiers(ent.Owner);
		RefreshCanRun(ent.Owner);
		UpdateHardpointUi(ent.Owner, ent.Comp);
		UpdateContainingVehicleUi(ent.Owner);
		RaiseHardpointSlotsChanged(ent.Owner);
		RaiseVehicleSlotsChanged(ent.Owner);
	}

	private void OnRemoved(Entity<HardpointSlotsComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlot(ent.Comp, ((ContainerModifiedMessage)args).Container.ID, out HardpointSlot _))
		{
			ApplyArmorHardpointModifiers(ent.Owner, ((ContainerModifiedMessage)args).Entity, adding: false);
			RefreshSupportModifiers(ent.Owner);
			ent.Comp.LastUiError = null;
			RefreshCanRun(ent.Owner);
			ent.Comp.PendingRemovals.Remove(((ContainerModifiedMessage)args).Container.ID);
			UpdateHardpointUi(ent.Owner, ent.Comp);
			UpdateContainingVehicleUi(ent.Owner);
			RaiseHardpointSlotsChanged(ent.Owner);
			RaiseVehicleSlotsChanged(ent.Owner);
		}
	}

	private void RefreshSupportModifiers(EntityUid owner)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		EntityUid vehicle = owner;
		HardpointSlotsComponent hardpoints = default(HardpointSlotsComponent);
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if ((!((EntitySystem)this).HasComp<VehicleComponent>(vehicle) && !TryGetContainingVehicleFrame(owner, out vehicle)) || !((EntitySystem)this).TryComp<HardpointSlotsComponent>(vehicle, ref hardpoints) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(vehicle, ref itemSlots))
		{
			return;
		}
		if (_net.IsClient)
		{
			RefreshVehicleGunModifiers(vehicle, hardpoints, itemSlots);
			return;
		}
		FixedPoint2 accuracyMult = FixedPoint2.New(1);
		float fireRateMult = 1f;
		float speedMult = 1f;
		float accelMult = 1f;
		float viewScale = 0f;
		float cursorMaxOffset = 0f;
		float cursorOffsetSpeed = 0.5f;
		float cursorPvsIncrease = 0f;
		bool hasWeaponMods = false;
		bool hasSpeedMods = false;
		bool hasAccelMods = false;
		bool hasViewMods = false;
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id) || !_itemSlots.TryGetSlot(vehicle, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				continue;
			}
			EntityUid item = itemSlot.Item.Value;
			Accumulate(item);
			if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(item, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(item, ref turretItemSlots))
			{
				continue;
			}
			foreach (HardpointSlot turretSlot in turretSlots.Slots)
			{
				if (!string.IsNullOrWhiteSpace(turretSlot.Id) && _itemSlots.TryGetSlot(item, turretSlot.Id, out ItemSlot turretItemSlot, turretItemSlots) && turretItemSlot.HasItem)
				{
					Accumulate(turretItemSlot.Item.Value);
				}
			}
		}
		if (hasWeaponMods)
		{
			VehicleWeaponSupportModifierComponent mods = ((EntitySystem)this).EnsureComp<VehicleWeaponSupportModifierComponent>(vehicle);
			mods.AccuracyMultiplier = accuracyMult;
			mods.FireRateMultiplier = fireRateMult;
			((EntitySystem)this).Dirty(vehicle, (IComponent)(object)mods, (MetaDataComponent)null);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred<VehicleWeaponSupportModifierComponent>(vehicle);
		}
		if (hasSpeedMods)
		{
			VehicleSpeedModifierComponent speed = ((EntitySystem)this).EnsureComp<VehicleSpeedModifierComponent>(vehicle);
			speed.SpeedMultiplier = speedMult;
			((EntitySystem)this).Dirty(vehicle, (IComponent)(object)speed, (MetaDataComponent)null);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred<VehicleSpeedModifierComponent>(vehicle);
		}
		if (hasAccelMods)
		{
			VehicleAccelerationModifierComponent accel = ((EntitySystem)this).EnsureComp<VehicleAccelerationModifierComponent>(vehicle);
			accel.AccelerationMultiplier = accelMult;
			((EntitySystem)this).Dirty(vehicle, (IComponent)(object)accel, (MetaDataComponent)null);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred<VehicleAccelerationModifierComponent>(vehicle);
		}
		if (hasViewMods && viewScale > 0f)
		{
			VehicleGunnerViewComponent view = ((EntitySystem)this).EnsureComp<VehicleGunnerViewComponent>(vehicle);
			view.PvsScale = viewScale;
			view.CursorMaxOffset = cursorMaxOffset;
			view.CursorOffsetSpeed = cursorOffsetSpeed;
			view.CursorPvsIncrease = cursorPvsIncrease;
			((EntitySystem)this).Dirty(vehicle, (IComponent)(object)view, (MetaDataComponent)null);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred<VehicleGunnerViewComponent>(vehicle);
		}
		RefreshVehicleGunModifiers(vehicle, hardpoints, itemSlots);
		void Accumulate(EntityUid val)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			VehicleWeaponSupportAttachmentComponent weaponMod = default(VehicleWeaponSupportAttachmentComponent);
			if (((EntitySystem)this).TryComp<VehicleWeaponSupportAttachmentComponent>(val, ref weaponMod))
			{
				accuracyMult *= weaponMod.AccuracyMultiplier;
				fireRateMult *= weaponMod.FireRateMultiplier;
				hasWeaponMods = true;
			}
			VehicleSpeedModifierAttachmentComponent speedMod = default(VehicleSpeedModifierAttachmentComponent);
			if (((EntitySystem)this).TryComp<VehicleSpeedModifierAttachmentComponent>(val, ref speedMod))
			{
				speedMult *= speedMod.SpeedMultiplier;
				hasSpeedMods = true;
			}
			VehicleAccelerationModifierAttachmentComponent accelMod = default(VehicleAccelerationModifierAttachmentComponent);
			if (((EntitySystem)this).TryComp<VehicleAccelerationModifierAttachmentComponent>(val, ref accelMod))
			{
				accelMult *= accelMod.AccelerationMultiplier;
				hasAccelMods = true;
			}
			VehicleGunnerViewAttachmentComponent viewMod = default(VehicleGunnerViewAttachmentComponent);
			if (((EntitySystem)this).TryComp<VehicleGunnerViewAttachmentComponent>(val, ref viewMod))
			{
				viewScale = Math.Max(viewScale, viewMod.PvsScale);
				cursorMaxOffset = Math.Max(cursorMaxOffset, viewMod.CursorMaxOffset);
				cursorOffsetSpeed = MathF.Max(cursorOffsetSpeed, viewMod.CursorOffsetSpeed);
				cursorPvsIncrease = Math.Max(cursorPvsIncrease, viewMod.CursorPvsIncrease);
				hasViewMods = true;
			}
		}
	}

	private void RefreshVehicleGunModifiers(EntityUid vehicle, HardpointSlotsComponent hardpoints, ItemSlotsComponent itemSlots)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in hardpoints.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id) || !_itemSlots.TryGetSlot(vehicle, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				continue;
			}
			RefreshGunModifiers(itemSlot.Item.Value);
			if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(itemSlot.Item.Value, ref turretSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(itemSlot.Item.Value, ref turretItemSlots))
			{
				continue;
			}
			foreach (HardpointSlot turretSlot in turretSlots.Slots)
			{
				if (!string.IsNullOrWhiteSpace(turretSlot.Id) && _itemSlots.TryGetSlot(itemSlot.Item.Value, turretSlot.Id, out ItemSlot turretItemSlot, turretItemSlots) && turretItemSlot.HasItem)
				{
					RefreshGunModifiers(turretItemSlot.Item.Value);
				}
			}
		}
	}

	private void RefreshGunModifiers(EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(item, ref gun))
		{
			_guns.RefreshModifiers(Entity<GunComponent>.op_Implicit((item, gun)));
		}
	}

	private void ApplyArmorHardpointModifiers(EntityUid vehicle, EntityUid hardpointItem, bool adding)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		VehicleArmorHardpointComponent armor = default(VehicleArmorHardpointComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<VehicleArmorHardpointComponent>(hardpointItem, ref armor))
		{
			return;
		}
		if (armor.ModifierSets.Count > 0)
		{
			DamageProtectionBuffComponent buff = ((EntitySystem)this).EnsureComp<DamageProtectionBuffComponent>(vehicle);
			DamageModifierSetPrototype modifier = default(DamageModifierSetPrototype);
			foreach (ProtoId<DamageModifierSetPrototype> setId in armor.ModifierSets)
			{
				if (_prototypeManager.TryIndex<DamageModifierSetPrototype>(setId, ref modifier))
				{
					if (adding)
					{
						buff.Modifiers[ProtoId<DamageModifierSetPrototype>.op_Implicit(setId)] = modifier;
					}
					else
					{
						buff.Modifiers.Remove(ProtoId<DamageModifierSetPrototype>.op_Implicit(setId));
					}
				}
			}
			if (!adding && buff.Modifiers.Count == 0)
			{
				((EntitySystem)this).RemComp<DamageProtectionBuffComponent>(vehicle);
			}
			else
			{
				((EntitySystem)this).Dirty(vehicle, (IComponent)(object)buff, (MetaDataComponent)null);
			}
		}
		if (armor.ExplosionCoefficient.HasValue)
		{
			ExplosionResistanceComponent resistance = default(ExplosionResistanceComponent);
			if (adding)
			{
				_explosion.SetExplosionResistance(vehicle, armor.ExplosionCoefficient.Value, worn: false);
			}
			else if (((EntitySystem)this).TryComp<ExplosionResistanceComponent>(vehicle, ref resistance) && MathF.Abs(resistance.DamageCoefficient - armor.ExplosionCoefficient.Value) < 0.0001f)
			{
				((EntitySystem)this).RemComp<ExplosionResistanceComponent>(vehicle);
			}
		}
	}

	private void RaiseHardpointSlotsChanged(EntityUid vehicle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsChangedEvent ev = new HardpointSlotsChangedEvent(vehicle);
		((EntitySystem)this).RaiseLocalEvent<HardpointSlotsChangedEvent>(vehicle, ev, true);
	}

	private void RaiseVehicleSlotsChanged(EntityUid owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetContainingVehicleFrame(owner, out var vehicle))
		{
			RaiseHardpointSlotsChanged(vehicle);
		}
	}

	private void OnVehicleCanRun(Entity<HardpointSlotsComponent> ent, ref VehicleCanRunEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanRun && !HasAllRequired(ent.Owner, ent.Comp))
		{
			args.CanRun = false;
		}
	}

	private void EnsureSlots(EntityUid uid, HardpointSlotsComponent component, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (component.Slots.Count == 0)
		{
			return;
		}
		if (itemSlots == null)
		{
			itemSlots = ((EntitySystem)this).EnsureComp<ItemSlotsComponent>(uid);
		}
		foreach (HardpointSlot slot in component.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			if (_itemSlots.TryGetSlot(uid, slot.Id, out ItemSlot existingSlot, itemSlots))
			{
				_itemSlots.SetEjectFlags(uid, existingSlot, disableEject: true, ejectOnInteract: false, ejectOnUse: false, itemSlots);
				continue;
			}
			EntityWhitelist whitelist = slot.Whitelist;
			if (whitelist == null)
			{
				EntityWhitelist entityWhitelist = new EntityWhitelist();
				entityWhitelist.Components = new string[1] { "HardpointItem" };
				whitelist = entityWhitelist;
			}
			else
			{
				bool num = whitelist.Components != null && whitelist.Components.Length != 0;
				bool hasTags = whitelist.Tags != null && whitelist.Tags.Count > 0;
				bool hasSizes = whitelist.Sizes != null && whitelist.Sizes.Count > 0;
				bool hasSkills = whitelist.Skills != null && whitelist.Skills.Count > 0;
				bool hasMinMobSize = whitelist.MinMobSize.HasValue;
				if (!num && !hasTags && !hasSizes && !hasSkills && !hasMinMobSize)
				{
					whitelist.Components = new string[1] { "HardpointItem" };
				}
			}
			ItemSlot itemSlot = new ItemSlot
			{
				Whitelist = whitelist
			};
			_itemSlots.AddItemSlot(uid, slot.Id, itemSlot, itemSlots);
			_itemSlots.SetEjectFlags(uid, itemSlot, disableEject: true, ejectOnInteract: false, ejectOnUse: false, itemSlots);
		}
	}

	internal bool TryGetSlot(HardpointSlotsComponent component, string? id, [NotNullWhen(true)] out HardpointSlot? slot)
	{
		slot = null;
		if (id == null)
		{
			return false;
		}
		foreach (HardpointSlot hardpoint in component.Slots)
		{
			if (hardpoint.Id == id)
			{
				slot = hardpoint;
				return true;
			}
		}
		return false;
	}

	internal bool IsValidHardpoint(EntityUid item, HardpointSlotsComponent slots, HardpointSlot slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		HardpointItemComponent hardpoint = default(HardpointItemComponent);
		if (!((EntitySystem)this).TryComp<HardpointItemComponent>(item, ref hardpoint))
		{
			return false;
		}
		ProtoId<HardpointVehicleFamilyPrototype>? vehicleFamily = slots.VehicleFamily;
		if (vehicleFamily.HasValue)
		{
			vehicleFamily = hardpoint.VehicleFamily;
			if (!vehicleFamily.HasValue)
			{
				return false;
			}
			ProtoId<HardpointVehicleFamilyPrototype> vehicleFamily2 = vehicleFamily.GetValueOrDefault();
			if (vehicleFamily2 != slots.VehicleFamily.Value)
			{
				return false;
			}
		}
		if (slot.SlotType.HasValue)
		{
			ProtoId<HardpointSlotTypePrototype>? slotType = hardpoint.SlotType;
			if (!slotType.HasValue)
			{
				return false;
			}
			ProtoId<HardpointSlotTypePrototype> slotType2 = slotType.GetValueOrDefault();
			if (slotType2 != slot.SlotType.Value)
			{
				return false;
			}
		}
		if (!string.IsNullOrWhiteSpace(slot.CompatibilityId))
		{
			if (string.IsNullOrWhiteSpace(hardpoint.CompatibilityId))
			{
				return false;
			}
			if (!string.Equals(hardpoint.CompatibilityId, slot.CompatibilityId, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		if (string.IsNullOrWhiteSpace(slot.HardpointType))
		{
			if (slot.Whitelist != null)
			{
				return _whitelist.IsValid(slot.Whitelist, item);
			}
			return true;
		}
		if (!string.Equals(hardpoint.HardpointType, slot.HardpointType, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (slot.Whitelist != null)
		{
			return _whitelist.IsValid(slot.Whitelist, item);
		}
		return true;
	}

	private bool HasAllRequired(EntityUid uid, HardpointSlotsComponent component, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (component.Slots.Count == 0)
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return true;
		}
		HardpointIntegrityComponent integrity = default(HardpointIntegrityComponent);
		foreach (HardpointSlot slot in component.Slots)
		{
			if (!slot.Required)
			{
				continue;
			}
			if (!_itemSlots.TryGetSlot(uid, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				return false;
			}
			EntityUid? item = itemSlot.Item;
			if (item.HasValue)
			{
				EntityUid item2 = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(item2, ref integrity) && integrity.Integrity <= 0f)
				{
					return false;
				}
			}
		}
		return true;
	}

	internal void RefreshCanRun(EntityUid uid)
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

	private void OnVehicleDamageModify(Entity<HardpointSlotsComponent> ent, ref DamageModifyEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (_net.IsClient || args.Damage.GetTotal().Float() <= 0f || !((EntitySystem)this).TryComp<ItemSlotsComponent>(ent.Owner, ref itemSlots))
		{
			return;
		}
		List<(EntityUid, HardpointIntegrityComponent)> topLevelHardpoints = new List<(EntityUid, HardpointIntegrityComponent)>();
		CollectIntactTopLevelHardpoints(ent.Owner, ent.Comp, itemSlots, topLevelHardpoints);
		bool anyTopLevelIntact = topLevelHardpoints.Count > 0;
		if (anyTopLevelIntact)
		{
			HashSet<EntityUid> visited = new HashSet<EntityUid>();
			foreach (var (item, integrity) in topLevelHardpoints)
			{
				ApplyDamageToHardpointTree(ent.Owner, item, integrity, args.Damage, visited);
			}
		}
		float hullFraction = (anyTopLevelIntact ? ent.Comp.FrameDamageFractionWhileIntact : 1f);
		HardpointIntegrityComponent frameIntegrity = default(HardpointIntegrityComponent);
		if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(ent.Owner, ref frameIntegrity))
		{
			DamageSpecifier frameDamage = ScaleDamage(args.Damage, hullFraction);
			float frameAmount = GetVehicleFrameDamageAmount(ent.Owner, frameDamage);
			if (frameAmount > 0f)
			{
				DamageHardpoint(ent.Owner, ent.Owner, frameAmount, frameIntegrity);
			}
		}
		args.Damage = ScaleDamage(args.Damage, hullFraction);
	}

	private void CollectIntactTopLevelHardpoints(EntityUid owner, HardpointSlotsComponent slots, ItemSlotsComponent itemSlots, List<(EntityUid Item, HardpointIntegrityComponent Integrity)> intactHardpoints)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		HardpointIntegrityComponent integrity = default(HardpointIntegrityComponent);
		foreach (HardpointSlot slot in slots.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id) || !_itemSlots.TryGetSlot(owner, slot.Id, out ItemSlot itemSlot, itemSlots) || !itemSlot.HasItem)
			{
				continue;
			}
			EntityUid? item = itemSlot.Item;
			if (item.HasValue)
			{
				EntityUid item2 = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(item2, ref integrity) && integrity.Integrity > 0f)
				{
					intactHardpoints.Add((item2, integrity));
				}
			}
		}
	}

	private void ApplyDamageToHardpointTree(EntityUid vehicle, EntityUid hardpoint, HardpointIntegrityComponent integrity, DamageSpecifier damage, HashSet<EntityUid> visited)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!visited.Add(hardpoint))
		{
			return;
		}
		ApplyDamageToHardpoint(vehicle, hardpoint, integrity, damage);
		HardpointSlotsComponent childSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent childItemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<HardpointSlotsComponent>(hardpoint, ref childSlots) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(hardpoint, ref childItemSlots))
		{
			return;
		}
		HardpointIntegrityComponent childIntegrity = default(HardpointIntegrityComponent);
		foreach (HardpointSlot slot in childSlots.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id) || !_itemSlots.TryGetSlot(hardpoint, slot.Id, out ItemSlot itemSlot, childItemSlots))
			{
				continue;
			}
			EntityUid? item = itemSlot.Item;
			if (item.HasValue)
			{
				EntityUid childHardpoint = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(childHardpoint, ref childIntegrity) && !(childIntegrity.Integrity <= 0f))
				{
					ApplyDamageToHardpointTree(vehicle, childHardpoint, childIntegrity, damage, visited);
				}
			}
		}
	}

	private DamageSpecifier ScaleDamage(DamageSpecifier source, float fraction)
	{
		if (MathF.Abs(fraction - 1f) < 0.0001f)
		{
			return source;
		}
		DamageSpecifier scaled = new DamageSpecifier();
		foreach (var (type, value) in source.DamageDict)
		{
			scaled.DamageDict[type] = value * fraction;
		}
		return scaled;
	}

	private void ApplyDamageToHardpoint(EntityUid vehicle, EntityUid hardpoint, HardpointIntegrityComponent integrity, DamageSpecifier damage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		float amount = GetHardpointDamageAmount(hardpoint, damage);
		if (!(amount <= 0f))
		{
			DamageHardpoint(vehicle, hardpoint, amount, integrity);
		}
	}

	private float GetHardpointDamageAmount(EntityUid hardpoint, DamageSpecifier damage)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float total = MathF.Max(damage.GetTotal().Float(), 0f);
		List<DamageModifierSet> modifierSets = new List<DamageModifierSet>();
		CollectHardpointDamageModifierSets(hardpoint, modifierSets);
		if (modifierSets.Count > 0)
		{
			total = MathF.Max(DamageSpecifier.ApplyModifierSets(damage, modifierSets).GetTotal().Float(), 0f);
		}
		HardpointItemComponent hardpointItem = default(HardpointItemComponent);
		if (((EntitySystem)this).TryComp<HardpointItemComponent>(hardpoint, ref hardpointItem))
		{
			total *= MathF.Max(hardpointItem.DamageMultiplier, 0f);
		}
		return total;
	}

	private void CollectHardpointDamageModifierSets(EntityUid hardpoint, List<DamageModifierSet> modifierSets)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		HardpointDamageModifierComponent hardpointModifiers = default(HardpointDamageModifierComponent);
		if (((EntitySystem)this).TryComp<HardpointDamageModifierComponent>(hardpoint, ref hardpointModifiers))
		{
			DamageModifierSetPrototype modifierSet = default(DamageModifierSetPrototype);
			foreach (ProtoId<DamageModifierSetPrototype> modifierSetId in hardpointModifiers.ModifierSets)
			{
				if (_prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSetId, ref modifierSet))
				{
					modifierSets.Add(modifierSet);
				}
			}
		}
		VehicleArmorHardpointComponent armorHardpoint = default(VehicleArmorHardpointComponent);
		if (!((EntitySystem)this).TryComp<VehicleArmorHardpointComponent>(hardpoint, ref armorHardpoint))
		{
			return;
		}
		DamageModifierSetPrototype modifierSet2 = default(DamageModifierSetPrototype);
		foreach (ProtoId<DamageModifierSetPrototype> modifierSetId2 in armorHardpoint.ModifierSets)
		{
			if (_prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSetId2, ref modifierSet2))
			{
				modifierSets.Add(modifierSet2);
			}
		}
	}

	private float GetVehicleFrameDamageAmount(EntityUid vehicle, DamageSpecifier damage)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float total = MathF.Max(damage.GetTotal().Float(), 0f);
		DamageProtectionBuffComponent protection = default(DamageProtectionBuffComponent);
		if (!((EntitySystem)this).TryComp<DamageProtectionBuffComponent>(vehicle, ref protection) || protection.Modifiers.Count == 0)
		{
			return total;
		}
		DamageSpecifier modifiedDamage = damage;
		foreach (DamageModifierSetPrototype modifier in protection.Modifiers.Values)
		{
			modifiedDamage = DamageSpecifier.ApplyModifierSet(modifiedDamage, modifier);
		}
		return MathF.Max(modifiedDamage.GetTotal().Float(), 0f);
	}

	private void OnHardpointIntegrityInit(Entity<HardpointIntegrityComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Integrity <= 0f)
		{
			ent.Comp.Integrity = ent.Comp.MaxIntegrity;
		}
		UpdateFrameDamageAppearance(ent.Owner, ent.Comp);
	}

	private void OnHardpointExamined(Entity<HardpointIntegrityComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		float current = ent.Comp.Integrity;
		float max = ent.Comp.MaxIntegrity;
		float percent = ((max > 0f) ? (current / max) : 0f);
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			args.PushMarkup(base.Loc.GetString(GetHardpointConditionString(percent)));
			return;
		}
		string color = GetHardpointIntegrityColor(percent);
		args.PushMarkup(base.Loc.GetString("rmc-hardpoint-integrity-examine", new(string, object)[4]
		{
			("color", color),
			("current", (int)MathF.Ceiling(current)),
			("max", (int)MathF.Ceiling(max)),
			("percent", (int)MathF.Round(percent * 100f))
		}));
		if (TryGetArmorExamineModifiers(ent.Owner, out var acid, out var slash, out var bullet, out var explosive, out var blunt))
		{
			args.PushMarkup(base.Loc.GetString("rmc-hardpoint-armor-modifiers-examine", new(string, object)[5]
			{
				("acid", FormatModifierValue(acid)),
				("slash", FormatModifierValue(slash)),
				("bullet", FormatModifierValue(bullet)),
				("explosive", FormatModifierValue(explosive)),
				("blunt", FormatModifierValue(blunt))
			}));
		}
	}

	private bool TryGetArmorExamineModifiers(EntityUid uid, out float acid, out float slash, out float bullet, out float explosive, out float blunt)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		acid = 1f;
		slash = 1f;
		bullet = 1f;
		explosive = 1f;
		blunt = 1f;
		VehicleArmorHardpointComponent armor = default(VehicleArmorHardpointComponent);
		if (!((EntitySystem)this).TryComp<VehicleArmorHardpointComponent>(uid, ref armor))
		{
			return false;
		}
		HardpointItemComponent item = default(HardpointItemComponent);
		if (((EntitySystem)this).TryComp<HardpointItemComponent>(uid, ref item))
		{
			ProtoId<HardpointVehicleFamilyPrototype>? vehicleFamily = item.VehicleFamily;
			ProtoId<HardpointVehicleFamilyPrototype>? val = ProtoId<HardpointVehicleFamilyPrototype>.op_Implicit("Tank");
			DamageModifierSetPrototype tankBase = default(DamageModifierSetPrototype);
			if (vehicleFamily.HasValue == val.HasValue && (!vehicleFamily.HasValue || vehicleFamily.GetValueOrDefault() == val.GetValueOrDefault()) && _prototypeManager.TryIndex<DamageModifierSetPrototype>("VehicleFrameTank", ref tankBase))
			{
				ApplyDamageModifierCoefficients(tankBase, ref acid, ref slash, ref bullet, ref explosive, ref blunt);
			}
		}
		DamageModifierSetPrototype modifierSet = default(DamageModifierSetPrototype);
		foreach (ProtoId<DamageModifierSetPrototype> modifierSetId in armor.ModifierSets)
		{
			if (_prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSetId, ref modifierSet))
			{
				ApplyDamageModifierCoefficients(modifierSet, ref acid, ref slash, ref bullet, ref explosive, ref blunt);
			}
		}
		return true;
	}

	private static void ApplyDamageModifierCoefficients(DamageModifierSet modifierSet, ref float acid, ref float slash, ref float bullet, ref float explosive, ref float blunt)
	{
		if (modifierSet.Coefficients.TryGetValue("Caustic", out var acidCoefficient))
		{
			acid *= acidCoefficient;
		}
		if (modifierSet.Coefficients.TryGetValue("Slash", out var slashCoefficient))
		{
			slash *= slashCoefficient;
		}
		if (modifierSet.Coefficients.TryGetValue("Piercing", out var bulletCoefficient))
		{
			bullet *= bulletCoefficient;
		}
		if (modifierSet.Coefficients.TryGetValue("Structural", out var explosiveCoefficient))
		{
			explosive *= explosiveCoefficient;
		}
		if (modifierSet.Coefficients.TryGetValue("Blunt", out var bluntCoefficient))
		{
			blunt *= bluntCoefficient;
		}
	}

	private static string FormatModifierValue(float value)
	{
		return value.ToString("0.###");
	}

	private string GetHardpointIntegrityColor(float percent)
	{
		if (percent >= 0.9f)
		{
			return "green";
		}
		if (percent >= 0.7f)
		{
			return "yellow";
		}
		if (percent >= 0.4f)
		{
			return "orange";
		}
		if (percent >= 0.15f)
		{
			return "red";
		}
		return "crimson";
	}

	private string GetHardpointConditionString(float percent)
	{
		if (percent >= 0.9f)
		{
			return "rmc-hardpoint-condition-pristine";
		}
		if (percent >= 0.7f)
		{
			return "rmc-hardpoint-condition-good";
		}
		if (percent >= 0.4f)
		{
			return "rmc-hardpoint-condition-worn";
		}
		if (percent >= 0.15f)
		{
			return "rmc-hardpoint-condition-bad";
		}
		return "rmc-hardpoint-condition-critical";
	}

	public bool DamageHardpoint(EntityUid vehicle, EntityUid hardpoint, float amount, HardpointIntegrityComponent? integrity = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || amount <= 0f)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<HardpointIntegrityComponent>(hardpoint, ref integrity, false))
		{
			return false;
		}
		if (integrity.Integrity <= 0f)
		{
			return false;
		}
		if (integrity.Integrity > integrity.MaxIntegrity && integrity.MaxIntegrity > 0f)
		{
			integrity.Integrity = integrity.MaxIntegrity;
		}
		float previous = integrity.Integrity;
		integrity.Integrity = MathF.Max(0f, integrity.Integrity - amount);
		if (Math.Abs(previous - integrity.Integrity) < 0.01f)
		{
			return false;
		}
		((EntitySystem)this).Dirty(hardpoint, (IComponent)(object)integrity, (MetaDataComponent)null);
		UpdateFrameDamageAppearance(hardpoint, integrity);
		VehicleWheelItemComponent vehicleWheelItemComponent = default(VehicleWheelItemComponent);
		if (((EntitySystem)this).TryComp<VehicleWheelItemComponent>(hardpoint, ref vehicleWheelItemComponent))
		{
			_wheels.OnWheelDamaged(vehicle);
		}
		if (previous > 0f && integrity.Integrity <= 0f)
		{
			RefreshCanRun(vehicle);
			if (hardpoint == vehicle)
			{
				RMCVehicleFrameDestroyedEvent destroyed = new RMCVehicleFrameDestroyedEvent(vehicle);
				((EntitySystem)this).RaiseLocalEvent<RMCVehicleFrameDestroyedEvent>(vehicle, destroyed, false);
			}
		}
		UpdateHardpointUi(vehicle);
		return true;
	}

	private void OnHardpointRepair(Entity<HardpointIntegrityComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		_ = args.User;
		EntityUid used = args.Used;
		bool isFrame = ((EntitySystem)this).HasComp<HardpointSlotsComponent>(ent.Owner);
		bool usedWelder = _tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.RepairToolQuality)) && ((EntitySystem)this).HasComp<BlowtorchComponent>(used);
		bool usedWrench = isFrame && _tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.FrameFinishToolQuality));
		if (!usedWelder && !usedWrench)
		{
			return;
		}
		float repairCap = ent.Comp.MaxIntegrity * ent.Comp.RepairCapFraction;
		if (ent.Comp.Integrity >= repairCap - ent.Comp.FrameRepairEpsilon)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-hardpoint-intact"), ent.Owner, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		if (ent.Comp.Repairing)
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		float weldCap = MathF.Min(ent.Comp.MaxIntegrity * ent.Comp.FrameWeldCapFraction, repairCap);
		if (usedWelder && isFrame && ent.Comp.Integrity >= weldCap - ent.Comp.FrameRepairEpsilon)
		{
			_popup.PopupClient("Finish tightening the frame with a wrench.", ent.Owner, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		if (usedWrench && ent.Comp.Integrity < weldCap - ent.Comp.FrameRepairEpsilon)
		{
			_popup.PopupClient("Weld the frame before tightening it.", ent.Owner, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		if (usedWelder && !_repairable.UseFuel(used, args.User, ent.Comp.RepairFuelCost, attempt: true))
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		float repairAmount = GetRepairAmountForCurrentStep(ent.Owner, ent.Comp, usedWelder, usedWrench, isFrame);
		if (repairAmount <= 0f)
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		float repairTime = GetRepairTimeForCurrentStep(ent.Owner, args.User, ent.Comp, repairAmount, isFrame);
		ent.Comp.Repairing = true;
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, repairTime, new HardpointRepairDoAfterEvent(), ent.Owner, ent.Owner, used)
		{
			BreakOnMove = true,
			BreakOnDamage = true,
			NeedHand = true
		};
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			ent.Comp.Repairing = false;
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnHardpointRepairDoAfter(Entity<HardpointIntegrityComponent> ent, ref HardpointRepairDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Repairing = false;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? used = args.Used;
		bool isFrame = ((EntitySystem)this).HasComp<HardpointSlotsComponent>(ent.Owner);
		bool usedWelder = used.HasValue && _tool.HasQuality(used.Value, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.RepairToolQuality)) && ((EntitySystem)this).HasComp<BlowtorchComponent>(used);
		bool usedWrench = isFrame && used.HasValue && _tool.HasQuality(used.Value, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.FrameFinishToolQuality));
		if ((!usedWelder && !usedWrench) || (usedWelder && (!used.HasValue || !_repairable.UseFuel(used.Value, args.User, ent.Comp.RepairFuelCost))))
		{
			return;
		}
		float repairAmount = GetRepairAmountForCurrentStep(ent.Owner, ent.Comp, usedWelder, usedWrench, isFrame);
		if (!(repairAmount <= 0f))
		{
			float repairCapApply = ent.Comp.MaxIntegrity * ent.Comp.RepairCapFraction;
			ent.Comp.Integrity = MathF.Min(repairCapApply, ent.Comp.Integrity + repairAmount);
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			UpdateFrameDamageAppearance(ent.Owner, ent.Comp);
			if (ent.Comp.RepairSound != null)
			{
				_audio.PlayPredicted(ent.Comp.RepairSound, ent.Owner, (EntityUid?)args.User, (AudioParams?)null);
			}
			_popup.PopupClient(base.Loc.GetString("rmc-hardpoint-repaired"), ent.Owner, args.User);
			EntityUid vehicle = ent.Owner;
			VehicleWheelItemComponent vehicleWheelItemComponent = default(VehicleWheelItemComponent);
			if (((EntitySystem)this).TryComp<VehicleWheelItemComponent>(ent.Owner, ref vehicleWheelItemComponent))
			{
				vehicle = (EntityUid)(((_003F?)GetVehicleFromPart(ent.Owner)) ?? ent.Owner);
				_wheels.OnWheelDamaged(vehicle);
			}
			else
			{
				RefreshCanRun(ent.Owner);
			}
			if (ent.Comp.BypassEntryOnZero)
			{
				RefreshCanRun(vehicle);
			}
			UpdateHardpointUi(vehicle);
			if (ShouldRepeatRepair(ent.Owner, ent.Comp, usedWelder, usedWrench, isFrame))
			{
				args.Repeat = true;
			}
		}
	}

	private float GetRepairAmountForCurrentStep(EntityUid uid, HardpointIntegrityComponent integrity, bool usedWelder, bool usedWrench, bool isFrame)
	{
		if (integrity.MaxIntegrity <= 0f)
		{
			return 0f;
		}
		float chunkSize = MathF.Max(integrity.RepairChunkMinimum, integrity.MaxIntegrity * integrity.RepairChunkFraction);
		float repairCap = integrity.MaxIntegrity * integrity.RepairCapFraction;
		float weldCap = MathF.Min(integrity.MaxIntegrity * integrity.FrameWeldCapFraction, repairCap);
		if (usedWelder)
		{
			float target = (isFrame ? weldCap : repairCap);
			if (!isFrame)
			{
				return MathF.Max(0f, target - integrity.Integrity);
			}
			return MathF.Max(0f, MathF.Min(chunkSize, target - integrity.Integrity));
		}
		if (usedWrench)
		{
			return MathF.Max(0f, MathF.Min(chunkSize, repairCap - integrity.Integrity));
		}
		return 0f;
	}

	private float GetRepairTimeForCurrentStep(EntityUid uid, EntityUid user, HardpointIntegrityComponent integrity, float repairAmount, bool isFrame)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (integrity.MaxIntegrity <= 0f || repairAmount <= 0f)
		{
			return 0f;
		}
		float repairFraction = repairAmount / integrity.MaxIntegrity;
		float skillMultiplier = _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), EngineerSkill);
		if (isFrame)
		{
			return integrity.FrameRepairChunkSeconds * (repairFraction / integrity.RepairChunkFraction) * skillMultiplier;
		}
		return integrity.ModuleRepairSeconds * skillMultiplier;
	}

	private bool ShouldRepeatRepair(EntityUid uid, HardpointIntegrityComponent integrity, bool usedWelder, bool usedWrench, bool isFrame)
	{
		float repairCap = integrity.MaxIntegrity * integrity.RepairCapFraction;
		if (integrity.Integrity >= repairCap - integrity.FrameRepairEpsilon)
		{
			return false;
		}
		if (isFrame)
		{
			float weldCap = MathF.Min(integrity.MaxIntegrity * integrity.FrameWeldCapFraction, repairCap);
			if (usedWelder)
			{
				return integrity.Integrity < weldCap - integrity.FrameRepairEpsilon;
			}
			if (usedWrench)
			{
				if (integrity.Integrity >= weldCap - integrity.FrameRepairEpsilon)
				{
					return integrity.Integrity < repairCap;
				}
				return false;
			}
			return false;
		}
		if (usedWelder && integrity.Integrity > 0f)
		{
			return integrity.Integrity < repairCap;
		}
		return false;
	}

	private EntityUid? GetVehicleFromPart(EntityUid part)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(part), ref container))
		{
			return null;
		}
		return container.Owner;
	}

	internal void UpdateHardpointUi(EntityUid uid, HardpointSlotsComponent? component = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !((EntitySystem)this).Resolve<HardpointSlotsComponent>(uid, ref component, false) || !((EntitySystem)this).Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
		{
			return;
		}
		List<HardpointUiEntry> entries = new List<HardpointUiEntry>(component.Slots.Count);
		float frameIntegrity = 0f;
		float frameMaxIntegrity = 0f;
		bool hasFrameIntegrity = false;
		HardpointIntegrityComponent frame = default(HardpointIntegrityComponent);
		if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(uid, ref frame))
		{
			frameIntegrity = frame.Integrity;
			frameMaxIntegrity = frame.MaxIntegrity;
			hasFrameIntegrity = true;
		}
		HardpointIntegrityComponent hardpointIntegrity = default(HardpointIntegrityComponent);
		HardpointSlotsComponent turretSlots = default(HardpointSlotsComponent);
		ItemSlotsComponent turretItemSlots = default(ItemSlotsComponent);
		foreach (HardpointSlot slot in component.Slots)
		{
			if (string.IsNullOrWhiteSpace(slot.Id))
			{
				continue;
			}
			ItemSlot itemSlot;
			bool hasItem = _itemSlots.TryGetSlot(uid, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem;
			string installedName = null;
			NetEntity? installedEntity = null;
			float integrity = 0f;
			float maxIntegrity = 0f;
			bool hasIntegrity = false;
			EntityUid? item;
			if (hasItem)
			{
				item = itemSlot.Item;
				if (item.HasValue)
				{
					EntityUid item2 = item.GetValueOrDefault();
					installedEntity = ((EntitySystem)this).GetNetEntity(item2, (MetaDataComponent)null);
					installedName = ((EntitySystem)this).Name(item2, (MetaDataComponent)null);
					if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(item2, ref hardpointIntegrity))
					{
						integrity = hardpointIntegrity.Integrity;
						maxIntegrity = hardpointIntegrity.MaxIntegrity;
						hasIntegrity = true;
					}
				}
			}
			entries.Add(new HardpointUiEntry(slot.Id, slot.HardpointType, installedName, installedEntity, integrity, maxIntegrity, hasIntegrity, hasItem, slot.Required, component.PendingRemovals.Contains(slot.Id)));
			if (!hasItem)
			{
				continue;
			}
			item = itemSlot?.Item;
			if (item.HasValue)
			{
				EntityUid turretItem = item.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<HardpointSlotsComponent>(turretItem, ref turretSlots) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(turretItem, ref turretItemSlots))
				{
					AppendTurretEntries(entries, slot.Id, turretItem, turretSlots, turretItemSlots);
				}
			}
		}
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)HardpointUiKey.Key, (BoundUserInterfaceState)(object)new HardpointBoundUserInterfaceState(entries, frameIntegrity, frameMaxIntegrity, hasFrameIntegrity, component.LastUiError));
	}

	internal bool HasAttachedHardpoints(EntityUid owner, HardpointSlotsComponent slots, ItemSlotsComponent itemSlots)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		foreach (HardpointSlot slot in slots.Slots)
		{
			if (!string.IsNullOrWhiteSpace(slot.Id) && _itemSlots.TryGetSlot(owner, slot.Id, out ItemSlot itemSlot, itemSlots) && itemSlot.HasItem)
			{
				return true;
			}
		}
		return false;
	}

	private void AppendTurretEntries(List<HardpointUiEntry> entries, string parentSlotId, EntityUid turretUid, HardpointSlotsComponent turretSlots, ItemSlotsComponent turretItemSlots)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		HardpointIntegrityComponent hardpointIntegrity = default(HardpointIntegrityComponent);
		foreach (HardpointSlot turretSlot in turretSlots.Slots)
		{
			if (string.IsNullOrWhiteSpace(turretSlot.Id))
			{
				continue;
			}
			string compositeId = VehicleTurretSlotIds.Compose(parentSlotId, turretSlot.Id);
			ItemSlot itemSlot;
			bool hasItem = _itemSlots.TryGetSlot(turretUid, turretSlot.Id, out itemSlot, turretItemSlots) && itemSlot.HasItem;
			string installedName = null;
			NetEntity? installedEntity = null;
			float integrity = 0f;
			float maxIntegrity = 0f;
			bool hasIntegrity = false;
			if (hasItem)
			{
				EntityUid? item = itemSlot.Item;
				if (item.HasValue)
				{
					EntityUid installedItem = item.GetValueOrDefault();
					installedEntity = ((EntitySystem)this).GetNetEntity(installedItem, (MetaDataComponent)null);
					installedName = ((EntitySystem)this).Name(installedItem, (MetaDataComponent)null);
					if (((EntitySystem)this).TryComp<HardpointIntegrityComponent>(installedItem, ref hardpointIntegrity))
					{
						integrity = hardpointIntegrity.Integrity;
						maxIntegrity = hardpointIntegrity.MaxIntegrity;
						hasIntegrity = true;
					}
				}
			}
			entries.Add(new HardpointUiEntry(compositeId, turretSlot.HardpointType, installedName, installedEntity, integrity, maxIntegrity, hasIntegrity, hasItem, turretSlot.Required, turretSlots.PendingRemovals.Contains(turretSlot.Id)));
		}
	}

	internal void UpdateContainingVehicleUi(EntityUid owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetContainingVehicleFrame(owner, out var vehicle))
		{
			UpdateHardpointUi(vehicle);
		}
	}

	internal void SetContainingVehicleUiError(EntityUid owner, string? error)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		HardpointSlotsComponent slots = default(HardpointSlotsComponent);
		if (TryGetContainingVehicleFrame(owner, out var vehicle) && ((EntitySystem)this).TryComp<HardpointSlotsComponent>(vehicle, ref slots))
		{
			slots.LastUiError = error;
		}
	}

	internal bool TryGetContainingVehicleFrame(EntityUid owner, out EntityUid vehicle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _topology.TryGetVehicle(owner, out vehicle);
	}

	private void UpdateFrameDamageAppearance(EntityUid uid, HardpointIntegrityComponent component)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			float max = ((component.MaxIntegrity > 0f) ? component.MaxIntegrity : 1f);
			float fraction = Math.Clamp((max > 0f) ? (component.Integrity / max) : 1f, 0f, 1f);
			_appearance.SetData(uid, (Enum)VehicleFrameDamageVisuals.IntegrityFraction, (object)fraction, appearance);
		}
	}

	internal bool TryGetPryingTool(EntityUid user, ProtoId<ToolQualityPrototype> quality, out EntityUid tool)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		tool = default(EntityUid);
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(user, ref hands))
		{
			return false;
		}
		string activeHand = _hands.GetActiveHand(Entity<HandsComponent>.op_Implicit((user, hands)));
		if (activeHand == null)
		{
			return false;
		}
		if (!_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit((user, hands)), activeHand, out var held))
		{
			return false;
		}
		ToolComponent toolComp = default(ToolComponent);
		if (!((EntitySystem)this).TryComp<ToolComponent>(held.Value, ref toolComp))
		{
			return false;
		}
		if (!_tool.HasQuality(held.Value, ProtoId<ToolQualityPrototype>.op_Implicit(quality), toolComp))
		{
			return false;
		}
		tool = held.Value;
		return true;
	}
}
