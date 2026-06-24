using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Melee.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedMeleeWeaponSystem) })]
public sealed class BonusMeleeAttackRateComponent : Component, ISerializationGenerated<BonusMeleeAttackRateComponent>, ISerializationGenerated
{
	[DataField("flatModifier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float FlatModifier;

	[DataField("multiplier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Multiplier = 1f;

	[DataField("heavyWindupFlatModifier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float HeavyWindupFlatModifier;

	[DataField("heavyWindupMultiplier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float HeavyWindupMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BonusMeleeAttackRateComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BonusMeleeAttackRateComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BonusMeleeAttackRateComponent>(this, ref target, hookCtx, false, context))
		{
			float FlatModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FlatModifier, ref FlatModifierTemp, hookCtx, false, context))
			{
				FlatModifierTemp = FlatModifier;
			}
			target.FlatModifier = FlatModifierTemp;
			float MultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Multiplier, ref MultiplierTemp, hookCtx, false, context))
			{
				MultiplierTemp = Multiplier;
			}
			target.Multiplier = MultiplierTemp;
			float HeavyWindupFlatModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HeavyWindupFlatModifier, ref HeavyWindupFlatModifierTemp, hookCtx, false, context))
			{
				HeavyWindupFlatModifierTemp = HeavyWindupFlatModifier;
			}
			target.HeavyWindupFlatModifier = HeavyWindupFlatModifierTemp;
			float HeavyWindupMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HeavyWindupMultiplier, ref HeavyWindupMultiplierTemp, hookCtx, false, context))
			{
				HeavyWindupMultiplierTemp = HeavyWindupMultiplier;
			}
			target.HeavyWindupMultiplier = HeavyWindupMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BonusMeleeAttackRateComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonusMeleeAttackRateComponent cast = (BonusMeleeAttackRateComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonusMeleeAttackRateComponent cast = (BonusMeleeAttackRateComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonusMeleeAttackRateComponent def = (BonusMeleeAttackRateComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BonusMeleeAttackRateComponent Instantiate()
	{
		return new BonusMeleeAttackRateComponent();
	}
}
