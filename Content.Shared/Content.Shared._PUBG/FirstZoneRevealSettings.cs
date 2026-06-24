using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class FirstZoneRevealSettings : ISerializationGenerated<FirstZoneRevealSettings>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Enabled;

	[DataField(null, false, 1, false, false, null)]
	public float DelaySeconds;

	[DataField(null, false, 1, false, false, null)]
	public bool RandomizeCenter = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FirstZoneRevealSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<FirstZoneRevealSettings>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			float DelaySecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DelaySeconds, ref DelaySecondsTemp, hookCtx, false, context))
			{
				DelaySecondsTemp = DelaySeconds;
			}
			target.DelaySeconds = DelaySecondsTemp;
			bool RandomizeCenterTemp = false;
			if (!serialization.TryCustomCopy<bool>(RandomizeCenter, ref RandomizeCenterTemp, hookCtx, false, context))
			{
				RandomizeCenterTemp = RandomizeCenter;
			}
			target.RandomizeCenter = RandomizeCenterTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FirstZoneRevealSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FirstZoneRevealSettings cast = (FirstZoneRevealSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public FirstZoneRevealSettings Instantiate()
	{
		return new FirstZoneRevealSettings();
	}
}
