using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Robust.Shared.Map;

internal sealed class NetworkedMapManager : MapManager, INetworkedMapManager, IMapManagerInternal, IMapManager
{
	public void CullDeletionHistory(GameTick upToTick)
	{
		AllEntityQueryEnumerator<MapGridComponent> allEntityQueryEnumerator = EntityManager.AllEntityQueryEnumerator<MapGridComponent>();
		MapGridComponent comp;
		while (allEntityQueryEnumerator.MoveNext(out comp))
		{
			comp.ChunkDeletionHistory.RemoveAll(((GameTick tick, Vector2i indices) t) => t.tick < upToTick);
		}
	}

	MapGridComponent IMapManager.CreateGrid(MapId currentMapId, in GridCreateOptions options)
	{
		return CreateGrid(currentMapId, in options);
	}
}
