using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Collision.Shapes;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class PolygonShape : IPhysShape, IEquatable<IPhysShape>, ISerializationHooks, IEquatable<PolygonShape>, IApproxEquatable<PolygonShape>, ISerializationGenerated<PolygonShape>, ISerializationGenerated
{
	[DataField("vertices", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public Vector2[] Vertices = Array.Empty<Vector2>();

	[ViewVariables]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public Vector2[] Normals = Array.Empty<Vector2>();

	[ViewVariables]
	public int VertexCount => Vertices.Length;

	[ViewVariables]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public Vector2 Centroid { get; internal set; } = Vector2.Zero;

	public int ChildCount => 1;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Radius { get; set; } = 0.01f;

	public ShapeType ShapeType => ShapeType.Polygon;

	public bool Set(List<Vector2> vertices)
	{
		Span<Vector2> span = CollectionsMarshal.AsSpan(vertices);
		return Set(span, vertices.Count);
	}

	public bool Set(ReadOnlySpan<Vector2> vertices, int count)
	{
		InternalPhysicsHull hull = InternalPhysicsHull.ComputeHull(vertices, count);
		if (hull.Count < 3)
		{
			return false;
		}
		Set(hull);
		return true;
	}

	internal void Set(InternalPhysicsHull hull)
	{
		int count = hull.Count;
		Array.Resize(ref Vertices, count);
		Array.Resize(ref Normals, count);
		for (int i = 0; i < count; i++)
		{
			Vertices[i] = hull.Points[i];
		}
		for (int j = 0; j < count; j++)
		{
			int num = ((j + 1 < count) ? (j + 1) : 0);
			Vector2 vector = Vertices[num] - Vertices[j];
			float num2 = 1f;
			Vector2 vector2 = Vector2Helpers.Cross(ref vector, ref num2);
			Normals[j] = Vector2Helpers.Normalized(vector2);
		}
		Centroid = ComputeCentroid(Vertices, VertexCount);
	}

	public bool Validate()
	{
		int vertexCount = VertexCount;
		if ((vertexCount < 3 || vertexCount > 8) ? true : false)
		{
			return false;
		}
		InternalPhysicsHull hull = default(InternalPhysicsHull);
		for (int i = 0; i < vertexCount; i++)
		{
			hull.Points[i] = Vertices[i];
		}
		hull.Count = vertexCount;
		return InternalPhysicsHull.ValidateHull(hull);
	}

	private static Vector2 ComputeCentroid(Vector2[] vs, int count)
	{
		Vector2 vector = new Vector2(0f, 0f);
		float num = 0f;
		Vector2 vector2 = vs[0];
		for (int i = 0; i < count; i++)
		{
			Vector2 vector3 = vs[0] - vector2;
			Vector2 vector4 = vs[i] - vector2;
			Vector2 vector5 = ((i + 1 < count) ? (vs[i + 1] - vector2) : (vs[0] - vector2));
			Vector2 vector6 = vector4 - vector3;
			Vector2 vector7 = vector5 - vector3;
			float num2 = Vector2Helpers.Cross(vector6, vector7);
			float num3 = 0.5f * num2;
			num += num3;
			vector += (vector3 + vector4 + vector5) * num3 * (1f / 3f);
		}
		return vector * (1f / num) + vector2;
	}

	public PolygonShape()
	{
	}

	internal PolygonShape(SlimPolygon poly)
	{
		Vertices = new Vector2[poly.VertexCount];
		Normals = new Vector2[poly.VertexCount];
		Span<Vector2> asSpan = poly._vertices.AsSpan;
		asSpan.Slice(0, VertexCount).CopyTo(Vertices);
		asSpan = poly._normals.AsSpan;
		asSpan.Slice(0, VertexCount).CopyTo(Normals);
		Centroid = poly.Centroid;
	}

	internal PolygonShape(Polygon poly)
	{
		Vertices = new Vector2[poly.VertexCount];
		Normals = new Vector2[poly.VertexCount];
		Span<Vector2> asSpan = poly._vertices.AsSpan;
		asSpan.Slice(0, VertexCount).CopyTo(Vertices);
		asSpan = poly._normals.AsSpan;
		asSpan.Slice(0, VertexCount).CopyTo(Normals);
		Centroid = poly.Centroid;
	}

	public PolygonShape(float radius)
	{
		Radius = radius;
	}

	void ISerializationHooks.AfterDeserialization()
	{
		Set(Vertices.AsSpan(), VertexCount);
	}

	public void Set(Box2Rotated bounds)
	{
		Span<Vector2> vertices = stackalloc Vector2[4];
		vertices[0] = ((Box2Rotated)(ref bounds)).BottomLeft;
		vertices[1] = ((Box2Rotated)(ref bounds)).BottomRight;
		vertices[2] = ((Box2Rotated)(ref bounds)).TopRight;
		vertices[3] = ((Box2Rotated)(ref bounds)).TopLeft;
		InternalPhysicsHull hull = new InternalPhysicsHull(vertices, 4);
		Set(hull);
	}

	public void SetAsBox(Box2 box)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Array.Resize(ref Vertices, 4);
		Array.Resize(ref Normals, 4);
		Vertices[0] = box.BottomLeft;
		Vertices[1] = ((Box2)(ref box)).BottomRight;
		Vertices[2] = box.TopRight;
		Vertices[3] = ((Box2)(ref box)).TopLeft;
		Normals[0] = new Vector2(0f, -1f);
		Normals[1] = new Vector2(1f, 0f);
		Normals[2] = new Vector2(0f, 1f);
		Normals[3] = new Vector2(-1f, 0f);
		Centroid = ((Box2)(ref box)).Center;
	}

	public void SetAsBox(float halfWidth, float halfHeight)
	{
		Array.Resize(ref Vertices, 4);
		Array.Resize(ref Normals, 4);
		Vertices[0] = new Vector2(0f - halfWidth, 0f - halfHeight);
		Vertices[1] = new Vector2(halfWidth, 0f - halfHeight);
		Vertices[2] = new Vector2(halfWidth, halfHeight);
		Vertices[3] = new Vector2(0f - halfWidth, halfHeight);
		Normals[0] = new Vector2(0f, -1f);
		Normals[1] = new Vector2(1f, 0f);
		Normals[2] = new Vector2(0f, 1f);
		Normals[3] = new Vector2(-1f, 0f);
		Centroid = Vector2.Zero;
	}

	public void SetAsBox(float halfWidth, float halfHeight, Vector2 center, float angle)
	{
		Array.Resize(ref Vertices, 4);
		Array.Resize(ref Normals, 4);
		Vertices[0] = new Vector2(0f - halfWidth, 0f - halfHeight);
		Vertices[1] = new Vector2(halfWidth, 0f - halfHeight);
		Vertices[2] = new Vector2(halfWidth, halfHeight);
		Vertices[3] = new Vector2(0f - halfWidth, halfHeight);
		Normals[0] = new Vector2(0f, -1f);
		Normals[1] = new Vector2(1f, 0f);
		Normals[2] = new Vector2(0f, 1f);
		Normals[3] = new Vector2(-1f, 0f);
		Centroid = center;
		Transform transform = new Transform(center, angle);
		for (int i = 0; i < VertexCount; i++)
		{
			Vertices[i] = Transform.Mul(in transform, in Vertices[i]);
			Normals[i] = Transform.Mul(in transform.Quaternion2D, in Normals[i]);
		}
	}

	public bool Equals(IPhysShape? other)
	{
		if (!(other is PolygonShape polygonShape))
		{
			return false;
		}
		if (VertexCount != polygonShape.VertexCount)
		{
			return false;
		}
		for (int i = 0; i < VertexCount; i++)
		{
			Vector2 vector = Vertices[i];
			if (!vector.Equals(polygonShape.Vertices[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool EqualsApprox(PolygonShape other)
	{
		return EqualsApprox(other, 0.001);
	}

	public bool EqualsApprox(PolygonShape other, double tolerance)
	{
		if (VertexCount != other.VertexCount || !MathHelper.CloseTo((double)Radius, (double)other.Radius, tolerance))
		{
			return false;
		}
		for (int i = 0; i < VertexCount; i++)
		{
			if (!Vector2Helpers.EqualsApprox(Vertices[i], other.Vertices[i], tolerance))
			{
				return false;
			}
		}
		return true;
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = Transform.Mul(in transform, in Vertices[0]);
		Vector2 vector2 = vector;
		for (int i = 1; i < VertexCount; i++)
		{
			Vector2 value = Transform.Mul(in transform, in Vertices[i]);
			vector = Vector2.Min(vector, value);
			vector2 = Vector2.Max(vector2, value);
		}
		Vector2 vector3 = new Vector2(Radius, Radius);
		return new Box2(vector - vector3, vector2 + vector3);
	}

	public static explicit operator PolygonShape(PhysShapeAabb aabb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Box2 localBounds = aabb.LocalBounds;
		PolygonShape polygonShape = new PolygonShape(aabb.Radius);
		polygonShape.Vertices = new Vector2[4]
		{
			localBounds.BottomLeft,
			((Box2)(ref localBounds)).BottomRight,
			localBounds.TopRight,
			((Box2)(ref localBounds)).TopLeft
		};
		polygonShape.Normals = new Vector2[4]
		{
			new Vector2(0f, -1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(-1f, 0f)
		};
		return polygonShape;
	}

	public bool Equals(PolygonShape? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (!Radius.Equals(other.Radius) || VertexCount != other.VertexCount)
		{
			return false;
		}
		for (int i = 0; i < VertexCount; i++)
		{
			Vector2 vector = Vertices[i];
			Vector2 other2 = other.Vertices[i];
			if (!vector.Equals(other2))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is PolygonShape other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(VertexCount, Vertices.AsSpan(0, VertexCount).ToArray(), Radius);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PolygonShape target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: true, context))
		{
			Vector2[] target2 = null;
			if (Vertices == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Vertices, ref target2, hookCtx, hasHooks: true, context))
			{
				target2 = serialization.CreateCopy(Vertices, hookCtx, context);
			}
			target.Vertices = target2;
			float target3 = 0f;
			if (!serialization.TryCustomCopy(Radius, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Radius;
			}
			target.Radius = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PolygonShape target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PolygonShape target2 = (PolygonShape)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PolygonShape Instantiate()
	{
		return new PolygonShape();
	}
}
