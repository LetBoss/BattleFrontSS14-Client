using System;
using Content.Shared.Robotics;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Robotics.UI;

public sealed class RoboticsConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	public RoboticsConsoleWindow _window;

	public RoboticsConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RoboticsConsoleWindow>((BoundUserInterface)(object)this);
		_window.SetEntity(((BoundUserInterface)this).Owner);
		RoboticsConsoleWindow window = _window;
		window.OnDisablePressed = (Action<string>)Delegate.Combine(window.OnDisablePressed, (Action<string>)delegate(string address)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new RoboticsConsoleDisableMessage(address));
		});
		RoboticsConsoleWindow window2 = _window;
		window2.OnDestroyPressed = (Action<string>)Delegate.Combine(window2.OnDestroyPressed, (Action<string>)delegate(string address)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new RoboticsConsoleDestroyMessage(address));
		});
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is RoboticsConsoleState state2)
		{
			_window.UpdateState(state2);
		}
	}
}
