using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects;

public sealed class HealthChange : EntityEffect, ISerializationGenerated<HealthChange>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[JsonPropertyName("damage")]
	public DamageSpecifier Damage;

	[DataField(null, false, 1, false, false, null)]
	[JsonPropertyName("scaleByQuantity")]
	public bool ScaleByQuantity;

	[DataField(null, false, 1, false, false, null)]
	[JsonPropertyName("ignoreResistances")]
	public bool IgnoreResistances = true;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		List<string> damages = new List<string>();
		bool heals = false;
		bool deals = false;
		DamageSpecifier damageSpec = new DamageSpecifier(Damage);
		float universalReagentDamageModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentDamageModifier;
		float universalReagentHealModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentHealModifier;
		string key;
		FixedPoint2 value;
		if (universalReagentDamageModifier != 1f || universalReagentHealModifier != 1f)
		{
			foreach (KeyValuePair<string, FixedPoint2> item in damageSpec.DamageDict)
			{
				item.Deconstruct(out key, out value);
				string type = key;
				FixedPoint2 val = value;
				if (val < 0f)
				{
					damageSpec.DamageDict[type] = val * universalReagentHealModifier;
				}
				if (val > 0f)
				{
					damageSpec.DamageDict[type] = val * universalReagentDamageModifier;
				}
			}
		}
		damageSpec = entSys.GetEntitySystem<DamageableSystem>().ApplyUniversalAllModifiers(damageSpec);
		foreach (KeyValuePair<string, FixedPoint2> item2 in damageSpec.DamageDict)
		{
			item2.Deconstruct(out key, out value);
			string kind = key;
			FixedPoint2 amount = value;
			int sign = FixedPoint2.Sign(amount);
			if (sign < 0)
			{
				heals = true;
			}
			if (sign > 0)
			{
				deals = true;
			}
			damages.Add(Loc.GetString("health-change-display", new(string, object)[3]
			{
				("kind", prototype.Index<DamageTypePrototype>(kind).LocalizedName),
				("amount", MathF.Abs(amount.Float())),
				("deltasign", sign)
			}));
		}
		string healsordeals = ((!heals) ? (deals ? "deals" : "none") : (deals ? "both" : "heals"));
		return Loc.GetString("reagent-effect-guidebook-health-change", new(string, object)[3]
		{
			("chance", Probability),
			("changes", ContentLocalizationManager.FormatList(damages)),
			("healsordeals", healsordeals)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 scale = FixedPoint2.New(1);
		DamageSpecifier damageSpec = new DamageSpecifier(Damage);
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			scale = (ScaleByQuantity ? (reagentArgs.Quantity * reagentArgs.Scale) : reagentArgs.Scale);
		}
		float universalReagentDamageModifier = args.EntityManager.System<DamageableSystem>().UniversalReagentDamageModifier;
		float universalReagentHealModifier = args.EntityManager.System<DamageableSystem>().UniversalReagentHealModifier;
		if (universalReagentDamageModifier != 1f || universalReagentHealModifier != 1f)
		{
			foreach (var (type, val) in damageSpec.DamageDict)
			{
				if (val < 0f)
				{
					damageSpec.DamageDict[type] = val * universalReagentHealModifier;
				}
				if (val > 0f)
				{
					damageSpec.DamageDict[type] = val * universalReagentDamageModifier;
				}
			}
		}
		args.EntityManager.System<DamageableSystem>().TryChangeDamage(args.TargetEntity, damageSpec * scale, IgnoreResistances, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HealthChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HealthChange)definitionCast;
		if (serialization.TryCustomCopy<HealthChange>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier DamageTemp = null;
		if (Damage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, false, context))
		{
			if (Damage == null)
			{
				DamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, context, true);
			}
		}
		target.Damage = DamageTemp;
		bool ScaleByQuantityTemp = false;
		if (!serialization.TryCustomCopy<bool>(ScaleByQuantity, ref ScaleByQuantityTemp, hookCtx, false, context))
		{
			ScaleByQuantityTemp = ScaleByQuantity;
		}
		target.ScaleByQuantity = ScaleByQuantityTemp;
		bool IgnoreResistancesTemp = false;
		if (!serialization.TryCustomCopy<bool>(IgnoreResistances, ref IgnoreResistancesTemp, hookCtx, false, context))
		{
			IgnoreResistancesTemp = IgnoreResistances;
		}
		target.IgnoreResistances = IgnoreResistancesTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HealthChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HealthChange cast = (HealthChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HealthChange cast = (HealthChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HealthChange Instantiate()
	{
		return new HealthChange();
	}
}
