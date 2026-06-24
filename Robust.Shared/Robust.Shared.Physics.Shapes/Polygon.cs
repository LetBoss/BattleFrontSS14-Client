using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Shapes;

internal record struct Polygon : IPhysShape, IEquatable<IPhysShape>
{
	[DataField(null, false, 1, false, false, null)]
	public byte VertexCount { get; internal set; }

	public Vector2[] Vertices => _vertices.AsSpan.Slice(0, VertexCount).ToArray();

	public Vector2[] Normals => _normals.AsSpan.Slice(0, VertexCount).ToArray();

	public int ChildCount => 1;

	public float Radius { get; set; }

	public ShapeType ShapeType => ShapeType.Polygon;

	[DataField(null, false, 1, false, false, null)]
	internal FixedArray8<Vector2> _vertices;

	internal FixedArray8<Vector2> _normals;

	public Vector2 Centroid;

	public Polygon(IPhysShape shape)
		: this((PolygonShape)shape)
	{
	}

	public Polygon(PhysShapeAabb aabb)
		: this(aabb.LocalBounds)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public Polygon(PolygonShape polyShape)
	{
		Radius = 0.01f;
		Unsafe.SkipInit<Polygon>(out this);
		Radius = polyShape.Radius;
		Centroid = polyShape.Centroid;
		VertexCount = (byte)polyShape.VertexCount;
		Span<Vector2> span = polyShape.Vertices.AsSpan();
		span.Slice(0, VertexCount).CopyTo(_vertices.AsSpan);
		span = polyShape.Normals.AsSpan();
		span.Slice(0, VertexCount).CopyTo(_normals.AsSpan);
	}

	internal Polygon(SlimPolygon slim)
	{
		Radius = 0.01f;
		Unsafe.SkipInit<Polygon>(out this);
		Radius = slim.Radius;
		VertexCount = slim.VertexCount;
		_vertices._00 = slim._vertices._00;
		_vertices._01 = slim._vertices._01;
		_vertices._02 = slim._vertices._02;
		_vertices._03 = slim._vertices._03;
		_normals._00 = slim._normals._00;
		_normals._01 = slim._normals._01;
		_normals._02 = slim._normals._02;
		_normals._03 = slim._normals._03;
		Centroid = slim.Centroid;
	}

	public Polygon(Box2 box)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Radius = 0.01f;
		Unsafe.SkipInit<Polygon>(out this);
		Radius = 0f;
		VertexCount = 4;
		_vertices._00 = box.BottomLeft;
		_vertices._01 = ((Box2)(ref box)).BottomRight;
		_vertices._02 = box.TopRight;
		_vertices._03 = ((Box2)(ref box)).TopLeft;
		_normals._00 = new Vector2(0f, -1f);
		_normals._01 = new Vector2(1f, 0f);
		_normals._02 = new Vector2(0f, 1f);
		_normals._03 = new Vector2(-1f, 0f);
	}

	public Polygon(Box2Rotated bounds)
	{
		Radius = 0.01f;
		Unsafe.SkipInit<Polygon>(out this);
		Radius = 0f;
		VertexCount = 4;
		_vertices._00 = ((Box2Rotated)(ref bounds)).BottomLeft;
		_vertices._01 = ((Box2Rotated)(ref bounds)).BottomRight;
		_vertices._02 = ((Box2Rotated)(ref bounds)).TopRight;
		_vertices._03 = ((Box2Rotated)(ref bounds)).TopLeft;
		CalculateNormals(_vertices.AsSpan, _normals.AsSpan, 4);
		Centroid = ((Box2Rotated)(ref bounds)).Center;
	}

	internal Polygon(ReadOnlySpan<Vector2> vertices, ReadOnlySpan<Vector2> normals, Vector2 centroid, byte count)
	{
		Radius = 0.01f;
		Unsafe.SkipInit<Polygon>(out this);
		vertices.Slice(0, VertexCount).CopyTo(_vertices.AsSpan);
		normals.Slice(0, VertexCount).CopyTo(_normals.AsSpan);
		Centroid = centroid;
		VertexCount = count;
		Radius = 0f;
	}

	public Polygon(Vector2[] vertices)
	{
		Radius = 0.01f;
		Unsafe.SkipInit<Polygon>(out this);
		InternalPhysicsHull hull = InternalPhysicsHull.ComputeHull(vertices, vertices.Length);
		if (hull.Count < 3)
		{
			VertexCount = 0;
			return;
		}
		VertexCount = (byte)vertices.Length;
		Span<Vector2> asSpan = _vertices.AsSpan;
		vertices.AsSpan().CopyTo(asSpan);
		Set(hull);
		Centroid = ComputeCentroid(asSpan);
	}

	public static explicit operator Polygon(PolygonShape polyShape)
	{
		return new Polygon(polyShape);
	}

	private void Set(InternalPhysicsHull hull)
	{
		int count = hull.Count;
		Span<Vector2> asSpan = _vertices.AsSpan;
		Span<Vector2> asSpan2 = _normals.AsSpan;
		for (int i = 0; i < count; i++)
		{
			asSpan[i] = hull.Points[i];
		}
		CalculateNormals(asSpan, asSpan2, count);
	}

	public static void CalculateNormals(ReadOnlySpan<Vector2> vertices, Span<Vector2> normals, int count)
	{
		for (int i = 0; i < count; i++)
		{
			int index = ((i + 1 < count) ? (i + 1) : 0);
			Vector2 vector = vertices[index] - vertices[i];
			float num = 1f;
			Vector2 vector2 = Vector2Helpers.Cross(ref vector, ref num);
			normals[i] = Vector2Helpers.Normalized(vector2);
		}
	}

	public static Vector2 ComputeCentroid(ReadOnlySpan<Vector2> vs)
	{
		int length = vs.Length;
		Vector2 vector = new Vector2(0f, 0f);
		float num = 0f;
		Vector2 vector2 = vs[0];
		for (int i = 0; i < length; i++)
		{
			Vector2 vector3 = vs[0] - vector2;
			Vector2 vector4 = vs[i] - vector2;
			Vector2 vector5 = ((i + 1 < length) ? (vs[i + 1] - vector2) : (vs[0] - vector2));
			Vector2 vector6 = vector4 - vector3;
			Vector2 vector7 = vector5 - vector3;
			float num2 = Vector2Helpers.Cross(vector6, vector7);
			float num3 = 0.5f * num2;
			num += num3;
			vector += (vector3 + vector4 + vector5) * num3 * (1f / 3f);
		}
		return vector * (1f / num) + vector2;
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Span<Vector2> asSpan = _vertices.AsSpan;
		Vector2 vector = Transform.Mul(in transform, in asSpan[0]);
		Vector2 vector2 = vector;
		for (int i = 1; i < VertexCount; i++)
		{
			Vector2 value = Transform.Mul(in transform, in asSpan[i]);
			vector = Vector2.Min(vector, value);
			vector2 = Vector2.Max(vector2, value);
		}
		Vector2 vector3 = new Vector2(Radius, Radius);
		return new Box2(vector - vector3, vector2 + vector3);
	}

	public bool Equals(IPhysShape? other)
	{
		if (other is SlimPolygon other2)
		{
			return Equals(other2);
		}
		if (other is Polygon other3)
		{
			return Equals(other3);
		}
		return false;
	}

	public bool Equals(Polygon other)
	{
		if (VertexCount != other.VertexCount)
		{
			return false;
		}
		Span<Vector2> asSpan = _vertices.AsSpan;
		Span<Vector2> asSpan2 = other._vertices.AsSpan;
		for (int i = 0; i < VertexCount; i++)
		{
			Vector2 vector = asSpan[i];
			if (!vector.Equals(asSpan2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool Equals(SlimPolygon other)
	{
		if (VertexCount != other.VertexCount)
		{
			return false;
		}
		Span<Vector2> asSpan = _vertices.AsSpan;
		Span<Vector2> asSpan2 = other._vertices.AsSpan;
		for (int i = 0; i < VertexCount; i++)
		{
			Vector2 vector = asSpan[i];
			if (!vector.Equals(asSpan2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(VertexCount, _vertices.AsSpan.ToArray(), Radius);
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("VertexCount = ");
		builder.Append(VertexCount.ToString());
		builder.Append(", Vertices = ");
		builder.Append(Vertices);
		builder.Append(", Normals = ");
		builder.Append(Normals);
		builder.Append(", Centroid = ");
		builder.Append(Centroid.ToString());
		builder.Append(", ChildCount = ");
		builder.Append(ChildCount.ToString());
		builder.Append(", Radius = ");
		builder.Append(Radius.ToString());
		builder.Append(", ShapeType = ");
		builder.Append(ShapeType.ToString());
		return true;
	}
}
