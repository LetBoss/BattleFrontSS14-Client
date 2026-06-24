using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public sealed class PubgLoadoutWeaponState
{
	public NetEntity Entity { get; }

	public string Name { get; }

	public List<PubgLoadoutWeaponSlotState> Slots { get; }

	public PubgWeaponStats Stats { get; }

	public PubgLoadoutWeaponState(NetEntity entity, string name, List<PubgLoadoutWeaponSlotState> slots, PubgWeaponStats stats)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Name = name;
		Slots = slots;
		Stats = stats;
	}
}
