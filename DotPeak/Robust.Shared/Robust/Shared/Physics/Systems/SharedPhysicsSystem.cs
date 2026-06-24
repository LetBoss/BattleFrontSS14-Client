// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.SharedPhysicsSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.Extensions.ObjectPool;
using Prometheus;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public abstract class SharedPhysicsSystem : EntitySystem
{
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
  private const int ContactPoolInitialSize = 128 /*0x80*/;
  private const int ContactsPerThread = 32 /*0x20*/;
  private Microsoft.Extensions.ObjectPool.ObjectPool<Contact> _contactPool;
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
  private Robust.Shared.GameObjects.EntityQuery<CollideOnAnchorComponent> _anchorQuery;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixturesQuery;
  private Robust.Shared.GameObjects.EntityQuery<JointComponent> JointQuery;
  private Robust.Shared.GameObjects.EntityQuery<JointRelayTargetComponent> RelayTargetQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MapComponent> MapQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> PhysicsQuery;
  protected Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;
  private ComponentRegistration _physicsReg;
  private byte _angularVelocityIndex;
  [Robust.Shared.IoC.Dependency]
  private readonly FixtureSystem _fixtures;
  private const int MaxIslands = 256 /*0x0100*/;
  private readonly Microsoft.Extensions.ObjectPool.ObjectPool<List<Entity<PhysicsComponent, TransformComponent>>> _islandBodyPool = (Microsoft.Extensions.ObjectPool.ObjectPool<List<Entity<PhysicsComponent, TransformComponent>>>) new DefaultObjectPool<List<Entity<PhysicsComponent, TransformComponent>>>((IPooledObjectPolicy<List<Entity<PhysicsComponent, TransformComponent>>>) new ListPolicy<Entity<PhysicsComponent, TransformComponent>>(), 256 /*0x0100*/);
  private readonly Microsoft.Extensions.ObjectPool.ObjectPool<List<Contact>> _islandContactPool = (Microsoft.Extensions.ObjectPool.ObjectPool<List<Contact>>) new DefaultObjectPool<List<Contact>>((IPooledObjectPolicy<List<Contact>>) new ListPolicy<Contact>(), 256 /*0x0100*/);
  private readonly Microsoft.Extensions.ObjectPool.ObjectPool<List<(Joint Original, Joint Joint)>> _islandJointPool = (Microsoft.Extensions.ObjectPool.ObjectPool<List<(Joint, Joint)>>) new DefaultObjectPool<List<(Joint, Joint)>>((IPooledObjectPolicy<List<(Joint, Joint)>>) new ListPolicy<(Joint, Joint)>(), 256 /*0x0100*/);
  private readonly HashSet<Entity<PhysicsComponent, TransformComponent>> _islandSet = new HashSet<Entity<PhysicsComponent, TransformComponent>>(64 /*0x40*/);
  private readonly Stack<Entity<PhysicsComponent, TransformComponent>> _bodyStack = new Stack<Entity<PhysicsComponent, TransformComponent>>(64 /*0x40*/);
  private readonly List<Entity<PhysicsComponent, TransformComponent>> _awakeBodyList = new List<Entity<PhysicsComponent, TransformComponent>>(256 /*0x0100*/);
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
  private const int VelocityConstraintsPerThread = 16 /*0x10*/;
  private const int PositionConstraintsPerThread = 16 /*0x10*/;
  [Robust.Shared.IoC.Dependency]
  private readonly IGameTiming _gameTiming;
  private bool _autoClearForces;
  protected readonly Dictionary<EntityUid, EntityUid> LerpData = new Dictionary<EntityUid, EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  internal readonly HashSet<FixtureProxy> MoveBuffer = new HashSet<FixtureProxy>();
  [Robust.Shared.ViewVariables.ViewVariables]
  internal readonly HashSet<EntityUid> MovedGrids = new HashSet<EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly HashSet<Entity<PhysicsComponent, TransformComponent>> AwakeBodies = new HashSet<Entity<PhysicsComponent, TransformComponent>>();
  private float _invDt0;

  private void OnPhysicsInit(EntityUid uid, PhysicsComponent component, ComponentInit args)
  {
    TransformComponent transformComponent = this.Transform(uid);
    FixturesComponent fixturesComponent = this.EnsureComp<FixturesComponent>(uid);
    if (component.CanCollide && (this._containerSystem.IsEntityOrParentInContainer(uid) || transformComponent.MapID == MapId.Nullspace))
      this.SetCanCollide(uid, false, false, manager: fixturesComponent, body: component);
    if (component.CanCollide && component.BodyType != BodyType.Static)
      this.SetAwake((Entity<PhysicsComponent>) (uid, component), true);
    this._fixtures.OnPhysicsInit(uid, fixturesComponent, component);
    if (fixturesComponent.FixtureCount == 0)
      component.CanCollide = false;
    CollisionChangeEvent message = new CollisionChangeEvent(uid, component, component.CanCollide);
    this.RaiseLocalEvent<CollisionChangeEvent>(ref message);
  }

  private void OnPhysicsGetState(
    EntityUid uid,
    PhysicsComponent component,
    ref ComponentGetState args)
  {
    if (args.FromTick > component.CreationTick && component.LastFieldUpdate >= args.FromTick)
    {
      bool flag = false;
      for (int index = 0; index < (int) this._angularVelocityIndex; ++index)
      {
        if (!(component.LastModifiedFields[index] < args.FromTick))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        if (component.LastModifiedFields[(int) this._angularVelocityIndex] >= args.FromTick)
        {
          args.State = (IComponentState) new PhysicsVelocityDeltaState()
          {
            AngularVelocity = component.AngularVelocity,
            LinearVelocity = component.LinearVelocity
          };
          return;
        }
        args.State = (IComponentState) new PhysicsLinearVelocityDeltaState()
        {
          LinearVelocity = component.LinearVelocity
        };
        return;
      }
    }
    args.State = (IComponentState) new PhysicsComponentState()
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

  private void OnPhysicsHandleState(
    EntityUid uid,
    PhysicsComponent component,
    ref ComponentHandleState args)
  {
    if (args.Current == null)
      return;
    FixturesComponent component1;
    this._fixturesQuery.TryComp(uid, out component1);
    if (args.Current is PhysicsLinearVelocityDeltaState current2)
    {
      EntityUid uid1 = uid;
      Vector2 linearVelocity = current2.LinearVelocity;
      PhysicsComponent physicsComponent = component;
      FixturesComponent manager = component1;
      PhysicsComponent body = physicsComponent;
      this.SetLinearVelocity(uid1, linearVelocity, false, manager: manager, body: body);
    }
    else if (args.Current is PhysicsVelocityDeltaState current1)
    {
      EntityUid uid2 = uid;
      Vector2 linearVelocity = current1.LinearVelocity;
      PhysicsComponent physicsComponent1 = component;
      FixturesComponent manager1 = component1;
      PhysicsComponent body1 = physicsComponent1;
      this.SetLinearVelocity(uid2, linearVelocity, false, manager: manager1, body: body1);
      EntityUid uid3 = uid;
      double angularVelocity = (double) current1.AngularVelocity;
      PhysicsComponent physicsComponent2 = component;
      FixturesComponent manager2 = component1;
      PhysicsComponent body2 = physicsComponent2;
      this.SetAngularVelocity(uid3, (float) angularVelocity, false, manager2, body2);
    }
    else
    {
      if (!(args.Current is PhysicsComponentState current))
        return;
      this.SetSleepingAllowed(uid, component, current.SleepingAllowed, false);
      this.SetFixedRotation(uid, current.FixedRotation, false, body: component);
      this.SetCanCollide(uid, current.CanCollide, false, body: component);
      component.BodyStatus = current.Status;
      EntityUid uid4 = uid;
      Vector2 linearVelocity = current.LinearVelocity;
      PhysicsComponent physicsComponent3 = component;
      FixturesComponent manager3 = component1;
      PhysicsComponent body3 = physicsComponent3;
      this.SetLinearVelocity(uid4, linearVelocity, false, manager: manager3, body: body3);
      EntityUid uid5 = uid;
      double angularVelocity = (double) current.AngularVelocity;
      PhysicsComponent physicsComponent4 = component;
      FixturesComponent manager4 = component1;
      PhysicsComponent body4 = physicsComponent4;
      this.SetAngularVelocity(uid5, (float) angularVelocity, false, manager4, body4);
      this.SetBodyType(uid, current.BodyType, component1, component);
      this.SetFriction(uid, component, current.Friction, false);
      this.SetLinearDamping(uid, component, current.LinearDamping, false);
      this.SetAngularDamping(uid, component, current.AngularDamping, false);
      component.Force = current.Force;
      component.Torque = current.Torque;
    }
  }

  private bool IsMoveable(PhysicsComponent body)
  {
    return (body.BodyType & (BodyType.KinematicController | BodyType.Dynamic)) != 0;
  }

  public void ApplyAngularImpulse(
    EntityUid uid,
    float impulse,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this.IsMoveable(body) || !this.WakeBody(uid, manager: manager, body: body))
      return;
    this.SetAngularVelocity(uid, body.AngularVelocity + impulse * body.InvI, body: body);
  }

  public void ApplyForce(
    EntityUid uid,
    Vector2 force,
    Vector2 point,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this.IsMoveable(body) || !this.WakeBody(uid, manager: manager, body: body))
      return;
    body.Force += force;
    body.Torque += Vector2Helpers.Cross(point - body._localCenter, force);
  }

  public void ApplyForce(
    EntityUid uid,
    Vector2 force,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this.IsMoveable(body) || !this.WakeBody(uid, manager: manager, body: body))
      return;
    body.Force += force;
  }

  public void ApplyTorque(
    EntityUid uid,
    float torque,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this.IsMoveable(body) || !this.WakeBody(uid, manager: manager, body: body))
      return;
    body.Torque += torque;
    this.DirtyField<PhysicsComponent>(uid, body, "Torque");
  }

  public void ApplyLinearImpulse(
    EntityUid uid,
    Vector2 impulse,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this.IsMoveable(body) || !this.WakeBody(uid, manager: manager, body: body))
      return;
    this.SetLinearVelocity(uid, body.LinearVelocity + impulse * body._invMass, body: body);
  }

  public void ApplyLinearImpulse(
    EntityUid uid,
    Vector2 impulse,
    Vector2 point,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this.IsMoveable(body) || !this.WakeBody(uid, manager: manager, body: body))
      return;
    this.SetLinearVelocity(uid, body.LinearVelocity + impulse * body._invMass, body: body);
    this.SetAngularVelocity(uid, body.AngularVelocity + body.InvI * Vector2Helpers.Cross(point - body._localCenter, impulse), body: body);
  }

  public void DestroyContacts(PhysicsComponent body)
  {
    if (body.Contacts.Count == 0)
      return;
    LinkedListNode<Contact> next;
    for (LinkedListNode<Contact> node = body.Contacts.First; node != null; node = next)
      this.DestroyContact(node.Value, node, out next);
  }

  public void ResetDynamics(EntityUid uid, PhysicsComponent body, bool dirty = true)
  {
    body.Torque = 0.0f;
    body.AngularVelocity = 0.0f;
    body.Force = Vector2.Zero;
    body.LinearVelocity = Vector2.Zero;
    if (!dirty)
      return;
    this.DirtyFields<PhysicsComponent>(uid, body, (MetaDataComponent) null, "Torque", "AngularVelocity", "Force", "LinearVelocity");
  }

  public void ResetMassData(EntityUid uid, FixturesComponent? manager = null, PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this._fixturesQuery.Resolve(uid, ref manager))
      return;
    float mass = body._mass;
    float inertia = body._inertia;
    body._mass = 0.0f;
    body._invMass = 0.0f;
    body._inertia = 0.0f;
    body.InvI = 0.0f;
    Vector2 zero = Vector2.Zero;
    foreach (Fixture fixture in manager.Fixtures.Values)
    {
      if ((double) fixture.Density > 0.0)
      {
        MassData data = new MassData();
        FixtureSystem.GetMassData<IPhysShape>(fixture.Shape, ref data, fixture.Density);
        body._mass += data.Mass;
        zero += data.Center * data.Mass;
        body._inertia += data.I;
      }
    }
    if ((double) body._mass > 0.0)
    {
      body._invMass = 1f / body._mass;
      zero *= body._invMass;
    }
    else
    {
      body._mass = 1f;
      body._invMass = 1f;
    }
    if ((double) body._inertia > 0.0 && !body.FixedRotation)
    {
      body._inertia -= body._mass * Vector2.Dot(zero, zero);
      body.InvI = 1f / body._inertia;
    }
    else
    {
      body._inertia = 0.0f;
      body.InvI = 0.0f;
    }
    Vector2 localCenter = body._localCenter;
    body._localCenter = zero;
    if ((body.BodyType & BodyType.Static) == BodyType.Kinematic)
    {
      double angularVelocity = (double) body.AngularVelocity;
      Vector2 vector2_1 = zero - localCenter;
      ref Vector2 local = ref vector2_1;
      Vector2 vector2_2 = Vector2Helpers.Cross((float) angularVelocity, ref local);
      if (vector2_2 != Vector2.Zero)
      {
        body.LinearVelocity += vector2_2;
        this.DirtyField<PhysicsComponent>(uid, body, "LinearVelocity");
      }
    }
    if ((double) body._mass == (double) mass && (double) body._inertia == (double) inertia && localCenter == zero)
      return;
    MassDataChangedEvent args = new MassDataChangedEvent((Entity<PhysicsComponent, FixturesComponent>) (uid, body, manager), mass, inertia, localCenter);
    this.RaiseLocalEvent<MassDataChangedEvent>(uid, ref args);
  }

  public bool SetAngularVelocity(
    EntityUid uid,
    float value,
    bool dirty = true,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || body.BodyType == BodyType.Static || (double) value * (double) value > 0.0 && !this.WakeBody(uid, manager: manager, body: body) || MathHelper.CloseToPercent(body.AngularVelocity, value, 9.9999997473787516E-06))
      return false;
    body.AngularVelocity = value;
    if (dirty)
      this.DirtyField<PhysicsComponent>(uid, body, "AngularVelocity");
    return true;
  }

  public bool SetLinearVelocity(
    EntityUid uid,
    Vector2 velocity,
    bool dirty = true,
    bool wakeBody = true,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || body.BodyType == BodyType.Static || wakeBody && (double) Vector2.Dot(velocity, velocity) > 0.0 && !this.WakeBody(uid, manager: manager, body: body) || Vector2Helpers.EqualsApprox(body.LinearVelocity, velocity, 1.0000000116860974E-07))
      return false;
    body.LinearVelocity = velocity;
    if (dirty)
      this.DirtyField<PhysicsComponent>(uid, body, "LinearVelocity");
    return true;
  }

  public void SetAngularDamping(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
  {
    if (MathHelper.CloseTo(body.AngularDamping, value, 1E-07f))
      return;
    body.AngularDamping = value;
    if (!dirty)
      return;
    this.DirtyField<PhysicsComponent>(uid, body, "AngularDamping");
  }

  public void SetLinearDamping(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
  {
    if (MathHelper.CloseTo(body.LinearDamping, value, 1E-07f))
      return;
    body.LinearDamping = value;
    if (!dirty)
      return;
    this.DirtyField<PhysicsComponent>(uid, body, "LinearDamping");
  }

  [Obsolete("Use SetAwake with EntityUid<PhysicsComponent>")]
  public void SetAwake(EntityUid uid, PhysicsComponent body, bool value, bool updateSleepTime = true)
  {
    this.SetAwake(new Entity<PhysicsComponent>(uid, body), value, updateSleepTime);
  }

  public void SetAwake(Entity<PhysicsComponent> ent, bool value, bool updateSleepTime = true)
  {
    (EntityUid entityUid, PhysicsComponent physicsComponent) = ent;
    bool flag = physicsComponent.BodyType != BodyType.Static && physicsComponent.CanCollide;
    if (physicsComponent.Awake == value || value && !flag)
      return;
    physicsComponent.Awake = value;
    if (value)
    {
      PhysicsWakeEvent args = new PhysicsWakeEvent(entityUid, physicsComponent);
      this.RaiseLocalEvent<PhysicsWakeEvent>(entityUid, ref args, true);
    }
    else
    {
      PhysicsSleepEvent args = new PhysicsSleepEvent(entityUid, physicsComponent);
      this.RaiseLocalEvent<PhysicsSleepEvent>(entityUid, ref args, true);
      this.ResetDynamics((EntityUid) ent, physicsComponent, false);
    }
    if (!value && physicsComponent.CanCollide)
      this._wakeSystem.UpdateCanCollide(ent, false, false);
    if (updateSleepTime)
      this.SetSleepTime(physicsComponent, 0.0f);
    if (physicsComponent.Awake != value)
    {
      this.Log.Error($"Found a corrupted physics awake state for {this.ToPrettyString(new EntityUid?((EntityUid) ent))}! Did you forget to cancel the sleep subscription? Forcing body awake");
      physicsComponent.Awake = true;
    }
    if (physicsComponent.Awake)
      this.AddAwakeBody((Entity<PhysicsComponent, TransformComponent>) (entityUid, physicsComponent, this.Transform(entityUid)));
    else
      this.RemoveSleepBody((Entity<PhysicsComponent, TransformComponent>) (entityUid, physicsComponent, this.Transform(entityUid)));
  }

  public void TrySetBodyType(
    EntityUid uid,
    BodyType value,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this._fixturesQuery.Resolve(uid, ref manager, false) || !this.PhysicsQuery.Resolve(uid, ref body, false) || !this.XformQuery.Resolve(uid, ref xform, false))
      return;
    this.SetBodyType(uid, value, manager, body, xform);
  }

  public void SetBodyType(
    EntityUid uid,
    BodyType value,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || body.BodyType == value)
      return;
    BodyType bodyType = body.BodyType;
    body.BodyType = value;
    body.Force = Vector2.Zero;
    body.Torque = 0.0f;
    if (body.BodyType == BodyType.Static)
    {
      this.SetAwake((Entity<PhysicsComponent>) (uid, body), false);
      body.LinearVelocity = Vector2.Zero;
      body.AngularVelocity = 0.0f;
      this.DirtyFields<PhysicsComponent>(uid, body, (MetaDataComponent) null, "LinearVelocity", "AngularVelocity", "Force", "Torque");
    }
    else if (body.CanCollide)
    {
      this.SetAwake((Entity<PhysicsComponent>) (uid, body), true);
      this.DirtyFields<PhysicsComponent>(uid, body, (MetaDataComponent) null, "Force", "Torque");
    }
    this._broadphase.RegenerateContacts((Entity<PhysicsComponent, FixturesComponent, TransformComponent>) (uid, body, manager, xform));
    if (!body.Initialized)
      return;
    PhysicsBodyTypeChangedEvent args = new PhysicsBodyTypeChangedEvent(uid, body.BodyType, bodyType, body);
    this.RaiseLocalEvent<PhysicsBodyTypeChangedEvent>(uid, ref args, true);
  }

  public void SetBodyStatus(EntityUid uid, PhysicsComponent body, BodyStatus status, bool dirty = true)
  {
    if (body.BodyStatus == status)
      return;
    body.BodyStatus = status;
    if (!dirty)
      return;
    this.DirtyField<PhysicsComponent>(uid, body, "BodyStatus");
  }

  public bool SetCanCollide(
    EntityUid uid,
    bool value,
    bool dirty = true,
    bool force = false,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body))
      return false;
    if (body.CanCollide == value)
      return value;
    if (value && !force && (this._containerSystem.IsEntityOrParentInContainer(uid) || !this._fixturesQuery.Resolve(uid, ref manager) || manager.FixtureCount == 0 && !this._gridQuery.HasComp(uid)))
      return false;
    body.CanCollide = value;
    if (!value)
      this.SetAwake((Entity<PhysicsComponent>) (uid, body), false);
    if (body.Initialized)
    {
      CollisionChangeEvent message = new CollisionChangeEvent(uid, body, value);
      this.RaiseLocalEvent<CollisionChangeEvent>(ref message);
    }
    if (dirty)
      this.DirtyField<PhysicsComponent>(uid, body, "CanCollide");
    return value;
  }

  public void SetFixedRotation(
    EntityUid uid,
    bool value,
    bool dirty = true,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || body.FixedRotation == value)
      return;
    body.FixedRotation = value;
    body.AngularVelocity = 0.0f;
    if (dirty)
      this.DirtyFields<PhysicsComponent>(uid, body, (MetaDataComponent) null, "FixedRotation", "AngularVelocity");
    this.ResetMassData(uid, manager, body);
  }

  public void SetFriction(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
  {
    if (MathHelper.CloseTo(body.Friction, value, 1E-07f))
      return;
    body._friction = value;
    if (!dirty)
      return;
    this.DirtyField<PhysicsComponent>(uid, body, "Friction");
  }

  public void SetInertia(EntityUid uid, PhysicsComponent body, float value, bool dirty = true)
  {
    if (body.BodyType != BodyType.Dynamic || MathHelper.CloseToPercent(body._inertia, value, 1E-05) || (double) value <= 0.0 || body.FixedRotation)
      return;
    body._inertia = value - body.Mass * Vector2.Dot(body._localCenter, body._localCenter);
    body.InvI = 1f / body._inertia;
  }

  public void SetLocalCenter(EntityUid uid, PhysicsComponent body, Vector2 value)
  {
    if (body.BodyType != BodyType.Dynamic || Vector2Helpers.EqualsApprox(value, body._localCenter))
      return;
    body._localCenter = value;
  }

  public void SetSleepingAllowed(EntityUid uid, PhysicsComponent body, bool value, bool dirty = true)
  {
    if (body.SleepingAllowed == value)
      return;
    if (!value)
      this.SetAwake((Entity<PhysicsComponent>) (uid, body), true);
    body.SleepingAllowed = value;
    if (!dirty)
      return;
    this.DirtyField<PhysicsComponent>(uid, body, "SleepingAllowed");
  }

  public void SetSleepTime(PhysicsComponent body, float value)
  {
    if (MathHelper.CloseToPercent(value, body.SleepTime, 1E-05))
      return;
    body.SleepTime = value;
  }

  public bool WakeBody(
    EntityUid uid,
    bool force = false,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body))
      return false;
    EntityUid uid1 = uid;
    FixturesComponent fixturesComponent = manager;
    PhysicsComponent physicsComponent = body;
    int num = force ? 1 : 0;
    FixturesComponent manager1 = fixturesComponent;
    PhysicsComponent body1 = physicsComponent;
    if (!this.SetCanCollide(uid1, true, force: num != 0, manager: manager1, body: body1))
      return false;
    this.SetAwake((Entity<PhysicsComponent>) (uid, body), true);
    return body.Awake;
  }

  public Robust.Shared.Physics.Transform GetRelativePhysicsTransform(
    Robust.Shared.Physics.Transform worldTransform,
    Entity<TransformComponent?> relative)
  {
    if (!this.XformQuery.Resolve(relative.Owner, ref relative.Comp))
      return Robust.Shared.Physics.Transform.Empty;
    (Vector2 _, Angle WorldRotation, Matrix3x2 _, Matrix3x2 matrix3x2) = this._transform.GetWorldPositionRotationMatrixWithInv(relative.Comp);
    return new Robust.Shared.Physics.Transform(Vector2.Transform(worldTransform.Position, matrix3x2), Angle.op_Subtraction(Angle.op_Implicit(worldTransform.Quaternion2D.Angle), WorldRotation));
  }

  public Robust.Shared.Physics.Transform GetRelativePhysicsTransform(
    Entity<TransformComponent?> entity,
    Entity<TransformComponent?> relative)
  {
    if (!this.XformQuery.Resolve(entity.Owner, ref entity.Comp) || !this.XformQuery.Resolve(relative.Owner, ref relative.Comp))
      return Robust.Shared.Physics.Transform.Empty;
    (Vector2 vector2, Angle WorldRotation1) = this._transform.GetWorldPositionRotation(entity.Comp);
    (Vector2 _, Angle WorldRotation2, Matrix3x2 _, Matrix3x2 matrix3x2) = this._transform.GetWorldPositionRotationMatrixWithInv(relative.Comp);
    return new Robust.Shared.Physics.Transform(Vector2.Transform(vector2, matrix3x2), Angle.op_Subtraction(WorldRotation1, WorldRotation2));
  }

  public Robust.Shared.Physics.Transform GetLocalPhysicsTransform(
    EntityUid uid,
    TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform) || !xform.Broadphase.HasValue)
      return Robust.Shared.Physics.Transform.Empty;
    EntityUid uid1 = xform.Broadphase.Value.Uid;
    return xform.ParentUid == uid1 ? new Robust.Shared.Physics.Transform(xform.LocalPosition, xform.LocalRotation) : this.GetRelativePhysicsTransform((Entity<TransformComponent>) (uid, xform), (Entity<TransformComponent>) uid1);
  }

  public Robust.Shared.Physics.Transform GetPhysicsTransform(
    EntityUid uid,
    TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return Robust.Shared.Physics.Transform.Empty;
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(xform);
    return new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
  }

  public Box2 GetWorldAABB(
    EntityUid uid,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this.Resolve<FixturesComponent, PhysicsComponent, TransformComponent>(uid, ref manager, ref body, ref xform))
      return new Box2();
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(xform);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, (float) positionRotation.WorldRotation.Theta);
    Box2 worldAabb;
    // ISSUE: explicit constructor call
    ((Box2) ref worldAabb).\u002Ector(transform.Position, transform.Position);
    foreach (Fixture fixture in manager.Fixtures.Values)
    {
      for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
      {
        Box2 aabb = fixture.Shape.ComputeAABB(transform, childIndex);
        worldAabb = ((Box2) ref worldAabb).Union(ref aabb);
      }
    }
    return worldAabb;
  }

  public Box2 GetHardAABB(
    EntityUid uid,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref body) || !this._fixturesQuery.Resolve(uid, ref manager) || !this.Resolve(uid, ref xform))
      return Box2.Empty;
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(xform);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, (float) positionRotation.WorldRotation.Theta);
    Box2 hardAabb;
    // ISSUE: explicit constructor call
    ((Box2) ref hardAabb).\u002Ector(transform.Position, transform.Position);
    foreach (Fixture fixture in manager.Fixtures.Values)
    {
      if (fixture.Hard)
      {
        for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
        {
          Box2 aabb = fixture.Shape.ComputeAABB(transform, childIndex);
          hardAabb = ((Box2) ref hardAabb).Union(ref aabb);
        }
      }
    }
    return hardAabb;
  }

  public (int Layer, int Mask) GetHardCollision(EntityUid uid, FixturesComponent? manager = null)
  {
    return !this._fixturesQuery.Resolve(uid, ref manager, false) ? (0, 0) : SharedPhysicsSystem.GetHardCollision(manager);
  }

  public static (int Layer, int Mask) GetHardCollision(FixturesComponent manager)
  {
    int num1 = 0;
    int num2 = 0;
    foreach (Fixture fixture in manager.Fixtures.Values)
    {
      if (fixture.Hard)
      {
        num1 |= fixture.CollisionLayer;
        num2 |= fixture.CollisionMask;
      }
    }
    return (num1, num2);
  }

  public virtual void UpdateIsPredicted(EntityUid? uid, PhysicsComponent? physics = null)
  {
  }

  private int ContactCount => this._activeContacts.Count;

  private static void SetContact(
    Contact contact,
    bool enabled,
    Entity<PhysicsComponent?, TransformComponent?> entA,
    Entity<PhysicsComponent?, TransformComponent?> entB,
    string fixtureAId,
    string fixtureBId,
    Fixture? fixtureA,
    int indexA,
    Fixture? fixtureB,
    int indexB)
  {
    EntityUid owner1 = entA.Owner;
    EntityUid owner2 = entB.Owner;
    contact.Enabled = enabled;
    contact.IsTouching = false;
    contact.EntityA = owner1;
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
    contact.TangentSpeed = 0.0f;
  }

  private void InitializeContacts()
  {
    this._contactPool = (Microsoft.Extensions.ObjectPool.ObjectPool<Contact>) new DefaultObjectPool<Contact>((IPooledObjectPolicy<Contact>) new SharedPhysicsSystem.ContactPoolPolicy(this._debugPhysics, this._manifoldManager), 4096 /*0x1000*/);
    this.InitializePool();
    this.EntityManager.EntityQueueDeleted += new Action<EntityUid>(this.OnContactEntityQueueDel);
  }

  private void ShutdownContacts()
  {
    this.EntityManager.EntityQueueDeleted -= new Action<EntityUid>(this.OnContactEntityQueueDel);
  }

  private void OnContactEntityQueueDel(EntityUid obj)
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>(obj, out comp))
      return;
    this.DestroyContacts(comp);
  }

  private void InitializePool()
  {
    Contact[] contactArray = new Contact[128 /*0x80*/];
    for (int index = 0; index < 128 /*0x80*/; ++index)
      contactArray[index] = this._contactPool.Get();
    for (int index = 0; index < 128 /*0x80*/; ++index)
      this._contactPool.Return(contactArray[index]);
  }

  private Contact CreateContact(
    Entity<PhysicsComponent?, TransformComponent?> entA,
    Entity<PhysicsComponent?, TransformComponent?> entB,
    string fixtureAId,
    string fixtureBId,
    Fixture fixtureA,
    int indexA,
    Fixture fixtureB,
    int indexB)
  {
    ShapeType shapeType1 = fixtureA.Shape.ShapeType;
    ShapeType shapeType2 = fixtureB.Shape.ShapeType;
    Contact contact = this._contactPool.Get();
    contact.Flags = ContactFlags.PreInit;
    if ((shapeType1 >= shapeType2 || shapeType1 == ShapeType.Edge && shapeType2 == ShapeType.Polygon) && (shapeType2 != ShapeType.Edge || shapeType1 != ShapeType.Polygon))
      SharedPhysicsSystem.SetContact(contact, true, entA, entB, fixtureAId, fixtureBId, fixtureA, indexA, fixtureB, indexB);
    else
      SharedPhysicsSystem.SetContact(contact, true, entB, entA, fixtureBId, fixtureAId, fixtureB, indexB, fixtureA, indexA);
    contact.Type = SharedPhysicsSystem._registers[(int) shapeType1, (int) shapeType2];
    return contact;
  }

  internal void AddPair(
    Entity<PhysicsComponent, TransformComponent> entA,
    Entity<PhysicsComponent, TransformComponent> entB,
    string fixtureAId,
    string fixtureBId,
    Fixture fixtureA,
    int indexA,
    Fixture fixtureB,
    int indexB,
    PhysicsComponent bodyA,
    PhysicsComponent bodyB,
    ContactFlags flags = ContactFlags.None)
  {
    TransformComponent comp2_1 = entA.Comp2;
    TransformComponent comp2_2 = entB.Comp2;
    if (!this.ShouldCollideSlow(entA.Owner, entB.Owner, bodyA, bodyB, fixtureA, fixtureB, comp2_1, comp2_2))
      return;
    Contact contact = this.CreateContact((Entity<PhysicsComponent, TransformComponent>) (entA.Owner, entA.Comp1, entA.Comp2), (Entity<PhysicsComponent, TransformComponent>) (entB.Owner, entB.Comp1, entB.Comp2), fixtureAId, fixtureBId, fixtureA, indexA, fixtureB, indexB);
    contact.Flags = flags;
    Fixture fixtureA1 = contact.FixtureA;
    Fixture fixtureB1 = contact.FixtureB;
    PhysicsComponent bodyA1 = contact.BodyA;
    PhysicsComponent bodyB1 = contact.BodyB;
    this._activeContacts.AddLast(contact.MapNode);
    fixtureA1.Contacts.Add(fixtureB1, contact);
    bodyA1.Contacts.AddLast(contact.BodyANode);
    fixtureB1.Contacts.Add(fixtureA1, contact);
    bodyB1.Contacts.AddLast(contact.BodyBNode);
    if (bodyA.BodyType != BodyType.Static || bodyB.Awake)
      return;
    this.WakeBody(entB.Owner, body: bodyB);
  }

  internal void AddPair(
    string fixtureAId,
    string fixtureBId,
    in FixtureProxy proxyA,
    in FixtureProxy proxyB,
    ContactFlags flags = ContactFlags.None)
  {
    this.AddPair((Entity<PhysicsComponent, TransformComponent>) (proxyA.Entity, proxyA.Body, proxyA.Xform), (Entity<PhysicsComponent, TransformComponent>) (proxyB.Entity, proxyB.Body, proxyB.Xform), fixtureAId, fixtureBId, proxyA.Fixture, proxyA.ChildIndex, proxyB.Fixture, proxyB.ChildIndex, proxyA.Body, proxyB.Body, flags);
  }

  internal static bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
  {
    return (fixtureA.CollisionMask & fixtureB.CollisionLayer) != 0 || (fixtureB.CollisionMask & fixtureA.CollisionLayer) != 0;
  }

  public void DestroyContact(Contact contact)
  {
    this.DestroyContact(contact, (LinkedListNode<Contact>) null, out LinkedListNode<Contact> _);
  }

  internal void DestroyContact(
    Contact contact,
    LinkedListNode<Contact>? node,
    out LinkedListNode<Contact>? next)
  {
    if ((contact.Flags & (ContactFlags.Deleting | ContactFlags.Deleted)) != ContactFlags.None)
    {
      next = node?.Next;
    }
    else
    {
      Fixture fixtureA1 = contact.FixtureA;
      Fixture fixtureB1 = contact.FixtureB;
      PhysicsComponent bodyA = contact.BodyA;
      PhysicsComponent bodyB = contact.BodyB;
      EntityUid entityA = contact.EntityA;
      EntityUid entityB = contact.EntityB;
      contact.Flags |= ContactFlags.Deleting;
      if (contact.IsTouching)
      {
        EndCollideEvent args1 = new EndCollideEvent(entityA, entityB, contact.FixtureAId, contact.FixtureBId, fixtureA1, fixtureB1, bodyA, bodyB);
        EndCollideEvent args2 = new EndCollideEvent(entityB, entityA, contact.FixtureBId, contact.FixtureAId, fixtureB1, fixtureA1, bodyB, bodyA);
        this.RaiseLocalEvent<EndCollideEvent>(entityA, ref args1);
        this.RaiseLocalEvent<EndCollideEvent>(entityB, ref args2);
      }
      if (contact.Manifold.PointCount > 0)
      {
        Fixture fixtureA2 = contact.FixtureA;
        if ((fixtureA2 != null ? (fixtureA2.Hard ? 1 : 0) : 0) != 0)
        {
          Fixture fixtureB2 = contact.FixtureB;
          if ((fixtureB2 != null ? (fixtureB2.Hard ? 1 : 0) : 0) != 0)
          {
            if (bodyA.CanCollide && !this.TerminatingOrDeleted(entityA))
              this.SetAwake((Entity<PhysicsComponent>) (entityA, bodyA), true);
            if (bodyB.CanCollide && !this.TerminatingOrDeleted(entityB))
              this.SetAwake((Entity<PhysicsComponent>) (entityB, bodyB), true);
          }
        }
      }
      next = node?.Next;
      this._activeContacts.Remove(contact.MapNode);
      fixtureA1.Contacts.Remove(fixtureB1);
      bodyA.Contacts.Remove(contact.BodyANode);
      fixtureB1.Contacts.Remove(fixtureA1);
      bodyB.Contacts.Remove(contact.BodyBNode);
      contact.Flags = ContactFlags.Deleted;
      this._contactPool.Return(contact);
    }
  }

  internal void CollideContacts()
  {
    Contact[] contactArray = ArrayPool<Contact>.Shared.Rent(this.ContactCount);
    int num1 = 0;
    LinkedListNode<Contact> linkedListNode = this._activeContacts.First;
    while (linkedListNode != null)
    {
      Contact contact = linkedListNode.Value;
      linkedListNode = linkedListNode.Next;
      if (contact.Enabled)
      {
        contact.Flags &= ~ContactFlags.PreInit;
        Fixture fixtureA = contact.FixtureA;
        Fixture fixtureB = contact.FixtureB;
        int childIndexA = contact.ChildIndexA;
        int childIndexB = contact.ChildIndexB;
        PhysicsComponent bodyA = contact.BodyA;
        PhysicsComponent bodyB = contact.BodyB;
        EntityUid entityA = contact.EntityA;
        EntityUid entityB = contact.EntityB;
        if (!bodyA.CanCollide || !bodyB.CanCollide)
        {
          this.DestroyContact(contact);
        }
        else
        {
          TransformComponent xformA = contact.XformA;
          TransformComponent xformB = contact.XformB;
          if (xformA.MapID == MapId.Nullspace || xformB.MapID == MapId.Nullspace)
          {
            this.DestroyContact(contact);
          }
          else
          {
            if ((contact.Flags & ContactFlags.Filter) != ContactFlags.None)
            {
              if (!SharedPhysicsSystem.ShouldCollide(fixtureA, fixtureB) || !this.ShouldCollideSlow(entityA, entityB, bodyA, bodyB, fixtureA, fixtureB, xformA, xformB) || !this.ShouldCollideJoints((Entity<JointComponent>) entityA, (Entity<JointComponent>) entityB))
              {
                this.DestroyContact(contact);
                continue;
              }
              contact.Flags &= ~ContactFlags.Filter;
            }
            int num2 = !bodyA.Awake ? 0 : (bodyA.BodyType != BodyType.Static ? 1 : 0);
            bool flag1 = bodyB.Awake && bodyB.BodyType != BodyType.Static;
            if (num2 != 0 || flag1)
            {
              EntityUid? nullable1 = xformA.MapUid;
              if (nullable1.HasValue)
              {
                nullable1 = xformA.MapUid;
                EntityUid? nullable2 = xformB.MapUid;
                if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
                {
                  if ((contact.Flags & ContactFlags.Grid) != ContactFlags.None)
                  {
                    Box2 aabb1 = fixtureA.Shape.ComputeAABB(this.GetPhysicsTransform(entityA, xformA), 0);
                    Box2 aabb2 = fixtureB.Shape.ComputeAABB(this.GetPhysicsTransform(entityB, xformB), 0);
                    if (!((Box2) ref aabb1).Intersects(ref aabb2))
                    {
                      this.DestroyContact(contact);
                      continue;
                    }
                    contact.Flags &= ~ContactFlags.Island;
                    if (num1 >= contactArray.Length)
                      this.Log.Error($"Insufficient contact length at 388! Index {num1} and length is {contactArray.Length}. Tell Sloth");
                    contactArray[num1++] = contact;
                    continue;
                  }
                  if (childIndexA >= fixtureA.Proxies.Length)
                  {
                    this.Log.Error($"Found invalid contact index of {childIndexA} on {contact.FixtureAId} / {this.ToPrettyString((Entity<MetaDataComponent>) entityA)}, expected {fixtureA.Proxies.Length}");
                    this.DestroyContact(contact);
                    continue;
                  }
                  if (childIndexB >= fixtureB.Proxies.Length)
                  {
                    this.Log.Error($"Found invalid contact index of {childIndexB} on {contact.FixtureBId} / {this.ToPrettyString((Entity<MetaDataComponent>) entityB)}, expected {fixtureB.Proxies.Length}");
                    this.DestroyContact(contact);
                    continue;
                  }
                  FixtureProxy proxy1 = fixtureA.Proxies[childIndexA];
                  FixtureProxy proxy2 = fixtureB.Proxies[childIndexB];
                  ref BroadphaseData? local1 = ref xformA.Broadphase;
                  EntityUid? nullable3;
                  if (!local1.HasValue)
                  {
                    nullable2 = new EntityUid?();
                    nullable3 = nullable2;
                  }
                  else
                    nullable3 = new EntityUid?(local1.GetValueOrDefault().Uid);
                  EntityUid? nullable4 = nullable3;
                  ref BroadphaseData? local2 = ref xformB.Broadphase;
                  EntityUid? nullable5;
                  if (!local2.HasValue)
                  {
                    nullable2 = new EntityUid?();
                    nullable5 = nullable2;
                  }
                  else
                    nullable5 = new EntityUid?(local2.GetValueOrDefault().Uid);
                  EntityUid? nullable6 = nullable5;
                  bool flag2 = false;
                  if (nullable4.HasValue && nullable6.HasValue)
                  {
                    nullable2 = nullable4;
                    nullable1 = nullable6;
                    if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
                    {
                      flag2 = ((Box2) ref proxy1.AABB).Intersects(ref proxy2.AABB);
                    }
                    else
                    {
                      Box2 box2_1 = Matrix3Helpers.TransformBox(this._transform.GetWorldMatrix(this.XformQuery.GetComponent(nullable4.Value)), ref proxy1.AABB);
                      Box2 box2_2 = Matrix3Helpers.TransformBox(this._transform.GetWorldMatrix(this.XformQuery.GetComponent(nullable6.Value)), ref proxy2.AABB);
                      flag2 = ((Box2) ref box2_1).Intersects(ref box2_2);
                    }
                  }
                  if (!flag2)
                  {
                    this.DestroyContact(contact);
                    continue;
                  }
                  contact.Flags &= ~ContactFlags.Island;
                  if (num1 >= contactArray.Length)
                    this.Log.Error($"Insufficient contact length at 429! Index {num1} and length is {contactArray.Length}. Tell Sloth");
                  contactArray[num1++] = contact;
                  continue;
                }
              }
              this.DestroyContact(contact);
            }
          }
        }
      }
    }
    ContactStatus[] contactStatusArray = ArrayPool<ContactStatus>.Shared.Rent(num1);
    FixedArray4<Vector2>[] fixedArray4Array = ArrayPool<FixedArray4<Vector2>>.Shared.Rent(num1);
    this.BuildManifolds(contactArray, num1, contactStatusArray, fixedArray4Array);
    for (int index = 0; index < num1; ++index)
    {
      if (index >= contactArray.Length)
      {
        this.Log.Error("Invalid contact length for contact events!");
      }
      else
      {
        Contact contact = contactArray[index];
        if (contact.Enabled)
          this.RunContactEvents(contactStatusArray[index], contact, fixedArray4Array[index]);
      }
    }
    ArrayPool<Contact>.Shared.Return(contactArray);
    ArrayPool<ContactStatus>.Shared.Return(contactStatusArray);
    ArrayPool<FixedArray4<Vector2>>.Shared.Return(fixedArray4Array);
  }

  internal void RunContactEvents(
    ContactStatus status,
    Contact contact,
    FixedArray4<Vector2> worldPoint)
  {
    switch (status)
    {
      case ContactStatus.NoContact:
        break;
      case ContactStatus.StartTouching:
        if (!contact.IsTouching)
          break;
        Fixture fixtureA1 = contact.FixtureA;
        Fixture fixtureB1 = contact.FixtureB;
        PhysicsComponent bodyA1 = contact.BodyA;
        PhysicsComponent bodyB1 = contact.BodyB;
        EntityUid entityA1 = contact.EntityA;
        EntityUid entityB1 = contact.EntityB;
        FixedArray2<Vector2> worldPoints = new FixedArray2<Vector2>(worldPoint._00, worldPoint._01);
        Vector2 worldNormal = worldPoint._02;
        StartCollideEvent args1 = new StartCollideEvent(entityA1, entityB1, contact.FixtureAId, contact.FixtureBId, fixtureA1, fixtureB1, bodyA1, bodyB1, worldPoints, contact.Manifold.PointCount, worldNormal);
        StartCollideEvent args2 = new StartCollideEvent(entityB1, entityA1, contact.FixtureBId, contact.FixtureAId, fixtureB1, fixtureA1, bodyB1, bodyA1, worldPoints, contact.Manifold.PointCount, worldNormal);
        this.RaiseLocalEvent<StartCollideEvent>(entityA1, ref args1, true);
        this.RaiseLocalEvent<StartCollideEvent>(entityB1, ref args2, true);
        break;
      case ContactStatus.Touching:
        break;
      case ContactStatus.EndTouching:
        Fixture fixtureA2 = contact.FixtureA;
        Fixture fixtureB2 = contact.FixtureB;
        if (fixtureA2 == null || fixtureB2 == null)
          break;
        PhysicsComponent bodyA2 = contact.BodyA;
        PhysicsComponent bodyB2 = contact.BodyB;
        EntityUid entityA2 = contact.EntityA;
        EntityUid entityB2 = contact.EntityB;
        EndCollideEvent args3 = new EndCollideEvent(entityA2, entityB2, contact.FixtureAId, contact.FixtureBId, fixtureA2, fixtureB2, bodyA2, bodyB2);
        EndCollideEvent args4 = new EndCollideEvent(entityB2, entityA2, contact.FixtureBId, contact.FixtureAId, fixtureB2, fixtureA2, bodyB2, bodyA2);
        this.RaiseLocalEvent<EndCollideEvent>(entityA2, ref args3);
        this.RaiseLocalEvent<EndCollideEvent>(entityB2, ref args4);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private void BuildManifolds(
    Contact[] contacts,
    int count,
    ContactStatus[] status,
    FixedArray4<Vector2>[] worldPoints)
  {
    if (count == 0)
      return;
    bool[] array = ArrayPool<bool>.Shared.Rent(count);
    this._parallel.ProcessNow((IParallelRobustJob) new SharedPhysicsSystem.ManifoldsJob()
    {
      Physics = this,
      Status = status,
      WorldPoints = worldPoints,
      Contacts = contacts,
      Wake = array
    }, count);
    for (int index = 0; index < count; ++index)
    {
      if (array[index])
      {
        Contact contact = contacts[index];
        PhysicsComponent bodyA = contact.BodyA;
        PhysicsComponent bodyB = contact.BodyB;
        EntityUid entityA = contact.EntityA;
        EntityUid entityB = contact.EntityB;
        this.SetAwake((Entity<PhysicsComponent>) (entityA, bodyA), true);
        this.SetAwake((Entity<PhysicsComponent>) (entityB, bodyB), true);
      }
    }
    ArrayPool<bool>.Shared.Return(array);
  }

  private void UpdateContact(
    Contact[] contacts,
    int index,
    ContactStatus[] status,
    bool[] wake,
    FixedArray4<Vector2>[] worldPoints)
  {
    Contact contact = contacts[index];
    if (!contact.Enabled)
    {
      status[index] = ContactStatus.NoContact;
      wake[index] = false;
    }
    else
    {
      EntityUid entityA = contact.EntityA;
      EntityUid entityB = contact.EntityB;
      Robust.Shared.Physics.Transform physicsTransform1 = this.GetPhysicsTransform(entityA);
      Robust.Shared.Physics.Transform physicsTransform2 = this.GetPhysicsTransform(entityB);
      ContactStatus contactStatus = contact.Update(physicsTransform1, physicsTransform2, out wake[index]);
      status[index] = contactStatus;
      if (contactStatus != ContactStatus.StartTouching)
        return;
      FixedArray4<Vector2> fixedArray4 = new FixedArray4<Vector2>();
      Vector2 normal;
      contact.GetWorldManifold(physicsTransform1, physicsTransform2, out normal, fixedArray4.AsSpan);
      fixedArray4._02 = normal;
      worldPoints[index] = fixedArray4;
    }
  }

  internal bool ShouldCollideJoints(Entity<JointComponent?> entA, Entity<JointComponent?> entB)
  {
    if (this.JointQuery.Resolve(entA.Owner, ref entA.Comp, false) && this.JointQuery.HasComp((EntityUid) entB))
    {
      foreach (Joint joint in entA.Comp.Joints.Values)
      {
        if (!joint.CollideConnected && (entB.Owner == joint.BodyAUid || entB.Owner == joint.BodyBUid))
          return false;
      }
    }
    return true;
  }

  internal bool ShouldCollideSlow(
    EntityUid uid,
    EntityUid other,
    PhysicsComponent body,
    PhysicsComponent otherBody,
    Fixture fixture,
    Fixture otherFixture,
    TransformComponent xform,
    TransformComponent otherXform)
  {
    if ((body.BodyType & BodyType.Static) != BodyType.Kinematic && (otherBody.BodyType & BodyType.Static) != BodyType.Kinematic || fixture.Hard && body.BodyType == BodyType.KinematicController && otherFixture.Hard && otherBody.BodyType == BodyType.KinematicController || fixture.Hard && otherFixture.Hard && (uid == other || other == xform.ParentUid && body.BodyType == BodyType.Static || uid == otherXform.ParentUid && otherBody.BodyType == BodyType.Static))
      return false;
    PreventCollideEvent args = new PreventCollideEvent(uid, other, body, otherBody, fixture, otherFixture);
    this.RaiseLocalEvent<PreventCollideEvent>(uid, ref args);
    if (args.Cancelled)
      return false;
    args = new PreventCollideEvent(other, uid, otherBody, body, otherFixture, fixture);
    this.RaiseLocalEvent<PreventCollideEvent>(other, ref args);
    return !args.Cancelled;
  }

  public void RegenerateContacts(Entity<PhysicsComponent?> entity)
  {
    if (!this.PhysicsQuery.Resolve(entity.Owner, ref entity.Comp))
      return;
    this._broadphase.RegenerateContacts((Entity<PhysicsComponent, FixturesComponent, TransformComponent>) entity);
  }

  public int GetTouchingContacts(Entity<FixturesComponent?> entity, string? ignoredFixtureId = null)
  {
    if (!this._fixturesQuery.Resolve(entity.Owner, ref entity.Comp))
      return 0;
    int touchingContacts = 0;
    foreach ((string key, Fixture fixture) in entity.Comp.Fixtures)
    {
      if (!(ignoredFixtureId == key))
      {
        foreach (Contact contact in fixture.Contacts.Values)
        {
          if (contact.IsTouching)
            ++touchingContacts;
        }
      }
    }
    return touchingContacts;
  }

  public ContactEnumerator GetContacts(Entity<FixturesComponent?> entity, bool includeDeleting = false)
  {
    this._fixturesQuery.Resolve(entity.Owner, ref entity.Comp);
    return new ContactEnumerator(entity.Comp, includeDeleting);
  }

  public bool MetricsEnabled { get; protected set; }

  public override void Initialize()
  {
    base.Initialize();
    this._physicsReg = this.EntityManager.ComponentFactory.GetRegistration(CompIdx.Index<PhysicsComponent>());
    this.EntityManager.ComponentFactory.RegisterNetworkedFields(this._physicsReg, "CanCollide", "BodyStatus", "BodyType", "SleepingAllowed", "FixedRotation", "Friction", "Force", "Torque", "LinearDamping", "AngularDamping", "AngularVelocity", "LinearVelocity");
    this._angularVelocityIndex = (byte) 10;
    this._anchorQuery = this.GetEntityQuery<CollideOnAnchorComponent>();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    this.JointQuery = this.GetEntityQuery<JointComponent>();
    this.RelayTargetQuery = this.GetEntityQuery<JointRelayTargetComponent>();
    this.MapQuery = this.GetEntityQuery<MapComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this.PhysicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.XformQuery = this.GetEntityQuery<TransformComponent>();
    this.SubscribeLocalEvent<GridAddEvent>(new EntityEventHandler<GridAddEvent>(this.OnGridAdd));
    this.SubscribeLocalEvent<CollisionChangeEvent>(new EntityEventRefHandler<CollisionChangeEvent>(this.OnCollisionChange));
    this.SubscribeLocalEvent<PhysicsComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<PhysicsComponent, EntGotRemovedFromContainerMessage>(this.HandleContainerRemoved));
    this.SubscribeLocalEvent<PhysicsComponent, ComponentInit>(new ComponentEventHandler<PhysicsComponent, ComponentInit>(this.OnPhysicsInit));
    this.SubscribeLocalEvent<PhysicsComponent, ComponentShutdown>(new ComponentEventHandler<PhysicsComponent, ComponentShutdown>(this.OnPhysicsShutdown));
    this.SubscribeLocalEvent<PhysicsComponent, ComponentGetState>(new ComponentEventRefHandler<PhysicsComponent, ComponentGetState>(this.OnPhysicsGetState));
    this.SubscribeLocalEvent<PhysicsComponent, ComponentHandleState>(new ComponentEventRefHandler<PhysicsComponent, ComponentHandleState>(this.OnPhysicsHandleState));
    this.InitializeIsland();
    this.InitializeContacts();
    this.Subs.CVar<bool>(this._cfg, CVars.AutoClearForces, new Action<bool>(this.OnAutoClearChange), true);
    this.Subs.CVar<int>(this._cfg, CVars.NetTickrate, new Action<int>(this.UpdateSubsteps), true);
    this.Subs.CVar<int>(this._cfg, CVars.TargetMinimumTickrate, new Action<int>(this.UpdateSubsteps), true);
  }

  private void OnPhysicsShutdown(EntityUid uid, PhysicsComponent component, ComponentShutdown args)
  {
    this.SetCanCollide(uid, false, false, body: component);
    if (this.LifeStage(uid) > EntityLifeStage.MapInitialized)
      return;
    this.RemComp<FixturesComponent>(uid);
  }

  private void OnCollisionChange(ref CollisionChangeEvent ev)
  {
    if (this.Transform(ev.BodyUid).MapID == MapId.Nullspace || ev.CanCollide)
      return;
    this.DestroyContacts(ev.Body);
  }

  private void OnAutoClearChange(bool value) => this._autoClearForces = value;

  private void UpdateSubsteps(int obj)
  {
    this._substeps = (int) Math.Ceiling((double) this._cfg.GetCVar<int>(CVars.TargetMinimumTickrate) / (double) this._cfg.GetCVar<int>(CVars.NetTickrate));
  }

  internal void OnParentChange(
    Entity<TransformComponent, MetaDataComponent> ent,
    EntityUid oldParent,
    EntityUid? oldMap)
  {
    (EntityUid entityUid, TransformComponent transformComponent, MetaDataComponent comp2) = ent;
    if (transformComponent.ChildCount == 0 && comp2.EntityLifeStage < EntityLifeStage.Initialized)
      return;
    EntityUid? nullable;
    if (!oldMap.HasValue)
    {
      nullable = transformComponent.MapUid;
      if (!nullable.HasValue)
        return;
    }
    PhysicsComponent physicsComponent = this.PhysicsQuery.CompOrNull(entityUid);
    nullable = oldMap;
    EntityUid? mapUid = transformComponent.MapUid;
    if ((nullable.HasValue == mapUid.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != mapUid.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
    {
      this.HandleMapChange(entityUid, transformComponent, physicsComponent);
    }
    else
    {
      if (physicsComponent == null)
        return;
      this.HandleParentChangeVelocity(entityUid, physicsComponent, oldParent, transformComponent);
    }
  }

  private void HandleMapChange(EntityUid uid, TransformComponent xform, PhysicsComponent? body)
  {
    this.RecursiveMapUpdate(uid, xform, body);
  }

  private void RecursiveMapUpdate(EntityUid uid, TransformComponent xform, PhysicsComponent? body)
  {
    this._joints.ClearJoints(uid);
    foreach (EntityUid child in xform._children)
    {
      TransformComponent component1;
      if (this.XformQuery.TryGetComponent(child, out component1))
      {
        PhysicsComponent component2;
        this.PhysicsQuery.TryGetComponent(child, out component2);
        this.RecursiveMapUpdate(child, component1, component2);
      }
    }
  }

  private void OnGridAdd(GridAddEvent ev)
  {
    EntityUid entityUid = ev.EntityUid;
    if (this.HasComp<MapComponent>(entityUid))
      return;
    PhysicsComponent body = this.EnsureComp<PhysicsComponent>(entityUid);
    FixturesComponent manager = this.EnsureComp<FixturesComponent>(entityUid);
    this.SetCanCollide(entityUid, true, manager: manager, body: body);
    this.SetBodyType(entityUid, BodyType.Static, manager, body);
  }

  public override void Shutdown()
  {
    base.Shutdown();
    this.ShutdownContacts();
  }

  private void HandleContainerRemoved(
    EntityUid uid,
    PhysicsComponent physics,
    EntGotRemovedFromContainerMessage message)
  {
    CollideOnAnchorComponent component;
    if (this.MetaData(uid).EntityLifeStage >= EntityLifeStage.Terminating || this._anchorQuery.TryGetComponent(uid, out component) && component.Enable)
      return;
    this.WakeBody(uid, body: physics);
  }

  protected void SimulateWorld(float deltaTime, bool prediction)
  {
    float num = deltaTime / (float) this._substeps;
    this.EffectiveCurTime = new TimeSpan?(this._gameTiming.CurTime);
    for (int index = 0; index < this._substeps; ++index)
    {
      PhysicsUpdateBeforeSolveEvent message1 = new PhysicsUpdateBeforeSolveEvent(prediction, num);
      this.RaiseLocalEvent<PhysicsUpdateBeforeSolveEvent>(ref message1);
      this._broadphase.FindNewContacts();
      this.CollideContacts();
      this.Step(num, prediction);
      PhysicsUpdateAfterSolveEvent message2 = new PhysicsUpdateAfterSolveEvent(prediction, num);
      this.RaiseLocalEvent<PhysicsUpdateAfterSolveEvent>(ref message2);
      if (index == this._substeps - 1)
        this.FinalStep();
      this.EffectiveCurTime = new TimeSpan?(this.EffectiveCurTime.Value + TimeSpan.FromSeconds((double) num));
    }
    this.EffectiveCurTime = new TimeSpan?();
  }

  protected virtual void FinalStep()
  {
  }

  public void SetDensity(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    float value,
    bool update = true,
    FixturesComponent? manager = null)
  {
    if (fixture.Density.Equals(value) || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.Density = value;
    if (!update)
      return;
    this._fixtures.FixtureUpdate(uid, manager: manager);
  }

  public void SetFriction(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    float value,
    bool update = true,
    FixturesComponent? manager = null)
  {
    if (fixture.Friction.Equals(value) || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.Friction = value;
    if (!update)
      return;
    this._fixtures.FixtureUpdate(uid, manager: manager);
  }

  public void SetHard(EntityUid uid, Fixture fixture, bool value, FixturesComponent? manager = null)
  {
    if (fixture.Hard.Equals(value) || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.Hard = value;
    this._fixtures.FixtureUpdate(uid, manager: manager);
    this.WakeBody(uid);
  }

  public void SetRestitution(
    EntityUid uid,
    Fixture fixture,
    float value,
    bool update = true,
    FixturesComponent? manager = null)
  {
    if (fixture.Restitution.Equals(value) || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.Restitution = value;
    if (!update)
      return;
    this._fixtures.FixtureUpdate(uid, manager: manager);
  }

  public void ScaleFixtures(Entity<FixturesComponent?> ent, float factor)
  {
    if (!this.Resolve<FixturesComponent>((EntityUid) ent, ref ent.Comp))
      return;
    foreach ((string str, Fixture fixture) in ent.Comp.Fixtures)
    {
      switch (fixture.Shape)
      {
        case EdgeShape edge:
          this.SetVertices((EntityUid) ent, str, fixture, edge, edge.Vertex0 * factor, edge.Vertex1 * factor, edge.Vertex2 * factor, edge.Vertex3 * factor, ent.Comp);
          continue;
        case PhysShapeCircle shape:
          this.SetPositionRadius((EntityUid) ent, str, fixture, shape, shape.Position * factor, shape.Radius * factor, ent.Comp);
          continue;
        case PolygonShape poly:
          Vector2[] vertices = poly.Vertices;
          for (int index = 0; index < poly.VertexCount; ++index)
            vertices[index] *= factor;
          this.SetVertices((EntityUid) ent, str, fixture, poly, vertices, ent.Comp);
          continue;
        default:
          throw new NotImplementedException();
      }
    }
  }

  public bool IsCurrentlyHardCollidable(
    Entity<FixturesComponent?, PhysicsComponent?> bodyA,
    Entity<FixturesComponent?, PhysicsComponent?> bodyB)
  {
    return this._fixturesQuery.Resolve((EntityUid) bodyA, ref bodyA.Comp1, false) && this._fixturesQuery.Resolve((EntityUid) bodyB, ref bodyB.Comp1, false) && this.PhysicsQuery.Resolve((EntityUid) bodyA, ref bodyA.Comp2, false) && this.PhysicsQuery.Resolve((EntityUid) bodyB, ref bodyB.Comp2, false) && bodyA.Comp2.CanCollide && bodyB.Comp2.CanCollide && this.IsHardCollidable(bodyA, bodyB);
  }

  public bool IsHardCollidable(
    Entity<FixturesComponent?, PhysicsComponent?> bodyA,
    Entity<FixturesComponent?, PhysicsComponent?> bodyB)
  {
    if (!this._fixturesQuery.Resolve((EntityUid) bodyA, ref bodyA.Comp1, false) || !this._fixturesQuery.Resolve((EntityUid) bodyB, ref bodyB.Comp1, false) || !this.PhysicsQuery.Resolve((EntityUid) bodyA, ref bodyA.Comp2, false) || !this.PhysicsQuery.Resolve((EntityUid) bodyB, ref bodyB.Comp2, false) || !bodyA.Comp2.Hard || !bodyB.Comp2.Hard || (bodyA.Comp2.CollisionLayer & bodyB.Comp2.CollisionMask) == 0 && (bodyA.Comp2.CollisionMask & bodyB.Comp2.CollisionLayer) == 0)
      return false;
    foreach (Fixture fixture1 in bodyA.Comp1.Fixtures.Values)
    {
      if (fixture1.Hard)
      {
        foreach (Fixture fixture2 in bodyB.Comp1.Fixtures.Values)
        {
          if (fixture2.Hard && ((fixture1.CollisionLayer & fixture2.CollisionMask) != 0 || (fixture1.CollisionMask & fixture2.CollisionLayer) != 0))
            return true;
        }
      }
    }
    return false;
  }

  public void AddCollisionMask(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    int mask,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if ((fixture.CollisionMask & mask) == mask || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.CollisionMask |= mask;
    if (body != null || this.TryComp<PhysicsComponent>(uid, out body))
      this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
    this._broadphase.Refilter(uid, fixture);
  }

  public void SetCollisionMask(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    int mask,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (fixture.CollisionMask == mask || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.CollisionMask = mask;
    if (body != null || this.TryComp<PhysicsComponent>(uid, out body))
      this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
    this._broadphase.Refilter(uid, fixture);
  }

  public void RemoveCollisionMask(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    int mask,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if ((fixture.CollisionMask & mask) == 0 || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.CollisionMask &= ~mask;
    if (body != null || this.TryComp<PhysicsComponent>(uid, out body))
      this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
    this._broadphase.Refilter(uid, fixture);
  }

  public void AddCollisionLayer(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    int layer,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if ((fixture.CollisionLayer & layer) == layer || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.CollisionLayer |= layer;
    if (body != null || this.TryComp<PhysicsComponent>(uid, out body))
      this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
    this._broadphase.Refilter(uid, fixture);
  }

  public void SetCollisionLayer(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    int layer,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (fixture.CollisionLayer.Equals(layer) || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.CollisionLayer = layer;
    if (body != null || this.TryComp<PhysicsComponent>(uid, out body))
      this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
    this._broadphase.Refilter(uid, fixture);
  }

  public void RemoveCollisionLayer(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    int layer,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if ((fixture.CollisionLayer & layer) == 0 || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    fixture.CollisionLayer &= ~layer;
    if (body != null || this.TryComp<PhysicsComponent>(uid, out body))
      this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
    this._broadphase.Refilter(uid, fixture);
  }

  private void InitializeIsland()
  {
    this.Subs.CVar<int>(this._cfg, CVars.NetTickrate, new Action<int>(this.SetTickRate), true);
    this.Subs.CVar<bool>(this._cfg, CVars.WarmStarting, new Action<bool>(this.SetWarmStarting), true);
    this.Subs.CVar<float>(this._cfg, CVars.MaxLinearCorrection, new Action<float>(this.SetMaxLinearCorrection), true);
    this.Subs.CVar<float>(this._cfg, CVars.MaxAngularCorrection, new Action<float>(this.SetMaxAngularCorrection), true);
    this.Subs.CVar<int>(this._cfg, CVars.VelocityIterations, new Action<int>(this.SetVelocityIterations), true);
    this.Subs.CVar<int>(this._cfg, CVars.PositionIterations, new Action<int>(this.SetPositionIterations), true);
    this.Subs.CVar<float>(this._cfg, CVars.MaxLinVelocity, new Action<float>(this.SetMaxLinearVelocity), true);
    this.Subs.CVar<float>(this._cfg, CVars.MaxAngVelocity, new Action<float>(this.SetMaxAngularVelocity), true);
    this.Subs.CVar<bool>(this._cfg, CVars.SleepAllowed, new Action<bool>(this.SetSleepAllowed), true);
    this.Subs.CVar<float>(this._cfg, CVars.AngularSleepTolerance, new Action<float>(this.SetAngularToleranceSqr), true);
    this.Subs.CVar<float>(this._cfg, CVars.LinearSleepTolerance, new Action<float>(this.SetLinearToleranceSqr), true);
    this.Subs.CVar<float>(this._cfg, CVars.TimeToSleep, new Action<float>(this.SetTimeToSleep), true);
    this.Subs.CVar<float>(this._cfg, CVars.VelocityThreshold, new Action<float>(this.SetVelocityThreshold), true);
    this.Subs.CVar<float>(this._cfg, CVars.Baumgarte, new Action<float>(this.SetBaumgarte), true);
  }

  private void SetWarmStarting(bool value) => this._warmStarting = value;

  private void SetMaxLinearCorrection(float value) => this._maxLinearCorrection = value;

  private void SetMaxAngularCorrection(float value) => this._maxAngularCorrection = value;

  private void SetVelocityIterations(int value) => this._velocityIterations = value;

  private void SetPositionIterations(int value) => this._positionIterations = value;

  private void SetMaxLinearVelocity(float value)
  {
    this._maxLinearVelocity = value;
    this.UpdateMaxTranslation();
  }

  private void SetMaxAngularVelocity(float value)
  {
    this._maxAngularVelocity = value;
    this.UpdateMaxRotation();
  }

  private void SetTickRate(int value)
  {
    this._tickRate = value;
    this.UpdateMaxTranslation();
    this.UpdateMaxRotation();
  }

  private void SetSleepAllowed(bool value) => this._sleepAllowed = value;

  private void SetAngularToleranceSqr(float value) => this.AngularToleranceSqr = value;

  private void SetLinearToleranceSqr(float value) => this.LinearToleranceSqr = value;

  private void SetTimeToSleep(float value) => this.TimeToSleep = value;

  private void SetVelocityThreshold(float value) => this._velocityThreshold = value;

  private void SetBaumgarte(float value) => this._baumgarte = value;

  private void UpdateMaxTranslation()
  {
    this._maxTranslationPerTick = this._maxLinearVelocity / (float) this._tickRate;
  }

  private void UpdateMaxRotation()
  {
    this._maxRotationPerTick = 6.28318548f * this._maxAngularVelocity / (float) this._tickRate;
  }

  public void Step(float frameTime, bool prediction)
  {
    float invDt = (double) frameTime > 0.0 ? 1f / frameTime : 0.0f;
    float dtRatio = this._invDt0 * frameTime;
    this.Solve(frameTime, dtRatio, invDt, prediction);
    if (this._autoClearForces)
      this.ClearForces();
    this._invDt0 = invDt;
  }

  private void ClearForces()
  {
    foreach (Entity<PhysicsComponent, TransformComponent> awakeBody in this.AwakeBodies)
    {
      EntityUid owner = awakeBody.Owner;
      PhysicsComponent comp1 = awakeBody.Comp1;
      if (comp1.Force != Vector2.Zero)
      {
        comp1.Force = Vector2.Zero;
        this.DirtyField<PhysicsComponent>(owner, comp1, "Force");
      }
      if ((double) comp1.Torque != 0.0)
      {
        comp1.Torque = 0.0f;
        this.DirtyField<PhysicsComponent>(owner, comp1, "Torque");
      }
    }
  }

  private void Solve(float frameTime, float dtRatio, float invDt, bool prediction)
  {
    this._bodyStack.EnsureCapacity(this.AwakeBodies.Count);
    this._islandSet.EnsureCapacity(this.AwakeBodies.Count);
    this._awakeBodyList.AddRange((IEnumerable<Entity<PhysicsComponent, TransformComponent>>) this.AwakeBodies);
    int num1 = 0;
    SharedPhysicsSystem.IslandData island1;
    ref SharedPhysicsSystem.IslandData local = ref island1;
    int Index = num1;
    int num2 = Index + 1;
    List<Entity<PhysicsComponent, TransformComponent>> Bodies1 = this._islandBodyPool.Get();
    List<Contact> Contacts1 = this._islandContactPool.Get();
    List<(Joint, Joint)> Joints1 = this._islandJointPool.Get();
    List<(Joint, float)> BrokenJoints = new List<(Joint, float)>();
    local = new SharedPhysicsSystem.IslandData(Index, true, Bodies1, Contacts1, Joints1, BrokenJoints);
    List<SharedPhysicsSystem.IslandData> islands = new List<SharedPhysicsSystem.IslandData>();
    List<(Joint, Joint)> valueTupleList = new List<(Joint, Joint)>();
    foreach (Entity<PhysicsComponent, TransformComponent> awakeBody in this._awakeBodyList)
    {
      TransformComponent comp2_1 = awakeBody.Comp2;
      PhysicsComponent comp1_1 = awakeBody.Comp1;
      if (!comp1_1.Island)
      {
        EntityUid owner1 = comp1_1.Owner;
        EntityUid? mapUid = comp2_1.MapUid;
        if (mapUid.HasValue)
        {
          MetaDataComponent component1;
          if (!this.EntityManager.MetaQuery.TryGetComponent(owner1, out component1))
          {
            this.Log.Error($"Found deleted entity {this.ToPrettyString((Entity<MetaDataComponent>) owner1)} on map!");
            this.RemoveSleepBody(awakeBody);
          }
          else if ((!component1.EntityPaused || comp1_1.IgnorePaused) && (!prediction || comp1_1.Predict) && comp1_1.CanCollide && comp1_1.BodyType != BodyType.Static)
          {
            List<Entity<PhysicsComponent, TransformComponent>> Bodies2 = this._islandBodyPool.Get();
            List<Contact> Contacts2 = this._islandContactPool.Get();
            List<(Joint, Joint)> Joints2 = this._islandJointPool.Get();
            this._bodyStack.Push(awakeBody);
            comp1_1.Island = true;
            Entity<PhysicsComponent, TransformComponent> result;
            while (this._bodyStack.TryPop(out result))
            {
              EntityUid owner2 = result.Owner;
              PhysicsComponent comp1_2 = result.Comp1;
              Bodies2.Add(result);
              this._islandSet.Add(result);
              if (comp1_2.BodyType != BodyType.Static)
              {
                this.SetAwake(owner2, comp1_2, true, false);
                LinkedListNode<Contact> linkedListNode = comp1_2.Contacts.First;
                while (linkedListNode != null)
                {
                  Contact contact = linkedListNode.Value;
                  linkedListNode = linkedListNode.Next;
                  if ((contact.Flags & (ContactFlags.PreInit | ContactFlags.Island)) == ContactFlags.None && contact.Enabled && contact.IsTouching)
                  {
                    Fixture fixtureA = contact.FixtureA;
                    if ((fixtureA != null ? (!fixtureA.Hard ? 1 : 0) : 1) == 0)
                    {
                      Fixture fixtureB = contact.FixtureB;
                      if ((fixtureB != null ? (!fixtureB.Hard ? 1 : 0) : 1) == 0)
                      {
                        Contacts2.Add(contact);
                        contact.Flags |= ContactFlags.Island;
                        PhysicsComponent comp1_3 = contact.OtherBody(owner2);
                        if (!comp1_3.Island)
                        {
                          EntityUid owner3 = contact.OtherEnt(owner2);
                          TransformComponent comp2_2 = contact.OtherTransform(owner2);
                          this._bodyStack.Push(new Entity<PhysicsComponent, TransformComponent>(owner3, comp1_3, comp2_2));
                          comp1_3.Island = true;
                        }
                      }
                    }
                  }
                }
                JointRelayTargetComponent component2;
                if (this.RelayTargetQuery.TryGetComponent(owner2, out component2))
                {
                  foreach (EntityUid uid in component2.Relayed)
                  {
                    JointComponent component3;
                    if (this.JointQuery.TryGetComponent(uid, out component3))
                    {
                      foreach (Joint joint1 in component3.GetJoints.Values)
                      {
                        if (!joint1.IslandFlag)
                        {
                          EntityUid bodyAuid = joint1.BodyAUid;
                          EntityUid bodyBuid = joint1.BodyBUid;
                          JointComponent component4;
                          if (this.JointQuery.TryGetComponent(bodyAuid, out component4) && component4.Relay.HasValue)
                            bodyAuid = component4.Relay.Value;
                          JointComponent component5;
                          if (this.JointQuery.TryGetComponent(bodyBuid, out component5) && component5.Relay.HasValue)
                            bodyBuid = component5.Relay.Value;
                          Joint joint2 = joint1.Clone(bodyAuid, bodyBuid);
                          valueTupleList.Add((joint1, joint2));
                          joint1.IslandFlag = true;
                        }
                      }
                    }
                  }
                }
                JointComponent component6;
                if (this.JointQuery.TryGetComponent(owner2, out component6) && !component6.Relay.HasValue)
                {
                  foreach (Joint joint3 in component6.Joints.Values)
                  {
                    if (!joint3.IslandFlag)
                    {
                      EntityUid bodyAuid = joint3.BodyAUid;
                      EntityUid bodyBuid = joint3.BodyBUid;
                      JointComponent component7;
                      if (this.JointQuery.TryGetComponent(bodyAuid, out component7) && component7.Relay.HasValue)
                        bodyAuid = component7.Relay.Value;
                      JointComponent component8;
                      if (this.JointQuery.TryGetComponent(bodyBuid, out component8) && component8.Relay.HasValue)
                        bodyBuid = component8.Relay.Value;
                      Joint joint4 = joint3.Clone(bodyAuid, bodyBuid);
                      valueTupleList.Add((joint3, joint4));
                      joint3.IslandFlag = true;
                    }
                  }
                }
                foreach ((Joint joint5, Joint joint6) in valueTupleList)
                {
                  PhysicsComponent component9 = this.PhysicsQuery.GetComponent(joint6.BodyAUid);
                  PhysicsComponent component10 = this.PhysicsQuery.GetComponent(joint6.BodyBUid);
                  if (component9.CanCollide && component10.CanCollide)
                  {
                    Joints2.Add((joint5, joint6));
                    if (!component9.Island)
                    {
                      this._bodyStack.Push(new Entity<PhysicsComponent, TransformComponent>(joint6.BodyAUid, component9, this.XformQuery.GetComponent(joint6.BodyAUid)));
                      component9.Island = true;
                    }
                    if (!component10.Island)
                    {
                      this._bodyStack.Push(new Entity<PhysicsComponent, TransformComponent>(joint6.BodyBUid, component10, this.XformQuery.GetComponent(joint6.BodyBUid)));
                      component10.Island = true;
                    }
                  }
                }
                valueTupleList.Clear();
              }
            }
            int index1;
            if (Contacts2.Count == 0 && Joints2.Count == 0)
            {
              island1.MapUid = mapUid.Value;
              island1.Bodies.Add(Bodies2[0]);
              index1 = island1.Index;
            }
            else
            {
              SharedPhysicsSystem.IslandData islandData = new SharedPhysicsSystem.IslandData(num2++, false, Bodies2, Contacts2, Joints2, new List<(Joint, float)>())
              {
                MapUid = mapUid.Value
              };
              islands.Add(islandData);
              index1 = islandData.Index;
            }
            for (int index2 = 0; index2 < Bodies2.Count; ++index2)
            {
              PhysicsComponent comp1_4 = Bodies2[index2].Comp1;
              if (comp1_4.BodyType == BodyType.Static)
                comp1_4.Island = false;
              comp1_4.IslandIndex[index1] = index2;
            }
          }
        }
      }
    }
    if (island1.Bodies.Count > 0)
      islands.Add(island1);
    else
      this.ReturnIsland(in island1);
    this.SolveIslands(islands, frameTime, dtRatio, invDt, prediction);
    foreach (SharedPhysicsSystem.IslandData island2 in islands)
      this.ReturnIsland(in island2);
    this.Cleanup(frameTime);
  }

  private void ReturnIsland(in SharedPhysicsSystem.IslandData island)
  {
    foreach (Entity<PhysicsComponent, TransformComponent> body in island.Bodies)
      body.Comp1.IslandIndex.Remove(island.Index);
    this._islandBodyPool.Return(island.Bodies);
    this._islandContactPool.Return(island.Contacts);
    foreach ((Joint joint, Joint Joint) in island.Joints)
    {
      if (joint != Joint)
        Joint.CopyTo(joint);
      joint.IslandFlag = false;
    }
    this._islandJointPool.Return(island.Joints);
    island.BrokenJoints.Clear();
  }

  protected virtual void Cleanup(float frameTime)
  {
    foreach (Entity<PhysicsComponent, TransformComponent> island in this._islandSet)
    {
      PhysicsComponent comp1 = island.Comp1;
      if (comp1.Island && !comp1.Deleted)
        comp1.Island = false;
    }
    this._islandSet.Clear();
    this._islandSet.Clear();
    this._awakeBodyList.Clear();
  }

  private void SolveIslands(
    List<SharedPhysicsSystem.IslandData> islands,
    float frameTime,
    float dtRatio,
    float invDt,
    bool prediction)
  {
    int fromInclusive = 0;
    SolverData data = new SolverData(frameTime, dtRatio, invDt, this._warmStarting, this._maxLinearCorrection, this._maxAngularCorrection, this._velocityIterations, this._positionIterations, this._maxLinearVelocity, this._maxAngularVelocity, this._maxTranslationPerTick, this._maxRotationPerTick, this._sleepAllowed, this.AngularToleranceSqr, this.LinearToleranceSqr, this.TimeToSleep, this._velocityThreshold, this._baumgarte);
    islands.Sort((Comparison<SharedPhysicsSystem.IslandData>) ((x, y) => SharedPhysicsSystem.InternalParallel(y).CompareTo(SharedPhysicsSystem.InternalParallel(x))));
    int minimumLength = 0;
    SharedPhysicsSystem.IslandData[] actualIslands = islands.ToArray();
    for (int index = 0; index < islands.Count; ++index)
    {
      ref SharedPhysicsSystem.IslandData local = ref actualIslands[index];
      local.Offset = minimumLength;
      this.UpdateLerpData(local.Bodies);
      minimumLength += local.Bodies.Count;
    }
    Vector2[] solvedPositions = ArrayPool<Vector2>.Shared.Rent(minimumLength);
    float[] solvedAngles = ArrayPool<float>.Shared.Rent(minimumLength);
    Vector2[] linearVelocities = ArrayPool<Vector2>.Shared.Rent(minimumLength);
    float[] angularVelocities = ArrayPool<float>.Shared.Rent(minimumLength);
    bool[] sleepStatus = ArrayPool<bool>.Shared.Rent(minimumLength);
    for (int index = 0; index < minimumLength; ++index)
      sleepStatus[index] = false;
    ParallelOptions parallelOptions = new ParallelOptions()
    {
      MaxDegreeOfParallelism = this._parallel.ParallelProcessCount
    };
    for (; fromInclusive < actualIslands.Length; ++fromInclusive)
    {
      ref SharedPhysicsSystem.IslandData local = ref actualIslands[fromInclusive];
      if (SharedPhysicsSystem.InternalParallel(local))
        this.SolveIsland(ref local, in data, parallelOptions, prediction, solvedPositions, solvedAngles, linearVelocities, angularVelocities, sleepStatus);
      else
        break;
    }
    Parallel.For(fromInclusive, actualIslands.Length, parallelOptions, (Action<int>) (i => this.SolveIsland(ref actualIslands[i], in data, (ParallelOptions) null, prediction, solvedPositions, solvedAngles, linearVelocities, angularVelocities, sleepStatus)));
    for (int index = 0; index < actualIslands.Length; ++index)
    {
      ref SharedPhysicsSystem.IslandData local = ref actualIslands[index];
      this.UpdateBodies(in local, solvedPositions, solvedAngles, linearVelocities, angularVelocities);
      this.SleepBodies(in local, sleepStatus);
    }
    ArrayPool<Vector2>.Shared.Return(solvedPositions);
    ArrayPool<float>.Shared.Return(solvedAngles);
    ArrayPool<Vector2>.Shared.Return(linearVelocities);
    ArrayPool<float>.Shared.Return(angularVelocities);
    ArrayPool<bool>.Shared.Return(sleepStatus);
  }

  protected virtual void UpdateLerpData(
    List<Entity<PhysicsComponent, TransformComponent>> bodies)
  {
  }

  private static bool InternalParallel(SharedPhysicsSystem.IslandData island)
  {
    return island.Bodies.Count > 128 /*0x80*/ || island.Contacts.Count > 128 /*0x80*/ || island.Joints.Count > 128 /*0x80*/;
  }

  private void SolveIsland(
    ref SharedPhysicsSystem.IslandData island,
    in SolverData data,
    ParallelOptions? options,
    bool prediction,
    Vector2[] solvedPositions,
    float[] solvedAngles,
    Vector2[] linearVelocities,
    float[] angularVelocities,
    bool[] sleepStatus)
  {
    int count1 = island.Bodies.Count;
    Vector2[] vector2Array = ArrayPool<Vector2>.Shared.Rent(count1);
    float[] numArray = ArrayPool<float>.Shared.Rent(count1);
    int offset = island.Offset;
    Vector2 gravity = this.Gravity;
    for (int index = 0; index < island.Bodies.Count; ++index)
    {
      Entity<PhysicsComponent, TransformComponent> body = island.Bodies[index];
      PhysicsComponent comp1 = body.Comp1;
      (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(body.Comp2);
      Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
      Vector2 vector2_1 = Robust.Shared.Physics.Transform.Mul(in transform, comp1.LocalCenter);
      float angle = transform.Quaternion2D.Angle;
      Vector2 vector2_2 = comp1.LinearVelocity;
      float num1 = comp1.AngularVelocity;
      if (comp1.BodyType == BodyType.Dynamic)
      {
        Vector2 vector2_3 = !comp1.IgnoreGravity ? vector2_2 + (gravity + comp1.Force * comp1.InvMass) * data.FrameTime : vector2_2 + comp1.Force * data.FrameTime * comp1.InvMass;
        float num2 = num1 + comp1.InvI * comp1.Torque * data.FrameTime;
        vector2_2 = vector2_3 * Math.Clamp((float) (1.0 - (double) data.FrameTime * (double) comp1.LinearDamping), 0.0f, 1f);
        num1 = num2 * Math.Clamp((float) (1.0 - (double) data.FrameTime * (double) comp1.AngularDamping), 0.0f, 1f);
      }
      vector2Array[index] = vector2_1;
      numArray[index] = angle;
      linearVelocities[index + offset] = vector2_2;
      angularVelocities[index + offset] = num1;
    }
    int count2 = island.Contacts.Count;
    ContactVelocityConstraint[] velocityConstraintArray = ArrayPool<ContactVelocityConstraint>.Shared.Rent(count2);
    ContactPositionConstraint[] positionConstraintArray = ArrayPool<ContactPositionConstraint>.Shared.Rent(count2);
    this.ResetSolver(in data, in island, velocityConstraintArray, positionConstraintArray);
    this.InitializeVelocityConstraints(in data, in island, velocityConstraintArray, positionConstraintArray, vector2Array, numArray, linearVelocities, angularVelocities);
    if (data.WarmStarting)
      this.WarmStart(in data, in island, velocityConstraintArray, linearVelocities, angularVelocities);
    int count3 = island.Joints.Count;
    if (count3 > 0)
    {
      for (int index = 0; index < island.Joints.Count; ++index)
      {
        Joint joint = island.Joints[index].Joint;
        if (joint.Enabled)
        {
          PhysicsComponent component1 = this.PhysicsQuery.GetComponent(joint.BodyAUid);
          PhysicsComponent component2 = this.PhysicsQuery.GetComponent(joint.BodyBUid);
          joint.InitVelocityConstraints(in data, in island, component1, component2, vector2Array, numArray, linearVelocities, angularVelocities);
        }
      }
    }
    for (int index1 = 0; index1 < data.VelocityIterations; ++index1)
    {
      for (int index2 = 0; index2 < count3; ++index2)
      {
        Joint joint = island.Joints[index2].Joint;
        if (joint.Enabled)
        {
          joint.SolveVelocityConstraints(in data, in island, linearVelocities, angularVelocities);
          float num = joint.Validate(data.InvDt);
          if ((double) num > 0.0)
            island.BrokenJoints.Add((island.Joints[index2].Original, num));
        }
      }
      SharedPhysicsSystem.SolveVelocityConstraints(in island, options, velocityConstraintArray, linearVelocities, angularVelocities);
    }
    this.StoreImpulses(in island, velocityConstraintArray);
    float num3 = data.MaxTranslation / data.FrameTime;
    float num4 = num3 * num3;
    float num5 = data.MaxRotation / data.FrameTime;
    float num6 = num5 * num5;
    for (int index = 0; index < count1; ++index)
    {
      Vector2 linearVelocity = linearVelocities[offset + index];
      float angularVelocity = angularVelocities[offset + index];
      float x = linearVelocity.LengthSquared();
      if ((double) x > (double) num4)
      {
        linearVelocity *= num3 / MathF.Sqrt(x);
        linearVelocities[offset + index] = linearVelocity;
      }
      if ((double) angularVelocity * (double) angularVelocity > (double) num6)
      {
        angularVelocity *= num5 / MathF.Abs(angularVelocity);
        angularVelocities[offset + index] = angularVelocity;
      }
      vector2Array[index] += linearVelocity * data.FrameTime;
      numArray[index] += angularVelocity * data.FrameTime;
    }
    island.PositionSolved = false;
    for (int index3 = 0; index3 < data.PositionIterations; ++index3)
    {
      bool flag1 = SharedPhysicsSystem.SolvePositionConstraints(in data, in island, options, positionConstraintArray, vector2Array, numArray);
      bool flag2 = true;
      for (int index4 = 0; index4 < island.Joints.Count; ++index4)
      {
        Joint joint = island.Joints[index4].Joint;
        if (joint.Enabled)
        {
          bool flag3 = joint.SolvePositionConstraints(in data, vector2Array, numArray);
          flag2 &= flag3;
        }
      }
      if (flag1 & flag2)
      {
        island.PositionSolved = true;
        break;
      }
    }
    List<Entity<PhysicsComponent, TransformComponent>> bodies = island.Bodies;
    if (options != null)
      ProcessParallelInternal(this, options, count1, offset, bodies, vector2Array, numArray, solvedPositions, solvedAngles);
    else
      this.FinalisePositions(0, count1, offset, bodies, vector2Array, numArray, solvedPositions, solvedAngles);
    if (island.LoneIsland)
    {
      if (!prediction && data.SleepAllowed)
      {
        for (int index = 0; index < count1; ++index)
        {
          PhysicsComponent comp1 = island.Bodies[index].Comp1;
          if (comp1.BodyType != BodyType.Static)
          {
            if (!comp1.SleepingAllowed || (double) comp1.AngularVelocity * (double) comp1.AngularVelocity > (double) data.AngTolSqr || (double) Vector2.Dot(comp1.LinearVelocity, comp1.LinearVelocity) > (double) data.LinTolSqr)
              this.SetSleepTime(comp1, 0.0f);
            else
              this.SetSleepTime(comp1, comp1.SleepTime + data.FrameTime);
            if ((double) comp1.SleepTime >= (double) data.TimeToSleep && island.PositionSolved)
              sleepStatus[offset + index] = true;
          }
        }
      }
    }
    else if (!prediction && data.SleepAllowed)
    {
      float x = float.MaxValue;
      for (int index = 0; index < count1; ++index)
      {
        PhysicsComponent comp1 = island.Bodies[index].Comp1;
        if (comp1.BodyType != BodyType.Static)
        {
          if (!comp1.SleepingAllowed || (double) comp1.AngularVelocity * (double) comp1.AngularVelocity > (double) data.AngTolSqr || (double) Vector2.Dot(comp1.LinearVelocity, comp1.LinearVelocity) > (double) data.LinTolSqr)
          {
            this.SetSleepTime(comp1, 0.0f);
            x = 0.0f;
          }
          else
          {
            this.SetSleepTime(comp1, comp1.SleepTime + data.FrameTime);
            x = MathF.Min(x, comp1.SleepTime);
          }
        }
      }
      if ((double) x >= (double) data.TimeToSleep && island.PositionSolved)
      {
        for (int index = 0; index < island.Bodies.Count; ++index)
          sleepStatus[offset + index] = true;
      }
    }
    ArrayPool<Vector2>.Shared.Return(vector2Array);
    ArrayPool<float>.Shared.Return(numArray);
    ArrayPool<ContactVelocityConstraint>.Shared.Return(velocityConstraintArray);
    ArrayPool<ContactPositionConstraint>.Shared.Return(positionConstraintArray);

    static void ProcessParallelInternal(
      SharedPhysicsSystem system,
      ParallelOptions options,
      int bodyCount,
      int offset,
      List<Entity<PhysicsComponent, TransformComponent>> bodies,
      Vector2[] positions,
      float[] angles,
      Vector2[] solvedPositions,
      float[] solvedAngles)
    {
      Parallel.For(0, (int) MathF.Ceiling((float) bodyCount / 32f), options, (Action<int>) (i =>
      {
        int start = i * 32 /*0x20*/;
        int end = Math.Min(bodyCount, start + 32 /*0x20*/);
        system.FinalisePositions(start, end, offset, bodies, positions, angles, solvedPositions, solvedAngles);
      }));
    }
  }

  private void FinalisePositions(
    int start,
    int end,
    int offset,
    List<Entity<PhysicsComponent, TransformComponent>> bodies,
    Vector2[] positions,
    float[] angles,
    Vector2[] solvedPositions,
    float[] solvedAngles)
  {
    for (int index = start; index < end; ++index)
    {
      Entity<PhysicsComponent, TransformComponent> body = bodies[index];
      PhysicsComponent comp1 = body.Comp1;
      if (comp1.BodyType != BodyType.Static)
      {
        TransformComponent comp2 = body.Comp2;
        TransformComponent comp;
        if (this.TryComp(comp2.ParentUid, out comp))
        {
          (Vector2 _, Angle WorldRotation, Matrix3x2 matrix3x2) = this._transform.GetWorldPositionRotationInvMatrix(comp);
          float num = (float) Angle.op_Implicit(Angle.op_Addition(WorldRotation, comp2._localRotation));
          float angle = angles[index];
          Quaternion2D quaternion2D = new Quaternion2D(angle);
          Vector2 vector2 = Vector2.Transform(positions[index] - Robust.Shared.Physics.Transform.Mul(in quaternion2D, comp1.LocalCenter), matrix3x2);
          solvedPositions[offset + index] = vector2 - comp2.LocalPosition;
          solvedAngles[offset + index] = angle - num;
        }
      }
    }
  }

  private void UpdateBodies(
    in SharedPhysicsSystem.IslandData island,
    Vector2[] positions,
    float[] angles,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    foreach ((Joint Joint, float num) in island.BrokenJoints)
    {
      JointBreakEvent jointBreakEvent = new JointBreakEvent(Joint, MathF.Sqrt(num));
      this.RaiseLocalEvent<JointBreakEvent>(Joint.BodyAUid, ref jointBreakEvent);
      this.RaiseLocalEvent<JointBreakEvent>(Joint.BodyBUid, ref jointBreakEvent);
      this.RaiseLocalEvent<JointBreakEvent>(ref jointBreakEvent);
      Joint.Dirty();
    }
    int offset = island.Offset;
    for (int index = 0; index < island.Bodies.Count; ++index)
    {
      Entity<PhysicsComponent, TransformComponent> body = island.Bodies[index];
      PhysicsComponent comp1 = body.Comp1;
      if (comp1.BodyType != BodyType.Static)
      {
        EntityUid owner = body.Owner;
        Vector2 position = positions[offset + index];
        float angle = angles[offset + index];
        TransformComponent comp2 = body.Comp2;
        Vector2 linearVelocity = linearVelocities[offset + index];
        bool flag = false;
        if (!float.IsNaN(linearVelocity.X) && !float.IsNaN(linearVelocity.Y))
          flag |= this.SetLinearVelocity(owner, linearVelocity, false, body: comp1);
        float angularVelocity = angularVelocities[offset + index];
        if (!float.IsNaN(angularVelocity))
          flag |= this.SetAngularVelocity(owner, angularVelocity, false, body: comp1);
        if (!float.IsNaN(position.X) && !float.IsNaN(position.Y))
          this._transform.SetLocalPositionRotation(owner, comp2.LocalPosition + position, Angle.op_Addition(comp2.LocalRotation, Angle.op_Implicit(angle)), comp2);
        if (flag)
          this.Dirty(owner, (IComponent) comp1);
      }
    }
  }

  private void SleepBodies(in SharedPhysicsSystem.IslandData island, bool[] sleepStatus)
  {
    int offset = island.Offset;
    for (int index = 0; index < island.Bodies.Count; ++index)
    {
      if (sleepStatus[offset + index])
      {
        Entity<PhysicsComponent, TransformComponent> body = island.Bodies[index];
        this.SetAwake(body.Owner, (PhysicsComponent) body, false);
      }
    }
  }

  internal void AddAwakeBody(Entity<PhysicsComponent, TransformComponent> ent)
  {
    PhysicsComponent comp1 = ent.Comp1;
    if (!comp1.CanCollide)
      this.Log.Error($"Tried to add non-colliding {this.ToPrettyString(new EntityUid?((EntityUid) ent))} as an awake body to map!");
    else if (comp1.BodyType == BodyType.Static)
      this.Log.Error($"Tried to add static body {this.ToPrettyString(new EntityUid?((EntityUid) ent))} as an awake body to map!");
    else
      this.AwakeBodies.Add(ent);
  }

  internal void RemoveSleepBody(Entity<PhysicsComponent, TransformComponent> ent)
  {
    this.AwakeBodies.Remove(ent);
  }

  public bool TryCollideRect(Box2 collider, MapId mapId, bool approximate = true)
  {
    (Box2, MapId, bool) state1 = (collider, mapId, false);
    this._broadphase.GetBroadphases(mapId, collider, (SharedBroadphaseSystem.BroadphaseCallback) (broadphase =>
    {
      Box2 gridCollider = Matrix3Helpers.TransformBox(this._transform.GetInvWorldMatrix((EntityUid) broadphase), ref collider);
      broadphase.Comp.StaticTree.QueryAabb<(Box2, MapId, bool)>(ref state1, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<(Box2, MapId, bool)>) ((ref (Box2 collider, MapId map, bool found) state, in FixtureProxy proxy) =>
      {
        if (proxy.Fixture.CollisionLayer == 0 || !((Box2) ref proxy.AABB).Intersects(ref gridCollider))
          return true;
        state.Item3 = true;
        return false;
      }), gridCollider, approximate);
      broadphase.Comp.DynamicTree.QueryAabb<(Box2, MapId, bool)>(ref state1, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<(Box2, MapId, bool)>) ((ref (Box2 collider, MapId map, bool found) state, in FixtureProxy proxy) =>
      {
        if (proxy.Fixture.CollisionLayer == 0 || !((Box2) ref proxy.AABB).Intersects(ref gridCollider))
          return true;
        state.Item3 = true;
        return false;
      }), gridCollider, approximate);
    }));
    return state1.Item3;
  }

  public HashSet<EntityUid> GetEntitiesIntersectingBody(
    EntityUid uid,
    int collisionMask,
    bool approximate = true,
    PhysicsComponent? body = null,
    FixturesComponent? fixtureComp = null,
    TransformComponent? xform = null)
  {
    HashSet<EntityUid> intersectingBody = new HashSet<EntityUid>();
    BroadphaseComponent broadphase;
    if (!this.Resolve<PhysicsComponent, FixturesComponent, TransformComponent>(uid, ref body, ref fixtureComp, ref xform, false) || !this._lookup.TryGetCurrentBroadphase(xform, out broadphase))
      return intersectingBody;
    (PhysicsComponent, HashSet<EntityUid>) state1 = (body, intersectingBody);
    foreach (Fixture fixture in fixtureComp.Fixtures.Values)
    {
      foreach (FixtureProxy proxy in fixture.Proxies)
      {
        broadphase.StaticTree.QueryAabb<(PhysicsComponent, HashSet<EntityUid>)>(ref state1, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<(PhysicsComponent, HashSet<EntityUid>)>) ((ref (PhysicsComponent body, HashSet<EntityUid> entities) state, in FixtureProxy other) =>
        {
          if (other.Body.Deleted || other.Body == body || (collisionMask & other.Fixture.CollisionLayer) == 0)
            return true;
          state.Item2.Add(other.Entity);
          return true;
        }), proxy.AABB, approximate);
        broadphase.DynamicTree.QueryAabb<(PhysicsComponent, HashSet<EntityUid>)>(ref state1, (DynamicTree<FixtureProxy>.QueryCallbackDelegate<(PhysicsComponent, HashSet<EntityUid>)>) ((ref (PhysicsComponent body, HashSet<EntityUid> entities) state, in FixtureProxy other) =>
        {
          if (other.Body.Deleted || other.Body == body || (collisionMask & other.Fixture.CollisionLayer) == 0)
            return true;
          state.Item2.Add(other.Entity);
          return true;
        }), proxy.AABB, approximate);
      }
    }
    return intersectingBody;
  }

  [Obsolete("Use EntityLookupSystem")]
  public IEnumerable<PhysicsComponent> GetCollidingEntities(MapId mapId, in Box2 worldAABB)
  {
    if (mapId == MapId.Nullspace)
      return (IEnumerable<PhysicsComponent>) Array.Empty<PhysicsComponent>();
    Box2 box2 = worldAABB;
    HashSet<PhysicsComponent> collidingEntities = new HashSet<PhysicsComponent>();
    (SharedTransformSystem, HashSet<PhysicsComponent>, Box2) state = (this._transform, collidingEntities, box2);
    this._broadphase.GetBroadphases<(SharedTransformSystem, HashSet<PhysicsComponent>, Box2)>(mapId, worldAABB, ref state, (SharedBroadphaseSystem.BroadphaseCallback<(SharedTransformSystem, HashSet<PhysicsComponent>, Box2)>) ((Entity<BroadphaseComponent> entity, ref (SharedTransformSystem _transform, HashSet<PhysicsComponent> bodies, Box2 aabb) tuple) =>
    {
      Box2 aabb = Matrix3Helpers.TransformBox(tuple.Item1.GetInvWorldMatrix(entity.Owner), ref tuple.Item3);
      foreach (FixtureProxy fixtureProxy in entity.Comp.StaticTree.QueryAabb(aabb))
        tuple.Item2.Add(fixtureProxy.Body);
      foreach (FixtureProxy fixtureProxy in entity.Comp.DynamicTree.QueryAabb(aabb))
        tuple.Item2.Add(fixtureProxy.Body);
    }));
    return (IEnumerable<PhysicsComponent>) collidingEntities;
  }

  [Obsolete("Use EntityLookupSystem")]
  public IEnumerable<Entity<PhysicsComponent>> GetCollidingEntities(
    MapId mapId,
    in Box2Rotated worldBounds)
  {
    if (mapId == MapId.Nullspace)
      return (IEnumerable<Entity<PhysicsComponent>>) Array.Empty<Entity<PhysicsComponent>>();
    HashSet<Entity<PhysicsComponent>> collidingEntities = new HashSet<Entity<PhysicsComponent>>();
    (SharedTransformSystem, HashSet<Entity<PhysicsComponent>>, Box2Rotated) state = (this._transform, collidingEntities, worldBounds);
    this._broadphase.GetBroadphases<(SharedTransformSystem, HashSet<Entity<PhysicsComponent>>, Box2Rotated)>(mapId, ((Box2Rotated) ref worldBounds).CalcBoundingBox(), ref state, (SharedBroadphaseSystem.BroadphaseCallback<(SharedTransformSystem, HashSet<Entity<PhysicsComponent>>, Box2Rotated)>) ((Entity<BroadphaseComponent> entity, ref (SharedTransformSystem _transform, HashSet<Entity<PhysicsComponent>> bodies, Box2Rotated worldBounds) tuple) =>
    {
      Box2 aabb = Matrix3Helpers.TransformBox(tuple.Item1.GetInvWorldMatrix(entity.Owner), ref tuple.Item3);
      foreach (FixtureProxy fixtureProxy in entity.Comp.StaticTree.QueryAabb(aabb))
        tuple.Item2.Add((Entity<PhysicsComponent>) (fixtureProxy.Entity, fixtureProxy.Body));
      foreach (FixtureProxy fixtureProxy in entity.Comp.DynamicTree.QueryAabb(aabb))
        tuple.Item2.Add((Entity<PhysicsComponent>) (fixtureProxy.Entity, fixtureProxy.Body));
    }));
    return (IEnumerable<Entity<PhysicsComponent>>) collidingEntities;
  }

  public void GetContactingEntities(
    Entity<PhysicsComponent?> ent,
    HashSet<EntityUid> contacting,
    bool approximate = false)
  {
    if (!this.Resolve<PhysicsComponent>(ent.Owner, ref ent.Comp))
      return;
    LinkedListNode<Contact> linkedListNode = ent.Comp.Contacts.First;
    while (linkedListNode != null)
    {
      Contact contact = linkedListNode.Value;
      linkedListNode = linkedListNode.Next;
      if (approximate || contact.IsTouching)
        contacting.Add(ent.Owner == contact.EntityA ? contact.EntityB : contact.EntityA);
    }
  }

  public HashSet<EntityUid> GetContactingEntities(
    EntityUid uid,
    PhysicsComponent? body = null,
    bool approximate = false)
  {
    HashSet<EntityUid> contacting = new HashSet<EntityUid>();
    this.GetContactingEntities((Entity<PhysicsComponent>) (uid, body), contacting, approximate);
    return contacting;
  }

  public bool IsInContact(PhysicsComponent body, bool approximate = false)
  {
    for (LinkedListNode<Contact> linkedListNode = body.Contacts.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
    {
      if (approximate || linkedListNode.Value.IsTouching)
        return true;
    }
    return false;
  }

  public IEnumerable<RayCastResults> IntersectRayWithPredicate(
    MapId mapId,
    CollisionRay ray,
    float maxLength = 50f,
    Func<EntityUid, bool>? predicate = null,
    bool returnOnFirstHit = true)
  {
    Func<EntityUid, Func<EntityUid, bool>, bool> predicate1 = (Func<EntityUid, Func<EntityUid, bool>, bool>) ((uid, wrapped) => wrapped != null && wrapped(uid));
    return this.IntersectRayWithPredicate<Func<EntityUid, bool>>(mapId, ray, predicate, predicate1, maxLength, returnOnFirstHit);
  }

  public IEnumerable<RayCastResults> IntersectRayWithPredicate<TState>(
    MapId mapId,
    CollisionRay ray,
    TState state,
    Func<EntityUid, TState, bool> predicate,
    float maxLength = 50f,
    bool returnOnFirstHit = true)
  {
    List<RayCastResults> results = new List<RayCastResults>();
    Vector2 vector2 = ray.Position + Vector2Helpers.Normalized(ray.Direction) * maxLength;
    Box2 aabb;
    // ISSUE: explicit constructor call
    ((Box2) ref aabb).\u002Ector(Vector2.Min(ray.Position, vector2), Vector2.Max(ray.Position, vector2));
    this._broadphase.GetBroadphases(mapId, aabb, (SharedBroadphaseSystem.BroadphaseCallback) (broadphase =>
    {
      (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) rotationMatrixWithInv = this._transform.GetWorldPositionRotationMatrixWithInv(broadphase.Owner);
      Angle worldRotation = rotationMatrixWithInv.WorldRotation;
      Matrix3x2 matrix = rotationMatrixWithInv.WorldMatrix;
      Vector2 position = Vector2.Transform(ray.Position, rotationMatrixWithInv.InvWorldMatrix);
      Angle angle;
      // ISSUE: explicit constructor call
      ((Angle) ref angle).\u002Ector(-worldRotation.Theta);
      ref Angle local1 = ref angle;
      Vector2 direction1 = ray.Direction;
      ref Vector2 local2 = ref direction1;
      Vector2 direction2 = ((Angle) ref local1).RotateVec(ref local2);
      CollisionRay collisionRay = new CollisionRay(position, direction2, ray.CollisionMask);
      broadphase.Comp.StaticTree.QueryRay((DynamicTree<FixtureProxy>.RayQueryCallbackDelegate) ((in FixtureProxy proxy, in Vector2 point, float distFromOrigin) =>
      {
        if (returnOnFirstHit && results.Count > 0 || (double) distFromOrigin > (double) maxLength || (proxy.Fixture.CollisionLayer & ray.CollisionMask) == 0 || !proxy.Fixture.Hard || predicate(proxy.Entity, state))
          return true;
        results.Add(new RayCastResults(distFromOrigin, Vector2.Transform(point, matrix), proxy.Entity));
        return true;
      }), (Ray) collisionRay);
      broadphase.Comp.DynamicTree.QueryRay((DynamicTree<FixtureProxy>.RayQueryCallbackDelegate) ((in FixtureProxy proxy, in Vector2 point, float distFromOrigin) =>
      {
        if (returnOnFirstHit && results.Count > 0 || (double) distFromOrigin > (double) maxLength || (proxy.Fixture.CollisionLayer & ray.CollisionMask) == 0 || !proxy.Fixture.Hard || predicate(proxy.Entity, state))
          return true;
        results.Add(new RayCastResults(distFromOrigin, Vector2.Transform(point, matrix), proxy.Entity));
        return true;
      }), (Ray) collisionRay);
    }));
    results.Sort((Comparison<RayCastResults>) ((a, b) => a.Distance.CompareTo(b.Distance)));
    return (IEnumerable<RayCastResults>) results;
  }

  public IEnumerable<RayCastResults> IntersectRay(
    MapId mapId,
    CollisionRay ray,
    float maxLength = 50f,
    EntityUid? ignoredEnt = null,
    bool returnOnFirstHit = true)
  {
    Func<EntityUid, EntityUid?, bool> predicate = (Func<EntityUid, EntityUid?, bool>) ((uid, ignored) =>
    {
      EntityUid entityUid = uid;
      EntityUid? nullable = ignored;
      return nullable.HasValue && entityUid == nullable.GetValueOrDefault();
    });
    return this.IntersectRayWithPredicate<EntityUid?>(mapId, ray, ignoredEnt, predicate, maxLength, returnOnFirstHit);
  }

  public float IntersectRayPenetration(
    MapId mapId,
    CollisionRay ray,
    float maxLength,
    EntityUid? ignoredEnt = null)
  {
    float penetration = 0.0f;
    Vector2 vector2 = ray.Position + Vector2Helpers.Normalized(ray.Direction) * maxLength;
    Box2 aabb;
    // ISSUE: explicit constructor call
    ((Box2) ref aabb).\u002Ector(Vector2.Min(ray.Position, vector2), Vector2.Max(ray.Position, vector2));
    this._broadphase.GetBroadphases(mapId, aabb, (SharedBroadphaseSystem.BroadphaseCallback) (broadphase =>
    {
      (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) rotationInvMatrix = this._transform.GetWorldPositionRotationInvMatrix((EntityUid) broadphase);
      Angle worldRotation = rotationInvMatrix.WorldRotation;
      Vector2 position = Vector2.Transform(ray.Position, rotationInvMatrix.InvWorldMatrix);
      Angle angle;
      // ISSUE: explicit constructor call
      ((Angle) ref angle).\u002Ector(-worldRotation.Theta);
      ref Angle local1 = ref angle;
      Vector2 direction1 = ray.Direction;
      ref Vector2 local2 = ref direction1;
      Vector2 direction2 = ((Angle) ref local1).RotateVec(ref local2);
      CollisionRay gridRay = new CollisionRay(position, direction2, ray.CollisionMask);
      broadphase.Comp.StaticTree.QueryRay((DynamicTree<FixtureProxy>.RayQueryCallbackDelegate) ((in FixtureProxy proxy, in Vector2 point, float distFromOrigin) =>
      {
        if ((double) distFromOrigin <= (double) maxLength)
        {
          EntityUid entity = proxy.Entity;
          EntityUid? nullable = ignoredEnt;
          Vector2 hitPos;
          if ((nullable.HasValue ? (entity == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && proxy.Fixture.Hard && (proxy.Fixture.CollisionLayer & ray.CollisionMask) != 0 && new Ray(point + gridRay.Direction * ((Box2) ref proxy.AABB).Size.Length() * 2f, -gridRay.Direction).Intersects(proxy.AABB, out float _, out hitPos))
          {
            penetration += (point - hitPos).Length();
            return true;
          }
        }
        return true;
      }), (Ray) gridRay);
      broadphase.Comp.DynamicTree.QueryRay((DynamicTree<FixtureProxy>.RayQueryCallbackDelegate) ((in FixtureProxy proxy, in Vector2 point, float distFromOrigin) =>
      {
        if ((double) distFromOrigin <= (double) maxLength)
        {
          EntityUid entity = proxy.Entity;
          EntityUid? nullable = ignoredEnt;
          Vector2 hitPos;
          if ((nullable.HasValue ? (entity == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && proxy.Fixture.Hard && (proxy.Fixture.CollisionLayer & ray.CollisionMask) != 0 && new Ray(point + gridRay.Direction * ((Box2) ref proxy.AABB).Size.Length() * 2f, -gridRay.Direction).Intersects(proxy.AABB, out float _, out hitPos))
          {
            penetration += (point - hitPos).Length();
            return true;
          }
        }
        return true;
      }), (Ray) gridRay);
    }));
    return penetration;
  }

  public bool TryGetDistance(
    EntityUid uidA,
    EntityUid uidB,
    out float distance,
    TransformComponent? xformA = null,
    TransformComponent? xformB = null,
    FixturesComponent? managerA = null,
    FixturesComponent? managerB = null,
    PhysicsComponent? bodyA = null,
    PhysicsComponent? bodyB = null)
  {
    return this.TryGetNearest(uidA, uidB, out Vector2 _, out Vector2 _, out distance, xformA, xformB, managerA, managerB, bodyA, bodyB);
  }

  public bool TryGetNearestPoints(
    EntityUid uidA,
    EntityUid uidB,
    out Vector2 pointA,
    out Vector2 pointB,
    TransformComponent? xformA = null,
    TransformComponent? xformB = null,
    FixturesComponent? managerA = null,
    FixturesComponent? managerB = null,
    PhysicsComponent? bodyA = null,
    PhysicsComponent? bodyB = null)
  {
    return this.TryGetNearest(uidA, uidB, out pointA, out pointB, out float _, xformA, xformB, managerA, managerB, bodyA, bodyB);
  }

  public bool TryGetNearest(
    EntityUid uidA,
    EntityUid uidB,
    out Vector2 pointA,
    out Vector2 pointB,
    out float distance,
    Robust.Shared.Physics.Transform xfA,
    Robust.Shared.Physics.Transform xfB,
    FixturesComponent? managerA = null,
    FixturesComponent? managerB = null,
    PhysicsComponent? bodyA = null,
    PhysicsComponent? bodyB = null)
  {
    pointA = Vector2.Zero;
    pointB = Vector2.Zero;
    if (!this.Resolve<FixturesComponent, PhysicsComponent>(uidA, ref managerA, ref bodyA) || !this.Resolve<FixturesComponent, PhysicsComponent>(uidB, ref managerB, ref bodyB) || managerA.FixtureCount == 0 || managerB.FixtureCount == 0)
    {
      distance = 0.0f;
      return false;
    }
    distance = float.MaxValue;
    DistanceInput input = new DistanceInput()
    {
      TransformA = xfA,
      TransformB = xfB,
      UseRadii = true
    };
    foreach (Fixture fixture1 in managerA.Fixtures.Values)
    {
      if (!bodyA.Hard || fixture1.Hard)
      {
        for (int index1 = 0; index1 < fixture1.Shape.ChildCount; ++index1)
        {
          input.ProxyA.Set<IPhysShape>(fixture1.Shape, index1);
          foreach (Fixture fixture2 in managerB.Fixtures.Values)
          {
            if (!bodyB.Hard || fixture2.Hard)
            {
              for (int index2 = 0; index2 < fixture2.Shape.ChildCount; ++index2)
              {
                input.ProxyB.Set<IPhysShape>(fixture2.Shape, index2);
                DistanceOutput output;
                DistanceManager.ComputeDistance(out output, out SimplexCache _, in input);
                if ((double) distance >= (double) output.Distance)
                {
                  pointA = output.PointA;
                  pointB = output.PointB;
                  distance = output.Distance;
                }
              }
            }
          }
        }
      }
    }
    return true;
  }

  public bool TryGetNearest(
    EntityUid uid,
    MapCoordinates coordinates,
    out Vector2 point,
    out float distance,
    TransformComponent? xformA = null,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this.Resolve(uid, ref xformA) || xformA.MapID != coordinates.MapId)
    {
      point = Vector2.Zero;
      distance = 0.0f;
      return false;
    }
    point = Vector2.Zero;
    if (!this.Resolve<FixturesComponent, PhysicsComponent>(uid, ref manager, ref body) || manager.FixtureCount == 0)
    {
      distance = 0.0f;
      return false;
    }
    Robust.Shared.Physics.Transform physicsTransform = this.GetPhysicsTransform(uid, xformA);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(coordinates.Position, Angle.Zero);
    distance = float.MaxValue;
    DistanceInput input = new DistanceInput();
    input.TransformA = physicsTransform;
    input.TransformB = transform;
    input.UseRadii = true;
    PhysShapeCircle shape = new PhysShapeCircle(1.401298E-44f, Vector2.Zero);
    foreach (Fixture fixture in manager.Fixtures.Values)
    {
      if (!body.Hard || fixture.Hard)
      {
        input.ProxyA.Set<IPhysShape>(fixture.Shape, 0);
        input.ProxyB.Set<PhysShapeCircle>(shape, 0);
        DistanceOutput output;
        DistanceManager.ComputeDistance(out output, out SimplexCache _, in input);
        if ((double) distance >= (double) output.Distance)
        {
          point = output.PointA;
          distance = output.Distance;
        }
      }
    }
    return true;
  }

  public bool TryGetNearest(
    EntityUid uidA,
    EntityUid uidB,
    out Vector2 point,
    out Vector2 pointB,
    out float distance,
    TransformComponent? xformA = null,
    TransformComponent? xformB = null,
    FixturesComponent? managerA = null,
    FixturesComponent? managerB = null,
    PhysicsComponent? bodyA = null,
    PhysicsComponent? bodyB = null)
  {
    if (!this.Resolve(uidA, ref xformA) || !this.Resolve(uidB, ref xformB) || xformA.MapID != xformB.MapID)
    {
      point = Vector2.Zero;
      pointB = Vector2.Zero;
      distance = 0.0f;
      return false;
    }
    Robust.Shared.Physics.Transform physicsTransform1 = this.GetPhysicsTransform(uidA, xformA);
    Robust.Shared.Physics.Transform physicsTransform2 = this.GetPhysicsTransform(uidB, xformB);
    return this.TryGetNearest(uidA, uidB, out point, out pointB, out distance, physicsTransform1, physicsTransform2, managerA, managerB, bodyA, bodyB);
  }

  public void SetRadius(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    IPhysShape shape,
    float radius,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (MathHelper.CloseTo(shape.Radius, radius, 1E-07f) || !this.Resolve<FixturesComponent, PhysicsComponent, TransformComponent>(uid, ref manager, ref body, ref xform))
      return;
    shape.Radius = radius;
    if (body.CanCollide)
    {
      ref BroadphaseData? local = ref xform.Broadphase;
      BroadphaseComponent comp;
      if (this.TryComp<BroadphaseComponent>(local.HasValue ? new EntityUid?(local.GetValueOrDefault().Uid) : new EntityUid?(), out comp))
      {
        this._lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
        this._lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
      }
    }
    this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
  }

  public void SetPositionRadius(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    PhysShapeCircle shape,
    Vector2 position,
    float radius,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (MathHelper.CloseTo(shape.Radius, radius, 1E-07f) && Vector2Helpers.EqualsApprox(shape.Position, position) || !this.Resolve<FixturesComponent, PhysicsComponent, TransformComponent>(uid, ref manager, ref body, ref xform))
      return;
    shape.Position = position;
    shape.Radius = radius;
    if (body.CanCollide)
    {
      ref BroadphaseData? local = ref xform.Broadphase;
      BroadphaseComponent comp;
      if (this.TryComp<BroadphaseComponent>(local.HasValue ? new EntityUid?(local.GetValueOrDefault().Uid) : new EntityUid?(), out comp))
      {
        this._lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
        this._lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
      }
    }
    this.Dirty(uid, (IComponent) manager);
  }

  public void SetPosition(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    PhysShapeCircle circle,
    Vector2 position,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (Vector2Helpers.EqualsApprox(circle.Position, position) || !this.Resolve<FixturesComponent, PhysicsComponent, TransformComponent>(uid, ref manager, ref body, ref xform))
      return;
    circle.Position = position;
    if (body.CanCollide)
    {
      ref BroadphaseData? local = ref xform.Broadphase;
      BroadphaseComponent comp;
      if (this.TryComp<BroadphaseComponent>(local.HasValue ? new EntityUid?(local.GetValueOrDefault().Uid) : new EntityUid?(), out comp))
      {
        this._lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
        this._lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
      }
    }
    this.Dirty(uid, (IComponent) manager);
  }

  public void SetVertices(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    EdgeShape edge,
    Vector2 vertex0,
    Vector2 vertex1,
    Vector2 vertex2,
    Vector2 vertex3,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this.Resolve<FixturesComponent, PhysicsComponent, TransformComponent>(uid, ref manager, ref body, ref xform))
      return;
    edge.Vertex0 = vertex0;
    edge.Vertex1 = vertex1;
    edge.Vertex2 = vertex2;
    edge.Vertex3 = vertex3;
    if (body.CanCollide)
    {
      ref BroadphaseData? local = ref xform.Broadphase;
      BroadphaseComponent comp;
      if (this.TryComp<BroadphaseComponent>(local.HasValue ? new EntityUid?(local.GetValueOrDefault().Uid) : new EntityUid?(), out comp))
      {
        this._lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
        this._lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
      }
    }
    this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
  }

  public void SetVertices(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    PolygonShape poly,
    Vector2[] vertices,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this.Resolve<FixturesComponent, PhysicsComponent, TransformComponent>(uid, ref manager, ref body, ref xform))
      return;
    poly.Set((ReadOnlySpan<Vector2>) vertices, vertices.Length);
    if (body.CanCollide)
    {
      ref BroadphaseData? local = ref xform.Broadphase;
      BroadphaseComponent comp;
      if (this.TryComp<BroadphaseComponent>(local.HasValue ? new EntityUid?(local.GetValueOrDefault().Uid) : new EntityUid?(), out comp))
      {
        this._lookup.DestroyProxies(uid, fixtureId, fixture, xform, comp);
        this._lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
      }
    }
    this._fixtures.FixtureUpdate(uid, manager: manager, body: body);
  }

  private void ResetSolver(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    ContactVelocityConstraint[] velocityConstraints,
    ContactPositionConstraint[] positionConstraints)
  {
    int count = island.Contacts.Count;
    for (int index1 = 0; index1 < count; ++index1)
    {
      Contact contact = island.Contacts[index1];
      Fixture fixtureA = contact.FixtureA;
      Fixture fixtureB = contact.FixtureB;
      IPhysShape shape1 = fixtureA.Shape;
      IPhysShape shape2 = fixtureB.Shape;
      float radius1 = shape1.Radius;
      float radius2 = shape2.Radius;
      PhysicsComponent bodyA = contact.BodyA;
      PhysicsComponent bodyB = contact.BodyB;
      Manifold manifold = contact.Manifold;
      int pointCount = manifold.PointCount;
      ref ContactVelocityConstraint local1 = ref velocityConstraints[index1];
      local1.Friction = contact.Friction;
      local1.Restitution = contact.Restitution;
      local1.TangentSpeed = contact.TangentSpeed;
      local1.IndexA = bodyA.IslandIndex[island.Index];
      local1.IndexB = bodyB.IslandIndex[island.Index];
      (float num1, float num2) = this.GetInvMass(bodyA, bodyB);
      ref float local2 = ref local1.InvMassA;
      ref float local3 = ref local1.InvMassB;
      float num3 = num1;
      float num4 = num2;
      local2 = num3;
      double num5 = (double) num4;
      local3 = (float) num5;
      local1.InvIA = bodyA.InvI;
      local1.InvIB = bodyB.InvI;
      local1.ContactIndex = index1;
      local1.PointCount = pointCount;
      local1.K = Vector4.Zero;
      local1.NormalMass = Vector4.Zero;
      ref ContactPositionConstraint local4 = ref positionConstraints[index1];
      local4.IndexA = bodyA.IslandIndex[island.Index];
      local4.IndexB = bodyB.IslandIndex[island.Index];
      ref float local5 = ref local4.InvMassA;
      ref float local6 = ref local4.InvMassB;
      float num6 = num1;
      float num7 = num2;
      local5 = num6;
      double num8 = (double) num7;
      local6 = (float) num8;
      local4.LocalCenterA = bodyA.LocalCenter;
      local4.LocalCenterB = bodyB.LocalCenter;
      local4.InvIA = bodyA.InvI;
      local4.InvIB = bodyB.InvI;
      local4.LocalNormal = manifold.LocalNormal;
      local4.LocalPoint = manifold.LocalPoint;
      local4.PointCount = pointCount;
      local4.RadiusA = radius1;
      local4.RadiusB = radius2;
      local4.Type = manifold.Type;
      Span<ManifoldPoint> asSpan1 = manifold.Points.AsSpan;
      Span<Vector2> asSpan2 = local4.LocalPoints.AsSpan;
      Span<VelocityConstraintPoint> asSpan3 = local1.Points.AsSpan;
      for (int index2 = 0; index2 < pointCount; ++index2)
      {
        ManifoldPoint manifoldPoint = asSpan1[index2];
        ref VelocityConstraintPoint local7 = ref asSpan3[index2];
        if (this._warmStarting)
        {
          local7.NormalImpulse = data.DtRatio * manifoldPoint.NormalImpulse;
          local7.TangentImpulse = data.DtRatio * manifoldPoint.TangentImpulse;
        }
        else
        {
          local7.NormalImpulse = 0.0f;
          local7.TangentImpulse = 0.0f;
        }
        local7.RelativeVelocityA = Vector2.Zero;
        local7.RelativeVelocityB = Vector2.Zero;
        local7.NormalMass = 0.0f;
        local7.TangentMass = 0.0f;
        local7.VelocityBias = 0.0f;
        asSpan2[index2] = manifoldPoint.LocalPoint;
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
          case BodyType.KinematicController:
            return (0.0f, 0.0f);
          case BodyType.Dynamic:
            return (bodyA.InvMass, 0.0f);
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
            return (0.0f, bodyB.InvMass);
          default:
            throw new ArgumentOutOfRangeException();
        }
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private unsafe void InitializeVelocityConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    ContactVelocityConstraint[] velocityConstraints,
    ContactPositionConstraint[] positionConstraints,
    Vector2[] positions,
    float[] angles,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    ; // Unable to render the statement
    Span<Vector2> points = new Span<Vector2>((void*) pointer, 2);
    int count = island.Contacts.Count;
    List<Contact> contacts = island.Contacts;
    int offset = island.Offset;
    for (int index1 = 0; index1 < count; ++index1)
    {
      ref ContactVelocityConstraint local1 = ref velocityConstraints[index1];
      ContactPositionConstraint positionConstraint = positionConstraints[index1];
      float radiusA = positionConstraint.RadiusA;
      float radiusB = positionConstraint.RadiusB;
      Manifold manifold = contacts[local1.ContactIndex].Manifold;
      int indexA = local1.IndexA;
      int indexB = local1.IndexB;
      float invMassA = local1.InvMassA;
      float invMassB = local1.InvMassB;
      float invIa = local1.InvIA;
      float invIb = local1.InvIB;
      Vector2 vector1 = positionConstraint.LocalCenterA;
      Vector2 vector2 = positionConstraint.LocalCenterB;
      Vector2 position1 = positions[indexA];
      float angle1 = angles[indexA];
      Vector2 linearVelocity1 = linearVelocities[offset + indexA];
      float angularVelocity1 = angularVelocities[offset + indexA];
      Vector2 position2 = positions[indexB];
      float angle2 = angles[indexB];
      Vector2 linearVelocity2 = linearVelocities[offset + indexB];
      float angularVelocity2 = angularVelocities[offset + indexB];
      Robust.Shared.Physics.Transform xfA = new Robust.Shared.Physics.Transform(angle1);
      Robust.Shared.Physics.Transform xfB = new Robust.Shared.Physics.Transform(angle2);
      xfA.Position = position1 - Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in vector1);
      xfB.Position = position2 - Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in vector2);
      Vector2 normal;
      SharedPhysicsSystem.InitializeManifold(ref manifold, in xfA, in xfB, radiusA, radiusB, out normal, points);
      local1.Normal = normal;
      int pointCount = local1.PointCount;
      Span<VelocityConstraintPoint> asSpan = local1.Points.AsSpan;
      for (int index2 = 0; index2 < pointCount; ++index2)
      {
        ref VelocityConstraintPoint local2 = ref asSpan[index2];
        local2.RelativeVelocityA = points[index2] - position1;
        local2.RelativeVelocityB = points[index2] - position2;
        float num1 = Vector2Helpers.Cross(local2.RelativeVelocityA, local1.Normal);
        float num2 = Vector2Helpers.Cross(local2.RelativeVelocityB, local1.Normal);
        float num3 = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num1 * (double) num1 + (double) invIb * (double) num2 * (double) num2);
        local2.NormalMass = (double) num3 > 0.0 ? 1f / num3 : 0.0f;
        ref Vector2 local3 = ref local1.Normal;
        float num4 = 1f;
        ref float local4 = ref num4;
        Vector2 vector2_1 = Vector2Helpers.Cross(ref local3, ref local4);
        float num5 = Vector2Helpers.Cross(local2.RelativeVelocityA, vector2_1);
        float num6 = Vector2Helpers.Cross(local2.RelativeVelocityB, vector2_1);
        float num7 = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num5 * (double) num5 + (double) invIb * (double) num6 * (double) num6);
        local2.TangentMass = (double) num7 > 0.0 ? 1f / num7 : 0.0f;
        local2.VelocityBias = 0.0f;
        float num8 = Vector2.Dot(local1.Normal, linearVelocity2 + Vector2Helpers.Cross(angularVelocity2, ref local2.RelativeVelocityB) - linearVelocity1 - Vector2Helpers.Cross(angularVelocity1, ref local2.RelativeVelocityA));
        if ((double) num8 < -(double) data.VelocityThreshold)
          local2.VelocityBias = -local1.Restitution * num8;
      }
      if (local1.PointCount == 2)
      {
        VelocityConstraintPoint velocityConstraintPoint1 = local1.Points._00;
        VelocityConstraintPoint velocityConstraintPoint2 = local1.Points._01;
        float num9 = Vector2Helpers.Cross(velocityConstraintPoint1.RelativeVelocityA, local1.Normal);
        float num10 = Vector2Helpers.Cross(velocityConstraintPoint1.RelativeVelocityB, local1.Normal);
        float num11 = Vector2Helpers.Cross(velocityConstraintPoint2.RelativeVelocityA, local1.Normal);
        float num12 = Vector2Helpers.Cross(velocityConstraintPoint2.RelativeVelocityB, local1.Normal);
        float x = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num9 * (double) num9 + (double) invIb * (double) num10 * (double) num10);
        float w = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num11 * (double) num11 + (double) invIb * (double) num12 * (double) num12);
        float num13 = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num9 * (double) num11 + (double) invIb * (double) num10 * (double) num12);
        if ((double) x * (double) x < 1000.0 * ((double) x * (double) w - (double) num13 * (double) num13))
        {
          local1.K = new Vector4(x, num13, num13, w);
          local1.NormalMass = Vector4Helpers.Inverse(local1.K);
        }
        else
          local1.PointCount = 1;
      }
    }
  }

  private void WarmStart(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    ContactVelocityConstraint[] velocityConstraints,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    int offset = island.Offset;
    for (int index1 = 0; index1 < island.Contacts.Count; ++index1)
    {
      ContactVelocityConstraint velocityConstraint = velocityConstraints[index1];
      Span<VelocityConstraintPoint> asSpan = velocityConstraint.Points.AsSpan;
      int indexA = velocityConstraint.IndexA;
      int indexB = velocityConstraint.IndexB;
      float invMassA = velocityConstraint.InvMassA;
      float invIa = velocityConstraint.InvIA;
      float invMassB = velocityConstraint.InvMassB;
      float invIb = velocityConstraint.InvIB;
      int pointCount = velocityConstraint.PointCount;
      ref Vector2 local1 = ref linearVelocities[offset + indexA];
      ref float local2 = ref angularVelocities[offset + indexA];
      ref Vector2 local3 = ref linearVelocities[offset + indexB];
      ref float local4 = ref angularVelocities[offset + indexB];
      Vector2 normal = velocityConstraint.Normal;
      ref Vector2 local5 = ref normal;
      float num = 1f;
      ref float local6 = ref num;
      Vector2 vector2_1 = Vector2Helpers.Cross(ref local5, ref local6);
      for (int index2 = 0; index2 < pointCount; ++index2)
      {
        VelocityConstraintPoint velocityConstraintPoint = asSpan[index2];
        Vector2 vector2_2 = normal * velocityConstraintPoint.NormalImpulse + vector2_1 * velocityConstraintPoint.TangentImpulse;
        local2 -= invIa * Vector2Helpers.Cross(velocityConstraintPoint.RelativeVelocityA, vector2_2);
        local1 -= vector2_2 * invMassA;
        local4 += invIb * Vector2Helpers.Cross(velocityConstraintPoint.RelativeVelocityB, vector2_2);
        local3 += vector2_2 * invMassB;
      }
    }
  }

  private static void SolveVelocityConstraints(
    in SharedPhysicsSystem.IslandData island,
    ParallelOptions? options,
    ContactVelocityConstraint[] velocityConstraints,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    int count = island.Contacts.Count;
    if (options != null && count > 32 /*0x20*/)
      ProcessParallelInternal(island, count, options, velocityConstraints, linearVelocities, angularVelocities);
    else
      SharedPhysicsSystem.SolveVelocityConstraints(in island, 0, count, velocityConstraints, linearVelocities, angularVelocities);

    static void ProcessParallelInternal(
      SharedPhysicsSystem.IslandData island,
      int contactCount,
      ParallelOptions options,
      ContactVelocityConstraint[] velocityConstraints,
      Vector2[] linearVelocities,
      float[] angularVelocities)
    {
      Parallel.For(0, (int) Math.Ceiling((double) contactCount / 16.0), options, (Action<int>) (i =>
      {
        int start = i * 16 /*0x10*/;
        int end = Math.Min(start + 16 /*0x10*/, contactCount);
        SharedPhysicsSystem.SolveVelocityConstraints(in island, start, end, velocityConstraints, linearVelocities, angularVelocities);
      }));
    }
  }

  private static void SolveVelocityConstraints(
    in SharedPhysicsSystem.IslandData island,
    int start,
    int end,
    ContactVelocityConstraint[] velocityConstraints,
    Vector2[] linearVelocities,
    float[] angularVelocities)
  {
    int offset = island.Offset;
    for (int index1 = start; index1 < end; ++index1)
    {
      ref ContactVelocityConstraint local1 = ref velocityConstraints[index1];
      int indexA = local1.IndexA;
      int indexB = local1.IndexB;
      float invMassA = local1.InvMassA;
      float invIa = local1.InvIA;
      float invMassB = local1.InvMassB;
      float invIb = local1.InvIB;
      int pointCount = local1.PointCount;
      ref Vector2 local2 = ref linearVelocities[offset + indexA];
      ref float local3 = ref angularVelocities[offset + indexA];
      ref Vector2 local4 = ref linearVelocities[offset + indexB];
      ref float local5 = ref angularVelocities[offset + indexB];
      Vector2 normal = local1.Normal;
      ref Vector2 local6 = ref normal;
      float num1 = 1f;
      ref float local7 = ref num1;
      Vector2 vector2_1 = Vector2Helpers.Cross(ref local6, ref local7);
      float friction = local1.Friction;
      Span<VelocityConstraintPoint> asSpan = local1.Points.AsSpan;
      for (int index2 = 0; index2 < pointCount; ++index2)
      {
        ref VelocityConstraintPoint local8 = ref asSpan[index2];
        float num2 = Vector2.Dot(local4 + Vector2Helpers.Cross(local5, ref local8.RelativeVelocityB) - local2 - Vector2Helpers.Cross(local3, ref local8.RelativeVelocityA), vector2_1) - local1.TangentSpeed;
        float num3 = local8.TangentMass * -num2;
        float max = friction * local8.NormalImpulse;
        float num4 = Math.Clamp(local8.TangentImpulse + num3, -max, max);
        float num5 = num4 - local8.TangentImpulse;
        local8.TangentImpulse = num4;
        Vector2 vector2_2 = vector2_1 * num5;
        local2 -= vector2_2 * invMassA;
        local3 -= invIa * Vector2Helpers.Cross(local8.RelativeVelocityA, vector2_2);
        local4 += vector2_2 * invMassB;
        local5 += invIb * Vector2Helpers.Cross(local8.RelativeVelocityB, vector2_2);
      }
      if (local1.PointCount == 1)
      {
        ref VelocityConstraintPoint local9 = ref local1.Points._00;
        float num6 = Vector2.Dot(local4 + Vector2Helpers.Cross(local5, ref local9.RelativeVelocityB) - local2 - Vector2Helpers.Cross(local3, ref local9.RelativeVelocityA), normal);
        float num7 = (float) (-(double) local9.NormalMass * ((double) num6 - (double) local9.VelocityBias));
        float num8 = Math.Max(local9.NormalImpulse + num7, 0.0f);
        float num9 = num8 - local9.NormalImpulse;
        local9.NormalImpulse = num8;
        Vector2 vector2_3 = normal * num9;
        local2 -= vector2_3 * invMassA;
        local3 -= invIa * Vector2Helpers.Cross(local9.RelativeVelocityA, vector2_3);
        local4 += vector2_3 * invMassB;
        local5 += invIb * Vector2Helpers.Cross(local9.RelativeVelocityB, vector2_3);
      }
      else
      {
        ref VelocityConstraintPoint local10 = ref local1.Points._00;
        ref VelocityConstraintPoint local11 = ref local1.Points._01;
        Vector2 v1 = new Vector2(local10.NormalImpulse, local11.NormalImpulse);
        Vector2 vector2_4 = local4 + Vector2Helpers.Cross(local5, ref local10.RelativeVelocityB) - local2 - Vector2Helpers.Cross(local3, ref local10.RelativeVelocityA);
        Vector2 vector2_5 = local4 + Vector2Helpers.Cross(local5, ref local11.RelativeVelocityB) - local2 - Vector2Helpers.Cross(local3, ref local11.RelativeVelocityA);
        float num10 = Vector2.Dot(vector2_4, normal);
        Vector2 vector2_6 = normal;
        float num11 = Vector2.Dot(vector2_5, vector2_6);
        Vector2 v2 = new Vector2()
        {
          X = num10 - local10.VelocityBias,
          Y = num11 - local11.VelocityBias
        } - Robust.Shared.Physics.Transform.Mul(local1.K, v1);
        Vector2 vector2_7 = -Robust.Shared.Physics.Transform.Mul(local1.NormalMass, v2);
        if ((double) vector2_7.X >= 0.0 && (double) vector2_7.Y >= 0.0)
        {
          Vector2 vector2_8 = vector2_7 - v1;
          Vector2 vector2_9 = normal * vector2_8.X;
          Vector2 vector2_10 = normal * vector2_8.Y;
          local2 -= (vector2_9 + vector2_10) * invMassA;
          local3 -= invIa * (Vector2Helpers.Cross(local10.RelativeVelocityA, vector2_9) + Vector2Helpers.Cross(local11.RelativeVelocityA, vector2_10));
          local4 += (vector2_9 + vector2_10) * invMassB;
          local5 += invIb * (Vector2Helpers.Cross(local10.RelativeVelocityB, vector2_9) + Vector2Helpers.Cross(local11.RelativeVelocityB, vector2_10));
          local10.NormalImpulse = vector2_7.X;
          local11.NormalImpulse = vector2_7.Y;
        }
        else
        {
          vector2_7.X = -local10.NormalMass * v2.X;
          vector2_7.Y = 0.0f;
          float num12 = local1.K.Y * vector2_7.X + v2.Y;
          if ((double) vector2_7.X >= 0.0 && (double) num12 >= 0.0)
          {
            Vector2 vector2_11 = vector2_7 - v1;
            Vector2 vector2_12 = normal * vector2_11.X;
            Vector2 vector2_13 = normal * vector2_11.Y;
            local2 -= (vector2_12 + vector2_13) * invMassA;
            local3 -= invIa * (Vector2Helpers.Cross(local10.RelativeVelocityA, vector2_12) + Vector2Helpers.Cross(local11.RelativeVelocityA, vector2_13));
            local4 += (vector2_12 + vector2_13) * invMassB;
            local5 += invIb * (Vector2Helpers.Cross(local10.RelativeVelocityB, vector2_12) + Vector2Helpers.Cross(local11.RelativeVelocityB, vector2_13));
            local10.NormalImpulse = vector2_7.X;
            local11.NormalImpulse = vector2_7.Y;
          }
          else
          {
            vector2_7.X = 0.0f;
            vector2_7.Y = -local11.NormalMass * v2.Y;
            float num13 = local1.K.Z * vector2_7.Y + v2.X;
            if ((double) vector2_7.Y >= 0.0 && (double) num13 >= 0.0)
            {
              Vector2 vector2_14 = vector2_7 - v1;
              Vector2 vector2_15 = normal * vector2_14.X;
              Vector2 vector2_16 = normal * vector2_14.Y;
              local2 -= (vector2_15 + vector2_16) * invMassA;
              local3 -= invIa * (Vector2Helpers.Cross(local10.RelativeVelocityA, vector2_15) + Vector2Helpers.Cross(local11.RelativeVelocityA, vector2_16));
              local4 += (vector2_15 + vector2_16) * invMassB;
              local5 += invIb * (Vector2Helpers.Cross(local10.RelativeVelocityB, vector2_15) + Vector2Helpers.Cross(local11.RelativeVelocityB, vector2_16));
              local10.NormalImpulse = vector2_7.X;
              local11.NormalImpulse = vector2_7.Y;
            }
            else
            {
              vector2_7.X = 0.0f;
              vector2_7.Y = 0.0f;
              float x = v2.X;
              float y = v2.Y;
              if ((double) x >= 0.0 && (double) y >= 0.0)
              {
                Vector2 vector2_17 = vector2_7 - v1;
                Vector2 vector2_18 = normal * vector2_17.X;
                Vector2 vector2_19 = normal * vector2_17.Y;
                local2 -= (vector2_18 + vector2_19) * invMassA;
                local3 -= invIa * (Vector2Helpers.Cross(local10.RelativeVelocityA, vector2_18) + Vector2Helpers.Cross(local11.RelativeVelocityA, vector2_19));
                local4 += (vector2_18 + vector2_19) * invMassB;
                local5 += invIb * (Vector2Helpers.Cross(local10.RelativeVelocityB, vector2_18) + Vector2Helpers.Cross(local11.RelativeVelocityB, vector2_19));
                local10.NormalImpulse = vector2_7.X;
                local11.NormalImpulse = vector2_7.Y;
              }
            }
          }
        }
      }
    }
  }

  private void StoreImpulses(
    in SharedPhysicsSystem.IslandData island,
    ContactVelocityConstraint[] velocityConstraints)
  {
    for (int index1 = 0; index1 < island.Contacts.Count; ++index1)
    {
      ref ContactVelocityConstraint local1 = ref velocityConstraints[index1];
      Span<ManifoldPoint> asSpan1 = island.Contacts[local1.ContactIndex].Manifold.Points.AsSpan;
      Span<VelocityConstraintPoint> asSpan2 = local1.Points.AsSpan;
      for (int index2 = 0; index2 < local1.PointCount; ++index2)
      {
        ref ManifoldPoint local2 = ref asSpan1[index2];
        local2.NormalImpulse = asSpan2[index2].NormalImpulse;
        local2.TangentImpulse = asSpan2[index2].TangentImpulse;
      }
    }
  }

  private static bool SolvePositionConstraints(
    in SolverData data,
    in SharedPhysicsSystem.IslandData island,
    ParallelOptions? options,
    ContactPositionConstraint[] positionConstraints,
    Vector2[] positions,
    float[] angles)
  {
    int count = island.Contacts.Count;
    return options != null && count > 32 /*0x20*/ ? ProcessParallelInternal(count, data, options, positionConstraints, positions, angles) : SharedPhysicsSystem.SolvePositionConstraints(in data, 0, count, positionConstraints, positions, angles);

    static bool ProcessParallelInternal(
      int contactCount,
      SolverData data,
      ParallelOptions options,
      ContactPositionConstraint[] positionConstraints,
      Vector2[] positions,
      float[] angles)
    {
      int unsolved = 0;
      Parallel.For(0, (int) Math.Ceiling((double) contactCount / 16.0), options, (Action<int>) (i =>
      {
        int start = i * 16 /*0x10*/;
        int end = Math.Min(start + 16 /*0x10*/, contactCount);
        if (SharedPhysicsSystem.SolvePositionConstraints(in data, start, end, positionConstraints, positions, angles))
          return;
        Interlocked.Increment(ref unsolved);
      }));
      return unsolved == 0;
    }
  }

  private static bool SolvePositionConstraints(
    in SolverData data,
    int start,
    int end,
    ContactPositionConstraint[] positionConstraints,
    Vector2[] positions,
    float[] angles)
  {
    float val1 = 0.0f;
    for (int index1 = start; index1 < end; ++index1)
    {
      ContactPositionConstraint pc = positionConstraints[index1];
      int indexA = pc.IndexA;
      int indexB = pc.IndexB;
      Vector2 vector1 = pc.LocalCenterA;
      float invMassA = pc.InvMassA;
      float invIa = pc.InvIA;
      Vector2 vector2 = pc.LocalCenterB;
      float invMassB = pc.InvMassB;
      float invIb = pc.InvIB;
      int pointCount = pc.PointCount;
      ref Vector2 local1 = ref positions[indexA];
      ref float local2 = ref angles[indexA];
      ref Vector2 local3 = ref positions[indexB];
      ref float local4 = ref angles[indexB];
      for (int index2 = 0; index2 < pointCount; ++index2)
      {
        Robust.Shared.Physics.Transform xfA = new Robust.Shared.Physics.Transform(local2);
        Robust.Shared.Physics.Transform xfB = new Robust.Shared.Physics.Transform(local4);
        xfA.Position = local1 - Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in vector1);
        xfB.Position = local3 - Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in vector2);
        Vector2 normal;
        Vector2 point;
        float separation;
        SharedPhysicsSystem.PositionSolverManifoldInitialize(in pc, index2, in xfA, in xfB, out normal, out point, out separation);
        Vector2 vector2_1 = point - local1;
        Vector2 vector2_2 = point - local3;
        val1 = Math.Min(val1, separation);
        float num1 = Math.Clamp(data.Baumgarte * (separation + 0.005f), -data.MaxLinearCorrection, 0.0f);
        float num2 = Vector2Helpers.Cross(vector2_1, normal);
        float num3 = Vector2Helpers.Cross(vector2_2, normal);
        float num4 = (float) ((double) invMassA + (double) invMassB + (double) invIa * (double) num2 * (double) num2 + (double) invIb * (double) num3 * (double) num3);
        float num5 = (double) num4 > 0.0 ? -num1 / num4 : 0.0f;
        Vector2 vector2_3 = normal * num5;
        local1 -= vector2_3 * invMassA;
        local2 -= invIa * Vector2Helpers.Cross(vector2_1, vector2_3);
        local3 += vector2_3 * invMassB;
        local4 += invIb * Vector2Helpers.Cross(vector2_2, vector2_3);
      }
    }
    return (double) val1 >= -0.014999999664723873;
  }

  internal static void InitializeManifold(
    ref Manifold manifold,
    in Robust.Shared.Physics.Transform xfA,
    in Robust.Shared.Physics.Transform xfB,
    float radiusA,
    float radiusB,
    out Vector2 normal,
    Span<Vector2> points)
  {
    normal = Vector2.Zero;
    if (manifold.PointCount == 0)
      return;
    switch (manifold.Type)
    {
      case ManifoldType.Circles:
        normal = new Vector2(1f, 0.0f);
        Vector2 vector2_1 = Robust.Shared.Physics.Transform.Mul(in xfA, in manifold.LocalPoint);
        Vector2 vector2_2 = Robust.Shared.Physics.Transform.Mul(in xfB, in manifold.Points._00.LocalPoint);
        if ((double) (vector2_1 - vector2_2).LengthSquared() > 0.0)
        {
          normal = vector2_2 - vector2_1;
          normal = Vector2Helpers.Normalized(normal);
        }
        Vector2 vector2_3 = vector2_1 + normal * radiusA;
        Vector2 vector2_4 = vector2_2 - normal * radiusB;
        points[0] = (vector2_3 + vector2_4) * 0.5f;
        break;
      case ManifoldType.FaceA:
        normal = Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in manifold.LocalNormal);
        Vector2 vector2_5 = Robust.Shared.Physics.Transform.Mul(in xfA, in manifold.LocalPoint);
        Span<ManifoldPoint> asSpan1 = manifold.Points.AsSpan;
        for (int index = 0; index < manifold.PointCount; ++index)
        {
          Vector2 vector2_6 = Robust.Shared.Physics.Transform.Mul(in xfB, in asSpan1[index].LocalPoint);
          Vector2 vector2_7 = vector2_6 + normal * (radiusA - Vector2.Dot(vector2_6 - vector2_5, normal));
          Vector2 vector2_8 = vector2_6 - normal * radiusB;
          points[index] = (vector2_7 + vector2_8) * 0.5f;
        }
        break;
      case ManifoldType.FaceB:
        normal = Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in manifold.LocalNormal);
        Vector2 vector2_9 = Robust.Shared.Physics.Transform.Mul(in xfB, in manifold.LocalPoint);
        Span<ManifoldPoint> asSpan2 = manifold.Points.AsSpan;
        for (int index = 0; index < manifold.PointCount; ++index)
        {
          Vector2 vector2_10 = Robust.Shared.Physics.Transform.Mul(in xfA, in asSpan2[index].LocalPoint);
          Vector2 vector2_11 = vector2_10 + normal * (radiusB - Vector2.Dot(vector2_10 - vector2_9, normal));
          Vector2 vector2_12 = vector2_10 - normal * radiusA;
          points[index] = (vector2_12 + vector2_11) * 0.5f;
        }
        normal = -normal;
        break;
      default:
        throw new InvalidOperationException();
    }
  }

  private static void PositionSolverManifoldInitialize(
    in ContactPositionConstraint pc,
    int index,
    in Robust.Shared.Physics.Transform xfA,
    in Robust.Shared.Physics.Transform xfB,
    out Vector2 normal,
    out Vector2 point,
    out float separation)
  {
    switch (pc.Type)
    {
      case ManifoldType.Circles:
        Vector2 vector2_1 = Robust.Shared.Physics.Transform.Mul(in xfA, in pc.LocalPoint);
        Vector2 vector2_2 = Robust.Shared.Physics.Transform.Mul(in xfB, in pc.LocalPoints._00);
        normal = vector2_2 - vector2_1;
        if (normal != Vector2.Zero)
          normal = Vector2Helpers.Normalized(normal);
        point = (vector2_1 + vector2_2) * 0.5f;
        separation = Vector2.Dot(vector2_2 - vector2_1, normal) - pc.RadiusA - pc.RadiusB;
        break;
      case ManifoldType.FaceA:
        Span<Vector2> asSpan1 = pc.LocalPoints.AsSpan;
        normal = Robust.Shared.Physics.Transform.Mul(in xfA.Quaternion2D, in pc.LocalNormal);
        Vector2 vector2_3 = Robust.Shared.Physics.Transform.Mul(in xfA, in pc.LocalPoint);
        Vector2 vector2_4 = Robust.Shared.Physics.Transform.Mul(in xfB, in asSpan1[index]);
        separation = Vector2.Dot(vector2_4 - vector2_3, normal) - pc.RadiusA - pc.RadiusB;
        point = vector2_4;
        break;
      case ManifoldType.FaceB:
        Span<Vector2> asSpan2 = pc.LocalPoints.AsSpan;
        normal = Robust.Shared.Physics.Transform.Mul(in xfB.Quaternion2D, in pc.LocalNormal);
        Vector2 vector2_5 = Robust.Shared.Physics.Transform.Mul(in xfB, in pc.LocalPoint);
        Vector2 vector2_6 = Robust.Shared.Physics.Transform.Mul(in xfA, in asSpan2[index]);
        separation = Vector2.Dot(vector2_6 - vector2_5, normal) - pc.RadiusA - pc.RadiusB;
        point = vector2_6;
        normal = -normal;
        break;
      default:
        normal = Vector2.Zero;
        point = Vector2.Zero;
        separation = 0.0f;
        break;
    }
  }

  public Vector2 GetLinearVelocity(
    EntityUid uid,
    Vector2 point,
    PhysicsComponent? component = null,
    TransformComponent? xform = null)
  {
    if (!this.PhysicsQuery.Resolve(uid, ref component) || !this.XformQuery.Resolve(uid, ref xform))
      return Vector2.Zero;
    Vector2 linearVelocity = component.LinearVelocity;
    Angle localRotation = xform.LocalRotation;
    ref Angle local1 = ref localRotation;
    double angularVelocity = (double) component.AngularVelocity;
    Vector2 vector2_1 = point - component.LocalCenter;
    ref Vector2 local2 = ref vector2_1;
    Vector2 vector2_2 = Vector2Helpers.Cross((float) angularVelocity, ref local2);
    ref Vector2 local3 = ref vector2_2;
    Vector2 vector2_3 = ((Angle) ref local1).RotateVec(ref local3);
    return linearVelocity + vector2_3;
  }

  public Vector2 GetMapLinearVelocity(EntityCoordinates coordinates)
  {
    if (!coordinates.IsValid((IEntityManager) this.EntityManager))
      return Vector2.Zero;
    EntityUid? map = this._transform.GetMap(coordinates);
    EntityUid uid = coordinates.EntityId;
    Vector2 vector2_1 = coordinates.Position;
    Vector2 zero = Vector2.Zero;
    Vector2 vector2_2 = Vector2.Zero;
    while (true)
    {
      EntityUid entityUid = uid;
      EntityUid? nullable = map;
      if ((nullable.HasValue ? (entityUid != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0 && uid.IsValid())
      {
        TransformComponent component1 = this.XformQuery.GetComponent(uid);
        PhysicsComponent component2;
        if (this.PhysicsQuery.TryGetComponent(uid, out component2))
        {
          zero += component2.LinearVelocity;
          Vector2 vector2_3 = vector2_2;
          double angularVelocity = (double) component2.AngularVelocity;
          Vector2 vector2_4 = vector2_1 - component2.LocalCenter;
          ref Vector2 local = ref vector2_4;
          Vector2 vector2_5 = Vector2Helpers.Cross((float) angularVelocity, ref local);
          vector2_2 = vector2_3 + vector2_5;
          Angle localRotation = component1.LocalRotation;
          vector2_2 = ((Angle) ref localRotation).RotateVec(ref vector2_2);
        }
        Vector2 localPosition = component1.LocalPosition;
        Angle localRotation1 = component1.LocalRotation;
        Vector2 vector2_6 = ((Angle) ref localRotation1).RotateVec(ref vector2_1);
        vector2_1 = localPosition + vector2_6;
        uid = component1.ParentUid;
      }
      else
        break;
    }
    return zero;
  }

  public Vector2 GetMapLinearVelocity(
    EntityUid uid,
    PhysicsComponent? component = null,
    TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return Vector2.Zero;
    this.PhysicsQuery.Resolve(uid, ref component, false);
    EntityUid parentUid = xform.ParentUid;
    Vector2 vector2_1 = xform.LocalPosition;
    Vector2 vector2_2 = component != null ? component.LinearVelocity : Vector2.Zero;
    Vector2 vector2_3 = Vector2.Zero;
    while (true)
    {
      EntityUid entityUid = parentUid;
      EntityUid? mapUid = xform.MapUid;
      if ((mapUid.HasValue ? (entityUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && parentUid.IsValid())
      {
        xform = this.XformQuery.GetComponent(parentUid);
        PhysicsComponent component1;
        if (this.PhysicsQuery.TryGetComponent(parentUid, out component1))
        {
          vector2_2 += component1.LinearVelocity;
          Vector2 vector2_4 = vector2_3;
          double angularVelocity = (double) component1.AngularVelocity;
          Vector2 vector2_5 = vector2_1 - component1.LocalCenter;
          ref Vector2 local = ref vector2_5;
          Vector2 vector2_6 = Vector2Helpers.Cross((float) angularVelocity, ref local);
          vector2_3 = vector2_4 + vector2_6;
          Angle localRotation = xform.LocalRotation;
          vector2_3 = ((Angle) ref localRotation).RotateVec(ref vector2_3);
        }
        Vector2 localPosition = xform.LocalPosition;
        Angle localRotation1 = xform.LocalRotation;
        Vector2 vector2_7 = ((Angle) ref localRotation1).RotateVec(ref vector2_1);
        vector2_1 = localPosition + vector2_7;
        parentUid = xform.ParentUid;
      }
      else
        break;
    }
    return vector2_2 + vector2_3;
  }

  public float GetMapAngularVelocity(
    EntityUid uid,
    PhysicsComponent? component = null,
    TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return 0.0f;
    this.PhysicsQuery.Resolve(uid, ref component, false);
    float mapAngularVelocity = component != null ? component.AngularVelocity : 0.0f;
    while (true)
    {
      EntityUid parentUid = xform.ParentUid;
      EntityUid? mapUid = xform.MapUid;
      if ((mapUid.HasValue ? (parentUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && xform.ParentUid.IsValid())
      {
        PhysicsComponent component1;
        if (this.PhysicsQuery.TryGetComponent(xform.ParentUid, out component1))
          mapAngularVelocity += component1.AngularVelocity;
        xform = this.XformQuery.GetComponent(xform.ParentUid);
      }
      else
        break;
    }
    return mapAngularVelocity;
  }

  public (Vector2, float) GetMapVelocities(
    EntityUid uid,
    PhysicsComponent? component = null,
    TransformComponent? xform = null)
  {
    if (!this.XformQuery.Resolve(uid, ref xform))
      return (Vector2.Zero, 0.0f);
    this.PhysicsQuery.Resolve(uid, ref component, false);
    EntityUid parentUid = xform.ParentUid;
    Vector2 vector2_1 = xform.LocalPosition;
    Vector2 vector2_2 = component != null ? component.LinearVelocity : Vector2.Zero;
    float num = component != null ? component.AngularVelocity : 0.0f;
    Vector2 vector2_3 = Vector2.Zero;
    while (true)
    {
      EntityUid entityUid = parentUid;
      EntityUid? mapUid = xform.MapUid;
      if ((mapUid.HasValue ? (entityUid != mapUid.GetValueOrDefault() ? 1 : 0) : 1) != 0 && parentUid.IsValid())
      {
        xform = this.XformQuery.GetComponent(parentUid);
        PhysicsComponent component1;
        Angle localRotation;
        if (this.PhysicsQuery.TryGetComponent(parentUid, out component1))
        {
          num += component1.AngularVelocity;
          vector2_2 += component1.LinearVelocity;
          Vector2 vector2_4 = vector2_3;
          double angularVelocity = (double) component1.AngularVelocity;
          Vector2 vector2_5 = vector2_1 - component1.LocalCenter;
          ref Vector2 local = ref vector2_5;
          Vector2 vector2_6 = Vector2Helpers.Cross((float) angularVelocity, ref local);
          vector2_3 = vector2_4 + vector2_6;
          localRotation = xform.LocalRotation;
          vector2_3 = ((Angle) ref localRotation).RotateVec(ref vector2_3);
        }
        Vector2 localPosition = xform.LocalPosition;
        localRotation = xform.LocalRotation;
        Vector2 vector2_7 = ((Angle) ref localRotation).RotateVec(ref vector2_1);
        vector2_1 = localPosition + vector2_7;
        parentUid = xform.ParentUid;
      }
      else
        break;
    }
    return (vector2_2 + vector2_3, num);
  }

  private void HandleParentChangeVelocity(
    EntityUid uid,
    PhysicsComponent physics,
    EntityUid oldParent,
    TransformComponent xform)
  {
    if (this._gameTiming.ApplyingState || physics.LifeStage != ComponentLifeStage.Running || physics.BodyType == BodyType.Static || xform.MapID == MapId.Nullspace || !physics.CanCollide)
      return;
    FixturesComponent manager = (FixturesComponent) null;
    (Vector2 vector2_1, float num) = this.GetMapVelocities(uid, physics, xform);
    if (oldParent == EntityUid.Invalid)
    {
      this.SetLinearVelocity(uid, physics.LinearVelocity * 2f - vector2_1, manager: manager, body: physics);
      this.SetAngularVelocity(uid, physics.AngularVelocity * 2f - num, manager: manager, body: physics);
    }
    else
    {
      EntityUid uid1 = oldParent;
      TransformComponent component1 = this.XformQuery.GetComponent(uid1);
      Vector2 vector2_2 = Vector2.Transform(this._transform.GetWorldPosition(xform), this._transform.GetInvWorldMatrix(component1));
      Vector2 linearVelocity = physics.LinearVelocity;
      float angularVelocity1 = physics.AngularVelocity;
      Vector2 vector2_3 = Vector2.Zero;
      do
      {
        PhysicsComponent component2;
        Angle localRotation;
        if (this.PhysicsQuery.TryGetComponent(uid1, out component2))
        {
          angularVelocity1 += component2.AngularVelocity;
          linearVelocity += component2.LinearVelocity;
          Vector2 vector2_4 = vector2_3;
          double angularVelocity2 = (double) component2.AngularVelocity;
          Vector2 vector2_5 = vector2_2 - component2.LocalCenter;
          ref Vector2 local = ref vector2_5;
          Vector2 vector2_6 = Vector2Helpers.Cross((float) angularVelocity2, ref local);
          vector2_3 = vector2_4 + vector2_6;
          localRotation = component1.LocalRotation;
          vector2_3 = ((Angle) ref localRotation).RotateVec(ref vector2_3);
        }
        Vector2 localPosition = component1.LocalPosition;
        localRotation = component1.LocalRotation;
        Vector2 vector2_7 = ((Angle) ref localRotation).RotateVec(ref vector2_2);
        vector2_2 = localPosition + vector2_7;
        uid1 = component1.ParentUid;
      }
      while (uid1.IsValid() && this.XformQuery.TryGetComponent(uid1, out component1));
      Vector2 vector2_8 = linearVelocity + vector2_3;
      this.SetLinearVelocity(uid, physics.LinearVelocity + vector2_8 - vector2_1, manager: manager, body: physics);
      this.SetAngularVelocity(uid, physics.AngularVelocity + angularVelocity1 - num, manager: manager, body: physics);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 Gravity { get; private set; }

  public void SetGravity(Vector2 value)
  {
    if (this.Gravity.Equals(value))
      return;
    this.Gravity = value;
  }

  static SharedPhysicsSystem()
  {
    HistogramConfiguration histogramConfiguration1 = new HistogramConfiguration();
    ((MetricConfiguration) histogramConfiguration1).LabelNames = new string[1]
    {
      "controller"
    };
    histogramConfiguration1.Buckets = Histogram.ExponentialBuckets(1E-06, 1.5, 25);
    SharedPhysicsSystem.TickUsageControllerBeforeSolveHistogram = Metrics.CreateHistogram("robust_entity_physics_controller_before_solve", "Amount of time spent running a controller's UpdateBeforeSolve", histogramConfiguration1);
    HistogramConfiguration histogramConfiguration2 = new HistogramConfiguration();
    ((MetricConfiguration) histogramConfiguration2).LabelNames = new string[1]
    {
      "controller"
    };
    histogramConfiguration2.Buckets = Histogram.ExponentialBuckets(1E-06, 1.5, 25);
    SharedPhysicsSystem.TickUsageControllerAfterSolveHistogram = Metrics.CreateHistogram("robust_entity_physics_controller_after_solve", "Amount of time spent running a controller's UpdateAfterSolve", histogramConfiguration2);
  }

  private sealed class ContactPoolPolicy : IPooledObjectPolicy<Contact>
  {
    private readonly SharedDebugPhysicsSystem _debugPhysicsSystem;
    private readonly IManifoldManager _manifoldManager;

    public ContactPoolPolicy(
      SharedDebugPhysicsSystem debugPhysicsSystem,
      IManifoldManager manifoldManager)
    {
      this._debugPhysicsSystem = debugPhysicsSystem;
      this._manifoldManager = manifoldManager;
    }

    public Contact Create()
    {
      return new Contact(this._manifoldManager)
      {
        Manifold = new Manifold()
      };
    }

    public bool Return(Contact obj)
    {
      SharedPhysicsSystem.SetContact(obj, false, new Entity<PhysicsComponent, TransformComponent>(EntityUid.Invalid, (PhysicsComponent) null, (TransformComponent) null), new Entity<PhysicsComponent, TransformComponent>(EntityUid.Invalid, (PhysicsComponent) null, (TransformComponent) null), string.Empty, string.Empty, (Fixture) null, 0, (Fixture) null, 0);
      return true;
    }
  }

  private record struct ManifoldsJob : IParallelRobustJob, IParallelRangeRobustJob
  {
    public SharedPhysicsSystem Physics;
    public Contact[] Contacts;
    public ContactStatus[] Status;
    public FixedArray4<Vector2>[] WorldPoints;
    public bool[] Wake;

    public int BatchSize => 32 /*0x20*/;

    public void Execute(int index)
    {
      this.Physics.UpdateContact(this.Contacts, index, this.Status, this.Wake, this.WorldPoints);
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (((EqualityComparer<SharedPhysicsSystem>.Default.GetHashCode(this.Physics) * -1521134295 + EqualityComparer<Contact[]>.Default.GetHashCode(this.Contacts)) * -1521134295 + EqualityComparer<ContactStatus[]>.Default.GetHashCode(this.Status)) * -1521134295 + EqualityComparer<FixedArray4<Vector2>[]>.Default.GetHashCode(this.WorldPoints)) * -1521134295 + EqualityComparer<bool[]>.Default.GetHashCode(this.Wake);
    }

    [CompilerGenerated]
    public readonly bool Equals(SharedPhysicsSystem.ManifoldsJob other)
    {
      return EqualityComparer<SharedPhysicsSystem>.Default.Equals(this.Physics, other.Physics) && EqualityComparer<Contact[]>.Default.Equals(this.Contacts, other.Contacts) && EqualityComparer<ContactStatus[]>.Default.Equals(this.Status, other.Status) && EqualityComparer<FixedArray4<Vector2>[]>.Default.Equals(this.WorldPoints, other.WorldPoints) && EqualityComparer<bool[]>.Default.Equals(this.Wake, other.Wake);
    }
  }

  internal record struct IslandData(
    int Index,
    bool LoneIsland,
    List<Entity<PhysicsComponent, TransformComponent>> Bodies,
    List<Contact> Contacts,
    List<(Joint Original, Joint Joint)> Joints,
    List<(Joint Joint, float Error)> BrokenJoints)
  {
    public EntityUid MapUid = new EntityUid();
    public readonly int Index = Index;
    public readonly bool LoneIsland = LoneIsland;
    public int Offset = 0;
    public readonly List<Entity<PhysicsComponent, TransformComponent>> Bodies = Bodies;
    public readonly List<Contact> Contacts = Contacts;
    public readonly List<(Joint Original, Joint Joint)> Joints = Joints;
    public bool PositionSolved = false;
    public readonly List<(Joint Joint, float Error)> BrokenJoints = BrokenJoints;

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (((((((EqualityComparer<EntityUid>.Default.GetHashCode(this.MapUid) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Index)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.LoneIsland)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Offset)) * -1521134295 + EqualityComparer<List<Entity<PhysicsComponent, TransformComponent>>>.Default.GetHashCode(this.Bodies)) * -1521134295 + EqualityComparer<List<Contact>>.Default.GetHashCode(this.Contacts)) * -1521134295 + EqualityComparer<List<(Joint, Joint)>>.Default.GetHashCode(this.Joints)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.PositionSolved)) * -1521134295 + EqualityComparer<List<(Joint, float)>>.Default.GetHashCode(this.BrokenJoints);
    }

    [CompilerGenerated]
    public readonly bool Equals(SharedPhysicsSystem.IslandData other)
    {
      return EqualityComparer<EntityUid>.Default.Equals(this.MapUid, other.MapUid) && EqualityComparer<int>.Default.Equals(this.Index, other.Index) && EqualityComparer<bool>.Default.Equals(this.LoneIsland, other.LoneIsland) && EqualityComparer<int>.Default.Equals(this.Offset, other.Offset) && EqualityComparer<List<Entity<PhysicsComponent, TransformComponent>>>.Default.Equals(this.Bodies, other.Bodies) && EqualityComparer<List<Contact>>.Default.Equals(this.Contacts, other.Contacts) && EqualityComparer<List<(Joint, Joint)>>.Default.Equals(this.Joints, other.Joints) && EqualityComparer<bool>.Default.Equals(this.PositionSolved, other.PositionSolved) && EqualityComparer<List<(Joint, float)>>.Default.Equals(this.BrokenJoints, other.BrokenJoints);
    }

    [CompilerGenerated]
    public readonly void Deconstruct(
      out int Index,
      out bool LoneIsland,
      out List<Entity<PhysicsComponent, TransformComponent>> Bodies,
      out List<Contact> Contacts,
      out List<(Joint Original, Joint Joint)> Joints,
      out List<(Joint Joint, float Error)> BrokenJoints)
    {
      Index = this.Index;
      LoneIsland = this.LoneIsland;
      Bodies = this.Bodies;
      Contacts = this.Contacts;
      Joints = this.Joints;
      BrokenJoints = this.BrokenJoints;
    }
  }
}
