using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Cases;

[Serializable]
[NetSerializable]
public sealed class CaseOpenResultMessage : EntityEventArgs
{
	public string CaseId { get; }

	public int WinningIndex { get; }

	public List<CaseReelCellInfo> Cells { get; }

	public CaseRewardInfo Reward { get; }

	public CaseOpenResultMessage(string caseId, int winningIndex, List<CaseReelCellInfo> cells, CaseRewardInfo reward)
	{
		CaseId = caseId;
		WinningIndex = winningIndex;
		Cells = cells;
		Reward = reward;
	}
}
