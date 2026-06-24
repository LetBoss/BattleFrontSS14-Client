using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Plasma;

public sealed class XenoTransferPlasmaActionEvent : EntityTargetActionEvent, ISerializationGenerated<XenoTransferPlasmaActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Amount = 50;

	[DataField(null, false, 1, false, false, null)]
	public float Range = 2.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoTransferPlasmaActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoTransferPlasmaActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoTransferPlasmaActionEvent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 AmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = serialization.CreateCopy<FixedPoint2>(Amount, hookCtx, context, false);
			}
			target.Amount = AmountTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoTransferPlasmaActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTransferPlasmaActionEvent cast = (XenoTransferPlasmaActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTransferPlasmaActionEvent cast = (XenoTransferPlasmaActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoTransferPlasmaActionEvent Instantiate()
	{
		return new XenoTransferPlasmaActionEvent();
	}
}
