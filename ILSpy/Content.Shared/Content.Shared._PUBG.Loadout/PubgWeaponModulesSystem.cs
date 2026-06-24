using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Vision;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared._PUBG.Loadout;

public sealed class PubgWeaponModulesSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private UnpoweredFlashlightSystem _flashlight;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, GunGetAmmoSpreadEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, GunGetAmmoSpreadEvent>)OnWeaponGetAmmoSpread, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, GunRefreshModifiersEvent>)OnGunRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, GunMuzzleFlashAttemptEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, GunMuzzleFlashAttemptEvent>)OnGunMuzzleFlashAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, GetItemActionsEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, PubgToggleWeaponLightActionEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, PubgToggleWeaponLightActionEvent>)OnToggleWeaponLightAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, ExaminedEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, ExaminedEvent>)OnWeaponExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModuleComponent, ExaminedEvent>((EntityEventRefHandler<PubgWeaponModuleComponent, ExaminedEvent>)OnModuleExamined, (Type[])null, (Type[])null);
	}

	public bool TryGetSlotDefinition(EntityUid weapon, string slotId, out PubgWeaponModuleSlotDefinition slotDefinition, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		slotDefinition = null;
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return false;
		}
		foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
		{
			if (slotId.Equals(GetResolvedContainerId(slot), StringComparison.OrdinalIgnoreCase))
			{
				slotDefinition = slot;
				return true;
			}
		}
		return false;
	}

	public bool TryGetSlotDefinition(EntityUid weapon, PubgModuleSlotType slotType, out PubgWeaponModuleSlotDefinition slotDefinition, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		slotDefinition = null;
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return false;
		}
		foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
		{
			if (slot.Slot == slotType)
			{
				slotDefinition = slot;
				return true;
			}
		}
		return false;
	}

	public IEnumerable<(PubgWeaponModuleSlotDefinition Slot, EntityUid? Module)> EnumerateInstalledModules(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			yield break;
		}
		BaseContainer container = default(BaseContainer);
		foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
		{
			EntityUid? installed = null;
			string slotId = GetResolvedContainerId(slot);
			if (_container.TryGetContainer(weapon, slotId, ref container, (ContainerManagerComponent)null))
			{
				ContainerSlot containerSlot = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
				if (containerSlot != null)
				{
					installed = containerSlot.ContainedEntity;
				}
			}
			yield return (Slot: slot, Module: installed);
		}
	}

	public bool IsModuleAllowedInSlot(EntityUid module, PubgWeaponModuleSlotDefinition slotDefinition, PubgWeaponModuleComponent? moduleComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModuleComponent>(module, ref moduleComp, false))
		{
			return false;
		}
		if (moduleComp.Slot != slotDefinition.Slot)
		{
			return false;
		}
		if (slotDefinition.AllowedModules.Count == 0)
		{
			return false;
		}
		MetaDataComponent meta = default(MetaDataComponent);
		if (!((EntitySystem)this).TryComp(module, ref meta) || meta.EntityPrototype == null)
		{
			return false;
		}
		string id = meta.EntityPrototype.ID;
		foreach (EntProtoId allowedModule in slotDefinition.AllowedModules)
		{
			EntProtoId allowed = allowedModule;
			if (id.Equals(((EntProtoId)(ref allowed)).Id, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}

	public float GetSpreadMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 1f;
		}
		float spreadMultiplier = 1f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		PubgBipodComponent bipod = default(PubgBipodComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				spreadMultiplier = ((!((EntitySystem)this).TryComp<PubgBipodComponent>(installed.Value, ref bipod) || !bipod.Deployed) ? (spreadMultiplier * moduleComp.SpreadMultiplier) : (spreadMultiplier * bipod.DeployedSpreadMultiplier));
			}
		}
		return Math.Clamp(spreadMultiplier, 0.5f, 1.5f);
	}

	public float GetHipfireSpreadMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 1f;
		}
		float spreadMultiplier = 1f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		PubgBipodComponent bipod = default(PubgBipodComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				spreadMultiplier = ((!((EntitySystem)this).TryComp<PubgBipodComponent>(installed.Value, ref bipod) || !bipod.Deployed) ? (spreadMultiplier * moduleComp.HipfireSpreadMultiplier) : (spreadMultiplier * bipod.DeployedHipfireSpreadMultiplier));
			}
		}
		return Math.Clamp(spreadMultiplier, 0.5f, 1.75f);
	}

	public float GetRecoilMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 1f;
		}
		float recoilMultiplier = 1f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		PubgBipodComponent bipod = default(PubgBipodComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				recoilMultiplier = ((!((EntitySystem)this).TryComp<PubgBipodComponent>(installed.Value, ref bipod) || !bipod.Deployed) ? (recoilMultiplier * moduleComp.RecoilMultiplier) : (recoilMultiplier * bipod.DeployedRecoilMultiplier));
			}
		}
		return Math.Clamp(recoilMultiplier, 0.25f, 2f);
	}

	public float GetReloadTimeMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 1f;
		}
		float reloadMultiplier = 1f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				reloadMultiplier *= moduleComp.ReloadTimeMultiplier;
			}
		}
		return Math.Clamp(reloadMultiplier, 0.4f, 2f);
	}

	public int GetMagazineCapacityBonus(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 0;
		}
		int bonus = 0;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				bonus += moduleComp.MagazineCapacityBonus;
			}
		}
		return Math.Clamp(bonus, -20, 200);
	}

	public float GetRangeMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 1f;
		}
		float multiplier = 1f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				multiplier *= moduleComp.RangeMultiplier;
			}
		}
		return Math.Clamp(multiplier, 0.1f, 10f);
	}

	public float GetFocusBonusTiles(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return 0f;
		}
		float focusBonus = 0f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				focusBonus += moduleComp.FocusBonusTiles;
			}
		}
		return Math.Clamp(focusBonus, 0f, 8f);
	}

	public SoundSpecifier? GetGunshotOverrideSound(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return null;
		}
		SoundSpecifier sound = null;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp) && moduleComp.SoundGunshotOverride != null)
			{
				sound = moduleComp.SoundGunshotOverride;
			}
		}
		return sound;
	}

	public bool ShouldSuppressMuzzleFlash(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return false;
		}
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp) && moduleComp.SuppressMuzzleFlash)
			{
				return true;
			}
		}
		return false;
	}

	public PubgSpatialGunshotModifiers GetSpatialGunshotModifiers(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return PubgSpatialGunshotModifiers.Default;
		}
		SoundSpecifier farSoundOverride = null;
		bool disableFarSound = false;
		float audioRangeMultiplier = 1f;
		float nearRangeMultiplier = 1f;
		float coneAngleMultiplier = 1f;
		float nearVolumeMultiplier = 1f;
		PubgWeaponModuleComponent moduleComp = default(PubgWeaponModuleComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModuleComponent>(installed.Value, ref moduleComp))
			{
				if (moduleComp.SpatialFarSoundOverride != null)
				{
					farSoundOverride = moduleComp.SpatialFarSoundOverride;
				}
				disableFarSound |= moduleComp.DisableSpatialFarSound;
				audioRangeMultiplier *= moduleComp.SpatialAudioRangeMultiplier;
				nearRangeMultiplier *= moduleComp.SpatialNearRangeMultiplier;
				coneAngleMultiplier *= moduleComp.SpatialConeAngleMultiplier;
				nearVolumeMultiplier *= moduleComp.SpatialNearVolumeMultiplier;
			}
		}
		return new PubgSpatialGunshotModifiers(farSoundOverride, disableFarSound, Math.Clamp(audioRangeMultiplier, 0.2f, 2f), Math.Clamp(nearRangeMultiplier, 0.2f, 2f), Math.Clamp(coneAngleMultiplier, 0.2f, 2f), Math.Clamp(nearVolumeMultiplier, 0.2f, 2f));
	}

	public string GetResolvedContainerId(PubgWeaponModuleSlotDefinition slotDefinition)
	{
		if (!string.IsNullOrWhiteSpace(slotDefinition.ContainerId))
		{
			return slotDefinition.ContainerId;
		}
		return "pubg_module_" + slotDefinition.Slot.ToString().ToLowerInvariant();
	}

	public bool HasInstalledModuleInSlot(EntityUid weapon, PubgModuleSlotType slotType, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return false;
		}
		foreach (var (slot, installed) in EnumerateInstalledModules(weapon, modules))
		{
			if (slot.Slot == slotType && installed.HasValue)
			{
				return true;
			}
		}
		return false;
	}

	public bool TryGetInstalledModule(EntityUid weapon, PubgModuleSlotType slotType, out EntityUid module, out PubgWeaponModuleSlotDefinition slotDefinition, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		module = EntityUid.Invalid;
		slotDefinition = null;
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return false;
		}
		foreach (var (slot, installed) in EnumerateInstalledModules(weapon, modules))
		{
			if (slot.Slot == slotType && installed.HasValue)
			{
				module = installed.Value;
				slotDefinition = slot;
				return true;
			}
		}
		return false;
	}

	public bool HasRequiredModulesForReload(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
		{
			return true;
		}
		foreach (var (slot, installed) in EnumerateInstalledModules(weapon, modules))
		{
			if (slot.RequiredForReloading && !installed.HasValue)
			{
				return false;
			}
		}
		return true;
	}

	public static string GetSlotLocKey(PubgModuleSlotType slot)
	{
		return slot switch
		{
			PubgModuleSlotType.Optic => "pubg-loadout-slot-optic", 
			PubgModuleSlotType.Muzzle => "pubg-loadout-slot-muzzle", 
			PubgModuleSlotType.Underbarrel => "pubg-loadout-slot-underbarrel", 
			PubgModuleSlotType.Tactical => "pubg-loadout-slot-tactical", 
			PubgModuleSlotType.Magazine => "pubg-loadout-slot-magazine", 
			_ => "pubg-loadout-slot-custom", 
		};
	}

	private void OnWeaponGetAmmoSpread(Entity<PubgWeaponModulesComponent> ent, ref GunGetAmmoSpreadEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float spread = GetSpreadMultiplier(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
		PubgFocusViewComponent focusComp = default(PubgFocusViewComponent);
		if (((EntitySystem)this).TryComp<PubgFocusViewComponent>(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ref focusComp) && !focusComp.Active)
		{
			spread *= GetHipfireSpreadMultiplier(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
		}
		args.Spread = Angle.op_Implicit(Angle.op_Implicit(args.Spread) * (double)spread);
	}

	private void OnGunRefreshModifiers(Entity<PubgWeaponModulesComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		args.CameraRecoilScalar *= GetRecoilMultiplier(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
		SoundSpecifier soundOverride = GetGunshotOverrideSound(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
		if (soundOverride != null)
		{
			args.SoundGunshot = soundOverride;
		}
	}

	private void OnGunMuzzleFlashAttempt(Entity<PubgWeaponModulesComponent> ent, ref GunMuzzleFlashAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (ShouldSuppressMuzzleFlash(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp))
		{
			args.Cancelled = true;
		}
	}

	private void OnGetItemActions(Entity<PubgWeaponModulesComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetInstalledFlashlights(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp, out var anyEnabled))
		{
			args.AddAction(ref ent.Comp.FlashlightToggleActionEntity, ent.Comp.FlashlightToggleAction);
			SharedActionsSystem actions = _actions;
			EntityUid? flashlightToggleActionEntity = ent.Comp.FlashlightToggleActionEntity;
			actions.SetToggled(flashlightToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(flashlightToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), anyEnabled);
		}
		PubgBipodComponent bipod = default(PubgBipodComponent);
		if (TryGetInstalledModule(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), PubgModuleSlotType.Underbarrel, out EntityUid module, out PubgWeaponModuleSlotDefinition _, ent.Comp) && ((EntitySystem)this).TryComp<PubgBipodComponent>(module, ref bipod))
		{
			args.AddAction(ref ent.Comp.BipodToggleActionEntity, ent.Comp.BipodToggleAction);
			SharedActionsSystem actions2 = _actions;
			EntityUid? flashlightToggleActionEntity = ent.Comp.BipodToggleActionEntity;
			actions2.SetToggled(flashlightToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(flashlightToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), bipod.Deployed);
		}
	}

	private void OnToggleWeaponLightAction(Entity<PubgWeaponModulesComponent> ent, ref PubgToggleWeaponLightActionEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<HandsComponent>(args.Performer, ref handsComp) || !_hands.IsHolding(Entity<HandsComponent>.op_Implicit((args.Performer, handsComp)), ent.Owner))
		{
			return;
		}
		List<EntityUid> flashlightModules = new List<EntityUid>();
		bool anyEnabled = false;
		UnpoweredFlashlightComponent flashlight = default(UnpoweredFlashlightComponent);
		foreach (var item in EnumerateInstalledModules(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<UnpoweredFlashlightComponent>(installed.Value, ref flashlight))
			{
				flashlightModules.Add(installed.Value);
				anyEnabled |= flashlight.LightOn;
			}
		}
		if (flashlightModules.Count == 0)
		{
			return;
		}
		bool targetState = !anyEnabled;
		foreach (EntityUid module in flashlightModules)
		{
			_flashlight.SetLight(Entity<UnpoweredFlashlightComponent>.op_Implicit((ValueTuple<EntityUid, UnpoweredFlashlightComponent>)(module, null)), targetState, args.Performer);
		}
		SharedActionsSystem actions = _actions;
		EntityUid? flashlightToggleActionEntity = ent.Comp.FlashlightToggleActionEntity;
		actions.SetToggled(flashlightToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(flashlightToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), targetState);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private bool TryGetInstalledFlashlights(EntityUid weapon, PubgWeaponModulesComponent modules, out bool anyEnabled)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		anyEnabled = false;
		bool found = false;
		UnpoweredFlashlightComponent flashlight = default(UnpoweredFlashlightComponent);
		foreach (var item in EnumerateInstalledModules(weapon, modules))
		{
			EntityUid? installed = item.Module;
			if (installed.HasValue && ((EntitySystem)this).TryComp<UnpoweredFlashlightComponent>(installed.Value, ref flashlight))
			{
				found = true;
				anyEnabled |= flashlight.LightOn;
			}
		}
		return found;
	}

	private void OnModuleExamined(Entity<PubgWeaponModuleComponent> ent, ref ExaminedEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-header"));
		string category = base.Loc.GetString(ent.Comp.UiCategoryLocKey);
		args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-category", (ValueTuple<string, object>)("category", category)));
		string slot = base.Loc.GetString(GetSlotLocKey(ent.Comp.Slot));
		args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-slot", (ValueTuple<string, object>)("slot", slot)));
		PubgMagazineModuleAmmoComponent magazineAmmo = default(PubgMagazineModuleAmmoComponent);
		if (((EntitySystem)this).TryComp<PubgMagazineModuleAmmoComponent>(Entity<PubgWeaponModuleComponent>.op_Implicit(ent), ref magazineAmmo) && (magazineAmmo.CurrentAmmo > 0 || magazineAmmo.Capacity > 0))
		{
			args.PushMarkup((magazineAmmo.Capacity > 0) ? base.Loc.GetString("pubg-loadout-ammo-display", (ValueTuple<string, object>)("current", magazineAmmo.CurrentAmmo), (ValueTuple<string, object>)("capacity", magazineAmmo.Capacity)) : base.Loc.GetString("pubg-loadout-ammo-display-current", (ValueTuple<string, object>)("current", magazineAmmo.CurrentAmmo)));
		}
		float spreadDelta = (1f - ent.Comp.SpreadMultiplier) * 100f;
		if (MathF.Abs(spreadDelta) > 0.01f)
		{
			args.PushMarkup((spreadDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-spread-reduce", (ValueTuple<string, object>)("value", MathF.Round(spreadDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-spread-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(spreadDelta), 1))));
		}
		if (MathF.Abs(ent.Comp.FocusBonusTiles) > 0.01f)
		{
			args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-focus", (ValueTuple<string, object>)("value", MathF.Round(ent.Comp.FocusBonusTiles, 1))));
		}
		float recoilDelta = (1f - ent.Comp.RecoilMultiplier) * 100f;
		if (MathF.Abs(recoilDelta) > 0.01f)
		{
			args.PushMarkup((recoilDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-recoil-reduce", (ValueTuple<string, object>)("value", MathF.Round(recoilDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-recoil-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(recoilDelta), 1))));
		}
		float hipfireDelta = (1f - ent.Comp.HipfireSpreadMultiplier) * 100f;
		if (MathF.Abs(hipfireDelta) > 0.01f)
		{
			args.PushMarkup((hipfireDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-hipfire-reduce", (ValueTuple<string, object>)("value", MathF.Round(hipfireDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-hipfire-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(hipfireDelta), 1))));
		}
		float reloadDelta = (1f - ent.Comp.ReloadTimeMultiplier) * 100f;
		if (MathF.Abs(reloadDelta) > 0.01f)
		{
			args.PushMarkup((reloadDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-reload-reduce", (ValueTuple<string, object>)("value", MathF.Round(reloadDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-reload-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(reloadDelta), 1))));
		}
		if (ent.Comp.MagazineCapacityBonus != 0)
		{
			args.PushMarkup((ent.Comp.MagazineCapacityBonus > 0) ? base.Loc.GetString("pubg-loadout-tooltip-module-magazine-bonus", (ValueTuple<string, object>)("value", ent.Comp.MagazineCapacityBonus)) : base.Loc.GetString("pubg-loadout-tooltip-module-magazine-penalty", (ValueTuple<string, object>)("value", Math.Abs(ent.Comp.MagazineCapacityBonus))));
		}
		if (ent.Comp.SoundGunshotOverride != null)
		{
			args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-sound-override"));
		}
		if (ent.Comp.DisableSpatialFarSound)
		{
			args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-far-sound-disabled"));
		}
		float spatialAudioRangeDelta = (1f - ent.Comp.SpatialAudioRangeMultiplier) * 100f;
		if (MathF.Abs(spatialAudioRangeDelta) > 0.01f)
		{
			args.PushMarkup((spatialAudioRangeDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-audio-range-reduce", (ValueTuple<string, object>)("value", MathF.Round(spatialAudioRangeDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-audio-range-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(spatialAudioRangeDelta), 1))));
		}
		float spatialNearRangeDelta = (1f - ent.Comp.SpatialNearRangeMultiplier) * 100f;
		if (MathF.Abs(spatialNearRangeDelta) > 0.01f)
		{
			args.PushMarkup((spatialNearRangeDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-near-range-reduce", (ValueTuple<string, object>)("value", MathF.Round(spatialNearRangeDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-near-range-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(spatialNearRangeDelta), 1))));
		}
		float spatialNearVolumeDelta = (1f - ent.Comp.SpatialNearVolumeMultiplier) * 100f;
		if (MathF.Abs(spatialNearVolumeDelta) > 0.01f)
		{
			args.PushMarkup((spatialNearVolumeDelta >= 0f) ? base.Loc.GetString("pubg-loadout-tooltip-module-near-volume-reduce", (ValueTuple<string, object>)("value", MathF.Round(spatialNearVolumeDelta, 1))) : base.Loc.GetString("pubg-loadout-tooltip-module-near-volume-increase", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(spatialNearVolumeDelta), 1))));
		}
		if (ent.Comp.SuppressMuzzleFlash)
		{
			args.PushMarkup(base.Loc.GetString("pubg-loadout-tooltip-module-muzzle-flash-hidden"));
		}
		AppendCompatibilityInfo(ent, ref args);
	}

	private void AppendCompatibilityInfo(Entity<PubgWeaponModuleComponent> module, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(args.Examiner, ref hands))
		{
			return;
		}
		bool headerShown = false;
		PubgWeaponModulesComponent modulesComp = default(PubgWeaponModulesComponent);
		foreach (string handId in hands.Hands.Keys)
		{
			if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit((args.Examiner, hands)), handId, out var heldItem) && heldItem.HasValue && ((EntitySystem)this).TryComp<PubgWeaponModulesComponent>(heldItem.Value, ref modulesComp))
			{
				if (!headerShown)
				{
					args.PushMarkup(base.Loc.GetString("pubg-modules-examine-fit-header"));
					headerShown = true;
				}
				args.PushMarkup(BuildCompatibilityLine(module, heldItem.Value, modulesComp));
			}
		}
	}

	private string BuildCompatibilityLine(Entity<PubgWeaponModuleComponent> module, EntityUid weapon, PubgWeaponModulesComponent modulesComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		string weaponName = ((EntitySystem)this).Name(weapon, (MetaDataComponent)null);
		PubgWeaponModuleSlotDefinition targetSlot = null;
		foreach (PubgWeaponModuleSlotDefinition slot in modulesComp.Slots)
		{
			if (slot.Slot == module.Comp.Slot && IsModuleAllowedInSlot(module.Owner, slot, module.Comp))
			{
				targetSlot = slot;
				break;
			}
		}
		if (targetSlot == null)
		{
			return base.Loc.GetString("pubg-modules-examine-fit-incompatible", (ValueTuple<string, object>)("weapon", weaponName));
		}
		string slotName = base.Loc.GetString(targetSlot.DisplayNameLocKey);
		string containerId = GetResolvedContainerId(targetSlot);
		BaseContainer baseContainer = default(BaseContainer);
		if (_container.TryGetContainer(weapon, containerId, ref baseContainer, (ContainerManagerComponent)null))
		{
			ContainerSlot slotContainer = (ContainerSlot)(object)((baseContainer is ContainerSlot) ? baseContainer : null);
			if (slotContainer != null)
			{
				EntityUid? containedEntity = slotContainer.ContainedEntity;
				if (containedEntity.HasValue)
				{
					EntityUid occupant = containedEntity.GetValueOrDefault();
					return base.Loc.GetString("pubg-modules-examine-fit-replaces", new(string, object)[3]
					{
						("weapon", weaponName),
						("slot", slotName),
						("module", ((EntitySystem)this).Name(occupant, (MetaDataComponent)null))
					});
				}
			}
		}
		return base.Loc.GetString("pubg-modules-examine-fit-empty", (ValueTuple<string, object>)("weapon", weaponName), (ValueTuple<string, object>)("slot", slotName));
	}

	private void OnWeaponExamined(Entity<PubgWeaponModulesComponent> ent, ref ExaminedEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("PubgWeaponModulesComponent"))
		{
			args.PushMarkup(base.Loc.GetString("pubg-loadout-examine-weapon-header"));
			PubgMagazineModuleAmmoComponent magazineAmmo = default(PubgMagazineModuleAmmoComponent);
			foreach (var item in EnumerateInstalledModules(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp))
			{
				PubgWeaponModuleSlotDefinition slot = item.Slot;
				EntityUid? module = item.Module;
				string slotName = base.Loc.GetString(slot.DisplayNameLocKey);
				string moduleName = (module.HasValue ? ((EntitySystem)this).Name(module.Value, (MetaDataComponent)null) : base.Loc.GetString("pubg-loadout-slot-empty"));
				args.PushMarkup(base.Loc.GetString("pubg-loadout-examine-weapon-module-entry", (ValueTuple<string, object>)("slot", slotName), (ValueTuple<string, object>)("module", moduleName)));
				if (slot.Slot == PubgModuleSlotType.Magazine && slot.StoresAmmo)
				{
					if (module.HasValue && ((EntitySystem)this).TryComp<PubgMagazineModuleAmmoComponent>(module.Value, ref magazineAmmo))
					{
						args.PushMarkup((magazineAmmo.Capacity > 0) ? base.Loc.GetString("pubg-loadout-ammo-display", (ValueTuple<string, object>)("current", magazineAmmo.CurrentAmmo), (ValueTuple<string, object>)("capacity", magazineAmmo.Capacity)) : base.Loc.GetString("pubg-loadout-ammo-display-current", (ValueTuple<string, object>)("current", magazineAmmo.CurrentAmmo)));
					}
					else
					{
						args.PushMarkup(base.Loc.GetString("pubg-loadout-ammo-empty"));
					}
				}
			}
			float spreadMultiplier = GetSpreadMultiplier(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
			float focusBonus = GetFocusBonusTiles(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
			float spreadDelta = (1f - spreadMultiplier) * 100f;
			if (spreadDelta >= 0f)
			{
				args.PushMarkup(base.Loc.GetString("pubg-loadout-examine-weapon-spread-reduce-total", (ValueTuple<string, object>)("value", MathF.Round(spreadDelta, 1))));
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("pubg-loadout-examine-weapon-spread-increase-total", (ValueTuple<string, object>)("value", MathF.Round(MathF.Abs(spreadDelta), 1))));
			}
			args.PushMarkup(base.Loc.GetString("pubg-loadout-examine-weapon-focus-total", (ValueTuple<string, object>)("value", MathF.Round(focusBonus, 1))));
		}
	}
}
