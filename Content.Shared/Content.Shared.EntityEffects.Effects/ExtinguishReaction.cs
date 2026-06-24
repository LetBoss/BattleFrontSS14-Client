using System;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ExtinguishReaction : EntityEffect, ISerializationGenerated<ExtinguishReaction>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float FireStacksAdjustment = -1.5f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-extinguish-reaction", new(string, object)[1] { ("chance", Probability) });
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		ExtinguishEvent ev = new ExtinguishEvent
		{
			FireStacksAdjustment = FireStacksAdjustment
		};
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			ev.FireStacksAdjustment *= (float)reagentArgs.Quantity;
		}
		((IDirectedEventBus)args.EntityManager.EventBus).RaiseLocalEvent<ExtinguishEvent>(args.TargetEntity, ref ev, false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExtinguishReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExtinguishReaction)definitionCast;
		if (!serialization.TryCustomCopy<ExtinguishReaction>(this, ref target, hookCtx, false, context))
		{
			float FireStacksAdjustmentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FireStacksAdjustment, ref FireStacksAdjustmentTemp, hookCtx, false, context))
			{
				FireStacksAdjustmentTemp = FireStacksAdjustment;
			}
			target.FireStacksAdjustment = FireStacksAdjustmentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExtinguishReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtinguishReaction cast = (ExtinguishReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtinguishReaction cast = (ExtinguishReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExtinguishReaction Instantiate()
	{
		return new ExtinguishReaction();
	}
}
