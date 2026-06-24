using System;
using System.Numerics;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Lunge;

public sealed class XenoLungeSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ThrownItemSystem _thrownItem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedRMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private RMCObstacleSlammingSystem _rmcObstacleSlamming;

	[Dependency]
	private XenoLeapSystem _leap;

	[Dependency]
	private RMCSizeStunSystem _size;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<ThrownItemComponent> _thrownItemQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_thrownItemQuery = ((EntitySystem)this).GetEntityQuery<ThrownItemComponent>();
		((EntitySystem)this).SubscribeAllEvent<XenoLungePredictedHitEvent>((EntitySessionEventHandler<XenoLungePredictedHitEvent>)OnPredictedHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLungeComponent, XenoLungeActionEvent>((EntityEventRefHandler<XenoLungeComponent, XenoLungeActionEvent>)OnXenoLungeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLungeComponent, MeleeAttackAttemptEvent>((EntityEventRefHandler<XenoLungeComponent, MeleeAttackAttemptEvent>)OnAttackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveLungeComponent, ThrowDoHitEvent>((EntityEventRefHandler<XenoActiveLungeComponent, ThrowDoHitEvent>)OnXenoLungeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveLungeComponent, LandEvent>((EntityEventRefHandler<XenoActiveLungeComponent, LandEvent>)OnXenoLungeLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCLungeProtectionComponent, XenoLungeHitAttempt>((EntityEventRefHandler<RMCLungeProtectionComponent, XenoLungeHitAttempt>)OnXenoLungeHitAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoLungeStunnedComponent, PullStoppedMessage>((EntityEventRefHandler<XenoLungeStunnedComponent, PullStoppedMessage>)OnXenoLungeStunnedPullStopped, (Type[])null, (Type[])null);
	}

	private void OnPredictedHit(XenoLungePredictedHitEvent msg, EntitySessionEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid ent = attachedEntity.GetValueOrDefault();
		XenoActiveLungeComponent lunging = default(XenoActiveLungeComponent);
		if (((EntitySystem)this).TryComp<XenoActiveLungeComponent>(ent, ref lunging))
		{
			EntityUid target = ((EntitySystem)this).GetEntity(msg.Target);
			if (((EntityUid)(ref target)).Valid && ((Component)lunging).Running && !(lunging.Target != target))
			{
				_rmcLagCompensation.SetLastRealTick(((EntitySessionEventArgs)(ref args)).SenderSession.UserId, msg.LastRealTick);
				ApplyLungeHitEffects(Entity<XenoActiveLungeComponent>.op_Implicit((ent, lunging)), target, stopThrow: true, predicted: false);
			}
		}
	}

	private void OnXenoLungeAction(Entity<XenoLungeComponent> xeno, ref XenoLungeActionEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? entity = args.Entity;
		if (!entity.HasValue)
		{
			return;
		}
		EntityUid target = entity.GetValueOrDefault();
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoLungeComponent>.op_Implicit(xeno), target) || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoLungeAttemptEvent attempt = default(XenoLungeAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoLungeAttemptEvent>(Entity<XenoLungeComponent>.op_Implicit(xeno), ref attempt, false);
		if (attempt.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoLungeComponent>.op_Implicit(xeno));
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoLungeComponent>.op_Implicit(xeno), (TransformComponent)null);
		EntityCoordinates targetCoords = _rmcLagCompensation.GetCoordinates(target, (EntityUid?)Entity<XenoLungeComponent>.op_Implicit(xeno), (TransformComponent?)null);
		Vector2 diff = targetCoords.Position - origin.Position;
		diff = Vector2Helpers.Normalized(diff) * xeno.Comp.Range;
		XenoActiveLungeComponent xenoActiveLungeComponent = ((EntitySystem)this).EnsureComp<XenoActiveLungeComponent>(Entity<XenoLungeComponent>.op_Implicit(xeno));
		xenoActiveLungeComponent.Origin = origin;
		xenoActiveLungeComponent.Charge = diff;
		xenoActiveLungeComponent.Target = target;
		xenoActiveLungeComponent.TargetCoordinates = _transform.ToMapCoordinates(targetCoords, true);
		xenoActiveLungeComponent.Range = xeno.Comp.Range;
		xenoActiveLungeComponent.StunTime = xeno.Comp.StunTime;
		((EntitySystem)this).Dirty<XenoLungeComponent>(xeno, (MetaDataComponent)null);
		_rmcObstacleSlamming.MakeImmune(Entity<XenoLungeComponent>.op_Implicit(xeno), 0.5f);
		_throwing.TryThrow(Entity<XenoLungeComponent>.op_Implicit(xeno), diff, 30f, null, 2f, null, compensateFriction: false, recoil: true, animated: false);
		PhysicsComponent physics = default(PhysicsComponent);
		if (!_physicsQuery.TryGetComponent(Entity<XenoLungeComponent>.op_Implicit(xeno), ref physics))
		{
			return;
		}
		foreach (EntityUid ent in _physics.GetContactingEntities(xeno.Owner, physics, false))
		{
			if (!(ent != target) && ApplyLungeHitEffects(Entity<XenoActiveLungeComponent>.op_Implicit(xeno.Owner), ent, stopThrow: true))
			{
				break;
			}
		}
	}

	private void OnAttackAttempt(Entity<XenoLungeComponent> ent, ref MeleeAttackAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netAttacker = ((EntitySystem)this).GetNetEntity(Entity<XenoLungeComponent>.op_Implicit(ent), (MetaDataComponent)null);
		XenoLungeStunnedComponent stunned = default(XenoLungeStunnedComponent);
		if (((EntitySystem)this).TryComp<XenoLungeStunnedComponent>(((EntitySystem)this).GetEntity(args.Target), ref stunned))
		{
			NetEntity val = netAttacker;
			NetEntity? stunner = stunned.Stunner;
			if (stunner.HasValue && !(val != stunner.GetValueOrDefault()) && args.Attack is DisarmAttackEvent disarm)
			{
				args.Attack = new LightAttackEvent(disarm.Target, netAttacker, disarm.Coordinates);
			}
		}
	}

	private void OnXenoLungeHit(Entity<XenoActiveLungeComponent> xeno, ref ThrowDoHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsAlive(Entity<XenoActiveLungeComponent>.op_Implicit(xeno)) || ((EntitySystem)this).HasComp<StunnedComponent>(Entity<XenoActiveLungeComponent>.op_Implicit(xeno)))
		{
			((EntitySystem)this).RemCompDeferred<XenoActiveLungeComponent>(Entity<XenoActiveLungeComponent>.op_Implicit(xeno));
		}
		else
		{
			ApplyLungeHitEffects(xeno.AsNullable(), args.Target, stopThrow: true);
		}
	}

	private void OnXenoLungeLand(Entity<XenoActiveLungeComponent> ent, ref LandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!_pulling.IsPulling(Entity<XenoActiveLungeComponent>.op_Implicit(ent)))
		{
			ApplyLungeHitEffects(ent.AsNullable(), ent.Comp.Target, stopThrow: false);
		}
		((EntitySystem)this).RemCompDeferred<XenoActiveLungeComponent>(Entity<XenoActiveLungeComponent>.op_Implicit(ent));
	}

	private bool ApplyLungeHitEffects(Entity<XenoActiveLungeComponent?> xeno, EntityUid targetId, bool stopThrow, bool predicted = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoActiveLungeComponent>(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return false;
		}
		if (_mobState.IsDead(targetId))
		{
			return false;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		ThrownItemComponent thrown = default(ThrownItemComponent);
		if (_physicsQuery.TryGetComponent(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), ref physics) && _thrownItemQuery.TryGetComponent(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), ref thrown))
		{
			_thrownItem.LandComponent(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), thrown, physics, playSound: true);
			if (stopThrow)
			{
				_thrownItem.StopThrow(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), thrown);
			}
		}
		XenoLungeHitAttempt ev = new XenoLungeHitAttempt(Entity<XenoActiveLungeComponent>.op_Implicit(xeno));
		((EntitySystem)this).RaiseLocalEvent<XenoLungeHitAttempt>(targetId, ref ev, false);
		if (ev.Cancelled)
		{
			return true;
		}
		XenoComponent xenoComp = default(XenoComponent);
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), targetId) || (_size.TryGetSize(targetId, out var size) && (int)size >= 5) || (((EntitySystem)this).TryComp<XenoComponent>(targetId, ref xenoComp) && xenoComp.Tier >= 2))
		{
			return true;
		}
		if (_net.IsServer)
		{
			TimeSpan stunTime = _xeno.TryApplyXenoDebuffMultiplier(targetId, xeno.Comp.StunTime);
			_stun.TryParalyze(targetId, stunTime, refresh: true);
			XenoLungeStunnedComponent stunned = ((EntitySystem)this).EnsureComp<XenoLungeStunnedComponent>(targetId);
			stunned.ExpireAt = _timing.CurTime + stunTime;
			stunned.Stunner = ((EntitySystem)this).GetNetEntity(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), (MetaDataComponent)null);
			((EntitySystem)this).Dirty(targetId, (IComponent)(object)stunned, (MetaDataComponent)null);
		}
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), ref melee))
		{
			melee.NextAttack = _timing.CurTime;
			((EntitySystem)this).Dirty(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), (IComponent)(object)melee, (MetaDataComponent)null);
		}
		if (_net.IsClient && predicted)
		{
			XenoLungePredictedHitEvent predictedEv = new XenoLungePredictedHitEvent(((EntitySystem)this).GetNetEntity(targetId, (MetaDataComponent)null), _rmcLagCompensation.GetLastRealTick(null));
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)predictedEv);
			if (_timing.InPrediction && _timing.IsFirstTimePredicted)
			{
				((EntitySystem)this).RaisePredictiveEvent<XenoLungePredictedHitEvent>(predictedEv);
			}
		}
		StopLunge(Entity<XenoActiveLungeComponent>.op_Implicit(xeno));
		_transform.SetMapCoordinates(targetId, xeno.Comp.TargetCoordinates);
		MapCoordinates coordinates = _transform.GetMapCoordinates(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), (TransformComponent)null);
		if (xeno.Comp.TargetCoordinates.MapId == coordinates.MapId && !((MapCoordinates)(ref xeno.Comp.TargetCoordinates)).InRange(coordinates, 1.25f))
		{
			Vector2 distance = xeno.Comp.TargetCoordinates.Position - coordinates.Position;
			float length = distance.Length();
			MapCoordinates newPosition = ((MapCoordinates)(ref coordinates)).Offset((float)((double)length - 1.25) / length * distance);
			_transform.SetMapCoordinates(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), newPosition);
		}
		_pulling.TryStartPull(Entity<XenoActiveLungeComponent>.op_Implicit(xeno), targetId);
		((EntitySystem)this).RemCompDeferred<XenoActiveLungeComponent>(Entity<XenoActiveLungeComponent>.op_Implicit(xeno));
		return true;
	}

	private void OnXenoLungeStunnedPullStopped(Entity<XenoLungeStunnedComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.PulledUid != ent.Owner))
		{
			ProtoId<StatusEffectPrototype>[] effects = ent.Comp.Effects;
			foreach (ProtoId<StatusEffectPrototype> effect in effects)
			{
				_statusEffects.TryRemoveStatusEffect(Entity<XenoLungeStunnedComponent>.op_Implicit(ent), ProtoId<StatusEffectPrototype>.op_Implicit(effect));
			}
			((EntitySystem)this).RemCompDeferred<XenoLungeStunnedComponent>(ent.Owner);
		}
	}

	private void OnXenoLungeHitAttempt(Entity<RMCLungeProtectionComponent> ent, ref XenoLungeHitAttempt args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		XenoActiveLungeComponent lunging = default(XenoActiveLungeComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<XenoActiveLungeComponent>(args.Lunging, ref lunging))
		{
			args.Cancelled = _leap.AttemptBlockLeap(ent.Owner, ent.Comp.StunDuration, ent.Comp.BlockSound, args.Lunging, _transform.ToCoordinates(lunging.Origin), ent.Comp.FullProtection);
		}
	}

	private void StopLunge(EntityUid lunging)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (_physicsQuery.TryGetComponent(lunging, ref physics))
		{
			_physics.SetLinearVelocity(lunging, Vector2.Zero, true, true, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(lunging, physics, (BodyStatus)0, true);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoLungeStunnedComponent> stunnedQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoLungeStunnedComponent>();
		EntityUid uid = default(EntityUid);
		XenoLungeStunnedComponent stunned = default(XenoLungeStunnedComponent);
		while (stunnedQuery.MoveNext(ref uid, ref stunned))
		{
			if (!(time < stunned.ExpireAt))
			{
				((EntitySystem)this).RemCompDeferred<XenoLungeStunnedComponent>(uid);
			}
		}
	}
}
