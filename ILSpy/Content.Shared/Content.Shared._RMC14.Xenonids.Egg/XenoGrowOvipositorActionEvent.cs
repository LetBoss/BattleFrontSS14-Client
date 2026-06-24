using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Egg;

public sealed class XenoGrowOvipositorActionEvent : InstantActionEvent, ISerializationGenerated<XenoGrowOvipositorActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int AttachPlasmaCost = 700;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan AttachDoAfter = TimeSpan.FromSeconds(20L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DetachDoAfter = TimeSpan.FromSeconds(5L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoGrowOvipositorActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoGrowOvipositorActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoGrowOvipositorActionEvent>(this, ref target, hookCtx, false, context))
		{
			int AttachPlasmaCostTemp = 0;
			if (!serialization.TryCustomCopy<int>(AttachPlasmaCost, ref AttachPlasmaCostTemp, hookCtx, false, context))
			{
				AttachPlasmaCostTemp = AttachPlasmaCost;
			}
			target.AttachPlasmaCost = AttachPlasmaCostTemp;
			TimeSpan AttachDoAfterTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(AttachDoAfter, ref AttachDoAfterTemp, hookCtx, false, context))
			{
				AttachDoAfterTemp = serialization.CreateCopy<TimeSpan>(AttachDoAfter, hookCtx, context, false);
			}
			target.AttachDoAfter = AttachDoAfterTemp;
			TimeSpan DetachDoAfterTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DetachDoAfter, ref DetachDoAfterTemp, hookCtx, false, context))
			{
				DetachDoAfterTemp = serialization.CreateCopy<TimeSpan>(DetachDoAfter, hookCtx, context, false);
			}
			target.DetachDoAfter = DetachDoAfterTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoGrowOvipositorActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoGrowOvipositorActionEvent cast = (XenoGrowOvipositorActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoGrowOvipositorActionEvent cast = (XenoGrowOvipositorActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoGrowOvipositorActionEvent Instantiate()
	{
		return new XenoGrowOvipositorActionEvent();
	}
}
