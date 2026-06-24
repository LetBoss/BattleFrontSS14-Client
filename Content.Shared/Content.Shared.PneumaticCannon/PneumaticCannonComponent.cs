using System;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.PneumaticCannon;

[RegisterComponent]
[NetworkedComponent]
public sealed class PneumaticCannonComponent : Component, ISerializationGenerated<PneumaticCannonComponent>, ISerializationGenerated
{
	public const string TankSlotId = "gas_tank";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public PneumaticCannonPower Power = PneumaticCannonPower.Medium;

	[DataField("toolModifyPower", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
	public string ToolModifyPower = "Anchoring";

	[DataField("highPowerStunTime", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float HighPowerStunTime = 3f;

	[DataField("gasUsage", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float GasUsage = 0.142f;

	[DataField("baseProjectileSpeed", false, 1, false, false, null)]
	public float BaseProjectileSpeed = 20f;

	[DataField(null, false, 1, false, false, null)]
	public float? ProjectileSpeed;

	[DataField("throwItems", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool ThrowItems = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PneumaticCannonComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PneumaticCannonComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PneumaticCannonComponent>(this, ref target, hookCtx, false, context))
		{
			string ToolModifyPowerTemp = null;
			if (ToolModifyPower == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ToolModifyPower, ref ToolModifyPowerTemp, hookCtx, false, context))
			{
				ToolModifyPowerTemp = ToolModifyPower;
			}
			target.ToolModifyPower = ToolModifyPowerTemp;
			float HighPowerStunTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HighPowerStunTime, ref HighPowerStunTimeTemp, hookCtx, false, context))
			{
				HighPowerStunTimeTemp = HighPowerStunTime;
			}
			target.HighPowerStunTime = HighPowerStunTimeTemp;
			float GasUsageTemp = 0f;
			if (!serialization.TryCustomCopy<float>(GasUsage, ref GasUsageTemp, hookCtx, false, context))
			{
				GasUsageTemp = GasUsage;
			}
			target.GasUsage = GasUsageTemp;
			float BaseProjectileSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BaseProjectileSpeed, ref BaseProjectileSpeedTemp, hookCtx, false, context))
			{
				BaseProjectileSpeedTemp = BaseProjectileSpeed;
			}
			target.BaseProjectileSpeed = BaseProjectileSpeedTemp;
			float? ProjectileSpeedTemp = null;
			if (!serialization.TryCustomCopy<float?>(ProjectileSpeed, ref ProjectileSpeedTemp, hookCtx, false, context))
			{
				ProjectileSpeedTemp = ProjectileSpeed;
			}
			target.ProjectileSpeed = ProjectileSpeedTemp;
			bool ThrowItemsTemp = false;
			if (!serialization.TryCustomCopy<bool>(ThrowItems, ref ThrowItemsTemp, hookCtx, false, context))
			{
				ThrowItemsTemp = ThrowItems;
			}
			target.ThrowItems = ThrowItemsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PneumaticCannonComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PneumaticCannonComponent cast = (PneumaticCannonComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PneumaticCannonComponent cast = (PneumaticCannonComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PneumaticCannonComponent def = (PneumaticCannonComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PneumaticCannonComponent Instantiate()
	{
		return new PneumaticCannonComponent();
	}
}
