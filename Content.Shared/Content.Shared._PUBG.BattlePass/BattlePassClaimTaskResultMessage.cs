using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassClaimTaskResultMessage : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public int XpAwarded { get; }

	public BattlePassClaimTaskResultMessage(bool success, string? error = null, int xpAwarded = 0)
	{
		Success = success;
		Error = error;
		XpAwarded = xpAwarded;
	}
}
