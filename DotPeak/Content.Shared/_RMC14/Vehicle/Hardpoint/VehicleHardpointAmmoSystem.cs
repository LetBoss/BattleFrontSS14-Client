// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleHardpointAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleHardpointAmmoSystem : EntitySystem
{
  [Dependency]
  private readonly SharedGunSystem _gun;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleHardpointAmmoComponent, TakeAmmoEvent>(new EntityEventRefHandler<VehicleHardpointAmmoComponent, TakeAmmoEvent>(this.OnTakeAmmo), after: new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<VehicleHardpointAmmoComponent, OnEmptyGunShotEvent>(new EntityEventRefHandler<VehicleHardpointAmmoComponent, OnEmptyGunShotEvent>(this.OnEmptyGunShot));
  }

  private void OnTakeAmmo(Entity<VehicleHardpointAmmoComponent> ent, ref TakeAmmoEvent args)
  {
    BallisticAmmoProviderComponent comp;
    if (!this.TryComp<BallisticAmmoProviderComponent>(ent.Owner, out comp) || comp.Count > 0)
      return;
    this.NormalizeAmmoQueue(ent, comp);
  }

  private void OnEmptyGunShot(
    Entity<VehicleHardpointAmmoComponent> ent,
    ref OnEmptyGunShotEvent args)
  {
    BallisticAmmoProviderComponent comp;
    if (!this.TryComp<BallisticAmmoProviderComponent>(ent.Owner, out comp) || comp.Count > 0)
      return;
    this.NormalizeAmmoQueue(ent, comp);
  }

  public bool NormalizeAmmoQueue(
    Entity<VehicleHardpointAmmoComponent> ent,
    BallisticAmmoProviderComponent ammo)
  {
    return ammo.Count <= 0 && this.TryChamberNextMagazine(ent, ammo);
  }

  public bool TryChamberNextMagazine(
    Entity<VehicleHardpointAmmoComponent> ent,
    BallisticAmmoProviderComponent ammo)
  {
    int magazineSize = this.GetMagazineSize(ent.Comp, ammo);
    IReadOnlyList<int> storedRoundSlots = this.GetStoredRoundSlots(ent.Comp, magazineSize);
    int index1 = -1;
    for (int index2 = 0; index2 < storedRoundSlots.Count; ++index2)
    {
      if (storedRoundSlots[index2] > 0)
      {
        index1 = index2;
        break;
      }
    }
    if (index1 < 0)
      return false;
    int count = Math.Min(magazineSize, storedRoundSlots[index1]);
    int[] slots = new int[storedRoundSlots.Count];
    for (int index3 = 0; index3 < storedRoundSlots.Count; ++index3)
      slots[index3] = storedRoundSlots[index3];
    slots[index1] -= count;
    this.CompactStoredRoundSlots(ent, (IReadOnlyList<int>) slots, magazineSize);
    this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) (ent.Owner, ammo), count);
    this.RaiseAmmoChanged(ent.Owner);
    return true;
  }

  public List<VehicleAmmoSlotState> GetAmmoQueueSlots(
    VehicleHardpointAmmoComponent hardpointAmmo,
    BallisticAmmoProviderComponent ammo)
  {
    int magazineSize = this.GetMagazineSize(hardpointAmmo, ammo);
    List<VehicleAmmoSlotState> ammoQueueSlots = new List<VehicleAmmoSlotState>(Math.Max(1, hardpointAmmo.MaxStoredMagazines));
    ammoQueueSlots.Add(new VehicleAmmoSlotState(0, Math.Clamp(ammo.Count, 0, magazineSize), magazineSize, true));
    IReadOnlyList<int> storedRoundSlots = this.GetStoredRoundSlots(hardpointAmmo, magazineSize);
    for (int index = 0; index < storedRoundSlots.Count; ++index)
      ammoQueueSlots.Add(new VehicleAmmoSlotState(index + 1, storedRoundSlots[index], magazineSize, false));
    return ammoQueueSlots;
  }

  public bool HasLoadSpace(
    VehicleHardpointAmmoComponent hardpointAmmo,
    BallisticAmmoProviderComponent ammo,
    int ammoSlot)
  {
    if (ammoSlot < 0)
      return false;
    int magazineSize = this.GetMagazineSize(hardpointAmmo, ammo);
    if (ammoSlot == 0)
      return Math.Clamp(ammo.Count, 0, magazineSize) < magazineSize;
    int reserveSlot = ammoSlot - 1;
    return reserveSlot < this.GetMaxStoredRoundSlots(hardpointAmmo) && this.GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize) < magazineSize;
  }

  public bool HasUnloadRounds(
    VehicleHardpointAmmoComponent hardpointAmmo,
    BallisticAmmoProviderComponent ammo,
    int ammoSlot)
  {
    if (ammoSlot < 0)
      return false;
    int magazineSize = this.GetMagazineSize(hardpointAmmo, ammo);
    if (ammoSlot == 0)
      return Math.Min(Math.Clamp(ammo.Count, 0, magazineSize), ammo.UnspawnedCount) > 0;
    int reserveSlot = ammoSlot - 1;
    return reserveSlot < this.GetMaxStoredRoundSlots(hardpointAmmo) && this.GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize) > 0;
  }

  public int GetLoadAmount(
    VehicleHardpointAmmoComponent hardpointAmmo,
    BallisticAmmoProviderComponent ammo,
    int ammoSlot,
    int availableRounds)
  {
    if (ammoSlot < 0 || availableRounds <= 0)
      return 0;
    int magazineSize = this.GetMagazineSize(hardpointAmmo, ammo);
    if (ammoSlot == 0)
    {
      int num = Math.Clamp(ammo.Count, 0, magazineSize);
      int val2 = magazineSize - num;
      return val2 > 0 ? Math.Min(availableRounds, val2) : 0;
    }
    int reserveSlot = ammoSlot - 1;
    if (reserveSlot >= this.GetMaxStoredRoundSlots(hardpointAmmo))
      return 0;
    int storedSlotRounds = this.GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize);
    int val2_1 = magazineSize - storedSlotRounds;
    return val2_1 > 0 ? Math.Min(availableRounds, val2_1) : 0;
  }

  public int GetUnloadAmount(
    VehicleHardpointAmmoComponent hardpointAmmo,
    BallisticAmmoProviderComponent ammo,
    int ammoSlot)
  {
    if (ammoSlot < 0)
      return 0;
    int magazineSize = this.GetMagazineSize(hardpointAmmo, ammo);
    if (ammoSlot == 0)
      return Math.Min(Math.Clamp(ammo.Count, 0, magazineSize), ammo.UnspawnedCount);
    int reserveSlot = ammoSlot - 1;
    return reserveSlot >= this.GetMaxStoredRoundSlots(hardpointAmmo) ? 0 : this.GetStoredSlotRounds(hardpointAmmo, reserveSlot, magazineSize);
  }

  public bool TryLoadIntoSlot(
    Entity<VehicleHardpointAmmoComponent> ent,
    BallisticAmmoProviderComponent ammo,
    int ammoSlot,
    int rounds)
  {
    int loadAmount = this.GetLoadAmount(ent.Comp, ammo, ammoSlot, rounds);
    if (loadAmount <= 0)
      return false;
    int magazineSize = this.GetMagazineSize(ent.Comp, ammo);
    if (ammoSlot == 0)
    {
      this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) (ent.Owner, ammo), ammo.UnspawnedCount + loadAmount);
      this.RaiseAmmoChanged(ent.Owner);
    }
    else
    {
      int reserveSlot = ammoSlot - 1;
      int storedSlotRounds = this.GetStoredSlotRounds(ent.Comp, reserveSlot, magazineSize);
      this.SetStoredSlotRounds(ent, reserveSlot, storedSlotRounds + loadAmount, magazineSize);
    }
    return true;
  }

  public int TryUnloadFromSlot(
    Entity<VehicleHardpointAmmoComponent> ent,
    BallisticAmmoProviderComponent ammo,
    int ammoSlot,
    int maxRounds)
  {
    int num = Math.Min(this.GetUnloadAmount(ent.Comp, ammo, ammoSlot), Math.Max(0, maxRounds));
    if (num <= 0)
      return 0;
    int magazineSize = this.GetMagazineSize(ent.Comp, ammo);
    if (ammoSlot == 0)
    {
      this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) (ent.Owner, ammo), ammo.UnspawnedCount - num);
      this.NormalizeAmmoQueue(ent, ammo);
      this.RaiseAmmoChanged(ent.Owner);
    }
    else
    {
      int reserveSlot = ammoSlot - 1;
      int storedSlotRounds = this.GetStoredSlotRounds(ent.Comp, reserveSlot, magazineSize);
      this.SetStoredSlotRounds(ent, reserveSlot, storedSlotRounds - num, magazineSize);
    }
    return num;
  }

  public int GetMagazineSize(
    VehicleHardpointAmmoComponent hardpointAmmo,
    BallisticAmmoProviderComponent ammo)
  {
    return Math.Min(Math.Max(1, hardpointAmmo.MagazineSize), Math.Max(1, ammo.Capacity));
  }

  public int GetStoredRounds(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
  {
    if (hardpointAmmo.StoredRoundSlots.Count <= 0)
      return VehicleHardpointAmmoSystem.GetStoredRoundsFallback(hardpointAmmo, magazineSize);
    int storedRounds = 0;
    int num = Math.Min(hardpointAmmo.StoredRoundSlots.Count, this.GetMaxStoredRoundSlots(hardpointAmmo));
    for (int index = 0; index < num; ++index)
      storedRounds += Math.Clamp(hardpointAmmo.StoredRoundSlots[index], 0, Math.Max(1, magazineSize));
    return storedRounds;
  }

  public int GetMaxStoredRounds(VehicleHardpointAmmoComponent hardpointAmmo, int magazineSize)
  {
    return this.GetMaxStoredRoundSlots(hardpointAmmo) * Math.Max(1, magazineSize);
  }

  public int GetMaxStoredRoundSlots(VehicleHardpointAmmoComponent hardpointAmmo)
  {
    return Math.Max(0, hardpointAmmo.MaxStoredMagazines - 1);
  }

  public IReadOnlyList<int> GetStoredRoundSlots(
    VehicleHardpointAmmoComponent hardpointAmmo,
    int magazineSize)
  {
    int storedRoundSlots1 = this.GetMaxStoredRoundSlots(hardpointAmmo);
    int num1 = Math.Max(1, magazineSize);
    int[] storedRoundSlots2 = new int[storedRoundSlots1];
    if (hardpointAmmo.StoredRoundSlots.Count > 0)
    {
      int num2 = Math.Min(storedRoundSlots1, hardpointAmmo.StoredRoundSlots.Count);
      for (int index = 0; index < num2; ++index)
        storedRoundSlots2[index] = Math.Clamp(hardpointAmmo.StoredRoundSlots[index], 0, num1);
      return (IReadOnlyList<int>) storedRoundSlots2;
    }
    int val2 = Math.Clamp(VehicleHardpointAmmoSystem.GetStoredRoundsFallback(hardpointAmmo, magazineSize), 0, storedRoundSlots1 * num1);
    for (int index = 0; index < storedRoundSlots1 && val2 > 0; ++index)
    {
      storedRoundSlots2[index] = Math.Min(num1, val2);
      val2 -= storedRoundSlots2[index];
    }
    return (IReadOnlyList<int>) storedRoundSlots2;
  }

  public int GetStoredSlotRounds(
    VehicleHardpointAmmoComponent hardpointAmmo,
    int reserveSlot,
    int magazineSize)
  {
    IReadOnlyList<int> storedRoundSlots = this.GetStoredRoundSlots(hardpointAmmo, magazineSize);
    return reserveSlot >= 0 && reserveSlot < storedRoundSlots.Count ? storedRoundSlots[reserveSlot] : 0;
  }

  public void SetStoredRounds(
    Entity<VehicleHardpointAmmoComponent> ent,
    int rounds,
    int magazineSize)
  {
    int maxStoredRounds = this.GetMaxStoredRounds(ent.Comp, magazineSize);
    int num1 = Math.Max(1, magazineSize);
    int val2 = Math.Clamp(rounds, 0, maxStoredRounds);
    ent.Comp.StoredRoundSlots.Clear();
    for (int index = 0; index < this.GetMaxStoredRoundSlots(ent.Comp); ++index)
    {
      int num2 = Math.Min(num1, val2);
      ent.Comp.StoredRoundSlots.Add(num2);
      val2 -= num2;
    }
    VehicleHardpointAmmoSystem.UpdateStoredRoundTotals(ent.Comp, num1);
    this.Dirty<VehicleHardpointAmmoComponent>(ent);
    this.RaiseAmmoChanged(ent.Owner);
  }

  public void SetStoredSlotRounds(
    Entity<VehicleHardpointAmmoComponent> ent,
    int reserveSlot,
    int rounds,
    int magazineSize)
  {
    int storedRoundSlots1 = this.GetMaxStoredRoundSlots(ent.Comp);
    if (reserveSlot < 0 || reserveSlot >= storedRoundSlots1)
      return;
    int num1 = Math.Max(1, magazineSize);
    IReadOnlyList<int> storedRoundSlots2 = this.GetStoredRoundSlots(ent.Comp, num1);
    ent.Comp.StoredRoundSlots.Clear();
    for (int index = 0; index < storedRoundSlots1; ++index)
    {
      int num2 = index < storedRoundSlots2.Count ? storedRoundSlots2[index] : 0;
      if (index == reserveSlot)
        num2 = rounds;
      ent.Comp.StoredRoundSlots.Add(Math.Clamp(num2, 0, num1));
    }
    VehicleHardpointAmmoSystem.UpdateStoredRoundTotals(ent.Comp, num1);
    this.Dirty<VehicleHardpointAmmoComponent>(ent);
    this.RaiseAmmoChanged(ent.Owner);
  }

  private static int GetStoredRoundsFallback(
    VehicleHardpointAmmoComponent hardpointAmmo,
    int magazineSize)
  {
    return hardpointAmmo.StoredRounds > 0 ? hardpointAmmo.StoredRounds : Math.Max(0, hardpointAmmo.StoredMagazines) * Math.Max(1, magazineSize);
  }

  private void CompactStoredRoundSlots(
    Entity<VehicleHardpointAmmoComponent> ent,
    IReadOnlyList<int> slots,
    int magazineSize)
  {
    int storedRoundSlots = this.GetMaxStoredRoundSlots(ent.Comp);
    int num1 = Math.Max(1, magazineSize);
    ent.Comp.StoredRoundSlots.Clear();
    foreach (int slot in (IEnumerable<int>) slots)
    {
      if (ent.Comp.StoredRoundSlots.Count < storedRoundSlots)
      {
        int num2 = Math.Clamp(slot, 0, num1);
        if (num2 > 0)
          ent.Comp.StoredRoundSlots.Add(num2);
      }
      else
        break;
    }
    while (ent.Comp.StoredRoundSlots.Count < storedRoundSlots)
      ent.Comp.StoredRoundSlots.Add(0);
    VehicleHardpointAmmoSystem.UpdateStoredRoundTotals(ent.Comp, num1);
    this.Dirty<VehicleHardpointAmmoComponent>(ent);
    this.RaiseAmmoChanged(ent.Owner);
  }

  private void RaiseAmmoChanged(EntityUid ammoProvider)
  {
    VehicleAmmoChangedEvent args = new VehicleAmmoChangedEvent(ammoProvider);
    this.RaiseLocalEvent<VehicleAmmoChangedEvent>(ammoProvider, args, true);
  }

  private static void UpdateStoredRoundTotals(
    VehicleHardpointAmmoComponent hardpointAmmo,
    int magazineSize)
  {
    int num = 0;
    foreach (int storedRoundSlot in hardpointAmmo.StoredRoundSlots)
      num += Math.Clamp(storedRoundSlot, 0, magazineSize);
    hardpointAmmo.StoredRounds = num;
    hardpointAmmo.StoredMagazines = num / Math.Max(1, magazineSize);
  }
}
