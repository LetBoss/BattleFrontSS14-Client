using System;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Egg;

[Serializable]
[NetSerializable]
public sealed class XenoGrowOvipositorDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<XenoGrowOvipositorDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 PlasmaCost;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoGrowOvipositorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoGrowOvipositorDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoGrowOvipositorDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 PlasmaCostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = serialization.CreateCopy<FixedPoint2>(PlasmaCost, hookCtx, context, false);
			}
			target.PlasmaCost = PlasmaCostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoGrowOvipositorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoGrowOvipositorDoAfterEvent cast = (XenoGrowOvipositorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoGrowOvipositorDoAfterEvent cast = (XenoGrowOvipositorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoGrowOvipositorDoAfterEvent Instantiate()
	{
		return new XenoGrowOvipositorDoAfterEvent();
	}
}
