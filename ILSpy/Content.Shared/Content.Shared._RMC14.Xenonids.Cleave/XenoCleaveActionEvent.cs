using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Cleave;

public sealed class XenoCleaveActionEvent : EntityTargetActionEvent, ISerializationGenerated<XenoCleaveActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Flings;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoCleaveActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoCleaveActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoCleaveActionEvent>(this, ref target, hookCtx, false, context))
		{
			bool FlingsTemp = false;
			if (!serialization.TryCustomCopy<bool>(Flings, ref FlingsTemp, hookCtx, false, context))
			{
				FlingsTemp = Flings;
			}
			target.Flings = FlingsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoCleaveActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCleaveActionEvent cast = (XenoCleaveActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCleaveActionEvent cast = (XenoCleaveActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoCleaveActionEvent Instantiate()
	{
		return new XenoCleaveActionEvent();
	}
}
