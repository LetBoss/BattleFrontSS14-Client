using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._CIV14merka.Loadout;

public static class CivLoadoutManifest
{
	private static readonly HashSet<string> WeaponSlots = new HashSet<string> { "suitstorage", "back" };

	public static List<CivLoadoutItemInfo> GetRemovableItems(StartingGearPrototype gear, IPrototypeManager prototypes, IComponentFactory componentFactory, HashSet<string>? swapSlots = null)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		List<CivLoadoutItemInfo> result = new List<CivLoadoutItemInfo>();
		EntityPrototype proto = default(EntityPrototype);
		StorageFillComponent fill = default(StorageFillComponent);
		foreach (KeyValuePair<string, EntProtoId> item in gear.Equipment)
		{
			var (slot, entProto) = (KeyValuePair<string, EntProtoId>)(ref item);
			if (string.IsNullOrEmpty(((EntProtoId)(ref entProto)).Id) || !prototypes.TryIndex<EntityPrototype>(((EntProtoId)(ref entProto)).Id, ref proto))
			{
				continue;
			}
			if (proto.TryGetComponent<StorageFillComponent>(ref fill, componentFactory))
			{
				Dictionary<string, int> counts = new Dictionary<string, int>();
				List<string> order = new List<string>();
				foreach (EntitySpawnEntry entry in fill.Contents)
				{
					EntProtoId? prototypeId = entry.PrototypeId;
					if (!prototypeId.HasValue)
					{
						continue;
					}
					EntProtoId id = prototypeId.GetValueOrDefault();
					if (!string.IsNullOrEmpty(((EntProtoId)(ref id)).Id))
					{
						int amount = Math.Max(1, entry.Amount);
						if (counts.TryGetValue(((EntProtoId)(ref id)).Id, out var existing))
						{
							counts[((EntProtoId)(ref id)).Id] = existing + amount;
							continue;
						}
						counts[((EntProtoId)(ref id)).Id] = amount;
						order.Add(((EntProtoId)(ref id)).Id);
					}
				}
				foreach (string protoId in order)
				{
					result.Add(new CivLoadoutItemInfo(slot, protoId, counts[protoId]));
				}
			}
			else if (WeaponSlots.Contains(slot) && (swapSlots == null || !swapSlots.Contains(slot)))
			{
				result.Add(new CivLoadoutItemInfo(slot, ((EntProtoId)(ref entProto)).Id, 1));
			}
		}
		return result;
	}

	public static Dictionary<string, List<string>> GetSlotOptions(string faction, CivTdmClass cls, IPrototypeManager prototypes)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
		foreach (CivLoadoutOptionsPrototype opt in prototypes.EnumeratePrototypes<CivLoadoutOptionsPrototype>())
		{
			if (opt.Faction != faction || opt.Class != cls)
			{
				continue;
			}
			foreach (var (slot, protos) in opt.Slots)
			{
				if (protos.Count < 2)
				{
					continue;
				}
				if (!result.TryGetValue(slot, out var list2))
				{
					list2 = (result[slot] = new List<string>());
				}
				foreach (EntProtoId item in protos)
				{
					EntProtoId proto = item;
					if (!string.IsNullOrEmpty(((EntProtoId)(ref proto)).Id) && !list2.Contains(((EntProtoId)(ref proto)).Id))
					{
						list2.Add(((EntProtoId)(ref proto)).Id);
					}
				}
			}
		}
		return result;
	}

	public static Dictionary<string, List<string>> GetEffectiveSwaps(StartingGearPrototype gear, string faction, CivTdmClass cls, IReadOnlySet<string> owned, IPrototypeManager prototypes, IComponentFactory componentFactory)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
		string key;
		foreach (KeyValuePair<string, List<string>> slotOption in GetSlotOptions(faction, cls, prototypes))
		{
			slotOption.Deconstruct(out key, out var value);
			string slot = key;
			List<string> opts = value;
			List<string> visible = new List<string>();
			for (int i = 0; i < opts.Count; i++)
			{
				if (i == 0 || owned.Contains(opts[i]))
				{
					visible.Add(opts[i]);
				}
			}
			if (visible.Count >= 2)
			{
				result[slot] = visible;
			}
		}
		EntityPrototype baseProto = default(EntityPrototype);
		ClothingComponent baseClothing = default(ClothingComponent);
		EntityPrototype ownedEnt = default(EntityPrototype);
		ClothingComponent ownedClothing = default(ClothingComponent);
		foreach (KeyValuePair<string, EntProtoId> item in gear.Equipment)
		{
			item.Deconstruct(out key, out var value2);
			string slot2 = key;
			EntProtoId entProto = value2;
			if (string.IsNullOrEmpty(((EntProtoId)(ref entProto)).Id) || !prototypes.TryIndex<EntityPrototype>(((EntProtoId)(ref entProto)).Id, ref baseProto) || !baseProto.TryGetComponent<ClothingComponent>(ref baseClothing, componentFactory) || baseClothing.Slots == SlotFlags.NONE)
			{
				continue;
			}
			foreach (string ownedProto in owned)
			{
				if (!(ownedProto == ((EntProtoId)(ref entProto)).Id) && prototypes.TryIndex<EntityPrototype>(ownedProto, ref ownedEnt) && ownedEnt.TryGetComponent<ClothingComponent>(ref ownedClothing, componentFactory) && (ownedClothing.Slots & baseClothing.Slots) != SlotFlags.NONE)
				{
					if (!result.TryGetValue(slot2, out var list))
					{
						list = (result[slot2] = new List<string> { ((EntProtoId)(ref entProto)).Id });
					}
					if (!list.Contains(ownedProto))
					{
						list.Add(ownedProto);
					}
				}
			}
		}
		return result;
	}
}
