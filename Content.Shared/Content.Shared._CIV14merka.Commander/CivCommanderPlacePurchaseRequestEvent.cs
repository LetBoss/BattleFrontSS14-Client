using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderPlacePurchaseRequestEvent : EntityEventArgs
{
	public string EntryId { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public CivCommanderPlacePurchaseRequestEvent(string entryId, MapId mapId, Vector2 position)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		EntryId = entryId;
		MapId = mapId;
		Position = position;
	}
}
