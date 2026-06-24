using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPartyPingBroadcastEvent : EntityEventArgs
{
	public NetEntity Source { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public PubgPartyPingKind Kind { get; }

	public string? ItemPrototypeId { get; }

	public DateTime ExpiresAtUtc { get; }

	public PubgPartyPingBroadcastEvent(NetEntity source, MapId mapId, Vector2 position, PubgPartyPingKind kind, string? itemPrototypeId, DateTime expiresAtUtc)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Source = source;
		MapId = mapId;
		Position = position;
		Kind = kind;
		ItemPrototypeId = itemPrototypeId;
		ExpiresAtUtc = expiresAtUtc;
	}
}
