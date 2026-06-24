using System;
using System.Collections.Generic;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Chemistry.Effects;

public abstract class RMCChemicalEffect : EntityEffect, ISerializationGenerated<RMCChemicalEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Potency;

	private float _moddedPotency;

	[DataField(null, false, 1, false, false, null)]
	public float NutFactor;

	[DataField(null, false, 1, false, false, null)]
	public float NutMetabolism;

	public float ActualPotency => ((_moddedPotency != 0f) ? _moddedPotency : Potency) * 0.5f;

	public float PotencyPerSecond => ActualPotency * 0.5f;

	public float NutrimentFactor => NutFactor * NutMetabolism;

	public override void Effect(EntityEffectBaseArgs args)
	{
		if (!(args is EntityEffectReagentArgs reagentArgs))
		{
			return;
		}
		ReagentPrototype reagent = reagentArgs.Reagent;
		if (reagent == null)
		{
			return;
		}
		DamageableSystem damageable = args.EntityManager.System<DamageableSystem>();
		FixedPoint2 scale = reagentArgs.Scale;
		float boost = CalculateReagentBoost(reagentArgs);
		_moddedPotency = Potency + boost;
		FixedPoint2 scaledPotency = PotencyPerSecond * scale;
		Tick(damageable, scaledPotency, reagentArgs);
		FixedPoint2 totalQuantity = FixedPoint2.Zero;
		if (reagentArgs.Source != null)
		{
			totalQuantity = reagentArgs.Source.GetTotalPrototypeQuantity(reagent.ID);
		}
		if (reagent.Overdose.HasValue)
		{
			FixedPoint2 value = totalQuantity;
			FixedPoint2? overdose = reagent.Overdose;
			if (value >= overdose)
			{
				TickOverdose(damageable, scaledPotency, reagentArgs);
			}
		}
		if (reagent.CriticalOverdose.HasValue)
		{
			FixedPoint2 value = totalQuantity;
			FixedPoint2? overdose = reagent.CriticalOverdose;
			if (value >= overdose)
			{
				TickCriticalOverdose(damageable, scaledPotency, reagentArgs);
			}
		}
	}

	private static float CalculateReagentBoost(EntityEffectReagentArgs args)
	{
		float boost = 0f;
		if (args.Reagent?.Metabolisms == null)
		{
			return boost;
		}
		foreach (KeyValuePair<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry> metabolism in args.Reagent.Metabolisms)
		{
			metabolism.Deconstruct(out var _, out var value);
			EntityEffect[] effects = value.Effects;
			for (int i = 0; i < effects.Length; i++)
			{
				if (effects[i] is RMCChemicalEffect rmcEffect)
				{
					rmcEffect.ReagentBoost(args, ref boost);
				}
			}
		}
		return boost;
	}

	protected virtual void ReagentBoost(EntityEffectReagentArgs args, ref float boost)
	{
	}

	protected virtual void Tick(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
	}

	protected virtual void TickOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
	}

	protected virtual void TickCriticalOverdose(DamageableSystem damageable, FixedPoint2 potency, EntityEffectReagentArgs args)
	{
	}

	public RMCChemicalEffect()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCChemicalEffect)definitionCast;
		if (!serialization.TryCustomCopy<RMCChemicalEffect>(this, ref target, hookCtx, false, context))
		{
			float PotencyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Potency, ref PotencyTemp, hookCtx, false, context))
			{
				PotencyTemp = Potency;
			}
			target.Potency = PotencyTemp;
			float NutFactorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NutFactor, ref NutFactorTemp, hookCtx, false, context))
			{
				NutFactorTemp = NutFactor;
			}
			target.NutFactor = NutFactorTemp;
			float NutMetabolismTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NutMetabolism, ref NutMetabolismTemp, hookCtx, false, context))
			{
				NutMetabolismTemp = NutMetabolism;
			}
			target.NutMetabolism = NutMetabolismTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref RMCChemicalEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect cast = (RMCChemicalEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCChemicalEffect cast = (RMCChemicalEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCChemicalEffect Instantiate()
	{
		throw new NotImplementedException();
	}
}
