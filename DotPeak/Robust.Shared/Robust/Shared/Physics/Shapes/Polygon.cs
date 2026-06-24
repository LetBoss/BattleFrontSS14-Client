// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Shapes.Polygon
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Shapes;

internal record struct Polygon : IPhysShape, IEquatable<IPhysShape>
{
  [DataField(null, false, 1, false, false, null)]
  internal FixedArray8<Vector2> _vertices;
  internal FixedArray8<Vector2> _normals;
  public Vector2 Centroid;

  [DataField(null, false, 1, false, false, null)]
  public byte VertexCount { get; internal set; }

  public Vector2[] Vertices => this._vertices.AsSpan.Slice(0, (int) this.VertexCount).ToArray();

  public Vector2[] Normals => this._normals.AsSpan.Slice(0, (int) this.VertexCount).ToArray();

  public int ChildCount => 1;

  public float Radius { get; set; }

  public ShapeType ShapeType => ShapeType.Polygon;

  public Polygon(IPhysShape shape)
    : this((PolygonShape) shape)
  {
  }

  public Polygon(PhysShapeAabb aabb)
    : this(aabb.LocalBounds)
  {
  }

  public Polygon(PolygonShape polyShape)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<Polygon>(out this);
    this.Radius = polyShape.Radius;
    this.Centroid = polyShape.Centroid;
    this.VertexCount = (byte) polyShape.VertexCount;
    polyShape.Vertices.AsSpan<Vector2>().Slice(0, (int) this.VertexCount).CopyTo(this._vertices.AsSpan);
    polyShape.Normals.AsSpan<Vector2>().Slice(0, (int) this.VertexCount).CopyTo(this._normals.AsSpan);
  }

  internal Polygon(SlimPolygon slim)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<Polygon>(out this);
    this.Radius = slim.Radius;
    this.VertexCount = slim.VertexCount;
    this._vertices._00 = slim._vertices._00;
    this._vertices._01 = slim._vertices._01;
    this._vertices._02 = slim._vertices._02;
    this._vertices._03 = slim._vertices._03;
    this._normals._00 = slim._normals._00;
    this._normals._01 = slim._normals._01;
    this._normals._02 = slim._normals._02;
    this._normals._03 = slim._normals._03;
    this.Centroid = slim.Centroid;
  }

  public Polygon(Box2 box)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<Polygon>(out this);
    this.Radius = 0.0f;
    this.VertexCount = (byte) 4;
    this._vertices._00 = box.BottomLeft;
    this._vertices._01 = ((Box2) ref box).BottomRight;
    this._vertices._02 = box.TopRight;
    this._vertices._03 = ((Box2) ref box).TopLeft;
    this._normals._00 = new Vector2(0.0f, -1f);
    this._normals._01 = new Vector2(1f, 0.0f);
    this._normals._02 = new Vector2(0.0f, 1f);
    this._normals._03 = new Vector2(-1f, 0.0f);
  }

  public Polygon(Box2Rotated bounds)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<Polygon>(out this);
    this.Radius = 0.0f;
    this.VertexCount = (byte) 4;
    this._vertices._00 = ((Box2Rotated) ref bounds).BottomLeft;
    this._vertices._01 = ((Box2Rotated) ref bounds).BottomRight;
    this._vertices._02 = ((Box2Rotated) ref bounds).TopRight;
    this._vertices._03 = ((Box2Rotated) ref bounds).TopLeft;
    Polygon.CalculateNormals((ReadOnlySpan<Vector2>) this._vertices.AsSpan, this._normals.AsSpan, 4);
    this.Centroid = ((Box2Rotated) ref bounds).Center;
  }

  internal Polygon(
    ReadOnlySpan<Vector2> vertices,
    ReadOnlySpan<Vector2> normals,
    Vector2 centroid,
    byte count)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<Polygon>(out this);
    vertices.Slice(0, (int) this.VertexCount).CopyTo(this._vertices.AsSpan);
    normals.Slice(0, (int) this.VertexCount).CopyTo(this._normals.AsSpan);
    this.Centroid = centroid;
    this.VertexCount = count;
    this.Radius = 0.0f;
  }

  public Polygon(Vector2[] vertices)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<Polygon>(out this);
    InternalPhysicsHull hull = InternalPhysicsHull.ComputeHull((ReadOnlySpan<Vector2>) vertices, vertices.Length);
    if (hull.Count < 3)
    {
      this.VertexCount = (byte) 0;
    }
    else
    {
      this.VertexCount = (byte) vertices.Length;
      Span<Vector2> asSpan = this._vertices.AsSpan;
      vertices.AsSpan<Vector2>().CopyTo(asSpan);
      this.Set(hull);
      this.Centroid = Polygon.ComputeCentroid((ReadOnlySpan<Vector2>) asSpan);
    }
  }

  public static explicit operator Polygon(PolygonShape polyShape) => new Polygon(polyShape);

  private void Set(InternalPhysicsHull hull)
  {
    int count = hull.Count;
    Span<Vector2> asSpan1 = this._vertices.AsSpan;
    Span<Vector2> asSpan2 = this._normals.AsSpan;
    for (int index = 0; index < count; ++index)
      asSpan1[index] = hull.Points[index];
    Polygon.CalculateNormals((ReadOnlySpan<Vector2>) asSpan1, asSpan2, count);
  }

  public static void CalculateNormals(
    ReadOnlySpan<Vector2> vertices,
    Span<Vector2> normals,
    int count)
  {
    for (int index1 = 0; index1 < count; ++index1)
    {
      int index2 = index1 + 1 < count ? index1 + 1 : 0;
      Vector2 vector2_1 = vertices[index2] - vertices[index1];
      ref Vector2 local1 = ref vector2_1;
      float num = 1f;
      ref float local2 = ref num;
      Vector2 vector2_2 = Vector2Helpers.Cross(ref local1, ref local2);
      normals[index1] = Vector2Helpers.Normalized(vector2_2);
    }
  }

  public static Vector2 ComputeCentroid(ReadOnlySpan<Vector2> vs)
  {
    int length = vs.Length;
    Vector2 vector2_1 = new Vector2(0.0f, 0.0f);
    float num1 = 0.0f;
    Vector2 vector2_2 = vs[0];
    for (int index = 0; index < length; ++index)
    {
      Vector2 vector2_3 = vs[0] - vector2_2;
      Vector2 vector2_4 = vs[index] - vector2_2;
      Vector2 vector2_5 = index + 1 < length ? vs[index + 1] - vector2_2 : vs[0] - vector2_2;
      float num2 = 0.5f * Vector2Helpers.Cross(vector2_4 - vector2_3, vector2_5 - vector2_3);
      num1 += num2;
      vector2_1 += (vector2_3 + vector2_4 + vector2_5) * num2 * 0.333333343f;
    }
    return vector2_1 * (1f / num1) + vector2_2;
  }

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    Span<Vector2> asSpan = this._vertices.AsSpan;
    Vector2 vector2_1 = Transform.Mul(in transform, in asSpan[0]);
    Vector2 vector2_2 = vector2_1;
    for (int index = 1; index < (int) this.VertexCount; ++index)
    {
      Vector2 vector2_3 = Transform.Mul(in transform, in asSpan[index]);
      vector2_1 = Vector2.Min(vector2_1, vector2_3);
      vector2_2 = Vector2.Max(vector2_2, vector2_3);
    }
    Vector2 vector2_4 = new Vector2(this.Radius, this.Radius);
    return new Box2(vector2_1 - vector2_4, vector2_2 + vector2_4);
  }

  public bool Equals(IPhysShape? other)
  {
    switch (other)
    {
      case SlimPolygon other1:
        return this.Equals(other1);
      case Polygon other2:
        return this.Equals(other2);
      default:
        return false;
    }
  }

  public bool Equals(Polygon other)
  {
    if ((int) this.VertexCount != (int) other.VertexCount)
      return false;
    Span<Vector2> asSpan1 = this._vertices.AsSpan;
    Span<Vector2> asSpan2 = other._vertices.AsSpan;
    for (int index = 0; index < (int) this.VertexCount; ++index)
    {
      if (!asSpan1[index].Equals(asSpan2[index]))
        return false;
    }
    return true;
  }

  public bool Equals(SlimPolygon other)
  {
    if ((int) this.VertexCount != (int) other.VertexCount)
      return false;
    Span<Vector2> asSpan1 = this._vertices.AsSpan;
    Span<Vector2> asSpan2 = other._vertices.AsSpan;
    for (int index = 0; index < (int) this.VertexCount; ++index)
    {
      if (!asSpan1[index].Equals(asSpan2[index]))
        return false;
    }
    return true;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<byte, Vector2[], float>(this.VertexCount, this._vertices.AsSpan.ToArray(), this.Radius);
  }
}
