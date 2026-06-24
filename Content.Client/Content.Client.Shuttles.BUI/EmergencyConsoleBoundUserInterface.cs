using System;
using Content.Client.Computer;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Robust.Shared.GameObjects;

namespace Content.Client.Shuttles.BUI;

public sealed class EmergencyConsoleBoundUserInterface : ComputerBoundUserInterface<EmergencyConsoleWindow, EmergencyConsoleBoundUserInterfaceState>
{
	public EmergencyConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)

}
