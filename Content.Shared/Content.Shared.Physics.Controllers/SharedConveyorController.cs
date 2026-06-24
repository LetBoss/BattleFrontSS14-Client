using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Conveyor;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Threading;

namespace Content.Shared.Physics.Controllers;

public abstract class SharedConveyorController : VirtualController
{
	private record struct ConveyorJob : IParallelRobustJob, IParallelRangeRobustJob
	{
		public int BatchSize => 16;

		public List<(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> Entity, Vector2 Direction, bool Result)> Conveyed;

		public SharedConveyorController System;

		public bool Prediction;

		public ConveyorJob(SharedConveyorController controller)
		{
			Prediction = false;
			Conveyed = new List<(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>, Vector2, bool)>();
			System = controller;
		}

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>, Vector2, bool) convey = Conveyed[index];
			Vector2 direction;
			bool result = System.TryConvey(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>.op_Implicit((convey.Item1.Owner, convey.Item1.Comp1, convey.Item1.Comp2, convey.Item1.Comp3, convey.Item1.Comp4)), Prediction, out direction);
			Conveyed[index] = (convey.Item1, direction, result);
		}
	}

	[Dependency]
	protected IMapManager MapManager;

	[Dependency]
	private IParallelManager _parallel;

	[Dependency]
	private CollisionWakeSystem _wake;

	[Dependency]
	protected EntityLookupSystem Lookup;

	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedMoverController _mover;

	protected const string ConveyorFixture = "conveyor";

	private ConveyorJob _job;

	private EntityQuery<ConveyorComponent> _conveyorQuery;

	private EntityQuery<ConveyedComponent> _conveyedQuery;

	protected EntityQuery<PhysicsComponent> PhysicsQuery;

	protected EntityQuery<TransformComponent> XformQuery;

	protected HashSet<EntityUid> Intersecting = new HashSet<EntityUid>();

	public override void Initialize()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		_job = new ConveyorJob(this);
		_conveyorQuery = ((EntitySystem)this).GetEntityQuery<ConveyorComponent>();
		_conveyedQuery = ((EntitySystem)this).GetEntityQuery<ConveyedComponent>();
		PhysicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		XformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedMoverController));
		((EntitySystem)this).SubscribeLocalEvent<ConveyedComponent, TileFrictionEvent>((EntityEventRefHandler<ConveyedComponent, TileFrictionEvent>)OnConveyedFriction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConveyedComponent, ComponentStartup>((EntityEventRefHandler<ConveyedComponent, ComponentStartup>)OnConveyedStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConveyedComponent, ComponentShutdown>((EntityEventRefHandler<ConveyedComponent, ComponentShutdown>)OnConveyedShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConveyorComponent, StartCollideEvent>((EntityEventRefHandler<ConveyorComponent, StartCollideEvent>)OnConveyorStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConveyorComponent, ComponentStartup>((EntityEventRefHandler<ConveyorComponent, ComponentStartup>)OnConveyorStartup, (Type[])null, (Type[])null);
		((VirtualController)this).Initialize();
	}

	private void OnConveyedFriction(Entity<ConveyedComponent> ent, ref TileFrictionEvent args)
	{
		args.Modifier = 0f;
	}

	private void OnConveyedStartup(Entity<ConveyedComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_wake.SetEnabled(ent.Owner, false, (CollisionWakeComponent)null);
	}

	private void OnConveyedShutdown(Entity<ConveyedComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_wake.SetEnabled(ent.Owner, true, (CollisionWakeComponent)null);
	}

	private void OnConveyorStartup(Entity<ConveyorComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AwakenConveyor(Entity<TransformComponent>.op_Implicit(ent.Owner));
	}

	protected virtual void AwakenConveyor(Entity<TransformComponent?> ent)
	{
	}

	protected void WakeConveyed(EntityUid conveyorUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ContactEnumerator contacts = base.PhysicsSystem.GetContacts(Entity<FixturesComponent>.op_Implicit(conveyorUid), false);
		Contact contact = default(Contact);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact))
		{
			EntityUid other = contact.OtherEnt(conveyorUid);
			if (contact.OtherFixture(conveyorUid).Item2.Hard && (int)contact.OtherBody(conveyorUid).BodyType != 4)
			{
				((EntitySystem)this).EnsureComp<ConveyedComponent>(other);
			}
			if (_conveyedQuery.HasComp(other))
			{
				base.PhysicsSystem.WakeBody(other, false, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
	}

	private void OnConveyorStartCollide(Entity<ConveyorComponent> conveyor, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		if (args.OtherFixture.Hard && (int)args.OtherBody.BodyType != 4)
		{
			((EntitySystem)this).EnsureComp<ConveyedComponent>(otherUid);
		}
	}

	public override void UpdateBeforeSolve(bool prediction, float frameTime)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).UpdateBeforeSolve(prediction, frameTime);
		_job.Prediction = prediction;
		_job.Conveyed.Clear();
		EntityQueryEnumerator<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		ConveyedComponent comp = default(ConveyedComponent);
		FixturesComponent fixtures = default(FixturesComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref comp, ref fixtures, ref physics, ref xform))
		{
			_job.Conveyed.Add((Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>.op_Implicit((uid, comp, fixtures, physics, xform)), Vector2.Zero, false));
		}
		_parallel.ProcessNow((IParallelRobustJob)(object)_job, _job.Conveyed.Count);
		foreach (var ent in _job.Conveyed)
		{
			if (!ent.Entity.Comp3.Predict && prediction)
			{
				continue;
			}
			Vector2 velocity = ent.Entity.Comp3.LinearVelocity;
			Vector2 targetDir = ent.Direction;
			Vector2 wishDir = _mover.GetWishDir(Entity<InputMoverComponent>.op_Implicit(ent.Entity.Owner));
			if (Vector2.Dot(wishDir, targetDir) > 0f)
			{
				targetDir += wishDir;
			}
			bool usedMob2;
			if (ent.Result)
			{
				SetConveying(ent.Entity.Owner, ent.Entity.Comp1, targetDir.LengthSquared() > 0f);
				if (!_mover.UsedMobMovement.TryGetValue(ent.Entity.Owner, out var usedMob) || !usedMob)
				{
					_mover.Friction(0.2f, frameTime, 5f, ref velocity);
				}
				SharedMoverController.Accelerate(ref velocity, in targetDir, 20f, frameTime);
			}
			else if (!_mover.UsedMobMovement.TryGetValue(ent.Entity.Owner, out usedMob2) || !usedMob2)
			{
				_mover.Friction(0f, frameTime, 40f, ref velocity);
			}
			base.PhysicsSystem.SetLinearVelocity(ent.Entity.Owner, velocity, true, false, (FixturesComponent)null, (PhysicsComponent)null);
			if (!IsConveyed(Entity<FixturesComponent>.op_Implicit((ent.Entity.Owner, ent.Entity.Comp2))))
			{
				((EntitySystem)this).RemComp<ConveyedComponent>(ent.Entity.Owner);
			}
		}
	}

	private void SetConveying(EntityUid uid, ConveyedComponent conveyed, bool value)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (conveyed.Conveying != value)
		{
			conveyed.Conveying = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)conveyed, (MetaDataComponent)null);
		}
	}

	private bool TryConvey(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> entity, bool prediction, out Vector2 direction)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Invalid comparison between Unknown and I4
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		direction = Vector2.Zero;
		FixturesComponent fixtures = entity.Comp2;
		PhysicsComponent physics = entity.Comp3;
		TransformComponent xform = entity.Comp4;
		if (!physics.Awake)
		{
			return true;
		}
		if (!physics.Predict && prediction)
		{
			return true;
		}
		if (!xform.GridUid.HasValue)
		{
			return true;
		}
		if ((int)physics.BodyStatus == 1 || _gravity.IsWeightless(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>.op_Implicit(entity), physics, xform))
		{
			return true;
		}
		Entity<ConveyorComponent> bestConveyor = default(Entity<ConveyorComponent>);
		float bestSpeed = 0f;
		ContactEnumerator contacts = base.PhysicsSystem.GetContacts(Entity<FixturesComponent>.op_Implicit((entity.Owner, fixtures)), false);
		Transform transform = base.PhysicsSystem.GetPhysicsTransform(entity.Owner, (TransformComponent)null);
		bool anyConveyors = false;
		Contact contact = default(Contact);
		ConveyorComponent conveyor = default(ConveyorComponent);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact))
		{
			if (!contact.IsTouching)
			{
				continue;
			}
			EntityUid other = contact.OtherEnt(entity.Owner);
			if (_conveyorQuery.TryComp(other, ref conveyor))
			{
				anyConveyors = true;
				(string, Fixture) otherFixture = contact.OtherFixture(entity.Owner);
				Transform otherTransform = base.PhysicsSystem.GetPhysicsTransform(other, (TransformComponent)null);
				if (_fixtures.TestPoint<IPhysShape>(otherFixture.Item2.Shape, otherTransform, transform.Position) && conveyor.Speed > bestSpeed && CanRun(conveyor))
				{
					bestSpeed = conveyor.Speed;
					bestConveyor = Entity<ConveyorComponent>.op_Implicit((other, conveyor));
				}
			}
		}
		if (!anyConveyors)
		{
			return true;
		}
		if (bestSpeed == 0f || bestConveyor == default(Entity<ConveyorComponent>))
		{
			return true;
		}
		ConveyorComponent comp = bestConveyor.Comp;
		TransformComponent conveyorXform = XformQuery.GetComponent(bestConveyor.Owner);
		ValueTuple<Vector2, Angle> worldPositionRotation = base.TransformSystem.GetWorldPositionRotation(conveyorXform);
		Vector2 conveyorPos = worldPositionRotation.Item1;
		Angle conveyorRot = worldPositionRotation.Item2;
		conveyorRot += bestConveyor.Comp.Angle;
		if (comp.State == ConveyorState.Reverse)
		{
			conveyorRot += Angle.op_Implicit((float)Math.PI);
		}
		Vector2 conveyorDirection = ((Angle)(ref conveyorRot)).ToWorldVec();
		direction = conveyorDirection;
		Vector2 itemRelative = conveyorPos - transform.Position;
		direction = Convey(direction, bestSpeed, itemRelative);
		contacts = base.PhysicsSystem.GetContacts(Entity<FixturesComponent>.op_Implicit((entity.Owner, fixtures)), false);
		Contact contact2 = default(Contact);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact2))
		{
			if (contact2.Hard && contact2.IsTouching)
			{
				EntityUid other2 = contact2.OtherEnt(entity.Owner);
				if ((int)contact2.OtherBody(entity.Owner).BodyType == 4 && Vector2.Dot(base.PhysicsSystem.GetPhysicsTransform(other2, (TransformComponent)null).Position - transform.Position, direction) > 1.5f)
				{
					direction = Vector2.Zero;
					return false;
				}
			}
		}
		return true;
	}

	private static Vector2 Convey(Vector2 direction, float speed, Vector2 itemRelative)
	{
		if (speed == 0f || direction.LengthSquared() == 0f)
		{
			return Vector2.Zero;
		}
		Vector2 p = direction * (Vector2.Dot(itemRelative, direction) / Vector2.Dot(direction, direction));
		Vector2 r = itemRelative - p;
		if ((double)r.Length() < 0.01)
		{
			return direction * speed;
		}
		return Vector2Helpers.Normalized(r + direction * 0.2f) * speed;
	}

	public bool CanRun(ConveyorComponent component)
	{
		if (component.State != ConveyorState.Off)
		{
			return component.Powered;
		}
		return false;
	}

	private bool IsConveyed(Entity<FixturesComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FixturesComponent>(ent.Owner, ref ent.Comp, true))
		{
			return false;
		}
		ContactEnumerator contacts = base.PhysicsSystem.GetContacts(Entity<FixturesComponent>.op_Implicit(ent.Owner), false);
		Contact contact = default(Contact);
		ConveyorComponent comp = default(ConveyorComponent);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact))
		{
			if (contact.IsTouching)
			{
				EntityUid other = contact.OtherEnt(ent.Owner);
				if (_conveyorQuery.TryComp(other, ref comp) && CanRun(comp))
				{
					return true;
				}
			}
		}
		return false;
	}
}
