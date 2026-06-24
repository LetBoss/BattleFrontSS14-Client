using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chat;

public sealed class SharedSuicideSystem : EntitySystem
{
	private static readonly ProtoId<DamageTypePrototype> FallbackDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	public void ApplyLethalDamage(Entity<DamageableComponent> target, DamageSpecifier damageSpecifier)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier appliedDamageSpecifier = new DamageSpecifier(damageSpecifier);
		MobThresholdsComponent mobThresholds = default(MobThresholdsComponent);
		if (!((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<DamageableComponent>.op_Implicit(target), ref mobThresholds))
		{
			return;
		}
		FixedPoint2 lethalAmountOfDamage = mobThresholds.Thresholds.Keys.Last() - target.Comp.TotalDamage;
		FixedPoint2 totalDamage = appliedDamageSpecifier.GetTotal();
		appliedDamageSpecifier.DamageDict.Remove("Structural");
		foreach (var (key, value) in appliedDamageSpecifier.DamageDict)
		{
			appliedDamageSpecifier.DamageDict[key] = Math.Ceiling((double)(value * lethalAmountOfDamage / totalDamage));
		}
		_damageableSystem.TryChangeDamage(Entity<DamageableComponent>.op_Implicit(target), appliedDamageSpecifier, ignoreResistances: true, interruptsDoAfters: true, null, Entity<DamageableComponent>.op_Implicit(target));
	}

	public void ApplyLethalDamage(Entity<DamageableComponent> target, ProtoId<DamageTypePrototype>? damageType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		MobThresholdsComponent mobThresholds = default(MobThresholdsComponent);
		if (((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<DamageableComponent>.op_Implicit(target), ref mobThresholds))
		{
			FixedPoint2 lethalAmountOfDamage = mobThresholds.Thresholds.Keys.Last() - target.Comp.TotalDamage;
			DamageTypePrototype damagePrototype = default(DamageTypePrototype);
			if (!_prototypeManager.TryIndex<DamageTypePrototype>(damageType, ref damagePrototype) || damagePrototype.ID == "Structural")
			{
				((EntitySystem)this).Log.Error($"{"SharedSuicideSystem"} could not find the damage type prototype associated with {damageType}. Falling back to {FallbackDamageType}");
				damagePrototype = _prototypeManager.Index<DamageTypePrototype>(FallbackDamageType);
			}
			DamageSpecifier damage = new DamageSpecifier(damagePrototype, lethalAmountOfDamage);
			_damageableSystem.TryChangeDamage(Entity<DamageableComponent>.op_Implicit(target), damage, ignoreResistances: true, interruptsDoAfters: true, null, Entity<DamageableComponent>.op_Implicit(target));
		}
	}
}
