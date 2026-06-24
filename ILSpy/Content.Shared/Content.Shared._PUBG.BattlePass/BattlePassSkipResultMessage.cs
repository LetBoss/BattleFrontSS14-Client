using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassSkipResultMessage : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public int SkipsRemaining { get; }

	public BattlePassSkipResultMessage(bool success, string? error = null, int skipsRemaining = 0)
	{
		Success = success;
		Error = error;
		SkipsRemaining = skipsRemaining;
	}
}
