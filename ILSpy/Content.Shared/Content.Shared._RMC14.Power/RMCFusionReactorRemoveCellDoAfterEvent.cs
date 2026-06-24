using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Power;

[Serializable]
[NetSerializable]
public sealed class RMCFusionReactorRemoveCellDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<RMCFusionReactorRemoveCellDoAfterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCFusionReactorRemoveCellDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCFusionReactorRemoveCellDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<RMCFusionReactorRemoveCellDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCFusionReactorRemoveCellDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFusionReactorRemoveCellDoAfterEvent cast = (RMCFusionReactorRemoveCellDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFusionReactorRemoveCellDoAfterEvent cast = (RMCFusionReactorRemoveCellDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCFusionReactorRemoveCellDoAfterEvent Instantiate()
	{
		return new RMCFusionReactorRemoveCellDoAfterEvent();
	}
}
