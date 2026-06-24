using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Emote;

[Serializable]
[NetSerializable]
public enum RMCHandsEmoteState : byte
{
	Fistbump,
	Highfive,
	Tailswipe,
	Hug
}
