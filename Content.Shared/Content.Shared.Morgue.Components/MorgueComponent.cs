using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Morgue.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MorgueComponent : Component, ISerializationGenerated<MorgueComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool DoSoulBeep = true;

	[DataField(null, false, 1, false, false, null)]
	public float AccumulatedFrameTime;

	[DataField(null, false, 1, false, false, null)]
	public float BeepTime = 10f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier OccupantHasSoulAlarmSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MorgueComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MorgueComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MorgueComponent>(this, ref target, hookCtx, false, context))
		{
			bool DoSoulBeepTemp = false;
			if (!serialization.TryCustomCopy<bool>(DoSoulBeep, ref DoSoulBeepTemp, hookCtx, false, context))
			{
				DoSoulBeepTemp = DoSoulBeep;
			}
			target.DoSoulBeep = DoSoulBeepTemp;
			float AccumulatedFrameTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(AccumulatedFrameTime, ref AccumulatedFrameTimeTemp, hookCtx, false, context))
			{
				AccumulatedFrameTimeTemp = AccumulatedFrameTime;
			}
			target.AccumulatedFrameTime = AccumulatedFrameTimeTemp;
			float BeepTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BeepTime, ref BeepTimeTemp, hookCtx, false, context))
			{
				BeepTimeTemp = BeepTime;
			}
			target.BeepTime = BeepTimeTemp;
			SoundSpecifier OccupantHasSoulAlarmSoundTemp = null;
			if (OccupantHasSoulAlarmSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(OccupantHasSoulAlarmSound, ref OccupantHasSoulAlarmSoundTemp, hookCtx, true, context))
			{
				OccupantHasSoulAlarmSoundTemp = serialization.CreateCopy<SoundSpecifier>(OccupantHasSoulAlarmSound, hookCtx, context, false);
			}
			target.OccupantHasSoulAlarmSound = OccupantHasSoulAlarmSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MorgueComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MorgueComponent cast = (MorgueComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MorgueComponent cast = (MorgueComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MorgueComponent def = (MorgueComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MorgueComponent Instantiate()
	{
		return new MorgueComponent();
	}
}
