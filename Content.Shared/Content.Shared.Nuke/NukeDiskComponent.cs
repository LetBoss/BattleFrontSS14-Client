using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Nuke;

[RegisterComponent]
[NetworkedComponent]
public sealed class NukeDiskComponent : Component, ISerializationGenerated<NukeDiskComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? TimeModifier;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MicrowaveMean = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MicrowaveStd = TimeSpan.FromSeconds(27.35);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NukeDiskComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NukeDiskComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NukeDiskComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan? TimeModifierTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(TimeModifier, ref TimeModifierTemp, hookCtx, false, context))
			{
				TimeModifierTemp = serialization.CreateCopy<TimeSpan?>(TimeModifier, hookCtx, context, false);
			}
			target.TimeModifier = TimeModifierTemp;
			TimeSpan MicrowaveMeanTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MicrowaveMean, ref MicrowaveMeanTemp, hookCtx, false, context))
			{
				MicrowaveMeanTemp = serialization.CreateCopy<TimeSpan>(MicrowaveMean, hookCtx, context, false);
			}
			target.MicrowaveMean = MicrowaveMeanTemp;
			TimeSpan MicrowaveStdTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MicrowaveStd, ref MicrowaveStdTemp, hookCtx, false, context))
			{
				MicrowaveStdTemp = serialization.CreateCopy<TimeSpan>(MicrowaveStd, hookCtx, context, false);
			}
			target.MicrowaveStd = MicrowaveStdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NukeDiskComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NukeDiskComponent cast = (NukeDiskComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NukeDiskComponent cast = (NukeDiskComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NukeDiskComponent def = (NukeDiskComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NukeDiskComponent Instantiate()
	{
		return new NukeDiskComponent();
	}
}
