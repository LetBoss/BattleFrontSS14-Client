using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StoreDiscount.Components;

[RegisterComponent]
public sealed class StoreDiscountComponent : Component, ISerializationGenerated<StoreDiscountComponent>, ISerializationGenerated
{
	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public IReadOnlyList<StoreDiscountData> Discounts = Array.Empty<StoreDiscountData>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StoreDiscountComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StoreDiscountComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StoreDiscountComponent>(this, ref target, hookCtx, false, context))
		{
			IReadOnlyList<StoreDiscountData> DiscountsTemp = null;
			if (Discounts == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IReadOnlyList<StoreDiscountData>>(Discounts, ref DiscountsTemp, hookCtx, true, context))
			{
				DiscountsTemp = serialization.CreateCopy<IReadOnlyList<StoreDiscountData>>(Discounts, hookCtx, context, false);
			}
			target.Discounts = DiscountsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StoreDiscountComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreDiscountComponent cast = (StoreDiscountComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreDiscountComponent cast = (StoreDiscountComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreDiscountComponent def = (StoreDiscountComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StoreDiscountComponent Instantiate()
	{
		return new StoreDiscountComponent();
	}
}
