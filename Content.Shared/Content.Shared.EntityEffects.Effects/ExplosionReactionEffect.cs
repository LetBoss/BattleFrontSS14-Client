using System;
using Content.Shared.Database;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class ExplosionReactionEffect : EventEntityEffect<ExplosionReactionEffect>, ISerializationGenerated<ExplosionReactionEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, typeof(PrototypeIdSerializer<ExplosionPrototype>))]
	public string ExplosionType;

	[DataField(null, false, 1, false, false, null)]
	public float MaxIntensity = 5f;

	[DataField(null, false, 1, false, false, null)]
	public float IntensitySlope = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxTotalIntensity = 100f;

	[DataField(null, false, 1, false, false, null)]
	public float IntensityPerUnit = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float TileBreakScale = 1f;

	public override bool ShouldLog => true;

	public override LogImpact LogImpact => LogImpact.High;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-explosion-reaction-effect", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExplosionReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<ExplosionReactionEffect> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExplosionReactionEffect)definitionCast;
		if (!serialization.TryCustomCopy<ExplosionReactionEffect>(this, ref target, hookCtx, false, context))
		{
			string ExplosionTypeTemp = null;
			if (ExplosionType == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ExplosionType, ref ExplosionTypeTemp, hookCtx, false, context))
			{
				ExplosionTypeTemp = ExplosionType;
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
			float MaxTotalIntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxTotalIntensity, ref MaxTotalIntensityTemp, hookCtx, false, context))
			{
				MaxTotalIntensityTemp = MaxTotalIntensity;
			}
			target.MaxTotalIntensity = MaxTotalIntensityTemp;
			float IntensityPerUnitTemp = 0f;
			if (!serialization.TryCustomCopy<float>(IntensityPerUnit, ref IntensityPerUnitTemp, hookCtx, false, context))
			{
				IntensityPerUnitTemp = IntensityPerUnit;
			}
			target.IntensityPerUnit = IntensityPerUnitTemp;
			float TileBreakScaleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(TileBreakScale, ref TileBreakScaleTemp, hookCtx, false, context))
			{
				TileBreakScaleTemp = TileBreakScale;
			}
			target.TileBreakScale = TileBreakScaleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExplosionReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ExplosionReactionEffect> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionReactionEffect cast = (ExplosionReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionReactionEffect cast = (ExplosionReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExplosionReactionEffect Instantiate()
	{
		return new ExplosionReactionEffect();
	}
}
