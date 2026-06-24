using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._PUBG.RespawnTowers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client._PUBG.RespawnTowers;

public sealed class PubgRespawnTowerSystem : EntitySystem
{
	public MapId MapId { get; private set; } = MapId.Nullspace;

	public IReadOnlyList<Vector2> TowerPositions { get; private set; } = Array.Empty<Vector2>();

	public IReadOnlyList<Vector2> ActiveTowerPositions { get; private set; } = Array.Empty<Vector2>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgRespawnTowerStateEvent>((EntitySessionEventHandler<PubgRespawnTowerStateEvent>)OnTowerState, (Type[])null, (Type[])null);
	}

	private void OnTowerState(PubgRespawnTowerStateEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MapId = ev.MapId;
		TowerPositions = ev.TowerPositions;
		ActiveTowerPositions = ev.ActiveTowerPositions;
	}
}
