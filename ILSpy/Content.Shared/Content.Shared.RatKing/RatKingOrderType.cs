using System;
using Robust.Shared.Serialization;

namespace Content.Shared.RatKing;

[Serializable]
[NetSerializable]
public enum RatKingOrderType : byte
{
	Stay,
	Follow,
	CheeseEm,
	Loose
}
