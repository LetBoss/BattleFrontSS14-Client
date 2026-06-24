using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Projectiles.Penetration;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids.Damage;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
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
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Projectiles;

public abstract class SharedProjectileSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class RemoveEmbeddedProjectileEvent : DoAfterEvent, ISerializationGenerated<RemoveEmbeddedProjectileEvent>, ISerializationGenerated
	{
		public override DoAfterEvent Clone()
		{
			return this;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref RemoveEmbeddedProjectileEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (RemoveEmbeddedProjectileEvent)definitionCast;
			serialization.TryCustomCopy<RemoveEmbeddedProjectileEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref RemoveEmbeddedProjectileEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			RemoveEmbeddedProjectileEvent cast = (RemoveEmbeddedProjectileEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			RemoveEmbeddedProjectileEvent cast = (RemoveEmbeddedProjectileEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override RemoveEmbeddedProjectileEvent Instantiate()
		{
			return new RemoveEmbeddedProjectileEvent();
		}
	}

	public const string ProjectileFixture = "projectile";

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private SharedGunSystem _guns;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ProjectileComponent, StartCollideEvent>((ComponentEventRefHandler<ProjectileComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileComponent, PreventCollideEvent>((ComponentEventRefHandler<ProjectileComponent, PreventCollideEvent>)PreventCollision, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmbeddableProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<EmbeddableProjectileComponent, ProjectileHitEvent>)OnEmbedProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmbeddableProjectileComponent, ThrowDoHitEvent>((EntityEventRefHandler<EmbeddableProjectileComponent, ThrowDoHitEvent>)OnEmbedThrowDoHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmbeddableProjectileComponent, ActivateInWorldEvent>((EntityEventRefHandler<EmbeddableProjectileComponent, ActivateInWorldEvent>)OnEmbedActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmbeddableProjectileComponent, RemoveEmbeddedProjectileEvent>((EntityEventRefHandler<EmbeddableProjectileComponent, RemoveEmbeddedProjectileEvent>)OnEmbedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmbeddableProjectileComponent, ComponentShutdown>((EntityEventRefHandler<EmbeddableProjectileComponent, ComponentShutdown>)OnEmbeddableCompShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmbeddedContainerComponent, EntityTerminatingEvent>((EntityEventRefHandler<EmbeddedContainerComponent, EntityTerminatingEvent>)OnEmbeddableTermination, (Type[])null, (Type[])null);
	}

	private void OnStartCollide(EntityUid uid, ProjectileComponent component, ref StartCollideEvent args)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (args.OurFixtureId != "projectile" || !args.OtherFixture.Hard || component.ProjectileSpent)
		{
			return;
		}
		if (component != null)
		{
			EntityUid? weapon = component.Weapon;
			if (!weapon.HasValue && component.OnlyCollideWhenShot)
			{
				return;
			}
		}
		bool predicted = _net.IsServer && _guns.GunPrediction && ((EntitySystem)this).HasComp<PredictedProjectileServerComponent>(uid);
		ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((uid, component, args.OurBody)), args.OtherEntity, predicted);
	}

	public void ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent> projectile, EntityUid target, bool predicted = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		Entity<ProjectileComponent, PhysicsComponent> val = projectile;
		EntityUid val2 = default(EntityUid);
		ProjectileComponent projectileComponent = default(ProjectileComponent);
		PhysicsComponent val3 = default(PhysicsComponent);
		val.Deconstruct(ref val2, ref projectileComponent, ref val3);
		EntityUid uid = val2;
		ProjectileComponent component = projectileComponent;
		if (projectile.Comp1.ProjectileSpent)
		{
			if (_net.IsServer && component.DeleteOnCollide)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
			return;
		}
		ProjectileReflectAttemptEvent attemptEv = new ProjectileReflectAttemptEvent(uid, component, Cancelled: false);
		((EntitySystem)this).RaiseLocalEvent<ProjectileReflectAttemptEvent>(target, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			SetShooter(uid, component, target);
			return;
		}
		ProjectileHitEvent ev = new ProjectileHitEvent(component.Damage * _damageableSystem.UniversalProjectileDamageModifier, target, component.Shooter);
		((EntitySystem)this).RaiseLocalEvent<ProjectileHitEvent>(uid, ref ev, false);
		if (ev.Handled)
		{
			return;
		}
		EntityCoordinates coordinates = ((EntitySystem)this).Transform(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit(projectile)).Coordinates;
		EntityStringRepresentation otherName = ((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target));
		DamageSpecifier modifiedDamage = (_net.IsServer ? _damageableSystem.TryChangeDamage(target, ev.Damage, component.IgnoreResistances, interruptsDoAfters: true, null, component.Shooter, uid) : new DamageSpecifier(ev.Damage));
		bool deleted = ((EntitySystem)this).Deleted(target, (MetaDataComponent)null);
		DamageDealtEvent popupEv = new DamageDealtEvent(component.Shooter, modifiedDamage);
		((EntitySystem)this).RaiseLocalEvent<DamageDealtEvent>(target, ref popupEv, false);
		Filter filter = Filter.Pvs(coordinates, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null);
		if (_guns.GunPrediction && predicted)
		{
			PredictedProjectileServerComponent serverProjectile = default(PredictedProjectileServerComponent);
			if (((EntitySystem)this).TryComp<PredictedProjectileServerComponent>(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit(projectile), ref serverProjectile))
			{
				ICommonSession shooter = serverProjectile.Shooter;
				if (shooter != null)
				{
					filter = filter.RemovePlayer(shooter);
				}
			}
			XenoProjectileShotComponent shot = default(XenoProjectileShotComponent);
			if (_net.IsServer && ((EntitySystem)this).TryComp<XenoProjectileShotComponent>(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit(projectile), ref shot))
			{
				ICommonSession xenoShooter = shot.Shooter;
				if (xenoShooter != null)
				{
					filter = filter.RemovePlayer(xenoShooter);
				}
			}
		}
		if (modifiedDamage != null && (((EntitySystem)this).Exists(component.Shooter) || ((EntitySystem)this).Exists(component.Weapon)))
		{
			if (modifiedDamage.AnyPositive() && !deleted)
			{
				Filter colorFilter = filter;
				if (_net.IsServer && !predicted)
				{
					EntityUid? shooter2 = component.Shooter;
					if (shooter2.HasValue)
					{
						EntityUid shooter3 = shooter2.GetValueOrDefault();
						colorFilter = colorFilter.AddWhereAttachedEntity((Predicate<EntityUid>)((EntityUid attached) => attached == shooter3));
					}
				}
				_color.RaiseEffect(Color.Red, new List<EntityUid> { target }, colorFilter);
			}
			EntityUid shooterOrWeapon = (((EntitySystem)this).Exists(component.Shooter) ? component.Shooter.Value : component.Weapon.Value);
			ISharedAdminLogManager adminLogger = _adminLogger;
			int impact = ((!((EntitySystem)this).HasComp<ActorComponent>(target)) ? (-1) : 0);
			LogStringHandler handler = new LogStringHandler(43, 4);
			handler.AppendLiteral("Projectile ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "projectile", "ToPrettyString(uid)");
			handler.AppendLiteral(" shot by ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(shooterOrWeapon)), "source", "ToPrettyString(shooterOrWeapon)");
			handler.AppendLiteral(" hit ");
			handler.AppendFormatted<EntityStringRepresentation>(otherName, "target", "otherName");
			handler.AppendLiteral(" and dealt ");
			handler.AppendFormatted(modifiedDamage.GetTotal(), "damage", "modifiedDamage.GetTotal()");
			handler.AppendLiteral(" damage");
			adminLogger.Add(LogType.BulletHit, (LogImpact)impact, ref handler);
		}
		if (!deleted && filter.Count > 0)
		{
			_guns.PlayImpactSound(target, modifiedDamage, component.SoundHit, component.ForceSound, filter, Entity<ProjectileComponent, PhysicsComponent>.op_Implicit(projectile));
		}
		component.ProjectileSpent = true;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		AfterProjectileHitEvent additionalHits = new AfterProjectileHitEvent(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit(projectile), target);
		((EntitySystem)this).RaiseLocalEvent<AfterProjectileHitEvent>(uid, ref additionalHits, false);
		if (!predicted && component.DeleteOnCollide && (_net.IsServer || ((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null)))
		{
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
		else if (_net.IsServer && component.DeleteOnCollide)
		{
			PredictedProjectileHitComponent predictedComp = ((EntitySystem)this).EnsureComp<PredictedProjectileHitComponent>(uid);
			predictedComp.Origin = _transform.GetMoverCoordinates(coordinates);
			EntityCoordinates targetCoords = _transform.GetMoverCoordinates(target);
			float distance = default(float);
			if (((EntityCoordinates)(ref predictedComp.Origin)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, targetCoords, ref distance))
			{
				predictedComp.Distance = distance;
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)predictedComp, (MetaDataComponent)null);
		}
		if ((_net.IsServer || ((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null)) && component.ImpactEffect.HasValue)
		{
			EntProtoId? impactEffect = component.ImpactEffect;
			ImpactEffectEvent impactEffectEv = new ImpactEffectEvent(impactEffect.HasValue ? EntProtoId.op_Implicit(impactEffect.GetValueOrDefault()) : null, ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null));
			if (_net.IsServer)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)impactEffectEv, filter, true);
			}
			else
			{
				((EntitySystem)this).RaiseLocalEvent<ImpactEffectEvent>(impactEffectEv);
			}
		}
	}

	private void OnEmbedActivate(Entity<EmbeddableProjectileComponent> embeddable, ref ActivateInWorldEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (embeddable.Comp.RemovalTime.HasValue && !((HandledEntityEventArgs)args).Handled && args.Complex && ((EntitySystem)this).TryComp<PhysicsComponent>(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), ref physics) && (int)physics.BodyType == 4)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, embeddable.Comp.RemovalTime.Value, new RemoveEmbeddedProjectileEvent(), Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable)));
		}
	}

	private void OnEmbedRemove(Entity<EmbeddableProjectileComponent> embeddable, ref RemoveEmbeddedProjectileEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EmbedDetach(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), embeddable.Comp, args.User);
			_hands.TryPickupAnyHand(args.User, Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable));
		}
	}

	private void OnEmbeddableCompShutdown(Entity<EmbeddableProjectileComponent> embeddable, ref ComponentShutdown arg)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EmbedDetach(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), embeddable.Comp);
	}

	private void OnEmbedThrowDoHit(Entity<EmbeddableProjectileComponent> embeddable, ref ThrowDoHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (embeddable.Comp.EmbedOnThrow)
		{
			EmbedAttach(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), args.Target, null, embeddable.Comp);
		}
	}

	private void OnEmbedProjectileHit(Entity<EmbeddableProjectileComponent> embeddable, ref ProjectileHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		EmbedAttach(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), args.Target, args.Shooter, embeddable.Comp);
		ProjectileComponent projectile = default(ProjectileComponent);
		if (!((EntitySystem)this).TryComp<ProjectileComponent>(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), ref projectile))
		{
			return;
		}
		EntityUid? shooter = projectile.Shooter;
		if (shooter.HasValue)
		{
			EntityUid shooter2 = shooter.GetValueOrDefault();
			shooter = projectile.Weapon;
			if (shooter.HasValue)
			{
				EntityUid weapon = shooter.GetValueOrDefault();
				ProjectileEmbedEvent ev = new ProjectileEmbedEvent(shooter2, weapon, args.Target);
				((EntitySystem)this).RaiseLocalEvent<ProjectileEmbedEvent>(Entity<EmbeddableProjectileComponent>.op_Implicit(embeddable), ref ev, false);
			}
		}
	}

	private void EmbedAttach(EntityUid uid, EntityUid target, EntityUid? user, EmbeddableProjectileComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref physics);
		_physics.SetLinearVelocity(uid, Vector2.Zero, true, true, (FixturesComponent)null, physics);
		_physics.SetBodyType(uid, (BodyType)4, (FixturesComponent)null, physics, (TransformComponent)null);
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		_transform.SetParent(uid, xform, target, (TransformComponent)null);
		if (component.Offset != Vector2.Zero)
		{
			Angle rotation = xform.LocalRotation;
			ThrowingAngleComponent throwingAngleComp = default(ThrowingAngleComponent);
			if (((EntitySystem)this).TryComp<ThrowingAngleComponent>(uid, ref throwingAngleComp))
			{
				rotation += throwingAngleComp.Angle;
			}
			_transform.SetLocalPosition(uid, xform.LocalPosition + ((Angle)(ref rotation)).RotateVec(ref component.Offset), xform);
		}
		_audio.PlayPredicted(component.Sound, uid, (EntityUid?)null, (AudioParams?)null);
		component.EmbeddedIntoUid = target;
		EmbedEvent ev = new EmbedEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<EmbedEvent>(uid, ref ev, false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		EmbeddedContainerComponent embeddedContainer = default(EmbeddedContainerComponent);
		((EntitySystem)this).EnsureComp<EmbeddedContainerComponent>(target, ref embeddedContainer);
		embeddedContainer.EmbeddedObjects.Add(uid);
	}

	public void EmbedDetach(EntityUid uid, EmbeddableProjectileComponent? component, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EmbeddableProjectileComponent>(uid, ref component, true))
		{
			return;
		}
		EntityUid? embeddedIntoUid = component.EmbeddedIntoUid;
		EmbeddedContainerComponent embeddedContainer = default(EmbeddedContainerComponent);
		if (embeddedIntoUid.HasValue && ((EntitySystem)this).TryComp<EmbeddedContainerComponent>(component.EmbeddedIntoUid.Value, ref embeddedContainer))
		{
			embeddedContainer.EmbeddedObjects.Remove(uid);
			((EntitySystem)this).Dirty(component.EmbeddedIntoUid.Value, (IComponent)(object)embeddedContainer, (MetaDataComponent)null);
			if (embeddedContainer.EmbeddedObjects.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<EmbeddedContainerComponent>(component.EmbeddedIntoUid.Value);
			}
		}
		if (component.DeleteOnRemove && _net.IsServer)
		{
			((EntitySystem)this).QueueDel((EntityUid?)uid);
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		if (!((EntitySystem)this).TerminatingOrDeleted(xform.GridUid, (MetaDataComponent)null) || !((EntitySystem)this).TerminatingOrDeleted(xform.MapUid, (MetaDataComponent)null))
		{
			PhysicsComponent physics = default(PhysicsComponent);
			((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref physics);
			_physics.SetBodyType(uid, (BodyType)8, (FixturesComponent)null, physics, xform);
			_transform.AttachToGridOrMap(uid, xform);
			component.EmbeddedIntoUid = null;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			ProjectileComponent projectile = default(ProjectileComponent);
			if (((EntitySystem)this).TryComp<ProjectileComponent>(uid, ref projectile))
			{
				projectile.Shooter = null;
				projectile.Weapon = null;
				projectile.ProjectileSpent = false;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)projectile, (MetaDataComponent)null);
			}
			if (user.HasValue)
			{
				LandEvent landEv = new LandEvent(user, PlaySound: true);
				((EntitySystem)this).RaiseLocalEvent<LandEvent>(uid, ref landEv, false);
			}
			_physics.WakeBody(uid, false, (FixturesComponent)null, physics);
		}
	}

	private void OnEmbeddableTermination(Entity<EmbeddedContainerComponent> container, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DetachAllEmbedded(container);
	}

	public void DetachAllEmbedded(Entity<EmbeddedContainerComponent> container)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EmbeddableProjectileComponent embeddedComp = default(EmbeddableProjectileComponent);
		foreach (EntityUid embedded in container.Comp.EmbeddedObjects)
		{
			if (((EntitySystem)this).TryComp<EmbeddableProjectileComponent>(embedded, ref embeddedComp))
			{
				EmbedDetach(embedded, embeddedComp);
			}
		}
	}

	private void PreventCollision(EntityUid uid, ProjectileComponent component, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (component.IgnoreShooter)
		{
			EntityUid otherEntity = args.OtherEntity;
			EntityUid? shooter = component.Shooter;
			if (!shooter.HasValue || !(otherEntity == shooter.GetValueOrDefault()))
			{
				otherEntity = args.OtherEntity;
				shooter = component.Weapon;
				if (!shooter.HasValue || !(otherEntity == shooter.GetValueOrDefault()))
				{
					goto IL_0062;
				}
			}
			args.Cancelled = true;
			return;
		}
		goto IL_0062;
		IL_0062:
		if (!component.Weapon.HasValue || !((EntitySystem)this).HasComp<GunIgnoreContainerOwnerCollisionComponent>(component.Weapon.Value))
		{
			return;
		}
		EntityUid current = component.Weapon.Value;
		BaseContainer container = default(BaseContainer);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(current, null)), ref container))
		{
			if (args.OtherEntity == container.Owner)
			{
				args.Cancelled = true;
				break;
			}
			current = container.Owner;
		}
	}

	public void SetShooter(EntityUid id, ProjectileComponent component, EntityUid? shooterId = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? shooter = component.Shooter;
		EntityUid? val = shooterId;
		if ((shooter.HasValue != val.HasValue || (shooter.HasValue && !(shooter.GetValueOrDefault() == val.GetValueOrDefault()))) && shooterId.HasValue)
		{
			component.Shooter = shooterId;
			((EntitySystem)this).Dirty(id, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
