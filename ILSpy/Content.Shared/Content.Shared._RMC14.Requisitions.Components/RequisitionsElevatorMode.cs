using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Requisitions.Components;

[Serializable]
[NetSerializable]
public enum RequisitionsElevatorMode
{
	Lowered,
	Raised,
	Lowering,
	Raising,
	Preparing
}
