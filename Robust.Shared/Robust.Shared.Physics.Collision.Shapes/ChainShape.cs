using System;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Robust.Shared.Physics.Collision.Shapes;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class ChainShape : IPhysShape, IEquatable<IPhysShape>, ISerializationGenerated<ChainShape>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Vector2[] Vertices = Array.Empty<Vector2>();

	[DataField(null, false, 1, false, false, null)]
	public Vector2 PrevVertex;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 NextVertex;

	public int Count => Vertices.Length - 1;

	public int ChildCount => Count - 1;

	[DataField(null, false, 1, false, false, null)]
	public float Radius { get; set; } = 0.01f;

	public ShapeType ShapeType => ShapeType.Chain;

	public void Clear()
	{
		Vertices = Array.Empty<Vector2>();
	}

	public void CreateLoop(Vector2 position, float radius, bool outer = true, float count = 16f)
	{
		int num = Math.Max(16, (int)(radius * count));
		float num2 = (float)Math.PI * 2f / (float)num;
		Span<Vector2> span = stackalloc Vector2[num];
		for (int i = 0; i < num; i++)
		{
			int num3 = (outer ? i : (-i));
			Vector2 vector = new Vector2(MathF.Cos(num2 * (float)num3) * radius, MathF.Sin(num2 * (float)num3) * radius);
			span[i] = vector;
		}
		CreateLoop(span);
	}

	public void CreateLoop(ReadOnlySpan<Vector2> vertices)
	{
		int length = vertices.Length;
		if (length >= 3)
		{
			Array.Resize(ref Vertices, length + 1);
			vertices.CopyTo(Vertices);
			Vertices[length] = Vertices[0];
			PrevVertex = Vertices[Count - 2];
			NextVertex = Vertices[1];
		}
	}

	public void CreateChain(ReadOnlySpan<Vector2> vertices, Vector2 prevVertex, Vector2 nextVertex)
	{
		int length = vertices.Length;
		Array.Resize(ref Vertices, length);
		vertices.CopyTo(Vertices);
		PrevVertex = prevVertex;
		NextVertex = nextVertex;
	}

	public EdgeShape GetChildEdge(ref EdgeShape edge, int index)
	{
		Vector2 v = ((index <= 0) ? PrevVertex : Vertices[index - 1]);
		Vector2 v2 = ((index >= Count - 2) ? NextVertex : Vertices[index + 2]);
		edge.SetOneSided(v, Vertices[index], Vertices[index + 1], v2);
		return edge;
	}

	public bool Equals(IPhysShape? other)
	{
		if (!(other is ChainShape otherChain))
		{
			return false;
		}
		return Equals(otherChain);
	}

	public bool Equals(ChainShape otherChain)
	{
		if (Count == otherChain.Count && NextVertex == otherChain.NextVertex && PrevVertex == otherChain.PrevVertex)
		{
			return Vertices.SequenceEqual(otherChain.Vertices);
		}
		return false;
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		int num = childIndex + 1;
		if (num == Count)
		{
			num = 0;
		}
		Vector2 value = Transform.Mul(in transform, in Vertices[childIndex]);
		Vector2 value2 = Transform.Mul(in transform, in Vertices[num]);
		Vector2 vector = Vector2.Min(value, value2);
		Vector2 vector2 = Vector2.Max(value, value2);
		Vector2 vector3 = new Vector2(Radius, Radius);
		return new Box2(vector - vector3, vector2 + vector3);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChainShape target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
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
			Vector2 target4 = default(Vector2);
			if (!serialization.TryCustomCopy(PrevVertex, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = serialization.CreateCopy(PrevVertex, hookCtx, context);
			}
			target.PrevVertex = target4;
			Vector2 target5 = default(Vector2);
			if (!serialization.TryCustomCopy(NextVertex, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = serialization.CreateCopy(NextVertex, hookCtx, context);
			}
			target.NextVertex = target5;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChainShape target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChainShape target2 = (ChainShape)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ChainShape Instantiate()
	{
		return new ChainShape();
	}
}
