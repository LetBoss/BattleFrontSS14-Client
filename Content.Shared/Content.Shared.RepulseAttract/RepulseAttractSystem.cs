using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Physics;
using Content.Shared.RepulseAttract.Events;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared.RepulseAttract;

public sealed class RepulseAttractSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private ThrowingSystem _throw;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedTransformSystem _xForm;

	[Dependency]
	private UseDelaySystem _delay;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private HashSet<EntityUid> _entSet = new HashSet<EntityUid>();

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RepulseAttractComponent, MeleeHitEvent>((EntityEventRefHandler<RepulseAttractComponent, MeleeHitEvent>)OnMeleeAttempt, new Type[1] { typeof(UseDelayOnMeleeHitSystem) }, new Type[1] { typeof(SharedWieldableSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RepulseAttractComponent, RepulseAttractActionEvent>((EntityEventRefHandler<RepulseAttractComponent, RepulseAttractActionEvent>)OnRepulseAttractAction, (Type[])null, (Type[])null);
	}

	private void OnMeleeAttempt(Entity<RepulseAttractComponent> ent, ref MeleeHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!_delay.IsDelayed(Entity<UseDelayComponent>.op_Implicit(ent.Owner)))
		{
			TryRepulseAttract(ent, args.User);
		}
	}

	private void OnRepulseAttractAction(Entity<RepulseAttractComponent> ent, ref RepulseAttractActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			MapCoordinates position = _xForm.GetMapCoordinates(args.Performer, (TransformComponent)null);
			((HandledEntityEventArgs)args).Handled = TryRepulseAttract(position, args.Performer, ent.Comp.Speed, ent.Comp.Range, ent.Comp.Whitelist, ent.Comp.CollisionMask);
		}
	}

	public bool TryRepulseAttract(Entity<RepulseAttractComponent> ent, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates position = _xForm.GetMapCoordinates(ent.Owner, (TransformComponent)null);
		return TryRepulseAttract(position, user, ent.Comp.Speed, ent.Comp.Range, ent.Comp.Whitelist, ent.Comp.CollisionMask);
	}

	public bool TryRepulseAttract(MapCoordinates position, EntityUid? user, float speed, float range, EntityWhitelist? whitelist = null, CollisionGroup layer = CollisionGroup.SingularityLayer)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		_entSet.Clear();
		Vector2 epicenter = position.Position;
		_lookup.GetEntitiesInRange(position.MapId, epicenter, range, _entSet, (LookupFlags)10);
		PhysicsComponent physics = default(PhysicsComponent);
		foreach (EntityUid target in _entSet)
		{
			if (_physicsQuery.TryGetComponent(target, ref physics) && ((uint)physics.CollisionLayer & (uint)layer) == 0 && !_whitelist.IsWhitelistFail(whitelist, target))
			{
				Vector2 direction = _xForm.GetWorldPosition(target) - epicenter;
				if (!(direction == Vector2.Zero))
				{
					Vector2 throwDirection = ((speed < 0f) ? (-direction) : (Vector2Helpers.Normalized(direction) * (range - direction.Length())));
					_throw.TryThrow(target, throwDirection, Math.Abs(speed), user, 2f, null, compensateFriction: true, recoil: false);
				}
			}
		}
		return true;
	}
}
