using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared._RMC14.Weapons.Ranged.Homing;

public sealed class HomingProjectileSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedPhysicsSystem _physics;

	private readonly List<EntityUid> _toRemove = new List<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HomingProjectileComponent, StartCollideEvent>((EntityEventRefHandler<HomingProjectileComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HomingProjectileComponent, PreventCollideEvent>((EntityEventRefHandler<HomingProjectileComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnStartCollide(Entity<HomingProjectileComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.OtherEntity != ent.Comp.Target))
		{
			((EntitySystem)this).RemComp<HomingProjectileComponent>(Entity<HomingProjectileComponent>.op_Implicit(ent));
		}
	}

	private void OnPreventCollide(Entity<HomingProjectileComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.OtherEntity != ent.Comp.Target))
		{
			((EntitySystem)this).RemComp<HomingProjectileComponent>(Entity<HomingProjectileComponent>.op_Implicit(ent));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		_toRemove.Clear();
		EntityQueryEnumerator<HomingProjectileComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HomingProjectileComponent>();
		EntityUid projectile = default(EntityUid);
		HomingProjectileComponent component = default(HomingProjectileComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		ProjectileComponent projectileComponent = default(ProjectileComponent);
		while (query.MoveNext(ref projectile, ref component))
		{
			if (!((EntitySystem)this).TryComp<PhysicsComponent>(projectile, ref physics))
			{
				continue;
			}
			EntityUid target = component.Target;
			MapCoordinates targetCoords = _transform.GetMapCoordinates(target, ((EntitySystem)this).Transform(target));
			MapCoordinates projectileCoords = _transform.GetMapCoordinates(projectile, ((EntitySystem)this).Transform(projectile));
			if (targetCoords.MapId != projectileCoords.MapId)
			{
				_toRemove.Add(projectile);
				continue;
			}
			Vector2 direction = targetCoords.Position - projectileCoords.Position;
			if (_transform.InRange(((EntitySystem)this).Transform(projectile).Coordinates, ((EntitySystem)this).Transform(target).Coordinates, 1f))
			{
				_toRemove.Add(projectile);
				continue;
			}
			Vector2 targetMapVelocity = Vector2.Zero + Vector2Helpers.Normalized(direction) * component.ProjectileSpeed;
			Vector2 currentMapVelocity = _physics.GetMapLinearVelocity(projectile, physics, (TransformComponent)null);
			Vector2 newLinear = physics.LinearVelocity + targetMapVelocity - currentMapVelocity;
			_physics.SetLinearVelocity(projectile, newLinear, true, true, (FixturesComponent)null, physics);
			if (((EntitySystem)this).TryComp<ProjectileComponent>(projectile, ref projectileComponent))
			{
				_transform.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit(projectile), DirectionExtensions.ToWorldAngle(direction) + projectileComponent.Angle);
			}
		}
		try
		{
			foreach (EntityUid remove in _toRemove)
			{
				((EntitySystem)this).RemComp<HomingProjectileComponent>(remove);
			}
		}
		finally
		{
			_toRemove.Clear();
		}
	}
}
