using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Survivor;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Content.Shared.Station;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared._RMC14.Item;

public sealed class ItemCamouflageSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedStationSpawningSystem _stationSpawning;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private INetManager _net;

	private readonly Queue<Entity<ItemCamouflageComponent>> _items = new Queue<Entity<ItemCamouflageComponent>>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public CamouflageType CurrentMapCamouflage { get; set; } = CamouflageType.Jungle;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PlayerSpawnCompleteEvent>((EntityEventHandler<PlayerSpawnCompleteEvent>)OnPlayerSpawnComplete, (Type[])null, new Type[1] { typeof(SurvivorSystem) });
		((EntitySystem)this).SubscribeLocalEvent<ItemCamouflageComponent, MapInitEvent>((EntityEventRefHandler<ItemCamouflageComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (ev.JobId != null)
		{
			EntityUid mob = ev.Mob;
			JobPrototype jobPrototype = default(JobPrototype);
			if (_prototypes.TryIndex<JobPrototype>(ev.JobId, ref jobPrototype) && jobPrototype.CamouflageStartingGear != null && jobPrototype.CamouflageStartingGear.TryGetValue(CurrentMapCamouflage, out ProtoId<StartingGearPrototype> startingGear))
			{
				StartingGearPrototype gearProto = default(StartingGearPrototype);
				_prototypes.TryIndex<StartingGearPrototype>(startingGear, ref gearProto);
				EquipMapCamoGear(mob, gearProto);
			}
		}
	}

	private void EquipMapCamoGear(EntityUid mob, IEquipmentLoadout? startingGear)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (startingGear == null)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(mob));
		EntityUid item;
		SlotDefinition slot;
		while (slots.NextItem(out item, out slot))
		{
			if (!string.IsNullOrEmpty(startingGear.GetGear(slot.Name)))
			{
				_inventory.TryUnequip(mob, slot.Name, silent: true, force: true);
				((EntitySystem)this).QueueDel((EntityUid?)item);
			}
		}
		_stationSpawning.EquipStartingGear(mob, startingGear);
	}

	private void OnMapInit(Entity<ItemCamouflageComponent> ent, ref MapInitEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			_items.Enqueue(ent);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (_items.Count != 0)
		{
			Entity<ItemCamouflageComponent> ent;
			while (_items.TryDequeue(out ent))
			{
				_appearance.SetData(Entity<ItemCamouflageComponent>.op_Implicit(ent), (Enum)ItemCamouflageVisuals.Camo, (object)CurrentMapCamouflage, (AppearanceComponent)null);
			}
		}
	}
}
