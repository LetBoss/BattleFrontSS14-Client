using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Collision.Shapes;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class PhysShapeAabb : IPhysShape, IEquatable<IPhysShape>, IEquatable<PhysShapeAabb>, ISerializationGenerated<PhysShapeAabb>, ISerializationGenerated
{
	private float _radius;

	[DataField("bounds", false, 1, false, false, null)]
	[ViewVariables(VVAccess.ReadWrite)]
	private Box2 _localBounds = Box2.UnitCentered;

	public int ChildCount => 1;

	[ViewVariables(VVAccess.ReadWrite)]
	public float Radius
	{
		get
		{
			return _radius;
		}
		set
		{
			if (!MathHelper.CloseToPercent(_radius, value, 1E-05))
			{
				_radius = value;
			}
		}
	}

	internal Vector2 Centroid => Vector2.Zero;

	public ShapeType ShapeType => ShapeType.Unknown;

	public Box2 LocalBounds => _localBounds;

	public PhysShapeAabb(float radius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_radius = radius;
	}

	public PhysShapeAabb()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_radius = 0.01f;
	}

	public Box2 ComputeAABB(Transform transform, int childIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Box2Rotated val = new Box2Rotated(((Box2)(ref _localBounds)).Translated(transform.Position), Angle.op_Implicit(transform.Quaternion2D.Angle), transform.Position);
		Box2 val2 = ((Box2Rotated)(ref val)).CalcBoundingBox();
		return ((Box2)(ref val2)).Enlarged(_radius);
	}

	internal List<Vector2> GetVertices()
	{
		return new List<Vector2>
		{
			((Box2)(ref _localBounds)).BottomRight,
			_localBounds.TopRight,
			((Box2)(ref _localBounds)).TopLeft,
			_localBounds.BottomLeft
		};
	}

	public bool Equals(IPhysShape? other)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!(other is PhysShapeAabb physShapeAabb))
		{
			return false;
		}
		return ((Box2)(ref _localBounds)).EqualsApprox(physShapeAabb._localBounds);
	}

	public bool Equals(PhysShapeAabb? other)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (_radius.Equals(other._radius))
		{
			return ((Box2)(ref _localBounds)).Equals(other._localBounds);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is PhysShapeAabb other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return HashCode.Combine<float, Box2>(_radius, _localBounds);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PhysShapeAabb target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Box2 target2 = default(Box2);
			if (!serialization.TryCustomCopy(_localBounds, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = serialization.CreateCopy<Box2>(_localBounds, hookCtx, context);
			}
			target._localBounds = target2;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PhysShapeAabb target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysShapeAabb target2 = (PhysShapeAabb)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PhysShapeAabb Instantiate()
	{
		return new PhysShapeAabb();
	}
}
