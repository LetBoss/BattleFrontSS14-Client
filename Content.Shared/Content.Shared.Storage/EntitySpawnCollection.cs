using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Storage;

public static class EntitySpawnCollection
{
	public sealed class OrGroup
	{
		public List<EntitySpawnEntry> Entries { get; set; } = new List<EntitySpawnEntry>();

		public float CumulativeProbability { get; set; }
	}

	public static List<string> GetSpawns(ProtoId<EntitySpawnEntryPrototype> proto, IPrototypeManager? protoManager = null, IRobustRandom? random = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IPrototypeManager, IRobustRandom>(ref protoManager, ref random);
		return GetSpawns(protoManager.Index<EntitySpawnEntryPrototype>(proto).Entries, random);
	}

	public static List<string?> GetSpawns(ProtoId<EntitySpawnEntryPrototype> proto, System.Random random, IPrototypeManager? protoManager = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IPrototypeManager>(ref protoManager);
		return GetSpawns((IEnumerable<EntitySpawnEntry>)protoManager.Index<EntitySpawnEntryPrototype>(proto).Entries, random);
	}

	public static List<string> GetSpawns(IEnumerable<EntitySpawnEntry> entries, IRobustRandom? random = null)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IRobustRandom>(ref random);
		List<string> spawned = new List<string>();
		List<OrGroup> orGroupedSpawns;
		foreach (EntitySpawnEntry entry in CollectOrGroups(entries, out orGroupedSpawns))
		{
			if ((entry.SpawnProbability == 1f || RandomExtensions.Prob(random, entry.SpawnProbability)) && entry.PrototypeId.HasValue)
			{
				int amount = (int)entry.GetAmount(random);
				for (int i = 0; i < amount; i++)
				{
					EntProtoId? prototypeId = entry.PrototypeId;
					spawned.Add(prototypeId.HasValue ? EntProtoId.op_Implicit(prototypeId.GetValueOrDefault()) : null);
				}
			}
		}
		foreach (OrGroup spawnValue in orGroupedSpawns)
		{
			double diceRoll = random.NextDouble() * (double)spawnValue.CumulativeProbability;
			double cumulative = 0.0;
			foreach (EntitySpawnEntry entry2 in spawnValue.Entries)
			{
				cumulative += (double)entry2.SpawnProbability;
				if (diceRoll > cumulative)
				{
					continue;
				}
				if (entry2.PrototypeId.HasValue)
				{
					int amount2 = (int)entry2.GetAmount(random);
					for (int j = 0; j < amount2; j++)
					{
						EntProtoId? prototypeId = entry2.PrototypeId;
						spawned.Add(prototypeId.HasValue ? EntProtoId.op_Implicit(prototypeId.GetValueOrDefault()) : null);
					}
				}
				break;
			}
		}
		return spawned;
	}

	public static List<string?> GetSpawns(IEnumerable<EntitySpawnEntry> entries, System.Random random)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		List<string> spawned = new List<string>();
		List<OrGroup> orGroupedSpawns;
		foreach (EntitySpawnEntry entry in CollectOrGroups(entries, out orGroupedSpawns))
		{
			if (entry.SpawnProbability == 1f || !(random.NextDouble() >= (double)entry.SpawnProbability))
			{
				int amount = (int)entry.GetAmount(random);
				for (int i = 0; i < amount; i++)
				{
					EntProtoId? prototypeId = entry.PrototypeId;
					spawned.Add(prototypeId.HasValue ? EntProtoId.op_Implicit(prototypeId.GetValueOrDefault()) : null);
				}
			}
		}
		foreach (OrGroup spawnValue in orGroupedSpawns)
		{
			double diceRoll = random.NextDouble() * (double)spawnValue.CumulativeProbability;
			double cumulative = 0.0;
			foreach (EntitySpawnEntry entry2 in spawnValue.Entries)
			{
				cumulative += (double)entry2.SpawnProbability;
				if (!(diceRoll > cumulative))
				{
					int amount2 = (int)entry2.GetAmount(random);
					for (int j = 0; j < amount2; j++)
					{
						EntProtoId? prototypeId = entry2.PrototypeId;
						spawned.Add(prototypeId.HasValue ? EntProtoId.op_Implicit(prototypeId.GetValueOrDefault()) : null);
					}
					break;
				}
			}
		}
		return spawned;
	}

	public static double GetAmount(this EntitySpawnEntry entry, System.Random random, bool getAverage = false)
	{
		if (entry.MaxAmount <= entry.Amount)
		{
			return entry.Amount;
		}
		if (getAverage)
		{
			return (double)(entry.Amount + entry.MaxAmount) / 2.0;
		}
		return random.Next(entry.Amount, entry.MaxAmount);
	}

	public static List<EntitySpawnEntry> CollectOrGroups(IEnumerable<EntitySpawnEntry> entries, out List<OrGroup> orGroups)
	{
		List<EntitySpawnEntry> ungrouped = new List<EntitySpawnEntry>();
		Dictionary<string, OrGroup> orGroupsDict = new Dictionary<string, OrGroup>();
		foreach (EntitySpawnEntry entry in entries)
		{
			if (!string.IsNullOrEmpty(entry.GroupId))
			{
				if (!orGroupsDict.TryGetValue(entry.GroupId, out var orGroup))
				{
					orGroup = new OrGroup();
					orGroupsDict.Add(entry.GroupId, orGroup);
				}
				orGroup.Entries.Add(entry);
				orGroup.CumulativeProbability += entry.SpawnProbability;
			}
			else
			{
				ungrouped.Add(entry);
			}
		}
		orGroups = orGroupsDict.Values.ToList();
		return ungrouped;
	}

	public static double GetAmount(this EntitySpawnEntry entry, IRobustRandom? random = null, bool getAverage = false)
	{
		if (entry.MaxAmount <= entry.Amount)
		{
			return entry.Amount;
		}
		if (getAverage)
		{
			return (double)(entry.Amount + entry.MaxAmount) / 2.0;
		}
		IoCManager.Resolve<IRobustRandom>(ref random);
		return random.Next(entry.Amount, entry.MaxAmount);
	}
}
