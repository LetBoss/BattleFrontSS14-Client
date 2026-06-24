using System;
using Content.Shared.Cloning.CloningConsole;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.CloningConsole.UI;

public sealed class CloningConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private CloningConsoleWindow? _window;

	public CloningConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<CloningConsoleWindow>((BoundUserInterface)(object)this);
		((DefaultWindow)_window).Title = Loc.GetString("cloning-console-window-title");
		((BaseButton)_window.CloneButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new UiButtonPressedMessage(UiButton.Clone));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		_window?.Populate((CloningConsoleBoundUserInterfaceState)(object)state);
	}
}
