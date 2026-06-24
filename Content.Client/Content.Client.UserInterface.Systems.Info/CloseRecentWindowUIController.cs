using System.Collections.Generic;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Info;

public sealed class CloseRecentWindowUIController : UIController
{
	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	private List<BaseWindow> recentlyInteractedWindows = new List<BaseWindow>();

	public override void Initialize()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		_uiManager.OnKeyBindDown += OnKeyBindDown;
		((Control)_uiManager.WindowRoot).OnChildAdded += OnRootChildAdded;
		_inputManager.SetInputCommand(EngineKeyFunctions.WindowCloseRecent, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			CloseMostRecentWindow();
		}, (StateInputCmdDelegate)null, true, true));
	}

	public void CloseMostRecentWindow()
	{
		for (int num = recentlyInteractedWindows.Count - 1; num >= 0; num--)
		{
			BaseWindow val = recentlyInteractedWindows[num];
			recentlyInteractedWindows.RemoveAt(num);
			if (val.IsOpen)
			{
				val.Close();
				break;
			}
		}
	}

	private void OnKeyBindDown(Control control)
	{
		BaseWindow windowForControl = GetWindowForControl(control);
		if (windowForControl != null)
		{
			SetMostRecentlyInteractedWindow(windowForControl);
		}
	}

	public void SetMostRecentlyInteractedWindow(BaseWindow window)
	{
		for (int num = recentlyInteractedWindows.Count - 1; num >= 0; num--)
		{
			if (recentlyInteractedWindows[num] == window)
			{
				if (num == recentlyInteractedWindows.Count - 1)
				{
					return;
				}
				recentlyInteractedWindows.RemoveAt(num);
				break;
			}
		}
		recentlyInteractedWindows.Add(window);
	}

	private BaseWindow? GetWindowForControl(Control? control)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		if (control == null)
		{
			return null;
		}
		if (control is BaseWindow)
		{
			return (BaseWindow)control;
		}
		return GetWindowForControl(control.Parent);
	}

	private void OnRootChildAdded(Control control)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		if (control is BaseWindow)
		{
			SetMostRecentlyInteractedWindow((BaseWindow)control);
		}
	}

	public bool HasClosableWindow()
	{
		for (int num = recentlyInteractedWindows.Count - 1; num >= 0; num--)
		{
			if (recentlyInteractedWindows[num].IsOpen)
			{
				return true;
			}
		}
		return false;
	}
}
