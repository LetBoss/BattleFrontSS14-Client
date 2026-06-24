using System;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Wieldable.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedWieldableSystem) })]
public sealed class IncreaseDamageOnWieldComponent : Component, ISerializationGenerated<IncreaseDamageOnWieldComponent>, ISerializationGenerated
{
	[DataField("damage", false, 1, true, false, null)]
	public DamageSpecifier BonusDamage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IncreaseDamageOnWieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IncreaseDamageOnWieldComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<IncreaseDamageOnWieldComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier BonusDamageTemp = null;
		if (BonusDamage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(BonusDamage, ref BonusDamageTemp, hookCtx, false, context))
		{
			if (BonusDamage == null)
			{
				BonusDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(BonusDamage, ref BonusDamageTemp, hookCtx, context, true);
			}
		}
		target.BonusDamage = BonusDamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IncreaseDamageOnWieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IncreaseDamageOnWieldComponent cast = (IncreaseDamageOnWieldComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IncreaseDamageOnWieldComponent cast = (IncreaseDamageOnWieldComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IncreaseDamageOnWieldComponent def = (IncreaseDamageOnWieldComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IncreaseDamageOnWieldComponent Instantiate()
	{
		return new IncreaseDamageOnWieldComponent();
	}
}
