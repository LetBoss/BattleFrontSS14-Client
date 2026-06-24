using System;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public sealed class ReagentGrinderInterfaceState : BoundUserInterfaceState
{
	public bool IsBusy;

	public bool HasBeakerIn;

	public bool Powered;

	public bool CanJuice;

	public bool CanGrind;

	public NetEntity[] ChamberContents;

	public ReagentQuantity[]? ReagentQuantities;

	public GrinderAutoMode AutoMode;

	public ReagentGrinderInterfaceState(bool isBusy, bool hasBeaker, bool powered, bool canJuice, bool canGrind, GrinderAutoMode autoMode, NetEntity[] chamberContents, ReagentQuantity[]? heldBeakerContents)
	{
		IsBusy = isBusy;
		HasBeakerIn = hasBeaker;
		Powered = powered;
		CanJuice = canJuice;
		CanGrind = canGrind;
		AutoMode = autoMode;
		ChamberContents = chamberContents;
		ReagentQuantities = heldBeakerContents;
	}
}
