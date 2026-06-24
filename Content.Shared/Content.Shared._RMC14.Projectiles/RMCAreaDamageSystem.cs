using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Projectiles;

public sealed class RMCAreaDamageSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCAreaDamageComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCAreaDamageComponent, ProjectileHitEvent>)OnAreaDamageProjectileHit, (Type[])null, (Type[])null);
	}

	private void OnAreaDamageProjectileHit(Entity<RMCAreaDamageComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		BeforeAreaDamageEvent ev = new BeforeAreaDamageEvent(args.Target, args.Damage);
		((EntitySystem)this).RaiseLocalEvent<BeforeAreaDamageEvent>(Entity<RMCAreaDamageComponent>.op_Implicit(ent), ref ev, false);
		if (!ev.Cancelled)
		{
			ApplyAreaDamage(Entity<RMCAreaDamageComponent>.op_Implicit(ent), args.Target, args.Damage, args.Shooter);
		}
	}

	private void ApplyAreaDamage(EntityUid uid, EntityUid target, DamageSpecifier damage, EntityUid? shooter = null, RMCAreaDamageComponent? areaDamage = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (!((EntitySystem)this).Resolve<RMCAreaDamageComponent>(uid, ref areaDamage, true) || areaDamage.DamageArea == 0f || !((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState))
		{
			return;
		}
		CMArmorPiercingComponent piercing = default(CMArmorPiercingComponent);
		foreach (Entity<MobStateComponent> entity in _entityLookup.GetEntitiesInRange<MobStateComponent>(((EntitySystem)this).Transform(target).Coordinates, areaDamage.DamageArea, (LookupFlags)110))
		{
			if (entity.Owner == target)
			{
				continue;
			}
			EntityUid val = Entity<MobStateComponent>.op_Implicit(entity);
			EntityUid? val2 = shooter;
			if (val2.HasValue && val == val2.GetValueOrDefault())
			{
				continue;
			}
			MapCoordinates fromCoords = _transform.GetMapCoordinates(target, (TransformComponent)null);
			Vector2 distance = _transform.GetMapCoordinates(Entity<MobStateComponent>.op_Implicit(entity), (TransformComponent)null).Position - fromCoords.Position;
			DamageSpecifier newDamage = damage;
			int armorPiercing = 0;
			if (areaDamage.FalloffDistance / distance.Length() < 1f)
			{
				newDamage *= areaDamage.FalloffDistance / distance.Length();
			}
			_sizeStun.TryGetSize(Entity<MobStateComponent>.op_Implicit(entity), out var size);
			if (((EntitySystem)this).TryComp<CMArmorPiercingComponent>(uid, ref piercing))
			{
				armorPiercing = piercing.Amount;
			}
			if ((int)size >= 3)
			{
				newDamage *= 2f;
			}
			DamageableSystem damage2 = _damage;
			EntityUid? uid2 = Entity<MobStateComponent>.op_Implicit(entity);
			DamageSpecifier damage3 = newDamage;
			int armorPiercing2 = armorPiercing;
			FixedPoint2? fixedPoint = damage2.TryChangeDamage(uid2, damage3, ignoreResistances: false, interruptsDoAfters: true, null, null, null, armorPiercing2)?.GetTotal();
			FixedPoint2 zero = FixedPoint2.Zero;
			if (fixedPoint.HasValue && fixedPoint.GetValueOrDefault() > zero && _net.IsClient)
			{
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(entity), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid hit) => hit == entity.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(entity) }, filter);
			}
		}
	}
}
