using System;
using System.Collections.Generic;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleHardpointAmmoSystem : EntitySystem
{
	[Dependency]
	private readonly SharedGunSystem _gun;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointAmmoComponent, TakeAmmoEvent>((EntityEventRefHandler<VehicleHardpointAmmoComponent, TakeAmmoEvent>)OnTakeAmmo, (Type[])null, new Type[1] { typeof(SharedGunSystem) });
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointAmmoComponent, OnEmptyGunShotEvent>((EntityEventRefHandler<VehicleHardpointAmmoComponent, OnEmptyGunShotEvent>)OnEmptyGunShot, (Type[])null, (Type[])null);
	}

	private void OnTakeAmmo(Entity<VehicleHardpointAmmoComponent> ent, ref TakeAmmoEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		BallisticAmmoProviderComponent ammo = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(ent.Owner, ref ammo) && ammo.Count <= 0)
		{
			NormalizeAmmoQueue(ent, ammo);
		}
	}

	private void OnEmptyGunShot(Entity<VehicleHardpointAmmoComponent> ent, ref OnEmptyGunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		BallisticAmmoProviderComponent ammo = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(ent.Owner, ref ammo) && ammo.Count <= 0)
		{
			NormalizeAmmoQueue(ent, ammo);
		}
	}

	public bool NormalizeAmmoQueue(Entity<VehicleHardpointAmmoComponent> ent, BallisticAmmoProviderComponent ammo)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (ammo.Count > 0)
		{
			return false;
		}
		return TryChamberNextMagazine(ent, ammo);
	}

	public bool TryChamberNextMagazine(Entity<VehicleHardpointAmmoComponent> ent, BallisticAmmoProviderComponent ammo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		int magazineSize = GetMagazineSize(ent.Comp, ammo);
		IReadOnlyList<int> slots = GetStoredRoundSlots(ent.Comp, magazineSize);
		int reserveSlot = -1;
		for (int i = 0; i < slots.Count; i++)
		{
			if (slots[i] > 0)
			{
				reserveSlot = i;
				break;
			}
		}
		if (reserveSlot < 0)
		{
			return false;
		}
		int chamberSize = Math.Min(magazineSize, slots[reserveSlot]);
		int[] updatedSlots = new int[slots.Count];
		for (int j = 0; j < slots.Count; j++)
		{
			updatedSlots[j] = slots[j];
		}
		updatedSlots[reserveSlot] -= chamberSize;
		CompactStoredRoundSlots(ent, updatedSlots, magazineSize);
		_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((ent.Owner, ammo)), chamberSize);
		RaiseAmmoChanged(ent.Owner);
		return true;
	}

	public List<VehicleAmmoSlotState> GetAmmoQueueSlots(VehicleHardpointAmmoComponent hardpointAmmo, BallisticAmmoProviderComponent ammo)
	{
		int magazineSize = GetMagazineSize(hardpointAmmo, ammo);
		List<VehicleAmmoSlotState> entries = new List<VehicleAmmoSlotState>(Math.Max(1, hardpointAmmo.MaxStoredMagazines));
		entries.Add(new VehicleAmmoSlotState(0, Math.Clamp(ammo.Count, 0, magazineSize), magazineSize, IsReadySlot: true));
		IReadOnlyList<int> reserveSlots = GetStoredRoundSlots(hardpointAmmo, magazineSize);
		for (int i = 0; i < reserveSlots.Count; i++)
		{
			entries.Add(new VehicleAmmoSlotState(i + 1, reserveSlots[i], magazineSize, IsReadySlot: false));
		}
		return entries;
	}

	public bool HasLoadSpace(VehicleHardpointAmmoComponent hardpointAmmo, BallisticAmmoProviderComponent ammo, int ammoSlot)
	{
		if (ammoSlot < 0)
		{
			return false;
		}
		int magazineSize = GetMagazineSize(hardpointAmmo, ammo);
		if (ammoSlot == 0)
		{
			return Math.Clamp(ammo.Count, 0, magazineSize) < magazineSize;
		}
		int reserveSlot = ammoSlot - 1;
		if (reserveSlot >= GetMaxStoredRoundSlots(hardpointAmmo))
		{
			return false;
		}
		return GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize) < magazineSize;
	}

	public bool HasUnloadRounds(VehicleHardpointAmmoComponent hardpointAmmo, BallisticAmmoProviderComponent ammo, int ammoSlot)
	{
		if (ammoSlot < 0)
		{
			return false;
		}
		int magazineSize = GetMagazineSize(hardpointAmmo, ammo);
		if (ammoSlot == 0)
		{
			return Math.Min(Math.Clamp(ammo.Count, 0, magazineSize), ammo.UnspawnedCount) > 0;
		}
		int reserveSlot = ammoSlot - 1;
		if (reserveSlot >= GetMaxStoredRoundSlots(hardpointAmmo))
		{
			return false;
		}
		return GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize) > 0;
	}

	public int GetLoadAmount(VehicleHardpointAmmoComponent hardpointAmmo, BallisticAmmoProviderComponent ammo, int ammoSlot, int availableRounds)
	{
		if (ammoSlot < 0 || availableRounds <= 0)
		{
			return 0;
		}
		int magazineSize = GetMagazineSize(hardpointAmmo, ammo);
		if (ammoSlot == 0)
		{
			int chambered = Math.Clamp(ammo.Count, 0, magazineSize);
			int chamberSpace = magazineSize - chambered;
			if (chamberSpace > 0)
			{
				return Math.Min(availableRounds, chamberSpace);
			}
			return 0;
		}
		int reserveSlot = ammoSlot - 1;
		if (reserveSlot >= GetMaxStoredRoundSlots(hardpointAmmo))
		{
			return 0;
		}
		int storedRounds = GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize);
		int reserveSpace = magazineSize - storedRounds;
		if (reserveSpace > 0)
		{
			return Math.Min(availableRounds, reserveSpace);
		}
		return 0;
	}

	public int GetUnloadAmount(VehicleHardpointAmmoComponent hardpointAmmo, BallisticAmmoProviderComponent ammo, int ammoSlot)
	{
		if (ammoSlot < 0)
		{
			return 0;
		}
		int magazineSize = GetMagazineSize(hardpointAmmo, ammo);
		if (ammoSlot == 0)
		{
			return Math.Min(Math.Clamp(ammo.Count, 0, magazineSize), ammo.UnspawnedCount);
		}
		int reserveSlot = ammoSlot - 1;
		if (reserveSlot >= GetMaxStoredRoundSlots(hardpointAmmo))
		{
			return 0;
		}
		return GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize);
	}

	public bool TryLoadIntoSlot(Entity<VehicleHardpointAmmoComponent> ent, BallisticAmmoProviderComponent ammo, int ammoSlot, int rounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		int amount = GetLoadAmount(ent.Comp, ammo, ammoSlot, rounds);
		if (amount <= 0)
		{
			return false;
		}
		int magazineSize = GetMagazineSize(ent.Comp, ammo);
		if (ammoSlot == 0)
		{
			_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((ent.Owner, ammo)), ammo.UnspawnedCount + amount);
			RaiseAmmoChanged(ent.Owner);
		}
		else
		{
			int reserveSlot = ammoSlot - 1;
			int storedRounds = GetStoredSlotRounds(ent.Comp, reserveSlot, magazineSize);
			SetStoredSlotRounds(ent, reserveSlot, storedRounds + amount, magazineSize);
		}
		return true;
	}

	public int TryUnloadFromSlot(Entity<VehicleHardpointAmmoComponent> ent, BallisticAmmoProviderComponent ammo, int ammoSlot, int maxRounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		int amount = Math.Min(GetUnloadAmount(ent.Comp, ammo, ammoSlot), Math.Max(0, maxRounds));
		if (amount <= 0)
		{
			return 0;
		}
		int magazineSize = GetMagazineSize(ent.Comp, ammo);
		if (ammoSlot == 0)
		{
			_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((ent.Owner, ammo)), ammo.UnspawnedCount - amount);
			NormalizeAmmoQueue(ent, ammo);
			RaiseAmmoChanged(ent.Owner);
		}
		else
		{
			int reserveSlot = ammoSlot - 1;
			int storedRounds = GetStoredSlotRounds(ent.Comp, reserveSlot, magazineSize);
			SetStoredSlotRounds(ent, reserveSlot, storedRounds - amount, magazineSize);
		}
		return amount;
	}

	public int GetMagazineSize(VehicleHardpointAmmoComponent hardpointAmmo, BallisticAmmoProviderComponent ammo)
	{
		int val = Math.Max(1, hardpointAmmo.MagazineSize);
		int capacity = Math.Max(1, ammo.Capacity);
		return Math.Min(val, capacity);
	}

	public int GetStoredRounds(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
	{
		if (hardpointAmmo.StoredRoundSlots.Count > 0)
		{
			int total = 0;
			int slots = Math.Min(hardpointAmmo.StoredRoundSlots.Count, GetMaxStoredRoundSlots(hardpointAmmo));
			for (int i = 0; i < slots; i++)
			{
				total += Math.Clamp(hardpointAmmo.StoredRoundSlots[i], 0, Math.Max(1, magazineSize));
			}
			return total;
		}
		return GetStoredRoundsFallback(hardpointAmmo, magazineSize);
	}

	public int GetMaxStoredRounds(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
	{
		return GetMaxStoredRoundSlots(hardpointAmmo) * Math.Max(1, magazineSize);
	}

	public int GetMaxStoredRoundSlots(VehicleHardpointAmmoComponent hardpointAmmo)
	{
		return Math.Max(0, hardpointAmmo.MaxStoredMagazines - 1);
	}

	public IReadOnlyList<int> GetStoredRoundSlots(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
	{
		int maxSlots = GetMaxStoredRoundSlots(hardpointAmmo);
		int capacity = Math.Max(1, magazineSize);
		int[] slots = new int[maxSlots];
		if (hardpointAmmo.StoredRoundSlots.Count > 0)
		{
			int copy = Math.Min(maxSlots, hardpointAmmo.StoredRoundSlots.Count);
			for (int i = 0; i < copy; i++)
			{
				slots[i] = Math.Clamp(hardpointAmmo.StoredRoundSlots[i], 0, capacity);
			}
			return slots;
		}
		int remaining = Math.Clamp(GetStoredRoundsFallback(hardpointAmmo, magazineSize), 0, maxSlots * capacity);
		for (int j = 0; j < maxSlots; j++)
		{
			if (remaining <= 0)
			{
				break;
			}
			slots[j] = Math.Min(capacity, remaining);
			remaining -= slots[j];
		}
		return slots;
	}

	public int GetStoredSlotRounds(VehicleHardpointAmmoComponent hardpointAmmo, int reserveSlot, int magazineSize)
	{
		IReadOnlyList<int> slots = GetStoredRoundSlots(hardpointAmmo, magazineSize);
		if (reserveSlot >= 0 && reserveSlot < slots.Count)
		{
			return slots[reserveSlot];
		}
		return 0;
	}

	public void SetStoredRounds(Entity<VehicleHardpointAmmoComponent> ent, int rounds, int magazineSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		int maxRounds = GetMaxStoredRounds(ent.Comp, magazineSize);
		int capacity = Math.Max(1, magazineSize);
		int remaining = Math.Clamp(rounds, 0, maxRounds);
		ent.Comp.StoredRoundSlots.Clear();
		for (int i = 0; i < GetMaxStoredRoundSlots(ent.Comp); i++)
		{
			int slotRounds = Math.Min(capacity, remaining);
			ent.Comp.StoredRoundSlots.Add(slotRounds);
			remaining -= slotRounds;
		}
		UpdateStoredRoundTotals(ent.Comp, capacity);
		((EntitySystem)this).Dirty<VehicleHardpointAmmoComponent>(ent, (MetaDataComponent)null);
		RaiseAmmoChanged(ent.Owner);
	}

	public void SetStoredSlotRounds(Entity<VehicleHardpointAmmoComponent> ent, int reserveSlot, int rounds, int magazineSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		int maxSlots = GetMaxStoredRoundSlots(ent.Comp);
		if (reserveSlot < 0 || reserveSlot >= maxSlots)
		{
			return;
		}
		int capacity = Math.Max(1, magazineSize);
		IReadOnlyList<int> slots = GetStoredRoundSlots(ent.Comp, capacity);
		ent.Comp.StoredRoundSlots.Clear();
		for (int i = 0; i < maxSlots; i++)
		{
			int slotRounds = ((i < slots.Count) ? slots[i] : 0);
			if (i == reserveSlot)
			{
				slotRounds = rounds;
			}
			ent.Comp.StoredRoundSlots.Add(Math.Clamp(slotRounds, 0, capacity));
		}
		UpdateStoredRoundTotals(ent.Comp, capacity);
		((EntitySystem)this).Dirty<VehicleHardpointAmmoComponent>(ent, (MetaDataComponent)null);
		RaiseAmmoChanged(ent.Owner);
	}

	private static int GetStoredRoundsFallback(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
	{
		if (hardpointAmmo.StoredRounds > 0)
		{
			return hardpointAmmo.StoredRounds;
		}
		return Math.Max(0, hardpointAmmo.StoredMagazines) * Math.Max(1, magazineSize);
	}

	private void CompactStoredRoundSlots(Entity<VehicleHardpointAmmoComponent> ent, IReadOnlyList<int> slots, int magazineSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		int maxSlots = GetMaxStoredRoundSlots(ent.Comp);
		int capacity = Math.Max(1, magazineSize);
		ent.Comp.StoredRoundSlots.Clear();
		foreach (int rounds in slots)
		{
			if (ent.Comp.StoredRoundSlots.Count >= maxSlots)
			{
				break;
			}
			int clamped = Math.Clamp(rounds, 0, capacity);
			if (clamped > 0)
			{
				ent.Comp.StoredRoundSlots.Add(clamped);
			}
		}
		while (ent.Comp.StoredRoundSlots.Count < maxSlots)
		{
			ent.Comp.StoredRoundSlots.Add(0);
		}
		UpdateStoredRoundTotals(ent.Comp, capacity);
		((EntitySystem)this).Dirty<VehicleHardpointAmmoComponent>(ent, (MetaDataComponent)null);
		RaiseAmmoChanged(ent.Owner);
	}

	private void RaiseAmmoChanged(EntityUid ammoProvider)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		VehicleAmmoChangedEvent ev = new VehicleAmmoChangedEvent(ammoProvider);
		((EntitySystem)this).RaiseLocalEvent<VehicleAmmoChangedEvent>(ammoProvider, ev, true);
	}

	private static void UpdateStoredRoundTotals(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
	{
		int total = 0;
		foreach (int rounds in hardpointAmmo.StoredRoundSlots)
		{
			total += Math.Clamp(rounds, 0, magazineSize);
		}
		hardpointAmmo.StoredRounds = total;
		hardpointAmmo.StoredMagazines = total / Math.Max(1, magazineSize);
	}
}
