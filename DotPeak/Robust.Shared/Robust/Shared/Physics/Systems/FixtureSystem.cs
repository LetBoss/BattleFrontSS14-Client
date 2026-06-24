// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.FixtureSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public sealed class FixtureSystem : EntitySystem
{
  [Dependency]
  private readonly EntityLookupSystem _lookup;
  [Dependency]
  private readonly SharedBroadphaseSystem _broadphase;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixtureQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FixturesComponent, ComponentShutdown>(new ComponentEventHandler<FixturesComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<FixturesComponent, ComponentGetState>(new ComponentEventRefHandler<FixturesComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<FixturesComponent, ComponentHandleState>(new ComponentEventRefHandler<FixturesComponent, ComponentHandleState>(this.OnHandleState));
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._fixtureQuery = this.GetEntityQuery<FixturesComponent>();
  }

  private void OnShutdown(EntityUid uid, FixturesComponent component, ComponentShutdown args)
  {
    PhysicsComponent component1;
    if (!this._physicsQuery.TryGetComponent(uid, out component1))
      return;
    this._physics.DestroyContacts(component1);
  }

  public bool TryCreateFixture(
    EntityUid uid,
    IPhysShape shape,
    string id,
    float density = 1f,
    bool hard = true,
    int collisionLayer = 0,
    int collisionMask = 0,
    float friction = 0.4f,
    float restitution = 0.0f,
    bool updates = true,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this._physicsQuery.Resolve(uid, ref body) || !this._fixtureQuery.Resolve(uid, ref manager) || manager.Fixtures.ContainsKey(id))
      return false;
    Fixture fixture = new Fixture(shape, collisionLayer, collisionMask, hard, density, friction, restitution);
    this.CreateFixture(uid, id, fixture, updates, manager, body, xform);
    return true;
  }

  internal void CreateFixture(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    bool updates = true,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null,
    TransformComponent? xform = null)
  {
    if (!this._physicsQuery.Resolve(uid, ref body) || !this._fixtureQuery.Resolve(uid, ref manager))
      return;
    if (string.IsNullOrEmpty(fixtureId))
      throw new InvalidOperationException("Tried to create a fixture without an ID!");
    manager.Fixtures.Add(fixtureId, fixture);
    fixture.Owner = uid;
    if (body.CanCollide && this.Resolve(uid, ref xform))
      this._lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
    if (!updates)
      return;
    this.FixtureUpdate(uid, false, manager: manager, body: body);
    this.Dirty(uid, (IComponent) manager);
  }

  public Fixture? GetFixtureOrNull(EntityUid uid, string id, FixturesComponent? manager = null)
  {
    if (!this._fixtureQuery.Resolve(uid, ref manager))
      return (Fixture) null;
    Fixture fixture;
    return !manager.Fixtures.TryGetValue(id, out fixture) ? (Fixture) null : fixture;
  }

  public void DestroyFixture(
    EntityUid uid,
    string id,
    bool updates = true,
    PhysicsComponent? body = null,
    FixturesComponent? manager = null,
    TransformComponent? xform = null)
  {
    if (!this._fixtureQuery.Resolve(uid, ref manager))
      return;
    Fixture fixtureOrNull = this.GetFixtureOrNull(uid, id, manager);
    if (fixtureOrNull == null)
      return;
    this.DestroyFixture(uid, id, fixtureOrNull, updates, body, manager, xform);
  }

  public void DestroyFixture(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    bool updates = true,
    PhysicsComponent? body = null,
    FixturesComponent? manager = null,
    TransformComponent? xform = null)
  {
    if (!this.Resolve<PhysicsComponent, FixturesComponent, TransformComponent>(uid, ref body, ref manager, ref xform))
      return;
    if (!manager.Fixtures.Remove(fixtureId))
    {
      this.Log.Error($"Tried to remove fixture from {this.ToPrettyString((Entity<MetaDataComponent>) uid)} that was already removed.");
    }
    else
    {
      foreach (Contact contact in fixture.Contacts.Values.ToArray<Contact>())
        this._physics.DestroyContact(contact);
      BroadphaseComponent broadphase;
      if (this._lookup.TryGetCurrentBroadphase(xform, out broadphase))
        this._lookup.DestroyProxies(uid, fixtureId, fixture, xform, broadphase);
      if (!updates)
        return;
      bool resetMass = (double) fixture.Density > 0.0;
      this.FixtureUpdate(uid, resetMass: resetMass, manager: manager, body: body);
    }
  }

  internal void OnPhysicsInit(EntityUid uid, FixturesComponent component, PhysicsComponent? body = null)
  {
    if (!this.Resolve<PhysicsComponent>(uid, ref body, false))
      return;
    foreach ((string key, Fixture fixture) in component.Fixtures)
    {
      if (string.IsNullOrEmpty(key))
        throw new InvalidOperationException($"Tried to setup fixture on init for {this.ToPrettyString((Entity<MetaDataComponent>) uid)} with no ID!");
      fixture.Owner = uid;
    }
    this.FixtureUpdate(uid, false, manager: component, body: body);
  }

  private void OnGetState(EntityUid uid, FixturesComponent component, ref ComponentGetState args)
  {
    Dictionary<string, Fixture> dictionary = new Dictionary<string, Fixture>(component.Fixtures.Count);
    foreach ((string key, Fixture fixture1) in component.Fixtures)
    {
      Fixture fixture2 = new Fixture();
      Fixture fixture3 = fixture2;
      fixture1.CopyTo(fixture3);
      dictionary[key] = fixture2;
    }
    args.State = (IComponentState) new FixtureSystem.FixtureManagerComponentState()
    {
      Fixtures = dictionary
    };
  }

  private void OnHandleState(
    EntityUid uid,
    FixturesComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is FixtureSystem.FixtureManagerComponentState current))
      return;
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>(uid, out comp))
    {
      this.Log.Error($"Tried to apply fixture state for an entity without physics: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
    else
    {
      ValueList<(string, Fixture)> valueList1 = new ValueList<(string, Fixture)>();
      ValueList<(string, Fixture)> valueList2 = new ValueList<(string, Fixture)>();
      bool flag1 = false;
      Dictionary<string, Fixture> dictionary = new Dictionary<string, Fixture>(current.Fixtures.Count);
      foreach ((string key5, Fixture fixture7) in current.Fixtures)
      {
        string key2 = key5;
        Fixture fixture2 = fixture7;
        Fixture fixture3 = new Fixture();
        Fixture fixture4 = fixture3;
        fixture2.CopyTo(fixture4);
        dictionary.Add(key2, fixture3);
        fixture3.Owner = uid;
      }
      TransformComponent xform = (TransformComponent) null;
      bool flag2 = false;
      foreach ((key5, fixture7) in dictionary)
      {
        string key4 = key5;
        Fixture other = fixture7;
        Fixture fixture6;
        if (!component.Fixtures.TryGetValue(key4, out fixture6))
          valueList1.Add((key4, other));
        else if (!fixture6.Equivalent(other))
        {
          other.CopyTo(fixture6);
          flag1 = true;
          flag2 = true;
        }
      }
      foreach ((key5, fixture7) in component.Fixtures)
      {
        string key6 = key5;
        Fixture fixture8 = fixture7;
        if (!dictionary.ContainsKey(key6))
          valueList2.Add((key6, fixture8));
      }
      Span<(string, Fixture)> span1 = valueList2.Span;
      for (int index = 0; index < span1.Length; ++index)
      {
        (string fixtureId, Fixture fixture) = span1[index];
        flag1 = true;
        this.DestroyFixture(uid, fixtureId, fixture, false, comp, component);
      }
      Span<(string, Fixture)> span2 = valueList1.Span;
      for (int index = 0; index < span2.Length; ++index)
      {
        (string fixtureId, Fixture fixture) = span2[index];
        flag1 = true;
        this.CreateFixture(uid, fixtureId, fixture, false, component, comp, xform);
      }
      if (flag1)
        this.FixtureUpdate(uid, manager: component, body: comp);
      if (!flag2)
        return;
      this._broadphase.RegenerateContacts((Entity<PhysicsComponent, FixturesComponent, TransformComponent>) (uid, comp, component, xform));
    }
  }

  public void SetRestitution(
    EntityUid uid,
    string fixtureId,
    Fixture fixture,
    float value,
    bool update = true,
    FixturesComponent? manager = null)
  {
    fixture.Restitution = value;
    if (!update || !this.Resolve<FixturesComponent>(uid, ref manager))
      return;
    this.FixtureUpdate(uid, manager: manager);
  }

  public void FixtureUpdate(
    EntityUid uid,
    bool dirty = true,
    bool resetMass = true,
    FixturesComponent? manager = null,
    PhysicsComponent? body = null)
  {
    if (!this._physicsQuery.Resolve(uid, ref body) || !this._fixtureQuery.Resolve(uid, ref manager))
      return;
    int num1 = 0;
    int num2 = 0;
    bool flag = false;
    foreach (Fixture fixture in manager.Fixtures.Values)
    {
      num1 |= fixture.CollisionMask;
      num2 |= fixture.CollisionLayer;
      flag |= fixture.Hard;
    }
    if (resetMass)
      this._physics.ResetMassData(uid, manager, body);
    int collisionLayer = body.CollisionLayer;
    body.CollisionMask = num1;
    body.CollisionLayer = num2;
    body.Hard = flag;
    if (manager.FixtureCount == 0)
      this._physics.SetCanCollide(uid, false, manager: manager, body: body);
    int num3 = num2;
    if (collisionLayer != num3)
    {
      CollisionLayerChangeEvent message = new CollisionLayerChangeEvent((Entity<PhysicsComponent>) (uid, body));
      this.RaiseLocalEvent<CollisionLayerChangeEvent>(ref message);
    }
    if (!dirty)
      return;
    this.Dirty(uid, (IComponent) manager);
  }

  public int GetFixtureCount(EntityUid uid, FixturesComponent? manager = null)
  {
    return !this._fixtureQuery.Resolve(uid, ref manager) ? 0 : manager.FixtureCount;
  }

  public bool TestPoint<T>(T shape, Robust.Shared.Physics.Transform xform, Vector2 worldPoint) where T : IPhysShape
  {
    if ((object) shape is ChainShape || (object) shape is EdgeShape)
      return false;
    switch (shape)
    {
      case PhysShapeAabb shape1:
        return this.TestPoint<PolygonShape>((PolygonShape) shape1, xform, worldPoint);
      case PhysShapeCircle physShapeCircle:
        Vector2 vector2_1 = xform.Position + Robust.Shared.Physics.Transform.Mul(in xform.Quaternion2D, in physShapeCircle.Position);
        Vector2 vector2_2 = worldPoint - vector2_1;
        return (double) Vector2.Dot(vector2_2, vector2_2) <= (double) physShapeCircle.Radius * (double) physShapeCircle.Radius;
      case PolygonShape polygonShape:
        Vector2 vector2_3 = Robust.Shared.Physics.Transform.MulT(xform.Quaternion2D, worldPoint - xform.Position);
        for (int index = 0; index < polygonShape.VertexCount; ++index)
        {
          if ((double) Vector2.Dot(polygonShape.Normals[index], vector2_3 - polygonShape.Vertices[index]) > 0.0)
            return false;
        }
        return true;
      case SlimPolygon slimPolygon:
        Vector2 vector2_4 = Robust.Shared.Physics.Transform.MulT(xform.Quaternion2D, worldPoint - xform.Position);
        Span<Vector2> asSpan1 = slimPolygon._normals.AsSpan;
        Span<Vector2> asSpan2 = slimPolygon._vertices.AsSpan;
        for (int index = 0; index < (int) slimPolygon.VertexCount; ++index)
        {
          if ((double) Vector2.Dot(asSpan1[index], vector2_4 - asSpan2[index]) > 0.0)
            return false;
        }
        return true;
      case Polygon polygon:
        Vector2 vector2_5 = Robust.Shared.Physics.Transform.MulT(xform.Quaternion2D, worldPoint - xform.Position);
        Span<Vector2> asSpan3 = polygon._normals.AsSpan;
        Span<Vector2> asSpan4 = polygon._vertices.AsSpan;
        for (int index = 0; index < (int) polygon.VertexCount; ++index)
        {
          if ((double) Vector2.Dot(asSpan3[index], vector2_5 - asSpan4[index]) > 0.0)
            return false;
        }
        return true;
      default:
        throw new ArgumentOutOfRangeException($"No implemented TestPoint for {shape.GetType()}");
    }
  }

  public static MassData GetMassData<T>(T shape, float density) where T : IPhysShape
  {
    MassData data = new MassData();
    FixtureSystem.GetMassData<T>(shape, ref data, density);
    return data;
  }

  public static void GetMassData<T>(T shape, ref MassData data, float density) where T : IPhysShape
  {
    if (!((object) shape is ChainShape))
    {
      switch (shape)
      {
        case EdgeShape edgeShape:
          data.Mass = 0.0f;
          data.Center = (edgeShape.Vertex1 + edgeShape.Vertex2) * 0.5f;
          data.I = 0.0f;
          break;
        case PhysShapeCircle physShapeCircle:
          data.Mass = density * 3.14159274f * physShapeCircle.Radius * physShapeCircle.Radius;
          data.Center = physShapeCircle.Position;
          data.I = data.Mass * (0.5f * physShapeCircle.Radius * physShapeCircle.Radius + Vector2.Dot(physShapeCircle.Position, physShapeCircle.Position));
          break;
        case PhysShapeAabb aabb:
          FixtureSystem.GetMassData<Polygon>(new Polygon(aabb), ref data, density);
          break;
        case PolygonShape polyShape:
          FixtureSystem.GetMassData<Polygon>(new Polygon(polyShape), ref data, density);
          break;
        case SlimPolygon slim:
          FixtureSystem.GetMassData<Polygon>(new Polygon(slim), ref data, density);
          break;
        case Polygon polygon:
          byte vertexCount = polygon.VertexCount;
          Vector2 vector2_1 = new Vector2(0.0f, 0.0f);
          float num1 = 0.0f;
          float num2 = 0.0f;
          Vector2 vector2_2 = polygon._vertices._00;
          Span<Vector2> asSpan = polygon._vertices.AsSpan;
          for (int index = 0; index < (int) vertexCount; ++index)
          {
            Vector2 vector2_3 = asSpan[index] - vector2_2;
            Vector2 vector2_4 = index + 1 < (int) vertexCount ? asSpan[index + 1] - vector2_2 : asSpan[0] - vector2_2;
            float num3 = Vector2Helpers.Cross(vector2_3, vector2_4);
            float num4 = 0.5f * num3;
            num1 += num4;
            vector2_1 += (vector2_3 + vector2_4) * num4 * 0.333333343f;
            float x1 = vector2_3.X;
            float y1 = vector2_3.Y;
            float x2 = vector2_4.X;
            float y2 = vector2_4.Y;
            float num5 = (float) ((double) x1 * (double) x1 + (double) x2 * (double) x1 + (double) x2 * (double) x2);
            float num6 = (float) ((double) y1 * (double) y1 + (double) y2 * (double) y1 + (double) y2 * (double) y2);
            num2 += (float) (0.0833333358168602 * (double) num3 * ((double) num5 + (double) num6));
          }
          data.Mass = density * num1;
          vector2_1 *= 1f / num1;
          data.Center = vector2_1 + vector2_2;
          data.I = density * num2;
          data.I += data.Mass * (Vector2.Dot(data.Center, data.Center) - Vector2.Dot(vector2_1, vector2_1));
          break;
        default:
          throw new NotImplementedException($"Cannot get MassData for {shape} as it's not implemented!");
      }
    }
    else
    {
      data.Mass = 0.0f;
      data.Center = Vector2.Zero;
      data.I = 0.0f;
    }
  }

  [NetSerializable]
  [Serializable]
  private sealed class FixtureManagerComponentState : ComponentState
  {
    public Dictionary<string, Fixture> Fixtures;
  }
}
