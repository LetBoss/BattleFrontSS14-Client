using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Dynamics.Contacts;

public sealed class Contact : IEquatable<Contact>
{
	public enum ContactType : byte
	{
		NotSupported,
		Polygon,
		PolygonAndCircle,
		Circle,
		EdgeAndPolygon,
		EdgeAndCircle,
		ChainAndPolygon,
		ChainAndCircle
	}

	private readonly IManifoldManager _manifoldManager;

	public readonly LinkedListNode<Contact> MapNode;

	public readonly LinkedListNode<Contact> BodyANode;

	public readonly LinkedListNode<Contact> BodyBNode;

	public EntityUid EntityA;

	public EntityUid EntityB;

	public string FixtureAId = string.Empty;

	public string FixtureBId = string.Empty;

	public Fixture? FixtureA;

	public Fixture? FixtureB;

	public PhysicsComponent? BodyA;

	public PhysicsComponent? BodyB;

	public TransformComponent? XformA;

	public TransformComponent? XformB;

	public Manifold Manifold;

	internal ContactType Type;

	internal ContactFlags Flags;

	[ViewVariables]
	public bool IsTouching { get; internal set; }

	[ViewVariables]
	public bool Enabled { get; set; }

	public int ChildIndexA { get; internal set; }

	public int ChildIndexB { get; internal set; }

	public float Friction { get; set; }

	public float Restitution { get; set; }

	public float TangentSpeed { get; set; }

	[ViewVariables]
	public bool Deleting => (Flags & ContactFlags.Deleting) == ContactFlags.Deleting;

	public bool Hard
	{
		get
		{
			if (FixtureA != null && FixtureB != null)
			{
				if (FixtureA.Hard)
				{
					return FixtureB.Hard;
				}
				return false;
			}
			return false;
		}
	}

	internal Contact(IManifoldManager manifoldManager)
	{
		_manifoldManager = manifoldManager;
		MapNode = new LinkedListNode<Contact>(this);
		BodyANode = new LinkedListNode<Contact>(this);
		BodyBNode = new LinkedListNode<Contact>(this);
	}

	public void ResetRestitution()
	{
		Restitution = MathF.Max(FixtureA?.Restitution ?? 0f, FixtureB?.Restitution ?? 0f);
	}

	public void ResetFriction()
	{
		Friction = MathF.Sqrt((FixtureA?.Friction ?? 0f) * (FixtureB?.Friction ?? 0f));
	}

	public void GetWorldManifold(Transform transformA, Transform transformB, out Vector2 normal)
	{
		IPhysShape physShape = FixtureA?.Shape;
		IPhysShape physShape2 = FixtureB?.Shape;
		Span<Vector2> points = stackalloc Vector2[8];
		SharedPhysicsSystem.InitializeManifold(ref Manifold, in transformA, in transformB, physShape.Radius, physShape2.Radius, out normal, points);
	}

	public void GetWorldManifold(Transform transformA, Transform transformB, out Vector2 normal, Span<Vector2> points)
	{
		IPhysShape physShape = FixtureA?.Shape;
		IPhysShape physShape2 = FixtureB?.Shape;
		SharedPhysicsSystem.InitializeManifold(ref Manifold, in transformA, in transformB, physShape.Radius, physShape2.Radius, out normal, points);
	}

	internal ContactStatus Update(Transform bodyATransform, Transform bodyBTransform, out bool wake)
	{
		Manifold manifold = Manifold;
		Enabled = true;
		bool isTouching = IsTouching;
		wake = false;
		bool flag;
		if (!FixtureA.Hard || !FixtureB.Hard)
		{
			IPhysShape shape = FixtureA.Shape;
			IPhysShape shape2 = FixtureB.Shape;
			flag = _manifoldManager.TestOverlap(shape, ChildIndexA, shape2, ChildIndexB, in bodyATransform, in bodyBTransform);
			Manifold.PointCount = 0;
		}
		else
		{
			Evaluate(ref Manifold, in bodyATransform, in bodyBTransform);
			flag = Manifold.PointCount > 0;
			Span<ManifoldPoint> asSpan = Manifold.Points.AsSpan;
			Span<ManifoldPoint> asSpan2 = manifold.Points.AsSpan;
			for (int i = 0; i < Manifold.PointCount; i++)
			{
				ManifoldPoint manifoldPoint = asSpan[i];
				manifoldPoint.NormalImpulse = 0f;
				manifoldPoint.TangentImpulse = 0f;
				ContactID id = manifoldPoint.Id;
				for (int j = 0; j < manifold.PointCount; j++)
				{
					ManifoldPoint manifoldPoint2 = asSpan2[j];
					if (manifoldPoint2.Id.Key == id.Key)
					{
						manifoldPoint.NormalImpulse = manifoldPoint2.NormalImpulse;
						manifoldPoint.TangentImpulse = manifoldPoint2.TangentImpulse;
						break;
					}
				}
				asSpan[i] = manifoldPoint;
			}
			if (flag != isTouching)
			{
				wake = true;
			}
		}
		IsTouching = flag;
		return GetContactStatus(this, isTouching);
	}

	internal static ContactStatus GetContactStatus(Contact contact, bool wasTouching)
	{
		if (!wasTouching)
		{
			if (contact.IsTouching)
			{
				return ContactStatus.StartTouching;
			}
			return ContactStatus.NoContact;
		}
		if (!contact.IsTouching)
		{
			return ContactStatus.EndTouching;
		}
		return ContactStatus.Touching;
	}

	internal void UpdateIsTouching(Transform bodyATransform, Transform bodyBTransform)
	{
		if (!FixtureA.Hard || !FixtureB.Hard)
		{
			IPhysShape shape = FixtureA.Shape;
			IPhysShape shape2 = FixtureB.Shape;
			IsTouching = _manifoldManager.TestOverlap(shape, ChildIndexA, shape2, ChildIndexB, in bodyATransform, in bodyBTransform);
		}
		else
		{
			Manifold manifold = Manifold;
			Evaluate(ref manifold, in bodyATransform, in bodyBTransform);
			IsTouching = manifold.PointCount > 0;
		}
	}

	private void Evaluate(ref Manifold manifold, in Transform transformA, in Transform transformB)
	{
		switch (Type)
		{
		case ContactType.Polygon:
			_manifoldManager.CollidePolygons(ref manifold, (PolygonShape)FixtureA.Shape, in transformA, (PolygonShape)FixtureB.Shape, in transformB);
			break;
		case ContactType.PolygonAndCircle:
			_manifoldManager.CollidePolygonAndCircle(ref manifold, (PolygonShape)FixtureA.Shape, in transformA, (PhysShapeCircle)FixtureB.Shape, in transformB);
			break;
		case ContactType.EdgeAndCircle:
			_manifoldManager.CollideEdgeAndCircle(ref manifold, (EdgeShape)FixtureA.Shape, in transformA, (PhysShapeCircle)FixtureB.Shape, in transformB);
			break;
		case ContactType.EdgeAndPolygon:
			_manifoldManager.CollideEdgeAndPolygon(ref manifold, (EdgeShape)FixtureA.Shape, in transformA, (PolygonShape)FixtureB.Shape, in transformB);
			break;
		case ContactType.ChainAndCircle:
		{
			ChainShape obj2 = (ChainShape)FixtureA.Shape;
			EdgeShape edge2 = _manifoldManager.GetContactEdge();
			obj2.GetChildEdge(ref edge2, ChildIndexA);
			_manifoldManager.CollideEdgeAndCircle(ref manifold, edge2, in transformA, (PhysShapeCircle)FixtureB.Shape, in transformB);
			_manifoldManager.ReturnEdge(edge2);
			break;
		}
		case ContactType.ChainAndPolygon:
		{
			ChainShape obj = (ChainShape)FixtureA.Shape;
			EdgeShape edge = _manifoldManager.GetContactEdge();
			obj.GetChildEdge(ref edge, ChildIndexA);
			_manifoldManager.CollideEdgeAndPolygon(ref manifold, edge, in transformA, (PolygonShape)FixtureB.Shape, in transformB);
			_manifoldManager.ReturnEdge(edge);
			break;
		}
		case ContactType.Circle:
			_manifoldManager.CollideCircles(ref manifold, (PhysShapeCircle)FixtureA.Shape, in transformA, (PhysShapeCircle)FixtureB.Shape, in transformB);
			break;
		default:
			throw new ArgumentOutOfRangeException($"Collision between {FixtureA.Shape.GetType()} and {FixtureB.Shape.GetType()} not supported");
		}
	}

	public bool Equals(Contact? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (object.Equals(FixtureA, other.FixtureA) && object.Equals(FixtureB, other.FixtureB) && Manifold.Equals(other.Manifold) && Type == other.Type && Enabled == other.Enabled && ChildIndexA == other.ChildIndexA && ChildIndexB == other.ChildIndexB && Friction.Equals(other.Friction))
		{
			return Restitution.Equals(other.Restitution);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is Contact other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(EntityA, EntityB);
	}

	public EntityUid OurEnt(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return EntityA;
		}
		if (uid == EntityB)
		{
			return EntityB;
		}
		throw new InvalidOperationException();
	}

	public EntityUid OtherEnt(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return EntityB;
		}
		if (uid == EntityB)
		{
			return EntityA;
		}
		throw new InvalidOperationException();
	}

	public (string Id, Fixture) OurFixture(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return (Id: FixtureAId, FixtureA);
		}
		if (uid == EntityB)
		{
			return (Id: FixtureBId, FixtureB);
		}
		throw new InvalidOperationException();
	}

	public (string Id, Fixture) OtherFixture(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return (Id: FixtureBId, FixtureB);
		}
		if (uid == EntityB)
		{
			return (Id: FixtureAId, FixtureA);
		}
		throw new InvalidOperationException();
	}

	public PhysicsComponent OurBody(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return BodyA;
		}
		if (uid == EntityB)
		{
			return BodyB;
		}
		throw new InvalidOperationException();
	}

	public PhysicsComponent OtherBody(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return BodyB;
		}
		if (uid == EntityB)
		{
			return BodyA;
		}
		throw new InvalidOperationException();
	}

	public TransformComponent OtherTransform(EntityUid uid)
	{
		if (uid == EntityA)
		{
			return XformB;
		}
		if (uid == EntityB)
		{
			return XformA;
		}
		throw new InvalidOperationException();
	}
}
