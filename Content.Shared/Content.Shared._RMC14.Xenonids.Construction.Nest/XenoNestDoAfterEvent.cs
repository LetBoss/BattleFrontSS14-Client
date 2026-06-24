using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Nest;

[Serializable]
[NetSerializable]
public sealed class XenoNestDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoNestDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool AllDirs;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoNestDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoNestDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoNestDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			bool AllDirsTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllDirs, ref AllDirsTemp, hookCtx, false, context))
			{
				AllDirsTemp = AllDirs;
			}
			target.AllDirs = AllDirsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoNestDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoNestDoAfterEvent cast = (XenoNestDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoNestDoAfterEvent cast = (XenoNestDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoNestDoAfterEvent Instantiate()
	{
		return new XenoNestDoAfterEvent();
	}
}
