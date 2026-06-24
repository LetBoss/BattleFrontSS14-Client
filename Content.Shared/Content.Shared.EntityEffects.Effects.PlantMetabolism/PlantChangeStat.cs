using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantChangeStat : EventEntityEffect<PlantChangeStat>, ISerializationGenerated<PlantChangeStat>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string TargetValue;

	[DataField(null, false, 1, false, false, null)]
	public float MinValue;

	[DataField(null, false, 1, false, false, null)]
	public float MaxValue;

	[DataField(null, false, 1, false, false, null)]
	public int Steps;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		throw new NotImplementedException();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantChangeStat target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<PlantChangeStat> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantChangeStat)definitionCast;
		if (!serialization.TryCustomCopy<PlantChangeStat>(this, ref target, hookCtx, false, context))
		{
			string TargetValueTemp = null;
			if (TargetValue == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(TargetValue, ref TargetValueTemp, hookCtx, false, context))
			{
				TargetValueTemp = TargetValue;
			}
			target.TargetValue = TargetValueTemp;
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
			int StepsTemp = 0;
			if (!serialization.TryCustomCopy<int>(Steps, ref StepsTemp, hookCtx, false, context))
			{
				StepsTemp = Steps;
			}
			target.Steps = StepsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantChangeStat target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<PlantChangeStat> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantChangeStat cast = (PlantChangeStat)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantChangeStat cast = (PlantChangeStat)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantChangeStat Instantiate()
	{
		return new PlantChangeStat();
	}
}
