using Content.Client.Eui;
using Content.Shared.Administration.Notes;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.AdminRemarks;

public sealed class AdminMessageEui : BaseEui
{
	private readonly AdminMessagePopupWindow _popup;

	public AdminMessageEui()
	{
		_popup = new AdminMessagePopupWindow();
		_popup.OnAcceptPressed += delegate
		{
			SendMessage(new AdminMessageEuiMsg.Dismiss(permanent: true));
		};
		_popup.OnDismissPressed += delegate
		{
			SendMessage(new AdminMessageEuiMsg.Dismiss(permanent: false));
		};
	}

	public override void HandleState(EuiStateBase state)
	{
		if (state is AdminMessageEuiState state2)
		{
			_popup.SetState(state2);
		}
	}

	public override void Opened()
	{
		((Control)((Control)_popup).UserInterfaceManager.WindowRoot).AddChild((Control)(object)_popup);
		LayoutContainer.SetAnchorPreset((Control)(object)_popup, (LayoutPreset)15, false);
	}

	public override void Closed()
	{
		((Control)_popup).Orphan();
	}
}
