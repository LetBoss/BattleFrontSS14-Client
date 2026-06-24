using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Armor.ThermalCloak;

public sealed class ThermalCloakTurnInvisibleActionEvent : InstantActionEvent, ISerializationGenerated<ThermalCloakTurnInvisibleActionEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ThermalCloakTurnInvisibleActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ThermalCloakTurnInvisibleActionEvent)definitionCast;
		serialization.TryCustomCopy<ThermalCloakTurnInvisibleActionEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ThermalCloakTurnInvisibleActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ThermalCloakTurnInvisibleActionEvent cast = (ThermalCloakTurnInvisibleActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ThermalCloakTurnInvisibleActionEvent cast = (ThermalCloakTurnInvisibleActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ThermalCloakTurnInvisibleActionEvent Instantiate()
	{
		return new ThermalCloakTurnInvisibleActionEvent();
	}
}
