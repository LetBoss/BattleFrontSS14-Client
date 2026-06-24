using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Systems;

public sealed class RayCastSystem : EntitySystem
{
	private sealed class RayComparer : IComparer<RayHit>
	{
		public int Compare(RayHit x, RayHit y)
		{
			return x.Fraction.CompareTo(y.Fraction);
		}
	}

	[Robust.Shared.IoC.Dependency]
	private readonly SharedBroadphaseSystem _broadphase;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedPhysicsSystem _physics;

	private readonly RayComparer _rayComparer = new RayComparer();

	private void AdjustResults(ref RayResult result, int index, Transform xf)
	{
		for (int i = index; i < result.Results.Count; i++)
		{
			result.Results[i].Point = Robust.Shared.Physics.Transform.Mul(in xf, in result.Results[i].Point);
		}
	}

	public void CastRay(Entity<BroadphaseComponent?> entity, ref RayResult result, Vector2 origin, Vector2 translation, QueryFilter filter, bool sorted = true)
	{
		if (Resolve(entity.Owner, ref entity.Comp))
		{
			RayCastInput input = new RayCastInput
			{
				Origin = origin,
				Translation = translation,
				MaxFraction = 1f
			};
			WorldRayCastContext state = new WorldRayCastContext
			{
				fcn = RayCastAllCallback,
				Filter = filter,
				Fraction = 1f,
				Physics = _physics,
				System = this,
				Result = result
			};
			entity.Comp.DynamicTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastCallback);
			input.MaxFraction = state.Fraction;
			entity.Comp.StaticTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastCallback);
			result = state.Result;
			if (sorted)
			{
				result.Results.Sort(_rayComparer);
			}
		}
	}

	public RayResult CastRay(MapId mapId, Vector2 origin, Vector2 translation, QueryFilter filter)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		RayCastInput item = new RayCastInput
		{
			Origin = origin,
			Translation = translation,
			MaxFraction = 1f
		};
		RayResult item2 = new RayResult();
		Vector2 value = origin + translation;
		Unsafe.SkipInit(out Box2 aabb);
		((Box2)(ref aabb))._002Ector(Vector2.Min(origin, value), Vector2.Max(origin, value));
		(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem) state = (item, filter, item2, this, _physics);
		_broadphase.GetBroadphases<(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem)>(mapId, aabb, ref state, delegate(Entity<BroadphaseComponent> entity, ref (RayCastInput input, QueryFilter filter, RayResult result, RayCastSystem system, SharedPhysicsSystem Physics) tuple)
		{
			Transform physicsTransform = tuple.Physics.GetPhysicsTransform(entity.Owner);
			Vector2 vector = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.input.Origin);
			Vector2 translation2 = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.input.Origin + tuple.input.Translation) - vector;
			int count = tuple.result.Results.Count;
			tuple.system.CastRay((Owner: entity.Owner, Comp: entity.Comp), ref tuple.result, vector, translation2, tuple.filter, sorted: false);
			tuple.system.AdjustResults(ref tuple.result, count, physicsTransform);
		});
		item2 = state.Item3;
		item2.Results.Sort(_rayComparer);
		return item2;
	}

	public void CastRayClosest(Entity<BroadphaseComponent?> entity, ref RayResult result, Vector2 origin, Vector2 translation, QueryFilter filter)
	{
		if (Resolve(entity.Owner, ref entity.Comp))
		{
			RayCastInput input = new RayCastInput
			{
				Origin = origin,
				Translation = translation,
				MaxFraction = 1f
			};
			WorldRayCastContext state = new WorldRayCastContext
			{
				fcn = RayCastClosestCallback,
				Filter = filter,
				Fraction = 1f,
				Physics = _physics,
				System = this,
				Result = result
			};
			entity.Comp.DynamicTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastCallback);
			input.MaxFraction = state.Fraction;
			entity.Comp.StaticTree.Tree.RayCastNew(input, filter.MaskBits, ref state, RayCastCallback);
			result = state.Result;
		}
	}

	public RayResult CastRayClosest(MapId mapId, Vector2 origin, Vector2 translation, QueryFilter filter)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		RayCastInput item = new RayCastInput
		{
			Origin = origin,
			Translation = translation,
			MaxFraction = 1f
		};
		RayResult item2 = new RayResult();
		Vector2 value = origin + translation;
		Unsafe.SkipInit(out Box2 aabb);
		((Box2)(ref aabb))._002Ector(Vector2.Min(origin, value), Vector2.Max(origin, value));
		(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem) state = (item, filter, item2, this, _physics);
		_broadphase.GetBroadphases<(RayCastInput, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem)>(mapId, aabb, ref state, delegate(Entity<BroadphaseComponent> entity, ref (RayCastInput input, QueryFilter filter, RayResult result, RayCastSystem system, SharedPhysicsSystem _physics) tuple)
		{
			Transform physicsTransform = tuple._physics.GetPhysicsTransform(entity.Owner);
			Vector2 vector = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.input.Origin);
			Vector2 translation2 = Robust.Shared.Physics.Transform.InvTransformPoint(physicsTransform, tuple.input.Origin + tuple.input.Translation) - vector;
			int count = tuple.result.Results.Count;
			tuple.system.CastRayClosest((Owner: entity.Owner, Comp: entity.Comp), ref tuple.result, vector, translation2, tuple.filter);
			tuple.system.AdjustResults(ref tuple.result, count, physicsTransform);
		});
		return state.Item3;
	}

	public RayResult CastShape(MapId mapId, IPhysShape shape, Transform originTransform, Vector2 translation, QueryFilter filter, CastResult callback)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Box2 val = shape.ComputeAABB(originTransform, 0);
		Box2 val2 = shape.ComputeAABB(new Transform(originTransform.Position + translation, originTransform.Quaternion2D.Angle), 0);
		Box2 aabb = ((Box2)(ref val)).Union(ref val2);
		(Transform, Vector2, IPhysShape, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem, CastResult) state = new ValueTuple<Transform, Vector2, IPhysShape, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem, ValueTuple<CastResult>>(item5: new RayResult(), item1: originTransform, item2: translation, item3: shape, item4: filter, item6: this, item7: _physics, rest: new ValueTuple<CastResult>(callback));
		_broadphase.GetBroadphases<(Transform, Vector2, IPhysShape, QueryFilter, RayResult, RayCastSystem, SharedPhysicsSystem, CastResult)>(mapId, aabb, ref state, delegate(Entity<BroadphaseComponent> entity, ref (Transform origin, Vector2 translation, IPhysShape shape, QueryFilter filter, RayResult result, RayCastSystem system, SharedPhysicsSystem _physics, CastResult callback) tuple)
		{
			Transform A = tuple._physics.GetPhysicsTransform(entity.Owner);
			Transform originTransform2 = Robust.Shared.Physics.Transform.MulT(in A, in tuple.origin);
			Vector2 translation2 = Robust.Shared.Physics.Transform.InvTransformPoint(A, tuple.origin.Position + tuple.translation) - originTransform2.Position;
			int count = tuple.result.Results.Count;
			tuple.system.CastShape((Owner: entity.Owner, Comp: entity.Comp), ref tuple.result, tuple.shape, originTransform2, translation2, tuple.filter, tuple.callback);
			tuple.system.AdjustResults(ref tuple.result, count, A);
		});
		return state.Item5;
	}

	public void CastShape(Entity<BroadphaseComponent?> entity, ref RayResult result, IPhysShape shape, Transform originTransform, Vector2 translation, QueryFilter filter, CastResult callback)
	{
		if (!Resolve(entity.Owner, ref entity.Comp))
		{
			return;
		}
		if (!(shape is PhysShapeCircle circle))
		{
			if (!(shape is SlimPolygon poly))
			{
				if (!(shape is Polygon poly2))
				{
					if (shape is PolygonShape polygon)
					{
						CastPolygon(entity, ref result, polygon, originTransform, translation, filter, callback);
					}
					else
					{
						base.Log.Error("Tried to shapecast for shape not implemented.");
					}
				}
				else
				{
					CastPolygon(entity, ref result, new PolygonShape(poly2), originTransform, translation, filter, callback);
				}
			}
			else
			{
				CastPolygon(entity, ref result, new PolygonShape(poly), originTransform, translation, filter, callback);
			}
		}
		else
		{
			CastCircle(entity, ref result, circle, originTransform, translation, filter, callback);
		}
	}

	public void CastCircle(Entity<BroadphaseComponent?> entity, ref RayResult result, PhysShapeCircle circle, Transform originTransform, Vector2 translation, QueryFilter filter, CastResult callback)
	{
		if (Resolve(entity.Owner, ref entity.Comp))
		{
			ShapeCastInput input = new ShapeCastInput
			{
				Points = new Vector2[1],
				Count = 1,
				Radius = circle.Radius,
				Translation = translation,
				MaxFraction = 1f
			};
			input.Points[0] = Robust.Shared.Physics.Transform.Mul(in originTransform, in circle.Position);
			WorldRayCastContext state = new WorldRayCastContext
			{
				System = this,
				Physics = _physics,
				Filter = filter,
				Fraction = 1f,
				Result = result,
				fcn = callback
			};
			entity.Comp.StaticTree.Tree.ShapeCast(input, filter.MaskBits, ShapeCastCallback, ref state);
			input.MaxFraction = state.Fraction;
			entity.Comp.DynamicTree.Tree.ShapeCast(input, filter.MaskBits, ShapeCastCallback, ref state);
			result = state.Result;
		}
	}

	public void CastPolygon(Entity<BroadphaseComponent?> entity, ref RayResult result, PolygonShape polygon, Transform originTransform, Vector2 translation, QueryFilter filter, CastResult callback)
	{
		if (Resolve(entity.Owner, ref entity.Comp))
		{
			ShapeCastInput input = new ShapeCastInput
			{
				Points = new Vector2[polygon.VertexCount]
			};
			for (int i = 0; i < polygon.VertexCount; i++)
			{
				input.Points[i] = Robust.Shared.Physics.Transform.Mul(in originTransform, in polygon.Vertices[i]);
			}
			input.Count = polygon.VertexCount;
			input.Radius = polygon.Radius;
			input.Translation = translation;
			input.MaxFraction = 1f;
			WorldRayCastContext state = new WorldRayCastContext
			{
				System = this,
				Physics = _physics,
				Filter = filter,
				Fraction = 1f,
				Result = result,
				fcn = callback
			};
			if ((filter.Flags & QueryFlags.Static) == QueryFlags.Static)
			{
				entity.Comp.StaticTree.Tree.ShapeCast(input, filter.MaskBits, ShapeCastCallback, ref state);
				input.MaxFraction = state.Fraction;
			}
			if ((filter.Flags & QueryFlags.Dynamic) == QueryFlags.Dynamic)
			{
				entity.Comp.DynamicTree.Tree.ShapeCast(input, filter.MaskBits, ShapeCastCallback, ref state);
			}
			result = state.Result;
		}
	}

	public static float RayCastAllCallback(FixtureProxy proxy, Vector2 point, Vector2 normal, float fraction, ref RayResult result)
	{
		result.Results.Add(new RayHit(proxy.Entity, normal, fraction)
		{
			Point = point
		});
		return 1f;
	}

	public static float RayCastClosestCallback(FixtureProxy proxy, Vector2 point, Vector2 normal, float fraction, ref RayResult result)
	{
		bool flag = false;
		if (result.Results.Count > 0)
		{
			if (result.Results[0].Fraction > fraction)
			{
				flag = true;
				result.Results.Clear();
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			result.Results.Add(new RayHit(proxy.Entity, normal, fraction)
			{
				Point = point
			});
		}
		return fraction;
	}

	private CastOutput RayCastShape(RayCastInput input, IPhysShape shape, Transform transform)
	{
		RayCastInput input2 = input with
		{
			Origin = Robust.Shared.Physics.Transform.InvTransformPoint(transform, input.Origin),
			Translation = Quaternion2D.InvRotateVector(transform.Quaternion2D, input.Translation)
		};
		CastOutput result = default(CastOutput);
		if (!(shape is PhysShapeCircle shape2))
		{
			if (!(shape is PolygonShape polygonShape))
			{
				if (!(shape is Polygon shape3))
				{
					return result;
				}
				result = RayCastPolygon(input2, shape3);
			}
			else
			{
				result = RayCastPolygon(input2, (Polygon)polygonShape);
			}
		}
		else
		{
			result = RayCastCircle(input2, shape2);
		}
		result.Point = Robust.Shared.Physics.Transform.Mul(in transform, in result.Point);
		result.Normal = Quaternion2D.RotateVector(transform.Quaternion2D, result.Normal);
		return result;
	}

	private static float RayCastCallback(RayCastInput input, FixtureProxy proxy, ref WorldRayCastContext worldContext)
	{
		if ((proxy.Fixture.CollisionLayer & worldContext.Filter.MaskBits) == 0L && (proxy.Fixture.CollisionMask & worldContext.Filter.LayerBits) == 0L)
		{
			return input.MaxFraction;
		}
		Func<EntityUid, bool>? isIgnored = worldContext.Filter.IsIgnored;
		if (isIgnored != null && isIgnored(proxy.Entity))
		{
			return input.MaxFraction;
		}
		Transform localPhysicsTransform = worldContext.Physics.GetLocalPhysicsTransform(proxy.Entity);
		CastOutput castOutput = worldContext.System.RayCastShape(input, proxy.Fixture.Shape, localPhysicsTransform);
		if (castOutput.Hit)
		{
			return worldContext.fcn(proxy, castOutput.Point, castOutput.Normal, castOutput.Fraction, ref worldContext.Result);
		}
		return input.MaxFraction;
	}

	internal CastOutput RayCastCircle(RayCastInput input, PhysShapeCircle shape)
	{
		Vector2 position = shape.Position;
		CastOutput result = default(CastOutput);
		Vector2 vector = Vector2.Subtract(input.Origin, position);
		float num = 0f;
		Vector2 lengthAndNormalize = Vector2Helpers.GetLengthAndNormalize(input.Translation, ref num);
		if (num == 0f)
		{
			return result;
		}
		float num2 = 0f - Vector2.Dot(vector, lengthAndNormalize);
		Vector2 vector2 = Vector2.Add(vector, num2 * lengthAndNormalize);
		float num3 = Vector2.Dot(vector2, vector2);
		float radius = shape.Radius;
		float num4 = radius * radius;
		if (num3 > num4)
		{
			return result;
		}
		float num5 = MathF.Sqrt(num4 - num3);
		float num6 = num2 - num5;
		if (num6 < 0f || input.MaxFraction * num < num6)
		{
			return result;
		}
		Vector2 vector3 = Vector2.Add(vector, num6 * lengthAndNormalize);
		result.Fraction = num6 / num;
		result.Normal = Vector2Helpers.Normalized(vector3);
		result.Point = Vector2.Add(position, shape.Radius * result.Normal);
		result.Hit = true;
		return result;
	}

	private CastOutput RayCastPolygon(RayCastInput input, Polygon shape)
	{
		Span<Vector2> asSpan = shape._vertices.AsSpan;
		CastOutput output = new CastOutput
		{
			Fraction = 0f
		};
		if (shape.Radius == 0f)
		{
			Vector2 origin = input.Origin;
			Vector2 translation = input.Translation;
			float num = 0f;
			float num2 = input.MaxFraction;
			int num3 = -1;
			Span<Vector2> asSpan2 = shape._normals.AsSpan;
			for (int i = 0; i < shape.VertexCount; i++)
			{
				float num4 = Vector2.Dot(asSpan2[i], Vector2.Subtract(asSpan[i], origin));
				float num5 = Vector2.Dot(asSpan2[i], translation);
				if (num5 == 0f)
				{
					if (num4 < 0f)
					{
						return output;
					}
				}
				else if (num5 < 0f && num4 < num * num5)
				{
					num = num4 / num5;
					num3 = i;
				}
				else if (num5 > 0f && num4 < num2 * num5)
				{
					num2 = num4 / num5;
				}
				if (num2 < num)
				{
					return output;
				}
			}
			if (num3 >= 0)
			{
				output.Fraction = num;
				output.Normal = asSpan2[num3];
				output.Point = Vector2.Add(origin, num * translation);
				output.Hit = true;
			}
			return output;
		}
		Span<Vector2> span = new Vector2[1] { input.Origin };
		ShapeCast(ref output, new ShapeCastPairInput
		{
			ProxyA = DistanceProxy.MakeProxy(asSpan, shape.VertexCount, shape.Radius),
			ProxyB = DistanceProxy.MakeProxy(span, 1, 0f),
			TransformA = Robust.Shared.Physics.Transform.Empty,
			TransformB = Robust.Shared.Physics.Transform.Empty,
			TranslationB = input.Translation,
			MaxFraction = input.MaxFraction
		});
		return output;
	}

	private CastOutput RayCastSegment(RayCastInput input, EdgeShape shape, bool oneSided)
	{
		CastOutput result = default(CastOutput);
		if (oneSided && Vector2Helpers.Cross(Vector2.Subtract(input.Origin, shape.Vertex0), Vector2.Subtract(shape.Vertex1, shape.Vertex0)) < 0f)
		{
			return result;
		}
		Vector2 origin = input.Origin;
		Vector2 translation = input.Translation;
		Vector2 vertex = shape.Vertex0;
		Vector2 vector = Vector2.Subtract(shape.Vertex1, vertex);
		float num = 0f;
		Vector2 lengthAndNormalize = Vector2Helpers.GetLengthAndNormalize(vector, ref num);
		if (num == 0f)
		{
			return result;
		}
		Vector2 vector2 = Vector2Helpers.RightPerp(lengthAndNormalize);
		float num2 = Vector2.Dot(vector2, Vector2.Subtract(vertex, origin));
		float num3 = Vector2.Dot(vector2, translation);
		if (num3 == 0f)
		{
			return result;
		}
		float num4 = num2 / num3;
		if (num4 < 0f || input.MaxFraction < num4)
		{
			return result;
		}
		float num5 = Vector2.Dot(Vector2.Subtract(Vector2.Add(origin, num4 * translation), vertex), lengthAndNormalize);
		if (num5 < 0f || num < num5)
		{
			return result;
		}
		if (num2 > 0f)
		{
			vector2 = -vector2;
		}
		result.Fraction = num4;
		result.Point = Vector2.Add(origin, num4 * translation);
		result.Normal = vector2;
		result.Hit = true;
		return result;
	}

	private CastOutput ShapeCastShape(ShapeCastInput input, IPhysShape shape, Transform transform)
	{
		ShapeCastInput input2 = input;
		for (int i = 0; i < input2.Count; i++)
		{
			input2.Points[i] = Robust.Shared.Physics.Transform.MulT(in transform, in input.Points[i]);
		}
		input2.Translation = Quaternion2D.InvRotateVector(transform.Quaternion2D, input.Translation);
		CastOutput result;
		if (!(shape is PhysShapeCircle shape2))
		{
			if (!(shape is PolygonShape polygonShape))
			{
				if (!(shape is Polygon shape3))
				{
					return default(CastOutput);
				}
				result = ShapeCastPolygon(input2, shape3);
			}
			else
			{
				result = ShapeCastPolygon(input2, (Polygon)polygonShape);
			}
		}
		else
		{
			result = ShapeCastCircle(input2, shape2);
		}
		result.Point = Robust.Shared.Physics.Transform.Mul(in transform, in result.Point);
		result.Normal = Quaternion2D.RotateVector(transform.Quaternion2D, result.Normal);
		return result;
	}

	private float ShapeCastCallback(ShapeCastInput input, FixtureProxy proxy, ref WorldRayCastContext worldContext)
	{
		QueryFilter filter = worldContext.Filter;
		if ((proxy.Fixture.CollisionLayer & filter.MaskBits) == 0L && (proxy.Fixture.CollisionMask & filter.LayerBits) == 0L)
		{
			return input.MaxFraction;
		}
		if ((filter.Flags & QueryFlags.Sensors) == 0 && !proxy.Fixture.Hard)
		{
			return input.MaxFraction;
		}
		Func<EntityUid, bool>? isIgnored = worldContext.Filter.IsIgnored;
		if (isIgnored != null && isIgnored(proxy.Entity))
		{
			return input.MaxFraction;
		}
		Transform localPhysicsTransform = worldContext.Physics.GetLocalPhysicsTransform(proxy.Entity);
		CastOutput castOutput = ShapeCastShape(input, proxy.Fixture.Shape, localPhysicsTransform);
		if (castOutput.Hit)
		{
			return worldContext.fcn(proxy, castOutput.Point, castOutput.Normal, castOutput.Fraction, ref worldContext.Result);
		}
		return input.MaxFraction;
	}

	private void ShapeCast(ref CastOutput output, in ShapeCastPairInput input)
	{
		output.Fraction = input.MaxFraction;
		DistanceProxy proxyA = input.ProxyA;
		int length = input.ProxyB.Vertices.Length;
		Transform A = input.TransformA;
		Transform B = input.TransformB;
		Transform transform = Robust.Shared.Physics.Transform.InvMulTransforms(in A, in B);
		Vector2[] array = new Vector2[input.ProxyB.Vertices.Length];
		for (int i = 0; i < length; i++)
		{
			array[i] = Robust.Shared.Physics.Transform.Mul(in transform, in input.ProxyB.Vertices[i]);
		}
		DistanceProxy proxy = DistanceProxy.MakeProxy(array, length, input.ProxyB.Radius);
		float num = proxyA.Radius + proxy.Radius;
		Vector2 vector = Quaternion2D.RotateVector(transform.Quaternion2D, input.TranslationB);
		float num2 = 0f;
		float maxFraction = input.MaxFraction;
		Simplex s = new Simplex
		{
			Count = 0,
			V = default(FixedArray4<SimplexVertex>)
		};
		int index = FindSupport(proxyA, -vector);
		Vector2 left = proxyA.Vertices[index];
		int index2 = FindSupport(proxy, vector);
		Vector2 right = proxy.Vertices[index2];
		Vector2 vector2 = Vector2.Subtract(left, right);
		float num3 = MathF.Max(0.005f, num - 0.005f);
		int j;
		for (j = 0; j < 20; j++)
		{
			if (!(vector2.Length() > num3 + 0.0025f))
			{
				break;
			}
			output.Iterations++;
			index = FindSupport(proxyA, -vector2);
			left = proxyA.Vertices[index];
			index2 = FindSupport(proxy, vector2);
			right = proxy.Vertices[index2];
			Vector2 value = Vector2.Subtract(left, right);
			vector2 = Vector2Helpers.Normalized(vector2);
			float num4 = Vector2.Dot(vector2, value);
			float num5 = Vector2.Dot(vector2, vector);
			if (num4 - num3 > num2 * num5)
			{
				if (num5 <= 0f)
				{
					return;
				}
				num2 = (num4 - num3) / num5;
				if (num2 > maxFraction)
				{
					return;
				}
				s.Count = 0;
			}
			ref SimplexVertex reference = ref s.V.AsSpan[s.Count];
			reference.IndexA = index2;
			reference.WA = new Vector2(right.X + num2 * vector.X, right.Y + num2 * vector.Y);
			reference.IndexB = index;
			reference.WB = left;
			reference.W = Vector2.Subtract(reference.WB, reference.WA);
			reference.A = 1f;
			s.Count++;
			switch (s.Count)
			{
			case 2:
				Simplex.SolveSimplex2(ref s);
				break;
			case 3:
				Simplex.SolveSimplex3(ref s);
				break;
			default:
				throw new NotImplementedException();
			case 1:
				break;
			}
			if (s.Count == 3)
			{
				return;
			}
			vector2 = Simplex.ComputeSimplexClosestPoint(s);
		}
		if (j != 0 && num2 != 0f)
		{
			Vector2 b = Vector2.Zero;
			Vector2 a = Vector2.Zero;
			Simplex.ComputeSimplexWitnessPoints(ref a, ref b, s);
			Vector2 v = Vector2Helpers.Normalized(-vector2);
			output.Point = Robust.Shared.Physics.Transform.Mul(in A, new Vector2(b.X + proxyA.Radius * v.X, b.Y + proxyA.Radius * v.Y));
			output.Normal = Quaternion2D.RotateVector(A.Quaternion2D, v);
			output.Fraction = num2;
			output.Iterations = j;
			output.Hit = true;
		}
	}

	private int FindSupport(DistanceProxy proxy, Vector2 direction)
	{
		int result = 0;
		float num = Vector2.Dot(proxy.Vertices[0], direction);
		for (int i = 1; i < proxy.Vertices.Length; i++)
		{
			float num2 = Vector2.Dot(proxy.Vertices[i], direction);
			if (num2 > num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	private CastOutput ShapeCastCircle(ShapeCastInput input, PhysShapeCircle shape)
	{
		Span<Vector2> span = new Vector2[1] { shape.Position };
		ShapeCastPairInput input2 = new ShapeCastPairInput
		{
			ProxyA = DistanceProxy.MakeProxy(span, 1, shape.Radius),
			ProxyB = DistanceProxy.MakeProxy(input.Points, input.Count, input.Radius),
			TransformA = Robust.Shared.Physics.Transform.Empty,
			TransformB = Robust.Shared.Physics.Transform.Empty,
			TranslationB = input.Translation,
			MaxFraction = input.MaxFraction
		};
		CastOutput output = default(CastOutput);
		ShapeCast(ref output, in input2);
		return output;
	}

	private CastOutput ShapeCastPolygon(ShapeCastInput input, Polygon shape)
	{
		ShapeCastPairInput input2 = new ShapeCastPairInput
		{
			ProxyA = DistanceProxy.MakeProxy(shape._vertices.AsSpan, shape.VertexCount, shape.Radius),
			ProxyB = DistanceProxy.MakeProxy(input.Points, input.Count, input.Radius),
			TransformA = Robust.Shared.Physics.Transform.Empty,
			TransformB = Robust.Shared.Physics.Transform.Empty,
			TranslationB = input.Translation,
			MaxFraction = input.MaxFraction
		};
		CastOutput output = default(CastOutput);
		ShapeCast(ref output, in input2);
		return output;
	}

	private CastOutput ShapeCastSegment(ShapeCastInput input, EdgeShape shape)
	{
		Span<Vector2> span = new Vector2[1] { shape.Vertex0 };
		ShapeCastPairInput input2 = new ShapeCastPairInput
		{
			ProxyA = DistanceProxy.MakeProxy(span, 2, 0f),
			ProxyB = DistanceProxy.MakeProxy(input.Points, input.Count, input.Radius),
			TransformA = Robust.Shared.Physics.Transform.Empty,
			TransformB = Robust.Shared.Physics.Transform.Empty,
			TranslationB = input.Translation,
			MaxFraction = input.MaxFraction
		};
		CastOutput output = default(CastOutput);
		ShapeCast(ref output, in input2);
		return output;
	}
}
