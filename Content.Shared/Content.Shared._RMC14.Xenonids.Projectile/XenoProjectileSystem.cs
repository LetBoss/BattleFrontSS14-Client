using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Light;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Xenonids.Projectile;

public sealed class XenoProjectileSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedGunPredictionSystem _gunPrediction;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedProjectileSystem _projectile;

	[Dependency]
	private SharedRMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private CMPoweredLightSystem _rmcPoweredLight;

	[Dependency]
	private RMCPseudoRandomSystem _rmcPseudoRandom;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	private EntityQuery<ProjectileComponent> _projectileQuery;

	private EntityQuery<PreventAttackLightOffComponent> _preventAttackLightOffQuery;

	private int _limitHitsId;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		_preventAttackLightOffQuery = ((EntitySystem)this).GetEntityQuery<PreventAttackLightOffComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<XenoProjectilePredictedHitEvent>((EntitySessionEventHandler<XenoProjectilePredictedHitEvent>)OnPredictedHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShooterComponent, ComponentRemove>((EntityEventRefHandler<XenoProjectileShooterComponent, ComponentRemove>)OnShooterRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShooterComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoProjectileShooterComponent, EntityTerminatingEvent>)OnShooterRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShotComponent, ComponentRemove>((EntityEventRefHandler<XenoProjectileShotComponent, ComponentRemove>)OnShotRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShotComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoProjectileShotComponent, EntityTerminatingEvent>)OnShotRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoClientProjectileShotComponent, StartCollideEvent>((EntityEventRefHandler<XenoClientProjectileShotComponent, StartCollideEvent>)OnShotCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileComponent, PreventCollideEvent>((EntityEventRefHandler<XenoProjectileComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<XenoProjectileComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileComponent, CMClusterSpawnedEvent>((EntityEventRefHandler<XenoProjectileComponent, CMClusterSpawnedEvent>)OnClusterSpawned, (Type[])null, (Type[])null);
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_limitHitsId = 0;
	}

	private void OnPredictedHit(XenoProjectilePredictedHitEvent msg, EntitySessionEventArgs args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !_gunPrediction.GunPrediction)
		{
			return;
		}
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid ent = attachedEntity.GetValueOrDefault();
		EntityUid target = ((EntitySystem)this).GetEntity(msg.Target);
		XenoProjectileShooterComponent shooter = default(XenoProjectileShooterComponent);
		EntityUid? shot = default(EntityUid?);
		if (((EntityUid)(ref target)).Valid && ((EntitySystem)this).TryComp<XenoProjectileShooterComponent>(ent, ref shooter) && shooter.Shot.Count != 0 && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)shooter.Shot, (Func<EntityUid, bool>)((EntityUid e) => ((EntitySystem)this).CompOrNull<XenoProjectileShotComponent>(e)?.Id == msg.Id), ref shot) && !((EntitySystem)this).TerminatingOrDeleted(shot, (MetaDataComponent)null))
		{
			_rmcLagCompensation.SetLastRealTick(((EntitySessionEventArgs)(ref args)).SenderSession.UserId, msg.LastRealTick);
			MapCoordinates coordinates = _transform.ToMapCoordinates(_rmcLagCompensation.GetCoordinates(target, ((EntitySessionEventArgs)(ref args)).SenderSession), true);
			ProjectileComponent projectile = default(ProjectileComponent);
			PhysicsComponent physics = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<ProjectileComponent>(shot, ref projectile) && ((EntitySystem)this).TryComp<PhysicsComponent>(shot, ref physics) && _rmcLagCompensation.Collides(Entity<FixturesComponent>.op_Implicit(target), Entity<PhysicsComponent>.op_Implicit((shot.Value, physics)), coordinates))
			{
				_projectile.ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((shot.Value, projectile, physics)), target, predicted: true);
			}
		}
	}

	private void OnShooterRemove<T>(Entity<XenoProjectileShooterComponent> ent, ref T args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState)
		{
			return;
		}
		foreach (EntityUid shot in ent.Comp.Shot)
		{
			((EntitySystem)this).RemCompDeferred<XenoProjectileShotComponent>(shot);
		}
		ent.Comp.Shot.Clear();
		((EntitySystem)this).Dirty<XenoProjectileShooterComponent>(ent, (MetaDataComponent)null);
	}

	private void OnShotRemove<T>(Entity<XenoProjectileShotComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? shooterEnt = ent.Comp.ShooterEnt;
		if (shooterEnt.HasValue)
		{
			EntityUid shooter = shooterEnt.GetValueOrDefault();
			XenoProjectileShooterComponent shooterComp = default(XenoProjectileShooterComponent);
			if (((EntitySystem)this).TryComp<XenoProjectileShooterComponent>(shooter, ref shooterComp) && shooterComp.Shot.Remove(Entity<XenoProjectileShotComponent>.op_Implicit(ent)))
			{
				((EntitySystem)this).Dirty(shooter, (IComponent)(object)shooterComp, (MetaDataComponent)null);
			}
		}
	}

	private void OnShotCollide(Entity<XenoClientProjectileShotComponent> ent, ref StartCollideEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		XenoProjectileShotComponent shot = default(XenoProjectileShotComponent);
		if (!_net.IsServer && ((EntitySystem)this).IsClientSide(Entity<XenoClientProjectileShotComponent>.op_Implicit(ent), (MetaDataComponent)null) && ((EntitySystem)this).TryComp<XenoProjectileShotComponent>(Entity<XenoClientProjectileShotComponent>.op_Implicit(ent), ref shot))
		{
			XenoProjectilePredictedHitEvent ev = new XenoProjectilePredictedHitEvent(shot.Id, ((EntitySystem)this).GetNetEntity(args.OtherEntity, (MetaDataComponent)null), _rmcLagCompensation.GetLastRealTick(null));
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev);
		}
	}

	private void OnPreventCollide(Entity<XenoProjectileComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			if (_preventAttackLightOffQuery.HasComp(args.OtherEntity) && _rmcPoweredLight.IsOff(args.OtherEntity))
			{
				args.Cancelled = true;
			}
			else if (!ent.Comp.DeleteOnFriendlyXeno && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(args.OtherEntity)) && (((EntitySystem)this).HasComp<XenoComponent>(args.OtherEntity) || ((EntitySystem)this).HasComp<HiveCoreComponent>(args.OtherEntity)))
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnProjectileHit(Entity<XenoProjectileComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(args.Target)))
		{
			args.Handled = true;
			if (_net.IsServer || ((EntitySystem)this).IsClientSide(Entity<XenoProjectileComponent>.op_Implicit(ent), (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<XenoProjectileComponent>.op_Implicit(ent));
			}
			return;
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Target))
		{
			args.Damage = _xeno.TryApplyXenoProjectileDamageMultiplier(args.Target, args.Damage);
		}
		ProjectileComponent projectile = default(ProjectileComponent);
		if (_projectileQuery.TryComp(Entity<XenoProjectileComponent>.op_Implicit(ent), ref projectile))
		{
			EntityUid? shooter = projectile.Shooter;
			if (shooter.HasValue)
			{
				EntityUid shooter2 = shooter.GetValueOrDefault();
				XenoProjectileHitUserEvent ev = new XenoProjectileHitUserEvent(args.Target);
				((EntitySystem)this).RaiseLocalEvent<XenoProjectileHitUserEvent>(shooter2, ref ev, false);
			}
		}
	}

	private void OnClusterSpawned(Entity<XenoProjectileComponent> ent, ref CMClusterSpawnedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
		if (!hive.HasValue)
		{
			return;
		}
		Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
		foreach (EntityUid spawned in args.Spawned)
		{
			_hive.SetHive(Entity<HiveMemberComponent>.op_Implicit(spawned), Entity<HiveComponent>.op_Implicit(hive2));
		}
	}

	public bool TryShoot(EntityUid xeno, EntityCoordinates targetCoords, FixedPoint2 plasma, EntProtoId projectileId, SoundSpecifier? sound, int shots, Angle deviation, float speed, float? stopAtDistance = null, EntityUid? target = null, bool predicted = true, int? projectileHitLimit = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		if (!predicted && _net.IsClient)
		{
			return false;
		}
		MapCoordinates origin = _transform.GetMapCoordinates(xeno, (TransformComponent)null);
		MapCoordinates targetMap = _transform.ToMapCoordinates(targetCoords, true);
		if (origin.MapId != targetMap.MapId || origin.Position == targetMap.Position)
		{
			return false;
		}
		if (!_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno), plasma))
		{
			return false;
		}
		_audio.PlayPredicted(sound, xeno, (EntityUid?)xeno, (AudioParams?)null);
		if ((_net.IsClient && !_gunPrediction.GunPrediction) || !_timing.IsFirstTimePredicted)
		{
			return true;
		}
		AmmoShotEvent ammoShotEvent = new AmmoShotEvent
		{
			FiredProjectiles = new List<EntityUid>(shots)
		};
		if (target.HasValue && ((EntitySystem)this).HasComp<MobStateComponent>(target) && !_xeno.CanAbilityAttackTarget(xeno, target.Value))
		{
			target = null;
		}
		XenoProjectileShooterComponent shooter = null;
		ActorComponent obj = ((EntitySystem)this).CompOrNull<ActorComponent>(xeno);
		ICommonSession shooterPlayer = ((obj != null) ? obj.PlayerSession : null);
		Xoroshiro64S xoroshiro = _rmcPseudoRandom.GetXoroshiro64S(xeno);
		Vector2 originalDiff = targetMap.Position - origin.Position;
		double halfDeviation = Angle.op_Implicit(deviation) / 2.0;
		if (projectileHitLimit.HasValue)
		{
			_limitHitsId++;
		}
		for (int i = 0; i < shots; i++)
		{
			Angle angleOffset = Angle.Zero;
			if (i > 0 && deviation != Angle.Zero)
			{
				angleOffset = _rmcPseudoRandom.NextAngle(ref xoroshiro, Angle.op_Implicit(0.0 - halfDeviation), Angle.op_Implicit(halfDeviation));
			}
			Vector2 diff = new MapCoordinates(origin.Position + ((Angle)(ref angleOffset)).RotateVec(ref originalDiff), targetMap.MapId).Position - origin.Position;
			EntityUid projectile = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(projectileId), origin, (ComponentRegistry)null, default(Angle));
			diff *= speed / diff.Length();
			_gun.ShootProjectile(projectile, diff, Vector2.Zero, xeno, xeno, speed);
			ammoShotEvent.FiredProjectiles.Add(projectile);
			((EntitySystem)this).EnsureComp<XenoProjectileComponent>(projectile);
			_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveMemberComponent>.op_Implicit(projectile));
			if (stopAtDistance.HasValue)
			{
				ProjectileFixedDistanceComponent fixedDistanceComp = ((EntitySystem)this).EnsureComp<ProjectileFixedDistanceComponent>(projectile);
				fixedDistanceComp.FlyEndTime = _timing.CurTime + TimeSpan.FromSeconds(stopAtDistance.Value / speed);
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)fixedDistanceComp, (MetaDataComponent)null);
			}
			if (target.HasValue)
			{
				TargetedProjectileComponent targeted = ((EntitySystem)this).EnsureComp<TargetedProjectileComponent>(projectile);
				targeted.Target = target.Value;
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)targeted, (MetaDataComponent)null);
			}
			if (projectileHitLimit.HasValue)
			{
				ProjectileLimitHitsComponent limitHits = ((EntitySystem)this).EnsureComp<ProjectileLimitHitsComponent>(projectile);
				limitHits.Limit = projectileHitLimit.Value;
				limitHits.OriginEntity = xeno;
				limitHits.ExtraId = _limitHitsId;
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)limitHits, (MetaDataComponent)null);
			}
			if (predicted)
			{
				if (shooter == null)
				{
					shooter = ((EntitySystem)this).EnsureComp<XenoProjectileShooterComponent>(xeno);
				}
				shooter.Shot.Add(projectile);
				((EntitySystem)this).Dirty(xeno, (IComponent)(object)shooter, (MetaDataComponent)null);
				XenoProjectileShotComponent shot = ((EntitySystem)this).EnsureComp<XenoProjectileShotComponent>(projectile);
				shot.Id = shooter.NextId++;
				shot.Shooter = shooterPlayer;
				shot.ShooterEnt = xeno;
				((EntitySystem)this).Dirty(projectile, (IComponent)(object)shot, (MetaDataComponent)null);
			}
			if (!_net.IsServer)
			{
				((EntitySystem)this).EnsureComp<XenoClientProjectileShotComponent>(projectile);
				_physics.UpdateIsPredicted((EntityUid?)projectile, (PhysicsComponent)null);
			}
		}
		((EntitySystem)this).RaiseLocalEvent<AmmoShotEvent>(xeno, ammoShotEvent, false);
		return true;
	}
}
