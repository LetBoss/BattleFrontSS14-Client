using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Capture;

[Serializable]
[NetSerializable]
public sealed class CivPointCapturePointState
{
	public int Id { get; }

	public string Label { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public int OwnerTeamId { get; }

	public int CapturingTeamId { get; }

	public float CaptureProgress { get; }

	public CivPointCapturePointState(int id, string label, MapId mapId, Vector2 position, int ownerTeamId, int capturingTeamId, float captureProgress)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Id = id;
		Label = label;
		MapId = mapId;
		Position = position;
		OwnerTeamId = ownerTeamId;
		CapturingTeamId = capturingTeamId;
		CaptureProgress = captureProgress;
	}
}
