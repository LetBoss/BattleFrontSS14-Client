using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
public sealed class DamageUserOnTriggerComponent : Component, ISerializationGenerated<DamageUserOnTriggerComponent>, ISerializationGenerated
{
	[DataField("ignoreResistances", false, 1, false, false, null)]
	public bool IgnoreResistances;

	[DataField("damage", false, 1, true, false, null)]
	public DamageSpecifier Damage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageUserOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageUserOnTriggerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DamageUserOnTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		bool IgnoreResistancesTemp = false;
		if (!serialization.TryCustomCopy<bool>(IgnoreResistances, ref IgnoreResistancesTemp, hookCtx, false, context))
		{
			IgnoreResistancesTemp = IgnoreResistances;
		}
		target.IgnoreResistances = IgnoreResistancesTemp;
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
	public void Copy(ref DamageUserOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageUserOnTriggerComponent cast = (DamageUserOnTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageUserOnTriggerComponent cast = (DamageUserOnTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageUserOnTriggerComponent def = (DamageUserOnTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageUserOnTriggerComponent Instantiate()
	{
		return new DamageUserOnTriggerComponent();
	}
}
