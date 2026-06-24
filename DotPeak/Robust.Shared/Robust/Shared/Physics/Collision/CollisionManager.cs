// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.CollisionManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.Extensions.ObjectPool;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Collision;

internal sealed class CollisionManager : IManifoldManager
{
  private Microsoft.Extensions.ObjectPool.ObjectPool<EdgeShape> _edgePool = (Microsoft.Extensions.ObjectPool.ObjectPool<EdgeShape>) new DefaultObjectPool<EdgeShape>((IPooledObjectPolicy<EdgeShape>) new DefaultPooledObjectPolicy<EdgeShape>());

  public void CollideCircles(
    ref Manifold manifold,
    PhysShapeCircle circleA,
    in Transform xfA,
    PhysShapeCircle circleB,
    in Transform xfB)
  {
    manifold.PointCount = 0;
    Vector2 vector2_1 = Transform.Mul(in xfA, in circleA.Position);
    Vector2 vector2_2 = Transform.Mul(in xfB, in circleB.Position) - vector2_1;
    double num1 = (double) Vector2.Dot(vector2_2, vector2_2);
    float num2 = circleA.Radius + circleB.Radius;
    double num3 = (double) num2 * (double) num2;
    if (num1 > num3)
      return;
    manifold.Type = ManifoldType.Circles;
    manifold.LocalPoint = circleA.Position;
    manifold.LocalNormal = Vector2.Zero;
    manifold.PointCount = 1;
    ref ManifoldPoint local = ref manifold.Points._00;
    local.LocalPoint = Vector2.Zero;
    local.Id.Key = 0U;
  }

  public static void GetPointStates(
    ref PointState[] state1,
    ref PointState[] state2,
    in Manifold manifold1,
    in Manifold manifold2)
  {
    Span<ManifoldPoint> asSpan1 = manifold1.Points.AsSpan;
    Span<ManifoldPoint> asSpan2 = manifold2.Points.AsSpan;
    for (int index1 = 0; index1 < manifold1.PointCount; ++index1)
    {
      ContactID id = asSpan1[index1].Id;
      state1[index1] = PointState.Remove;
      for (int index2 = 0; index2 < manifold2.PointCount; ++index2)
      {
        if ((int) asSpan2[index2].Id.Key == (int) id.Key)
        {
          state1[index1] = PointState.Persist;
          break;
        }
      }
    }
    for (int index3 = 0; index3 < manifold2.PointCount; ++index3)
    {
      ContactID id = asSpan2[index3].Id;
      state2[index3] = PointState.Add;
      for (int index4 = 0; index4 < manifold1.PointCount; ++index4)
      {
        if ((int) asSpan1[index4].Id.Key == (int) id.Key)
        {
          state2[index3] = PointState.Persist;
          break;
        }
      }
    }
  }

  private static int ClipSegmentToLine(
    Span<ClipVertex> vOut,
    Span<ClipVertex> vIn,
    Vector2 normal,
    float offset,
    int vertexIndexA)
  {
    ClipVertex clipVertex1 = vIn[0];
    ClipVertex clipVertex2 = vIn[1];
    int index = 0;
    float num1 = (float) ((double) normal.X * (double) clipVertex1.V.X + (double) normal.Y * (double) clipVertex1.V.Y) - offset;
    float num2 = (float) ((double) normal.X * (double) clipVertex2.V.X + (double) normal.Y * (double) clipVertex2.V.Y) - offset;
    if ((double) num1 <= 0.0)
      vOut[index++] = clipVertex1;
    if ((double) num2 <= 0.0)
      vOut[index++] = clipVertex2;
    if ((double) num1 * (double) num2 < 0.0)
    {
      float num3 = num1 / (num1 - num2);
      ref ClipVertex local = ref vOut[index];
      local.V.X = clipVertex1.V.X + num3 * (clipVertex2.V.X - clipVertex1.V.X);
      local.V.Y = clipVertex1.V.Y + num3 * (clipVertex2.V.Y - clipVertex1.V.Y);
      local.ID.Features.IndexA = (byte) vertexIndexA;
      local.ID.Features.IndexB = clipVertex1.ID.Features.IndexB;
      local.ID.Features.TypeA = (byte) 0;
      local.ID.Features.TypeB = (byte) 1;
      ++index;
    }
    return index;
  }

  public EdgeShape GetContactEdge() => this._edgePool.Get();

  public void ReturnEdge(EdgeShape edge) => this._edgePool.Return(edge);

  public void CollideEdgeAndCircle(
    ref Manifold manifold,
    EdgeShape edgeA,
    in Transform transformA,
    PhysShapeCircle circleB,
    in Transform transformB)
  {
    manifold.PointCount = 0;
    Vector2 vector2_1 = Transform.MulT(in transformA, Transform.Mul(in transformB, in circleB.Position));
    Vector2 vertex1 = edgeA.Vertex1;
    Vector2 vertex2 = edgeA.Vertex2;
    Vector2 vector2_2 = vertex2 - vertex1;
    Vector2 vector2_3 = new Vector2(vector2_2.Y, -vector2_2.X);
    float num1 = Vector2.Dot(vector2_3, vector2_1 - vertex1);
    if (edgeA.OneSided && (double) num1 < 0.0)
      return;
    float num2 = Vector2.Dot(vector2_2, vertex2 - vector2_1);
    float num3 = Vector2.Dot(vector2_2, vector2_1 - vertex1);
    float num4 = edgeA.Radius + circleB.Radius;
    ContactFeature contactFeature = new ContactFeature();
    contactFeature.IndexB = (byte) 0;
    contactFeature.TypeB = (byte) 0;
    if ((double) num3 <= 0.0)
    {
      Vector2 vector2_4 = vertex1;
      Vector2 vector2_5 = vector2_1 - vector2_4;
      if ((double) Vector2.Dot(vector2_5, vector2_5) > (double) num4 * (double) num4)
        return;
      if (edgeA.OneSided)
      {
        Vector2 vertex0 = edgeA.Vertex0;
        Vector2 vector2_6 = vertex1;
        if ((double) Vector2.Dot(vector2_6 - vertex0, vector2_6 - vector2_1) > 0.0)
          return;
      }
      contactFeature.IndexA = (byte) 0;
      contactFeature.TypeA = (byte) 0;
      manifold.PointCount = 1;
      manifold.Type = ManifoldType.Circles;
      manifold.LocalNormal = Vector2.Zero;
      manifold.LocalPoint = vector2_4;
      manifold.Points._00.Id.Key = 0U;
      manifold.Points._00.Id.Features = contactFeature;
      manifold.Points._00.LocalPoint = circleB.Position;
    }
    else if ((double) num2 <= 0.0)
    {
      Vector2 vector2_7 = vertex2;
      Vector2 vector2_8 = vector2_1 - vector2_7;
      if ((double) Vector2.Dot(vector2_8, vector2_8) > (double) num4 * (double) num4)
        return;
      if (edgeA.OneSided)
      {
        Vector2 vertex3 = edgeA.Vertex3;
        Vector2 vector2_9 = vertex2;
        Vector2 vector2_10 = vector2_9;
        if ((double) Vector2.Dot(vertex3 - vector2_10, vector2_1 - vector2_9) > 0.0)
          return;
      }
      contactFeature.IndexA = (byte) 1;
      contactFeature.TypeA = (byte) 0;
      manifold.PointCount = 1;
      manifold.Type = ManifoldType.Circles;
      manifold.LocalNormal = Vector2.Zero;
      manifold.LocalPoint = vector2_7;
      manifold.Points._00.Id.Key = 0U;
      manifold.Points._00.Id.Features = contactFeature;
      manifold.Points._00.LocalPoint = circleB.Position;
    }
    else
    {
      float num5 = Vector2.Dot(vector2_2, vector2_2);
      Vector2 vector2_11 = (vertex1 * num2 + vertex2 * num3) * (1f / num5);
      Vector2 vector2_12 = vector2_1 - vector2_11;
      if ((double) Vector2.Dot(vector2_12, vector2_12) > (double) num4 * (double) num4)
        return;
      if ((double) num1 < 0.0)
        vector2_3 = new Vector2(-vector2_3.X, -vector2_3.Y);
      Vector2 vector2_13 = Vector2Helpers.Normalized(vector2_3);
      contactFeature.IndexA = (byte) 0;
      contactFeature.TypeA = (byte) 1;
      manifold.PointCount = 1;
      manifold.Type = ManifoldType.FaceA;
      manifold.LocalNormal = vector2_13;
      manifold.LocalPoint = vertex1;
      manifold.Points._00.Id.Key = 0U;
      manifold.Points._00.Id.Features = contactFeature;
      manifold.Points._00.LocalPoint = circleB.Position;
    }
  }

  public unsafe void CollideEdgeAndPolygon(
    ref Manifold manifold,
    EdgeShape edgeA,
    in Transform xfA,
    PolygonShape polygonB,
    in Transform xfB)
  {
    manifold.PointCount = 0;
    Transform transform = Transform.MulT(in xfA, in xfB);
    Vector2 vector2_1 = Transform.Mul(in transform, polygonB.Centroid);
    Vector2 vertex1 = edgeA.Vertex1;
    Vector2 vertex2 = edgeA.Vertex2;
    Vector2 vector2_2 = Vector2Helpers.Normalized(vertex2 - vertex1);
    Vector2 normal1 = new Vector2(vector2_2.Y, -vector2_2.X);
    float num1 = Vector2.Dot(normal1, vector2_1 - vertex1);
    bool oneSided = edgeA.OneSided;
    if (oneSided && (double) num1 < 0.0)
      return;
    int vertexCount = polygonB.VertexCount;
    Vector2[] tempPolyVerts = new Vector2[vertexCount];
    Vector2[] tempPolyNorms = new Vector2[vertexCount];
    for (int index = 0; index < vertexCount; ++index)
    {
      tempPolyVerts[index] = Transform.Mul(in transform, in polygonB.Vertices[index]);
      tempPolyNorms[index] = Transform.Mul(in transform.Quaternion2D, in polygonB.Normals[index]);
    }
    float num2 = polygonB.Radius + edgeA.Radius;
    EPAxis edgeSeparation = CollisionManager.ComputeEdgeSeparation((Span<Vector2>) tempPolyVerts, vertex1, normal1);
    if ((double) edgeSeparation.Separation > (double) num2)
      return;
    EPAxis polygonSeparation = this.ComputePolygonSeparation((Span<Vector2>) tempPolyVerts, (Span<Vector2>) tempPolyNorms, vertex1, vertex2);
    if ((double) polygonSeparation.Separation > (double) num2)
      return;
    EPAxis epAxis = (double) polygonSeparation.Separation - (double) num2 <= 0.98000001907348633 * ((double) edgeSeparation.Separation - (double) num2) + 1.0 / 1000.0 ? edgeSeparation : polygonSeparation;
    if (oneSided)
    {
      Vector2 vector2_3 = Vector2Helpers.Normalized(vertex1 - edgeA.Vertex0);
      Vector2 vector2_4 = new Vector2(vector2_3.Y, -vector2_3.X);
      bool flag1 = (double) Vector2Helpers.Cross(vector2_3, vector2_2) >= 0.0;
      Vector2 vector2_5 = Vector2Helpers.Normalized(edgeA.Vertex3 - vertex2);
      Vector2 vector2_6 = new Vector2(vector2_5.Y, -vector2_5.X);
      bool flag2 = (double) Vector2Helpers.Cross(vector2_2, vector2_5) >= 0.0;
      if ((double) Vector2.Dot(epAxis.Normal, vector2_2) <= 0.0)
      {
        if (flag1)
        {
          if ((double) Vector2Helpers.Cross(epAxis.Normal, vector2_4) > 0.10000000149011612)
            return;
        }
        else
          epAxis = edgeSeparation;
      }
      else if (flag2)
      {
        if ((double) Vector2Helpers.Cross(vector2_6, epAxis.Normal) > 0.10000000149011612)
          return;
      }
      else
        epAxis = edgeSeparation;
    }
    ; // Unable to render the statement
    Span<ClipVertex> vIn = new Span<ClipVertex>((void*) pointer1, 2);
    ReferenceFace referenceFace;
    if (epAxis.Type == EPAxisType.EdgeA)
    {
      manifold.Type = ManifoldType.FaceA;
      int num3 = 0;
      float num4 = Vector2.Dot(epAxis.Normal, tempPolyNorms[0]);
      for (int index = 1; index < tempPolyVerts.Length; ++index)
      {
        float num5 = Vector2.Dot(epAxis.Normal, tempPolyNorms[index]);
        if ((double) num5 < (double) num4)
        {
          num4 = num5;
          num3 = index;
        }
      }
      int index1 = num3;
      int index2 = index1 + 1 < tempPolyVerts.Length ? index1 + 1 : 0;
      vIn[0].V = tempPolyVerts[index1];
      vIn[0].ID.Features.IndexA = (byte) 0;
      vIn[0].ID.Features.IndexB = (byte) index1;
      vIn[0].ID.Features.TypeA = (byte) 1;
      vIn[0].ID.Features.TypeB = (byte) 0;
      vIn[1].V = tempPolyVerts[index2];
      vIn[1].ID.Features.IndexA = (byte) 0;
      vIn[1].ID.Features.IndexB = (byte) index2;
      vIn[1].ID.Features.TypeA = (byte) 1;
      vIn[1].ID.Features.TypeB = (byte) 0;
      referenceFace.i1 = 0;
      referenceFace.i2 = 1;
      referenceFace.v1 = vertex1;
      referenceFace.v2 = vertex2;
      referenceFace.normal = epAxis.Normal;
      referenceFace.sideNormal1 = -vector2_2;
      referenceFace.sideNormal2 = vector2_2;
    }
    else
    {
      manifold.Type = ManifoldType.FaceB;
      vIn[0].V = vertex2;
      vIn[0].ID.Features.IndexA = (byte) 1;
      vIn[0].ID.Features.IndexB = (byte) epAxis.Index;
      vIn[0].ID.Features.TypeA = (byte) 0;
      vIn[0].ID.Features.TypeB = (byte) 1;
      vIn[1].V = vertex1;
      vIn[1].ID.Features.IndexA = (byte) 0;
      vIn[1].ID.Features.IndexB = (byte) epAxis.Index;
      vIn[1].ID.Features.TypeA = (byte) 0;
      vIn[1].ID.Features.TypeB = (byte) 1;
      referenceFace.i1 = epAxis.Index;
      referenceFace.i2 = referenceFace.i1 + 1 < vertexCount ? referenceFace.i1 + 1 : 0;
      referenceFace.v1 = tempPolyVerts[referenceFace.i1];
      referenceFace.v2 = tempPolyVerts[referenceFace.i2];
      referenceFace.normal = tempPolyNorms[referenceFace.i1];
      referenceFace.sideNormal1 = new Vector2(referenceFace.normal.Y, -referenceFace.normal.X);
      referenceFace.sideNormal2 = -referenceFace.sideNormal1;
    }
    referenceFace.sideOffset1 = Vector2.Dot(referenceFace.sideNormal1, referenceFace.v1);
    referenceFace.sideOffset2 = Vector2.Dot(referenceFace.sideNormal2, referenceFace.v2);
    ; // Unable to render the statement
    Span<ClipVertex> span = new Span<ClipVertex>((void*) pointer2, 2);
    ; // Unable to render the statement
    Span<ClipVertex> vOut = new Span<ClipVertex>((void*) pointer3, 2);
    if (CollisionManager.ClipSegmentToLine(span, vIn, referenceFace.sideNormal1, referenceFace.sideOffset1, referenceFace.i1) < 2 || CollisionManager.ClipSegmentToLine(vOut, span, referenceFace.sideNormal2, referenceFace.sideOffset2, referenceFace.i2) < 2)
      return;
    if (epAxis.Type == EPAxisType.EdgeA)
    {
      manifold.LocalNormal = referenceFace.normal;
      manifold.LocalPoint = referenceFace.v1;
    }
    else
    {
      manifold.LocalNormal = tempPolyNorms[referenceFace.i1];
      manifold.LocalPoint = tempPolyVerts[referenceFace.i1];
    }
    int index3 = 0;
    Span<ManifoldPoint> asSpan = manifold.Points.AsSpan;
    for (int index4 = 0; index4 < 2; ++index4)
    {
      if ((double) Vector2.Dot(referenceFace.normal, vOut[index4].V - referenceFace.v1) <= (double) num2)
      {
        ref ManifoldPoint local = ref asSpan[index3];
        if (epAxis.Type == EPAxisType.EdgeA)
        {
          local.LocalPoint = Transform.MulT(in transform, in vOut[index4].V);
          local.Id = vOut[index4].ID;
        }
        else
        {
          local.LocalPoint = vOut[index4].V;
          local.Id.Features.TypeA = vOut[index4].ID.Features.TypeB;
          local.Id.Features.TypeB = vOut[index4].ID.Features.TypeA;
          local.Id.Features.IndexA = vOut[index4].ID.Features.IndexB;
          local.Id.Features.IndexB = vOut[index4].ID.Features.IndexA;
        }
        ++index3;
      }
    }
    manifold.PointCount = index3;
  }

  private static EPAxis ComputeEdgeSeparation(
    Span<Vector2> tempPolyVerts,
    Vector2 v1,
    Vector2 normal1)
  {
    EPAxis edgeSeparation = new EPAxis()
    {
      Type = EPAxisType.EdgeA,
      Index = -1,
      Separation = float.MinValue,
      Normal = Vector2.Zero
    };
    Span<Vector2> span = stackalloc Vector2[2]
    {
      normal1,
      -normal1
    };
    for (int index1 = 0; index1 < 2; ++index1)
    {
      float num1 = float.MaxValue;
      for (int index2 = 0; index2 < tempPolyVerts.Length; ++index2)
      {
        float num2 = Vector2.Dot(span[index1], tempPolyVerts[index2] - v1);
        if ((double) num2 < (double) num1)
          num1 = num2;
      }
      if ((double) num1 > (double) edgeSeparation.Separation)
      {
        edgeSeparation.Index = index1;
        edgeSeparation.Separation = num1;
        edgeSeparation.Normal = span[index1];
      }
    }
    return edgeSeparation;
  }

  private EPAxis ComputePolygonSeparation(
    Span<Vector2> tempPolyVerts,
    Span<Vector2> tempPolyNorms,
    Vector2 v1,
    Vector2 v2)
  {
    EPAxis polygonSeparation = new EPAxis()
    {
      Type = EPAxisType.Unknown,
      Index = -1,
      Separation = float.MinValue,
      Normal = Vector2.Zero
    };
    for (int index = 0; index < tempPolyVerts.Length; ++index)
    {
      Vector2 vector2 = -tempPolyNorms[index];
      float num = MathF.Min(Vector2.Dot(vector2, tempPolyVerts[index] - v1), Vector2.Dot(vector2, tempPolyVerts[index] - v2));
      if ((double) num > (double) polygonSeparation.Separation)
      {
        polygonSeparation.Type = EPAxisType.EdgeB;
        polygonSeparation.Index = index;
        polygonSeparation.Separation = num;
        polygonSeparation.Normal = vector2;
      }
    }
    return polygonSeparation;
  }

  public bool TestOverlap<T, U>(
    T shapeA,
    int indexA,
    U shapeB,
    int indexB,
    in Transform xfA,
    in Transform xfB)
    where T : IPhysShape
    where U : IPhysShape
  {
    DistanceInput input = new DistanceInput();
    input.ProxyA.Set<T>(shapeA, indexA);
    input.ProxyB.Set<U>(shapeB, indexB);
    input.TransformA = xfA;
    input.TransformB = xfB;
    input.UseRadii = true;
    DistanceOutput output;
    DistanceManager.ComputeDistance(out output, out SimplexCache _, in input);
    return (double) output.Distance < 1.4012984643248171E-44;
  }

  public void CollidePolygonAndCircle(
    ref Manifold manifold,
    PolygonShape polygonA,
    in Transform xfA,
    PhysShapeCircle circleB,
    in Transform xfB)
  {
    manifold.PointCount = 0;
    Vector2 v = Transform.Mul(in xfB, in circleB.Position);
    Vector2 vector2_1 = Transform.MulT(in xfA, in v);
    int index1 = 0;
    float num1 = float.MinValue;
    float num2 = polygonA.Radius + circleB.Radius;
    int vertexCount = polygonA.VertexCount;
    Vector2[] vertices = polygonA.Vertices;
    Vector2[] normals = polygonA.Normals;
    for (int index2 = 0; index2 < vertexCount; ++index2)
    {
      float num3 = Vector2.Dot(normals[index2], vector2_1 - vertices[index2]);
      if ((double) num3 > (double) num2)
        return;
      if ((double) num3 > (double) num1)
      {
        num1 = num3;
        index1 = index2;
      }
    }
    int index3 = index1;
    int index4 = index3 + 1 < vertexCount ? index3 + 1 : 0;
    Vector2 vector2_2 = vertices[index3];
    Vector2 vector2_3 = vertices[index4];
    if ((double) num1 < 1.4012984643248171E-45)
    {
      manifold.PointCount = 1;
      manifold.Type = ManifoldType.FaceA;
      manifold.LocalNormal = normals[index1];
      manifold.LocalPoint = (vector2_2 + vector2_3) * 0.5f;
      ref ManifoldPoint local = ref manifold.Points._00;
      local.LocalPoint = circleB.Position;
      local.Id.Key = 0U;
    }
    else
    {
      double num4 = (double) Vector2.Dot(vector2_1 - vector2_2, vector2_3 - vector2_2);
      float num5 = Vector2.Dot(vector2_1 - vector2_3, vector2_2 - vector2_3);
      if (num4 <= 0.0)
      {
        if ((double) (vector2_1 - vector2_2).LengthSquared() > (double) num2 * (double) num2)
          return;
        manifold.PointCount = 1;
        manifold.Type = ManifoldType.FaceA;
        manifold.LocalNormal = Vector2Helpers.Normalized(vector2_1 - vector2_2);
        manifold.LocalPoint = vector2_2;
        ref ManifoldPoint local = ref manifold.Points._00;
        local.LocalPoint = circleB.Position;
        local.Id.Key = 0U;
      }
      else if ((double) num5 <= 0.0)
      {
        if ((double) (vector2_1 - vector2_3).LengthSquared() > (double) num2 * (double) num2)
          return;
        manifold.PointCount = 1;
        manifold.Type = ManifoldType.FaceA;
        manifold.LocalNormal = Vector2Helpers.Normalized(vector2_1 - vector2_3);
        manifold.LocalPoint = vector2_3;
        ref ManifoldPoint local = ref manifold.Points._00;
        local.LocalPoint = circleB.Position;
        local.Id.Key = 0U;
      }
      else
      {
        Vector2 vector2_4 = (vector2_2 + vector2_3) * 0.5f;
        if ((double) Vector2.Dot(vector2_1 - vector2_4, normals[index3]) > (double) num2)
          return;
        manifold.PointCount = 1;
        manifold.Type = ManifoldType.FaceA;
        manifold.LocalNormal = normals[index3];
        manifold.LocalPoint = vector2_4;
        ref ManifoldPoint local = ref manifold.Points._00;
        local.LocalPoint = circleB.Position;
        local.Id.Key = 0U;
      }
    }
  }

  private static float FindMaxSeparation(
    out int edgeIndex,
    PolygonShape poly1,
    in Transform xf1,
    PolygonShape poly2,
    in Transform xf2)
  {
    Vector2[] normals = poly1.Normals;
    Vector2[] vertices1 = poly1.Vertices;
    Vector2[] vertices2 = poly2.Vertices;
    int vertexCount1 = poly1.VertexCount;
    int vertexCount2 = poly2.VertexCount;
    Transform transform = Transform.MulT(in xf2, in xf1);
    int num1 = 0;
    float maxSeparation = float.MinValue;
    for (int index1 = 0; index1 < vertexCount1; ++index1)
    {
      Vector2 vector2_1 = Transform.Mul(in transform.Quaternion2D, in normals[index1]);
      Vector2 vector2_2 = Transform.Mul(in transform, in vertices1[index1]);
      float num2 = float.MaxValue;
      for (int index2 = 0; index2 < vertexCount2; ++index2)
      {
        float num3 = Vector2.Dot(vector2_1, vertices2[index2] - vector2_2);
        if ((double) num3 < (double) num2)
          num2 = num3;
      }
      if ((double) num2 > (double) maxSeparation)
      {
        maxSeparation = num2;
        num1 = index1;
      }
    }
    edgeIndex = num1;
    return maxSeparation;
  }

  private static void FindIncidentEdge(
    Span<ClipVertex> c,
    PolygonShape poly1,
    in Transform xf1,
    int edge1,
    PolygonShape poly2,
    in Transform xf2)
  {
    Vector2[] normals1 = poly1.Normals;
    int vertexCount = poly2.VertexCount;
    Vector2[] vertices = poly2.Vertices;
    Vector2[] normals2 = poly2.Normals;
    Vector2 vector2 = Transform.MulT(xf2.Quaternion2D, Transform.Mul(in xf1.Quaternion2D, in normals1[edge1]));
    int num1 = 0;
    float num2 = float.MaxValue;
    for (int index = 0; index < vertexCount; ++index)
    {
      float num3 = Vector2.Dot(vector2, normals2[index]);
      if ((double) num3 < (double) num2)
      {
        num2 = num3;
        num1 = index;
      }
    }
    int index1 = num1;
    int index2 = index1 + 1 < vertexCount ? index1 + 1 : 0;
    ref ClipVertex local1 = ref c[0];
    local1.V = Transform.Mul(in xf2, in vertices[index1]);
    local1.ID.Features.IndexA = (byte) edge1;
    local1.ID.Features.IndexB = (byte) index1;
    local1.ID.Features.TypeA = (byte) 1;
    local1.ID.Features.TypeB = (byte) 0;
    ref ClipVertex local2 = ref c[1];
    local2.V = Transform.Mul(in xf2, in vertices[index2]);
    local2.ID.Features.IndexA = (byte) edge1;
    local2.ID.Features.IndexB = (byte) index2;
    local2.ID.Features.TypeA = (byte) 1;
    local2.ID.Features.TypeB = (byte) 0;
  }

  public unsafe void CollidePolygons(
    ref Manifold manifold,
    PolygonShape polyA,
    in Transform transformA,
    PolygonShape polyB,
    in Transform transformB)
  {
    manifold.PointCount = 0;
    float num1 = polyA.Radius + polyB.Radius;
    int edgeIndex1 = 0;
    float maxSeparation1 = CollisionManager.FindMaxSeparation(out edgeIndex1, polyA, in transformA, polyB, in transformB);
    if ((double) maxSeparation1 > (double) num1)
      return;
    int edgeIndex2 = 0;
    float maxSeparation2 = CollisionManager.FindMaxSeparation(out edgeIndex2, polyB, in transformB, polyA, in transformA);
    if ((double) maxSeparation2 > (double) num1)
      return;
    PolygonShape poly1;
    PolygonShape poly2;
    Transform transform1;
    Transform transform2;
    int edge1;
    bool flag;
    if ((double) maxSeparation2 > 0.98000001907348633 * (double) maxSeparation1 + 1.0 / 1000.0)
    {
      poly1 = polyB;
      poly2 = polyA;
      transform1 = transformB;
      transform2 = transformA;
      edge1 = edgeIndex2;
      manifold.Type = ManifoldType.FaceB;
      flag = true;
    }
    else
    {
      poly1 = polyA;
      poly2 = polyB;
      transform1 = transformA;
      transform2 = transformB;
      edge1 = edgeIndex1;
      manifold.Type = ManifoldType.FaceA;
      flag = false;
    }
    ; // Unable to render the statement
    Span<ClipVertex> span1 = new Span<ClipVertex>((void*) pointer1, 2);
    CollisionManager.FindIncidentEdge(span1, poly1, in transform1, edge1, poly2, in transform2);
    int vertexCount = poly1.VertexCount;
    int vertexIndexA1 = edge1;
    int vertexIndexA2 = edge1 + 1 < vertexCount ? edge1 + 1 : 0;
    Vector2 vector1 = poly1.Vertices[vertexIndexA1];
    Vector2 vector2 = poly1.Vertices[vertexIndexA2];
    Vector2 vector3 = Vector2Helpers.Normalized(vector2 - vector1);
    Vector2 vector2_1 = new Vector2(vector3.Y, -vector3.X);
    Vector2 vector2_2 = (vector1 + vector2) * 0.5f;
    Vector2 normal = Transform.Mul(in transform1.Quaternion2D, in vector3);
    float y = normal.Y;
    float num2 = -normal.X;
    Vector2 vector2_3 = Transform.Mul(in transform1, in vector1);
    vector2 = Transform.Mul(in transform1, in vector2);
    float num3 = (float) ((double) y * (double) vector2_3.X + (double) num2 * (double) vector2_3.Y);
    float offset1 = (float) -((double) normal.X * (double) vector2_3.X + (double) normal.Y * (double) vector2_3.Y) + num1;
    float offset2 = (float) ((double) normal.X * (double) vector2.X + (double) normal.Y * (double) vector2.Y) + num1;
    ; // Unable to render the statement
    Span<ClipVertex> span2 = new Span<ClipVertex>((void*) pointer2, 2);
    if (CollisionManager.ClipSegmentToLine(span2, span1, -normal, offset1, vertexIndexA1) < 2)
      return;
    ; // Unable to render the statement
    Span<ClipVertex> vOut = new Span<ClipVertex>((void*) pointer3, 2);
    if (CollisionManager.ClipSegmentToLine(vOut, span2, normal, offset2, vertexIndexA2) < 2)
      return;
    manifold.LocalNormal = vector2_1;
    manifold.LocalPoint = vector2_2;
    int index1 = 0;
    Span<ManifoldPoint> asSpan = manifold.Points.AsSpan;
    for (int index2 = 0; index2 < 2; ++index2)
    {
      Vector2 v = vOut[index2].V;
      if ((double) y * (double) v.X + (double) num2 * (double) v.Y - (double) num3 <= (double) num1)
      {
        ref ManifoldPoint local = ref asSpan[index1];
        local.LocalPoint = Transform.MulT(in transform2, in vOut[index2].V);
        local.Id = vOut[index2].ID;
        if (flag)
        {
          ContactFeature features = local.Id.Features;
          local.Id.Features.IndexA = features.IndexB;
          local.Id.Features.IndexB = features.IndexA;
          local.Id.Features.TypeA = features.TypeB;
          local.Id.Features.TypeB = features.TypeA;
        }
        ++index1;
      }
    }
    manifold.PointCount = index1;
  }

  bool IManifoldManager.TestOverlap<T, U>(
    T shapeA,
    int indexA,
    U shapeB,
    int indexB,
    in Transform xfA,
    in Transform xfB)
  {
    return this.TestOverlap<T, U>(shapeA, indexA, shapeB, indexB, in xfA, in xfB);
  }

  void IManifoldManager.CollideCircles(
    ref Manifold manifold,
    PhysShapeCircle circleA,
    in Transform xfA,
    PhysShapeCircle circleB,
    in Transform xfB)
  {
    this.CollideCircles(ref manifold, circleA, in xfA, circleB, in xfB);
  }

  void IManifoldManager.CollideEdgeAndCircle(
    ref Manifold manifold,
    EdgeShape edgeA,
    in Transform transformA,
    PhysShapeCircle circleB,
    in Transform transformB)
  {
    this.CollideEdgeAndCircle(ref manifold, edgeA, in transformA, circleB, in transformB);
  }

  void IManifoldManager.CollideEdgeAndPolygon(
    ref Manifold manifold,
    EdgeShape edgeA,
    in Transform xfA,
    PolygonShape polygonB,
    in Transform xfB)
  {
    this.CollideEdgeAndPolygon(ref manifold, edgeA, in xfA, polygonB, in xfB);
  }

  void IManifoldManager.CollidePolygonAndCircle(
    ref Manifold manifold,
    PolygonShape polygonA,
    in Transform xfA,
    PhysShapeCircle circleB,
    in Transform xfB)
  {
    this.CollidePolygonAndCircle(ref manifold, polygonA, in xfA, circleB, in xfB);
  }

  void IManifoldManager.CollidePolygons(
    ref Manifold manifold,
    PolygonShape polyA,
    in Transform transformA,
    PolygonShape polyB,
    in Transform transformB)
  {
    this.CollidePolygons(ref manifold, polyA, in transformA, polyB, in transformB);
  }
}
