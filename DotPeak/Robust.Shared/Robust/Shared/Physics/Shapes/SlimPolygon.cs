// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Shapes.SlimPolygon
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
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

#nullable enable
namespace Robust.Shared.Physics.Shapes;

internal record struct SlimPolygon : IPhysShape, IEquatable<IPhysShape>
{
  [DataField(null, false, 1, false, false, null)]
  public FixedArray4<Vector2> _vertices;
  public FixedArray4<Vector2> _normals;
  public Vector2 Centroid;

  public Vector2[] Vertices => this._vertices.AsSpan.Slice(0, (int) this.VertexCount).ToArray();

  public Vector2[] Normals => this._normals.AsSpan.Slice(0, (int) this.VertexCount).ToArray();

  public byte VertexCount => 4;

  public int ChildCount => 1;

  public float Radius { get; set; }

  public ShapeType ShapeType => ShapeType.Polygon;

  public SlimPolygon(Box2 box)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    this.Radius = 0.0f;
    this._vertices._00 = box.BottomLeft;
    this._vertices._01 = ((Box2) ref box).BottomRight;
    this._vertices._02 = box.TopRight;
    this._vertices._03 = ((Box2) ref box).TopLeft;
    this._normals._00 = new Vector2(0.0f, -1f);
    this._normals._01 = new Vector2(1f, 0.0f);
    this._normals._02 = new Vector2(0.0f, 1f);
    this._normals._03 = new Vector2(-1f, 0.0f);
    this.Centroid = ((Box2) ref box).Center;
  }

  public SlimPolygon(in Box2 box, in Matrix3x2 transform, out Box2 aabb)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<SlimPolygon>(out this);
    Vector128<float> left;
    Vector128<float> right;
    Matrix3Helpers.TransformBox(transform, ref box, ref left, ref right);
    Vector128<float> aabb1 = SimdHelpers.GetAABB(left, right);
    aabb = Unsafe.As<Vector128<float>, Box2>(ref aabb1);
    if (Sse.IsSupported)
    {
      Span<Vector128<float>> span = MemoryMarshal.Cast<Vector2, Vector128<float>>(this._vertices.AsSpan);
      span[0] = Sse.UnpackLow(left, right);
      span[1] = Sse.UnpackHigh(left, right);
    }
    else
    {
      this._vertices._00 = new Vector2(left[0], right[0]);
      this._vertices._01 = new Vector2(left[1], right[1]);
      this._vertices._02 = new Vector2(left[2], right[2]);
      this._vertices._03 = new Vector2(left[3], right[3]);
    }
    this.Radius = 0.0f;
    this.Centroid = (this._vertices._00 + this._vertices._02) / 2f;
    Polygon.CalculateNormals((ReadOnlySpan<Vector2>) this._vertices.AsSpan, this._normals.AsSpan, 4);
  }

  public SlimPolygon(in Box2Rotated box, in Matrix3x2 transform, out Box2 aabb)
    : this(in box.Box, ((Box2Rotated) ref box).Transform * transform, out aabb)
  {
  }

  public SlimPolygon(in Box2Rotated box)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CRadius\u003Ek__BackingField = 0.01f;
    Unsafe.SkipInit<SlimPolygon>(out this);
    Vector128<float> left;
    Vector128<float> right;
    ((Box2Rotated) ref box).GetVertices(ref left, ref right);
    if (Sse.IsSupported)
    {
      Span<Vector128<float>> span = MemoryMarshal.Cast<Vector2, Vector128<float>>(this._vertices.AsSpan);
      span[0] = Sse.UnpackLow(left, right);
      span[1] = Sse.UnpackHigh(left, right);
    }
    else
    {
      this._vertices._00 = new Vector2(left[0], right[0]);
      this._vertices._01 = new Vector2(left[1], right[1]);
      this._vertices._02 = new Vector2(left[2], right[2]);
      this._vertices._03 = new Vector2(left[3], right[3]);
    }
    this.Radius = 0.0f;
    this.Centroid = (this._vertices._00 + this._vertices._02) / 2f;
    Polygon.CalculateNormals((ReadOnlySpan<Vector2>) this._vertices.AsSpan, this._normals.AsSpan, 4);
  }

  public Box2 ComputeAABBSlow(Transform transform)
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

  public Box2 ComputeAABBSse(Transform transform)
  {
    Span<Vector128<float>> span = MemoryMarshal.Cast<Vector2, Vector128<float>>(this._vertices.AsSpan);
    Vector128<float> x = Sse.Shuffle(span[0], span[1], (byte) 136);
    Vector128<float> y = Sse.Shuffle(span[0], span[1], (byte) 221);
    Vector128<float> xOut;
    Vector128<float> yOut;
    Transform.MulSimd(in transform, x, y, out xOut, out yOut);
    Vector128<float> aabb = SimdHelpers.GetAABB(xOut, yOut);
    Vector128<float> zero = Vector128<float>.Zero;
    Vector128<float> left = Vector128.Create(this.Radius);
    Vector128<float> source = aabb - Sse.MoveLowToHigh(left, zero) + Sse.MoveHighToLow(left, zero);
    return Unsafe.As<Vector128<float>, Box2>(ref source);
  }

  public Box2 ComputeAABB(Transform transform, int childIndex)
  {
    return !Sse.IsSupported ? this.ComputeAABBSlow(transform) : this.ComputeAABBSse(transform);
  }

  public bool Equals(SlimPolygon other)
  {
    return this.Radius.Equals(other.Radius) && ((ReadOnlySpan<Vector2>) this._vertices.AsSpan.Slice(0, (int) this.VertexCount)).SequenceEqual<Vector2>((ReadOnlySpan<Vector2>) other._vertices.AsSpan.Slice(0, (int) this.VertexCount));
  }

  public override readonly int GetHashCode()
  {
    return HashCode.Combine<FixedArray4<Vector2>, FixedArray4<Vector2>, Vector2, float>(this._vertices, this._normals, this.Centroid, this.Radius);
  }

  public bool Equals(IPhysShape? other)
  {
    switch (other)
    {
      case Polygon polygon:
        return polygon.Equals(this);
      case SlimPolygon other1:
        return this.Equals(other1);
      default:
        return false;
    }
  }
}
