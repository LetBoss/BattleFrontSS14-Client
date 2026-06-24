using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(RMCVehicleDestroyedFireSystem) })]
public sealed class RMCVehicleDestroyedFireComponent : Component, ISerializationGenerated<RMCVehicleDestroyedFireComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId InteriorFire = EntProtoId.op_Implicit("RMCTileFireForeverWeak");

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId ExteriorFire = EntProtoId.op_Implicit("RMCTileFire");

	[DataField(null, false, 1, false, false, null)]
	public float ExteriorFireChance = 0.3f;

	[DataField(null, false, 1, false, false, null)]
	public int ExteriorPaddingTiles = 1;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCVehicleDestroyedFireComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCVehicleDestroyedFireComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCVehicleDestroyedFireComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId InteriorFireTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(InteriorFire, ref InteriorFireTemp, hookCtx, false, context))
			{
				InteriorFireTemp = serialization.CreateCopy<EntProtoId>(InteriorFire, hookCtx, context, false);
			}
			target.InteriorFire = InteriorFireTemp;
			EntProtoId ExteriorFireTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(ExteriorFire, ref ExteriorFireTemp, hookCtx, false, context))
			{
				ExteriorFireTemp = serialization.CreateCopy<EntProtoId>(ExteriorFire, hookCtx, context, false);
			}
			target.ExteriorFire = ExteriorFireTemp;
			float ExteriorFireChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExteriorFireChance, ref ExteriorFireChanceTemp, hookCtx, false, context))
			{
				ExteriorFireChanceTemp = ExteriorFireChance;
			}
			target.ExteriorFireChance = ExteriorFireChanceTemp;
			int ExteriorPaddingTilesTemp = 0;
			if (!serialization.TryCustomCopy<int>(ExteriorPaddingTiles, ref ExteriorPaddingTilesTemp, hookCtx, false, context))
			{
				ExteriorPaddingTilesTemp = ExteriorPaddingTiles;
			}
			target.ExteriorPaddingTiles = ExteriorPaddingTilesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCVehicleDestroyedFireComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDestroyedFireComponent cast = (RMCVehicleDestroyedFireComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDestroyedFireComponent cast = (RMCVehicleDestroyedFireComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDestroyedFireComponent def = (RMCVehicleDestroyedFireComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCVehicleDestroyedFireComponent Instantiate()
	{
		return new RMCVehicleDestroyedFireComponent();
	}
}
