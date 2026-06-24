using System;
using System.Linq;
using Content.Shared._RMC14.Throwing;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.Movement.Pulling.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Throwing;

public sealed class ThrownItemSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedBroadphaseSystem _broadphase;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedGravitySystem _gravity;

	private const string ThrowingFixture = "throw-fixture";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, MapInitEvent>((ComponentEventHandler<ThrownItemComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, PhysicsSleepEvent>((ComponentEventRefHandler<ThrownItemComponent, PhysicsSleepEvent>)OnSleep, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, StartCollideEvent>((ComponentEventRefHandler<ThrownItemComponent, StartCollideEvent>)HandleCollision, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, PreventCollideEvent>((ComponentEventRefHandler<ThrownItemComponent, PreventCollideEvent>)PreventCollision, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, ThrownEvent>((ComponentEventRefHandler<ThrownItemComponent, ThrownEvent>)ThrowItem, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullStartedMessage>((EntityEventHandler<PullStartedMessage>)HandlePullStarted, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, ThrownItemComponent component, MapInitEvent args)
	{
		TimeSpan valueOrDefault = component.ThrownTime.GetValueOrDefault();
		if (!component.ThrownTime.HasValue)
		{
			valueOrDefault = _gameTiming.CurTime;
			component.ThrownTime = valueOrDefault;
		}
	}

	private void ThrowItem(EntityUid uid, ThrownItemComponent component, ref ThrownEvent @event)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixturesComponent = default(FixturesComponent);
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixturesComponent) && fixturesComponent.Fixtures.Count == 1 && ((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref body))
		{
			IPhysShape shape = fixturesComponent.Fixtures.Values.First().Shape;
			_fixtures.TryCreateFixture(uid, shape, "throw-fixture", 1f, false, 0, 74, 0.4f, 0f, true, fixturesComponent, body, (TransformComponent)null);
		}
	}

	private void HandleCollision(EntityUid uid, ThrownItemComponent component, ref StartCollideEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (args.OtherFixture.Hard)
		{
			EntityUid otherEntity = args.OtherEntity;
			EntityUid? thrower = component.Thrower;
			if (!thrower.HasValue || !(otherEntity == thrower.GetValueOrDefault()))
			{
				ThrowCollideInteraction(component, args.OurEntity, args.OtherEntity);
			}
		}
	}

	private void PreventCollision(EntityUid uid, ThrownItemComponent component, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ThrownHitUserComponent>(uid))
		{
			EntityUid otherEntity = args.OtherEntity;
			EntityUid? thrower = component.Thrower;
			if (thrower.HasValue && otherEntity == thrower.GetValueOrDefault())
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnSleep(EntityUid uid, ThrownItemComponent thrownItem, ref PhysicsSleepEvent @event)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopThrow(uid, thrownItem);
	}

	private void HandlePullStarted(PullStartedMessage message)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ThrownItemComponent thrownItemComponent = default(ThrownItemComponent);
		if (((EntitySystem)this).TryComp<ThrownItemComponent>(message.PulledUid, ref thrownItemComponent))
		{
			StopThrow(message.PulledUid, thrownItemComponent);
		}
	}

	public void StopThrow(EntityUid uid, ThrownItemComponent thrownItemComponent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref physics))
		{
			_physics.SetBodyStatus(uid, physics, (BodyStatus)0, true);
			if (physics.Awake)
			{
				_broadphase.RegenerateContacts(Entity<PhysicsComponent, FixturesComponent, TransformComponent>.op_Implicit((uid, physics)));
			}
		}
		FixturesComponent manager = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(uid, ref manager))
		{
			Fixture fixture = _fixtures.GetFixtureOrNull(uid, "throw-fixture", manager);
			if (fixture != null)
			{
				_fixtures.DestroyFixture(uid, "throw-fixture", fixture, true, (PhysicsComponent)null, manager, (TransformComponent)null);
			}
		}
		StopThrowEvent ev = new StopThrowEvent(thrownItemComponent.Thrower);
		((EntitySystem)this).RaiseLocalEvent<StopThrowEvent>(uid, ref ev, false);
		((EntitySystem)this).RemComp<ThrownItemComponent>(uid);
	}

	public void LandComponent(EntityUid uid, ThrownItemComponent thrownItem, PhysicsComponent physics, bool playSound)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (!thrownItem.Landed && !((Component)thrownItem).Deleted && !_gravity.IsWeightless(uid) && !((EntitySystem)this).Deleted(uid, (MetaDataComponent)null))
		{
			thrownItem.Landed = true;
			EntityUid? thrower = thrownItem.Thrower;
			if (thrower.HasValue)
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(19, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
				handler.AppendLiteral(" thrown by ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(thrownItem.Thrower.Value)), "thrower", "ToPrettyString(thrownItem.Thrower.Value)");
				handler.AppendLiteral(" landed.");
				adminLogger.Add(LogType.Landed, LogImpact.Low, ref handler);
			}
			_broadphase.RegenerateContacts(Entity<PhysicsComponent, FixturesComponent, TransformComponent>.op_Implicit((uid, physics)));
			LandEvent landEvent = new LandEvent(thrownItem.Thrower, playSound);
			((EntitySystem)this).RaiseLocalEvent<LandEvent>(uid, ref landEvent, false);
		}
	}

	public void ThrowCollideInteraction(ThrownItemComponent component, EntityUid thrown, EntityUid target)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? thrower = component.Thrower;
		if (thrower.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(17, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(thrown)), "thrown", "ToPrettyString(thrown)");
			handler.AppendLiteral(" thrown by ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(component.Thrower.Value)), "thrower", "ToPrettyString(component.Thrower.Value)");
			handler.AppendLiteral(" hit ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(".");
			adminLogger.Add(LogType.ThrowHit, LogImpact.Low, ref handler);
		}
		((EntitySystem)this).RaiseLocalEvent<ThrowHitByEvent>(target, new ThrowHitByEvent(thrown, target, component), true);
		((EntitySystem)this).RaiseLocalEvent<ThrowDoHitEvent>(thrown, new ThrowDoHitEvent(thrown, target, component), true);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ThrownItemComponent, PhysicsComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ThrownItemComponent, PhysicsComponent>();
		EntityUid uid = default(EntityUid);
		ThrownItemComponent thrown = default(ThrownItemComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		while (query.MoveNext(ref uid, ref thrown, ref physics))
		{
			if (!_netMan.IsClient || physics.Predict)
			{
				if (thrown.LandTime <= _gameTiming.CurTime)
				{
					LandComponent(uid, thrown, physics, thrown.PlayLandSound);
				}
				if ((thrown.LandTime ?? thrown.ThrownTime) <= _gameTiming.CurTime)
				{
					StopThrow(uid, thrown);
				}
			}
		}
	}
}
