using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLineState
{
	public int Id { get; }

	public int TeamId { get; }

	public CivCommanderLineColor Color { get; }

	public MapId MapId { get; }

	public Vector2 Start { get; }

	public Vector2 End { get; }

	public NetUserId Owner { get; }

	public CivCommanderLineState(int id, int teamId, CivCommanderLineColor color, MapId mapId, Vector2 start, Vector2 end, NetUserId owner)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Id = id;
		TeamId = teamId;
		Color = color;
		MapId = mapId;
		Start = start;
		End = end;
		Owner = owner;
	}
}
