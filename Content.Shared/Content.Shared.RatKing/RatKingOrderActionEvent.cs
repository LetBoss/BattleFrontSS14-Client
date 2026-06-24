using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.RatKing;

public sealed class RatKingOrderActionEvent : InstantActionEvent, ISerializationGenerated<RatKingOrderActionEvent>, ISerializationGenerated
{
	[DataField("type", false, 1, false, false, null)]
	public RatKingOrderType Type;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RatKingOrderActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RatKingOrderActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<RatKingOrderActionEvent>(this, ref target, hookCtx, false, context))
		{
			RatKingOrderType TypeTemp = RatKingOrderType.Stay;
			if (!serialization.TryCustomCopy<RatKingOrderType>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RatKingOrderActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RatKingOrderActionEvent cast = (RatKingOrderActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RatKingOrderActionEvent cast = (RatKingOrderActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RatKingOrderActionEvent Instantiate()
	{
		return new RatKingOrderActionEvent();
	}
}
