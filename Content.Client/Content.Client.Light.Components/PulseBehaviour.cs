using System;
using Robust.Client.Animations;
using Robust.Shared.Animations;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Light.Components;

public sealed class PulseBehaviour : LightBehaviourAnimationTrack, ISerializationGenerated<PulseBehaviour>, ISerializationGenerated
{
	public override (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected I4, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected I4, but got Unknown
		float num = prevPlayingTime + frameTime;
		float num2 = num / base.MaxTime;
		if (Property == "AnimatedEnable")
		{
			ApplyProperty(num2 < 0.5f);
			return (KeyFrameIndex: -1, FramePlayingTime: num);
		}
		if (num2 < 0.5f)
		{
			AnimationInterpolationMode interpolateMode = base.InterpolateMode;
			switch ((int)interpolateMode)
			{
			case 0:
				ApplyProperty(AnimationTrackProperty.InterpolateLinear((object)base.StartValue, (object)base.EndValue, num2 * 2f));
				break;
			case 1:
				ApplyProperty(AnimationTrackProperty.InterpolateCubic((object)base.EndValue, (object)base.StartValue, (object)base.EndValue, (object)base.StartValue, num2 * 2f));
				break;
			default:
				ApplyProperty(base.StartValue);
				break;
			}
		}
		else
		{
			AnimationInterpolationMode interpolateMode = base.InterpolateMode;
			switch ((int)interpolateMode)
			{
			case 0:
				ApplyProperty(AnimationTrackProperty.InterpolateLinear((object)base.EndValue, (object)base.StartValue, (num2 - 0.5f) * 2f));
				break;
			case 1:
				ApplyProperty(AnimationTrackProperty.InterpolateCubic((object)base.StartValue, (object)base.EndValue, (object)base.StartValue, (object)base.EndValue, (num2 - 0.5f) * 2f));
				break;
			default:
				ApplyProperty(base.EndValue);
				break;
			}
		}
		return (KeyFrameIndex: -1, FramePlayingTime: num);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PulseBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourAnimationTrack target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (PulseBehaviour)target2;
		serialization.TryCustomCopy<PulseBehaviour>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PulseBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LightBehaviourAnimationTrack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PulseBehaviour target2 = (PulseBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PulseBehaviour target2 = (PulseBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PulseBehaviour Instantiate()
	{
		return new PulseBehaviour();
	}
}
