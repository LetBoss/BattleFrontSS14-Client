using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Aid;

public sealed class XenoAidActionEvent : EntityTargetActionEvent, ISerializationGenerated<XenoAidActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public XenoAidMode aidType;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoAidActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoAidActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoAidActionEvent>(this, ref target, hookCtx, false, context))
		{
			XenoAidMode aidTypeTemp = XenoAidMode.Healing;
			if (!serialization.TryCustomCopy<XenoAidMode>(aidType, ref aidTypeTemp, hookCtx, false, context))
			{
				aidTypeTemp = aidType;
			}
			target.aidType = aidTypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoAidActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAidActionEvent cast = (XenoAidActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAidActionEvent cast = (XenoAidActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoAidActionEvent Instantiate()
	{
		return new XenoAidActionEvent();
	}
}
