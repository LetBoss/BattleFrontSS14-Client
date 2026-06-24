using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public enum CivRosterPlayerState : byte
{
	Lobby,
	Ready,
	Joined,
	Disconnected
}
