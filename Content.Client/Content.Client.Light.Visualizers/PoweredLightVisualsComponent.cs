using System;
using System.Collections.Generic;
using Content.Shared.Light;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Light.Visualizers;

[RegisterComponent]
[Access(new Type[] { typeof(PoweredLightVisualizerSystem) })]
public sealed class PoweredLightVisualsComponent : Component, ISerializationGenerated<PoweredLightVisualsComponent>, ISerializationGenerated
{
	[DataField("spriteStateMap", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<PoweredLightState, string> SpriteStateMap = new Dictionary<PoweredLightState, string>
	{
		[PoweredLightState.Empty] = "empty",
		[PoweredLightState.Off] = "off",
		[PoweredLightState.On] = "on",
		[PoweredLightState.Broken] = "broken",
		[PoweredLightState.Burned] = "burn"
	};

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public const string BlinkingAnimationKey = "poweredlight_blinking";

	[DataField("minBlinkingTime", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MinBlinkingAnimationCycleTime = 0.5f;

	[DataField("maxBlinkingTime", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxBlinkingAnimationCycleTime = 2f;

	[DataField("blinkingSound", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier? BlinkingSound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool IsBlinking;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PoweredLightVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PoweredLightVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<PoweredLightVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<PoweredLightState, string> spriteStateMap = null;
			if (SpriteStateMap == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<PoweredLightState, string>>(SpriteStateMap, ref spriteStateMap, hookCtx, true, context))
			{
				spriteStateMap = serialization.CreateCopy<Dictionary<PoweredLightState, string>>(SpriteStateMap, hookCtx, context, false);
			}
			target.SpriteStateMap = spriteStateMap;
			float minBlinkingAnimationCycleTime = 0f;
			if (!serialization.TryCustomCopy<float>(MinBlinkingAnimationCycleTime, ref minBlinkingAnimationCycleTime, hookCtx, false, context))
			{
				minBlinkingAnimationCycleTime = MinBlinkingAnimationCycleTime;
			}
			target.MinBlinkingAnimationCycleTime = minBlinkingAnimationCycleTime;
			float maxBlinkingAnimationCycleTime = 0f;
			if (!serialization.TryCustomCopy<float>(MaxBlinkingAnimationCycleTime, ref maxBlinkingAnimationCycleTime, hookCtx, false, context))
			{
				maxBlinkingAnimationCycleTime = MaxBlinkingAnimationCycleTime;
			}
			target.MaxBlinkingAnimationCycleTime = maxBlinkingAnimationCycleTime;
			SoundSpecifier blinkingSound = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(BlinkingSound, ref blinkingSound, hookCtx, true, context))
			{
				blinkingSound = serialization.CreateCopy<SoundSpecifier>(BlinkingSound, hookCtx, context, false);
			}
			target.BlinkingSound = blinkingSound;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PoweredLightVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PoweredLightVisualsComponent target2 = (PoweredLightVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PoweredLightVisualsComponent target2 = (PoweredLightVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PoweredLightVisualsComponent target2 = (PoweredLightVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PoweredLightVisualsComponent Instantiate()
	{
		return new PoweredLightVisualsComponent();
	}
}
