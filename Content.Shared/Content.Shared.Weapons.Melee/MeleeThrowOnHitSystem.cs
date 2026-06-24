using System;
using System.Numerics;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Weapons.Melee;

public sealed class MeleeThrowOnHitSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private UseDelaySystem _delay;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private ThrowingSystem _throwing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MeleeThrowOnHitComponent, MeleeHitEvent>((EntityEventRefHandler<MeleeThrowOnHitComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeThrowOnHitComponent, ThrowDoHitEvent>((EntityEventRefHandler<MeleeThrowOnHitComponent, ThrowDoHitEvent>)OnThrowHit, (Type[])null, (Type[])null);
	}

	private void OnMeleeHit(Entity<MeleeThrowOnHitComponent> weapon, ref MeleeHitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || _delay.IsDelayed(Entity<UseDelayComponent>.op_Implicit(weapon.Owner)) || args.HitEntities.Count == 0)
		{
			return;
		}
		Vector2 userPos = _transform.GetWorldPosition(args.User);
		foreach (EntityUid target in args.HitEntities)
		{
			Vector2 targetPos = _transform.GetMapCoordinates(target, (TransformComponent)null).Position;
			Vector2 direction = args.Direction ?? (targetPos - userPos);
			ThrowOnHitHelper(weapon, args.User, target, direction);
		}
	}

	private void OnThrowHit(Entity<MeleeThrowOnHitComponent> weapon, ref ThrowDoHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent weaponPhysics = default(PhysicsComponent);
		if (weapon.Comp.ActivateOnThrown && ((EntitySystem)this).TryComp<PhysicsComponent>(args.Thrown, ref weaponPhysics))
		{
			ThrowOnHitHelper(weapon, args.Component.Thrower, args.Target, weaponPhysics.LinearVelocity);
		}
	}

	private void ThrowOnHitHelper(Entity<MeleeThrowOnHitComponent> ent, EntityUid? user, EntityUid target, Vector2 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		AttemptMeleeThrowOnHitEvent attemptEvent = new AttemptMeleeThrowOnHitEvent(target, user);
		((EntitySystem)this).RaiseLocalEvent<AttemptMeleeThrowOnHitEvent>(ent.Owner, ref attemptEvent, false);
		if (!attemptEvent.Cancelled)
		{
			MeleeThrowOnHitStartEvent startEvent = new MeleeThrowOnHitStartEvent(ent.Owner, user);
			((EntitySystem)this).RaiseLocalEvent<MeleeThrowOnHitStartEvent>(target, ref startEvent, false);
			if (ent.Comp.StunTime.HasValue)
			{
				_stun.TryParalyze(target, ent.Comp.StunTime.Value, refresh: false);
			}
			if (!(direction == Vector2.Zero))
			{
				ThrowingSystem throwing = _throwing;
				Vector2 direction2 = Vector2Helpers.Normalized(direction) * ent.Comp.Distance;
				float speed = ent.Comp.Speed;
				bool unanchorOnHit = ent.Comp.UnanchorOnHit;
				throwing.TryThrow(target, direction2, speed, user, 2f, null, compensateFriction: false, recoil: true, animated: true, playSound: true, doSpin: true, unanchorOnHit);
			}
		}
	}
}
