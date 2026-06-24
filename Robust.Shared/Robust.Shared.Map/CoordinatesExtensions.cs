using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Robust.Shared.Map;

public static class CoordinatesExtensions
{
	public static EntityCoordinates AlignWithClosestGridTile(this EntityCoordinates coords, float searchBoxSize = 1.5f, IEntityManager? entityManager = null, IMapManager? mapManager = null)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve(ref entityManager, ref mapManager);
		SharedTransformSystem sharedTransformSystem = entityManager.System<SharedTransformSystem>();
		EntityUid? grid = sharedTransformSystem.GetGrid(coords);
		SharedMapSystem sharedMapSystem = entityManager.System<SharedMapSystem>();
		if (entityManager.TryGetComponent<MapGridComponent>(grid, out MapGridComponent component))
		{
			return sharedMapSystem.GridTileToLocal(grid.Value, component, sharedMapSystem.CoordinatesToTile(grid.Value, component, coords));
		}
		MapCoordinates mapCoordinates = sharedTransformSystem.ToMapCoordinates(coords);
		if (mapManager.TryFindGridAt(mapCoordinates, out EntityUid uid, out component))
		{
			return sharedMapSystem.GridTileToLocal(uid, component, sharedMapSystem.CoordinatesToTile(uid, component, coords));
		}
		Box2 val = ((Box2)(ref Box2.UnitCentered)).Scale(searchBoxSize);
		Box2 worldAABB = ((Box2)(ref val)).Translated(mapCoordinates.Position);
		List<Entity<MapGridComponent>> grids = new List<Entity<MapGridComponent>>();
		mapManager.FindGridsIntersecting(mapCoordinates.MapId, worldAABB, ref grids);
		uid = EntityUid.Invalid;
		MapGridComponent mapGridComponent = null;
		float num = float.PositiveInfinity;
		Box2 val2 = default(Box2);
		EntityQuery<TransformComponent> entityQuery = entityManager.GetEntityQuery<TransformComponent>();
		foreach (Entity<MapGridComponent> item in grids)
		{
			TransformComponent component2 = entityQuery.GetComponent(item.Owner);
			Matrix3x2 worldMatrix = sharedTransformSystem.GetWorldMatrix(component2);
			val = item.Comp.LocalAABB;
			Box2 val3 = Matrix3Helpers.TransformBox(worldMatrix, ref val);
			Box2 val4 = ((Box2)(ref worldAABB)).Intersect(ref val3);
			float num2 = (((Box2)(ref val4)).Center - mapCoordinates.Position).LengthSquared();
			if (!(num2 >= num))
			{
				uid = item.Owner;
				num = num2;
				mapGridComponent = item;
				val2 = val4;
			}
		}
		if (mapGridComponent != null)
		{
			Vector2 vector = mapCoordinates.Position - ((Box2)(ref val2)).Center;
			Vector2i gridTile = sharedMapSystem.WorldToTile(uid, mapGridComponent, ((Box2)(ref val2)).Center);
			Vector2 posWorld = sharedMapSystem.GridTileToWorldPos(uid, mapGridComponent, gridTile) + vector * (int)mapGridComponent.TileSize;
			coords = new EntityCoordinates(uid, sharedMapSystem.WorldToLocal(uid, mapGridComponent, posWorld));
		}
		return coords;
	}
}
