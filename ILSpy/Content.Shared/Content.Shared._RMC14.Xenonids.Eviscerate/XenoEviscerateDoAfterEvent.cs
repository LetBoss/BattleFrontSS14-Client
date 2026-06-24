using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Eviscerate;

[Serializable]
[NetSerializable]
public sealed class XenoEviscerateDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoEviscerateDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Rage;

	public XenoEviscerateDoAfterEvent(int rage)
	{
		Rage = rage;
	}

	public XenoEviscerateDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoEviscerateDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoEviscerateDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoEviscerateDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			int RageTemp = 0;
			if (!serialization.TryCustomCopy<int>(Rage, ref RageTemp, hookCtx, false, context))
			{
				RageTemp = Rage;
			}
			target.Rage = RageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoEviscerateDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoEviscerateDoAfterEvent cast = (XenoEviscerateDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoEviscerateDoAfterEvent cast = (XenoEviscerateDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoEviscerateDoAfterEvent Instantiate()
	{
		return new XenoEviscerateDoAfterEvent();
	}
}
