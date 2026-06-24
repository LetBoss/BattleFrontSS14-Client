using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

[Serializable]
[NetSerializable]
public sealed class XenoPlaceParasiteInHoleDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoPlaceParasiteInHoleDoAfterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoPlaceParasiteInHoleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoPlaceParasiteInHoleDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<XenoPlaceParasiteInHoleDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoPlaceParasiteInHoleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlaceParasiteInHoleDoAfterEvent cast = (XenoPlaceParasiteInHoleDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlaceParasiteInHoleDoAfterEvent cast = (XenoPlaceParasiteInHoleDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoPlaceParasiteInHoleDoAfterEvent Instantiate()
	{
		return new XenoPlaceParasiteInHoleDoAfterEvent();
	}
}
