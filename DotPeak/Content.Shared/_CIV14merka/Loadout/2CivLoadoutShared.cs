// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutManifest
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Loadout;

public static class CivLoadoutManifest
{
  private static readonly HashSet<string> WeaponSlots = new HashSet<string>()
  {
    "suitstorage",
    "back"
  };

  public static List<CivLoadoutItemInfo> GetRemovableItems(
    StartingGearPrototype gear,
    IPrototypeManager prototypes,
    IComponentFactory componentFactory,
    HashSet<string>? swapSlots = null)
  {
    List<CivLoadoutItemInfo> removableItems = new List<CivLoadoutItemInfo>();
    foreach ((string str1, EntProtoId entProtoId) in gear.Equipment)
    {
      EntityPrototype prototype;
      if (!string.IsNullOrEmpty(entProtoId.Id) && prototypes.TryIndex<EntityPrototype>(entProtoId.Id, out prototype))
      {
        StorageFillComponent component;
        if (prototype.TryGetComponent<StorageFillComponent>(out component, componentFactory))
        {
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          List<string> stringList = new List<string>();
          foreach (EntitySpawnEntry content in component.Contents)
          {
            EntProtoId? prototypeId = content.PrototypeId;
            if (prototypeId.HasValue)
            {
              EntProtoId valueOrDefault = prototypeId.GetValueOrDefault();
              if (!string.IsNullOrEmpty(valueOrDefault.Id))
              {
                int num1 = Math.Max(1, content.Amount);
                int num2;
                if (dictionary.TryGetValue(valueOrDefault.Id, out num2))
                {
                  dictionary[valueOrDefault.Id] = num2 + num1;
                }
                else
                {
                  dictionary[valueOrDefault.Id] = num1;
                  stringList.Add(valueOrDefault.Id);
                }
              }
            }
          }
          foreach (string str2 in stringList)
            removableItems.Add(new CivLoadoutItemInfo(str1, str2, dictionary[str2]));
        }
        else if (CivLoadoutManifest.WeaponSlots.Contains(str1) && (swapSlots == null || !swapSlots.Contains(str1)))
          removableItems.Add(new CivLoadoutItemInfo(str1, entProtoId.Id, 1));
      }
    }
    return removableItems;
  }

  public static Dictionary<string, List<string>> GetSlotOptions(
    string faction,
    CivTdmClass cls,
    IPrototypeManager prototypes)
  {
    Dictionary<string, List<string>> slotOptions = new Dictionary<string, List<string>>();
    foreach (CivLoadoutOptionsPrototype enumeratePrototype in prototypes.EnumeratePrototypes<CivLoadoutOptionsPrototype>())
    {
      if (!(enumeratePrototype.Faction != faction) && enumeratePrototype.Class == cls)
      {
        foreach ((string key, List<EntProtoId> entProtoIdList) in enumeratePrototype.Slots)
        {
          if (entProtoIdList.Count >= 2)
          {
            List<string> stringList;
            if (!slotOptions.TryGetValue(key, out stringList))
            {
              stringList = new List<string>();
              slotOptions[key] = stringList;
            }
            foreach (EntProtoId entProtoId in entProtoIdList)
            {
              if (!string.IsNullOrEmpty(entProtoId.Id) && !stringList.Contains(entProtoId.Id))
                stringList.Add(entProtoId.Id);
            }
          }
        }
      }
    }
    return slotOptions;
  }

  public static Dictionary<string, List<string>> GetEffectiveSwaps(
    StartingGearPrototype gear,
    string faction,
    CivTdmClass cls,
    IReadOnlySet<string> owned,
    IPrototypeManager prototypes,
    IComponentFactory componentFactory)
  {
    Dictionary<string, List<string>> effectiveSwaps = new Dictionary<string, List<string>>();
    foreach ((string key3, List<string> stringList1) in CivLoadoutManifest.GetSlotOptions(faction, cls, prototypes))
    {
      string key2 = key3;
      List<string> stringList2 = stringList1;
      List<string> stringList3 = new List<string>();
      for (int index = 0; index < stringList2.Count; ++index)
      {
        if (index == 0 || owned.Contains(stringList2[index]))
          stringList3.Add(stringList2[index]);
      }
      if (stringList3.Count >= 2)
        effectiveSwaps[key2] = stringList3;
    }
    EntProtoId entProtoId2;
    foreach ((key3, entProtoId2) in gear.Equipment)
    {
      string key4 = key3;
      EntProtoId entProtoId3 = entProtoId2;
      EntityPrototype prototype1;
      ClothingComponent component1;
      if (!string.IsNullOrEmpty(entProtoId3.Id) && prototypes.TryIndex<EntityPrototype>(entProtoId3.Id, out prototype1) && prototype1.TryGetComponent<ClothingComponent>(out component1, componentFactory) && component1.Slots != SlotFlags.NONE)
      {
        foreach (string id in (IEnumerable<string>) owned)
        {
          EntityPrototype prototype2;
          ClothingComponent component2;
          if (!(id == entProtoId3.Id) && prototypes.TryIndex<EntityPrototype>(id, out prototype2) && prototype2.TryGetComponent<ClothingComponent>(out component2, componentFactory) && (component2.Slots & component1.Slots) != SlotFlags.NONE)
          {
            List<string> stringList;
            if (!effectiveSwaps.TryGetValue(key4, out stringList))
            {
              stringList = new List<string>()
              {
                entProtoId3.Id
              };
              effectiveSwaps[key4] = stringList;
            }
            if (!stringList.Contains(id))
              stringList.Add(id);
          }
        }
      }
    }
    return effectiveSwaps;
  }
}
