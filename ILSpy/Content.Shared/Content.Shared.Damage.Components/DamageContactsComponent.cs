using System;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Damage.Components;

[NetworkedComponent]
[RegisterComponent]
public sealed class DamageContactsComponent : Component, ISerializationGenerated<DamageContactsComponent>, ISerializationGenerated
{
	[DataField("damage", false, 1, true, false, null)]
	public DamageSpecifier Damage = new DamageSpecifier();

	[DataField("ignoreWhitelist", false, 1, false, false, null)]
	public EntityWhitelist? IgnoreWhitelist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageContactsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageContactsComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DamageContactsComponent>(this, ref target, hookCtx, false, context))
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
		EntityWhitelist IgnoreWhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(IgnoreWhitelist, ref IgnoreWhitelistTemp, hookCtx, false, context))
		{
			if (IgnoreWhitelist == null)
			{
				IgnoreWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(IgnoreWhitelist, ref IgnoreWhitelistTemp, hookCtx, context, false);
			}
		}
		target.IgnoreWhitelist = IgnoreWhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageContactsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageContactsComponent cast = (DamageContactsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageContactsComponent cast = (DamageContactsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageContactsComponent def = (DamageContactsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageContactsComponent Instantiate()
	{
		return new DamageContactsComponent();
	}
}
