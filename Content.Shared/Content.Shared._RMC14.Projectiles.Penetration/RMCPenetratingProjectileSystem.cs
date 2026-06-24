using System;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

namespace Content.Shared._RMC14.Projectiles.Penetration;

public sealed class RMCPenetratingProjectileSystem : EntitySystem
{
	private const int HardCollisionGroup = 10;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCSizeStunSystem _rmcSize;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCPenetratingProjectileComponent, MapInitEvent>((EntityEventRefHandler<RMCPenetratingProjectileComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPenetratingProjectileComponent, PreventCollideEvent>((EntityEventRefHandler<RMCPenetratingProjectileComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPenetratingProjectileComponent, StartCollideEvent>((EntityEventRefHandler<RMCPenetratingProjectileComponent, StartCollideEvent>)OnStartCollide, (Type[])null, new Type[1] { typeof(SharedProjectileSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCPenetratingProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCPenetratingProjectileComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPenetratingProjectileComponent, AfterProjectileHitEvent>((EntityEventRefHandler<RMCPenetratingProjectileComponent, AfterProjectileHitEvent>)OnAllowAdditionalHits, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<RMCPenetratingProjectileComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ShotFrom = _transform.GetMoverCoordinates(Entity<RMCPenetratingProjectileComponent>.op_Implicit(ent));
		((EntitySystem)this).Dirty<RMCPenetratingProjectileComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPreventCollide(Entity<RMCPenetratingProjectileComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.HitTargets.Contains(args.OtherEntity))
		{
			args.Cancelled = true;
		}
	}

	private void OnProjectileHit(Entity<RMCPenetratingProjectileComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.HitTargets.Contains(args.Target))
		{
			args.Handled = true;
			return;
		}
		ent.Comp.HitTargets.Add(args.Target);
		((EntitySystem)this).Dirty<RMCPenetratingProjectileComponent>(ent, (MetaDataComponent)null);
	}

	private void OnStartCollide(Entity<RMCPenetratingProjectileComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectile = default(ProjectileComponent);
		if (!((EntitySystem)this).TryComp<ProjectileComponent>(Entity<RMCPenetratingProjectileComponent>.op_Implicit(ent), ref projectile) || !ent.Comp.ShotFrom.HasValue)
		{
			return;
		}
		float rangeLoss = ent.Comp.RangeLossPerHit;
		float damageLoss = ent.Comp.DamageMultiplierLossPerHit;
		_rmcSize.TryGetSize(args.OtherEntity, out var size);
		if ((args.OtherFixture.CollisionLayer & 0xA) != 0)
		{
			OccluderComponent occluder = default(OccluderComponent);
			if (((EntitySystem)this).TryComp<OccluderComponent>(args.OtherEntity, ref occluder) && !occluder.Enabled)
			{
				rangeLoss *= ent.Comp.ThickMembraneMultiplier;
				damageLoss *= ent.Comp.ThickMembraneMultiplier;
				if (((EntitySystem)this).HasComp<XenoStructureUpgradeableComponent>(args.OtherEntity))
				{
					rangeLoss *= ent.Comp.MembraneMultiplier;
					damageLoss *= ent.Comp.MembraneMultiplier;
				}
			}
			else
			{
				rangeLoss *= ent.Comp.WallMultiplier;
				damageLoss *= ent.Comp.WallMultiplier;
			}
		}
		else if ((int)size >= 5)
		{
			rangeLoss *= ent.Comp.BigXenoMultiplier;
			damageLoss *= ent.Comp.BigXenoMultiplier;
		}
		ent.Comp.Range -= rangeLoss;
		((EntitySystem)this).Dirty<RMCPenetratingProjectileComponent>(ent, (MetaDataComponent)null);
		projectile.Damage *= 1f - damageLoss;
		((EntitySystem)this).Dirty(Entity<RMCPenetratingProjectileComponent>.op_Implicit(ent), (IComponent)(object)projectile, (MetaDataComponent)null);
	}

	private void OnAllowAdditionalHits(Entity<RMCPenetratingProjectileComponent> ent, ref AfterProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ShotFrom.HasValue)
		{
			float distanceTravelled = (_transform.GetMoverCoordinates(Entity<RMCPenetratingProjectileComponent>.op_Implicit(ent)).Position - ent.Comp.ShotFrom.Value.Position).Length();
			float num = ent.Comp.Range - distanceTravelled;
			ent.Comp.HitTargets.Add(args.Target);
			((EntitySystem)this).Dirty<RMCPenetratingProjectileComponent>(ent, (MetaDataComponent)null);
			if (!(num < 0f))
			{
				args.Projectile.Comp.ProjectileSpent = false;
				((EntitySystem)this).Dirty<ProjectileComponent>(args.Projectile, (MetaDataComponent)null);
			}
		}
	}
}
