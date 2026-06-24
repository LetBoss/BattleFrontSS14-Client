using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassLevelInfo
{
	public int Level { get; set; }

	public int XpRequired { get; set; }

	public List<BattlePassRewardInfo> Rewards { get; set; } = new List<BattlePassRewardInfo>();
}
