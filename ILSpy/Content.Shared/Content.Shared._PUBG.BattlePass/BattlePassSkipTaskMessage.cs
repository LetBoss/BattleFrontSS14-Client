using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassSkipTaskMessage : EntityEventArgs
{
	public string TaskId { get; }

	public BattlePassSkipTaskMessage(string taskId)
	{
		TaskId = taskId;
	}
}
