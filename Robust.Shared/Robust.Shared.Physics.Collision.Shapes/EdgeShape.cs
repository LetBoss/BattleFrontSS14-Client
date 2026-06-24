using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Physics.Collision.Shapes;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class EdgeShape : IPhysShape, IEquatable<IPhysShape>, IEquatable<EdgeShape>, ISerializationGenerated<EdgeShape>, ISerializationGenerated
{
	[DataField("vertex1", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	internal Vector2 Vertex1;

	[DataField("vertex2", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	internal Vector2 Vertex2;

	[DataField("vertex0", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	internal Vector2 Vertex0;

	[DataField("vertex3", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	internal Vector2 Vertex3;

	[DataField("oneSided", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool OneSided;

	public int ChildCount => 1;

	public ShapeType ShapeType => ShapeType.Edge;

	[DataField("radius", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Radius { get; set; } = 0.01f;

	public EdgeShape()
	{
	}

	public EdgeShape(Vector2 v1, Vector2 v2)
	{
		SetTwoSided(v1, v2);
	}

	public void SetOneSided(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
	{
		Vertex0 = v0;
		Vertex1 = v1;
		Vertex2 = v2;
		Vertex3 = v3;
		OneSided = true;
	}

	public void SetTwoSided(Vector2 start, Vector2 end)
	{
		Vertex1 = start;
		Vertex2 = end;
		OneSided = false;
	}

	public bool Equals(IPhysShape? other)
	{
		if (!(other is EdgeShape edgeShape))
		{
			return false;
		}
		if (OneSided == edgeShape.OneSided && Vertex0.Equals(edgeShape.Vertex0) && Vertex1.Equals(edgeShape.Vertex1) && Vertex2.Equals(edgeShape.Vertex2))
		{
			return Vertex3.Equals(edgeShape.Vertex3);
		}
		return false;
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 value = Transform.Mul(in transform, in Vertex1);
		Vector2 value2 = Transform.Mul(in transform, in Vertex2);
		Vector2 vector = Vector2.Min(value, value2);
		Vector2 vector2 = Vector2.Max(value, value2);
		Vector2 vector3 = new Vector2(Radius, Radius);
		return new Box2(vector - vector3, vector2 + vector3);
	}

	public float CalculateArea()
	{
		return 0f;
	}

	public bool Equals(EdgeShape? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (OneSided == other.OneSided && Vertex1.Equals(other.Vertex1) && Vertex2.Equals(other.Vertex2) && Vertex0.Equals(other.Vertex0) && Vertex3.Equals(other.Vertex3))
		{
			return Radius.Equals(other.Radius);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is EdgeShape other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(OneSided, Vertex1, Vertex2, Vertex0, Vertex3, Radius);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EdgeShape target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Vector2 target2 = default(Vector2);
			if (!serialization.TryCustomCopy(Vertex1, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = serialization.CreateCopy(Vertex1, hookCtx, context);
			}
			target.Vertex1 = target2;
			Vector2 target3 = default(Vector2);
			if (!serialization.TryCustomCopy(Vertex2, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy(Vertex2, hookCtx, context);
			}
			target.Vertex2 = target3;
			Vector2 target4 = default(Vector2);
			if (!serialization.TryCustomCopy(Vertex0, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = serialization.CreateCopy(Vertex0, hookCtx, context);
			}
			target.Vertex0 = target4;
			Vector2 target5 = default(Vector2);
			if (!serialization.TryCustomCopy(Vertex3, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = serialization.CreateCopy(Vertex3, hookCtx, context);
			}
			target.Vertex3 = target5;
			bool target6 = false;
			if (!serialization.TryCustomCopy(OneSided, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = OneSided;
			}
			target.OneSided = target6;
			float target7 = 0f;
			if (!serialization.TryCustomCopy(Radius, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = Radius;
			}
			target.Radius = target7;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EdgeShape target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EdgeShape target2 = (EdgeShape)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public EdgeShape Instantiate()
	{
		return new EdgeShape();
	}
}
