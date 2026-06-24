using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Speech.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamagedSiliconAccentComponent : Component, ISerializationGenerated<DamagedSiliconAccentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool EnableDamageCorruption = true;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? OverrideTotalDamage;

	[DataField(null, false, 1, false, false, null)]
	public float MaxDamageCorruption = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? DamageAtMaxCorruption;

	[DataField(null, false, 1, false, false, null)]
	public bool EnableChargeCorruption = true;

	[DataField(null, false, 1, false, false, null)]
	public float? OverrideChargeLevel;

	[DataField(null, false, 1, false, false, null)]
	public float ChargeThresholdForPowerCorruption = 0.15f;

	[DataField(null, false, 1, false, false, null)]
	public int StartPowerCorruptionAtCharIdx = 8;

	[DataField(null, false, 1, false, false, null)]
	public int MaxPowerCorruptionAtCharIdx = 40;

	[DataField(null, false, 1, false, false, null)]
	public float MaxDropProbFromPower = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public float ProbToCorruptDotFromPower = 0.6f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamagedSiliconAccentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamagedSiliconAccentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DamagedSiliconAccentComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnableDamageCorruptionTemp = false;
			if (!serialization.TryCustomCopy<bool>(EnableDamageCorruption, ref EnableDamageCorruptionTemp, hookCtx, false, context))
			{
				EnableDamageCorruptionTemp = EnableDamageCorruption;
			}
			target.EnableDamageCorruption = EnableDamageCorruptionTemp;
			FixedPoint2? OverrideTotalDamageTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(OverrideTotalDamage, ref OverrideTotalDamageTemp, hookCtx, false, context))
			{
				OverrideTotalDamageTemp = serialization.CreateCopy<FixedPoint2?>(OverrideTotalDamage, hookCtx, context, false);
			}
			target.OverrideTotalDamage = OverrideTotalDamageTemp;
			float MaxDamageCorruptionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxDamageCorruption, ref MaxDamageCorruptionTemp, hookCtx, false, context))
			{
				MaxDamageCorruptionTemp = MaxDamageCorruption;
			}
			target.MaxDamageCorruption = MaxDamageCorruptionTemp;
			FixedPoint2? DamageAtMaxCorruptionTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(DamageAtMaxCorruption, ref DamageAtMaxCorruptionTemp, hookCtx, false, context))
			{
				DamageAtMaxCorruptionTemp = serialization.CreateCopy<FixedPoint2?>(DamageAtMaxCorruption, hookCtx, context, false);
			}
			target.DamageAtMaxCorruption = DamageAtMaxCorruptionTemp;
			bool EnableChargeCorruptionTemp = false;
			if (!serialization.TryCustomCopy<bool>(EnableChargeCorruption, ref EnableChargeCorruptionTemp, hookCtx, false, context))
			{
				EnableChargeCorruptionTemp = EnableChargeCorruption;
			}
			target.EnableChargeCorruption = EnableChargeCorruptionTemp;
			float? OverrideChargeLevelTemp = null;
			if (!serialization.TryCustomCopy<float?>(OverrideChargeLevel, ref OverrideChargeLevelTemp, hookCtx, false, context))
			{
				OverrideChargeLevelTemp = OverrideChargeLevel;
			}
			target.OverrideChargeLevel = OverrideChargeLevelTemp;
			float ChargeThresholdForPowerCorruptionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ChargeThresholdForPowerCorruption, ref ChargeThresholdForPowerCorruptionTemp, hookCtx, false, context))
			{
				ChargeThresholdForPowerCorruptionTemp = ChargeThresholdForPowerCorruption;
			}
			target.ChargeThresholdForPowerCorruption = ChargeThresholdForPowerCorruptionTemp;
			int StartPowerCorruptionAtCharIdxTemp = 0;
			if (!serialization.TryCustomCopy<int>(StartPowerCorruptionAtCharIdx, ref StartPowerCorruptionAtCharIdxTemp, hookCtx, false, context))
			{
				StartPowerCorruptionAtCharIdxTemp = StartPowerCorruptionAtCharIdx;
			}
			target.StartPowerCorruptionAtCharIdx = StartPowerCorruptionAtCharIdxTemp;
			int MaxPowerCorruptionAtCharIdxTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxPowerCorruptionAtCharIdx, ref MaxPowerCorruptionAtCharIdxTemp, hookCtx, false, context))
			{
				MaxPowerCorruptionAtCharIdxTemp = MaxPowerCorruptionAtCharIdx;
			}
			target.MaxPowerCorruptionAtCharIdx = MaxPowerCorruptionAtCharIdxTemp;
			float MaxDropProbFromPowerTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxDropProbFromPower, ref MaxDropProbFromPowerTemp, hookCtx, false, context))
			{
				MaxDropProbFromPowerTemp = MaxDropProbFromPower;
			}
			target.MaxDropProbFromPower = MaxDropProbFromPowerTemp;
			float ProbToCorruptDotFromPowerTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ProbToCorruptDotFromPower, ref ProbToCorruptDotFromPowerTemp, hookCtx, false, context))
			{
				ProbToCorruptDotFromPowerTemp = ProbToCorruptDotFromPower;
			}
			target.ProbToCorruptDotFromPower = ProbToCorruptDotFromPowerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamagedSiliconAccentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedSiliconAccentComponent cast = (DamagedSiliconAccentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedSiliconAccentComponent cast = (DamagedSiliconAccentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedSiliconAccentComponent def = (DamagedSiliconAccentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamagedSiliconAccentComponent Instantiate()
	{
		return new DamagedSiliconAccentComponent();
	}
}
