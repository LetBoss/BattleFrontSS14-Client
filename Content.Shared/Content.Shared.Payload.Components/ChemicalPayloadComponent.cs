using System;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Payload.Components;

[RegisterComponent]
public sealed class ChemicalPayloadComponent : Component, ISerializationGenerated<ChemicalPayloadComponent>, ISerializationGenerated
{
	[DataField("beakerSlotA", false, 1, true, false, null)]
	public ItemSlot BeakerSlotA = new ItemSlot();

	[DataField("beakerSlotB", false, 1, true, false, null)]
	public ItemSlot BeakerSlotB = new ItemSlot();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChemicalPayloadComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChemicalPayloadComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ChemicalPayloadComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ItemSlot BeakerSlotATemp = null;
		if (BeakerSlotA == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(BeakerSlotA, ref BeakerSlotATemp, hookCtx, false, context))
		{
			if (BeakerSlotA == null)
			{
				BeakerSlotATemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(BeakerSlotA, ref BeakerSlotATemp, hookCtx, context, true);
			}
		}
		target.BeakerSlotA = BeakerSlotATemp;
		ItemSlot BeakerSlotBTemp = null;
		if (BeakerSlotB == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ItemSlot>(BeakerSlotB, ref BeakerSlotBTemp, hookCtx, false, context))
		{
			if (BeakerSlotB == null)
			{
				BeakerSlotBTemp = null;
			}
			else
			{
				serialization.CopyTo<ItemSlot>(BeakerSlotB, ref BeakerSlotBTemp, hookCtx, context, true);
			}
		}
		target.BeakerSlotB = BeakerSlotBTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChemicalPayloadComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemicalPayloadComponent cast = (ChemicalPayloadComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemicalPayloadComponent cast = (ChemicalPayloadComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemicalPayloadComponent def = (ChemicalPayloadComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChemicalPayloadComponent Instantiate()
	{
		return new ChemicalPayloadComponent();
	}
}
