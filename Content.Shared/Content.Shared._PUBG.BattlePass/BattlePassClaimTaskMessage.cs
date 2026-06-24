using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassClaimTaskMessage : EntityEventArgs
{
	public string TaskId { get; }

	public BattlePassClaimTaskMessage(string taskId)
	{
		TaskId = taskId;
	}
}
