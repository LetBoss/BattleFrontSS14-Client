using System.Diagnostics.CodeAnalysis;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared.Spawning;

public static class EntitySystemExtensions
{
	public static EntityUid? SpawnIfUnobstructed(this IEntityManager entityManager, string? prototypeName, EntityCoordinates coordinates, CollisionGroup collisionLayer, in Box2? box = null, SharedPhysicsSystem? physicsManager = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (physicsManager == null)
		{
			physicsManager = entityManager.System<SharedPhysicsSystem>();
		}
		MapCoordinates mapCoordinates = entityManager.System<SharedTransformSystem>().ToMapCoordinates(coordinates, true);
		return entityManager.SpawnIfUnobstructed(prototypeName, mapCoordinates, collisionLayer, in box, physicsManager);
	}

	public static EntityUid? SpawnIfUnobstructed(this IEntityManager entityManager, string? prototypeName, MapCoordinates coordinates, CollisionGroup collisionLayer, in Box2? box = null, SharedPhysicsSystem? collision = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Box2 valueOrDefault = box.GetValueOrDefault(Box2.UnitCentered);
		Box2 boxOrDefault = ((Box2)(ref valueOrDefault)).Translated(coordinates.Position);
		if (collision == null)
		{
			collision = entityManager.System<SharedPhysicsSystem>();
		}
		foreach (PhysicsComponent body in collision.GetCollidingEntities(coordinates.MapId, ref boxOrDefault))
		{
			if (body.Hard && collisionLayer != CollisionGroup.None && ((uint)body.CollisionMask & (uint)collisionLayer) != 0)
			{
				return null;
			}
		}
		return entityManager.SpawnEntity(prototypeName, coordinates, (ComponentRegistry)null);
	}

	public static bool TrySpawnIfUnobstructed(this IEntityManager entityManager, string? prototypeName, EntityCoordinates coordinates, CollisionGroup collisionLayer, [NotNullWhen(true)] out EntityUid? entity, Box2? box = null, SharedPhysicsSystem? physicsManager = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		entity = entityManager.SpawnIfUnobstructed(prototypeName, coordinates, collisionLayer, in box, physicsManager);
		return entity.HasValue;
	}

	public static bool TrySpawnIfUnobstructed(this IEntityManager entityManager, string? prototypeName, MapCoordinates coordinates, CollisionGroup collisionLayer, [NotNullWhen(true)] out EntityUid? entity, in Box2? box = null, SharedPhysicsSystem? physicsManager = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		entity = entityManager.SpawnIfUnobstructed(prototypeName, coordinates, collisionLayer, in box, physicsManager);
		return entity.HasValue;
	}
}
