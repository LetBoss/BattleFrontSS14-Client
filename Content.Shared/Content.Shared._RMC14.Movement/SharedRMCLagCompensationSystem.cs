using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Coordinates;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Movement;

public abstract class SharedRMCLagCompensationSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<ActorComponent> _actorQuery;

	private readonly Dictionary<NetUserId, GameTick> _lastRealTicks = new Dictionary<NetUserId, GameTick>();

	private GameTick _lastSentRealTick;

	public float MarginTiles { get; private set; }

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_actorQuery = ((EntitySystem)this).GetEntityQuery<ActorComponent>();
		((EntitySystem)this).SubscribeNetworkEvent<RMCSetLastRealTickEvent>((EntitySessionEventHandler<RMCSetLastRealTickEvent>)OnSetLastRealTick, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCLagCompensationMarginTiles, (Action<float>)delegate(float v)
		{
			MarginTiles = v;
		}, true);
	}

	private void OnSetLastRealTick(RMCSetLastRealTickEvent msg, EntitySessionEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		SetLastRealTick(((EntitySessionEventArgs)(ref args)).SenderSession.UserId, msg.Tick - 1u);
	}

	public virtual (EntityCoordinates Coordinates, Angle Angle) GetCoordinatesAngle(EntityUid uid, ICommonSession? pSession, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(uid, ref xform, true))
		{
			return (Coordinates: EntityCoordinates.Invalid, Angle: Angle.Zero);
		}
		return (Coordinates: xform.Coordinates, Angle: xform.LocalRotation);
	}

	public virtual Angle GetAngle(EntityUid uid, ICommonSession? session, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetCoordinatesAngle(uid, session, xform).Angle;
	}

	public virtual EntityCoordinates GetCoordinates(EntityUid uid, ICommonSession? session, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetCoordinatesAngle(uid, session, xform).Coordinates;
	}

	public EntityCoordinates GetCoordinates(EntityUid uid, EntityUid? session, TransformComponent? xform = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (!_actorQuery.TryComp(session, ref actor))
		{
			return GetCoordinates(uid, (ICommonSession?)null, xform);
		}
		return GetCoordinates(uid, actor.PlayerSession, xform);
	}

	public bool IsWithinMargin(Entity<TransformComponent?> sessionEnt, Entity<TransformComponent?> lagCompensatedTarget, ICommonSession? session, float range)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates targetCoords = GetCoordinates(Entity<TransformComponent>.op_Implicit(lagCompensatedTarget), session);
		if (_net.IsServer)
		{
			EntityCoordinates targetCurrentCoords = lagCompensatedTarget.Owner.ToCoordinates();
			if (!_transform.InRange(targetCoords, targetCurrentCoords, 0.01f))
			{
				range += MarginTiles;
			}
		}
		return _transform.InRange(sessionEnt.Owner.ToCoordinates(), targetCoords, range);
	}

	public virtual GameTick GetLastRealTick(NetUserId? session)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (session.HasValue)
		{
			return _lastRealTicks.GetValueOrDefault(session.Value, _timing.CurTick);
		}
		return _timing.CurTick;
	}

	public void SetLastRealTick(NetUserId session, GameTick tick)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && (!_lastRealTicks.TryGetValue(session, out var currentTick) || !(tick <= currentTick)))
		{
			_lastRealTicks[session] = tick;
		}
	}

	public void SendLastRealTick()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsServer)
		{
			GameTick tick = GetLastRealTick(null);
			if (!(tick == _lastSentRealTick))
			{
				_lastSentRealTick = tick;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCSetLastRealTickEvent(tick));
			}
		}
	}

	public bool Collides(Entity<FixturesComponent?> target, Entity<PhysicsComponent?> projectile, MapCoordinates targetCoordinates, Angle targetAngle = default(Angle))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FixturesComponent>(Entity<FixturesComponent>.op_Implicit(target), ref target.Comp, false) || !((EntitySystem)this).Resolve<PhysicsComponent>(Entity<PhysicsComponent>.op_Implicit(projectile), ref projectile.Comp, false))
		{
			return false;
		}
		MapCoordinates projectileCoordinates = _transform.GetMapCoordinates(Entity<PhysicsComponent>.op_Implicit(projectile), (TransformComponent)null);
		Vector2 projectilePosition = projectileCoordinates.Position;
		Transform transform = default(Transform);
		((Transform)(ref transform))._002Ector(targetCoordinates.Position, targetAngle);
		Box2 bounds = default(Box2);
		((Box2)(ref bounds))._002Ector(transform.Position, transform.Position);
		foreach (Fixture fixture in target.Comp.Fixtures.Values)
		{
			if ((fixture.CollisionLayer & projectile.Comp.CollisionMask) != 0)
			{
				for (int i = 0; i < fixture.Shape.ChildCount; i++)
				{
					Box2 boundy = fixture.Shape.ComputeAABB(transform, i);
					bounds = ((Box2)(ref bounds)).Union(ref boundy);
				}
			}
		}
		if (((Box2)(ref bounds)).Contains(projectilePosition, true))
		{
			return true;
		}
		Vector2 projectileVelocity = _physics.GetLinearVelocity(Entity<PhysicsComponent>.op_Implicit(projectile), projectile.Comp.LocalCenter, (PhysicsComponent)null, (TransformComponent)null);
		Vector2 nextProjectilePosition = projectileCoordinates.Position + projectileVelocity / (int)_timing.TickRate / 1.5f;
		if (((Box2)(ref bounds)).Contains(nextProjectilePosition, true))
		{
			return true;
		}
		Vector2 min = Vector2.Min(projectilePosition, nextProjectilePosition);
		Vector2 max = Vector2.Max(projectilePosition, nextProjectilePosition);
		Box2 swept = default(Box2);
		((Box2)(ref swept))._002Ector(min, max);
		if (((Box2)(ref bounds)).Intersects(ref swept))
		{
			return true;
		}
		if ((((Box2)(ref bounds)).ClosestPoint(ref nextProjectilePosition) - nextProjectilePosition).LengthSquared() <= MarginTiles * MarginTiles)
		{
			return true;
		}
		return false;
	}

	public bool Collides(Entity<FixturesComponent?> target, Entity<PhysicsComponent?> projectile, ICommonSession session)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		var (coordinates, angle) = GetCoordinatesAngle(Entity<FixturesComponent>.op_Implicit(target), session);
		return Collides(target, projectile, _transform.ToMapCoordinates(coordinates, true), angle);
	}
}
