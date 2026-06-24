using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public sealed class PubgLoadoutStateMessage : EntityEventArgs
{
	public List<PubgLoadoutItemState> GroundItems { get; }

	public List<PubgLoadoutItemState> InventoryItems { get; }

	public List<PubgLoadoutWeaponState> Weapons { get; }

	public bool HasRigContainer { get; }

	public bool HasBackpackContainer { get; }

	public int Revision { get; }

	public PubgLoadoutStateMessage(List<PubgLoadoutItemState> groundItems, List<PubgLoadoutItemState> inventoryItems, List<PubgLoadoutWeaponState> weapons, bool hasRigContainer, bool hasBackpackContainer, int revision)
	{
		GroundItems = groundItems;
		InventoryItems = inventoryItems;
		Weapons = weapons;
		HasRigContainer = hasRigContainer;
		HasBackpackContainer = hasBackpackContainer;
		Revision = revision;
	}
}
