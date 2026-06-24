using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Armor;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCArmorModifierComponent : Component, ISerializationGenerated<RMCArmorModifierComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int MeleeArmorModifier = 4;

	[DataField(null, false, 1, false, false, null)]
	public int RangedArmorModifier = 4;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCArmorModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCArmorModifierComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCArmorModifierComponent>(this, ref target, hookCtx, false, context))
		{
			int MeleeArmorModifierTemp = 0;
			if (!serialization.TryCustomCopy<int>(MeleeArmorModifier, ref MeleeArmorModifierTemp, hookCtx, false, context))
			{
				MeleeArmorModifierTemp = MeleeArmorModifier;
			}
			target.MeleeArmorModifier = MeleeArmorModifierTemp;
			int RangedArmorModifierTemp = 0;
			if (!serialization.TryCustomCopy<int>(RangedArmorModifier, ref RangedArmorModifierTemp, hookCtx, false, context))
			{
				RangedArmorModifierTemp = RangedArmorModifier;
			}
			target.RangedArmorModifier = RangedArmorModifierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCArmorModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCArmorModifierComponent cast = (RMCArmorModifierComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCArmorModifierComponent cast = (RMCArmorModifierComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCArmorModifierComponent def = (RMCArmorModifierComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCArmorModifierComponent Instantiate()
	{
		return new RMCArmorModifierComponent();
	}
}
