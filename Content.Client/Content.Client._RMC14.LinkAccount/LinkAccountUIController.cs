using System;
using Content.Client.Lobby.UI;
using Content.Client.Message;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.LinkAccount;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountUIController : UIController, IOnSystemChanged<LinkAccountSystem>, IOnSystemLoaded<LinkAccountSystem>, IOnSystemUnloaded<LinkAccountSystem>
{
	[Dependency]
	private IClipboardManager _clipboard;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private LinkAccountManager _linkAccount;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IUriOpener _uriOpener;

	private LinkAccountWindow? _window;

	private PatronPerksWindow? _patronPerksWindow;

	private TimeSpan _disableUntil;

	private Guid _code;

	public override void Initialize()
	{
		_linkAccount.CodeReceived += OnCode;
		_linkAccount.Updated += OnUpdated;
	}

	private void OnCode(Guid code)
	{
		_code = code;
		if (_window != null)
		{
			((BaseButton)_window.CopyButton).Disabled = false;
		}
	}

	private void OnUpdated()
	{
		if (base.UIManager.ActiveScreen is LobbyGui lobbyGui)
		{
			((Control)lobbyGui.CharacterPreview.PatronPerks).Visible = _linkAccount.CanViewPatronPerks();
		}
	}

	private void OnLobbyMessageReceived(SharedRMCDisplayLobbyMessageEvent message)
	{
	}

	public void ToggleWindow()
	{
		if (_window == null)
		{
			_window = new LinkAccountWindow();
			((BaseWindow)_window).OnClose += delegate
			{
				_window = null;
			};
			_window.Label.SetMarkupPermissive(Loc.GetString("rmc-ui-link-discord-account-text") ?? "");
			if (_linkAccount.Linked)
			{
				_window.Label.SetMarkupPermissive(Loc.GetString("rmc-ui-link-discord-account-already-linked") + "\n\n" + Loc.GetString("rmc-ui-link-discord-account-text"));
			}
			((BaseButton)_window.CopyButton).OnPressed += delegate
			{
				_clipboard.SetText(_code.ToString());
				_window.CopyButton.Text = Loc.GetString("rmc-ui-link-discord-account-copied");
				((BaseButton)_window.CopyButton).Disabled = true;
				_disableUntil = _timing.RealTime.Add(TimeSpan.FromSeconds(3L));
			};
			string messageLink = _config.GetCVar<string>(RMCCVars.RMCDiscordAccountLinkingMessageLink);
			if (string.IsNullOrEmpty(messageLink))
			{
				((Control)_window.LinkButton).Visible = false;
				((Control)_window.CopyButton).RemoveStyleClass("OpenRight");
			}
			else
			{
				((Control)_window.LinkButton).Visible = true;
				((BaseButton)_window.LinkButton).OnPressed += delegate
				{
					_uriOpener.OpenUri(messageLink);
				};
				((Control)_window.CopyButton).AddStyleClass("OpenRight");
			}
			((BaseWindow)_window).OpenCentered();
			if (_code == default(Guid))
			{
				((BaseButton)_window.CopyButton).Disabled = true;
			}
			_net.ClientSendMessage((NetMessage)(object)new LinkAccountRequestMsg());
		}
		else
		{
			((BaseWindow)_window).Close();
			_window = null;
		}
	}

	public void TogglePatronPerksWindow()
	{
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		if (_patronPerksWindow == null)
		{
			_patronPerksWindow = new PatronPerksWindow();
			((BaseWindow)_patronPerksWindow).OnClose += delegate
			{
				_patronPerksWindow = null;
			};
			SharedRMCPatronTier tier = _linkAccount.Tier;
			TabContainer.SetTabTitle((Control)(object)_patronPerksWindow.LobbyMessageTab, Loc.GetString("rmc-ui-lobby-message"));
			TabContainer.SetTabVisible((Control)(object)_patronPerksWindow.LobbyMessageTab, tier?.LobbyMessage ?? false);
			_patronPerksWindow.LobbyMessage.OnTextEntered += ChangeLobbyMessage;
			_patronPerksWindow.LobbyMessage.OnFocusExit += ChangeLobbyMessage;
			string text = _linkAccount.LobbyMessage?.Message;
			if (text != null)
			{
				_patronPerksWindow.LobbyMessage.Text = text;
			}
			TabContainer.SetTabTitle((Control)(object)_patronPerksWindow.ShoutoutTab, Loc.GetString("rmc-ui-shoutout"));
			TabContainer.SetTabVisible((Control)(object)_patronPerksWindow.ShoutoutTab, tier?.RoundEndShoutout ?? false);
			_patronPerksWindow.MarineShoutout.OnTextEntered += ChangeMarineShoutout;
			_patronPerksWindow.MarineShoutout.OnFocusExit += ChangeMarineShoutout;
			string text2 = _linkAccount.RoundEndShoutout?.Marine;
			if (text2 != null)
			{
				_patronPerksWindow.MarineShoutout.Text = text2;
			}
			_patronPerksWindow.XenoShoutout.OnTextEntered += ChangeXenoShoutout;
			_patronPerksWindow.XenoShoutout.OnFocusExit += ChangeXenoShoutout;
			string text3 = _linkAccount.RoundEndShoutout?.Xeno;
			if (text3 != null)
			{
				_patronPerksWindow.XenoShoutout.Text = text3;
			}
			TabContainer.SetTabTitle((Control)(object)_patronPerksWindow.GhostColorTab, Loc.GetString("rmc-ui-ghost-color"));
			TabContainer.SetTabVisible((Control)(object)_patronPerksWindow.GhostColorTab, tier?.GhostColor ?? false);
			_patronPerksWindow.GhostColorSliders.Color = (Color)(((_003F?)_linkAccount.GhostColor) ?? Color.White);
			ColorSelectorSliders ghostColorSliders = _patronPerksWindow.GhostColorSliders;
			ghostColorSliders.OnColorChanged = (Action<Color>)Delegate.Combine(ghostColorSliders.OnColorChanged, new Action<Color>(OnGhostColorChanged));
			((BaseButton)_patronPerksWindow.GhostColorClearButton).OnPressed += OnGhostColorClear;
			((BaseButton)_patronPerksWindow.GhostColorSaveButton).OnPressed += OnGhostColorSave;
			TabContainer.SetTabTitle((Control)(object)_patronPerksWindow.NamedItemsReferenceTab, Loc.GetString("rmc-ui-named-items"));
			TabContainer.SetTabVisible((Control)(object)_patronPerksWindow.NamedItemsReferenceTab, tier?.NamedItems ?? false);
			TabContainer.SetTabTitle((Control)(object)_patronPerksWindow.FigurineReferenceTab, Loc.GetString("rmc-ui-figurine"));
			TabContainer.SetTabVisible((Control)(object)_patronPerksWindow.FigurineReferenceTab, tier?.Figurines ?? false);
			UpdateExamples();
			for (int num = 0; num < ((Control)_patronPerksWindow.Tabs).ChildCount; num++)
			{
				if (((Control)_patronPerksWindow.Tabs).GetChild(num).GetValue<bool>(TabContainer.TabVisibleProperty))
				{
					_patronPerksWindow.Tabs.CurrentTab = num;
					break;
				}
			}
			((BaseWindow)_patronPerksWindow).OpenCentered();
		}
		else
		{
			((BaseWindow)_patronPerksWindow).Close();
			_patronPerksWindow = null;
		}
	}

	private void ChangeLobbyMessage(LineEditEventArgs args)
	{
		string text = args.Text;
		if (text.Length > 40)
		{
			text = text.Substring(0, 40);
			PatronPerksWindow? patronPerksWindow = _patronPerksWindow;
			if (patronPerksWindow != null)
			{
				patronPerksWindow.LobbyMessage.SetText(text, false);
			}
		}
		_net.ClientSendMessage((NetMessage)(object)new RMCChangeLobbyMessageMsg
		{
			Text = text
		});
	}

	private void ChangeMarineShoutout(LineEditEventArgs args)
	{
		string text = args.Text;
		if (text.Length > 50)
		{
			text = text.Substring(0, 50);
			PatronPerksWindow? patronPerksWindow = _patronPerksWindow;
			if (patronPerksWindow != null)
			{
				patronPerksWindow.LobbyMessage.SetText(text, false);
			}
		}
		_net.ClientSendMessage((NetMessage)(object)new RMCChangeMarineShoutoutMsg
		{
			Name = text
		});
		UpdateExamples();
	}

	private void ChangeXenoShoutout(LineEditEventArgs args)
	{
		string text = args.Text;
		if (text.Length > 50)
		{
			text = text.Substring(0, 50);
			PatronPerksWindow? patronPerksWindow = _patronPerksWindow;
			if (patronPerksWindow != null)
			{
				patronPerksWindow.LobbyMessage.SetText(text, false);
			}
		}
		_net.ClientSendMessage((NetMessage)(object)new RMCChangeXenoShoutoutMsg
		{
			Name = text
		});
		UpdateExamples();
	}

	private void OnGhostColorChanged(Color color)
	{
		PatronPerksWindow patronPerksWindow = _patronPerksWindow;
		if (patronPerksWindow != null && ((BaseWindow)patronPerksWindow).IsOpen)
		{
			((BaseButton)_patronPerksWindow.GhostColorSaveButton).Disabled = false;
		}
	}

	private void OnGhostColorClear(ButtonEventArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		PatronPerksWindow patronPerksWindow = _patronPerksWindow;
		if (patronPerksWindow != null && ((BaseWindow)patronPerksWindow).IsOpen)
		{
			_patronPerksWindow.GhostColorSliders.Color = Color.White;
			((BaseButton)_patronPerksWindow.GhostColorSaveButton).Disabled = true;
			_net.ClientSendMessage((NetMessage)(object)new RMCClearGhostColorMsg());
		}
	}

	private void OnGhostColorSave(ButtonEventArgs args)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		PatronPerksWindow patronPerksWindow = _patronPerksWindow;
		if (patronPerksWindow != null && ((BaseWindow)patronPerksWindow).IsOpen)
		{
			((BaseButton)_patronPerksWindow.GhostColorSaveButton).Disabled = true;
			_net.ClientSendMessage((NetMessage)(object)new RMCChangeGhostColorMsg
			{
				Color = _patronPerksWindow.GhostColorSliders.Color
			});
		}
	}

	private void UpdateExamples()
	{
		if (_patronPerksWindow != null)
		{
			string text = _patronPerksWindow.MarineShoutout.Text.Trim();
			_patronPerksWindow.MarineShoutoutExample.SetMarkupPermissive(string.IsNullOrWhiteSpace(text) ? " " : (Loc.GetString("rmc-ui-shoutout-example") + " " + Loc.GetString("rmc-ui-shoutout-marine", new(string, object)[1] { ("name", text) })));
			string text2 = _patronPerksWindow.XenoShoutout.Text.Trim();
			_patronPerksWindow.XenoShoutoutExample.SetMarkupPermissive(string.IsNullOrWhiteSpace(text2) ? " " : (Loc.GetString("rmc-ui-shoutout-example") + " " + Loc.GetString("rmc-ui-shoutout-xeno", new(string, object)[1] { ("name", text2) })));
		}
	}

	public void OnSystemLoaded(LinkAccountSystem system)
	{
		system.LobbyMessageReceived += OnLobbyMessageReceived;
	}

	public void OnSystemUnloaded(LinkAccountSystem system)
	{
		system.LobbyMessageReceived -= OnLobbyMessageReceived;
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		if (_window != null)
		{
			TimeSpan realTime = _timing.RealTime;
			if (_disableUntil != default(TimeSpan) && realTime > _disableUntil)
			{
				_disableUntil = default(TimeSpan);
				_window.CopyButton.Text = Loc.GetString("rmc-ui-link-discord-account-copy");
				((BaseButton)_window.CopyButton).Disabled = false;
			}
		}
	}
}
