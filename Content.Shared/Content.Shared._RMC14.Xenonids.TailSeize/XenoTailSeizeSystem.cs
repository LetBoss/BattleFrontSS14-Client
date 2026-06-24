using System;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hook;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.ActionBlocker;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.TailSeize;

public sealed class XenoTailSeizeSystem : EntitySystem
{
	[Dependency]
	private XenoHookSystem _hook;

	[Dependency]
	private XenoProjectileSystem _projectile;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private RMCPullingSystem _pulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCObstacleSlammingSystem _obstacleSlamming;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoTailSeizeComponent, XenoTailSeizeActionEvent>((EntityEventRefHandler<XenoTailSeizeComponent, XenoTailSeizeActionEvent>)OnTailSeizeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimTailSeizedComponent, StopThrowEvent>((EntityEventRefHandler<VictimTailSeizedComponent, StopThrowEvent>)OnSeizeEnd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHookComponent, AmmoShotEvent>((EntityEventRefHandler<XenoHookComponent, AmmoShotEvent>)OnHookMade, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHookOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<XenoHookOnHitComponent, ProjectileHitEvent>)OnHookHit, (Type[])null, (Type[])null);
	}

	private void OnHookMade(Entity<XenoHookComponent> hook, ref AmmoShotEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid shot in args.FiredProjectiles)
		{
			_hook.TryHookTarget(hook, shot);
		}
	}

	private void OnHookHit(Entity<XenoHookOnHitComponent> hook, ref ProjectileHitEvent args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		XenoHookComponent hookComp = default(XenoHookComponent);
		if (!_net.IsClient && args.Shooter.HasValue && _xeno.CanAbilityAttackTarget(args.Shooter.Value, args.Target) && ((EntitySystem)this).TryComp<XenoHookComponent>(args.Shooter, ref hookComp) && _hook.TryHookTarget(Entity<XenoHookComponent>.op_Implicit((args.Shooter.Value, hookComp)), args.Target))
		{
			_pulling.TryStopAllPullsFromAndOn(args.Target);
			EntityCoordinates origin = _transform.GetMoverCoordinates(args.Shooter.Value);
			MapCoordinates mapCoords = _transform.GetMapCoordinates(args.Shooter.Value, (TransformComponent)null);
			EntityCoordinates target = _transform.GetMoverCoordinates(args.Target);
			float dis = default(float);
			if (((EntityCoordinates)(ref origin)).TryDistance((IEntityManager)(object)base.EntityManager, target, ref dis))
			{
				float knockBackDistance = 0f - Math.Max(dis - 2f, 0.5f);
				_obstacleSlamming.MakeImmune(args.Target);
				_size.KnockBack(args.Target, mapCoords, knockBackDistance, knockBackDistance, 10f);
				((EntitySystem)this).EnsureComp<VictimTailSeizedComponent>(args.Target);
			}
		}
	}

	private void OnSeizeEnd(Entity<VictimTailSeizedComponent> victim, ref StopThrowEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_slow.TrySlowdown(Entity<VictimTailSeizedComponent>.op_Implicit(victim), victim.Comp.SlowTime, refresh: true, ignoreDurationModifier: true);
		_slow.TryRoot(Entity<VictimTailSeizedComponent>.op_Implicit(victim), victim.Comp.RootTime);
		((EntitySystem)this).RemCompDeferred<VictimTailSeizedComponent>(Entity<VictimTailSeizedComponent>.op_Implicit(victim));
	}

	private void OnTailSeizeAction(Entity<XenoTailSeizeComponent> xeno, ref XenoTailSeizeActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_actionBlocker.CanAttack(Entity<XenoTailSeizeComponent>.op_Implicit(xeno)))
		{
			return;
		}
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<XenoTailSeizeComponent>.op_Implicit(xeno), ref melee))
		{
			if (_timing.CurTime < melee.NextAttack)
			{
				return;
			}
			melee.NextAttack = _timing.CurTime + TimeSpan.FromSeconds(1L);
			((EntitySystem)this).Dirty(Entity<XenoTailSeizeComponent>.op_Implicit(xeno), (IComponent)(object)melee, (MetaDataComponent)null);
		}
		XenoProjectileSystem projectile = _projectile;
		EntityUid xeno2 = Entity<XenoTailSeizeComponent>.op_Implicit(xeno);
		EntityCoordinates target = args.Target;
		FixedPoint2 plasma = 0;
		EntProtoId projectile2 = xeno.Comp.Projectile;
		Angle zero = Angle.Zero;
		float speed = xeno.Comp.Speed;
		EntityUid? entity = args.Entity;
		projectile.TryShoot(xeno2, target, plasma, projectile2, null, 1, zero, speed, null, entity);
		MeleeAttackEvent attackEv = new MeleeAttackEvent(Entity<XenoTailSeizeComponent>.op_Implicit(xeno));
		((EntitySystem)this).RaiseLocalEvent<MeleeAttackEvent>(Entity<XenoTailSeizeComponent>.op_Implicit(xeno), ref attackEv, false);
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoTailSeizeComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoTailSeizeComponent>.op_Implicit(xeno), (AudioParams?)null);
		((HandledEntityEventArgs)args).Handled = true;
	}
}
