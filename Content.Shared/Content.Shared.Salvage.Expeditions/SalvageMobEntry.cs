using System;
using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Salvage.Expeditions;

[DataDefinition]
public record struct SalvageMobEntry() : IBudgetEntry, IProbEntry, ISerializationGenerated<SalvageMobEntry>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("cost", false, 1, false, false, null)]
	public float Cost { get; set; } = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("prob", false, 1, false, false, null)]
	public float Prob { get; set; } = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string Proto { get; set; } = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SalvageMobEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<SalvageMobEntry>(this, ref target, hookCtx, false, context))
		{
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
			string ProtoTemp = null;
			if (Proto == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Proto, ref ProtoTemp, hookCtx, false, context))
			{
				ProtoTemp = Proto;
			}
			target = target with
			{
				Cost = CostTemp,
				Prob = ProbTemp,
				Proto = ProtoTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SalvageMobEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageMobEntry cast = (SalvageMobEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SalvageMobEntry Instantiate()
	{
		return new SalvageMobEntry();
	}
}
