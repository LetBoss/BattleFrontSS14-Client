using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed class ExtinguishOnInteractComponent : Component, ISerializationGenerated<ExtinguishOnInteractComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? ExtinguishAttemptSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/candle_blowing.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Probability = 0.9f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float StackDelta = -5f;

	[DataField(null, false, 1, false, false, null)]
	public LocId ExtinguishFailed = LocId.op_Implicit("candle-extinguish-failed");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExtinguishOnInteractComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExtinguishOnInteractComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ExtinguishOnInteractComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier ExtinguishAttemptSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(ExtinguishAttemptSound, ref ExtinguishAttemptSoundTemp, hookCtx, true, context))
			{
				ExtinguishAttemptSoundTemp = serialization.CreateCopy<SoundSpecifier>(ExtinguishAttemptSound, hookCtx, context, false);
			}
			target.ExtinguishAttemptSound = ExtinguishAttemptSoundTemp;
			float ProbabilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Probability, ref ProbabilityTemp, hookCtx, false, context))
			{
				ProbabilityTemp = Probability;
			}
			target.Probability = ProbabilityTemp;
			float StackDeltaTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StackDelta, ref StackDeltaTemp, hookCtx, false, context))
			{
				StackDeltaTemp = StackDelta;
			}
			target.StackDelta = StackDeltaTemp;
			LocId ExtinguishFailedTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ExtinguishFailed, ref ExtinguishFailedTemp, hookCtx, false, context))
			{
				ExtinguishFailedTemp = serialization.CreateCopy<LocId>(ExtinguishFailed, hookCtx, context, false);
			}
			target.ExtinguishFailed = ExtinguishFailedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExtinguishOnInteractComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtinguishOnInteractComponent cast = (ExtinguishOnInteractComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtinguishOnInteractComponent cast = (ExtinguishOnInteractComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtinguishOnInteractComponent def = (ExtinguishOnInteractComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExtinguishOnInteractComponent Instantiate()
	{
		return new ExtinguishOnInteractComponent();
	}
}
