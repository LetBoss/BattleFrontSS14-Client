using System;
using Robust.Client.Animations;
using Robust.Shared.Animations;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Light.Components;

public sealed class RandomizeBehaviour : LightBehaviourAnimationTrack, ISerializationGenerated<RandomizeBehaviour>, ISerializationGenerated
{
	private float _randomValue1;

	private float _randomValue2;

	private float _randomValue3;

	private float _randomValue4;

	public override void OnInitialize()
	{
		_randomValue1 = (float)AnimationTrackProperty.InterpolateLinear((object)base.StartValue, (object)base.EndValue, (float)_random.NextDouble());
		_randomValue2 = (float)AnimationTrackProperty.InterpolateLinear((object)base.StartValue, (object)base.EndValue, (float)_random.NextDouble());
		_randomValue3 = (float)AnimationTrackProperty.InterpolateLinear((object)base.StartValue, (object)base.EndValue, (float)_random.NextDouble());
	}

	public override void OnStart()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		if (Property == "AnimatedEnable")
		{
			ApplyProperty(_random.NextDouble() < 0.5);
			return;
		}
		if ((int)base.InterpolateMode == 1)
		{
			_randomValue1 = _randomValue2;
			_randomValue2 = _randomValue3;
		}
		_randomValue3 = _randomValue4;
		_randomValue4 = (float)AnimationTrackProperty.InterpolateLinear((object)base.StartValue, (object)base.EndValue, (float)_random.NextDouble());
	}

	public override (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		float num = prevPlayingTime + frameTime;
		float num2 = num / base.MaxTime;
		if (Property == "AnimatedEnable")
		{
			return (KeyFrameIndex: -1, FramePlayingTime: num);
		}
		AnimationInterpolationMode interpolateMode = base.InterpolateMode;
		switch ((int)interpolateMode)
		{
		case 0:
			ApplyProperty(AnimationTrackProperty.InterpolateLinear((object)_randomValue3, (object)_randomValue4, num2));
			break;
		case 1:
			ApplyProperty(AnimationTrackProperty.InterpolateCubic((object)_randomValue1, (object)_randomValue2, (object)_randomValue3, (object)_randomValue4, num2));
			break;
		default:
			ApplyProperty((num2 < 0.5f) ? _randomValue3 : _randomValue4);
			break;
		}
		return (KeyFrameIndex: -1, FramePlayingTime: num);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomizeBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourAnimationTrack target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (RandomizeBehaviour)target2;
		serialization.TryCustomCopy<RandomizeBehaviour>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomizeBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LightBehaviourAnimationTrack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomizeBehaviour target2 = (RandomizeBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomizeBehaviour target2 = (RandomizeBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RandomizeBehaviour Instantiate()
	{
		return new RandomizeBehaviour();
	}
}
