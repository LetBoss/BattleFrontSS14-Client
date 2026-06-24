using System;
using System.Numerics;
using Content.Client.Administration.UI.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Client.UserInterface.Systems.Bwoink;

public sealed class UserAHelpUIHandler : IAHelpUIHandler, IDisposable
{
	private readonly NetUserId _ownerId;

	private DefaultWindow? _window;

	private BwoinkPanel? _chatPanel;

	private bool _discordRelayActive;

	public bool IsAdmin => false;

	public bool IsOpen
	{
		get
		{
			DefaultWindow window = _window;
			if (window != null && !((Control)window).Disposed)
			{
				return ((BaseWindow)window).IsOpen;
			}
			return false;
		}
	}

	public Action<NetUserId, string, bool, bool>? SendMessageAction { get; set; }

	public event Action? OnClose;

	public event Action? OnOpen;

	public event Action<NetUserId, string>? InputTextChanged;

	public UserAHelpUIHandler(NetUserId owner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_ownerId = owner;
	}

	public void Receive(SharedBwoinkSystem.BwoinkTextMessage message)
	{
		EnsureInit(_discordRelayActive);
		_chatPanel.ReceiveLine(message);
		((BaseWindow)_window).OpenCentered();
	}

	public void Close()
	{
		DefaultWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	public void ToggleWindow()
	{
		EnsureInit(_discordRelayActive);
		if (((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).Close();
		}
		else
		{
			((BaseWindow)_window).OpenCentered();
		}
	}

	public void PopOut()
	{
	}

	public void DiscordRelayChanged(bool active)
	{
		_discordRelayActive = active;
		if (_chatPanel != null)
		{
			((Control)_chatPanel.RelayedToDiscordLabel).Visible = active;
		}
	}

	public void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args)
	{
	}

	public void Open(NetUserId channelId, bool relayActive)
	{
		EnsureInit(relayActive);
		((BaseWindow)_window).OpenCentered();
	}

	private void EnsureInit(bool relayActive)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		DefaultWindow window = _window;
		if (window == null || ((Control)window).Disposed)
		{
			_chatPanel = new BwoinkPanel(delegate(string arg)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				SendMessageAction?.Invoke(_ownerId, arg, arg3: true, arg4: false);
			});
			_chatPanel.InputTextChanged += delegate(string arg)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				this.InputTextChanged?.Invoke(_ownerId, arg);
			};
			((Control)_chatPanel.RelayedToDiscordLabel).Visible = relayActive;
			_window = new DefaultWindow
			{
				TitleClass = "windowTitleAlert",
				HeaderClass = "windowHeaderAlert",
				Title = Loc.GetString("bwoink-user-title"),
				MinSize = new Vector2(500f, 300f)
			};
			((BaseWindow)_window).OnClose += delegate
			{
				this.OnClose?.Invoke();
			};
			((BaseWindow)_window).OnOpen += delegate
			{
				this.OnOpen?.Invoke();
			};
			_window.Contents.AddChild((Control)(object)_chatPanel);
			string text = Loc.GetString("bwoink-system-introductory-message");
			SharedBwoinkSystem.BwoinkTextMessage message = new SharedBwoinkSystem.BwoinkTextMessage(_ownerId, SharedBwoinkSystem.SystemUserId, text);
			Receive(message);
		}
	}

	public void Dispose()
	{
		DefaultWindow? window = _window;
		if (window != null)
		{
			((Control)window).Orphan();
		}
		_window = null;
		_chatPanel = null;
	}
}
