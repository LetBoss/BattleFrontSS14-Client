using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Shared.Coordinates.Helpers;

public static class SnapgridHelper
{
	public static EntityCoordinates SnapToGrid(this EntityCoordinates coordinates, IEntityManager? entMan = null, IMapManager? mapManager = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.Resolve<IEntityManager, IMapManager>(ref entMan, ref mapManager);
		SharedTransformSystem xformSys = entMan.System<SharedTransformSystem>();
		EntityUid? gridId = xformSys.GetGrid(coordinates);
		if (!gridId.HasValue)
		{
			MapCoordinates mapPos = xformSys.ToMapCoordinates(coordinates, true);
			float mapX = (float)(int)Math.Floor(((MapCoordinates)(ref mapPos)).X) + 0.5f;
			float mapY = (float)(int)Math.Floor(((MapCoordinates)(ref mapPos)).Y) + 0.5f;
			((MapCoordinates)(ref mapPos))._002Ector(new Vector2(mapX, mapY), mapPos.MapId);
			return xformSys.ToCoordinates(Entity<TransformComponent>.op_Implicit(coordinates.EntityId), mapPos);
		}
		ushort tileSize = entMan.GetComponent<MapGridComponent>(gridId.Value).TileSize;
		Vector2 position = xformSys.WithEntityId(coordinates, gridId.Value).Position;
		float x = (float)(int)Math.Floor(position.X / (float)(int)tileSize) + (float)(int)tileSize / 2f;
		float y = (float)(int)Math.Floor(position.Y / (float)(int)tileSize) + (float)(int)tileSize / 2f;
		EntityCoordinates gridPos = default(EntityCoordinates);
		((EntityCoordinates)(ref gridPos))._002Ector(gridId.Value, new Vector2(x, y));
		return xformSys.WithEntityId(gridPos, coordinates.EntityId);
	}

	public static EntityCoordinates SnapToGrid(this EntityCoordinates coordinates, MapGridComponent grid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ushort tileSize = grid.TileSize;
		Vector2 position = coordinates.Position;
		float x = (float)(int)Math.Floor(position.X / (float)(int)tileSize) + (float)(int)tileSize / 2f;
		float y = (float)(int)Math.Floor(position.Y / (float)(int)tileSize) + (float)(int)tileSize / 2f;
		return new EntityCoordinates(coordinates.EntityId, x, y);
	}
}
