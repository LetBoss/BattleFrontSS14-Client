using System;
using System.Text.Json.Serialization;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class RemoveDamage : EntityEffect, ISerializationGenerated<RemoveDamage>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[JsonPropertyName("group")]
	public ProtoId<DamageGroupPrototype> Group;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DamageGroupPrototype type = default(DamageGroupPrototype);
		if (!prototype.TryIndex<DamageGroupPrototype>(Group, ref type))
		{
			return null;
		}
		return "Removes all " + type.LocalizedName + " damage";
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		DamageGroupPrototype group = default(DamageGroupPrototype);
		if ((args is EntityEffectReagentArgs reagent && reagent.Scale < 0.95f) || !args.EntityManager.TryGetComponent<DamageableComponent>(args.TargetEntity, ref damageable) || !IoCManager.Resolve<IPrototypeManager>().TryIndex<DamageGroupPrototype>(Group, ref group))
		{
			return;
		}
		DamageSpecifier damage = new DamageSpecifier();
		foreach (string type in group.DamageTypes)
		{
			if (damageable.Damage.DamageDict.TryGetValue(type, out var amount))
			{
				damage.DamageDict[type] = -amount;
			}
		}
		args.EntityManager.System<DamageableSystem>().TryChangeDamage(args.TargetEntity, damage, ignoreResistances: true, interruptsDoAfters: false);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RemoveDamage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RemoveDamage)definitionCast;
		if (!serialization.TryCustomCopy<RemoveDamage>(this, ref target, hookCtx, false, context))
		{
			ProtoId<DamageGroupPrototype> GroupTemp = default(ProtoId<DamageGroupPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(Group, ref GroupTemp, hookCtx, false, context))
			{
				GroupTemp = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(Group, hookCtx, context, false);
			}
			target.Group = GroupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RemoveDamage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RemoveDamage cast = (RemoveDamage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RemoveDamage cast = (RemoveDamage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RemoveDamage Instantiate()
	{
		return new RemoveDamage();
	}
}
