using System;
using Content.Shared.Containers;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SlotBasedConnectedContainerSystem) })]
[NetworkedComponent]
public sealed class SlotBasedConnectedContainerComponent : Component, ISerializationGenerated<SlotBasedConnectedContainerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public SlotFlags TargetSlot;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? ContainerWhitelist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SlotBasedConnectedContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SlotBasedConnectedContainerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SlotBasedConnectedContainerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		SlotFlags TargetSlotTemp = SlotFlags.NONE;
		if (!serialization.TryCustomCopy<SlotFlags>(TargetSlot, ref TargetSlotTemp, hookCtx, false, context))
		{
			TargetSlotTemp = TargetSlot;
		}
		target.TargetSlot = TargetSlotTemp;
		EntityWhitelist ContainerWhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(ContainerWhitelist, ref ContainerWhitelistTemp, hookCtx, false, context))
		{
			if (ContainerWhitelist == null)
			{
				ContainerWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(ContainerWhitelist, ref ContainerWhitelistTemp, hookCtx, context, false);
			}
		}
		target.ContainerWhitelist = ContainerWhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SlotBasedConnectedContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlotBasedConnectedContainerComponent cast = (SlotBasedConnectedContainerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlotBasedConnectedContainerComponent cast = (SlotBasedConnectedContainerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlotBasedConnectedContainerComponent def = (SlotBasedConnectedContainerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SlotBasedConnectedContainerComponent Instantiate()
	{
		return new SlotBasedConnectedContainerComponent();
	}
}
