// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.SharedSalvageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.Dataset;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonLayers;
using Content.Shared.Procedural.Loot;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.Salvage.Expeditions;
using Content.Shared.Salvage.Expeditions.Modifiers;
using Content.Shared.Salvage.Magnet;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared.Salvage;

public abstract class SharedSalvageSystem : EntitySystem
{
  [Dependency]
  protected IConfigurationManager CfgManager;
  [Dependency]
  private IPrototypeManager _proto;
  public static readonly ProtoId<SalvageLootPrototype> ExpeditionsLootProto = (ProtoId<SalvageLootPrototype>) "SalvageLoot";
  private readonly List<SalvageMapPrototype> _salvageMaps = new List<SalvageMapPrototype>();
  private readonly Dictionary<ISalvageMagnetOffering, float> _offeringWeights = new Dictionary<ISalvageMagnetOffering, float>()
  {
    {
      (ISalvageMagnetOffering) new AsteroidOffering(),
      4.5f
    },
    {
      (ISalvageMagnetOffering) new DebrisOffering(),
      3.5f
    },
    {
      (ISalvageMagnetOffering) new SalvageOffering(),
      2f
    }
  };
  private readonly List<ProtoId<DungeonConfigPrototype>> _asteroidConfigs = new List<ProtoId<DungeonConfigPrototype>>()
  {
    (ProtoId<DungeonConfigPrototype>) "BlobAsteroid",
    (ProtoId<DungeonConfigPrototype>) "ClusterAsteroid",
    (ProtoId<DungeonConfigPrototype>) "SpindlyAsteroid",
    (ProtoId<DungeonConfigPrototype>) "SwissCheeseAsteroid"
  };
  private readonly ProtoId<WeightedRandomPrototype> _asteroidOreWeights = (ProtoId<WeightedRandomPrototype>) "AsteroidOre";
  private readonly MinMax _asteroidOreCount = new MinMax(5, 7);
  private readonly List<ProtoId<DungeonConfigPrototype>> _debrisConfigs = new List<ProtoId<DungeonConfigPrototype>>()
  {
    (ProtoId<DungeonConfigPrototype>) "ChunkDebris"
  };

  public string GetFTLName(LocalizedDatasetPrototype dataset, int seed)
  {
    System.Random random = new System.Random(seed);
    return $"{this.Loc.GetString(dataset.Values[random.Next(dataset.Values.Count)])}-{random.Next(10, 100)}-{(char) (65 + random.Next(26))}";
  }

  public SalvageMission GetMission(SalvageDifficultyPrototype difficulty, int seed)
  {
    float modifierBudget = difficulty.ModifierBudget;
    System.Random rand = new System.Random(seed);
    SalvageBiomeModPrototype mod = this.GetMod<SalvageBiomeModPrototype>(rand, ref modifierBudget);
    SalvageLightMod biomeMod1 = this.GetBiomeMod<SalvageLightMod>(mod.ID, rand, ref modifierBudget);
    SalvageTemperatureMod biomeMod2 = this.GetBiomeMod<SalvageTemperatureMod>(mod.ID, rand, ref modifierBudget);
    SalvageAirMod biomeMod3 = this.GetBiomeMod<SalvageAirMod>(mod.ID, rand, ref modifierBudget);
    SalvageDungeonModPrototype biomeMod4 = this.GetBiomeMod<SalvageDungeonModPrototype>(mod.ID, rand, ref modifierBudget);
    List<SalvageFactionPrototype> list = this._proto.EnumeratePrototypes<SalvageFactionPrototype>().ToList<SalvageFactionPrototype>();
    list.Sort((Comparison<SalvageFactionPrototype>) ((x, y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal)));
    SalvageFactionPrototype factionPrototype = list[rand.Next(list.Count)];
    List<string> Modifiers = new List<string>();
    if (biomeMod3.Description != (LocId) string.Empty)
      Modifiers.Add(this.Loc.GetString((string) biomeMod3.Description));
    if (biomeMod2.Description != (LocId) string.Empty && !biomeMod3.Space)
      Modifiers.Add(this.Loc.GetString((string) biomeMod2.Description));
    if (biomeMod1.Description != (LocId) string.Empty)
      Modifiers.Add(this.Loc.GetString((string) biomeMod1.Description));
    TimeSpan Duration = TimeSpan.FromSeconds((double) this.CfgManager.GetCVar<float>(CCVars.SalvageExpeditionDuration));
    return new SalvageMission(seed, biomeMod4.ID, factionPrototype.ID, mod.ID, biomeMod3.ID, biomeMod2.Temperature, biomeMod1.Color, Duration, Modifiers);
  }

  public T GetBiomeMod<T>(string biome, System.Random rand, ref float rating) where T : class, IPrototype, IBiomeSpecificMod
  {
    List<T> list = this._proto.EnumeratePrototypes<T>().ToList<T>();
    list.Sort((Comparison<T>) ((x, y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal)));
    rand.Shuffle<T>(CollectionsMarshal.AsSpan<T>(list));
    foreach (T biomeMod in list)
    {
      if ((double) biomeMod.Cost <= (double) rating && (biomeMod.Biomes == null || biomeMod.Biomes.Contains(biome)))
      {
        rating -= biomeMod.Cost;
        return biomeMod;
      }
    }
    throw new InvalidOperationException();
  }

  public T GetMod<T>(System.Random rand, ref float rating) where T : class, IPrototype, ISalvageMod
  {
    List<T> list = this._proto.EnumeratePrototypes<T>().ToList<T>();
    list.Sort((Comparison<T>) ((x, y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal)));
    rand.Shuffle<T>(CollectionsMarshal.AsSpan<T>(list));
    foreach (T mod in list)
    {
      if ((double) mod.Cost <= (double) rating)
      {
        rating -= mod.Cost;
        return mod;
      }
    }
    throw new InvalidOperationException();
  }

  public ISalvageMagnetOffering GetSalvageOffering(int seed)
  {
    System.Random random = new System.Random(seed);
    ISalvageMagnetOffering salvageMagnetOffering = SharedRandomExtensions.Pick<ISalvageMagnetOffering>(this._offeringWeights, random);
    switch (salvageMagnetOffering)
    {
      case AsteroidOffering _:
        ProtoId<DungeonConfigPrototype> asteroidConfig = this._asteroidConfigs[random.Next(this._asteroidConfigs.Count)];
        DungeonConfigPrototype dungeonConfigPrototype = this._proto.Index<DungeonConfigPrototype>(asteroidConfig);
        Dictionary<string, int> dict = new Dictionary<string, int>();
        DungeonConfig dungeonConfig = new DungeonConfig()
        {
          Layers = new List<IDunGenLayer>((IEnumerable<IDunGenLayer>) dungeonConfigPrototype.Layers),
          MaxCount = dungeonConfigPrototype.MaxCount,
          MaxOffset = dungeonConfigPrototype.MaxOffset,
          MinCount = dungeonConfigPrototype.MinCount,
          MinOffset = dungeonConfigPrototype.MinOffset,
          ReserveTiles = dungeonConfigPrototype.ReserveTiles
        };
        int num1 = this._asteroidOreCount.Next(random);
        WeightedRandomPrototype prototype = this._proto.Index<WeightedRandomPrototype>(this._asteroidOreWeights);
        for (int index = 0; index < num1; ++index)
        {
          string str = prototype.Pick(random);
          dungeonConfig.Layers.Add((IDunGenLayer) this._proto.Index<OreDunGenPrototype>(str));
          int num2 = dict.GetOrNew<string, int>(str) + 1;
          dict[str] = num2;
        }
        return (ISalvageMagnetOffering) new AsteroidOffering()
        {
          Id = (string) asteroidConfig,
          DungeonConfig = dungeonConfig,
          MarkerLayers = dict
        };
      case DebrisOffering _:
        ProtoId<DungeonConfigPrototype> debrisConfig = this._debrisConfigs[random.Next(this._debrisConfigs.Count)];
        return (ISalvageMagnetOffering) new DebrisOffering()
        {
          Id = (string) debrisConfig
        };
      case SalvageOffering _:
        this._salvageMaps.Clear();
        this._salvageMaps.AddRange(this._proto.EnumeratePrototypes<SalvageMapPrototype>());
        this._salvageMaps.Sort((Comparison<SalvageMapPrototype>) ((x, y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal)));
        SalvageMapPrototype salvageMap = this._salvageMaps[random.Next(this._salvageMaps.Count)];
        return (ISalvageMagnetOffering) new SalvageOffering()
        {
          SalvageMap = salvageMap
        };
      default:
        throw new NotImplementedException($"Salvage type {salvageMagnetOffering} not implemented!");
    }
  }
}
