using System;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.Burrow;

[Serializable]
[NetSerializable]
public sealed class XenoBurrowMoveDoAfter : SimpleDoAfterEvent, ISerializationGenerated<XenoBurrowMoveDoAfter>, ISerializationGenerated
{
	public NetCoordinates TargetCoords;

	public XenoBurrowMoveDoAfter(NetCoordinates targetCoords)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetCoords = targetCoords;
	}

	public XenoBurrowMoveDoAfter()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoBurrowMoveDoAfter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoBurrowMoveDoAfter)definitionCast;
		serialization.TryCustomCopy<XenoBurrowMoveDoAfter>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoBurrowMoveDoAfter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoBurrowMoveDoAfter cast = (XenoBurrowMoveDoAfter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoBurrowMoveDoAfter cast = (XenoBurrowMoveDoAfter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoBurrowMoveDoAfter Instantiate()
	{
		return new XenoBurrowMoveDoAfter();
	}
}
