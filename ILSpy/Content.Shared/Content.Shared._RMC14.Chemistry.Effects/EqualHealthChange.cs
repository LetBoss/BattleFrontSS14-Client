using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Content.Shared._RMC14.Damage;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class EqualHealthChange : EntityEffect, ISerializationGenerated<EqualHealthChange>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[JsonPropertyName("damage")]
	public List<(ProtoId<DamageGroupPrototype> Group, FixedPoint2 Amount)> Damage = new List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>();

	[DataField(null, false, 1, false, false, null)]
	[JsonPropertyName("ignoreResistances")]
	public bool IgnoreResistances = true;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		List<string> damages = new List<string>();
		bool heals = false;
		bool deals = false;
		DamageGroupPrototype group = default(DamageGroupPrototype);
		foreach (var (groupId, amount) in Damage)
		{
			if (prototype.TryIndex<DamageGroupPrototype>(groupId, ref group))
			{
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
					("kind", group.LocalizedName),
					("amount", MathF.Abs(amount.Float())),
					("deltasign", sign)
				}));
			}
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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = new DamageSpecifier();
		SharedRMCDamageableSystem rmcDamageable = args.EntityManager.System<SharedRMCDamageableSystem>();
		FixedPoint2 scale = (args as EntityEffectReagentArgs)?.Scale ?? ((FixedPoint2)1);
		foreach (var item in Damage)
		{
			ProtoId<DamageGroupPrototype> group = item.Group;
			FixedPoint2 amount = item.Amount;
			damage = rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit(args.TargetEntity), group, amount * scale, damage);
		}
		args.EntityManager.System<DamageableSystem>().TryChangeDamage(args.TargetEntity, damage, IgnoreResistances, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EqualHealthChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EqualHealthChange)definitionCast;
		if (!serialization.TryCustomCopy<EqualHealthChange>(this, ref target, hookCtx, false, context))
		{
			List<(ProtoId<DamageGroupPrototype>, FixedPoint2)> DamageTemp = null;
			if (Damage == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>>(Damage, ref DamageTemp, hookCtx, true, context))
			{
				DamageTemp = serialization.CreateCopy<List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>>(Damage, hookCtx, context, false);
			}
			target.Damage = DamageTemp;
			bool IgnoreResistancesTemp = false;
			if (!serialization.TryCustomCopy<bool>(IgnoreResistances, ref IgnoreResistancesTemp, hookCtx, false, context))
			{
				IgnoreResistancesTemp = IgnoreResistances;
			}
			target.IgnoreResistances = IgnoreResistancesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EqualHealthChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EqualHealthChange cast = (EqualHealthChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EqualHealthChange cast = (EqualHealthChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EqualHealthChange Instantiate()
	{
		return new EqualHealthChange();
	}
}
