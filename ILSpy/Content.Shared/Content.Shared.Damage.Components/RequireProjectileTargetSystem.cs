using System;
using Content.Shared.Projectiles;
using Content.Shared.Standing;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Damage.Components;

public sealed class RequireProjectileTargetSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RequireProjectileTargetComponent, PreventCollideEvent>((EntityEventRefHandler<RequireProjectileTargetComponent, PreventCollideEvent>)PreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequireProjectileTargetComponent, StoodEvent>((EntityEventRefHandler<RequireProjectileTargetComponent, StoodEvent>)StandingBulletHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequireProjectileTargetComponent, DownedEvent>((EntityEventRefHandler<RequireProjectileTargetComponent, DownedEvent>)LayingBulletPass, (Type[])null, (Type[])null);
	}

	private void PreventCollide(Entity<RequireProjectileTargetComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || !ent.Comp.Active)
		{
			return;
		}
		EntityUid other = args.OtherEntity;
		ProjectileComponent projectile = default(ProjectileComponent);
		if (!((EntitySystem)this).TryComp<ProjectileComponent>(other, ref projectile))
		{
			return;
		}
		EntityUid? val = ((EntitySystem)this).CompOrNull<TargetedProjectileComponent>(other)?.Target;
		EntityUid val2 = Entity<RequireProjectileTargetComponent>.op_Implicit(ent);
		if (!val.HasValue || val.GetValueOrDefault() != val2)
		{
			EntityUid? shooter = projectile.Shooter;
			if (shooter.HasValue && !((EntitySystem)this).TerminatingOrDeleted(shooter.Value, (MetaDataComponent)null) && !_container.IsEntityOrParentInContainer(shooter.Value, (MetaDataComponent)null, (TransformComponent)null))
			{
				args.Cancelled = true;
			}
		}
	}

	private void SetActive(Entity<RequireProjectileTargetComponent> ent, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Active != value)
		{
			ent.Comp.Active = value;
			((EntitySystem)this).Dirty<RequireProjectileTargetComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void StandingBulletHit(Entity<RequireProjectileTargetComponent> ent, ref StoodEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetActive(ent, value: false);
	}

	private void LayingBulletPass(Entity<RequireProjectileTargetComponent> ent, ref DownedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetActive(ent, value: true);
	}
}
