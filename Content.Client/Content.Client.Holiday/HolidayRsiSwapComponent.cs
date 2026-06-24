using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Holiday;

[RegisterComponent]
public sealed class HolidayRsiSwapComponent : Component, ISerializationGenerated<HolidayRsiSwapComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, string> Sprite = new Dictionary<string, string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HolidayRsiSwapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (HolidayRsiSwapComponent)(object)val;
		if (!serialization.TryCustomCopy<HolidayRsiSwapComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, string> sprite = null;
			if (Sprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, string>>(Sprite, ref sprite, hookCtx, true, context))
			{
				sprite = serialization.CreateCopy<Dictionary<string, string>>(Sprite, hookCtx, context, false);
			}
			target.Sprite = sprite;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HolidayRsiSwapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HolidayRsiSwapComponent target2 = (HolidayRsiSwapComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HolidayRsiSwapComponent target2 = (HolidayRsiSwapComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HolidayRsiSwapComponent target2 = (HolidayRsiSwapComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HolidayRsiSwapComponent Instantiate()
	{
		return new HolidayRsiSwapComponent();
	}
}
