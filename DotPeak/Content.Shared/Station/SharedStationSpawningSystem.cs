// Decompiled with JetBrains decompiler
// Type: Content.Shared.Station.SharedStationSpawningSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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
  private Robust.Shared.GameObjects.EntityQuery<HandsComponent> _handsQuery;
  private Robust.Shared.GameObjects.EntityQuery<InventoryComponent> _inventoryQuery;
  private Robust.Shared.GameObjects.EntityQuery<StorageComponent> _storageQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._handsQuery = this.GetEntityQuery<HandsComponent>();
    this._inventoryQuery = this.GetEntityQuery<InventoryComponent>();
    this._storageQuery = this.GetEntityQuery<StorageComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
  }

  public void EquipRoleLoadout(
    EntityUid entity,
    RoleLoadout loadout,
    RoleLoadoutPrototype roleProto)
  {
    foreach (KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> keyValuePair in (IEnumerable<KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>>>) loadout.SelectedLoadouts.OrderBy<KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>>, int>((Func<KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>>, int>) (x => roleProto.Groups.FindIndex((Predicate<ProtoId<LoadoutGroupPrototype>>) (e => e == x.Key)))))
    {
      foreach (Loadout loadout1 in keyValuePair.Value)
      {
        LoadoutPrototype prototype;
        if (!this.PrototypeManager.TryIndex<LoadoutPrototype>(loadout1.Prototype, out prototype))
          this.Log.Error($"Unable to find loadout prototype for {loadout1.Prototype}");
        else
          this.EquipStartingGear(entity, prototype, false);
      }
    }
    this.EquipRoleName(entity, loadout, roleProto);
  }

  public void EquipRoleName(EntityUid entity, RoleLoadout loadout, RoleLoadoutPrototype roleProto)
  {
    string str = (string) null;
    if (roleProto.CanCustomizeName)
      str = loadout.EntityName;
    LocalizedDatasetPrototype prototype;
    if (string.IsNullOrEmpty(str) && this.PrototypeManager.TryIndex<LocalizedDatasetPrototype>(roleProto.NameDataset, out prototype))
      str = this.Loc.GetString(RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) prototype.Values));
    if (string.IsNullOrEmpty(str))
      return;
    this._metadata.SetEntityName(entity, str);
  }

  public void EquipStartingGear(EntityUid entity, LoadoutPrototype loadout, bool raiseEvent = true)
  {
    this.EquipStartingGear(entity, loadout.StartingGear, raiseEvent);
    this.EquipStartingGear(entity, (IEquipmentLoadout) loadout, raiseEvent);
  }

  public void EquipStartingGear(
    EntityUid entity,
    ProtoId<StartingGearPrototype>? startingGear,
    bool raiseEvent = true)
  {
    StartingGearPrototype prototype;
    this.PrototypeManager.TryIndex<StartingGearPrototype>(startingGear, out prototype);
    this.EquipStartingGear(entity, prototype, raiseEvent);
  }

  public void EquipStartingGear(
    EntityUid entity,
    StartingGearPrototype? startingGear,
    bool raiseEvent = true)
  {
    this.EquipStartingGear(entity, (IEquipmentLoadout) startingGear, raiseEvent);
  }

  public void EquipStartingGear(EntityUid entity, IEquipmentLoadout? startingGear, bool raiseEvent = true)
  {
    if (startingGear == null)
      return;
    TransformComponent component1 = this._xformQuery.GetComponent(entity);
    SlotDefinition[] slotDefinitions;
    if (this.InventorySystem.TryGetSlots(entity, out slotDefinitions))
    {
      foreach (SlotDefinition slotDefinition in slotDefinitions)
      {
        string gear = startingGear.GetGear(slotDefinition.Name);
        if (!string.IsNullOrEmpty(gear))
        {
          EntityUid itemUid = this.Spawn(gear, component1.Coordinates);
          this.InventorySystem.TryEquip(entity, itemUid, slotDefinition.Name, true, true);
        }
      }
    }
    HandsComponent component2;
    if (this._handsQuery.TryComp(entity, out component2))
    {
      List<EntProtoId> inhand = startingGear.Inhand;
      EntityCoordinates coordinates = component1.Coordinates;
      foreach (EntProtoId prototype in inhand)
      {
        EntityUid entity1 = this.Spawn((string) prototype, coordinates);
        string emptyHand;
        if (this._handsSystem.TryGetEmptyHand((Entity<HandsComponent>) (entity, component2), out emptyHand))
          this._handsSystem.TryPickup(entity, entity1, emptyHand, false, handsComp: component2);
      }
    }
    if (startingGear.Storage.Count > 0)
    {
      MapCoordinates mapCoordinates = this._xformSystem.GetMapCoordinates(entity);
      InventoryComponent component3;
      this._inventoryQuery.TryComp(entity, out component3);
      foreach ((string str, List<EntProtoId> entProtoIdList) in startingGear.Storage)
      {
        EntityUid? entityUid;
        StorageComponent component4;
        if (entProtoIdList != null && entProtoIdList.Count != 0 && component3 != null && this.InventorySystem.TryGetSlotEntity(entity, str, out entityUid, component3) && this._storageQuery.TryComp(entityUid, out component4))
        {
          foreach (EntProtoId prototype in entProtoIdList)
          {
            EntityUid uid1 = this.Spawn((string) prototype, mapCoordinates, rotation: new Angle());
            ItemComponent comp;
            if (this.TryComp<ItemComponent>(uid1, out comp))
            {
              CMStorageItemFillEvent args = new CMStorageItemFillEvent((Entity<ItemComponent>) (uid1, comp), component4);
              this.RaiseLocalEvent<CMStorageItemFillEvent>(entityUid.Value, ref args);
            }
            SharedStorageSystem storage = this._storage;
            EntityUid uid2 = entityUid.Value;
            EntityUid insertEnt = uid1;
            EntityUid? nullable;
            ref EntityUid? local = ref nullable;
            StorageComponent storageComponent = component4;
            EntityUid? user = new EntityUid?();
            StorageComponent storageComp = storageComponent;
            storage.Insert(uid2, insertEnt, out local, user, storageComp, false);
          }
        }
      }
    }
    if (!raiseEvent)
      return;
    StartingGearEquippedEvent args1 = new StartingGearEquippedEvent(entity);
    this.RaiseLocalEvent<StartingGearEquippedEvent>(entity, ref args1);
  }

  public string? GetGearForSlot(RoleLoadout? loadout, string slot)
  {
    if (loadout == null)
      return (string) null;
    foreach (KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> selectedLoadout in loadout.SelectedLoadouts)
    {
      foreach (Loadout loadout1 in selectedLoadout.Value)
      {
        LoadoutPrototype prototype;
        if (!this.PrototypeManager.TryIndex<LoadoutPrototype>(loadout1.Prototype, out prototype))
          return (string) null;
        string gear = prototype.GetGear(slot);
        if (gear != string.Empty)
          return gear;
      }
    }
    return (string) null;
  }
}
