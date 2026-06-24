using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Bed.Sleep;

[RegisterComponent]
public sealed class SleepEmitSoundComponent : Component, ISerializationGenerated<SleepEmitSoundComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier Snore = (SoundSpecifier)new SoundCollectionSpecifier("Snores", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.2f));

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Interval = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan MaxInterval = TimeSpan.FromSeconds(15L);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public LocId PopUp = LocId.op_Implicit("sleep-onomatopoeia");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SleepEmitSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SleepEmitSoundComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SleepEmitSoundComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier SnoreTemp = null;
			if (Snore == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Snore, ref SnoreTemp, hookCtx, true, context))
			{
				SnoreTemp = serialization.CreateCopy<SoundSpecifier>(Snore, hookCtx, context, false);
			}
			target.Snore = SnoreTemp;
			TimeSpan IntervalTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Interval, ref IntervalTemp, hookCtx, false, context))
			{
				IntervalTemp = serialization.CreateCopy<TimeSpan>(Interval, hookCtx, context, false);
			}
			target.Interval = IntervalTemp;
			TimeSpan MaxIntervalTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MaxInterval, ref MaxIntervalTemp, hookCtx, false, context))
			{
				MaxIntervalTemp = serialization.CreateCopy<TimeSpan>(MaxInterval, hookCtx, context, false);
			}
			target.MaxInterval = MaxIntervalTemp;
			LocId PopUpTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(PopUp, ref PopUpTemp, hookCtx, false, context))
			{
				PopUpTemp = serialization.CreateCopy<LocId>(PopUp, hookCtx, context, false);
			}
			target.PopUp = PopUpTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SleepEmitSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SleepEmitSoundComponent cast = (SleepEmitSoundComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SleepEmitSoundComponent cast = (SleepEmitSoundComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SleepEmitSoundComponent def = (SleepEmitSoundComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SleepEmitSoundComponent Instantiate()
	{
		return new SleepEmitSoundComponent();
	}
}
