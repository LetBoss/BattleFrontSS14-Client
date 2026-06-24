using System;
using Content.Client.Computer;
using Content.Shared.Solar;
using Robust.Shared.GameObjects;

namespace Content.Client.Power;

public sealed class SolarControlConsoleBoundUserInterface : ComputerBoundUserInterface<SolarControlWindow, SolarControlConsoleBoundInterfaceState>
{
	public SolarControlConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)

}
