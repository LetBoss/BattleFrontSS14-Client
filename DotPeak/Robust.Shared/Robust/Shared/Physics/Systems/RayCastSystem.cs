// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.RayCastSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public sealed class RayCastSystem : EntitySystem
{
  [Dependency]
  private readonly SharedBroadphaseSystem _broadphase;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  private readonly RayCastSystem.RayComparer _rayComparer = new RayCastSystem.RayComparer();

  private void AdjustResults(ref RayResult result, int index, Robust.Shared.Physics.Transform xf)
  {
    for (int index1 = index; index1 < result.Results.Count; ++index1)
      result.Results[index1].Point = Robust.Shared.Physics.Transform.Mul(in xf, in result.Results[index1].Point);
  }

  public void CastRay(
    Entity<BroadphaseComponent?> entity,
    ref RayResult result,
    Vector2 origin,
    Vector2 translation,
    QueryFilter filter,
    bool sorted = true)
  {
    if (!this.Resolve<BroadphaseComponent>(entity.Owner, ref entity.Comp))
      return;
    RayCastInput input = new RayCastInput()
    {
      Origin = origin,
      Translation = translation,
      MaxFraction = 1f
    };
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    WorldRayCastContext state = new WorldRayCastContext()
    {
      fcn = RayCastSystem.\u003C\u003EO.\u003C0\u003E__RayCastAllCallback ?? (RayCastSystem.\u003C\u003EO.\u003C0\u003E__RayCastAllCallback = new CastResult(RayCastSystem.RayCastAllCallback)),
      Filter = filter,
      Fraction = 1f,
      Physics = this._physics,
      System = this,
      Result = result
    };
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    entity.Comp.DynamicTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback ?? (RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback = new B2DynamicTree<FixtureProxy>.RayCallback(RayCastSystem.RayCastCallback)));
    input.MaxFraction = state.Fraction;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    entity.Comp.StaticTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback ?? (RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback = new B2DynamicTree<FixtureProxy>.RayCallback(RayCastSystem.RayCastCallback)));
    result = state.Result;
    if (!sorted)
      return;
    result.Results.Sort((IComparer<RayHit>) this._rayComparer);
  }

  public RayResult CastRay(MapId mapId, Vector2 origin, Vector2 translation, QueryFilter filter)
  {
    RayCastInput rayCastInput = new RayCastInput()
    {
      Origin = origin,
      Translation = translation,
      MaxFraction = 1f
    };
    RayResult rayResult1 = new RayResult();
    Vector2 vector2_1 = origin;
    Vector2 vector2_2 = origin + translation;
    Box2 aabb;
    // ISSUE: explicit constructor call
    ((Box2) ref aabb).\u002Ector(Vector2.Min(vector2_1, vector2_2), Vector2.Max(vector2_1, vector2_2));
    (RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem) state = (rayCastInput, filter, rayResult1, this, this._physics);
    this._broadphase.GetBroadphases<(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem)>(mapId, aabb, ref state, (SharedBroadphaseSystem.BroadphaseCallback<(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem)>) ((Entity<BroadphaseComponent> entity, ref (RayCastInput input, QueryFilter filter, RayResult result, RayCastSystem system, SharedPhysicsSystem Physics) tuple) =>
    {
      Robust.Shared.Physics.Transform physicsTransform = tuple.Item5.GetPhysicsTransform(entity.Owner);
      Vector2 origin1 = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.Item1.Origin);
      Vector2 translation1 = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.Item1.Origin + tuple.Item1.Translation) - origin1;
      int count = tuple.Item3.Results.Count;
      tuple.Item4.CastRay((Entity<BroadphaseComponent>) (entity.Owner, entity.Comp), ref tuple.Item3, origin1, translation1, tuple.Item2, false);
      tuple.Item4.AdjustResults(ref tuple.Item3, count, physicsTransform);
    }));
    RayResult rayResult2 = state.Item3;
    rayResult2.Results.Sort((IComparer<RayHit>) this._rayComparer);
    return rayResult2;
  }

  public void CastRayClosest(
    Entity<BroadphaseComponent?> entity,
    ref RayResult result,
    Vector2 origin,
    Vector2 translation,
    QueryFilter filter)
  {
    if (!this.Resolve<BroadphaseComponent>(entity.Owner, ref entity.Comp))
      return;
    RayCastInput input = new RayCastInput()
    {
      Origin = origin,
      Translation = translation,
      MaxFraction = 1f
    };
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    WorldRayCastContext state = new WorldRayCastContext()
    {
      fcn = RayCastSystem.\u003C\u003EO.\u003C2\u003E__RayCastClosestCallback ?? (RayCastSystem.\u003C\u003EO.\u003C2\u003E__RayCastClosestCallback = new CastResult(RayCastSystem.RayCastClosestCallback)),
      Filter = filter,
      Fraction = 1f,
      Physics = this._physics,
      System = this,
      Result = result
    };
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    entity.Comp.DynamicTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback ?? (RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback = new B2DynamicTree<FixtureProxy>.RayCallback(RayCastSystem.RayCastCallback)));
    input.MaxFraction = state.Fraction;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    entity.Comp.StaticTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback ?? (RayCastSystem.\u003C\u003EO.\u003C1\u003E__RayCastCallback = new B2DynamicTree<FixtureProxy>.RayCallback(RayCastSystem.RayCastCallback)));
    result = state.Result;
  }

  public RayResult CastRayClosest(
    MapId mapId,
    Vector2 origin,
    Vector2 translation,
    QueryFilter filter)
  {
    RayCastInput rayCastInput = new RayCastInput()
    {
      Origin = origin,
      Translation = translation,
      MaxFraction = 1f
    };
    RayResult rayResult = new RayResult();
    Vector2 vector2 = origin + translation;
    Box2 aabb;
    // ISSUE: explicit constructor call
    ((Box2) ref aabb).\u002Ector(Vector2.Min(origin, vector2), Vector2.Max(origin, vector2));
    (RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem) state = (rayCastInput, filter, rayResult, this, this._physics);
    this._broadphase.GetBroadphases<(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem)>(mapId, aabb, ref state, (SharedBroadphaseSystem.BroadphaseCallback<(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem)>) ((Entity<BroadphaseComponent> entity, ref (RayCastInput input, QueryFilter filter, RayResult result, RayCastSystem system, SharedPhysicsSystem _physics) tuple) =>
    {
      Robust.Shared.Physics.Transform physicsTransform = tuple.Item5.GetPhysicsTransform(entity.Owner);
      Vector2 origin1 = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.Item1.Origin);
      Vector2 translation1 = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.Item1.Origin + tuple.Item1.Translation) - origin1;
      int count = tuple.Item3.Results.Count;
      tuple.Item4.CastRayClosest((Entity<BroadphaseComponent>) (entity.Owner, entity.Comp), ref tuple.Item3, origin1, translation1, tuple.Item2);
      tuple.Item4.AdjustResults(ref tuple.Item3, count, physicsTransform);
    }));
    return state.Item3;
  }

  public RayResult CastShape(
    MapId mapId,
    IPhysShape shape,
    Robust.Shared.Physics.Transform originTransform,
    Vector2 translation,
    QueryFilter filter,
    CastResult callback)
  {
    Box2 aabb1 = shape.ComputeAABB(originTransform, 0);
    Box2 aabb2 = shape.ComputeAABB(new Robust.Shared.Physics.Transform(originTransform.Position + translation, originTransform.Quaternion2D.Angle), 0);
    Box2 aabb3 = ((Box2) ref aabb1).Union(ref aabb2);
    RayResult rayResult = new RayResult();
    (Robust.Shared.Physics.Transform, Vector2, IPhysShape, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem, CastResult) state = (originTransform, translation, shape, filter, rayResult, this, this._physics, callback);
    this._broadphase.GetBroadphases<(Robust.Shared.Physics.Transform, Vector2, IPhysShape, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem, CastResult)>(mapId, aabb3, ref state, (SharedBroadphaseSystem.BroadphaseCallback<(Robust.Shared.Physics.Transform, Vector2, IPhysShape, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem, CastResult)>) ((Entity<BroadphaseComponent> entity, ref (Robust.Shared.Physics.Transform origin, Vector2 translation, IPhysShape shape, QueryFilter filter, RayResult result, RayCastSystem system, SharedPhysicsSystem _physics, CastResult callback) tuple) =>
    {
      Robust.Shared.Physics.Transform A = tuple.Item7.GetPhysicsTransform(entity.Owner);
      Robust.Shared.Physics.Transform originTransform1 = Robust.Shared.Physics.Transform.MulT(in A, tuple.Item1);
      Vector2 translation1 = Robust.Shared.Physics.Transform.InvTransformPoint(A, tuple.Item1.Position + tuple.Item2) - originTransform1.Position;
      int count = tuple.Item5.Results.Count;
      tuple.Item6.CastShape((Entity<BroadphaseComponent>) (entity.Owner, entity.Comp), ref tuple.Item5, tuple.Item3, originTransform1, translation1, tuple.Item4, tuple.Item8);
      tuple.Item6.AdjustResults(ref tuple.Item5, count, A);
    }));
    return state.Item5;
  }

  public void CastShape(
    Entity<BroadphaseComponent?> entity,
    ref RayResult result,
    IPhysShape shape,
    Robust.Shared.Physics.Transform originTransform,
    Vector2 translation,
    QueryFilter filter,
    CastResult callback)
  {
    if (!this.Resolve<BroadphaseComponent>(entity.Owner, ref entity.Comp))
      return;
    switch (shape)
    {
      case PhysShapeCircle circle:
        this.CastCircle(entity, ref result, circle, originTransform, translation, filter, callback);
        break;
      case SlimPolygon poly1:
        this.CastPolygon(entity, ref result, new PolygonShape(poly1), originTransform, translation, filter, callback);
        break;
      case Polygon poly2:
        this.CastPolygon(entity, ref result, new PolygonShape(poly2), originTransform, translation, filter, callback);
        break;
      case PolygonShape polygon:
        this.CastPolygon(entity, ref result, polygon, originTransform, translation, filter, callback);
        break;
      default:
        this.Log.Error("Tried to shapecast for shape not implemented.");
        break;
    }
  }

  public void CastCircle(
    Entity<BroadphaseComponent?> entity,
    ref RayResult result,
    PhysShapeCircle circle,
    Robust.Shared.Physics.Transform originTransform,
    Vector2 translation,
    QueryFilter filter,
    CastResult callback)
  {
    if (!this.Resolve<BroadphaseComponent>(entity.Owner, ref entity.Comp))
      return;
    ShapeCastInput input = new ShapeCastInput()
    {
      Points = new Vector2[1],
      Count = 1,
      Radius = circle.Radius,
      Translation = translation,
      MaxFraction = 1f
    };
    input.Points[0] = Robust.Shared.Physics.Transform.Mul(in originTransform, in circle.Position);
    WorldRayCastContext state = new WorldRayCastContext()
    {
      System = this,
      Physics = this._physics,
      Filter = filter,
      Fraction = 1f,
      Result = result,
      fcn = callback
    };
    entity.Comp.StaticTree.Tree.ShapeCast(input, filter.MaskBits, new B2DynamicTree<FixtureProxy>.TreeShapeCastCallback(this.ShapeCastCallback), ref state);
    input.MaxFraction = state.Fraction;
    entity.Comp.DynamicTree.Tree.ShapeCast(input, filter.MaskBits, new B2DynamicTree<FixtureProxy>.TreeShapeCastCallback(this.ShapeCastCallback), ref state);
    result = state.Result;
  }

  public void CastPolygon(
    Entity<BroadphaseComponent?> entity,
    ref RayResult result,
    PolygonShape polygon,
    Robust.Shared.Physics.Transform originTransform,
    Vector2 translation,
    QueryFilter filter,
    CastResult callback)
  {
    if (!this.Resolve<BroadphaseComponent>(entity.Owner, ref entity.Comp))
      return;
    ShapeCastInput input = new ShapeCastInput()
    {
      Points = new Vector2[polygon.VertexCount]
    };
    for (int index = 0; index < polygon.VertexCount; ++index)
      input.Points[index] = Robust.Shared.Physics.Transform.Mul(in originTransform, in polygon.Vertices[index]);
    input.Count = polygon.VertexCount;
    input.Radius = polygon.Radius;
    input.Translation = translation;
    input.MaxFraction = 1f;
    WorldRayCastContext state = new WorldRayCastContext()
    {
      System = this,
      Physics = this._physics,
      Filter = filter,
      Fraction = 1f,
      Result = result,
      fcn = callback
    };
    if ((filter.Flags & QueryFlags.Static) == QueryFlags.Static)
    {
      entity.Comp.StaticTree.Tree.ShapeCast(input, filter.MaskBits, new B2DynamicTree<FixtureProxy>.TreeShapeCastCallback(this.ShapeCastCallback), ref state);
      input.MaxFraction = state.Fraction;
    }
    if ((filter.Flags & QueryFlags.Dynamic) == QueryFlags.Dynamic)
      entity.Comp.DynamicTree.Tree.ShapeCast(input, filter.MaskBits, new B2DynamicTree<FixtureProxy>.TreeShapeCastCallback(this.ShapeCastCallback), ref state);
    result = state.Result;
  }

  public static float RayCastAllCallback(
    FixtureProxy proxy,
    Vector2 point,
    Vector2 normal,
    float fraction,
    ref RayResult result)
  {
    result.Results.Add(new RayHit(proxy.Entity, normal, fraction)
    {
      Point = point
    });
    return 1f;
  }

  public static float RayCastClosestCallback(
    FixtureProxy proxy,
    Vector2 point,
    Vector2 normal,
    float fraction,
    ref RayResult result)
  {
    bool flag = false;
    if (result.Results.Count > 0)
    {
      if ((double) result.Results[0].Fraction > (double) fraction)
      {
        flag = true;
        result.Results.Clear();
      }
    }
    else
      flag = true;
    if (flag)
      result.Results.Add(new RayHit(proxy.Entity, normal, fraction)
      {
        Point = point
      });
    return fraction;
  }

  private CastOutput RayCastShape(RayCastInput input, IPhysShape shape, Robust.Shared.Physics.Transform transform)
  {
    RayCastInput input1 = input with
    {
      Origin = Robust.Shared.Physics.Transform.InvTransformPoint(transform, input.Origin),
      Translation = Quaternion2D.InvRotateVector(transform.Quaternion2D, input.Translation)
    };
    CastOutput castOutput1 = new CastOutput();
    CastOutput castOutput2;
    switch (shape)
    {
      case PhysShapeCircle shape1:
        castOutput2 = this.RayCastCircle(input1, shape1);
        break;
      case PolygonShape shape2:
        castOutput2 = this.RayCastPolygon(input1, (Polygon) shape2);
        break;
      case Polygon shape3:
        castOutput2 = this.RayCastPolygon(input1, shape3);
        break;
      default:
        return castOutput1;
    }
    castOutput2.Point = Robust.Shared.Physics.Transform.Mul(in transform, in castOutput2.Point);
    castOutput2.Normal = Quaternion2D.RotateVector(transform.Quaternion2D, castOutput2.Normal);
    return castOutput2;
  }

  private static float RayCastCallback(
    RayCastInput input,
    FixtureProxy proxy,
    ref WorldRayCastContext worldContext)
  {
    if (((long) proxy.Fixture.CollisionLayer & worldContext.Filter.MaskBits) == 0L && ((long) proxy.Fixture.CollisionMask & worldContext.Filter.LayerBits) == 0L)
      return input.MaxFraction;
    Func<EntityUid, bool> isIgnored = worldContext.Filter.IsIgnored;
    if ((isIgnored != null ? (isIgnored(proxy.Entity) ? 1 : 0) : 0) != 0)
      return input.MaxFraction;
    Robust.Shared.Physics.Transform physicsTransform = worldContext.Physics.GetLocalPhysicsTransform(proxy.Entity);
    CastOutput castOutput = worldContext.System.RayCastShape(input, proxy.Fixture.Shape, physicsTransform);
    return castOutput.Hit ? worldContext.fcn(proxy, castOutput.Point, castOutput.Normal, castOutput.Fraction, ref worldContext.Result) : input.MaxFraction;
  }

  internal CastOutput RayCastCircle(RayCastInput input, PhysShapeCircle shape)
  {
    Vector2 position = shape.Position;
    CastOutput castOutput = new CastOutput();
    Vector2 left = Vector2.Subtract(input.Origin, position);
    float num1 = 0.0f;
    Vector2 lengthAndNormalize = Vector2Helpers.GetLengthAndNormalize(input.Translation, ref num1);
    if ((double) num1 == 0.0)
      return castOutput;
    float num2 = -Vector2.Dot(left, lengthAndNormalize);
    Vector2 vector2_1 = Vector2.Add(left, num2 * lengthAndNormalize);
    float num3 = Vector2.Dot(vector2_1, vector2_1);
    double radius = (double) shape.Radius;
    float num4 = (float) (radius * radius);
    if ((double) num3 > (double) num4)
      return castOutput;
    float num5 = MathF.Sqrt(num4 - num3);
    float num6 = num2 - num5;
    if ((double) num6 < 0.0 || (double) input.MaxFraction * (double) num1 < (double) num6)
      return castOutput;
    Vector2 vector2_2 = Vector2.Add(left, num6 * lengthAndNormalize);
    castOutput.Fraction = num6 / num1;
    castOutput.Normal = Vector2Helpers.Normalized(vector2_2);
    castOutput.Point = Vector2.Add(position, shape.Radius * castOutput.Normal);
    castOutput.Hit = true;
    return castOutput;
  }

  private CastOutput RayCastPolygon(RayCastInput input, Polygon shape)
  {
    Span<Vector2> asSpan1 = shape._vertices.AsSpan;
    CastOutput output = new CastOutput() { Fraction = 0.0f };
    if ((double) shape.Radius == 0.0)
    {
      Vector2 origin = input.Origin;
      Vector2 translation = input.Translation;
      float num1 = 0.0f;
      float num2 = input.MaxFraction;
      int index1 = -1;
      Span<Vector2> asSpan2 = shape._normals.AsSpan;
      for (int index2 = 0; index2 < (int) shape.VertexCount; ++index2)
      {
        float num3 = Vector2.Dot(asSpan2[index2], Vector2.Subtract(asSpan1[index2], origin));
        float num4 = Vector2.Dot(asSpan2[index2], translation);
        if ((double) num4 == 0.0)
        {
          if ((double) num3 < 0.0)
            return output;
        }
        else if ((double) num4 < 0.0 && (double) num3 < (double) num1 * (double) num4)
        {
          num1 = num3 / num4;
          index1 = index2;
        }
        else if ((double) num4 > 0.0 && (double) num3 < (double) num2 * (double) num4)
          num2 = num3 / num4;
        if ((double) num2 < (double) num1)
          return output;
      }
      if (index1 >= 0)
      {
        output.Fraction = num1;
        output.Normal = asSpan2[index1];
        output.Point = Vector2.Add(origin, num1 * translation);
        output.Hit = true;
      }
      return output;
    }
    Span<Vector2> vertices = (Span<Vector2>) new Vector2[1]
    {
      input.Origin
    };
    ShapeCastPairInput input1 = new ShapeCastPairInput()
    {
      ProxyA = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) asSpan1, (int) shape.VertexCount, shape.Radius),
      ProxyB = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) vertices, 1, 0.0f),
      TransformA = Robust.Shared.Physics.Transform.Empty,
      TransformB = Robust.Shared.Physics.Transform.Empty,
      TranslationB = input.Translation,
      MaxFraction = input.MaxFraction
    };
    this.ShapeCast(ref output, in input1);
    return output;
  }

  private CastOutput RayCastSegment(RayCastInput input, EdgeShape shape, bool oneSided)
  {
    CastOutput castOutput = new CastOutput();
    if (oneSided && (double) Vector2Helpers.Cross(Vector2.Subtract(input.Origin, shape.Vertex0), Vector2.Subtract(shape.Vertex1, shape.Vertex0)) < 0.0)
      return castOutput;
    Vector2 origin = input.Origin;
    Vector2 translation = input.Translation;
    Vector2 vertex0 = shape.Vertex0;
    Vector2 vector2_1 = Vector2.Subtract(shape.Vertex1, vertex0);
    float num1 = 0.0f;
    ref float local = ref num1;
    Vector2 lengthAndNormalize = Vector2Helpers.GetLengthAndNormalize(vector2_1, ref local);
    if ((double) num1 == 0.0)
      return castOutput;
    Vector2 vector2_2 = Vector2Helpers.RightPerp(lengthAndNormalize);
    float num2 = Vector2.Dot(vector2_2, Vector2.Subtract(vertex0, origin));
    float num3 = Vector2.Dot(vector2_2, translation);
    if ((double) num3 == 0.0)
      return castOutput;
    float num4 = num2 / num3;
    if ((double) num4 < 0.0 || (double) input.MaxFraction < (double) num4)
      return castOutput;
    float num5 = Vector2.Dot(Vector2.Subtract(Vector2.Add(origin, num4 * translation), vertex0), lengthAndNormalize);
    if ((double) num5 < 0.0 || (double) num1 < (double) num5)
      return castOutput;
    if ((double) num2 > 0.0)
      vector2_2 = -vector2_2;
    castOutput.Fraction = num4;
    castOutput.Point = Vector2.Add(origin, num4 * translation);
    castOutput.Normal = vector2_2;
    castOutput.Hit = true;
    return castOutput;
  }

  private CastOutput ShapeCastShape(ShapeCastInput input, IPhysShape shape, Robust.Shared.Physics.Transform transform)
  {
    ShapeCastInput input1 = input;
    for (int index = 0; index < input1.Count; ++index)
      input1.Points[index] = Robust.Shared.Physics.Transform.MulT(in transform, in input.Points[index]);
    input1.Translation = Quaternion2D.InvRotateVector(transform.Quaternion2D, input.Translation);
    CastOutput castOutput;
    switch (shape)
    {
      case PhysShapeCircle shape1:
        castOutput = this.ShapeCastCircle(input1, shape1);
        break;
      case PolygonShape shape2:
        castOutput = this.ShapeCastPolygon(input1, (Polygon) shape2);
        break;
      case Polygon shape3:
        castOutput = this.ShapeCastPolygon(input1, shape3);
        break;
      default:
        return new CastOutput();
    }
    castOutput.Point = Robust.Shared.Physics.Transform.Mul(in transform, in castOutput.Point);
    castOutput.Normal = Quaternion2D.RotateVector(transform.Quaternion2D, castOutput.Normal);
    return castOutput;
  }

  private float ShapeCastCallback(
    ShapeCastInput input,
    FixtureProxy proxy,
    ref WorldRayCastContext worldContext)
  {
    QueryFilter filter = worldContext.Filter;
    if (((long) proxy.Fixture.CollisionLayer & filter.MaskBits) == 0L && ((long) proxy.Fixture.CollisionMask & filter.LayerBits) == 0L || (filter.Flags & QueryFlags.Sensors) == QueryFlags.None && !proxy.Fixture.Hard)
      return input.MaxFraction;
    Func<EntityUid, bool> isIgnored = worldContext.Filter.IsIgnored;
    if ((isIgnored != null ? (isIgnored(proxy.Entity) ? 1 : 0) : 0) != 0)
      return input.MaxFraction;
    Robust.Shared.Physics.Transform physicsTransform = worldContext.Physics.GetLocalPhysicsTransform(proxy.Entity);
    CastOutput castOutput = this.ShapeCastShape(input, proxy.Fixture.Shape, physicsTransform);
    return castOutput.Hit ? worldContext.fcn(proxy, castOutput.Point, castOutput.Normal, castOutput.Fraction, ref worldContext.Result) : input.MaxFraction;
  }

  private void ShapeCast(ref CastOutput output, in ShapeCastPairInput input)
  {
    output.Fraction = input.MaxFraction;
    DistanceProxy proxyA = input.ProxyA;
    int length = input.ProxyB.Vertices.Length;
    Robust.Shared.Physics.Transform transformA = input.TransformA;
    Robust.Shared.Physics.Transform B = input.TransformB;
    Robust.Shared.Physics.Transform transform = Robust.Shared.Physics.Transform.InvMulTransforms(in transformA, in B);
    Vector2[] vertices = new Vector2[input.ProxyB.Vertices.Length];
    for (int index = 0; index < length; ++index)
      vertices[index] = Robust.Shared.Physics.Transform.Mul(in transform, in input.ProxyB.Vertices[index]);
    DistanceProxy proxy = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) vertices, length, input.ProxyB.Radius);
    float num1 = proxyA.Radius + proxy.Radius;
    Vector2 direction1 = Quaternion2D.RotateVector(transform.Quaternion2D, input.TranslationB);
    float num2 = 0.0f;
    float maxFraction = input.MaxFraction;
    Simplex s = new Simplex()
    {
      Count = 0,
      V = new FixedArray4<SimplexVertex>()
    };
    int support1 = this.FindSupport(proxyA, -direction1);
    Vector2 left1 = proxyA.Vertices[support1];
    int support2 = this.FindSupport(proxy, direction1);
    Vector2 right1 = proxy.Vertices[support2];
    Vector2 direction2 = Vector2.Subtract(left1, right1);
    float num3 = MathF.Max(0.005f, num1 - 0.005f);
    int num4;
    for (num4 = 0; num4 < 20 && (double) direction2.Length() > (double) num3 + 1.0 / 400.0; ++num4)
    {
      ++output.Iterations;
      int support3 = this.FindSupport(proxyA, -direction2);
      Vector2 left2 = proxyA.Vertices[support3];
      int support4 = this.FindSupport(proxy, direction2);
      Vector2 right2 = proxy.Vertices[support4];
      Vector2 vector2_1 = Vector2.Subtract(left2, right2);
      Vector2 vector2_2 = Vector2Helpers.Normalized(direction2);
      float num5 = Vector2.Dot(vector2_2, vector2_1);
      float num6 = Vector2.Dot(vector2_2, direction1);
      if ((double) num5 - (double) num3 > (double) num2 * (double) num6)
      {
        if ((double) num6 <= 0.0)
          return;
        num2 = (num5 - num3) / num6;
        if ((double) num2 > (double) maxFraction)
          return;
        s.Count = 0;
      }
      ref SimplexVertex local = ref s.V.AsSpan[s.Count];
      local.IndexA = support4;
      local.WA = new Vector2(right2.X + num2 * direction1.X, right2.Y + num2 * direction1.Y);
      local.IndexB = support3;
      local.WB = left2;
      local.W = Vector2.Subtract(local.WB, local.WA);
      local.A = 1f;
      ++s.Count;
      switch (s.Count)
      {
        case 1:
          if (s.Count == 3)
            return;
          direction2 = Simplex.ComputeSimplexClosestPoint(s);
          continue;
        case 2:
          Simplex.SolveSimplex2(ref s);
          goto case 1;
        case 3:
          Simplex.SolveSimplex3(ref s);
          goto case 1;
        default:
          throw new NotImplementedException();
      }
    }
    if (num4 == 0 || (double) num2 == 0.0)
      return;
    Vector2 zero1 = Vector2.Zero;
    Vector2 zero2 = Vector2.Zero;
    Simplex.ComputeSimplexWitnessPoints(ref zero2, ref zero1, s);
    Vector2 v = Vector2Helpers.Normalized(-direction2);
    Vector2 vector = new Vector2(zero1.X + proxyA.Radius * v.X, zero1.Y + proxyA.Radius * v.Y);
    output.Point = Robust.Shared.Physics.Transform.Mul(in transformA, in vector);
    output.Normal = Quaternion2D.RotateVector(transformA.Quaternion2D, v);
    output.Fraction = num2;
    output.Iterations = num4;
    output.Hit = true;
  }

  private int FindSupport(DistanceProxy proxy, Vector2 direction)
  {
    int support = 0;
    float num1 = Vector2.Dot(proxy.Vertices[0], direction);
    for (int index = 1; index < proxy.Vertices.Length; ++index)
    {
      float num2 = Vector2.Dot(proxy.Vertices[index], direction);
      if ((double) num2 > (double) num1)
      {
        support = index;
        num1 = num2;
      }
    }
    return support;
  }

  private CastOutput ShapeCastCircle(ShapeCastInput input, PhysShapeCircle shape)
  {
    Span<Vector2> vertices = (Span<Vector2>) new Vector2[1]
    {
      shape.Position
    };
    ShapeCastPairInput input1 = new ShapeCastPairInput()
    {
      ProxyA = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) vertices, 1, shape.Radius),
      ProxyB = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) input.Points, input.Count, input.Radius),
      TransformA = Robust.Shared.Physics.Transform.Empty,
      TransformB = Robust.Shared.Physics.Transform.Empty,
      TranslationB = input.Translation,
      MaxFraction = input.MaxFraction
    };
    CastOutput output = new CastOutput();
    this.ShapeCast(ref output, in input1);
    return output;
  }

  private CastOutput ShapeCastPolygon(ShapeCastInput input, Polygon shape)
  {
    ShapeCastPairInput input1 = new ShapeCastPairInput()
    {
      ProxyA = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) shape._vertices.AsSpan, (int) shape.VertexCount, shape.Radius),
      ProxyB = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) input.Points, input.Count, input.Radius),
      TransformA = Robust.Shared.Physics.Transform.Empty,
      TransformB = Robust.Shared.Physics.Transform.Empty,
      TranslationB = input.Translation,
      MaxFraction = input.MaxFraction
    };
    CastOutput output = new CastOutput();
    this.ShapeCast(ref output, in input1);
    return output;
  }

  private CastOutput ShapeCastSegment(ShapeCastInput input, EdgeShape shape)
  {
    Span<Vector2> vertices = (Span<Vector2>) new Vector2[1]
    {
      shape.Vertex0
    };
    ShapeCastPairInput input1 = new ShapeCastPairInput()
    {
      ProxyA = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) vertices, 2, 0.0f),
      ProxyB = DistanceProxy.MakeProxy((ReadOnlySpan<Vector2>) input.Points, input.Count, input.Radius),
      TransformA = Robust.Shared.Physics.Transform.Empty,
      TransformB = Robust.Shared.Physics.Transform.Empty,
      TranslationB = input.Translation,
      MaxFraction = input.MaxFraction
    };
    CastOutput output = new CastOutput();
    this.ShapeCast(ref output, in input1);
    return output;
  }

  private sealed class RayComparer : IComparer<RayHit>
  {
    public int Compare(RayHit x, RayHit y) => x.Fraction.CompareTo(y.Fraction);
  }
}
