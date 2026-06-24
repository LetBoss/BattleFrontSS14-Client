using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Dynamics;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class Fixture : IEquatable<Fixture>, ISerializationHooks, ISerializationGenerated<Fixture>, ISerializationGenerated
{
	[NonSerialized]
	[ViewVariables]
	public int ProxyCount;

	[NonSerialized]
	public EntityUid Owner;

	[NonSerialized]
	[ViewVariables]
	public Dictionary<Fixture, Contact> Contacts = new Dictionary<Fixture, Contact>();

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("friction", false, 1, false, false, null)]
	[Access(new Type[]
	{
		typeof(SharedPhysicsSystem),
		typeof(FixtureSystem)
	}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Friction = 0.4f;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("restitution", false, 1, false, false, null)]
	[Access(new Type[]
	{
		typeof(SharedPhysicsSystem),
		typeof(FixtureSystem)
	}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Restitution;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("hard", false, 1, false, false, null)]
	[Access(new Type[]
	{
		typeof(SharedPhysicsSystem),
		typeof(FixtureSystem)
	}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool Hard = true;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("density", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Density = 1f;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("layer", false, 1, false, false, typeof(FlagSerializer<CollisionLayer>))]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public int CollisionLayer;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("mask", false, 1, false, false, typeof(FlagSerializer<CollisionMask>))]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public int CollisionMask;

	[ViewVariables]
	[field: NonSerialized]
	public FixtureProxy[] Proxies { get; set; } = Array.Empty<FixtureProxy>();

	[DataField("shape", false, 1, false, false, null)]
	public IPhysShape Shape { get; private set; } = new PhysShapeAabb();

	void ISerializationHooks.AfterDeserialization()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (Shape is PhysShapeAabb { LocalBounds: var localBounds })
		{
			PolygonShape polygonShape = new PolygonShape();
			Span<Vector2> span = stackalloc Vector2[4];
			span[0] = localBounds.BottomLeft;
			span[1] = ((Box2)(ref localBounds)).BottomRight;
			span[2] = localBounds.TopRight;
			span[3] = ((Box2)(ref localBounds)).TopLeft;
			polygonShape.Set(span, 4);
			Shape = polygonShape;
		}
	}

	internal Fixture(IPhysShape shape, int collisionLayer, int collisionMask, bool hard, float density = 1f, float friction = 0.4f, float restitution = 0f)
	{
		Shape = shape;
		CollisionLayer = collisionLayer;
		CollisionMask = collisionMask;
		Hard = hard;
		Density = density;
		Friction = friction;
		Restitution = restitution;
	}

	public Fixture()
	{
	}

	internal void CopyTo(Fixture fixture)
	{
		fixture.Shape = Shape;
		fixture.Friction = Friction;
		fixture.Restitution = Restitution;
		fixture.Hard = Hard;
		fixture.CollisionLayer = CollisionLayer;
		fixture.CollisionMask = CollisionMask;
		fixture.Density = Density;
	}

	public bool Equivalent(Fixture other)
	{
		if (Hard == other.Hard && CollisionLayer == other.CollisionLayer && CollisionMask == other.CollisionMask && Shape.Equals(other.Shape))
		{
			return MathHelper.CloseTo(Density, other.Density, 1E-07f);
		}
		return false;
	}

	public bool Equals(Fixture? other)
	{
		if (other == null)
		{
			return false;
		}
		if (Equivalent(other))
		{
			return Owner == other.Owner;
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Fixture target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: true, context))
		{
			IPhysShape target2 = null;
			if (Shape == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(Shape, ref target2, hookCtx, hasHooks: true, context))
			{
				target2 = serialization.CreateCopy(Shape, hookCtx, context);
			}
			target.Shape = target2;
			float target3 = 0f;
			if (!serialization.TryCustomCopy(Friction, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Friction;
			}
			target.Friction = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(Restitution, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = Restitution;
			}
			target.Restitution = target4;
			bool target5 = false;
			if (!serialization.TryCustomCopy(Hard, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = Hard;
			}
			target.Hard = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(Density, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = Density;
			}
			target.Density = target6;
			int num = 0;
			num = serialization.CreateCopy<int, FlagSerializer<CollisionLayer>>(CollisionLayer, hookCtx, context);
			target.CollisionLayer = num;
			int num2 = 0;
			num2 = serialization.CreateCopy<int, FlagSerializer<CollisionMask>>(CollisionMask, hookCtx, context);
			target.CollisionMask = num2;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Fixture target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Fixture target2 = (Fixture)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public Fixture Instantiate()
	{
		return new Fixture();
	}
}
