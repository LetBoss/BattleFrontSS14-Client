using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.Prototypes;

[Serializable]
[Flags]
[NetSerializable]
public enum EmoteCategory : byte
{
	Invalid = 0,
	Vocal = 1,
	Hands = 2,
	General = byte.MaxValue
}
