using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamageOnAttackedProtectionComponent : Component, IClothingSlots, ISerializationGenerated<DamageOnAttackedProtectionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public DamageModifierSet DamageProtection;

	[DataField(null, false, 1, false, false, null)]
	public SlotFlags Slots { get; set; } = SlotFlags.WITHOUT_POCKET;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageOnAttackedProtectionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageOnAttackedProtectionComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DamageOnAttackedProtectionComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageModifierSet DamageProtectionTemp = null;
		if (DamageProtection == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageModifierSet>(DamageProtection, ref DamageProtectionTemp, hookCtx, true, context))
		{
			if (DamageProtection == null)
			{
				DamageProtectionTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageModifierSet>(DamageProtection, ref DamageProtectionTemp, hookCtx, context, true);
			}
		}
		target.DamageProtection = DamageProtectionTemp;
		SlotFlags SlotsTemp = SlotFlags.NONE;
		if (!serialization.TryCustomCopy<SlotFlags>(Slots, ref SlotsTemp, hookCtx, false, context))
		{
			SlotsTemp = Slots;
		}
		target.Slots = SlotsTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageOnAttackedProtectionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageOnAttackedProtectionComponent cast = (DamageOnAttackedProtectionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageOnAttackedProtectionComponent cast = (DamageOnAttackedProtectionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageOnAttackedProtectionComponent def = (DamageOnAttackedProtectionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageOnAttackedProtectionComponent Instantiate()
	{
		return new DamageOnAttackedProtectionComponent();
	}
}
