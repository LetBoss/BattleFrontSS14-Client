using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Procedural.Loot;

public sealed class RandomSpawnsLoot : IDungeonLoot, ISerializationGenerated<IDungeonLoot>, ISerializationGenerated, ISerializationGenerated<RandomSpawnsLoot>
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("entries", false, 1, true, false, null)]
	public List<RandomSpawnLootEntry> Entries = new List<RandomSpawnLootEntry>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomSpawnsLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RandomSpawnsLoot>(this, ref target, hookCtx, false, context))
		{
			List<RandomSpawnLootEntry> EntriesTemp = null;
			if (Entries == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<RandomSpawnLootEntry>>(Entries, ref EntriesTemp, hookCtx, true, context))
			{
				EntriesTemp = serialization.CreateCopy<List<RandomSpawnLootEntry>>(Entries, hookCtx, context, false);
			}
			target.Entries = EntriesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomSpawnsLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomSpawnsLoot cast = (RandomSpawnsLoot)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDungeonLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomSpawnsLoot def = (RandomSpawnsLoot)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDungeonLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RandomSpawnsLoot Instantiate()
	{
		return new RandomSpawnsLoot();
	}

	IDungeonLoot IDungeonLoot.Instantiate()
	{
		return Instantiate();
	}

	IDungeonLoot ISerializationGenerated<IDungeonLoot>.Instantiate()
	{
		return Instantiate();
	}
}
