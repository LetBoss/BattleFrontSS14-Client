using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Content.Shared.Salvage;

public abstract class SharedSalvageSystem : EntitySystem
{
	[Dependency]
	protected IConfigurationManager CfgManager;

	[Dependency]
	private IPrototypeManager _proto;

	public static readonly ProtoId<SalvageLootPrototype> ExpeditionsLootProto = ProtoId<SalvageLootPrototype>.op_Implicit("SalvageLoot");

	private readonly List<SalvageMapPrototype> _salvageMaps = new List<SalvageMapPrototype>();

	private readonly Dictionary<ISalvageMagnetOffering, float> _offeringWeights = new Dictionary<ISalvageMagnetOffering, float>
	{
		{
			default(AsteroidOffering),
			4.5f
		},
		{
			default(DebrisOffering),
			3.5f
		},
		{
			default(SalvageOffering),
			2f
		}
	};

	private readonly List<ProtoId<DungeonConfigPrototype>> _asteroidConfigs = new List<ProtoId<DungeonConfigPrototype>>
	{
		ProtoId<DungeonConfigPrototype>.op_Implicit("BlobAsteroid"),
		ProtoId<DungeonConfigPrototype>.op_Implicit("ClusterAsteroid"),
		ProtoId<DungeonConfigPrototype>.op_Implicit("SpindlyAsteroid"),
		ProtoId<DungeonConfigPrototype>.op_Implicit("SwissCheeseAsteroid")
	};

	private readonly ProtoId<WeightedRandomPrototype> _asteroidOreWeights = ProtoId<WeightedRandomPrototype>.op_Implicit("AsteroidOre");

	private readonly MinMax _asteroidOreCount = new MinMax(5, 7);

	private readonly List<ProtoId<DungeonConfigPrototype>> _debrisConfigs = new List<ProtoId<DungeonConfigPrototype>> { ProtoId<DungeonConfigPrototype>.op_Implicit("ChunkDebris") };

	public string GetFTLName(LocalizedDatasetPrototype dataset, int seed)
	{
		System.Random random = new System.Random(seed);
		return $"{base.Loc.GetString(dataset.Values[random.Next(dataset.Values.Count)])}-{random.Next(10, 100)}-{(char)(ushort)(65 + random.Next(26))}";
	}

	public SalvageMission GetMission(SalvageDifficultyPrototype difficulty, int seed)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		float modifierBudget = difficulty.ModifierBudget;
		System.Random rand = new System.Random(seed);
		SalvageBiomeModPrototype biome = GetMod<SalvageBiomeModPrototype>(rand, ref modifierBudget);
		SalvageLightMod light = GetBiomeMod<SalvageLightMod>(biome.ID, rand, ref modifierBudget);
		SalvageTemperatureMod temp = GetBiomeMod<SalvageTemperatureMod>(biome.ID, rand, ref modifierBudget);
		SalvageAirMod air = GetBiomeMod<SalvageAirMod>(biome.ID, rand, ref modifierBudget);
		SalvageDungeonModPrototype dungeon = GetBiomeMod<SalvageDungeonModPrototype>(biome.ID, rand, ref modifierBudget);
		List<SalvageFactionPrototype> factionProtos = _proto.EnumeratePrototypes<SalvageFactionPrototype>().ToList();
		factionProtos.Sort((SalvageFactionPrototype x, SalvageFactionPrototype y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal));
		SalvageFactionPrototype faction = factionProtos[rand.Next(factionProtos.Count)];
		List<string> mods = new List<string>();
		if (air.Description != LocId.op_Implicit(string.Empty))
		{
			mods.Add(base.Loc.GetString(LocId.op_Implicit(air.Description)));
		}
		if (temp.Description != LocId.op_Implicit(string.Empty) && !air.Space)
		{
			mods.Add(base.Loc.GetString(LocId.op_Implicit(temp.Description)));
		}
		if (light.Description != LocId.op_Implicit(string.Empty))
		{
			mods.Add(base.Loc.GetString(LocId.op_Implicit(light.Description)));
		}
		TimeSpan duration = TimeSpan.FromSeconds(CfgManager.GetCVar<float>(CCVars.SalvageExpeditionDuration));
		return new SalvageMission(seed, dungeon.ID, faction.ID, biome.ID, air.ID, temp.Temperature, light.Color, duration, mods);
	}

	public T GetBiomeMod<T>(string biome, System.Random rand, ref float rating) where T : class, IPrototype, IBiomeSpecificMod
	{
		List<T> mods = _proto.EnumeratePrototypes<T>().ToList();
		mods.Sort((T x, T y) => string.Compare(((IPrototype)x).ID, ((IPrototype)y).ID, StringComparison.Ordinal));
		rand.Shuffle(CollectionsMarshal.AsSpan(mods));
		foreach (T mod in mods)
		{
			if (!(mod.Cost > rating) && (mod.Biomes == null || mod.Biomes.Contains(biome)))
			{
				rating -= mod.Cost;
				return mod;
			}
		}
		throw new InvalidOperationException();
	}

	public T GetMod<T>(System.Random rand, ref float rating) where T : class, IPrototype, ISalvageMod
	{
		List<T> mods = _proto.EnumeratePrototypes<T>().ToList();
		mods.Sort((T x, T y) => string.Compare(((IPrototype)x).ID, ((IPrototype)y).ID, StringComparison.Ordinal));
		rand.Shuffle(CollectionsMarshal.AsSpan(mods));
		foreach (T mod in mods)
		{
			if (!(mod.Cost > rating))
			{
				rating -= mod.Cost;
				return mod;
			}
		}
		throw new InvalidOperationException();
	}

	public ISalvageMagnetOffering GetSalvageOffering(int seed)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		System.Random rand = new System.Random(seed);
		ISalvageMagnetOffering type = SharedRandomExtensions.Pick(_offeringWeights, rand);
		if (!(type is AsteroidOffering))
		{
			if (!(type is DebrisOffering))
			{
				if (type is SalvageOffering)
				{
					_salvageMaps.Clear();
					_salvageMaps.AddRange(_proto.EnumeratePrototypes<SalvageMapPrototype>());
					_salvageMaps.Sort((SalvageMapPrototype x, SalvageMapPrototype y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal));
					int mapIndex = rand.Next(_salvageMaps.Count);
					SalvageMapPrototype map = _salvageMaps[mapIndex];
					return new SalvageOffering
					{
						SalvageMap = map
					};
				}
				throw new NotImplementedException($"Salvage type {type} not implemented!");
			}
			ProtoId<DungeonConfigPrototype> id = _debrisConfigs[rand.Next(_debrisConfigs.Count)];
			return new DebrisOffering
			{
				Id = ProtoId<DungeonConfigPrototype>.op_Implicit(id)
			};
		}
		ProtoId<DungeonConfigPrototype> configId = _asteroidConfigs[rand.Next(_asteroidConfigs.Count)];
		DungeonConfigPrototype configProto = _proto.Index<DungeonConfigPrototype>(configId);
		Dictionary<string, int> layers = new Dictionary<string, int>();
		DungeonConfig config = new DungeonConfig
		{
			Layers = new List<IDunGenLayer>(configProto.Layers),
			MaxCount = configProto.MaxCount,
			MaxOffset = configProto.MaxOffset,
			MinCount = configProto.MinCount,
			MinOffset = configProto.MinOffset,
			ReserveTiles = configProto.ReserveTiles
		};
		int count = _asteroidOreCount.Next(rand);
		WeightedRandomPrototype weightedProto = _proto.Index<WeightedRandomPrototype>(_asteroidOreWeights);
		for (int i = 0; i < count; i++)
		{
			string ore = weightedProto.Pick(rand);
			config.Layers.Add(_proto.Index<OreDunGenPrototype>(ore));
			int layerCount = Extensions.GetOrNew<string, int>(layers, ore);
			layerCount++;
			layers[ore] = layerCount;
		}
		return new AsteroidOffering
		{
			Id = ProtoId<DungeonConfigPrototype>.op_Implicit(configId),
			DungeonConfig = config,
			MarkerLayers = layers
		};
	}
}
