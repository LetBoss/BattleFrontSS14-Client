using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class HardpointDamageModifierComponent : Component, ISerializationGenerated<HardpointDamageModifierComponent>, ISerializationGenerated
{
	[DataField("modifierSets", false, 1, false, false, null)]
	public List<ProtoId<DamageModifierSetPrototype>> ModifierSets = new List<ProtoId<DamageModifierSetPrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HardpointDamageModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HardpointDamageModifierComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HardpointDamageModifierComponent>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<DamageModifierSetPrototype>> ModifierSetsTemp = null;
			if (ModifierSets == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<DamageModifierSetPrototype>>>(ModifierSets, ref ModifierSetsTemp, hookCtx, true, context))
			{
				ModifierSetsTemp = serialization.CreateCopy<List<ProtoId<DamageModifierSetPrototype>>>(ModifierSets, hookCtx, context, false);
			}
			target.ModifierSets = ModifierSetsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HardpointDamageModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointDamageModifierComponent cast = (HardpointDamageModifierComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointDamageModifierComponent cast = (HardpointDamageModifierComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointDamageModifierComponent def = (HardpointDamageModifierComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HardpointDamageModifierComponent Instantiate()
	{
		return new HardpointDamageModifierComponent();
	}
}
