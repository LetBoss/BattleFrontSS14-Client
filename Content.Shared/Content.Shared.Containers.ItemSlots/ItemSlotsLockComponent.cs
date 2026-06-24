using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Containers.ItemSlots;

[RegisterComponent]
[NetworkedComponent]
public sealed class ItemSlotsLockComponent : Component, ISerializationGenerated<ItemSlotsLockComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<string> Slots = new List<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemSlotsLockComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemSlotsLockComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemSlotsLockComponent>(this, ref target, hookCtx, false, context))
		{
			List<string> SlotsTemp = null;
			if (Slots == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Slots, ref SlotsTemp, hookCtx, true, context))
			{
				SlotsTemp = serialization.CreateCopy<List<string>>(Slots, hookCtx, context, false);
			}
			target.Slots = SlotsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemSlotsLockComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlotsLockComponent cast = (ItemSlotsLockComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlotsLockComponent cast = (ItemSlotsLockComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlotsLockComponent def = (ItemSlotsLockComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemSlotsLockComponent Instantiate()
	{
		return new ItemSlotsLockComponent();
	}
}
