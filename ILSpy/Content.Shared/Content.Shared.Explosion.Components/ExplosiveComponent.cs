using System;
using Content.Shared.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Explosion.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedExplosionSystem) })]
public sealed class ExplosiveComponent : Component, ISerializationGenerated<ExplosiveComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ExplosionPrototype> ExplosionType;

	[DataField(null, false, 1, false, false, null)]
	public float MaxIntensity = 4f;

	[DataField(null, false, 1, false, false, null)]
	public float IntensitySlope = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float TotalIntensity = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float TileBreakScale = 1f;

	[DataField(null, false, 1, false, false, null)]
	public int MaxTileBreak = int.MaxValue;

	[DataField(null, false, 1, false, false, null)]
	public bool CanCreateVacuum = true;

	[DataField(null, false, 1, false, false, null)]
	public bool? DeleteAfterExplosion;

	[DataField(null, false, 1, false, false, null)]
	public bool Repeatable;

	public bool Exploded;

	[DataField(null, false, 1, false, false, null)]
	public float? VehicleLightDamage;

	[DataField(null, false, 1, false, false, null)]
	public float? VehicleHeavyDamage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExplosiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExplosiveComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ExplosiveComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ExplosionPrototype> ExplosionTypeTemp = default(ProtoId<ExplosionPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ExplosionPrototype>>(ExplosionType, ref ExplosionTypeTemp, hookCtx, false, context))
			{
				ExplosionTypeTemp = serialization.CreateCopy<ProtoId<ExplosionPrototype>>(ExplosionType, hookCtx, context, false);
			}
			target.ExplosionType = ExplosionTypeTemp;
			float MaxIntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxIntensity, ref MaxIntensityTemp, hookCtx, false, context))
			{
				MaxIntensityTemp = MaxIntensity;
			}
			target.MaxIntensity = MaxIntensityTemp;
			float IntensitySlopeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(IntensitySlope, ref IntensitySlopeTemp, hookCtx, false, context))
			{
				IntensitySlopeTemp = IntensitySlope;
			}
			target.IntensitySlope = IntensitySlopeTemp;
			float TotalIntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(TotalIntensity, ref TotalIntensityTemp, hookCtx, false, context))
			{
				TotalIntensityTemp = TotalIntensity;
			}
			target.TotalIntensity = TotalIntensityTemp;
			float TileBreakScaleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(TileBreakScale, ref TileBreakScaleTemp, hookCtx, false, context))
			{
				TileBreakScaleTemp = TileBreakScale;
			}
			target.TileBreakScale = TileBreakScaleTemp;
			int MaxTileBreakTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxTileBreak, ref MaxTileBreakTemp, hookCtx, false, context))
			{
				MaxTileBreakTemp = MaxTileBreak;
			}
			target.MaxTileBreak = MaxTileBreakTemp;
			bool CanCreateVacuumTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanCreateVacuum, ref CanCreateVacuumTemp, hookCtx, false, context))
			{
				CanCreateVacuumTemp = CanCreateVacuum;
			}
			target.CanCreateVacuum = CanCreateVacuumTemp;
			bool? DeleteAfterExplosionTemp = null;
			if (!serialization.TryCustomCopy<bool?>(DeleteAfterExplosion, ref DeleteAfterExplosionTemp, hookCtx, false, context))
			{
				DeleteAfterExplosionTemp = DeleteAfterExplosion;
			}
			target.DeleteAfterExplosion = DeleteAfterExplosionTemp;
			bool RepeatableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Repeatable, ref RepeatableTemp, hookCtx, false, context))
			{
				RepeatableTemp = Repeatable;
			}
			target.Repeatable = RepeatableTemp;
			float? VehicleLightDamageTemp = null;
			if (!serialization.TryCustomCopy<float?>(VehicleLightDamage, ref VehicleLightDamageTemp, hookCtx, false, context))
			{
				VehicleLightDamageTemp = VehicleLightDamage;
			}
			target.VehicleLightDamage = VehicleLightDamageTemp;
			float? VehicleHeavyDamageTemp = null;
			if (!serialization.TryCustomCopy<float?>(VehicleHeavyDamage, ref VehicleHeavyDamageTemp, hookCtx, false, context))
			{
				VehicleHeavyDamageTemp = VehicleHeavyDamage;
			}
			target.VehicleHeavyDamage = VehicleHeavyDamageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExplosiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosiveComponent cast = (ExplosiveComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosiveComponent cast = (ExplosiveComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosiveComponent def = (ExplosiveComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExplosiveComponent Instantiate()
	{
		return new ExplosiveComponent();
	}
}
