using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[DataDefinition]
public sealed class RMCVehicleDamageScaleRule : ISerializationGenerated<RMCVehicleDamageScaleRule>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<string> DamageTypes = new List<string>();

	[DataField(null, false, 1, true, false, null)]
	public FixedPoint2 MaxDamage;

	[DataField(null, false, 1, true, false, null)]
	public float Multiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCVehicleDamageScaleRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RMCVehicleDamageScaleRule>(this, ref target, hookCtx, false, context))
		{
			List<string> DamageTypesTemp = null;
			if (DamageTypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(DamageTypes, ref DamageTypesTemp, hookCtx, true, context))
			{
				DamageTypesTemp = serialization.CreateCopy<List<string>>(DamageTypes, hookCtx, context, false);
			}
			target.DamageTypes = DamageTypesTemp;
			FixedPoint2 MaxDamageTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(MaxDamage, ref MaxDamageTemp, hookCtx, false, context))
			{
				MaxDamageTemp = serialization.CreateCopy<FixedPoint2>(MaxDamage, hookCtx, context, false);
			}
			target.MaxDamage = MaxDamageTemp;
			float MultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Multiplier, ref MultiplierTemp, hookCtx, false, context))
			{
				MultiplierTemp = Multiplier;
			}
			target.Multiplier = MultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCVehicleDamageScaleRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDamageScaleRule cast = (RMCVehicleDamageScaleRule)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCVehicleDamageScaleRule Instantiate()
	{
		return new RMCVehicleDamageScaleRule();
	}
}
