// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.Contacts.Contact
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Dynamics.Contacts;

public sealed class Contact : IEquatable<Contact>
{
  private readonly IManifoldManager _manifoldManager;
  public readonly LinkedListNode<Contact> MapNode;
  public readonly LinkedListNode<Contact> BodyANode;
  public readonly LinkedListNode<Contact> BodyBNode;
  public EntityUid EntityA;
  public EntityUid EntityB;
  public string FixtureAId = string.Empty;
  public string FixtureBId = string.Empty;
  public Fixture? FixtureA;
  public Fixture? FixtureB;
  public PhysicsComponent? BodyA;
  public PhysicsComponent? BodyB;
  public TransformComponent? XformA;
  public TransformComponent? XformB;
  public Manifold Manifold;
  internal Contact.ContactType Type;
  internal ContactFlags Flags;

  internal Contact(IManifoldManager manifoldManager)
  {
    this._manifoldManager = manifoldManager;
    this.MapNode = new LinkedListNode<Contact>(this);
    this.BodyANode = new LinkedListNode<Contact>(this);
    this.BodyBNode = new LinkedListNode<Contact>(this);
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsTouching { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Enabled { get; set; }

  public int ChildIndexA { get; internal set; }

  public int ChildIndexB { get; internal set; }

  public float Friction { get; set; }

  public float Restitution { get; set; }

  public float TangentSpeed { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Deleting => (this.Flags & ContactFlags.Deleting) == ContactFlags.Deleting;

  public bool Hard
  {
    get
    {
      return this.FixtureA != null && this.FixtureB != null && this.FixtureA.Hard && this.FixtureB.Hard;
    }
  }

  public void ResetRestitution()
  {
    Fixture fixtureA = this.FixtureA;
    double x = fixtureA != null ? (double) fixtureA.Restitution : 0.0;
    Fixture fixtureB = this.FixtureB;
    double y = fixtureB != null ? (double) fixtureB.Restitution : 0.0;
    this.Restitution = MathF.Max((float) x, (float) y);
  }

  public void ResetFriction()
  {
    Fixture fixtureA = this.FixtureA;
    double num1 = fixtureA != null ? (double) fixtureA.Friction : 0.0;
    Fixture fixtureB = this.FixtureB;
    double num2 = fixtureB != null ? (double) fixtureB.Friction : 0.0;
    this.Friction = MathF.Sqrt((float) (num1 * num2));
  }

  public unsafe void GetWorldManifold(
    Transform transformA,
    Transform transformB,
    out Vector2 normal)
  {
    IPhysShape shape1 = this.FixtureA?.Shape;
    IPhysShape shape2 = this.FixtureB?.Shape;
    ; // Unable to render the statement
    Span<Vector2> points = new Span<Vector2>((void*) pointer, 8);
    SharedPhysicsSystem.InitializeManifold(ref this.Manifold, in transformA, in transformB, shape1.Radius, shape2.Radius, out normal, points);
  }

  public void GetWorldManifold(
    Transform transformA,
    Transform transformB,
    out Vector2 normal,
    Span<Vector2> points)
  {
    IPhysShape shape1 = this.FixtureA?.Shape;
    IPhysShape shape2 = this.FixtureB?.Shape;
    SharedPhysicsSystem.InitializeManifold(ref this.Manifold, in transformA, in transformB, shape1.Radius, shape2.Radius, out normal, points);
  }

  internal ContactStatus Update(Transform bodyATransform, Transform bodyBTransform, out bool wake)
  {
    Manifold manifold = this.Manifold;
    this.Enabled = true;
    bool isTouching = this.IsTouching;
    wake = false;
    bool flag;
    if ((!this.FixtureA.Hard ? 1 : (!this.FixtureB.Hard ? 1 : 0)) != 0)
    {
      flag = this._manifoldManager.TestOverlap<IPhysShape, IPhysShape>(this.FixtureA.Shape, this.ChildIndexA, this.FixtureB.Shape, this.ChildIndexB, in bodyATransform, in bodyBTransform);
      this.Manifold.PointCount = 0;
    }
    else
    {
      this.Evaluate(ref this.Manifold, in bodyATransform, in bodyBTransform);
      flag = this.Manifold.PointCount > 0;
      Span<ManifoldPoint> asSpan1 = this.Manifold.Points.AsSpan;
      Span<ManifoldPoint> asSpan2 = manifold.Points.AsSpan;
      for (int index1 = 0; index1 < this.Manifold.PointCount; ++index1)
      {
        ManifoldPoint manifoldPoint1 = asSpan1[index1] with
        {
          NormalImpulse = 0.0f,
          TangentImpulse = 0.0f
        };
        ContactID id = manifoldPoint1.Id;
        for (int index2 = 0; index2 < manifold.PointCount; ++index2)
        {
          ManifoldPoint manifoldPoint2 = asSpan2[index2];
          if ((int) manifoldPoint2.Id.Key == (int) id.Key)
          {
            manifoldPoint1.NormalImpulse = manifoldPoint2.NormalImpulse;
            manifoldPoint1.TangentImpulse = manifoldPoint2.TangentImpulse;
            break;
          }
        }
        asSpan1[index1] = manifoldPoint1;
      }
      if (flag != isTouching)
        wake = true;
    }
    this.IsTouching = flag;
    return Contact.GetContactStatus(this, isTouching);
  }

  internal static ContactStatus GetContactStatus(Contact contact, bool wasTouching)
  {
    return !wasTouching ? (contact.IsTouching ? ContactStatus.StartTouching : ContactStatus.NoContact) : (!contact.IsTouching ? ContactStatus.EndTouching : ContactStatus.Touching);
  }

  internal void UpdateIsTouching(Transform bodyATransform, Transform bodyBTransform)
  {
    if ((!this.FixtureA.Hard ? 1 : (!this.FixtureB.Hard ? 1 : 0)) != 0)
    {
      this.IsTouching = this._manifoldManager.TestOverlap<IPhysShape, IPhysShape>(this.FixtureA.Shape, this.ChildIndexA, this.FixtureB.Shape, this.ChildIndexB, in bodyATransform, in bodyBTransform);
    }
    else
    {
      Manifold manifold = this.Manifold;
      this.Evaluate(ref manifold, in bodyATransform, in bodyBTransform);
      this.IsTouching = manifold.PointCount > 0;
    }
  }

  private void Evaluate(ref Manifold manifold, in Transform transformA, in Transform transformB)
  {
    switch (this.Type)
    {
      case Contact.ContactType.Polygon:
        this._manifoldManager.CollidePolygons(ref manifold, (PolygonShape) this.FixtureA.Shape, in transformA, (PolygonShape) this.FixtureB.Shape, in transformB);
        break;
      case Contact.ContactType.PolygonAndCircle:
        this._manifoldManager.CollidePolygonAndCircle(ref manifold, (PolygonShape) this.FixtureA.Shape, in transformA, (PhysShapeCircle) this.FixtureB.Shape, in transformB);
        break;
      case Contact.ContactType.Circle:
        this._manifoldManager.CollideCircles(ref manifold, (PhysShapeCircle) this.FixtureA.Shape, in transformA, (PhysShapeCircle) this.FixtureB.Shape, in transformB);
        break;
      case Contact.ContactType.EdgeAndPolygon:
        this._manifoldManager.CollideEdgeAndPolygon(ref manifold, (EdgeShape) this.FixtureA.Shape, in transformA, (PolygonShape) this.FixtureB.Shape, in transformB);
        break;
      case Contact.ContactType.EdgeAndCircle:
        this._manifoldManager.CollideEdgeAndCircle(ref manifold, (EdgeShape) this.FixtureA.Shape, in transformA, (PhysShapeCircle) this.FixtureB.Shape, in transformB);
        break;
      case Contact.ContactType.ChainAndPolygon:
        ChainShape shape1 = (ChainShape) this.FixtureA.Shape;
        EdgeShape contactEdge1 = this._manifoldManager.GetContactEdge();
        ref EdgeShape local1 = ref contactEdge1;
        int childIndexA1 = this.ChildIndexA;
        shape1.GetChildEdge(ref local1, childIndexA1);
        this._manifoldManager.CollideEdgeAndPolygon(ref manifold, contactEdge1, in transformA, (PolygonShape) this.FixtureB.Shape, in transformB);
        this._manifoldManager.ReturnEdge(contactEdge1);
        break;
      case Contact.ContactType.ChainAndCircle:
        ChainShape shape2 = (ChainShape) this.FixtureA.Shape;
        EdgeShape contactEdge2 = this._manifoldManager.GetContactEdge();
        ref EdgeShape local2 = ref contactEdge2;
        int childIndexA2 = this.ChildIndexA;
        shape2.GetChildEdge(ref local2, childIndexA2);
        this._manifoldManager.CollideEdgeAndCircle(ref manifold, contactEdge2, in transformA, (PhysShapeCircle) this.FixtureB.Shape, in transformB);
        this._manifoldManager.ReturnEdge(contactEdge2);
        break;
      default:
        throw new ArgumentOutOfRangeException($"Collision between {this.FixtureA.Shape.GetType()} and {this.FixtureB.Shape.GetType()} not supported");
    }
  }

  public bool Equals(Contact? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    if (object.Equals((object) this.FixtureA, (object) other.FixtureA) && object.Equals((object) this.FixtureB, (object) other.FixtureB) && this.Manifold.Equals(other.Manifold) && this.Type == other.Type && this.Enabled == other.Enabled && this.ChildIndexA == other.ChildIndexA && this.ChildIndexB == other.ChildIndexB)
    {
      float num = this.Friction;
      if (num.Equals(other.Friction))
      {
        num = this.Restitution;
        return num.Equals(other.Restitution);
      }
    }
    return false;
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is Contact other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<EntityUid, EntityUid>(this.EntityA, this.EntityB);
  }

  public EntityUid OurEnt(EntityUid uid)
  {
    if (uid == this.EntityA)
      return this.EntityA;
    return uid == this.EntityB ? this.EntityB : throw new InvalidOperationException();
  }

  public EntityUid OtherEnt(EntityUid uid)
  {
    if (uid == this.EntityA)
      return this.EntityB;
    if (uid == this.EntityB)
      return this.EntityA;
    throw new InvalidOperationException();
  }

  public (string Id, Fixture) OurFixture(EntityUid uid)
  {
    if (uid == this.EntityA)
      return (this.FixtureAId, this.FixtureA);
    if (uid == this.EntityB)
      return (this.FixtureBId, this.FixtureB);
    throw new InvalidOperationException();
  }

  public (string Id, Fixture) OtherFixture(EntityUid uid)
  {
    if (uid == this.EntityA)
      return (this.FixtureBId, this.FixtureB);
    if (uid == this.EntityB)
      return (this.FixtureAId, this.FixtureA);
    throw new InvalidOperationException();
  }

  public PhysicsComponent OurBody(EntityUid uid)
  {
    if (uid == this.EntityA)
      return this.BodyA;
    if (uid == this.EntityB)
      return this.BodyB;
    throw new InvalidOperationException();
  }

  public PhysicsComponent OtherBody(EntityUid uid)
  {
    if (uid == this.EntityA)
      return this.BodyB;
    if (uid == this.EntityB)
      return this.BodyA;
    throw new InvalidOperationException();
  }

  public TransformComponent OtherTransform(EntityUid uid)
  {
    if (uid == this.EntityA)
      return this.XformB;
    if (uid == this.EntityB)
      return this.XformA;
    throw new InvalidOperationException();
  }

  public enum ContactType : byte
  {
    NotSupported,
    Polygon,
    PolygonAndCircle,
    Circle,
    EdgeAndPolygon,
    EdgeAndCircle,
    ChainAndPolygon,
    ChainAndCircle,
  }
}
