using System;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Barricade;

public abstract class SharedDirectionalAttackBlockSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, MeleeAttackAttemptEvent>((EntityEventRefHandler<MobStateComponent, MeleeAttackAttemptEvent>)OnMeleeAttackAttempt, (Type[])null, (Type[])null);
	}

	private void OnMeleeAttackAttempt(Entity<MobStateComponent> ent, ref MeleeAttackAttemptEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		foreach (NetEntity potentialTarget in args.PotentialTargets)
		{
			if (potentialTarget == args.Target)
			{
				break;
			}
			EntityUid target = ((EntitySystem)this).GetEntity(potentialTarget);
			if (!IsAttackBlocked(Entity<MobStateComponent>.op_Implicit(ent), target))
			{
				continue;
			}
			NetCoordinates newCoordinates = ((EntitySystem)this).GetNetCoordinates(_transform.GetMoverCoordinates(((EntitySystem)this).GetEntity(potentialTarget)), (MetaDataComponent)null);
			AttackEvent attack = args.Attack;
			if (!(attack is LightAttackEvent))
			{
				if (attack is DisarmAttackEvent)
				{
					args.Attack = new LightAttackEvent(potentialTarget, args.Weapon, newCoordinates);
				}
			}
			else
			{
				args.Attack = new LightAttackEvent(potentialTarget, args.Weapon, newCoordinates);
			}
			break;
		}
	}

	public bool IsAttackBlocked(EntityUid attacker, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		DirectionalAttackBlockerComponent blocker = default(DirectionalAttackBlockerComponent);
		if (!((EntitySystem)this).TryComp<DirectionalAttackBlockerComponent>(target, ref blocker) || !((EntitySystem)this).Transform(target).Anchored)
		{
			return false;
		}
		if (!blocker.BlockMarineAttacks && ((EntitySystem)this).HasComp<MarineComponent>(attacker))
		{
			return false;
		}
		if (!IsFacingTarget(target, attacker))
		{
			return false;
		}
		uint value = _timing.CurTick.Value;
		int iD = ((EntitySystem)this).GetNetEntity(attacker, (MetaDataComponent)null).Id;
		long seed = (long)(((ulong)value << 32) | (uint)iD);
		DamageableComponent damageable = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(target, ref damageable))
		{
			float num = Math.Max(blocker.MinimumBlockChance, ((float)blocker.MaxHealth - (float)damageable.TotalDamage) / (float)blocker.MaxHealth);
			float blockRoll = new Xoroshiro64S(seed).NextFloat(0f, 1f);
			if (num < blockRoll)
			{
				return false;
			}
		}
		return true;
	}

	private sbyte GetRelativeDiff(EntityUid blocker, EntityUid target, EntityCoordinates? originCoordinates = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates targetCoordinates = _transform.GetMoverCoordinates(target);
		if (originCoordinates.HasValue)
		{
			targetCoordinates = originCoordinates.Value;
		}
		(EntityCoordinates, Angle) blockerCoordinates = _transform.GetMoverCoordinateRotation(blocker, ((EntitySystem)this).Transform(blocker));
		Direction dir = DirectionExtensions.GetDir(Vector2Helpers.Normalized(targetCoordinates.Position - blockerCoordinates.Item1.Position));
		Direction blockerDirection = ((Angle)(ref blockerCoordinates.Item2)).GetDir();
		return Math.Abs((sbyte)(dir - blockerDirection));
	}

	public bool IsFacingTarget(EntityUid blocker, EntityUid target, EntityCoordinates? originCoordinates = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		sbyte relativeDiff = GetRelativeDiff(blocker, target, originCoordinates);
		if ((uint)relativeDiff <= 1u || relativeDiff == 7)
		{
			return true;
		}
		return false;
	}

	public bool IsBehindTarget(EntityUid blocker, EntityUid target, EntityCoordinates? originCoordinates = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		sbyte relativeDiff = GetRelativeDiff(blocker, target, originCoordinates);
		if ((uint)(relativeDiff - 3) <= 2u)
		{
			return true;
		}
		return false;
	}
}
