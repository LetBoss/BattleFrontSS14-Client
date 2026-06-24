using System;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
public enum PlayerGameStatus : sbyte
{
	NotReadyToPlay,
	ReadyToPlay,
	JoinedGame
}
