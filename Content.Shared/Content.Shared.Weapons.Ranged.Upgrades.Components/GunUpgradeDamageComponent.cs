using System;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Weapons.Ranged.Upgrades.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(GunUpgradeSystem) })]
public sealed class GunUpgradeDamageComponent : Component, ISerializationGenerated<GunUpgradeDamageComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier Damage = new DamageSpecifier();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GunUpgradeDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GunUpgradeDamageComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<GunUpgradeDamageComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier DamageTemp = null;
		if (Damage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, false, context))
		{
			if (Damage == null)
			{
				DamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, context, true);
			}
		}
		target.Damage = DamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GunUpgradeDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeDamageComponent cast = (GunUpgradeDamageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeDamageComponent cast = (GunUpgradeDamageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeDamageComponent def = (GunUpgradeDamageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GunUpgradeDamageComponent Instantiate()
	{
		return new GunUpgradeDamageComponent();
	}
}
