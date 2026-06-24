using System;
using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.Visualizers;

[RegisterComponent]
[Access(new Type[] { typeof(FoamVisualizerSystem) })]
public sealed class FoamVisualsComponent : Component, ISerializationGenerated<FoamVisualsComponent>, ISerializationGenerated
{
	public const string AnimationKey = "foamdissolve_animation";

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan StartTime;

	[DataField(null, false, 1, false, false, null)]
	public float AnimationTime = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public string AnimationState = "foam-dissolve";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Animation Animation;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoamVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (FoamVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<FoamVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan startTime = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StartTime, ref startTime, hookCtx, false, context))
			{
				startTime = serialization.CreateCopy<TimeSpan>(StartTime, hookCtx, context, false);
			}
			target.StartTime = startTime;
			float animationTime = 0f;
			if (!serialization.TryCustomCopy<float>(AnimationTime, ref animationTime, hookCtx, false, context))
			{
				animationTime = AnimationTime;
			}
			target.AnimationTime = animationTime;
			string animationState = null;
			if (AnimationState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(AnimationState, ref animationState, hookCtx, false, context))
			{
				animationState = AnimationState;
			}
			target.AnimationState = animationState;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoamVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoamVisualsComponent target2 = (FoamVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoamVisualsComponent target2 = (FoamVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoamVisualsComponent target2 = (FoamVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoamVisualsComponent Instantiate()
	{
		return new FoamVisualsComponent();
	}
}
