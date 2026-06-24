using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Tools.Components;

[Serializable]
[NetSerializable]
public sealed class ToolOpenableDoAfterEventToggleOpen : SimpleDoAfterEvent, ISerializationGenerated<ToolOpenableDoAfterEventToggleOpen>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToolOpenableDoAfterEventToggleOpen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ToolOpenableDoAfterEventToggleOpen)definitionCast;
		serialization.TryCustomCopy<ToolOpenableDoAfterEventToggleOpen>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToolOpenableDoAfterEventToggleOpen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolOpenableDoAfterEventToggleOpen cast = (ToolOpenableDoAfterEventToggleOpen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolOpenableDoAfterEventToggleOpen cast = (ToolOpenableDoAfterEventToggleOpen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToolOpenableDoAfterEventToggleOpen Instantiate()
	{
		return new ToolOpenableDoAfterEventToggleOpen();
	}
}
