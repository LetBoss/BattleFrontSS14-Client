using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Weapons;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgAntiVehicleProjectileComponent : Component, ISerializationGenerated<PubgAntiVehicleProjectileComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float VehicleDamageMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float BonusFrameDamageFraction;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgAntiVehicleProjectileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgAntiVehicleProjectileComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgAntiVehicleProjectileComponent>(this, ref target, hookCtx, false, context))
		{
			float VehicleDamageMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VehicleDamageMultiplier, ref VehicleDamageMultiplierTemp, hookCtx, false, context))
			{
				VehicleDamageMultiplierTemp = VehicleDamageMultiplier;
			}
			target.VehicleDamageMultiplier = VehicleDamageMultiplierTemp;
			float BonusFrameDamageFractionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BonusFrameDamageFraction, ref BonusFrameDamageFractionTemp, hookCtx, false, context))
			{
				BonusFrameDamageFractionTemp = BonusFrameDamageFraction;
			}
			target.BonusFrameDamageFraction = BonusFrameDamageFractionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgAntiVehicleProjectileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgAntiVehicleProjectileComponent cast = (PubgAntiVehicleProjectileComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgAntiVehicleProjectileComponent cast = (PubgAntiVehicleProjectileComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgAntiVehicleProjectileComponent def = (PubgAntiVehicleProjectileComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgAntiVehicleProjectileComponent Instantiate()
	{
		return new PubgAntiVehicleProjectileComponent();
	}
}
