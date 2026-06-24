using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Collision;

internal sealed class CollisionManager : IManifoldManager
{
	private ObjectPool<EdgeShape> _edgePool = new DefaultObjectPool<EdgeShape>(new DefaultPooledObjectPolicy<EdgeShape>());

	public void CollideCircles(ref Manifold manifold, PhysShapeCircle circleA, in Transform xfA, PhysShapeCircle circleB, in Transform xfB)
	{
		manifold.PointCount = 0;
		Vector2 vector = Transform.Mul(in xfA, in circleA.Position);
		Vector2 vector2 = Transform.Mul(in xfB, in circleB.Position) - vector;
		float num = Vector2.Dot(vector2, vector2);
		float num2 = circleA.Radius + circleB.Radius;
		if (!(num > num2 * num2))
		{
			manifold.Type = ManifoldType.Circles;
			manifold.LocalPoint = circleA.Position;
			manifold.LocalNormal = Vector2.Zero;
			manifold.PointCount = 1;
			ref ManifoldPoint _ = ref manifold.Points._00;
			_.LocalPoint = Vector2.Zero;
			_.Id.Key = 0u;
		}
	}

	public static void GetPointStates(ref PointState[] state1, ref PointState[] state2, in Manifold manifold1, in Manifold manifold2)
	{
		FixedArray2<ManifoldPoint> points = manifold1.Points;
		Span<ManifoldPoint> asSpan = points.AsSpan;
		points = manifold2.Points;
		Span<ManifoldPoint> asSpan2 = points.AsSpan;
		for (int i = 0; i < manifold1.PointCount; i++)
		{
			ContactID id = asSpan[i].Id;
			state1[i] = PointState.Remove;
			for (int j = 0; j < manifold2.PointCount; j++)
			{
				if (asSpan2[j].Id.Key == id.Key)
				{
					state1[i] = PointState.Persist;
					break;
				}
			}
		}
		for (int k = 0; k < manifold2.PointCount; k++)
		{
			ContactID id2 = asSpan2[k].Id;
			state2[k] = PointState.Add;
			for (int l = 0; l < manifold1.PointCount; l++)
			{
				if (asSpan[l].Id.Key == id2.Key)
				{
					state2[k] = PointState.Persist;
					break;
				}
			}
		}
	}

	private static int ClipSegmentToLine(Span<ClipVertex> vOut, Span<ClipVertex> vIn, Vector2 normal, float offset, int vertexIndexA)
	{
		ClipVertex clipVertex = vIn[0];
		ClipVertex clipVertex2 = vIn[1];
		int num = 0;
		float num2 = normal.X * clipVertex.V.X + normal.Y * clipVertex.V.Y - offset;
		float num3 = normal.X * clipVertex2.V.X + normal.Y * clipVertex2.V.Y - offset;
		if (num2 <= 0f)
		{
			vOut[num++] = clipVertex;
		}
		if (num3 <= 0f)
		{
			vOut[num++] = clipVertex2;
		}
		if (num2 * num3 < 0f)
		{
			float num4 = num2 / (num2 - num3);
			ref ClipVertex reference = ref vOut[num];
			reference.V.X = clipVertex.V.X + num4 * (clipVertex2.V.X - clipVertex.V.X);
			reference.V.Y = clipVertex.V.Y + num4 * (clipVertex2.V.Y - clipVertex.V.Y);
			reference.ID.Features.IndexA = (byte)vertexIndexA;
			reference.ID.Features.IndexB = clipVertex.ID.Features.IndexB;
			reference.ID.Features.TypeA = 0;
			reference.ID.Features.TypeB = 1;
			num++;
		}
		return num;
	}

	public EdgeShape GetContactEdge()
	{
		return _edgePool.Get();
	}

	public void ReturnEdge(EdgeShape edge)
	{
		_edgePool.Return(edge);
	}

	public void CollideEdgeAndCircle(ref Manifold manifold, EdgeShape edgeA, in Transform transformA, PhysShapeCircle circleB, in Transform transformB)
	{
		manifold.PointCount = 0;
		Vector2 vector = Transform.MulT(in transformA, Transform.Mul(in transformB, in circleB.Position));
		Vector2 vertex = edgeA.Vertex1;
		Vector2 vertex2 = edgeA.Vertex2;
		Vector2 vector2 = vertex2 - vertex;
		Vector2 vector3 = new Vector2(vector2.Y, 0f - vector2.X);
		float num = Vector2.Dot(vector3, vector - vertex);
		if (edgeA.OneSided && num < 0f)
		{
			return;
		}
		float num2 = Vector2.Dot(vector2, vertex2 - vector);
		float num3 = Vector2.Dot(vector2, vector - vertex);
		float num4 = edgeA.Radius + circleB.Radius;
		ContactFeature features = new ContactFeature
		{
			IndexB = 0,
			TypeB = 0
		};
		Vector2 vector4;
		if (num3 <= 0f)
		{
			vector4 = vertex;
			Vector2 vector5 = vector - vector4;
			if (Vector2.Dot(vector5, vector5) > num4 * num4)
			{
				return;
			}
			if (edgeA.OneSided)
			{
				Vector2 vertex3 = edgeA.Vertex0;
				Vector2 vector6 = vertex;
				if (Vector2.Dot(vector6 - vertex3, vector6 - vector) > 0f)
				{
					return;
				}
			}
			features.IndexA = 0;
			features.TypeA = 0;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.Circles;
			manifold.LocalNormal = Vector2.Zero;
			manifold.LocalPoint = vector4;
			manifold.Points._00.Id.Key = 0u;
			manifold.Points._00.Id.Features = features;
			manifold.Points._00.LocalPoint = circleB.Position;
			return;
		}
		if (num2 <= 0f)
		{
			vector4 = vertex2;
			Vector2 vector7 = vector - vector4;
			if (Vector2.Dot(vector7, vector7) > num4 * num4)
			{
				return;
			}
			if (edgeA.OneSided)
			{
				Vector2 vertex4 = edgeA.Vertex3;
				Vector2 vector8 = vertex2;
				if (Vector2.Dot(vertex4 - vector8, vector - vector8) > 0f)
				{
					return;
				}
			}
			features.IndexA = 1;
			features.TypeA = 0;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.Circles;
			manifold.LocalNormal = Vector2.Zero;
			manifold.LocalPoint = vector4;
			manifold.Points._00.Id.Key = 0u;
			manifold.Points._00.Id.Features = features;
			manifold.Points._00.LocalPoint = circleB.Position;
			return;
		}
		float num5 = Vector2.Dot(vector2, vector2);
		vector4 = (vertex * num2 + vertex2 * num3) * (1f / num5);
		Vector2 vector9 = vector - vector4;
		if (!(Vector2.Dot(vector9, vector9) > num4 * num4))
		{
			if (num < 0f)
			{
				vector3 = new Vector2(0f - vector3.X, 0f - vector3.Y);
			}
			vector3 = Vector2Helpers.Normalized(vector3);
			features.IndexA = 0;
			features.TypeA = 1;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = vector3;
			manifold.LocalPoint = vertex;
			manifold.Points._00.Id.Key = 0u;
			manifold.Points._00.Id.Features = features;
			manifold.Points._00.LocalPoint = circleB.Position;
		}
	}

	public void CollideEdgeAndPolygon(ref Manifold manifold, EdgeShape edgeA, in Transform xfA, PolygonShape polygonB, in Transform xfB)
	{
		manifold.PointCount = 0;
		Transform transform = Transform.MulT(in xfA, in xfB);
		Vector2 vector = Transform.Mul(in transform, polygonB.Centroid);
		Vector2 vertex = edgeA.Vertex1;
		Vector2 vertex2 = edgeA.Vertex2;
		Vector2 vector2 = vertex2 - vertex;
		vector2 = Vector2Helpers.Normalized(vector2);
		Vector2 vector3 = new Vector2(vector2.Y, 0f - vector2.X);
		float num = Vector2.Dot(vector3, vector - vertex);
		bool oneSided = edgeA.OneSided;
		if (oneSided && num < 0f)
		{
			return;
		}
		int vertexCount = polygonB.VertexCount;
		Vector2[] array = new Vector2[vertexCount];
		Vector2[] array2 = new Vector2[vertexCount];
		for (int i = 0; i < vertexCount; i++)
		{
			array[i] = Transform.Mul(in transform, in polygonB.Vertices[i]);
			array2[i] = Transform.Mul(in transform.Quaternion2D, in polygonB.Normals[i]);
		}
		float num2 = polygonB.Radius + edgeA.Radius;
		EPAxis ePAxis = ComputeEdgeSeparation(array, vertex, vector3);
		if (ePAxis.Separation > num2)
		{
			return;
		}
		EPAxis ePAxis2 = ComputePolygonSeparation(array, array2, vertex, vertex2);
		if (ePAxis2.Separation > num2)
		{
			return;
		}
		EPAxis ePAxis3 = ((!(ePAxis2.Separation - num2 > 0.98f * (ePAxis.Separation - num2) + 0.001f)) ? ePAxis : ePAxis2);
		if (oneSided)
		{
			Vector2 vector4 = vertex - edgeA.Vertex0;
			vector4 = Vector2Helpers.Normalized(vector4);
			Vector2 vector5 = new Vector2(vector4.Y, 0f - vector4.X);
			bool flag = Vector2Helpers.Cross(vector4, vector2) >= 0f;
			Vector2 vector6 = edgeA.Vertex3 - vertex2;
			vector6 = Vector2Helpers.Normalized(vector6);
			Vector2 vector7 = new Vector2(vector6.Y, 0f - vector6.X);
			bool flag2 = Vector2Helpers.Cross(vector2, vector6) >= 0f;
			if (Vector2.Dot(ePAxis3.Normal, vector2) <= 0f)
			{
				if (flag)
				{
					if (Vector2Helpers.Cross(ePAxis3.Normal, vector5) > 0.1f)
					{
						return;
					}
				}
				else
				{
					ePAxis3 = ePAxis;
				}
			}
			else if (flag2)
			{
				if (Vector2Helpers.Cross(vector7, ePAxis3.Normal) > 0.1f)
				{
					return;
				}
			}
			else
			{
				ePAxis3 = ePAxis;
			}
		}
		Span<ClipVertex> vIn = stackalloc ClipVertex[2];
		Unsafe.SkipInit(out ReferenceFace referenceFace);
		if (ePAxis3.Type == EPAxisType.EdgeA)
		{
			manifold.Type = ManifoldType.FaceA;
			int num3 = 0;
			float num4 = Vector2.Dot(ePAxis3.Normal, array2[0]);
			for (int j = 1; j < array.Length; j++)
			{
				float num5 = Vector2.Dot(ePAxis3.Normal, array2[j]);
				if (num5 < num4)
				{
					num4 = num5;
					num3 = j;
				}
			}
			int num6 = num3;
			int num7 = ((num6 + 1 < array.Length) ? (num6 + 1) : 0);
			vIn[0].V = array[num6];
			vIn[0].ID.Features.IndexA = 0;
			vIn[0].ID.Features.IndexB = (byte)num6;
			vIn[0].ID.Features.TypeA = 1;
			vIn[0].ID.Features.TypeB = 0;
			vIn[1].V = array[num7];
			vIn[1].ID.Features.IndexA = 0;
			vIn[1].ID.Features.IndexB = (byte)num7;
			vIn[1].ID.Features.TypeA = 1;
			vIn[1].ID.Features.TypeB = 0;
			referenceFace.i1 = 0;
			referenceFace.i2 = 1;
			referenceFace.v1 = vertex;
			referenceFace.v2 = vertex2;
			referenceFace.normal = ePAxis3.Normal;
			referenceFace.sideNormal1 = -vector2;
			referenceFace.sideNormal2 = vector2;
		}
		else
		{
			manifold.Type = ManifoldType.FaceB;
			vIn[0].V = vertex2;
			vIn[0].ID.Features.IndexA = 1;
			vIn[0].ID.Features.IndexB = (byte)ePAxis3.Index;
			vIn[0].ID.Features.TypeA = 0;
			vIn[0].ID.Features.TypeB = 1;
			vIn[1].V = vertex;
			vIn[1].ID.Features.IndexA = 0;
			vIn[1].ID.Features.IndexB = (byte)ePAxis3.Index;
			vIn[1].ID.Features.TypeA = 0;
			vIn[1].ID.Features.TypeB = 1;
			referenceFace.i1 = ePAxis3.Index;
			referenceFace.i2 = ((referenceFace.i1 + 1 < vertexCount) ? (referenceFace.i1 + 1) : 0);
			referenceFace.v1 = array[referenceFace.i1];
			referenceFace.v2 = array[referenceFace.i2];
			referenceFace.normal = array2[referenceFace.i1];
			referenceFace.sideNormal1 = new Vector2(referenceFace.normal.Y, 0f - referenceFace.normal.X);
			referenceFace.sideNormal2 = -referenceFace.sideNormal1;
		}
		referenceFace.sideOffset1 = Vector2.Dot(referenceFace.sideNormal1, referenceFace.v1);
		referenceFace.sideOffset2 = Vector2.Dot(referenceFace.sideNormal2, referenceFace.v2);
		Span<ClipVertex> span = stackalloc ClipVertex[2];
		Span<ClipVertex> vOut = stackalloc ClipVertex[2];
		if (ClipSegmentToLine(span, vIn, referenceFace.sideNormal1, referenceFace.sideOffset1, referenceFace.i1) < 2 || ClipSegmentToLine(vOut, span, referenceFace.sideNormal2, referenceFace.sideOffset2, referenceFace.i2) < 2)
		{
			return;
		}
		if (ePAxis3.Type == EPAxisType.EdgeA)
		{
			manifold.LocalNormal = referenceFace.normal;
			manifold.LocalPoint = referenceFace.v1;
		}
		else
		{
			manifold.LocalNormal = array2[referenceFace.i1];
			manifold.LocalPoint = array[referenceFace.i1];
		}
		int num8 = 0;
		Span<ManifoldPoint> asSpan = manifold.Points.AsSpan;
		for (int k = 0; k < 2; k++)
		{
			if (Vector2.Dot(referenceFace.normal, vOut[k].V - referenceFace.v1) <= num2)
			{
				ref ManifoldPoint reference = ref asSpan[num8];
				if (ePAxis3.Type == EPAxisType.EdgeA)
				{
					reference.LocalPoint = Transform.MulT(in transform, in vOut[k].V);
					reference.Id = vOut[k].ID;
				}
				else
				{
					reference.LocalPoint = vOut[k].V;
					reference.Id.Features.TypeA = vOut[k].ID.Features.TypeB;
					reference.Id.Features.TypeB = vOut[k].ID.Features.TypeA;
					reference.Id.Features.IndexA = vOut[k].ID.Features.IndexB;
					reference.Id.Features.IndexB = vOut[k].ID.Features.IndexA;
				}
				num8++;
			}
		}
		manifold.PointCount = num8;
	}

	private static EPAxis ComputeEdgeSeparation(Span<Vector2> tempPolyVerts, Vector2 v1, Vector2 normal1)
	{
		EPAxis result = new EPAxis
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
		for (int i = 0; i < 2; i++)
		{
			float num = float.MaxValue;
			for (int j = 0; j < tempPolyVerts.Length; j++)
			{
				float num2 = Vector2.Dot(span[i], tempPolyVerts[j] - v1);
				if (num2 < num)
				{
					num = num2;
				}
			}
			if (num > result.Separation)
			{
				result.Index = i;
				result.Separation = num;
				result.Normal = span[i];
			}
		}
		return result;
	}

	private EPAxis ComputePolygonSeparation(Span<Vector2> tempPolyVerts, Span<Vector2> tempPolyNorms, Vector2 v1, Vector2 v2)
	{
		EPAxis result = new EPAxis
		{
			Type = EPAxisType.Unknown,
			Index = -1,
			Separation = float.MinValue,
			Normal = Vector2.Zero
		};
		for (int i = 0; i < tempPolyVerts.Length; i++)
		{
			Vector2 vector = -tempPolyNorms[i];
			float x = Vector2.Dot(vector, tempPolyVerts[i] - v1);
			float y = Vector2.Dot(vector, tempPolyVerts[i] - v2);
			float num = MathF.Min(x, y);
			if (num > result.Separation)
			{
				result.Type = EPAxisType.EdgeB;
				result.Index = i;
				result.Separation = num;
				result.Normal = vector;
			}
		}
		return result;
	}

	public bool TestOverlap<T, U>(T shapeA, int indexA, U shapeB, int indexB, in Transform xfA, in Transform xfB) where T : IPhysShape where U : IPhysShape
	{
		DistanceInput input = default(DistanceInput);
		input.ProxyA.Set(shapeA, indexA);
		input.ProxyB.Set(shapeB, indexB);
		input.TransformA = xfA;
		input.TransformB = xfB;
		input.UseRadii = true;
		DistanceManager.ComputeDistance(out var output, out var _, in input);
		return output.Distance < 1.4E-44f;
	}

	public void CollidePolygonAndCircle(ref Manifold manifold, PolygonShape polygonA, in Transform xfA, PhysShapeCircle circleB, in Transform xfB)
	{
		manifold.PointCount = 0;
		Vector2 vector = Transform.MulT(in xfA, Transform.Mul(in xfB, in circleB.Position));
		int num = 0;
		float num2 = float.MinValue;
		float num3 = polygonA.Radius + circleB.Radius;
		int vertexCount = polygonA.VertexCount;
		Vector2[] vertices = polygonA.Vertices;
		Vector2[] normals = polygonA.Normals;
		for (int i = 0; i < vertexCount; i++)
		{
			float num4 = Vector2.Dot(normals[i], vector - vertices[i]);
			if (num4 > num3)
			{
				return;
			}
			if (num4 > num2)
			{
				num2 = num4;
				num = i;
			}
		}
		int num5 = num;
		int num6 = ((num5 + 1 < vertexCount) ? (num5 + 1) : 0);
		Vector2 vector2 = vertices[num5];
		Vector2 vector3 = vertices[num6];
		if (num2 < float.Epsilon)
		{
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = normals[num];
			manifold.LocalPoint = (vector2 + vector3) * 0.5f;
			ref ManifoldPoint _ = ref manifold.Points._00;
			_.LocalPoint = circleB.Position;
			_.Id.Key = 0u;
			return;
		}
		float num7 = Vector2.Dot(vector - vector2, vector3 - vector2);
		float num8 = Vector2.Dot(vector - vector3, vector2 - vector3);
		if (num7 <= 0f)
		{
			if (!((vector - vector2).LengthSquared() > num3 * num3))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = Vector2Helpers.Normalized(vector - vector2);
				manifold.LocalPoint = vector2;
				ref ManifoldPoint _2 = ref manifold.Points._00;
				_2.LocalPoint = circleB.Position;
				_2.Id.Key = 0u;
			}
			return;
		}
		if (num8 <= 0f)
		{
			if (!((vector - vector3).LengthSquared() > num3 * num3))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = Vector2Helpers.Normalized(vector - vector3);
				manifold.LocalPoint = vector3;
				ref ManifoldPoint _3 = ref manifold.Points._00;
				_3.LocalPoint = circleB.Position;
				_3.Id.Key = 0u;
			}
			return;
		}
		Vector2 vector4 = (vector2 + vector3) * 0.5f;
		if (!(Vector2.Dot(vector - vector4, normals[num5]) > num3))
		{
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = normals[num5];
			manifold.LocalPoint = vector4;
			ref ManifoldPoint _4 = ref manifold.Points._00;
			_4.LocalPoint = circleB.Position;
			_4.Id.Key = 0u;
		}
	}

	private static float FindMaxSeparation(out int edgeIndex, PolygonShape poly1, in Transform xf1, PolygonShape poly2, in Transform xf2)
	{
		Vector2[] normals = poly1.Normals;
		Vector2[] vertices = poly1.Vertices;
		Vector2[] vertices2 = poly2.Vertices;
		int vertexCount = poly1.VertexCount;
		int vertexCount2 = poly2.VertexCount;
		Transform transform = Transform.MulT(in xf2, in xf1);
		int num = 0;
		float num2 = float.MinValue;
		for (int i = 0; i < vertexCount; i++)
		{
			Vector2 value = Transform.Mul(in transform.Quaternion2D, in normals[i]);
			Vector2 vector = Transform.Mul(in transform, in vertices[i]);
			float num3 = float.MaxValue;
			for (int j = 0; j < vertexCount2; j++)
			{
				float num4 = Vector2.Dot(value, vertices2[j] - vector);
				if (num4 < num3)
				{
					num3 = num4;
				}
			}
			if (num3 > num2)
			{
				num2 = num3;
				num = i;
			}
		}
		edgeIndex = num;
		return num2;
	}

	private static void FindIncidentEdge(Span<ClipVertex> c, PolygonShape poly1, in Transform xf1, int edge1, PolygonShape poly2, in Transform xf2)
	{
		Vector2[] normals = poly1.Normals;
		int vertexCount = poly2.VertexCount;
		Vector2[] vertices = poly2.Vertices;
		Vector2[] normals2 = poly2.Normals;
		Vector2 value = Transform.MulT(xf2.Quaternion2D, Transform.Mul(in xf1.Quaternion2D, in normals[edge1]));
		int num = 0;
		float num2 = float.MaxValue;
		for (int i = 0; i < vertexCount; i++)
		{
			float num3 = Vector2.Dot(value, normals2[i]);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		int num4 = num;
		int num5 = ((num4 + 1 < vertexCount) ? (num4 + 1) : 0);
		ref ClipVertex reference = ref c[0];
		reference.V = Transform.Mul(in xf2, in vertices[num4]);
		reference.ID.Features.IndexA = (byte)edge1;
		reference.ID.Features.IndexB = (byte)num4;
		reference.ID.Features.TypeA = 1;
		reference.ID.Features.TypeB = 0;
		ref ClipVertex reference2 = ref c[1];
		reference2.V = Transform.Mul(in xf2, in vertices[num5]);
		reference2.ID.Features.IndexA = (byte)edge1;
		reference2.ID.Features.IndexB = (byte)num5;
		reference2.ID.Features.TypeA = 1;
		reference2.ID.Features.TypeB = 0;
	}

	public void CollidePolygons(ref Manifold manifold, PolygonShape polyA, in Transform transformA, PolygonShape polyB, in Transform transformB)
	{
		manifold.PointCount = 0;
		float num = polyA.Radius + polyB.Radius;
		int edgeIndex = 0;
		float num2 = FindMaxSeparation(out edgeIndex, polyA, in transformA, polyB, in transformB);
		if (num2 > num)
		{
			return;
		}
		int edgeIndex2 = 0;
		float num3 = FindMaxSeparation(out edgeIndex2, polyB, in transformB, polyA, in transformA);
		if (num3 > num)
		{
			return;
		}
		PolygonShape polygonShape;
		PolygonShape poly;
		Transform xf;
		Transform xf2;
		int num4;
		bool flag;
		if (num3 > 0.98f * num2 + 0.001f)
		{
			polygonShape = polyB;
			poly = polyA;
			xf = transformB;
			xf2 = transformA;
			num4 = edgeIndex2;
			manifold.Type = ManifoldType.FaceB;
			flag = true;
		}
		else
		{
			polygonShape = polyA;
			poly = polyB;
			xf = transformA;
			xf2 = transformB;
			num4 = edgeIndex;
			manifold.Type = ManifoldType.FaceA;
			flag = false;
		}
		Span<ClipVertex> span = stackalloc ClipVertex[2];
		FindIncidentEdge(span, polygonShape, in xf, num4, poly, in xf2);
		int vertexCount = polygonShape.VertexCount;
		int num5 = num4;
		int num6 = ((num4 + 1 < vertexCount) ? (num4 + 1) : 0);
		Vector2 vector = polygonShape.Vertices[num5];
		Vector2 vector2 = polygonShape.Vertices[num6];
		Vector2 vector3 = vector2 - vector;
		vector3 = Vector2Helpers.Normalized(vector3);
		Vector2 localNormal = new Vector2(vector3.Y, 0f - vector3.X);
		Vector2 localPoint = (vector + vector2) * 0.5f;
		Vector2 vector4 = Transform.Mul(in xf.Quaternion2D, in vector3);
		float y = vector4.Y;
		float num7 = 0f - vector4.X;
		vector = Transform.Mul(in xf, in vector);
		vector2 = Transform.Mul(in xf, in vector2);
		float num8 = y * vector.X + num7 * vector.Y;
		float offset = 0f - (vector4.X * vector.X + vector4.Y * vector.Y) + num;
		float offset2 = vector4.X * vector2.X + vector4.Y * vector2.Y + num;
		Span<ClipVertex> span2 = stackalloc ClipVertex[2];
		if (ClipSegmentToLine(span2, span, -vector4, offset, num5) < 2)
		{
			return;
		}
		Span<ClipVertex> vOut = stackalloc ClipVertex[2];
		if (ClipSegmentToLine(vOut, span2, vector4, offset2, num6) < 2)
		{
			return;
		}
		manifold.LocalNormal = localNormal;
		manifold.LocalPoint = localPoint;
		int num9 = 0;
		Span<ManifoldPoint> asSpan = manifold.Points.AsSpan;
		for (int i = 0; i < 2; i++)
		{
			Vector2 v = vOut[i].V;
			if (y * v.X + num7 * v.Y - num8 <= num)
			{
				ref ManifoldPoint reference = ref asSpan[num9];
				reference.LocalPoint = Transform.MulT(in xf2, in vOut[i].V);
				reference.Id = vOut[i].ID;
				if (flag)
				{
					ContactFeature features = reference.Id.Features;
					reference.Id.Features.IndexA = features.IndexB;
					reference.Id.Features.IndexB = features.IndexA;
					reference.Id.Features.TypeA = features.TypeB;
					reference.Id.Features.TypeB = features.TypeA;
				}
				num9++;
			}
		}
		manifold.PointCount = num9;
	}

	bool IManifoldManager.TestOverlap<T, U>(T shapeA, int indexA, U shapeB, int indexB, in Transform xfA, in Transform xfB)
	{
		return TestOverlap(shapeA, indexA, shapeB, indexB, in xfA, in xfB);
	}

	void IManifoldManager.CollideCircles(ref Manifold manifold, PhysShapeCircle circleA, in Transform xfA, PhysShapeCircle circleB, in Transform xfB)
	{
		CollideCircles(ref manifold, circleA, in xfA, circleB, in xfB);
	}

	void IManifoldManager.CollideEdgeAndCircle(ref Manifold manifold, EdgeShape edgeA, in Transform transformA, PhysShapeCircle circleB, in Transform transformB)
	{
		CollideEdgeAndCircle(ref manifold, edgeA, in transformA, circleB, in transformB);
	}

	void IManifoldManager.CollideEdgeAndPolygon(ref Manifold manifold, EdgeShape edgeA, in Transform xfA, PolygonShape polygonB, in Transform xfB)
	{
		CollideEdgeAndPolygon(ref manifold, edgeA, in xfA, polygonB, in xfB);
	}

	void IManifoldManager.CollidePolygonAndCircle(ref Manifold manifold, PolygonShape polygonA, in Transform xfA, PhysShapeCircle circleB, in Transform xfB)
	{
		CollidePolygonAndCircle(ref manifold, polygonA, in xfA, circleB, in xfB);
	}

	void IManifoldManager.CollidePolygons(ref Manifold manifold, PolygonShape polyA, in Transform transformA, PolygonShape polyB, in Transform transformB)
	{
		CollidePolygons(ref manifold, polyA, in transformA, polyB, in transformB);
	}
}
