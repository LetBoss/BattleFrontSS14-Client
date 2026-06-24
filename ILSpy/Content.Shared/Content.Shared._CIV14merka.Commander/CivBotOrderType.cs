using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public enum CivBotOrderType : byte
{
	Idle,
	Move,
	AttackMove,
	HoldPosition,
	Follow,
	Defend,
	Patrol
}
