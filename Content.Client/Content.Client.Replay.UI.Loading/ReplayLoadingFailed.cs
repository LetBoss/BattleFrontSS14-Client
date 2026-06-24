using System;
using Content.Client.Stylesheets;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client.Replay.UI.Loading;

public sealed class ReplayLoadingFailed : State
{
	[Dependency]
	private IStylesheetManager _stylesheetManager;

	[Dependency]
	private IUserInterfaceManager _userInterface;

	private ReplayLoadingFailedControl? _control;

	public void SetData(Exception exception, Action? cancelPressed, Action? retryPressed)
	{
		_control.SetData(exception, cancelPressed, retryPressed);
	}

	protected override void Startup()
	{
		_control = new ReplayLoadingFailedControl(_stylesheetManager);
		((Control)_userInterface.StateRoot).AddChild((Control)(object)_control);
	}

	protected override void Shutdown()
	{
		ReplayLoadingFailedControl? control = _control;
		if (control != null)
		{
			((Control)control).Orphan();
		}
	}
}
