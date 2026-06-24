using System;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
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
public sealed class BonusMeleeDamageComponent : Component, ISerializationGenerated<BonusMeleeDamageComponent>, ISerializationGenerated
{
	[DataField("bonusDamage", false, 1, false, false, null)]
	public DamageSpecifier? BonusDamage;

	[DataField("damageModifierSet", false, 1, false, false, null)]
	public DamageModifierSet? DamageModifierSet;

	[DataField("heavyDamageFlatModifier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 HeavyDamageFlatModifier;

	[DataField("heavyDamageMultiplier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float HeavyDamageMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BonusMeleeDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BonusMeleeDamageComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<BonusMeleeDamageComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier BonusDamageTemp = null;
		if (!serialization.TryCustomCopy<DamageSpecifier>(BonusDamage, ref BonusDamageTemp, hookCtx, false, context))
		{
			if (BonusDamage == null)
			{
				BonusDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(BonusDamage, ref BonusDamageTemp, hookCtx, context, false);
			}
		}
		target.BonusDamage = BonusDamageTemp;
		DamageModifierSet DamageModifierSetTemp = null;
		if (!serialization.TryCustomCopy<DamageModifierSet>(DamageModifierSet, ref DamageModifierSetTemp, hookCtx, true, context))
		{
			if (DamageModifierSet == null)
			{
				DamageModifierSetTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageModifierSet>(DamageModifierSet, ref DamageModifierSetTemp, hookCtx, context, false);
			}
		}
		target.DamageModifierSet = DamageModifierSetTemp;
		FixedPoint2 HeavyDamageFlatModifierTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(HeavyDamageFlatModifier, ref HeavyDamageFlatModifierTemp, hookCtx, false, context))
		{
			HeavyDamageFlatModifierTemp = serialization.CreateCopy<FixedPoint2>(HeavyDamageFlatModifier, hookCtx, context, false);
		}
		target.HeavyDamageFlatModifier = HeavyDamageFlatModifierTemp;
		float HeavyDamageMultiplierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(HeavyDamageMultiplier, ref HeavyDamageMultiplierTemp, hookCtx, false, context))
		{
			HeavyDamageMultiplierTemp = HeavyDamageMultiplier;
		}
		target.HeavyDamageMultiplier = HeavyDamageMultiplierTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BonusMeleeDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonusMeleeDamageComponent cast = (BonusMeleeDamageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonusMeleeDamageComponent cast = (BonusMeleeDamageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonusMeleeDamageComponent def = (BonusMeleeDamageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BonusMeleeDamageComponent Instantiate()
	{
		return new BonusMeleeDamageComponent();
	}
}
