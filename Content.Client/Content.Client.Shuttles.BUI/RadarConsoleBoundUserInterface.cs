using System;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Shuttles.BUI;

public sealed class RadarConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private RadarConsoleWindow? _window;

	public RadarConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RadarConsoleWindow>((BoundUserInterface)(object)this);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is NavBoundUserInterfaceState navBoundUserInterfaceState)
		{
			_window?.UpdateState(navBoundUserInterfaceState.State);
		}
	}
}
