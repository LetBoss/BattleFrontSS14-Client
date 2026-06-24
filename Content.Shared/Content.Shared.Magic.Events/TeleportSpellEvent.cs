using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Magic.Events;

public sealed class TeleportSpellEvent : WorldTargetActionEvent, ISerializationGenerated<TeleportSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float BlinkVolume = 5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TeleportSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TeleportSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<TeleportSpellEvent>(this, ref target, hookCtx, false, context))
		{
			float BlinkVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BlinkVolume, ref BlinkVolumeTemp, hookCtx, false, context))
			{
				BlinkVolumeTemp = BlinkVolume;
			}
			target.BlinkVolume = BlinkVolumeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TeleportSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TeleportSpellEvent cast = (TeleportSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TeleportSpellEvent cast = (TeleportSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TeleportSpellEvent Instantiate()
	{
		return new TeleportSpellEvent();
	}
}
