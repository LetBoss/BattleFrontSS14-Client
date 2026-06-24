using System;
using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Procedural.Loot;

[DataDefinition]
public record struct RandomSpawnLootEntry() : IBudgetEntry, IProbEntry, ISerializationGenerated<RandomSpawnLootEntry>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string Proto { get; set; } = string.Empty;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("cost", false, 1, false, false, null)]
	public float Cost { get; set; } = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("prob", false, 1, false, false, null)]
	public float Prob { get; set; } = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomSpawnLootEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RandomSpawnLootEntry>(this, ref target, hookCtx, false, context))
		{
			string ProtoTemp = null;
			if (Proto == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Proto, ref ProtoTemp, hookCtx, false, context))
			{
				ProtoTemp = Proto;
			}
			float CostTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Cost, ref CostTemp, hookCtx, false, context))
			{
				CostTemp = Cost;
			}
			float ProbTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Prob, ref ProbTemp, hookCtx, false, context))
			{
				ProbTemp = Prob;
			}
			target = target with
			{
				Proto = ProtoTemp,
				Cost = CostTemp,
				Prob = ProbTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomSpawnLootEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomSpawnLootEntry cast = (RandomSpawnLootEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RandomSpawnLootEntry Instantiate()
	{
		return new RandomSpawnLootEntry();
	}
}
