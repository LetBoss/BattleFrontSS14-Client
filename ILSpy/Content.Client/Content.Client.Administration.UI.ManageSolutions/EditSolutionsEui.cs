using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Administration.UI.ManageSolutions;

public sealed class EditSolutionsEui : BaseEui
{
	private readonly EditSolutionsWindow _window;

	public EditSolutionsEui()
	{
		_window = new EditSolutionsWindow();
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void Opened()
	{
		base.Opened();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_window).Close();
	}

	public override void HandleState(EuiStateBase baseState)
	{
		EditSolutionsEuiState state = (EditSolutionsEuiState)baseState;
		_window.SetState(state);
	}
}
