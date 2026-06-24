using System;
using System.Numerics;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Tumble;

public sealed class XenoTumbleSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCObstacleSlammingSystem _rmcObstacleSlamming;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private ThrownItemSystem _thrownItem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

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
		((EntitySystem)this).SubscribeLocalEvent<XenoTumbleComponent, XenoTumbleActionEvent>((EntityEventRefHandler<XenoTumbleComponent, XenoTumbleActionEvent>)OnXenoTumbleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTumbleComponent, ThrowDoHitEvent>((EntityEventRefHandler<XenoTumbleComponent, ThrowDoHitEvent>)OnXenoTumbleHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTumbleComponent, LandEvent>((EntityEventRefHandler<XenoTumbleComponent, LandEvent>)OnXenoTumbleLand, (Type[])null, (Type[])null);
	}

	private void OnXenoTumbleAction(Entity<XenoTumbleComponent> xeno, ref XenoTumbleActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoTumbleComponent>.op_Implicit(xeno), (TransformComponent)null);
		Vector2 diff = _transform.ToMapCoordinates(args.Target, true).Position - origin.Position;
		diff = Vector2Helpers.Normalized(diff) * xeno.Comp.Range;
		Direction dir = DirectionExtensions.GetDir(diff);
		Angle worldRotation = _transform.GetWorldRotation(Entity<XenoTumbleComponent>.op_Implicit(xeno));
		(Direction, Direction) perpendiculars = ((Angle)(ref worldRotation)).GetCardinalDir().GetPerpendiculars();
		Direction towards;
		if (dir == perpendiculars.Item1)
		{
			(towards, _) = perpendiculars;
		}
		else
		{
			if (dir != perpendiculars.Item2)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-tumble-not-perpendicular"), args.Target, Entity<XenoTumbleComponent>.op_Implicit(xeno), PopupType.LargeCaution);
				return;
			}
			towards = perpendiculars.Item2;
		}
		diff = DirectionExtensions.ToVec(towards) * xeno.Comp.Range;
		((HandledEntityEventArgs)args).Handled = true;
		_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoTumbleComponent>.op_Implicit(xeno));
		xeno.Comp.Target = diff;
		((EntitySystem)this).Dirty<XenoTumbleComponent>(xeno, (MetaDataComponent)null);
		_rmcObstacleSlamming.MakeImmune(Entity<XenoTumbleComponent>.op_Implicit(xeno));
		_throwing.TryThrow(Entity<XenoTumbleComponent>.op_Implicit(xeno), diff, 30f, null, 2f, null, compensateFriction: false, recoil: true, animated: false);
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoTumbleComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoTumbleComponent>.op_Implicit(xeno), (AudioParams?)null);
	}

	private void OnXenoTumbleHit(Entity<XenoTumbleComponent> xeno, ref ThrowDoHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsAlive(Entity<XenoTumbleComponent>.op_Implicit(xeno)) || ((EntitySystem)this).HasComp<StunnedComponent>(Entity<XenoTumbleComponent>.op_Implicit(xeno)))
		{
			xeno.Comp.Target = null;
			((EntitySystem)this).Dirty<XenoTumbleComponent>(xeno, (MetaDataComponent)null);
		}
		else
		{
			if (_mobState.IsDead(args.Target))
			{
				return;
			}
			PhysicsComponent physics = default(PhysicsComponent);
			ThrownItemComponent thrown = default(ThrownItemComponent);
			if (_physicsQuery.TryGetComponent(Entity<XenoTumbleComponent>.op_Implicit(xeno), ref physics) && _thrownItemQuery.TryGetComponent(Entity<XenoTumbleComponent>.op_Implicit(xeno), ref thrown))
			{
				_thrownItem.LandComponent(Entity<XenoTumbleComponent>.op_Implicit(xeno), thrown, physics, playSound: true);
				_thrownItem.StopThrow(Entity<XenoTumbleComponent>.op_Implicit(xeno), thrown);
			}
			if (_timing.IsFirstTimePredicted && xeno.Comp.Target.HasValue)
			{
				xeno.Comp.Target = null;
			}
			if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(args.Target)))
			{
				if (_net.IsServer)
				{
					_stun.TryParalyze(args.Target, xeno.Comp.StunTime, refresh: true);
				}
				StopTumble(Entity<XenoTumbleComponent>.op_Implicit(xeno));
				MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoTumbleComponent>.op_Implicit(xeno), (TransformComponent)null);
				_sizeStun.KnockBack(args.Target, origin, xeno.Comp.ImpactRange, xeno.Comp.ImpactRange, 10f);
				_damageable.TryChangeDamage(args.Target, _xeno.TryApplyXenoSlashDamageMultiplier(args.Target, xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoTumbleComponent>.op_Implicit(xeno), Entity<XenoTumbleComponent>.op_Implicit(xeno), xeno.Comp.ArmorPiercing);
			}
		}
	}

	private void OnXenoTumbleLand(Entity<XenoTumbleComponent> xeno, ref LandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Target.HasValue)
		{
			xeno.Comp.Target = null;
			((EntitySystem)this).Dirty<XenoTumbleComponent>(xeno, (MetaDataComponent)null);
		}
	}

	private void StopTumble(EntityUid xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (_physicsQuery.TryGetComponent(xeno, ref physics))
		{
			_physics.SetLinearVelocity(xeno, Vector2.Zero, true, true, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(xeno, physics, (BodyStatus)0, true);
		}
	}
}
