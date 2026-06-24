using System;
using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Chemistry.Visualizers;

[RegisterComponent]
[Access(new Type[] { typeof(VaporVisualizerSystem) })]
public sealed class VaporVisualsComponent : Component, ISerializationGenerated<VaporVisualsComponent>, ISerializationGenerated
{
	public const string AnimationKey = "flick_animation";

	[DataField("animationTime", false, 1, false, false, null)]
	public float AnimationTime = 0.25f;

	[DataField("animationState", false, 1, false, false, null)]
	public string AnimationState = "chempuff";

	public Animation VaporFlick;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VaporVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (VaporVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<VaporVisualsComponent>(this, ref target, hookCtx, false, context))
		{
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
	public void Copy(ref VaporVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VaporVisualsComponent target2 = (VaporVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VaporVisualsComponent target2 = (VaporVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VaporVisualsComponent target2 = (VaporVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VaporVisualsComponent Instantiate()
	{
		return new VaporVisualsComponent();
	}
}
