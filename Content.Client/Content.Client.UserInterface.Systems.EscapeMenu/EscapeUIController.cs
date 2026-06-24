using Content.Client._RMC14.LinkAccount;
using Content.Client.Gameplay;
using Content.Client.Options.UI;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Guidebook;
using Content.Client.UserInterface.Systems.Info;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.CCVar;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class EscapeUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	[Dependency]
	private IClientConsoleHost _console;

	[Dependency]
	private IUriOpener _uri;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private ChangelogUIController _changelog;

	[Dependency]
	private InfoUIController _info;

	[Dependency]
	private OptionsUIController _options;

	[Dependency]
	private GuidebookUIController _guidebook;

	[Dependency]
	private LinkAccountManager _linkAccount;

	private Content.Client.Options.UI.EscapeMenu? _escapeWindow;

	private MenuButton? EscapeButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.EscapeButton;

	public override void Initialize()
	{
		_linkAccount.Updated += delegate
		{
			if (_escapeWindow != null)
			{
				((Control)_escapeWindow.PatronPerksButton).Visible = _linkAccount.CanViewPatronPerks();
			}
		};
	}

	public void UnloadButton()
	{
		if (EscapeButton != null)
		{
			((BaseButton)EscapeButton).Pressed = false;
			((BaseButton)EscapeButton).OnPressed -= EscapeButtonOnOnPressed;
		}
	}

	public void LoadButton()
	{
		if (EscapeButton != null)
		{
			((BaseButton)EscapeButton).OnPressed += EscapeButtonOnOnPressed;
		}
	}

	private void ActivateButton()
	{
		((BaseButton)EscapeButton).SetClickPressed(true);
	}

	private void DeactivateButton()
	{
		((BaseButton)EscapeButton).SetClickPressed(false);
	}

	public void OnStateEntered(GameplayState state)
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		_escapeWindow = base.UIManager.CreateWindow<Content.Client.Options.UI.EscapeMenu>();
		((BaseWindow)_escapeWindow).OnClose += DeactivateButton;
		((BaseWindow)_escapeWindow).OnOpen += ActivateButton;
		((BaseButton)_escapeWindow.ChangelogButton).OnPressed += delegate
		{
			CloseEscapeWindow();
			_changelog.ToggleWindow();
		};
		((BaseButton)_escapeWindow.DiscordButton).OnPressed += delegate
		{
			_uri.OpenUri("https://discord.gg/xdQ4vSKRB8");
		};
		((Control)_escapeWindow.PatronPerksButton).Visible = _linkAccount.CanViewPatronPerks();
		((BaseButton)_escapeWindow.PatronPerksButton).OnPressed += delegate
		{
			CloseEscapeWindow();
			base.UIManager.GetUIController<LinkAccountUIController>().TogglePatronPerksWindow();
		};
		((BaseButton)_escapeWindow.RulesButton).OnPressed += delegate
		{
			CloseEscapeWindow();
			_info.OpenWindow();
		};
		((BaseButton)_escapeWindow.DisconnectButton).OnPressed += delegate
		{
			CloseEscapeWindow();
			((IConsoleHost)_console).ExecuteCommand("disconnect");
		};
		((BaseButton)_escapeWindow.OptionsButton).OnPressed += delegate
		{
			CloseEscapeWindow();
			_options.OpenWindow();
		};
		((BaseButton)_escapeWindow.QuitButton).OnPressed += delegate
		{
			CloseEscapeWindow();
			((IConsoleHost)_console).ExecuteCommand("quit");
		};
		((BaseButton)_escapeWindow.WikiButton).OnPressed += delegate
		{
			_uri.OpenUri(_cfg.GetCVar<string>(CCVars.InfoLinksWiki));
		};
		((BaseButton)_escapeWindow.GuidebookButton).OnPressed += delegate
		{
			_guidebook.ToggleGuidebook();
		};
		((Control)_escapeWindow.WikiButton).Visible = _cfg.GetCVar<string>(CCVars.InfoLinksWiki) != "";
		CommandBinds.Builder.Bind(EngineKeyFunctions.EscapeMenu, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleWindow();
		}, (StateInputCmdDelegate)null, true, true)).Register<EscapeUIController>();
	}

	public void OnStateExited(GameplayState state)
	{
		if (_escapeWindow != null)
		{
			if (!((Control)_escapeWindow).Disposed)
			{
				((Control)_escapeWindow).Orphan();
			}
			_escapeWindow = null;
		}
		CommandBinds.Unregister<EscapeUIController>();
	}

	private void EscapeButtonOnOnPressed(ButtonEventArgs obj)
	{
		ToggleWindow();
	}

	private void CloseEscapeWindow()
	{
		Content.Client.Options.UI.EscapeMenu? escapeWindow = _escapeWindow;
		if (escapeWindow != null)
		{
			((BaseWindow)escapeWindow).Close();
		}
	}

	public void ToggleWindow()
	{
		if (_escapeWindow != null)
		{
			if (((BaseWindow)_escapeWindow).IsOpen)
			{
				CloseEscapeWindow();
				((BaseButton)EscapeButton).Pressed = false;
			}
			else
			{
				((BaseWindow)_escapeWindow).OpenCentered();
				((BaseButton)EscapeButton).Pressed = true;
			}
		}
	}
}
