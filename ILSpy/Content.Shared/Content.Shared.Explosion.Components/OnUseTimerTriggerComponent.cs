using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Guidebook;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class OnUseTimerTriggerComponent : Component, ISerializationGenerated<OnUseTimerTriggerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Delay = 1f;

	[DataField(null, false, 1, false, false, null)]
	public List<float>? DelayOptions;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? BeepSound;

	[DataField(null, false, 1, false, false, null)]
	public float? InitialBeepDelay;

	[DataField(null, false, 1, false, false, null)]
	public float BeepInterval = 1f;

	[DataField(null, false, 1, false, false, null)]
	public bool UseVerbInstead;

	[DataField(null, false, 1, false, false, null)]
	public bool StartOnStick;

	[DataField("canToggleStartOnStick", false, 1, false, false, null)]
	public bool AllowToggleStartOnStick;

	[DataField(null, false, 1, false, false, null)]
	public bool Examinable = true;

	[DataField(null, false, 1, false, false, null)]
	public bool DoPopup = true;

	[GuidebookData]
	public float? ShortestDelayOption => DelayOptions?.Min();

	[GuidebookData]
	public float? LongestDelayOption => DelayOptions?.Max();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OnUseTimerTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OnUseTimerTriggerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<OnUseTimerTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			float DelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = Delay;
			}
			target.Delay = DelayTemp;
			List<float> DelayOptionsTemp = null;
			if (!serialization.TryCustomCopy<List<float>>(DelayOptions, ref DelayOptionsTemp, hookCtx, true, context))
			{
				DelayOptionsTemp = serialization.CreateCopy<List<float>>(DelayOptions, hookCtx, context, false);
			}
			target.DelayOptions = DelayOptionsTemp;
			SoundSpecifier BeepSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(BeepSound, ref BeepSoundTemp, hookCtx, true, context))
			{
				BeepSoundTemp = serialization.CreateCopy<SoundSpecifier>(BeepSound, hookCtx, context, false);
			}
			target.BeepSound = BeepSoundTemp;
			float? InitialBeepDelayTemp = null;
			if (!serialization.TryCustomCopy<float?>(InitialBeepDelay, ref InitialBeepDelayTemp, hookCtx, false, context))
			{
				InitialBeepDelayTemp = InitialBeepDelay;
			}
			target.InitialBeepDelay = InitialBeepDelayTemp;
			float BeepIntervalTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BeepInterval, ref BeepIntervalTemp, hookCtx, false, context))
			{
				BeepIntervalTemp = BeepInterval;
			}
			target.BeepInterval = BeepIntervalTemp;
			bool UseVerbInsteadTemp = false;
			if (!serialization.TryCustomCopy<bool>(UseVerbInstead, ref UseVerbInsteadTemp, hookCtx, false, context))
			{
				UseVerbInsteadTemp = UseVerbInstead;
			}
			target.UseVerbInstead = UseVerbInsteadTemp;
			bool StartOnStickTemp = false;
			if (!serialization.TryCustomCopy<bool>(StartOnStick, ref StartOnStickTemp, hookCtx, false, context))
			{
				StartOnStickTemp = StartOnStick;
			}
			target.StartOnStick = StartOnStickTemp;
			bool AllowToggleStartOnStickTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowToggleStartOnStick, ref AllowToggleStartOnStickTemp, hookCtx, false, context))
			{
				AllowToggleStartOnStickTemp = AllowToggleStartOnStick;
			}
			target.AllowToggleStartOnStick = AllowToggleStartOnStickTemp;
			bool ExaminableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Examinable, ref ExaminableTemp, hookCtx, false, context))
			{
				ExaminableTemp = Examinable;
			}
			target.Examinable = ExaminableTemp;
			bool DoPopupTemp = false;
			if (!serialization.TryCustomCopy<bool>(DoPopup, ref DoPopupTemp, hookCtx, false, context))
			{
				DoPopupTemp = DoPopup;
			}
			target.DoPopup = DoPopupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OnUseTimerTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OnUseTimerTriggerComponent cast = (OnUseTimerTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OnUseTimerTriggerComponent cast = (OnUseTimerTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OnUseTimerTriggerComponent def = (OnUseTimerTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OnUseTimerTriggerComponent Instantiate()
	{
		return new OnUseTimerTriggerComponent();
	}
}
