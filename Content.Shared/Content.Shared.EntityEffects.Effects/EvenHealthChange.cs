using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.EntityEffects.Effects;

public sealed class EvenHealthChange : EntityEffect, ISerializationGenerated<EvenHealthChange>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> Damage = new Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>();

	[DataField(null, false, 1, false, false, null)]
	public bool ScaleByQuantity;

	[DataField(null, false, 1, false, false, null)]
	public bool IgnoreResistances = true;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		List<string> damages = new List<string>();
		bool heals = false;
		bool deals = false;
		DamageableSystem entitySystem = entSys.GetEntitySystem<DamageableSystem>();
		float universalReagentDamageModifier = entitySystem.UniversalReagentDamageModifier;
		float universalReagentHealModifier = entitySystem.UniversalReagentHealModifier;
		foreach (KeyValuePair<ProtoId<DamageGroupPrototype>, FixedPoint2> item in Damage)
		{
			item.Deconstruct(out var key, out var value);
			ProtoId<DamageGroupPrototype> group = key;
			FixedPoint2 amount = value;
			DamageGroupPrototype groupProto = prototype.Index<DamageGroupPrototype>(group);
			int sign = FixedPoint2.Sign(amount);
			float mod = 1f;
			if (sign < 0)
			{
				heals = true;
				mod = universalReagentHealModifier;
			}
			else if (sign > 0)
			{
				deals = true;
				mod = universalReagentDamageModifier;
			}
			damages.Add(Loc.GetString("health-change-display", new(string, object)[3]
			{
				("kind", groupProto.LocalizedName),
				("amount", MathF.Abs(amount.Float() * mod)),
				("deltasign", sign)
			}));
		}
		string healsordeals = ((!heals) ? (deals ? "deals" : "none") : (deals ? "both" : "heals"));
		return Loc.GetString("reagent-effect-guidebook-even-health-change", new(string, object)[3]
		{
			("chance", Probability),
			("changes", ContentLocalizationManager.FormatList(damages)),
			("healsordeals", healsordeals)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!args.EntityManager.TryGetComponent<DamageableComponent>(args.TargetEntity, ref damageable))
		{
			return;
		}
		IPrototypeManager protoMan = IoCManager.Resolve<IPrototypeManager>();
		FixedPoint2 scale = FixedPoint2.New(1);
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			scale = (ScaleByQuantity ? (reagentArgs.Quantity * reagentArgs.Scale) : reagentArgs.Scale);
		}
		DamageableSystem damagableSystem = args.EntityManager.System<DamageableSystem>();
		float universalReagentDamageModifier = damagableSystem.UniversalReagentDamageModifier;
		float universalReagentHealModifier = damagableSystem.UniversalReagentHealModifier;
		DamageSpecifier dspec = new DamageSpecifier();
		FixedPoint2 value;
		string key2;
		foreach (KeyValuePair<ProtoId<DamageGroupPrototype>, FixedPoint2> item in Damage)
		{
			item.Deconstruct(out var key, out value);
			ProtoId<DamageGroupPrototype> group = key;
			FixedPoint2 amount = value;
			DamageGroupPrototype damageGroupPrototype = protoMan.Index<DamageGroupPrototype>(group);
			Dictionary<string, FixedPoint2> groupDamage = new Dictionary<string, FixedPoint2>();
			foreach (string damageId in damageGroupPrototype.DamageTypes)
			{
				FixedPoint2 damageAmount = damageable.Damage.DamageDict.GetValueOrDefault(damageId);
				if (damageAmount != FixedPoint2.Zero)
				{
					groupDamage.Add(damageId, damageAmount);
				}
			}
			FixedPoint2 sum = groupDamage.Values.Sum();
			foreach (KeyValuePair<string, FixedPoint2> item2 in groupDamage)
			{
				item2.Deconstruct(out key2, out value);
				string damageId2 = key2;
				FixedPoint2 damageAmount2 = value;
				FixedPoint2 existing = Extensions.GetOrNew<string, FixedPoint2>(dspec.DamageDict, damageId2);
				dspec.DamageDict[damageId2] = existing + damageAmount2 / sum * amount;
			}
		}
		if (universalReagentDamageModifier != 1f || universalReagentHealModifier != 1f)
		{
			foreach (KeyValuePair<string, FixedPoint2> item3 in dspec.DamageDict)
			{
				item3.Deconstruct(out key2, out value);
				string type = key2;
				FixedPoint2 val = value;
				if (val < 0f)
				{
					dspec.DamageDict[type] = val * universalReagentHealModifier;
				}
				if (val > 0f)
				{
					dspec.DamageDict[type] = val * universalReagentDamageModifier;
				}
			}
		}
		damagableSystem.TryChangeDamage(args.TargetEntity, dspec * scale, IgnoreResistances, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EvenHealthChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EvenHealthChange)definitionCast;
		if (!serialization.TryCustomCopy<EvenHealthChange>(this, ref target, hookCtx, false, context))
		{
			Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> DamageTemp = null;
			if (Damage == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>>(Damage, ref DamageTemp, hookCtx, true, context))
			{
				DamageTemp = serialization.CreateCopy<Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>>(Damage, hookCtx, context, false);
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
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EvenHealthChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EvenHealthChange cast = (EvenHealthChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EvenHealthChange cast = (EvenHealthChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EvenHealthChange Instantiate()
	{
		return new EvenHealthChange();
	}
}
