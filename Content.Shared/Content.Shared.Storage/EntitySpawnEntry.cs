using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage;

[Serializable]
[DataDefinition]
public struct EntitySpawnEntry : ISerializationGenerated<EntitySpawnEntry>, ISerializationGenerated
{
	[DataField("id", false, 1, false, false, null)]
	public EntProtoId? PrototypeId = null;

	[DataField("prob", false, 1, false, false, null)]
	public float SpawnProbability = 1f;

	[DataField("orGroup", false, 1, false, false, null)]
	public string? GroupId = null;

	[DataField(null, false, 1, false, false, null)]
	public int Amount = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxAmount = 1;

	public EntitySpawnEntry()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntitySpawnEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<EntitySpawnEntry>(this, ref target, hookCtx, false, context))
		{
			EntProtoId? PrototypeIdTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(PrototypeId, ref PrototypeIdTemp, hookCtx, false, context))
			{
				PrototypeIdTemp = serialization.CreateCopy<EntProtoId?>(PrototypeId, hookCtx, context, false);
			}
			float SpawnProbabilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpawnProbability, ref SpawnProbabilityTemp, hookCtx, false, context))
			{
				SpawnProbabilityTemp = SpawnProbability;
			}
			string GroupIdTemp = null;
			if (!serialization.TryCustomCopy<string>(GroupId, ref GroupIdTemp, hookCtx, false, context))
			{
				GroupIdTemp = GroupId;
			}
			int AmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			int MaxAmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxAmount, ref MaxAmountTemp, hookCtx, false, context))
			{
				MaxAmountTemp = MaxAmount;
			}
			EntitySpawnEntry entitySpawnEntry = target;
			entitySpawnEntry.PrototypeId = PrototypeIdTemp;
			entitySpawnEntry.SpawnProbability = SpawnProbabilityTemp;
			entitySpawnEntry.GroupId = GroupIdTemp;
			entitySpawnEntry.Amount = AmountTemp;
			entitySpawnEntry.MaxAmount = MaxAmountTemp;
			target = entitySpawnEntry;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntitySpawnEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntitySpawnEntry cast = (EntitySpawnEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public EntitySpawnEntry Instantiate()
	{
		return new EntitySpawnEntry();
	}
}
