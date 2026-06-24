using System;
using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Trigger;

[RegisterComponent]
[Access(new Type[] { typeof(TimerTriggerVisualizerSystem) })]
public sealed class TimerTriggerVisualsComponent : Component, ISerializationGenerated<TimerTriggerVisualsComponent>, ISerializationGenerated
{
	[ViewVariables]
	public const string AnimationKey = "priming_animation";

	[DataField("unprimedSprite", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string UnprimedSprite = "icon";

	[DataField("primingSprite", false, 1, false, false, null)]
	public string PrimingSprite = "primed";

	[DataField("primingSound", false, 1, false, false, null)]
	public SoundSpecifier? PrimingSound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Animation PrimingAnimation;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TimerTriggerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (TimerTriggerVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<TimerTriggerVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string unprimedSprite = null;
			if (UnprimedSprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(UnprimedSprite, ref unprimedSprite, hookCtx, false, context))
			{
				unprimedSprite = UnprimedSprite;
			}
			target.UnprimedSprite = unprimedSprite;
			string primingSprite = null;
			if (PrimingSprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PrimingSprite, ref primingSprite, hookCtx, false, context))
			{
				primingSprite = PrimingSprite;
			}
			target.PrimingSprite = primingSprite;
			SoundSpecifier primingSound = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(PrimingSound, ref primingSound, hookCtx, true, context))
			{
				primingSound = serialization.CreateCopy<SoundSpecifier>(PrimingSound, hookCtx, context, false);
			}
			target.PrimingSound = primingSound;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TimerTriggerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TimerTriggerVisualsComponent target2 = (TimerTriggerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TimerTriggerVisualsComponent target2 = (TimerTriggerVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TimerTriggerVisualsComponent target2 = (TimerTriggerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TimerTriggerVisualsComponent Instantiate()
	{
		return new TimerTriggerVisualsComponent();
	}
}
