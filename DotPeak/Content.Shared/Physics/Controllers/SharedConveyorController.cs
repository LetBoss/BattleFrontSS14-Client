// Decompiled with JetBrains decompiler
// Type: Content.Shared.Physics.Controllers.SharedConveyorController
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Conveyor;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Threading;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Physics.Controllers;

public abstract class SharedConveyorController : VirtualController
{
  [Robust.Shared.IoC.Dependency]
  protected IMapManager MapManager;
  [Robust.Shared.IoC.Dependency]
  private IParallelManager _parallel;
  [Robust.Shared.IoC.Dependency]
  private CollisionWakeSystem _wake;
  [Robust.Shared.IoC.Dependency]
  protected EntityLookupSystem Lookup;
  [Robust.Shared.IoC.Dependency]
  private FixtureSystem _fixtures;
  [Robust.Shared.IoC.Dependency]
  private SharedGravitySystem _gravity;
  [Robust.Shared.IoC.Dependency]
  private SharedMoverController _mover;
  protected const string ConveyorFixture = "conveyor";
  private SharedConveyorController.ConveyorJob _job;
  private Robust.Shared.GameObjects.EntityQuery<ConveyorComponent> _conveyorQuery;
  private Robust.Shared.GameObjects.EntityQuery<ConveyedComponent> _conveyedQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> PhysicsQuery;
  protected Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;
  protected HashSet<EntityUid> Intersecting = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this._job = new SharedConveyorController.ConveyorJob(this);
    this._conveyorQuery = this.GetEntityQuery<ConveyorComponent>();
    this._conveyedQuery = this.GetEntityQuery<ConveyedComponent>();
    this.PhysicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.XformQuery = this.GetEntityQuery<TransformComponent>();
    this.UpdatesAfter.Add(typeof (SharedMoverController));
    this.SubscribeLocalEvent<ConveyedComponent, TileFrictionEvent>(new EntityEventRefHandler<ConveyedComponent, TileFrictionEvent>(this.OnConveyedFriction));
    this.SubscribeLocalEvent<ConveyedComponent, ComponentStartup>(new EntityEventRefHandler<ConveyedComponent, ComponentStartup>(this.OnConveyedStartup));
    this.SubscribeLocalEvent<ConveyedComponent, ComponentShutdown>(new EntityEventRefHandler<ConveyedComponent, ComponentShutdown>(this.OnConveyedShutdown));
    this.SubscribeLocalEvent<ConveyorComponent, StartCollideEvent>(new EntityEventRefHandler<ConveyorComponent, StartCollideEvent>(this.OnConveyorStartCollide));
    this.SubscribeLocalEvent<ConveyorComponent, ComponentStartup>(new EntityEventRefHandler<ConveyorComponent, ComponentStartup>(this.OnConveyorStartup));
    base.Initialize();
  }

  private void OnConveyedFriction(Entity<ConveyedComponent> ent, ref TileFrictionEvent args)
  {
    args.Modifier = 0.0f;
  }

  private void OnConveyedStartup(Entity<ConveyedComponent> ent, ref ComponentStartup args)
  {
    this._wake.SetEnabled(ent.Owner, false);
  }

  private void OnConveyedShutdown(Entity<ConveyedComponent> ent, ref ComponentShutdown args)
  {
    this._wake.SetEnabled(ent.Owner, true);
  }

  private void OnConveyorStartup(Entity<ConveyorComponent> ent, ref ComponentStartup args)
  {
    this.AwakenConveyor((Entity<TransformComponent>) ent.Owner);
  }

  protected virtual void AwakenConveyor(Entity<TransformComponent?> ent)
  {
  }

  protected void WakeConveyed(EntityUid conveyorUid)
  {
    ContactEnumerator contacts = this.PhysicsSystem.GetContacts((Entity<FixturesComponent>) conveyorUid);
    Contact contact;
    while (contacts.MoveNext(out contact))
    {
      EntityUid uid = contact.OtherEnt(conveyorUid);
      if (contact.OtherFixture(conveyorUid).Item2.Hard && contact.OtherBody(conveyorUid).BodyType != BodyType.Static)
        this.EnsureComp<ConveyedComponent>(uid);
      if (this._conveyedQuery.HasComp(uid))
        this.PhysicsSystem.WakeBody(uid);
    }
  }

  private void OnConveyorStartCollide(
    Entity<ConveyorComponent> conveyor,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (!args.OtherFixture.Hard || args.OtherBody.BodyType == BodyType.Static)
      return;
    this.EnsureComp<ConveyedComponent>(otherEntity);
  }

  public override void UpdateBeforeSolve(bool prediction, float frameTime)
  {
    base.UpdateBeforeSolve(prediction, frameTime);
    this._job.Prediction = prediction;
    this._job.Conveyed.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>();
    EntityUid uid;
    ConveyedComponent comp1;
    FixturesComponent comp2;
    PhysicsComponent comp3;
    TransformComponent comp4;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3, out comp4))
      this._job.Conveyed.Add(((Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>) (uid, comp1, comp2, comp3, comp4), Vector2.Zero, false));
    this._parallel.ProcessNow((IParallelRobustJob) this._job, this._job.Conveyed.Count);
    foreach ((Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> Entity, Vector2 Direction, bool Result) tuple in this._job.Conveyed)
    {
      if (!(!tuple.Entity.Comp3.Predict & prediction))
      {
        Vector2 linearVelocity = tuple.Entity.Comp3.LinearVelocity;
        Vector2 velocity = tuple.Direction;
        Vector2 wishDir = this._mover.GetWishDir((Entity<InputMoverComponent>) tuple.Entity.Owner);
        if ((double) Vector2.Dot(wishDir, velocity) > 0.0)
          velocity += wishDir;
        if (tuple.Result)
        {
          this.SetConveying(tuple.Entity.Owner, tuple.Entity.Comp1, (double) velocity.LengthSquared() > 0.0);
          bool flag;
          if (!this._mover.UsedMobMovement.TryGetValue(tuple.Entity.Owner, out flag) || !flag)
            this._mover.Friction(0.2f, frameTime, 5f, ref linearVelocity);
          SharedMoverController.Accelerate(ref linearVelocity, in velocity, 20f, frameTime);
        }
        else
        {
          bool flag;
          if (!this._mover.UsedMobMovement.TryGetValue(tuple.Entity.Owner, out flag) || !flag)
            this._mover.Friction(0.0f, frameTime, 40f, ref linearVelocity);
        }
        this.PhysicsSystem.SetLinearVelocity(tuple.Entity.Owner, linearVelocity, wakeBody: false);
        if (!this.IsConveyed((Entity<FixturesComponent>) (tuple.Entity.Owner, tuple.Entity.Comp2)))
          this.RemComp<ConveyedComponent>(tuple.Entity.Owner);
      }
    }
  }

  private void SetConveying(EntityUid uid, ConveyedComponent conveyed, bool value)
  {
    if (conveyed.Conveying == value)
      return;
    conveyed.Conveying = value;
    this.Dirty(uid, (IComponent) conveyed);
  }

  private bool TryConvey(
    Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> entity,
    bool prediction,
    out Vector2 direction)
  {
    direction = Vector2.Zero;
    FixturesComponent comp2 = entity.Comp2;
    PhysicsComponent comp3 = entity.Comp3;
    TransformComponent comp4 = entity.Comp4;
    if (!comp3.Awake || !comp3.Predict & prediction || !comp4.GridUid.HasValue || comp3.BodyStatus == BodyStatus.InAir || this._gravity.IsWeightless((EntityUid) entity, comp3, comp4))
      return true;
    Entity<ConveyorComponent> entity1 = new Entity<ConveyorComponent>();
    float speed = 0.0f;
    ContactEnumerator contacts1 = this.PhysicsSystem.GetContacts((Entity<FixturesComponent>) (entity.Owner, comp2));
    Robust.Shared.Physics.Transform physicsTransform = this.PhysicsSystem.GetPhysicsTransform(entity.Owner);
    bool flag = false;
    Contact contact1;
    while (contacts1.MoveNext(out contact1))
    {
      if (contact1.IsTouching)
      {
        EntityUid uid = contact1.OtherEnt(entity.Owner);
        ConveyorComponent component;
        if (this._conveyorQuery.TryComp(uid, out component))
        {
          flag = true;
          if (this._fixtures.TestPoint<IPhysShape>(contact1.OtherFixture(entity.Owner).Item2.Shape, this.PhysicsSystem.GetPhysicsTransform(uid), physicsTransform.Position) && (double) component.Speed > (double) speed && this.CanRun(component))
          {
            speed = component.Speed;
            entity1 = (Entity<ConveyorComponent>) (uid, component);
          }
        }
      }
    }
    if (!flag || (double) speed == 0.0 || entity1 == new Entity<ConveyorComponent>())
      return true;
    ConveyorComponent comp = entity1.Comp;
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this.TransformSystem.GetWorldPositionRotation(this.XformQuery.GetComponent(entity1.Owner));
    Vector2 worldPosition = positionRotation.WorldPosition;
    Angle angle = Angle.op_Addition(positionRotation.WorldRotation, entity1.Comp.Angle);
    if (comp.State == ConveyorState.Reverse)
      angle = Angle.op_Addition(angle, Angle.op_Implicit(3.14159274f));
    Vector2 worldVec = ((Angle) ref angle).ToWorldVec();
    direction = worldVec;
    Vector2 itemRelative = worldPosition - physicsTransform.Position;
    direction = SharedConveyorController.Convey(direction, speed, itemRelative);
    ContactEnumerator contacts2 = this.PhysicsSystem.GetContacts((Entity<FixturesComponent>) (entity.Owner, comp2));
    Contact contact2;
    while (contacts2.MoveNext(out contact2))
    {
      if (contact2.Hard && contact2.IsTouching)
      {
        EntityUid uid = contact2.OtherEnt(entity.Owner);
        if (contact2.OtherBody(entity.Owner).BodyType == BodyType.Static && (double) Vector2.Dot(this.PhysicsSystem.GetPhysicsTransform(uid).Position - physicsTransform.Position, direction) > 1.5)
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
    if ((double) speed == 0.0 || (double) direction.LengthSquared() == 0.0)
      return Vector2.Zero;
    Vector2 vector2_1 = direction * (Vector2.Dot(itemRelative, direction) / Vector2.Dot(direction, direction));
    Vector2 vector2_2 = itemRelative - vector2_1;
    return (double) vector2_2.Length() < 0.01 ? direction * speed : Vector2Helpers.Normalized(vector2_2 + direction * 0.2f) * speed;
  }

  public bool CanRun(ConveyorComponent component)
  {
    return component.State != ConveyorState.Off && component.Powered;
  }

  private bool IsConveyed(Entity<FixturesComponent?> ent)
  {
    if (!this.Resolve<FixturesComponent>(ent.Owner, ref ent.Comp))
      return false;
    ContactEnumerator contacts = this.PhysicsSystem.GetContacts((Entity<FixturesComponent>) ent.Owner);
    Contact contact;
    while (contacts.MoveNext(out contact))
    {
      ConveyorComponent component;
      if (contact.IsTouching && this._conveyorQuery.TryComp(contact.OtherEnt(ent.Owner), out component) && this.CanRun(component))
        return true;
    }
    return false;
  }

  private record struct ConveyorJob(SharedConveyorController controller) : 
    IParallelRobustJob,
    IParallelRangeRobustJob
  {
    public List<(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> Entity, Vector2 Direction, bool Result)> Conveyed = new List<(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>, Vector2, bool)>();
    public SharedConveyorController System = controller;
    public bool Prediction = false;

    public int BatchSize => 16 /*0x10*/;

    public void Execute(int index)
    {
      (Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent> Entity, Vector2 Direction, bool Result) tuple = this.Conveyed[index];
      Vector2 direction;
      bool flag = this.System.TryConvey((Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>) (tuple.Entity.Owner, tuple.Entity.Comp1, tuple.Entity.Comp2, tuple.Entity.Comp3, tuple.Entity.Comp4), this.Prediction, out direction);
      this.Conveyed[index] = (tuple.Entity, direction, flag);
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (EqualityComparer<List<(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>, Vector2, bool)>>.Default.GetHashCode(this.Conveyed) * -1521134295 + EqualityComparer<SharedConveyorController>.Default.GetHashCode(this.System)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Prediction);
    }

    [CompilerGenerated]
    public readonly bool Equals(SharedConveyorController.ConveyorJob other)
    {
      return EqualityComparer<List<(Entity<ConveyedComponent, FixturesComponent, PhysicsComponent, TransformComponent>, Vector2, bool)>>.Default.Equals(this.Conveyed, other.Conveyed) && EqualityComparer<SharedConveyorController>.Default.Equals(this.System, other.System) && EqualityComparer<bool>.Default.Equals(this.Prediction, other.Prediction);
    }
  }
}
