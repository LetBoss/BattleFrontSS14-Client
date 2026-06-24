using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLabelCreateRequestEvent : EntityEventArgs
{
	public CivCommanderLineColor Color { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public float Rotation { get; }

	public string Text { get; }

	public CivCommanderLabelCreateRequestEvent(CivCommanderLineColor color, MapId mapId, Vector2 position, float rotation, string text)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
		MapId = mapId;
		Position = position;
		Rotation = rotation;
		Text = text;
	}
}
