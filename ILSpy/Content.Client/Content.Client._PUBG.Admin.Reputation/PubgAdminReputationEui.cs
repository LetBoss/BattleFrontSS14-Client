using Content.Client.Eui;
using Content.Shared._PUBG.Admin.Reputation;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.Admin.Reputation;

public sealed class PubgAdminReputationEui : BaseEui
{
	private PubgAdminReputationWindow? _window;

	private PubgAdminReputationAdjustWindow? _adjustWindow;

	private PubgAdminReputationState? _state;

	public override void Opened()
	{
		base.Opened();
		_window = new PubgAdminReputationWindow();
		((BaseButton)_window.AdjustButton).OnPressed += delegate
		{
			OpenAdjustWindow();
		};
		Refresh();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		PubgAdminReputationAdjustWindow? adjustWindow = _adjustWindow;
		if (adjustWindow != null)
		{
			((BaseWindow)adjustWindow).Close();
		}
		PubgAdminReputationWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (state is PubgAdminReputationState state2)
		{
			_state = state2;
			Refresh();
		}
	}

	public override void HandleMessage(EuiMessageBase msg)
	{
		base.HandleMessage(msg);
		if (_window != null && msg is PubgAdminReputationErrorMsg pubgAdminReputationErrorMsg)
		{
			_window.ErrorLabel.SetMessage(pubgAdminReputationErrorMsg.Message, (Color?)null);
		}
	}

	private void OpenAdjustWindow()
	{
		if (_window == null || _state == null || !_state.CanEdit)
		{
			return;
		}
		if (_adjustWindow != null)
		{
			((BaseWindow)_adjustWindow).MoveToFront();
			return;
		}
		_adjustWindow = new PubgAdminReputationAdjustWindow();
		((BaseButton)_adjustWindow.ApplyButton).OnPressed += delegate
		{
			ApplyAdjustment();
		};
		((BaseWindow)_adjustWindow).OnClose += delegate
		{
			_adjustWindow = null;
		};
		((BaseWindow)_adjustWindow).OpenCentered();
	}

	private void ApplyAdjustment()
	{
		if (_adjustWindow == null)
		{
			return;
		}
		if (!int.TryParse(_adjustWindow.AmountEdit.Text, out var result) || result <= 0)
		{
			_adjustWindow.ErrorLabel.SetMessage(Loc.GetString("pubg-reputation-admin-error-invalid-amount"), (Color?)null);
			return;
		}
		string text = Rope.Collapse(_adjustWindow.ReasonEdit.TextRope).Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			_adjustWindow.ErrorLabel.SetMessage(Loc.GetString("pubg-reputation-admin-error-reason-required"), (Color?)null);
			return;
		}
		PubgAdminReputationAdjustMsg msg = new PubgAdminReputationAdjustMsg(_adjustWindow.IsIncrease, result, text);
		SendMessage(msg);
		((BaseWindow)_adjustWindow).Close();
		_adjustWindow = null;
	}

	private void Refresh()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected O, but got Unknown
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Expected O, but got Unknown
		if (_window == null || _state == null)
		{
			return;
		}
		_window.ErrorLabel.Text = string.Empty;
		_window.PlayerLabel.Text = Loc.GetString("pubg-reputation-admin-player", new(string, object)[1] { ("player", _state.PlayerName) });
		_window.CurrentValueLabel.Text = Loc.GetString("pubg-reputation-admin-current", new(string, object)[1] { ("value", _state.CurrentReputation) });
		_window.CurrentValueLabel.FontColorOverride = ((_state.CurrentReputation < 80) ? Color.IndianRed : Color.LightGreen);
		((BaseButton)_window.AdjustButton).Disabled = !_state.CanEdit;
		((Control)_window.HistoryContainer).DisposeAllChildren();
		foreach (PubgReputationHistoryEntry item in _state.History)
		{
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 4
			};
			Label val2 = new Label
			{
				Text = item.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
				MinWidth = 160f
			};
			Label val3 = new Label
			{
				Text = item.ChangedByName,
				MinWidth = 150f
			};
			string text = ((item.Delta >= 0) ? $"+{item.Delta}" : item.Delta.ToString());
			Label val4 = new Label
			{
				Text = text,
				MinWidth = 100f,
				FontColorOverride = ((item.Delta >= 0) ? Color.LightGreen : Color.IndianRed)
			};
			Label val5 = new Label
			{
				Text = $"{item.OldValue}->{item.NewValue}",
				MinWidth = 120f
			};
			Label val6 = new Label
			{
				Text = item.Source,
				MinWidth = 90f
			};
			Label val7 = new Label
			{
				Text = (item.Reason ?? string.Empty),
				HorizontalExpand = true
			};
			((Control)val).AddChild((Control)(object)val2);
			((Control)val).AddChild((Control)(object)val3);
			((Control)val).AddChild((Control)(object)val4);
			((Control)val).AddChild((Control)(object)val5);
			((Control)val).AddChild((Control)(object)val6);
			((Control)val).AddChild((Control)(object)val7);
			((Control)_window.HistoryContainer).AddChild((Control)(object)val);
		}
	}
}
