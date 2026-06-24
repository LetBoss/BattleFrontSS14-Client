using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class PlantMutateExudeGasses : EventEntityEffect<PlantMutateExudeGasses>, ISerializationGenerated<PlantMutateExudeGasses>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float MinValue = 0.01f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxValue = 0.5f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return "TODO";
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantMutateExudeGasses target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<PlantMutateExudeGasses> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantMutateExudeGasses)definitionCast;
		if (!serialization.TryCustomCopy<PlantMutateExudeGasses>(this, ref target, hookCtx, false, context))
		{
			float MinValueTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinValue, ref MinValueTemp, hookCtx, false, context))
			{
				MinValueTemp = MinValue;
			}
			target.MinValue = MinValueTemp;
			float MaxValueTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxValue, ref MaxValueTemp, hookCtx, false, context))
			{
				MaxValueTemp = MaxValue;
			}
			target.MaxValue = MaxValueTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantMutateExudeGasses target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<PlantMutateExudeGasses> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantMutateExudeGasses cast = (PlantMutateExudeGasses)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantMutateExudeGasses cast = (PlantMutateExudeGasses)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantMutateExudeGasses Instantiate()
	{
		return new PlantMutateExudeGasses();
	}
}
