using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Collision;

internal ref struct DistanceProxy
{
	internal float Radius;

	internal ReadOnlySpan<Vector2> Vertices;

	internal FixedArray2<Vector2> Buffer;

	internal DistanceProxy(ReadOnlySpan<Vector2> vertices, float radius)
	{
		Buffer = default(FixedArray2<Vector2>);
		Vertices = vertices;
		Radius = radius;
	}

	internal void Set<T>(T shape, int index) where T : IPhysShape
	{
		switch (shape.ShapeType)
		{
		case ShapeType.Circle:
		{
			PhysShapeCircle physShapeCircle = Unsafe.As<PhysShapeCircle>(shape);
			Buffer._00 = physShapeCircle.Position;
			Vertices = Buffer.AsSpan.Slice(0, 1);
			Radius = physShapeCircle.Radius;
			break;
		}
		case ShapeType.Polygon:
		{
			Span<Vector2> span2;
			if (shape is Polygon)
			{
				Polygon polygon = (Polygon)((((object)shape) is Polygon) ? ((object)shape) : null);
				Span<Vector2> span = new Vector2[polygon.VertexCount];
				span2 = polygon._vertices.AsSpan;
				span2.Slice(0, polygon.VertexCount).CopyTo(span);
				Vertices = span;
				Radius = polygon.Radius;
			}
			else if (shape is SlimPolygon)
			{
				SlimPolygon slimPolygon = (SlimPolygon)((((object)shape) is SlimPolygon) ? ((object)shape) : null);
				Span<Vector2> span3 = new Vector2[slimPolygon.VertexCount];
				span2 = slimPolygon._vertices.AsSpan;
				span2.Slice(0, slimPolygon.VertexCount).CopyTo(span3);
				Vertices = span3;
				Radius = slimPolygon.Radius;
			}
			else
			{
				PolygonShape polygonShape = Unsafe.As<PolygonShape>(shape);
				span2 = polygonShape.Vertices.AsSpan();
				Vertices = span2.Slice(0, polygonShape.VertexCount);
				Radius = polygonShape.Radius;
			}
			break;
		}
		case ShapeType.Chain:
		{
			ChainShape chainShape = Unsafe.As<ChainShape>(shape);
			Buffer._00 = chainShape.Vertices[index];
			Buffer._01 = ((index + 1 < chainShape.Vertices.Length) ? chainShape.Vertices[index + 1] : chainShape.Vertices[0]);
			Vertices = Buffer.AsSpan;
			Radius = chainShape.Radius;
			break;
		}
		case ShapeType.Edge:
		{
			EdgeShape edgeShape = Unsafe.As<EdgeShape>(shape);
			Buffer._00 = edgeShape.Vertex1;
			Buffer._01 = edgeShape.Vertex2;
			Vertices = Buffer.AsSpan;
			Radius = edgeShape.Radius;
			break;
		}
		default:
			throw new InvalidOperationException($"Invalid shapetype specified {shape.ShapeType}");
		}
	}

	public int GetSupport(Vector2 direction)
	{
		int result = 0;
		float num = Vector2.Dot(Vertices[0], direction);
		for (int i = 1; i < Vertices.Length; i++)
		{
			float num2 = Vector2.Dot(Vertices[i], direction);
			if (num2 > num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	public Vector2 GetSupportVertex(Vector2 direction)
	{
		int index = 0;
		float num = Vector2.Dot(Vertices[0], direction);
		for (int i = 1; i < Vertices.Length; i++)
		{
			float num2 = Vector2.Dot(Vertices[i], direction);
			if (num2 > num)
			{
				index = i;
				num = num2;
			}
		}
		return Vertices[index];
	}

	internal static DistanceProxy MakeProxy(ReadOnlySpan<Vector2> vertices, int count, float radius)
	{
		count = Math.Min(count, 8);
		return new DistanceProxy(vertices.Slice(0, count), radius);
	}
}
