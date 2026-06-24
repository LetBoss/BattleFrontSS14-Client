using System;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Projectiles.StoppingPower;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Projectiles;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Projectiles.Aimed;

public sealed class AimedProjectileSystem : EntitySystem
{
	private const float BigXenoSlowDurationMultiplier = 0.6f;

	private const float BigXenoBlindDurationMultiplier = 0.4f;

	private const string BlindKey = "Blinded";

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private MobThresholdSystem _mobThresholds;

	[Dependency]
	private RMCDazedSystem _dazed;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AimedProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<AimedProjectileComponent, ProjectileHitEvent>)OnAimedProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AimedProjectileComponent, BeforeAreaDamageEvent>((EntityEventRefHandler<AimedProjectileComponent, BeforeAreaDamageEvent>)OnBeforeAreaDamage, (Type[])null, (Type[])null);
	}

	private void OnAimedProjectileHit(Entity<AimedProjectileComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		AimedShotEffectComponent aimedEffect = default(AimedShotEffectComponent);
		if (!((EntitySystem)this).TryComp<AimedShotEffectComponent>(Entity<AimedProjectileComponent>.op_Implicit(ent), ref aimedEffect) || args.Handled)
		{
			return;
		}
		RMCFocusedShootingComponent focused = default(RMCFocusedShootingComponent);
		if (((EntitySystem)this).TryComp<RMCFocusedShootingComponent>(ent.Comp.Source, ref focused))
		{
			CalculateFocusEffects(ent, args.Target, focused, aimedEffect);
		}
		EntityUid target = args.Target;
		TimeSpan superSlowDuration = aimedEffect.SuperSlowDuration;
		TimeSpan blindDuration = aimedEffect.BlindDuration;
		_sizeStun.TryGetSize(Entity<AimedProjectileComponent>.op_Implicit(ent), out var size);
		if (!(target != ent.Comp.Target))
		{
			if ((int)size >= 5)
			{
				superSlowDuration *= 0.6000000238418579;
				blindDuration *= 0.4000000059604645;
			}
			int apValue = 0;
			DamageSpecifier damage = args.Damage * aimedEffect.ExtraHits + aimedEffect.CurrentHealthDamage;
			CMArmorPiercingComponent armorPiercing = default(CMArmorPiercingComponent);
			if (((EntitySystem)this).TryComp<CMArmorPiercingComponent>(Entity<AimedProjectileComponent>.op_Implicit(ent), ref armorPiercing))
			{
				apValue = armorPiercing.Amount;
			}
			DamageableSystem damageable = _damageable;
			EntityUid? uid = target;
			EntityUid? tool = ent.Comp.Source;
			int armorPiercing2 = apValue;
			damageable.TryChangeDamage(uid, damage, ignoreResistances: false, interruptsDoAfters: true, null, null, tool, armorPiercing2);
			_slow.TrySlowdown(target, aimedEffect.SlowDuration);
			_slow.TrySuperSlowdown(target, superSlowDuration);
			_statusEffects.TryAddStatusEffect<RMCBlindedComponent>(target, "Blinded", blindDuration, false, (StatusEffectsComponent?)null, false);
			IgniteOnProjectileHitComponent ignite = default(IgniteOnProjectileHitComponent);
			if (((EntitySystem)this).TryComp<IgniteOnProjectileHitComponent>(Entity<AimedProjectileComponent>.op_Implicit(ent), ref ignite))
			{
				ignite.Duration += aimedEffect.FireStacksOnHit;
			}
		}
	}

	private void OnBeforeAreaDamage(Entity<AimedProjectileComponent> ent, ref BeforeAreaDamageEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Target == ent.Comp.Target)
		{
			args.Cancelled = true;
		}
	}

	private void CalculateFocusEffects(Entity<AimedProjectileComponent> ent, EntityUid target, RMCFocusedShootingComponent focusEffect, AimedShotEffectComponent aimedEffect)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		float slowDuration = 0f;
		bool focusedFire = false;
		_sizeStun.TryGetSize(target, out var size);
		RMCStoppingPowerComponent stoppingPower = default(RMCStoppingPowerComponent);
		if (((EntitySystem)this).TryComp<RMCStoppingPowerComponent>(Entity<AimedProjectileComponent>.op_Implicit(ent), ref stoppingPower))
		{
			slowDuration = stoppingPower.CurrentStoppingPower;
		}
		if (slowDuration < focusEffect.SlowThreshold)
		{
			slowDuration = 0f;
		}
		if ((int)size >= 2)
		{
			float currentHealthDamage = focusEffect.CurrentHealthDamageSmallXeno;
			float damageIncreaseModifier = focusEffect.BonusDamageXeno;
			if ((int)size >= 4)
			{
				currentHealthDamage = focusEffect.CurrentHealthDamageXeno;
				slowDuration = Math.Max(slowDuration - 1f, 0f);
				focusedFire = true;
			}
			if ((int)size >= 5)
			{
				damageIncreaseModifier = focusEffect.BonusDamageBigXeno;
				currentHealthDamage = focusEffect.CurrentHealthDamageBigXeno;
				slowDuration = Math.Max(slowDuration - 1f, 0f);
				focusedFire = true;
			}
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(target, ref damageable))
			{
				_mobThresholds.TryGetIncapThreshold(target, out var threshold);
				if (!threshold.HasValue)
				{
					return;
				}
				DamageSpecifier damage = new DamageSpecifier();
				damage.DamageDict.Add("Piercing", (threshold.Value - damageable.TotalDamage) * currentHealthDamage);
				if (focusedFire)
				{
					damage *= focusEffect.BaseFocusMultiplier + focusEffect.FocusMultiplier * (float)focusEffect.FocusCounter;
					damageIncreaseModifier *= focusEffect.BaseFocusMultiplier + focusEffect.FocusMultiplier * (float)focusEffect.FocusCounter;
					slowDuration *= focusEffect.BaseFocusMultiplier + focusEffect.FocusMultiplier * (float)focusEffect.FocusCounter;
				}
				aimedEffect.ExtraHits = damageIncreaseModifier;
				aimedEffect.SuperSlowDuration = TimeSpan.FromSeconds(slowDuration);
				aimedEffect.CurrentHealthDamage = damage;
			}
		}
		if (size != RMCSizes.SmallXeno && slowDuration > 0f)
		{
			aimedEffect.SlowDuration = TimeSpan.FromSeconds(slowDuration);
			if (slowDuration > focusEffect.SlowThreshold)
			{
				aimedEffect.SuperSlowDuration = TimeSpan.FromSeconds(slowDuration);
			}
			if (stoppingPower != null && stoppingPower.CurrentStoppingPower > focusEffect.DazeThreshold)
			{
				_dazed.TryDaze(target, TimeSpan.FromSeconds(focusEffect.DazeDuration));
			}
		}
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)aimedEffect, (MetaDataComponent)null);
	}
}
