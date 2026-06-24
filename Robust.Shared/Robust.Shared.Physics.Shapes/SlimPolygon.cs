using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Shapes;

internal record struct SlimPolygon : IPhysShape, IEquatable<IPhysShape>
{
	public Vector2[] Vertices => _vertices.AsSpan.Slice(0, VertexCount).ToArray();

	public Vector2[] Normals => _normals.AsSpan.Slice(0, VertexCount).ToArray();

	public byte VertexCount => 4;

	public int ChildCount => 1;

	public float Radius { get; set; }

	public ShapeType ShapeType => ShapeType.Polygon;

	[DataField(null, false, 1, false, false, null)]
	public FixedArray4<Vector2> _vertices;

	public FixedArray4<Vector2> _normals;

	public Vector2 Centroid;

	public SlimPolygon(Box2 box)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Radius = 0.01f;
		Radius = 0f;
		_vertices._00 = box.BottomLeft;
		_vertices._01 = ((Box2)(ref box)).BottomRight;
		_vertices._02 = box.TopRight;
		_vertices._03 = ((Box2)(ref box)).TopLeft;
		_normals._00 = new Vector2(0f, -1f);
		_normals._01 = new Vector2(1f, 0f);
		_normals._02 = new Vector2(0f, 1f);
		_normals._03 = new Vector2(-1f, 0f);
		Centroid = ((Box2)(ref box)).Center;
	}

	public SlimPolygon(in Box2 box, in Matrix3x2 transform, out Box2 aabb)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Radius = 0.01f;
		Unsafe.SkipInit<SlimPolygon>(out this);
		Unsafe.SkipInit(out Vector128<float> vector);
		Unsafe.SkipInit(out Vector128<float> vector2);
		Matrix3Helpers.TransformBox(transform, ref box, ref vector, ref vector2);
		Vector128<float> source = SimdHelpers.GetAABB(vector, vector2);
		aabb = Unsafe.As<Vector128<float>, Box2>(ref source);
		if (Sse.IsSupported)
		{
			Span<Vector128<float>> span = MemoryMarshal.Cast<Vector2, Vector128<float>>(_vertices.AsSpan);
			span[0] = Sse.UnpackLow(vector, vector2);
			span[1] = Sse.UnpackHigh(vector, vector2);
		}
		else
		{
			_vertices._00 = new Vector2(vector[0], vector2[0]);
			_vertices._01 = new Vector2(vector[1], vector2[1]);
			_vertices._02 = new Vector2(vector[2], vector2[2]);
			_vertices._03 = new Vector2(vector[3], vector2[3]);
		}
		Radius = 0f;
		Centroid = (_vertices._00 + _vertices._02) / 2f;
		Polygon.CalculateNormals(_vertices.AsSpan, _normals.AsSpan, 4);
	}

	public SlimPolygon(in Box2Rotated box, in Matrix3x2 transform, out Box2 aabb)
		: this(in box.Box, ((Box2Rotated)(ref box)).Transform * transform, out aabb)
	{
	}

	public SlimPolygon(in Box2Rotated box)
	{
		Radius = 0.01f;
		Unsafe.SkipInit<SlimPolygon>(out this);
		Unsafe.SkipInit(out Vector128<float> left);
		Unsafe.SkipInit(out Vector128<float> right);
		((Box2Rotated)(ref box)).GetVertices(ref left, ref right);
		if (Sse.IsSupported)
		{
			Span<Vector128<float>> span = MemoryMarshal.Cast<Vector2, Vector128<float>>(_vertices.AsSpan);
			span[0] = Sse.UnpackLow(left, right);
			span[1] = Sse.UnpackHigh(left, right);
		}
		else
		{
			_vertices._00 = new Vector2(left[0], right[0]);
			_vertices._01 = new Vector2(left[1], right[1]);
			_vertices._02 = new Vector2(left[2], right[2]);
			_vertices._03 = new Vector2(left[3], right[3]);
		}
		Radius = 0f;
		Centroid = (_vertices._00 + _vertices._02) / 2f;
		Polygon.CalculateNormals(_vertices.AsSpan, _normals.AsSpan, 4);
	}

	public Box2 ComputeAABBSlow(Transform transform)
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

	public Box2 ComputeAABBSse(Transform transform)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Span<Vector128<float>> span = MemoryMarshal.Cast<Vector2, Vector128<float>>(_vertices.AsSpan);
		Vector128<float> x = Sse.Shuffle(span[0], span[1], 136);
		Vector128<float> y = Sse.Shuffle(span[0], span[1], 221);
		Transform.MulSimd(in transform, x, y, out var xOut, out var yOut);
		Vector128<float> aABB = SimdHelpers.GetAABB(xOut, yOut);
		Vector128<float> zero = Vector128<float>.Zero;
		Vector128<float> left = Vector128.Create(Radius);
		aABB = aABB - Sse.MoveLowToHigh(left, zero) + Sse.MoveHighToLow(left, zero);
		return Unsafe.As<Vector128<float>, Box2>(ref aABB);
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!Sse.IsSupported)
		{
			return ComputeAABBSlow(transform);
		}
		return ComputeAABBSse(transform);
	}

	public bool Equals(SlimPolygon other)
	{
		if (Radius.Equals(other.Radius))
		{
			return ((ReadOnlySpan<Vector2>)_vertices.AsSpan.Slice(0, VertexCount)).SequenceEqual((ReadOnlySpan<Vector2>)other._vertices.AsSpan.Slice(0, VertexCount));
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(_vertices, _normals, Centroid, Radius);
	}

	public bool Equals(IPhysShape? other)
	{
		if (other is Polygon polygon)
		{
			return polygon.Equals(this);
		}
		if (other is SlimPolygon other2)
		{
			return Equals(other2);
		}
		return false;
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Vertices = ");
		builder.Append(Vertices);
		builder.Append(", Normals = ");
		builder.Append(Normals);
		builder.Append(", _vertices = ");
		builder.Append(_vertices.ToString());
		builder.Append(", _normals = ");
		builder.Append(_normals.ToString());
		builder.Append(", Centroid = ");
		builder.Append(Centroid.ToString());
		builder.Append(", VertexCount = ");
		builder.Append(VertexCount.ToString());
		builder.Append(", ChildCount = ");
		builder.Append(ChildCount.ToString());
		builder.Append(", Radius = ");
		builder.Append(Radius.ToString());
		builder.Append(", ShapeType = ");
		builder.Append(ShapeType.ToString());
		return true;
	}
}
