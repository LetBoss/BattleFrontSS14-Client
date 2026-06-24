// Decompiled with JetBrains decompiler
// Type: Content.Shared.Spawning.EntitySystemExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Spawning;

public static class EntitySystemExtensions
{
  public static EntityUid? SpawnIfUnobstructed(
    this IEntityManager entityManager,
    string? prototypeName,
    EntityCoordinates coordinates,
    CollisionGroup collisionLayer,
    in Box2? box = null,
    SharedPhysicsSystem? physicsManager = null)
  {
    if (physicsManager == null)
      physicsManager = entityManager.System<SharedPhysicsSystem>();
    MapCoordinates mapCoordinates = entityManager.System<SharedTransformSystem>().ToMapCoordinates(coordinates);
    return entityManager.SpawnIfUnobstructed(prototypeName, mapCoordinates, collisionLayer, in box, physicsManager);
  }

  public static EntityUid? SpawnIfUnobstructed(
    this IEntityManager entityManager,
    string? prototypeName,
    MapCoordinates coordinates,
    CollisionGroup collisionLayer,
    in Box2? box = null,
    SharedPhysicsSystem? collision = null)
  {
    Box2 box2 = box ?? Box2.UnitCentered;
    Box2 worldAABB = ((Box2) ref box2).Translated(coordinates.Position);
    if (collision == null)
      collision = entityManager.System<SharedPhysicsSystem>();
    foreach (PhysicsComponent collidingEntity in collision.GetCollidingEntities(coordinates.MapId, in worldAABB))
    {
      if (collidingEntity.Hard && collisionLayer != CollisionGroup.None && ((CollisionGroup) collidingEntity.CollisionMask & collisionLayer) != CollisionGroup.None)
        return new EntityUid?();
    }
    return new EntityUid?(entityManager.SpawnEntity(prototypeName, coordinates));
  }

  public static bool TrySpawnIfUnobstructed(
    this IEntityManager entityManager,
    string? prototypeName,
    EntityCoordinates coordinates,
    CollisionGroup collisionLayer,
    [NotNullWhen(true)] out EntityUid? entity,
    Box2? box = null,
    SharedPhysicsSystem? physicsManager = null)
  {
    entity = entityManager.SpawnIfUnobstructed(prototypeName, coordinates, collisionLayer, in box, physicsManager);
    return entity.HasValue;
  }

  public static bool TrySpawnIfUnobstructed(
    this IEntityManager entityManager,
    string? prototypeName,
    MapCoordinates coordinates,
    CollisionGroup collisionLayer,
    [NotNullWhen(true)] out EntityUid? entity,
    in Box2? box = null,
    SharedPhysicsSystem? physicsManager = null)
  {
    entity = entityManager.SpawnIfUnobstructed(prototypeName, coordinates, collisionLayer, in box, physicsManager);
    return entity.HasValue;
  }
}
