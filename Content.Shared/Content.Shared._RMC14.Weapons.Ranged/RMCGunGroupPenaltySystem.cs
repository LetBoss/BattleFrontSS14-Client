using System;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCGunGroupPenaltySystem : EntitySystem
{
	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private CMGunSystem _rmcGun;

	private EntityQuery<GunGroupPenaltyComponent> _gunGroupPenalty;

	private EntityQuery<ProjectileComponent> _projectileQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_gunGroupPenalty = ((EntitySystem)this).GetEntityQuery<GunGroupPenaltyComponent>();
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GunGroupPenaltyComponent, GotEquippedHandEvent>((EntityEventRefHandler<GunGroupPenaltyComponent, GotEquippedHandEvent>)OnGroupSpreadPenaltyEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunGroupPenaltyComponent, GotUnequippedHandEvent>((EntityEventRefHandler<GunGroupPenaltyComponent, GotUnequippedHandEvent>)OnGroupSpreadPenaltyUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunGroupPenaltyComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunGroupPenaltyComponent, GunRefreshModifiersEvent>)OnGroupSpreadPenaltyRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunGroupPenaltyComponent, AmmoShotEvent>((EntityEventRefHandler<GunGroupPenaltyComponent, AmmoShotEvent>)OnGroupSpreadPenaltyAmmoShot, new Type[1] { typeof(CMGunSystem) }, (Type[])null);
	}

	private void OnGroupSpreadPenaltyEquippedHand(Entity<GunGroupPenaltyComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshGunHolderModifiers(ent);
	}

	private void OnGroupSpreadPenaltyUnequippedHand(Entity<GunGroupPenaltyComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshGunHolderModifiers(ent);
	}

	private void OnGroupSpreadPenaltyRefreshModifiers(Entity<GunGroupPenaltyComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (!_rmcGun.TryGetGunUser(Entity<GunGroupPenaltyComponent>.op_Implicit(ent), out Entity<HandsComponent> user))
		{
			return;
		}
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(user)))))
		{
			if (!(held == ent.Owner) && _gunGroupPenalty.HasComp(held))
			{
				args.CameraRecoilScalar += ent.Comp.Recoil;
				args.AngleIncrease += ent.Comp.AngleIncrease;
				args.MinAngle += Angle.op_Implicit(Angle.op_Implicit(ent.Comp.AngleIncrease) / 2.0);
				args.MaxAngle += ent.Comp.AngleIncrease;
				break;
			}
		}
	}

	private void OnGroupSpreadPenaltyAmmoShot(Entity<GunGroupPenaltyComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (!_rmcGun.TryGetGunUser(Entity<GunGroupPenaltyComponent>.op_Implicit(ent), out Entity<HandsComponent> user))
		{
			return;
		}
		bool other = false;
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(user)))))
		{
			if (held != ent.Owner && _gunGroupPenalty.HasComp(held))
			{
				other = true;
				break;
			}
		}
		if (!other)
		{
			return;
		}
		ProjectileComponent projectileComp = default(ProjectileComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (_projectileQuery.TryComp(projectile, ref projectileComp))
			{
				projectileComp.Damage *= ent.Comp.DamageMultiplier;
			}
		}
	}

	private void RefreshGunHolderModifiers(Entity<GunGroupPenaltyComponent> gun)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(gun.Owner));
		if (!_rmcGun.TryGetGunUser(Entity<GunGroupPenaltyComponent>.op_Implicit(gun), out Entity<HandsComponent> user))
		{
			return;
		}
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(user)))))
		{
			if (held != gun.Owner)
			{
				_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(held));
			}
		}
	}
}
