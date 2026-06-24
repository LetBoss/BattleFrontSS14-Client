using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Barricade;
using Content.Shared._RMC14.Barricade.Components;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Invisibility;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Spray;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.ActionBlocker;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Leap;

public sealed class XenoLeapSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private BlindableSystem _blindable;

	[Dependency]
	private SharedBroadphaseSystem _broadphase;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private DamageableSystem _damagable;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private RMCCameraShakeSystem _cameraShake;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCObstacleSlammingSystem _obstacleSlamming;

	[Dependency]
	private SharedDirectionalAttackBlockSystem _directionalBlock;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_fixturesQuery = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		((EntitySystem)this).SubscribeAllEvent<XenoLeapPredictedHitEvent>((EntitySessionEventHandler<XenoLeapPredictedHitEvent>)OnPredictedHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapComponent, XenoLeapActionEvent>((EntityEventRefHandler<XenoLeapComponent, XenoLeapActionEvent>)OnXenoLeapAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapComponent, XenoLeapDoAfterEvent>((EntityEventRefHandler<XenoLeapComponent, XenoLeapDoAfterEvent>)OnXenoLeapDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapComponent, MeleeHitEvent>((EntityEventRefHandler<XenoLeapComponent, MeleeHitEvent>)OnXenoLeapMelee, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapComponent, RMCMeleeUserGetRangeEvent>((EntityEventRefHandler<XenoLeapComponent, RMCMeleeUserGetRangeEvent>)OnXenoLeapingMeleeGetRange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotEquippedHandEvent>((EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotEquippedHandEvent>)OnEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotUnequippedHandEvent>((EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotUnequippedHandEvent>)OnUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotEquippedEvent>((EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotUnequippedEvent>((EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCLeapProtectionComponent, MapInitEvent>((EntityEventRefHandler<RMCLeapProtectionComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCLeapProtectionComponent, XenoLeapHitAttempt>((EntityEventRefHandler<RMCLeapProtectionComponent, XenoLeapHitAttempt>)OnXenoLeapHitAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapingComponent, StartCollideEvent>((EntityEventRefHandler<XenoLeapingComponent, StartCollideEvent>)OnXenoLeapingDoHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapingComponent, ComponentRemove>((EntityEventRefHandler<XenoLeapingComponent, ComponentRemove>)OnXenoLeapingRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapingComponent, PhysicsSleepEvent>((EntityEventRefHandler<XenoLeapingComponent, PhysicsSleepEvent>)OnXenoLeapingPhysicsSleep, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapingComponent, StartPullAttemptEvent>((EntityEventRefHandler<XenoLeapingComponent, StartPullAttemptEvent>)OnXenoLeapingStartPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLeapingComponent, PullAttemptEvent>((EntityEventRefHandler<XenoLeapingComponent, PullAttemptEvent>)OnXenoLeapingPullAttempt, (Type[])null, (Type[])null);
	}

	private void OnPredictedHit(XenoLeapPredictedHitEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid ent = attachedEntity.GetValueOrDefault();
		XenoLeapingComponent leaping = default(XenoLeapingComponent);
		if (!((EntitySystem)this).TryComp<XenoLeapingComponent>(ent, ref leaping))
		{
			return;
		}
		EntityUid target = ((EntitySystem)this).GetEntity(msg.Target);
		if (!((EntityUid)(ref target)).Valid)
		{
			return;
		}
		if (_net.IsServer)
		{
			if (!((EntitySystem)this).HasComp<XenoLeapComponent>(ent) || !((Component)leaping).Running)
			{
				return;
			}
			_rmcLagCompensation.SetLastRealTick(((EntitySessionEventArgs)(ref args)).SenderSession.UserId, msg.LastRealTick);
			if (!_rmcLagCompensation.Collides(Entity<FixturesComponent>.op_Implicit(target), Entity<PhysicsComponent>.op_Implicit(ent), ((EntitySessionEventArgs)(ref args)).SenderSession))
			{
				return;
			}
		}
		ApplyLeapingHitEffects(Entity<XenoLeapingComponent>.op_Implicit((ent, leaping)), target);
	}

	private void OnXenoLeapAction(Entity<XenoLeapComponent> xeno, ref XenoLeapActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			XenoLeapAttemptEvent attempt = default(XenoLeapAttemptEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoLeapAttemptEvent>(Entity<XenoLeapComponent>.op_Implicit(xeno), ref attempt, false);
			if (!attempt.Cancelled && (!(xeno.Comp.PlasmaCost > FixedPoint2.Zero) || _xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost)))
			{
				((HandledEntityEventArgs)args).Handled = true;
				XenoLeapDoAfterEvent ev = new XenoLeapDoAfterEvent(((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null));
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoLeapComponent>.op_Implicit(xeno), xeno.Comp.Delay, ev, Entity<XenoLeapComponent>.op_Implicit(xeno))
				{
					BreakOnMove = true,
					BreakOnDamage = true,
					DamageThreshold = FixedPoint2.New(10)
				};
				_doAfter.TryStartDoAfter(doAfter);
			}
		}
	}

	private void OnXenoLeapDoAfter(Entity<XenoLeapComponent> xeno, ref XenoLeapDoAfterEvent args)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (args.Cancelled)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-leap-cancelled"), Entity<XenoLeapComponent>.op_Implicit(xeno), Entity<XenoLeapComponent>.op_Implicit(xeno));
		}
		else
		{
			PhysicsComponent physics = default(PhysicsComponent);
			XenoLeapingComponent leaping = default(XenoLeapingComponent);
			if (!_physicsQuery.TryGetComponent(Entity<XenoLeapComponent>.op_Implicit(xeno), ref physics) || ((EntitySystem)this).EnsureComp<XenoLeapingComponent>(Entity<XenoLeapComponent>.op_Implicit(xeno), ref leaping))
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			leaping.KnockdownRequiresInvisibility = xeno.Comp.KnockdownRequiresInvisibility;
			leaping.DestroyObjects = xeno.Comp.DestroyObjects;
			leaping.MoveDelayTime = xeno.Comp.MoveDelayTime;
			leaping.Damage = xeno.Comp.Damage;
			leaping.HitEffect = xeno.Comp.HitEffect;
			leaping.TargetJitterTime = xeno.Comp.TargetJitterTime;
			leaping.TargetCameraShakeStrength = xeno.Comp.TargetCameraShakeStrength;
			leaping.IgnoredCollisionGroupLarge = xeno.Comp.IgnoredCollisionGroupLarge;
			leaping.IgnoredCollisionGroupSmall = xeno.Comp.IgnoredCollisionGroupSmall;
			if (xeno.Comp.PlasmaCost > FixedPoint2.Zero && !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
			{
				return;
			}
			_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoLeapComponent>.op_Implicit(xeno));
			MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoLeapComponent>.op_Implicit(xeno), (TransformComponent)null);
			Vector2 direction = _transform.ToMapCoordinates(args.Coordinates).Position - origin.Position;
			if (direction == Vector2.Zero)
			{
				return;
			}
			float length = direction.Length();
			float distance = Math.Clamp(length, 0.1f, xeno.Comp.Range.Float());
			direction *= distance / length;
			Vector2 impulse = Vector2Helpers.Normalized(direction) * xeno.Comp.Strength * physics.Mass;
			leaping.Origin = _transform.GetMoverCoordinates(Entity<XenoLeapComponent>.op_Implicit(xeno));
			leaping.ParalyzeTime = xeno.Comp.KnockdownTime;
			leaping.LeapSound = xeno.Comp.LeapSound;
			leaping.LeapEndTime = _timing.CurTime + TimeSpan.FromSeconds(direction.Length() / (float)xeno.Comp.Strength);
			_obstacleSlamming.MakeImmune(Entity<XenoLeapComponent>.op_Implicit(xeno), 0.5f);
			_physics.ApplyLinearImpulse(Entity<XenoLeapComponent>.op_Implicit(xeno), impulse, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(Entity<XenoLeapComponent>.op_Implicit(xeno), physics, (BodyStatus)1, true);
			FixturesComponent fixtures = default(FixturesComponent);
			if (((EntitySystem)this).TryComp<FixturesComponent>(Entity<XenoLeapComponent>.op_Implicit(xeno), ref fixtures))
			{
				int collisionGroup = (int)leaping.IgnoredCollisionGroupSmall;
				if (_size.TryGetSize(Entity<XenoLeapComponent>.op_Implicit(xeno), out var size) && (int)size > 3)
				{
					collisionGroup = (int)leaping.IgnoredCollisionGroupLarge;
				}
				KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
				_physics.SetCollisionMask(Entity<XenoLeapComponent>.op_Implicit(xeno), fixture.Key, fixture.Value, fixture.Value.CollisionMask ^ collisionGroup, (FixturesComponent)null, (PhysicsComponent)null);
			}
			foreach (EntityUid ent in _physics.GetContactingEntities(xeno.Owner, physics, false))
			{
				if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(ent)) && ApplyLeapingHitEffects(Entity<XenoLeapingComponent>.op_Implicit((Entity<XenoLeapComponent>.op_Implicit(xeno), leaping)), ent))
				{
					break;
				}
			}
		}
	}

	private void OnXenoLeapMelee(Entity<XenoLeapComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.UnrootOnMelee || !args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		using IEnumerator<EntityUid> enumerator = args.HitEntities.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		EntityUid entity = enumerator.Current;
		if (_xeno.CanAbilityAttackTarget(Entity<XenoLeapComponent>.op_Implicit(xeno), entity))
		{
			SlowedDownComponent root = default(SlowedDownComponent);
			if (((EntitySystem)this).TryComp<SlowedDownComponent>(Entity<XenoLeapComponent>.op_Implicit(xeno), ref root) && root.SprintSpeedModifier == 0f)
			{
				((EntitySystem)this).RemComp<SlowedDownComponent>(Entity<XenoLeapComponent>.op_Implicit(xeno));
				_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoLeapComponent>.op_Implicit(xeno));
			}
			xeno.Comp.LastHit = null;
			xeno.Comp.LastHitAt = null;
			((EntitySystem)this).Dirty<XenoLeapComponent>(xeno, (MetaDataComponent)null);
		}
	}

	private void OnXenoLeapingMeleeGetRange(Entity<XenoLeapComponent> ent, ref RMCMeleeUserGetRangeEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.LastHit.HasValue)
		{
			return;
		}
		EntityUid? lastHit = ent.Comp.LastHit;
		EntityUid? target = args.Target;
		if (lastHit.HasValue == target.HasValue && (!lastHit.HasValue || !(lastHit.GetValueOrDefault() != target.GetValueOrDefault())))
		{
			TimeSpan curTime = _timing.CurTime;
			TimeSpan? timeSpan = ent.Comp.LastHitAt + ent.Comp.MoveDelayTime;
			if (!(curTime > timeSpan))
			{
				args.Range = ent.Comp.LastHitRange;
			}
		}
	}

	private void OnXenoLeapingDoHit(Entity<XenoLeapingComponent> xeno, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		ApplyLeapingHitEffects(xeno, args.OtherEntity);
	}

	private void OnXenoLeapingRemove(Entity<XenoLeapingComponent> ent, ref ComponentRemove args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		XenoLeapStoppedEvent ev = default(XenoLeapStoppedEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoLeapStoppedEvent>(Entity<XenoLeapingComponent>.op_Implicit(ent), ref ev, false);
		StopLeap(ent);
	}

	private void OnXenoLeapingPhysicsSleep(Entity<XenoLeapingComponent> ent, ref PhysicsSleepEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopLeap(ent);
	}

	private void OnXenoLeapingStartPullAttempt(Entity<XenoLeapingComponent> ent, ref StartPullAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnXenoLeapingPullAttempt(Entity<XenoLeapingComponent> ent, ref PullAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnXenoLeapHitAttempt(Entity<RMCLeapProtectionComponent> ent, ref XenoLeapHitAttempt args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		XenoLeapingComponent leaping = default(XenoLeapingComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<XenoLeapingComponent>(args.Leaper, ref leaping))
		{
			args.Cancelled = AttemptBlockLeap(ent.Owner, ent.Comp.StunDuration, ent.Comp.BlockSound, args.Leaper, leaping.Origin, ent.Comp.FullProtection);
		}
	}

	private void OnGotEquipped(Entity<RMCGrantLeapProtectionComponent> ent, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			ApplyLeapProtection(args.Equipee, ent);
		}
	}

	private void OnGotUnequipped(Entity<RMCGrantLeapProtectionComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE && RemoveLeapProtection(args.Equipee, ent))
		{
			((EntitySystem)this).RemCompDeferred<RMCLeapProtectionComponent>(args.Equipee);
		}
	}

	private void OnEquippedHand(Entity<RMCGrantLeapProtectionComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ProtectsInHand)
		{
			ApplyLeapProtection(args.User, ent);
		}
	}

	private void OnUnequippedHand(Entity<RMCGrantLeapProtectionComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ProtectsInHand && RemoveLeapProtection(args.User, ent))
		{
			((EntitySystem)this).RemCompDeferred<RMCLeapProtectionComponent>(args.User);
		}
	}

	private void OnMapInit(Entity<RMCLeapProtectionComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.InherentStunDuration.HasValue)
		{
			ent.Comp.StunDuration = ent.Comp.InherentStunDuration.Value;
		}
	}

	private void ApplyLeapProtection(EntityUid receiver, Entity<RMCGrantLeapProtectionComponent> protection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		RMCLeapProtectionComponent leapProtection = ((EntitySystem)this).EnsureComp<RMCLeapProtectionComponent>(receiver);
		leapProtection.ProtectionProviders.Add(Entity<RMCGrantLeapProtectionComponent>.op_Implicit(protection));
		if (protection.Comp.StunDuration >= leapProtection.StunDuration)
		{
			leapProtection.StunDuration = protection.Comp.StunDuration;
			leapProtection.BlockSound = protection.Comp.BlockSound;
		}
		((EntitySystem)this).Dirty(receiver, (IComponent)(object)leapProtection, (MetaDataComponent)null);
	}

	private bool RemoveLeapProtection(EntityUid user, Entity<RMCGrantLeapProtectionComponent> protection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		RMCLeapProtectionComponent leapProtection = default(RMCLeapProtectionComponent);
		if (!((EntitySystem)this).TryComp<RMCLeapProtectionComponent>(user, ref leapProtection))
		{
			return true;
		}
		TimeSpan stunDuration = default(TimeSpan);
		leapProtection.ProtectionProviders.Remove(Entity<RMCGrantLeapProtectionComponent>.op_Implicit(protection));
		if (leapProtection.InherentStunDuration.HasValue)
		{
			stunDuration = leapProtection.InherentStunDuration.Value;
			leapProtection.BlockSound = leapProtection.InherentBlockSound;
		}
		RMCGrantLeapProtectionComponent grantProtection = default(RMCGrantLeapProtectionComponent);
		foreach (EntityUid protectionGranter in leapProtection.ProtectionProviders)
		{
			if (((EntitySystem)this).TryComp<RMCGrantLeapProtectionComponent>(protectionGranter, ref grantProtection) && !(grantProtection.StunDuration < stunDuration))
			{
				stunDuration = grantProtection.StunDuration;
				leapProtection.BlockSound = grantProtection.BlockSound;
			}
		}
		if (stunDuration != TimeSpan.Zero)
		{
			leapProtection.StunDuration = stunDuration;
			((EntitySystem)this).Dirty(user, (IComponent)(object)leapProtection, (MetaDataComponent)null);
			return false;
		}
		return true;
	}

	public bool AttemptBlockLeap(EntityUid blocker, TimeSpan stunDuration, SoundSpecifier blockSound, EntityUid leaper, EntityCoordinates leapOrigin, bool omnidirectionalProtection = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		if (!_directionalBlock.IsFacingTarget(blocker, leaper, leapOrigin) && !omnidirectionalProtection)
		{
			return false;
		}
		BarbedComponent barbed = default(BarbedComponent);
		if (((EntitySystem)this).HasComp<BarricadeComponent>(blocker) && (!((EntitySystem)this).TryComp<BarbedComponent>(blocker, ref barbed) || !barbed.IsBarbed))
		{
			return false;
		}
		if (_size.TryGetSize(leaper, out var size) && (int)size >= 5 && !((EntitySystem)this).HasComp<BarbedComponent>(blocker))
		{
			return false;
		}
		MapCoordinates blockerCoordinates = _transform.GetMapCoordinates(blocker, ((EntitySystem)this).Transform(blocker));
		if ((int)size < 5)
		{
			_stun.TryParalyze(leaper, stunDuration, refresh: true);
		}
		_size.KnockBack(leaper, blockerCoordinates, 1f, 1f, 5f, ignoreSize: true);
		_audio.PlayPredicted(blockSound, leaper, (EntityUid?)leaper, (AudioParams?)null);
		string selfMessage = base.Loc.GetString("rmc-obstacle-slam-self", (ValueTuple<string, object>)("object", Identity.Name(blocker, (IEntityManager)(object)base.EntityManager, leaper)));
		_popup.PopupClient(selfMessage, leaper, leaper, PopupType.MediumCaution);
		foreach (ICommonSession recipient in Filter.PvsExcept(leaper, 2f, (IEntityManager)null).Recipients)
		{
			EntityUid? attachedEntity = recipient.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid otherEnt = attachedEntity.GetValueOrDefault();
				string othersMessage = base.Loc.GetString("rmc-obstacle-slam-others", (ValueTuple<string, object>)("ent", Identity.Name(leaper, (IEntityManager)(object)base.EntityManager, otherEnt)), (ValueTuple<string, object>)("object", Identity.Name(blocker, (IEntityManager)(object)base.EntityManager, otherEnt)));
				_popup.PopupEntity(othersMessage, leaper, otherEnt, PopupType.MediumCaution);
			}
		}
		return true;
	}

	private bool IsValidLeapHit(Entity<XenoLeapingComponent> xeno, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.KnockedDown)
		{
			return false;
		}
		XenoLeapDestroyOnPassComponent destroy = default(XenoLeapDestroyOnPassComponent);
		if (xeno.Comp.DestroyObjects && ((EntitySystem)this).TryComp<XenoLeapDestroyOnPassComponent>(target, ref destroy))
		{
			if (_net.IsServer)
			{
				for (int i = 0; i < destroy.Amount; i++)
				{
					if (destroy.SpawnPrototype.HasValue)
					{
						EntProtoId? spawnPrototype = destroy.SpawnPrototype;
						((EntitySystem)this).SpawnAtPosition(spawnPrototype.HasValue ? EntProtoId.op_Implicit(spawnPrototype.GetValueOrDefault()) : null, target.ToCoordinates(), (ComponentRegistry)null);
					}
				}
				((EntitySystem)this).QueueDel((EntityUid?)target);
			}
			_physics.SetCanCollide(target, false, true, true, (FixturesComponent)null, (PhysicsComponent)null);
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoParasiteComponent>(target) || ((EntitySystem)this).HasComp<XenoFruitComponent>(target) || ((EntitySystem)this).HasComp<XenoEggComponent>(target) || ((EntitySystem)this).HasComp<XenoAcidSplatterComponent>(target))
		{
			return false;
		}
		if (_standing.IsDown(target))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<LeapIncapacitatedComponent>(target))
		{
			return false;
		}
		if (_size.TryGetSize(target, out var size) && (int)size >= 5)
		{
			return false;
		}
		if (size == RMCSizes.VerySmallXeno)
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoWeedsComponent>(target) || ((EntitySystem)this).HasComp<XenoConstructComponent>(target))
		{
			return false;
		}
		return true;
	}

	private bool ApplyLeapingHitEffects(Entity<XenoLeapingComponent> xeno, EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		if (!IsValidLeapHit(xeno, target))
		{
			return false;
		}
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			StopLeap(xeno);
			return true;
		}
		XenoLeapHitAttempt leapEv = new XenoLeapHitAttempt(xeno.Owner);
		((EntitySystem)this).RaiseLocalEvent<XenoLeapHitAttempt>(target, ref leapEv, false);
		if (leapEv.Cancelled)
		{
			xeno.Comp.KnockedDown = true;
			StopLeap(xeno);
			((EntitySystem)this).Dirty<XenoLeapingComponent>(xeno, (MetaDataComponent)null);
			return true;
		}
		if (!((EntitySystem)this).HasComp<MobStateComponent>(target) || _mobState.IsIncapacitated(target))
		{
			return false;
		}
		xeno.Comp.KnockedDown = true;
		((EntitySystem)this).Dirty<XenoLeapingComponent>(xeno, (MetaDataComponent)null);
		XenoLeapComponent leap = default(XenoLeapComponent);
		if (((EntitySystem)this).TryComp<XenoLeapComponent>(Entity<XenoLeapingComponent>.op_Implicit(xeno), ref leap))
		{
			leap.LastHit = target;
			leap.LastHitAt = _timing.CurTime;
			((EntitySystem)this).Dirty(Entity<XenoLeapingComponent>.op_Implicit(xeno), (IComponent)(object)leap, (MetaDataComponent)null);
		}
		PhysicsComponent physics = default(PhysicsComponent);
		if (_physicsQuery.TryGetComponent(Entity<XenoLeapingComponent>.op_Implicit(xeno), ref physics))
		{
			_physics.SetBodyStatus(Entity<XenoLeapingComponent>.op_Implicit(xeno), physics, (BodyStatus)0, true);
			if (physics.Awake)
			{
				_broadphase.RegenerateContacts(Entity<XenoLeapingComponent>.op_Implicit(xeno), physics, (FixturesComponent)null, (TransformComponent)null);
			}
		}
		if (!xeno.Comp.KnockdownRequiresInvisibility || ((EntitySystem)this).HasComp<XenoActiveInvisibleComponent>(Entity<XenoLeapingComponent>.op_Implicit(xeno)))
		{
			LeapIncapacitatedComponent victim = ((EntitySystem)this).EnsureComp<LeapIncapacitatedComponent>(target);
			victim.RecoverAt = _timing.CurTime + xeno.Comp.ParalyzeTime;
			((EntitySystem)this).Dirty(target, (IComponent)(object)victim, (MetaDataComponent)null);
			_stun.TrySlowdown(Entity<XenoLeapingComponent>.op_Implicit(xeno), xeno.Comp.MoveDelayTime, refresh: true, 0f, 0f);
			if (_net.IsServer)
			{
				_stun.TryParalyze(target, _xeno.TryApplyXenoDebuffMultiplier(target, xeno.Comp.ParalyzeTime), refresh: true);
			}
		}
		if (xeno.Comp.HitEffect.HasValue && _net.IsServer)
		{
			EntProtoId? hitEffect = xeno.Comp.HitEffect;
			((EntitySystem)this).SpawnAttachedTo(hitEffect.HasValue ? EntProtoId.op_Implicit(hitEffect.GetValueOrDefault()) : null, target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		if (_damagable.TryChangeDamage(target, _xeno.TryApplyXenoSlashDamageMultiplier(target, xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoLeapingComponent>.op_Implicit(xeno), Entity<XenoLeapingComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { target }, filter);
		}
		_jitter.DoJitter(target, xeno.Comp.TargetJitterTime, refresh: false);
		_cameraShake.ShakeCamera(target, 2, xeno.Comp.TargetCameraShakeStrength);
		XenoLeapHitEvent ev = new XenoLeapHitEvent(Entity<XenoLeapingComponent>.op_Implicit(xeno), target);
		((EntitySystem)this).RaiseLocalEvent<XenoLeapHitEvent>(Entity<XenoLeapingComponent>.op_Implicit(xeno), ref ev, false);
		if (!xeno.Comp.PlayedSound && _net.IsServer)
		{
			xeno.Comp.PlayedSound = true;
			_audio.PlayPvs(xeno.Comp.LeapSound, Entity<XenoLeapingComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		if (_net.IsClient)
		{
			XenoLeapPredictedHitEvent predictedEv = new XenoLeapPredictedHitEvent(((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null), _rmcLagCompensation.GetLastRealTick(null));
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)predictedEv);
			if (_timing.InPrediction && _timing.IsFirstTimePredicted)
			{
				((EntitySystem)this).RaisePredictiveEvent<XenoLeapPredictedHitEvent>(predictedEv);
			}
		}
		StopLeap(xeno);
		return true;
	}

	private void StopLeap(Entity<XenoLeapingComponent> leaping)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (_physicsQuery.TryGetComponent(Entity<XenoLeapingComponent>.op_Implicit(leaping), ref physics))
		{
			_physics.SetLinearVelocity(Entity<XenoLeapingComponent>.op_Implicit(leaping), Vector2.Zero, true, true, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(Entity<XenoLeapingComponent>.op_Implicit(leaping), physics, (BodyStatus)0, true);
		}
		FixturesComponent fixtures = default(FixturesComponent);
		if (_fixturesQuery.TryGetComponent(Entity<XenoLeapingComponent>.op_Implicit(leaping), ref fixtures))
		{
			int collisionGroup = (int)leaping.Comp.IgnoredCollisionGroupSmall;
			if (_size.TryGetSize(Entity<XenoLeapingComponent>.op_Implicit(leaping), out var size) && (int)size > 3)
			{
				collisionGroup = (int)leaping.Comp.IgnoredCollisionGroupLarge;
			}
			if ((int)size >= 3)
			{
				KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
				_physics.SetCollisionMask(Entity<XenoLeapingComponent>.op_Implicit(leaping), fixture.Key, fixture.Value, fixture.Value.CollisionMask | collisionGroup, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
		((EntitySystem)this).RemCompDeferred<XenoLeapingComponent>(Entity<XenoLeapingComponent>.op_Implicit(leaping));
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoLeapingComponent> leaping = ((EntitySystem)this).EntityQueryEnumerator<XenoLeapingComponent>();
		EntityUid uid = default(EntityUid);
		XenoLeapingComponent comp = default(XenoLeapingComponent);
		while (leaping.MoveNext(ref uid, ref comp))
		{
			if (!(time < comp.LeapEndTime))
			{
				StopLeap(Entity<XenoLeapingComponent>.op_Implicit((uid, comp)));
			}
		}
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<LeapIncapacitatedComponent> incapacitated = ((EntitySystem)this).EntityQueryEnumerator<LeapIncapacitatedComponent>();
		EntityUid uid2 = default(EntityUid);
		LeapIncapacitatedComponent victim = default(LeapIncapacitatedComponent);
		while (incapacitated.MoveNext(ref uid2, ref victim))
		{
			if (!(victim.RecoverAt > time))
			{
				((EntitySystem)this).RemCompDeferred<LeapIncapacitatedComponent>(uid2);
				_blindable.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(uid2));
				_actionBlocker.UpdateCanMove(uid2);
			}
		}
	}
}
