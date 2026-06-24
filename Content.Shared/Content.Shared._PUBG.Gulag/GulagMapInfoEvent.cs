using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagMapInfoEvent : EntityEventArgs
{
	public MapId? GameMapId { get; }

	public MapId? GulagMapId { get; }

	public MapId? LobbyMapId { get; }

	public GulagMapInfoEvent(MapId? gameMapId, MapId? gulagMapId, MapId? lobbyMapId)
	{
		GameMapId = gameMapId;
		GulagMapId = gulagMapId;
		LobbyMapId = lobbyMapId;
	}
}
