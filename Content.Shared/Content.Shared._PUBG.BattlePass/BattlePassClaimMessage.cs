using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassClaimMessage : EntityEventArgs
{
	public string RewardId { get; }

	public BattlePassClaimMessage(string rewardId)
	{
		RewardId = rewardId;
	}
}
