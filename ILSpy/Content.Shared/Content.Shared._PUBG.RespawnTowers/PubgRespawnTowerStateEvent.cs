using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.RespawnTowers;

[Serializable]
[NetSerializable]
public sealed class PubgRespawnTowerStateEvent : EntityEventArgs
{
	public MapId MapId;

	public Vector2[] TowerPositions;

	public Vector2[] ActiveTowerPositions;

	public PubgRespawnTowerStateEvent(MapId mapId, Vector2[] towerPositions, Vector2[] activeTowerPositions)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MapId = mapId;
		TowerPositions = towerPositions;
		ActiveTowerPositions = activeTowerPositions;
	}
}
