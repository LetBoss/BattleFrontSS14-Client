using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Client.Administration.UI.Bwoink;
using Content.Shared.Administration;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Network;

namespace Content.Client.UserInterface.Systems.Bwoink;

public sealed class AdminAHelpUIHandler : IAHelpUIHandler, IDisposable
{
	private readonly NetUserId _ownerId;

	private readonly Dictionary<NetUserId, BwoinkPanel> _activePanelMap = new Dictionary<NetUserId, BwoinkPanel>();

	public bool EverOpened;

	public BwoinkWindow? Window;

	public WindowRoot? WindowRoot;

	public IClydeWindow? ClydeWindow;

	public BwoinkControl? Control;

	public bool IsAdmin => true;

	public bool IsOpen
	{
		get
		{
			BwoinkWindow window = Window;
			if (window == null || ((Control)window).Disposed || !((BaseWindow)window).IsOpen)
			{
				IClydeWindow clydeWindow = ClydeWindow;
				if (clydeWindow != null)
				{
					return !clydeWindow.IsDisposed;
				}
				return false;
			}
			return true;
		}
	}

	public Action<NetUserId, string, bool, bool>? SendMessageAction { get; set; }

	public event Action? OnClose;

	public event Action? OnOpen;

	public event Action<NetUserId, string>? InputTextChanged;

	public AdminAHelpUIHandler(NetUserId owner)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_ownerId = owner;
	}

	public void Receive(SharedBwoinkSystem.BwoinkTextMessage message)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EnsurePanel(message.UserId).ReceiveLine(message);
		Control?.OnBwoink(message.UserId);
	}

	private void OpenWindow()
	{
		if (Window != null)
		{
			if (EverOpened)
			{
				((BaseWindow)Window).Open();
			}
			else
			{
				((BaseWindow)Window).OpenCentered();
			}
		}
	}

	public void Close()
	{
		BwoinkWindow? window = Window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		if (ClydeWindow == null)
		{
			return;
		}
		ClydeWindow.RequestClosed -= OnRequestClosed;
		((IDisposable)ClydeWindow).Dispose();
		if (Control != null)
		{
			foreach (KeyValuePair<NetUserId, BwoinkPanel> item in _activePanelMap)
			{
				item.Deconstruct(out var _, out var value);
				((Control)value).Orphan();
			}
			BwoinkControl? control = Control;
			if (control != null)
			{
				((Control)control).Orphan();
			}
		}
		this.OnClose?.Invoke();
	}

	public void ToggleWindow()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EnsurePanel(_ownerId);
		if (IsOpen)
		{
			Close();
		}
		else
		{
			OpenWindow();
		}
	}

	public void DiscordRelayChanged(bool active)
	{
	}

	public void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (_activePanelMap.TryGetValue(args.Channel, out BwoinkPanel value))
		{
			value.UpdatePlayerTyping(args.PlayerName, args.Typing);
		}
	}

	public void Open(NetUserId channelId, bool relayActive)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SelectChannel(channelId);
		OpenWindow();
	}

	public void OnRequestClosed(WindowRequestClosedEventArgs args)
	{
		Close();
	}

	private void EnsureControl()
	{
		BwoinkControl control = Control;
		if (control != null && !((Control)control).Disposed)
		{
			return;
		}
		Window = new BwoinkWindow();
		Control = Window.Bwoink;
		((BaseWindow)Window).OnClose += delegate
		{
			this.OnClose?.Invoke();
		};
		((BaseWindow)Window).OnOpen += delegate
		{
			this.OnOpen?.Invoke();
			EverOpened = true;
		};
		foreach (var (_, bwoinkPanel2) in _activePanelMap)
		{
			if (!((Control)Control.BwoinkArea).Children.Contains((Control)(object)bwoinkPanel2))
			{
				((Control)Control.BwoinkArea).AddChild((Control)(object)bwoinkPanel2);
			}
			((Control)bwoinkPanel2).Visible = false;
		}
	}

	public void HideAllPanels()
	{
		foreach (BwoinkPanel value in _activePanelMap.Values)
		{
			((Control)value).Visible = false;
		}
	}

	public BwoinkPanel EnsurePanel(NetUserId channelId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EnsureControl();
		if (_activePanelMap.TryGetValue(channelId, out BwoinkPanel value))
		{
			return value;
		}
		value = (_activePanelMap[channelId] = new BwoinkPanel(delegate(string text)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Action<NetUserId, string, bool, bool>? sendMessageAction = SendMessageAction;
			if (sendMessageAction != null)
			{
				NetUserId arg = channelId;
				BwoinkWindow? window = Window;
				bool arg2 = window == null || ((BaseButton)window.Bwoink.PlaySound).Pressed;
				BwoinkWindow? window2 = Window;
				sendMessageAction(arg, text, arg2, window2 != null && ((BaseButton)window2.Bwoink.AdminOnly).Pressed);
			}
		}));
		value.InputTextChanged += delegate(string text)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			this.InputTextChanged?.Invoke(channelId, text);
		};
		((Control)value).Visible = false;
		if (!((Control)Control.BwoinkArea).Children.Contains((Control)(object)value))
		{
			((Control)Control.BwoinkArea).AddChild((Control)(object)value);
		}
		return value;
	}

	public bool TryGetChannel(NetUserId ch, [NotNullWhen(true)] out BwoinkPanel? bp)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _activePanelMap.TryGetValue(ch, out bp);
	}

	private void SelectChannel(NetUserId uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		EnsurePanel(uid);
		Control.SelectChannel(uid);
	}

	public void Dispose()
	{
		BwoinkWindow? window = Window;
		if (window != null)
		{
			((Control)window).Orphan();
		}
		Window = null;
		Control = null;
		_activePanelMap.Clear();
		EverOpened = false;
	}
}
