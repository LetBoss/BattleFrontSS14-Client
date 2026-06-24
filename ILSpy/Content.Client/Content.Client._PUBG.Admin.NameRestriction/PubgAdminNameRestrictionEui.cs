using Content.Client.Eui;
using Content.Shared._PUBG.Admin.NameRestriction;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client._PUBG.Admin.NameRestriction;

public sealed class PubgAdminNameRestrictionEui : BaseEui
{
	private PubgAdminNameRestrictionWindow? _window;

	private PubgAdminNameRestrictionState? _state;

	public override void Opened()
	{
		base.Opened();
		_window = new PubgAdminNameRestrictionWindow();
		((BaseButton)_window.ApplyButton).OnPressed += delegate
		{
			ApplyStatus();
		};
		Refresh();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		PubgAdminNameRestrictionWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (state is PubgAdminNameRestrictionState state2)
		{
			_state = state2;
			Refresh();
		}
	}

	public override void HandleMessage(EuiMessageBase msg)
	{
		base.HandleMessage(msg);
		if (_window != null && msg is PubgAdminNameRestrictionErrorMsg pubgAdminNameRestrictionErrorMsg)
		{
			_window.ErrorLabel.Text = pubgAdminNameRestrictionErrorMsg.Message;
		}
	}

	private void ApplyStatus()
	{
		if (_window != null && _state != null && _state.CanEdit)
		{
			SendMessage(new PubgAdminNameRestrictionSetStatusMsg(_window.SelectedRestricted));
		}
	}

	private void Refresh()
	{
		if (_window != null && _state != null)
		{
			string text = Loc.GetString("pubg-name-restriction-admin-never");
			string text2 = (string.IsNullOrWhiteSpace(_state.ChangedByCkey) ? text : _state.ChangedByCkey);
			string text3 = _state.ChangedAtUtc?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? text;
			_window.ErrorLabel.Text = string.Empty;
			_window.PlayerValue.Text = _state.PlayerCkey;
			_window.LastChangedByValue.Text = text2;
			_window.LastChangedAtValue.Text = text3;
			_window.SetSelectedRestricted(_state.IsRestricted);
			((BaseButton)_window.StatusOption).Disabled = !_state.CanEdit;
			((BaseButton)_window.ApplyButton).Disabled = !_state.CanEdit;
		}
	}
}
