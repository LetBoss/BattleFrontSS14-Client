using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Storage;
using Content.Shared.Dataset;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Station;

public abstract class SharedStationSpawningSystem : EntitySystem
{
	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	protected InventorySystem InventorySystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private MetaDataSystem _metadata;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private SharedTransformSystem _xformSystem;

	private EntityQuery<HandsComponent> _handsQuery;

	private EntityQuery<InventoryComponent> _inventoryQuery;

	private EntityQuery<StorageComponent> _storageQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_handsQuery = ((EntitySystem)this).GetEntityQuery<HandsComponent>();
		_inventoryQuery = ((EntitySystem)this).GetEntityQuery<InventoryComponent>();
		_storageQuery = ((EntitySystem)this).GetEntityQuery<StorageComponent>();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
	}

	public void EquipRoleLoadout(EntityUid entity, RoleLoadout loadout, RoleLoadoutPrototype roleProto)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		LoadoutPrototype loadoutProto = default(LoadoutPrototype);
		foreach (KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> item in loadout.SelectedLoadouts.OrderBy<KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>>, int>((KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> x) => roleProto.Groups.FindIndex((ProtoId<LoadoutGroupPrototype> e) => e == x.Key)))
		{
			foreach (Loadout items in item.Value)
			{
				if (!PrototypeManager.TryIndex<LoadoutPrototype>(items.Prototype, ref loadoutProto))
				{
					((EntitySystem)this).Log.Error($"Unable to find loadout prototype for {items.Prototype}");
				}
				else
				{
					EquipStartingGear(entity, loadoutProto, raiseEvent: false);
				}
			}
		}
		EquipRoleName(entity, loadout, roleProto);
	}

	public void EquipRoleName(EntityUid entity, RoleLoadout loadout, RoleLoadoutPrototype roleProto)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		string name = null;
		if (roleProto.CanCustomizeName)
		{
			name = loadout.EntityName;
		}
		LocalizedDatasetPrototype nameData = default(LocalizedDatasetPrototype);
		if (string.IsNullOrEmpty(name) && PrototypeManager.TryIndex<LocalizedDatasetPrototype>(roleProto.NameDataset, ref nameData))
		{
			name = base.Loc.GetString(RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)nameData.Values));
		}
		if (!string.IsNullOrEmpty(name))
		{
			_metadata.SetEntityName(entity, name, (MetaDataComponent)null, true);
		}
	}

	public void EquipStartingGear(EntityUid entity, LoadoutPrototype loadout, bool raiseEvent = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		EquipStartingGear(entity, loadout.StartingGear, raiseEvent);
		EquipStartingGear(entity, (IEquipmentLoadout?)loadout, raiseEvent);
	}

	public void EquipStartingGear(EntityUid entity, ProtoId<StartingGearPrototype>? startingGear, bool raiseEvent = true)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		StartingGearPrototype gearProto = default(StartingGearPrototype);
		PrototypeManager.TryIndex<StartingGearPrototype>(startingGear, ref gearProto);
		EquipStartingGear(entity, gearProto, raiseEvent);
	}

	public void EquipStartingGear(EntityUid entity, StartingGearPrototype? startingGear, bool raiseEvent = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		EquipStartingGear(entity, (IEquipmentLoadout?)startingGear, raiseEvent);
	}

	public void EquipStartingGear(EntityUid entity, IEquipmentLoadout? startingGear, bool raiseEvent = true)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		if (startingGear == null)
		{
			return;
		}
		TransformComponent xform = _xformQuery.GetComponent(entity);
		if (InventorySystem.TryGetSlots(entity, out SlotDefinition[] slotDefinitions))
		{
			SlotDefinition[] array = slotDefinitions;
			foreach (SlotDefinition slot in array)
			{
				string equipmentStr = startingGear.GetGear(slot.Name);
				if (!string.IsNullOrEmpty(equipmentStr))
				{
					EntityUid equipmentEntity = ((EntitySystem)this).Spawn(equipmentStr, xform.Coordinates);
					InventorySystem.TryEquip(entity, equipmentEntity, slot.Name, silent: true, force: true);
				}
			}
		}
		HandsComponent handsComponent = default(HandsComponent);
		if (_handsQuery.TryComp(entity, ref handsComponent))
		{
			List<EntProtoId> inhand = startingGear.Inhand;
			EntityCoordinates coords = xform.Coordinates;
			foreach (EntProtoId prototype in inhand)
			{
				EntityUid inhandEntity = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(prototype), coords);
				if (_handsSystem.TryGetEmptyHand(Entity<HandsComponent>.op_Implicit((entity, handsComponent)), out string emptyHand))
				{
					_handsSystem.TryPickup(entity, inhandEntity, emptyHand, checkActionBlocker: false, animateUser: false, animate: true, handsComponent);
				}
			}
		}
		if (startingGear.Storage.Count > 0)
		{
			MapCoordinates coords2 = _xformSystem.GetMapCoordinates(entity, (TransformComponent)null);
			InventoryComponent inventoryComp = default(InventoryComponent);
			_inventoryQuery.TryComp(entity, ref inventoryComp);
			StorageComponent storage = default(StorageComponent);
			ItemComponent item = default(ItemComponent);
			foreach (var (slotName, entProtos) in startingGear.Storage)
			{
				if (entProtos == null || entProtos.Count == 0 || inventoryComp == null || !InventorySystem.TryGetSlotEntity(entity, slotName, out var slotEnt, inventoryComp) || !_storageQuery.TryComp(slotEnt, ref storage))
				{
					continue;
				}
				foreach (EntProtoId entProto in entProtos)
				{
					EntityUid spawnedEntity = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(entProto), coords2, (ComponentRegistry)null, default(Angle));
					if (((EntitySystem)this).TryComp<ItemComponent>(spawnedEntity, ref item))
					{
						CMStorageItemFillEvent ev = new CMStorageItemFillEvent(Entity<ItemComponent>.op_Implicit((spawnedEntity, item)), storage);
						((EntitySystem)this).RaiseLocalEvent<CMStorageItemFillEvent>(slotEnt.Value, ref ev, false);
					}
					SharedStorageSystem storage2 = _storage;
					EntityUid value = slotEnt.Value;
					StorageComponent storageComp = storage;
					storage2.Insert(value, spawnedEntity, out var _, null, storageComp, playSound: false);
				}
			}
		}
		if (raiseEvent)
		{
			StartingGearEquippedEvent ev2 = new StartingGearEquippedEvent(entity);
			((EntitySystem)this).RaiseLocalEvent<StartingGearEquippedEvent>(entity, ref ev2, false);
		}
	}

	public string? GetGearForSlot(RoleLoadout? loadout, string slot)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (loadout == null)
		{
			return null;
		}
		LoadoutPrototype loadoutPrototype = default(LoadoutPrototype);
		foreach (KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> selectedLoadout in loadout.SelectedLoadouts)
		{
			foreach (Loadout items in selectedLoadout.Value)
			{
				if (!PrototypeManager.TryIndex<LoadoutPrototype>(items.Prototype, ref loadoutPrototype))
				{
					return null;
				}
				string gear = ((IEquipmentLoadout)loadoutPrototype).GetGear(slot);
				if (gear != string.Empty)
				{
					return gear;
				}
			}
		}
		return null;
	}
}
