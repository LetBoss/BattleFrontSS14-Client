using Content.Client.Changelog;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class ChangelogUIController : UIController
{
	private ChangelogWindow _changeLogWindow;

	public void OpenWindow()
	{
		EnsureWindow();
		((BaseWindow)_changeLogWindow).OpenCentered();
		((BaseWindow)_changeLogWindow).MoveToFront();
	}

	private void EnsureWindow()
	{
		ChangelogWindow changeLogWindow = _changeLogWindow;
		if (changeLogWindow == null || ((Control)changeLogWindow).Disposed)
		{
			_changeLogWindow = base.UIManager.CreateWindow<ChangelogWindow>();
		}
	}

	public void ToggleWindow()
	{
		EnsureWindow();
		if (((BaseWindow)_changeLogWindow).IsOpen)
		{
			((BaseWindow)_changeLogWindow).Close();
		}
		else
		{
			OpenWindow();
		}
	}
}
