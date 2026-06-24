using System;
using System.Collections.Generic;
using Robust.Client.Animations;
using Robust.Shared.Animations;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Light.Components;

[DataDefinition]
public sealed class ColorCycleBehaviour : LightBehaviourAnimationTrack, ISerializationHooks, ISerializationGenerated<ColorCycleBehaviour>, ISerializationGenerated
{
	private int _colorIndex;

	[DataField("property", false, 1, false, false, null)]
	public override string Property { get; protected set; } = "Color";

	[DataField("colors", false, 1, false, false, null)]
	public List<Color> ColorsToCycle { get; set; } = new List<Color>();

	public override void OnStart()
	{
		_colorIndex++;
		if (_colorIndex > ColorsToCycle.Count - 1)
		{
			_colorIndex = 0;
		}
	}

	public override (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		float num = prevPlayingTime + frameTime;
		float num2 = num / base.MaxTime;
		AnimationInterpolationMode interpolateMode = base.InterpolateMode;
		switch ((int)interpolateMode)
		{
		case 0:
			ApplyProperty(AnimationTrackProperty.InterpolateLinear((object)ColorsToCycle[(_colorIndex - 1) % ColorsToCycle.Count], (object)ColorsToCycle[_colorIndex], num2));
			break;
		case 1:
			ApplyProperty(AnimationTrackProperty.InterpolateCubic((object)ColorsToCycle[_colorIndex], (object)ColorsToCycle[(_colorIndex + 1) % ColorsToCycle.Count], (object)ColorsToCycle[(_colorIndex + 2) % ColorsToCycle.Count], (object)ColorsToCycle[(_colorIndex + 3) % ColorsToCycle.Count], num2));
			break;
		default:
			ApplyProperty(ColorsToCycle[_colorIndex]);
			break;
		}
		return (KeyFrameIndex: -1, FramePlayingTime: num);
	}

	void ISerializationHooks.AfterDeserialization()
	{
		if (ColorsToCycle.Count < 2)
		{
			throw new InvalidOperationException("ColorCycleBehaviour has less than 2 colors to cycle");
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ColorCycleBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		LightBehaviourAnimationTrack target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (ColorCycleBehaviour)target2;
		if (!serialization.TryCustomCopy<ColorCycleBehaviour>(this, ref target, hookCtx, true, context))
		{
			string property = null;
			if (Property == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Property, ref property, hookCtx, false, context))
			{
				property = Property;
			}
			target.Property = property;
			List<Color> colorsToCycle = null;
			if (ColorsToCycle == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<Color>>(ColorsToCycle, ref colorsToCycle, hookCtx, true, context))
			{
				colorsToCycle = serialization.CreateCopy<List<Color>>(ColorsToCycle, hookCtx, context, false);
			}
			target.ColorsToCycle = colorsToCycle;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ColorCycleBehaviour target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LightBehaviourAnimationTrack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ColorCycleBehaviour target2 = (ColorCycleBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ColorCycleBehaviour target2 = (ColorCycleBehaviour)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ColorCycleBehaviour Instantiate()
	{
		return new ColorCycleBehaviour();
	}
}
