using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivAirstrikeFlybyEvent : EntityEventArgs
{
	public Vector2 Origin { get; }

	public Vector2 Entry { get; }

	public Vector2 EntryCtr { get; }

	public Vector2 Approach { get; }

	public Vector2 Target { get; }

	public Vector2 RunEnd { get; }

	public Vector2 ExitTurn { get; }

	public Vector2 ExitCtr { get; }

	public Vector2 Exit { get; }

	public float EntryLineLen { get; }

	public float EntryArcLen { get; }

	public float ExitLen { get; }

	public bool EntryCcw { get; }

	public bool ExitCcw { get; }

	public float Speed { get; }

	public int Count { get; }

	public float Spacing { get; }

	public float Alpha { get; }

	public float StartDelay { get; }

	public float ScaleMin { get; }

	public float ScaleMax { get; }

	public CivAirstrikeSide Side { get; }

	public MapId MapId { get; }

	public CivAirstrikeFlybyEvent(Vector2 origin, Vector2 entry, Vector2 entryCtr, Vector2 approach, Vector2 target, Vector2 runEnd, Vector2 exitTurn, Vector2 exitCtr, Vector2 exit, float entryLineLen, float entryArcLen, float exitLen, bool entryCcw, bool exitCcw, float speed, int count, float spacing, float alpha, float startDelay, float scaleMin, float scaleMax, CivAirstrikeSide side, MapId mapId)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Origin = origin;
		Entry = entry;
		EntryCtr = entryCtr;
		Approach = approach;
		Target = target;
		RunEnd = runEnd;
		ExitTurn = exitTurn;
		ExitCtr = exitCtr;
		Exit = exit;
		EntryLineLen = entryLineLen;
		EntryArcLen = entryArcLen;
		ExitLen = exitLen;
		EntryCcw = entryCcw;
		ExitCcw = exitCcw;
		Speed = speed;
		Count = count;
		Spacing = spacing;
		Alpha = alpha;
		StartDelay = startDelay;
		ScaleMin = scaleMin;
		ScaleMax = scaleMax;
		Side = side;
		MapId = mapId;
	}
}
