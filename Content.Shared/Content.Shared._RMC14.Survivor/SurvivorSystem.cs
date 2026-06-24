using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Storage;
using Content.Shared.GameTicking;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Survivor;

public sealed class SurvivorSystem : EntitySystem
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private RMCStorageSystem _rmcStorage;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedHandsSystem _hands;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EquipSurvivorPresetComponent, PlayerSpawnCompleteEvent>((EntityEventRefHandler<EquipSurvivorPresetComponent, PlayerSpawnCompleteEvent>)OnPresetPlayerSpawnComplete, (Type[])null, new Type[1] { typeof(CMArmorSystem) });
	}

	private void OnPresetPlayerSpawnComplete(Entity<EquipSurvivorPresetComponent> ent, ref PlayerSpawnCompleteEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ApplyPreset(Entity<EquipSurvivorPresetComponent>.op_Implicit(ent), ent.Comp.Preset);
	}

	private void ApplyPreset(EntityUid mob, EntProtoId<SurvivorPresetComponent> preset)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		SurvivorPresetComponent comp = default(SurvivorPresetComponent);
		if (!preset.TryGet(ref comp, _prototypes, _compFactory))
		{
			return;
		}
		if (comp.RandomStartingGear.Count > 0)
		{
			foreach (KeyValuePair<string, List<EntProtoId>> item6 in comp.RandomStartingGear)
			{
				item6.Deconstruct(out var key, out var value);
				string slot = key;
				List<EntProtoId> itemList = value;
				EntProtoId randomItem = RandomExtensions.Pick<EntProtoId>(_random, (IReadOnlyList<EntProtoId>)itemList);
				Equip(mob, randomItem, tryStorage: false, tryInHand: false, tryEquip: true, slot);
			}
		}
		if (comp.RandomOutfits.Count > 0)
		{
			foreach (EntProtoId item in RandomExtensions.Pick<List<EntProtoId>>(_random, (IReadOnlyList<List<EntProtoId>>)comp.RandomOutfits))
			{
				Equip(mob, item);
			}
		}
		if (RandomExtensions.Prob(_random, comp.PrimaryWeaponChance) && comp.PrimaryWeapons.Count > 0)
		{
			foreach (EntProtoId item2 in RandomExtensions.Pick<List<EntProtoId>>(_random, (IReadOnlyList<List<EntProtoId>>)comp.PrimaryWeapons))
			{
				Equip(mob, item2, tryStorage: true, tryInHand: true);
			}
		}
		if (comp.RandomWeapon.Count > 0)
		{
			foreach (EntProtoId item3 in RandomExtensions.Pick<List<EntProtoId>>(_random, (IReadOnlyList<List<EntProtoId>>)comp.RandomWeapon))
			{
				Equip(mob, item3, tryStorage: true, tryInHand: false, comp.TryEquipRandomWeapon);
			}
		}
		if (comp.RandomGear.Count > 0)
		{
			foreach (EntProtoId item4 in RandomExtensions.Pick<List<EntProtoId>>(_random, (IReadOnlyList<List<EntProtoId>>)comp.RandomGear))
			{
				Equip(mob, item4, tryStorage: true, tryInHand: true, tryEquip: false);
			}
		}
		if (comp.RandomGearOther.Count > 0)
		{
			foreach (List<List<EntProtoId>> other in comp.RandomGearOther)
			{
				if (other.Count == 0)
				{
					continue;
				}
				foreach (EntProtoId item5 in RandomExtensions.Pick<List<EntProtoId>>(_random, (IReadOnlyList<List<EntProtoId>>)other))
				{
					Equip(mob, item5, tryStorage: true, tryInHand: false, comp.TryEquipRandomOtherGear);
				}
			}
		}
		int rareItemNumber = _random.Next(1, comp.RareItemCoefficent);
		if (comp.RareItems.Count <= 0)
		{
			return;
		}
		foreach (var (entity, chance) in comp.RareItems)
		{
			if (rareItemNumber >= chance.Item1 && rareItemNumber <= chance.Item2)
			{
				Equip(mob, entity, tryStorage: true, tryInHand: true);
				break;
			}
		}
	}

	private void Equip(EntityUid mob, EntProtoId toSpawn, bool tryStorage = true, bool tryInHand = false, bool tryEquip = true, string? slotName = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(mob);
		EntityUid spawn = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(toSpawn), coordinates);
		if (tryEquip)
		{
			InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(mob));
			ContainerSlot slot;
			while (slots.MoveNext(out slot))
			{
				if ((slotName == null || !(((BaseContainer)slot).ID != slotName)) && !slot.ContainedEntity.HasValue && _inventory.TryEquip(mob, spawn, ((BaseContainer)slot).ID, silent: true))
				{
					return;
				}
			}
		}
		if ((!tryStorage || !TryInsertItemInStorage(mob, spawn)) && (!tryInHand || !_hands.TryPickupAnyHand(mob, spawn)))
		{
			((EntitySystem)this).Log.Warning($"Couldn't equip {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(spawn))} on {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mob))}");
			((EntitySystem)this).QueueDel((EntityUid?)spawn);
		}
	}

	public bool TryInsertItemInStorage(EntityUid mob, EntityUid toInsert)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(mob), SlotFlags.BACK);
		ContainerSlot slot;
		StorageComponent storage = default(StorageComponent);
		while (slots.MoveNext(out slot))
		{
			EntityUid? stackedEntity = slot.ContainedEntity;
			if (!stackedEntity.HasValue)
			{
				continue;
			}
			EntityUid storageItem = stackedEntity.GetValueOrDefault();
			if (((EntitySystem)this).TryComp<StorageComponent>(storageItem, ref storage))
			{
				if (!_rmcStorage.CanInsertStoreSkill(Entity<StorageComponent, StorageStoreSkillRequiredComponent>.op_Implicit(storageItem), toInsert, mob, out var _))
				{
					return false;
				}
				SharedStorageSystem storage2 = _storage;
				StorageComponent storageComp = storage;
				if (storage2.Insert(storageItem, toInsert, out stackedEntity, null, storageComp, playSound: false))
				{
					return true;
				}
			}
		}
		return false;
	}
}
