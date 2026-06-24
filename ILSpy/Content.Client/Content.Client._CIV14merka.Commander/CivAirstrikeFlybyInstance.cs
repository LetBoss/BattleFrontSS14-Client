using System;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Robust.Shared.Map;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeFlybyInstance
{
	public required Vector2 Origin;

	public required Vector2 Entry;

	public required Vector2 EntryCtr;

	public required Vector2 Approach;

	public required Vector2 Target;

	public required Vector2 RunEnd;

	public required Vector2 ExitTurn;

	public required Vector2 ExitCtr;

	public required Vector2 Exit;

	public required float EntryLineLen;

	public required float EntryArcLen;

	public required float ExitLen;

	public required bool EntryCcw;

	public required bool ExitCcw;

	public required float Speed;

	public required int Count;

	public required float Spacing;

	public required float Alpha;

	public required float ScaleMin;

	public required float ScaleMax;

	public required CivAirstrikeSide Side;

	public required MapId MapId;

	public required TimeSpan StartTime;

	public float RunInLen => (Target - Approach).Length();

	public float RunOutLen => (RunEnd - Target).Length();

	public float ExitLineLen => (Exit - ExitTurn).Length();

	public float ExitArcLen => MathF.Max(0f, ExitLen - ExitLineLen);

	public float EntryTurnEnd => EntryLineLen + EntryArcLen;

	public float ToTarget => EntryTurnEnd + RunInLen;

	public float RunEndDist => ToTarget + RunOutLen;

	public float ExitTurnEnd => RunEndDist + ExitArcLen;

	public float Total => ExitTurnEnd + ExitLineLen;
}
