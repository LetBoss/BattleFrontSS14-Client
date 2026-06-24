using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Airdrop;

[Serializable]
[NetSerializable]
public sealed class PubgAirdropStateEvent : EntityEventArgs
{
	public bool Active;

	public Vector2 Position;

	public int RemainingSeconds;

	public MapId MapId;

	public PubgAirdropStateEvent(bool active, Vector2 position, int remainingSeconds, MapId mapId)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Active = active;
		Position = position;
		RemainingSeconds = remainingSeconds;
		MapId = mapId;
	}
}
