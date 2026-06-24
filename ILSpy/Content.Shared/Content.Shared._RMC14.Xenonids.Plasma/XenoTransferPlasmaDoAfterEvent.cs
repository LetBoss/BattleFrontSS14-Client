using System;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Plasma;

[Serializable]
[NetSerializable]
public sealed class XenoTransferPlasmaDoAfterEvent : DoAfterEvent, ISerializationGenerated<XenoTransferPlasmaDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Amount = FixedPoint2.New(50);

	public XenoTransferPlasmaDoAfterEvent(FixedPoint2 amount)
	{
		Amount = amount;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public XenoTransferPlasmaDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoTransferPlasmaDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoTransferPlasmaDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoTransferPlasmaDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 AmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = serialization.CreateCopy<FixedPoint2>(Amount, hookCtx, context, false);
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoTransferPlasmaDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTransferPlasmaDoAfterEvent cast = (XenoTransferPlasmaDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTransferPlasmaDoAfterEvent cast = (XenoTransferPlasmaDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoTransferPlasmaDoAfterEvent Instantiate()
	{
		return new XenoTransferPlasmaDoAfterEvent();
	}
}
