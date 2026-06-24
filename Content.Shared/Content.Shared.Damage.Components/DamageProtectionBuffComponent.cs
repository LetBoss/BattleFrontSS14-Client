using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamageProtectionBuffComponent : Component, ISerializationGenerated<DamageProtectionBuffComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, DamageModifierSetPrototype> Modifiers = new Dictionary<string, DamageModifierSetPrototype>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageProtectionBuffComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageProtectionBuffComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DamageProtectionBuffComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, DamageModifierSetPrototype> ModifiersTemp = null;
			if (Modifiers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, DamageModifierSetPrototype>>(Modifiers, ref ModifiersTemp, hookCtx, true, context))
			{
				ModifiersTemp = serialization.CreateCopy<Dictionary<string, DamageModifierSetPrototype>>(Modifiers, hookCtx, context, false);
			}
			target.Modifiers = ModifiersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageProtectionBuffComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageProtectionBuffComponent cast = (DamageProtectionBuffComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageProtectionBuffComponent cast = (DamageProtectionBuffComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageProtectionBuffComponent def = (DamageProtectionBuffComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageProtectionBuffComponent Instantiate()
	{
		return new DamageProtectionBuffComponent();
	}
}
