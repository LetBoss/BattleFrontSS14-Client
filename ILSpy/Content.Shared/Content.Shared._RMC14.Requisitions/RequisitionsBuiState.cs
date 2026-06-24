using System;
using Content.Shared._RMC14.Requisitions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Requisitions;

[Serializable]
[NetSerializable]
public sealed class RequisitionsBuiState : BoundUserInterfaceState
{
	public RequisitionsElevatorMode? PlatformLowered;

	public bool Busy;

	public int Balance;

	public bool Full;

	public RequisitionsBuiState(RequisitionsElevatorMode? platformLowered, bool busy, int balance, bool full)
	{
		PlatformLowered = platformLowered;
		Busy = busy;
		Balance = balance;
		Full = full;
	}
}
