using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Containers.ItemSlots;

[RegisterComponent]
[Access(new Type[] { typeof(ItemSlotsSystem) })]
[NetworkedComponent]
public sealed class ItemSlotsComponent : Component, ISerializationGenerated<ItemSlotsComponent>, ISerializationGenerated
{
	[DataField(null, true, 1, false, false, null)]
	public Dictionary<string, ItemSlot> Slots = new Dictionary<string, ItemSlot>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemSlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemSlotsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemSlotsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, ItemSlot> SlotsTemp = null;
			if (Slots == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, ItemSlot>>(Slots, ref SlotsTemp, hookCtx, true, context))
			{
				SlotsTemp = serialization.CreateCopy<Dictionary<string, ItemSlot>>(Slots, hookCtx, context, false);
			}
			target.Slots = SlotsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemSlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlotsComponent cast = (ItemSlotsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlotsComponent cast = (ItemSlotsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemSlotsComponent def = (ItemSlotsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemSlotsComponent Instantiate()
	{
		return new ItemSlotsComponent();
	}
}
