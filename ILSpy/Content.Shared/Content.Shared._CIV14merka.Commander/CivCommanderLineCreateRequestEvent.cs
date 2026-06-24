using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLineCreateRequestEvent : EntityEventArgs
{
	public CivCommanderLineColor Color { get; }

	public MapId MapId { get; }

	public Vector2 Start { get; }

	public Vector2 End { get; }

	public CivCommanderLineCreateRequestEvent(CivCommanderLineColor color, MapId mapId, Vector2 start, Vector2 end)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
		MapId = mapId;
		Start = start;
		End = end;
	}
}
