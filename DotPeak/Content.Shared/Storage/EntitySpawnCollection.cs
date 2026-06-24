// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySpawnCollection
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Storage;

public static class EntitySpawnCollection
{
  public static List<string> GetSpawns(
    ProtoId<EntitySpawnEntryPrototype> proto,
    IPrototypeManager? protoManager = null,
    IRobustRandom? random = null)
  {
    IoCManager.Resolve<IPrototypeManager, IRobustRandom>(ref protoManager, ref random);
    return EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) protoManager.Index<EntitySpawnEntryPrototype>(proto).Entries, random);
  }

  public static List<string?> GetSpawns(
    ProtoId<EntitySpawnEntryPrototype> proto,
    System.Random random,
    IPrototypeManager? protoManager = null)
  {
    IoCManager.Resolve<IPrototypeManager>(ref protoManager);
    return EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) protoManager.Index<EntitySpawnEntryPrototype>(proto).Entries, random);
  }

  public static List<string> GetSpawns(IEnumerable<EntitySpawnEntry> entries, IRobustRandom? random = null)
  {
    IoCManager.Resolve<IRobustRandom>(ref random);
    List<string> spawns = new List<string>();
    List<EntitySpawnCollection.OrGroup> orGroups;
    foreach (EntitySpawnEntry collectOrGroup in EntitySpawnCollection.CollectOrGroups(entries, out orGroups))
    {
      if (((double) collectOrGroup.SpawnProbability == 1.0 || random.Prob(collectOrGroup.SpawnProbability)) && collectOrGroup.PrototypeId.HasValue)
      {
        int amount = (int) collectOrGroup.GetAmount(random);
        for (int index = 0; index < amount; ++index)
        {
          List<string> stringList = spawns;
          EntProtoId? prototypeId = collectOrGroup.PrototypeId;
          string valueOrDefault = prototypeId.HasValue ? (string) prototypeId.GetValueOrDefault() : (string) null;
          stringList.Add(valueOrDefault);
        }
      }
    }
    foreach (EntitySpawnCollection.OrGroup orGroup in orGroups)
    {
      double num1 = random.NextDouble() * (double) orGroup.CumulativeProbability;
      double num2 = 0.0;
      foreach (EntitySpawnEntry entry in orGroup.Entries)
      {
        num2 += (double) entry.SpawnProbability;
        if (num1 <= num2)
        {
          if (entry.PrototypeId.HasValue)
          {
            int amount = (int) entry.GetAmount(random);
            for (int index = 0; index < amount; ++index)
            {
              List<string> stringList = spawns;
              EntProtoId? prototypeId = entry.PrototypeId;
              string valueOrDefault = prototypeId.HasValue ? (string) prototypeId.GetValueOrDefault() : (string) null;
              stringList.Add(valueOrDefault);
            }
            break;
          }
          break;
        }
      }
    }
    return spawns;
  }

  public static List<string?> GetSpawns(IEnumerable<EntitySpawnEntry> entries, System.Random random)
  {
    List<string> spawns = new List<string>();
    List<EntitySpawnCollection.OrGroup> orGroups;
    foreach (EntitySpawnEntry collectOrGroup in EntitySpawnCollection.CollectOrGroups(entries, out orGroups))
    {
      if ((double) collectOrGroup.SpawnProbability == 1.0 || random.NextDouble() < (double) collectOrGroup.SpawnProbability)
      {
        int amount = (int) collectOrGroup.GetAmount(random);
        for (int index = 0; index < amount; ++index)
        {
          List<string> stringList = spawns;
          EntProtoId? prototypeId = collectOrGroup.PrototypeId;
          string valueOrDefault = prototypeId.HasValue ? (string) prototypeId.GetValueOrDefault() : (string) null;
          stringList.Add(valueOrDefault);
        }
      }
    }
    foreach (EntitySpawnCollection.OrGroup orGroup in orGroups)
    {
      double num1 = random.NextDouble() * (double) orGroup.CumulativeProbability;
      double num2 = 0.0;
      foreach (EntitySpawnEntry entry in orGroup.Entries)
      {
        num2 += (double) entry.SpawnProbability;
        if (num1 <= num2)
        {
          int amount = (int) entry.GetAmount(random);
          for (int index = 0; index < amount; ++index)
          {
            List<string> stringList = spawns;
            EntProtoId? prototypeId = entry.PrototypeId;
            string valueOrDefault = prototypeId.HasValue ? (string) prototypeId.GetValueOrDefault() : (string) null;
            stringList.Add(valueOrDefault);
          }
          break;
        }
      }
    }
    return spawns;
  }

  public static double GetAmount(this EntitySpawnEntry entry, System.Random random, bool getAverage = false)
  {
    if (entry.MaxAmount <= entry.Amount)
      return (double) entry.Amount;
    return getAverage ? (double) (entry.Amount + entry.MaxAmount) / 2.0 : (double) random.Next(entry.Amount, entry.MaxAmount);
  }

  public static List<EntitySpawnEntry> CollectOrGroups(
    IEnumerable<EntitySpawnEntry> entries,
    out List<EntitySpawnCollection.OrGroup> orGroups)
  {
    List<EntitySpawnEntry> entitySpawnEntryList = new List<EntitySpawnEntry>();
    Dictionary<string, EntitySpawnCollection.OrGroup> dictionary = new Dictionary<string, EntitySpawnCollection.OrGroup>();
    foreach (EntitySpawnEntry entry in entries)
    {
      if (!string.IsNullOrEmpty(entry.GroupId))
      {
        EntitySpawnCollection.OrGroup orGroup;
        if (!dictionary.TryGetValue(entry.GroupId, out orGroup))
        {
          orGroup = new EntitySpawnCollection.OrGroup();
          dictionary.Add(entry.GroupId, orGroup);
        }
        orGroup.Entries.Add(entry);
        orGroup.CumulativeProbability += entry.SpawnProbability;
      }
      else
        entitySpawnEntryList.Add(entry);
    }
    orGroups = dictionary.Values.ToList<EntitySpawnCollection.OrGroup>();
    return entitySpawnEntryList;
  }

  public static double GetAmount(
    this EntitySpawnEntry entry,
    IRobustRandom? random = null,
    bool getAverage = false)
  {
    if (entry.MaxAmount <= entry.Amount)
      return (double) entry.Amount;
    if (getAverage)
      return (double) (entry.Amount + entry.MaxAmount) / 2.0;
    IoCManager.Resolve<IRobustRandom>(ref random);
    return (double) random.Next(entry.Amount, entry.MaxAmount);
  }

  public sealed class OrGroup
  {
    public List<EntitySpawnEntry> Entries { get; set; } = new List<EntitySpawnEntry>();

    public float CumulativeProbability { get; set; }
  }
}
