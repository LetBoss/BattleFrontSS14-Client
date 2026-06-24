using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Radio.Components;

[RegisterComponent]
public sealed class HeadsetComponent : Component, ISerializationGenerated<HeadsetComponent>, ISerializationGenerated
{
	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled = true;

	public bool IsEquipped;

	[DataField("requiredSlot", false, 1, false, false, null)]
	public SlotFlags RequiredSlot = SlotFlags.EARS;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HeadsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HeadsetComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HeadsetComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			SlotFlags RequiredSlotTemp = SlotFlags.NONE;
			if (!serialization.TryCustomCopy<SlotFlags>(RequiredSlot, ref RequiredSlotTemp, hookCtx, false, context))
			{
				RequiredSlotTemp = RequiredSlot;
			}
			target.RequiredSlot = RequiredSlotTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HeadsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HeadsetComponent cast = (HeadsetComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HeadsetComponent cast = (HeadsetComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HeadsetComponent def = (HeadsetComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HeadsetComponent Instantiate()
	{
		return new HeadsetComponent();
	}
}
