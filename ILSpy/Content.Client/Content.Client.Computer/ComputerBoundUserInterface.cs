using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Computer;

[Virtual]
public class ComputerBoundUserInterface<TWindow, TState> : ComputerBoundUserInterfaceBase where TWindow : BaseWindow, IComputerWindow<TState>, new() where TState : BoundUserInterfaceState
{
	[ViewVariables]
	private TWindow? _window;

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<TWindow>((BoundUserInterface)(object)this);
		_window.SetupComputerWindow(this);
	}

	public ComputerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null)
		{
			_window.UpdateState((TState)(object)state);
		}
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		((IComputerWindow<TState>)_window)?.ReceiveMessage(message);
	}
}
