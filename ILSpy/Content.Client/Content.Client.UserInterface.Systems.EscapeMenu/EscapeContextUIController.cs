using Content.Client.UserInterface.Systems.Info;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class EscapeContextUIController : UIController
{
	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private CloseRecentWindowUIController _closeRecentWindowUIController;

	[Dependency]
	private EscapeUIController _escapeUIController;

	public override void Initialize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		_inputManager.SetInputCommand(ContentKeyFunctions.EscapeContext, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			CloseWindowOrOpenGameMenu();
		}, (StateInputCmdDelegate)null, true, true));
	}

	private void CloseWindowOrOpenGameMenu()
	{
		if (_closeRecentWindowUIController.HasClosableWindow())
		{
			_closeRecentWindowUIController.CloseMostRecentWindow();
		}
		else
		{
			_escapeUIController.ToggleWindow();
		}
	}
}
