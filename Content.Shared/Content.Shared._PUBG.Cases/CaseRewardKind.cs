using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Cases;

[Serializable]
[NetSerializable]
public enum CaseRewardKind : byte
{
	Coins,
	Scrap,
	Recipe
}
