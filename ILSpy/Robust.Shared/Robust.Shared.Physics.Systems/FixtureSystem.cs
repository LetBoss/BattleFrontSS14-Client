using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Systems;

public sealed class FixtureSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class FixtureManagerComponentState : ComponentState
	{
		public Dictionary<string, Fixture> Fixtures;
	}

	[Dependency]
	private readonly EntityLookupSystem _lookup;

	[Dependency]
	private readonly SharedBroadphaseSystem _broadphase;

	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<FixturesComponent> _fixtureQuery;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<FixturesComponent, ComponentShutdown>(OnShutdown);
		SubscribeLocalEvent<FixturesComponent, ComponentGetState>(OnGetState);
		SubscribeLocalEvent<FixturesComponent, ComponentHandleState>(OnHandleState);
		_physicsQuery = GetEntityQuery<PhysicsComponent>();
		_fixtureQuery = GetEntityQuery<FixturesComponent>();
	}

	private void OnShutdown(EntityUid uid, FixturesComponent component, ComponentShutdown args)
	{
		if (_physicsQuery.TryGetComponent(uid, out PhysicsComponent component2))
		{
			_physics.DestroyContacts(component2);
		}
	}

	public bool TryCreateFixture(EntityUid uid, IPhysShape shape, string id, float density = 1f, bool hard = true, int collisionLayer = 0, int collisionMask = 0, float friction = 0.4f, float restitution = 0f, bool updates = true, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (!_physicsQuery.Resolve(uid, ref body) || !_fixtureQuery.Resolve(uid, ref manager))
		{
			return false;
		}
		if (manager.Fixtures.ContainsKey(id))
		{
			return false;
		}
		Fixture fixture = new Fixture(shape, collisionLayer, collisionMask, hard, density, friction, restitution);
		CreateFixture(uid, id, fixture, updates, manager, body, xform);
		return true;
	}

	internal void CreateFixture(EntityUid uid, string fixtureId, Fixture fixture, bool updates = true, FixturesComponent? manager = null, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		if (_physicsQuery.Resolve(uid, ref body) && _fixtureQuery.Resolve(uid, ref manager))
		{
			if (string.IsNullOrEmpty(fixtureId))
			{
				throw new InvalidOperationException("Tried to create a fixture without an ID!");
			}
			manager.Fixtures.Add(fixtureId, fixture);
			fixture.Owner = uid;
			if (body.CanCollide && Resolve(uid, ref xform))
			{
				_lookup.CreateProxies(uid, fixtureId, fixture, xform, body);
			}
			if (updates)
			{
				FixtureUpdate(uid, dirty: false, resetMass: true, manager, body);
				Dirty(uid, manager);
			}
		}
	}

	public Fixture? GetFixtureOrNull(EntityUid uid, string id, FixturesComponent? manager = null)
	{
		if (!_fixtureQuery.Resolve(uid, ref manager))
		{
			return null;
		}
		if (!manager.Fixtures.TryGetValue(id, out Fixture value))
		{
			return null;
		}
		return value;
	}

	public void DestroyFixture(EntityUid uid, string id, bool updates = true, PhysicsComponent? body = null, FixturesComponent? manager = null, TransformComponent? xform = null)
	{
		if (_fixtureQuery.Resolve(uid, ref manager))
		{
			Fixture fixtureOrNull = GetFixtureOrNull(uid, id, manager);
			if (fixtureOrNull != null)
			{
				DestroyFixture(uid, id, fixtureOrNull, updates, body, manager, xform);
			}
		}
	}

	public void DestroyFixture(EntityUid uid, string fixtureId, Fixture fixture, bool updates = true, PhysicsComponent? body = null, FixturesComponent? manager = null, TransformComponent? xform = null)
	{
		if (!Resolve(uid, ref body, ref manager, ref xform))
		{
			return;
		}
		if (!manager.Fixtures.Remove(fixtureId))
		{
			base.Log.Error($"Tried to remove fixture from {ToPrettyString(uid)} that was already removed.");
			return;
		}
		Contact[] array = fixture.Contacts.Values.ToArray();
		foreach (Contact contact in array)
		{
			_physics.DestroyContact(contact);
		}
		if (_lookup.TryGetCurrentBroadphase(xform, out BroadphaseComponent broadphase))
		{
			_lookup.DestroyProxies(uid, fixtureId, fixture, xform, broadphase);
		}
		if (updates)
		{
			bool resetMass = fixture.Density > 0f;
			FixtureUpdate(uid, dirty: true, resetMass, manager, body);
		}
	}

	internal void OnPhysicsInit(EntityUid uid, FixturesComponent component, PhysicsComponent? body = null)
	{
		if (!Resolve(uid, ref body, logMissing: false))
		{
			return;
		}
		foreach (var (value, fixture2) in component.Fixtures)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new InvalidOperationException($"Tried to setup fixture on init for {ToPrettyString(uid)} with no ID!");
			}
			fixture2.Owner = uid;
		}
		FixtureUpdate(uid, dirty: false, resetMass: true, component, body);
	}

	private void OnGetState(EntityUid uid, FixturesComponent component, ref ComponentGetState args)
	{
		Dictionary<string, Fixture> dictionary = new Dictionary<string, Fixture>(component.Fixtures.Count);
		foreach (KeyValuePair<string, Fixture> fixture3 in component.Fixtures)
		{
			fixture3.Deconstruct(out var key, out var value);
			string key2 = key;
			Fixture fixture = value;
			Fixture fixture2 = new Fixture();
			fixture.CopyTo(fixture2);
			dictionary[key2] = fixture2;
		}
		args.State = new FixtureManagerComponentState
		{
			Fixtures = dictionary
		};
	}

	private void OnHandleState(EntityUid uid, FixturesComponent component, ref ComponentHandleState args)
	{
		if (!(args.Current is FixtureManagerComponentState fixtureManagerComponentState))
		{
			return;
		}
		if (!TryComp(uid, out PhysicsComponent comp))
		{
			base.Log.Error($"Tried to apply fixture state for an entity without physics: {ToPrettyString(uid)}");
			return;
		}
		ValueList<(string, Fixture)> valueList = default(ValueList<(string, Fixture)>);
		ValueList<(string, Fixture)> valueList2 = default(ValueList<(string, Fixture)>);
		bool flag = false;
		Dictionary<string, Fixture> dictionary = new Dictionary<string, Fixture>(fixtureManagerComponentState.Fixtures.Count);
		string key;
		Fixture value;
		foreach (KeyValuePair<string, Fixture> fixture4 in fixtureManagerComponentState.Fixtures)
		{
			fixture4.Deconstruct(out key, out value);
			string key2 = key;
			Fixture fixture = value;
			Fixture fixture2 = new Fixture();
			fixture.CopyTo(fixture2);
			dictionary.Add(key2, fixture2);
			fixture2.Owner = uid;
		}
		TransformComponent transformComponent = null;
		bool flag2 = false;
		foreach (KeyValuePair<string, Fixture> item6 in dictionary)
		{
			item6.Deconstruct(out key, out value);
			string text = key;
			Fixture fixture3 = value;
			if (!component.Fixtures.TryGetValue(text, out Fixture value2))
			{
				valueList.Add((text, fixture3));
			}
			else if (!value2.Equivalent(fixture3))
			{
				fixture3.CopyTo(value2);
				flag = true;
				flag2 = true;
			}
		}
		foreach (KeyValuePair<string, Fixture> fixture5 in component.Fixtures)
		{
			fixture5.Deconstruct(out key, out value);
			string text2 = key;
			Fixture item = value;
			if (!dictionary.ContainsKey(text2))
			{
				valueList2.Add((text2, item));
			}
		}
		Span<(string, Fixture)> span = valueList2.Span;
		for (int i = 0; i < span.Length; i++)
		{
			(string, Fixture) tuple = span[i];
			string item2 = tuple.Item1;
			Fixture item3 = tuple.Item2;
			flag = true;
			DestroyFixture(uid, item2, item3, updates: false, comp, component);
		}
		Span<(string, Fixture)> span2 = valueList.Span;
		for (int i = 0; i < span2.Length; i++)
		{
			(string, Fixture) tuple2 = span2[i];
			string item4 = tuple2.Item1;
			Fixture item5 = tuple2.Item2;
			flag = true;
			CreateFixture(uid, item4, item5, updates: false, component, comp, transformComponent);
		}
		if (flag)
		{
			FixtureUpdate(uid, dirty: true, resetMass: true, component, comp);
		}
		if (flag2)
		{
			_broadphase.RegenerateContacts((Owner: uid, Comp1: comp, Comp2: component, Comp3: transformComponent));
		}
	}

	public void SetRestitution(EntityUid uid, string fixtureId, Fixture fixture, float value, bool update = true, FixturesComponent? manager = null)
	{
		fixture.Restitution = value;
		if (update && Resolve(uid, ref manager))
		{
			FixtureUpdate(uid, dirty: true, resetMass: true, manager);
		}
	}

	public void FixtureUpdate(EntityUid uid, bool dirty = true, bool resetMass = true, FixturesComponent? manager = null, PhysicsComponent? body = null)
	{
		if (!_physicsQuery.Resolve(uid, ref body) || !_fixtureQuery.Resolve(uid, ref manager))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		bool flag = false;
		foreach (Fixture value in manager.Fixtures.Values)
		{
			num |= value.CollisionMask;
			num2 |= value.CollisionLayer;
			flag |= value.Hard;
		}
		if (resetMass)
		{
			_physics.ResetMassData(uid, manager, body);
		}
		int collisionLayer = body.CollisionLayer;
		body.CollisionMask = num;
		body.CollisionLayer = num2;
		body.Hard = flag;
		if (manager.FixtureCount == 0)
		{
			_physics.SetCanCollide(uid, value: false, dirty: true, force: false, manager, body);
		}
		if (collisionLayer != num2)
		{
			CollisionLayerChangeEvent message = new CollisionLayerChangeEvent((Owner: uid, Comp: body));
			RaiseLocalEvent(ref message);
		}
		if (dirty)
		{
			Dirty(uid, manager);
		}
	}

	public int GetFixtureCount(EntityUid uid, FixturesComponent? manager = null)
	{
		if (!_fixtureQuery.Resolve(uid, ref manager))
		{
			return 0;
		}
		return manager.FixtureCount;
	}

	public bool TestPoint<T>(T shape, Transform xform, Vector2 worldPoint) where T : IPhysShape
	{
		if (!(shape is ChainShape) && !(shape is EdgeShape))
		{
			if (!(shape is PhysShapeAabb physShapeAabb))
			{
				if (!(shape is PhysShapeCircle physShapeCircle))
				{
					if (!(shape is PolygonShape polygonShape))
					{
						if (shape is SlimPolygon)
						{
							SlimPolygon slimPolygon = (SlimPolygon)((((object)shape) is SlimPolygon) ? ((object)shape) : null);
							Vector2 vector = Robust.Shared.Physics.Transform.MulT(xform.Quaternion2D, worldPoint - xform.Position);
							Span<Vector2> asSpan = slimPolygon._normals.AsSpan;
							Span<Vector2> asSpan2 = slimPolygon._vertices.AsSpan;
							for (int i = 0; i < slimPolygon.VertexCount; i++)
							{
								if (Vector2.Dot(asSpan[i], vector - asSpan2[i]) > 0f)
								{
									return false;
								}
							}
							return true;
						}
						if (shape is Polygon)
						{
							Polygon polygon = (Polygon)((((object)shape) is Polygon) ? ((object)shape) : null);
							Vector2 vector2 = Robust.Shared.Physics.Transform.MulT(xform.Quaternion2D, worldPoint - xform.Position);
							Span<Vector2> asSpan3 = polygon._normals.AsSpan;
							Span<Vector2> asSpan4 = polygon._vertices.AsSpan;
							for (int j = 0; j < polygon.VertexCount; j++)
							{
								if (Vector2.Dot(asSpan3[j], vector2 - asSpan4[j]) > 0f)
								{
									return false;
								}
							}
							return true;
						}
						throw new ArgumentOutOfRangeException($"No implemented TestPoint for {shape.GetType()}");
					}
					Vector2 vector3 = Robust.Shared.Physics.Transform.MulT(xform.Quaternion2D, worldPoint - xform.Position);
					for (int k = 0; k < polygonShape.VertexCount; k++)
					{
						if (Vector2.Dot(polygonShape.Normals[k], vector3 - polygonShape.Vertices[k]) > 0f)
						{
							return false;
						}
					}
					return true;
				}
				Vector2 vector4 = xform.Position + Robust.Shared.Physics.Transform.Mul(in xform.Quaternion2D, in physShapeCircle.Position);
				Vector2 vector5 = worldPoint - vector4;
				return Vector2.Dot(vector5, vector5) <= physShapeCircle.Radius * physShapeCircle.Radius;
			}
			PolygonShape shape2 = (PolygonShape)physShapeAabb;
			return TestPoint(shape2, xform, worldPoint);
		}
		return false;
	}

	public static MassData GetMassData<T>(T shape, float density) where T : IPhysShape
	{
		MassData data = default(MassData);
		GetMassData(shape, ref data, density);
		return data;
	}

	public static void GetMassData<T>(T shape, ref MassData data, float density) where T : IPhysShape
	{
		if (!(shape is ChainShape))
		{
			if (!(shape is EdgeShape edgeShape))
			{
				if (!(shape is PhysShapeCircle physShapeCircle))
				{
					if (!(shape is PhysShapeAabb aabb))
					{
						if (!(shape is PolygonShape polyShape))
						{
							if (shape is SlimPolygon)
							{
								SlimPolygon slim = (SlimPolygon)((((object)shape) is SlimPolygon) ? ((object)shape) : null);
								GetMassData(new Polygon(slim), ref data, density);
								return;
							}
							if (!(shape is Polygon))
							{
								throw new NotImplementedException($"Cannot get MassData for {shape} as it's not implemented!");
							}
							Polygon polygon = (Polygon)((((object)shape) is Polygon) ? ((object)shape) : null);
							byte vertexCount = polygon.VertexCount;
							Vector2 vector = new Vector2(0f, 0f);
							float num = 0f;
							float num2 = 0f;
							Vector2 _ = polygon._vertices._00;
							Span<Vector2> asSpan = polygon._vertices.AsSpan;
							for (int i = 0; i < vertexCount; i++)
							{
								Vector2 vector2 = asSpan[i] - _;
								Vector2 vector3 = ((i + 1 < vertexCount) ? (asSpan[i + 1] - _) : (asSpan[0] - _));
								float num3 = Vector2Helpers.Cross(vector2, vector3);
								float num4 = 0.5f * num3;
								num += num4;
								vector += (vector2 + vector3) * num4 * (1f / 3f);
								float x = vector2.X;
								float y = vector2.Y;
								float x2 = vector3.X;
								float y2 = vector3.Y;
								float num5 = x * x + x2 * x + x2 * x2;
								float num6 = y * y + y2 * y + y2 * y2;
								num2 += 1f / 12f * num3 * (num5 + num6);
							}
							data.Mass = density * num;
							vector *= 1f / num;
							data.Center = vector + _;
							data.I = density * num2;
							data.I += data.Mass * (Vector2.Dot(data.Center, data.Center) - Vector2.Dot(vector, vector));
						}
						else
						{
							GetMassData(new Polygon(polyShape), ref data, density);
						}
					}
					else
					{
						GetMassData(new Polygon(aabb), ref data, density);
					}
				}
				else
				{
					data.Mass = density * (float)Math.PI * physShapeCircle.Radius * physShapeCircle.Radius;
					data.Center = physShapeCircle.Position;
					data.I = data.Mass * (0.5f * physShapeCircle.Radius * physShapeCircle.Radius + Vector2.Dot(physShapeCircle.Position, physShapeCircle.Position));
				}
			}
			else
			{
				data.Mass = 0f;
				data.Center = (edgeShape.Vertex1 + edgeShape.Vertex2) * 0.5f;
				data.I = 0f;
			}
		}
		else
		{
			data.Mass = 0f;
			data.Center = Vector2.Zero;
			data.I = 0f;
		}
	}
}
