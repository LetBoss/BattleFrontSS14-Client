// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Survivor.SurvivorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
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
    this.SubscribeLocalEvent<EquipSurvivorPresetComponent, PlayerSpawnCompleteEvent>(new EntityEventRefHandler<EquipSurvivorPresetComponent, PlayerSpawnCompleteEvent>(this.OnPresetPlayerSpawnComplete), after: new Type[1]
    {
      typeof (CMArmorSystem)
    });
  }

  private void OnPresetPlayerSpawnComplete(
    Entity<EquipSurvivorPresetComponent> ent,
    ref PlayerSpawnCompleteEvent args)
  {
    this.ApplyPreset((EntityUid) ent, ent.Comp.Preset);
  }

  private void ApplyPreset(EntityUid mob, EntProtoId<SurvivorPresetComponent> preset)
  {
    SurvivorPresetComponent comp;
    if (!preset.TryGet(out comp, this._prototypes, this._compFactory))
      return;
    if (comp.RandomStartingGear.Count > 0)
    {
      foreach ((string str, List<EntProtoId> list) in comp.RandomStartingGear)
      {
        EntProtoId toSpawn = RandomExtensions.Pick<EntProtoId>(this._random, (IReadOnlyList<EntProtoId>) list);
        this.Equip(mob, toSpawn, false, slotName: str);
      }
    }
    if (comp.RandomOutfits.Count > 0)
    {
      foreach (EntProtoId toSpawn in RandomExtensions.Pick<List<EntProtoId>>(this._random, (IReadOnlyList<List<EntProtoId>>) comp.RandomOutfits))
        this.Equip(mob, toSpawn);
    }
    if (this._random.Prob(comp.PrimaryWeaponChance) && comp.PrimaryWeapons.Count > 0)
    {
      foreach (EntProtoId toSpawn in RandomExtensions.Pick<List<EntProtoId>>(this._random, (IReadOnlyList<List<EntProtoId>>) comp.PrimaryWeapons))
        this.Equip(mob, toSpawn, tryInHand: true);
    }
    if (comp.RandomWeapon.Count > 0)
    {
      foreach (EntProtoId toSpawn in RandomExtensions.Pick<List<EntProtoId>>(this._random, (IReadOnlyList<List<EntProtoId>>) comp.RandomWeapon))
        this.Equip(mob, toSpawn, tryEquip: comp.TryEquipRandomWeapon);
    }
    if (comp.RandomGear.Count > 0)
    {
      foreach (EntProtoId toSpawn in RandomExtensions.Pick<List<EntProtoId>>(this._random, (IReadOnlyList<List<EntProtoId>>) comp.RandomGear))
        this.Equip(mob, toSpawn, tryInHand: true, tryEquip: false);
    }
    if (comp.RandomGearOther.Count > 0)
    {
      foreach (List<List<EntProtoId>> list in comp.RandomGearOther)
      {
        if (list.Count != 0)
        {
          foreach (EntProtoId toSpawn in RandomExtensions.Pick<List<EntProtoId>>(this._random, (IReadOnlyList<List<EntProtoId>>) list))
            this.Equip(mob, toSpawn, tryEquip: comp.TryEquipRandomOtherGear);
        }
      }
    }
    int num = this._random.Next(1, comp.RareItemCoefficent);
    if (comp.RareItems.Count <= 0)
      return;
    foreach ((EntProtoId entProtoId, (int, int) tuple) in comp.RareItems)
    {
      if (num >= tuple.Item1 && num <= tuple.Item2)
      {
        this.Equip(mob, entProtoId, tryInHand: true);
        break;
      }
    }
  }

  private void Equip(
    EntityUid mob,
    EntProtoId toSpawn,
    bool tryStorage = true,
    bool tryInHand = false,
    bool tryEquip = true,
    string? slotName = null)
  {
    if (this._net.IsClient)
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(mob);
    EntityUid entityUid = this.Spawn((string) toSpawn, moverCoordinates);
    if (tryEquip)
    {
      InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) mob);
      ContainerSlot container;
      while (slotEnumerator.MoveNext(out container))
      {
        if ((slotName == null || !(container.ID != slotName)) && !container.ContainedEntity.HasValue && this._inventory.TryEquip(mob, entityUid, container.ID, true))
          return;
      }
    }
    if (tryStorage && this.TryInsertItemInStorage(mob, entityUid) || tryInHand && this._hands.TryPickupAnyHand(mob, entityUid))
      return;
    this.Log.Warning($"Couldn't equip {this.ToPrettyString((Entity<MetaDataComponent>) entityUid)} on {this.ToPrettyString((Entity<MetaDataComponent>) mob)}");
    this.QueueDel(new EntityUid?(entityUid));
  }

  public bool TryInsertItemInStorage(EntityUid mob, EntityUid toInsert)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) mob, SlotFlags.BACK);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        StorageComponent comp;
        if (this.TryComp<StorageComponent>(valueOrDefault, out comp))
        {
          if (!this._rmcStorage.CanInsertStoreSkill((Entity<StorageComponent, StorageStoreSkillRequiredComponent>) valueOrDefault, toInsert, new EntityUid?(mob), out LocId _))
            return false;
          SharedStorageSystem storage = this._storage;
          EntityUid uid = valueOrDefault;
          EntityUid insertEnt = toInsert;
          ref EntityUid? local = ref containedEntity;
          StorageComponent storageComponent = comp;
          EntityUid? user = new EntityUid?();
          StorageComponent storageComp = storageComponent;
          if (storage.Insert(uid, insertEnt, out local, user, storageComp, false))
            return true;
        }
      }
    }
    return false;
  }
}
