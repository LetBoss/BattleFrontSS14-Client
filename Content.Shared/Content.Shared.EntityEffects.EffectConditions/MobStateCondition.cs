using System;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class MobStateCondition : EntityEffectCondition, ISerializationGenerated<MobStateCondition>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public MobState Mobstate = MobState.Alive;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (args.EntityManager.TryGetComponent<MobStateComponent>(args.TargetEntity, ref mobState) && mobState.CurrentState == Mobstate)
		{
			return true;
		}
		return false;
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-mob-state-condition", new(string, object)[1] { ("state", Mobstate) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MobStateCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MobStateCondition)definitionCast;
		if (!serialization.TryCustomCopy<MobStateCondition>(this, ref target, hookCtx, false, context))
		{
			MobState MobstateTemp = MobState.Invalid;
			if (!serialization.TryCustomCopy<MobState>(Mobstate, ref MobstateTemp, hookCtx, false, context))
			{
				MobstateTemp = Mobstate;
			}
			target.Mobstate = MobstateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MobStateCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobStateCondition cast = (MobStateCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobStateCondition cast = (MobStateCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MobStateCondition Instantiate()
	{
		return new MobStateCondition();
	}
}
