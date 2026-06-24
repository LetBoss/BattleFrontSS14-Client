using System;
using Robust.Client.Animations;
using Robust.Shared.Animations;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Light.Components;

public sealed class FadeBehaviour : LightBehaviourAnimationTrack, ISerializationGenerated<FadeBehaviour>, ISerializationGenerated
{
	[DataField("reverseWhenFinished", false, 1, false, false, null)]
	public bool ReverseWhenFinished { get; set; }

	public override (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
	{
		float num = prevPlayingTime + frameTime;
		float num2 = num / base.MaxTime;
		if (Property == "AnimatedEnable")
		{
			ApplyProperty(num2 < base.EndValue);
			return (KeyFrameIndex: -1, FramePlayingTime: num);
		}
		if (ReverseWhenFinished)
		{
			if (num2 < 0.5f)
			{
				ApplyInterpolation(base.StartValue, base.EndValue, num2 * 2f);
			}
			else
			{
				ApplyInterpolation(base.EndValue, base.StartValue, (num2 - 0.5f) * 2f);
			}
		}
		else
		{
			ApplyInterpolation(base.StartValue, base.EndValue, num2);
		}
		return (KeyFrameIndex: -1, FramePlayingTime: num);
	}

	private void ApplyInterpolation(float start, float end, float interpolateValue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected I4, but got Unknown
		AnimationInterpolationMode interpolateMode = base.InterpolateMode;
		switch ((int)interpolateMode)
		{
		case 0:
			ApplyProperty(AnimationTrackProperty.InterpolateLinear((object)start, (object)end, interpolateValue));
			break;
		case 1:
			ApplyProperty(AnimationTrackProperty.InterpolateCubic((object)end, (object)start, (object)end, (object)start, interpolateValue));
			break;
		default:
			ApplyProperty((interpolateValue < 0.5f) ? start : end);
			break;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FadeBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourAnimationTrack target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (FadeBehaviour)target2;
		if (!serialization.TryCustomCopy<FadeBehaviour>(this, ref target, hookCtx, false, context))
		{
			bool reverseWhenFinished = false;
			if (!serialization.TryCustomCopy<bool>(ReverseWhenFinished, ref reverseWhenFinished, hookCtx, false, context))
			{
				reverseWhenFinished = ReverseWhenFinished;
			}
			target.ReverseWhenFinished = reverseWhenFinished;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FadeBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LightBehaviourAnimationTrack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FadeBehaviour target2 = (FadeBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FadeBehaviour target2 = (FadeBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FadeBehaviour Instantiate()
	{
		return new FadeBehaviour();
	}
}
