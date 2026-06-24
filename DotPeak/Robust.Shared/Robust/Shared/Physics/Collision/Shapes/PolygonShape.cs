// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Shapes.PolygonShape
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Physics.Collision.Shapes;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class PolygonShape : 
  IPhysShape,
  IEquatable<IPhysShape>,
  ISerializationHooks,
  IEquatable<PolygonShape>,
  IApproxEquatable<PolygonShape>,
  ISerializationGenerated<PolygonShape>,
  ISerializationGenerated
{
  [DataField("vertices", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public Vector2[] Vertices = Array.Empty<Vector2>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public Vector2[] Normals = Array.Empty<Vector2>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public int VertexCount => this.Vertices.Length;

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public Vector2 Centroid { get; internal set; } = Vector2.Zero;

  public int ChildCount => 1;

  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Radius { get; set; } = 0.01f;

  public bool Set(List<Vector2> vertices)
  {
    return this.Set((ReadOnlySpan<Vector2>) CollectionsMarshal.AsSpan<Vector2>(vertices), vertices.Count);
  }

  public bool Set(ReadOnlySpan<Vector2> vertices, int count)
  {
    InternalPhysicsHull hull = InternalPhysicsHull.ComputeHull(vertices, count);
    if (hull.Count < 3)
      return false;
    this.Set(hull);
    return true;
  }

  internal void Set(InternalPhysicsHull hull)
  {
    int count = hull.Count;
    Array.Resize<Vector2>(ref this.Vertices, count);
    Array.Resize<Vector2>(ref this.Normals, count);
    for (int index = 0; index < count; ++index)
      this.Vertices[index] = hull.Points[index];
    for (int index = 0; index < count; ++index)
    {
      Vector2 vector2_1 = this.Vertices[index + 1 < count ? index + 1 : 0] - this.Vertices[index];
      ref Vector2 local1 = ref vector2_1;
      float num = 1f;
      ref float local2 = ref num;
      Vector2 vector2_2 = Vector2Helpers.Cross(ref local1, ref local2);
      this.Normals[index] = Vector2Helpers.Normalized(vector2_2);
    }
    this.Centroid = PolygonShape.ComputeCentroid(this.Vertices, this.VertexCount);
  }

  public bool Validate()
  {
    int vertexCount = this.VertexCount;
    if (vertexCount < 3 || vertexCount > 8)
      return false;
    InternalPhysicsHull hull = new InternalPhysicsHull();
    for (int index = 0; index < vertexCount; ++index)
      hull.Points[index] = this.Vertices[index];
    hull.Count = vertexCount;
    return InternalPhysicsHull.ValidateHull(hull);
  }

  private static Vector2 ComputeCentroid(Vector2[] vs, int count)
  {
    Vector2 vector2_1 = new Vector2(0.0f, 0.0f);
    float num1 = 0.0f;
    Vector2 v = vs[0];
    for (int index = 0; index < count; ++index)
    {
      Vector2 vector2_2 = vs[0] - v;
      Vector2 vector2_3 = vs[index] - v;
      Vector2 vector2_4 = index + 1 < count ? vs[index + 1] - v : vs[0] - v;
      float num2 = 0.5f * Vector2Helpers.Cross(vector2_3 - vector2_2, vector2_4 - vector2_2);
      num1 += num2;
      vector2_1 += (vector2_2 + vector2_3 + vector2_4) * num2 * 0.333333343f;
    }
    return vector2_1 * (1f / num1) + v;
  }

  public ShapeType ShapeType => ShapeType.Polygon;

  public PolygonShape()
  {
  }

  internal PolygonShape(SlimPolygon poly)
  {
    this.Vertices = new Vector2[(int) poly.VertexCount];
    this.Normals = new Vector2[(int) poly.VertexCount];
    poly._vertices.AsSpan.Slice(0, this.VertexCount).CopyTo((Span<Vector2>) this.Vertices);
    poly._normals.AsSpan.Slice(0, this.VertexCount).CopyTo((Span<Vector2>) this.Normals);
    this.Centroid = poly.Centroid;
  }

  internal PolygonShape(Polygon poly)
  {
    this.Vertices = new Vector2[(int) poly.VertexCount];
    this.Normals = new Vector2[(int) poly.VertexCount];
    poly._vertices.AsSpan.Slice(0, this.VertexCount).CopyTo((Span<Vector2>) this.Vertices);
    poly._normals.AsSpan.Slice(0, this.VertexCount).CopyTo((Span<Vector2>) this.Normals);
    this.Centroid = poly.Centroid;
  }

  public PolygonShape(float radius) => this.Radius = radius;

  void ISerializationHooks.AfterDeserialization()
  {
    this.Set((ReadOnlySpan<Vector2>) this.Vertices.AsSpan<Vector2>(), this.VertexCount);
  }

  public unsafe void Set(Box2Rotated bounds)
  {
    ; // Unable to render the statement
    Span<Vector2> vertices = new Span<Vector2>((void*) pointer, 4);
    vertices[0] = ((Box2Rotated) ref bounds).BottomLeft;
    vertices[1] = ((Box2Rotated) ref bounds).BottomRight;
    vertices[2] = ((Box2Rotated) ref bounds).TopRight;
    vertices[3] = ((Box2Rotated) ref bounds).TopLeft;
    this.Set(new InternalPhysicsHull(vertices, 4));
  }

  public void SetAsBox(Box2 box)
  {
    Array.Resize<Vector2>(ref this.Vertices, 4);
    Array.Resize<Vector2>(ref this.Normals, 4);
    this.Vertices[0] = box.BottomLeft;
    this.Vertices[1] = ((Box2) ref box).BottomRight;
    this.Vertices[2] = box.TopRight;
    this.Vertices[3] = ((Box2) ref box).TopLeft;
    this.Normals[0] = new Vector2(0.0f, -1f);
    this.Normals[1] = new Vector2(1f, 0.0f);
    this.Normals[2] = new Vector2(0.0f, 1f);
    this.Normals[3] = new Vector2(-1f, 0.0f);
    this.Centroid = ((Box2) ref box).Center;
  }

  public void SetAsBox(float halfWidth, float halfHeight)
  {
    Array.Resize<Vector2>(ref this.Vertices, 4);
    Array.Resize<Vector2>(ref this.Normals, 4);
    this.Vertices[0] = new Vector2(-halfWidth, -halfHeight);
    this.Vertices[1] = new Vector2(halfWidth, -halfHeight);
    this.Vertices[2] = new Vector2(halfWidth, halfHeight);
    this.Vertices[3] = new Vector2(-halfWidth, halfHeight);
    this.Normals[0] = new Vector2(0.0f, -1f);
    this.Normals[1] = new Vector2(1f, 0.0f);
    this.Normals[2] = new Vector2(0.0f, 1f);
    this.Normals[3] = new Vector2(-1f, 0.0f);
    this.Centroid = Vector2.Zero;
  }

  public void SetAsBox(float halfWidth, float halfHeight, Vector2 center, float angle)
  {
    Array.Resize<Vector2>(ref this.Vertices, 4);
    Array.Resize<Vector2>(ref this.Normals, 4);
    this.Vertices[0] = new Vector2(-halfWidth, -halfHeight);
    this.Vertices[1] = new Vector2(halfWidth, -halfHeight);
    this.Vertices[2] = new Vector2(halfWidth, halfHeight);
    this.Vertices[3] = new Vector2(-halfWidth, halfHeight);
    this.Normals[0] = new Vector2(0.0f, -1f);
    this.Normals[1] = new Vector2(1f, 0.0f);
    this.Normals[2] = new Vector2(0.0f, 1f);
    this.Normals[3] = new Vector2(-1f, 0.0f);
    this.Centroid = center;
    Transform transform = new Transform(center, angle);
    for (int index = 0; index < this.VertexCount; ++index)
    {
      this.Vertices[index] = Transform.Mul(in transform, in this.Vertices[index]);
      this.Normals[index] = Transform.Mul(in transform.Quaternion2D, in this.Normals[index]);
    }
  }

  public bool Equals(IPhysShape? other)
  {
    if (!(other is PolygonShape polygonShape) || this.VertexCount != polygonShape.VertexCount)
      return false;
    for (int index = 0; index < this.VertexCount; ++index)
    {
      if (!this.Vertices[index].Equals(polygonShape.Vertices[index]))
        return false;
    }
    return true;
  }

  public bool EqualsApprox(PolygonShape other) => this.EqualsApprox(other, 0.001);

  public bool EqualsApprox(PolygonShape other, double tolerance)
  {
    if (this.VertexCount != other.VertexCount || !MathHelper.CloseTo((double) this.Radius, (double) other.Radius, tolerance))
      return false;
    for (int index = 0; index < this.VertexCount; ++index)
    {
      if (!Vector2Helpers.EqualsApprox(this.Vertices[index], other.Vertices[index], tolerance))
        return false;
    }
    return true;
  }

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    Vector2 vector2_1 = Transform.Mul(in transform, in this.Vertices[0]);
    Vector2 vector2_2 = vector2_1;
    for (int index = 1; index < this.VertexCount; ++index)
    {
      Vector2 vector2_3 = Transform.Mul(in transform, in this.Vertices[index]);
      vector2_1 = Vector2.Min(vector2_1, vector2_3);
      vector2_2 = Vector2.Max(vector2_2, vector2_3);
    }
    Vector2 vector2_4 = new Vector2(this.Radius, this.Radius);
    return new Box2(vector2_1 - vector2_4, vector2_2 + vector2_4);
  }

  public static explicit operator PolygonShape(PhysShapeAabb aabb)
  {
    Box2 localBounds = aabb.LocalBounds;
    return new PolygonShape(aabb.Radius)
    {
      Vertices = new Vector2[4]
      {
        localBounds.BottomLeft,
        ((Box2) ref localBounds).BottomRight,
        localBounds.TopRight,
        ((Box2) ref localBounds).TopLeft
      },
      Normals = new Vector2[4]
      {
        new Vector2(0.0f, -1f),
        new Vector2(1f, 0.0f),
        new Vector2(0.0f, 1f),
        new Vector2(-1f, 0.0f)
      }
    };
  }

  public bool Equals(PolygonShape? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    if (!this.Radius.Equals(other.Radius) || this.VertexCount != other.VertexCount)
      return false;
    for (int index = 0; index < this.VertexCount; ++index)
    {
      if (!this.Vertices[index].Equals(other.Vertices[index]))
        return false;
    }
    return true;
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is PolygonShape other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<int, Vector2[], float>(this.VertexCount, this.Vertices.AsSpan<Vector2>(0, this.VertexCount).ToArray(), this.Radius);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PolygonShape target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PolygonShape>(this, ref target, hookCtx, true, context))
      return;
    Vector2[] target1 = (Vector2[]) null;
    if (this.Vertices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Vector2[]>(this.Vertices, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<Vector2[]>(this.Vertices, hookCtx, context);
    target.Vertices = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PolygonShape target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PolygonShape target1 = (PolygonShape) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PolygonShape Instantiate() => new PolygonShape();
}
