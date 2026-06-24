using System;
using System.Collections.Generic;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(VehicleWheelSystem) })]
public sealed class VehicleWheelSlotsComponent : Component, ISerializationGenerated<VehicleWheelSlotsComponent>, ISerializationGenerated
{
	public const string WheelComponentId = "VehicleWheelItem";

	public const string HardpointTypeId = "Wheel";

	[DataField(null, false, 1, false, false, null)]
	public int SlotCount = 1;

	[DataField(null, false, 1, false, false, null)]
	public List<string> Slots = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public string SlotPrefix = "wheel";

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist WheelWhitelist = new EntityWhitelist
	{
		Components = new string[1] { "VehicleWheelItem" }
	};

	[DataField(null, false, 1, false, false, null)]
	public float CollisionDamagePerSpeed;

	[DataField(null, false, 1, false, false, null)]
	public float MinCollisionDamage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleWheelSlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleWheelSlotsComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<VehicleWheelSlotsComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		int SlotCountTemp = 0;
		if (!serialization.TryCustomCopy<int>(SlotCount, ref SlotCountTemp, hookCtx, false, context))
		{
			SlotCountTemp = SlotCount;
		}
		target.SlotCount = SlotCountTemp;
		List<string> SlotsTemp = null;
		if (Slots == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<string>>(Slots, ref SlotsTemp, hookCtx, true, context))
		{
			SlotsTemp = serialization.CreateCopy<List<string>>(Slots, hookCtx, context, false);
		}
		target.Slots = SlotsTemp;
		string SlotPrefixTemp = null;
		if (SlotPrefix == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(SlotPrefix, ref SlotPrefixTemp, hookCtx, false, context))
		{
			SlotPrefixTemp = SlotPrefix;
		}
		target.SlotPrefix = SlotPrefixTemp;
		EntityWhitelist WheelWhitelistTemp = null;
		if (WheelWhitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(WheelWhitelist, ref WheelWhitelistTemp, hookCtx, false, context))
		{
			if (WheelWhitelist == null)
			{
				WheelWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(WheelWhitelist, ref WheelWhitelistTemp, hookCtx, context, true);
			}
		}
		target.WheelWhitelist = WheelWhitelistTemp;
		float CollisionDamagePerSpeedTemp = 0f;
		if (!serialization.TryCustomCopy<float>(CollisionDamagePerSpeed, ref CollisionDamagePerSpeedTemp, hookCtx, false, context))
		{
			CollisionDamagePerSpeedTemp = CollisionDamagePerSpeed;
		}
		target.CollisionDamagePerSpeed = CollisionDamagePerSpeedTemp;
		float MinCollisionDamageTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MinCollisionDamage, ref MinCollisionDamageTemp, hookCtx, false, context))
		{
			MinCollisionDamageTemp = MinCollisionDamage;
		}
		target.MinCollisionDamage = MinCollisionDamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleWheelSlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleWheelSlotsComponent cast = (VehicleWheelSlotsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleWheelSlotsComponent cast = (VehicleWheelSlotsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleWheelSlotsComponent def = (VehicleWheelSlotsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleWheelSlotsComponent Instantiate()
	{
		return new VehicleWheelSlotsComponent();
	}
}
