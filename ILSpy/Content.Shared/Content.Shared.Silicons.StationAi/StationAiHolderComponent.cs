using System;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Silicons.StationAi;

[RegisterComponent]
[NetworkedComponent]
public sealed class StationAiHolderComponent : Component, ISerializationGenerated<StationAiHolderComponent>, ISerializationGenerated
{
	public const string Container = "station_ai_mind_slot";

	[DataField(null, false, 1, false, false, null)]
	public ItemSlot Slot = new ItemSlot();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StationAiHolderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StationAiHolderComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<StationAiHolderComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ItemSlot SlotTemp = null;
		if (Slot == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(Slot, ref SlotTemp, hookCtx, false, context))
		{
			if (Slot == null)
			{
				SlotTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(Slot, ref SlotTemp, hookCtx, context, true);
			}
		}
		target.Slot = SlotTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StationAiHolderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationAiHolderComponent cast = (StationAiHolderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationAiHolderComponent cast = (StationAiHolderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationAiHolderComponent def = (StationAiHolderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StationAiHolderComponent Instantiate()
	{
		return new StationAiHolderComponent();
	}
}
