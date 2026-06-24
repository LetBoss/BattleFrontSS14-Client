using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class RobustHarvest : EventEntityEffect<RobustHarvest>, ISerializationGenerated<RobustHarvest>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int PotencyLimit = 50;

	[DataField(null, false, 1, false, false, null)]
	public int PotencyIncrease = 3;

	[DataField(null, false, 1, false, false, null)]
	public int PotencySeedlessThreshold = 30;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-plant-robust-harvest", new(string, object)[4]
		{
			("seedlesstreshold", PotencySeedlessThreshold),
			("limit", PotencyLimit),
			("increase", PotencyIncrease),
			("chance", Probability)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RobustHarvest target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<RobustHarvest> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RobustHarvest)definitionCast;
		if (!serialization.TryCustomCopy<RobustHarvest>(this, ref target, hookCtx, false, context))
		{
			int PotencyLimitTemp = 0;
			if (!serialization.TryCustomCopy<int>(PotencyLimit, ref PotencyLimitTemp, hookCtx, false, context))
			{
				PotencyLimitTemp = PotencyLimit;
			}
			target.PotencyLimit = PotencyLimitTemp;
			int PotencyIncreaseTemp = 0;
			if (!serialization.TryCustomCopy<int>(PotencyIncrease, ref PotencyIncreaseTemp, hookCtx, false, context))
			{
				PotencyIncreaseTemp = PotencyIncrease;
			}
			target.PotencyIncrease = PotencyIncreaseTemp;
			int PotencySeedlessThresholdTemp = 0;
			if (!serialization.TryCustomCopy<int>(PotencySeedlessThreshold, ref PotencySeedlessThresholdTemp, hookCtx, false, context))
			{
				PotencySeedlessThresholdTemp = PotencySeedlessThreshold;
			}
			target.PotencySeedlessThreshold = PotencySeedlessThresholdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RobustHarvest target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<RobustHarvest> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RobustHarvest cast = (RobustHarvest)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RobustHarvest cast = (RobustHarvest)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RobustHarvest Instantiate()
	{
		return new RobustHarvest();
	}
}
