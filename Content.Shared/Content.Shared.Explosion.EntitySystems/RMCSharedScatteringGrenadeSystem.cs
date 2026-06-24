using System;
using Content.Shared.Explosion.Components;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;

namespace Content.Shared.Explosion.EntitySystems;

public sealed class RMCSharedScatteringGrenadeSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private IRobustRandom _random;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ScatteringGrenadeComponent, StartCollideEvent>((EntityEventRefHandler<ScatteringGrenadeComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScatteringGrenadeComponent, ProjectileHitEvent>((EntityEventRefHandler<ScatteringGrenadeComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ScatteringGrenadeComponent, ScatterGrenadeContentsEvent>((EntityEventRefHandler<ScatteringGrenadeComponent, ScatterGrenadeContentsEvent>)OnScatterGrenadeContents, (Type[])null, (Type[])null);
	}

	private void OnScatterGrenadeContents(Entity<ScatteringGrenadeComponent> ent, ref ScatterGrenadeContentsEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		(EntityCoordinates, Angle) moverCoordinateRotation = _transformSystem.GetMoverCoordinateRotation(ent.Owner, ((EntitySystem)this).Transform(ent.Owner));
		double num = ((Angle)(ref moverCoordinateRotation.Item2)).Degrees + (double)ent.Comp.DirectionAngle;
		float spreadAngle = ent.Comp.SpreadAngle / (float)args.TotalCount;
		double angleMin = num - (double)(ent.Comp.SpreadAngle / 2f) + (double)(spreadAngle * (float)args.ThrownCount);
		double angleMax = num - (double)(ent.Comp.SpreadAngle / 2f) + (double)(spreadAngle * (float)(args.ThrownCount + 1));
		args.Angle = Angle.FromDegrees((double)_random.Next((int)angleMin, (int)angleMax));
		args.Handled = true;
	}

	private void OnStartCollide(Entity<ScatteringGrenadeComponent> ent, ref StartCollideEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((args.OtherFixture.CollisionLayer & 0xA) != 0 && ent.Comp.TriggerOnWallCollide)
		{
			ent.Comp.IsTriggered = true;
			((EntitySystem)this).Dirty<ScatteringGrenadeComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnProjectileHit(Entity<ScatteringGrenadeComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DirectHitTrigger)
		{
			ent.Comp.DirectionAngle += ent.Comp.ReboundAngle;
			ent.Comp.IsTriggered = true;
			((EntitySystem)this).Dirty<ScatteringGrenadeComponent>(ent, (MetaDataComponent)null);
		}
	}
}
