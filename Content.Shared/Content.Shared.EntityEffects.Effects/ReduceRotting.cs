using System;
using Content.Shared.Atmos.Rotting;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ReduceRotting : EntityEffect, ISerializationGenerated<ReduceRotting>, ISerializationGenerated
{
	[DataField("seconds", false, 1, false, false, null)]
	public double RottingAmount = 10.0;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-reduce-rotting", new(string, object)[2]
		{
			("chance", Probability),
			("time", RottingAmount)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!(args is EntityEffectReagentArgs reagentArgs) || !(reagentArgs.Scale != 1f))
		{
			args.EntityManager.EntitySysManager.GetEntitySystem<SharedRottingSystem>().ReduceAccumulator(args.TargetEntity, TimeSpan.FromSeconds(RottingAmount));
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReduceRotting target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReduceRotting)definitionCast;
		if (!serialization.TryCustomCopy<ReduceRotting>(this, ref target, hookCtx, false, context))
		{
			double RottingAmountTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(RottingAmount, ref RottingAmountTemp, hookCtx, false, context))
			{
				RottingAmountTemp = RottingAmount;
			}
			target.RottingAmount = RottingAmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReduceRotting target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReduceRotting cast = (ReduceRotting)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReduceRotting cast = (ReduceRotting)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReduceRotting Instantiate()
	{
		return new ReduceRotting();
	}
}
