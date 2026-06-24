using Content.Client.Eui;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Admin.GordenLox;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Admin.GordenLox;

public sealed class GordenLoxEui : BaseEui
{
	private GordenLoxWindow? _window;

	private GordenLoxState? _state;

	public override void Opened()
	{
		base.Opened();
		_window = new GordenLoxWindow();
		Refresh();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		GordenLoxWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (state is GordenLoxState state2)
		{
			_state = state2;
			Refresh();
		}
	}

	private void Refresh()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		if (_window == null || _state == null)
		{
			return;
		}
		_window.CountLabel.Text = Loc.GetString("pubg-gordenlox-admin-total", new(string, object)[1] { ("count", _state.Alerts.Count) });
		((Control)_window.HistoryContainer).DisposeAllChildren();
		if (_state.Alerts.Count == 0)
		{
			((Control)_window.HistoryContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("pubg-gordenlox-admin-empty"),
				FontColorOverride = Color.Gray
			});
			return;
		}
		for (int num = _state.Alerts.Count - 1; num >= 0; num--)
		{
			GordenLoxAlertEntry alert = _state.Alerts[num];
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 8
			};
			((Control)val).AddChild((Control)new Label
			{
				Text = alert.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
				MinWidth = 170f
			});
			((Control)val).AddChild((Control)new Label
			{
				Text = alert.Message,
				HorizontalExpand = true
			});
			ConfirmButton obj = new ConfirmButton
			{
				Text = Loc.GetString("pubg-gordenlox-admin-delete"),
				ConfirmationText = Loc.GetString("pubg-gordenlox-admin-delete-confirm")
			};
			((Control)obj).MinWidth = 120f;
			((Control)obj).StyleClasses.Add("OpenBoth");
			ConfirmButton confirmButton = obj;
			confirmButton.OnPressed += delegate
			{
				SendMessage(new GordenLoxDeleteAlertMsg(alert.Id));
			};
			((Control)val).AddChild((Control)(object)confirmButton);
			((Control)_window.HistoryContainer).AddChild((Control)(object)val);
		}
	}
}
