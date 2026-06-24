using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Flash.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class FlashOnTriggerComponent : Component, ISerializationGenerated<FlashOnTriggerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Range = 1f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Duration = TimeSpan.FromSeconds(8L);

	[DataField(null, false, 1, false, false, null)]
	public float Probability = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FlashOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FlashOnTriggerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FlashOnTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			TimeSpan DurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = serialization.CreateCopy<TimeSpan>(Duration, hookCtx, context, false);
			}
			target.Duration = DurationTemp;
			float ProbabilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Probability, ref ProbabilityTemp, hookCtx, false, context))
			{
				ProbabilityTemp = Probability;
			}
			target.Probability = ProbabilityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FlashOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlashOnTriggerComponent cast = (FlashOnTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlashOnTriggerComponent cast = (FlashOnTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlashOnTriggerComponent def = (FlashOnTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FlashOnTriggerComponent Instantiate()
	{
		return new FlashOnTriggerComponent();
	}
}
