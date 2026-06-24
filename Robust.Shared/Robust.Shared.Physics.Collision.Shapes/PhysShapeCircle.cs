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
public sealed class PhysShapeCircle : IPhysShape, IEquatable<IPhysShape>, IEquatable<PhysShapeCircle>, ISerializationGenerated<PhysShapeCircle>, ISerializationGenerated
{
	private const float DefaultRadius = 0.5f;

	[DataField("position", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public Vector2 Position;

	public int ChildCount => 1;

	public ShapeType ShapeType => ShapeType.Circle;

	[DataField("radius", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Radius { get; set; } = 0.5f;

	public PhysShapeCircle()
	{
	}

	public PhysShapeCircle(float radius)
	{
		Radius = radius;
		Position = Vector2.Zero;
	}

	public PhysShapeCircle(float radius, Vector2 position)
	{
		Radius = radius;
		Position = position;
	}

	public float CalculateArea()
	{
		return (float)Math.PI * Radius * Radius;
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = transform.Position + Transform.Mul(in transform.Quaternion2D, in Position);
		return new Box2(vector.X - Radius, vector.Y - Radius, vector.X + Radius, vector.Y + Radius);
	}

	public Box2 CalcLocalBounds()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		return new Box2(Position.X - Radius, Position.Y - Radius, Position.X + Radius, Position.Y + Radius);
	}

	public bool Equals(IPhysShape? other)
	{
		if (!(other is PhysShapeCircle physShapeCircle))
		{
			return false;
		}
		return physShapeCircle.Equals(this);
	}

	public bool Equals(PhysShapeCircle? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (MathHelper.CloseTo(Radius, other.Radius, 1E-07f))
		{
			return Vector2Helpers.EqualsApprox(Position, other.Position);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is PhysShapeCircle other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Radius, Position);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PhysShapeCircle target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			float target2 = 0f;
			if (!serialization.TryCustomCopy(Radius, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = Radius;
			}
			target.Radius = target2;
			Vector2 target3 = default(Vector2);
			if (!serialization.TryCustomCopy(Position, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy(Position, hookCtx, context);
			}
			target.Position = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PhysShapeCircle target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysShapeCircle target2 = (PhysShapeCircle)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PhysShapeCircle Instantiate()
	{
		return new PhysShapeCircle();
	}
}
