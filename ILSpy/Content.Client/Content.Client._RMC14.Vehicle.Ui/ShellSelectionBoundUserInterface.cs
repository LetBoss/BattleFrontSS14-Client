using System;
using Content.Shared._RMC14.Vehicle.Weapons;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Vehicle.Ui;

public sealed class ShellSelectionBoundUserInterface : BoundUserInterface
{
	private ShellSelectionWindow? _window;

	public ShellSelectionBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = new ShellSelectionWindow();
		((BaseWindow)_window).OnClose += base.Close;
		_window.OnShellSelected += OnShellSelected;
		_window.OnCancel += OnCancel;
		((BaseWindow)_window).OpenCentered();
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			if (_window != null)
			{
				((BaseWindow)_window).OnClose -= base.Close;
				_window.OnShellSelected -= OnShellSelected;
				_window.OnCancel -= OnCancel;
			}
			ShellSelectionWindow? window = _window;
			if (window != null)
			{
				((Control)window).Dispose();
			}
			_window = null;
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is ShellSelectionUiState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	private void OnShellSelected(string protoId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ShellTypeSelectMessage(EntProtoId.op_Implicit(protoId)));
		((BoundUserInterface)this).Close();
	}

	private void OnCancel()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ShellSelectionCancelMessage());
		((BoundUserInterface)this).Close();
	}
}
