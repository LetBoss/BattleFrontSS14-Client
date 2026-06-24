using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Armor.ThermalCloak;

[RegisterComponent]
[NetworkedComponent]
public sealed class CancelUseWithCloakComponent : Component, ISerializationGenerated<CancelUseWithCloakComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string CancelMessage = "rmc-cloak-attempt-prime";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CancelUseWithCloakComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CancelUseWithCloakComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CancelUseWithCloakComponent>(this, ref target, hookCtx, false, context))
		{
			string CancelMessageTemp = null;
			if (CancelMessage == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CancelMessage, ref CancelMessageTemp, hookCtx, false, context))
			{
				CancelMessageTemp = CancelMessage;
			}
			target.CancelMessage = CancelMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CancelUseWithCloakComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CancelUseWithCloakComponent cast = (CancelUseWithCloakComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CancelUseWithCloakComponent cast = (CancelUseWithCloakComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CancelUseWithCloakComponent def = (CancelUseWithCloakComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CancelUseWithCloakComponent Instantiate()
	{
		return new CancelUseWithCloakComponent();
	}
}
