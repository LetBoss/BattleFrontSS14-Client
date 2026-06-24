using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
public sealed class VehicleArmorHardpointComponent : Component, ISerializationGenerated<VehicleArmorHardpointComponent>, ISerializationGenerated
{
	[DataField("modifierSets", false, 1, false, false, null)]
	public List<ProtoId<DamageModifierSetPrototype>> ModifierSets = new List<ProtoId<DamageModifierSetPrototype>>();

	[DataField("explosionCoefficient", false, 1, false, false, null)]
	public float? ExplosionCoefficient;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleArmorHardpointComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleArmorHardpointComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleArmorHardpointComponent>(this, ref target, hookCtx, false, context))
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
			float? ExplosionCoefficientTemp = null;
			if (!serialization.TryCustomCopy<float?>(ExplosionCoefficient, ref ExplosionCoefficientTemp, hookCtx, false, context))
			{
				ExplosionCoefficientTemp = ExplosionCoefficient;
			}
			target.ExplosionCoefficient = ExplosionCoefficientTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleArmorHardpointComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleArmorHardpointComponent cast = (VehicleArmorHardpointComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleArmorHardpointComponent cast = (VehicleArmorHardpointComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleArmorHardpointComponent def = (VehicleArmorHardpointComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleArmorHardpointComponent Instantiate()
	{
		return new VehicleArmorHardpointComponent();
	}
}
