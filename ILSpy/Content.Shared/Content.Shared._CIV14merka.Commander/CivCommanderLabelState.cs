using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLabelState
{
	public int Id { get; }

	public int TeamId { get; }

	public CivCommanderLineColor Color { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public float Rotation { get; }

	public string Text { get; }

	public NetUserId Owner { get; }

	public CivCommanderLabelState(int id, int teamId, CivCommanderLineColor color, MapId mapId, Vector2 position, float rotation, string text, NetUserId owner)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Id = id;
		TeamId = teamId;
		Color = color;
		MapId = mapId;
		Position = position;
		Rotation = rotation;
		Text = text;
		Owner = owner;
	}
}
