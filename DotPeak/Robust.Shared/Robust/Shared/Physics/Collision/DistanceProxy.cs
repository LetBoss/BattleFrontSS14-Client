// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.DistanceProxy
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Utility;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Collision;

internal ref struct DistanceProxy
{
  internal float Radius;
  internal ReadOnlySpan<Vector2> Vertices;
  internal FixedArray2<Vector2> Buffer;

  internal DistanceProxy(ReadOnlySpan<Vector2> vertices, float radius)
  {
    this.Buffer = new FixedArray2<Vector2>();
    this.Vertices = vertices;
    this.Radius = radius;
  }

  internal void Set<T>(T shape, int index) where T : IPhysShape
  {
    switch (shape.ShapeType)
    {
      case ShapeType.Circle:
        PhysShapeCircle physShapeCircle = Unsafe.As<PhysShapeCircle>((object) shape);
        this.Buffer._00 = physShapeCircle.Position;
        this.Vertices = (ReadOnlySpan<Vector2>) this.Buffer.AsSpan.Slice(0, 1);
        this.Radius = physShapeCircle.Radius;
        break;
      case ShapeType.Edge:
        EdgeShape edgeShape = Unsafe.As<EdgeShape>((object) shape);
        this.Buffer._00 = edgeShape.Vertex1;
        this.Buffer._01 = edgeShape.Vertex2;
        this.Vertices = (ReadOnlySpan<Vector2>) this.Buffer.AsSpan;
        this.Radius = edgeShape.Radius;
        break;
      case ShapeType.Polygon:
        switch (shape)
        {
          case Polygon polygon:
            Span<Vector2> destination1 = (Span<Vector2>) new Vector2[(int) polygon.VertexCount];
            polygon._vertices.AsSpan.Slice(0, (int) polygon.VertexCount).CopyTo(destination1);
            this.Vertices = (ReadOnlySpan<Vector2>) destination1;
            this.Radius = polygon.Radius;
            return;
          case SlimPolygon slimPolygon:
            Span<Vector2> destination2 = (Span<Vector2>) new Vector2[(int) slimPolygon.VertexCount];
            slimPolygon._vertices.AsSpan.Slice(0, (int) slimPolygon.VertexCount).CopyTo(destination2);
            this.Vertices = (ReadOnlySpan<Vector2>) destination2;
            this.Radius = slimPolygon.Radius;
            return;
          default:
            PolygonShape polygonShape = Unsafe.As<PolygonShape>((object) shape);
            this.Vertices = (ReadOnlySpan<Vector2>) polygonShape.Vertices.AsSpan<Vector2>().Slice(0, polygonShape.VertexCount);
            this.Radius = polygonShape.Radius;
            return;
        }
      case ShapeType.Chain:
        ChainShape chainShape = Unsafe.As<ChainShape>((object) shape);
        this.Buffer._00 = chainShape.Vertices[index];
        this.Buffer._01 = index + 1 < chainShape.Vertices.Length ? chainShape.Vertices[index + 1] : chainShape.Vertices[0];
        this.Vertices = (ReadOnlySpan<Vector2>) this.Buffer.AsSpan;
        this.Radius = chainShape.Radius;
        break;
      default:
        throw new InvalidOperationException($"Invalid shapetype specified {shape.ShapeType}");
    }
  }

  public int GetSupport(Vector2 direction)
  {
    int support = 0;
    float num1 = Vector2.Dot(this.Vertices[0], direction);
    for (int index = 1; index < this.Vertices.Length; ++index)
    {
      float num2 = Vector2.Dot(this.Vertices[index], direction);
      if ((double) num2 > (double) num1)
      {
        support = index;
        num1 = num2;
      }
    }
    return support;
  }

  public Vector2 GetSupportVertex(Vector2 direction)
  {
    int index1 = 0;
    float num1 = Vector2.Dot(this.Vertices[0], direction);
    for (int index2 = 1; index2 < this.Vertices.Length; ++index2)
    {
      float num2 = Vector2.Dot(this.Vertices[index2], direction);
      if ((double) num2 > (double) num1)
      {
        index1 = index2;
        num1 = num2;
      }
    }
    return this.Vertices[index1];
  }

  internal static DistanceProxy MakeProxy(ReadOnlySpan<Vector2> vertices, int count, float radius)
  {
    count = Math.Min(count, 8);
    return new DistanceProxy(vertices.Slice(0, count), radius);
  }
}
