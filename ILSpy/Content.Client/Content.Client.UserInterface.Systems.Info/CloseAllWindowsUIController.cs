using System.Collections.Generic;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Info;

public sealed class CloseAllWindowsUIController : UIController
{
	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	public override void Initialize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		_inputManager.SetInputCommand(EngineKeyFunctions.WindowCloseAll, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			CloseAllWindows();
		}, (StateInputCmdDelegate)null, true, true));
	}

	private void CloseAllWindows()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		foreach (Control item in new List<Control>((IEnumerable<Control>)((Control)_uiManager.WindowRoot).Children))
		{
			if (item is BaseWindow)
			{
				((BaseWindow)item).Close();
			}
		}
	}
}
