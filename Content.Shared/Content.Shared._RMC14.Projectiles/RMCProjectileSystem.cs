using System;
using System.Numerics;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Projectiles;

public sealed class RMCProjectileSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private NpcFactionSystem _npcFaction;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DeleteOnCollideComponent, StartCollideEvent>((EntityEventRefHandler<DeleteOnCollideComponent, StartCollideEvent>)OnDeleteOnCollideStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ModifyTargetOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<ModifyTargetOnHitComponent, ProjectileHitEvent>)OnModifyTargetOnHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileMaxRangeComponent, MapInitEvent>((EntityEventRefHandler<ProjectileMaxRangeComponent, MapInitEvent>)OnProjectileMaxRangeMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCProjectileDamageFalloffComponent, MapInitEvent>((EntityEventRefHandler<RMCProjectileDamageFalloffComponent, MapInitEvent>)OnFalloffProjectileMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCProjectileDamageFalloffComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCProjectileDamageFalloffComponent, ProjectileHitEvent>)OnFalloffProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCProjectileAccuracyComponent, MapInitEvent>((EntityEventRefHandler<RMCProjectileAccuracyComponent, MapInitEvent>)OnProjectileAccuracyMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCProjectileAccuracyComponent, PreventCollideEvent>((EntityEventRefHandler<RMCProjectileAccuracyComponent, PreventCollideEvent>)OnProjectileAccuracyPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpawnOnTerminateComponent, MapInitEvent>((EntityEventRefHandler<SpawnOnTerminateComponent, MapInitEvent>)OnSpawnOnTerminatingMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpawnOnTerminateComponent, EntityTerminatingEvent>((EntityEventRefHandler<SpawnOnTerminateComponent, EntityTerminatingEvent>)OnSpawnOnTerminatingTerminate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PreventCollideWithDeadComponent, PreventCollideEvent>((EntityEventRefHandler<PreventCollideWithDeadComponent, PreventCollideEvent>)OnPreventCollideWithDead, (Type[])null, (Type[])null);
	}

	private void OnDeleteOnCollideStartCollide(Entity<DeleteOnCollideComponent> ent, ref StartCollideEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			((EntitySystem)this).QueueDel((EntityUid?)Entity<DeleteOnCollideComponent>.op_Implicit(ent));
		}
	}

	private void OnModifyTargetOnHit(Entity<ModifyTargetOnHitComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Target))
		{
			ComponentRegistry add = ent.Comp.Add;
			if (add != null)
			{
				base.EntityManager.AddComponents(args.Target, add, true);
			}
		}
	}

	private void OnProjectileMaxRangeMapInit(Entity<ProjectileMaxRangeComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Origin = _transform.GetMoverCoordinates(Entity<ProjectileMaxRangeComponent>.op_Implicit(ent));
		((EntitySystem)this).Dirty<ProjectileMaxRangeComponent>(ent, (MetaDataComponent)null);
	}

	private void OnFalloffProjectileMapInit(Entity<RMCProjectileDamageFalloffComponent> projectile, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		projectile.Comp.ShotFrom = _transform.GetMoverCoordinates(projectile.Owner);
		((EntitySystem)this).Dirty<RMCProjectileDamageFalloffComponent>(projectile, (MetaDataComponent)null);
	}

	private void OnFalloffProjectileHit(Entity<RMCProjectileDamageFalloffComponent> projectile, ref ProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!projectile.Comp.ShotFrom.HasValue || projectile.Comp.MinRemainingDamageMult < 0)
		{
			return;
		}
		float distance = (_transform.GetMoverCoordinates(args.Target).Position - projectile.Comp.ShotFrom.Value.Position).Length();
		FixedPoint2 minDamage = args.Damage.GetTotal() * projectile.Comp.MinRemainingDamageMult;
		foreach (DamageFalloffThreshold threshold in projectile.Comp.Thresholds)
		{
			float pastEffectiveRange = distance - threshold.Range;
			if (!(pastEffectiveRange <= 0f))
			{
				FixedPoint2 totalDamage = args.Damage.GetTotal();
				if (totalDamage <= minDamage)
				{
					break;
				}
				FixedPoint2 extraMult = (threshold.IgnoreModifiers ? ((FixedPoint2)1) : projectile.Comp.WeaponMult);
				FixedPoint2 minMult = FixedPoint2.Min(minDamage / totalDamage, 1);
				args.Damage *= FixedPoint2.Clamp((totalDamage - pastEffectiveRange * threshold.Falloff * extraMult) / totalDamage, minMult, 1);
			}
		}
	}

	public void SetProjectileFalloffWeaponMult(Entity<RMCProjectileDamageFalloffComponent> projectile, FixedPoint2 mult, float range)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		for (int count = 0; projectile.Comp.Thresholds.Count > count; count++)
		{
			DamageFalloffThreshold threshold = projectile.Comp.Thresholds[count];
			projectile.Comp.Thresholds[count] = threshold with
			{
				Range = threshold.Range + range
			};
		}
		projectile.Comp.WeaponMult = mult;
		((EntitySystem)this).Dirty<RMCProjectileDamageFalloffComponent>(projectile, (MetaDataComponent)null);
	}

	private void OnProjectileAccuracyMapInit(Entity<RMCProjectileAccuracyComponent> projectile, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		projectile.Comp.ShotFrom = _transform.GetMoverCoordinates(projectile.Owner);
		projectile.Comp.Tick = _timing.CurTick.Value;
		((EntitySystem)this).Dirty<RMCProjectileAccuracyComponent>(projectile, (MetaDataComponent)null);
	}

	private void OnProjectileAccuracyPreventCollide(Entity<RMCProjectileAccuracyComponent> projectile, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectileComponent = default(ProjectileComponent);
		EvasionComponent evasionComponent = default(EvasionComponent);
		if (args.Cancelled || projectile.Comp.ForceHit || !projectile.Comp.ShotFrom.HasValue || !((EntitySystem)this).TryComp<ProjectileComponent>(projectile.Owner, ref projectileComponent) || !((EntitySystem)this).TryComp<EvasionComponent>(args.OtherEntity, ref evasionComponent))
		{
			return;
		}
		FixedPoint2 accuracy = projectile.Comp.Accuracy;
		EntityCoordinates targetCoords = _transform.GetMoverCoordinates(args.OtherEntity);
		float distance = (targetCoords.Position - projectile.Comp.ShotFrom.Value.Position).Length();
		foreach (AccuracyFalloffThreshold threshold in projectile.Comp.Thresholds)
		{
			float pastRange = distance - threshold.Range;
			if (threshold.Buildup)
			{
				if (!(pastRange >= 0f))
				{
					accuracy += threshold.Falloff * pastRange;
				}
			}
			else if (!(pastRange <= 0f))
			{
				accuracy -= threshold.Falloff * pastRange;
			}
		}
		if (!_examine.InRangeUnOccluded(_transform.ToMapCoordinates(projectile.Comp.ShotFrom.Value, true), _transform.ToMapCoordinates(targetCoords, true), distance, null))
		{
			accuracy += (FixedPoint2)(-15);
		}
		if (!projectile.Comp.IgnoreFriendlyEvasion && IsProjectileTargetFriendly(projectile.Owner, args.OtherEntity))
		{
			accuracy -= evasionComponent.ModifiedEvasionFriendly;
		}
		accuracy -= evasionComponent.ModifiedEvasion;
		accuracy = ((accuracy > projectile.Comp.MinAccuracy) ? accuracy : projectile.Comp.MinAccuracy);
		float random = new Xoshiro128P(projectile.Comp.GunSeed, (long)(((ulong)projectile.Comp.Tick << 32) | (uint)((EntitySystem)this).GetNetEntity(args.OtherEntity, (MetaDataComponent)null).Id)).NextFloat(0f, 100f);
		if (!(accuracy >= random))
		{
			args.Cancelled = true;
		}
	}

	private bool IsProjectileTargetFriendly(EntityUid projectile, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectileComp = default(ProjectileComponent);
		if (!((EntitySystem)this).TryComp<ProjectileComponent>(projectile, ref projectileComp) || !projectileComp.Shooter.HasValue)
		{
			return false;
		}
		return _npcFaction.IsEntityFriendly(Entity<NpcFactionMemberComponent>.op_Implicit(projectileComp.Shooter.Value), Entity<NpcFactionMemberComponent>.op_Implicit(target));
	}

	private void OnSpawnOnTerminatingMapInit(Entity<SpawnOnTerminateComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Origin = _transform.GetMoverCoordinates(Entity<SpawnOnTerminateComponent>.op_Implicit(ent));
		((EntitySystem)this).Dirty<SpawnOnTerminateComponent>(ent, (MetaDataComponent)null);
	}

	private void OnSpawnOnTerminatingTerminate(Entity<SpawnOnTerminateComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp(Entity<SpawnOnTerminateComponent>.op_Implicit(ent), ref transform) || ((EntitySystem)this).TerminatingOrDeleted(transform.ParentUid, (MetaDataComponent)null))
		{
			return;
		}
		EntityCoordinates coordinates = transform.Coordinates;
		if (ent.Comp.ProjectileAdjust)
		{
			EntityCoordinates? origin = ent.Comp.Origin;
			if (origin.HasValue)
			{
				EntityCoordinates origin2 = origin.GetValueOrDefault();
				Vector2 delta = default(Vector2);
				if (((EntityCoordinates)(ref coordinates)).TryDelta((IEntityManager)(object)base.EntityManager, _transform, origin2, ref delta) && delta.Length() > 0f)
				{
					coordinates = ((EntityCoordinates)(ref coordinates)).Offset(Vector2Helpers.Normalized(delta) / -2f);
					if (((EntitySystem)this).HasComp<RMCFireProjectileComponent>(Entity<SpawnOnTerminateComponent>.op_Implicit(ent)))
					{
						coordinates = ((EntityCoordinates)(ref coordinates)).Offset(Vector2Helpers.Normalized(delta));
					}
				}
			}
		}
		EntityUid spawn = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(ent.Comp.Spawn), coordinates, (ComponentRegistry)null);
		_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(spawn));
		LocId? popup = ent.Comp.Popup;
		if (popup.HasValue)
		{
			LocId popup2 = popup.GetValueOrDefault();
			_popup.PopupCoordinates(base.Loc.GetString(LocId.op_Implicit(popup2)), coordinates, ent.Comp.PopupType.GetValueOrDefault());
		}
	}

	private void OnPreventCollideWithDead(Entity<PreventCollideWithDeadComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && _mobState.IsDead(args.OtherEntity))
		{
			args.Cancelled = true;
		}
	}

	public void SetMaxRange(Entity<ProjectileMaxRangeComponent> ent, float max)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Max = max;
		((EntitySystem)this).Dirty<ProjectileMaxRangeComponent>(ent, (MetaDataComponent)null);
	}

	private void StopProjectile(Entity<ProjectileMaxRangeComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Delete)
		{
			if (_net.IsServer || ((EntitySystem)this).IsClientSide(Entity<ProjectileMaxRangeComponent>.op_Implicit(ent), (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<ProjectileMaxRangeComponent>.op_Implicit(ent));
			}
		}
		else
		{
			_physics.SetLinearVelocity(Entity<ProjectileMaxRangeComponent>.op_Implicit(ent), Vector2.Zero, true, true, (FixturesComponent)null, (PhysicsComponent)null);
			((EntitySystem)this).RemCompDeferred<ProjectileMaxRangeComponent>(Entity<ProjectileMaxRangeComponent>.op_Implicit(ent));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<ProjectileMaxRangeComponent> maxQuery = ((EntitySystem)this).EntityQueryEnumerator<ProjectileMaxRangeComponent>();
		EntityUid uid = default(EntityUid);
		ProjectileMaxRangeComponent comp = default(ProjectileMaxRangeComponent);
		float distance = default(float);
		while (maxQuery.MoveNext(ref uid, ref comp))
		{
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(uid);
			EntityCoordinates? origin = comp.Origin;
			if (origin.HasValue)
			{
				EntityCoordinates origin2 = origin.GetValueOrDefault();
				if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, origin2, ref distance))
				{
					if (!(distance < comp.Max) || !(Math.Abs(distance - comp.Max) > 0.1f))
					{
						StopProjectile(Entity<ProjectileMaxRangeComponent>.op_Implicit((uid, comp)));
					}
					continue;
				}
			}
			StopProjectile(Entity<ProjectileMaxRangeComponent>.op_Implicit((uid, comp)));
		}
	}
}
