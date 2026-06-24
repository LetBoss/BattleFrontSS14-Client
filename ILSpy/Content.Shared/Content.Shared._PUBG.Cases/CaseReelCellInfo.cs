using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Cases;

[Serializable]
[NetSerializable]
public sealed class CaseReelCellInfo
{
	public CaseRewardKind Kind { get; }

	public string? ItemId { get; }

	public int? Amount { get; }

	public bool IsDuplicateRecipe { get; }

	public int? DuplicateCompensationScrap { get; }

	public CaseReelCellInfo(CaseRewardKind kind, string? itemId = null, int? amount = null, bool isDuplicateRecipe = false, int? duplicateCompensationScrap = null)
	{
		Kind = kind;
		ItemId = itemId;
		Amount = amount;
		IsDuplicateRecipe = isDuplicateRecipe;
		DuplicateCompensationScrap = duplicateCompensationScrap;
	}
}
