using System;
using System.Collections.Generic;
using Content.Shared.Light.Components;
using Robust.Client.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Light.Components;

[RegisterComponent]
public sealed class LightBehaviourComponent : SharedLightBehaviourComponent, ISerializationHooks, ISerializationGenerated<LightBehaviourComponent>, ISerializationGenerated
{
	public sealed class AnimationContainer
	{
		public string FullKey => "LightBehaviourComponent" + Key;

		public int Key { get; set; }

		public Animation Animation { get; set; }

		public LightBehaviourAnimationTrack LightBehaviour { get; set; }

		public AnimationContainer(int key, Animation animation, LightBehaviourAnimationTrack track)
		{
			Key = key;
			Animation = animation;
			LightBehaviour = track;
		}
	}

	public const string KeyPrefix = "LightBehaviourComponent";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("behaviours", false, 1, false, false, null)]
	public List<LightBehaviourAnimationTrack> Behaviours = new List<LightBehaviourAnimationTrack>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public readonly List<AnimationContainer> Animations = new List<AnimationContainer>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<string, object> OriginalPropertyValues = new Dictionary<string, object>();

	void ISerializationHooks.AfterDeserialization()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		int num = 0;
		foreach (LightBehaviourAnimationTrack behaviour in Behaviours)
		{
			Animation animation = new Animation
			{
				AnimationTracks = { (AnimationTrack)(object)behaviour }
			};
			Animations.Add(new AnimationContainer(num, animation, behaviour));
			num++;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LightBehaviourComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		SharedLightBehaviourComponent target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (LightBehaviourComponent)target2;
		if (!serialization.TryCustomCopy<LightBehaviourComponent>(this, ref target, hookCtx, true, context))
		{
			List<LightBehaviourAnimationTrack> behaviours = null;
			if (Behaviours == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<LightBehaviourAnimationTrack>>(Behaviours, ref behaviours, hookCtx, true, context))
			{
				behaviours = serialization.CreateCopy<List<LightBehaviourAnimationTrack>>(Behaviours, hookCtx, context, false);
			}
			target.Behaviours = behaviours;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LightBehaviourComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SharedLightBehaviourComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourComponent target2 = (LightBehaviourComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourComponent target2 = (LightBehaviourComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourComponent target2 = (LightBehaviourComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LightBehaviourComponent Instantiate()
	{
		return new LightBehaviourComponent();
	}
}
