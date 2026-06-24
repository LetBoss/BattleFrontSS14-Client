using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Weapons.Ranged.Components;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class BatteryWeaponFireMode : ISerializationGenerated<BatteryWeaponFireMode>, ISerializationGenerated
{
	[DataField("proto", false, 1, true, false, null)]
	public EntProtoId Prototype;

	[DataField(null, false, 1, false, false, null)]
	public float FireCost = 100f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BatteryWeaponFireMode target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<BatteryWeaponFireMode>(this, ref target, hookCtx, false, context))
		{
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			float FireCostTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FireCost, ref FireCostTemp, hookCtx, false, context))
			{
				FireCostTemp = FireCost;
			}
			target.FireCost = FireCostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BatteryWeaponFireMode target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BatteryWeaponFireMode cast = (BatteryWeaponFireMode)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BatteryWeaponFireMode Instantiate()
	{
		return new BatteryWeaponFireMode();
	}
}
