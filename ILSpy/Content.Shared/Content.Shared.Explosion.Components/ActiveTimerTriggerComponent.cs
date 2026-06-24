using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Explosion.Components;

[RegisterComponent]
public sealed class ActiveTimerTriggerComponent : Component, ISerializationGenerated<ActiveTimerTriggerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float TimeRemaining;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? User;

	[DataField(null, false, 1, false, false, null)]
	public float BeepInterval;

	[DataField(null, false, 1, false, false, null)]
	public float TimeUntilBeep;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? BeepSound;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActiveTimerTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActiveTimerTriggerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActiveTimerTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			float TimeRemainingTemp = 0f;
			if (!serialization.TryCustomCopy<float>(TimeRemaining, ref TimeRemainingTemp, hookCtx, false, context))
			{
				TimeRemainingTemp = TimeRemaining;
			}
			target.TimeRemaining = TimeRemainingTemp;
			EntityUid? UserTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(User, ref UserTemp, hookCtx, false, context))
			{
				UserTemp = serialization.CreateCopy<EntityUid?>(User, hookCtx, context, false);
			}
			target.User = UserTemp;
			float BeepIntervalTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BeepInterval, ref BeepIntervalTemp, hookCtx, false, context))
			{
				BeepIntervalTemp = BeepInterval;
			}
			target.BeepInterval = BeepIntervalTemp;
			float TimeUntilBeepTemp = 0f;
			if (!serialization.TryCustomCopy<float>(TimeUntilBeep, ref TimeUntilBeepTemp, hookCtx, false, context))
			{
				TimeUntilBeepTemp = TimeUntilBeep;
			}
			target.TimeUntilBeep = TimeUntilBeepTemp;
			SoundSpecifier BeepSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(BeepSound, ref BeepSoundTemp, hookCtx, true, context))
			{
				BeepSoundTemp = serialization.CreateCopy<SoundSpecifier>(BeepSound, hookCtx, context, false);
			}
			target.BeepSound = BeepSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActiveTimerTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveTimerTriggerComponent cast = (ActiveTimerTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveTimerTriggerComponent cast = (ActiveTimerTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveTimerTriggerComponent def = (ActiveTimerTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActiveTimerTriggerComponent Instantiate()
	{
		return new ActiveTimerTriggerComponent();
	}
}
