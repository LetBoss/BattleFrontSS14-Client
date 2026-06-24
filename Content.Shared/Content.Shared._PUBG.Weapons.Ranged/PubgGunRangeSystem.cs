using System;
using System.Numerics;
using Content.Shared._PUBG.Loadout;
using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared._PUBG.Weapons.Ranged;

public sealed class PubgGunRangeSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private PubgWeaponModulesSystem _weaponModules;

	private EntityQuery<ProjectileComponent> _projectileQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GunRangeModifierComponent, MapInitEvent>((EntityEventRefHandler<GunRangeModifierComponent, MapInitEvent>)OnGunRangeModifierMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunRangeModifierComponent, AmmoShotEvent>((EntityEventRefHandler<GunRangeModifierComponent, AmmoShotEvent>)OnGunRangeModifierAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponModulesComponent, GetGunRangeModifierEvent>((EntityEventRefHandler<PubgWeaponModulesComponent, GetGunRangeModifierEvent>)OnWeaponModulesGetRangeModifier, (Type[])null, (Type[])null);
	}

	private void OnGunRangeModifierMapInit(Entity<GunRangeModifierComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		RefreshGunRangeMultiplier(Entity<GunRangeModifierComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	private void OnWeaponModulesGetRangeModifier(Entity<PubgWeaponModulesComponent> ent, ref GetGunRangeModifierEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float multiplier = _weaponModules.GetRangeMultiplier(Entity<PubgWeaponModulesComponent>.op_Implicit(ent), ent.Comp);
		args.Multiplier *= multiplier;
	}

	private void OnGunRangeModifierAmmoShot(Entity<GunRangeModifierComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ModifiedMultiplier == 1.0)
		{
			return;
		}
		ProjectileMaxRangeComponent maxRange = default(ProjectileMaxRangeComponent);
		ProjectileFixedDistanceComponent fixedDistance = default(ProjectileFixedDistanceComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		RMCProjectileDamageFalloffComponent falloff = default(RMCProjectileDamageFalloffComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (((EntitySystem)this).TryComp<ProjectileMaxRangeComponent>(projectile, ref maxRange))
			{
				maxRange.Max *= ent.Comp.ModifiedMultiplier.Float();
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)maxRange, (MetaDataComponent)null);
			}
			if (((EntitySystem)this).TryComp<ProjectileFixedDistanceComponent>(projectile, ref fixedDistance))
			{
				MapCoordinates from = _transform.GetMapCoordinates(projectile, (TransformComponent)null);
				MapCoordinates? targetCoordinates = fixedDistance.TargetCoordinates;
				if (targetCoordinates.HasValue)
				{
					MapCoordinates target = targetCoordinates.GetValueOrDefault();
					if (from.MapId == target.MapId)
					{
						Vector2 direction = target.Position - from.Position;
						float currentDistance = direction.Length();
						float newDistance = currentDistance * ent.Comp.ModifiedMultiplier.Float();
						fixedDistance.TargetCoordinates = new MapCoordinates(from.Position + Vector2Helpers.Normalized(direction) * newDistance, from.MapId);
						if (_physicsQuery.TryComp(projectile, ref physics) && physics.LinearVelocity.Length() > 0f)
						{
							float speed = physics.LinearVelocity.Length();
							TimeSpan startTime = fixedDistance.FlyEndTime - TimeSpan.FromSeconds(currentDistance / speed);
							fixedDistance.FlyEndTime = startTime + TimeSpan.FromSeconds(newDistance / speed);
						}
						((EntitySystem)this).Dirty(projectile, (IComponent)(object)fixedDistance, (MetaDataComponent)null);
					}
				}
			}
			if (((EntitySystem)this).TryComp<RMCProjectileDamageFalloffComponent>(projectile, ref falloff))
			{
				float mult = ent.Comp.ModifiedMultiplier.Float();
				for (int i = 0; i < falloff.Thresholds.Count; i++)
				{
					DamageFalloffThreshold t = falloff.Thresholds[i];
					falloff.Thresholds[i] = t with
					{
						Range = t.Range * mult
					};
				}
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)falloff, (MetaDataComponent)null);
			}
		}
	}

	public void RefreshGunRangeMultiplier(Entity<GunRangeModifierComponent?> gun)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		gun.Comp = ((EntitySystem)this).EnsureComp<GunRangeModifierComponent>(Entity<GunRangeModifierComponent>.op_Implicit(gun));
		GetGunRangeModifierEvent ev = new GetGunRangeModifierEvent(gun.Comp.Multiplier);
		((EntitySystem)this).RaiseLocalEvent<GetGunRangeModifierEvent>(Entity<GunRangeModifierComponent>.op_Implicit(gun), ref ev, false);
		gun.Comp.ModifiedMultiplier = ev.Multiplier;
		((EntitySystem)this).Dirty<GunRangeModifierComponent>(gun, (MetaDataComponent)null);
	}
}
