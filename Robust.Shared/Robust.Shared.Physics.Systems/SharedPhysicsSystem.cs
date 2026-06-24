using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Prometheus;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Events;
using Robust.Shared.Threading;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Systems;

public abstract class SharedPhysicsSystem : EntitySystem
{
	private sealed class ContactPoolPolicy : IPooledObjectPolicy<Contact>
	{
		private readonly SharedDebugPhysicsSystem _debugPhysicsSystem;

		private readonly IManifoldManager _manifoldManager;

		public ContactPoolPolicy(SharedDebugPhysicsSystem debugPhysicsSystem, IManifoldManager manifoldManager)
		{
			_debugPhysicsSystem = debugPhysicsSystem;
			_manifoldManager = manifoldManager;
		}

		public Contact Create()
		{
			return new Contact(_manifoldManager)
			{
				Manifold = default(Manifold)
			};
		}

		public bool Return(Contact obj)
		{
			SetContact(obj, enabled: false, new Entity<PhysicsComponent, TransformComponent>(EntityUid.Invalid, null, null), new Entity<PhysicsComponent, TransformComponent>(EntityUid.Invalid, null, null), string.Empty, string.Empty, null, 0, null, 0);
			return true;
		}
	}

	private record struct ManifoldsJob : IParallelRobustJob, IParallelRangeRobustJob
	{
		public int BatchSize => 32;

		public SharedPhysicsSystem Physics;

		public Contact[] Contacts;

		public ContactStatus[] Status;

		public FixedArray4<Vector2>[] WorldPoints;

		public bool[] Wake;

		public void Execute(int index)
		{
			Physics.UpdateContact(Contacts, index, Status, Wake, WorldPoints);
		}
	}

	internal record struct IslandData(int Index, bool LoneIsland, List<Entity<PhysicsComponent, TransformComponent>> Bodies, List<Contact> Contacts, List<(Joint Original, Joint Joint)> Joints, List<(Joint Joint, float Error)> BrokenJoints)
	{
		public EntityUid MapUid = default(EntityUid);

		public readonly int Index = Index;

		public readonly bool LoneIsland = LoneIsland;

		public int Offset = 0;

		public readonly List<Entity<PhysicsComponent, TransformComponent>> Bodies = Bodies;

		public readonly List<Contact> Contacts = Contacts;

		public readonly List<(Joint Original, Joint Joint)> Joints = Joints;

		public bool PositionSolved = false;

		public readonly List<(Joint Joint, float Error)> BrokenJoints = BrokenJoints;

		[CompilerGenerated]
		public readonly void Deconstruct(out int Index, out bool LoneIsland, out List<Entity<PhysicsComponent, TransformComponent>> Bodies, out List<Contact> Contacts, out List<(Joint Original, Joint Joint)> Joints, out List<(Joint Joint, float Error)> BrokenJoints)
		{
			Index = this.Index;
			LoneIsland = this.LoneIsland;
			Bodies = this.Bodies;
			Contacts = this.Contacts;
			Joints = this.Joints;
			BrokenJoints = this.BrokenJoints;
		}
	}

	private static Contact.ContactType[,] _registers = new Contact.ContactType[4, 4]
	{
		{
			Contact.ContactType.Circle,
			Contact.ContactType.EdgeAndCircle,
			Contact.ContactType.PolygonAndCircle,
			Contact.ContactType.ChainAndCircle
		},
		{
			Contact.ContactType.EdgeAndCircle,
			Contact.ContactType.NotSupported,
			Contact.ContactType.EdgeAndPolygon,
			Contact.ContactType.NotSupported
		},
		{
			Contact.ContactType.PolygonAndCircle,
			Contact.ContactType.EdgeAndPolygon,
			Contact.ContactType.Polygon,
			Contact.ContactType.ChainAndPolygon
		},
		{
			Contact.ContactType.ChainAndCircle,
			Contact.ContactType.NotSupported,
			Contact.ContactType.ChainAndPolygon,
			Contact.ContactType.NotSupported
		}
	};

	private const int ContactPoolInitialSize = 128;

	private const int ContactsPerThread = 32;

	private ObjectPool<Contact> _contactPool;

	private readonly LinkedList<Contact> _activeContacts = new LinkedList<Contact>();

	public static readonly Histogram TickUsageControllerBeforeSolveHistogram;

	public static readonly Histogram TickUsageControllerAfterSolveHistogram;

	[Robust.Shared.IoC.Dependency]
	private readonly IConfigurationManager _cfg;

	[Robust.Shared.IoC.Dependency]
	private readonly IManifoldManager _manifoldManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IParallelManager _parallel;

	[Robust.Shared.IoC.Dependency]
	private readonly EntityLookupSystem _lookup;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedBroadphaseSystem _broadphase;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedContainerSystem _containerSystem;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedDebugPhysicsSystem _debugPhysics;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedJointSystem _joints;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedTransformSystem _transform;

	[Robust.Shared.IoC.Dependency]
	private readonly CollisionWakeSystem _wakeSystem;

	private int _substeps;

	public TimeSpan? EffectiveCurTime;

	private EntityQuery<CollideOnAnchorComponent> _anchorQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private EntityQuery<JointComponent> JointQuery;

	private EntityQuery<JointRelayTargetComponent> RelayTargetQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	protected EntityQuery<MapComponent> MapQuery;

	protected EntityQuery<PhysicsComponent> PhysicsQuery;

	protected EntityQuery<TransformComponent> XformQuery;

	private ComponentRegistration _physicsReg;

	private byte _angularVelocityIndex;

	[Robust.Shared.IoC.Dependency]
	private readonly FixtureSystem _fixtures;

	private const int MaxIslands = 256;

	private readonly ObjectPool<List<Entity<PhysicsComponent, TransformComponent>>> _islandBodyPool = new DefaultObjectPool<List<Entity<PhysicsComponent, TransformComponent>>>(new ListPolicy<Entity<PhysicsComponent, TransformComponent>>(), 256);

	private readonly ObjectPool<List<Contact>> _islandContactPool = new DefaultObjectPool<List<Contact>>(new ListPolicy<Contact>(), 256);

	private readonly ObjectPool<List<(Joint Original, Joint Joint)>> _islandJointPool = new DefaultObjectPool<List<(Joint, Joint)>>(new ListPolicy<(Joint, Joint)>(), 256);

	private readonly HashSet<Entity<PhysicsComponent, TransformComponent>> _islandSet = new HashSet<Entity<PhysicsComponent, TransformComponent>>(64);

	private readonly Stack<Entity<PhysicsComponent, TransformComponent>> _bodyStack = new Stack<Entity<PhysicsComponent, TransformComponent>>(64);

	private readonly List<Entity<PhysicsComponent, TransformComponent>> _awakeBodyList = new List<Entity<PhysicsComponent, TransformComponent>>(256);

	private bool _warmStarting;

	private float _maxLinearCorrection;

	private float _maxAngularCorrection;

	private int _velocityIterations;

	private int _positionIterations;

	private float _maxLinearVelocity;

	private float _maxAngularVelocity;

	private float _maxTranslationPerTick;

	private float _maxRotationPerTick;

	private int _tickRate;

	private bool _sleepAllowed;

	protected float AngularToleranceSqr;

	protected float LinearToleranceSqr;

	protected float TimeToSleep;

	private float _velocityThreshold;

	private float _baumgarte;

	private const int VelocityConstraintsPerThread = 16;

	private const int PositionConstraintsPerThread = 16;

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _gameTiming;

	private bool _autoClearForces;

	protected readonly Dictionary<EntityUid, EntityUid> LerpData = new Dictionary<EntityUid, EntityUid>();

	[ViewVariables]
	internal readonly HashSet<FixtureProxy> MoveBuffer = new HashSet<FixtureProxy>();

	[ViewVariables]
	internal readonly HashSet<EntityUid> MovedGrids = new HashSet<EntityUid>();

	[ViewVariables]
	public readonly HashSet<Entity<PhysicsComponent, TransformComponent>> AwakeBodies = new HashSet<Entity<PhysicsComponent, TransformComponent>>();

	private float _invDt0;

	private int ContactCount => _activeContacts.Count;

	public bool MetricsEnabled { get; protected set; }

	[ViewVariables]
	public Vector2 Gravity { get; private set; }

	private void OnPhysicsInit(EntityUid uid, PhysicsComponent component, ComponentInit args)
	{
		TransformComponent transformComponent = Transform(uid);
		FixturesComponent fixturesComponent = EnsureComp<FixturesComponent>(uid);
		if (component.CanCollide && (_containerSystem.IsEntityOrParentInContainer(uid) || transformComponent.MapID == MapId.Nullspace))
		{
			SetCanCollide(uid, value: false, dirty: false, force: false, fixturesComponent, component);
		}
		if (component.CanCollide && component.BodyType != BodyType.Static)
		{
			SetAwake((Owner: uid, Comp: component), value: true);
		}
		_fixtures.OnPhysicsInit(uid, fixturesComponent, component);
		if (fixturesComponent.FixtureCount == 0)
		{
			component.CanCollide = false;
		}
		CollisionChangeEvent message = new CollisionChangeEvent(uid, component, component.CanCollide);
		RaiseLocalEvent(ref message);
	}

	private void OnPhysicsGetState(EntityUid uid, PhysicsComponent component, ref ComponentGetState args)
	{
		if (args.FromTick > component.CreationTick && component.LastFieldUpdate >= args.FromTick)
		{
			bool flag = false;
			for (int i = 0; i < _angularVelocityIndex; i++)
			{
				if (!(component.LastModifiedFields[i] < args.FromTick))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (component.LastModifiedFields[_angularVelocityIndex] >= args.FromTick)
				{
					args.State = new PhysicsVelocityDeltaState
					{
						AngularVelocity = component.AngularVelocity,
						LinearVelocity = component.LinearVelocity
					};
				}
				else
				{
					args.State = new PhysicsLinearVelocityDeltaState
					{
						LinearVelocity = component.LinearVelocity
					};
				}
				return;
			}
		}
		args.State = new PhysicsComponentState
		{
			CanCollide = component.CanCollide,
			SleepingAllowed = component.SleepingAllowed,
			FixedRotation = component.FixedRotation,
			Status = component.BodyStatus,
			LinearVelocity = component.LinearVelocity,
			AngularVelocity = component.AngularVelocity,
			BodyType = component.BodyType,
			Friction = component._friction,
			LinearDamping = component.LinearDamping,
			AngularDamping = component.AngularDamping,
			Force = component.Force,
			Torque = component.Torque
		};
	}

	private void OnPhysicsHandleState(EntityUid uid, PhysicsComponent component, ref ComponentHandleState args)
	{
		if (args.Current != null)
		{
			_fixturesQuery.TryComp(uid, out FixturesComponent component2);
			if (args.Current is PhysicsLinearVelocityDeltaState physicsLinearVelocityDeltaState)
			{
				Vector2 linearVelocity = physicsLinearVelocityDeltaState.LinearVelocity;
				PhysicsComponent body = component;
				SetLinearVelocity(uid, linearVelocity, dirty: false, wakeBody: true, component2, body);
			}
			else if (args.Current is PhysicsVelocityDeltaState physicsVelocityDeltaState)
			{
				Vector2 linearVelocity2 = physicsVelocityDeltaState.LinearVelocity;
				PhysicsComponent body = component;
				SetLinearVelocity(uid, linearVelocity2, dirty: false, wakeBody: true, component2, body);
				float angularVelocity = physicsVelocityDeltaState.AngularVelocity;
				body = component;
				SetAngularVelocity(uid, angularVelocity, dirty: false, component2, body);
			}
			else if (args.Current is PhysicsComponentState physicsComponentState)
			{
				SetSleepingAllowed(uid, component, physicsComponentState.SleepingAllowed, dirty: false);
				SetFixedRotation(uid, physicsComponentState.FixedRotation, dirty: false, null, component);
				SetCanCollide(uid, physicsComponentState.CanCollide, dirty: false, force: false, null, component);
				component.BodyStatus = physicsComponentState.Status;
				Vector2 linearVelocity3 = physicsComponentState.LinearVelocity;
				PhysicsComponent body = component;
				SetLinearVelocity(uid, linearVelocity3, dirty: false, wakeBody: true, component2, body);
				float angularVelocity2 = physicsComponentState.AngularVelocity;
				body = component;
				SetAngularVelocity(uid, angularVelocity2, dirty: false, component2, body);
				SetBodyType(uid, physicsComponentState.BodyType, component2, component);
				SetFriction(uid, component, physicsComponentState.Friction, dirty: false);
				SetLinearDamping(uid, component, physicsComponentState.LinearDamping, dirty: false);
				SetAngularDamping(uid, component, physicsComponentState.AngularDamping, dirty: false);
				component.Force = physicsComponentState.Force;
				component.Torque = physicsComponentState.Torque;
			}
		}
	}

	private bool IsMoveable(PhysicsComponent body)
	{
		return (body.BodyType & (BodyType.KinematicController | BodyType.Dynamic)) != 0;
	}

	public void ApplyAngularImpulse(EntityUid uid, float impulse, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && IsMoveable(body) && WakeBody(uid, force: false, manager, body))
		{
			SetAngularVelocity(uid, body.AngularVelocity + impulse * body.InvI, dirty: true, null, body);
		}
	}

	public void ApplyForce(EntityUid uid, Vector2 force, Vector2 point, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && IsMoveable(body) && WakeBody(uid, force: false, manager, body))
		{
			body.Force += force;
			body.Torque += Vector2Helpers.Cross(point - body._localCenter, force);
		}
	}

	public void ApplyForce(EntityUid uid, Vector2 force, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && IsMoveable(body) && WakeBody(uid, force: false, manager, body))
		{
			body.Force += force;
		}
	}

	public void ApplyTorque(EntityUid uid, float torque, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && IsMoveable(body) && WakeBody(uid, force: false, manager, body))
		{
			body.Torque += torque;
			DirtyField(uid, body, "Torque");
		}
	}

	public void ApplyLinearImpulse(EntityUid uid, Vector2 impulse, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && IsMoveable(body) && WakeBody(uid, force: false, manager, body))
		{
			SetLinearVelocity(uid, body.LinearVelocity + impulse * body._invMass, dirty: true, wakeBody: true, null, body);
		}
	}

	public void ApplyLinearImpulse(EntityUid uid, Vector2 impulse, Vector2 point, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && IsMoveable(body) && WakeBody(uid, force: false, manager, body))
		{
			SetLinearVelocity(uid, body.LinearVelocity + impulse * body._invMass, dirty: true, wakeBody: true, null, body);
			SetAngularVelocity(uid, body.AngularVelocity + body.InvI * Vector2Helpers.Cross(point - body._localCenter, impulse), dirty: true, null, body);
		}
	}

	public void DestroyContacts(PhysicsComponent body)
	{
		if (body.Contacts.Count != 0)
		{
			LinkedListNode<Contact> linkedListNode = body.Contacts.First;
			while (linkedListNode != null)
			{
				Contact value = linkedListNode.Value;
				DestroyContact(value, linkedListNode, out LinkedListNode<Contact> next);
				linkedListNode = next;
			}
		}
	}

	public void ResetDynamics(EntityUid uid, PhysicsComponent body, bool dirty = true)
	{
		body.Torque = 0f;
		body.AngularVelocity = 0f;
		body.Force = Vector2.Zero;
		body.LinearVelocity = Vector2.Zero;
		if (dirty)
		{
			DirtyFields(uid, body, null, "Torque", "AngularVelocity", "Force", "LinearVelocity");
		}
	}

	public void ResetMassData(EntityUid uid, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!PhysicsQuery.Resolve(uid, ref body) || !_fixturesQuery.Resolve(uid, ref manager))
		{
			return;
		}
		float mass = body._mass;
		float inertia = body._inertia;
		body._mass = 0f;
		body._invMass = 0f;
		body._inertia = 0f;
		body.InvI = 0f;
		Vector2 zero = Vector2.Zero;
		foreach (Fixture value in manager.Fixtures.Values)
		{
			if (!(value.Density <= 0f))
			{
				MassData data = default(MassData);
				FixtureSystem.GetMassData(value.Shape, ref data, value.Density);
				body._mass += data.Mass;
				zero += data.Center * data.Mass;
				body._inertia += data.I;
			}
		}
		if (body._mass > 0f)
		{
			body._invMass = 1f / body._mass;
			zero *= body._invMass;
		}
		else
		{
			body._mass = 1f;
			body._invMass = 1f;
		}
		if (body._inertia > 0f && !body.FixedRotation)
		{
			body._inertia -= body._mass * Vector2.Dot(zero, zero);
			body.InvI = 1f / body._inertia;
		}
		else
		{
			body._inertia = 0f;
			body.InvI = 0f;
		}
		Vector2 localCenter = body._localCenter;
		body._localCenter = zero;
		if ((body.BodyType & BodyType.Static) == 0)
		{
			float angularVelocity = body.AngularVelocity;
			Vector2 vector = zero - localCenter;
			Vector2 vector2 = Vector2Helpers.Cross(angularVelocity, ref vector);
			if (vector2 != Vector2.Zero)
			{
				body.LinearVelocity += vector2;
				DirtyField(uid, body, "LinearVelocity");
			}
		}
		if (body._mass != mass || body._inertia != inertia || !(localCenter == zero))
		{
			MassDataChangedEvent args = new MassDataChangedEvent((Owner: uid, Comp1: body, Comp2: manager), mass, inertia, localCenter);
			RaiseLocalEvent(uid, ref args);
		}
	}

	public bool SetAngularVelocity(EntityUid uid, float value, bool dirty = true, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!PhysicsQuery.Resolve(uid, ref body))
		{
			return false;
		}
		if (body.BodyType == BodyType.Static)
		{
			return false;
		}
		if (value * value > 0f && !WakeBody(uid, force: false, manager, body))
		{
			return false;
		}
		if (MathHelper.CloseToPercent(body.AngularVelocity, value, 9.999999747378752E-06))
		{
			return false;
		}
		body.AngularVelocity = value;
		if (dirty)
		{
			DirtyField(uid, body, "AngularVelocity");
		}
		return true;
	}

	public bool SetLinearVelocity(EntityUid uid, Vector2 velocity, bool dirty = true, bool wakeBody = true, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!PhysicsQuery.Resolve(uid, ref body))
		{
			return false;
		}
		if (body.BodyType == BodyType.Static)
		{
			return false;
		}
		if (wakeBody && Vector2.Dot(velocity, velocity) > 0f && !WakeBody(uid, force: false, manager, body))
		{
			return false;
		}
		if (Vector2Helpers.EqualsApprox(body.LinearVelocity, velocity, 1.0000000116860974E-07))
		{
			return false;
		}
		body.LinearVelocity = velocity;
		if (dirty)
		{
			DirtyField(uid, body, "LinearVelocity");
		}
		return true;
	}

	public void SetAngularDamping(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
	{
		if (!MathHelper.CloseTo(body.AngularDamping, value, 1E-07f))
		{
			body.AngularDamping = value;
			if (dirty)
			{
				DirtyField(uid, body, "AngularDamping");
			}
		}
	}

	public void SetLinearDamping(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
	{
		if (!MathHelper.CloseTo(body.LinearDamping, value, 1E-07f))
		{
			body.LinearDamping = value;
			if (dirty)
			{
				DirtyField(uid, body, "LinearDamping");
			}
		}
	}

	[Obsolete("Use SetAwake with EntityUid<PhysicsComponent>")]
	public void SetAwake(EntityUid uid, PhysicsComponent body, bool value, bool updateSleepTime = true)
	{
		SetAwake(new Entity<PhysicsComponent>(uid, body), value, updateSleepTime);
	}

	public void SetAwake(Entity<PhysicsComponent> ent, bool value, bool updateSleepTime = true)
	{
		Entity<PhysicsComponent> entity = ent;
		entity.Deconstruct(out var owner, out var comp);
		EntityUid entityUid = owner;
		PhysicsComponent physicsComponent = comp;
		bool flag = physicsComponent.BodyType != BodyType.Static && physicsComponent.CanCollide;
		if (physicsComponent.Awake != value && (!value || flag))
		{
			physicsComponent.Awake = value;
			if (value)
			{
				PhysicsWakeEvent args = new PhysicsWakeEvent(entityUid, physicsComponent);
				RaiseLocalEvent(entityUid, ref args, broadcast: true);
			}
			else
			{
				PhysicsSleepEvent args2 = new PhysicsSleepEvent(entityUid, physicsComponent);
				RaiseLocalEvent(entityUid, ref args2, broadcast: true);
				ResetDynamics(ent, physicsComponent, dirty: false);
			}
			if (!value && physicsComponent.CanCollide)
			{
				_wakeSystem.UpdateCanCollide(ent, checkTerminating: false, dirty: false);
			}
			if (updateSleepTime)
			{
				SetSleepTime(physicsComponent, 0f);
			}
			if (physicsComponent.Awake != value)
			{
				base.Log.Error($"Found a corrupted physics awake state for {ToPrettyString(ent)}! Did you forget to cancel the sleep subscription? Forcing body awake");
				physicsComponent.Awake = true;
			}
			if (physicsComponent.Awake)
			{
				AddAwakeBody((Owner: entityUid, Comp1: physicsComponent, Comp2: Transform(entityUid)));
			}
			else
			{
				RemoveSleepBody((Owner: entityUid, Comp1: physicsComponent, Comp2: Transform(entityUid)));
			}
		}
	}

	public void TrySetBodyType(EntityUid uid, BodyType value, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (_fixturesQuery.Resolve(uid, ref manager, logMissing: false) && PhysicsQuery.Resolve(uid, ref body, logMissing: false) && XformQuery.Resolve(uid, ref xform, logMissing: false))
		{
			SetBodyType(uid, value, manager, body, xform);
		}
	}

	public void SetBodyType(EntityUid uid, BodyType value, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && body.BodyType != value)
		{
			BodyType bodyType = body.BodyType;
			body.BodyType = value;
			body.Force = Vector2.Zero;
			body.Torque = 0f;
			if (body.BodyType == BodyType.Static)
			{
				SetAwake((Owner: uid, Comp: body), value: false);
				body.LinearVelocity = Vector2.Zero;
				body.AngularVelocity = 0f;
				DirtyFields(uid, body, null, "LinearVelocity", "AngularVelocity", "Force", "Torque");
			}
			else if (body.CanCollide)
			{
				SetAwake((Owner: uid, Comp: body), value: true);
				DirtyFields(uid, body, null, "Force", "Torque");
			}
			_broadphase.RegenerateContacts((Owner: uid, Comp1: body, Comp2: manager, Comp3: xform));
			if (body.Initialized)
			{
				PhysicsBodyTypeChangedEvent args = new PhysicsBodyTypeChangedEvent(uid, body.BodyType, bodyType, body);
				RaiseLocalEvent(uid, ref args, broadcast: true);
			}
		}
	}

	public void SetBodyStatus(EntityUid uid, PhysicsComponent body, BodyStatus status, bool dirty = true)
	{
		if (body.BodyStatus != status)
		{
			body.BodyStatus = status;
			if (dirty)
			{
				DirtyField(uid, body, "BodyStatus");
			}
		}
	}

	public bool SetCanCollide(EntityUid uid, bool value, bool dirty = true, bool force = false, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!PhysicsQuery.Resolve(uid, ref body))
		{
			return false;
		}
		if (body.CanCollide == value)
		{
			return value;
		}
		if (value && !force)
		{
			if (_containerSystem.IsEntityOrParentInContainer(uid))
			{
				return false;
			}
			if (!_fixturesQuery.Resolve(uid, ref manager) || (manager.FixtureCount == 0 && !_gridQuery.HasComp(uid)))
			{
				return false;
			}
		}
		body.CanCollide = value;
		if (!value)
		{
			SetAwake((Owner: uid, Comp: body), value: false);
		}
		if (body.Initialized)
		{
			CollisionChangeEvent message = new CollisionChangeEvent(uid, body, value);
			RaiseLocalEvent(ref message);
		}
		if (dirty)
		{
			DirtyField(uid, body, "CanCollide");
		}
		return value;
	}

	public void SetFixedRotation(EntityUid uid, bool value, bool dirty = true, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (PhysicsQuery.Resolve(uid, ref body) && body.FixedRotation != value)
		{
			body.FixedRotation = value;
			body.AngularVelocity = 0f;
			if (dirty)
			{
				DirtyFields(uid, body, null, "FixedRotation", "AngularVelocity");
			}
			ResetMassData(uid, manager, body);
		}
	}

	public void SetFriction(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
	{
		if (!MathHelper.CloseTo(body.Friction, value, 1E-07f))
		{
			body._friction = value;
			if (dirty)
			{
				DirtyField(uid, body, "Friction");
			}
		}
	}

	public void SetInertia(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
	{
		if (body.BodyType == BodyType.Dynamic && !MathHelper.CloseToPercent(body._inertia, value, 1E-05) && value > 0f && !body.FixedRotation)
		{
			body._inertia = value - body.Mass * Vector2.Dot(body._localCenter, body._localCenter);
			body.InvI = 1f / body._inertia;
		}
	}

	public void SetLocalCenter(EntityUid uid, PhysicsComponent body, Vector2 value)
	{
		if (body.BodyType == BodyType.Dynamic && !Vector2Helpers.EqualsApprox(value, body._localCenter))
		{
			body._localCenter = value;
		}
	}

	public void SetSleepingAllowed(EntityUid uid, PhysicsComponent body, bool value, bool dirty = true)
	{
		if (body.SleepingAllowed != value)
		{
			if (!value)
			{
				SetAwake((Owner: uid, Comp: body), value: true);
			}
			body.SleepingAllowed = value;
			if (dirty)
			{
				DirtyField(uid, body, "SleepingAllowed");
			}
		}
	}

	public void SetSleepTime(PhysicsComponent body, float value)
	{
		if (!MathHelper.CloseToPercent(value, body.SleepTime, 1E-05))
		{
			body.SleepTime = value;
		}
	}

	public bool WakeBody(EntityUid uid, bool force = false, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!PhysicsQuery.Resolve(uid, ref body))
		{
			return false;
		}
		PhysicsComponent body2 = body;
		if (!SetCanCollide(uid, value: true, dirty: true, force, manager, body2))
		{
			return false;
		}
		SetAwake((Owner: uid, Comp: body), value: true);
		return body.Awake;
	}

	public Transform GetRelativePhysicsTransform(Transform worldTransform, Entity<TransformComponent?> relative)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(relative.Owner, ref relative.Comp))
		{
			return Robust.Shared.Physics.Transform.Empty;
		}
		(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) worldPositionRotationMatrixWithInv = _transform.GetWorldPositionRotationMatrixWithInv(relative.Comp);
		Angle item = worldPositionRotationMatrixWithInv.WorldRotation;
		Matrix3x2 item2 = worldPositionRotationMatrixWithInv.InvWorldMatrix;
		return new Transform(Vector2.Transform(worldTransform.Position, item2), Angle.op_Implicit(worldTransform.Quaternion2D.Angle) - item);
	}

	public Transform GetRelativePhysicsTransform(Entity<TransformComponent?> entity, Entity<TransformComponent?> relative)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(entity.Owner, ref entity.Comp) || !XformQuery.Resolve(relative.Owner, ref relative.Comp))
		{
			return Robust.Shared.Physics.Transform.Empty;
		}
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(entity.Comp);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) worldPositionRotationMatrixWithInv = _transform.GetWorldPositionRotationMatrixWithInv(relative.Comp);
		Angle item3 = worldPositionRotationMatrixWithInv.WorldRotation;
		Matrix3x2 item4 = worldPositionRotationMatrixWithInv.InvWorldMatrix;
		return new Transform(Vector2.Transform(item, item4), item2 - item3);
	}

	public Transform GetLocalPhysicsTransform(EntityUid uid, TransformComponent? xform = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(uid, ref xform) || !xform.Broadphase.HasValue)
		{
			return Robust.Shared.Physics.Transform.Empty;
		}
		EntityUid uid2 = xform.Broadphase.Value.Uid;
		if (xform.ParentUid == uid2)
		{
			return new Transform(xform.LocalPosition, xform.LocalRotation);
		}
		return GetRelativePhysicsTransform((Owner: uid, Comp: xform), uid2);
	}

	public Transform GetPhysicsTransform(EntityUid uid, TransformComponent? xform = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(uid, ref xform))
		{
			return Robust.Shared.Physics.Transform.Empty;
		}
		var (position, angle) = _transform.GetWorldPositionRotation(xform);
		return new Transform(position, angle);
	}

	public Box2 GetWorldAABB(EntityUid uid, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!Resolve(uid, ref manager, ref body, ref xform))
		{
			return default(Box2);
		}
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(xform);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		Transform transform = new Transform(item, (float)item2.Theta);
		Unsafe.SkipInit(out Box2 result);
		((Box2)(ref result))._002Ector(transform.Position, transform.Position);
		foreach (Fixture value in manager.Fixtures.Values)
		{
			for (int i = 0; i < value.Shape.ChildCount; i++)
			{
				Box2 val = value.Shape.ComputeAABB(transform, i);
				result = ((Box2)(ref result)).Union(ref val);
			}
		}
		return result;
	}

	public Box2 GetHardAABB(EntityUid uid, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (!PhysicsQuery.Resolve(uid, ref body) || !_fixturesQuery.Resolve(uid, ref manager) || !Resolve(uid, ref xform))
		{
			return Box2.Empty;
		}
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(xform);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		Transform transform = new Transform(item, (float)item2.Theta);
		Unsafe.SkipInit(out Box2 result);
		((Box2)(ref result))._002Ector(transform.Position, transform.Position);
		foreach (Fixture value in manager.Fixtures.Values)
		{
			if (value.Hard)
			{
				for (int i = 0; i < value.Shape.ChildCount; i++)
				{
					Box2 val = value.Shape.ComputeAABB(transform, i);
					result = ((Box2)(ref result)).Union(ref val);
				}
			}
		}
		return result;
	}

	public (int Layer, int Mask) GetHardCollision(EntityUid uid, FixturesComponent? manager = null)
	{
		if (!_fixturesQuery.Resolve(uid, ref manager, logMissing: false))
		{
			return (Layer: 0, Mask: 0);
		}
		return GetHardCollision(manager);
	}

	public static (int Layer, int Mask) GetHardCollision(FixturesComponent manager)
	{
		int num = 0;
		int num2 = 0;
		foreach (Fixture value in manager.Fixtures.Values)
		{
			if (value.Hard)
			{
				num |= value.CollisionLayer;
				num2 |= value.CollisionMask;
			}
		}
		return (Layer: num, Mask: num2);
	}

	public virtual void UpdateIsPredicted(EntityUid? uid, PhysicsComponent? physics = null)
	{
	}

	private static void SetContact(Contact contact, bool enabled, Entity<PhysicsComponent?, TransformComponent?> entA, Entity<PhysicsComponent?, TransformComponent?> entB, string fixtureAId, string fixtureBId, Fixture? fixtureA, int indexA, Fixture? fixtureB, int indexB)
	{
		EntityUid owner = entA.Owner;
		EntityUid owner2 = entB.Owner;
		contact.Enabled = enabled;
		contact.IsTouching = false;
		contact.EntityA = owner;
		contact.EntityB = owner2;
		contact.FixtureAId = fixtureAId;
		contact.FixtureBId = fixtureBId;
		contact.FixtureA = fixtureA;
		contact.FixtureB = fixtureB;
		contact.BodyA = entA.Comp1;
		contact.BodyB = entB.Comp1;
		contact.XformA = entA.Comp2;
		contact.XformB = entB.Comp2;
		contact.ChildIndexA = indexA;
		contact.ChildIndexB = indexB;
		contact.Manifold.PointCount = 0;
		if (fixtureA != null && fixtureB != null)
		{
			contact.Friction = MathF.Sqrt(fixtureA.Friction * fixtureB.Friction);
			contact.Restitution = MathF.Max(fixtureA.Restitution, fixtureB.Restitution);
		}
		contact.TangentSpeed = 0f;
	}

	private void InitializeContacts()
	{
		_contactPool = new DefaultObjectPool<Contact>(new ContactPoolPolicy(_debugPhysics, _manifoldManager), 4096);
		InitializePool();
		EntityManager.EntityQueueDeleted += OnContactEntityQueueDel;
	}

	private void ShutdownContacts()
	{
		EntityManager.EntityQueueDeleted -= OnContactEntityQueueDel;
	}

	private void OnContactEntityQueueDel(EntityUid obj)
	{
		if (TryComp(obj, out PhysicsComponent comp))
		{
			DestroyContacts(comp);
		}
	}

	private void InitializePool()
	{
		Contact[] array = new Contact[128];
		for (int i = 0; i < 128; i++)
		{
			array[i] = _contactPool.Get();
		}
		for (int j = 0; j < 128; j++)
		{
			_contactPool.Return(array[j]);
		}
	}

	private Contact CreateContact(Entity<PhysicsComponent?, TransformComponent?> entA, Entity<PhysicsComponent?, TransformComponent?> entB, string fixtureAId, string fixtureBId, Fixture fixtureA, int indexA, Fixture fixtureB, int indexB)
	{
		ShapeType shapeType = fixtureA.Shape.ShapeType;
		ShapeType shapeType2 = fixtureB.Shape.ShapeType;
		Contact contact = _contactPool.Get();
		contact.Flags = ContactFlags.PreInit;
		if ((shapeType >= shapeType2 || (shapeType == ShapeType.Edge && shapeType2 == ShapeType.Polygon)) && (shapeType2 != ShapeType.Edge || shapeType != ShapeType.Polygon))
		{
			SetContact(contact, enabled: true, entA, entB, fixtureAId, fixtureBId, fixtureA, indexA, fixtureB, indexB);
		}
		else
		{
			SetContact(contact, enabled: true, entB, entA, fixtureBId, fixtureAId, fixtureB, indexB, fixtureA, indexA);
		}
		contact.Type = _registers[(int)shapeType, (int)shapeType2];
		return contact;
	}

	internal void AddPair(Entity<PhysicsComponent, TransformComponent> entA, Entity<PhysicsComponent, TransformComponent> entB, string fixtureAId, string fixtureBId, Fixture fixtureA, int indexA, Fixture fixtureB, int indexB, PhysicsComponent bodyA, PhysicsComponent bodyB, ContactFlags flags = ContactFlags.None)
	{
		TransformComponent comp = entA.Comp2;
		TransformComponent comp2 = entB.Comp2;
		if (ShouldCollideSlow(entA.Owner, entB.Owner, bodyA, bodyB, fixtureA, fixtureB, comp, comp2))
		{
			Contact contact = CreateContact((Owner: entA.Owner, Comp1: entA.Comp1, Comp2: entA.Comp2), (Owner: entB.Owner, Comp1: entB.Comp1, Comp2: entB.Comp2), fixtureAId, fixtureBId, fixtureA, indexA, fixtureB, indexB);
			contact.Flags = flags;
			Fixture fixtureA2 = contact.FixtureA;
			Fixture fixtureB2 = contact.FixtureB;
			PhysicsComponent? bodyA2 = contact.BodyA;
			PhysicsComponent bodyB2 = contact.BodyB;
			_activeContacts.AddLast(contact.MapNode);
			fixtureA2.Contacts.Add(fixtureB2, contact);
			bodyA2.Contacts.AddLast(contact.BodyANode);
			fixtureB2.Contacts.Add(fixtureA2, contact);
			bodyB2.Contacts.AddLast(contact.BodyBNode);
			if (bodyA.BodyType == BodyType.Static && !bodyB.Awake)
			{
				WakeBody(entB.Owner, force: false, null, bodyB);
			}
		}
	}

	internal void AddPair(string fixtureAId, string fixtureBId, in FixtureProxy proxyA, in FixtureProxy proxyB, ContactFlags flags = ContactFlags.None)
	{
		AddPair((Owner: proxyA.Entity, Comp1: proxyA.Body, Comp2: proxyA.Xform), (Owner: proxyB.Entity, Comp1: proxyB.Body, Comp2: proxyB.Xform), fixtureAId, fixtureBId, proxyA.Fixture, proxyA.ChildIndex, proxyB.Fixture, proxyB.ChildIndex, proxyA.Body, proxyB.Body, flags);
	}

	internal static bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
	{
		if ((fixtureA.CollisionMask & fixtureB.CollisionLayer) == 0)
		{
			return (fixtureB.CollisionMask & fixtureA.CollisionLayer) != 0;
		}
		return true;
	}

	public void DestroyContact(Contact contact)
	{
		DestroyContact(contact, null, out LinkedListNode<Contact> _);
	}

	internal void DestroyContact(Contact contact, LinkedListNode<Contact>? node, out LinkedListNode<Contact>? next)
	{
		if ((contact.Flags & (ContactFlags.Deleting | ContactFlags.Deleted)) != ContactFlags.None)
		{
			next = node?.Next;
			return;
		}
		Fixture fixtureA = contact.FixtureA;
		Fixture fixtureB = contact.FixtureB;
		PhysicsComponent bodyA = contact.BodyA;
		PhysicsComponent bodyB = contact.BodyB;
		EntityUid entityA = contact.EntityA;
		EntityUid entityB = contact.EntityB;
		contact.Flags |= ContactFlags.Deleting;
		if (contact.IsTouching)
		{
			EndCollideEvent args = new EndCollideEvent(entityA, entityB, contact.FixtureAId, contact.FixtureBId, fixtureA, fixtureB, bodyA, bodyB);
			EndCollideEvent args2 = new EndCollideEvent(entityB, entityA, contact.FixtureBId, contact.FixtureAId, fixtureB, fixtureA, bodyB, bodyA);
			RaiseLocalEvent(entityA, ref args);
			RaiseLocalEvent(entityB, ref args2);
		}
		if (contact.Manifold.PointCount > 0)
		{
			Fixture? fixtureA2 = contact.FixtureA;
			if (fixtureA2 != null && fixtureA2.Hard)
			{
				Fixture? fixtureB2 = contact.FixtureB;
				if (fixtureB2 != null && fixtureB2.Hard)
				{
					if (bodyA.CanCollide && !TerminatingOrDeleted(entityA))
					{
						SetAwake((Owner: entityA, Comp: bodyA), value: true);
					}
					if (bodyB.CanCollide && !TerminatingOrDeleted(entityB))
					{
						SetAwake((Owner: entityB, Comp: bodyB), value: true);
					}
				}
			}
		}
		next = node?.Next;
		_activeContacts.Remove(contact.MapNode);
		fixtureA.Contacts.Remove(fixtureB);
		bodyA.Contacts.Remove(contact.BodyANode);
		fixtureB.Contacts.Remove(fixtureA);
		bodyB.Contacts.Remove(contact.BodyBNode);
		contact.Flags = ContactFlags.Deleted;
		_contactPool.Return(contact);
	}

	internal void CollideContacts()
	{
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		Contact[] array = ArrayPool<Contact>.Shared.Rent(ContactCount);
		int num = 0;
		LinkedListNode<Contact> linkedListNode = _activeContacts.First;
		while (linkedListNode != null)
		{
			Contact value = linkedListNode.Value;
			linkedListNode = linkedListNode.Next;
			if (!value.Enabled)
			{
				continue;
			}
			value.Flags &= ~ContactFlags.PreInit;
			Fixture fixtureA = value.FixtureA;
			Fixture fixtureB = value.FixtureB;
			int childIndexA = value.ChildIndexA;
			int childIndexB = value.ChildIndexB;
			PhysicsComponent bodyA = value.BodyA;
			PhysicsComponent bodyB = value.BodyB;
			EntityUid entityA = value.EntityA;
			EntityUid entityB = value.EntityB;
			if (!bodyA.CanCollide || !bodyB.CanCollide)
			{
				DestroyContact(value);
				continue;
			}
			TransformComponent xformA = value.XformA;
			TransformComponent xformB = value.XformB;
			if (xformA.MapID == MapId.Nullspace || xformB.MapID == MapId.Nullspace)
			{
				DestroyContact(value);
				continue;
			}
			if ((value.Flags & ContactFlags.Filter) != ContactFlags.None)
			{
				if (!ShouldCollide(fixtureA, fixtureB) || !ShouldCollideSlow(entityA, entityB, bodyA, bodyB, fixtureA, fixtureB, xformA, xformB) || !ShouldCollideJoints(entityA, entityB))
				{
					DestroyContact(value);
					continue;
				}
				value.Flags &= ~ContactFlags.Filter;
			}
			bool num2 = bodyA.Awake && bodyA.BodyType != BodyType.Static;
			bool flag = bodyB.Awake && bodyB.BodyType != BodyType.Static;
			if (!num2 && !flag)
			{
				continue;
			}
			if (!xformA.MapUid.HasValue || xformA.MapUid != xformB.MapUid)
			{
				DestroyContact(value);
				continue;
			}
			if ((value.Flags & ContactFlags.Grid) != ContactFlags.None)
			{
				Box2 val = fixtureA.Shape.ComputeAABB(GetPhysicsTransform(entityA, xformA), 0);
				Box2 val2 = fixtureB.Shape.ComputeAABB(GetPhysicsTransform(entityB, xformB), 0);
				if (!((Box2)(ref val)).Intersects(ref val2))
				{
					DestroyContact(value);
					continue;
				}
				value.Flags &= ~ContactFlags.Island;
				if (num >= array.Length)
				{
					base.Log.Error($"Insufficient contact length at 388! Index {num} and length is {array.Length}. Tell Sloth");
				}
				array[num++] = value;
				continue;
			}
			if (childIndexA >= fixtureA.Proxies.Length)
			{
				base.Log.Error($"Found invalid contact index of {childIndexA} on {value.FixtureAId} / {ToPrettyString(entityA)}, expected {fixtureA.Proxies.Length}");
				DestroyContact(value);
				continue;
			}
			if (childIndexB >= fixtureB.Proxies.Length)
			{
				base.Log.Error($"Found invalid contact index of {childIndexB} on {value.FixtureBId} / {ToPrettyString(entityB)}, expected {fixtureB.Proxies.Length}");
				DestroyContact(value);
				continue;
			}
			FixtureProxy fixtureProxy = fixtureA.Proxies[childIndexA];
			FixtureProxy fixtureProxy2 = fixtureB.Proxies[childIndexB];
			EntityUid? entityUid = xformA.Broadphase?.Uid;
			EntityUid? entityUid2 = xformB.Broadphase?.Uid;
			bool flag2 = false;
			if (entityUid.HasValue && entityUid2.HasValue)
			{
				if (entityUid == entityUid2)
				{
					flag2 = ((Box2)(ref fixtureProxy.AABB)).Intersects(ref fixtureProxy2.AABB);
				}
				else
				{
					Box2 val3 = Matrix3Helpers.TransformBox(_transform.GetWorldMatrix(XformQuery.GetComponent(entityUid.Value)), ref fixtureProxy.AABB);
					Box2 val4 = Matrix3Helpers.TransformBox(_transform.GetWorldMatrix(XformQuery.GetComponent(entityUid2.Value)), ref fixtureProxy2.AABB);
					flag2 = ((Box2)(ref val3)).Intersects(ref val4);
				}
			}
			if (!flag2)
			{
				DestroyContact(value);
				continue;
			}
			value.Flags &= ~ContactFlags.Island;
			if (num >= array.Length)
			{
				base.Log.Error($"Insufficient contact length at 429! Index {num} and length is {array.Length}. Tell Sloth");
			}
			array[num++] = value;
		}
		ContactStatus[] array2 = ArrayPool<ContactStatus>.Shared.Rent(num);
		FixedArray4<Vector2>[] array3 = ArrayPool<FixedArray4<Vector2>>.Shared.Rent(num);
		BuildManifolds(array, num, array2, array3);
		for (int i = 0; i < num; i++)
		{
			if (i >= array.Length)
			{
				base.Log.Error("Invalid contact length for contact events!");
				continue;
			}
			Contact contact = array[i];
			if (contact.Enabled)
			{
				RunContactEvents(array2[i], contact, array3[i]);
			}
		}
		ArrayPool<Contact>.Shared.Return(array);
		ArrayPool<ContactStatus>.Shared.Return(array2);
		ArrayPool<FixedArray4<Vector2>>.Shared.Return(array3);
	}

	internal void RunContactEvents(ContactStatus status, Contact contact, FixedArray4<Vector2> worldPoint)
	{
		switch (status)
		{
		case ContactStatus.StartTouching:
			if (contact.IsTouching)
			{
				Fixture fixtureA2 = contact.FixtureA;
				Fixture fixtureB2 = contact.FixtureB;
				PhysicsComponent bodyA2 = contact.BodyA;
				PhysicsComponent bodyB2 = contact.BodyB;
				EntityUid entityA2 = contact.EntityA;
				EntityUid entityB2 = contact.EntityB;
				FixedArray2<Vector2> worldPoints = new FixedArray2<Vector2>(worldPoint._00, worldPoint._01);
				Vector2 _ = worldPoint._02;
				StartCollideEvent args3 = new StartCollideEvent(entityA2, entityB2, contact.FixtureAId, contact.FixtureBId, fixtureA2, fixtureB2, bodyA2, bodyB2, worldPoints, contact.Manifold.PointCount, _);
				StartCollideEvent args4 = new StartCollideEvent(entityB2, entityA2, contact.FixtureBId, contact.FixtureAId, fixtureB2, fixtureA2, bodyB2, bodyA2, worldPoints, contact.Manifold.PointCount, _);
				RaiseLocalEvent(entityA2, ref args3, broadcast: true);
				RaiseLocalEvent(entityB2, ref args4, broadcast: true);
			}
			break;
		case ContactStatus.EndTouching:
		{
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			if (fixtureA != null && fixtureB != null)
			{
				PhysicsComponent bodyA = contact.BodyA;
				PhysicsComponent bodyB = contact.BodyB;
				EntityUid entityA = contact.EntityA;
				EntityUid entityB = contact.EntityB;
				EndCollideEvent args = new EndCollideEvent(entityA, entityB, contact.FixtureAId, contact.FixtureBId, fixtureA, fixtureB, bodyA, bodyB);
				EndCollideEvent args2 = new EndCollideEvent(entityB, entityA, contact.FixtureBId, contact.FixtureAId, fixtureB, fixtureA, bodyB, bodyA);
				RaiseLocalEvent(entityA, ref args);
				RaiseLocalEvent(entityB, ref args2);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		case ContactStatus.NoContact:
		case ContactStatus.Touching:
			break;
		}
	}

	private void BuildManifolds(Contact[] contacts, int count, ContactStatus[] status, FixedArray4<Vector2>[] worldPoints)
	{
		if (count == 0)
		{
			return;
		}
		bool[] array = ArrayPool<bool>.Shared.Rent(count);
		_parallel.ProcessNow(new ManifoldsJob
		{
			Physics = this,
			Status = status,
			WorldPoints = worldPoints,
			Contacts = contacts,
			Wake = array
		}, count);
		for (int i = 0; i < count; i++)
		{
			if (array[i])
			{
				Contact obj = contacts[i];
				PhysicsComponent bodyA = obj.BodyA;
				PhysicsComponent bodyB = obj.BodyB;
				EntityUid entityA = obj.EntityA;
				EntityUid entityB = obj.EntityB;
				SetAwake((Owner: entityA, Comp: bodyA), value: true);
				SetAwake((Owner: entityB, Comp: bodyB), value: true);
			}
		}
		ArrayPool<bool>.Shared.Return(array);
	}

	private void UpdateContact(Contact[] contacts, int index, ContactStatus[] status, bool[] wake, FixedArray4<Vector2>[] worldPoints)
	{
		Contact contact = contacts[index];
		if (!contact.Enabled)
		{
			status[index] = ContactStatus.NoContact;
			wake[index] = false;
			return;
		}
		EntityUid entityA = contact.EntityA;
		EntityUid entityB = contact.EntityB;
		Transform physicsTransform = GetPhysicsTransform(entityA);
		Transform physicsTransform2 = GetPhysicsTransform(entityB);
		if ((status[index] = contact.Update(physicsTransform, physicsTransform2, out wake[index])) == ContactStatus.StartTouching)
		{
			FixedArray4<Vector2> fixedArray = default(FixedArray4<Vector2>);
			contact.GetWorldManifold(physicsTransform, physicsTransform2, out var normal, fixedArray.AsSpan);
			fixedArray._02 = normal;
			worldPoints[index] = fixedArray;
		}
	}

	internal bool ShouldCollideJoints(Entity<JointComponent?> entA, Entity<JointComponent?> entB)
	{
		if (JointQuery.Resolve(entA.Owner, ref entA.Comp, logMissing: false) && JointQuery.HasComp(entB))
		{
			foreach (Joint value in entA.Comp.Joints.Values)
			{
				if (!value.CollideConnected && (entB.Owner == value.BodyAUid || entB.Owner == value.BodyBUid))
				{
					return false;
				}
			}
		}
		return true;
	}

	internal bool ShouldCollideSlow(EntityUid uid, EntityUid other, PhysicsComponent body, PhysicsComponent otherBody, Fixture fixture, Fixture otherFixture, TransformComponent xform, TransformComponent otherXform)
	{
		if (((body.BodyType & BodyType.Static) != BodyType.Kinematic && (otherBody.BodyType & BodyType.Static) != BodyType.Kinematic) || (fixture.Hard && body.BodyType == BodyType.KinematicController && otherFixture.Hard && otherBody.BodyType == BodyType.KinematicController))
		{
			return false;
		}
		if (fixture.Hard && otherFixture.Hard)
		{
			if (uid == other)
			{
				return false;
			}
			if (other == xform.ParentUid && body.BodyType == BodyType.Static)
			{
				return false;
			}
			if (uid == otherXform.ParentUid && otherBody.BodyType == BodyType.Static)
			{
				return false;
			}
		}
		PreventCollideEvent args = new PreventCollideEvent(uid, other, body, otherBody, fixture, otherFixture);
		RaiseLocalEvent(uid, ref args);
		if (args.Cancelled)
		{
			return false;
		}
		args = new PreventCollideEvent(other, uid, otherBody, body, otherFixture, fixture);
		RaiseLocalEvent(other, ref args);
		if (args.Cancelled)
		{
			return false;
		}
		return true;
	}

	public void RegenerateContacts(Entity<PhysicsComponent?> entity)
	{
		if (PhysicsQuery.Resolve(entity.Owner, ref entity.Comp))
		{
			_broadphase.RegenerateContacts(entity);
		}
	}

	public int GetTouchingContacts(Entity<FixturesComponent?> entity, string? ignoredFixtureId = null)
	{
		if (!_fixturesQuery.Resolve(entity.Owner, ref entity.Comp))
		{
			return 0;
		}
		int num = 0;
		foreach (var (text2, fixture2) in entity.Comp.Fixtures)
		{
			if (ignoredFixtureId == text2)
			{
				continue;
			}
			foreach (Contact value in fixture2.Contacts.Values)
			{
				if (value.IsTouching)
				{
					num++;
				}
			}
		}
		return num;
	}

	public ContactEnumerator GetContacts(Entity<FixturesComponent?> entity, bool includeDeleting = false)
	{
		_fixturesQuery.Resolve(entity.Owner, ref entity.Comp);
		return new ContactEnumerator(entity.Comp, includeDeleting);
	}

	public override void Initialize()
	{
		base.Initialize();
		_physicsReg = EntityManager.ComponentFactory.GetRegistration(CompIdx.Index<PhysicsComponent>());
		EntityManager.ComponentFactory.RegisterNetworkedFields(_physicsReg, "CanCollide", "BodyStatus", "BodyType", "SleepingAllowed", "FixedRotation", "Friction", "Force", "Torque", "LinearDamping", "AngularDamping", "AngularVelocity", "LinearVelocity");
		_angularVelocityIndex = 10;
		_anchorQuery = GetEntityQuery<CollideOnAnchorComponent>();
		_fixturesQuery = GetEntityQuery<FixturesComponent>();
		JointQuery = GetEntityQuery<JointComponent>();
		RelayTargetQuery = GetEntityQuery<JointRelayTargetComponent>();
		MapQuery = GetEntityQuery<MapComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		PhysicsQuery = GetEntityQuery<PhysicsComponent>();
		XformQuery = GetEntityQuery<TransformComponent>();
		SubscribeLocalEvent<GridAddEvent>(OnGridAdd);
		SubscribeLocalEvent<CollisionChangeEvent>(OnCollisionChange);
		SubscribeLocalEvent<PhysicsComponent, EntGotRemovedFromContainerMessage>(HandleContainerRemoved);
		SubscribeLocalEvent<PhysicsComponent, ComponentInit>(OnPhysicsInit);
		SubscribeLocalEvent<PhysicsComponent, ComponentShutdown>(OnPhysicsShutdown);
		SubscribeLocalEvent<PhysicsComponent, ComponentGetState>(OnPhysicsGetState);
		SubscribeLocalEvent<PhysicsComponent, ComponentHandleState>(OnPhysicsHandleState);
		InitializeIsland();
		InitializeContacts();
		base.Subs.CVar(_cfg, CVars.AutoClearForces, OnAutoClearChange, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.NetTickrate, UpdateSubsteps, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.TargetMinimumTickrate, UpdateSubsteps, invokeImmediately: true);
	}

	private void OnPhysicsShutdown(EntityUid uid, PhysicsComponent component, ComponentShutdown args)
	{
		SetCanCollide(uid, value: false, dirty: false, force: false, null, component);
		if ((int)LifeStage(uid) <= 3)
		{
			RemComp<FixturesComponent>(uid);
		}
	}

	private void OnCollisionChange(ref CollisionChangeEvent ev)
	{
		EntityUid bodyUid = ev.BodyUid;
		if (!(Transform(bodyUid).MapID == MapId.Nullspace) && !ev.CanCollide)
		{
			DestroyContacts(ev.Body);
		}
	}

	private void OnAutoClearChange(bool value)
	{
		_autoClearForces = value;
	}

	private void UpdateSubsteps(int obj)
	{
		float num = _cfg.GetCVar(CVars.TargetMinimumTickrate);
		float num2 = _cfg.GetCVar(CVars.NetTickrate);
		_substeps = (int)Math.Ceiling(num / num2);
	}

	internal void OnParentChange(Entity<TransformComponent, MetaDataComponent> ent, EntityUid oldParent, EntityUid? oldMap)
	{
		Entity<TransformComponent, MetaDataComponent> entity = ent;
		var (uid, transformComponent2, metaDataComponent2) = (Entity<TransformComponent, MetaDataComponent>)(ref entity);
		if ((transformComponent2.ChildCount != 0 || (int)metaDataComponent2.EntityLifeStage >= 2) && (oldMap.HasValue || transformComponent2.MapUid.HasValue))
		{
			PhysicsComponent physicsComponent = PhysicsQuery.CompOrNull(uid);
			if (oldMap != transformComponent2.MapUid)
			{
				HandleMapChange(uid, transformComponent2, physicsComponent);
			}
			else if (physicsComponent != null)
			{
				HandleParentChangeVelocity(uid, physicsComponent, oldParent, transformComponent2);
			}
		}
	}

	private void HandleMapChange(EntityUid uid, TransformComponent xform, PhysicsComponent? body)
	{
		RecursiveMapUpdate(uid, xform, body);
	}

	private void RecursiveMapUpdate(EntityUid uid, TransformComponent xform, PhysicsComponent? body)
	{
		_joints.ClearJoints(uid);
		foreach (EntityUid child in xform._children)
		{
			if (XformQuery.TryGetComponent(child, out TransformComponent component))
			{
				PhysicsQuery.TryGetComponent(child, out PhysicsComponent component2);
				RecursiveMapUpdate(child, component, component2);
			}
		}
	}

	private void OnGridAdd(GridAddEvent ev)
	{
		EntityUid entityUid = ev.EntityUid;
		if (!HasComp<MapComponent>(entityUid))
		{
			PhysicsComponent body = EnsureComp<PhysicsComponent>(entityUid);
			FixturesComponent manager = EnsureComp<FixturesComponent>(entityUid);
			SetCanCollide(entityUid, value: true, dirty: true, force: false, manager, body);
			SetBodyType(entityUid, BodyType.Static, manager, body);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		ShutdownContacts();
	}

	private void HandleContainerRemoved(EntityUid uid, PhysicsComponent physics, EntGotRemovedFromContainerMessage message)
	{
		if ((int)MetaData(uid).EntityLifeStage < 4 && (!_anchorQuery.TryGetComponent(uid, out CollideOnAnchorComponent component) || !component.Enable))
		{
			WakeBody(uid, force: false, null, physics);
		}
	}

	protected void SimulateWorld(float deltaTime, bool prediction)
	{
		float num = deltaTime / (float)_substeps;
		EffectiveCurTime = _gameTiming.CurTime;
		for (int i = 0; i < _substeps; i++)
		{
			PhysicsUpdateBeforeSolveEvent message = new PhysicsUpdateBeforeSolveEvent(prediction, num);
			RaiseLocalEvent(ref message);
			_broadphase.FindNewContacts();
			CollideContacts();
			Step(num, prediction);
			PhysicsUpdateAfterSolveEvent message2 = new PhysicsUpdateAfterSolveEvent(prediction, num);
			RaiseLocalEvent(ref message2);
			if (i == _substeps - 1)
			{
				FinalStep();
			}
			EffectiveCurTime = EffectiveCurTime.Value + TimeSpan.FromSeconds(num);
		}
		EffectiveCurTime = null;
	}

	protected virtual void FinalStep()
	{
	}

	public void SetDensity(EntityUid uid, string fixtureId, Fixture fixture, float value, bool update = true, FixturesComponent? manager = null)
	{
		if (!fixture.Density.Equals(value) && Resolve(uid, ref manager))
		{
			fixture.Density = value;
			if (update)
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager);
			}
		}
	}

	public void SetFriction(EntityUid uid, string fixtureId, Fixture fixture, float value, bool update = true, FixturesComponent? manager = null)
	{
		if (!fixture.Friction.Equals(value) && Resolve(uid, ref manager))
		{
			fixture.Friction = value;
			if (update)
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager);
			}
		}
	}

	public void SetHard(EntityUid uid, Fixture fixture, bool value, FixturesComponent? manager = null)
	{
		if (!fixture.Hard.Equals(value) && Resolve(uid, ref manager))
		{
			fixture.Hard = value;
			_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager);
			WakeBody(uid);
		}
	}

	public void SetRestitution(EntityUid uid, Fixture fixture, float value, bool update = true, FixturesComponent? manager = null)
	{
		if (!fixture.Restitution.Equals(value) && Resolve(uid, ref manager))
		{
			fixture.Restitution = value;
			if (update)
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager);
			}
		}
	}

	public void ScaleFixtures(Entity<FixturesComponent?> ent, float factor)
	{
		if (!Resolve(ent, ref ent.Comp))
		{
			return;
		}
		foreach (KeyValuePair<string, Fixture> fixture2 in ent.Comp.Fixtures)
		{
			fixture2.Deconstruct(out var key, out var value);
			string fixtureId = key;
			Fixture fixture = value;
			IPhysShape shape = fixture.Shape;
			if (!(shape is EdgeShape edgeShape))
			{
				if (!(shape is PhysShapeCircle physShapeCircle))
				{
					if (!(shape is PolygonShape polygonShape))
					{
						throw new NotImplementedException();
					}
					Vector2[] vertices = polygonShape.Vertices;
					for (int i = 0; i < polygonShape.VertexCount; i++)
					{
						vertices[i] *= factor;
					}
					SetVertices(ent, fixtureId, fixture, polygonShape, vertices, ent.Comp);
				}
				else
				{
					SetPositionRadius(ent, fixtureId, fixture, physShapeCircle, physShapeCircle.Position * factor, physShapeCircle.Radius * factor, ent.Comp);
				}
			}
			else
			{
				SetVertices(ent, fixtureId, fixture, edgeShape, edgeShape.Vertex0 * factor, edgeShape.Vertex1 * factor, edgeShape.Vertex2 * factor, edgeShape.Vertex3 * factor, ent.Comp);
			}
		}
	}

	public bool IsCurrentlyHardCollidable(Entity<FixturesComponent?, PhysicsComponent?> bodyA, Entity<FixturesComponent?, PhysicsComponent?> bodyB)
	{
		if (!_fixturesQuery.Resolve(bodyA, ref bodyA.Comp1, logMissing: false) || !_fixturesQuery.Resolve(bodyB, ref bodyB.Comp1, logMissing: false) || !PhysicsQuery.Resolve(bodyA, ref bodyA.Comp2, logMissing: false) || !PhysicsQuery.Resolve(bodyB, ref bodyB.Comp2, logMissing: false))
		{
			return false;
		}
		if (!bodyA.Comp2.CanCollide || !bodyB.Comp2.CanCollide)
		{
			return false;
		}
		return IsHardCollidable(bodyA, bodyB);
	}

	public bool IsHardCollidable(Entity<FixturesComponent?, PhysicsComponent?> bodyA, Entity<FixturesComponent?, PhysicsComponent?> bodyB)
	{
		if (!_fixturesQuery.Resolve(bodyA, ref bodyA.Comp1, logMissing: false) || !_fixturesQuery.Resolve(bodyB, ref bodyB.Comp1, logMissing: false) || !PhysicsQuery.Resolve(bodyA, ref bodyA.Comp2, logMissing: false) || !PhysicsQuery.Resolve(bodyB, ref bodyB.Comp2, logMissing: false))
		{
			return false;
		}
		if (!bodyA.Comp2.Hard || !bodyB.Comp2.Hard || ((bodyA.Comp2.CollisionLayer & bodyB.Comp2.CollisionMask) == 0 && (bodyA.Comp2.CollisionMask & bodyB.Comp2.CollisionLayer) == 0))
		{
			return false;
		}
		foreach (Fixture value in bodyA.Comp1.Fixtures.Values)
		{
			if (!value.Hard)
			{
				continue;
			}
			foreach (Fixture value2 in bodyB.Comp1.Fixtures.Values)
			{
				if (value2.Hard && ((value.CollisionLayer & value2.CollisionMask) != 0 || (value.CollisionMask & value2.CollisionLayer) != 0))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddCollisionMask(EntityUid uid, string fixtureId, Fixture fixture, int mask, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if ((fixture.CollisionMask & mask) != mask && Resolve(uid, ref manager))
		{
			fixture.CollisionMask |= mask;
			if (body != null || TryComp(uid, out body))
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
			}
			_broadphase.Refilter(uid, fixture);
		}
	}

	public void SetCollisionMask(EntityUid uid, string fixtureId, Fixture fixture, int mask, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (fixture.CollisionMask != mask && Resolve(uid, ref manager))
		{
			fixture.CollisionMask = mask;
			if (body != null || TryComp(uid, out body))
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
			}
			_broadphase.Refilter(uid, fixture);
		}
	}

	public void RemoveCollisionMask(EntityUid uid, string fixtureId, Fixture fixture, int mask, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if ((fixture.CollisionMask & mask) != 0 && Resolve(uid, ref manager))
		{
			fixture.CollisionMask &= ~mask;
			if (body != null || TryComp(uid, out body))
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
			}
			_broadphase.Refilter(uid, fixture);
		}
	}

	public void AddCollisionLayer(EntityUid uid, string fixtureId, Fixture fixture, int layer, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if ((fixture.CollisionLayer & layer) != layer && Resolve(uid, ref manager))
		{
			fixture.CollisionLayer |= layer;
			if (body != null || TryComp(uid, out body))
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
			}
			_broadphase.Refilter(uid, fixture);
		}
	}

	public void SetCollisionLayer(EntityUid uid, string fixtureId, Fixture fixture, int layer, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!fixture.CollisionLayer.Equals(layer) && Resolve(uid, ref manager))
		{
			fixture.CollisionLayer = layer;
			if (body != null || TryComp(uid, out body))
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
			}
			_broadphase.Refilter(uid, fixture);
		}
	}

	public void RemoveCollisionLayer(EntityUid uid, string fixtureId, Fixture fixture, int layer, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if ((fixture.CollisionLayer & layer) != 0 && Resolve(uid, ref manager))
		{
			fixture.CollisionLayer &= ~layer;
			if (body != null || TryComp(uid, out body))
			{
				_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
			}
			_broadphase.Refilter(uid, fixture);
		}
	}

	private void InitializeIsland()
	{
		base.Subs.CVar(_cfg, CVars.NetTickrate, SetTickRate, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.WarmStarting, SetWarmStarting, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.MaxLinearCorrection, SetMaxLinearCorrection, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.MaxAngularCorrection, SetMaxAngularCorrection, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.VelocityIterations, SetVelocityIterations, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.PositionIterations, SetPositionIterations, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.MaxLinVelocity, SetMaxLinearVelocity, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.MaxAngVelocity, SetMaxAngularVelocity, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.SleepAllowed, SetSleepAllowed, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.AngularSleepTolerance, SetAngularToleranceSqr, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.LinearSleepTolerance, SetLinearToleranceSqr, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.TimeToSleep, SetTimeToSleep, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.VelocityThreshold, SetVelocityThreshold, invokeImmediately: true);
		base.Subs.CVar(_cfg, CVars.Baumgarte, SetBaumgarte, invokeImmediately: true);
	}

	private void SetWarmStarting(bool value)
	{
		_warmStarting = value;
	}

	private void SetMaxLinearCorrection(float value)
	{
		_maxLinearCorrection = value;
	}

	private void SetMaxAngularCorrection(float value)
	{
		_maxAngularCorrection = value;
	}

	private void SetVelocityIterations(int value)
	{
		_velocityIterations = value;
	}

	private void SetPositionIterations(int value)
	{
		_positionIterations = value;
	}

	private void SetMaxLinearVelocity(float value)
	{
		_maxLinearVelocity = value;
		UpdateMaxTranslation();
	}

	private void SetMaxAngularVelocity(float value)
	{
		_maxAngularVelocity = value;
		UpdateMaxRotation();
	}

	private void SetTickRate(int value)
	{
		_tickRate = value;
		UpdateMaxTranslation();
		UpdateMaxRotation();
	}

	private void SetSleepAllowed(bool value)
	{
		_sleepAllowed = value;
	}

	private void SetAngularToleranceSqr(float value)
	{
		AngularToleranceSqr = value;
	}

	private void SetLinearToleranceSqr(float value)
	{
		LinearToleranceSqr = value;
	}

	private void SetTimeToSleep(float value)
	{
		TimeToSleep = value;
	}

	private void SetVelocityThreshold(float value)
	{
		_velocityThreshold = value;
	}

	private void SetBaumgarte(float value)
	{
		_baumgarte = value;
	}

	private void UpdateMaxTranslation()
	{
		_maxTranslationPerTick = _maxLinearVelocity / (float)_tickRate;
	}

	private void UpdateMaxRotation()
	{
		_maxRotationPerTick = (float)Math.PI * 2f * _maxAngularVelocity / (float)_tickRate;
	}

	public void Step(float frameTime, bool prediction)
	{
		float num = ((frameTime > 0f) ? (1f / frameTime) : 0f);
		float dtRatio = _invDt0 * frameTime;
		Solve(frameTime, dtRatio, num, prediction);
		if (_autoClearForces)
		{
			ClearForces();
		}
		_invDt0 = num;
	}

	private void ClearForces()
	{
		foreach (Entity<PhysicsComponent, TransformComponent> awakeBody in AwakeBodies)
		{
			EntityUid owner = awakeBody.Owner;
			PhysicsComponent comp = awakeBody.Comp1;
			if (comp.Force != Vector2.Zero)
			{
				comp.Force = Vector2.Zero;
				DirtyField(owner, comp, "Force");
			}
			if (comp.Torque != 0f)
			{
				comp.Torque = 0f;
				DirtyField(owner, comp, "Torque");
			}
		}
	}

	private void Solve(float frameTime, float dtRatio, float invDt, bool prediction)
	{
		_bodyStack.EnsureCapacity(AwakeBodies.Count);
		_islandSet.EnsureCapacity(AwakeBodies.Count);
		_awakeBodyList.AddRange(AwakeBodies);
		int num = 0;
		IslandData island = new IslandData(num++, LoneIsland: true, _islandBodyPool.Get(), _islandContactPool.Get(), _islandJointPool.Get(), new List<(Joint, float)>());
		List<IslandData> list = new List<IslandData>();
		List<(Joint, Joint)> list2 = new List<(Joint, Joint)>();
		foreach (Entity<PhysicsComponent, TransformComponent> awakeBody in _awakeBodyList)
		{
			TransformComponent comp = awakeBody.Comp2;
			PhysicsComponent comp2 = awakeBody.Comp1;
			if (comp2.Island)
			{
				continue;
			}
			EntityUid owner = comp2.Owner;
			EntityUid? mapUid = comp.MapUid;
			if (!mapUid.HasValue)
			{
				continue;
			}
			if (!EntityManager.MetaQuery.TryGetComponent(owner, out MetaDataComponent component))
			{
				base.Log.Error($"Found deleted entity {ToPrettyString(owner)} on map!");
				RemoveSleepBody(awakeBody);
			}
			else
			{
				if ((component.EntityPaused && !comp2.IgnorePaused) || (prediction && !comp2.Predict) || !comp2.CanCollide || comp2.BodyType == BodyType.Static)
				{
					continue;
				}
				List<Entity<PhysicsComponent, TransformComponent>> list3 = _islandBodyPool.Get();
				List<Contact> list4 = _islandContactPool.Get();
				List<(Joint, Joint)> list5 = _islandJointPool.Get();
				_bodyStack.Push(awakeBody);
				comp2.Island = true;
				Entity<PhysicsComponent, TransformComponent> result;
				while (_bodyStack.TryPop(out result))
				{
					EntityUid owner2 = result.Owner;
					PhysicsComponent comp3 = result.Comp1;
					list3.Add(result);
					_islandSet.Add(result);
					if (comp3.BodyType == BodyType.Static)
					{
						continue;
					}
					SetAwake(owner2, comp3, value: true, updateSleepTime: false);
					LinkedListNode<Contact> linkedListNode = comp3.Contacts.First;
					while (linkedListNode != null)
					{
						Contact value = linkedListNode.Value;
						linkedListNode = linkedListNode.Next;
						if ((value.Flags & (ContactFlags.PreInit | ContactFlags.Island)) != ContactFlags.None || !value.Enabled || !value.IsTouching)
						{
							continue;
						}
						Fixture? fixtureA = value.FixtureA;
						if (fixtureA == null || !fixtureA.Hard)
						{
							continue;
						}
						Fixture? fixtureB = value.FixtureB;
						if (fixtureB != null && fixtureB.Hard)
						{
							list4.Add(value);
							value.Flags |= ContactFlags.Island;
							PhysicsComponent physicsComponent = value.OtherBody(owner2);
							if (!physicsComponent.Island)
							{
								EntityUid owner3 = value.OtherEnt(owner2);
								TransformComponent comp4 = value.OtherTransform(owner2);
								_bodyStack.Push(new Entity<PhysicsComponent, TransformComponent>(owner3, physicsComponent, comp4));
								physicsComponent.Island = true;
							}
						}
					}
					if (RelayTargetQuery.TryGetComponent(owner2, out JointRelayTargetComponent component2))
					{
						foreach (EntityUid item6 in component2.Relayed)
						{
							if (!JointQuery.TryGetComponent(item6, out JointComponent component3))
							{
								continue;
							}
							foreach (Joint value2 in component3.GetJoints.Values)
							{
								if (!value2.IslandFlag)
								{
									EntityUid entityUid = value2.BodyAUid;
									EntityUid entityUid2 = value2.BodyBUid;
									if (JointQuery.TryGetComponent(entityUid, out JointComponent component4) && component4.Relay.HasValue)
									{
										entityUid = component4.Relay.Value;
									}
									if (JointQuery.TryGetComponent(entityUid2, out JointComponent component5) && component5.Relay.HasValue)
									{
										entityUid2 = component5.Relay.Value;
									}
									Joint item = value2.Clone(entityUid, entityUid2);
									list2.Add((value2, item));
									value2.IslandFlag = true;
								}
							}
						}
					}
					if (JointQuery.TryGetComponent(owner2, out JointComponent component6) && !component6.Relay.HasValue)
					{
						foreach (Joint value3 in component6.Joints.Values)
						{
							if (!value3.IslandFlag)
							{
								EntityUid entityUid3 = value3.BodyAUid;
								EntityUid entityUid4 = value3.BodyBUid;
								if (JointQuery.TryGetComponent(entityUid3, out JointComponent component7) && component7.Relay.HasValue)
								{
									entityUid3 = component7.Relay.Value;
								}
								if (JointQuery.TryGetComponent(entityUid4, out JointComponent component8) && component8.Relay.HasValue)
								{
									entityUid4 = component8.Relay.Value;
								}
								Joint item2 = value3.Clone(entityUid3, entityUid4);
								list2.Add((value3, item2));
								value3.IslandFlag = true;
							}
						}
					}
					foreach (var item7 in list2)
					{
						Joint item3 = item7.Item1;
						Joint item4 = item7.Item2;
						PhysicsComponent component9 = PhysicsQuery.GetComponent(item4.BodyAUid);
						PhysicsComponent component10 = PhysicsQuery.GetComponent(item4.BodyBUid);
						if (component9.CanCollide && component10.CanCollide)
						{
							list5.Add((item3, item4));
							if (!component9.Island)
							{
								_bodyStack.Push(new Entity<PhysicsComponent, TransformComponent>(item4.BodyAUid, component9, XformQuery.GetComponent(item4.BodyAUid)));
								component9.Island = true;
							}
							if (!component10.Island)
							{
								_bodyStack.Push(new Entity<PhysicsComponent, TransformComponent>(item4.BodyBUid, component10, XformQuery.GetComponent(item4.BodyBUid)));
								component10.Island = true;
							}
						}
					}
					list2.Clear();
				}
				int index;
				if (list4.Count == 0 && list5.Count == 0)
				{
					island.MapUid = mapUid.Value;
					island.Bodies.Add(list3[0]);
					index = island.Index;
				}
				else
				{
					IslandData islandData = new IslandData(num++, LoneIsland: false, list3, list4, list5, new List<(Joint, float)>());
					islandData.MapUid = mapUid.Value;
					IslandData item5 = islandData;
					list.Add(item5);
					index = item5.Index;
				}
				for (int i = 0; i < list3.Count; i++)
				{
					PhysicsComponent comp5 = list3[i].Comp1;
					if (comp5.BodyType == BodyType.Static)
					{
						comp5.Island = false;
					}
					comp5.IslandIndex[index] = i;
				}
			}
		}
		if (island.Bodies.Count > 0)
		{
			list.Add(island);
		}
		else
		{
			ReturnIsland(in island);
		}
		SolveIslands(list, frameTime, dtRatio, invDt, prediction);
		foreach (IslandData item8 in list)
		{
			ReturnIsland(item8);
		}
		Cleanup(frameTime);
	}

	private void ReturnIsland(in IslandData island)
	{
		foreach (Entity<PhysicsComponent, TransformComponent> body in island.Bodies)
		{
			body.Comp1.IslandIndex.Remove(island.Index);
		}
		_islandBodyPool.Return(island.Bodies);
		_islandContactPool.Return(island.Contacts);
		foreach (var (joint, joint2) in island.Joints)
		{
			if (joint != joint2)
			{
				joint2.CopyTo(joint);
			}
			joint.IslandFlag = false;
		}
		_islandJointPool.Return(island.Joints);
		island.BrokenJoints.Clear();
	}

	protected virtual void Cleanup(float frameTime)
	{
		foreach (Entity<PhysicsComponent, TransformComponent> item in _islandSet)
		{
			PhysicsComponent comp = item.Comp1;
			if (comp.Island && !comp.Deleted)
			{
				comp.Island = false;
			}
		}
		_islandSet.Clear();
		_islandSet.Clear();
		_awakeBodyList.Clear();
	}

	private void SolveIslands(List<IslandData> islands, float frameTime, float dtRatio, float invDt, bool prediction)
	{
		int num = 0;
		SolverData data = new SolverData(frameTime, dtRatio, invDt, _warmStarting, _maxLinearCorrection, _maxAngularCorrection, _velocityIterations, _positionIterations, _maxLinearVelocity, _maxAngularVelocity, _maxTranslationPerTick, _maxRotationPerTick, _sleepAllowed, AngularToleranceSqr, LinearToleranceSqr, TimeToSleep, _velocityThreshold, _baumgarte);
		islands.Sort((IslandData x, IslandData y) => InternalParallel(y).CompareTo(InternalParallel(x)));
		int num2 = 0;
		IslandData[] actualIslands = islands.ToArray();
		for (int num3 = 0; num3 < islands.Count; num3++)
		{
			ref IslandData reference = ref actualIslands[num3];
			reference.Offset = num2;
			UpdateLerpData(reference.Bodies);
			num2 += reference.Bodies.Count;
		}
		Vector2[] solvedPositions = ArrayPool<Vector2>.Shared.Rent(num2);
		float[] solvedAngles = ArrayPool<float>.Shared.Rent(num2);
		Vector2[] linearVelocities = ArrayPool<Vector2>.Shared.Rent(num2);
		float[] angularVelocities = ArrayPool<float>.Shared.Rent(num2);
		bool[] sleepStatus = ArrayPool<bool>.Shared.Rent(num2);
		for (int num4 = 0; num4 < num2; num4++)
		{
			sleepStatus[num4] = false;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = _parallel.ParallelProcessCount
		};
		for (; num < actualIslands.Length; num++)
		{
			ref IslandData reference2 = ref actualIslands[num];
			if (!InternalParallel(reference2))
			{
				break;
			}
			SolveIsland(ref reference2, in data, parallelOptions, prediction, solvedPositions, solvedAngles, linearVelocities, angularVelocities, sleepStatus);
		}
		Parallel.For(num, actualIslands.Length, parallelOptions, delegate(int i)
		{
			ref IslandData island2 = ref actualIslands[i];
			SolveIsland(ref island2, in data, null, prediction, solvedPositions, solvedAngles, linearVelocities, angularVelocities, sleepStatus);
		});
		for (int num5 = 0; num5 < actualIslands.Length; num5++)
		{
			ref IslandData island = ref actualIslands[num5];
			UpdateBodies(in island, solvedPositions, solvedAngles, linearVelocities, angularVelocities);
			SleepBodies(in island, sleepStatus);
		}
		ArrayPool<Vector2>.Shared.Return(solvedPositions);
		ArrayPool<float>.Shared.Return(solvedAngles);
		ArrayPool<Vector2>.Shared.Return(linearVelocities);
		ArrayPool<float>.Shared.Return(angularVelocities);
		ArrayPool<bool>.Shared.Return(sleepStatus);
	}

	protected virtual void UpdateLerpData(List<Entity<PhysicsComponent, TransformComponent>> bodies)
	{
	}

	private static bool InternalParallel(IslandData island)
	{
		if (island.Bodies.Count <= 128 && island.Contacts.Count <= 128)
		{
			return island.Joints.Count > 128;
		}
		return true;
	}

	private void SolveIsland(ref IslandData island, in SolverData data, ParallelOptions? options, bool prediction, Vector2[] solvedPositions, float[] solvedAngles, Vector2[] linearVelocities, float[] angularVelocities, bool[] sleepStatus)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		int count = island.Bodies.Count;
		Vector2[] array = ArrayPool<Vector2>.Shared.Rent(count);
		float[] array2 = ArrayPool<float>.Shared.Rent(count);
		int offset = island.Offset;
		Vector2 gravity = Gravity;
		for (int i = 0; i < island.Bodies.Count; i++)
		{
			Entity<PhysicsComponent, TransformComponent> entity = island.Bodies[i];
			PhysicsComponent comp = entity.Comp1;
			TransformComponent comp2 = entity.Comp2;
			(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(comp2);
			Vector2 item = worldPositionRotation.WorldPosition;
			Angle item2 = worldPositionRotation.WorldRotation;
			Transform transform = new Transform(item, item2);
			Vector2 vector = Robust.Shared.Physics.Transform.Mul(in transform, comp.LocalCenter);
			float angle = transform.Quaternion2D.Angle;
			Vector2 linearVelocity = comp.LinearVelocity;
			float num = comp.AngularVelocity;
			if (comp.BodyType == BodyType.Dynamic)
			{
				if (comp.IgnoreGravity)
				{
					linearVelocity += comp.Force * data.FrameTime * comp.InvMass;
				}
				else
				{
					linearVelocity += (gravity + comp.Force * comp.InvMass) * data.FrameTime;
				}
				num += comp.InvI * comp.Torque * data.FrameTime;
				linearVelocity *= Math.Clamp(1f - data.FrameTime * comp.LinearDamping, 0f, 1f);
				num *= Math.Clamp(1f - data.FrameTime * comp.AngularDamping, 0f, 1f);
			}
			array[i] = vector;
			array2[i] = angle;
			linearVelocities[i + offset] = linearVelocity;
			angularVelocities[i + offset] = num;
		}
		int count2 = island.Contacts.Count;
		ContactVelocityConstraint[] array3 = ArrayPool<ContactVelocityConstraint>.Shared.Rent(count2);
		ContactPositionConstraint[] array4 = ArrayPool<ContactPositionConstraint>.Shared.Rent(count2);
		ResetSolver(in data, in island, array3, array4);
		InitializeVelocityConstraints(in data, in island, array3, array4, array, array2, linearVelocities, angularVelocities);
		if (data.WarmStarting)
		{
			WarmStart(in data, in island, array3, linearVelocities, angularVelocities);
		}
		int count3 = island.Joints.Count;
		if (count3 > 0)
		{
			for (int j = 0; j < island.Joints.Count; j++)
			{
				Joint item3 = island.Joints[j].Joint;
				if (item3.Enabled)
				{
					PhysicsComponent component = PhysicsQuery.GetComponent(item3.BodyAUid);
					PhysicsComponent component2 = PhysicsQuery.GetComponent(item3.BodyBUid);
					item3.InitVelocityConstraints(in data, in island, component, component2, array, array2, linearVelocities, angularVelocities);
				}
			}
		}
		for (int k = 0; k < data.VelocityIterations; k++)
		{
			for (int l = 0; l < count3; l++)
			{
				Joint item4 = island.Joints[l].Joint;
				if (item4.Enabled)
				{
					item4.SolveVelocityConstraints(in data, in island, linearVelocities, angularVelocities);
					float num2 = item4.Validate(data.InvDt);
					if (num2 > 0f)
					{
						island.BrokenJoints.Add((island.Joints[l].Original, num2));
					}
				}
			}
			SolveVelocityConstraints(in island, options, array3, linearVelocities, angularVelocities);
		}
		StoreImpulses(in island, array3);
		float num3 = data.MaxTranslation / data.FrameTime;
		float num4 = num3 * num3;
		float num5 = data.MaxRotation / data.FrameTime;
		float num6 = num5 * num5;
		for (int m = 0; m < count; m++)
		{
			Vector2 vector2 = linearVelocities[offset + m];
			float num7 = angularVelocities[offset + m];
			float num8 = vector2.LengthSquared();
			if (num8 > num4)
			{
				vector2 *= num3 / MathF.Sqrt(num8);
				linearVelocities[offset + m] = vector2;
			}
			if (num7 * num7 > num6)
			{
				num7 = (angularVelocities[offset + m] = num7 * (num5 / MathF.Abs(num7)));
			}
			array[m] += vector2 * data.FrameTime;
			array2[m] += num7 * data.FrameTime;
		}
		island.PositionSolved = false;
		for (int n = 0; n < data.PositionIterations; n++)
		{
			bool flag = SolvePositionConstraints(in data, in island, options, array4, array, array2);
			bool flag2 = true;
			for (int num9 = 0; num9 < island.Joints.Count; num9++)
			{
				Joint item5 = island.Joints[num9].Joint;
				if (item5.Enabled)
				{
					bool flag3 = item5.SolvePositionConstraints(in data, array, array2);
					flag2 = flag2 && flag3;
				}
			}
			if (flag && flag2)
			{
				island.PositionSolved = true;
				break;
			}
		}
		List<Entity<PhysicsComponent, TransformComponent>> bodies = island.Bodies;
		if (options != null)
		{
			ProcessParallelInternal(this, options, count, offset, bodies, array, array2, solvedPositions, solvedAngles);
		}
		else
		{
			FinalisePositions(0, count, offset, bodies, array, array2, solvedPositions, solvedAngles);
		}
		if (island.LoneIsland)
		{
			if (!prediction && data.SleepAllowed)
			{
				for (int num10 = 0; num10 < count; num10++)
				{
					PhysicsComponent comp3 = island.Bodies[num10].Comp1;
					if (comp3.BodyType != BodyType.Static)
					{
						if (!comp3.SleepingAllowed || comp3.AngularVelocity * comp3.AngularVelocity > data.AngTolSqr || Vector2.Dot(comp3.LinearVelocity, comp3.LinearVelocity) > data.LinTolSqr)
						{
							SetSleepTime(comp3, 0f);
						}
						else
						{
							SetSleepTime(comp3, comp3.SleepTime + data.FrameTime);
						}
						if (comp3.SleepTime >= data.TimeToSleep && island.PositionSolved)
						{
							sleepStatus[offset + num10] = true;
						}
					}
				}
			}
		}
		else if (!prediction && data.SleepAllowed)
		{
			float num11 = float.MaxValue;
			for (int num12 = 0; num12 < count; num12++)
			{
				PhysicsComponent comp4 = island.Bodies[num12].Comp1;
				if (comp4.BodyType != BodyType.Static)
				{
					if (!comp4.SleepingAllowed || comp4.AngularVelocity * comp4.AngularVelocity > data.AngTolSqr || Vector2.Dot(comp4.LinearVelocity, comp4.LinearVelocity) > data.LinTolSqr)
					{
						SetSleepTime(comp4, 0f);
						num11 = 0f;
					}
					else
					{
						SetSleepTime(comp4, comp4.SleepTime + data.FrameTime);
						num11 = MathF.Min(num11, comp4.SleepTime);
					}
				}
			}
			if (num11 >= data.TimeToSleep && island.PositionSolved)
			{
				for (int num13 = 0; num13 < island.Bodies.Count; num13++)
				{
					sleepStatus[offset + num13] = true;
				}
			}
		}
		ArrayPool<Vector2>.Shared.Return(array);
		ArrayPool<float>.Shared.Return(array2);
		ArrayPool<ContactVelocityConstraint>.Shared.Return(array3);
		ArrayPool<ContactPositionConstraint>.Shared.Return(array4);
		static void ProcessParallelInternal(SharedPhysicsSystem system, ParallelOptions parallelOptions, int bodyCount, int offset2, List<Entity<PhysicsComponent, TransformComponent>> bodies2, Vector2[] positions, float[] angles, Vector2[] solvedPositions2, float[] solvedAngles2)
		{
			int toExclusive = (int)MathF.Ceiling((float)bodyCount / 32f);
			Parallel.For(0, toExclusive, parallelOptions, delegate(int num15)
			{
				int num14 = num15 * 32;
				int end = Math.Min(bodyCount, num14 + 32);
				system.FinalisePositions(num14, end, offset2, bodies2, positions, angles, solvedPositions2, solvedAngles2);
			});
		}
	}

	private void FinalisePositions(int start, int end, int offset, List<Entity<PhysicsComponent, TransformComponent>> bodies, Vector2[] positions, float[] angles, Vector2[] solvedPositions, float[] solvedAngles)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		for (int i = start; i < end; i++)
		{
			Entity<PhysicsComponent, TransformComponent> entity = bodies[i];
			PhysicsComponent comp = entity.Comp1;
			if (comp.BodyType != BodyType.Static)
			{
				TransformComponent comp2 = entity.Comp2;
				if (TryComp(comp2.ParentUid, out TransformComponent comp3))
				{
					(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) worldPositionRotationInvMatrix = _transform.GetWorldPositionRotationInvMatrix(comp3);
					Angle item = worldPositionRotationInvMatrix.WorldRotation;
					Matrix3x2 item2 = worldPositionRotationInvMatrix.InvWorldMatrix;
					float num = (float)Angle.op_Implicit(item + comp2._localRotation);
					float num2 = angles[i];
					Quaternion2D quaternion2D = new Quaternion2D(num2);
					Vector2 vector = Vector2.Transform(positions[i] - Robust.Shared.Physics.Transform.Mul(in quaternion2D, comp.LocalCenter), item2);
					solvedPositions[offset + i] = vector - comp2.LocalPosition;
					solvedAngles[offset + i] = num2 - num;
				}
			}
		}
	}

	private void UpdateBodies(in IslandData island, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities)
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		foreach (var brokenJoint in island.BrokenJoints)
		{
			Joint item = brokenJoint.Joint;
			float item2 = brokenJoint.Error;
			JointBreakEvent args = new JointBreakEvent(item, MathF.Sqrt(item2));
			RaiseLocalEvent(item.BodyAUid, ref args);
			RaiseLocalEvent(item.BodyBUid, ref args);
			RaiseLocalEvent(ref args);
			item.Dirty();
		}
		int offset = island.Offset;
		for (int i = 0; i < island.Bodies.Count; i++)
		{
			Entity<PhysicsComponent, TransformComponent> entity = island.Bodies[i];
			PhysicsComponent comp = entity.Comp1;
			if (comp.BodyType != BodyType.Static)
			{
				EntityUid owner = entity.Owner;
				Vector2 vector = positions[offset + i];
				float num = angles[offset + i];
				TransformComponent comp2 = entity.Comp2;
				Vector2 velocity = linearVelocities[offset + i];
				bool flag = false;
				if (!float.IsNaN(velocity.X) && !float.IsNaN(velocity.Y))
				{
					flag |= SetLinearVelocity(owner, velocity, dirty: false, wakeBody: true, null, comp);
				}
				float num2 = angularVelocities[offset + i];
				if (!float.IsNaN(num2))
				{
					flag |= SetAngularVelocity(owner, num2, dirty: false, null, comp);
				}
				if (!float.IsNaN(vector.X) && !float.IsNaN(vector.Y))
				{
					_transform.SetLocalPositionRotation(owner, comp2.LocalPosition + vector, comp2.LocalRotation + Angle.op_Implicit(num), comp2);
				}
				if (flag)
				{
					Dirty(owner, comp);
				}
			}
		}
	}

	private void SleepBodies(in IslandData island, bool[] sleepStatus)
	{
		int offset = island.Offset;
		for (int i = 0; i < island.Bodies.Count; i++)
		{
			if (sleepStatus[offset + i])
			{
				Entity<PhysicsComponent, TransformComponent> entity = island.Bodies[i];
				SetAwake(entity.Owner, entity, value: false);
			}
		}
	}

	internal void AddAwakeBody(Entity<PhysicsComponent, TransformComponent> ent)
	{
		PhysicsComponent comp = ent.Comp1;
		if (!comp.CanCollide)
		{
			base.Log.Error($"Tried to add non-colliding {ToPrettyString(ent)} as an awake body to map!");
		}
		else if (comp.BodyType == BodyType.Static)
		{
			base.Log.Error($"Tried to add static body {ToPrettyString(ent)} as an awake body to map!");
		}
		else
		{
			AwakeBodies.Add(ent);
		}
	}

	internal void RemoveSleepBody(Entity<PhysicsComponent, TransformComponent> ent)
	{
		AwakeBodies.Remove(ent);
	}

	public bool TryCollideRect(Box2 collider, MapId mapId, bool approximate = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		(Box2 collider, MapId mapId, bool found) state = (collider: collider, mapId: mapId, found: false);
		_broadphase.GetBroadphases(mapId, collider, delegate(Entity<BroadphaseComponent> broadphase)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			Box2 gridCollider = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(broadphase), ref collider);
			broadphase.Comp.StaticTree.QueryAabb<(Box2, MapId, bool)>(ref state, delegate(ref (Box2 collider, MapId map, bool found) reference, in FixtureProxy proxy)
			{
				if (proxy.Fixture.CollisionLayer == 0)
				{
					return true;
				}
				if (((Box2)(ref proxy.AABB)).Intersects(ref gridCollider))
				{
					reference.found = true;
					return false;
				}
				return true;
			}, gridCollider, approximate);
			broadphase.Comp.DynamicTree.QueryAabb<(Box2, MapId, bool)>(ref state, delegate(ref (Box2 collider, MapId map, bool found) reference, in FixtureProxy proxy)
			{
				if (proxy.Fixture.CollisionLayer == 0)
				{
					return true;
				}
				if (((Box2)(ref proxy.AABB)).Intersects(ref gridCollider))
				{
					reference.found = true;
					return false;
				}
				return true;
			}, gridCollider, approximate);
		});
		return state.found;
	}

	public HashSet<EntityUid> GetEntitiesIntersectingBody(EntityUid uid, int collisionMask, bool approximate = true, PhysicsComponent? body = null, FixturesComponent? fixtureComp = null, TransformComponent? xform = null)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		if (!Resolve(uid, ref body, ref fixtureComp, ref xform, logMissing: false))
		{
			return hashSet;
		}
		if (!_lookup.TryGetCurrentBroadphase(xform, out BroadphaseComponent broadphase))
		{
			return hashSet;
		}
		(PhysicsComponent, HashSet<EntityUid>) state = (body, hashSet);
		foreach (Fixture value in fixtureComp.Fixtures.Values)
		{
			FixtureProxy[] proxies = value.Proxies;
			foreach (FixtureProxy fixtureProxy in proxies)
			{
				broadphase.StaticTree.QueryAabb<(PhysicsComponent, HashSet<EntityUid>)>(ref state, delegate(ref (PhysicsComponent body, HashSet<EntityUid> entities) reference, in FixtureProxy other)
				{
					if (other.Body.Deleted || other.Body == body)
					{
						return true;
					}
					if ((collisionMask & other.Fixture.CollisionLayer) == 0)
					{
						return true;
					}
					reference.entities.Add(other.Entity);
					return true;
				}, fixtureProxy.AABB, approximate);
				broadphase.DynamicTree.QueryAabb<(PhysicsComponent, HashSet<EntityUid>)>(ref state, delegate(ref (PhysicsComponent body, HashSet<EntityUid> entities) reference, in FixtureProxy other)
				{
					if (other.Body.Deleted || other.Body == body)
					{
						return true;
					}
					if ((collisionMask & other.Fixture.CollisionLayer) == 0)
					{
						return true;
					}
					reference.entities.Add(other.Entity);
					return true;
				}, fixtureProxy.AABB, approximate);
			}
		}
		return hashSet;
	}

	[Obsolete("Use EntityLookupSystem")]
	public IEnumerable<PhysicsComponent> GetCollidingEntities(MapId mapId, in Box2 worldAABB)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace)
		{
			return Array.Empty<PhysicsComponent>();
		}
		Box2 item = worldAABB;
		HashSet<PhysicsComponent> hashSet = new HashSet<PhysicsComponent>();
		(SharedTransformSystem, HashSet<PhysicsComponent>, Box2) state = (_transform, hashSet, item);
		_broadphase.GetBroadphases<(SharedTransformSystem, HashSet<PhysicsComponent>, Box2)>(mapId, worldAABB, ref state, delegate(Entity<BroadphaseComponent> entity, ref (SharedTransformSystem _transform, HashSet<PhysicsComponent> bodies, Box2 aabb) tuple)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			Box2 aabb = Matrix3Helpers.TransformBox(tuple._transform.GetInvWorldMatrix(entity.Owner), ref tuple.aabb);
			foreach (FixtureProxy item2 in entity.Comp.StaticTree.QueryAabb(aabb))
			{
				tuple.bodies.Add(item2.Body);
			}
			foreach (FixtureProxy item3 in entity.Comp.DynamicTree.QueryAabb(aabb))
			{
				tuple.bodies.Add(item3.Body);
			}
		});
		return hashSet;
	}

	[Obsolete("Use EntityLookupSystem")]
	public IEnumerable<Entity<PhysicsComponent>> GetCollidingEntities(MapId mapId, in Box2Rotated worldBounds)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace)
		{
			return Array.Empty<Entity<PhysicsComponent>>();
		}
		HashSet<Entity<PhysicsComponent>> hashSet = new HashSet<Entity<PhysicsComponent>>();
		(SharedTransformSystem, HashSet<Entity<PhysicsComponent>>, Box2Rotated) state = (_transform, hashSet, worldBounds);
		_broadphase.GetBroadphases<(SharedTransformSystem, HashSet<Entity<PhysicsComponent>>, Box2Rotated)>(mapId, ((Box2Rotated)(ref worldBounds)).CalcBoundingBox(), ref state, delegate(Entity<BroadphaseComponent> entity, ref (SharedTransformSystem _transform, HashSet<Entity<PhysicsComponent>> bodies, Box2Rotated worldBounds) tuple)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			Box2 aabb = Matrix3Helpers.TransformBox(tuple._transform.GetInvWorldMatrix(entity.Owner), ref tuple.worldBounds);
			foreach (FixtureProxy item in entity.Comp.StaticTree.QueryAabb(aabb))
			{
				tuple.bodies.Add((Owner: item.Entity, Comp: item.Body));
			}
			foreach (FixtureProxy item2 in entity.Comp.DynamicTree.QueryAabb(aabb))
			{
				tuple.bodies.Add((Owner: item2.Entity, Comp: item2.Body));
			}
		});
		return hashSet;
	}

	public void GetContactingEntities(Entity<PhysicsComponent?> ent, HashSet<EntityUid> contacting, bool approximate = false)
	{
		if (!Resolve(ent.Owner, ref ent.Comp))
		{
			return;
		}
		LinkedListNode<Contact> linkedListNode = ent.Comp.Contacts.First;
		while (linkedListNode != null)
		{
			Contact value = linkedListNode.Value;
			linkedListNode = linkedListNode.Next;
			if (approximate || value.IsTouching)
			{
				contacting.Add((ent.Owner == value.EntityA) ? value.EntityB : value.EntityA);
			}
		}
	}

	public HashSet<EntityUid> GetContactingEntities(EntityUid uid, PhysicsComponent? body = null, bool approximate = false)
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		GetContactingEntities((Owner: uid, Comp: body), hashSet, approximate);
		return hashSet;
	}

	public bool IsInContact(PhysicsComponent body, bool approximate = false)
	{
		for (LinkedListNode<Contact> linkedListNode = body.Contacts.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (approximate || linkedListNode.Value.IsTouching)
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerable<RayCastResults> IntersectRayWithPredicate(MapId mapId, CollisionRay ray, float maxLength = 50f, Func<EntityUid, bool>? predicate = null, bool returnOnFirstHit = true)
	{
		Func<EntityUid, Func<EntityUid, bool>, bool> predicate2 = (EntityUid uid, Func<EntityUid, bool>? wrapped) => wrapped?.Invoke(uid) ?? false;
		return IntersectRayWithPredicate<Func<EntityUid, bool>>(mapId, ray, predicate, predicate2, maxLength, returnOnFirstHit);
	}

	public IEnumerable<RayCastResults> IntersectRayWithPredicate<TState>(MapId mapId, CollisionRay ray, TState state, Func<EntityUid, TState, bool> predicate, float maxLength = 50f, bool returnOnFirstHit = true)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		List<RayCastResults> results = new List<RayCastResults>();
		Vector2 value = ray.Position + Vector2Helpers.Normalized(ray.Direction) * maxLength;
		Unsafe.SkipInit(out Box2 aabb);
		((Box2)(ref aabb))._002Ector(Vector2.Min(ray.Position, value), Vector2.Max(ray.Position, value));
		_broadphase.GetBroadphases(mapId, aabb, delegate(Entity<BroadphaseComponent> broadphase)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			(Vector2, Angle, Matrix3x2, Matrix3x2) worldPositionRotationMatrixWithInv = _transform.GetWorldPositionRotationMatrixWithInv(broadphase.Owner);
			Angle item = worldPositionRotationMatrixWithInv.Item2;
			Matrix3x2 matrix = worldPositionRotationMatrixWithInv.Item3;
			Matrix3x2 item2 = worldPositionRotationMatrixWithInv.Item4;
			Vector2 position = Vector2.Transform(ray.Position, item2);
			Unsafe.SkipInit(out Angle val);
			((Angle)(ref val))._002Ector(0.0 - item.Theta);
			Vector2 direction = ray.Direction;
			Vector2 direction2 = ((Angle)(ref val)).RotateVec(ref direction);
			CollisionRay collisionRay = new CollisionRay(position, direction2, ray.CollisionMask);
			broadphase.Comp.StaticTree.QueryRay(delegate(in FixtureProxy proxy, in Vector2 point, float distFromOrigin)
			{
				if (returnOnFirstHit && results.Count > 0)
				{
					return true;
				}
				if (distFromOrigin > maxLength)
				{
					return true;
				}
				if ((proxy.Fixture.CollisionLayer & ray.CollisionMask) == 0)
				{
					return true;
				}
				if (!proxy.Fixture.Hard)
				{
					return true;
				}
				if (predicate(proxy.Entity, state))
				{
					return true;
				}
				RayCastResults item3 = new RayCastResults(distFromOrigin, Vector2.Transform(point, matrix), proxy.Entity);
				results.Add(item3);
				return true;
			}, (Ray)collisionRay);
			broadphase.Comp.DynamicTree.QueryRay(delegate(in FixtureProxy proxy, in Vector2 point, float distFromOrigin)
			{
				if (returnOnFirstHit && results.Count > 0)
				{
					return true;
				}
				if (distFromOrigin > maxLength)
				{
					return true;
				}
				if ((proxy.Fixture.CollisionLayer & ray.CollisionMask) == 0)
				{
					return true;
				}
				if (!proxy.Fixture.Hard)
				{
					return true;
				}
				if (predicate(proxy.Entity, state))
				{
					return true;
				}
				RayCastResults item3 = new RayCastResults(distFromOrigin, Vector2.Transform(point, matrix), proxy.Entity);
				results.Add(item3);
				return true;
			}, (Ray)collisionRay);
		});
		results.Sort((RayCastResults a, RayCastResults b) => a.Distance.CompareTo(b.Distance));
		return results;
	}

	public IEnumerable<RayCastResults> IntersectRay(MapId mapId, CollisionRay ray, float maxLength = 50f, EntityUid? ignoredEnt = null, bool returnOnFirstHit = true)
	{
		Func<EntityUid, EntityUid?, bool> predicate = (EntityUid uid, EntityUid? ignored) => uid == ignored;
		return IntersectRayWithPredicate(mapId, ray, ignoredEnt, predicate, maxLength, returnOnFirstHit);
	}

	public float IntersectRayPenetration(MapId mapId, CollisionRay ray, float maxLength, EntityUid? ignoredEnt = null)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		float penetration = 0f;
		Vector2 value = ray.Position + Vector2Helpers.Normalized(ray.Direction) * maxLength;
		Unsafe.SkipInit(out Box2 aabb);
		((Box2)(ref aabb))._002Ector(Vector2.Min(ray.Position, value), Vector2.Max(ray.Position, value));
		_broadphase.GetBroadphases(mapId, aabb, delegate(Entity<BroadphaseComponent> broadphase)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) worldPositionRotationInvMatrix = _transform.GetWorldPositionRotationInvMatrix(broadphase);
			Angle item = worldPositionRotationInvMatrix.WorldRotation;
			Matrix3x2 item2 = worldPositionRotationInvMatrix.InvWorldMatrix;
			Vector2 position = Vector2.Transform(ray.Position, item2);
			Unsafe.SkipInit(out Angle val);
			((Angle)(ref val))._002Ector(0.0 - item.Theta);
			Vector2 direction = ray.Direction;
			Vector2 direction2 = ((Angle)(ref val)).RotateVec(ref direction);
			CollisionRay gridRay = new CollisionRay(position, direction2, ray.CollisionMask);
			broadphase.Comp.StaticTree.QueryRay(delegate(in FixtureProxy proxy, in Vector2 point, float distFromOrigin)
			{
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				if (!(distFromOrigin > maxLength))
				{
					EntityUid entity = proxy.Entity;
					EntityUid? entityUid = ignoredEnt;
					if (!(entity == entityUid))
					{
						if (!proxy.Fixture.Hard)
						{
							return true;
						}
						if ((proxy.Fixture.CollisionLayer & ray.CollisionMask) == 0)
						{
							return true;
						}
						if (new Ray(point + gridRay.Direction * ((Box2)(ref proxy.AABB)).Size.Length() * 2f, -gridRay.Direction).Intersects(proxy.AABB, out var _, out var hitPos))
						{
							penetration += (point - hitPos).Length();
						}
						return true;
					}
				}
				return true;
			}, (Ray)gridRay);
			broadphase.Comp.DynamicTree.QueryRay(delegate(in FixtureProxy proxy, in Vector2 point, float distFromOrigin)
			{
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				if (!(distFromOrigin > maxLength))
				{
					EntityUid entity = proxy.Entity;
					EntityUid? entityUid = ignoredEnt;
					if (!(entity == entityUid))
					{
						if (!proxy.Fixture.Hard)
						{
							return true;
						}
						if ((proxy.Fixture.CollisionLayer & ray.CollisionMask) == 0)
						{
							return true;
						}
						if (new Ray(point + gridRay.Direction * ((Box2)(ref proxy.AABB)).Size.Length() * 2f, -gridRay.Direction).Intersects(proxy.AABB, out var _, out var hitPos))
						{
							penetration += (point - hitPos).Length();
						}
						return true;
					}
				}
				return true;
			}, (Ray)gridRay);
		});
		return penetration;
	}

	public bool TryGetDistance(EntityUid uidA, EntityUid uidB, out float distance, TransformComponent? xformA = null, TransformComponent? xformB = null, FixturesComponent? managerA = null, FixturesComponent? managerB = null, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
	{
		Vector2 point;
		Vector2 pointB;
		return TryGetNearest(uidA, uidB, out point, out pointB, out distance, xformA, xformB, managerA, managerB, bodyA, bodyB);
	}

	public bool TryGetNearestPoints(EntityUid uidA, EntityUid uidB, out Vector2 pointA, out Vector2 pointB, TransformComponent? xformA = null, TransformComponent? xformB = null, FixturesComponent? managerA = null, FixturesComponent? managerB = null, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
	{
		float distance;
		return TryGetNearest(uidA, uidB, out pointA, out pointB, out distance, xformA, xformB, managerA, managerB, bodyA, bodyB);
	}

	public bool TryGetNearest(EntityUid uidA, EntityUid uidB, out Vector2 pointA, out Vector2 pointB, out float distance, Transform xfA, Transform xfB, FixturesComponent? managerA = null, FixturesComponent? managerB = null, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
	{
		pointA = Vector2.Zero;
		pointB = Vector2.Zero;
		if (!Resolve(uidA, ref managerA, ref bodyA) || !Resolve(uidB, ref managerB, ref bodyB) || managerA.FixtureCount == 0 || managerB.FixtureCount == 0)
		{
			distance = 0f;
			return false;
		}
		distance = float.MaxValue;
		DistanceInput input = new DistanceInput
		{
			TransformA = xfA,
			TransformB = xfB,
			UseRadii = true
		};
		foreach (Fixture value in managerA.Fixtures.Values)
		{
			if (bodyA.Hard && !value.Hard)
			{
				continue;
			}
			for (int i = 0; i < value.Shape.ChildCount; i++)
			{
				input.ProxyA.Set(value.Shape, i);
				foreach (Fixture value2 in managerB.Fixtures.Values)
				{
					if (bodyB.Hard && !value2.Hard)
					{
						continue;
					}
					for (int j = 0; j < value2.Shape.ChildCount; j++)
					{
						input.ProxyB.Set(value2.Shape, j);
						DistanceManager.ComputeDistance(out var output, out var _, in input);
						if (!(distance < output.Distance))
						{
							pointA = output.PointA;
							pointB = output.PointB;
							distance = output.Distance;
						}
					}
				}
			}
		}
		return true;
	}

	public bool TryGetNearest(EntityUid uid, MapCoordinates coordinates, out Vector2 point, out float distance, TransformComponent? xformA = null, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!Resolve(uid, ref xformA) || xformA.MapID != coordinates.MapId)
		{
			point = Vector2.Zero;
			distance = 0f;
			return false;
		}
		point = Vector2.Zero;
		if (!Resolve(uid, ref manager, ref body) || manager.FixtureCount == 0)
		{
			distance = 0f;
			return false;
		}
		Transform physicsTransform = GetPhysicsTransform(uid, xformA);
		Transform transformB = new Transform(coordinates.Position, Angle.Zero);
		distance = float.MaxValue;
		DistanceInput input = new DistanceInput
		{
			TransformA = physicsTransform,
			TransformB = transformB,
			UseRadii = true
		};
		PhysShapeCircle shape = new PhysShapeCircle(1.4E-44f, Vector2.Zero);
		foreach (Fixture value in manager.Fixtures.Values)
		{
			if (!body.Hard || value.Hard)
			{
				input.ProxyA.Set(value.Shape, 0);
				input.ProxyB.Set(shape, 0);
				DistanceManager.ComputeDistance(out var output, out var _, in input);
				if (!(distance < output.Distance))
				{
					point = output.PointA;
					distance = output.Distance;
				}
			}
		}
		return true;
	}

	public bool TryGetNearest(EntityUid uidA, EntityUid uidB, out Vector2 point, out Vector2 pointB, out float distance, TransformComponent? xformA = null, TransformComponent? xformB = null, FixturesComponent? managerA = null, FixturesComponent? managerB = null, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
	{
		if (!Resolve(uidA, ref xformA) || !Resolve(uidB, ref xformB) || xformA.MapID != xformB.MapID)
		{
			point = Vector2.Zero;
			pointB = Vector2.Zero;
			distance = 0f;
			return false;
		}
		Transform physicsTransform = GetPhysicsTransform(uidA, xformA);
		Transform physicsTransform2 = GetPhysicsTransform(uidB, xformB);
		return TryGetNearest(uidA, uidB, out point, out pointB, out distance, physicsTransform, physicsTransform2, managerA, managerB, bodyA, bodyB);
	}

	public void SetRadius(EntityUid uid, string fixtureId, Fixture fixture, IPhysShape shape, float radius, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (!MathHelper.CloseTo(shape.Radius, radius, 1E-07f) && Resolve(uid, ref manager, ref body, ref xform))
		{
			shape.Radius = radius;
			if (body.CanCollide && TryComp(xform.Broadphase?.Uid, out BroadphaseComponent comp))
			{
				_lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
				_lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
			}
			_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
		}
	}

	public void SetPositionRadius(EntityUid uid, string fixtureId, Fixture fixture, PhysShapeCircle shape, Vector2 position, float radius, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if ((!MathHelper.CloseTo(shape.Radius, radius, 1E-07f) || !Vector2Helpers.EqualsApprox(shape.Position, position)) && Resolve(uid, ref manager, ref body, ref xform))
		{
			shape.Position = position;
			shape.Radius = radius;
			if (body.CanCollide && TryComp(xform.Broadphase?.Uid, out BroadphaseComponent comp))
			{
				_lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
				_lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
			}
			Dirty(uid, manager);
		}
	}

	public void SetPosition(EntityUid uid, string fixtureId, Fixture fixture, PhysShapeCircle circle, Vector2 position, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (!Vector2Helpers.EqualsApprox(circle.Position, position) && Resolve(uid, ref manager, ref body, ref xform))
		{
			circle.Position = position;
			if (body.CanCollide && TryComp(xform.Broadphase?.Uid, out BroadphaseComponent comp))
			{
				_lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
				_lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
			}
			Dirty(uid, manager);
		}
	}

	public void SetVertices(EntityUid uid, string fixtureId, Fixture fixture, EdgeShape edge, Vector2 vertex0, Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (Resolve(uid, ref manager, ref body, ref xform))
		{
			edge.Vertex0 = vertex0;
			edge.Vertex1 = vertex1;
			edge.Vertex2 = vertex2;
			edge.Vertex3 = vertex3;
			if (body.CanCollide && TryComp(xform.Broadphase?.Uid, out BroadphaseComponent comp))
			{
				_lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
				_lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
			}
			_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
		}
	}

	public void SetVertices(EntityUid uid, string fixtureId, Fixture fixture, PolygonShape poly, Vector2[] vertices, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (Resolve(uid, ref manager, ref body, ref xform))
		{
			poly.Set(vertices, vertices.Length);
			if (body.CanCollide && TryComp(xform.Broadphase?.Uid, out BroadphaseComponent comp))
			{
				_lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
				_lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
			}
			_fixtures.FixtureUpdate(uid, dirty: true, resetMass: true, manager, body);
		}
	}

	private void ResetSolver(in SolverData data, in IslandData island, ContactVelocityConstraint[] velocityConstraints, ContactPositionConstraint[] positionConstraints)
	{
		int count = island.Contacts.Count;
		for (int i = 0; i < count; i++)
		{
			Contact contact = island.Contacts[i];
			Fixture? fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			IPhysShape shape = fixtureA.Shape;
			IPhysShape shape2 = fixtureB.Shape;
			float radius = shape.Radius;
			float radius2 = shape2.Radius;
			PhysicsComponent bodyA = contact.BodyA;
			PhysicsComponent bodyB = contact.BodyB;
			Manifold manifold = contact.Manifold;
			int pointCount = manifold.PointCount;
			ref ContactVelocityConstraint reference = ref velocityConstraints[i];
			reference.Friction = contact.Friction;
			reference.Restitution = contact.Restitution;
			reference.TangentSpeed = contact.TangentSpeed;
			reference.IndexA = bodyA.IslandIndex[island.Index];
			reference.IndexB = bodyB.IslandIndex[island.Index];
			(float, float) invMass = GetInvMass(bodyA, bodyB);
			float item = invMass.Item1;
			float item2 = invMass.Item2;
			ref float invMassA = ref reference.InvMassA;
			ref float invMassB = ref reference.InvMassB;
			float num = item;
			float num2 = item2;
			invMassA = num;
			invMassB = num2;
			reference.InvIA = bodyA.InvI;
			reference.InvIB = bodyB.InvI;
			reference.ContactIndex = i;
			reference.PointCount = pointCount;
			reference.K = Vector4.Zero;
			reference.NormalMass = Vector4.Zero;
			ref ContactPositionConstraint reference2 = ref positionConstraints[i];
			reference2.IndexA = bodyA.IslandIndex[island.Index];
			reference2.IndexB = bodyB.IslandIndex[island.Index];
			invMassA = ref reference2.InvMassA;
			ref float invMassB2 = ref reference2.InvMassB;
			num2 = item;
			num = item2;
			invMassA = num2;
			invMassB2 = num;
			reference2.LocalCenterA = bodyA.LocalCenter;
			reference2.LocalCenterB = bodyB.LocalCenter;
			reference2.InvIA = bodyA.InvI;
			reference2.InvIB = bodyB.InvI;
			reference2.LocalNormal = manifold.LocalNormal;
			reference2.LocalPoint = manifold.LocalPoint;
			reference2.PointCount = pointCount;
			reference2.RadiusA = radius;
			reference2.RadiusB = radius2;
			reference2.Type = manifold.Type;
			Span<ManifoldPoint> asSpan = manifold.Points.AsSpan;
			Span<Vector2> asSpan2 = reference2.LocalPoints.AsSpan;
			Span<VelocityConstraintPoint> asSpan3 = reference.Points.AsSpan;
			for (int j = 0; j < pointCount; j++)
			{
				ManifoldPoint manifoldPoint = asSpan[j];
				ref VelocityConstraintPoint reference3 = ref asSpan3[j];
				if (_warmStarting)
				{
					reference3.NormalImpulse = data.DtRatio * manifoldPoint.NormalImpulse;
					reference3.TangentImpulse = data.DtRatio * manifoldPoint.TangentImpulse;
				}
				else
				{
					reference3.NormalImpulse = 0f;
					reference3.TangentImpulse = 0f;
				}
				reference3.RelativeVelocityA = Vector2.Zero;
				reference3.RelativeVelocityB = Vector2.Zero;
				reference3.NormalMass = 0f;
				reference3.TangentMass = 0f;
				reference3.VelocityBias = 0f;
				asSpan2[j] = manifoldPoint.LocalPoint;
			}
		}
	}

	private (float, float) GetInvMass(PhysicsComponent bodyA, PhysicsComponent bodyB)
	{
		switch (bodyA.BodyType)
		{
		case BodyType.Kinematic:
		case BodyType.Static:
			return (bodyA.InvMass, bodyB.InvMass);
		case BodyType.KinematicController:
			switch (bodyB.BodyType)
			{
			case BodyType.Kinematic:
			case BodyType.Static:
				return (bodyA.InvMass, bodyB.InvMass);
			case BodyType.Dynamic:
				return (bodyA.InvMass, 0f);
			case BodyType.KinematicController:
				return (0f, 0f);
			default:
				throw new ArgumentOutOfRangeException();
			}
		case BodyType.Dynamic:
			switch (bodyB.BodyType)
			{
			case BodyType.Kinematic:
			case BodyType.Static:
			case BodyType.Dynamic:
				return (bodyA.InvMass, bodyB.InvMass);
			case BodyType.KinematicController:
				return (0f, bodyB.InvMass);
			default:
				throw new ArgumentOutOfRangeException();
			}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void InitializeVelocityConstraints(in SolverData data, in IslandData island, ContactVelocityConstraint[] velocityConstraints, ContactPositionConstraint[] positionConstraints, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities)
	{
		Span<Vector2> points = stackalloc Vector2[2];
		int count = island.Contacts.Count;
		List<Contact> contacts = island.Contacts;
		int offset = island.Offset;
		for (int i = 0; i < count; i++)
		{
			ref ContactVelocityConstraint reference = ref velocityConstraints[i];
			ContactPositionConstraint contactPositionConstraint = positionConstraints[i];
			float radiusA = contactPositionConstraint.RadiusA;
			float radiusB = contactPositionConstraint.RadiusB;
			Manifold manifold = contacts[reference.ContactIndex].Manifold;
			int indexA = reference.IndexA;
			int indexB = reference.IndexB;
			float invMassA = reference.InvMassA;
			float invMassB = reference.InvMassB;
			float invIA = reference.InvIA;
			float invIB = reference.InvIB;
			Vector2 vector = contactPositionConstraint.LocalCenterA;
			Vector2 vector2 = contactPositionConstraint.LocalCenterB;
			Vector2 vector3 = positions[indexA];
			float angle = angles[indexA];
			Vector2 vector4 = linearVelocities[offset + indexA];
			float num = angularVelocities[offset + indexA];
			Vector2 vector5 = positions[indexB];
			float angle2 = angles[indexB];
			Vector2 vector6 = linearVelocities[offset + indexB];
			float num2 = angularVelocities[offset + indexB];
			Transform xfA = new Transform(angle);
			Transform xfB = new Transform(angle2);
			xfA.Position = vector3 - Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in vector);
			xfB.Position = vector5 - Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in vector2);
			InitializeManifold(ref manifold, in xfA, in xfB, radiusA, radiusB, out var normal, points);
			reference.Normal = normal;
			int pointCount = reference.PointCount;
			Span<VelocityConstraintPoint> asSpan = reference.Points.AsSpan;
			for (int j = 0; j < pointCount; j++)
			{
				ref VelocityConstraintPoint reference2 = ref asSpan[j];
				reference2.RelativeVelocityA = points[j] - vector3;
				reference2.RelativeVelocityB = points[j] - vector5;
				float num3 = Vector2Helpers.Cross(reference2.RelativeVelocityA, reference.Normal);
				float num4 = Vector2Helpers.Cross(reference2.RelativeVelocityB, reference.Normal);
				float num5 = invMassA + invMassB + invIA * num3 * num3 + invIB * num4 * num4;
				reference2.NormalMass = ((num5 > 0f) ? (1f / num5) : 0f);
				ref Vector2 normal2 = ref reference.Normal;
				float num6 = 1f;
				Vector2 vector7 = Vector2Helpers.Cross(ref normal2, ref num6);
				float num7 = Vector2Helpers.Cross(reference2.RelativeVelocityA, vector7);
				float num8 = Vector2Helpers.Cross(reference2.RelativeVelocityB, vector7);
				float num9 = invMassA + invMassB + invIA * num7 * num7 + invIB * num8 * num8;
				reference2.TangentMass = ((num9 > 0f) ? (1f / num9) : 0f);
				reference2.VelocityBias = 0f;
				float num10 = Vector2.Dot(reference.Normal, vector6 + Vector2Helpers.Cross(num2, ref reference2.RelativeVelocityB) - vector4 - Vector2Helpers.Cross(num, ref reference2.RelativeVelocityA));
				if (num10 < 0f - data.VelocityThreshold)
				{
					reference2.VelocityBias = (0f - reference.Restitution) * num10;
				}
			}
			if (reference.PointCount == 2)
			{
				VelocityConstraintPoint _ = reference.Points._00;
				VelocityConstraintPoint _2 = reference.Points._01;
				float num11 = Vector2Helpers.Cross(_.RelativeVelocityA, reference.Normal);
				float num12 = Vector2Helpers.Cross(_.RelativeVelocityB, reference.Normal);
				float num13 = Vector2Helpers.Cross(_2.RelativeVelocityA, reference.Normal);
				float num14 = Vector2Helpers.Cross(_2.RelativeVelocityB, reference.Normal);
				float num15 = invMassA + invMassB + invIA * num11 * num11 + invIB * num12 * num12;
				float num16 = invMassA + invMassB + invIA * num13 * num13 + invIB * num14 * num14;
				float num17 = invMassA + invMassB + invIA * num11 * num13 + invIB * num12 * num14;
				if (num15 * num15 < 1000f * (num15 * num16 - num17 * num17))
				{
					reference.K = new Vector4(num15, num17, num17, num16);
					reference.NormalMass = Vector4Helpers.Inverse(reference.K);
				}
				else
				{
					reference.PointCount = 1;
				}
			}
		}
	}

	private void WarmStart(in SolverData data, in IslandData island, ContactVelocityConstraint[] velocityConstraints, Vector2[] linearVelocities, float[] angularVelocities)
	{
		int offset = island.Offset;
		for (int i = 0; i < island.Contacts.Count; i++)
		{
			ContactVelocityConstraint contactVelocityConstraint = velocityConstraints[i];
			Span<VelocityConstraintPoint> asSpan = contactVelocityConstraint.Points.AsSpan;
			int indexA = contactVelocityConstraint.IndexA;
			int indexB = contactVelocityConstraint.IndexB;
			float invMassA = contactVelocityConstraint.InvMassA;
			float invIA = contactVelocityConstraint.InvIA;
			float invMassB = contactVelocityConstraint.InvMassB;
			float invIB = contactVelocityConstraint.InvIB;
			int pointCount = contactVelocityConstraint.PointCount;
			ref Vector2 reference = ref linearVelocities[offset + indexA];
			ref float reference2 = ref angularVelocities[offset + indexA];
			ref Vector2 reference3 = ref linearVelocities[offset + indexB];
			ref float reference4 = ref angularVelocities[offset + indexB];
			Vector2 normal = contactVelocityConstraint.Normal;
			float num = 1f;
			Vector2 vector = Vector2Helpers.Cross(ref normal, ref num);
			for (int j = 0; j < pointCount; j++)
			{
				VelocityConstraintPoint velocityConstraintPoint = asSpan[j];
				Vector2 vector2 = normal * velocityConstraintPoint.NormalImpulse + vector * velocityConstraintPoint.TangentImpulse;
				reference2 -= invIA * Vector2Helpers.Cross(velocityConstraintPoint.RelativeVelocityA, vector2);
				reference -= vector2 * invMassA;
				reference4 += invIB * Vector2Helpers.Cross(velocityConstraintPoint.RelativeVelocityB, vector2);
				reference3 += vector2 * invMassB;
			}
		}
	}

	private static void SolveVelocityConstraints(in IslandData island, ParallelOptions? options, ContactVelocityConstraint[] velocityConstraints, Vector2[] linearVelocities, float[] angularVelocities)
	{
		int count = island.Contacts.Count;
		if (options != null && count > 32)
		{
			ProcessParallelInternal(island, count, options, velocityConstraints, linearVelocities, angularVelocities);
		}
		else
		{
			SolveVelocityConstraints(in island, 0, count, velocityConstraints, linearVelocities, angularVelocities);
		}
		static void ProcessParallelInternal(IslandData island2, int contactCount, ParallelOptions parallelOptions, ContactVelocityConstraint[] velocityConstraints2, Vector2[] linearVelocities2, float[] angularVelocities2)
		{
			int toExclusive = (int)Math.Ceiling((float)contactCount / 16f);
			Parallel.For(0, toExclusive, parallelOptions, delegate(int i)
			{
				int num = i * 16;
				int end = Math.Min(num + 16, contactCount);
				SolveVelocityConstraints(in island2, num, end, velocityConstraints2, linearVelocities2, angularVelocities2);
			});
		}
	}

	private static void SolveVelocityConstraints(in IslandData island, int start, int end, ContactVelocityConstraint[] velocityConstraints, Vector2[] linearVelocities, float[] angularVelocities)
	{
		int offset = island.Offset;
		for (int i = start; i < end; i++)
		{
			ref ContactVelocityConstraint reference = ref velocityConstraints[i];
			int indexA = reference.IndexA;
			int indexB = reference.IndexB;
			float invMassA = reference.InvMassA;
			float invIA = reference.InvIA;
			float invMassB = reference.InvMassB;
			float invIB = reference.InvIB;
			int pointCount = reference.PointCount;
			ref Vector2 reference2 = ref linearVelocities[offset + indexA];
			ref float reference3 = ref angularVelocities[offset + indexA];
			ref Vector2 reference4 = ref linearVelocities[offset + indexB];
			ref float reference5 = ref angularVelocities[offset + indexB];
			Vector2 normal = reference.Normal;
			float num = 1f;
			Vector2 vector = Vector2Helpers.Cross(ref normal, ref num);
			float friction = reference.Friction;
			Span<VelocityConstraintPoint> asSpan = reference.Points.AsSpan;
			for (int j = 0; j < pointCount; j++)
			{
				ref VelocityConstraintPoint reference6 = ref asSpan[j];
				float num2 = Vector2.Dot(reference4 + Vector2Helpers.Cross(reference5, ref reference6.RelativeVelocityB) - reference2 - Vector2Helpers.Cross(reference3, ref reference6.RelativeVelocityA), vector) - reference.TangentSpeed;
				float num3 = reference6.TangentMass * (0f - num2);
				float num4 = friction * reference6.NormalImpulse;
				float num5 = Math.Clamp(reference6.TangentImpulse + num3, 0f - num4, num4);
				num3 = num5 - reference6.TangentImpulse;
				reference6.TangentImpulse = num5;
				Vector2 vector2 = vector * num3;
				reference2 -= vector2 * invMassA;
				reference3 -= invIA * Vector2Helpers.Cross(reference6.RelativeVelocityA, vector2);
				reference4 += vector2 * invMassB;
				reference5 += invIB * Vector2Helpers.Cross(reference6.RelativeVelocityB, vector2);
			}
			if (reference.PointCount == 1)
			{
				ref VelocityConstraintPoint _ = ref reference.Points._00;
				float num6 = Vector2.Dot(reference4 + Vector2Helpers.Cross(reference5, ref _.RelativeVelocityB) - reference2 - Vector2Helpers.Cross(reference3, ref _.RelativeVelocityA), normal);
				float num7 = (0f - _.NormalMass) * (num6 - _.VelocityBias);
				float num8 = Math.Max(_.NormalImpulse + num7, 0f);
				num7 = num8 - _.NormalImpulse;
				_.NormalImpulse = num8;
				Vector2 vector3 = normal * num7;
				reference2 -= vector3 * invMassA;
				reference3 -= invIA * Vector2Helpers.Cross(_.RelativeVelocityA, vector3);
				reference4 += vector3 * invMassB;
				reference5 += invIB * Vector2Helpers.Cross(_.RelativeVelocityB, vector3);
				continue;
			}
			ref VelocityConstraintPoint _2 = ref reference.Points._00;
			ref VelocityConstraintPoint _3 = ref reference.Points._01;
			Vector2 vector4 = new Vector2(_2.NormalImpulse, _3.NormalImpulse);
			Vector2 value = reference4 + Vector2Helpers.Cross(reference5, ref _2.RelativeVelocityB) - reference2 - Vector2Helpers.Cross(reference3, ref _2.RelativeVelocityA);
			Vector2 value2 = reference4 + Vector2Helpers.Cross(reference5, ref _3.RelativeVelocityB) - reference2 - Vector2Helpers.Cross(reference3, ref _3.RelativeVelocityA);
			float num9 = Vector2.Dot(value, normal);
			float num10 = Vector2.Dot(value2, normal);
			Vector2 v = new Vector2
			{
				X = num9 - _2.VelocityBias,
				Y = num10 - _3.VelocityBias
			};
			v -= Robust.Shared.Physics.Transform.Mul(reference.K, vector4);
			Vector2 vector5 = -Robust.Shared.Physics.Transform.Mul(reference.NormalMass, v);
			if (vector5.X >= 0f && vector5.Y >= 0f)
			{
				Vector2 vector6 = vector5 - vector4;
				Vector2 vector7 = normal * vector6.X;
				Vector2 vector8 = normal * vector6.Y;
				reference2 -= (vector7 + vector8) * invMassA;
				reference3 -= invIA * (Vector2Helpers.Cross(_2.RelativeVelocityA, vector7) + Vector2Helpers.Cross(_3.RelativeVelocityA, vector8));
				reference4 += (vector7 + vector8) * invMassB;
				reference5 += invIB * (Vector2Helpers.Cross(_2.RelativeVelocityB, vector7) + Vector2Helpers.Cross(_3.RelativeVelocityB, vector8));
				_2.NormalImpulse = vector5.X;
				_3.NormalImpulse = vector5.Y;
				continue;
			}
			vector5.X = (0f - _2.NormalMass) * v.X;
			vector5.Y = 0f;
			num9 = 0f;
			num10 = reference.K.Y * vector5.X + v.Y;
			if (vector5.X >= 0f && num10 >= 0f)
			{
				Vector2 vector9 = vector5 - vector4;
				Vector2 vector10 = normal * vector9.X;
				Vector2 vector11 = normal * vector9.Y;
				reference2 -= (vector10 + vector11) * invMassA;
				reference3 -= invIA * (Vector2Helpers.Cross(_2.RelativeVelocityA, vector10) + Vector2Helpers.Cross(_3.RelativeVelocityA, vector11));
				reference4 += (vector10 + vector11) * invMassB;
				reference5 += invIB * (Vector2Helpers.Cross(_2.RelativeVelocityB, vector10) + Vector2Helpers.Cross(_3.RelativeVelocityB, vector11));
				_2.NormalImpulse = vector5.X;
				_3.NormalImpulse = vector5.Y;
				continue;
			}
			vector5.X = 0f;
			vector5.Y = (0f - _3.NormalMass) * v.Y;
			num9 = reference.K.Z * vector5.Y + v.X;
			num10 = 0f;
			if (vector5.Y >= 0f && num9 >= 0f)
			{
				Vector2 vector12 = vector5 - vector4;
				Vector2 vector13 = normal * vector12.X;
				Vector2 vector14 = normal * vector12.Y;
				reference2 -= (vector13 + vector14) * invMassA;
				reference3 -= invIA * (Vector2Helpers.Cross(_2.RelativeVelocityA, vector13) + Vector2Helpers.Cross(_3.RelativeVelocityA, vector14));
				reference4 += (vector13 + vector14) * invMassB;
				reference5 += invIB * (Vector2Helpers.Cross(_2.RelativeVelocityB, vector13) + Vector2Helpers.Cross(_3.RelativeVelocityB, vector14));
				_2.NormalImpulse = vector5.X;
				_3.NormalImpulse = vector5.Y;
				continue;
			}
			vector5.X = 0f;
			vector5.Y = 0f;
			num9 = v.X;
			num10 = v.Y;
			if (num9 >= 0f && num10 >= 0f)
			{
				Vector2 vector15 = vector5 - vector4;
				Vector2 vector16 = normal * vector15.X;
				Vector2 vector17 = normal * vector15.Y;
				reference2 -= (vector16 + vector17) * invMassA;
				reference3 -= invIA * (Vector2Helpers.Cross(_2.RelativeVelocityA, vector16) + Vector2Helpers.Cross(_3.RelativeVelocityA, vector17));
				reference4 += (vector16 + vector17) * invMassB;
				reference5 += invIB * (Vector2Helpers.Cross(_2.RelativeVelocityB, vector16) + Vector2Helpers.Cross(_3.RelativeVelocityB, vector17));
				_2.NormalImpulse = vector5.X;
				_3.NormalImpulse = vector5.Y;
			}
		}
	}

	private void StoreImpulses(in IslandData island, ContactVelocityConstraint[] velocityConstraints)
	{
		for (int i = 0; i < island.Contacts.Count; i++)
		{
			ref ContactVelocityConstraint reference = ref velocityConstraints[i];
			Span<ManifoldPoint> asSpan = island.Contacts[reference.ContactIndex].Manifold.Points.AsSpan;
			Span<VelocityConstraintPoint> asSpan2 = reference.Points.AsSpan;
			for (int j = 0; j < reference.PointCount; j++)
			{
				ref ManifoldPoint reference2 = ref asSpan[j];
				reference2.NormalImpulse = asSpan2[j].NormalImpulse;
				reference2.TangentImpulse = asSpan2[j].TangentImpulse;
			}
		}
	}

	private static bool SolvePositionConstraints(in SolverData data, in IslandData island, ParallelOptions? options, ContactPositionConstraint[] positionConstraints, Vector2[] positions, float[] angles)
	{
		int count = island.Contacts.Count;
		if (options != null && count > 32)
		{
			return ProcessParallelInternal(count, data, options, positionConstraints, positions, angles);
		}
		return SolvePositionConstraints(in data, 0, count, positionConstraints, positions, angles);
		static bool ProcessParallelInternal(int contactCount, SolverData data2, ParallelOptions parallelOptions, ContactPositionConstraint[] positionConstraints2, Vector2[] positions2, float[] angles2)
		{
			int unsolved = 0;
			int toExclusive = (int)Math.Ceiling((float)contactCount / 16f);
			Parallel.For(0, toExclusive, parallelOptions, delegate(int i)
			{
				int num = i * 16;
				int end = Math.Min(num + 16, contactCount);
				if (!SolvePositionConstraints(in data2, num, end, positionConstraints2, positions2, angles2))
				{
					Interlocked.Increment(ref unsolved);
				}
			});
			return unsolved == 0;
		}
	}

	private static bool SolvePositionConstraints(in SolverData data, int start, int end, ContactPositionConstraint[] positionConstraints, Vector2[] positions, float[] angles)
	{
		float num = 0f;
		for (int i = start; i < end; i++)
		{
			ContactPositionConstraint pc = positionConstraints[i];
			int indexA = pc.IndexA;
			int indexB = pc.IndexB;
			Vector2 vector = pc.LocalCenterA;
			float invMassA = pc.InvMassA;
			float invIA = pc.InvIA;
			Vector2 vector2 = pc.LocalCenterB;
			float invMassB = pc.InvMassB;
			float invIB = pc.InvIB;
			int pointCount = pc.PointCount;
			ref Vector2 reference = ref positions[indexA];
			ref float reference2 = ref angles[indexA];
			ref Vector2 reference3 = ref positions[indexB];
			ref float reference4 = ref angles[indexB];
			for (int j = 0; j < pointCount; j++)
			{
				Transform xfA = new Transform(reference2);
				Transform xfB = new Transform(reference4);
				xfA.Position = reference - Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in vector);
				xfB.Position = reference3 - Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in vector2);
				PositionSolverManifoldInitialize(in pc, j, in xfA, in xfB, out var normal, out var point, out var separation);
				Vector2 vector3 = point - reference;
				Vector2 vector4 = point - reference3;
				num = Math.Min(num, separation);
				float num2 = Math.Clamp(data.Baumgarte * (separation + 0.005f), 0f - data.MaxLinearCorrection, 0f);
				float num3 = Vector2Helpers.Cross(vector3, normal);
				float num4 = Vector2Helpers.Cross(vector4, normal);
				float num5 = invMassA + invMassB + invIA * num3 * num3 + invIB * num4 * num4;
				float num6 = ((num5 > 0f) ? ((0f - num2) / num5) : 0f);
				Vector2 vector5 = normal * num6;
				reference -= vector5 * invMassA;
				reference2 -= invIA * Vector2Helpers.Cross(vector3, vector5);
				reference3 += vector5 * invMassB;
				reference4 += invIB * Vector2Helpers.Cross(vector4, vector5);
			}
		}
		return num >= -0.015f;
	}

	internal static void InitializeManifold(ref Manifold manifold, in Transform xfA, in Transform xfB, float radiusA, float radiusB, out Vector2 normal, Span<Vector2> points)
	{
		normal = Vector2.Zero;
		if (manifold.PointCount == 0)
		{
			return;
		}
		switch (manifold.Type)
		{
		case ManifoldType.Circles:
		{
			normal = new Vector2(1f, 0f);
			Vector2 vector9 = Robust.Shared.Physics.Transform.Mul(in xfA, in manifold.LocalPoint);
			Vector2 vector10 = Robust.Shared.Physics.Transform.Mul(in xfB, in manifold.Points._00.LocalPoint);
			if ((vector9 - vector10).LengthSquared() > 0f)
			{
				normal = vector10 - vector9;
				normal = Vector2Helpers.Normalized(normal);
			}
			Vector2 vector11 = vector9 + normal * radiusA;
			Vector2 vector12 = vector10 - normal * radiusB;
			points[0] = (vector11 + vector12) * 0.5f;
			break;
		}
		case ManifoldType.FaceA:
		{
			normal = Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in manifold.LocalNormal);
			Vector2 vector5 = Robust.Shared.Physics.Transform.Mul(in xfA, in manifold.LocalPoint);
			Span<ManifoldPoint> asSpan2 = manifold.Points.AsSpan;
			for (int j = 0; j < manifold.PointCount; j++)
			{
				Vector2 vector6 = Robust.Shared.Physics.Transform.Mul(in xfB, in asSpan2[j].LocalPoint);
				Vector2 vector7 = vector6 + normal * (radiusA - Vector2.Dot(vector6 - vector5, normal));
				Vector2 vector8 = vector6 - normal * radiusB;
				points[j] = (vector7 + vector8) * 0.5f;
			}
			break;
		}
		case ManifoldType.FaceB:
		{
			normal = Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in manifold.LocalNormal);
			Vector2 vector = Robust.Shared.Physics.Transform.Mul(in xfB, in manifold.LocalPoint);
			Span<ManifoldPoint> asSpan = manifold.Points.AsSpan;
			for (int i = 0; i < manifold.PointCount; i++)
			{
				Vector2 vector2 = Robust.Shared.Physics.Transform.Mul(in xfA, in asSpan[i].LocalPoint);
				Vector2 vector3 = vector2 + normal * (radiusB - Vector2.Dot(vector2 - vector, normal));
				Vector2 vector4 = vector2 - normal * radiusA;
				points[i] = (vector4 + vector3) * 0.5f;
			}
			normal = -normal;
			break;
		}
		default:
			throw new InvalidOperationException();
		}
	}

	private static void PositionSolverManifoldInitialize(in ContactPositionConstraint pc, int index, in Transform xfA, in Transform xfB, out Vector2 normal, out Vector2 point, out float separation)
	{
		FixedArray2<Vector2> localPoints;
		switch (pc.Type)
		{
		case ManifoldType.Circles:
		{
			Vector2 vector5 = Robust.Shared.Physics.Transform.Mul(in xfA, in pc.LocalPoint);
			Vector2 vector6 = Robust.Shared.Physics.Transform.Mul(in xfB, in pc.LocalPoints._00);
			normal = vector6 - vector5;
			if (normal != Vector2.Zero)
			{
				normal = Vector2Helpers.Normalized(normal);
			}
			point = (vector5 + vector6) * 0.5f;
			separation = Vector2.Dot(vector6 - vector5, normal) - pc.RadiusA - pc.RadiusB;
			break;
		}
		case ManifoldType.FaceA:
		{
			localPoints = pc.LocalPoints;
			Span<Vector2> asSpan2 = localPoints.AsSpan;
			normal = Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in pc.LocalNormal);
			Vector2 vector3 = Robust.Shared.Physics.Transform.Mul(in xfA, in pc.LocalPoint);
			Vector2 vector4 = Robust.Shared.Physics.Transform.Mul(in xfB, in asSpan2[index]);
			separation = Vector2.Dot(vector4 - vector3, normal) - pc.RadiusA - pc.RadiusB;
			point = vector4;
			break;
		}
		case ManifoldType.FaceB:
		{
			localPoints = pc.LocalPoints;
			Span<Vector2> asSpan = localPoints.AsSpan;
			normal = Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in pc.LocalNormal);
			Vector2 vector = Robust.Shared.Physics.Transform.Mul(in xfB, in pc.LocalPoint);
			Vector2 vector2 = Robust.Shared.Physics.Transform.Mul(in xfA, in asSpan[index]);
			separation = Vector2.Dot(vector2 - vector, normal) - pc.RadiusA - pc.RadiusB;
			point = vector2;
			normal = -normal;
			break;
		}
		default:
			normal = Vector2.Zero;
			point = Vector2.Zero;
			separation = 0f;
			break;
		}
	}

	public Vector2 GetLinearVelocity(EntityUid uid, Vector2 point, PhysicsComponent? component = null, TransformComponent? xform = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!PhysicsQuery.Resolve(uid, ref component))
		{
			return Vector2.Zero;
		}
		if (!XformQuery.Resolve(uid, ref xform))
		{
			return Vector2.Zero;
		}
		Vector2 linearVelocity = component.LinearVelocity;
		Angle localRotation = xform.LocalRotation;
		float angularVelocity = component.AngularVelocity;
		Vector2 vector = point - component.LocalCenter;
		Vector2 vector2 = Vector2Helpers.Cross(angularVelocity, ref vector);
		Vector2 vector3 = ((Angle)(ref localRotation)).RotateVec(ref vector2);
		return linearVelocity + vector3;
	}

	public Vector2 GetMapLinearVelocity(EntityCoordinates coordinates)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!coordinates.IsValid(EntityManager))
		{
			return Vector2.Zero;
		}
		EntityUid? map = _transform.GetMap(coordinates);
		EntityUid entityUid = coordinates.EntityId;
		Vector2 vector = coordinates.Position;
		Vector2 zero = Vector2.Zero;
		Vector2 vector2 = Vector2.Zero;
		while (entityUid != map && entityUid.IsValid())
		{
			TransformComponent component = XformQuery.GetComponent(entityUid);
			Angle localRotation;
			if (PhysicsQuery.TryGetComponent(entityUid, out PhysicsComponent component2))
			{
				zero += component2.LinearVelocity;
				Vector2 vector3 = vector2;
				float angularVelocity = component2.AngularVelocity;
				Vector2 vector4 = vector - component2.LocalCenter;
				vector2 = vector3 + Vector2Helpers.Cross(angularVelocity, ref vector4);
				localRotation = component.LocalRotation;
				vector2 = ((Angle)(ref localRotation)).RotateVec(ref vector2);
			}
			Vector2 localPosition = component.LocalPosition;
			localRotation = component.LocalRotation;
			vector = localPosition + ((Angle)(ref localRotation)).RotateVec(ref vector);
			entityUid = component.ParentUid;
		}
		return zero;
	}

	public Vector2 GetMapLinearVelocity(EntityUid uid, PhysicsComponent? component = null, TransformComponent? xform = null)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(uid, ref xform))
		{
			return Vector2.Zero;
		}
		PhysicsQuery.Resolve(uid, ref component, logMissing: false);
		EntityUid parentUid = xform.ParentUid;
		Vector2 vector = xform.LocalPosition;
		Vector2 vector2 = component?.LinearVelocity ?? Vector2.Zero;
		Vector2 vector3 = Vector2.Zero;
		while (true)
		{
			EntityUid value = parentUid;
			EntityUid? mapUid = xform.MapUid;
			if (!(value != mapUid) || !parentUid.IsValid())
			{
				break;
			}
			xform = XformQuery.GetComponent(parentUid);
			Angle localRotation;
			if (PhysicsQuery.TryGetComponent(parentUid, out PhysicsComponent component2))
			{
				vector2 += component2.LinearVelocity;
				Vector2 vector4 = vector3;
				float angularVelocity = component2.AngularVelocity;
				Vector2 vector5 = vector - component2.LocalCenter;
				vector3 = vector4 + Vector2Helpers.Cross(angularVelocity, ref vector5);
				localRotation = xform.LocalRotation;
				vector3 = ((Angle)(ref localRotation)).RotateVec(ref vector3);
			}
			Vector2 localPosition = xform.LocalPosition;
			localRotation = xform.LocalRotation;
			vector = localPosition + ((Angle)(ref localRotation)).RotateVec(ref vector);
			parentUid = xform.ParentUid;
		}
		return vector2 + vector3;
	}

	public float GetMapAngularVelocity(EntityUid uid, PhysicsComponent? component = null, TransformComponent? xform = null)
	{
		if (!XformQuery.Resolve(uid, ref xform))
		{
			return 0f;
		}
		PhysicsQuery.Resolve(uid, ref component, logMissing: false);
		float num = component?.AngularVelocity ?? 0f;
		while (true)
		{
			EntityUid parentUid = xform.ParentUid;
			EntityUid? mapUid = xform.MapUid;
			if (!(parentUid != mapUid) || !xform.ParentUid.IsValid())
			{
				break;
			}
			if (PhysicsQuery.TryGetComponent(xform.ParentUid, out PhysicsComponent component2))
			{
				num += component2.AngularVelocity;
			}
			xform = XformQuery.GetComponent(xform.ParentUid);
		}
		return num;
	}

	public (Vector2, float) GetMapVelocities(EntityUid uid, PhysicsComponent? component = null, TransformComponent? xform = null)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (!XformQuery.Resolve(uid, ref xform))
		{
			return (Vector2.Zero, 0f);
		}
		PhysicsQuery.Resolve(uid, ref component, logMissing: false);
		EntityUid parentUid = xform.ParentUid;
		Vector2 vector = xform.LocalPosition;
		Vector2 vector2 = component?.LinearVelocity ?? Vector2.Zero;
		float num = component?.AngularVelocity ?? 0f;
		Vector2 vector3 = Vector2.Zero;
		while (true)
		{
			EntityUid value = parentUid;
			EntityUid? mapUid = xform.MapUid;
			if (!(value != mapUid) || !parentUid.IsValid())
			{
				break;
			}
			xform = XformQuery.GetComponent(parentUid);
			Angle localRotation;
			if (PhysicsQuery.TryGetComponent(parentUid, out PhysicsComponent component2))
			{
				num += component2.AngularVelocity;
				vector2 += component2.LinearVelocity;
				Vector2 vector4 = vector3;
				float angularVelocity = component2.AngularVelocity;
				Vector2 vector5 = vector - component2.LocalCenter;
				vector3 = vector4 + Vector2Helpers.Cross(angularVelocity, ref vector5);
				localRotation = xform.LocalRotation;
				vector3 = ((Angle)(ref localRotation)).RotateVec(ref vector3);
			}
			Vector2 localPosition = xform.LocalPosition;
			localRotation = xform.LocalRotation;
			vector = localPosition + ((Angle)(ref localRotation)).RotateVec(ref vector);
			parentUid = xform.ParentUid;
		}
		return (vector2 + vector3, num);
	}

	private void HandleParentChangeVelocity(EntityUid uid, PhysicsComponent physics, EntityUid oldParent, TransformComponent xform)
	{
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (_gameTiming.ApplyingState || physics.LifeStage != ComponentLifeStage.Running || physics.BodyType == BodyType.Static || xform.MapID == MapId.Nullspace || !physics.CanCollide)
		{
			return;
		}
		FixturesComponent manager = null;
		var (vector, num) = GetMapVelocities(uid, physics, xform);
		if (oldParent == EntityUid.Invalid)
		{
			SetLinearVelocity(uid, physics.LinearVelocity * 2f - vector, dirty: true, wakeBody: true, manager, physics);
			SetAngularVelocity(uid, physics.AngularVelocity * 2f - num, dirty: true, manager, physics);
			return;
		}
		EntityUid uid2 = oldParent;
		TransformComponent component = XformQuery.GetComponent(uid2);
		Vector2 vector2 = Vector2.Transform(_transform.GetWorldPosition(xform), _transform.GetInvWorldMatrix(component));
		Vector2 linearVelocity = physics.LinearVelocity;
		float num2 = physics.AngularVelocity;
		Vector2 vector3 = Vector2.Zero;
		do
		{
			Angle localRotation;
			if (PhysicsQuery.TryGetComponent(uid2, out PhysicsComponent component2))
			{
				num2 += component2.AngularVelocity;
				linearVelocity += component2.LinearVelocity;
				Vector2 vector4 = vector3;
				float angularVelocity = component2.AngularVelocity;
				Vector2 vector5 = vector2 - component2.LocalCenter;
				vector3 = vector4 + Vector2Helpers.Cross(angularVelocity, ref vector5);
				localRotation = component.LocalRotation;
				vector3 = ((Angle)(ref localRotation)).RotateVec(ref vector3);
			}
			Vector2 localPosition = component.LocalPosition;
			localRotation = component.LocalRotation;
			vector2 = localPosition + ((Angle)(ref localRotation)).RotateVec(ref vector2);
			uid2 = component.ParentUid;
		}
		while (uid2.IsValid() && XformQuery.TryGetComponent(uid2, out component));
		linearVelocity += vector3;
		SetLinearVelocity(uid, physics.LinearVelocity + linearVelocity - vector, dirty: true, wakeBody: true, manager, physics);
		SetAngularVelocity(uid, physics.AngularVelocity + num2 - num, dirty: true, manager, physics);
	}

	public void SetGravity(Vector2 value)
	{
		if (!Gravity.Equals(value))
		{
			Gravity = value;
		}
	}

	static SharedPhysicsSystem()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		HistogramConfiguration val = new HistogramConfiguration();
		((MetricConfiguration)val).LabelNames = new string[1] { "controller" };
		val.Buckets = Histogram.ExponentialBuckets(1E-06, 1.5, 25);
		TickUsageControllerBeforeSolveHistogram = Metrics.CreateHistogram("robust_entity_physics_controller_before_solve", "Amount of time spent running a controller's UpdateBeforeSolve", val);
		val = new HistogramConfiguration();
		((MetricConfiguration)val).LabelNames = new string[1] { "controller" };
		val.Buckets = Histogram.ExponentialBuckets(1E-06, 1.5, 25);
		TickUsageControllerAfterSolveHistogram = Metrics.CreateHistogram("robust_entity_physics_controller_after_solve", "Amount of time spent running a controller's UpdateAfterSolve", val);
	}
}
