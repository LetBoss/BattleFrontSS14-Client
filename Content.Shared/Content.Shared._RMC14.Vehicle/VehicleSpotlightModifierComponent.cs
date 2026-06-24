using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleSpotlightModifierComponent : Component, ISerializationGenerated<VehicleSpotlightModifierComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float RadiusMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float RadiusAdd;

	[DataField(null, false, 1, false, false, null)]
	public float EnergyMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float EnergyAdd;

	[DataField(null, false, 1, false, false, null)]
	public float SoftnessMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float SoftnessAdd;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleSpotlightModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleSpotlightModifierComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleSpotlightModifierComponent>(this, ref target, hookCtx, false, context))
		{
			float RadiusMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RadiusMultiplier, ref RadiusMultiplierTemp, hookCtx, false, context))
			{
				RadiusMultiplierTemp = RadiusMultiplier;
			}
			target.RadiusMultiplier = RadiusMultiplierTemp;
			float RadiusAddTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RadiusAdd, ref RadiusAddTemp, hookCtx, false, context))
			{
				RadiusAddTemp = RadiusAdd;
			}
			target.RadiusAdd = RadiusAddTemp;
			float EnergyMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EnergyMultiplier, ref EnergyMultiplierTemp, hookCtx, false, context))
			{
				EnergyMultiplierTemp = EnergyMultiplier;
			}
			target.EnergyMultiplier = EnergyMultiplierTemp;
			float EnergyAddTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EnergyAdd, ref EnergyAddTemp, hookCtx, false, context))
			{
				EnergyAddTemp = EnergyAdd;
			}
			target.EnergyAdd = EnergyAddTemp;
			float SoftnessMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SoftnessMultiplier, ref SoftnessMultiplierTemp, hookCtx, false, context))
			{
				SoftnessMultiplierTemp = SoftnessMultiplier;
			}
			target.SoftnessMultiplier = SoftnessMultiplierTemp;
			float SoftnessAddTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SoftnessAdd, ref SoftnessAddTemp, hookCtx, false, context))
			{
				SoftnessAddTemp = SoftnessAdd;
			}
			target.SoftnessAdd = SoftnessAddTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleSpotlightModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSpotlightModifierComponent cast = (VehicleSpotlightModifierComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSpotlightModifierComponent cast = (VehicleSpotlightModifierComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSpotlightModifierComponent def = (VehicleSpotlightModifierComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleSpotlightModifierComponent Instantiate()
	{
		return new VehicleSpotlightModifierComponent();
	}
}
