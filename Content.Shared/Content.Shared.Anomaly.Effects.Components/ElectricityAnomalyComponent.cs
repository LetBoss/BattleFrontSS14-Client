using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
public sealed class ElectricityAnomalyComponent : Component, ISerializationGenerated<ElectricityAnomalyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int MinBoltCount = 2;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int MaxBoltCount = 5;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxElectrocuteRange = 7f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxElectrocuteDamage = 20f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan MaxElectrocuteDuration = TimeSpan.FromSeconds(8L);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float PassiveElectrocutionChance = 0.05f;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan NextSecond = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float EmpEnergyConsumption = 100000f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float EmpDisabledDuration = 60f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ElectricityAnomalyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ElectricityAnomalyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ElectricityAnomalyComponent>(this, ref target, hookCtx, false, context))
		{
			int MinBoltCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinBoltCount, ref MinBoltCountTemp, hookCtx, false, context))
			{
				MinBoltCountTemp = MinBoltCount;
			}
			target.MinBoltCount = MinBoltCountTemp;
			int MaxBoltCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxBoltCount, ref MaxBoltCountTemp, hookCtx, false, context))
			{
				MaxBoltCountTemp = MaxBoltCount;
			}
			target.MaxBoltCount = MaxBoltCountTemp;
			float MaxElectrocuteRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxElectrocuteRange, ref MaxElectrocuteRangeTemp, hookCtx, false, context))
			{
				MaxElectrocuteRangeTemp = MaxElectrocuteRange;
			}
			target.MaxElectrocuteRange = MaxElectrocuteRangeTemp;
			float MaxElectrocuteDamageTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxElectrocuteDamage, ref MaxElectrocuteDamageTemp, hookCtx, false, context))
			{
				MaxElectrocuteDamageTemp = MaxElectrocuteDamage;
			}
			target.MaxElectrocuteDamage = MaxElectrocuteDamageTemp;
			TimeSpan MaxElectrocuteDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MaxElectrocuteDuration, ref MaxElectrocuteDurationTemp, hookCtx, false, context))
			{
				MaxElectrocuteDurationTemp = serialization.CreateCopy<TimeSpan>(MaxElectrocuteDuration, hookCtx, context, false);
			}
			target.MaxElectrocuteDuration = MaxElectrocuteDurationTemp;
			float PassiveElectrocutionChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PassiveElectrocutionChance, ref PassiveElectrocutionChanceTemp, hookCtx, false, context))
			{
				PassiveElectrocutionChanceTemp = PassiveElectrocutionChance;
			}
			target.PassiveElectrocutionChance = PassiveElectrocutionChanceTemp;
			TimeSpan NextSecondTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NextSecond, ref NextSecondTemp, hookCtx, false, context))
			{
				NextSecondTemp = serialization.CreateCopy<TimeSpan>(NextSecond, hookCtx, context, false);
			}
			target.NextSecond = NextSecondTemp;
			float EmpEnergyConsumptionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EmpEnergyConsumption, ref EmpEnergyConsumptionTemp, hookCtx, false, context))
			{
				EmpEnergyConsumptionTemp = EmpEnergyConsumption;
			}
			target.EmpEnergyConsumption = EmpEnergyConsumptionTemp;
			float EmpDisabledDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EmpDisabledDuration, ref EmpDisabledDurationTemp, hookCtx, false, context))
			{
				EmpDisabledDurationTemp = EmpDisabledDuration;
			}
			target.EmpDisabledDuration = EmpDisabledDurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ElectricityAnomalyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ElectricityAnomalyComponent cast = (ElectricityAnomalyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ElectricityAnomalyComponent cast = (ElectricityAnomalyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ElectricityAnomalyComponent def = (ElectricityAnomalyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ElectricityAnomalyComponent Instantiate()
	{
		return new ElectricityAnomalyComponent();
	}
}
