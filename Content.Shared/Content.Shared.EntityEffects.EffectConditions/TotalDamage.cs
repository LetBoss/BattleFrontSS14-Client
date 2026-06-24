using System;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class TotalDamage : EntityEffectCondition, ISerializationGenerated<TotalDamage>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Max = FixedPoint2.MaxValue;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Min = FixedPoint2.Zero;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damage = default(DamageableComponent);
		if (args.EntityManager.TryGetComponent<DamageableComponent>(args.TargetEntity, ref damage))
		{
			FixedPoint2 total = damage.TotalDamage;
			if (total > Min && total < Max)
			{
				return true;
			}
		}
		return false;
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-total-damage", new(string, object)[2]
		{
			("max", (Max == FixedPoint2.MaxValue) ? 2.1474836E+09f : Max.Float()),
			("min", Min.Float())
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TotalDamage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TotalDamage)definitionCast;
		if (!serialization.TryCustomCopy<TotalDamage>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 MaxTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = serialization.CreateCopy<FixedPoint2>(Max, hookCtx, context, false);
			}
			target.Max = MaxTemp;
			FixedPoint2 MinTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Min, ref MinTemp, hookCtx, false, context))
			{
				MinTemp = serialization.CreateCopy<FixedPoint2>(Min, hookCtx, context, false);
			}
			target.Min = MinTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TotalDamage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TotalDamage cast = (TotalDamage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TotalDamage cast = (TotalDamage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TotalDamage Instantiate()
	{
		return new TotalDamage();
	}
}
