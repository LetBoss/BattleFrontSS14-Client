using System;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Breathing : EventEntityEffectCondition<Breathing>, ISerializationGenerated<Breathing>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool IsBreathing = true;

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-breathing", new(string, object)[1] { ("isBreathing", IsBreathing) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Breathing target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffectCondition<Breathing> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Breathing)definitionCast;
		if (!serialization.TryCustomCopy<Breathing>(this, ref target, hookCtx, false, context))
		{
			bool IsBreathingTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsBreathing, ref IsBreathingTemp, hookCtx, false, context))
			{
				IsBreathingTemp = IsBreathing;
			}
			target.IsBreathing = IsBreathingTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Breathing target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffectCondition<Breathing> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Breathing cast = (Breathing)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Breathing cast = (Breathing)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Breathing Instantiate()
	{
		return new Breathing();
	}
}
