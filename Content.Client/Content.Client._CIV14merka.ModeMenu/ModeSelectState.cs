using System;
using Content.Client._CIV14merka.ModeMenu.UI;
using Content.Client.GameTicking.Managers;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._CIV14merka.ModeMenu;

public sealed class ModeSelectState : State
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	private ClientGameTicker _gameTicker;

	protected override Type? LinkedScreenType => typeof(ModeSelectGui);

	public ModeSelectGui? Screen { get; private set; }

	protected override void Startup()
	{
		if (_userInterfaceManager.ActiveScreen != null)
		{
			Screen = (ModeSelectGui)(object)_userInterfaceManager.ActiveScreen;
			_gameTicker = _entityManager.System<ClientGameTicker>();
			((BaseButton)Screen.PubgButton).OnPressed += OnPubgPressed;
			((BaseButton)Screen.Civ14Button).OnPressed += OnCivPressed;
			_gameTicker.ModeMenuStatusUpdated += UpdateUi;
			UpdateUi();
		}
	}

	protected override void Shutdown()
	{
		if (Screen != null)
		{
			((BaseButton)Screen.PubgButton).OnPressed -= OnPubgPressed;
			((BaseButton)Screen.Civ14Button).OnPressed -= OnCivPressed;
		}
		if (_gameTicker != null)
		{
			_gameTicker.ModeMenuStatusUpdated -= UpdateUi;
		}
		Screen = null;
	}

	private void OnPubgPressed(ButtonEventArgs args)
	{
		_gameTicker.SelectPubgMode();
	}

	private void OnCivPressed(ButtonEventArgs args)
	{
		_gameTicker.SelectCiv14Mode();
	}

	private void UpdateUi()
	{
		if (Screen != null)
		{
			Screen.ServerOnlineLabel.Text = Loc.GetString("civ-ui-mode-online", new(string, object)[1] { ("count", _gameTicker.ServerOnlineCount) });
			UpdateModeButton(Screen.PubgButton, "PUBG", _gameTicker.PubgModeOnlineCount, _gameTicker.IsPubgModeSelectable);
			UpdateModeButton(Screen.Civ14Button, "TDM", _gameTicker.Civ14ModeOnlineCount, _gameTicker.IsCiv14ModeSelectable);
		}
	}

	private static void UpdateModeButton(Button button, string modeName, int onlineCount, bool enabled)
	{
		((BaseButton)button).Disabled = !enabled;
		button.Text = (enabled ? Loc.GetString("civ-ui-mode-button-online", new(string, object)[2]
		{
			("mode", modeName),
			("count", onlineCount)
		}) : Loc.GetString("civ-ui-mode-button-disabled", new(string, object)[1] { ("mode", modeName) }));
	}
}
